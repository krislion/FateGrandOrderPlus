using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        //     CLICKSCAN#300#300#1#0#0#0#500#200#LOADGAME_TITLESCREEN_001_FIND_001.png#
        // CLICKSCAN takes x,y coordinates to click, first, then the number of items to find and the number of items to avoid
        // After these, there is a chance to share a filename (needle) to seek as well as the rectangular area of the screen to check
        // CLICKSCAN also takes Find(post) and Avoid(post) images to know when it has clicked successfully.
        // After each of these, 
        // CLICKSCAN#NUM_FIND#NUM_AVOID#NUM_FIND_POST#NUM_AVOID_POST#MS_WAIT_FIND#MS_WAIT_POST
        // #NAME1_FIND#X1#Y1#X2#Y2
        // #NAME2_FIND#X1#Y1#X2#Y2
        // ..
        // #NAME1_AVOID#X1#Y1#X2#Y2
        // ...
        // #NAME1_FINDPOST#X1#Y1#X2#Y2
        // ...
        // #NAME1_AVOIDPOST#X1#Y1#X2#Y2
        // ONCE items are found, and seen for at least 



        public FateGrandOrderPlus()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            timerPlay.Start();
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
            // cancels on right-alt
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
            
        }
    }
}
