namespace PhotoCruzGui
{
    partial class PhotoCruzMainWindow
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
            this.PreviewPanel = new System.Windows.Forms.Panel();
            this.PreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.mHiddenPanel = new System.Windows.Forms.Panel();
            this.SeekTimeLabel = new System.Windows.Forms.Label();
            this.SeekTrackBar = new System.Windows.Forms.TrackBar();
            this.StopButton = new System.Windows.Forms.Button();
            this.PCPlayButton = new System.Windows.Forms.Button();
            this.CreateButton = new System.Windows.Forms.Button();
            this.PhotoCruzCancelButton = new System.Windows.Forms.Button();
            this.mAuthorGroupBox = new System.Windows.Forms.GroupBox();
            this.mFpsCombo = new System.Windows.Forms.ComboBox();
            this.mFpsLabel = new System.Windows.Forms.Label();
            this.mQualityCombo = new System.Windows.Forms.ComboBox();
            this.mQualityLabel = new System.Windows.Forms.Label();
            this.mUseMotionBlurCheckBox = new System.Windows.Forms.CheckBox();
            this.SizeComboBox = new System.Windows.Forms.ComboBox();
            this.GenerateComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BuildDVDonly = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.WriteToDVDcheckBox = new System.Windows.Forms.CheckBox();
            this.BufferFull = new System.Windows.Forms.ProgressBar();
            this.WriterComboBox = new System.Windows.Forms.ComboBox();
            this.mTotalCompleteLabel2 = new System.Windows.Forms.Label();
            this.mTotalCompleteLabel = new System.Windows.Forms.Label();
            this.mETATextBox = new System.Windows.Forms.Label();
            this.mSizeLabel = new System.Windows.Forms.Label();
            this.ElapsedTimeTextBox = new System.Windows.Forms.Label();
            this.BufferLabel = new System.Windows.Forms.Label();
            this.ElapsedTime = new System.Windows.Forms.Label();
            this.EstimateTimeLeft = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.mWriterLabel = new System.Windows.Forms.Label();
            this.Go = new System.Windows.Forms.Button();
            this.EncodeCheckPointsTextBox = new System.Windows.Forms.TextBox();
            this.mPreviousWizard3 = new System.Windows.Forms.Button();
            this.DoneButton = new System.Windows.Forms.Button();
            this.mAuthorLabel = new System.Windows.Forms.Label();
            this.CurrentProcessTextBox = new System.Windows.Forms.Label();
            this.mChapelLabel = new System.Windows.Forms.Label();
            this.PreviewPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SeekTrackBar)).BeginInit();
            this.mAuthorGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // PreviewPanel
            // 
            this.PreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewPanel.Controls.Add(this.PreviewPictureBox);
            this.PreviewPanel.Location = new System.Drawing.Point(0, 0);
            this.PreviewPanel.Name = "PreviewPanel";
            this.PreviewPanel.Size = new System.Drawing.Size(494, 370);
            this.PreviewPanel.TabIndex = 0;
            // 
            // PreviewPictureBox
            // 
            this.PreviewPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewPictureBox.Location = new System.Drawing.Point(0, 0);
            this.PreviewPictureBox.Name = "PreviewPictureBox";
            this.PreviewPictureBox.Size = new System.Drawing.Size(494, 370);
            this.PreviewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PreviewPictureBox.TabIndex = 0;
            this.PreviewPictureBox.TabStop = false;
            this.PreviewPictureBox.Click += new System.EventHandler(this.PreviewPictureBox_Click);
            // 
            // mHiddenPanel
            // 
            this.mHiddenPanel.Location = new System.Drawing.Point(90, 642);
            this.mHiddenPanel.Name = "mHiddenPanel";
            this.mHiddenPanel.Size = new System.Drawing.Size(40, 35);
            this.mHiddenPanel.TabIndex = 1;
            // 
            // SeekTimeLabel
            // 
            this.SeekTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SeekTimeLabel.Location = new System.Drawing.Point(407, 715);
            this.SeekTimeLabel.Name = "SeekTimeLabel";
            this.SeekTimeLabel.Size = new System.Drawing.Size(81, 29);
            this.SeekTimeLabel.TabIndex = 4;
            this.SeekTimeLabel.Text = "00:00:00.00";
            this.SeekTimeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // SeekTrackBar
            // 
            this.SeekTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SeekTrackBar.Location = new System.Drawing.Point(79, 715);
            this.SeekTrackBar.Name = "SeekTrackBar";
            this.SeekTrackBar.Size = new System.Drawing.Size(337, 45);
            this.SeekTrackBar.TabIndex = 5;
            this.SeekTrackBar.TabStop = false;
            this.SeekTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // StopButton
            // 
            this.StopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StopButton.Location = new System.Drawing.Point(43, 715);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(40, 23);
            this.StopButton.TabIndex = 1;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            // 
            // PCPlayButton
            // 
            this.PCPlayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PCPlayButton.Location = new System.Drawing.Point(4, 715);
            this.PCPlayButton.Name = "PCPlayButton";
            this.PCPlayButton.Size = new System.Drawing.Size(40, 23);
            this.PCPlayButton.TabIndex = 0;
            this.PCPlayButton.Text = "Play";
            this.PCPlayButton.UseVisualStyleBackColor = true;
            // 
            // CreateButton
            // 
            this.CreateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateButton.Location = new System.Drawing.Point(416, 748);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(72, 23);
            this.CreateButton.TabIndex = 2;
            this.CreateButton.Text = "Create";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // PhotoCruzCancelButton
            // 
            this.PhotoCruzCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PhotoCruzCancelButton.Location = new System.Drawing.Point(349, 748);
            this.PhotoCruzCancelButton.Name = "PhotoCruzCancelButton";
            this.PhotoCruzCancelButton.Size = new System.Drawing.Size(63, 23);
            this.PhotoCruzCancelButton.TabIndex = 3;
            this.PhotoCruzCancelButton.Text = "Cancel";
            this.PhotoCruzCancelButton.UseVisualStyleBackColor = true;
            this.PhotoCruzCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // mAuthorGroupBox
            // 
            this.mAuthorGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mAuthorGroupBox.Controls.Add(this.mFpsCombo);
            this.mAuthorGroupBox.Controls.Add(this.mFpsLabel);
            this.mAuthorGroupBox.Controls.Add(this.mQualityCombo);
            this.mAuthorGroupBox.Controls.Add(this.mQualityLabel);
            this.mAuthorGroupBox.Controls.Add(this.mUseMotionBlurCheckBox);
            this.mAuthorGroupBox.Controls.Add(this.SizeComboBox);
            this.mAuthorGroupBox.Controls.Add(this.GenerateComboBox);
            this.mAuthorGroupBox.Controls.Add(this.label1);
            this.mAuthorGroupBox.Controls.Add(this.BuildDVDonly);
            this.mAuthorGroupBox.Controls.Add(this.label6);
            this.mAuthorGroupBox.Controls.Add(this.WriteToDVDcheckBox);
            this.mAuthorGroupBox.Controls.Add(this.BufferFull);
            this.mAuthorGroupBox.Controls.Add(this.WriterComboBox);
            this.mAuthorGroupBox.Controls.Add(this.mTotalCompleteLabel2);
            this.mAuthorGroupBox.Controls.Add(this.mTotalCompleteLabel);
            this.mAuthorGroupBox.Controls.Add(this.mETATextBox);
            this.mAuthorGroupBox.Controls.Add(this.mSizeLabel);
            this.mAuthorGroupBox.Controls.Add(this.ElapsedTimeTextBox);
            this.mAuthorGroupBox.Controls.Add(this.BufferLabel);
            this.mAuthorGroupBox.Controls.Add(this.ElapsedTime);
            this.mAuthorGroupBox.Controls.Add(this.EstimateTimeLeft);
            this.mAuthorGroupBox.Controls.Add(this.progressBar1);
            this.mAuthorGroupBox.Controls.Add(this.mWriterLabel);
            this.mAuthorGroupBox.Controls.Add(this.Go);
            this.mAuthorGroupBox.Controls.Add(this.EncodeCheckPointsTextBox);
            this.mAuthorGroupBox.Controls.Add(this.mPreviousWizard3);
            this.mAuthorGroupBox.Controls.Add(this.DoneButton);
            this.mAuthorGroupBox.Controls.Add(this.mAuthorLabel);
            this.mAuthorGroupBox.Controls.Add(this.CurrentProcessTextBox);
            this.mAuthorGroupBox.Location = new System.Drawing.Point(4, 408);
            this.mAuthorGroupBox.Name = "mAuthorGroupBox";
            this.mAuthorGroupBox.Size = new System.Drawing.Size(484, 237);
            this.mAuthorGroupBox.TabIndex = 27;
            this.mAuthorGroupBox.TabStop = false;
            // 
            // mFpsCombo
            // 
            this.mFpsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mFpsCombo.DropDownWidth = 80;
            this.mFpsCombo.FormattingEnabled = true;
            this.mFpsCombo.Items.AddRange(new object[] {
            "12",
            "15",
            "20",
            "24",
            "25  (default)",
            "30",
            "50",
            "60"});
            this.mFpsCombo.Location = new System.Drawing.Point(183, 98);
            this.mFpsCombo.Name = "mFpsCombo";
            this.mFpsCombo.Size = new System.Drawing.Size(42, 21);
            this.mFpsCombo.TabIndex = 59;
            this.mFpsCombo.Visible = false;
            // 
            // mFpsLabel
            // 
            this.mFpsLabel.AutoSize = true;
            this.mFpsLabel.Location = new System.Drawing.Point(154, 99);
            this.mFpsLabel.Name = "mFpsLabel";
            this.mFpsLabel.Size = new System.Drawing.Size(25, 13);
            this.mFpsLabel.TabIndex = 58;
            this.mFpsLabel.Text = "Fps";
            this.mFpsLabel.Visible = false;
            // 
            // mQualityCombo
            // 
            this.mQualityCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mQualityCombo.FormattingEnabled = true;
            this.mQualityCombo.Items.AddRange(new object[] {
            "Low",
            "Good",
            "High",
            "Maximum"});
            this.mQualityCombo.Location = new System.Drawing.Point(70, 98);
            this.mQualityCombo.Name = "mQualityCombo";
            this.mQualityCombo.Size = new System.Drawing.Size(76, 21);
            this.mQualityCombo.TabIndex = 57;
            this.mQualityCombo.Visible = false;
            // 
            // mQualityLabel
            // 
            this.mQualityLabel.AutoSize = true;
            this.mQualityLabel.Location = new System.Drawing.Point(8, 99);
            this.mQualityLabel.Name = "mQualityLabel";
            this.mQualityLabel.Size = new System.Drawing.Size(43, 13);
            this.mQualityLabel.TabIndex = 56;
            this.mQualityLabel.Text = "Quality";
            this.mQualityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mQualityLabel.Visible = false;
            // 
            // mUseMotionBlurCheckBox
            // 
            this.mUseMotionBlurCheckBox.AutoSize = true;
            this.mUseMotionBlurCheckBox.Checked = true;
            this.mUseMotionBlurCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mUseMotionBlurCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mUseMotionBlurCheckBox.Location = new System.Drawing.Point(303, 73);
            this.mUseMotionBlurCheckBox.Name = "mUseMotionBlurCheckBox";
            this.mUseMotionBlurCheckBox.Size = new System.Drawing.Size(180, 17);
            this.mUseMotionBlurCheckBox.TabIndex = 55;
            this.mUseMotionBlurCheckBox.Text = "Generate with motion blur on";
            this.mUseMotionBlurCheckBox.UseVisualStyleBackColor = true;
            // 
            // SizeComboBox
            // 
            this.SizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SizeComboBox.Location = new System.Drawing.Point(70, 70);
            this.SizeComboBox.Name = "SizeComboBox";
            this.SizeComboBox.Size = new System.Drawing.Size(218, 21);
            this.SizeComboBox.TabIndex = 29;
            this.SizeComboBox.Visible = false;
            // 
            // GenerateComboBox
            // 
            this.GenerateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GenerateComboBox.Location = new System.Drawing.Point(70, 42);
            this.GenerateComboBox.Name = "GenerateComboBox";
            this.GenerateComboBox.Size = new System.Drawing.Size(218, 21);
            this.GenerateComboBox.TabIndex = 54;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 23);
            this.label1.TabIndex = 53;
            this.label1.Text = "Generate";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BuildDVDonly
            // 
            this.BuildDVDonly.AutoSize = true;
            this.BuildDVDonly.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BuildDVDonly.Location = new System.Drawing.Point(401, 22);
            this.BuildDVDonly.Name = "BuildDVDonly";
            this.BuildDVDonly.Size = new System.Drawing.Size(118, 17);
            this.BuildDVDonly.TabIndex = 52;
            this.BuildDVDonly.Text = "Build DVD FS only";
            this.BuildDVDonly.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 146);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 23);
            this.label6.TabIndex = 20;
            this.label6.Text = "Current process:";
            // 
            // WriteToDVDcheckBox
            // 
            this.WriteToDVDcheckBox.AutoSize = true;
            this.WriteToDVDcheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WriteToDVDcheckBox.Location = new System.Drawing.Point(303, 46);
            this.WriteToDVDcheckBox.Name = "WriteToDVDcheckBox";
            this.WriteToDVDcheckBox.Size = new System.Drawing.Size(92, 17);
            this.WriteToDVDcheckBox.TabIndex = 51;
            this.WriteToDVDcheckBox.Text = "Write to disk";
            this.WriteToDVDcheckBox.UseVisualStyleBackColor = true;
            this.WriteToDVDcheckBox.CheckedChanged += new System.EventHandler(this.WriteToDVDcheckBox_CheckedChanged);
            // 
            // BufferFull
            // 
            this.BufferFull.Location = new System.Drawing.Point(360, 155);
            this.BufferFull.Name = "BufferFull";
            this.BufferFull.Size = new System.Drawing.Size(118, 16);
            this.BufferFull.TabIndex = 22;
            this.BufferFull.Click += new System.EventHandler(this.BufferFull_Click);
            // 
            // WriterComboBox
            // 
            this.WriterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WriterComboBox.Location = new System.Drawing.Point(70, 69);
            this.WriterComboBox.Name = "WriterComboBox";
            this.WriterComboBox.Size = new System.Drawing.Size(218, 21);
            this.WriterComboBox.TabIndex = 16;
            // 
            // mTotalCompleteLabel2
            // 
            this.mTotalCompleteLabel2.BackColor = System.Drawing.SystemColors.Control;
            this.mTotalCompleteLabel2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTotalCompleteLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mTotalCompleteLabel2.Location = new System.Drawing.Point(409, 127);
            this.mTotalCompleteLabel2.Name = "mTotalCompleteLabel2";
            this.mTotalCompleteLabel2.Size = new System.Drawing.Size(56, 16);
            this.mTotalCompleteLabel2.TabIndex = 28;
            // 
            // mTotalCompleteLabel
            // 
            this.mTotalCompleteLabel.BackColor = System.Drawing.Color.Transparent;
            this.mTotalCompleteLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTotalCompleteLabel.Location = new System.Drawing.Point(300, 126);
            this.mTotalCompleteLabel.Name = "mTotalCompleteLabel";
            this.mTotalCompleteLabel.Size = new System.Drawing.Size(120, 16);
            this.mTotalCompleteLabel.TabIndex = 27;
            this.mTotalCompleteLabel.Text = "Total Complete:";
            // 
            // mETATextBox
            // 
            this.mETATextBox.BackColor = System.Drawing.SystemColors.Control;
            this.mETATextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mETATextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mETATextBox.Location = new System.Drawing.Point(409, 100);
            this.mETATextBox.Name = "mETATextBox";
            this.mETATextBox.Size = new System.Drawing.Size(56, 16);
            this.mETATextBox.TabIndex = 26;
            // 
            // mSizeLabel
            // 
            this.mSizeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSizeLabel.Location = new System.Drawing.Point(8, 70);
            this.mSizeLabel.Name = "mSizeLabel";
            this.mSizeLabel.Size = new System.Drawing.Size(68, 23);
            this.mSizeLabel.TabIndex = 30;
            this.mSizeLabel.Text = "Resolution";
            this.mSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mSizeLabel.Visible = false;
            // 
            // ElapsedTimeTextBox
            // 
            this.ElapsedTimeTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.ElapsedTimeTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ElapsedTimeTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ElapsedTimeTextBox.Location = new System.Drawing.Point(102, 126);
            this.ElapsedTimeTextBox.Name = "ElapsedTimeTextBox";
            this.ElapsedTimeTextBox.Size = new System.Drawing.Size(95, 23);
            this.ElapsedTimeTextBox.TabIndex = 25;
            // 
            // BufferLabel
            // 
            this.BufferLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BufferLabel.Location = new System.Drawing.Point(310, 155);
            this.BufferLabel.Name = "BufferLabel";
            this.BufferLabel.Size = new System.Drawing.Size(40, 16);
            this.BufferLabel.TabIndex = 23;
            this.BufferLabel.Text = "Buffer";
            this.BufferLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // ElapsedTime
            // 
            this.ElapsedTime.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ElapsedTime.Location = new System.Drawing.Point(8, 126);
            this.ElapsedTime.Name = "ElapsedTime";
            this.ElapsedTime.Size = new System.Drawing.Size(88, 23);
            this.ElapsedTime.TabIndex = 10;
            this.ElapsedTime.Text = "Elapsed tIme:";
            // 
            // EstimateTimeLeft
            // 
            this.EstimateTimeLeft.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EstimateTimeLeft.Location = new System.Drawing.Point(300, 100);
            this.EstimateTimeLeft.Name = "EstimateTimeLeft";
            this.EstimateTimeLeft.Size = new System.Drawing.Size(112, 23);
            this.EstimateTimeLeft.TabIndex = 11;
            this.EstimateTimeLeft.Text = "Estimated time left:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(5, 177);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(473, 23);
            this.progressBar1.TabIndex = 9;
            // 
            // mWriterLabel
            // 
            this.mWriterLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mWriterLabel.Location = new System.Drawing.Point(8, 69);
            this.mWriterLabel.Name = "mWriterLabel";
            this.mWriterLabel.Size = new System.Drawing.Size(40, 23);
            this.mWriterLabel.TabIndex = 17;
            this.mWriterLabel.Text = "Writer";
            this.mWriterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Go
            // 
            this.Go.BackColor = System.Drawing.SystemColors.Control;
            this.Go.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Go.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Go.Location = new System.Drawing.Point(390, 206);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(88, 23);
            this.Go.TabIndex = 6;
            this.Go.Text = "Create video";
            this.Go.UseVisualStyleBackColor = false;
            this.Go.Click += new System.EventHandler(this.Go_Click_1);
            // 
            // EncodeCheckPointsTextBox
            // 
            this.EncodeCheckPointsTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.EncodeCheckPointsTextBox.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EncodeCheckPointsTextBox.Location = new System.Drawing.Point(12, 209);
            this.EncodeCheckPointsTextBox.Name = "EncodeCheckPointsTextBox";
            this.EncodeCheckPointsTextBox.ReadOnly = true;
            this.EncodeCheckPointsTextBox.Size = new System.Drawing.Size(188, 20);
            this.EncodeCheckPointsTextBox.TabIndex = 50;
            this.EncodeCheckPointsTextBox.Visible = false;
            // 
            // mPreviousWizard3
            // 
            this.mPreviousWizard3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mPreviousWizard3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPreviousWizard3.Location = new System.Drawing.Point(313, 206);
            this.mPreviousWizard3.Name = "mPreviousWizard3";
            this.mPreviousWizard3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mPreviousWizard3.Size = new System.Drawing.Size(75, 23);
            this.mPreviousWizard3.TabIndex = 47;
            this.mPreviousWizard3.Text = "< Back";
            this.mPreviousWizard3.Click += new System.EventHandler(this.mPreviousWizard3_Click);
            // 
            // DoneButton
            // 
            this.DoneButton.BackColor = System.Drawing.SystemColors.Control;
            this.DoneButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.DoneButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoneButton.Location = new System.Drawing.Point(208, 206);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(80, 23);
            this.DoneButton.TabIndex = 24;
            this.DoneButton.Text = "Cancel";
            this.DoneButton.UseVisualStyleBackColor = false;
            // 
            // mAuthorLabel
            // 
            this.mAuthorLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mAuthorLabel.Location = new System.Drawing.Point(8, 16);
            this.mAuthorLabel.Name = "mAuthorLabel";
            this.mAuthorLabel.Size = new System.Drawing.Size(128, 23);
            this.mAuthorLabel.TabIndex = 48;
            this.mAuthorLabel.Text = "Author";
            // 
            // CurrentProcessTextBox
            // 
            this.CurrentProcessTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.CurrentProcessTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentProcessTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CurrentProcessTextBox.Location = new System.Drawing.Point(102, 146);
            this.CurrentProcessTextBox.Name = "CurrentProcessTextBox";
            this.CurrentProcessTextBox.Size = new System.Drawing.Size(186, 48);
            this.CurrentProcessTextBox.TabIndex = 21;
            // 
            // mChapelLabel
            // 
            this.mChapelLabel.AutoSize = true;
            this.mChapelLabel.Location = new System.Drawing.Point(4, 416);
            this.mChapelLabel.Name = "mChapelLabel";
            this.mChapelLabel.Size = new System.Drawing.Size(0, 13);
            this.mChapelLabel.TabIndex = 28;
            // 
            // PhotoCruzMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 776);
            this.ControlBox = false;
            this.Controls.Add(this.mChapelLabel);
            this.Controls.Add(this.mHiddenPanel);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.mAuthorGroupBox);
            this.Controls.Add(this.CreateButton);
            this.Controls.Add(this.PhotoCruzCancelButton);
            this.Controls.Add(this.SeekTimeLabel);
            this.Controls.Add(this.PCPlayButton);
            this.Controls.Add(this.PreviewPanel);
            this.Controls.Add(this.SeekTrackBar);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PhotoCruzMainWindow";
            this.ShowInTaskbar = false;
            this.Text = "PhotoCruz";
            this.Load += new System.EventHandler(this.PhotoCruzMainWindow_Load);
            this.PreviewPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PreviewPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SeekTrackBar)).EndInit();
            this.mAuthorGroupBox.ResumeLayout(false);
            this.mAuthorGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel PreviewPanel;
        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Button PhotoCruzCancelButton;
        private System.Windows.Forms.TrackBar SeekTrackBar;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button PCPlayButton;
        private System.Windows.Forms.Label SeekTimeLabel;
        private System.Windows.Forms.PictureBox PreviewPictureBox;
        private System.Windows.Forms.GroupBox mAuthorGroupBox;
        private System.Windows.Forms.ComboBox WriterComboBox;
        private System.Windows.Forms.Label mTotalCompleteLabel2;
        private System.Windows.Forms.Label mTotalCompleteLabel;
        private System.Windows.Forms.Label mETATextBox;
        private System.Windows.Forms.Label ElapsedTimeTextBox;
        private System.Windows.Forms.Label BufferLabel;
        private System.Windows.Forms.ProgressBar BufferFull;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label ElapsedTime;
        private System.Windows.Forms.Label EstimateTimeLeft;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label mWriterLabel;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.TextBox EncodeCheckPointsTextBox;
        private System.Windows.Forms.Button mPreviousWizard3;
        private System.Windows.Forms.Button DoneButton;
        private System.Windows.Forms.Label mAuthorLabel;
        private System.Windows.Forms.Label CurrentProcessTextBox;
        private System.Windows.Forms.CheckBox WriteToDVDcheckBox;
        private System.Windows.Forms.CheckBox BuildDVDonly;
        private System.Windows.Forms.Label mChapelLabel;
        private System.Windows.Forms.ComboBox GenerateComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox SizeComboBox;
        private System.Windows.Forms.Label mSizeLabel;
        private System.Windows.Forms.Panel mHiddenPanel;
        private System.Windows.Forms.CheckBox mUseMotionBlurCheckBox;
        private System.Windows.Forms.ComboBox mFpsCombo;
        private System.Windows.Forms.Label mFpsLabel;
        private System.Windows.Forms.ComboBox mQualityCombo;
        private System.Windows.Forms.Label mQualityLabel;
    }
}