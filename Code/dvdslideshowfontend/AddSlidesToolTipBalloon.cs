using System;
using System.IO;
using System.Resources;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SquidgySoft.UI.Controls;

namespace dvdslideshowfontend
{
	
	public class AddSlidesToolIpBalloon : BalloonWindow
	{
		private System.Windows.Forms.Timer tmrMoveTime;
		private System.Windows.Forms.TextBox textBox1;
		private System.ComponentModel.IContainer components = null;

		public AddSlidesToolIpBalloon()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

            Form1.ReduceFontSizeToMatchDPI(this.Controls);

			// TODO: Add any initialization after the InitializeComponent call
		}

		protected override void OnLoad(EventArgs e)
		{
			SetRadianTick();

			base.OnLoad(e);
		}

		private void SetRadianTick()
		{
		/*	int drawWidth = pnlTrigShape.Width-1;
			//double period = ((double)numericUpDown2.Value)*(2*Math.PI);
			double period = 2*Math.PI;

			__radianTick = period/drawWidth;
			*/
		}

		private double __radianTick;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.tmrMoveTime = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tmrMoveTime
            // 
            this.tmrMoveTime.Tick += new System.EventHandler(this.tmrMoveTime_Tick);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(225)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(8, 9);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(216, 31);
            this.textBox1.TabIndex = 11;
            this.textBox1.TabStop = false;
            this.textBox1.Text = "Click here to begin adding pictures and video to the current slideshow";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // AddSlidesToolIpBalloon
            // 
            this.AnchorPoint = new System.Drawing.Point(-100, 30);
            this.AnchorQuadrantBase = SquidgySoft.UI.Controls.AnchorQuadrant.Bottom;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(256, 64);
            this.Controls.Add(this.textBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "AddSlidesToolIpBalloon";
            this.Shadow = false;
            this.Timeout = 20000;
            this.Leave += new System.EventHandler(this.lf);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void lf(object o, System.EventArgs e)
		{
			int i=3;
			i++;
		}

		Point displayPoint = Point.Empty;
		double nextRadian = 0;
		private void pnlTrigShape_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics grx = e.Graphics;
			grx.DrawRectangle(Pens.Black, displayPoint.X, displayPoint.Y, 2, 2);
		//	label5.Text = displayPoint.ToString();
		}


		private void numericUpDown2_ValueChanged(object sender, System.EventArgs e)
		{
			SetRadianTick();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			tmrMoveTime.Enabled = Visible;

			base.OnVisibleChanged(e);
		}

		private void tmrMoveTime_Tick(object sender, System.EventArgs e)
		{
			/*
			string trig = this.cboTrigShape.Text;

			switch(trig)
			{
				case "Sine":
					displayPoint.Y = (int)(Math.Sin(nextRadian)*this.pnlTrigShape.Height);
					break;
				case "Cosine":
					displayPoint.Y = (int)(Math.Cos(nextRadian)*this.pnlTrigShape.Height);
					break;
				case "Tangent":
					displayPoint.Y = (int)(Math.Tan(nextRadian)*this.pnlTrigShape.Height);
					break;
			}

			displayPoint.Y = displayPoint.Y/2 + this.pnlTrigShape.Height/2;

			displayPoint.X = (displayPoint.X+1) % this.pnlTrigShape.Width;
			nextRadian += this.__radianTick;
			this.pnlTrigShape.Invalidate(false);
			*/
		}

		private void textBox1_TextChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
