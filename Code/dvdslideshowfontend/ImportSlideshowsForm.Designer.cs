namespace dvdslideshowfontend
{
    partial class ImportSlideshowsForm
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
            this.mSlideshowsFoundTextBox = new System.Windows.Forms.Label();
            this.mSelectAllTickBox = new System.Windows.Forms.CheckBox();
            this.mLabel1 = new System.Windows.Forms.Label();
            this.mCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.mImportButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mSlideshowsFoundTextBox
            // 
            this.mSlideshowsFoundTextBox.AutoSize = true;
            this.mSlideshowsFoundTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSlideshowsFoundTextBox.Location = new System.Drawing.Point(12, 9);
            this.mSlideshowsFoundTextBox.Name = "mSlideshowsFoundTextBox";
            this.mSlideshowsFoundTextBox.Size = new System.Drawing.Size(334, 13);
            this.mSlideshowsFoundTextBox.TabIndex = 1;
            this.mSlideshowsFoundTextBox.Text = "Slideshow(s) found in my wqdqwdqwdqwdqwholduats.pds file";
            // 
            // mSelectAllTickBox
            // 
            this.mSelectAllTickBox.AutoSize = true;
            this.mSelectAllTickBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSelectAllTickBox.Location = new System.Drawing.Point(15, 55);
            this.mSelectAllTickBox.Name = "mSelectAllTickBox";
            this.mSelectAllTickBox.Size = new System.Drawing.Size(72, 17);
            this.mSelectAllTickBox.TabIndex = 2;
            this.mSelectAllTickBox.TabStop = false;
            this.mSelectAllTickBox.Text = "Select All";
            this.mSelectAllTickBox.UseVisualStyleBackColor = true;
            this.mSelectAllTickBox.CheckedChanged += new System.EventHandler(this.mSelectAllTickBox_CheckedChanged);
            // 
            // mLabel1
            // 
            this.mLabel1.AutoSize = true;
            this.mLabel1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLabel1.Location = new System.Drawing.Point(12, 31);
            this.mLabel1.Name = "mLabel1";
            this.mLabel1.Size = new System.Drawing.Size(305, 13);
            this.mLabel1.TabIndex = 3;
            this.mLabel1.Text = "Select which slideshows to import into the current project";
            // 
            // mCheckedListBox
            // 
            this.mCheckedListBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCheckedListBox.FormattingEnabled = true;
            this.mCheckedListBox.Location = new System.Drawing.Point(15, 78);
            this.mCheckedListBox.Name = "mCheckedListBox";
            this.mCheckedListBox.Size = new System.Drawing.Size(226, 123);
            this.mCheckedListBox.TabIndex = 4;
            this.mCheckedListBox.TabStop = false;
            this.mCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.mCheckedListBox_ItemCheck);
            this.mCheckedListBox.SelectedValueChanged += new System.EventHandler(this.mCheckedListBox_SelectedValueChanged);
            // 
            // mImportButton
            // 
            this.mImportButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mImportButton.Location = new System.Drawing.Point(247, 165);
            this.mImportButton.Name = "mImportButton";
            this.mImportButton.Size = new System.Drawing.Size(75, 23);
            this.mImportButton.TabIndex = 5;
            this.mImportButton.TabStop = false;
            this.mImportButton.Text = "Import";
            this.mImportButton.UseVisualStyleBackColor = true;
            this.mImportButton.Click += new System.EventHandler(this.mImportButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCancelButton.Location = new System.Drawing.Point(247, 194);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 1;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // ImportSlideshowsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(334, 229);
            this.ControlBox = false;
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mImportButton);
            this.Controls.Add(this.mCheckedListBox);
            this.Controls.Add(this.mLabel1);
            this.Controls.Add(this.mSelectAllTickBox);
            this.Controls.Add(this.mSlideshowsFoundTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportSlideshowsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import slideshows into current project";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mSlideshowsFoundTextBox;
        private System.Windows.Forms.CheckBox mSelectAllTickBox;
        private System.Windows.Forms.Label mLabel1;
        private System.Windows.Forms.CheckedListBox mCheckedListBox;
        private System.Windows.Forms.Button mImportButton;
        private System.Windows.Forms.Button mCancelButton;
    }
}