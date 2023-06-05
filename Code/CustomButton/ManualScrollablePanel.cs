using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CustomButton
{
    public class ManualScrollablePanel : UserControl
    {
        public ManualScrollablePanel()
        {
            InitializeComponent();
            mFramePanel.BackColor = Color.FromArgb(153, 180, 209);
            mSlidesPanel.AllowDrop = true;
        }

        public Panel GetPanel()
        {
            return this.mSlidesPanel;
        }

        public HScrollBar GetHScrollBar()
        {
            return this.hScrollBar;
        }

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
            this.mFramePanel = new System.Windows.Forms.Panel();
            this.mSlidesPanel = new DoubleBufferPanel();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.mFramePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mFramePanel
            //
            this.mFramePanel.Controls.Add(this.mSlidesPanel);
            this.mFramePanel.Controls.Add(this.hScrollBar);
            this.mFramePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mFramePanel.Location = new System.Drawing.Point(0, 0);
            this.mFramePanel.Name = "mFramePanel";
            this.mFramePanel.Size = new System.Drawing.Size(240, 186);
            this.mFramePanel.TabIndex = 0;
            // 
            // mSlidesPanel
            // 
            this.mSlidesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mSlidesPanel.Location = new System.Drawing.Point(0, 0);
            this.mSlidesPanel.Name = "mSlidesPanel";
            this.mSlidesPanel.Size = new System.Drawing.Size(240, 170);
            this.mSlidesPanel.TabIndex = 1;
            // 
            // hScrollBar
            // 
            this.hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar.Location = new System.Drawing.Point(0, 169);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(240, 18);
            this.hScrollBar.TabIndex = 0;
            // 
            // ManualScrollablePanel
            // 
            this.Controls.Add(this.mFramePanel);
            this.Name = "ManualScrollablePanel";
            this.Size = new System.Drawing.Size(240, 186);
            this.mFramePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.AutoScaleMode = AutoScaleMode.None;

        }

        #endregion

        private System.Windows.Forms.Panel mFramePanel;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private DoubleBufferPanel mSlidesPanel;

    }
}
