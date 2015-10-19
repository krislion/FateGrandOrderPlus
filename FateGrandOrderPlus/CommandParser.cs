using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FateGrandOrderPlus
{
    static class CommandParser
    {
        delegate Task AsyncAction();

        // Only uses the ability to 
        public static async void ParseAndRunCommand(string[] cparam, CheckBox checkBoxCanRun, Graphics g, Bitmap b)
        {
            String c = cparam[0];
            
            switch(c)
            {
                case "WAIT": await ClearCanRun(int.Parse(cparam[1]), checkBoxCanRun); return;
                case "TYPE": await TypeItIn(cparam[1], checkBoxCanRun); return;
                //case "BATTLE": await FullBattle(checkBoxCanRun); return;
                case "SCREENSHOT": await RecordScreenshot(cparam, checkBoxCanRun, g, b); return;
                case "SMARTDRAG": await SmartDrag(cparam, checkBoxCanRun, g, b); return;
                case "SMARTCLICK": await SmartClick(cparam, checkBoxCanRun, g, b); return;
                case "SMARTCLICKONFIRSTFIND": await SmartClickOnFirstFind(cparam, checkBoxCanRun, g, b); return;
            }
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static async Task SmartAction(string[] smartparam, AsyncAction a, Graphics g, Bitmap b)
        {
            //CLICKSCAN#NUM_F #NUM_A #NUM_FP #NUM_AP #MS_WAIT_F #MS_FSTAB #MS_WAIT_P #MS_PSTAB
            //# NAME1_FIND#X1#Y1#X2#Y2
            int numFindPreconditions = int.Parse(smartparam[0]);
            int numAvoidPreconditions = int.Parse(smartparam[1]);
            int numFindPostconditions = int.Parse(smartparam[2]);
            int numAvoidPostconditions = int.Parse(smartparam[3]);
            int pre_time_allowed_to_find = int.Parse(smartparam[4]);
            int pre_time_required_to_stabilize = int.Parse(smartparam[5]);
            int post_time_allowed_to_find = int.Parse(smartparam[6]);
            int post_time_required_to_stabilize = int.Parse(smartparam[7]);
            List<string> r = new List<string>(smartparam.SubArray(8, smartparam.Length - 8));
            ScanParameters[] prefind = ExtractItems(r, numFindPreconditions);
            ScanParameters[] preavoid = ExtractItems(r, numAvoidPreconditions);
            ScanParameters[] postfind = ExtractItems(r, numFindPostconditions);
            ScanParameters[] postavoid = ExtractItems(r, numAvoidPostconditions);

            bool preconditions = false;
            bool postconditions = false;
            //if (numFindPreconditions == 0 && numAvoidPreconditions == 0)
            //{
            //    preconditions = true;
            //}
            int pre_time_waited = 0;
            int pre_time_stabilized = 0;
            Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
            int iterations = 0;
            while (iterations < 5000) // break conditions inside the loop are time-based
            {// this either times out or finds the image and sees it stabilize. no guarantee, though
                iterations++;
                await Task.Delay(100);
                g.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                foreach (ScanParameters s in prefind)
                {
                    s.SetHaystack(b);
                }
                foreach (ScanParameters s in preavoid)
                {
                    s.SetHaystack(b);
                }
                if (Scan.Seek(prefind, preavoid))
                {
                    pre_time_waited = 0;
                    pre_time_stabilized += 100;
                    if (pre_time_stabilized > pre_time_required_to_stabilize)
                    {
                        preconditions = true;
                        break;
                    }
                }
                else
                {
                    pre_time_waited += 100;
                    pre_time_stabilized = 0;
                    if (pre_time_waited > pre_time_allowed_to_find)
                    {
                        preconditions = false; //already set to false
                        break;
                    }
                }
            }

            // TAKE THE ACTION!!!
            if (!preconditions)
            {
                //ERROR! take a screenshot of what defeated the automation... then click anyway
                string[] tmpParam = { "", "ERROR_FIND_", "0", "26", "300", "480" };
                await RecordScreenshot(tmpParam
                    , null
                    , g
                    , b);
                await a();
            }
            else
            {
                await a();
            }
            
            int post_time_waited = 0;
            int post_time_stabilized = 0;
            iterations = 0;
            while (iterations < 50000)
            { // wait until settled time is exceeded
                iterations++;
                await Task.Delay(100);
                g.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                foreach (ScanParameters s in postfind)
                {
                    s.SetHaystack(b);
                }
                foreach (ScanParameters s in postavoid)
                {
                    s.SetHaystack(b);
                }
                if (Scan.Seek(postfind, postavoid))
                {
                    post_time_waited = 0;
                    post_time_stabilized += 100;
                    if (post_time_stabilized > post_time_required_to_stabilize)
                    {
                        postconditions = true;
                        break;
                    }
                }
                else
                {
                    post_time_waited += 100;
                    post_time_stabilized = 0;
                    if (post_time_waited > post_time_allowed_to_find)
                    {
                        postconditions = false; //already set to false
                        break;
                    }
                }
            }
            if (!postconditions)
            {
                string[] tmpParam = { "", "ERROR_POST", "0", "26", "300", "480" };
                await RecordScreenshot(tmpParam
                    , null
                    , g
                    , b);
            }
            await Task.Delay(300); // rest after completion
        }

        static Dictionary<string, Bitmap> bmpCache = new Dictionary<string, Bitmap>();

        private static ScanParameters[] ExtractItems(List<string> r, int num)
        {
            ScanParameters[] retval = new ScanParameters[num];
            for (int i=0; i<num; i++)
            {
                string name = r[0];
                int x1 = int.Parse(r[1]);
                int y1 = int.Parse(r[2]);
                int x2 = int.Parse(r[3]);
                int y2 = int.Parse(r[4]);
                r.RemoveRange(0, 5);
                Bitmap needle;
                if (!bmpCache.ContainsKey(name))
                {
                    String fileName = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                        r[0]);
                    bmpCache[name] = new Bitmap(fileName, false);
                }

                needle = bmpCache[name];
                retval[i] = new ScanParameters(needle, null, new Rectangle(x1, y1, x2 - x1, y2 - y1));
            }
            return retval;
        }

        private static async Task SmartDrag(string[] cparam, CheckBox checkBoxCanRun, Graphics g, Bitmap b)
        {
            // 0 is the command choice
            int x1 = int.Parse(cparam[1]);
            int y1 = int.Parse(cparam[2]);
            int x2 = int.Parse(cparam[3]);
            int y2 = int.Parse(cparam[4]);
            AsyncAction a = async () => {
                await MouseOperations.MyDrag(x1, y1, x2, y2);
            };
            await SmartAction(cparam.SubArray(5, cparam.Length - 5), a, g, b);
        }

        private static async Task SmartClick(string[] cparam, CheckBox checkBoxCanRun, Graphics g, Bitmap b)
        {
            // 0 is the command choice
            int x1 = int.Parse(cparam[1]);
            int y1 = int.Parse(cparam[2]);
            AsyncAction a = async () => {
                await MouseOperations.MyClickOnce(x1, y1);
            };
            await SmartAction(cparam.SubArray(3, cparam.Length - 3), a, g, b);
        }
        
        private static async Task SmartClickOnFirstFind(string[] cparam, CheckBox checkBoxCanRun, Graphics g, Bitmap b)
        {   // 0 is the command choice
            int x = int.Parse(cparam[1]);
            int y = int.Parse(cparam[2]);
            AsyncAction a = async () => {
                // if i can still find it, click it instead of x, y
                //Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
                Bitmap needle = bmpCache[cparam[3 + 8]]; //after 8 items set for smart numbers, 1st "find" name

                int x1 = int.Parse(cparam[3 + 1 + 8]);
                int y1 = int.Parse(cparam[3 + 2 + 8]);
                int x2 = int.Parse(cparam[3 + 3 + 8]);
                int y2 = int.Parse(cparam[4 + 4 + 8]);
                Bitmap haystack = ScanParameters.Copy(b, new Rectangle(x1, y1, x2 - x1, y2 - y1));
                Rectangle location = Scan.SearchBitmap(needle, haystack, 0.0);
                if (location.Height != 0)
                {
                    x = (int)(x1 + location.X + location.Width / 2.0);
                    y = (int)(x1 + location.Y + location.Height / 2.0);
                }
                await MouseOperations.MyClickOnce(x, y);
            };
            await SmartAction(cparam.SubArray(3, cparam.Length - 3), a, g, b);
        }

        private static async Task ClearCanRun(int duration, CheckBox checkBoxCanRun)
        {
            await Task.Delay(duration);
            checkBoxCanRun.Checked = true;
        }

        private static async Task TypeItIn(string toType, CheckBox c)
        {
            foreach (char ch in toType)
            {
                SendKeys.SendWait(ch.ToString());
                await Task.Delay(120);
            }
            await ClearCanRun(0, c);
        }
        //FullTutorial as one decision? Bad idea
        
        private static async Task RecordScreenshot(string[] cparam, CheckBox c, Graphics captureGraphics, Bitmap captureBitmap)
        {
            String name = cparam[1];
            String postfix = DateTime.Now.ToString("_yyyy_MM_dd_H_mm_ss");
            name = name + postfix;
            String fileName = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                @"\SCRN\SCRN__" + name + ".png");
            Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            if (cparam.Length > 2)
            {
                int x1 = int.Parse(cparam[2]);
                int y1 = int.Parse(cparam[3]);
                int width = int.Parse(cparam[4]);
                int height = int.Parse(cparam[5]);
                ScanParameters.Copy(captureBitmap, new Rectangle(x1, y1, width, height)).Save(fileName);
            }
            else
            {
                captureBitmap.Save(fileName);
            }
            if (c != null)
            {
                // might consider taking different tactic to allow reuse instead of ignoring a param.
                await ClearCanRun(0, c);
            }
        }
    }
}
