namespace CustomButton
{
    partial class EditSlideMediaForm
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
            this.mPreviewPictureBox = new System.Windows.Forms.PictureBox();
            this.mControlTabs = new System.Windows.Forms.TabControl();
            this.mAdjustTab = new System.Windows.Forms.TabPage();
            this.mAdjustImageColoursControl1 = new CustomButton.AdjustImageColoursControl();
            this.mCropTabPage = new System.Windows.Forms.TabPage();
            this.mCropUserControl1 = new CustomButton.CropUserControl();
            this.mTrimVideoTabPage = new System.Windows.Forms.TabPage();
            this.mTrimVideoControl1 = new CustomButton.TrimVideoControl();
            this.mTrimVideoDoneButton = new System.Windows.Forms.Button();
            this.mOrientationTab = new System.Windows.Forms.TabPage();
            this.mOrientateControl1 = new CustomButton.OrientateControl();
            this.mTextTabPage = new System.Windows.Forms.TabPage();
            this.mTextEditControl1 = new CustomButton.TextEditControl();
            this.mAlphaMapTabPage = new System.Windows.Forms.TabPage();
            this.mSelectAlphaMapControl1 = new CustomButton.SelectAlphaMapControl();
            this.mDoneButton = new System.Windows.Forms.Button();
            this.mResetButton = new System.Windows.Forms.Button();
            this.mPreviewPanel = new System.Windows.Forms.Panel();
            this.mSelectmageButton = new System.Windows.Forms.Button();
            this.mFilenameTextBox = new System.Windows.Forms.TextBox();
            this.mPreviousSlideButton = new System.Windows.Forms.Button();
            this.mNextSlideButton = new System.Windows.Forms.Button();
            this.mTextPBPanel = new System.Windows.Forms.Panel();
            this.mTextPBPanelDarkBackGround = new System.Windows.Forms.Panel();
            this.mEncryptFile = new System.Windows.Forms.Button();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mEditTemplateMediaOrder1 = new CustomButton.EditTemplateMediaOrder();
            this.mShowPreviewCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPictureBox)).BeginInit();
            this.mControlTabs.SuspendLayout();
            this.mAdjustTab.SuspendLayout();
            this.mCropTabPage.SuspendLayout();
            this.mTrimVideoTabPage.SuspendLayout();
            this.mOrientationTab.SuspendLayout();
            this.mTextTabPage.SuspendLayout();
            this.mAlphaMapTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // mPreviewPictureBox
            // 
            this.mPreviewPictureBox.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mPreviewPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPictureBox.Location = new System.Drawing.Point(12, 141);
            this.mPreviewPictureBox.Name = "mPreviewPictureBox";
            this.mPreviewPictureBox.Size = new System.Drawing.Size(470, 352);
            this.mPreviewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.mPreviewPictureBox.TabIndex = 1;
            this.mPreviewPictureBox.TabStop = false;
            // 
            // mControlTabs
            // 
            this.mControlTabs.Controls.Add(this.mAdjustTab);
            this.mControlTabs.Controls.Add(this.mCropTabPage);
            this.mControlTabs.Controls.Add(this.mTrimVideoTabPage);
            this.mControlTabs.Controls.Add(this.mOrientationTab);
            this.mControlTabs.Controls.Add(this.mTextTabPage);
            this.mControlTabs.Controls.Add(this.mAlphaMapTabPage);
            this.mControlTabs.Location = new System.Drawing.Point(496, 141);
            this.mControlTabs.Name = "mControlTabs";
            this.mControlTabs.SelectedIndex = 0;
            this.mControlTabs.Size = new System.Drawing.Size(449, 352);
            this.mControlTabs.TabIndex = 3;
            this.mControlTabs.TabStop = false;
            this.mControlTabs.SelectedIndexChanged += new System.EventHandler(this.mControlsTabControl_SelectedIndexChanged);
            // 
            // mAdjustTab
            // 
            this.mAdjustTab.Controls.Add(this.mAdjustImageColoursControl1);
            this.mAdjustTab.Location = new System.Drawing.Point(4, 22);
            this.mAdjustTab.Name = "mAdjustTab";
            this.mAdjustTab.Padding = new System.Windows.Forms.Padding(3);
            this.mAdjustTab.Size = new System.Drawing.Size(441, 326);
            this.mAdjustTab.TabIndex = 0;
            this.mAdjustTab.Text = "Adjust Color";
            this.mAdjustTab.UseVisualStyleBackColor = true;
            // 
            // mAdjustImageColoursControl1
            // 
            this.mAdjustImageColoursControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mAdjustImageColoursControl1.Location = new System.Drawing.Point(6, 6);
            this.mAdjustImageColoursControl1.Name = "mAdjustImageColoursControl1";
            this.mAdjustImageColoursControl1.Size = new System.Drawing.Size(384, 183);
            this.mAdjustImageColoursControl1.TabIndex = 0;
            this.mAdjustImageColoursControl1.ImageChanged += new CustomButton.AdjustImageColoursControl.ImageChangedCallbackDelegate(this.AdjustColourImageCallback);
            // 
            // mCropTabPage
            // 
            this.mCropTabPage.Controls.Add(this.mCropUserControl1);
            this.mCropTabPage.Location = new System.Drawing.Point(4, 22);
            this.mCropTabPage.Name = "mCropTabPage";
            this.mCropTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mCropTabPage.Size = new System.Drawing.Size(441, 326);
            this.mCropTabPage.TabIndex = 1;
            this.mCropTabPage.Text = "Crop";
            this.mCropTabPage.UseVisualStyleBackColor = true;
            // 
            // mCropUserControl1
            // 
            this.mCropUserControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCropUserControl1.Location = new System.Drawing.Point(6, 6);
            this.mCropUserControl1.Name = "mCropUserControl1";
            this.mCropUserControl1.Size = new System.Drawing.Size(429, 314);
            this.mCropUserControl1.TabIndex = 0;
            this.mCropUserControl1.ImageChanged += new CustomButton.CropUserControl.ImageChangedCallbackDelegate(this.mCropUserControl1_ImageChanged);
            this.mCropUserControl1.MovingCropArea += new CustomButton.CropUserControl.ImageChangedCallbackDelegate(this.mCropUserControl1_MovingCropArea);
            // 
            // mTrimVideoTabPage
            // 
            this.mTrimVideoTabPage.Controls.Add(this.mTrimVideoControl1);
            this.mTrimVideoTabPage.Controls.Add(this.mTrimVideoDoneButton);
            this.mTrimVideoTabPage.Location = new System.Drawing.Point(4, 22);
            this.mTrimVideoTabPage.Name = "mTrimVideoTabPage";
            this.mTrimVideoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mTrimVideoTabPage.Size = new System.Drawing.Size(441, 326);
            this.mTrimVideoTabPage.TabIndex = 3;
            this.mTrimVideoTabPage.Text = "Trim Video";
            this.mTrimVideoTabPage.UseVisualStyleBackColor = true;
            // 
            // mTrimVideoControl1
            // 
            this.mTrimVideoControl1.BackColor = System.Drawing.Color.White;
            this.mTrimVideoControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTrimVideoControl1.Location = new System.Drawing.Point(0, 2);
            this.mTrimVideoControl1.Name = "mTrimVideoControl1";
            this.mTrimVideoControl1.Size = new System.Drawing.Size(426, 324);
            this.mTrimVideoControl1.TabIndex = 33;
            this.mTrimVideoControl1.Trimmed += new CustomButton.TrimVideoControl.ChangedCallbackDelegate(this.mTrimVideoControl1_Trimmed);
            this.mTrimVideoControl1.ScrollStarted += new CustomButton.TrimVideoControl.ScrollStartedCallbackDelegate(this.mTrimVideoControl1_ScrollStarted);
            // 
            // mTrimVideoDoneButton
            // 
            this.mTrimVideoDoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mTrimVideoDoneButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mTrimVideoDoneButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTrimVideoDoneButton.Location = new System.Drawing.Point(327, 391);
            this.mTrimVideoDoneButton.Name = "mTrimVideoDoneButton";
            this.mTrimVideoDoneButton.Size = new System.Drawing.Size(75, 23);
            this.mTrimVideoDoneButton.TabIndex = 32;
            this.mTrimVideoDoneButton.Text = "Done";
            // 
            // mOrientationTab
            // 
            this.mOrientationTab.Controls.Add(this.mOrientateControl1);
            this.mOrientationTab.Location = new System.Drawing.Point(4, 22);
            this.mOrientationTab.Name = "mOrientationTab";
            this.mOrientationTab.Padding = new System.Windows.Forms.Padding(3);
            this.mOrientationTab.Size = new System.Drawing.Size(441, 326);
            this.mOrientationTab.TabIndex = 5;
            this.mOrientationTab.Text = "Orientate";
            this.mOrientationTab.UseVisualStyleBackColor = true;
            // 
            // mOrientateControl1
            // 
            this.mOrientateControl1.Location = new System.Drawing.Point(6, 7);
            this.mOrientateControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mOrientateControl1.Name = "mOrientateControl1";
            this.mOrientateControl1.Size = new System.Drawing.Size(346, 350);
            this.mOrientateControl1.TabIndex = 0;
            this.mOrientateControl1.ImageChanged += new CustomButton.OrientateControl.ImageChangedCallbackDelegate(this.mOrientateControl1_ImageChanged);
            // 
            // mTextTabPage
            // 
            this.mTextTabPage.Controls.Add(this.mTextEditControl1);
            this.mTextTabPage.Location = new System.Drawing.Point(4, 22);
            this.mTextTabPage.Name = "mTextTabPage";
            this.mTextTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mTextTabPage.Size = new System.Drawing.Size(441, 326);
            this.mTextTabPage.TabIndex = 2;
            this.mTextTabPage.Text = "Text";
            this.mTextTabPage.UseVisualStyleBackColor = true;
            // 
            // mTextEditControl1
            // 
            this.mTextEditControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTextEditControl1.Location = new System.Drawing.Point(3, 0);
            this.mTextEditControl1.Name = "mTextEditControl1";
            this.mTextEditControl1.Size = new System.Drawing.Size(438, 312);
            this.mTextEditControl1.TabIndex = 0;
            // 
            // mAlphaMapTabPage
            // 
            this.mAlphaMapTabPage.Controls.Add(this.mSelectAlphaMapControl1);
            this.mAlphaMapTabPage.Location = new System.Drawing.Point(4, 22);
            this.mAlphaMapTabPage.Name = "mAlphaMapTabPage";
            this.mAlphaMapTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mAlphaMapTabPage.Size = new System.Drawing.Size(441, 326);
            this.mAlphaMapTabPage.TabIndex = 4;
            this.mAlphaMapTabPage.Text = "Outline mask";
            this.mAlphaMapTabPage.UseVisualStyleBackColor = true;
            // 
            // mSelectAlphaMapControl1
            // 
            this.mSelectAlphaMapControl1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSelectAlphaMapControl1.Location = new System.Drawing.Point(3, 6);
            this.mSelectAlphaMapControl1.Name = "mSelectAlphaMapControl1";
            this.mSelectAlphaMapControl1.Size = new System.Drawing.Size(435, 314);
            this.mSelectAlphaMapControl1.TabIndex = 0;
            this.mSelectAlphaMapControl1.AlphaMapSelected += new CustomButton.SelectAlphaMapControl.AlphaMapSelectedCallbackDelegate(this.mSelectAlphaMapControl1_AlphaMapSelected);
            // 
            // mDoneButton
            // 
            this.mDoneButton.Location = new System.Drawing.Point(874, 505);
            this.mDoneButton.Name = "mDoneButton";
            this.mDoneButton.Size = new System.Drawing.Size(75, 23);
            this.mDoneButton.TabIndex = 4;
            this.mDoneButton.Text = "Done";
            this.mDoneButton.UseVisualStyleBackColor = true;
            this.mDoneButton.Click += new System.EventHandler(this.mDoneButton_Click);
            // 
            // mResetButton
            // 
            this.mResetButton.Location = new System.Drawing.Point(500, 505);
            this.mResetButton.Name = "mResetButton";
            this.mResetButton.Size = new System.Drawing.Size(127, 23);
            this.mResetButton.TabIndex = 5;
            this.mResetButton.TabStop = false;
            this.mResetButton.Text = "Reset all settings";
            this.mToolTip.SetToolTip(this.mResetButton, "Resets all the current settings to their default values");
            this.mResetButton.UseVisualStyleBackColor = true;
            this.mResetButton.Click += new System.EventHandler(this.mResetButton_Click);
            // 
            // mPreviewPanel
            // 
            this.mPreviewPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mPreviewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewPanel.Location = new System.Drawing.Point(753, 4);
            this.mPreviewPanel.Name = "mPreviewPanel";
            this.mPreviewPanel.Size = new System.Drawing.Size(196, 110);
            this.mPreviewPanel.TabIndex = 11;
            // 
            // mSelectmageButton
            // 
            this.mSelectmageButton.Location = new System.Drawing.Point(9, 502);
            this.mSelectmageButton.Name = "mSelectmageButton";
            this.mSelectmageButton.Size = new System.Drawing.Size(75, 30);
            this.mSelectmageButton.TabIndex = 12;
            this.mSelectmageButton.TabStop = false;
            this.mSelectmageButton.Text = "Browse...";
            this.mToolTip.SetToolTip(this.mSelectmageButton, "Replaces the current image or video with another image or video\r\nstored on the lo" +
        "cal machine");
            this.mSelectmageButton.UseVisualStyleBackColor = true;
            this.mSelectmageButton.Click += new System.EventHandler(this.mSelectmageButton_Click);
            // 
            // mFilenameTextBox
            // 
            this.mFilenameTextBox.BackColor = System.Drawing.Color.White;
            this.mFilenameTextBox.Location = new System.Drawing.Point(86, 506);
            this.mFilenameTextBox.Name = "mFilenameTextBox";
            this.mFilenameTextBox.ReadOnly = true;
            this.mFilenameTextBox.Size = new System.Drawing.Size(393, 22);
            this.mFilenameTextBox.TabIndex = 14;
            this.mFilenameTextBox.TabStop = false;
            // 
            // mPreviousSlideButton
            // 
            this.mPreviousSlideButton.Location = new System.Drawing.Point(720, 502);
            this.mPreviousSlideButton.Name = "mPreviousSlideButton";
            this.mPreviousSlideButton.Size = new System.Drawing.Size(89, 23);
            this.mPreviousSlideButton.TabIndex = 15;
            this.mPreviousSlideButton.TabStop = false;
            this.mPreviousSlideButton.Text = "Previous Slide";
            this.mPreviousSlideButton.UseVisualStyleBackColor = true;
            this.mPreviousSlideButton.Visible = false;
            this.mPreviousSlideButton.Click += new System.EventHandler(this.mPreviousSlideButton_Click);
            // 
            // mNextSlideButton
            // 
            this.mNextSlideButton.Location = new System.Drawing.Point(793, 502);
            this.mNextSlideButton.Name = "mNextSlideButton";
            this.mNextSlideButton.Size = new System.Drawing.Size(75, 23);
            this.mNextSlideButton.TabIndex = 16;
            this.mNextSlideButton.TabStop = false;
            this.mNextSlideButton.Text = "Next Slide";
            this.mNextSlideButton.UseVisualStyleBackColor = true;
            this.mNextSlideButton.Visible = false;
            this.mNextSlideButton.Click += new System.EventHandler(this.mNextSlideButton_Click);
            // 
            // mTextPBPanel
            // 
            this.mTextPBPanel.AllowDrop = true;
            this.mTextPBPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mTextPBPanel.Location = new System.Drawing.Point(672, 506);
            this.mTextPBPanel.Name = "mTextPBPanel";
            this.mTextPBPanel.Size = new System.Drawing.Size(20, 26);
            this.mTextPBPanel.TabIndex = 17;
            // 
            // mTextPBPanelDarkBackGround
            // 
            this.mTextPBPanelDarkBackGround.AllowDrop = true;
            this.mTextPBPanelDarkBackGround.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mTextPBPanelDarkBackGround.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTextPBPanelDarkBackGround.Location = new System.Drawing.Point(646, 506);
            this.mTextPBPanelDarkBackGround.Name = "mTextPBPanelDarkBackGround";
            this.mTextPBPanelDarkBackGround.Size = new System.Drawing.Size(20, 26);
            this.mTextPBPanelDarkBackGround.TabIndex = 18;
            // 
            // mEncryptFile
            // 
            this.mEncryptFile.ForeColor = System.Drawing.Color.Fuchsia;
            this.mEncryptFile.Location = new System.Drawing.Point(874, 120);
            this.mEncryptFile.Name = "mEncryptFile";
            this.mEncryptFile.Size = new System.Drawing.Size(33, 23);
            this.mEncryptFile.TabIndex = 19;
            this.mEncryptFile.TabStop = false;
            this.mEncryptFile.Text = "Enc";
            this.mEncryptFile.UseVisualStyleBackColor = true;
            this.mEncryptFile.Visible = false;
            this.mEncryptFile.Click += new System.EventHandler(this.mEncryptFile_Click);
            // 
            // mEditTemplateMediaOrder1
            // 
            this.mEditTemplateMediaOrder1.Location = new System.Drawing.Point(4, 4);
            this.mEditTemplateMediaOrder1.Margin = new System.Windows.Forms.Padding(4);
            this.mEditTemplateMediaOrder1.Name = "mEditTemplateMediaOrder1";
            this.mEditTemplateMediaOrder1.Size = new System.Drawing.Size(750, 127);
            this.mEditTemplateMediaOrder1.TabIndex = 10;
            this.mEditTemplateMediaOrder1.TabStop = false;
            this.mEditTemplateMediaOrder1.MediaSelectedIndexChanged += new CustomButton.EditTemplateMediaOrder.MediaSelectedDelegate(this.mEditTemplateMediaOrder1_MediaSelectedIndexChanged);
            this.mEditTemplateMediaOrder1.MediaSourceChangeRequest += new CustomButton.EditTemplateMediaOrder.MediaSelectedDelegate(this.mEditTemplateMediaOrder1_MediaSourceChangeRequest);
            // 
            // mShowPreviewCheckBox
            // 
            this.mShowPreviewCheckBox.AutoSize = true;
            this.mShowPreviewCheckBox.Location = new System.Drawing.Point(753, 118);
            this.mShowPreviewCheckBox.Name = "mShowPreviewCheckBox";
            this.mShowPreviewCheckBox.Size = new System.Drawing.Size(92, 17);
            this.mShowPreviewCheckBox.TabIndex = 20;
            this.mShowPreviewCheckBox.Text = "Preview slide";
            this.mShowPreviewCheckBox.UseVisualStyleBackColor = true;
            this.mShowPreviewCheckBox.CheckedChanged += new System.EventHandler(this.mShowPreviewCheckBox_CheckedChanged);
            // 
            // EditSlideMediaForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(957, 537);
            this.Controls.Add(this.mShowPreviewCheckBox);
            this.Controls.Add(this.mEncryptFile);
            this.Controls.Add(this.mPreviewPictureBox);
            this.Controls.Add(this.mNextSlideButton);
            this.Controls.Add(this.mPreviousSlideButton);
            this.Controls.Add(this.mFilenameTextBox);
            this.Controls.Add(this.mSelectmageButton);
            this.Controls.Add(this.mPreviewPanel);
            this.Controls.Add(this.mResetButton);
            this.Controls.Add(this.mDoneButton);
            this.Controls.Add(this.mControlTabs);
            this.Controls.Add(this.mTextPBPanel);
            this.Controls.Add(this.mTextPBPanelDarkBackGround);
            this.Controls.Add(this.mEditTemplateMediaOrder1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditSlideMediaForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit slide input media";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditSlideMediaForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewPictureBox)).EndInit();
            this.mControlTabs.ResumeLayout(false);
            this.mAdjustTab.ResumeLayout(false);
            this.mCropTabPage.ResumeLayout(false);
            this.mTrimVideoTabPage.ResumeLayout(false);
            this.mOrientationTab.ResumeLayout(false);
            this.mTextTabPage.ResumeLayout(false);
            this.mAlphaMapTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox mPreviewPictureBox;
        private System.Windows.Forms.TabControl mControlTabs;
        private System.Windows.Forms.TabPage mAdjustTab;
        private System.Windows.Forms.TabPage mCropTabPage;
        private System.Windows.Forms.TabPage mTextTabPage;
        private System.Windows.Forms.Button mDoneButton;
        private System.Windows.Forms.Button mResetButton;
        private System.Windows.Forms.TabPage mTrimVideoTabPage;
        private System.Windows.Forms.Button mTrimVideoDoneButton;
        private System.Windows.Forms.TabPage mAlphaMapTabPage;
        private EditTemplateMediaOrder mEditTemplateMediaOrder1;
        private System.Windows.Forms.Panel mPreviewPanel;
        private AdjustImageColoursControl mAdjustImageColoursControl1;
        private SelectAlphaMapControl mSelectAlphaMapControl1;
        private System.Windows.Forms.Button mSelectmageButton;
        private System.Windows.Forms.TextBox mFilenameTextBox;
        private CropUserControl mCropUserControl1;
        private System.Windows.Forms.TabPage mOrientationTab;
        private OrientateControl mOrientateControl1;
        private TrimVideoControl mTrimVideoControl1;
        private System.Windows.Forms.Button mPreviousSlideButton;
        private System.Windows.Forms.Button mNextSlideButton;
        private TextEditControl mTextEditControl1;
        private System.Windows.Forms.Panel mTextPBPanel;
        private System.Windows.Forms.Panel mTextPBPanelDarkBackGround;
        private System.Windows.Forms.Button mEncryptFile;
        private System.Windows.Forms.ToolTip mToolTip;
        private System.Windows.Forms.CheckBox mShowPreviewCheckBox;
    }
}