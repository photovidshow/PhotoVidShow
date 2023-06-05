namespace CustomButton
{
    partial class CreateSlideFromTemplateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateSlideFromTemplateForm));
            this.mPreviewPanel = new System.Windows.Forms.Panel();
            this.CancelButton = new System.Windows.Forms.Button();
            this.mCreateSlideButton = new System.Windows.Forms.Button();
            this.mPredefinedSlideDesignsControl = new CustomButton.PredefinedSlideDesignsControl();
            this.SuspendLayout();
            // 
            // mPreviewPanel
            // 
            this.mPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPanel.Location = new System.Drawing.Point(444, 3);
            this.mPreviewPanel.Name = "mPreviewPanel";
            this.mPreviewPanel.Size = new System.Drawing.Size(554, 311);
            this.mPreviewPanel.TabIndex = 1;
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButton.Location = new System.Drawing.Point(815, 323);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // mCreateSlideButton
            // 
            this.mCreateSlideButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCreateSlideButton.Enabled = false;
            this.mCreateSlideButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCreateSlideButton.Location = new System.Drawing.Point(896, 323);
            this.mCreateSlideButton.Name = "mCreateSlideButton";
            this.mCreateSlideButton.Size = new System.Drawing.Size(102, 23);
            this.mCreateSlideButton.TabIndex = 3;
            this.mCreateSlideButton.Text = "Create slide";
            this.mCreateSlideButton.UseVisualStyleBackColor = true;
            this.mCreateSlideButton.Click += new System.EventHandler(this.mCreateSlideButton_Click);
            // 
            // mPredefinedSlideDesignsControl
            // 
            this.mPredefinedSlideDesignsControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mPredefinedSlideDesignsControl.BackgroundImage")));
            this.mPredefinedSlideDesignsControl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPredefinedSlideDesignsControl.Location = new System.Drawing.Point(0, 0);
            this.mPredefinedSlideDesignsControl.Name = "mPredefinedSlideDesignsControl";
            this.mPredefinedSlideDesignsControl.Size = new System.Drawing.Size(435, 397);
            this.mPredefinedSlideDesignsControl.TabIndex = 0;
            // 
            // CreateSlideFromTemplateForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1005, 353);
            this.Controls.Add(this.mCreateSlideButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.mPreviewPanel);
            this.Controls.Add(this.mPredefinedSlideDesignsControl);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateSlideFromTemplateForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create slide from template";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateSlideFromTemplateForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private PredefinedSlideDesignsControl mPredefinedSlideDesignsControl;
        private System.Windows.Forms.Panel mPreviewPanel;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button mCreateSlideButton;
    }
}