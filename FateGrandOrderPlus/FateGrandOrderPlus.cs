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
    }
}
