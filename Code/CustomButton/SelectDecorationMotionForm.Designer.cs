namespace CustomButton
{
    partial class SelectDecorationMotionForm
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
            this.components = new System.ComponentModel.Container();
            this.mDoneButton = new System.Windows.Forms.Button();
            this.mPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mMotionInEffectComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mMotionEffectsEditorButton = new System.Windows.Forms.Button();
            this.mSlideLengthTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mMotionInLengthTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mMotionOutLengthTextBox = new System.Windows.Forms.TextBox();
            this.mMotionOutEffectComboBox = new System.Windows.Forms.ComboBox();
            this.mHighlightSelectedDecorationCheckBox = new System.Windows.Forms.CheckBox();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.mShownTimeGroupBox = new System.Windows.Forms.GroupBox();
            this.mSetStartEndSlideTimes = new CustomButton.SetStartEndSlideTime();
            this.mDecorationsColorListBox = new TestControls.ColorListBox();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.mShownTimeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mDoneButton
            // 
            this.mDoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mDoneButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDoneButton.Location = new System.Drawing.Point(890, 400);
            this.mDoneButton.Name = "mDoneButton";
            this.mDoneButton.Size = new System.Drawing.Size(75, 23);
            this.mDoneButton.TabIndex = 1;
            this.mDoneButton.Text = "Done";
            this.mDoneButton.UseVisualStyleBackColor = true;
            this.mDoneButton.Click += new System.EventHandler(this.mDoneButton_Click);
            // 
            // mPreviewPictureBox
            // 
            this.mPreviewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPictureBox.Location = new System.Drawing.Point(313, 23);
            this.mPreviewPictureBox.Name = "mPreviewPictureBox";
            this.mPreviewPictureBox.Size = new System.Drawing.Size(654, 368);
            this.mPreviewPictureBox.TabIndex = 5;
            this.mPreviewPictureBox.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Decorations";
            // 
            // mMotionInEffectComboBox
            // 
            this.mMotionInEffectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mMotionInEffectComboBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMotionInEffectComboBox.FormattingEnabled = true;
            this.mMotionInEffectComboBox.IntegralHeight = false;
            this.mMotionInEffectComboBox.Location = new System.Drawing.Point(6, 22);
            this.mMotionInEffectComboBox.MaxDropDownItems = 15;
            this.mMotionInEffectComboBox.Name = "mMotionInEffectComboBox";
            this.mMotionInEffectComboBox.Size = new System.Drawing.Size(228, 28);
            this.mMotionInEffectComboBox.TabIndex = 12;
            this.mMotionInEffectComboBox.TabStop = false;
            this.mMotionInEffectComboBox.SelectedIndexChanged += new System.EventHandler(this.mMotionInEffectComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(310, 404);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Total slide length";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mMotionEffectsEditorButton
            // 
            this.mMotionEffectsEditorButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMotionEffectsEditorButton.ForeColor = System.Drawing.Color.Fuchsia;
            this.mMotionEffectsEditorButton.Location = new System.Drawing.Point(752, 403);
            this.mMotionEffectsEditorButton.Name = "mMotionEffectsEditorButton";
            this.mMotionEffectsEditorButton.Size = new System.Drawing.Size(120, 23);
            this.mMotionEffectsEditorButton.TabIndex = 21;
            this.mMotionEffectsEditorButton.TabStop = false;
            this.mMotionEffectsEditorButton.Text = "Motion Effects Editor";
            this.mMotionEffectsEditorButton.UseVisualStyleBackColor = true;
            this.mMotionEffectsEditorButton.Visible = false;
            this.mMotionEffectsEditorButton.Click += new System.EventHandler(this.mMotionEffectsEditorButton_Click);
            // 
            // mSlideLengthTextBox
            // 
            this.mSlideLengthTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSlideLengthTextBox.Location = new System.Drawing.Point(411, 402);
            this.mSlideLengthTextBox.Name = "mSlideLengthTextBox";
            this.mSlideLengthTextBox.ReadOnly = true;
            this.mSlideLengthTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.mSlideLengthTextBox.Size = new System.Drawing.Size(26, 22);
            this.mSlideLengthTextBox.TabIndex = 22;
            this.mSlideLengthTextBox.TabStop = false;
            this.mToolTip.SetToolTip(this.mSlideLengthTextBox, "The length of the slide in seconds");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.mMotionInLengthTextBox);
            this.groupBox1.Controls.Add(this.mMotionInEffectComboBox);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 152);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 60);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Motion in effect (start)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(268, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "s";
            // 
            // mMotionInLengthTextBox
            // 
            this.mMotionInLengthTextBox.Location = new System.Drawing.Point(240, 28);
            this.mMotionInLengthTextBox.Name = "mMotionInLengthTextBox";
            this.mMotionInLengthTextBox.ReadOnly = true;
            this.mMotionInLengthTextBox.Size = new System.Drawing.Size(26, 22);
            this.mMotionInLengthTextBox.TabIndex = 26;
            this.mMotionInLengthTextBox.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.mMotionOutLengthTextBox);
            this.groupBox2.Controls.Add(this.mMotionOutEffectComboBox);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(13, 219);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(295, 60);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Motion out effect (end)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(268, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "s";
            // 
            // mMotionOutLengthTextBox
            // 
            this.mMotionOutLengthTextBox.Location = new System.Drawing.Point(240, 26);
            this.mMotionOutLengthTextBox.Name = "mMotionOutLengthTextBox";
            this.mMotionOutLengthTextBox.ReadOnly = true;
            this.mMotionOutLengthTextBox.Size = new System.Drawing.Size(26, 22);
            this.mMotionOutLengthTextBox.TabIndex = 28;
            this.mMotionOutLengthTextBox.TabStop = false;
            // 
            // mMotionOutEffectComboBox
            // 
            this.mMotionOutEffectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mMotionOutEffectComboBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMotionOutEffectComboBox.FormattingEnabled = true;
            this.mMotionOutEffectComboBox.IntegralHeight = false;
            this.mMotionOutEffectComboBox.Location = new System.Drawing.Point(6, 22);
            this.mMotionOutEffectComboBox.MaxDropDownItems = 15;
            this.mMotionOutEffectComboBox.Name = "mMotionOutEffectComboBox";
            this.mMotionOutEffectComboBox.Size = new System.Drawing.Size(228, 28);
            this.mMotionOutEffectComboBox.TabIndex = 12;
            this.mMotionOutEffectComboBox.TabStop = false;
            this.mMotionOutEffectComboBox.SelectedIndexChanged += new System.EventHandler(this.mMotionOutEffectComboBox_SelectedIndexChanged);
            // 
            // mHighlightSelectedDecorationCheckBox
            // 
            this.mHighlightSelectedDecorationCheckBox.AutoSize = true;
            this.mHighlightSelectedDecorationCheckBox.Checked = true;
            this.mHighlightSelectedDecorationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mHighlightSelectedDecorationCheckBox.Location = new System.Drawing.Point(12, 406);
            this.mHighlightSelectedDecorationCheckBox.Name = "mHighlightSelectedDecorationCheckBox";
            this.mHighlightSelectedDecorationCheckBox.Size = new System.Drawing.Size(179, 17);
            this.mHighlightSelectedDecorationCheckBox.TabIndex = 36;
            this.mHighlightSelectedDecorationCheckBox.TabStop = false;
            this.mHighlightSelectedDecorationCheckBox.Text = "Highlight selected decoration";
            this.mHighlightSelectedDecorationCheckBox.UseVisualStyleBackColor = true;
            this.mHighlightSelectedDecorationCheckBox.CheckedChanged += new System.EventHandler(this.mHighlightSelectedDecorationCheckBox_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(440, 404);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(12, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "s";
            // 
            // mShownTimeGroupBox
            // 
            this.mShownTimeGroupBox.Controls.Add(this.mSetStartEndSlideTimes);
            this.mShownTimeGroupBox.Location = new System.Drawing.Point(13, 285);
            this.mShownTimeGroupBox.Name = "mShownTimeGroupBox";
            this.mShownTimeGroupBox.Size = new System.Drawing.Size(294, 106);
            this.mShownTimeGroupBox.TabIndex = 39;
            this.mShownTimeGroupBox.TabStop = false;
            this.mShownTimeGroupBox.Text = "Shown time";
            // 
            // mSetStartEndSlideTimes
            // 
            this.mSetStartEndSlideTimes.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSetStartEndSlideTimes.Location = new System.Drawing.Point(6, 15);
            this.mSetStartEndSlideTimes.Name = "mSetStartEndSlideTimes";
            this.mSetStartEndSlideTimes.Size = new System.Drawing.Size(213, 87);
            this.mSetStartEndSlideTimes.TabIndex = 37;
            // 
            // mDecorationsColorListBox
            // 
            this.mDecorationsColorListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.mDecorationsColorListBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDecorationsColorListBox.FormattingEnabled = true;
            this.mDecorationsColorListBox.Location = new System.Drawing.Point(12, 25);
            this.mDecorationsColorListBox.Name = "mDecorationsColorListBox";
            this.mDecorationsColorListBox.Size = new System.Drawing.Size(295, 121);
            this.mDecorationsColorListBox.TabIndex = 8;
            this.mDecorationsColorListBox.TabStop = false;
            this.mDecorationsColorListBox.SelectedIndexChanged += new System.EventHandler(this.mDecorationsListBox_SelectedIndexChanged);
            // 
            // SelectDecorationMotionForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(977, 432);
            this.Controls.Add(this.mShownTimeGroupBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mHighlightSelectedDecorationCheckBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mSlideLengthTextBox);
            this.Controls.Add(this.mMotionEffectsEditorButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mDecorationsColorListBox);
            this.Controls.Add(this.mPreviewPictureBox);
            this.Controls.Add(this.mDoneButton);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectDecorationMotionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select decoration motion effects";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectDecorationMotionForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.mShownTimeGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mDoneButton;
        private System.Windows.Forms.PictureBox mPreviewPictureBox;
        private TestControls.ColorListBox mDecorationsColorListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox mMotionInEffectComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button mMotionEffectsEditorButton;
        private System.Windows.Forms.TextBox mSlideLengthTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox mMotionOutEffectComboBox;
        private System.Windows.Forms.TextBox mMotionInLengthTextBox;
        private System.Windows.Forms.TextBox mMotionOutLengthTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox mHighlightSelectedDecorationCheckBox;
        private System.Windows.Forms.ToolTip mToolTip;
        private SetStartEndSlideTime mSetStartEndSlideTimes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox mShownTimeGroupBox;
    }
}