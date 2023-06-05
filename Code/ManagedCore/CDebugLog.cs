using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Globalization;

namespace ManagedCore
{
    /// <summary>
    /// Summary description for CDebugLog.
    /// </summary>
    /// 

    public interface ILogHook
    {
        void DeclareEntry(string e);
    }
     
    //
    //  Shortcut static class to debug log
    //
    public class Log
    {
        public static void Error(string error)
        {
            CDebugLog.GetInstance().Error(error);
        }

        public static void Warning(string warning)
        {
            CDebugLog.GetInstance().Warning(warning);
        }

        public static void FatalError(string error)
        {
            CDebugLog.GetInstance().FatalError(error);
        }

        public static void Info(string info)
        {
            CDebugLog.GetInstance().Trace(info);
        }
    }

    public class CDebugLog
    {
        private static CDebugLog mTheLog;

        public enum LoggingLevel
        {
            FatalError,
            Error,
            Warning,
            Info
        }

        // developer debug mode
        private bool mDebugMode = false;

        public bool DebugMode
        {
            get { return mDebugMode; }
            set { mDebugMode = value; }
        }

        // do we want to display the checkpoint system when encoding
        private bool mTraceEncode = false;

        public bool TraceEncode
        {
            get { return mTraceEncode; }
            set { mTraceEncode = value; }
        }

        private LoggingLevel mLogLevel = LoggingLevel.FatalError;

        public LoggingLevel LogLevel
        {
            get { return mLogLevel; }
            set { mLogLevel = value; }
        }

        private object mEncodeLogLock = new Object();
        private ArrayList mHooks;
        private ArrayList mEncodeCheckpointList = new ArrayList();
        private bool mCreatedLogFileThisSession = false; // Indicates if we creates the log file this session
        private bool mLogFileExists = false;    // Indicates if we know the log files already exists
        private string mLogFileFilename = "";

        public string LogFileFileName
        {
            get { return mLogFileFilename; }
            set { mLogFileFilename = value; }
        }

        //*******************************************************************
        private CDebugLog()
        {
            mHooks = new ArrayList();
        }

        //*******************************************************************
        public static CDebugLog GetInstance()
        {
            if (mTheLog == null)
            {
                mTheLog = new CDebugLog();
            }

            return mTheLog;
        }

        //*******************************************************************
        // method to retrieve the stack of checkpoints, this method typically
        // called in GUI and is in a different thread to the one modifying the
        // checkpoints list.
        public ArrayList EncodeCheckPointList
        {
            get
            {
                ArrayList new_list = new ArrayList();
                // lock list and make copy of list before releasing it
                lock (mEncodeLogLock)
                {
                    // do full clone
                    foreach (char c in mEncodeCheckpointList)
                    {
                        new_list.Add(c);
                    }
                }

                // return copied list
                return new_list;
            }
        }

        //*******************************************************************
        // This functiom is called when ever in encoding reaches a checkpoint
        public void DebugAddEncodePosition(char pos)
        {
            // lock list whilst modifing
            lock (mEncodeLogLock)
            {
                if (mEncodeCheckpointList.Count >= 25)
                {
                    mEncodeCheckpointList.RemoveAt(0);
                }

                mEncodeCheckpointList.Add(pos);
            }
        }

        //*******************************************************************
        public void LogToLogFile(string message)
        {
            if (LogFileFileName == "")
            {
                return;
            }

            try
            {
                string log_file = LogFileFileName;

                if (mLogFileExists == false)
                {
                    if (System.IO.File.Exists(log_file) == false)
                    {
                        FileStream fs = File.Create(log_file);

                        fs.Close();
                        mCreatedLogFileThisSession = true;
                    }
                    mLogFileExists = true;
                }

                //
                // Get the current culture
                //
                CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;

                //
                // Set the culture to GB i.e. English uk
                //
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");

                System.IO.StreamWriter sr = new StreamWriter(log_file, true);
                DateTime now = DateTime.Now;
                sr.Write(now.ToShortDateString());
                sr.Write(" ");
                sr.Write(now.ToLongTimeString());
                sr.Write(" ");
                sr.WriteLine(message);
                sr.Close();

                //
                // Restore original culture.
                //
                Thread.CurrentThread.CurrentCulture = originalCulture;

            }
            catch
            {
            }
        }

        //*******************************************************************
        public bool CreatedLogFileThisSession()
        {
            return mCreatedLogFileThisSession;
        }

        //*******************************************************************
        public void AddHook(ILogHook hook)
        {
            mHooks.Add(hook);
        }

        //*******************************************************************
        public void Trace(string e)
        {
            if (mLogLevel >= LoggingLevel.Info) LogToLogFile(e);
            if (mDebugMode == false) return;
            AddLogEntry(e);
        }

        object mylock = new object();

        //*******************************************************************
        public void Error(string e)
        {
            if (mLogLevel < LoggingLevel.Error)
            {
                return;
            }
            lock (mylock)
            {
                if (mLogLevel >= LoggingLevel.Error) LogToLogFile("Error: " + e);
                if (mDebugMode == false) return;
                string se = "Error: " + e;
                AddLogEntry(se);
            }
        }

        //*******************************************************************
        public void Warning(string e)
        {
            if (mLogLevel >= LoggingLevel.Warning) LogToLogFile("Warning: " + e);
            if (mDebugMode == false) return;
            string se = "Warning: " + e;
            AddLogEntry(se);
        }

        //*******************************************************************
        public void FatalError(string e)
        {
            string se = "Fatal Error: " + e;

            if (mLogLevel >= LoggingLevel.FatalError) LogToLogFile(se);

            if (mDebugMode == true)
            {
                AddLogEntry(se);
            }
            UserMessage.Show(e, "Fatal Error",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        //*******************************************************************
        private void AddLogEntry(string entry)
        {
            if (mDebugMode == false) return;
            foreach (ILogHook h in mHooks)
            {
                h.DeclareEntry(entry);
            }

            Console.WriteLine(entry);
        }

        //*******************************************************************
        public void PopupError(string error)
        {
            UserMessage.Show(error, "Error",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }


        //*******************************************************************
        public void PopupWarning(string error)
        {
            UserMessage.Show(error, "Warning",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }
    }
}
