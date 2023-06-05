namespace PhotoCruzBurn
{
    partial class PhotoCruzBurnMainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 
        
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhotoCruzBurnMainWindow));
            this.SeekTimeLabel = new System.Windows.Forms.Label();
            this.BufferLabel = new System.Windows.Forms.Label();
            this.mTotalCompleteLabel2 = new System.Windows.Forms.Label();
            this.mAuthorGroupBox = new System.Windows.Forms.GroupBox();
            this.txtDirNameMpeg2 = new System.Windows.Forms.TextBox();
            this.txtDirName = new System.Windows.Forms.TextBox();
            this.HiddenPanel = new System.Windows.Forms.Panel();
            this.PreviewPanel = new System.Windows.Forms.Panel();
            this.WriteToDVDcheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSelectMpegDir = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mETATextBox = new System.Windows.Forms.Label();
            this.btnSelectVideoDir = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.BufferFull = new System.Windows.Forms.ProgressBar();
            this.CurrentProcessTextBox = new System.Windows.Forms.Label();
            this.WriterComboBox = new System.Windows.Forms.ComboBox();
            this.mTotalCompleteLabel = new System.Windows.Forms.Label();
            this.ElapsedTimeTextBox = new System.Windows.Forms.Label();
            this.ElapsedTime = new System.Windows.Forms.Label();
            this.EstimateTimeLeft = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.Go = new System.Windows.Forms.Button();
            this.mAuthorLabel = new System.Windows.Forms.Label();
            this.StopButton = new System.Windows.Forms.Button();
            this.mChapelLabel = new System.Windows.Forms.Label();
            this.CreateButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.mAuthorGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // SeekTimeLabel
            // 
            this.SeekTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SeekTimeLabel.Location = new System.Drawing.Point(512, 246);
            this.SeekTimeLabel.Name = "SeekTimeLabel";
            this.SeekTimeLabel.Size = new System.Drawing.Size(81, 29);
            this.SeekTimeLabel.TabIndex = 34;
            this.SeekTimeLabel.Text = "00:00:00.00";
            this.SeekTimeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // BufferLabel
            // 
            this.BufferLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BufferLabel.Location = new System.Drawing.Point(316, 229);
            this.BufferLabel.Name = "BufferLabel";
            this.BufferLabel.Size = new System.Drawing.Size(40, 16);
            this.BufferLabel.TabIndex = 23;
            this.BufferLabel.Text = "Buffer";
            this.BufferLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // mTotalCompleteLabel2
            // 
            this.mTotalCompleteLabel2.BackColor = System.Drawing.SystemColors.Control;
            this.mTotalCompleteLabel2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTotalCompleteLabel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mTotalCompleteLabel2.Location = new System.Drawing.Point(394, 197);
            this.mTotalCompleteLabel2.Name = "mTotalCompleteLabel2";
            this.mTotalCompleteLabel2.Size = new System.Drawing.Size(63, 23);
            this.mTotalCompleteLabel2.TabIndex = 28;
            // 
            // mAuthorGroupBox
            // 
            this.mAuthorGroupBox.BackColor = System.Drawing.SystemColors.Control;
            this.mAuthorGroupBox.Controls.Add(this.txtDirNameMpeg2);
            this.mAuthorGroupBox.Controls.Add(this.txtDirName);
            this.mAuthorGroupBox.Controls.Add(this.HiddenPanel);
            this.mAuthorGroupBox.Controls.Add(this.PreviewPanel);
            this.mAuthorGroupBox.Controls.Add(this.WriteToDVDcheckBox);
            this.mAuthorGroupBox.Controls.Add(this.label3);
            this.mAuthorGroupBox.Controls.Add(this.btnSelectMpegDir);
            this.mAuthorGroupBox.Controls.Add(this.label5);
            this.mAuthorGroupBox.Controls.Add(this.label1);
            this.mAuthorGroupBox.Controls.Add(this.mETATextBox);
            this.mAuthorGroupBox.Controls.Add(this.btnSelectVideoDir);
            this.mAuthorGroupBox.Controls.Add(this.label6);
            this.mAuthorGroupBox.Controls.Add(this.BufferFull);
            this.mAuthorGroupBox.Controls.Add(this.CurrentProcessTextBox);
            this.mAuthorGroupBox.Controls.Add(this.WriterComboBox);
            this.mAuthorGroupBox.Controls.Add(this.mTotalCompleteLabel2);
            this.mAuthorGroupBox.Controls.Add(this.mTotalCompleteLabel);
            this.mAuthorGroupBox.Controls.Add(this.ElapsedTimeTextBox);
            this.mAuthorGroupBox.Controls.Add(this.BufferLabel);
            this.mAuthorGroupBox.Controls.Add(this.ElapsedTime);
            this.mAuthorGroupBox.Controls.Add(this.EstimateTimeLeft);
            this.mAuthorGroupBox.Controls.Add(this.progressBar1);
            this.mAuthorGroupBox.Controls.Add(this.label2);
            this.mAuthorGroupBox.Controls.Add(this.Go);
            this.mAuthorGroupBox.Controls.Add(this.mAuthorLabel);
            this.mAuthorGroupBox.Location = new System.Drawing.Point(4, 0);
            this.mAuthorGroupBox.Name = "mAuthorGroupBox";
            this.mAuthorGroupBox.Size = new System.Drawing.Size(484, 310);
            this.mAuthorGroupBox.TabIndex = 36;
            this.mAuthorGroupBox.TabStop = false;
            this.mAuthorGroupBox.Enter += new System.EventHandler(this.mAuthorGroupBox_Enter);
            // 
            // txtDirNameMpeg2
            // 
            this.txtDirNameMpeg2.BackColor = System.Drawing.SystemColors.Control;
            this.txtDirNameMpeg2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDirNameMpeg2.Location = new System.Drawing.Point(11, 110);
            this.txtDirNameMpeg2.Name = "txtDirNameMpeg2";
            this.txtDirNameMpeg2.ReadOnly = true;
            this.txtDirNameMpeg2.Size = new System.Drawing.Size(437, 22);
            this.txtDirNameMpeg2.TabIndex = 58;
            this.txtDirNameMpeg2.TextChanged += new System.EventHandler(this.txtDirNameMpeg2_TextChanged);
            // 
            // txtDirName
            // 
            this.txtDirName.BackColor = System.Drawing.SystemColors.Control;
            this.txtDirName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDirName.Location = new System.Drawing.Point(11, 63);
            this.txtDirName.Name = "txtDirName";
            this.txtDirName.ReadOnly = true;
            this.txtDirName.Size = new System.Drawing.Size(437, 22);
            this.txtDirName.TabIndex = 53;
            // 
            // HiddenPanel
            // 
            this.HiddenPanel.Location = new System.Drawing.Point(370, 16);
            this.HiddenPanel.Name = "HiddenPanel";
            this.HiddenPanel.Size = new System.Drawing.Size(36, 25);
            this.HiddenPanel.TabIndex = 62;
            this.HiddenPanel.Visible = false;
            // 
            // PreviewPanel
            // 
            this.PreviewPanel.Location = new System.Drawing.Point(325, 18);
            this.PreviewPanel.Name = "PreviewPanel";
            this.PreviewPanel.Size = new System.Drawing.Size(37, 21);
            this.PreviewPanel.TabIndex = 61;
            this.PreviewPanel.Visible = false;
            // 
            // WriteToDVDcheckBox
            // 
            this.WriteToDVDcheckBox.AutoSize = true;
            this.WriteToDVDcheckBox.Checked = true;
            this.WriteToDVDcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WriteToDVDcheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WriteToDVDcheckBox.Location = new System.Drawing.Point(389, 143);
            this.WriteToDVDcheckBox.Name = "WriteToDVDcheckBox";
            this.WriteToDVDcheckBox.Size = new System.Drawing.Size(92, 17);
            this.WriteToDVDcheckBox.TabIndex = 60;
            this.WriteToDVDcheckBox.Text = "Write to disk";
            this.WriteToDVDcheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 59;
            this.label3.Text = "OR select a video file";
            // 
            // btnSelectMpegDir
            // 
            this.btnSelectMpegDir.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectMpegDir.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectMpegDir.Location = new System.Drawing.Point(452, 108);
            this.btnSelectMpegDir.Name = "btnSelectMpegDir";
            this.btnSelectMpegDir.Size = new System.Drawing.Size(26, 24);
            this.btnSelectMpegDir.TabIndex = 57;
            this.btnSelectMpegDir.Text = "...";
            this.btnSelectMpegDir.Click += new System.EventHandler(this.btnSelectMpegDir_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(105, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 23);
            this.label5.TabIndex = 55;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "Select a VIDEO_TS or BDMV source folder";
            // 
            // mETATextBox
            // 
            this.mETATextBox.BackColor = System.Drawing.SystemColors.Control;
            this.mETATextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mETATextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mETATextBox.Location = new System.Drawing.Point(394, 174);
            this.mETATextBox.Name = "mETATextBox";
            this.mETATextBox.Size = new System.Drawing.Size(56, 23);
            this.mETATextBox.TabIndex = 26;
            // 
            // btnSelectVideoDir
            // 
            this.btnSelectVideoDir.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSelectVideoDir.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectVideoDir.Location = new System.Drawing.Point(452, 61);
            this.btnSelectVideoDir.Name = "btnSelectVideoDir";
            this.btnSelectVideoDir.Size = new System.Drawing.Size(26, 24);
            this.btnSelectVideoDir.TabIndex = 52;
            this.btnSelectVideoDir.Text = "...";
            this.btnSelectVideoDir.Click += new System.EventHandler(this.btnSelectVideoDir_Click);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(10, 197);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 23);
            this.label6.TabIndex = 20;
            this.label6.Text = "Current process:";
            // 
            // BufferFull
            // 
            this.BufferFull.Location = new System.Drawing.Point(362, 229);
            this.BufferFull.Name = "BufferFull";
            this.BufferFull.Size = new System.Drawing.Size(118, 16);
            this.BufferFull.TabIndex = 22;
            // 
            // CurrentProcessTextBox
            // 
            this.CurrentProcessTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.CurrentProcessTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentProcessTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CurrentProcessTextBox.Location = new System.Drawing.Point(104, 197);
            this.CurrentProcessTextBox.Name = "CurrentProcessTextBox";
            this.CurrentProcessTextBox.Size = new System.Drawing.Size(186, 48);
            this.CurrentProcessTextBox.TabIndex = 21;
            // 
            // WriterComboBox
            // 
            this.WriterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WriterComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WriterComboBox.Location = new System.Drawing.Point(50, 139);
            this.WriterComboBox.Name = "WriterComboBox";
            this.WriterComboBox.Size = new System.Drawing.Size(240, 21);
            this.WriterComboBox.TabIndex = 16;
            // 
            // mTotalCompleteLabel
            // 
            this.mTotalCompleteLabel.BackColor = System.Drawing.Color.Transparent;
            this.mTotalCompleteLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTotalCompleteLabel.Location = new System.Drawing.Point(286, 197);
            this.mTotalCompleteLabel.Name = "mTotalCompleteLabel";
            this.mTotalCompleteLabel.Size = new System.Drawing.Size(120, 16);
            this.mTotalCompleteLabel.TabIndex = 27;
            this.mTotalCompleteLabel.Text = "Total Complete:";
            // 
            // ElapsedTimeTextBox
            // 
            this.ElapsedTimeTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.ElapsedTimeTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ElapsedTimeTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ElapsedTimeTextBox.Location = new System.Drawing.Point(104, 174);
            this.ElapsedTimeTextBox.Name = "ElapsedTimeTextBox";
            this.ElapsedTimeTextBox.Size = new System.Drawing.Size(95, 23);
            this.ElapsedTimeTextBox.TabIndex = 25;
            // 
            // ElapsedTime
            // 
            this.ElapsedTime.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ElapsedTime.Location = new System.Drawing.Point(10, 174);
            this.ElapsedTime.Name = "ElapsedTime";
            this.ElapsedTime.Size = new System.Drawing.Size(88, 23);
            this.ElapsedTime.TabIndex = 10;
            this.ElapsedTime.Text = "Elapsed tIme:";
            // 
            // EstimateTimeLeft
            // 
            this.EstimateTimeLeft.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EstimateTimeLeft.Location = new System.Drawing.Point(286, 174);
            this.EstimateTimeLeft.Name = "EstimateTimeLeft";
            this.EstimateTimeLeft.Size = new System.Drawing.Size(123, 23);
            this.EstimateTimeLeft.TabIndex = 11;
            this.EstimateTimeLeft.Text = "Estimated time left:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(7, 251);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(473, 23);
            this.progressBar1.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 23);
            this.label2.TabIndex = 17;
            this.label2.Text = "Writer";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Go
            // 
            this.Go.BackColor = System.Drawing.SystemColors.Control;
            this.Go.Enabled = false;
            this.Go.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Go.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Go.Location = new System.Drawing.Point(421, 280);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(59, 23);
            this.Go.TabIndex = 6;
            this.Go.Text = "Burn";
            this.Go.UseVisualStyleBackColor = false;
            this.Go.Click += new System.EventHandler(this.Go_Click_2);
            // 
            // mAuthorLabel
            // 
            this.mAuthorLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mAuthorLabel.Location = new System.Drawing.Point(8, 16);
            this.mAuthorLabel.Name = "mAuthorLabel";
            this.mAuthorLabel.Size = new System.Drawing.Size(226, 23);
            this.mAuthorLabel.TabIndex = 48;
            this.mAuthorLabel.Text = "Author DVD and Blu-ray Video";
            // 
            // StopButton
            // 
            this.StopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StopButton.Location = new System.Drawing.Point(-62, 246);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(37, 23);
            this.StopButton.TabIndex = 31;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            // 
            // mChapelLabel
            // 
            this.mChapelLabel.AutoSize = true;
            this.mChapelLabel.Location = new System.Drawing.Point(-101, 239);
            this.mChapelLabel.Name = "mChapelLabel";
            this.mChapelLabel.Size = new System.Drawing.Size(0, 13);
            this.mChapelLabel.TabIndex = 37;
            // 
            // CreateButton
            // 
            this.CreateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateButton.Location = new System.Drawing.Point(521, 279);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(72, 23);
            this.CreateButton.TabIndex = 32;
            this.CreateButton.Text = "Create";
            this.CreateButton.UseVisualStyleBackColor = true;
            // 
            // PlayButton
            // 
            this.PlayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayButton.Location = new System.Drawing.Point(-101, 246);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(35, 23);
            this.PlayButton.TabIndex = 29;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            // 
            // PhotoCruzBurnMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 312);
            this.Controls.Add(this.mAuthorGroupBox);
            this.Controls.Add(this.SeekTimeLabel);
            this.Controls.Add(this.CreateButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.mChapelLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PhotoCruzBurnMainWindow";
            this.Text = "MemoryGuard DVD Burner";
            this.Shown += new System.EventHandler(this.PhotoCruzBurnMainWindow_Shown);
            this.mAuthorGroupBox.ResumeLayout(false);
            this.mAuthorGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SeekTimeLabel;
        private System.Windows.Forms.Label BufferLabel;
        private System.Windows.Forms.Label mTotalCompleteLabel2;
        private System.Windows.Forms.GroupBox mAuthorGroupBox;
        private System.Windows.Forms.Button btnSelectVideoDir;
        private System.Windows.Forms.TextBox txtDirName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar BufferFull;
        private System.Windows.Forms.Label CurrentProcessTextBox;
        private System.Windows.Forms.ComboBox WriterComboBox;
        private System.Windows.Forms.Label mTotalCompleteLabel;
        private System.Windows.Forms.Label ElapsedTimeTextBox;
        private System.Windows.Forms.Label ElapsedTime;
        private System.Windows.Forms.Label EstimateTimeLeft;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.Label mAuthorLabel;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Label mChapelLabel;
        private System.Windows.Forms.Button CreateButton;
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Label mETATextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSelectMpegDir;
        private System.Windows.Forms.TextBox txtDirNameMpeg2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox WriteToDVDcheckBox;
        private System.Windows.Forms.Panel PreviewPanel;
        private System.Windows.Forms.Panel HiddenPanel;

    }
}

