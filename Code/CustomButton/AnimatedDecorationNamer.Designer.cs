namespace CustomButton
{
    partial class AnimatedDecorationNamer
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
            this.mNameTextBox = new System.Windows.Forms.TextBox();
            this.mCreateApplyButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mNameTextBox
            // 
            this.mNameTextBox.Location = new System.Drawing.Point(44, 12);
            this.mNameTextBox.Name = "mNameTextBox";
            this.mNameTextBox.Size = new System.Drawing.Size(226, 22);
            this.mNameTextBox.TabIndex = 0;
            // 
            // mCreateApplyButton
            // 
            this.mCreateApplyButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mCreateApplyButton.Location = new System.Drawing.Point(276, 12);
            this.mCreateApplyButton.Name = "mCreateApplyButton";
            this.mCreateApplyButton.Size = new System.Drawing.Size(48, 23);
            this.mCreateApplyButton.TabIndex = 1;
            this.mCreateApplyButton.Text = "Create";
            this.mCreateApplyButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(330, 12);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(50, 23);
            this.mCancelButton.TabIndex = 2;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mNameLabel
            // 
            this.mNameLabel.AutoSize = true;
            this.mNameLabel.Location = new System.Drawing.Point(3, 15);
            this.mNameLabel.Name = "mNameLabel";
            this.mNameLabel.Size = new System.Drawing.Size(36, 13);
            this.mNameLabel.TabIndex = 3;
            this.mNameLabel.Text = "Name";
            // 
            // AnimatedDecorationNamer
            // 
            this.AcceptButton = this.mCreateApplyButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(385, 44);
            this.Controls.Add(this.mNameLabel);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mCreateApplyButton);
            this.Controls.Add(this.mNameTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnimatedDecorationNamer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create new effect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mNameTextBox;
        private System.Windows.Forms.Button mCreateApplyButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label mNameLabel;
    }
}