namespace dvdslideshowfontend
{
    partial class CBurnPreviousEncodedProjectWindow
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
            this.mProjectsListBox = new System.Windows.Forms.ListBox();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mSelectProjectButton = new System.Windows.Forms.Button();
            this.mSelectedProjectDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mProjectsListBox
            // 
            this.mProjectsListBox.FormattingEnabled = true;
            this.mProjectsListBox.Location = new System.Drawing.Point(12, 12);
            this.mProjectsListBox.Name = "mProjectsListBox";
            this.mProjectsListBox.Size = new System.Drawing.Size(495, 225);
            this.mProjectsListBox.TabIndex = 0;
            this.mProjectsListBox.SelectedIndexChanged += new System.EventHandler(this.mProjectsListBox_SelectedIndexChanged);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Location = new System.Drawing.Point(351, 253);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 1;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
            // 
            // mSelectProjectButton
            // 
            this.mSelectProjectButton.Enabled = false;
            this.mSelectProjectButton.Location = new System.Drawing.Point(432, 253);
            this.mSelectProjectButton.Name = "mSelectProjectButton";
            this.mSelectProjectButton.Size = new System.Drawing.Size(75, 23);
            this.mSelectProjectButton.TabIndex = 2;
            this.mSelectProjectButton.Text = "Select";
            this.mSelectProjectButton.UseVisualStyleBackColor = true;
            this.mSelectProjectButton.Click += new System.EventHandler(this.mSelectProjectButton_Click);
            // 
            // mSelectedProjectDescription
            // 
            this.mSelectedProjectDescription.AutoSize = true;
            this.mSelectedProjectDescription.Location = new System.Drawing.Point(12, 253);
            this.mSelectedProjectDescription.Name = "mSelectedProjectDescription";
            this.mSelectedProjectDescription.Size = new System.Drawing.Size(129, 13);
            this.mSelectedProjectDescription.TabIndex = 4;
            this.mSelectedProjectDescription.Text = "description placeholder";
            // 
            // CBurnPreviousEncodedProjectWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(519, 288);
            this.Controls.Add(this.mSelectedProjectDescription);
            this.Controls.Add(this.mSelectProjectButton);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mProjectsListBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "CBurnPreviousEncodedProjectWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please select a previously created project from the list below";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox mProjectsListBox;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Button mSelectProjectButton;
        private System.Windows.Forms.Label mSelectedProjectDescription;
    }
}