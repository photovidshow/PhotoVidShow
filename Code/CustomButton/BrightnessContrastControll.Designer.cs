namespace CustomButton
{
    partial class BrightnessContrastControll
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
            this.mColourAdjustCheckBox = new System.Windows.Forms.CheckBox();
            this.mContrastBLabel = new System.Windows.Forms.Label();
            this.mContrastGLabel = new System.Windows.Forms.Label();
            this.mContrastRLabel = new System.Windows.Forms.Label();
            this.mContrastBTrackBar = new System.Windows.Forms.TrackBar();
            this.mContrastGTrackBar = new System.Windows.Forms.TrackBar();
            this.mContrastRTrackBar = new System.Windows.Forms.TrackBar();
            this.mBrightnessBLabel = new System.Windows.Forms.Label();
            this.mBrightnessGLabel = new System.Windows.Forms.Label();
            this.mBrightnessRLabel = new System.Windows.Forms.Label();
            this.mBrightnessBTrackBar = new System.Windows.Forms.TrackBar();
            this.mBrightnessGTrackBar = new System.Windows.Forms.TrackBar();
            this.mBrightnessRTrackBar = new System.Windows.Forms.TrackBar();
            this.mBrightnessLabel = new System.Windows.Forms.Label();
            this.mContrastLabel = new System.Windows.Forms.Label();
            this.mTransparent = new System.Windows.Forms.Label();
            this.mContrastBnumericUpDown1 = new CustomButton.NumericUpDownOverride();
            this.mBrightnessBnumericUpDown1 = new CustomButton.NumericUpDownOverride();
            this.mContrastGnumericUpDown1 = new CustomButton.NumericUpDownOverride();
            this.mContrastRnumericUpDown1 = new CustomButton.NumericUpDownOverride();
            this.mBrightnessGnumericUpDown1 = new CustomButton.NumericUpDownOverride();
            this.mBrightnessRnumericUpDown1 = new CustomButton.NumericUpDownOverride();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastBTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastGTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastRTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessBTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessGTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessRTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastBnumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessBnumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastGnumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastRnumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessGnumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessRnumericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // mColourAdjustCheckBox
            // 
            this.mColourAdjustCheckBox.AutoSize = true;
            this.mColourAdjustCheckBox.Checked = true;
            this.mColourAdjustCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mColourAdjustCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mColourAdjustCheckBox.Location = new System.Drawing.Point(0, 0);
            this.mColourAdjustCheckBox.Name = "mColourAdjustCheckBox";
            this.mColourAdjustCheckBox.Size = new System.Drawing.Size(195, 17);
            this.mColourAdjustCheckBox.TabIndex = 25;
            this.mColourAdjustCheckBox.TabStop = false;
            this.mColourAdjustCheckBox.Text = "Adjust RGB channels individually";
            this.mColourAdjustCheckBox.UseVisualStyleBackColor = true;
            this.mColourAdjustCheckBox.CheckedChanged += new System.EventHandler(this.mColourAdjustCheckBox_CheckedChanged);
            // 
            // mContrastBLabel
            // 
            this.mContrastBLabel.AutoSize = true;
            this.mContrastBLabel.Location = new System.Drawing.Point(194, 84);
            this.mContrastBLabel.Name = "mContrastBLabel";
            this.mContrastBLabel.Size = new System.Drawing.Size(14, 13);
            this.mContrastBLabel.TabIndex = 15;
            this.mContrastBLabel.Text = "B";
            this.mContrastBLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mContrastGLabel
            // 
            this.mContrastGLabel.AutoSize = true;
            this.mContrastGLabel.Location = new System.Drawing.Point(194, 61);
            this.mContrastGLabel.Name = "mContrastGLabel";
            this.mContrastGLabel.Size = new System.Drawing.Size(15, 13);
            this.mContrastGLabel.TabIndex = 14;
            this.mContrastGLabel.Text = "G";
            this.mContrastGLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mContrastRLabel
            // 
            this.mContrastRLabel.AutoSize = true;
            this.mContrastRLabel.Location = new System.Drawing.Point(194, 40);
            this.mContrastRLabel.Name = "mContrastRLabel";
            this.mContrastRLabel.Size = new System.Drawing.Size(14, 13);
            this.mContrastRLabel.TabIndex = 13;
            this.mContrastRLabel.Text = "R";
            this.mContrastRLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mContrastBTrackBar
            // 
            this.mContrastBTrackBar.BackColor = System.Drawing.Color.White;
            this.mContrastBTrackBar.Location = new System.Drawing.Point(217, 84);
            this.mContrastBTrackBar.Maximum = 400;
            this.mContrastBTrackBar.Name = "mContrastBTrackBar";
            this.mContrastBTrackBar.Size = new System.Drawing.Size(100, 45);
            this.mContrastBTrackBar.TabIndex = 12;
            this.mContrastBTrackBar.TabStop = false;
            this.mContrastBTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mContrastBTrackBar.Value = 100;
            this.mContrastBTrackBar.Scroll += new System.EventHandler(this.mContrastBTrackBar_Scroll);
            this.mContrastBTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mContrastBTrackBar_MouseUp);
            // 
            // mContrastGTrackBar
            // 
            this.mContrastGTrackBar.BackColor = System.Drawing.Color.White;
            this.mContrastGTrackBar.Location = new System.Drawing.Point(217, 61);
            this.mContrastGTrackBar.Maximum = 400;
            this.mContrastGTrackBar.Name = "mContrastGTrackBar";
            this.mContrastGTrackBar.Size = new System.Drawing.Size(100, 45);
            this.mContrastGTrackBar.TabIndex = 11;
            this.mContrastGTrackBar.TabStop = false;
            this.mContrastGTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mContrastGTrackBar.Value = 100;
            this.mContrastGTrackBar.Scroll += new System.EventHandler(this.mContrastGTrackBar_Scroll);
            this.mContrastGTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mContrastGTrackBar_MouseUp);
            // 
            // mContrastRTrackBar
            // 
            this.mContrastRTrackBar.BackColor = System.Drawing.Color.White;
            this.mContrastRTrackBar.Location = new System.Drawing.Point(217, 38);
            this.mContrastRTrackBar.Maximum = 400;
            this.mContrastRTrackBar.Name = "mContrastRTrackBar";
            this.mContrastRTrackBar.Size = new System.Drawing.Size(100, 45);
            this.mContrastRTrackBar.TabIndex = 10;
            this.mContrastRTrackBar.TabStop = false;
            this.mContrastRTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mContrastRTrackBar.Value = 100;
            this.mContrastRTrackBar.Scroll += new System.EventHandler(this.mContrastRTrackBar_Scroll);
            this.mContrastRTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mContrastRTrackBar_MouseUp);
            // 
            // mBrightnessBLabel
            // 
            this.mBrightnessBLabel.AutoSize = true;
            this.mBrightnessBLabel.Location = new System.Drawing.Point(3, 84);
            this.mBrightnessBLabel.Name = "mBrightnessBLabel";
            this.mBrightnessBLabel.Size = new System.Drawing.Size(14, 13);
            this.mBrightnessBLabel.TabIndex = 15;
            this.mBrightnessBLabel.Text = "B";
            // 
            // mBrightnessGLabel
            // 
            this.mBrightnessGLabel.AutoSize = true;
            this.mBrightnessGLabel.Location = new System.Drawing.Point(2, 61);
            this.mBrightnessGLabel.Name = "mBrightnessGLabel";
            this.mBrightnessGLabel.Size = new System.Drawing.Size(15, 13);
            this.mBrightnessGLabel.TabIndex = 14;
            this.mBrightnessGLabel.Text = "G";
            // 
            // mBrightnessRLabel
            // 
            this.mBrightnessRLabel.AutoSize = true;
            this.mBrightnessRLabel.Location = new System.Drawing.Point(3, 40);
            this.mBrightnessRLabel.Name = "mBrightnessRLabel";
            this.mBrightnessRLabel.Size = new System.Drawing.Size(14, 13);
            this.mBrightnessRLabel.TabIndex = 13;
            this.mBrightnessRLabel.Text = "R";
            // 
            // mBrightnessBTrackBar
            // 
            this.mBrightnessBTrackBar.BackColor = System.Drawing.Color.White;
            this.mBrightnessBTrackBar.Location = new System.Drawing.Point(30, 84);
            this.mBrightnessBTrackBar.Maximum = 200;
            this.mBrightnessBTrackBar.Name = "mBrightnessBTrackBar";
            this.mBrightnessBTrackBar.Size = new System.Drawing.Size(100, 45);
            this.mBrightnessBTrackBar.TabIndex = 12;
            this.mBrightnessBTrackBar.TabStop = false;
            this.mBrightnessBTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mBrightnessBTrackBar.Value = 100;
            this.mBrightnessBTrackBar.Scroll += new System.EventHandler(this.mBrightnessB_Scroll);
            this.mBrightnessBTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mBrightnessBTrackBar_MouseUp);
            // 
            // mBrightnessGTrackBar
            // 
            this.mBrightnessGTrackBar.BackColor = System.Drawing.Color.White;
            this.mBrightnessGTrackBar.Location = new System.Drawing.Point(30, 61);
            this.mBrightnessGTrackBar.Maximum = 200;
            this.mBrightnessGTrackBar.Name = "mBrightnessGTrackBar";
            this.mBrightnessGTrackBar.Size = new System.Drawing.Size(100, 45);
            this.mBrightnessGTrackBar.TabIndex = 11;
            this.mBrightnessGTrackBar.TabStop = false;
            this.mBrightnessGTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mBrightnessGTrackBar.Value = 100;
            this.mBrightnessGTrackBar.Scroll += new System.EventHandler(this.mBrightnessG_Scroll);
            this.mBrightnessGTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mBrightnessGTrackBar_MouseUp);
            // 
            // mBrightnessRTrackBar
            // 
            this.mBrightnessRTrackBar.BackColor = System.Drawing.Color.White;
            this.mBrightnessRTrackBar.Location = new System.Drawing.Point(30, 38);
            this.mBrightnessRTrackBar.Maximum = 200;
            this.mBrightnessRTrackBar.Name = "mBrightnessRTrackBar";
            this.mBrightnessRTrackBar.Size = new System.Drawing.Size(100, 45);
            this.mBrightnessRTrackBar.TabIndex = 10;
            this.mBrightnessRTrackBar.TabStop = false;
            this.mBrightnessRTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mBrightnessRTrackBar.Value = 100;
            this.mBrightnessRTrackBar.Scroll += new System.EventHandler(this.mBrightnessR_Scroll);
            this.mBrightnessRTrackBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mBrightnessRTrackBar_MouseUp);
            // 
            // mBrightnessLabel
            // 
            this.mBrightnessLabel.AutoSize = true;
            this.mBrightnessLabel.Location = new System.Drawing.Point(54, 20);
            this.mBrightnessLabel.Name = "mBrightnessLabel";
            this.mBrightnessLabel.Size = new System.Drawing.Size(62, 13);
            this.mBrightnessLabel.TabIndex = 26;
            this.mBrightnessLabel.Text = "Brightness";
            // 
            // mContrastLabel
            // 
            this.mContrastLabel.AutoSize = true;
            this.mContrastLabel.Location = new System.Drawing.Point(242, 20);
            this.mContrastLabel.Name = "mContrastLabel";
            this.mContrastLabel.Size = new System.Drawing.Size(51, 13);
            this.mContrastLabel.TabIndex = 27;
            this.mContrastLabel.Text = "Contrast";
            // 
            // mTransparent
            // 
            this.mTransparent.Location = new System.Drawing.Point(30, 62);
            this.mTransparent.Name = "mTransparent";
            this.mTransparent.Size = new System.Drawing.Size(287, 50);
            this.mTransparent.TabIndex = 28;
            // 
            // mContrastBnumericUpDown1
            // 
            this.mContrastBnumericUpDown1.DecimalPlaces = 2;
            this.mContrastBnumericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.mContrastBnumericUpDown1.Location = new System.Drawing.Point(323, 77);
            this.mContrastBnumericUpDown1.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.mContrastBnumericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.mContrastBnumericUpDown1.Name = "mContrastBnumericUpDown1";
            this.mContrastBnumericUpDown1.ReadOnly = true;
            this.mContrastBnumericUpDown1.Size = new System.Drawing.Size(55, 22);
            this.mContrastBnumericUpDown1.TabIndex = 19;
            this.mContrastBnumericUpDown1.TabStop = false;
            this.mContrastBnumericUpDown1.UpButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mContrastBnumericUpDown1_UpButtonSelected);
            this.mContrastBnumericUpDown1.DownButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mContrastBnumericUpDown1_DownButtonSelected);
            // 
            // mBrightnessBnumericUpDown1
            // 
            this.mBrightnessBnumericUpDown1.DecimalPlaces = 2;
            this.mBrightnessBnumericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.mBrightnessBnumericUpDown1.Location = new System.Drawing.Point(136, 77);
            this.mBrightnessBnumericUpDown1.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mBrightnessBnumericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.mBrightnessBnumericUpDown1.Name = "mBrightnessBnumericUpDown1";
            this.mBrightnessBnumericUpDown1.ReadOnly = true;
            this.mBrightnessBnumericUpDown1.Size = new System.Drawing.Size(55, 22);
            this.mBrightnessBnumericUpDown1.TabIndex = 18;
            this.mBrightnessBnumericUpDown1.TabStop = false;
            this.mBrightnessBnumericUpDown1.UpButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mBrightnessBnumericUpDown1_UpButtonSelected);
            this.mBrightnessBnumericUpDown1.DownButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mBrightnessBnumericUpDown1_DownButtonSelected);
            // 
            // mContrastGnumericUpDown1
            // 
            this.mContrastGnumericUpDown1.DecimalPlaces = 2;
            this.mContrastGnumericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.mContrastGnumericUpDown1.Location = new System.Drawing.Point(323, 59);
            this.mContrastGnumericUpDown1.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.mContrastGnumericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.mContrastGnumericUpDown1.Name = "mContrastGnumericUpDown1";
            this.mContrastGnumericUpDown1.ReadOnly = true;
            this.mContrastGnumericUpDown1.Size = new System.Drawing.Size(55, 22);
            this.mContrastGnumericUpDown1.TabIndex = 18;
            this.mContrastGnumericUpDown1.TabStop = false;
            this.mContrastGnumericUpDown1.UpButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mContrastGnumericUpDown1_UpButtonSelected);
            this.mContrastGnumericUpDown1.DownButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mContrastGnumericUpDown1_DownButtonSelected);
            // 
            // mContrastRnumericUpDown1
            // 
            this.mContrastRnumericUpDown1.DecimalPlaces = 2;
            this.mContrastRnumericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.mContrastRnumericUpDown1.Location = new System.Drawing.Point(323, 38);
            this.mContrastRnumericUpDown1.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.mContrastRnumericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.mContrastRnumericUpDown1.Name = "mContrastRnumericUpDown1";
            this.mContrastRnumericUpDown1.ReadOnly = true;
            this.mContrastRnumericUpDown1.Size = new System.Drawing.Size(55, 22);
            this.mContrastRnumericUpDown1.TabIndex = 17;
            this.mContrastRnumericUpDown1.TabStop = false;
            this.mContrastRnumericUpDown1.UpButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mContrastRnumericUpDown1_UpButtonSelected);
            this.mContrastRnumericUpDown1.DownButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mContrastRnumericUpDown1_DownButtonSelected);
            // 
            // mBrightnessGnumericUpDown1
            // 
            this.mBrightnessGnumericUpDown1.DecimalPlaces = 2;
            this.mBrightnessGnumericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.mBrightnessGnumericUpDown1.Location = new System.Drawing.Point(136, 59);
            this.mBrightnessGnumericUpDown1.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mBrightnessGnumericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.mBrightnessGnumericUpDown1.Name = "mBrightnessGnumericUpDown1";
            this.mBrightnessGnumericUpDown1.ReadOnly = true;
            this.mBrightnessGnumericUpDown1.Size = new System.Drawing.Size(55, 22);
            this.mBrightnessGnumericUpDown1.TabIndex = 17;
            this.mBrightnessGnumericUpDown1.TabStop = false;
            this.mBrightnessGnumericUpDown1.UpButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mBrightnessGnumericUpDown1_UpButtonSelected);
            this.mBrightnessGnumericUpDown1.DownButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mBrightnessGnumericUpDown1_DownButtonSelected);
            // 
            // mBrightnessRnumericUpDown1
            // 
            this.mBrightnessRnumericUpDown1.DecimalPlaces = 2;
            this.mBrightnessRnumericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.mBrightnessRnumericUpDown1.Location = new System.Drawing.Point(136, 38);
            this.mBrightnessRnumericUpDown1.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mBrightnessRnumericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.mBrightnessRnumericUpDown1.Name = "mBrightnessRnumericUpDown1";
            this.mBrightnessRnumericUpDown1.ReadOnly = true;
            this.mBrightnessRnumericUpDown1.Size = new System.Drawing.Size(55, 22);
            this.mBrightnessRnumericUpDown1.TabIndex = 16;
            this.mBrightnessRnumericUpDown1.TabStop = false;
            this.mBrightnessRnumericUpDown1.UpButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mBrightnessRnumericUpDown1_UpButtonSelected);
            this.mBrightnessRnumericUpDown1.DownButtonSelected += new CustomButton.NumericUpDownButtonDelegate(this.mBrightnessRnumericUpDown1_DownButtonSelected);
            // 
            // BrightnessContrastControll
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.mBrightnessBTrackBar);
            this.Controls.Add(this.mBrightnessGTrackBar);
            this.Controls.Add(this.mContrastBnumericUpDown1);
            this.Controls.Add(this.mBrightnessBnumericUpDown1);
            this.Controls.Add(this.mContrastGnumericUpDown1);
            this.Controls.Add(this.mColourAdjustCheckBox);
            this.Controls.Add(this.mContrastRnumericUpDown1);
            this.Controls.Add(this.mBrightnessGnumericUpDown1);
            this.Controls.Add(this.mContrastBLabel);
            this.Controls.Add(this.mBrightnessRnumericUpDown1);
            this.Controls.Add(this.mContrastGLabel);
            this.Controls.Add(this.mContrastRLabel);
            this.Controls.Add(this.mBrightnessBLabel);
            this.Controls.Add(this.mContrastBTrackBar);
            this.Controls.Add(this.mContrastGTrackBar);
            this.Controls.Add(this.mBrightnessGLabel);
            this.Controls.Add(this.mTransparent);
            this.Controls.Add(this.mContrastRTrackBar);
            this.Controls.Add(this.mBrightnessRTrackBar);
            this.Controls.Add(this.mBrightnessRLabel);
            this.Controls.Add(this.mBrightnessLabel);
            this.Controls.Add(this.mContrastLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "BrightnessContrastControll";
            this.Size = new System.Drawing.Size(391, 108);
            ((System.ComponentModel.ISupportInitialize)(this.mContrastBTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastGTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastRTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessBTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessGTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessRTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastBnumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessBnumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastGnumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mContrastRnumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessGnumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mBrightnessRnumericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox mColourAdjustCheckBox;
        private NumericUpDownOverride mContrastBnumericUpDown1;
        private NumericUpDownOverride mContrastGnumericUpDown1;
        private NumericUpDownOverride mContrastRnumericUpDown1;
        private System.Windows.Forms.Label mContrastBLabel;
        private System.Windows.Forms.Label mContrastGLabel;
        private System.Windows.Forms.Label mContrastRLabel;
        private System.Windows.Forms.TrackBar mContrastBTrackBar;
        private System.Windows.Forms.TrackBar mContrastGTrackBar;
        private System.Windows.Forms.TrackBar mContrastRTrackBar;
        private NumericUpDownOverride mBrightnessBnumericUpDown1;
        private NumericUpDownOverride mBrightnessGnumericUpDown1;
        private NumericUpDownOverride mBrightnessRnumericUpDown1;
        private System.Windows.Forms.Label mBrightnessBLabel;
        private System.Windows.Forms.Label mBrightnessGLabel;
        private System.Windows.Forms.Label mBrightnessRLabel;
        private System.Windows.Forms.TrackBar mBrightnessBTrackBar;
        private System.Windows.Forms.TrackBar mBrightnessGTrackBar;
        private System.Windows.Forms.TrackBar mBrightnessRTrackBar;
        private System.Windows.Forms.Label mBrightnessLabel;
        private System.Windows.Forms.Label mContrastLabel;
        private System.Windows.Forms.Label mTransparent;
    }
}
