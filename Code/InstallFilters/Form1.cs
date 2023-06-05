using System;
using System.Text;
using System.Management;


// *********************************************************************************************************
// BUILD THIS AS ANY CPU
// *********************************************************************************************************


namespace InstallFilters
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			this.Size = new System.Drawing.Size(300,300);
			this.Text = "Form1";
		}
        #endregion


        //************************************************************************************
        private static string GetInfo(string category, string id)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("SELECT ");
            builder.Append(id);
            builder.Append(" FROM Win32_");
            builder.Append(category);

            string searchString = builder.ToString();

            string result = string.Empty;

            try
            {

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(searchString);
                foreach (ManagementObject os in searcher.Get())
                {
                    result = os[id].ToString();
                    break;
                }
            }
            catch
            {
            }

            return result;
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string [] args) 
		{
			string exe_path = Environment.CurrentDirectory;

            string installType = @"/s ";

			if (args.Length >0)
			{       
                if (args[0] == "-u")
                {
                    installType = @"/u /s ";

                    string pathtodelete = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    Environment.CurrentDirectory = pathtodelete;
                    
                }
                else
                {
                    exe_path = args[0];
                    for (int i = 1; i < args.Length; i++)
                    {
                        exe_path += " " + args[i];
                    }
                    //				MessageBox.Show("running regsvr32.exe with "+@"/s " + exe_path+@"wavdest.ax");

                    Environment.CurrentDirectory = exe_path;                   
                }
			}

			//System.Diagnostics.Process.Start("regsvr32.exe", installType+"wavdest.ax");
            System.Diagnostics.Process.Start("regsvr32.exe", installType + @"DVDVideoBurnerTool\starburnx.dll");
            
            //
            // If running Vista, install video lav filers as well
            //
            string os = GetInfo("OperatingSystem", "Caption");
            if (os.ToLower().Contains("vista") == true)
            {
                System.Diagnostics.Process.Start("regsvr32.exe", installType + @"dsfilters\lav\LAVAudio.ax");
                System.Diagnostics.Process.Start("regsvr32.exe", installType + @"dsfilters\lav\LAVVideo.ax");
                System.Diagnostics.Process.Start("regsvr32.exe", installType + @"dsfilters\lav\LAVSplitter.ax");
            }

            Environment.CurrentDirectory = exe_path;
		}
	}
}
