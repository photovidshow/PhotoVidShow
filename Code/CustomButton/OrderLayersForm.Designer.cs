namespace CustomButton
{
    partial class OrderLayersForm
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
            this.mMoveUpButton = new System.Windows.Forms.Button();
            this.mMoveDownButton = new System.Windows.Forms.Button();
            this.mRemoveButton = new System.Windows.Forms.Button();
            this.mDoneButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.mHighlightSelectedDecorationCheckBox = new System.Windows.Forms.CheckBox();
            this.mColorListBox = new TestControls.ColorListBox();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mMoveUpButton
            // 
            this.mMoveUpButton.BackgroundImage = global::CustomButton.Properties.Resources.undo1;
            this.mMoveUpButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mMoveUpButton.Enabled = false;
            this.mMoveUpButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.mMoveUpButton.Location = new System.Drawing.Point(313, 123);
            this.mMoveUpButton.Margin = new System.Windows.Forms.Padding(0);
            this.mMoveUpButton.Name = "mMoveUpButton";
            this.mMoveUpButton.Size = new System.Drawing.Size(28, 33);
            this.mMoveUpButton.TabIndex = 1;
            this.mMoveUpButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mMoveUpButton, "Move the decoration up one layer");
            this.mMoveUpButton.UseVisualStyleBackColor = true;
            this.mMoveUpButton.Click += new System.EventHandler(this.mMoveUpButton_Click);
            // 
            // mMoveDownButton
            // 
            this.mMoveDownButton.BackgroundImage = global::CustomButton.Properties.Resources.redo1;
            this.mMoveDownButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mMoveDownButton.Enabled = false;
            this.mMoveDownButton.Location = new System.Drawing.Point(313, 158);
            this.mMoveDownButton.Margin = new System.Windows.Forms.Padding(0);
            this.mMoveDownButton.Name = "mMoveDownButton";
            this.mMoveDownButton.Size = new System.Drawing.Size(28, 33);
            this.mMoveDownButton.TabIndex = 2;
            this.mMoveDownButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mMoveDownButton, "Move the decoration down one layer");
            this.mMoveDownButton.UseVisualStyleBackColor = true;
            this.mMoveDownButton.Click += new System.EventHandler(this.mMoveDownButton_Click);
            // 
            // mRemoveButton
            // 
            this.mRemoveButton.Enabled = false;
            this.mRemoveButton.Location = new System.Drawing.Point(313, 212);
            this.mRemoveButton.Name = "mRemoveButton";
            this.mRemoveButton.Size = new System.Drawing.Size(66, 23);
            this.mRemoveButton.TabIndex = 3;
            this.mRemoveButton.TabStop = false;
            this.mRemoveButton.Text = "Remove";
            this.mToolTip.SetToolTip(this.mRemoveButton, "Remove the decoration from the slide");
            this.mRemoveButton.UseVisualStyleBackColor = true;
            this.mRemoveButton.Click += new System.EventHandler(this.mRemoveButton_Click);
            // 
            // mDoneButton
            // 
            this.mDoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mDoneButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDoneButton.Location = new System.Drawing.Point(987, 404);
            this.mDoneButton.Name = "mDoneButton";
            this.mDoneButton.Size = new System.Drawing.Size(75, 23);
            this.mDoneButton.TabIndex = 1;
            this.mDoneButton.Text = "Done";
            this.mDoneButton.UseVisualStyleBackColor = true;
            this.mDoneButton.Click += new System.EventHandler(this.mDoneButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(308, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Top";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(308, 390);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Bottom";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "Decoration layers";
            // 
            // mPreviewPictureBox
            // 
            this.mPreviewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPictureBox.Location = new System.Drawing.Point(394, 25);
            this.mPreviewPictureBox.Name = "mPreviewPictureBox";
            this.mPreviewPictureBox.Size = new System.Drawing.Size(668, 376);
            this.mPreviewPictureBox.TabIndex = 5;
            this.mPreviewPictureBox.TabStop = false;
            // 
            // mHighlightSelectedDecorationCheckBox
            // 
            this.mHighlightSelectedDecorationCheckBox.AutoSize = true;
            this.mHighlightSelectedDecorationCheckBox.Checked = true;
            this.mHighlightSelectedDecorationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mHighlightSelectedDecorationCheckBox.Location = new System.Drawing.Point(394, 410);
            this.mHighlightSelectedDecorationCheckBox.Name = "mHighlightSelectedDecorationCheckBox";
            this.mHighlightSelectedDecorationCheckBox.Size = new System.Drawing.Size(179, 17);
            this.mHighlightSelectedDecorationCheckBox.TabIndex = 37;
            this.mHighlightSelectedDecorationCheckBox.TabStop = false;
            this.mHighlightSelectedDecorationCheckBox.Text = "Highlight selected decoration";
            this.mHighlightSelectedDecorationCheckBox.UseVisualStyleBackColor = true;
            this.mHighlightSelectedDecorationCheckBox.CheckedChanged += new System.EventHandler(this.mHighlightSelectedDecorationCheckBox_CheckedChanged);
            // 
            // mColorListBox
            // 
            this.mColorListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.mColorListBox.FormattingEnabled = true;
            this.mColorListBox.Location = new System.Drawing.Point(12, 25);
            this.mColorListBox.Name = "mColorListBox";
            this.mColorListBox.Size = new System.Drawing.Size(295, 381);
            this.mColorListBox.TabIndex = 8;
            this.mColorListBox.TabStop = false;
            this.mColorListBox.SelectedIndexChanged += new System.EventHandler(this.mColorListBox_SelectedIndexChanged);
            // 
            // OrderLayersForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1074, 435);
            this.ControlBox = false;
            this.Controls.Add(this.mHighlightSelectedDecorationCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mColorListBox);
            this.Controls.Add(this.mPreviewPictureBox);
            this.Controls.Add(this.mDoneButton);
            this.Controls.Add(this.mRemoveButton);
            this.Controls.Add(this.mMoveDownButton);
            this.Controls.Add(this.mMoveUpButton);
            this.Controls.Add(this.label3);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderLayersForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Order layers";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrderLayersForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mMoveUpButton;
        private System.Windows.Forms.Button mMoveDownButton;
        private System.Windows.Forms.Button mRemoveButton;
        private System.Windows.Forms.Button mDoneButton;
        private System.Windows.Forms.PictureBox mPreviewPictureBox;
        private TestControls.ColorListBox mColorListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox mHighlightSelectedDecorationCheckBox;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}