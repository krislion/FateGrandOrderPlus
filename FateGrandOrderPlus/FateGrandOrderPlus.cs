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
        // Holding down left-ctrl will set the Click position
        // Holding down left-alt will grab the screen down-right from the mouse for Find[cchFind]
        // Holding down left-shift will increment cchFind
        // Holding down right-alt will grab the screen down-right from the mouse for Avoid[cchAvoid]
        // Holding down right-shift will increment cchAvoid
        // Holding down left-ctrl + left-alt will not set Click, will grab screen for find_post
        // Holding down left-ctrl + left-shift will increment cchFindPost
        // Holding down right-ctrl + right-alt will grab screen for avoid_post
        // Holding down right-ctrl + right-shift will increment cchAvoidPost
        // Holding down spacebar will publish!
        //  1. grab all screenshot areas from the form (if set, up to 3 to find and 3 to avoid)
        //  2. write them out as "SCENARIO_ITEM_XXX_FIND_YYY" and "SCENARIO_ITEM_XXX_AVOID_YYY"
        //  3. clear the screenshots from memory/clear that they were set
        //  4. increment the XXX (item counter)
        //  5. calculate the bounding rectangle from both needles, adding 2 on each dimension
        //  6. print out the script necessary to generate the relevant ScanParams and Scan function call
        //  7. reset all data to empty aside from the SCENARIO, ITEM, and XXX
        //
        // Example:
        // Will save LOADGAME_TITLESCREEN_001_FIND_001.png (a picture of the game's title screen)
        // And print out the following line:
        //     CLICKSCAN#300#300#1#0#0#0#500#200#500#LOADGAME_TITLESCREEN_001_FIND_001.png#
        // CLICKSCAN takes x,y coordinates to click, first, then the number of items to find and the number of items to avoid
        // After these, there is a chance to share a filename (needle) to seek as well as the rectangular area of the screen to check
        // CLICKSCAN also takes Find(post) and Avoid(post) images to know when it has clicked successfully.
        // After each of these, 
        // CLICKSCAN#NUM_FIND#NUM_AVOID#NUM_FIND_POST#NUM_AVOID_POST#MS_WAIT_FIND#MS_FIND_STABILIZE#MS_WAIT_POST#MS_POST_STABILIZE
        // #NAME1_FIND#X1#Y1#X2#Y2
        // #NAME2_FIND#X1#Y1#X2#Y2
        // ..
        // #NAME1_AVOID#X1#Y1#X2#Y2
        // ...
        // #NAME1_FINDPOST#X1#Y1#X2#Y2
        // ...
        // #NAME1_AVOIDPOST#X1#Y1#X2#Y2
        // ONCE items are found, and seen for at least  MS_FIND_STABILIZE, then the click happens once.
        // This waits up to MS_WAIT_POST for the post conditions to become true.
        // If not true by then, then reset state and look at the action again anew. (failed to happen?)
        // If true before then, then begin to wait for everything to stabilize and be true every time for MS_POST_STABILIZE ms.
        // If it does not stabilize or does not ever find the conditions before MS_WAIT_FIND, then takes a screenshot and CLICKS AGAIN ANYWAY
        // then tries to continue. Always dismisses dialogs, which are tracked in another thread.
        // MIGHT restart the script, likely deleting progress/data.

        private Graphics captureGraphics;
        private Rectangle captureRectangle;
        private Bitmap captureBitmap;
        private TextBox[] findText;
        private TextBox[] avoidText;
        private TextBox[] postFindText;
        private TextBox[] postAvoidText;
        private PictureBox[] findPicture;
        private PictureBox[] avoidPicture;
        private PictureBox[] postFindPicture;
        private PictureBox[] postAvoidPicture;


        private void FateGrandOrderPlus_Load(object sender, EventArgs e)
        {
            this.captureBitmap = new Bitmap(1366, 768);
            this.captureRectangle = Screen.AllScreens[0].Bounds;
            this.captureGraphics = Graphics.FromImage(captureBitmap);
            this.findText = new TextBox[3];
            findText[0] = textFind1;
            findText[1] = textFind2;
            findText[2] = textFind3;
            this.avoidText = new TextBox[3];
            avoidText[0] = textAvoid1;
            avoidText[1] = textAvoid2;
            avoidText[2] = textAvoid3;
            this.postFindText = new TextBox[3];
            postFindText[0] = textPostFind1;
            postFindText[1] = textPostFind2;
            postFindText[2] = textPostFind3;
            this.postAvoidText = new TextBox[3];
            postAvoidText[0] = textPostAvoid1;
            postAvoidText[1] = textPostAvoid2;
            postAvoidText[2] = textPostAvoid3;

            this.findPicture = new PictureBox[3];
            findPicture[0] = pictureFind1;
            findPicture[1] = pictureFind2;
            findPicture[2] = pictureFind3;
            this.avoidPicture = new PictureBox[3];
            avoidPicture[0] = pictureAvoid1;
            avoidPicture[1] = pictureAvoid2;
            avoidPicture[2] = pictureAvoid3;
            this.postFindPicture = new PictureBox[3];
            postFindPicture[0] = pictureFindPost1;
            postFindPicture[1] = pictureFindPost2;
            postFindPicture[2] = pictureFindPost3;
            this.postAvoidPicture = new PictureBox[3];
            postAvoidPicture[0] = pictureAvoidPost1;
            postAvoidPicture[1] = pictureAvoidPost2;
            postAvoidPicture[2] = pictureAvoidPost3;
            
        }

        public FateGrandOrderPlus()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            timerPlay.Start();
            progressBar1.Value = 0;
            // Note: does not immediately execute the next command if already running (ala next line)
            //checkBoxCanRun.Checked = true;
            //richTextScript.BackColor = Color.White;
            //richTextScript.Clear();
            //richTextScript.SelectedText = "BIG TEXT\n";
            //richTextScript.SelectionFont = new Font("Verdana", 12);
            //richTextScript.SelectionBullet = true;
            //richTextScript.SelectionColor = Color.DarkBlue;
            //richTextScript.SelectedText = "BLUE" + "\n";
            //richTextScript.SelectionFont = new Font("Verdana", 12);
            //richTextScript.SelectionColor = Color.Orange;
            //richTextScript.SelectedText = "ORANGE" + "\n";
            //richTextScript.SelectionFont = new Font("Verdana", 12);
            //richTextScript.SelectionColor = Color.Green;
            //richTextScript.SelectedText = ".BULLETED" + "\n";
            //richTextScript.SelectionColor = Color.Red;
            //richTextScript.SelectedText = "XYZZY" + "\n";
            //richTextScript.SelectionBullet = false;
            //richTextScript.SelectionFont = new Font("Tahoma", 10);
            //richTextScript.SelectionColor = Color.Black;
            //richTextScript.SelectedText = "TESTTEXT";
        }

        private void timerPlay_Tick(object sender, EventArgs e)
        {
            // everything called by the play timer will act asynchronously
            // this keeps the ui from freezing during a long check/sleep/check/sleep/check/act cycle
            if (Keyboard.IsKeyDown(Key.RightAlt))
            {
                progressBar1.Value = 0;
                timerPlay.Stop();
                checkBoxCanRun.Checked = true;
            }
            else
            {
                if (richTextScript.Lines.Length > 0
                    && checkBoxCanRun.Checked)
                {
                    checkBoxCanRun.Checked = false;
                    String command = richTextScript.Lines[0];
                    if (command == "-----") // EXACT match for termination, otherwise this will loop
                    {
                        progressBar1.Value = progressBar1.Maximum;
                        timerPlay.Stop();
                    }
                    else if (command.Contains("-----"))
                    {
                        progressBar1.Value = 0;
                    }
                    if (progressBar1.Value == 0)
                    {
                        progressBar1.Maximum = richTextScript.Lines.Length;
                        int pos = 0;
                        foreach (String line in richTextScript.Lines)
                        {
                            if (pos != 0 && line.Contains("-----")) break;
                            pos++;
                        }
                        progressBar1.Value = progressBar1.Maximum - pos;
                    }
                    else if (progressBar1.Value < progressBar1.Maximum)
                    {
                        progressBar1.Value += 1;
                    }

                    String[] cparam = command.Split('#');
                    CommandParser.ParseAndRunCommand(cparam, this.checkBoxCanRun, this.captureGraphics, this.captureBitmap);

                    // Cycle command list
                    List<String> l = new List<String>(richTextScript.Lines);
                    String tmp = l[0];
                    l.RemoveAt(0);
                    l.Add(tmp);
                    richTextScript.Lines = l.ToArray();
                }
            }
        }

        private void checkBoxCreate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCreate.Checked)
            {
                timerCreate.Start();
            }
            else
            {
                timerCreate.Stop();
            }
        }

        private void timerCreate_Tick(object sender, EventArgs e)
        {
            // check a lot of state. grab state of the world and begin to process
            // creation steps are synchronous, since you never intend to do more than one of these at a time

            // Holding down left-ctrl + left-alt will not set Click, will grab screen for find_post
            // Holding down left-ctrl + left-shift will increment cchFindPost
            // Holding down right-ctrl + right-alt will grab screen for avoid_post
            // Holding down right-ctrl + right-shift will increment cchAvoidPost

            // Holding down spacebar will publish!

            if (Keyboard.IsKeyDown(Key.LeftCtrl)
                && Keyboard.IsKeyDown(Key.LeftShift))
            {
                IncrementUD(numericUDFindPost);
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl)
                && Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if (numericUDFindPost.Value == -1) numericUDFindPost.Value = 0;
                Grab(postFindPicture[(int)numericUDFindPost.Value]
                    , postFindText[(int)numericUDFindPost.Value]);
                
            }
            else if (Keyboard.IsKeyDown(Key.RightCtrl)
                && Keyboard.IsKeyDown(Key.RightShift))
            {
                IncrementUD(numericUDAvoidPost);
            }
            else if (Keyboard.IsKeyDown(Key.RightCtrl)
                && Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (numericUDAvoidPost.Value == -1) numericUDAvoidPost.Value = 0;
                Grab(postAvoidPicture[(int)numericUDAvoidPost.Value]
                    , postAvoidText[(int)numericUDAvoidPost.Value]);
            }
            else if (Keyboard.IsKeyDown(Key.Space))
            {
                //CLICKSCAN#X#Y#NUM_FIND#NUM_AVOID#NUM_FIND_POST#NUM_AVOID_POST
                // #MS_WAIT_FIND#MS_FIND_STABILIZE#MS_WAIT_POST#MS_POST_STABILIZE
                // #NAME1_FIND#X1#Y1#X2#Y2
                string command = "SMARTCLICK";
                command += "#" + textX.Text;
                command += "#" + textY.Text;
                command += "#" + (numericUDFind.Value + 1).ToString(); //number of each of these
                command += "#" + (numericUDAvoid.Value + 1).ToString();
                command += "#" + (numericUDFindPost.Value + 1).ToString();
                command += "#" + (numericUDAvoidPost.Value + 1).ToString();
                command += "#" + numericUDWait.Value.ToString(); // wait values
                command += "#" + numericUDstabilize.Value.ToString();
                command += "#" + numericUDPostWait.Value.ToString();
                command += "#" + numericUDPostStabilize.Value.ToString();
                command = AppendFinds(this.findText, this.findPicture, numericUDFind.Value, command);
                command = AppendFinds(this.avoidText, this.avoidPicture, numericUDAvoid.Value, command);
                command = AppendFinds(this.postFindText, this.postFindPicture, numericUDFindPost.Value, command);
                command = AppendFinds(this.postAvoidText, this.postAvoidPicture, this.numericUDAvoidPost.Value, command);
                richTextScript.Text += "\n" + command;
                ClearCreate();
            } //PUBLISH

            // Holding down left-alt will grab the screen down-right from the mouse for Find[cchFind]
            // Holding down left-shift will increment cchFind
            // Holding down right-alt will grab the screen down-right from the mouse for Avoid[cchAvoid]
            // Holding down right-shift will increment cchAvoid
            else if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                if (numericUDFind.Value == -1) numericUDFind.Value = 0;
                Grab(findPicture[(int)numericUDFind.Value]
                    , findText[(int)numericUDFind.Value]);
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                IncrementUD(numericUDFind);
            }
            else if (Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (numericUDAvoid.Value == -1) numericUDAvoid.Value = 0;
                Grab(avoidPicture[(int)numericUDAvoid.Value]
                    , avoidText[(int)numericUDAvoid.Value]);
            }
            else if (Keyboard.IsKeyDown(Key.RightShift))
            {
                IncrementUD(numericUDAvoid);
            }
            // Holding down left-ctrl will set the Click position
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                textX.Text = System.Windows.Forms.Cursor.Position.X.ToString();
                textY.Text = System.Windows.Forms.Cursor.Position.Y.ToString();
            }
        }

        private void IncrementUD(NumericUpDown numericUDAvoidPost)
        {
            if (numericUDAvoidPost.Value < numericUDAvoidPost.Maximum)
            {
                numericUDAvoidPost.Value += 1;
            }
        }

        private string AppendFinds(TextBox[] findText, PictureBox[] findPicture, decimal v, string command)
        {
            for (int i = 0; i < (v + 1); i++)
            {
                command += findText[i].Text;
                string name = findText[i].Text.Split('#')[1];
                String fileName = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                    @"\" + name);
                findPicture[i].Image.Save(fileName);
            }
            return command;
        }

        private void ClearCreate()
        {
            numericUDAvoid.Value = -1;
            numericUDAvoidPost.Value = -1;
            numericUDFind.Value = -1;
            numericUDFindPost.Value = -1;
            numericUDItem.Value = 0;
            pictureAvoid1.Image = null;
            pictureAvoid2.Image = null;
            pictureAvoid3.Image = null;
            pictureAvoidPost1.Image = null;
            pictureAvoidPost2.Image = null;
            pictureAvoidPost3.Image = null;
            pictureFind1.Image = null;
            pictureFind2.Image = null;
            pictureFind3.Image = null;
            pictureFindPost1.Image = null;
            pictureFindPost2.Image = null;
            pictureFindPost3.Image = null;
            textAvoid1.Text = "";
            textAvoid2.Text = "";
            textAvoid3.Text = "";
            textPostAvoid1.Text = "";
            textPostAvoid2.Text = "";
            textPostAvoid3.Text = "";
            textFind1.Text = "";
            textFind2.Text = "";
            textFind3.Text = "";
            textPostFind1.Text = "";
            textPostFind2.Text = "";
            textPostFind3.Text = "";
        }

        private void Grab(PictureBox pictureBox, TextBox textBox)
        {
            int x = System.Windows.Forms.Cursor.Position.X;
            int y = System.Windows.Forms.Cursor.Position.Y;
            //textX.Text = x.ToString();
            //textY.Text = y.ToString();
            int width = (int)numericUDWidth.Value;
            int height = (int)numericUDHeight.Value;
            Rectangle tmpCapture = new Rectangle(x, y, width, height);
            this.captureGraphics.CopyFromScreen(
                this.captureRectangle.Left
                , captureRectangle.Top, 0, 0
                , captureRectangle.Size);
            pictureBox.Image = ScanParameters.Copy(this.captureBitmap, tmpCapture);
            string name = textScenario.Text + "_" + textItem.Text + "_" + numericUDItem.Value.ToString();
            textBox.Text = "#" + name + ".png#"
                + (x).ToString() + "#" + (y).ToString() + "#"
                + (x + width + 1) + "#" + (y + height + 1).ToString();
            numericUDItem.Value += 1;
            return;
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

        private async Task WaitForADBBitmap(string name, string machineName, Rectangle location, int timeout=5)
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
                     return;
                }
                await Task.Delay(50);
            }
            throw new ArithmeticException("COULD NOT FIND " + name);
        }

        private async Task WaitForMainMenu(string machineName)
        {
            await WaitForADBBitmap("CHALDEA_MENU_1.png", machineName, new Rectangle(437, 764, 23, 27));
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
                await ADBTap(tapLocation.X, tapLocation.Y, machineName);
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
            await ADBType("999999", machineName);  //TYPE 99
            await Task.Delay(200);//TEXT APPEARS NEAR INSTANTLY. LETTING IT SETTLE (?)
            await ADBTap(615, 410, machineName);//CONFIRM CHANGE
            await WaitForADBBitmap("CLEAR_SEARCH_1.png", machineName, new Rectangle(400, 329, 12, 33), 9);
            await ADBTap(800, 80, machineName); //EXIT MENU
        }

        private Object BattleLock1 = new Object();
        private Object BattleLock2 = new Object();
        private Object BattleLock3 = new Object();
        private async Task RunRealBattle(Object battleLock, string machineName, bool intro=true)
        {

            lock (battleLock)
            {
                if (BattleRunning[machineName])
                {
                    return;
                }
                BattleRunning[machineName] = true;
            }
            while (checkBoxCanRun.Checked)
            {
                try
                {

                    //await ClearSearchModify(machineName);
                    //return;

                    List<string> attackOrFinished = new List<string>();
                    attackOrFinished.Add("BATTLE_ATTACK_1.png");
                    attackOrFinished.Add("BATTLE_FINISHED_NEXT_1.png");
                    List<Rectangle> locationsToCheck = new List<Rectangle>();
                    locationsToCheck.Add(new Rectangle(27, 735, 20, 50));
                    locationsToCheck.Add(new Rectangle(20, 702, 10, 40));
                    Point tapLocation = new Point(356, 205);
                    if (intro)
                    {
                        await WaitForMainMenu(machineName);
                        await ADBTap(640, 130, machineName); //TOUCH 1st BATTLE OPTION
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
                }
                catch (ArithmeticException e)
                {
                    Console.WriteLine(e);
                    checkBoxCanRun.Checked = false;
                }
            }
            lock(battleLock)
            {
                BattleRunning[machineName] = false;
            }
            return;
        }

        Dictionary<string, bool> BattleRunning = new Dictionary<string, bool>() {
            { "LGLS6652c36d46f", false},
            { "LGLS6654ea13159", false},
            { "LGLS665cf91cebc", false}
        };
        private void buttonGrabAndroid_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock1, "LGLS6652c36d46f"); //async!
        }

        private void buttonGrabAndroid2_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock2, "LGLS6654ea13159"); //2nd machine name
        }

        private void buttonGrabAndroid3_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock3, "LGLS665cf91cebc"); //3rd machine name
        }

        private void androidNoStart_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock1, "LGLS6652c36d46f", false); //async!
        }

        private void androidNoStart2_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock2, "LGLS6654ea13159", false); //2nd machine name
        }

        private void androidNoStart3_Click(object sender, EventArgs e)
        {
            RunRealBattle(BattleLock3, "LGLS665cf91cebc", false); //3rd machine name
        }
    }
}
