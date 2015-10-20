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
                case "IMPATIENTLYCLICK": await ClickImpatientlyUntil(cparam, checkBoxCanRun, g, b); return;
                case "FIGHTUNTILDONE": await FightUntilDone(checkBoxCanRun, g, b); return;
                case "WAIT": await ClearCanRun(int.Parse(cparam[1]), checkBoxCanRun); return;
                case "TYPE": await TypeItIn(cparam[1], checkBoxCanRun); return;
                //case "BATTLE": await FullBattle(checkBoxCanRun); return;
                case "SCREENSHOT": await RecordScreenshot(cparam, checkBoxCanRun, g, b); return;
                case "SMARTDRAG": await SmartDrag(cparam, checkBoxCanRun, g, b); return;
                case "SMARTCLICK": await SmartClick(cparam, checkBoxCanRun, g, b); return;
                case "SMARTCLICKONFIRSTFIND": await SmartClickOnFirstFind(cparam, checkBoxCanRun, g, b); return;
                case "SMARTTYPE": await SmartType(cparam, checkBoxCanRun, g, b); return;
                default: await ClearCanRun(500, checkBoxCanRun); return;
            }
        }

        private static async Task ClickImpatientlyUntil(string[] cparam, CheckBox checkBoxCanRun, Graphics g, Bitmap b)
        {
            try
            {
                // ASSUMES 1 POSTCONDITION ONLY!!! WARNING!!!
                // 0 is the command choice
                int x1 = int.Parse(cparam[1]);
                int y1 = int.Parse(cparam[2]);
                int maxnum = int.Parse(cparam[3]);
                string check_name = cparam[12];
                int check_x1 = int.Parse(cparam[13]);
                int check_y1 = int.Parse(cparam[14]);
                int check_x2 = int.Parse(cparam[15]);
                int check_y2 = int.Parse(cparam[16]);
                Bitmap check_bmp;
                Bitmap small_bmp = GetBmp(check_name);
                Rectangle check_rect = new Rectangle(check_x1, check_y1
                    , check_x2 - check_x1
                    , check_y2 - check_y1);
                Rectangle check_result;
                AsyncAction a = async () =>
                {
                    await
                        MouseOperations.MyClickOnce(x1, y1);
                };
                int iterations = 0;
                while (iterations < maxnum)
                {
                    await
                        SmartAction(cparam.SubArray(4, cparam.Length - 4), a, g, b, checkBoxCanRun, false, false);
                    // no need to capture again, just recently captured in the smartaction
                    check_bmp = ScanParameters.Copy(b, check_rect);
                    check_result = Scan.SearchBitmap(small_bmp, check_bmp, 0.05);
                    if (check_result.Height != 0)
                    {
                        await
                            ClearCanRun(50, checkBoxCanRun);
                        return;
                    }
                }
                // failed to find the new image... time to screenshot
                string[] tmpParam = { "", "ERROR_IMPATIENT_", "0", "22", "500", "310" };
                await
                RecordScreenshot(tmpParam
                    , null
                    , g
                    , b);
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }
        }

        private static async Task FightUntilDone(CheckBox checkBoxCanRun, Graphics g, Bitmap b)
        {
            string[] attack = "SMARTCLICK#425#256#1#0#0#1#40000#600#1500#600#ATTACK_ITEM_0.png#418#229#429#240#ATTACK_ITEM_0.png#418#229#429#240".Split('#');
            string[] c1 = "SMARTCLICK#45#214#0#0#1#0#2000#600#1500#1000#ATTACK1_POSTFIND_0.png#40#204#46#230".Split('#');
            string[] c2 = "SMARTCLICK#139#203#0#0#1#0#25000#600#1500#1000#ATTACK_POSTFIND_0.png#139#203#145#214".Split('#');
            string[] c3 = "SMARTCLICK#242#226#1#0#0#1#2000#600#1500#2000#ATTACK_POSTFIND_0.png#139#203#145#214#ATTACK_POSTFIND_0.png#139#203#145#214".Split('#');
            string[] dismiss = "SMARTCLICK#237#282#2#0#0#2#35000#600#2000#600#DISMISS_ITEM_0.png#228#265#239#271#DISMISS_ITEM_1.png#242#265#253#280#DISMISS_ITEM_0.png#228#265#239#271#DISMISS_ITEM_1.png#242#265#253#280".Split('#');

            int iterations = 0;
            bool done = false;
            Rectangle captureRectangle = new Rectangle(0, 0, 500, 350);
            Rectangle attackRectangle = new Rectangle(418, 229, 13, 12);
            Rectangle dismissRectangle = new Rectangle(228, 265, 13, 8);
            Rectangle dismiss2Rectangle = new Rectangle(242,265,12,16);
            while (iterations < 5000 && !done)
            {
                iterations++;
                int findstartcch = 0;
                Rectangle attack_result = new Rectangle(0, 0, 0, 0);
                Rectangle dismiss_result = new Rectangle(0, 0, 0, 0);
                Rectangle dismiss2_result = new Rectangle(0, 0, 0, 0);
                while (findstartcch < 30000)
                {
                    findstartcch++;
                    await Task.Delay(100);
                    g.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                    Bitmap attack_check = ScanParameters.Copy(b, attackRectangle);
                    attack_result = Scan.SearchBitmap(GetBmp("ATTACK_ITEM_0.png"), attack_check, 0.05);
                    if (attack_result.Height != 0)
                    {
                        break; // ATTACK!!!
                    }
                    Bitmap dismiss_check = ScanParameters.Copy(b, dismissRectangle);
                    dismiss_result = Scan.SearchBitmap(GetBmp("DISMISS_ITEM_0.png"), dismiss_check, 0.05);
                    Bitmap dismiss2_check = ScanParameters.Copy(b, dismiss2Rectangle);
                    dismiss2_result = Scan.SearchBitmap(GetBmp("DISMISS_ITEM_1.png"), dismiss2_check, 0.04);
                    if (dismiss_result.Height != 0 && dismiss2_result.Height !=0)
                    {
                        break; // DONE!
                    }
                }

                if (attack_result.Height != 0)
                {
                    await SmartClick(attack, checkBoxCanRun, g, b, false);
                    await SmartClick(c1, checkBoxCanRun, g, b, false);
                    await SmartClick(c2, checkBoxCanRun, g, b, false);
                    await SmartClick(c3, checkBoxCanRun, g, b, false);
                }
                else if (dismiss_result.Height != 0 && dismiss2_result.Height != 0)
                {
                    await Task.Delay(3000);
                    g.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                    Bitmap dismiss_check = ScanParameters.Copy(b, dismissRectangle);
                    dismiss_result = Scan.SearchBitmap(GetBmp("DISMISS_ITEM_0.png"), dismiss_check, 0.05);
                    Bitmap dismiss2_check = ScanParameters.Copy(b, dismiss2Rectangle);
                    dismiss2_result = Scan.SearchBitmap(GetBmp("DISMISS_ITEM_1.png"), dismiss2_check, 0.04);
                    
                    Bitmap attack_check = ScanParameters.Copy(b, attackRectangle);
                    attack_result = Scan.SearchBitmap(GetBmp("ATTACK_ITEM_0.png"), attack_check, 0.05);
                    if (attack_result.Height != 0)
                    {
                        continue; // ATTACK!!!
                    }
                    if (dismiss_result.Height != 0 && dismiss2_result.Height != 0)
                    {
                        await SmartClick(dismiss, checkBoxCanRun, g, b);
                        return;
                    }
                }
            }
        }

        private static Bitmap GetBmp(string name)
        {
            if (!bmpCache.ContainsKey(name))
            {
                String fileName = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    @"\" + name);
                try
                {
                    Bitmap x = (Bitmap)Image.FromFile(fileName, false);
                    bmpCache[name] = x;
                }
                catch (Exception e)
                {
                    // debugging
                    string s = e.ToString();
                }
            }
            return bmpCache[name];
        }

        private static async Task SmartType(string[] cparam, CheckBox checkBoxCanRun, Graphics g, Bitmap b)
        {
            // 0 is the command choice
            string textToType = cparam[1];

            AsyncAction a = async () => {
                await
                    TypeItIn(textToType, null);
            };
            await
                SmartAction(cparam.SubArray(2, cparam.Length - 2), a, g, b, checkBoxCanRun);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static async Task SmartAction(string[] smartparam, AsyncAction a, Graphics g, Bitmap b, CheckBox checkBoxCanRun, bool clearCheck=true, bool takeScreenshots=true)
        {
            try
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
                int outiterations = 0;
                while (!postconditions && outiterations < 5)
                {
                    outiterations++;
                    int pre_time_waited = 0;
                    int pre_time_stabilized = 0;
                    Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
                    int iterations = 0;
                    while (iterations < 5000) // break conditions inside the loop are time-based
                    {// this either times out or finds the image and sees it stabilize. no guarantee, though
                        iterations++;
                        await
                            Task.Delay(100);
                        //System.Threading.Thread.Sleep(100);
                        if (checkBoxCanRun.Checked) //someone else has decided to move on to the next command, abort!
                        {
                            return;
                        }
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
                            pre_time_stabilized = 0; //be more friendly about noticing...?
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
                        if (takeScreenshots)
                        {
                            string[] tmpParam = { "", "ERROR_FIND_", "0", "22", "480", "300" };
                            await
                            RecordScreenshot(tmpParam
                                , null
                                , g
                                , b);
                        }
                        await
                            a();
                    }
                    else
                    {
                        await
                            a();
                    }
                    //System.Threading.Thread.Sleep(1000);
                    int post_time_waited = 0;
                    int post_time_stabilized = 0;
                    iterations = 0;
                    await Task.Delay(100);
                    while (iterations < 50000)
                    { // wait until settled time is exceeded
                        iterations++;
                        await
                            Task.Delay(100);
                        //System.Threading.Thread.Sleep(100);
                        if (checkBoxCanRun.Checked) //someone else has decided to move on to the next command, abort!
                        {
                            return;
                        }
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
                            //post_time_waited = 0; //keep the accrual in case of trying to run this again!
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
                        if (takeScreenshots)
                        {
                            string[] tmpParam = { "", "ERROR_POST", "0", "26", "480", "300" };
                            await
                                RecordScreenshot(tmpParam
                                , null
                                , g
                                , b);
                            // loop and try the action again
                        }
                    }
                }
            }
            catch(Exception e)
            {
                    string s = e.ToString();
            }
            //await Task.Delay(300); // rest after completion
            if (clearCheck)
            {
                await
                    ClearCanRun(300, checkBoxCanRun);
            }
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
                needle = GetBmp(name);
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
            await SmartAction(cparam.SubArray(5, cparam.Length - 5), a, g, b, checkBoxCanRun);
        }

        private static async Task SmartClick(string[] cparam, CheckBox checkBoxCanRun, Graphics g, Bitmap b, bool clearCheck = true)
        {
            try
            {
                // 0 is the command choice
                int x1 = int.Parse(cparam[1]);
                int y1 = int.Parse(cparam[2]);

                AsyncAction a = async () =>
                {
                    await
                        MouseOperations.MyClickOnce(x1, y1);
                };
                await
                    SmartAction(cparam.SubArray(3, cparam.Length - 3), a, g, b, checkBoxCanRun, clearCheck);
            }
            catch (Exception e)
            {
                string s = e.ToString();
            }
        }
        
        private static async Task SmartClickOnFirstFind(string[] cparam, CheckBox checkBoxCanRun, Graphics g, Bitmap b)
        {   // 0 is the command choice
            int x = int.Parse(cparam[1]);
            int y = int.Parse(cparam[2]);
            AsyncAction a = async () =>
            {
                Bitmap needle = CommandParser.GetBmp(cparam[3 + 8]);
                //after 8 items set for smart numbers, 1st "find" name
                int x1 = int.Parse(cparam[3 + 1 + 8]);
                int y1 = int.Parse(cparam[3 + 2 + 8]);
                int x2 = int.Parse(cparam[3 + 3 + 8]);
                int y2 = int.Parse(cparam[3 + 4 + 8]);
                Bitmap haystack = ScanParameters.Copy(b, new Rectangle(x1, y1, x2 - x1, y2 - y1));
                Rectangle location = Scan.SearchBitmap(needle, haystack, 0.0);
                if (location.Height != 0)
                {
                    x = (int)(x1 + location.X + location.Width / 2.0);
                    y = (int)(y1 + location.Y + location.Height / 2.0);
                }
                await MouseOperations.MyClickOnce(x, y);
            };
            await SmartAction(cparam.SubArray(3, cparam.Length - 3), a, g, b, checkBoxCanRun);
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
            if (c != null)
            {
                await ClearCanRun(100, c);
            }
        }
        //FullTutorial as one decision? Bad idea
        
        private static async Task RecordScreenshot(string[] cparam, CheckBox c, Graphics captureGraphics, Bitmap captureBitmap)
        {
            try
            {
                String name = cparam[1];
                String postfix = DateTime.Now.ToString("_yyyy_MM_dd_H_mm_ss");
                name = name + postfix;
                String fileName = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    @"\SCRN\SCRN__" + name + ".png");
                Rectangle captureRectangle = new Rectangle(0, 0, 500, 350);
                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                if (cparam.Length > 2)
                {
                    int x1 = int.Parse(cparam[2]);
                    int y1 = int.Parse(cparam[3]);
                    int width = int.Parse(cparam[4]);
                    int height = int.Parse(cparam[5]);
                    Bitmap tmpBitmap = ScanParameters.Copy(captureBitmap, new Rectangle(x1, y1, width, height));
                    tmpBitmap.Save(fileName);
                }
                else
                {
                    Bitmap screentosave = ScanParameters.Copy(captureBitmap, captureRectangle);
                    screentosave.Save(fileName);
                }
                if (c != null)
                {
                    // might consider taking different tactic to allow reuse instead of ignoring a param.
                    await ClearCanRun(0, c);
                }
            }
            catch(Exception e)
            {
                string s = e.ToString();
            }
        }
    }
}
