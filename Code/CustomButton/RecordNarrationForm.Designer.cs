namespace CustomButton
{
    partial class RecordNarrationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordNarrationForm));
            this.mStoreName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mStoreLocationTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.mEchoRecordingCheckBox = new System.Windows.Forms.CheckBox();
            this.mDevicesCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mDoneButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.mImageList1 = new System.Windows.Forms.ImageList(this.components);
            this.mToolStrip = new System.Windows.Forms.ToolStrip();
            this.mRecordButton = new System.Windows.Forms.ToolStripButton();
            this.mStopButton = new System.Windows.Forms.ToolStripButton();
            this.mRecordingLabel = new System.Windows.Forms.Label();
            this.mRedDot = new System.Windows.Forms.PictureBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.mTimer1 = new System.Windows.Forms.Timer(this.components);
            this.mRecordNarrationFromTimeLabel = new System.Windows.Forms.Label();
            this.mToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mRedDot)).BeginInit();
            this.SuspendLayout();
            // 
            // mStoreName
            // 
            this.mStoreName.Location = new System.Drawing.Point(91, 6);
            this.mStoreName.Name = "mStoreName";
            this.mStoreName.Size = new System.Drawing.Size(217, 22);
            this.mStoreName.TabIndex = 3;
            this.mStoreName.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(-19, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mStoreLocationTextBox
            // 
            this.mStoreLocationTextBox.Location = new System.Drawing.Point(91, 32);
            this.mStoreLocationTextBox.Name = "mStoreLocationTextBox";
            this.mStoreLocationTextBox.ReadOnly = true;
            this.mStoreLocationTextBox.Size = new System.Drawing.Size(217, 22);
            this.mStoreLocationTextBox.TabIndex = 5;
            this.mStoreLocationTextBox.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(-19, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 19);
            this.label2.TabIndex = 6;
            this.label2.Text = "Store location";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mBrowseButton
            // 
            this.mBrowseButton.Location = new System.Drawing.Point(310, 32);
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.mBrowseButton.TabIndex = 7;
            this.mBrowseButton.TabStop = false;
            this.mBrowseButton.Text = "Browse";
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.mBrowseButton_Click);
            // 
            // mEchoRecordingCheckBox
            // 
            this.mEchoRecordingCheckBox.AutoSize = true;
            this.mEchoRecordingCheckBox.Location = new System.Drawing.Point(10, 96);
            this.mEchoRecordingCheckBox.Name = "mEchoRecordingCheckBox";
            this.mEchoRecordingCheckBox.Size = new System.Drawing.Size(298, 17);
            this.mEchoRecordingCheckBox.TabIndex = 8;
            this.mEchoRecordingCheckBox.TabStop = false;
            this.mEchoRecordingCheckBox.Text = "Echo recording (may work best with headphones on)";
            this.mEchoRecordingCheckBox.UseVisualStyleBackColor = true;
            // 
            // mDevicesCombo
            // 
            this.mDevicesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mDevicesCombo.FormattingEnabled = true;
            this.mDevicesCombo.Location = new System.Drawing.Point(91, 58);
            this.mDevicesCombo.Name = "mDevicesCombo";
            this.mDevicesCombo.Size = new System.Drawing.Size(217, 21);
            this.mDevicesCombo.TabIndex = 9;
            this.mDevicesCombo.TabStop = false;
            this.mDevicesCombo.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(-19, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 19);
            this.label3.TabIndex = 10;
            this.label3.Text = "Record device";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mDoneButton
            // 
            this.mDoneButton.Location = new System.Drawing.Point(317, 201);
            this.mDoneButton.Name = "mDoneButton";
            this.mDoneButton.Size = new System.Drawing.Size(75, 23);
            this.mDoneButton.TabIndex = 1;
            this.mDoneButton.Text = "Done";
            this.mDoneButton.UseVisualStyleBackColor = true;
            this.mDoneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(314, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = ".wma";
            // 
            // mImageList1
            // 
            this.mImageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mImageList1.ImageStream")));
            this.mImageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.mImageList1.Images.SetKeyName(0, "Actions-media-playback-pause-icon-medium.png");
            this.mImageList1.Images.SetKeyName(1, "Actions-media-record-icon-small.png");
            // 
            // mToolStrip
            // 
            this.mToolStrip.AutoSize = false;
            this.mToolStrip.BackColor = System.Drawing.Color.White;
            this.mToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mToolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.mToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mRecordButton,
            this.mStopButton});
            this.mToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mToolStrip.Location = new System.Drawing.Point(3, 155);
            this.mToolStrip.Name = "mToolStrip";
            this.mToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mToolStrip.Size = new System.Drawing.Size(161, 102);
            this.mToolStrip.TabIndex = 13;
            this.mToolStrip.Text = "toolStrip1";
            // 
            // mRecordButton
            // 
            this.mRecordButton.AutoSize = false;
            this.mRecordButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mRecordButton.Image = ((System.Drawing.Image)(resources.GetObject("mRecordButton.Image")));
            this.mRecordButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mRecordButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mRecordButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(68, 68);
            this.mRecordButton.Text = "Record";
            this.mRecordButton.Click += new System.EventHandler(this.Record_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.AutoSize = false;
            this.mStopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mStopButton.Enabled = false;
            this.mStopButton.Image = ((System.Drawing.Image)(resources.GetObject("mStopButton.Image")));
            this.mStopButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mStopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mStopButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(68, 68);
            this.mStopButton.Text = "Stop";
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mRecordingLabel
            // 
            this.mRecordingLabel.AutoSize = true;
            this.mRecordingLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mRecordingLabel.Location = new System.Drawing.Point(181, 182);
            this.mRecordingLabel.Name = "mRecordingLabel";
            this.mRecordingLabel.Size = new System.Drawing.Size(82, 17);
            this.mRecordingLabel.TabIndex = 15;
            this.mRecordingLabel.Text = "Recording...";
            this.mRecordingLabel.Visible = false;
            // 
            // mRedDot
            // 
            this.mRedDot.BackgroundImage = global::CustomButton.Properties.Resources.red_dot21;
            this.mRedDot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.mRedDot.Location = new System.Drawing.Point(163, 182);
            this.mRedDot.Name = "mRedDot";
            this.mRedDot.Size = new System.Drawing.Size(16, 16);
            this.mRedDot.TabIndex = 14;
            this.mRedDot.TabStop = false;
            this.mRedDot.Visible = false;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.AutoSize = false;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.AutoSize = false;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton3.Text = "toolStripButton3";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.AutoSize = false;
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton4.Text = "toolStripButton4";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.AutoSize = false;
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton5.Text = "toolStripButton5";
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.AutoSize = false;
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(68, 68);
            this.toolStripButton6.Text = "toolStripButton6";
            // 
            // mTimer1
            // 
            this.mTimer1.Interval = 500;
            this.mTimer1.Tick += new System.EventHandler(this.mTimer1_Tick);
            // 
            // mRecordNarrationFromTimeLabel
            // 
            this.mRecordNarrationFromTimeLabel.AutoSize = true;
            this.mRecordNarrationFromTimeLabel.Location = new System.Drawing.Point(7, 128);
            this.mRecordNarrationFromTimeLabel.Name = "mRecordNarrationFromTimeLabel";
            this.mRecordNarrationFromTimeLabel.Size = new System.Drawing.Size(209, 13);
            this.mRecordNarrationFromTimeLabel.TabIndex = 16;
            this.mRecordNarrationFromTimeLabel.Text = "Record narration from time: 00:00:00.00";
            // 
            // RecordNarrationForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(397, 231);
            this.Controls.Add(this.mRecordNarrationFromTimeLabel);
            this.Controls.Add(this.mRecordingLabel);
            this.Controls.Add(this.mRedDot);
            this.Controls.Add(this.mToolStrip);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mDoneButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mDevicesCombo);
            this.Controls.Add(this.mEchoRecordingCheckBox);
            this.Controls.Add(this.mBrowseButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mStoreLocationTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mStoreName);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordNarrationForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Record narration";
            this.Load += new System.EventHandler(this.RecordNarrationForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RecordNarrationForm_FormClosed);
            this.mToolStrip.ResumeLayout(false);
            this.mToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mRedDot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mStoreName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mStoreLocationTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button mBrowseButton;
        private System.Windows.Forms.CheckBox mEchoRecordingCheckBox;
        private System.Windows.Forms.ComboBox mDevicesCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button mDoneButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ImageList mImageList1;
        private System.Windows.Forms.ToolStrip mToolStrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripButton mRecordButton;
        private System.Windows.Forms.ToolStripButton mStopButton;
        private System.Windows.Forms.PictureBox mRedDot;
        private System.Windows.Forms.Label mRecordingLabel;
        private System.Windows.Forms.Timer mTimer1;
        private System.Windows.Forms.Label mRecordNarrationFromTimeLabel;
    }
}