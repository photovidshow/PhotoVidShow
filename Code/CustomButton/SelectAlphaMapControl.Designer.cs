namespace CustomButton
{
    partial class SelectAlphaMapControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectAlphaMapControl));
            this.mAlphaMapsListView = new System.Windows.Forms.ListView();
            this.mAlphaMapImageList = new System.Windows.Forms.ImageList(this.components);
            this.mClearButton = new System.Windows.Forms.Button();
            this.mGenerateImages = new System.Windows.Forms.Button();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mAlphaMapsListView
            // 
            this.mAlphaMapsListView.HideSelection = false;
            this.mAlphaMapsListView.LargeImageList = this.mAlphaMapImageList;
            this.mAlphaMapsListView.Location = new System.Drawing.Point(3, 3);
            this.mAlphaMapsListView.MultiSelect = false;
            this.mAlphaMapsListView.Name = "mAlphaMapsListView";
            this.mAlphaMapsListView.Size = new System.Drawing.Size(429, 276);
            this.mAlphaMapsListView.TabIndex = 0;
            this.mAlphaMapsListView.TabStop = false;
            this.mAlphaMapsListView.UseCompatibleStateImageBehavior = false;
            this.mAlphaMapsListView.VisibleChanged += new System.EventHandler(this.mAlphaMapsListView_VisibleChanged);
            this.mAlphaMapsListView.SelectedIndexChanged += new System.EventHandler(this.mAlphaMapsListView_SelectedIndexChanged);
            // 
            // mAlphaMapImageList
            // 
            this.mAlphaMapImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mAlphaMapImageList.ImageStream")));
            this.mAlphaMapImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mAlphaMapImageList.Images.SetKeyName(0, "playbutton4.png");
            // 
            // mClearButton
            // 
            this.mClearButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mClearButton.Location = new System.Drawing.Point(3, 285);
            this.mClearButton.Name = "mClearButton";
            this.mClearButton.Size = new System.Drawing.Size(140, 23);
            this.mClearButton.TabIndex = 1;
            this.mClearButton.TabStop = false;
            this.mClearButton.Text = "Reset mask settings";
            this.mToolTip.SetToolTip(this.mClearButton, "Resets the mask settings back to the default value");
            this.mClearButton.UseVisualStyleBackColor = true;
            this.mClearButton.Click += new System.EventHandler(this.mClearButton_Click);
            // 
            // mGenerateImages
            // 
            this.mGenerateImages.ForeColor = System.Drawing.Color.Magenta;
            this.mGenerateImages.Location = new System.Drawing.Point(318, 285);
            this.mGenerateImages.Name = "mGenerateImages";
            this.mGenerateImages.Size = new System.Drawing.Size(108, 23);
            this.mGenerateImages.TabIndex = 2;
            this.mGenerateImages.TabStop = false;
            this.mGenerateImages.Text = "Generate images";
            this.mGenerateImages.UseVisualStyleBackColor = true;
            this.mGenerateImages.Click += new System.EventHandler(this.mGenerateImages_Click);
            // 
            // SelectAlphaMapControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.mGenerateImages);
            this.Controls.Add(this.mClearButton);
            this.Controls.Add(this.mAlphaMapsListView);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SelectAlphaMapControl";
            this.Size = new System.Drawing.Size(435, 322);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mAlphaMapsListView;
        private System.Windows.Forms.Button mClearButton;
        private System.Windows.Forms.ImageList mAlphaMapImageList;
        private System.Windows.Forms.Button mGenerateImages;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
