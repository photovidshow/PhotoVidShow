using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ManagedCore;
using DVDSlideshow;
using PhotoCruz;
using System.IO;
using System.Collections;
using MangedToUnManagedWrapper;
using PhotoCruzGui;
using System.Threading;

namespace PhotoCruzMain
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CGlobals.RunningPhotoCruz = true;

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            CGlobals.SetUserDirectory(Environment.CurrentDirectory);
            SetUpDirectories(Environment.CurrentDirectory);

            CGlobals.mShowErrorStack = true;

            // photocruz does not need a license file to run
            ManagedCore.License.License.Valid = true;

            // Set unmanaged log callbacks
            MangedToUnManagedWrapper.CManagedErrors.SetUnmanagedCallbacks();
            DVDSlideshow.BluRay.BluRayErrors.SetCallbacks();

            CGlobals.mDefaultMenuTemplate = CGlobals.GetRootDirectory() + "\\dvd\\motion menu.mpg";

            Application.EnableVisualStyles();
            Application.DoEvents();
            string open_with_file = "";

            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (args[1].ToLower().EndsWith(".pds"))
                {

                    open_with_file = args[1];
                    string app_dir = System.IO.Path.GetDirectoryName(System.Environment.GetCommandLineArgs()[0]);
                    SetUpDirectories(app_dir);
                }
                else // unknown type
                {
                    UserMessage.Show("Trying to run PhotoCruz with an unknown file type.", "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    return;
                }
            }
           
            String appname = System.IO.Path.GetFileNameWithoutExtension(System.Environment.GetCommandLineArgs()[0]);

            System.Diagnostics.Process[] RunningProcesses = System.Diagnostics.Process.GetProcessesByName(appname);


            //
            // Set up log filename
            //
            CDebugLog.GetInstance().LogFileFileName = CGlobals.GetUserDirectory() + "\\log.txt";

            //
            // Ok if we are here we now run the editor
            //
            CDebugLog.GetInstance().LogToLogFile("Started PhotoCruz...");

            //
            // Load program settings from the installed directory
            //
            CGlobals.LoadProgramSettingsFile();
            CDebugLog.GetInstance().LogToLogFile("Running version " + "2.4.1");

            //
            // If we created the log.txt file for the first time in this session, dump our system info as well
            //
            if (CDebugLog.GetInstance().CreatedLogFileThisSession() == true)
            {
                CSystem system = new CSystem();
                CDebugLog.GetInstance().LogToLogFile(system.GetSystemInfo());
            }

            ManagedCore.CDebugLog.GetInstance().DebugMode = CGlobals.mIsDebug;
            ManagedCore.Updater.mVersion = CGlobals.VersionNumber();

            //
            // Load user settings from user directory
            //
            PhotoCruzSettings settings = new PhotoCruzSettings();

#if (DEBUG)
            Application.Run(new PhotoCruzMainWindow(ConverterType.PHOTOCRUZ_CONVERTER));
           
#else
            //	if (CGlobals.mIsDebug==true)
		//	{
		//		Application.Run(new Form1());
		//	} 
		//	else
			{
				try
		    	{ 
					CustomExceptionHandler eh = new CustomExceptionHandler(); 
					Application.ThreadException += new ThreadExceptionEventHandler(eh.OnThreadException);

                    AppDomain.CurrentDomain.UnhandledException +=
                        new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


					Application.Run(new PhotoCruzMainWindow(ConverterType.PHOTOCRUZ_CONVERTER));
				}
				catch 
				{
					UserMessage.Show("An unexpected error has occurred!","Application error",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Exclamation);
				}
			}
#endif
        }

        public static void SetUpDirectories(string root)
        {
            ManagedCore.IO.SetRootDirectory(root);

            // set up temp;

            try
            {
                string temp = CGlobals.GetTempDirectory();

                MangedToUnManagedWrapper.CManagedMemoryBufferCache.set_temp_directory(temp);

                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

                temp = CGlobals.GetTempDirectory() + "\\dvdfolder";

                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

                temp = CGlobals.GetTempDirectory() + "\\blurayfolder";

                if (System.IO.Directory.Exists(temp) == false)
                {
                    System.IO.Directory.CreateDirectory(temp);
                }

            }
            catch (Exception e2)
            {
                CDebugLog.GetInstance().Error("Setting up the initial directories threw an exception :" + e2.Message);
            }
        }

        // Handles the exception event for all other threads

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception t = (Exception)e.ExceptionObject;

            bool skip = false;
            if (t.StackTrace.IndexOf("System.Windows.Forms.ChildWindow") >= 0)
            {
                skip = true;
            }

            string ss = "PhotoCruz: An unexpected error has occurred!";

            if (skip == true) ss += " (skipped) ";

            if (CGlobals.mShowErrorStack == true)
            {
                UserMessage.Show(ss + "\n \n" + t.Message + "\n\n" + t.GetType() +
                    "\n\nStack Trace:\n" +
                    t.StackTrace, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
            else if (skip == false)
            {
                UserMessage.Show(ss, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
        }

        // Handles the exception event for all other threads


        internal class CustomExceptionHandler
        { 
            public void OnThreadException(object sender, ThreadExceptionEventArgs t)
            {
                bool skip = false;
                if (t.Exception.StackTrace.IndexOf("System.Windows.Forms.ChildWindow") >= 0)
                {
                    skip = true;
                }

                string ss = "PhotoCruz: An unexpected error has occured!";

                if (skip == true) ss += " (skipped) ";

                if (CGlobals.mShowErrorStack == true)
                {
                    UserMessage.Show(ss + "\n \n" + t.Exception.Message + "\n\n" + t.Exception.GetType() +
                        "\n\nStack Trace:\n" +
                        t.Exception.StackTrace, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
                else if (skip == false)
                {
                    UserMessage.Show(ss, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}
