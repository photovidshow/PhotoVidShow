#region Using Directives
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Threading;
#endregion
using DVDSlideshow;
using CustomButton;
using System.IO;

namespace dvdslideshowfontend
{
    public delegate void DelegateCloseSplash();

    public class SplashForm : Form
    {
        private bool mFromCD = false;
        private bool mDownloadNoTrial = false;

        #region Constructor
        public SplashForm(String imageFile, Color col)
        {
            this.InitializeComponent();

            //
            // Do we need to show some kind of 'register the software' options?
            //
            if (CGlobals.RunningSteamDemo == false && 
                CGlobals.RunningWindowsStoreTrial == false && 
                ManagedCore.License.License.Valid==false && 
                mFromAbout ==false)
			{
                // cd file =
                string cdFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVidShow" , "CD.txt");
                //
                // Are we running this after installing from CD show registraion window
                //
                if (File.Exists(cdFile) == true)
                {
                    mFromCD = true;
                }

                // cd file =
                string ntFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PhotoVidShow", "NT.txt");
                if (File.Exists(ntFile) == true)
                {
                    mDownloadNoTrial = true;
                }

                if (mFromCD || mDownloadNoTrial)
                {
                    RegisterKeyButton.Visible = false;
                    ContinueButton.Visible = false;
                    Shown += SplashForm_ShowRegisterWindow;
                }
            }
            else
            {
                RegisterKeyButton.Visible = false;
                ContinueButton.Visible = false;
            }

			if (mFromAbout==true)
			{
				this.MouseDown+=new MouseEventHandler(LeaveSplashFromAbout);
			}

            Debug.Assert(imageFile != null && imageFile.Length > 0, 
                "A valid file path has to be given");
            // ====================================================================================
            // Setup the form
            // ==================================================================================== 
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			 this.ControlBox = false;
		
            this.ShowInTaskbar = false;
        //    this.TopMost = true;

            // make form transparent
         //   this.TransparencyKey = this.BackColor;

            // tie up the events
       //    this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SplashForm_KeyUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SplashForm_Paint);
         //   this.MouseDown += new MouseEventHandler(SplashForm_MouseClick);

            // load and make the bitmap transparent
            m_bmp = new Bitmap(imageFile);

            if(m_bmp == null)
                throw new Exception("Failed to load the bitmap file " + imageFile);
           m_bmp.MakeTransparent(col);

            // resize the form to the size of the iamge
            this.Width = m_bmp.Width;
            this.Height = m_bmp.Height;

            // center the form
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // thread handling
            m_delegateClose = new DelegateCloseSplash(InternalCloseSplash);

			DrawInnerText();

        }

        private void SplashForm_ShowRegisterWindow(object sender, EventArgs e)
        {
            RegisterKeyButton_Click(this, e);
        }

        #endregion // Constructor

        private void LeaveSplashFromAbout(object o, System.Windows.Forms.MouseEventArgs e)
		{
			InternalCloseSplash();
		}

		private void DrawInnerText()
		{
			CTextStyle ts= new CTextStyle();
			ts.Bold=true;
			ts.Outline=false;
			ts.Shadow=true;
			ts.FontName="Arial";
			ts.FontSize=14;
			ts.TextColor=Color.White;

			CTextStyle ts2= new CTextStyle();
			ts2.Bold=true;
			ts2.Outline=false;
			ts2.Shadow=true;
			ts2.FontName="Arial";
			ts2.FontSize=14;
			ts2.TextColor=Color.FromArgb(228,135,65);

            CTextStyle bottomstyle = CTextStyleDatabase.GetInstance().GetStyleFromName("Salt & Pepper").Clone();
            bottomstyle.FontSize = 16;
            bottomstyle.TextColor = Color.FromArgb(228, 135, 65);
            bottomstyle.TextColor2 = Color.FromArgb(228-30, 135-30, 65-30); 

            CTextStyle titlestyle = CTextStyleDatabase.GetInstance().GetStyleFromName("Salt & Pepper").Clone();
            titlestyle.FontSize = 30;

            CTextStyle versionstyle = CTextStyleDatabase.GetInstance().GetStyleFromName("Salt & Pepper").Clone();
            versionstyle.FontSize = 15;
          
            CTextStyle copywritestyle = CTextStyleDatabase.GetInstance().GetStyleFromName("Salt & Pepper").Clone();
            copywritestyle.FontSize = 8;

			RectangleF cov = new RectangleF(0.05f,0.35f,0.6f,0.4f);

			string s1 = FrontEndGlobals.mApplicationName;
			if (s1.EndsWith(" - Evaluation Copy"))
			{
				s1 =s1.Substring(0,s1.Length-18);         
			}

            if (CGlobals.RunningSteamDemo == true)
            {
                s1 += " Demo";
                titlestyle.FontSize = 20;
            }
            else if (CGlobals.RunningWindowsStoreTrial == true)
            {
                s1 += " trial";
                titlestyle.FontSize = 20;
            }



            CTextDecoration td = new CTextDecoration(s1,cov,1,ts);

			RectangleF cov2 = new RectangleF(0.055f,0.51f,0.25f,0.4f);
			CTextDecoration td2 = new CTextDecoration(CGlobals.CompleteVersionString(),cov2,1,ts);

			RectangleF cov3 = new RectangleF(0.06f,0.58f,0.35f,0.4f);
			CTextDecoration td3 = new CTextDecoration(CGlobals.copywright_string +" "+CGlobals.mCompanyName,cov3,1,ts);

			RectangleF cov31 = new RectangleF(0.06f,0.62f,0.25f,0.4f);
			CTextDecoration td31 = new CTextDecoration(CGlobals.mWebSite,cov31,1,ts);


			RectangleF cov4 = new RectangleF(0.06f,0.73f,0.65f,0.4f);
            CTextDecoration td4 = new CTextDecoration("Slideshow authoring software", cov4, 1, bottomstyle);

			Graphics gp = Graphics.FromImage(this.m_bmp);
			RectangleF im_d = new RectangleF(0,0,m_bmp.Width,m_bmp.Height);

            GDITextDrawer textdrawer = new GDITextDrawer();

		//	td.RenderToGraphics(gp,im_d,0,null);
            textdrawer.DrawString(gp, td.mText, 15, 120, titlestyle);

		//	td2.RenderToGraphics(gp,im_d,0,null);
            textdrawer.DrawString(gp, td2.mText, 20, 175, versionstyle);

		//	td3.RenderToGraphics(gp,im_d,0,null);
            textdrawer.DrawString(gp, td3.mText, 22, 200, copywritestyle);
            
	//		td31.RenderToGraphics(gp,im_d,0,null);
            textdrawer.DrawString(gp, td31.mText, 22, 215, copywritestyle);

			//td4.RenderToGraphics(gp,im_d,0,null);
            textdrawer.DrawString(gp, td4.mText, 20, 260, bottomstyle);
			gp.Dispose();	
		}


