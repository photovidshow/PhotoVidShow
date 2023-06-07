using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using ManagedCore;
using DVDSlideshow;
using DVDSlideshow.Memento;
using MangedToUnManagedWrapper;
using Microsoft.Win32;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using dvdslideshowfontend;


namespace PhotoVidShow
{
    class PhotoVidShow
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Set unmanaged log callbacks
            MangedToUnManagedWrapper.CManagedErrors.SetUnmanagedCallbacks();
            DVDSlideshow.BluRay.BluRayErrors.SetCallbacks();

            Application.EnableVisualStyles();
            Application.DoEvents();
            string open_with_file = "";

            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (args[1].ToLower().EndsWith(".pds") || args[1].ToLower().EndsWith(".pds2" ))
                {
                    open_with_file = args[1];
                    string app_dir = System.IO.Path.GetDirectoryName(System.Environment.GetCommandLineArgs()[0]);
                    CGlobals.SetUpDirectories(app_dir);
                    System.IO.Directory.SetCurrentDirectory(app_dir);
                }
                else // unknown type
                {
                    MessageBox.Show("Trying to run PhotoVidShow with an unknown file type.", "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    return;
                }
            }
            else
            {
                CGlobals.SetUpDirectories(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
            }

            //
            // If PhotoVidShow already running return, only one instance allowed to run at once
            //
            /*
            String appname = System.IO.Path.GetFileNameWithoutExtension(System.Environment.GetCommandLineArgs()[0]);

            System.Diagnostics.Process[] RunningProcesses = System.Diagnostics.Process.GetProcessesByName(appname);

            if (RunningProcesses.Length > 1)
            {
                BringProcessToFront(RunningProcesses[0]);
                return;
            }
            */

            //
            // Set up log filename
            //
            StringBuilder logfileBuilder = new StringBuilder();
            logfileBuilder.Append(Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToString());
            logfileBuilder.Append("\\PhotoVidShow\\log.txt");
            CDebugLog.GetInstance().LogFileFileName = logfileBuilder.ToString();

            //
            // Ok if we are here we now run the editor
            //
            CDebugLog.GetInstance().LogToLogFile("Started PhotoVidShow...");


            //
            // Load program settings from the installed directory
            //
            CGlobals.LoadProgramSettingsFile();
            CDebugLog.GetInstance().LogToLogFile("Running version " + CGlobals.VersionNumber().ToString());

            //
            // If we created the log.txt file for the first time in this session, dump our system info as well
            //
            if (CDebugLog.GetInstance().CreatedLogFileThisSession() == true)
            {
                CSystem system = new CSystem();
                CDebugLog.GetInstance().LogToLogFile(system.GetSystemInfo());
            }

            //
            // Load user settings from user directory
            //
            PhotoVidShowSettings settings = new PhotoVidShowSettings();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;
  
            ManagedCore.CDebugLog.GetInstance().DebugMode = CGlobals.mIsDebug;
            ManagedCore.Updater.mVersion = CGlobals.VersionNumber();

            if (CGlobals.RunningSteamVersion == true)
            {
                CDebugLog.GetInstance().LogToLogFile("Running steam version");

                uint steamAppIdd = 909460; // 940030 = demo  ,  909460 = retail

                if (steamAppIdd == 940030)
                {
                    CGlobals.RunningSteamDemo = true;
                }

                CSteamAPI steam = new CSteamAPI();
                if (steam.RestartAppIfNecessary(steamAppIdd) == true)   
                {
                    //
                    // Exit application, steam will now launch it from steam (checks valid licence first)
                    //
                    return;
                }
            }

            /*  SRG removed windows store version
            // If running Windows app store version
            if (CGlobals.RunningWindowsStoreVersion == true)
            {
                CWindowsStore st = new CWindowsStore();
                st.Initialise();
                bool trial = st.IsTrial();
                CGlobals.RunningWindowsStoreTrial = trial;
            }
            */

#if (DEBUG)
            Form1 mainWindow = new Form1(open_with_file);
            if (SplashForm.mClosedApp == false)
            {
                Application.Run(mainWindow);
            }

            CDebugLog.GetInstance().LogToLogFile("Closed PhotoVidShow");

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

                    Form1 mainWindow = new Form1(open_with_file);
                    if (SplashForm.mClosedApp == false)
                    {
                        Application.Run(mainWindow);
                    }
				}
				catch (Exception exception)
				{
                    string ss = "An unexpected error has occurred 3! - ";

                    StringBuilder builder = new StringBuilder();
                    builder.Append(ss);
                    builder.Append("\r\n\r\n");
                    builder.Append(exception.Message);
                    builder.Append("\r\n\r\n");
                    builder.Append(exception.GetType());
                    builder.Append("\r\n\r\nStack Trace:\r\n");
                    builder.Append(exception.StackTrace);

                    string message = builder.ToString();
                    Log.Error(message);

                    if (CGlobals.mShowErrorStack == true)
                    {
                        MessageBox.Show(message, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        MessageBox.Show(ss, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                    }
				}
			}
#endif
        }

        // ****************************************************************************************************
        internal class CustomExceptionHandler
        {
            public void OnThreadException(object sender, ThreadExceptionEventArgs t)
            {
                // some weird windows error think this is long gone now
                if (t.Exception.StackTrace.IndexOf("System.Windows.Forms.ChildWindow") >= 0)
                {
                    return;
                }

                string ss = "An unexpected error has occurred 2!";

                StringBuilder builder = new StringBuilder();
                builder.Append(ss);
                builder.Append("\r\n\r\n");
                builder.Append(t.Exception.Message);
                builder.Append("\r\n\r\n");
                builder.Append(t.Exception.GetType());
                builder.Append("\r\n\r\nStack Trace:\r\n");
                builder.Append(t.Exception.StackTrace);

                string message = builder.ToString();

                Log.Error(message);

                if (CGlobals.mShowErrorStack == true)
                {
                    MessageBox.Show(message, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show(ss, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
            }
        }

        // ****************************************************************************************************
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception t = (Exception)e.ExceptionObject;

            // some weird windows error think this is long gone now
            if (t.StackTrace.IndexOf("System.Windows.Forms.ChildWindow") >= 0)
            {
                return;
            }

            string ss = "An unexpected error has occurred 1!";

            StringBuilder builder = new StringBuilder();
            builder.Append(ss);
            builder.Append("\r\n\r\n");
            builder.Append(t.Message);
            builder.Append("\r\n\r\n");
            builder.Append(t.GetType());
            builder.Append("\r\n\r\nStack Trace:\r\n");
            builder.Append(t.StackTrace);

            string message = builder.ToString();

            Log.Error(message);

            if (CGlobals.mShowErrorStack == true)
            {
                MessageBox.Show(message, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show(ss, "Application error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
        }


        //	Use this piece of Code
        // Rather then TestApp use ur process name


        // ****************************************************************************************************
        private const int SW_RESTORE = 9;

        static public void BringProcessToFront(System.Diagnostics.Process pr)
        {
            if (pr != null)
            {
                IntPtr hWnd = pr.MainWindowHandle;
                // restore the window 
                Form1.ShowWindowAsync(hWnd, SW_RESTORE);
                // show the window or activate it as foreground window
                bool result = Form1.SetForegroundWindow(hWnd);
                if (!result)
                {// if u need to display message
                    //MessageBox.Show("Can not set in Foreground.");
                }
            }
            return;
        }
    }
}
