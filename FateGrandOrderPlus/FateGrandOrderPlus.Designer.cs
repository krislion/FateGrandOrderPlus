namespace FateGrandOrderPlus
{
    partial class FateGrandOrderPlus
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerPlay = new System.Windows.Forms.Timer(this.components);
            this.timerCreate = new System.Windows.Forms.Timer(this.components);
            this.buttonGrabAndroid = new System.Windows.Forms.Button();
            this.buttonGrabAndroid2 = new System.Windows.Forms.Button();
            this.buttonGrabAndroid3 = new System.Windows.Forms.Button();
            this.labelCompareOutput = new System.Windows.Forms.Label();
            this.buttonSearchLHSInRHS = new System.Windows.Forms.Button();
            this.textBoxLHS = new System.Windows.Forms.TextBox();
            this.textBoxRHS = new System.Windows.Forms.TextBox();
            this.checkBoxFindNext = new System.Windows.Forms.CheckBox();
            this.checkBoxIntro = new System.Windows.Forms.CheckBox();
            this.checkBoxLoopBattles = new System.Windows.Forms.CheckBox();
            this.buttonOnOffAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonGrabAndroid
            // 
            this.buttonGrabAndroid.Location = new System.Drawing.Point(128, -4);
            this.buttonGrabAndroid.Name = "buttonGrabAndroid";
            this.buttonGrabAndroid.Size = new System.Drawing.Size(75, 23);
            this.buttonGrabAndroid.TabIndex = 54;
            this.buttonGrabAndroid.Text = "ANDROID GRAB";
            this.buttonGrabAndroid.UseVisualStyleBackColor = true;
            this.buttonGrabAndroid.Click += new System.EventHandler(this.buttonGrabAndroid_Click);
            // 
            // buttonGrabAndroid2
            // 
            this.buttonGrabAndroid2.Location = new System.Drawing.Point(128, 25);
            this.buttonGrabAndroid2.Name = "buttonGrabAndroid2";
            this.buttonGrabAndroid2.Size = new System.Drawing.Size(75, 23);
            this.buttonGrabAndroid2.TabIndex = 56;
            this.buttonGrabAndroid2.Text = "ANDROID2";
            this.buttonGrabAndroid2.UseVisualStyleBackColor = true;
            this.buttonGrabAndroid2.Click += new System.EventHandler(this.buttonGrabAndroid2_Click);
            // 
            // buttonGrabAndroid3
            // 
            this.buttonGrabAndroid3.Location = new System.Drawing.Point(128, 54);
            this.buttonGrabAndroid3.Name = "buttonGrabAndroid3";
            this.buttonGrabAndroid3.Size = new System.Drawing.Size(75, 23);
            this.buttonGrabAndroid3.TabIndex = 57;
            this.buttonGrabAndroid3.Text = "ANDROID3";
            this.buttonGrabAndroid3.UseVisualStyleBackColor = true;
            this.buttonGrabAndroid3.Click += new System.EventHandler(this.buttonGrabAndroid3_Click);
            // 
            // labelCompareOutput
            // 
            this.labelCompareOutput.AutoSize = true;
            this.labelCompareOutput.Location = new System.Drawing.Point(1, 265);
            this.labelCompareOutput.Name = "labelCompareOutput";
            this.labelCompareOutput.Size = new System.Drawing.Size(87, 13);
            this.labelCompareOutput.TabIndex = 61;
            this.labelCompareOutput.Text = "location of A in B";
            // 
            // buttonSearchLHSInRHS
            // 
            this.buttonSearchLHSInRHS.Location = new System.Drawing.Point(0, 239);
            this.buttonSearchLHSInRHS.Name = "buttonSearchLHSInRHS";
            this.buttonSearchLHSInRHS.Size = new System.Drawing.Size(109, 23);
            this.buttonSearchLHSInRHS.TabIndex = 62;
            this.buttonSearchLHSInRHS.Text = "SEARCH A IN B";
            this.buttonSearchLHSInRHS.UseVisualStyleBackColor = true;
            this.buttonSearchLHSInRHS.Click += new System.EventHandler(this.buttonSearchLHSInRHS_Click);
            // 
            // textBoxLHS
            // 
            this.textBoxLHS.Location = new System.Drawing.Point(0, 187);
            this.textBoxLHS.Name = "textBoxLHS";
            this.textBoxLHS.Size = new System.Drawing.Size(109, 20);
            this.textBoxLHS.TabIndex = 63;
            // 
            // textBoxRHS
            // 
            this.textBoxRHS.Location = new System.Drawing.Point(0, 213);
            this.textBoxRHS.Name = "textBoxRHS";
            this.textBoxRHS.Size = new System.Drawing.Size(109, 20);
            this.textBoxRHS.TabIndex = 64;
            // 
            // checkBoxFindNext
            // 
            this.checkBoxFindNext.AutoSize = true;
            this.checkBoxFindNext.Location = new System.Drawing.Point(4, 83);
            this.checkBoxFindNext.Name = "checkBoxFindNext";
            this.checkBoxFindNext.Size = new System.Drawing.Size(118, 17);
            this.checkBoxFindNext.TabIndex = 65;
            this.checkBoxFindNext.Text = "Use \"Next\" buttons";
            this.checkBoxFindNext.UseVisualStyleBackColor = true;
            // 
            // checkBoxIntro
            // 
            this.checkBoxIntro.AutoSize = true;
            this.checkBoxIntro.Checked = true;
            this.checkBoxIntro.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIntro.Location = new System.Drawing.Point(4, 105);
            this.checkBoxIntro.Name = "checkBoxIntro";
            this.checkBoxIntro.Size = new System.Drawing.Size(77, 17);
            this.checkBoxIntro.TabIndex = 66;
            this.checkBoxIntro.Text = "Battle Intro";
            this.checkBoxIntro.UseVisualStyleBackColor = true;
            // 
            // checkBoxLoopBattles
            // 
            this.checkBoxLoopBattles.AutoSize = true;
            this.checkBoxLoopBattles.Checked = true;
            this.checkBoxLoopBattles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLoopBattles.Location = new System.Drawing.Point(4, 129);
            this.checkBoxLoopBattles.Name = "checkBoxLoopBattles";
            this.checkBoxLoopBattles.Size = new System.Drawing.Size(138, 17);
            this.checkBoxLoopBattles.TabIndex = 67;
            this.checkBoxLoopBattles.Text = "Continue to next battle?";
            this.checkBoxLoopBattles.UseVisualStyleBackColor = true;
            // 
            // buttonOnOffAll
            // 
            this.buttonOnOffAll.Location = new System.Drawing.Point(0, -4);
            this.buttonOnOffAll.Name = "buttonOnOffAll";
            this.buttonOnOffAll.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonOnOffAll.Size = new System.Drawing.Size(122, 81);
            this.buttonOnOffAll.TabIndex = 68;
            this.buttonOnOffAll.Text = "TURN_ONOFF_ALL DEVICES";
            this.buttonOnOffAll.UseVisualStyleBackColor = true;
            this.buttonOnOffAll.Click += new System.EventHandler(this.buttonOnOffAll_Click);
            // 
            // FateGrandOrderPlus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(202, 283);
            this.Controls.Add(this.buttonOnOffAll);
            this.Controls.Add(this.checkBoxLoopBattles);
            this.Controls.Add(this.checkBoxIntro);
            this.Controls.Add(this.checkBoxFindNext);
            this.Controls.Add(this.textBoxRHS);
            this.Controls.Add(this.textBoxLHS);
            this.Controls.Add(this.buttonSearchLHSInRHS);
            this.Controls.Add(this.labelCompareOutput);
            this.Controls.Add(this.buttonGrabAndroid3);
            this.Controls.Add(this.buttonGrabAndroid2);
            this.Controls.Add(this.buttonGrabAndroid);
            this.Name = "FateGrandOrderPlus";
            this.Text = "FateGrandOrderPlus";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timerPlay;
        private System.Windows.Forms.Timer timerCreate;
        private System.Windows.Forms.Button buttonGrabAndroid;
        private System.Windows.Forms.Button buttonGrabAndroid2;
        private System.Windows.Forms.Button buttonGrabAndroid3;
        private System.Windows.Forms.Label labelCompareOutput;
        private System.Windows.Forms.Button buttonSearchLHSInRHS;
        private System.Windows.Forms.TextBox textBoxLHS;
        private System.Windows.Forms.TextBox textBoxRHS;
        private System.Windows.Forms.CheckBox checkBoxFindNext;
        private System.Windows.Forms.CheckBox checkBoxIntro;
        private System.Windows.Forms.CheckBox checkBoxLoopBattles;
        private System.Windows.Forms.Button buttonOnOffAll;
    }
}

