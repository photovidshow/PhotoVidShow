namespace CustomButton
{
    partial class OrientateControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrientateControl));
            this.mFlipYButton = new System.Windows.Forms.Button();
            this.mFlipXButton = new System.Windows.Forms.Button();
            this.mRotateCCW90Button = new System.Windows.Forms.Button();
            this.mRotateCW90Button = new System.Windows.Forms.Button();
            this.mResetOrientationButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mFlipYButton
            // 
            this.mFlipYButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mFlipYButton.BackgroundImage")));
            this.mFlipYButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mFlipYButton.Location = new System.Drawing.Point(27, 37);
            this.mFlipYButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mFlipYButton.Name = "mFlipYButton";
            this.mFlipYButton.Size = new System.Drawing.Size(73, 68);
            this.mFlipYButton.TabIndex = 13;
            this.mFlipYButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mFlipYButton, "Flip the image horizontally");
            this.mFlipYButton.UseVisualStyleBackColor = true;
            this.mFlipYButton.Click += new System.EventHandler(this.mFlipYButton_Click);
            // 
            // mFlipXButton
            // 
            this.mFlipXButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mFlipXButton.BackgroundImage")));
            this.mFlipXButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mFlipXButton.Location = new System.Drawing.Point(124, 37);
            this.mFlipXButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mFlipXButton.Name = "mFlipXButton";
            this.mFlipXButton.Size = new System.Drawing.Size(73, 68);
            this.mFlipXButton.TabIndex = 12;
            this.mFlipXButton.TabStop = false;
            this.mToolTip.SetToolTip(this.mFlipXButton, "Flip the image vertically");
            this.mFlipXButton.UseVisualStyleBackColor = true;
            this.mFlipXButton.Click += new System.EventHandler(this.mFlipXButton_Click);
            // 
            // mRotateCCW90Button
            // 
            this.mRotateCCW90Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mRotateCCW90Button.BackgroundImage")));
            this.mRotateCCW90Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mRotateCCW90Button.Location = new System.Drawing.Point(124, 38);
            this.mRotateCCW90Button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mRotateCCW90Button.Name = "mRotateCCW90Button";
            this.mRotateCCW90Button.Size = new System.Drawing.Size(73, 68);
            this.mRotateCCW90Button.TabIndex = 11;
            this.mRotateCCW90Button.TabStop = false;
            this.mToolTip.SetToolTip(this.mRotateCCW90Button, "Rotate the image anticlockwise");
            this.mRotateCCW90Button.UseVisualStyleBackColor = true;
            this.mRotateCCW90Button.Click += new System.EventHandler(this.mRotateCCW90Button_Click);
            // 
            // mRotateCW90Button
            // 
            this.mRotateCW90Button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("mRotateCW90Button.BackgroundImage")));
            this.mRotateCW90Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mRotateCW90Button.Location = new System.Drawing.Point(27, 38);
            this.mRotateCW90Button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mRotateCW90Button.Name = "mRotateCW90Button";
            this.mRotateCW90Button.Size = new System.Drawing.Size(73, 68);
            this.mRotateCW90Button.TabIndex = 10;
            this.mRotateCW90Button.TabStop = false;
            this.mToolTip.SetToolTip(this.mRotateCW90Button, "Rotate the image clockwise");
            this.mRotateCW90Button.UseVisualStyleBackColor = true;
            this.mRotateCW90Button.Click += new System.EventHandler(this.mRotateCW90Button_Click);
            // 
            // mResetOrientationButton
            // 
            this.mResetOrientationButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mResetOrientationButton.Location = new System.Drawing.Point(4, 275);
            this.mResetOrientationButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mResetOrientationButton.Name = "mResetOrientationButton";
            this.mResetOrientationButton.Size = new System.Drawing.Size(151, 23);
            this.mResetOrientationButton.TabIndex = 16;
            this.mResetOrientationButton.TabStop = false;
            this.mResetOrientationButton.Text = "Reset orientation";
            this.mToolTip.SetToolTip(this.mResetOrientationButton, "Reset the orientation to the default value");
            this.mResetOrientationButton.UseVisualStyleBackColor = true;
            this.mResetOrientationButton.Click += new System.EventHandler(this.mResetOrientationButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mRotateCW90Button);
            this.groupBox1.Controls.Add(this.mRotateCCW90Button);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(229, 129);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rotate";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mFlipYButton);
            this.groupBox2.Controls.Add(this.mFlipXButton);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(4, 135);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(229, 129);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Flip";
            // 
            // OrientateControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mResetOrientationButton);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "OrientateControl";
            this.Size = new System.Drawing.Size(461, 363);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mFlipYButton;
        private System.Windows.Forms.Button mFlipXButton;
        private System.Windows.Forms.Button mRotateCCW90Button;
        private System.Windows.Forms.Button mRotateCW90Button;
        private System.Windows.Forms.Button mResetOrientationButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
