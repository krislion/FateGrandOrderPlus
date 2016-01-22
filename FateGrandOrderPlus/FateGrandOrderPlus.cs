using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace FateGrandOrderPlus
{
    public partial class FateGrandOrderPlus : Form
    {
        private Graphics captureGraphics;
        private Rectangle captureRectangle;
        private Bitmap captureBitmap;
        public FateGrandOrderPlus()
        {
            InitializeComponent();
        }
        
        public static async Task<int> RunProcessAsync(string fileName, string args)
        {
            using (var process = new Process
            {
                StartInfo =
        {
            FileName = fileName, Arguments = args,
            UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true
        },
                EnableRaisingEvents = true
            })
            {
                return await RunProcessAsync(process).ConfigureAwait(false);
            }
        }

        private static Task<int> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<int>();

            process.Exited += (s, ea) => tcs.SetResult(process.ExitCode);
            process.OutputDataReceived += (s, ea) => Console.WriteLine(ea.Data);
            process.ErrorDataReceived += (s, ea) => Console.WriteLine("ERR: " + ea.Data);

            bool started = process.Start();
            if (!started)
            {
                //you may allow for the process to be re-used (started = false) 
                //but I'm not sure about the guarantees of the Exited event in such a case
                throw new InvalidOperationException("Could not start process: " + process);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }

        private async Task<Bitmap> ADBImageGrab(string machineName)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.FileName = @"cmd.exe"; //"C:\Users\HDSwap\Downloads\autoscrn\test.cmd";
            processStartInfo.WorkingDirectory = @"C:\Users\HDSwap\Downloads\autoscrn";
            string filename = machineName + DateTime.Now.Ticks;
            processStartInfo.Arguments = @"/C C:\Users\HDSwap\Downloads\autoscrn\test.cmd " + filename +  " " + machineName;
            string filenameWithDirectory = @"C:\Users\HDSwap\Downloads\autoscrn\" + filename + ".png";
            //processStartInfo.Arguments = " -c \"adb shell screencap - p | sed 's/\r$//' > c:\\\\\\\\Users\\\\\\\\HDSwap\\\\\\\\Downloads\\\\\\\\autoscrn\\\\\\\\888_auto.png\"";
            //processStartInfo.Arguments = "adb shell screencap -p | sed 's/\r$//' > 888_auto.png";
            //processStartInfo.RedirectStandardOutput = true;
            //processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = true;
            //trying something from a website
            //processStartInfo.CreateNoWindow = true;
            //processStartInfo.UseShellExecute = false;
            process.StartInfo = processStartInfo;
            //await RunProcessAsync(process); /// try passing in the text necessary.
            process.Start();
            process.WaitForExit();
            //String error = process.StandardError.ReadToEnd();
            //String output = process.StandardOutput.ReadToEnd();
            //string a = error + output;
            //ViewBag.Error = error;
            //ViewBag.Ouput = output;
            await Task.Delay(227);
            Bitmap fileImg = (Bitmap) Bitmap.FromFile(filenameWithDirectory);
            //delete image?
            Bitmap retval = new Bitmap(fileImg);
            fileImg.Dispose();
            File.Delete(filenameWithDirectory);
            return retval;
        }

        private async Task ADBTap(int x, int y, string machineName)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.FileName = @"cmd.exe"; //"C:\Users\HDSwap\Downloads\autoscrn\test.cmd";
            processStartInfo.WorkingDirectory = @"C:\Users\HDSwap\Downloads\autoscrn";
            processStartInfo.Arguments = @"/C C:\Users\HDSwap\Downloads\autoscrn\tap.cmd " + x + " " + y + " " + machineName;
            processStartInfo.UseShellExecute = true;
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            //await RunProcessAsync(process);
            await Task.Delay(20);
        }

        private async Task ADBDel(string machineName)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.FileName = @"cmd.exe";
            processStartInfo.WorkingDirectory = @"C:\Users\HDSwap\Downloads\autoscrn";
            processStartInfo.Arguments = @"/C C:\Users\HDSwap\Downloads\autoscrn\delete7.cmd " + machineName;
            processStartInfo.UseShellExecute = true;
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            //await RunProcessAsync(process);
            await Task.Delay(20);
        }



        private async Task ADBType(string s, string machineName)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.FileName = @"cmd.exe"; //"C:\Users\HDSwap\Downloads\autoscrn\test.cmd";
            processStartInfo.WorkingDirectory = @"C:\Users\HDSwap\Downloads\autoscrn";
            processStartInfo.Arguments = @"/C C:\Users\HDSwap\Downloads\autoscrn\type.cmd " + s + " " + machineName;
            processStartInfo.UseShellExecute = true;
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            //await RunProcessAsync(process);
            await Task.Delay(20);
        }

        private async Task ADBKeyEvent(string s, string machineName)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.FileName = @"cmd.exe";
            processStartInfo.WorkingDirectory = @"C:\Users\HDSwap\Downloads\autoscrn";
            processStartInfo.Arguments = @"/C C:\Users\HDSwap\Downloads\autoscrn\keyevent.cmd " + s + " " + machineName;
            processStartInfo.UseShellExecute = true;
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            //await RunProcessAsync(process);
            await Task.Delay(20);
        }

        private async Task<Rectangle> WaitForADBBitmap(string name, string machineName, Rectangle location, int timeout=5)
        {
            for (int i = 0; i < timeout*5; i++)
            {
                ScanParameters[] find = new ScanParameters[1];
                ScanParameters[] avoid = new ScanParameters[0];
                Bitmap haystack = await ADBImageGrab(machineName);
                Bitmap needle = CommandParser.GetBmp(name);
                find[0] = new ScanParameters(needle, haystack, location);
                bool found = Scan.Seek(find, avoid);
                if (found)
                {
                    return Scan.SearchBitmap(needle, haystack, 0.03); //hopefully NO DUPLICATE LOCATIONS!!! assuming true
                }
                await Task.Delay(40);
            }
            throw new ArithmeticException("COULD NOT FIND " + name);
        }

        private async Task WaitForAndMovePastMainMenu(string machineName, bool findNext=false)
        {
            if (!findNext)
            {
                //await Task.Delay(1000);
                await WaitForADBBitmap("CHALDEA_MENU_1.png", machineName, new Rectangle(471, 33, 4, 50));
                await ADBTap(640, 130, machineName); //TOUCH 1st BATTLE OPTION
            }
            else
            {
                string result = "";
                int countdown = 20;
                while (result != "SUPPORT_MENU_1.png" && countdown > 0)
                {
                    countdown--;
                    Rectangle nextLocation = await WaitForADBBitmap("NEXT_TIP.png", machineName, new Rectangle(0, 0, 480, 854), 8);
                    int tapX = nextLocation.X - 38;
                    int tapY = nextLocation.Y;
                    int rotatedTapY = 480 - tapX; //assumed height!!!
                    if (rotatedTapY > 480)
                    {
                        rotatedTapY = 480; //max screen spot... not likely correct..., think more on this later
                    }
                    int rotatedTapX = tapY;
                    await ADBTap(rotatedTapX, rotatedTapY, machineName);
                    List<string> supportOrNext = new List<string>();
                    supportOrNext.Add("SUPPORT_MENU_1.png");
                    supportOrNext.Add("NEXT_TIP.png");
                    List<Rectangle> locationsToCheck = new List<Rectangle>();
                    locationsToCheck.Add(new Rectangle(388, 771, 10, 15));
                    locationsToCheck.Add(new Rectangle(0, 0, 480, 854));
                    result = await WaitForOneADBBitmapWhileTapping(supportOrNext, locationsToCheck, new Point(-1, -1), machineName, 30);
                }
                if (result != "SUPPORT_MENU_1.png")
                {
                    throw new ArithmeticException("NOPE! WEIRD LOOP WITH TOO MANY NEXT OPTIONS");
                }
            }
        }

        private async Task<int[]> CheckBattlePositions(string machineName)
        {
            int[] retval = { 0, 1, 2 };
            return retval;
        }

        private async Task<int[]> MakeBattleChoices(string machineName)
        {
            int[] retval = await CheckBattlePositions(machineName);
            return retval;
        }

        private async Task FightOneRound(string machineName)
        {
            await WaitForADBBitmap("BATTLE_ATTACK_1.png", machineName, new Rectangle(27, 735, 20, 50), 70);
            await ADBTap(760, 400, machineName); //TOUCH ATTACK BUTTON
            int[] choices = await MakeBattleChoices(machineName);//CHOOSE AND TOUCH 3 PARTY MEMBERS TO ATTACK
            await Task.Delay(550);
            foreach (int choice in choices)
            {
                switch (choice)
                {
                    case 0:
                        await ADBTap(85, 300, machineName);
                        break;
                    //ADBTapWaitVerify(x, y, );
                    case 1:
                        await ADBTap(256, 300, machineName);
                        break;
                    case 2:
                        await ADBTap(426, 300, machineName);
                        break;
                    case 3:
                        await ADBTap(598, 300, machineName);
                        break;
                    case 4:
                        await ADBTap(771, 300, machineName);
                        break;
                }
            }
            await ADBTap(771, 300, machineName);
            await ADBTap(598, 300, machineName);
            await ADBTap(426, 300, machineName);
        }

        private async Task<string> WaitForOneADBBitmapWhileTapping(List<string> imageNames, List<Rectangle> locations, Point tapLocation, string machineName, int timeout)
        {
            bool foundOne = false;
            int selection = 0;
            int iterations = 0;
            while (!foundOne && iterations < timeout * 5)
            {
                Bitmap haystack = await ADBImageGrab(machineName);
                int len = imageNames.Count;
                for (int i =0; i< len; i++)
                {
                    Bitmap needle = CommandParser.GetBmp(imageNames[i]);
                    ScanParameters[] find = new ScanParameters[1];
                    ScanParameters[] avoid = new ScanParameters[0];
                    find[0] = new ScanParameters(needle, haystack, locations[i]);
                    foundOne = Scan.Seek(find, avoid);
                    if (foundOne)
                    {
                        selection = i;
                        break;
                    }
                }
                if (tapLocation.X >= 0)
                {
                    await ADBTap(tapLocation.X, tapLocation.Y, machineName);
                }
                await Task.Delay(20); //200ms? lower due to the tap pause
                iterations++;
            }
            return imageNames[selection];
        }

        private async Task ClearSearch(string machineName)
        {
            await ADBTap(428, 30, machineName); //OPEN MENU
            //await Task.Delay(700);
            await WaitForADBBitmap("CLEAR_SEARCH_1.png", machineName, new Rectangle(400, 329, 12, 33), 8);
            await ADBTap(775, 220, machineName);//TAP ...
            //await Task.Delay(500);
            await WaitForADBBitmap("CLEAR_SEARCH_2.png", machineName, new Rectangle(135, 194, 14, 89), 9);
            await ADBTap(220, 340, machineName);//TAP CLEAR
            await WaitForADBBitmap("CLEAR_SEARCH_3.png", machineName, new Rectangle(305, 208, 15, 59), 9);
            await ADBTap(610, 310, machineName);//CONFIRM CLEAR
            await WaitForADBBitmap("CLEAR_SEARCH_1.png", machineName, new Rectangle(400, 329, 12, 33), 9);
            await ADBTap(800, 80, machineName); //EXIT MENU
        }

        private async Task SearchModify(string machineName)
        {
            await ADBTap(428, 30, machineName); //OPEN MENU
            await WaitForADBBitmap("CLEAR_SEARCH_1.png", machineName, new Rectangle(400, 329, 12, 33), 9);
            await ADBTap(130, 220, machineName);//SEARCH START
            //await WaitForADBBitmap("CLEAR_SEARCH_4.png", machineName, new Rectangle(135, 194, 14, 89), 4);
            await Task.Delay(1100);
            await ADBTap(610, 360, machineName);//PERFORM SEARCH
            await WaitForADBBitmap("CLEAR_SEARCH_1.png", machineName, new Rectangle(400, 329, 12, 33), 8);
            await ADBTap(775, 220, machineName);//TAP ...
            await WaitForADBBitmap("CLEAR_SEARCH_2.png", machineName, new Rectangle(135, 194, 14, 89), 9); //67 --- +22 for width difference
            await ADBTap(220, 140, machineName);//TAP EDIT ALL
            await WaitForADBBitmap("CLEAR_SEARCH_5.png", machineName, new Rectangle(310, 182, 25, 75), 9); //11, 45
            await ADBDel(machineName);
            await ADBType("994740", machineName);  //TYPE 99
            await Task.Delay(200);//TEXT APPEARS NEAR INSTANTLY. LETTING IT SETTLE (?)
            await ADBTap(615, 410, machineName);//CONFIRM CHANGE
            await WaitForADBBitmap("CLEAR_SEARCH_1.png", machineName, new Rectangle(400, 329, 12, 33), 9);
            await ADBTap(800, 80, machineName); //EXIT MENU
        }

        private Object BattleLock1 = new Object();
        private Object BattleLock2 = new Object();
        private Object BattleLock3 = new Object();
        private async Task RunRealBattle(Object battleLock, string machineName, bool intro=true, bool findNext=false)
        {

            lock (battleLock)
            {
                if (BattleRunning[machineName])
                {
                    return;
                }
                BattleRunning[machineName] = true;
            }
            while (checkBoxLoopBattles.Checked)
            {
                try
                {

                    //await ClearSearchModify(machineName);
                    //return;
                    List<string> attackOrFinished = new List<string>();
                    attackOrFinished.Add("BATTLE_ATTACK_1.png");
                    attackOrFinished.Add("BATTLE_FINISHED_NEXT_1.png");
                    //attackOrFinished.Add("BATTLE_FINISHED_GET_1.png");
                    List<Rectangle> locationsToCheck = new List<Rectangle>();
                    locationsToCheck.Add(new Rectangle(27, 735, 20, 50));
                    locationsToCheck.Add(new Rectangle(20, 702, 10, 40));
                    //locationsToCheck.Add(new Rectangle(116, 572, 30, 4));
                    Point tapLocation = new Point(356, 205);
                    if (intro)
                    {
                        await WaitForAndMovePastMainMenu(machineName, findNext);
                        await WaitForADBBitmap("SUPPORT_MENU_1.png", machineName, new Rectangle(388, 771, 10, 15), 22);
                        await ADBTap(400, 165, machineName); //TOUCH 1st SUPPORT OPTION
                        await WaitForADBBitmap("SUPPORT_MENU_2.png", machineName, new Rectangle(24, 815, 15, 22), 24);
                        await ADBTap(800, 455, machineName); //TOUCH START BUTTON
                        await ClearSearch(machineName);
                        await WaitForADBBitmap("BATTLE_ATTACK_1.png", machineName, new Rectangle(27, 735, 20, 50), 70);
                        await FightOneRound(machineName);
                        await SearchModify(machineName);
                    }
                    string result = await WaitForOneADBBitmapWhileTapping(attackOrFinished, locationsToCheck, tapLocation, machineName, 140);
                    bool battleFinished = false;
                    if (result == "BATTLE_FINISHED_NEXT_1.png")
                    {
                        await ADBTap(740, 455, machineName); //TOUCH POST-BATTLE "NEXT" BUTTON
                        battleFinished = true;
                    }
                    while (!battleFinished)
                    {
                        await FightOneRound(machineName);
                        //SEE IF THE BATTLE ENDED
                        result = await WaitForOneADBBitmapWhileTapping(attackOrFinished, locationsToCheck, tapLocation, machineName, 140);
                        if (result == "BATTLE_FINISHED_NEXT_1.png")
                        {
                            await ADBTap(740, 455, machineName); //TOUCH POST-BATTLE "NEXT" BUTTON
                            battleFinished = true;
                        }
                    }
                    tapLocation = new Point(135, 120);
                    bool postBattleDone = false;
                    
                    attackOrFinished.Add("CHALDEA_MENU_1.png");
                    locationsToCheck.Add(new Rectangle(471, 33, 4, 50));
                    while (!postBattleDone)
                    {
                        result = await WaitForOneADBBitmapWhileTapping(attackOrFinished, locationsToCheck, tapLocation, machineName, 140); //TAP IMPATIENTLY UNTIL 
                        if (result == "CHALDEA_MENU_1.png") // main menu appears
                        {
                            postBattleDone = true;
                        }
                    }
                }
                catch (ArithmeticException e)
                {
                    Console.WriteLine(e);
                    checkBoxLoopBattles.Checked = false;
                }
            }
            lock(battleLock)
            {
                BattleRunning[machineName] = false;
            }
            return;
        }
        

        Dictionary<string, bool> BattleRunning = new Dictionary<string, bool>() {
            { "LGLS6652c36d46f", false}, //For a while, this account has had 1-star berseker in charge at 4960
            { "LGLS6654ea13159", false}, //
            { "LGLS665cf91cebc", false}  //Account with Max Level Vlad and Carmilla
        };
        private void buttonGrabAndroid_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock1, "LGLS6652c36d46f", checkBoxIntro.Checked, checkBoxFindNext.Checked); //async!
        }

        private void buttonGrabAndroid2_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock2, "LGLS6654ea13159", checkBoxIntro.Checked, checkBoxFindNext.Checked); //2nd machine name
        }

        private void buttonGrabAndroid3_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock3, "LGLS665cf91cebc", checkBoxIntro.Checked, checkBoxFindNext.Checked); //3rd machine name
        }
        
        private void buttonSearchLHSInRHS_Click(object sender, EventArgs e)
        {
            Bitmap lhs = (Bitmap)Bitmap.FromFile(textBoxLHS.Lines[0]);
            Bitmap rhs = (Bitmap)Bitmap.FromFile(textBoxRHS.Lines[0]);
            Rectangle result = Scan.SearchBitmap(lhs, rhs, 0.03);
            if (result != null)
            {
                labelCompareOutput.Text = result.ToString();
            }
            else
            {
                labelCompareOutput.Text = "NO MATCH";
            }
            lhs.Dispose();
            rhs.Dispose();
        }
        
        private void buttonOnOffAll_Click(object sender, EventArgs e)
        {
            foreach (string machineName in BattleRunning.Keys)
            {
                ADBKeyEvent("26", machineName);
            }
        }

    }
}
