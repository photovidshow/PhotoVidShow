namespace CustomButton
{
    partial class SlideFiltersControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlideFiltersControl));
            this.label1 = new System.Windows.Forms.Label();
            this.mAddFilterButton = new System.Windows.Forms.Button();
            this.mRemoveFilterButton = new System.Windows.Forms.Button();
            this.mOrderLayersButton = new System.Windows.Forms.Button();
            this.mFiltersListView = new System.Windows.Forms.ListView();
            this.mFilterImageList = new System.Windows.Forms.ImageList(this.components);
            this.mCurrentFiltersListView = new System.Windows.Forms.ListView();
            this.mBlurSoftRadio = new System.Windows.Forms.RadioButton();
            this.mBlurMediumRadio = new System.Windows.Forms.RadioButton();
            this.mBlurHardRadio = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.mBrightnessContrastControl = new CustomButton.BrightnessContrastControll();
            this.mBlurPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mLensBlurRadioButton = new System.Windows.Forms.RadioButton();
            this.mNormalBlurRadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.mDownFilterOrderButton = new System.Windows.Forms.Button();
            this.mUpFilterOrderButton = new System.Windows.Forms.Button();
            this.mPanZoomOnAllCheckBox = new System.Windows.Forms.CheckBox();
            this.mClearSelectedButton = new System.Windows.Forms.Button();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mBlurPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(340, 264);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Currrent filters";
            // 
            // mAddFilterButton
            // 
            this.mAddFilterButton.Location = new System.Drawing.Point(339, 123);
            this.mAddFilterButton.Name = "mAddFilterButton";
            this.mAddFilterButton.Size = new System.Drawing.Size(103, 23);
            this.mAddFilterButton.TabIndex = 5;
            this.mAddFilterButton.TabStop = false;
            this.mAddFilterButton.Text = "Add filter";
            this.mToolTip.SetToolTip(this.mAddFilterButton, "Adds the selected filter to the slide");
            this.mAddFilterButton.UseVisualStyleBackColor = true;
            this.mAddFilterButton.Click += new System.EventHandler(this.mAddFilterButton_Click);
            // 
            // mRemoveFilterButton
            // 
            this.mRemoveFilterButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mRemoveFilterButton.Location = new System.Drawing.Point(339, 311);
            this.mRemoveFilterButton.Name = "mRemoveFilterButton";
            this.mRemoveFilterButton.Size = new System.Drawing.Size(103, 23);
            this.mRemoveFilterButton.TabIndex = 6;
            this.mRemoveFilterButton.TabStop = false;
            this.mRemoveFilterButton.Text = "Remove filter";
            this.mToolTip.SetToolTip(this.mRemoveFilterButton, "Removes the currently selected filter");
            this.mRemoveFilterButton.UseVisualStyleBackColor = true;
            this.mRemoveFilterButton.Click += new System.EventHandler(this.mRemoveFilterButton_Click);
            // 
            // mOrderLayersButton
            // 
            this.mOrderLayersButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mOrderLayersButton.Location = new System.Drawing.Point(339, 340);
            this.mOrderLayersButton.Name = "mOrderLayersButton";
            this.mOrderLayersButton.Size = new System.Drawing.Size(103, 23);
            this.mOrderLayersButton.TabIndex = 7;
            this.mOrderLayersButton.TabStop = false;
            this.mOrderLayersButton.Text = "Order layers";
            this.mToolTip.SetToolTip(this.mOrderLayersButton, "Allows you to customize the order the slide decorations are drawn in");
            this.mOrderLayersButton.UseVisualStyleBackColor = true;
            this.mOrderLayersButton.Click += new System.EventHandler(this.mOrderLayersButton_Click);
            // 
            // mFiltersListView
            // 
            this.mFiltersListView.HideSelection = false;
            this.mFiltersListView.LargeImageList = this.mFilterImageList;
            this.mFiltersListView.Location = new System.Drawing.Point(3, 3);
            this.mFiltersListView.MultiSelect = false;
            this.mFiltersListView.Name = "mFiltersListView";
            this.mFiltersListView.Size = new System.Drawing.Size(330, 143);
            this.mFiltersListView.TabIndex = 8;
            this.mFiltersListView.TabStop = false;
            this.mFiltersListView.UseCompatibleStateImageBehavior = false;
            this.mFiltersListView.SelectedIndexChanged += new System.EventHandler(this.mFiltersListView_SelectedIndexChanged);
            this.mFiltersListView.Enter += new System.EventHandler(this.mFiltersListView_Enter);
            // 
            // mFilterImageList
            // 
            this.mFilterImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mFilterImageList.ImageStream")));
            this.mFilterImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mFilterImageList.Images.SetKeyName(0, "star.png");
            this.mFilterImageList.Images.SetKeyName(1, "video2-small.png");
            this.mFilterImageList.Images.SetKeyName(2, "imagefilter.png");
            // 
            // mCurrentFiltersListView
            // 
            this.mCurrentFiltersListView.HideSelection = false;
            this.mCurrentFiltersListView.LargeImageList = this.mFilterImageList;
            this.mCurrentFiltersListView.Location = new System.Drawing.Point(3, 267);
            this.mCurrentFiltersListView.MultiSelect = false;
            this.mCurrentFiltersListView.Name = "mCurrentFiltersListView";
            this.mCurrentFiltersListView.Size = new System.Drawing.Size(330, 96);
            this.mCurrentFiltersListView.TabIndex = 9;
            this.mCurrentFiltersListView.TabStop = false;
            this.mToolTip.SetToolTip(this.mCurrentFiltersListView, "Current slide filters");
            this.mCurrentFiltersListView.UseCompatibleStateImageBehavior = false;
            this.mCurrentFiltersListView.SelectedIndexChanged += new System.EventHandler(this.mCurrentFiltersListView_SelectedIndexChanged);
            this.mCurrentFiltersListView.Enter += new System.EventHandler(this.mCurrentFiltersListView_Enter);
            // 
            // mBlurSoftRadio
            // 
            this.mBlurSoftRadio.AutoSize = true;
            this.mBlurSoftRadio.Location = new System.Drawing.Point(3, 21);
            this.mBlurSoftRadio.Name = "mBlurSoftRadio";
            this.mBlurSoftRadio.Size = new System.Drawing.Size(46, 17);
            this.mBlurSoftRadio.TabIndex = 18;
            this.mBlurSoftRadio.Text = "Soft";
            this.mBlurSoftRadio.UseVisualStyleBackColor = true;
            this.mBlurSoftRadio.CheckedChanged += new System.EventHandler(this.mBlurSoftRadio_CheckedChanged);
            // 
            // mBlurMediumRadio
            // 
            this.mBlurMediumRadio.AutoSize = true;
            this.mBlurMediumRadio.Checked = true;
            this.mBlurMediumRadio.Location = new System.Drawing.Point(3, 44);
            this.mBlurMediumRadio.Name = "mBlurMediumRadio";
            this.mBlurMediumRadio.Size = new System.Drawing.Size(67, 17);
            this.mBlurMediumRadio.TabIndex = 19;
            this.mBlurMediumRadio.TabStop = true;
            this.mBlurMediumRadio.Text = "Medium";
            this.mBlurMediumRadio.UseVisualStyleBackColor = true;
            this.mBlurMediumRadio.CheckedChanged += new System.EventHandler(this.mBlurMediumRadio_CheckedChanged);
            // 
            // mBlurHardRadio
            // 
            this.mBlurHardRadio.AutoSize = true;
            this.mBlurHardRadio.Location = new System.Drawing.Point(3, 67);
            this.mBlurHardRadio.Name = "mBlurHardRadio";
            this.mBlurHardRadio.Size = new System.Drawing.Size(50, 17);
            this.mBlurHardRadio.TabIndex = 20;
            this.mBlurHardRadio.Text = "Hard";
            this.mBlurHardRadio.UseVisualStyleBackColor = true;
            this.mBlurHardRadio.CheckedChanged += new System.EventHandler(this.mBlurHardRadio_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(340, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Available filters";
            // 
            // mBrightnessContrastControl
            // 
            this.mBrightnessContrastControl.BackColor = System.Drawing.Color.Transparent;
            this.mBrightnessContrastControl.BrightnessB = 1F;
            this.mBrightnessContrastControl.BrightnessG = 1F;
            this.mBrightnessContrastControl.BrightnessR = 1F;
            this.mBrightnessContrastControl.ContrastB = 0F;
            this.mBrightnessContrastControl.ContrastG = 0F;
            this.mBrightnessContrastControl.ContrastR = 0F;
            this.mBrightnessContrastControl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mBrightnessContrastControl.Location = new System.Drawing.Point(4, 153);
            this.mBrightnessContrastControl.Name = "mBrightnessContrastControl";
            this.mBrightnessContrastControl.Size = new System.Drawing.Size(394, 108);
            this.mBrightnessContrastControl.TabIndex = 26;
            this.mBrightnessContrastControl.TabStop = false;
            this.mBrightnessContrastControl.BrightnessChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_BrightnessChanged);
            this.mBrightnessContrastControl.BrightnessRChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_BrightnessRChanged);
            this.mBrightnessContrastControl.BrightnessGChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_BrightnessGChanged);
            this.mBrightnessContrastControl.BrightnessBChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_BrightnessBChanged);
            this.mBrightnessContrastControl.ContrastChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_ContrastChanged);
            this.mBrightnessContrastControl.ContrastRChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_ContrastRChanged);
            this.mBrightnessContrastControl.ContrastGChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_ContrastGChanged);
            this.mBrightnessContrastControl.ContrastBChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_ContrastBChanged);
            this.mBrightnessContrastControl.FinishedBrightnessContrastChange += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControl_FinishedBrightnessContrastChange);
            // 
            // mBlurPanel
            // 
            this.mBlurPanel.BackColor = System.Drawing.Color.Transparent;
            this.mBlurPanel.Controls.Add(this.panel1);
            this.mBlurPanel.Controls.Add(this.mBlurSoftRadio);
            this.mBlurPanel.Controls.Add(this.mBlurMediumRadio);
            this.mBlurPanel.Controls.Add(this.mBlurHardRadio);
            this.mBlurPanel.Controls.Add(this.label2);
            this.mBlurPanel.Location = new System.Drawing.Point(13, 406);
            this.mBlurPanel.Name = "mBlurPanel";
            this.mBlurPanel.Size = new System.Drawing.Size(307, 86);
            this.mBlurPanel.TabIndex = 27;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mLensBlurRadioButton);
            this.panel1.Controls.Add(this.mNormalBlurRadioButton);
            this.panel1.Location = new System.Drawing.Point(121, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(142, 79);
            this.panel1.TabIndex = 23;
            // 
            // mLensBlurRadioButton
            // 
            this.mLensBlurRadioButton.AutoSize = true;
            this.mLensBlurRadioButton.Location = new System.Drawing.Point(3, 41);
            this.mLensBlurRadioButton.Name = "mLensBlurRadioButton";
            this.mLensBlurRadioButton.Size = new System.Drawing.Size(72, 17);
            this.mLensBlurRadioButton.TabIndex = 23;
            this.mLensBlurRadioButton.Text = "Lens blur";
            this.mLensBlurRadioButton.UseVisualStyleBackColor = true;
            this.mLensBlurRadioButton.CheckedChanged += new System.EventHandler(this.mLensBlurRadioButton_CheckedChanged);
            // 
            // mNormalBlurRadioButton
            // 
            this.mNormalBlurRadioButton.AutoSize = true;
            this.mNormalBlurRadioButton.Checked = true;
            this.mNormalBlurRadioButton.Location = new System.Drawing.Point(3, 18);
            this.mNormalBlurRadioButton.Name = "mNormalBlurRadioButton";
            this.mNormalBlurRadioButton.Size = new System.Drawing.Size(86, 17);
            this.mNormalBlurRadioButton.TabIndex = 22;
            this.mNormalBlurRadioButton.TabStop = true;
            this.mNormalBlurRadioButton.Text = "Normal blur";
            this.mNormalBlurRadioButton.UseVisualStyleBackColor = true;
            this.mNormalBlurRadioButton.CheckedChanged += new System.EventHandler(this.mNormalBlurRadioButton_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Blur";
            // 
            // mDownFilterOrderButton
            // 
            this.mDownFilterOrderButton.BackgroundImage = global::CustomButton.Properties.Resources.redo1;
            this.mDownFilterOrderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mDownFilterOrderButton.Location = new System.Drawing.Point(377, 283);
            this.mDownFilterOrderButton.Margin = new System.Windows.Forms.Padding(0);
            this.mDownFilterOrderButton.Name = "mDownFilterOrderButton";
            this.mDownFilterOrderButton.Size = new System.Drawing.Size(39, 23);
            this.mDownFilterOrderButton.TabIndex = 25;
            this.mDownFilterOrderButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mDownFilterOrderButton, "Moves the current filter down in the drawn order");
            this.mDownFilterOrderButton.UseVisualStyleBackColor = true;
            this.mDownFilterOrderButton.Click += new System.EventHandler(this.mDownFilterOrderButton_Click);
            // 
            // mUpFilterOrderButton
            // 
            this.mUpFilterOrderButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mUpFilterOrderButton.BackgroundImage = global::CustomButton.Properties.Resources.undo1;
            this.mUpFilterOrderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mUpFilterOrderButton.Location = new System.Drawing.Point(339, 283);
            this.mUpFilterOrderButton.Margin = new System.Windows.Forms.Padding(0);
            this.mUpFilterOrderButton.Name = "mUpFilterOrderButton";
            this.mUpFilterOrderButton.Size = new System.Drawing.Size(38, 23);
            this.mUpFilterOrderButton.TabIndex = 24;
            this.mUpFilterOrderButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mUpFilterOrderButton, "Moves the current filter up in the drawn order");
            this.mUpFilterOrderButton.UseVisualStyleBackColor = true;
            this.mUpFilterOrderButton.Click += new System.EventHandler(this.mUpFilterOrderButton_Click);
            // 
            // mPanZoomOnAllCheckBox
            // 
            this.mPanZoomOnAllCheckBox.AutoSize = true;
            this.mPanZoomOnAllCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mPanZoomOnAllCheckBox.Location = new System.Drawing.Point(13, 383);
            this.mPanZoomOnAllCheckBox.Name = "mPanZoomOnAllCheckBox";
            this.mPanZoomOnAllCheckBox.Size = new System.Drawing.Size(109, 17);
            this.mPanZoomOnAllCheckBox.TabIndex = 28;
            this.mPanZoomOnAllCheckBox.TabStop = false;
            this.mPanZoomOnAllCheckBox.Text = "Pan/zoom on all";
            this.mPanZoomOnAllCheckBox.UseVisualStyleBackColor = false;
            this.mPanZoomOnAllCheckBox.Visible = false;
            this.mPanZoomOnAllCheckBox.CheckedChanged += new System.EventHandler(this.mPanZoomOnAllCheckBox_CheckedChanged);
            // 
            // mClearSelectedButton
            // 
            this.mClearSelectedButton.Location = new System.Drawing.Point(339, 94);
            this.mClearSelectedButton.Name = "mClearSelectedButton";
            this.mClearSelectedButton.Size = new System.Drawing.Size(103, 23);
            this.mClearSelectedButton.TabIndex = 29;
            this.mClearSelectedButton.TabStop = false;
            this.mClearSelectedButton.Text = "Clear preview";
            this.mToolTip.SetToolTip(this.mClearSelectedButton, "Clears the current preview filter");
            this.mClearSelectedButton.UseVisualStyleBackColor = true;
            this.mClearSelectedButton.Click += new System.EventHandler(this.ClearSelectedButton_Click);
            // 
            // SlideFiltersControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.mClearSelectedButton);
            this.Controls.Add(this.mPanZoomOnAllCheckBox);
            this.Controls.Add(this.mBlurPanel);
            this.Controls.Add(this.mBrightnessContrastControl);
            this.Controls.Add(this.mDownFilterOrderButton);
            this.Controls.Add(this.mUpFilterOrderButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mCurrentFiltersListView);
            this.Controls.Add(this.mFiltersListView);
            this.Controls.Add(this.mOrderLayersButton);
            this.Controls.Add(this.mRemoveFilterButton);
            this.Controls.Add(this.mAddFilterButton);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SlideFiltersControl";
            this.Size = new System.Drawing.Size(445, 495);
            this.mBlurPanel.ResumeLayout(false);
            this.mBlurPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mAddFilterButton;
        private System.Windows.Forms.Button mRemoveFilterButton;
        private System.Windows.Forms.Button mOrderLayersButton;
        private System.Windows.Forms.ListView mFiltersListView;
        private System.Windows.Forms.ListView mCurrentFiltersListView;
        private System.Windows.Forms.ImageList mFilterImageList;
        private System.Windows.Forms.RadioButton mBlurSoftRadio;
        private System.Windows.Forms.RadioButton mBlurMediumRadio;
        private System.Windows.Forms.RadioButton mBlurHardRadio;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button mUpFilterOrderButton;
        private System.Windows.Forms.Button mDownFilterOrderButton;
        private BrightnessContrastControll mBrightnessContrastControl;
        private System.Windows.Forms.Panel mBlurPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton mLensBlurRadioButton;
        private System.Windows.Forms.RadioButton mNormalBlurRadioButton;
        private System.Windows.Forms.CheckBox mPanZoomOnAllCheckBox;
        private System.Windows.Forms.Button mClearSelectedButton;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
