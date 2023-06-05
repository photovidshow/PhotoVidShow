namespace CustomButton
{
    partial class FontSelectorControl
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
            this.mGradientCheckBox = new System.Windows.Forms.CheckBox();
            this.mOutlineColourButton = new System.Windows.Forms.Button();
            this.mTextColourPicker = new System.Windows.Forms.Button();
            this.mItalicCheckBox = new System.Windows.Forms.CheckBox();
            this.mShadowTickbox = new System.Windows.Forms.CheckBox();
            this.mBoldCheckBox = new System.Windows.Forms.CheckBox();
            this.mApplyButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mShadowColourButton = new System.Windows.Forms.Button();
            this.mShadowLengthTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.mShadowLengthTextBox = new System.Windows.Forms.TextBox();
            this.mShadowAngleTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mShadowAngleTrackBar = new System.Windows.Forms.TrackBar();
            this.mShadowGroupBox = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mShadowAlphaTrackBar = new System.Windows.Forms.TrackBar();
            this.mShadowStrengthTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mOutlineGroupBox = new System.Windows.Forms.GroupBox();
            this.mTransparentLabel = new System.Windows.Forms.Label();
            this.mOutlineAlphaTrackBar = new System.Windows.Forms.TrackBar();
            this.mOutlineStrengthTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.mOutlineWidthTrackBar = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.mOutlineWidthText = new System.Windows.Forms.TextBox();
            this.mOutlineCheckBox = new System.Windows.Forms.CheckBox();
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.mGradientGroupBox = new System.Windows.Forms.GroupBox();
            this.mGradientColourButton = new System.Windows.Forms.Button();
            this.mStylesListView = new System.Windows.Forms.ListView();
            this.mTextExamplePictureBox = new System.Windows.Forms.PictureBox();
            this.mFontCombo = new SquidgySoft.UI.Controls.FontComboBox();
            this.mUnderlineCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mShadowLengthTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mShadowAngleTrackBar)).BeginInit();
            this.mShadowGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mShadowAlphaTrackBar)).BeginInit();
            this.mOutlineGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mOutlineAlphaTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mOutlineWidthTrackBar)).BeginInit();
            this.mGradientGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mTextExamplePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mGradientCheckBox
            // 
            this.mGradientCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mGradientCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mGradientCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mGradientCheckBox.Location = new System.Drawing.Point(12, 134);
            this.mGradientCheckBox.Name = "mGradientCheckBox";
            this.mGradientCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mGradientCheckBox.Size = new System.Drawing.Size(120, 21);
            this.mGradientCheckBox.TabIndex = 42;
            this.mGradientCheckBox.TabStop = false;
            this.mGradientCheckBox.Text = "Gradient body";
            this.mGradientCheckBox.UseVisualStyleBackColor = false;
            this.mGradientCheckBox.CheckedChanged += new System.EventHandler(this.mGradientCheckBox_CheckedChanged);
            // 
            // mOutlineColourButton
            // 
            this.mOutlineColourButton.BackColor = System.Drawing.SystemColors.HighlightText;
            this.mOutlineColourButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mOutlineColourButton.Location = new System.Drawing.Point(9, 27);
            this.mOutlineColourButton.Name = "mOutlineColourButton";
            this.mOutlineColourButton.Size = new System.Drawing.Size(24, 24);
            this.mOutlineColourButton.TabIndex = 39;
            this.mOutlineColourButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mOutlineColourButton, "Sets the outline color of the font");
            this.mOutlineColourButton.UseVisualStyleBackColor = false;
            this.mOutlineColourButton.Click += new System.EventHandler(this.mOutlineColourButton_Click);
            // 
            // mTextColourPicker
            // 
            this.mTextColourPicker.BackColor = System.Drawing.SystemColors.HighlightText;
            this.mTextColourPicker.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mTextColourPicker.Location = new System.Drawing.Point(182, 3);
            this.mTextColourPicker.Name = "mTextColourPicker";
            this.mTextColourPicker.Size = new System.Drawing.Size(24, 24);
            this.mTextColourPicker.TabIndex = 33;
            this.mTextColourPicker.TabStop = false;
            this.mToolTip.SetToolTip(this.mTextColourPicker, "Sets the body color of the font");
            this.mTextColourPicker.UseVisualStyleBackColor = false;
            this.mTextColourPicker.Click += new System.EventHandler(this.mTextColourPicker_Click);
            // 
            // mItalicCheckBox
            // 
            this.mItalicCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.mItalicCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mItalicCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mItalicCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mItalicCheckBox.Location = new System.Drawing.Point(230, 2);
            this.mItalicCheckBox.Name = "mItalicCheckBox";
            this.mItalicCheckBox.Size = new System.Drawing.Size(24, 24);
            this.mItalicCheckBox.TabIndex = 37;
            this.mItalicCheckBox.TabStop = false;
            this.mItalicCheckBox.Text = "I";
            this.mItalicCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mToolTip.SetToolTip(this.mItalicCheckBox, "Italic");
            this.mItalicCheckBox.UseVisualStyleBackColor = false;
            this.mItalicCheckBox.CheckedChanged += new System.EventHandler(this.mItalicCheckBox_CheckedChanged);
            // 
            // mShadowTickbox
            // 
            this.mShadowTickbox.BackColor = System.Drawing.Color.Transparent;
            this.mShadowTickbox.Checked = true;
            this.mShadowTickbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mShadowTickbox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mShadowTickbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mShadowTickbox.Location = new System.Drawing.Point(9, 203);
            this.mShadowTickbox.Name = "mShadowTickbox";
            this.mShadowTickbox.Size = new System.Drawing.Size(81, 24);
            this.mShadowTickbox.TabIndex = 34;
            this.mShadowTickbox.TabStop = false;
            this.mShadowTickbox.Text = "Shadow";
            this.mShadowTickbox.UseVisualStyleBackColor = false;
            this.mShadowTickbox.CheckedChanged += new System.EventHandler(this.mShadowTickbox_CheckedChanged);
            // 
            // mBoldCheckBox
            // 
            this.mBoldCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.mBoldCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mBoldCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mBoldCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mBoldCheckBox.Location = new System.Drawing.Point(206, 2);
            this.mBoldCheckBox.Name = "mBoldCheckBox";
            this.mBoldCheckBox.Size = new System.Drawing.Size(24, 24);
            this.mBoldCheckBox.TabIndex = 36;
            this.mBoldCheckBox.TabStop = false;
            this.mBoldCheckBox.Text = "B";
            this.mToolTip.SetToolTip(this.mBoldCheckBox, "Bold");
            this.mBoldCheckBox.UseVisualStyleBackColor = false;
            this.mBoldCheckBox.CheckedChanged += new System.EventHandler(this.mBoldCheckBox_CheckedChanged);
            // 
            // mApplyButton
            // 
            this.mApplyButton.Location = new System.Drawing.Point(602, 292);
            this.mApplyButton.Name = "mApplyButton";
            this.mApplyButton.Size = new System.Drawing.Size(75, 23);
            this.mApplyButton.TabIndex = 2;
            this.mApplyButton.Text = "Apply";
            this.mApplyButton.UseVisualStyleBackColor = true;
            this.mApplyButton.Click += new System.EventHandler(this.mApplyButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Location = new System.Drawing.Point(521, 292);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 1;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Bottom color";
            // 
            // mShadowColourButton
            // 
            this.mShadowColourButton.BackColor = System.Drawing.SystemColors.HighlightText;
            this.mShadowColourButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mShadowColourButton.Location = new System.Drawing.Point(9, 27);
            this.mShadowColourButton.Name = "mShadowColourButton";
            this.mShadowColourButton.Size = new System.Drawing.Size(24, 24);
            this.mShadowColourButton.TabIndex = 50;
            this.mShadowColourButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mShadowColourButton, "Sets the shadow color of the font");
            this.mShadowColourButton.UseVisualStyleBackColor = false;
            this.mShadowColourButton.Click += new System.EventHandler(this.mShadowColourButton_Click);
            // 
            // mShadowLengthTrackBar
            // 
            this.mShadowLengthTrackBar.Location = new System.Drawing.Point(95, 23);
            this.mShadowLengthTrackBar.Maximum = 100;
            this.mShadowLengthTrackBar.Name = "mShadowLengthTrackBar";
            this.mShadowLengthTrackBar.Size = new System.Drawing.Size(134, 45);
            this.mShadowLengthTrackBar.TabIndex = 51;
            this.mShadowLengthTrackBar.TabStop = false;
            this.mShadowLengthTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mToolTip.SetToolTip(this.mShadowLengthTrackBar, "Sets the length of the shadow");
            this.mShadowLengthTrackBar.Scroll += new System.EventHandler(this.mShadowLengthTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(44, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 19);
            this.label2.TabIndex = 52;
            this.label2.Text = "Length";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mShadowLengthTextBox
            // 
            this.mShadowLengthTextBox.Location = new System.Drawing.Point(235, 24);
            this.mShadowLengthTextBox.Name = "mShadowLengthTextBox";
            this.mShadowLengthTextBox.ReadOnly = true;
            this.mShadowLengthTextBox.Size = new System.Drawing.Size(30, 22);
            this.mShadowLengthTextBox.TabIndex = 53;
            this.mShadowLengthTextBox.TabStop = false;
            // 
            // mShadowAngleTextBox
            // 
            this.mShadowAngleTextBox.Location = new System.Drawing.Point(235, 50);
            this.mShadowAngleTextBox.Name = "mShadowAngleTextBox";
            this.mShadowAngleTextBox.ReadOnly = true;
            this.mShadowAngleTextBox.Size = new System.Drawing.Size(30, 22);
            this.mShadowAngleTextBox.TabIndex = 56;
            this.mShadowAngleTextBox.TabStop = false;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(44, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 19);
            this.label3.TabIndex = 55;
            this.label3.Text = "Angle";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mShadowAngleTrackBar
            // 
            this.mShadowAngleTrackBar.Location = new System.Drawing.Point(95, 49);
            this.mShadowAngleTrackBar.Maximum = 359;
            this.mShadowAngleTrackBar.Name = "mShadowAngleTrackBar";
            this.mShadowAngleTrackBar.Size = new System.Drawing.Size(134, 45);
            this.mShadowAngleTrackBar.TabIndex = 54;
            this.mShadowAngleTrackBar.TabStop = false;
            this.mShadowAngleTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mToolTip.SetToolTip(this.mShadowAngleTrackBar, "Sets the angle of the shadow");
            this.mShadowAngleTrackBar.Scroll += new System.EventHandler(this.mShadowAngleTrackBar_Scroll);
            // 
            // mShadowGroupBox
            // 
            this.mShadowGroupBox.Controls.Add(this.label8);
            this.mShadowGroupBox.Controls.Add(this.mShadowAlphaTrackBar);
            this.mShadowGroupBox.Controls.Add(this.mShadowStrengthTextBox);
            this.mShadowGroupBox.Controls.Add(this.label4);
            this.mShadowGroupBox.Controls.Add(this.mShadowAngleTrackBar);
            this.mShadowGroupBox.Controls.Add(this.mShadowAngleTextBox);
            this.mShadowGroupBox.Controls.Add(this.mShadowColourButton);
            this.mShadowGroupBox.Controls.Add(this.label3);
            this.mShadowGroupBox.Controls.Add(this.mShadowLengthTrackBar);
            this.mShadowGroupBox.Controls.Add(this.label2);
            this.mShadowGroupBox.Controls.Add(this.mShadowLengthTextBox);
            this.mShadowGroupBox.Location = new System.Drawing.Point(3, 203);
            this.mShadowGroupBox.Name = "mShadowGroupBox";
            this.mShadowGroupBox.Size = new System.Drawing.Size(271, 105);
            this.mShadowGroupBox.TabIndex = 57;
            this.mShadowGroupBox.TabStop = false;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(47, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(180, 10);
            this.label8.TabIndex = 64;
            // 
            // mShadowAlphaTrackBar
            // 
            this.mShadowAlphaTrackBar.Location = new System.Drawing.Point(95, 75);
            this.mShadowAlphaTrackBar.Maximum = 255;
            this.mShadowAlphaTrackBar.Name = "mShadowAlphaTrackBar";
            this.mShadowAlphaTrackBar.Size = new System.Drawing.Size(134, 45);
            this.mShadowAlphaTrackBar.TabIndex = 58;
            this.mShadowAlphaTrackBar.TabStop = false;
            this.mShadowAlphaTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mToolTip.SetToolTip(this.mShadowAlphaTrackBar, "Sets the strength or transparency of the shadow");
            this.mShadowAlphaTrackBar.Scroll += new System.EventHandler(this.mShadowAlphaTrackBar_Scroll);
            // 
            // mShadowStrengthTextBox
            // 
            this.mShadowStrengthTextBox.Location = new System.Drawing.Point(235, 76);
            this.mShadowStrengthTextBox.Name = "mShadowStrengthTextBox";
            this.mShadowStrengthTextBox.ReadOnly = true;
            this.mShadowStrengthTextBox.Size = new System.Drawing.Size(30, 22);
            this.mShadowStrengthTextBox.TabIndex = 59;
            this.mShadowStrengthTextBox.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(34, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 19);
            this.label4.TabIndex = 57;
            this.label4.Text = "Strength";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mOutlineGroupBox
            // 
            this.mOutlineGroupBox.Controls.Add(this.mTransparentLabel);
            this.mOutlineGroupBox.Controls.Add(this.mOutlineColourButton);
            this.mOutlineGroupBox.Controls.Add(this.mOutlineAlphaTrackBar);
            this.mOutlineGroupBox.Controls.Add(this.mOutlineStrengthTextBox);
            this.mOutlineGroupBox.Controls.Add(this.label6);
            this.mOutlineGroupBox.Controls.Add(this.mOutlineWidthTrackBar);
            this.mOutlineGroupBox.Controls.Add(this.label5);
            this.mOutlineGroupBox.Controls.Add(this.mOutlineWidthText);
            this.mOutlineGroupBox.Location = new System.Drawing.Point(3, 41);
            this.mOutlineGroupBox.Name = "mOutlineGroupBox";
            this.mOutlineGroupBox.Size = new System.Drawing.Size(271, 89);
            this.mOutlineGroupBox.TabIndex = 58;
            this.mOutlineGroupBox.TabStop = false;
            // 
            // mTransparentLabel
            // 
            this.mTransparentLabel.BackColor = System.Drawing.Color.Transparent;
            this.mTransparentLabel.Location = new System.Drawing.Point(74, 79);
            this.mTransparentLabel.Name = "mTransparentLabel";
            this.mTransparentLabel.Size = new System.Drawing.Size(180, 23);
            this.mTransparentLabel.TabIndex = 63;
            // 
            // mOutlineAlphaTrackBar
            // 
            this.mOutlineAlphaTrackBar.Location = new System.Drawing.Point(95, 49);
            this.mOutlineAlphaTrackBar.Maximum = 255;
            this.mOutlineAlphaTrackBar.Name = "mOutlineAlphaTrackBar";
            this.mOutlineAlphaTrackBar.Size = new System.Drawing.Size(134, 45);
            this.mOutlineAlphaTrackBar.TabIndex = 61;
            this.mOutlineAlphaTrackBar.TabStop = false;
            this.mOutlineAlphaTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mToolTip.SetToolTip(this.mOutlineAlphaTrackBar, "Sets the strength or gradient of the font outline");
            this.mOutlineAlphaTrackBar.Scroll += new System.EventHandler(this.mOutlineAlphaTrackBar_Scroll);
            // 
            // mOutlineStrengthTextBox
            // 
            this.mOutlineStrengthTextBox.Location = new System.Drawing.Point(235, 50);
            this.mOutlineStrengthTextBox.Name = "mOutlineStrengthTextBox";
            this.mOutlineStrengthTextBox.ReadOnly = true;
            this.mOutlineStrengthTextBox.Size = new System.Drawing.Size(30, 22);
            this.mOutlineStrengthTextBox.TabIndex = 62;
            this.mOutlineStrengthTextBox.TabStop = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(34, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 19);
            this.label6.TabIndex = 60;
            this.label6.Text = "Strength";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mOutlineWidthTrackBar
            // 
            this.mOutlineWidthTrackBar.Location = new System.Drawing.Point(95, 23);
            this.mOutlineWidthTrackBar.Maximum = 15;
            this.mOutlineWidthTrackBar.Minimum = 1;
            this.mOutlineWidthTrackBar.Name = "mOutlineWidthTrackBar";
            this.mOutlineWidthTrackBar.Size = new System.Drawing.Size(134, 45);
            this.mOutlineWidthTrackBar.TabIndex = 54;
            this.mOutlineWidthTrackBar.TabStop = false;
            this.mOutlineWidthTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mToolTip.SetToolTip(this.mOutlineWidthTrackBar, "Sets the width of the font outline");
            this.mOutlineWidthTrackBar.Value = 1;
            this.mOutlineWidthTrackBar.Scroll += new System.EventHandler(this.mOutlineWidthTrackBar_Scroll);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(44, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 19);
            this.label5.TabIndex = 55;
            this.label5.Text = "Width";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mOutlineWidthText
            // 
            this.mOutlineWidthText.Location = new System.Drawing.Point(235, 24);
            this.mOutlineWidthText.Name = "mOutlineWidthText";
            this.mOutlineWidthText.ReadOnly = true;
            this.mOutlineWidthText.Size = new System.Drawing.Size(30, 22);
            this.mOutlineWidthText.TabIndex = 56;
            this.mOutlineWidthText.TabStop = false;
            // 
            // mOutlineCheckBox
            // 
            this.mOutlineCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mOutlineCheckBox.Checked = true;
            this.mOutlineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mOutlineCheckBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mOutlineCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mOutlineCheckBox.ImageList = this.mImageList;
            this.mOutlineCheckBox.Location = new System.Drawing.Point(9, 37);
            this.mOutlineCheckBox.Name = "mOutlineCheckBox";
            this.mOutlineCheckBox.Size = new System.Drawing.Size(74, 24);
            this.mOutlineCheckBox.TabIndex = 40;
            this.mOutlineCheckBox.TabStop = false;
            this.mOutlineCheckBox.Text = "Outline";
            this.mOutlineCheckBox.UseCompatibleTextRendering = true;
            this.mOutlineCheckBox.UseVisualStyleBackColor = false;
            this.mOutlineCheckBox.CheckedChanged += new System.EventHandler(this.mOutlineCheckBox_CheckedChanged);
            // 
            // mImageList
            // 
            this.mImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.mImageList.ImageSize = new System.Drawing.Size(76, 54);
            this.mImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mGradientGroupBox
            // 
            this.mGradientGroupBox.Controls.Add(this.mGradientColourButton);
            this.mGradientGroupBox.Controls.Add(this.label1);
            this.mGradientGroupBox.Location = new System.Drawing.Point(3, 136);
            this.mGradientGroupBox.Name = "mGradientGroupBox";
            this.mGradientGroupBox.Size = new System.Drawing.Size(271, 67);
            this.mGradientGroupBox.TabIndex = 59;
            this.mGradientGroupBox.TabStop = false;
            // 
            // mGradientColourButton
            // 
            this.mGradientColourButton.BackColor = System.Drawing.SystemColors.HighlightText;
            this.mGradientColourButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.mGradientColourButton.Location = new System.Drawing.Point(9, 27);
            this.mGradientColourButton.Name = "mGradientColourButton";
            this.mGradientColourButton.Size = new System.Drawing.Size(24, 24);
            this.mGradientColourButton.TabIndex = 60;
            this.mGradientColourButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mGradientColourButton, "Sets the bottom body color of the font");
            this.mGradientColourButton.UseVisualStyleBackColor = false;
            this.mGradientColourButton.Click += new System.EventHandler(this.mGradientColourButton_Click);
            // 
            // mStylesListView
            // 
            this.mStylesListView.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mStylesListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.mStylesListView.LargeImageList = this.mImageList;
            this.mStylesListView.Location = new System.Drawing.Point(285, 179);
            this.mStylesListView.MultiSelect = false;
            this.mStylesListView.Name = "mStylesListView";
            this.mStylesListView.Size = new System.Drawing.Size(392, 107);
            this.mStylesListView.TabIndex = 60;
            this.mStylesListView.TabStop = false;
            this.mStylesListView.TileSize = new System.Drawing.Size(128, 128);
            this.mStylesListView.UseCompatibleStateImageBehavior = false;
            this.mStylesListView.SelectedIndexChanged += new System.EventHandler(this.mStylesListView_SelectedIndexChanged);
            // 
            // mTextExamplePictureBox
            // 
            this.mTextExamplePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTextExamplePictureBox.Location = new System.Drawing.Point(285, 36);
            this.mTextExamplePictureBox.Name = "mTextExamplePictureBox";
            this.mTextExamplePictureBox.Size = new System.Drawing.Size(392, 119);
            this.mTextExamplePictureBox.TabIndex = 61;
            this.mTextExamplePictureBox.TabStop = false;
            // 
            // mFontCombo
            // 
            this.mFontCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.mFontCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mFontCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mFontCombo.IntegralHeight = false;
            this.mFontCombo.ItemHeight = 22;
            this.mFontCombo.Location = new System.Drawing.Point(3, 3);
            this.mFontCombo.MaxDropDownItems = 10;
            this.mFontCombo.Name = "mFontCombo";
            this.mFontCombo.Size = new System.Drawing.Size(176, 28);
            this.mFontCombo.TabIndex = 38;
            this.mFontCombo.TabStop = false;
            this.mFontCombo.SelectedIndexChanged += new System.EventHandler(this.mFontCombo_SelectedIndexChanged);
            // 
            // mUnderlineCheckBox
            // 
            this.mUnderlineCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.mUnderlineCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mUnderlineCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mUnderlineCheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mUnderlineCheckBox.Location = new System.Drawing.Point(254, 2);
            this.mUnderlineCheckBox.Name = "mUnderlineCheckBox";
            this.mUnderlineCheckBox.Size = new System.Drawing.Size(24, 24);
            this.mUnderlineCheckBox.TabIndex = 62;
            this.mUnderlineCheckBox.TabStop = false;
            this.mUnderlineCheckBox.Text = "U";
            this.mUnderlineCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mToolTip.SetToolTip(this.mUnderlineCheckBox, "Underline");
            this.mUnderlineCheckBox.UseVisualStyleBackColor = false;
            this.mUnderlineCheckBox.CheckedChanged += new System.EventHandler(this.mUnderlineCheckBox_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(282, 161);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 13);
            this.label7.TabIndex = 63;
            this.label7.Text = "Recent and default fonts";
            // 
            // FontSelectorControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mUnderlineCheckBox);
            this.Controls.Add(this.mShadowTickbox);
            this.Controls.Add(this.mGradientCheckBox);
            this.Controls.Add(this.mOutlineCheckBox);
            this.Controls.Add(this.mTextExamplePictureBox);
            this.Controls.Add(this.mStylesListView);
            this.Controls.Add(this.mGradientGroupBox);
            this.Controls.Add(this.mOutlineGroupBox);
            this.Controls.Add(this.mShadowGroupBox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mApplyButton);
            this.Controls.Add(this.mFontCombo);
            this.Controls.Add(this.mTextColourPicker);
            this.Controls.Add(this.mItalicCheckBox);
            this.Controls.Add(this.mBoldCheckBox);
            this.Controls.Add(this.label7);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FontSelectorControl";
            this.Size = new System.Drawing.Size(684, 323);
            ((System.ComponentModel.ISupportInitialize)(this.mShadowLengthTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mShadowAngleTrackBar)).EndInit();
            this.mShadowGroupBox.ResumeLayout(false);
            this.mShadowGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mShadowAlphaTrackBar)).EndInit();
            this.mOutlineGroupBox.ResumeLayout(false);
            this.mOutlineGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mOutlineAlphaTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mOutlineWidthTrackBar)).EndInit();
            this.mGradientGroupBox.ResumeLayout(false);
            this.mGradientGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mTextExamplePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox mGradientCheckBox;
        private System.Windows.Forms.Button mOutlineColourButton;
        private SquidgySoft.UI.Controls.FontComboBox mFontCombo;
        private System.Windows.Forms.Button mTextColourPicker;
        private System.Windows.Forms.CheckBox mItalicCheckBox;
        private System.Windows.Forms.CheckBox mShadowTickbox;
        private System.Windows.Forms.CheckBox mBoldCheckBox;
        private System.Windows.Forms.Button mApplyButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mShadowColourButton;
        private System.Windows.Forms.TrackBar mShadowLengthTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mShadowLengthTextBox;
        private System.Windows.Forms.TextBox mShadowAngleTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar mShadowAngleTrackBar;
        private System.Windows.Forms.GroupBox mShadowGroupBox;
        private System.Windows.Forms.TrackBar mShadowAlphaTrackBar;
        private System.Windows.Forms.TextBox mShadowStrengthTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox mOutlineGroupBox;
        private System.Windows.Forms.CheckBox mOutlineCheckBox;
        private System.Windows.Forms.TrackBar mOutlineWidthTrackBar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox mOutlineWidthText;
        private System.Windows.Forms.TrackBar mOutlineAlphaTrackBar;
        private System.Windows.Forms.TextBox mOutlineStrengthTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox mGradientGroupBox;
        private System.Windows.Forms.Button mGradientColourButton;
        private System.Windows.Forms.ListView mStylesListView;
        private System.Windows.Forms.PictureBox mTextExamplePictureBox;
        private System.Windows.Forms.CheckBox mUnderlineCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ImageList mImageList;
        private System.Windows.Forms.Label mTransparentLabel;
        private System.Windows.Forms.ToolTip mToolTip;
        private System.Windows.Forms.Label label8;

    }
}
