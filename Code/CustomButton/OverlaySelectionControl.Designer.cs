namespace CustomButton
{
    partial class OverlaySelectionControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverlaySelectionControl));
            this.mBordersListView = new System.Windows.Forms.ListView();
            this.mBorderImageList = new System.Windows.Forms.ImageList(this.components);
            this.mShowBorderAfterTransitionEffectCheckBox = new System.Windows.Forms.CheckBox();
            this.mClearButton = new System.Windows.Forms.Button();
            this.mGenerateImages = new System.Windows.Forms.Button();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mBordersListView
            // 
            this.mBordersListView.LargeImageList = this.mBorderImageList;
            this.mBordersListView.Location = new System.Drawing.Point(3, 3);
            this.mBordersListView.MultiSelect = false;
            this.mBordersListView.Name = "mBordersListView";
            this.mBordersListView.Size = new System.Drawing.Size(426, 318);
            this.mBordersListView.TabIndex = 0;
            this.mBordersListView.TabStop = false;
            this.mBordersListView.UseCompatibleStateImageBehavior = false;
            this.mBordersListView.SelectedIndexChanged += new System.EventHandler(this.mBordersListView_SelectedIndexChanged);
            // 
            // mBorderImageList
            // 
            this.mBorderImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mBorderImageList.ImageStream")));
            this.mBorderImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mBorderImageList.Images.SetKeyName(0, "Paint_brush.png");
            // 
            // mShowBorderAfterTransitionEffectCheckBox
            // 
            this.mShowBorderAfterTransitionEffectCheckBox.AutoSize = true;
            this.mShowBorderAfterTransitionEffectCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.mShowBorderAfterTransitionEffectCheckBox.Location = new System.Drawing.Point(3, 327);
            this.mShowBorderAfterTransitionEffectCheckBox.Name = "mShowBorderAfterTransitionEffectCheckBox";
            this.mShowBorderAfterTransitionEffectCheckBox.Size = new System.Drawing.Size(208, 17);
            this.mShowBorderAfterTransitionEffectCheckBox.TabIndex = 1;
            this.mShowBorderAfterTransitionEffectCheckBox.TabStop = false;
            this.mShowBorderAfterTransitionEffectCheckBox.Text = "Show border after transition effect ";
            this.mShowBorderAfterTransitionEffectCheckBox.UseVisualStyleBackColor = false;
            this.mShowBorderAfterTransitionEffectCheckBox.CheckedChanged += new System.EventHandler(this.mShowBorderAfterTransitionEffectCheckBox_CheckedChanged);
            // 
            // mClearButton
            // 
            this.mClearButton.Location = new System.Drawing.Point(346, 327);
            this.mClearButton.Name = "mClearButton";
            this.mClearButton.Size = new System.Drawing.Size(83, 23);
            this.mClearButton.TabIndex = 2;
            this.mClearButton.TabStop = false;
            this.mClearButton.Text = "Clear";
            this.mToolTip.SetToolTip(this.mClearButton, "Removes the current border effect");
            this.mClearButton.UseVisualStyleBackColor = true;
            this.mClearButton.Click += new System.EventHandler(this.mClearButton_Click);
            // 
            // mGenerateImages
            // 
            this.mGenerateImages.ForeColor = System.Drawing.Color.Magenta;
            this.mGenerateImages.Location = new System.Drawing.Point(217, 327);
            this.mGenerateImages.Name = "mGenerateImages";
            this.mGenerateImages.Size = new System.Drawing.Size(72, 34);
            this.mGenerateImages.TabIndex = 3;
            this.mGenerateImages.TabStop = false;
            this.mGenerateImages.Text = "Generate images";
            this.mGenerateImages.UseVisualStyleBackColor = true;
            this.mGenerateImages.Click += new System.EventHandler(this.mGenerateImages_Click);
            // 
            // OverlaySelectionControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::CustomButton.Properties.Resources.back24;
            this.Controls.Add(this.mGenerateImages);
            this.Controls.Add(this.mClearButton);
            this.Controls.Add(this.mShowBorderAfterTransitionEffectCheckBox);
            this.Controls.Add(this.mBordersListView);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OverlaySelectionControl";
            this.Size = new System.Drawing.Size(435, 373);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView mBordersListView;
        private System.Windows.Forms.CheckBox mShowBorderAfterTransitionEffectCheckBox;
        private System.Windows.Forms.Button mClearButton;
        private System.Windows.Forms.ImageList mBorderImageList;
        private System.Windows.Forms.Button mGenerateImages;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
