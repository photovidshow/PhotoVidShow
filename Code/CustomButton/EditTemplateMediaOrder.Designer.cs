namespace CustomButton
{
    partial class EditTemplateMediaOrder
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
            this.mSlideMediaListView = new System.Windows.Forms.ListView();
            this.mDragDropLabel = new System.Windows.Forms.Label();
            this.mDownOrderButton = new System.Windows.Forms.Button();
            this.mUpOrderButton = new System.Windows.Forms.Button();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mSlideMediaListView
            // 
            this.mSlideMediaListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.mSlideMediaListView.AllowDrop = true;
            this.mSlideMediaListView.Location = new System.Drawing.Point(3, 0);
            this.mSlideMediaListView.Margin = new System.Windows.Forms.Padding(0);
            this.mSlideMediaListView.MultiSelect = false;
            this.mSlideMediaListView.Name = "mSlideMediaListView";
            this.mSlideMediaListView.RightToLeftLayout = true;
            this.mSlideMediaListView.ShowItemToolTips = true;
            this.mSlideMediaListView.Size = new System.Drawing.Size(710, 110);
            this.mSlideMediaListView.TabIndex = 14;
            this.mSlideMediaListView.TabStop = false;
            this.mSlideMediaListView.UseCompatibleStateImageBehavior = false;
            this.mSlideMediaListView.SelectedIndexChanged += new System.EventHandler(this.mSlideMediaListView_SelectedIndexChanged);
            this.mSlideMediaListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.mSlideMediaListView_DragDrop);
            this.mSlideMediaListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.mSlideMediaListView_DragEnter);
            this.mSlideMediaListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.mSlideMediaListView_ItemDrag);
            // 
            // mDragDropLabel
            // 
            this.mDragDropLabel.AutoSize = true;
            this.mDragDropLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDragDropLabel.Location = new System.Drawing.Point(3, 112);
            this.mDragDropLabel.Name = "mDragDropLabel";
            this.mDragDropLabel.Size = new System.Drawing.Size(417, 13);
            this.mDragDropLabel.TabIndex = 17;
            this.mDragDropLabel.Text = "Select and drag to swap images or use move up and down buttons on the right";
            // 
            // mDownOrderButton
            // 
            this.mDownOrderButton.BackgroundImage = global::CustomButton.Properties.Resources.redo1;
            this.mDownOrderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mDownOrderButton.Location = new System.Drawing.Point(715, 36);
            this.mDownOrderButton.Name = "mDownOrderButton";
            this.mDownOrderButton.Size = new System.Drawing.Size(28, 33);
            this.mDownOrderButton.TabIndex = 16;
            this.mDownOrderButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mDownOrderButton, "Swaps the currently selected image with the next image in the slide");
            this.mDownOrderButton.UseVisualStyleBackColor = true;
            this.mDownOrderButton.Click += new System.EventHandler(this.mDownOrderButton_Click);
            // 
            // mUpOrderButton
            // 
            this.mUpOrderButton.BackgroundImage = global::CustomButton.Properties.Resources.undo1;
            this.mUpOrderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mUpOrderButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mUpOrderButton.Location = new System.Drawing.Point(715, 2);
            this.mUpOrderButton.Name = "mUpOrderButton";
            this.mUpOrderButton.Size = new System.Drawing.Size(28, 33);
            this.mUpOrderButton.TabIndex = 15;
            this.mUpOrderButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mUpOrderButton, "Swaps the currently selected image with the previous image in the slide");
            this.mUpOrderButton.UseVisualStyleBackColor = true;
            this.mUpOrderButton.Click += new System.EventHandler(this.mUpOrderButton_Click);
            // 
            // EditTemplateMediaOrder
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.mDragDropLabel);
            this.Controls.Add(this.mDownOrderButton);
            this.Controls.Add(this.mUpOrderButton);
            this.Controls.Add(this.mSlideMediaListView);
            this.Name = "EditTemplateMediaOrder";
            this.Size = new System.Drawing.Size(755, 132);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mDownOrderButton;
        private System.Windows.Forms.Button mUpOrderButton;
        private System.Windows.Forms.ListView mSlideMediaListView;
        private System.Windows.Forms.Label mDragDropLabel;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
