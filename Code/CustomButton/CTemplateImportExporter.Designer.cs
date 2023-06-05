namespace CustomButton
{
    partial class CTemplateImportExporter
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
            this.mDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mExportButton = new System.Windows.Forms.Button();
            this.mDetectedAnimatedEffects = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mNameTextBox
            // 
            this.mNameTextBox.Location = new System.Drawing.Point(125, 12);
            this.mNameTextBox.Name = "mNameTextBox";
            this.mNameTextBox.Size = new System.Drawing.Size(174, 22);
            this.mNameTextBox.TabIndex = 0;
            // 
            // mDescriptionTextBox
            // 
            this.mDescriptionTextBox.Location = new System.Drawing.Point(125, 38);
            this.mDescriptionTextBox.Multiline = true;
            this.mDescriptionTextBox.Name = "mDescriptionTextBox";
            this.mDescriptionTextBox.Size = new System.Drawing.Size(253, 112);
            this.mDescriptionTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Description";
            // 
            // mExportButton
            // 
            this.mExportButton.Location = new System.Drawing.Point(219, 316);
            this.mExportButton.Name = "mExportButton";
            this.mExportButton.Size = new System.Drawing.Size(159, 23);
            this.mExportButton.TabIndex = 4;
            this.mExportButton.Text = "Export to template xml file";
            this.mExportButton.UseVisualStyleBackColor = true;
            this.mExportButton.Click += new System.EventHandler(this.mExportButton_Click);
            // 
            // mDetectedAnimatedEffects
            // 
            this.mDetectedAnimatedEffects.Location = new System.Drawing.Point(125, 161);
            this.mDetectedAnimatedEffects.Multiline = true;
            this.mDetectedAnimatedEffects.Name = "mDetectedAnimatedEffects";
            this.mDetectedAnimatedEffects.ReadOnly = true;
            this.mDetectedAnimatedEffects.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mDetectedAnimatedEffects.Size = new System.Drawing.Size(253, 141);
            this.mDetectedAnimatedEffects.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 52);
            this.label3.TabIndex = 6;
            this.label3.Text = "Detected required\r\n animated effects\r\nwhich will be stored\r\nin the template";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // CTemplateImportExporter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(390, 351);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mDetectedAnimatedEffects);
            this.Controls.Add(this.mExportButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mDescriptionTextBox);
            this.Controls.Add(this.mNameTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CTemplateImportExporter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export slideshow as template";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mNameTextBox;
        private System.Windows.Forms.TextBox mDescriptionTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button mExportButton;
        private System.Windows.Forms.TextBox mDetectedAnimatedEffects;
        private System.Windows.Forms.Label label3;
    }
}