using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using DVDSlideshow;
using System.Globalization;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for CUpdateWindow.
	/// </summary>
	public class CUpdateWindow : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Button CancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Thread mThread;
		private System.Windows.Forms.Label StatusLabel;
        private LinkLabel mLinkDownloadLabel;
		private ManagedCore.Updater mUpdater;

		public CUpdateWindow()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			System.Threading.ThreadStart st = new ThreadStart(CheckForupdates);
			mThread= new Thread(st);
			mThread.Start();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        //*******************************************************************
        private delegate void SetStatusLabelDelegate(string t);
        private void SetStatusLabel(string t)
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new SetStatusLabelDelegate(SetStatusLabel),
                                            new object[] { t });
                return;
            }
            this.StatusLabel.Text = t;
		    this.StatusLabel.Refresh();
        }


        //*******************************************************************
        private delegate void EnableDownloadLinkDelegate();
        private void EnableDownloadLink()
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new EnableDownloadLinkDelegate(EnableDownloadLink));
                return;
            }

            mLinkDownloadLabel.Visible = true;
        }

        //*******************************************************************
        private delegate void SetCancelButtonTextDelegate(string t);
        private void SetCancelButtonText(string t)
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new SetCancelButtonTextDelegate(SetCancelButtonText),
                                            new object[] { t });
                return;
            }
            this.CancelButton.Text = t;
        }


        //*******************************************************************
		private void CheckForupdates()
		{
            //
            // If running steam, updates are done there
            //
            if (CGlobals.RunningSteamVersion == true)
            {
                SetStatusLabel("This is a Steam version of PhotoVidShow.\r\nPlease run Steam to check for updates.");
                SetCancelButtonText("Done");
                return;
            }

            //
            // If running Windows store version, update are done there
            //
            if (CGlobals.RunningWindowsStoreVersion == true)
            {
                SetStatusLabel("This is a Windows store version of PhotoVidShow.\r\nPlease see Windows store to check for updates.");
                SetCancelButtonText("Done");
                return;
            }

            mUpdater = new ManagedCore.Updater();

			string new_version="";
			if (mUpdater.CheckIfUpdateRequired(ref new_version)==true)
			{
                // 4 01 005

				float majour = float.Parse(new_version.Substring(0,1),CultureInfo.InvariantCulture);
                float middle = float.Parse(new_version.Substring(1,1), CultureInfo.InvariantCulture);
                float end = float.Parse(new_version.Substring(3,3), CultureInfo.InvariantCulture);

                string nnv = majour.ToString() + "." + middle.ToString() + "." + end.ToString();

                SetStatusLabel("New version found v" + nnv + ", select link below to download.");

                EnableDownloadLink();
			}
			else
			{
				SetStatusLabel("Current version is up to date.");
                SetCancelButtonText("Done");
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
            this.StatusLabel = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.mLinkDownloadLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // StatusLabel
            // 
            this.StatusLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.StatusLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Location = new System.Drawing.Point(0, 8);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(400, 50);
            this.StatusLabel.TabIndex = 0;
            this.StatusLabel.Text = "Checking for updates......";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CancelButton
            // 
            this.CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.CancelButton.Location = new System.Drawing.Point(325, 88);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // mLinkDownloadLabel
            // 
            this.mLinkDownloadLabel.AutoSize = true;
            this.mLinkDownloadLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mLinkDownloadLabel.Location = new System.Drawing.Point(72, 58);
            this.mLinkDownloadLabel.Name = "mLinkDownloadLabel";
            this.mLinkDownloadLabel.Size = new System.Drawing.Size(281, 17);
            this.mLinkDownloadLabel.TabIndex = 2;
            this.mLinkDownloadLabel.TabStop = true;
            this.mLinkDownloadLabel.Text = "https://www.photovidshow.com/download.html";
            this.mLinkDownloadLabel.Visible = false;
            this.mLinkDownloadLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mLinkDownloadLabel_LinkClicked);
            // 
            // CUpdateWindow
            // 
            this.ClientSize = new System.Drawing.Size(408, 123);
            this.ControlBox = false;
            this.Controls.Add(this.mLinkDownloadLabel);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.StatusLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBox = false;
            this.Name = "CUpdateWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Check for updates";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void CancelButton_Click(object sender, System.EventArgs e)
		{
			if (mThread!=null)
			{
				mThread.Abort();
			}
			this.Close();
		}

		private void InstallButton_Click(object sender, System.EventArgs e)
		{
			this.StatusLabel.Text="Downloading and installing..... (please wait)";
			this.StatusLabel.Refresh();
			this.CancelButton.Enabled=false;

            // 2005 SRG FIX ME
			mUpdater.Upgrade(CGlobals.GetRootDirectory());

			this.Close();
		}

        // ******************************************************************************************
        private void mLinkDownloadLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ManagedCore.CSystem.OpenBrowser("www.photovidshow.com/download.html");
            this.Close();
        }
	}
}
