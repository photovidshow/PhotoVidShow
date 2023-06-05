namespace CustomButton
{
    partial class CropUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mLockAspectCheckBox = new System.Windows.Forms.CheckBox();
            this.mCropEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.mUse4by3AspectButton = new System.Windows.Forms.Button();
            this.mUse16by9AspectButton = new System.Windows.Forms.Button();
            this.mUseImageAspectButton = new System.Windows.Forms.Button();
            this.mAspectGroupBox = new System.Windows.Forms.GroupBox();
            this.mUseSquareAspectButton = new System.Windows.Forms.Button();
            this.mCroppedRegionGroupBox = new System.Windows.Forms.GroupBox();
            this.mCroppedBottomTextBox = new System.Windows.Forms.TextBox();
            this.mCroppedTopTextBox = new System.Windows.Forms.TextBox();
            this.mCroppedRightTextBox = new System.Windows.Forms.TextBox();
            this.mCroppedLeftTextBox = new System.Windows.Forms.TextBox();
            this.mOriginalSizeTextBox = new System.Windows.Forms.TextBox();
            this.mCroppedSizeTextBox = new System.Windows.Forms.TextBox();
            this.mToolStrip = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.mAspectGroupBox.SuspendLayout();
            this.mCroppedRegionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(0, 255);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 18);
            this.label8.TabIndex = 11;
            this.label8.Text = "Cropped size";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(0, 230);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 18);
            this.label7.TabIndex = 10;
            this.label7.Text = "Original size";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 18);
            this.label6.TabIndex = 9;
            this.label6.Text = "Top";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(2, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 18);
            this.label5.TabIndex = 8;
            this.label5.Text = "Bottom";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 18);
            this.label4.TabIndex = 7;
            this.label4.Text = "Right";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(6, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Left";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mLockAspectCheckBox
            // 
            this.mLockAspectCheckBox.AutoSize = true;
            this.mLockAspectCheckBox.Location = new System.Drawing.Point(9, 3);
            this.mLockAspectCheckBox.Name = "mLockAspectCheckBox";
            this.mLockAspectCheckBox.Size = new System.Drawing.Size(15, 14);
            this.mLockAspectCheckBox.TabIndex = 12;
            this.mLockAspectCheckBox.TabStop = false;
            this.mToolStrip.SetToolTip(this.mLockAspectCheckBox, "If ticked, any further modifications to the cropped area will always \r\ncontain th" +
                    "e current aspect ratio\r\n");
            this.mLockAspectCheckBox.UseVisualStyleBackColor = true;
            this.mLockAspectCheckBox.CheckedChanged += new System.EventHandler(this.mLockAspectCheckBox_CheckedChanged);
            // 
            // mCropEnabledCheckBox
            // 
            this.mCropEnabledCheckBox.AutoSize = true;
            this.mCropEnabledCheckBox.Location = new System.Drawing.Point(3, 3);
            this.mCropEnabledCheckBox.Name = "mCropEnabledCheckBox";
            this.mCropEnabledCheckBox.Size = new System.Drawing.Size(96, 17);
            this.mCropEnabledCheckBox.TabIndex = 14;
            this.mCropEnabledCheckBox.TabStop = false;
            this.mCropEnabledCheckBox.Text = "Crop enabled";
            this.mToolStrip.SetToolTip(this.mCropEnabledCheckBox, "Enables or disables a cropped area");
            this.mCropEnabledCheckBox.UseVisualStyleBackColor = true;
            this.mCropEnabledCheckBox.CheckedChanged += new System.EventHandler(this.mCropEnabledCheckBox_CheckedChanged);
            // 
            // mUse4by3AspectButton
            // 
            this.mUse4by3AspectButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.mUse4by3AspectButton.Location = new System.Drawing.Point(198, 23);
            this.mUse4by3AspectButton.Name = "mUse4by3AspectButton";
            this.mUse4by3AspectButton.Size = new System.Drawing.Size(96, 29);
            this.mUse4by3AspectButton.TabIndex = 15;
            this.mUse4by3AspectButton.TabStop = false;
            this.mUse4by3AspectButton.Text = "Standard 4:3 ";
            this.mToolStrip.SetToolTip(this.mUse4by3AspectButton, "Adjusts the cropped area such that it matches the aspect\r\nratio of the old TV sta" +
                    "ndard of 4 x 3");
            this.mUse4by3AspectButton.UseVisualStyleBackColor = true;
            this.mUse4by3AspectButton.Click += new System.EventHandler(this.mUse4by3AspectButton_Click);
            // 
            // mUse16by9AspectButton
            // 
            this.mUse16by9AspectButton.Location = new System.Drawing.Point(300, 23);
            this.mUse16by9AspectButton.Name = "mUse16by9AspectButton";
            this.mUse16by9AspectButton.Size = new System.Drawing.Size(118, 29);
            this.mUse16by9AspectButton.TabIndex = 16;
            this.mUse16by9AspectButton.TabStop = false;
            this.mUse16by9AspectButton.Text = "Widescreen 16:9 ";
            this.mToolStrip.SetToolTip(this.mUse16by9AspectButton, "Adjusts the cropped area such that it matches the aspect\r\nratio of a widescreen T" +
                    "V of 16 x 9\r\n");
            this.mUse16by9AspectButton.UseVisualStyleBackColor = true;
            this.mUse16by9AspectButton.Click += new System.EventHandler(this.mUse16by9AspectButton_Click);
            // 
            // mUseImageAspectButton
            // 
            this.mUseImageAspectButton.Location = new System.Drawing.Point(3, 23);
            this.mUseImageAspectButton.Name = "mUseImageAspectButton";
            this.mUseImageAspectButton.Size = new System.Drawing.Size(100, 29);
            this.mUseImageAspectButton.TabIndex = 17;
            this.mUseImageAspectButton.TabStop = false;
            this.mUseImageAspectButton.Text = "Image aspect";
            this.mToolStrip.SetToolTip(this.mUseImageAspectButton, "Adjusts the cropped area such that it matches the original image\'s aspect ratio");
            this.mUseImageAspectButton.UseVisualStyleBackColor = true;
            this.mUseImageAspectButton.Click += new System.EventHandler(this.mUseImageAspectButton_Click);
            // 
            // mAspectGroupBox
            // 
            this.mAspectGroupBox.Controls.Add(this.mUseSquareAspectButton);
            this.mAspectGroupBox.Controls.Add(this.mUse4by3AspectButton);
            this.mAspectGroupBox.Controls.Add(this.mUseImageAspectButton);
            this.mAspectGroupBox.Controls.Add(this.mLockAspectCheckBox);
            this.mAspectGroupBox.Controls.Add(this.mUse16by9AspectButton);
            this.mAspectGroupBox.Location = new System.Drawing.Point(3, 32);
            this.mAspectGroupBox.Name = "mAspectGroupBox";
            this.mAspectGroupBox.Size = new System.Drawing.Size(425, 59);
            this.mAspectGroupBox.TabIndex = 18;
            this.mAspectGroupBox.TabStop = false;
            // 
            // mUseSquareAspectButton
            // 
            this.mUseSquareAspectButton.Location = new System.Drawing.Point(109, 23);
            this.mUseSquareAspectButton.Name = "mUseSquareAspectButton";
            this.mUseSquareAspectButton.Size = new System.Drawing.Size(83, 29);
            this.mUseSquareAspectButton.TabIndex = 18;
            this.mUseSquareAspectButton.TabStop = false;
            this.mUseSquareAspectButton.Text = "Square 1:1";
            this.mToolStrip.SetToolTip(this.mUseSquareAspectButton, "Adjusts the cropped area such that the width and height lengths match");
            this.mUseSquareAspectButton.UseVisualStyleBackColor = true;
            this.mUseSquareAspectButton.Click += new System.EventHandler(this.mUseSquareAspectButton_Click);
            // 
            // mCroppedRegionGroupBox
            // 
            this.mCroppedRegionGroupBox.Controls.Add(this.mCroppedBottomTextBox);
            this.mCroppedRegionGroupBox.Controls.Add(this.mCroppedTopTextBox);
            this.mCroppedRegionGroupBox.Controls.Add(this.mCroppedRightTextBox);
            this.mCroppedRegionGroupBox.Controls.Add(this.mCroppedLeftTextBox);
            this.mCroppedRegionGroupBox.Controls.Add(this.label3);
            this.mCroppedRegionGroupBox.Controls.Add(this.label4);
            this.mCroppedRegionGroupBox.Controls.Add(this.label6);
            this.mCroppedRegionGroupBox.Controls.Add(this.label5);
            this.mCroppedRegionGroupBox.Location = new System.Drawing.Point(3, 96);
            this.mCroppedRegionGroupBox.Name = "mCroppedRegionGroupBox";
            this.mCroppedRegionGroupBox.Size = new System.Drawing.Size(122, 125);
            this.mCroppedRegionGroupBox.TabIndex = 19;
            this.mCroppedRegionGroupBox.TabStop = false;
            this.mCroppedRegionGroupBox.Text = "Cropped region";
            // 
            // mCroppedBottomTextBox
            // 
            this.mCroppedBottomTextBox.Location = new System.Drawing.Point(58, 93);
            this.mCroppedBottomTextBox.Name = "mCroppedBottomTextBox";
            this.mCroppedBottomTextBox.ReadOnly = true;
            this.mCroppedBottomTextBox.Size = new System.Drawing.Size(57, 22);
            this.mCroppedBottomTextBox.TabIndex = 13;
            this.mCroppedBottomTextBox.TabStop = false;
            this.mCroppedBottomTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mCroppedTopTextBox
            // 
            this.mCroppedTopTextBox.Location = new System.Drawing.Point(58, 71);
            this.mCroppedTopTextBox.Name = "mCroppedTopTextBox";
            this.mCroppedTopTextBox.ReadOnly = true;
            this.mCroppedTopTextBox.Size = new System.Drawing.Size(57, 22);
            this.mCroppedTopTextBox.TabIndex = 12;
            this.mCroppedTopTextBox.TabStop = false;
            this.mCroppedTopTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mCroppedRightTextBox
            // 
            this.mCroppedRightTextBox.Location = new System.Drawing.Point(58, 49);
            this.mCroppedRightTextBox.Name = "mCroppedRightTextBox";
            this.mCroppedRightTextBox.ReadOnly = true;
            this.mCroppedRightTextBox.Size = new System.Drawing.Size(57, 22);
            this.mCroppedRightTextBox.TabIndex = 11;
            this.mCroppedRightTextBox.TabStop = false;
            this.mCroppedRightTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mCroppedLeftTextBox
            // 
            this.mCroppedLeftTextBox.Location = new System.Drawing.Point(58, 27);
            this.mCroppedLeftTextBox.Name = "mCroppedLeftTextBox";
            this.mCroppedLeftTextBox.ReadOnly = true;
            this.mCroppedLeftTextBox.Size = new System.Drawing.Size(57, 22);
            this.mCroppedLeftTextBox.TabIndex = 10;
            this.mCroppedLeftTextBox.TabStop = false;
            this.mCroppedLeftTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mOriginalSizeTextBox
            // 
            this.mOriginalSizeTextBox.Location = new System.Drawing.Point(94, 228);
            this.mOriginalSizeTextBox.Name = "mOriginalSizeTextBox";
            this.mOriginalSizeTextBox.ReadOnly = true;
            this.mOriginalSizeTextBox.Size = new System.Drawing.Size(86, 22);
            this.mOriginalSizeTextBox.TabIndex = 20;
            this.mOriginalSizeTextBox.TabStop = false;
            this.mOriginalSizeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mCroppedSizeTextBox
            // 
            this.mCroppedSizeTextBox.Location = new System.Drawing.Point(94, 254);
            this.mCroppedSizeTextBox.Name = "mCroppedSizeTextBox";
            this.mCroppedSizeTextBox.ReadOnly = true;
            this.mCroppedSizeTextBox.Size = new System.Drawing.Size(86, 22);
            this.mCroppedSizeTextBox.TabIndex = 21;
            this.mCroppedSizeTextBox.TabStop = false;
            this.mCroppedSizeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Lock aspect";
            // 
            // CropUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mCroppedSizeTextBox);
            this.Controls.Add(this.mOriginalSizeTextBox);
            this.Controls.Add(this.mCroppedRegionGroupBox);
            this.Controls.Add(this.mAspectGroupBox);
            this.Controls.Add(this.mCropEnabledCheckBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CropUserControl";
            this.Size = new System.Drawing.Size(431, 319);
            this.mAspectGroupBox.ResumeLayout(false);
            this.mAspectGroupBox.PerformLayout();
            this.mCroppedRegionGroupBox.ResumeLayout(false);
            this.mCroppedRegionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox mLockAspectCheckBox;
        private System.Windows.Forms.CheckBox mCropEnabledCheckBox;
        private System.Windows.Forms.Button mUse4by3AspectButton;
        private System.Windows.Forms.Button mUse16by9AspectButton;
        private System.Windows.Forms.Button mUseImageAspectButton;
        private System.Windows.Forms.GroupBox mAspectGroupBox;
        private System.Windows.Forms.GroupBox mCroppedRegionGroupBox;
        private System.Windows.Forms.TextBox mCroppedBottomTextBox;
        private System.Windows.Forms.TextBox mCroppedTopTextBox;
        private System.Windows.Forms.TextBox mCroppedRightTextBox;
        private System.Windows.Forms.TextBox mCroppedLeftTextBox;
        private System.Windows.Forms.TextBox mOriginalSizeTextBox;
        private System.Windows.Forms.TextBox mCroppedSizeTextBox;
        private System.Windows.Forms.Button mUseSquareAspectButton;
        private System.Windows.Forms.ToolTip mToolStrip;
        private System.Windows.Forms.Label label1;
    }
}
