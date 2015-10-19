using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private void FateGrandOrderPlus_Load(object sender, EventArgs e)
        {
            this.captureBitmap = new Bitmap(1366, 768);
            this.captureRectangle = Screen.AllScreens[0].Bounds;
            this.captureGraphics = Graphics.FromImage(captureBitmap);
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
            //richTextScript.SelectedText = "Mindcracker Network \n";
            //richTextScript.SelectionFont = new Font("Verdana", 12);
            //richTextScript.SelectionBullet = true;
            //richTextScript.SelectionColor = Color.DarkBlue;
            //richTextScript.SelectedText = "C# Corner" + "\n";
            //richTextScript.SelectionFont = new Font("Verdana", 12);
            //richTextScript.SelectionColor = Color.Orange;
            //richTextScript.SelectedText = "VB.NET Heaven" + "\n";
            //richTextScript.SelectionFont = new Font("Verdana", 12);
            //richTextScript.SelectionColor = Color.Green;
            //richTextScript.SelectedText = ".Longhorn Corner" + "\n";
            //richTextScript.SelectionColor = Color.Red;
            //richTextScript.SelectedText = ".NET Heaven" + "\n";
            //richTextScript.SelectionBullet = false;
            //richTextScript.SelectionFont = new Font("Tahoma", 10);
            //richTextScript.SelectionColor = Color.Black;
            //richTextScript.SelectedText = "This is a list of Mindcracker Network websites.\n";
        }

        private void timerPlay_Tick(object sender, EventArgs e)
        {
            // everything called by the play timer will act asynchronously
            // this keeps the ui from freezing during a long check/sleep/check/sleep/check/act cycle
            if (Keyboard.IsKeyDown(Key.RightAlt))
            {
                progressBar1.Value = 0;
                checkBoxCanRun.Checked = true;
                timerPlay.Stop();
            }
            else
            {
                if (richTextScript.Lines.Length > 0
                    && checkBoxCanRun.Checked)
                {
                    String command = richTextScript.Lines[0];
                    if (command == "-----") // EXACT match for termination, otherwise this will loop
                    {
                        progressBar1.Value = progressBar1.Maximum;
                        timerPlay.Stop();
                    }
                    else
                    {
                        if (progressBar1.Value == 0)
                        {
                            progressBar1.Maximum = richTextScript.Lines.Length;
                            int pos = 0;
                            foreach (String line in richTextScript.Lines)
                            {
                                pos++;
                                if (line.Contains("-----")) break;
                            }
                            progressBar1.Value = progressBar1.Maximum - pos;
                        }
                        else if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value += 1;

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

            if (Keyboard.IsKeyDown(Key.LeftCtrl)
                && Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (numericUDFindPost.Value < numericUDFindPost.Maximum)
                {
                    numericUDFindPost.Value += 1;
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl)
                && Keyboard.IsKeyDown(Key.LeftAlt))
            {
            }
            else if (Keyboard.IsKeyDown(Key.RightCtrl)
                && Keyboard.IsKeyDown(Key.RightShift))
            {
                if (numericUDAvoidPost.Value < numericUDAvoidPost.Maximum)
                {
                    numericUDAvoidPost.Value += 1;
                }
            }
            else if (Keyboard.IsKeyDown(Key.RightCtrl)
                && Keyboard.IsKeyDown(Key.RightAlt))
            { }
            else if (Keyboard.IsKeyDown(Key.Space))
            {
                numericUDAvoid.Value = 0;
                numericUDAvoidPost.Value = 0;
                numericUDFind.Value = 0;
                numericUDFindPost.Value = 0;
            } //PUBLISH
            
        }
    }
}
