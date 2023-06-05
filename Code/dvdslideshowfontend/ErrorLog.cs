using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DVDSlideshow;
using ManagedCore;
using System.IO;


namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for ErrorLog.
	/// </summary>
	public class ErrorLog : System.Windows.Forms.Form , ILogHook
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox LogTextBox;
		private System.Windows.Forms.Panel ErrorLogPanel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ErrorLog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        public void StartShow()
        {
            CDebugLog.GetInstance().AddHook(this);
            this.Show();
        }

        private delegate void DeclareEntryDelegate(string s);

		public void DeclareEntry(string e)
		{
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new DeclareEntryDelegate(DeclareEntry),
                                            new object[] { e });
                return ;
            }
			try
			{
				LogTextBox.Text+=e;
				LogTextBox.Text+="\r\n";

				LogTextBox.SelectionStart = LogTextBox.Text.Length;
				LogTextBox.ScrollToCaret();

			}
			catch(Exception)
			{
				Console.WriteLine("ERROR throw an exception in DeclareEntry");
			}

		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.LogTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.ErrorLogPanel = new System.Windows.Forms.Panel();
			this.ErrorLogPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// LogTextBox
			// 
			this.LogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LogTextBox.Location = new System.Drawing.Point(0, 0);
			this.LogTextBox.Multiline = true;
			this.LogTextBox.Name = "LogTextBox";
			this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.LogTextBox.Size = new System.Drawing.Size(528, 272);
			this.LogTextBox.TabIndex = 0;
			this.LogTextBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Log";
			// 
			// ErrorLogPanel
			// 
			this.ErrorLogPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.ErrorLogPanel.BackColor = System.Drawing.SystemColors.Window;
			this.ErrorLogPanel.Controls.Add(this.LogTextBox);
			this.ErrorLogPanel.Location = new System.Drawing.Point(0, 24);
			this.ErrorLogPanel.Name = "ErrorLogPanel";
			this.ErrorLogPanel.Size = new System.Drawing.Size(528, 272);
			this.ErrorLogPanel.TabIndex = 0;
			// 
			// ErrorLog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(528, 302);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ErrorLogPanel);
			this.Name = "ErrorLog";
			this.Text = "ErrorLog";
			this.ErrorLogPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}

}
