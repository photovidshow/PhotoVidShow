namespace CustomButton
{
    partial class SelectBackgroundControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectBackgroundControl));
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.mClearButton = new System.Windows.Forms.Button();
            this.mApplySettingToAllSlides = new System.Windows.Forms.Button();
            this.mBackgroundsListView = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.mSetAsColourButton = new System.Windows.Forms.Button();
            this.mBackgroundColourButton = new System.Windows.Forms.Button();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mBrowseButton
            // 
            this.mBrowseButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mBrowseButton.Location = new System.Drawing.Point(314, 46);
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.Size = new System.Drawing.Size(74, 23);
            this.mBrowseButton.TabIndex = 7;
            this.mBrowseButton.TabStop = false;
            this.mBrowseButton.Text = "Browse...";
            this.mToolTip.SetToolTip(this.mBrowseButton, "Browse for a background image or video");
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.mBrowseButton_Click);
            // 
            // mClearButton
            // 
            this.mClearButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mClearButton.Location = new System.Drawing.Point(314, 75);
            this.mClearButton.Name = "mClearButton";
            this.mClearButton.Size = new System.Drawing.Size(74, 23);
            this.mClearButton.TabIndex = 6;
            this.mClearButton.TabStop = false;
            this.mClearButton.Text = "Clear";
            this.mToolTip.SetToolTip(this.mClearButton, "Clears the current background back to black");
            this.mClearButton.UseVisualStyleBackColor = true;
            this.mClearButton.Click += new System.EventHandler(this.mClearButton_Click);
            // 
            // mApplySettingToAllSlides
            // 
            this.mApplySettingToAllSlides.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mApplySettingToAllSlides.Location = new System.Drawing.Point(314, 313);
            this.mApplySettingToAllSlides.Name = "mApplySettingToAllSlides";
            this.mApplySettingToAllSlides.Size = new System.Drawing.Size(74, 35);
            this.mApplySettingToAllSlides.TabIndex = 5;
            this.mApplySettingToAllSlides.TabStop = false;
            this.mApplySettingToAllSlides.Text = "Apply to all slides";
            this.mToolTip.SetToolTip(this.mApplySettingToAllSlides, "Applies the current background to all the slides in the slideshow");
            this.mApplySettingToAllSlides.UseVisualStyleBackColor = true;
            this.mApplySettingToAllSlides.Click += new System.EventHandler(this.mApplySettingToAllSlides_Click);
            // 
            // mBackgroundsListView
            // 
            this.mBackgroundsListView.LargeImageList = this.imageList1;
            this.mBackgroundsListView.Location = new System.Drawing.Point(3, 3);
            this.mBackgroundsListView.MultiSelect = false;
            this.mBackgroundsListView.Name = "mBackgroundsListView";
            this.mBackgroundsListView.Size = new System.Drawing.Size(304, 348);
            this.mBackgroundsListView.TabIndex = 4;
            this.mBackgroundsListView.TabStop = false;
            this.mBackgroundsListView.TileSize = new System.Drawing.Size(138, 79);
            this.mBackgroundsListView.UseCompatibleStateImageBehavior = false;
            this.mBackgroundsListView.View = System.Windows.Forms.View.Tile;
            this.mBackgroundsListView.SelectedIndexChanged += new System.EventHandler(this.mBackgroundsListView_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(134, 75);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mSetAsColourButton
            // 
            this.mSetAsColourButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mSetAsColourButton.Location = new System.Drawing.Point(314, 3);
            this.mSetAsColourButton.Name = "mSetAsColourButton";
            this.mSetAsColourButton.Size = new System.Drawing.Size(74, 37);
            this.mSetAsColourButton.TabIndex = 8;
            this.mSetAsColourButton.TabStop = false;
            this.mSetAsColourButton.Text = "Set as color";
            this.mToolTip.SetToolTip(this.mSetAsColourButton, "Sets the background color to the color box setting");
            this.mSetAsColourButton.UseVisualStyleBackColor = true;
            this.mSetAsColourButton.Click += new System.EventHandler(this.mSetAsColourButton_Click);
            // 
            // mBackgroundColourButton
            // 
            this.mBackgroundColourButton.BackColor = System.Drawing.Color.Black;
            this.mBackgroundColourButton.Location = new System.Drawing.Point(391, 3);
            this.mBackgroundColourButton.Name = "mBackgroundColourButton";
            this.mBackgroundColourButton.Size = new System.Drawing.Size(36, 37);
            this.mBackgroundColourButton.TabIndex = 9;
            this.mBackgroundColourButton.TabStop = false;
            this.mBackgroundColourButton.UseVisualStyleBackColor = false;
            this.mBackgroundColourButton.Click += new System.EventHandler(this.mBackgroundColourButton_Click);
            // 
            // SelectBackgroundControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.mBackgroundColourButton);
            this.Controls.Add(this.mSetAsColourButton);
            this.Controls.Add(this.mBrowseButton);
            this.Controls.Add(this.mClearButton);
            this.Controls.Add(this.mApplySettingToAllSlides);
            this.Controls.Add(this.mBackgroundsListView);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SelectBackgroundControl";
            this.Size = new System.Drawing.Size(435, 444);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mBrowseButton;
        private System.Windows.Forms.Button mClearButton;
        private System.Windows.Forms.Button mApplySettingToAllSlides;
        private System.Windows.Forms.ListView mBackgroundsListView;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button mSetAsColourButton;
        private System.Windows.Forms.Button mBackgroundColourButton;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
