namespace CustomButton
{
    partial class SetStartEndSlideTime
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
            this.mMaxTimeLabel = new System.Windows.Forms.Label();
            this.mStartSlideTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.mStartSlideTimeLabel = new System.Windows.Forms.Label();
            this.mEndSlideTimeLabel = new System.Windows.Forms.Label();
            this.mEndSlideTimeTrackBar = new System.Windows.Forms.TrackBar();
            this.mStartTimeValueNumerical = new System.Windows.Forms.NumericUpDown();
            this.mEndTimeValueNumerical = new System.Windows.Forms.NumericUpDown();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mStartSlideTimeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mEndSlideTimeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStartTimeValueNumerical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mEndTimeValueNumerical)).BeginInit();
            this.SuspendLayout();
            // 
            // mMaxTimeLabel
            // 
            this.mMaxTimeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMaxTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mMaxTimeLabel.Location = new System.Drawing.Point(168, 30);
            this.mMaxTimeLabel.Name = "mMaxTimeLabel";
            this.mMaxTimeLabel.Size = new System.Drawing.Size(41, 24);
            this.mMaxTimeLabel.TabIndex = 34;
            this.mMaxTimeLabel.Text = "label3";
            // 
            // mStartSlideTimeTrackBar
            // 
            this.mStartSlideTimeTrackBar.AutoSize = false;
            this.mStartSlideTimeTrackBar.BackColor = System.Drawing.Color.White;
            this.mStartSlideTimeTrackBar.Location = new System.Drawing.Point(28, 1);
            this.mStartSlideTimeTrackBar.Maximum = 100;
            this.mStartSlideTimeTrackBar.Name = "mStartSlideTimeTrackBar";
            this.mStartSlideTimeTrackBar.Size = new System.Drawing.Size(140, 45);
            this.mStartSlideTimeTrackBar.TabIndex = 30;
            this.mStartSlideTimeTrackBar.TabStop = false;
            this.mStartSlideTimeTrackBar.TickFrequency = 100;
            this.mStartSlideTimeTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.mToolTip.SetToolTip(this.mStartSlideTimeTrackBar, "Sets the start time");
            this.mStartSlideTimeTrackBar.Scroll += new System.EventHandler(this.mStartSlideTimeTrackBar_Scroll);
            // 
            // mStartSlideTimeLabel
            // 
            this.mStartSlideTimeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mStartSlideTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mStartSlideTimeLabel.Location = new System.Drawing.Point(-7, 0);
            this.mStartSlideTimeLabel.Name = "mStartSlideTimeLabel";
            this.mStartSlideTimeLabel.Size = new System.Drawing.Size(45, 37);
            this.mStartSlideTimeLabel.TabIndex = 31;
            this.mStartSlideTimeLabel.Text = "Start";
            this.mStartSlideTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mEndSlideTimeLabel
            // 
            this.mEndSlideTimeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEndSlideTimeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mEndSlideTimeLabel.Location = new System.Drawing.Point(-5, 45);
            this.mEndSlideTimeLabel.Name = "mEndSlideTimeLabel";
            this.mEndSlideTimeLabel.Size = new System.Drawing.Size(45, 35);
            this.mEndSlideTimeLabel.TabIndex = 33;
            this.mEndSlideTimeLabel.Text = "End";
            this.mEndSlideTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mEndSlideTimeTrackBar
            // 
            this.mEndSlideTimeTrackBar.AutoSize = false;
            this.mEndSlideTimeTrackBar.BackColor = System.Drawing.Color.White;
            this.mEndSlideTimeTrackBar.Location = new System.Drawing.Point(28, 45);
            this.mEndSlideTimeTrackBar.Maximum = 100;
            this.mEndSlideTimeTrackBar.Name = "mEndSlideTimeTrackBar";
            this.mEndSlideTimeTrackBar.Size = new System.Drawing.Size(140, 45);
            this.mEndSlideTimeTrackBar.TabIndex = 32;
            this.mEndSlideTimeTrackBar.TabStop = false;
            this.mEndSlideTimeTrackBar.TickFrequency = 100;
            this.mEndSlideTimeTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.mToolTip.SetToolTip(this.mEndSlideTimeTrackBar, "Sets the end time");
            this.mEndSlideTimeTrackBar.Value = 100;
            this.mEndSlideTimeTrackBar.Scroll += new System.EventHandler(this.mEndSlideTimeTrackBar_Scroll);
            // 
            // mStartTimeValueNumerical
            // 
            this.mStartTimeValueNumerical.DecimalPlaces = 1;
            this.mStartTimeValueNumerical.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.mStartTimeValueNumerical.Location = new System.Drawing.Point(166, 5);
            this.mStartTimeValueNumerical.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mStartTimeValueNumerical.Name = "mStartTimeValueNumerical";
            this.mStartTimeValueNumerical.ReadOnly = true;
            this.mStartTimeValueNumerical.Size = new System.Drawing.Size(45, 22);
            this.mStartTimeValueNumerical.TabIndex = 36;
            this.mStartTimeValueNumerical.TabStop = false;
            this.mStartTimeValueNumerical.ValueChanged += new System.EventHandler(this.mStartTimeValueNumerical_ValueChanged);
            // 
            // mEndTimeValueNumerical
            // 
            this.mEndTimeValueNumerical.DecimalPlaces = 1;
            this.mEndTimeValueNumerical.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.mEndTimeValueNumerical.Location = new System.Drawing.Point(166, 52);
            this.mEndTimeValueNumerical.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mEndTimeValueNumerical.Name = "mEndTimeValueNumerical";
            this.mEndTimeValueNumerical.ReadOnly = true;
            this.mEndTimeValueNumerical.Size = new System.Drawing.Size(45, 22);
            this.mEndTimeValueNumerical.TabIndex = 37;
            this.mEndTimeValueNumerical.TabStop = false;
            this.mEndTimeValueNumerical.ValueChanged += new System.EventHandler(this.mEndTimeValueNumerical_ValueChanged);
            // 
            // SetStartEndSlideTime
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.mEndTimeValueNumerical);
            this.Controls.Add(this.mStartTimeValueNumerical);
            this.Controls.Add(this.mEndSlideTimeTrackBar);
            this.Controls.Add(this.mMaxTimeLabel);
            this.Controls.Add(this.mStartSlideTimeTrackBar);
            this.Controls.Add(this.mEndSlideTimeLabel);
            this.Controls.Add(this.mStartSlideTimeLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SetStartEndSlideTime";
            this.Size = new System.Drawing.Size(213, 87);
            ((System.ComponentModel.ISupportInitialize)(this.mStartSlideTimeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mEndSlideTimeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStartTimeValueNumerical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mEndTimeValueNumerical)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label mMaxTimeLabel;
        private System.Windows.Forms.TrackBar mStartSlideTimeTrackBar;
        private System.Windows.Forms.Label mStartSlideTimeLabel;
        private System.Windows.Forms.Label mEndSlideTimeLabel;
        private System.Windows.Forms.TrackBar mEndSlideTimeTrackBar;
        private System.Windows.Forms.NumericUpDown mStartTimeValueNumerical;
        private System.Windows.Forms.NumericUpDown mEndTimeValueNumerical;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
