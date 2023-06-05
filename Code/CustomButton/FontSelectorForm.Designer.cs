namespace CustomButton
{
    partial class FontSelectorForm
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
            this.mFontSelectorControl1 = new CustomButton.FontSelectorControl();
            this.SuspendLayout();
            // 
            // mFontSelectorControl1
            // 
            this.mFontSelectorControl1.BackColor = System.Drawing.Color.White;
            this.mFontSelectorControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mFontSelectorControl1.Location = new System.Drawing.Point(0, 0);
            this.mFontSelectorControl1.Name = "mFontSelectorControl1";
            this.mFontSelectorControl1.Size = new System.Drawing.Size(683, 322);
            this.mFontSelectorControl1.TabIndex = 0;
            this.mFontSelectorControl1.Done += new CustomButton.FontSelectorControl.DoneCallbackDelegate(this.mFontSelectorControl1_Done);
            // 
            // FontSelectorForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(682, 321);
            this.Controls.Add(this.mFontSelectorControl1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FontSelectorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit font";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FontSelectorForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private FontSelectorControl mFontSelectorControl1;

    }
}