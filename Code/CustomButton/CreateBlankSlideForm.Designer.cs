namespace CustomButton
{
    partial class CreateBlankSlideForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateBlankSlideForm));
            this.mPreviewPanel = new System.Windows.Forms.Panel();
            this.mCreateSlideButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mSelectBackgroundControl = new CustomButton.SelectBackgroundControl();
            this.SuspendLayout();
            // 
            // mPreviewPanel
            // 
            this.mPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPanel.Location = new System.Drawing.Point(317, 150);
            this.mPreviewPanel.Name = "mPreviewPanel";
            this.mPreviewPanel.Size = new System.Drawing.Size(164, 93);
            this.mPreviewPanel.TabIndex = 1;
            // 
            // mCreateSlideButton
            // 
            this.mCreateSlideButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mCreateSlideButton.Location = new System.Drawing.Point(374, 354);
            this.mCreateSlideButton.Name = "mCreateSlideButton";
            this.mCreateSlideButton.Size = new System.Drawing.Size(110, 23);
            this.mCreateSlideButton.TabIndex = 2;
            this.mCreateSlideButton.Text = "Add blank slide";
            this.mCreateSlideButton.UseVisualStyleBackColor = true;
            this.mCreateSlideButton.Click += new System.EventHandler(this.mCreateSlideButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Location = new System.Drawing.Point(374, 325);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(110, 23);
            this.mCancelButton.TabIndex = 3;
            this.mCancelButton.TabStop = false;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(374, 246);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Preview";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Default backgrounds";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(322, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Color";
            // 
            // mSelectBackgroundControl
            // 
            this.mSelectBackgroundControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mSelectBackgroundControl.BackgroundImage")));
            this.mSelectBackgroundControl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSelectBackgroundControl.Location = new System.Drawing.Point(2, 30);
            this.mSelectBackgroundControl.Name = "mSelectBackgroundControl";
            this.mSelectBackgroundControl.Size = new System.Drawing.Size(432, 351);
            this.mSelectBackgroundControl.TabIndex = 0;
            // 
            // CreateBlankSlideForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(489, 386);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mCreateSlideButton);
            this.Controls.Add(this.mPreviewPanel);
            this.Controls.Add(this.mSelectBackgroundControl);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "CreateBlankSlideForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add blank slide";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SelectBackgroundControl mSelectBackgroundControl;
        private System.Windows.Forms.Panel mPreviewPanel;
        private System.Windows.Forms.Button mCreateSlideButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}