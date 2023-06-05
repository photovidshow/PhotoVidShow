namespace CustomButton
{
    partial class TextEditControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mRichTextBox1 = new System.Windows.Forms.TextBox();
            this.mFontButton = new System.Windows.Forms.Button();
            this.mClearTextButton = new System.Windows.Forms.Button();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mRichTextBox1
            // 
            this.mRichTextBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mRichTextBox1.Location = new System.Drawing.Point(3, 3);
            this.mRichTextBox1.Name = "mRichTextBox1";
            this.mRichTextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mRichTextBox1.Size = new System.Drawing.Size(347, 25);
            this.mRichTextBox1.TabIndex = 1;
            this.mRichTextBox1.TabStop = false;
            this.mRichTextBox1.Text = "Original text";
            this.mRichTextBox1.WordWrap = false;
            this.mRichTextBox1.TextChanged += new System.EventHandler(this.mRichTextBox1_TextChanged);
            // 
            // mFontButton
            // 
            this.mFontButton.Location = new System.Drawing.Point(356, 3);
            this.mFontButton.Name = "mFontButton";
            this.mFontButton.Size = new System.Drawing.Size(75, 23);
            this.mFontButton.TabIndex = 3;
            this.mFontButton.TabStop = false;
            this.mFontButton.Text = "Font";
            this.mToolTip.SetToolTip(this.mFontButton, "Sets the font of the current editing text");
            this.mFontButton.UseVisualStyleBackColor = true;
            this.mFontButton.Click += new System.EventHandler(this.mFontButton_Click);
            // 
            // mClearTextButton
            // 
            this.mClearTextButton.Location = new System.Drawing.Point(356, 30);
            this.mClearTextButton.Name = "mClearTextButton";
            this.mClearTextButton.Size = new System.Drawing.Size(75, 23);
            this.mClearTextButton.TabIndex = 4;
            this.mClearTextButton.TabStop = false;
            this.mClearTextButton.Text = "Clear text";
            this.mToolTip.SetToolTip(this.mClearTextButton, "Sets the current editing text to empty");
            this.mClearTextButton.UseVisualStyleBackColor = true;
            this.mClearTextButton.Click += new System.EventHandler(this.mClearTextButton_Click);
            // 
            // TextEditControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.mClearTextButton);
            this.Controls.Add(this.mFontButton);
            this.Controls.Add(this.mRichTextBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TextEditControl";
            this.Size = new System.Drawing.Size(435, 312);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mRichTextBox1;
        private System.Windows.Forms.Button mFontButton;
        private System.Windows.Forms.Button mClearTextButton;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