        #region Public methods
        // this can be used for About dialogs
        public static void ShowModal(String imageFile, Color col)
        {
            m_imageFile = imageFile;
            m_transColor = col;
            MySplashThreadFunc();
        }
        // Call this method with the image file path and the color 
        // in the image to be rendered transparent
        public static void StartSplash(String imageFile, Color col, bool from_about)
        {
			mFromAbout = from_about;
            m_imageFile = imageFile;
            m_transColor = col;
            // Create and Start the splash thread
            Thread InstanceCaller = new Thread(new ThreadStart(MySplashThreadFunc));
            InstanceCaller.Start();
        }

        // Call this at the end of your apps initialization to close the splash screen
        public static void CloseSplash()
        {
            try
            {
                // m_instance.TopMost = false;

                if (m_instance != null)
                    m_instance.Invoke(m_instance.m_delegateClose);
            }
            catch
            {
            }
        }
        #endregion // Public methods

        #region Dispose
        protected override void Dispose( bool disposing )
        {
            m_bmp.Dispose();
            base.Dispose( disposing );
            m_instance = null;
        }
        #endregion // Dispose

        #region Threading code
        // ultimately this is called for closing the splash window

        void InternalCloseSplash()
        {
            this.Close();
            this.Dispose();
            mDone = true;
        }
        // this is called by the new thread to show the splash screen
        private static void MySplashThreadFunc()
        {
            m_instance = new SplashForm(m_imageFile, m_transColor);
            m_instance.TopMost = false;
            m_instance.ShowDialog();
        }
        #endregion // Multithreading code

        #region Event Handlers

        void SplashForm_MouseClick(object sender, MouseEventArgs e)
        {
            this.InternalCloseSplash();
        }

        private void SplashForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawImage(m_bmp, 0,0);
        }

        private void SplashForm_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
                this.InternalCloseSplash();
        }
        #endregion // Event Handlers

        #region Private variables
        private static SplashForm m_instance;
		private System.Windows.Forms.Button RegisterKeyButton;
		private System.Windows.Forms.Button ContinueButton;
        private static String m_imageFile;
        private static bool mFromAbout = false;

		private void InitializeComponent()
		{
            this.RegisterKeyButton = new System.Windows.Forms.Button();
            this.ContinueButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RegisterKeyButton
            // 
            this.RegisterKeyButton.BackColor = System.Drawing.Color.White;
            this.RegisterKeyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RegisterKeyButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterKeyButton.Location = new System.Drawing.Point(234, 296);
            this.RegisterKeyButton.Name = "RegisterKeyButton";
            this.RegisterKeyButton.Size = new System.Drawing.Size(140, 24);
            this.RegisterKeyButton.TabIndex = 1;
            this.RegisterKeyButton.Text = "Enter registration key";
            this.RegisterKeyButton.UseVisualStyleBackColor = false;
            this.RegisterKeyButton.Click += new System.EventHandler(this.RegisterKeyButton_Click);
            // 
            // ContinueButton
            // 
            this.ContinueButton.BackColor = System.Drawing.Color.White;
            this.ContinueButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ContinueButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContinueButton.Location = new System.Drawing.Point(380, 296);
            this.ContinueButton.Name = "ContinueButton";
            this.ContinueButton.Size = new System.Drawing.Size(100, 24);
            this.ContinueButton.TabIndex = 0;
            this.ContinueButton.Text = "Continue trial";
            this.ContinueButton.UseVisualStyleBackColor = false;
            this.ContinueButton.Click += new System.EventHandler(this.ContinueButton_Click);
            // 
            // SplashForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(496, 342);
            this.Controls.Add(this.ContinueButton);
            this.Controls.Add(this.RegisterKeyButton);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SplashForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

		}
	
		public static bool mDone = false;
        public static bool mClosedApp = false;

        private static Color m_transColor;
        private Bitmap m_bmp;
        private DelegateCloseSplash m_delegateClose;
        #endregion

		private void RegisterKeyButton_Click(object sender, System.EventArgs e)
		{
            RegistrationWindow registration_window = new RegistrationWindow(mFromCD);
            registration_window.ShowDialog();

            if (ManagedCore.License.License.Valid == false && (mFromCD == true || mDownloadNoTrial==true))
            {
                SplashForm.mClosedApp = true;
            }

             this.InternalCloseSplash();
        }

		private void ContinueButton_Click(object sender, System.EventArgs e)
		{
			ManagedCore.License.License.TrialValid = true;
			this.InternalCloseSplash();
		}

		
    }
}
