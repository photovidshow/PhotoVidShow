namespace CustomButton
{
    partial class AdjustImageColoursControl
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
            this.mBlackAndWhiteButton = new System.Windows.Forms.CheckBox();
            this.mResetButton = new System.Windows.Forms.Button();
            this.mBrightnessContrastControll1 = new CustomButton.BrightnessContrastControll();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // mBlackAndWhiteButton
            // 
            this.mBlackAndWhiteButton.AutoSize = true;
            this.mBlackAndWhiteButton.Location = new System.Drawing.Point(3, 117);
            this.mBlackAndWhiteButton.Name = "mBlackAndWhiteButton";
            this.mBlackAndWhiteButton.Size = new System.Drawing.Size(81, 17);
            this.mBlackAndWhiteButton.TabIndex = 1;
            this.mBlackAndWhiteButton.TabStop = false;
            this.mBlackAndWhiteButton.Text = "Monotone";
            this.mBlackAndWhiteButton.UseVisualStyleBackColor = true;
            this.mBlackAndWhiteButton.CheckedChanged += new System.EventHandler(this.mBlackAndWhiteButton_CheckedChanged);
            // 
            // mResetButton
            // 
            this.mResetButton.Location = new System.Drawing.Point(3, 156);
            this.mResetButton.Name = "mResetButton";
            this.mResetButton.Size = new System.Drawing.Size(138, 23);
            this.mResetButton.TabIndex = 2;
            this.mResetButton.TabStop = false;
            this.mResetButton.Text = "Reset color settings";
            this.mToolTip.SetToolTip(this.mResetButton, "Resets the color settings to their default values");
            this.mResetButton.UseVisualStyleBackColor = true;
            this.mResetButton.Click += new System.EventHandler(this.mResetButton_Click);
            // 
            // mBrightnessContrastControll1
            // 
            this.mBrightnessContrastControll1.BrightnessB = 1F;
            this.mBrightnessContrastControll1.BrightnessG = 1F;
            this.mBrightnessContrastControll1.BrightnessR = 1F;
            this.mBrightnessContrastControll1.ContrastB = 1F;
            this.mBrightnessContrastControll1.ContrastG = 1F;
            this.mBrightnessContrastControll1.ContrastR = 1F;
            this.mBrightnessContrastControll1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mBrightnessContrastControll1.Location = new System.Drawing.Point(3, 3);
            this.mBrightnessContrastControll1.Name = "mBrightnessContrastControll1";
            this.mBrightnessContrastControll1.Size = new System.Drawing.Size(391, 108);
            this.mBrightnessContrastControll1.TabIndex = 0;
            this.mBrightnessContrastControll1.BrightnessBChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControll1_BrightnessBChanged);
            this.mBrightnessContrastControll1.ContrastGChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControll1_ContrastGChanged);
            this.mBrightnessContrastControll1.ContrastChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControll1_ContrastChanged);
            this.mBrightnessContrastControll1.BrightnessChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControll1_BrightnessChanged);
            this.mBrightnessContrastControll1.ContrastBChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControll1_ContrastBChanged);
            this.mBrightnessContrastControll1.ContrastRChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControll1_ContrastRChanged);
            this.mBrightnessContrastControll1.BrightnessGChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControll1_BrightnessGChanged);
            this.mBrightnessContrastControll1.BrightnessRChanged += new CustomButton.BrightnessContrastChangedCallback(this.mBrightnessContrastControll1_BrightnessRChanged);
            // 
            // AdjustImageColoursControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.mResetButton);
            this.Controls.Add(this.mBlackAndWhiteButton);
            this.Controls.Add(this.mBrightnessContrastControll1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AdjustImageColoursControl";
            this.Size = new System.Drawing.Size(384, 267);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightnessContrastControll mBrightnessContrastControll1;
        private System.Windows.Forms.CheckBox mBlackAndWhiteButton;
        private System.Windows.Forms.Button mResetButton;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}
