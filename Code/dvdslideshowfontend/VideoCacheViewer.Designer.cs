namespace dvdslideshowfontend
{
    partial class VideoCacheViewer
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
            this.mVideoPlayerListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mEstimatedFootPrint = new System.Windows.Forms.Label();
            this.mAdjustmentFactorLabel = new System.Windows.Forms.Label();
            this.somelabel = new System.Windows.Forms.Label();
            this.mCacheSize = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.mNormalSizeLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.mNumberPlayersLabel = new System.Windows.Forms.Label();
            this.mVideoSnapshotFramesListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mNumberSnapshotFrames = new System.Windows.Forms.Label();
            this.mMake200megButton = new System.Windows.Forms.Button();
            this.mMake500megButton = new System.Windows.Forms.Button();
            this.mMake1gButton = new System.Windows.Forms.Button();
            this.mMake2gigButton = new System.Windows.Forms.Button();
            this.mClearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mVideoPlayerListBox
            // 
            this.mVideoPlayerListBox.FormattingEnabled = true;
            this.mVideoPlayerListBox.Location = new System.Drawing.Point(12, 82);
            this.mVideoPlayerListBox.Name = "mVideoPlayerListBox";
            this.mVideoPlayerListBox.Size = new System.Drawing.Size(409, 238);
            this.mVideoPlayerListBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Video player cache";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Estimated foorprint:";
            // 
            // mEstimatedFootPrint
            // 
            this.mEstimatedFootPrint.AutoSize = true;
            this.mEstimatedFootPrint.Location = new System.Drawing.Point(118, 13);
            this.mEstimatedFootPrint.Name = "mEstimatedFootPrint";
            this.mEstimatedFootPrint.Size = new System.Drawing.Size(13, 13);
            this.mEstimatedFootPrint.TabIndex = 3;
            this.mEstimatedFootPrint.Text = "0";
            // 
            // mAdjustmentFactorLabel
            // 
            this.mAdjustmentFactorLabel.AutoSize = true;
            this.mAdjustmentFactorLabel.Location = new System.Drawing.Point(384, 13);
            this.mAdjustmentFactorLabel.Name = "mAdjustmentFactorLabel";
            this.mAdjustmentFactorLabel.Size = new System.Drawing.Size(13, 13);
            this.mAdjustmentFactorLabel.TabIndex = 5;
            this.mAdjustmentFactorLabel.Text = "0";
            // 
            // somelabel
            // 
            this.somelabel.AutoSize = true;
            this.somelabel.Location = new System.Drawing.Point(289, 13);
            this.somelabel.Name = "somelabel";
            this.somelabel.Size = new System.Drawing.Size(89, 13);
            this.somelabel.TabIndex = 4;
            this.somelabel.Text = "Adjustment factor";
            // 
            // mCacheSize
            // 
            this.mCacheSize.AutoSize = true;
            this.mCacheSize.Location = new System.Drawing.Point(517, 13);
            this.mCacheSize.Name = "mCacheSize";
            this.mCacheSize.Size = new System.Drawing.Size(13, 13);
            this.mCacheSize.TabIndex = 7;
            this.mCacheSize.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(414, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Current cache size";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(455, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // mNormalSizeLabel
            // 
            this.mNormalSizeLabel.AutoSize = true;
            this.mNormalSizeLabel.Location = new System.Drawing.Point(261, 13);
            this.mNormalSizeLabel.Name = "mNormalSizeLabel";
            this.mNormalSizeLabel.Size = new System.Drawing.Size(13, 13);
            this.mNormalSizeLabel.TabIndex = 10;
            this.mNormalSizeLabel.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(194, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Normal size";
            // 
            // mNumberPlayersLabel
            // 
            this.mNumberPlayersLabel.AutoSize = true;
            this.mNumberPlayersLabel.Location = new System.Drawing.Point(113, 61);
            this.mNumberPlayersLabel.Name = "mNumberPlayersLabel";
            this.mNumberPlayersLabel.Size = new System.Drawing.Size(13, 13);
            this.mNumberPlayersLabel.TabIndex = 11;
            this.mNumberPlayersLabel.Text = "0";
            // 
            // mVideoSnapshotFramesListBox
            // 
            this.mVideoSnapshotFramesListBox.FormattingEnabled = true;
            this.mVideoSnapshotFramesListBox.Location = new System.Drawing.Point(12, 365);
            this.mVideoSnapshotFramesListBox.Name = "mVideoSnapshotFramesListBox";
            this.mVideoSnapshotFramesListBox.Size = new System.Drawing.Size(409, 238);
            this.mVideoSnapshotFramesListBox.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 340);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Video snapshot frame cache";
            // 
            // mNumberSnapshotFrames
            // 
            this.mNumberSnapshotFrames.AutoSize = true;
            this.mNumberSnapshotFrames.Location = new System.Drawing.Point(162, 340);
            this.mNumberSnapshotFrames.Name = "mNumberSnapshotFrames";
            this.mNumberSnapshotFrames.Size = new System.Drawing.Size(13, 13);
            this.mNumberSnapshotFrames.TabIndex = 14;
            this.mNumberSnapshotFrames.Text = "0";
            // 
            // mMake200megButton
            // 
            this.mMake200megButton.Location = new System.Drawing.Point(455, 141);
            this.mMake200megButton.Name = "mMake200megButton";
            this.mMake200megButton.Size = new System.Drawing.Size(75, 23);
            this.mMake200megButton.TabIndex = 15;
            this.mMake200megButton.Text = "200mb";
            this.mMake200megButton.UseVisualStyleBackColor = true;
            this.mMake200megButton.Click += new System.EventHandler(this.mMake200megButton_Click);
            // 
            // mMake500megButton
            // 
            this.mMake500megButton.Location = new System.Drawing.Point(455, 170);
            this.mMake500megButton.Name = "mMake500megButton";
            this.mMake500megButton.Size = new System.Drawing.Size(75, 23);
            this.mMake500megButton.TabIndex = 16;
            this.mMake500megButton.Text = "500mb";
            this.mMake500megButton.UseVisualStyleBackColor = true;
            this.mMake500megButton.Click += new System.EventHandler(this.mMake500megButton_Click);
            // 
            // mMake1gButton
            // 
            this.mMake1gButton.Location = new System.Drawing.Point(455, 199);
            this.mMake1gButton.Name = "mMake1gButton";
            this.mMake1gButton.Size = new System.Drawing.Size(75, 23);
            this.mMake1gButton.TabIndex = 17;
            this.mMake1gButton.Text = "1gig";
            this.mMake1gButton.UseVisualStyleBackColor = true;
            this.mMake1gButton.Click += new System.EventHandler(this.mMake1gButton_Click);
            // 
            // mMake2gigButton
            // 
            this.mMake2gigButton.Location = new System.Drawing.Point(455, 228);
            this.mMake2gigButton.Name = "mMake2gigButton";
            this.mMake2gigButton.Size = new System.Drawing.Size(75, 23);
            this.mMake2gigButton.TabIndex = 18;
            this.mMake2gigButton.Text = "2gig";
            this.mMake2gigButton.UseVisualStyleBackColor = true;
            this.mMake2gigButton.Click += new System.EventHandler(this.mMake2gigButton_Click);
            // 
            // mClearButton
            // 
            this.mClearButton.Location = new System.Drawing.Point(455, 257);
            this.mClearButton.Name = "mClearButton";
            this.mClearButton.Size = new System.Drawing.Size(75, 23);
            this.mClearButton.TabIndex = 19;
            this.mClearButton.Text = "Clear";
            this.mClearButton.UseVisualStyleBackColor = true;
            this.mClearButton.Click += new System.EventHandler(this.mClearButton_Click);
            // 
            // VideoCacheViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 673);
            this.Controls.Add(this.mClearButton);
            this.Controls.Add(this.mMake2gigButton);
            this.Controls.Add(this.mMake1gButton);
            this.Controls.Add(this.mMake500megButton);
            this.Controls.Add(this.mMake200megButton);
            this.Controls.Add(this.mNumberSnapshotFrames);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mVideoSnapshotFramesListBox);
            this.Controls.Add(this.mNumberPlayersLabel);
            this.Controls.Add(this.mNormalSizeLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mCacheSize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mAdjustmentFactorLabel);
            this.Controls.Add(this.somelabel);
            this.Controls.Add(this.mEstimatedFootPrint);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mVideoPlayerListBox);
            this.Name = "VideoCacheViewer";
            this.Text = "VideoCacheViewer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox mVideoPlayerListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label mEstimatedFootPrint;
        private System.Windows.Forms.Label mAdjustmentFactorLabel;
        private System.Windows.Forms.Label somelabel;
        private System.Windows.Forms.Label mCacheSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label mNormalSizeLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label mNumberPlayersLabel;
        private System.Windows.Forms.ListBox mVideoSnapshotFramesListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label mNumberSnapshotFrames;
        private System.Windows.Forms.Button mMake200megButton;
        private System.Windows.Forms.Button mMake500megButton;
        private System.Windows.Forms.Button mMake1gButton;
        private System.Windows.Forms.Button mMake2gigButton;
        private System.Windows.Forms.Button mClearButton;
    }
}