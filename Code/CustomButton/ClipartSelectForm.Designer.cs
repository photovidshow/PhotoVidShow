namespace CustomButton
{
    partial class ClipartSelectForm
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
            this.mClipartPanel = new System.Windows.Forms.Panel();
            this.mAddToSlideButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mClipartPanel
            // 
            this.mClipartPanel.AllowDrop = true;
            this.mClipartPanel.AutoScroll = true;
            this.mClipartPanel.BackColor = System.Drawing.SystemColors.Window;
            this.mClipartPanel.Location = new System.Drawing.Point(0, 1);
            this.mClipartPanel.Name = "mClipartPanel";
            this.mClipartPanel.Size = new System.Drawing.Size(631, 391);
            this.mClipartPanel.TabIndex = 1;
            // 
            // mAddToSlideButton
            // 
            this.mAddToSlideButton.Enabled = false;
            this.mAddToSlideButton.Location = new System.Drawing.Point(529, 398);
            this.mAddToSlideButton.Name = "mAddToSlideButton";
            this.mAddToSlideButton.Size = new System.Drawing.Size(102, 23);
            this.mAddToSlideButton.TabIndex = 2;
            this.mAddToSlideButton.Text = "Add to slide";
            this.mAddToSlideButton.UseVisualStyleBackColor = true;
            this.mAddToSlideButton.Click += new System.EventHandler(this.mAddToSlideButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Location = new System.Drawing.Point(448, 398);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 3;
            this.mCancelButton.Text = "Close";
            this.mCancelButton.UseVisualStyleBackColor = true;
            this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
            // 
            // ClipartSelectForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(633, 428);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mAddToSlideButton);
            this.Controls.Add(this.mClipartPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClipartSelectForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Clipart";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mClipartPanel;
        private System.Windows.Forms.Button mAddToSlideButton;
        private System.Windows.Forms.Button mCancelButton;
    }
}