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
        public FateGrandOrderPlus()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {


            richTextScript.BackColor = Color.White;
            richTextScript.Clear();

            richTextScript.BulletIndent = 10;
            richTextScript.SelectionFont = new Font("Georgia", 16, FontStyle.Bold);
            richTextScript.SelectedText = "Mindcracker Network \n";
            richTextScript.SelectionFont = new Font("Verdana", 12);
            richTextScript.SelectionBullet = true;
            richTextScript.SelectionColor = Color.DarkBlue;
            richTextScript.SelectedText = "C# Corner" + "\n";
            richTextScript.SelectionFont = new Font("Verdana", 12);
            richTextScript.SelectionColor = Color.Orange;
            richTextScript.SelectedText = "VB.NET Heaven" + "\n";
            richTextScript.SelectionFont = new Font("Verdana", 12);
            richTextScript.SelectionColor = Color.Green;
            richTextScript.SelectedText = ".Longhorn Corner" + "\n";
            richTextScript.SelectionColor = Color.Red;
            richTextScript.SelectedText = ".NET Heaven" + "\n";
            richTextScript.SelectionBullet = false;
            richTextScript.SelectionFont = new Font("Tahoma", 10);
            richTextScript.SelectionColor = Color.Black;
            richTextScript.SelectedText = "This is a list of Mindcracker Network websites.\n";

        }
    }
}
