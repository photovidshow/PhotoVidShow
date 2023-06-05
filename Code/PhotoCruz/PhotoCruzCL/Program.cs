using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ManagedCore;
using DVDSlideshow;
using PhotoCruz;
using System.IO;
using System.Collections;
using MangedToUnManagedWrapper;
using PhotoCruzGui;
using System.Threading;

namespace PhotoCruzCL
{
    class Program
    {
        static private int width = 1280;
        static private int height = 720;
        static private float fps=25;
        static private CGlobals.MP4Quality quality = CGlobals.MP4Quality.Good;
        static private bool doDVD = false;
        static private bool doMp4 = true;
        static private bool doBD = false;
        static private bool useMB = false;
        static private string errorMessage = "";
        static private int errorCode = 0;
        static private string myExeDir;
        static private bool reCreate = false;
        static void Main(string[] args)
        {
            try
            {
                //
                // Supress pop up messages
                //
                ManagedCore.UserMessage.GetInstance().ShowAsMessageBox = false;

                //
                // Add callback to any user message to us instead
                //
                ManagedCore.UserMessage.GetInstance().MessageShown+=new UserMessageCallBackDelegate(UserMessageShow);

                //
                //
                // Force current path to where exe is located (in case it is not already)
                //
                myExeDir = (new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location)).Directory.ToString();


                Directory.SetCurrentDirectory(myExeDir);

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

                if (args.Length < 1)
                {
                    PhotoCruzError("No arguments passed", 1);
                    return;
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
                CDebugLog.GetInstance().LogToLogFile("Running PhotoCruz command line v1.1.1");
                Console.WriteLine("PhotoCruz command line v1.1.1");

                //
                // Load program settings from the installed directory
                //
                CGlobals.LoadProgramSettingsFile();

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


                // Run Command Line PhotoCruz 

                StringBuilder message = new StringBuilder();
                message.Append("Passed parameters:");
                foreach (string s in args)
                {
                    message.Append(" ");
                    message.Append(s);
                }

                Console.WriteLine(message);

                CDebugLog.GetInstance().LogToLogFile(message.ToString());
                if (ReadArguments(args) == false)
                {
                    return;
                }

                Form f = new PhotoCruzCLWindowHandle(width, height, fps, quality, useMB, doMp4, doDVD, doBD, reCreate);
                f.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                f.ShowInTaskbar = false;
                f.StartPosition = FormStartPosition.Manual;
                f.Location = new System.Drawing.Point(-2000, -2000);
                f.Size = new System.Drawing.Size(1, 1);

                Application.Run(f);
            }
            catch (Exception e)
            {
                PhotoCruzError("Exception occured:" + e.Message + "\r\n" + e.StackTrace, 4);
            }
            finally
            {
                WriteVideoFile();
            }
        }

        private static void UserMessageShow(string text, string caption)
        {
            PhotoCruzError(text, 7);
        }


        private static void WriteVideoFile()
        {
            try
            {
                StreamReader reader = File.OpenText(myExeDir + "\\dvd\\video");
                string project_name = reader.ReadLine();
                string line2 = reader.ReadLine();
                if (line2 != null)
                {
                    string line2upper = line2.ToUpper();
                    if (line2upper.Contains("RECREATE") == false)
                    {
                        line2 = null;
                    }
                }
                reader.Close();
                reader.Dispose();

                StreamWriter writer = new StreamWriter(myExeDir + "\\dvd\\video");
                if (project_name != null)
                {
                    writer.WriteLine(project_name);
                }
                if (line2 != null)
                {
                    writer.WriteLine(line2);
                }
                if (errorCode != 0)
                {
                    writer.WriteLine(errorCode.ToString());
                    writer.WriteLine(errorMessage);
                }
                writer.Close();
                writer.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to re-write video file: " + e.Message);
                return;
            }
        }


        private static bool ReadArguments(string[] args)
        {
            bool result = true;

            for (int i = 0; i < args.Length - 1 && result == true; i+=2)
            {
                string command = args[i].ToLower();
                string parameter = args[i + 1].ToLower();

                if (command == "--resolution")
                {
                    result = DoResolution(parameter);
                }
                else if (command == "--fps")
                {
                    result = DoFps(parameter);
                }
                else if (command == "--quality")
                {
                    result = DoQuality(parameter);
                }
                else if (command == "--motionblur")
                {
                    result = DoMotionBlur(parameter);
                }
                else if (command == "--output")
                {
                    result = DoOutput(parameter);
                }
                else if (command == "--recreate")
                {
                    reCreate = true;
                }
                else 
                {
                    PhotoCruzError("Unknown command:" + command, 2);
                    result = false;
                }
            }
            return result;
        }

        private static bool DoFps(string parameter)
        {
            try
            {
                fps = float.Parse(parameter);
            }
            catch
            {
                PhotoCruzError("Unknown resolution:" + parameter, 2);
                return false;
            }
            return true;
        }


        private static bool DoResolution(string parameter)
        {
            if (parameter == "720p")
            {
                width = 1280;
                height = 720;

            }
            else if (parameter == "1080p")
            {
                width = 1920;
                height = 1080;
            }
            else
            {
                try
                {
                    string[] res = parameter.Split('x');
                    if (res != null && res.Length == 2)
                    {
                        width = int.Parse(res[0]);
                        height = int.Parse(res[1]);
                    }
                }
                catch
                {
                    PhotoCruzError("Unknown resolution:" + parameter, 2);
                    return false;
                }
            }
            return true;
        }

        private static bool DoQuality(string parameter)
        {
            if (parameter == "good")
            {
                quality = CGlobals.MP4Quality.Good;
            }
            else if (parameter == "high") 
            {
                quality = CGlobals.MP4Quality.High;
            }
            else if (parameter == "maximum")
            {
                quality = CGlobals.MP4Quality.Maximum;
            }
            else
            {
                PhotoCruzError("Unknown quality:" + parameter, 2);
                return false;
            }
            return true;
        }

        private static bool DoMotionBlur(string parameter)
        {
            if (parameter == "on")
            {
                useMB = true;
            }
            else if (parameter == "off")
            {
                useMB = false;
            }
            else
            {
                PhotoCruzError("Unknown motion blur parameter:" + parameter, 2);
                return false;
            }
            return true;
        }

        private static bool DoOutput(string parameters)
        {
            string[] outputs = parameters.Split(',');

            //
            // ok set outputs, remove default settins
            //
            doMp4 = false;
            doDVD = false;
            doBD = false;

            foreach (string output in outputs)
            {
                if (output.Contains("dvd") == true)
                {
                    doDVD = true;
                }
                else if (output.Contains("mp4") == true)
                {
                    doMp4 = true;
                }
                else if (output.Contains("bd") == true)
                {
                    doBD = true;
                }
                else
                {
                    PhotoCruzError("Unknown output type in parameters:" + output, 2);
                    return false;
                }
            }
            return true;
        }

        private static void SetUpDirectories(string root)
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
                PhotoCruzError("Setting up the initial directories threw an exception :" + e2.Message, 4);
            }
        }

        public static void PhotoCruzError(string message, int number)
        {
            Console.WriteLine(message);
            ManagedCore.Log.Error(message);

            if (errorMessage == "")
            {
                errorMessage = message;
                errorCode = number;
            }
        }  
    }
}
