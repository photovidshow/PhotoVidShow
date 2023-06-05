namespace CustomButton
{
    partial class PreviewMenuForm
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
            this.mDoneButton = new System.Windows.Forms.Button();
            this.mPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.mRestartButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mDoneButton
            // 
            this.mDoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mDoneButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDoneButton.Location = new System.Drawing.Point(595, 383);
            this.mDoneButton.Name = "mDoneButton";
            this.mDoneButton.Size = new System.Drawing.Size(75, 23);
            this.mDoneButton.TabIndex = 4;
            this.mDoneButton.Text = "Done";
            this.mDoneButton.UseVisualStyleBackColor = true;
            this.mDoneButton.Click += new System.EventHandler(this.mDoneButton_Click);
            // 
            // mPreviewPictureBox
            // 
            this.mPreviewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPictureBox.Location = new System.Drawing.Point(2, 3);
            this.mPreviewPictureBox.Name = "mPreviewPictureBox";
            this.mPreviewPictureBox.Size = new System.Drawing.Size(668, 376);
            this.mPreviewPictureBox.TabIndex = 5;
            this.mPreviewPictureBox.TabStop = false;
            // 
            // mRestartButton
            // 
            this.mRestartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mRestartButton.Location = new System.Drawing.Point(514, 383);
            this.mRestartButton.Name = "mRestartButton";
            this.mRestartButton.Size = new System.Drawing.Size(75, 23);
            this.mRestartButton.TabIndex = 6;
            this.mRestartButton.Text = "Restart";
            this.mRestartButton.UseVisualStyleBackColor = true;
            this.mRestartButton.Click += new System.EventHandler(this.mRestartButton_Click);
            // 
            // PreviewMenuForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(672, 410);
            this.Controls.Add(this.mRestartButton);
            this.Controls.Add(this.mPreviewPictureBox);
            this.Controls.Add(this.mDoneButton);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PreviewMenuForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preview menu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreviewMenuForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mDoneButton;
        private System.Windows.Forms.PictureBox mPreviewPictureBox;
        private System.Windows.Forms.Button mRestartButton;
    }
}