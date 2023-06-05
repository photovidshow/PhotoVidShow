namespace CustomButton
{
    partial class PredefinedSlideDesignsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PredefinedSlideDesignsControl));
            this.mSlideDesignsImageList = new System.Windows.Forms.ImageList(this.components);
            this.mApplyDesignButton = new System.Windows.Forms.Button();
            this.mUseNextSlidePicturesToPopulateDesignCheckBox = new System.Windows.Forms.CheckBox();
            this.mEditSlideMediaButton = new System.Windows.Forms.Button();
            this.mTemplateMotionBlurCombo = new System.Windows.Forms.ComboBox();
            this.mTemplateMotionLabel = new System.Windows.Forms.Label();
            this.mPreviewTimeLabel = new System.Windows.Forms.Label();
            this.mPreviewTimeTextBox = new System.Windows.Forms.TextBox();
            this.mGenerateImages = new System.Windows.Forms.Button();
            this.mDescriptionLabel = new System.Windows.Forms.Label();
            this.mInputImagesLabel = new System.Windows.Forms.Label();
            this.mEditableLabel = new System.Windows.Forms.Label();
            this.mDesignLabel = new System.Windows.Forms.Label();
            this.mSlideDesignsListView = new System.Windows.Forms.ListView();
            this.mClearSelectedButton = new System.Windows.Forms.Button();
            this.mReCalcSlideLengthOnImage1Change = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mSlideDesignsImageList
            // 
            this.mSlideDesignsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mSlideDesignsImageList.ImageStream")));
            this.mSlideDesignsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mSlideDesignsImageList.Images.SetKeyName(0, "addblankslide4.png");
            // 
            // mApplyDesignButton
            // 
            this.mApplyDesignButton.Enabled = false;
            this.mApplyDesignButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mApplyDesignButton.Location = new System.Drawing.Point(315, 300);
            this.mApplyDesignButton.Name = "mApplyDesignButton";
            this.mApplyDesignButton.Size = new System.Drawing.Size(112, 23);
            this.mApplyDesignButton.TabIndex = 7;
            this.mApplyDesignButton.TabStop = false;
            this.mApplyDesignButton.Text = "Apply design";
            this.toolTip1.SetToolTip(this.mApplyDesignButton, "Applies the selected design to this slide");
            this.mApplyDesignButton.UseVisualStyleBackColor = true;
            this.mApplyDesignButton.Click += new System.EventHandler(this.mApplyDesignButton_Click);
            // 
            // mUseNextSlidePicturesToPopulateDesignCheckBox
            // 
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.AutoSize = true;
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.Checked = true;
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.Location = new System.Drawing.Point(3, 352);
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.Name = "mUseNextSlidePicturesToPopulateDesignCheckBox";
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.Size = new System.Drawing.Size(266, 17);
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.TabIndex = 18;
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.TabStop = false;
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.Text = "Use following slide images to populate design\r\n";
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.UseVisualStyleBackColor = false;
            this.mUseNextSlidePicturesToPopulateDesignCheckBox.CheckedChanged += new System.EventHandler(this.mUseNextSlidePicturesToPopulateDesignCheckBox_CheckedChanged);
            // 
            // mEditSlideMediaButton
            // 
            this.mEditSlideMediaButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEditSlideMediaButton.Location = new System.Drawing.Point(314, 358);
            this.mEditSlideMediaButton.Name = "mEditSlideMediaButton";
            this.mEditSlideMediaButton.Size = new System.Drawing.Size(112, 23);
            this.mEditSlideMediaButton.TabIndex = 15;
            this.mEditSlideMediaButton.TabStop = false;
            this.mEditSlideMediaButton.Text = "Edit slide images";
            this.toolTip1.SetToolTip(this.mEditSlideMediaButton, "Edit, change or re-order the current slide images");
            this.mEditSlideMediaButton.UseVisualStyleBackColor = true;
            this.mEditSlideMediaButton.Click += new System.EventHandler(this.EditSlideMediaButton_Click);
            // 
            // mTemplateMotionBlurCombo
            // 
            this.mTemplateMotionBlurCombo.ForeColor = System.Drawing.Color.Fuchsia;
            this.mTemplateMotionBlurCombo.FormattingEnabled = true;
            this.mTemplateMotionBlurCombo.Items.AddRange(new object[] {
            "1",
            "5",
            "10",
            "15",
            "20",
            "25",
            "30"});
            this.mTemplateMotionBlurCombo.Location = new System.Drawing.Point(92, 418);
            this.mTemplateMotionBlurCombo.Name = "mTemplateMotionBlurCombo";
            this.mTemplateMotionBlurCombo.Size = new System.Drawing.Size(41, 21);
            this.mTemplateMotionBlurCombo.TabIndex = 20;
            this.mTemplateMotionBlurCombo.TabStop = false;
            this.mTemplateMotionBlurCombo.Text = "1";
            this.mTemplateMotionBlurCombo.SelectedIndexChanged += new System.EventHandler(this.mTemplateMotionBlurCombo_SelectedIndexChanged);
            // 
            // mTemplateMotionLabel
            // 
            this.mTemplateMotionLabel.AutoSize = true;
            this.mTemplateMotionLabel.BackColor = System.Drawing.Color.Transparent;
            this.mTemplateMotionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTemplateMotionLabel.ForeColor = System.Drawing.Color.Fuchsia;
            this.mTemplateMotionLabel.Location = new System.Drawing.Point(55, 421);
            this.mTemplateMotionLabel.Name = "mTemplateMotionLabel";
            this.mTemplateMotionLabel.Size = new System.Drawing.Size(40, 13);
            this.mTemplateMotionLabel.TabIndex = 21;
            this.mTemplateMotionLabel.Text = "m blur";
            // 
            // mPreviewTimeLabel
            // 
            this.mPreviewTimeLabel.AutoSize = true;
            this.mPreviewTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.mPreviewTimeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPreviewTimeLabel.ForeColor = System.Drawing.Color.Fuchsia;
            this.mPreviewTimeLabel.Location = new System.Drawing.Point(139, 421);
            this.mPreviewTimeLabel.Name = "mPreviewTimeLabel";
            this.mPreviewTimeLabel.Size = new System.Drawing.Size(54, 13);
            this.mPreviewTimeLabel.TabIndex = 22;
            this.mPreviewTimeLabel.Text = "preivew t";
            // 
            // mPreviewTimeTextBox
            // 
            this.mPreviewTimeTextBox.ForeColor = System.Drawing.Color.Fuchsia;
            this.mPreviewTimeTextBox.Location = new System.Drawing.Point(197, 419);
            this.mPreviewTimeTextBox.Name = "mPreviewTimeTextBox";
            this.mPreviewTimeTextBox.Size = new System.Drawing.Size(41, 22);
            this.mPreviewTimeTextBox.TabIndex = 23;
            this.mPreviewTimeTextBox.TabStop = false;
            this.mPreviewTimeTextBox.TextChanged += new System.EventHandler(this.mPreviewTimeTextBox_TextChanged);
            // 
            // mGenerateImages
            // 
            this.mGenerateImages.ForeColor = System.Drawing.Color.Fuchsia;
            this.mGenerateImages.Location = new System.Drawing.Point(0, 418);
            this.mGenerateImages.Name = "mGenerateImages";
            this.mGenerateImages.Size = new System.Drawing.Size(43, 23);
            this.mGenerateImages.TabIndex = 24;
            this.mGenerateImages.TabStop = false;
            this.mGenerateImages.Text = "Gen images";
            this.mGenerateImages.UseVisualStyleBackColor = true;
            this.mGenerateImages.Click += new System.EventHandler(this.mGenerateImages_Click);
            // 
            // mDescriptionLabel
            // 
            this.mDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.mDescriptionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDescriptionLabel.Location = new System.Drawing.Point(3, 300);
            this.mDescriptionLabel.Name = "mDescriptionLabel";
            this.mDescriptionLabel.Size = new System.Drawing.Size(306, 55);
            this.mDescriptionLabel.TabIndex = 21;
            this.mDescriptionLabel.Text = "label1";
            // 
            // mInputImagesLabel
            // 
            this.mInputImagesLabel.AutoSize = true;
            this.mInputImagesLabel.BackColor = System.Drawing.Color.Transparent;
            this.mInputImagesLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mInputImagesLabel.Location = new System.Drawing.Point(224, 280);
            this.mInputImagesLabel.Name = "mInputImagesLabel";
            this.mInputImagesLabel.Size = new System.Drawing.Size(77, 13);
            this.mInputImagesLabel.TabIndex = 20;
            this.mInputImagesLabel.Text = "Input images:";
            // 
            // mEditableLabel
            // 
            this.mEditableLabel.AutoSize = true;
            this.mEditableLabel.BackColor = System.Drawing.Color.Transparent;
            this.mEditableLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEditableLabel.Location = new System.Drawing.Point(356, 280);
            this.mEditableLabel.Name = "mEditableLabel";
            this.mEditableLabel.Size = new System.Drawing.Size(70, 13);
            this.mEditableLabel.TabIndex = 18;
            this.mEditableLabel.Text = "Editable: Yes";
            // 
            // mDesignLabel
            // 
            this.mDesignLabel.AutoSize = true;
            this.mDesignLabel.BackColor = System.Drawing.Color.Transparent;
            this.mDesignLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDesignLabel.Location = new System.Drawing.Point(3, 280);
            this.mDesignLabel.Name = "mDesignLabel";
            this.mDesignLabel.Size = new System.Drawing.Size(180, 13);
            this.mDesignLabel.TabIndex = 16;
            this.mDesignLabel.Text = "Design:  Default - None  (1 image)";
            // 
            // mSlideDesignsListView
            // 
            this.mSlideDesignsListView.AllowColumnReorder = true;
            this.mSlideDesignsListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(251)))));
            this.mSlideDesignsListView.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSlideDesignsListView.HideSelection = false;
            this.mSlideDesignsListView.LargeImageList = this.mSlideDesignsImageList;
            this.mSlideDesignsListView.Location = new System.Drawing.Point(3, 3);
            this.mSlideDesignsListView.Margin = new System.Windows.Forms.Padding(0);
            this.mSlideDesignsListView.MultiSelect = false;
            this.mSlideDesignsListView.Name = "mSlideDesignsListView";
            this.mSlideDesignsListView.ShowItemToolTips = true;
            this.mSlideDesignsListView.Size = new System.Drawing.Size(426, 275);
            this.mSlideDesignsListView.TabIndex = 2;
            this.mSlideDesignsListView.TabStop = false;
            this.mSlideDesignsListView.UseCompatibleStateImageBehavior = false;
            this.mSlideDesignsListView.SelectedIndexChanged += new System.EventHandler(this.mSlideDesignsListView_SelectedIndexChanged);
            // 
            // mClearSelectedButton
            // 
            this.mClearSelectedButton.Location = new System.Drawing.Point(314, 329);
            this.mClearSelectedButton.Name = "mClearSelectedButton";
            this.mClearSelectedButton.Size = new System.Drawing.Size(112, 23);
            this.mClearSelectedButton.TabIndex = 25;
            this.mClearSelectedButton.TabStop = false;
            this.mClearSelectedButton.Text = "Clear selected";
            this.toolTip1.SetToolTip(this.mClearSelectedButton, "Clears the current selected design");
            this.mClearSelectedButton.UseVisualStyleBackColor = true;
            this.mClearSelectedButton.Click += new System.EventHandler(this.mClearSelectedButton_Click);
            // 
            // mReCalcSlideLengthOnImage1Change
            // 
            this.mReCalcSlideLengthOnImage1Change.AutoSize = true;
            this.mReCalcSlideLengthOnImage1Change.ForeColor = System.Drawing.Color.Fuchsia;
            this.mReCalcSlideLengthOnImage1Change.Location = new System.Drawing.Point(275, 421);
            this.mReCalcSlideLengthOnImage1Change.Name = "mReCalcSlideLengthOnImage1Change";
            this.mReCalcSlideLengthOnImage1Change.Size = new System.Drawing.Size(126, 17);
            this.mReCalcSlideLengthOnImage1Change.TabIndex = 26;
            this.mReCalcSlideLengthOnImage1Change.TabStop = false;
            this.mReCalcSlideLengthOnImage1Change.Text = "Re-calc slide length";
            this.mReCalcSlideLengthOnImage1Change.UseVisualStyleBackColor = true;
            this.mReCalcSlideLengthOnImage1Change.CheckedChanged += new System.EventHandler(this.mReCalcSlideLengthOnImage1Change_CheckedChanged);
            // 
            // PredefinedSlideDesignsControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::CustomButton.Properties.Resources.back24;
            this.Controls.Add(this.mDescriptionLabel);
            this.Controls.Add(this.mEditSlideMediaButton);
            this.Controls.Add(this.mReCalcSlideLengthOnImage1Change);
            this.Controls.Add(this.mClearSelectedButton);
            this.Controls.Add(this.mGenerateImages);
            this.Controls.Add(this.mEditableLabel);
            this.Controls.Add(this.mInputImagesLabel);
            this.Controls.Add(this.mPreviewTimeTextBox);
            this.Controls.Add(this.mPreviewTimeLabel);
            this.Controls.Add(this.mDesignLabel);
            this.Controls.Add(this.mTemplateMotionLabel);
            this.Controls.Add(this.mTemplateMotionBlurCombo);
            this.Controls.Add(this.mUseNextSlidePicturesToPopulateDesignCheckBox);
            this.Controls.Add(this.mApplyDesignButton);
            this.Controls.Add(this.mSlideDesignsListView);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PredefinedSlideDesignsControl";
            this.Size = new System.Drawing.Size(435, 444);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView mSlideDesignsListView;
        private System.Windows.Forms.Button mApplyDesignButton;
        private System.Windows.Forms.ImageList mSlideDesignsImageList;
        private System.Windows.Forms.Label mDesignLabel;
        private System.Windows.Forms.CheckBox mUseNextSlidePicturesToPopulateDesignCheckBox;
        private System.Windows.Forms.Button mEditSlideMediaButton;
        private System.Windows.Forms.ComboBox mTemplateMotionBlurCombo;
        private System.Windows.Forms.Label mTemplateMotionLabel;
        private System.Windows.Forms.Label mPreviewTimeLabel;
        private System.Windows.Forms.TextBox mPreviewTimeTextBox;
        private System.Windows.Forms.Label mEditableLabel;
        private System.Windows.Forms.Label mInputImagesLabel;
        private System.Windows.Forms.Button mGenerateImages;
        private System.Windows.Forms.Label mDescriptionLabel;
        private System.Windows.Forms.Button mClearSelectedButton;
        private System.Windows.Forms.CheckBox mReCalcSlideLengthOnImage1Change;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
