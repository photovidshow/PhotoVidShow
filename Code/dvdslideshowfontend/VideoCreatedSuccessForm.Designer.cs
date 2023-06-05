namespace dvdslideshowfontend
{
    partial class VideoCreatedSuccessForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoCreatedSuccessForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mOpenFileLocationButton = new System.Windows.Forms.Button();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.mFilename = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Video creation successful!";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(44, 39);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // mPlayButton
            // 
            this.mPlayButton.Location = new System.Drawing.Point(167, 10);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(69, 23);
            this.mPlayButton.TabIndex = 3;
            this.mPlayButton.Text = "Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mOpenFileLocationButton
            // 
            this.mOpenFileLocationButton.Location = new System.Drawing.Point(242, 10);
            this.mOpenFileLocationButton.Name = "mOpenFileLocationButton";
            this.mOpenFileLocationButton.Size = new System.Drawing.Size(125, 23);
            this.mOpenFileLocationButton.TabIndex = 4;
            this.mOpenFileLocationButton.Text = "Open file location";
            this.mOpenFileLocationButton.UseVisualStyleBackColor = true;
            this.mOpenFileLocationButton.Click += new System.EventHandler(this.mOpenFileLocationButton_Click);
            // 
            // mCloseButton
            // 
            this.mCloseButton.Location = new System.Drawing.Point(373, 10);
            this.mCloseButton.Name = "mCloseButton";
            this.mCloseButton.Size = new System.Drawing.Size(62, 23);
            this.mCloseButton.TabIndex = 5;
            this.mCloseButton.Text = "Close";
            this.mCloseButton.UseVisualStyleBackColor = true;
            this.mCloseButton.Click += new System.EventHandler(this.mCloseButton_Click);
            // 
            // mFilename
            // 
            this.mFilename.AutoSize = true;
            this.mFilename.BackColor = System.Drawing.Color.Transparent;
            this.mFilename.Location = new System.Drawing.Point(12, 64);
            this.mFilename.Name = "mFilename";
            this.mFilename.Size = new System.Drawing.Size(285, 13);
            this.mFilename.TabIndex = 6;
            this.mFilename.Text = "c:\\my documents\\photovidshow\\authored\\wdwd.mp4";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(382, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(51, 50);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.mCloseButton);
            this.panel1.Controls.Add(this.mOpenFileLocationButton);
            this.panel1.Controls.Add(this.mPlayButton);
            this.panel1.Location = new System.Drawing.Point(-2, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 45);
            this.panel1.TabIndex = 8;
            // 
            // VideoCreatedSuccessForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(449, 135);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.mFilename);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VideoCreatedSuccessForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Operation complete";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.Button mOpenFileLocationButton;
        private System.Windows.Forms.Button mCloseButton;
        private System.Windows.Forms.Label mFilename;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel1;
    }
}