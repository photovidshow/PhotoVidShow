namespace CustomButton
{
    partial class VideoPropertiesForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mVideoLengthLabel = new System.Windows.Forms.Label();
            this.mFrameWidthLabel = new System.Windows.Forms.Label();
            this.mFrameHeightLabel = new System.Windows.Forms.Label();
            this.mFrameRateLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.mAudioLabel = new System.Windows.Forms.Label();
            this.mShowRenderGraphCheckBox = new System.Windows.Forms.CheckBox();
            this.mGraphTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Length";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Frame width";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Frame height";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Frame rate";
            // 
            // mVideoLengthLabel
            // 
            this.mVideoLengthLabel.AutoSize = true;
            this.mVideoLengthLabel.Location = new System.Drawing.Point(119, 9);
            this.mVideoLengthLabel.Name = "mVideoLengthLabel";
            this.mVideoLengthLabel.Size = new System.Drawing.Size(64, 13);
            this.mVideoLengthLabel.TabIndex = 6;
            this.mVideoLengthLabel.Text = "00:00:00.00";
            // 
            // mFrameWidthLabel
            // 
            this.mFrameWidthLabel.AutoSize = true;
            this.mFrameWidthLabel.Location = new System.Drawing.Point(119, 31);
            this.mFrameWidthLabel.Name = "mFrameWidthLabel";
            this.mFrameWidthLabel.Size = new System.Drawing.Size(31, 13);
            this.mFrameWidthLabel.TabIndex = 7;
            this.mFrameWidthLabel.Text = "1920";
            // 
            // mFrameHeightLabel
            // 
            this.mFrameHeightLabel.AutoSize = true;
            this.mFrameHeightLabel.Location = new System.Drawing.Point(119, 53);
            this.mFrameHeightLabel.Name = "mFrameHeightLabel";
            this.mFrameHeightLabel.Size = new System.Drawing.Size(31, 13);
            this.mFrameHeightLabel.TabIndex = 8;
            this.mFrameHeightLabel.Text = "1080";
            // 
            // mFrameRateLabel
            // 
            this.mFrameRateLabel.AutoSize = true;
            this.mFrameRateLabel.Location = new System.Drawing.Point(119, 75);
            this.mFrameRateLabel.Name = "mFrameRateLabel";
            this.mFrameRateLabel.Size = new System.Drawing.Size(112, 13);
            this.mFrameRateLabel.TabIndex = 9;
            this.mFrameRateLabel.Text = "30.0 frames / second";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 97);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 13);
            this.label10.TabIndex = 10;
            this.label10.Text = "Audio";
            // 
            // mAudioLabel
            // 
            this.mAudioLabel.AutoSize = true;
            this.mAudioLabel.Location = new System.Drawing.Point(119, 97);
            this.mAudioLabel.Name = "mAudioLabel";
            this.mAudioLabel.Size = new System.Drawing.Size(22, 13);
            this.mAudioLabel.TabIndex = 11;
            this.mAudioLabel.Text = "Yes";
            // 
            // mShowRenderGraphCheckBox
            // 
            this.mShowRenderGraphCheckBox.AutoSize = true;
            this.mShowRenderGraphCheckBox.Location = new System.Drawing.Point(15, 135);
            this.mShowRenderGraphCheckBox.Name = "mShowRenderGraphCheckBox";
            this.mShowRenderGraphCheckBox.Size = new System.Drawing.Size(121, 17);
            this.mShowRenderGraphCheckBox.TabIndex = 12;
            this.mShowRenderGraphCheckBox.Text = "Show player filters";
            this.mShowRenderGraphCheckBox.UseVisualStyleBackColor = true;
            this.mShowRenderGraphCheckBox.CheckedChanged += new System.EventHandler(this.mShowRenderGraphCheckBox_CheckedChanged);
            // 
            // mGraphTextBox
            // 
            this.mGraphTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.mGraphTextBox.Location = new System.Drawing.Point(15, 158);
            this.mGraphTextBox.Multiline = true;
            this.mGraphTextBox.Name = "mGraphTextBox";
            this.mGraphTextBox.ReadOnly = true;
            this.mGraphTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mGraphTextBox.Size = new System.Drawing.Size(429, 315);
            this.mGraphTextBox.TabIndex = 13;
            // 
            // VideoPropertiesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(456, 485);
            this.Controls.Add(this.mGraphTextBox);
            this.Controls.Add(this.mShowRenderGraphCheckBox);
            this.Controls.Add(this.mAudioLabel);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.mFrameRateLabel);
            this.Controls.Add(this.mFrameHeightLabel);
            this.Controls.Add(this.mFrameWidthLabel);
            this.Controls.Add(this.mVideoLengthLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "VideoPropertiesForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Video properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label mVideoLengthLabel;
        private System.Windows.Forms.Label mFrameWidthLabel;
        private System.Windows.Forms.Label mFrameHeightLabel;
        private System.Windows.Forms.Label mFrameRateLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label mAudioLabel;
        private System.Windows.Forms.CheckBox mShowRenderGraphCheckBox;
        private System.Windows.Forms.TextBox mGraphTextBox;
    }
}