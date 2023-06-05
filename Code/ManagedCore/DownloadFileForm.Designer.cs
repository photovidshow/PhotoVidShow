namespace ManagedCore
{
    partial class DownloadFileForm
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
            this.mProgressLabel = new System.Windows.Forms.Label();
            this.mProgressBar = new System.Windows.Forms.ProgressBar();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mProgressLabel
            // 
            this.mProgressLabel.AutoSize = true;
            this.mProgressLabel.Location = new System.Drawing.Point(2, 11);
            this.mProgressLabel.Name = "mProgressLabel";
            this.mProgressLabel.Size = new System.Drawing.Size(81, 13);
            this.mProgressLabel.TabIndex = 0;
            this.mProgressLabel.Text = "Progress 0%";
            // 
            // mProgressBar
            // 
            this.mProgressBar.Location = new System.Drawing.Point(89, 6);
            this.mProgressBar.Name = "mProgressBar";
            this.mProgressBar.Size = new System.Drawing.Size(203, 23);
            this.mProgressBar.TabIndex = 1;
            // 
            // mCancelButton
            // 
            this.mCancelButton.Location = new System.Drawing.Point(298, 6);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 2;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
            // 
            // DownloadFileForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(381, 38);
            this.ControlBox = false;
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mProgressBar);
            this.Controls.Add(this.mProgressLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DownloadFileForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Download";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mProgressLabel;
        private System.Windows.Forms.ProgressBar mProgressBar;
        private System.Windows.Forms.Button mCancelButton;
    }
}