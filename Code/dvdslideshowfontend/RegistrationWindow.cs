using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DVDSlideshow;
using System.Text.RegularExpressions;
using ManagedCore;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for RegistrationWindow.
	/// </summary>
	public class RegistrationWindow : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox RegistrationCodeTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Button EnterButton;
        private System.Windows.Forms.Label mExampleLabel;
        public Label mEnterRegistraionKetLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox EmailAddress;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label PleaseVistWebSiteLabel;
        private PictureBox mKeyPictureBox;
		private bool mValidEmail=false;
        private Label mExampleLabel2;
        private Label mServerStatusLabel;
        private Label label3;
        private bool mFromCD = false;

		public RegistrationWindow(bool fromCD)
		{
            mFromCD = fromCD;

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            if (mFromCD == true)
            {
                mEnterRegistraionKetLabel.Text = "Enter your email and registration key as printed on the CD case.";
            }

            float size = (9.75f / (CGlobals.dpiX / 96.0f));
            this.RegistrationCodeTextBox.Font = new Font("Courier New", size, FontStyle.Bold);

			this.EnterButton.Enabled=false;

		    this.TransparencyKey = Color.FromArgb(128,128,128,128);
			this.PleaseVistWebSiteLabel.BackColor = Color.FromArgb(0,128,128,128);

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            this.RegistrationCodeTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.EnterButton = new System.Windows.Forms.Button();
            this.mExampleLabel = new System.Windows.Forms.Label();
            this.PleaseVistWebSiteLabel = new System.Windows.Forms.Label();
            this.mEnterRegistraionKetLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.EmailAddress = new System.Windows.Forms.TextBox();
            this.mKeyPictureBox = new System.Windows.Forms.PictureBox();
            this.mExampleLabel2 = new System.Windows.Forms.Label();
            this.mServerStatusLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mKeyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // RegistrationCodeTextBox
            // 
            this.RegistrationCodeTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.RegistrationCodeTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegistrationCodeTextBox.Location = new System.Drawing.Point(124, 148);
            this.RegistrationCodeTextBox.MaxLength = 32;
            this.RegistrationCodeTextBox.Name = "RegistrationCodeTextBox";
            this.RegistrationCodeTextBox.Size = new System.Drawing.Size(264, 22);
            this.RegistrationCodeTextBox.TabIndex = 2;
            this.RegistrationCodeTextBox.WordWrap = false;
            this.RegistrationCodeTextBox.TextChanged += new System.EventHandler(this.RegistrationCodeTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Registration key";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CancelButton
            // 
            this.CancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButton.Location = new System.Drawing.Point(313, 239);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = false;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // EnterButton
            // 
            this.EnterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.EnterButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.EnterButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnterButton.Location = new System.Drawing.Point(227, 239);
            this.EnterButton.Name = "EnterButton";
            this.EnterButton.Size = new System.Drawing.Size(80, 23);
            this.EnterButton.TabIndex = 3;
            this.EnterButton.Text = "Register";
            this.EnterButton.UseVisualStyleBackColor = false;
            this.EnterButton.Click += new System.EventHandler(this.EnterButton_Click);
            // 
            // mExampleLabel
            // 
            this.mExampleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mExampleLabel.Location = new System.Drawing.Point(121, 173);
            this.mExampleLabel.Name = "mExampleLabel";
            this.mExampleLabel.Size = new System.Drawing.Size(267, 22);
            this.mExampleLabel.TabIndex = 5;
            this.mExampleLabel.Text = "The registration key is made up of 32 characters";
            this.mExampleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.mExampleLabel.Click += new System.EventHandler(this.StatusLabel_Click);
            // 
            // PleaseVistWebSiteLabel
            // 
            this.PleaseVistWebSiteLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PleaseVistWebSiteLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PleaseVistWebSiteLabel.ForeColor = System.Drawing.Color.Black;
            this.PleaseVistWebSiteLabel.Location = new System.Drawing.Point(11, 62);
            this.PleaseVistWebSiteLabel.Name = "PleaseVistWebSiteLabel";
            this.PleaseVistWebSiteLabel.Size = new System.Drawing.Size(249, 18);
            this.PleaseVistWebSiteLabel.TabIndex = 7;
            this.PleaseVistWebSiteLabel.Text = "For any problem please visit";
            // 
            // mEnterRegistraionKetLabel
            // 
            this.mEnterRegistraionKetLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mEnterRegistraionKetLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mEnterRegistraionKetLabel.Location = new System.Drawing.Point(10, 16);
            this.mEnterRegistraionKetLabel.Name = "mEnterRegistraionKetLabel";
            this.mEnterRegistraionKetLabel.Size = new System.Drawing.Size(297, 64);
            this.mEnterRegistraionKetLabel.TabIndex = 8;
            this.mEnterRegistraionKetLabel.Text = "Enter your email and registration key";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Location = new System.Drawing.Point(11, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 23);
            this.label4.TabIndex = 9;
            this.label4.Text = "Email Address";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EmailAddress
            // 
            this.EmailAddress.BackColor = System.Drawing.SystemColors.Window;
            this.EmailAddress.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailAddress.Location = new System.Drawing.Point(124, 116);
            this.EmailAddress.Name = "EmailAddress";
            this.EmailAddress.Size = new System.Drawing.Size(264, 22);
            this.EmailAddress.TabIndex = 1;
            this.EmailAddress.TextChanged += new System.EventHandler(this.EmailAddress_TextChanged);
            // 
            // mKeyPictureBox
            // 
            this.mKeyPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.mKeyPictureBox.BackgroundImage = global::dvdslideshowfontend.Properties.Resources.Actions_document_decrypt_icon_medium;
            this.mKeyPictureBox.Location = new System.Drawing.Point(313, 16);
            this.mKeyPictureBox.Name = "mKeyPictureBox";
            this.mKeyPictureBox.Size = new System.Drawing.Size(64, 64);
            this.mKeyPictureBox.TabIndex = 12;
            this.mKeyPictureBox.TabStop = false;
            // 
            // mExampleLabel2
            // 
            this.mExampleLabel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mExampleLabel2.Location = new System.Drawing.Point(121, 192);
            this.mExampleLabel2.Name = "mExampleLabel2";
            this.mExampleLabel2.Size = new System.Drawing.Size(267, 22);
            this.mExampleLabel2.TabIndex = 13;
            this.mExampleLabel2.Text = "e.g \'4M7C9B2563456KE2KFC8278D2EBM9K78\'";
            this.mExampleLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mServerStatusLabel
            // 
            this.mServerStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mServerStatusLabel.Location = new System.Drawing.Point(11, 239);
            this.mServerStatusLabel.Name = "mServerStatusLabel";
            this.mServerStatusLabel.Size = new System.Drawing.Size(201, 22);
            this.mServerStatusLabel.TabIndex = 15;
            this.mServerStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(11, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(249, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "www.photovidshow.com";
            // 
            // RegistrationWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(402, 272);
            this.ControlBox = false;
            this.Controls.Add(this.mServerStatusLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mExampleLabel2);
            this.Controls.Add(this.PleaseVistWebSiteLabel);
            this.Controls.Add(this.mKeyPictureBox);
            this.Controls.Add(this.EmailAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mEnterRegistraionKetLabel);
            this.Controls.Add(this.mExampleLabel);
            this.Controls.Add(this.EnterButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RegistrationCodeTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegistrationWindow";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Register PhotoVidShow software";
            ((System.ComponentModel.ISupportInitialize)(this.mKeyPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void CancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void RegistrationCodeTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (this.RegistrationCodeTextBox.Text.Length==32 && 
				mValidEmail==true)
			{
				this.EnterButton.Enabled=true;
			}
			else
			{
				this.EnterButton.Enabled=false;
			}
		}

		private void EnterButton_Click(object sender, System.EventArgs e)
		{
			bool could_not_connect=false;

			//	Console.WriteLine("Check key");
			if (this.RegistrationCodeTextBox.Text.Length==32)
			{
                string correctedkey = RegistrationCodeTextBox.Text;
                correctedkey = correctedkey.Replace('M', '0');
                correctedkey = correctedkey.Replace('K', '1');

                ManagedCore.License.CActivateKey key = new ManagedCore.License.CActivateKey(correctedkey);
				this.mServerStatusLabel.Text ="Checking server...";
				this.mServerStatusLabel.Refresh();

				this.CancelButton.Enabled=false;
				this.EnterButton.Enabled=false;

				// alpha build remove until i can protect assemblies
				if (key.IsValid(1,ref could_not_connect)==true)
				{	
					ManagedCore.CMacAddress mc = new ManagedCore.CMacAddress();
					ManagedCore.License.CSoftwareKey soft_key = key.GenerateSoftkey(mc,1);
					soft_key.Save(CGlobals.GetUserDirectory()+"//photovidshow run config(do not delete).txt");

					// ok generate second sfot key from host name, as some people have problems with mac
					// names

					string name =System.Environment.MachineName;
					
					ManagedCore.License.CSoftwareKey soft_key2 = key.GenerateSoftkey(name,1);
                    soft_key2.Save(CGlobals.GetUserDirectory() + "//photovidshow run config2(do not delete).txt");

					ManagedCore.License.License.Valid = true;
					this.CancelButton.Enabled=false;
					this.EnterButton.Enabled=false;
					try
					{
						ManagedCore.CNetFile netfile = new ManagedCore.CNetFile("");
						int key_num = key.GetOriginalKeyNumber("",1);
						netfile.CallWebCGIScript("register.pl","regnum="+key_num.ToString()+" "+this.EmailAddress.Text+" "+CGlobals.CompleteVersionString()+" "+name);
					}
					catch
					{
					}


				}


				// srg this is allowed bacuase soft key has mac address embedded in it
				else
				{
					// check for admin software key

					ManagedCore.License.CSoftwareKey soft_key = new ManagedCore.License.CSoftwareKey(correctedkey);
					int key_num=0;
					if (soft_key.IsValidForThisComputer(1, ref key_num)==true)
					{
						ManagedCore.License.License.Valid = true;
						this.CancelButton.Enabled=false;
						this.EnterButton.Enabled=false;
						soft_key.Save(CGlobals.GetUserDirectory()+"//license.txt");
					}
				}
				
			}

			this.mServerStatusLabel.Text ="";

			if (ManagedCore.License.License.Valid ==false)
			{
				if ( could_not_connect==true)
				{
                    UserMessage.Show("Could not connect to server.  Check internet is connected and the firewall is not blocking this application.", "Could not connect",
						System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
				}
				else
				{
                    UserMessage.Show("This license key is not valid!", "Invalid key",
						System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
				}
				this.CancelButton.Enabled=true;
				this.EnterButton.Enabled=true;
			}
			else
			{
                if (mFromCD == false)
                {
                    UserMessage.Show("This license key is valid!\n\r\n\rPhotoVidShow has now switched to full mode.\n\r\n\rPLEASE backup your license key in case you need it again in the future.", "Valid key",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }
                else
                {
                    UserMessage.Show("This license key is valid!\n\r\n\rPLEASE keep your CD safe in case you need it again in the future.", "Valid key",
                      System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                }

				this.Close();
			}
		}

		public bool isEmail(string inputEmail)
		{
		//	inputEmail  = NulltoString(inputEmail);
			string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
				@"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + 
				@".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
			Regex re = new Regex(strRegex);
			if (re.IsMatch(inputEmail))
				return (true);
			else
				return (false);
		}

		private void EmailAddress_TextChanged(object sender, System.EventArgs e)
		{
			mValidEmail = isEmail(this.EmailAddress.Text);
			RegistrationCodeTextBox_TextChanged(sender,e);
		}

		private void StatusLabel_Click(object sender, System.EventArgs e)
		{
		
		}
    }
}
