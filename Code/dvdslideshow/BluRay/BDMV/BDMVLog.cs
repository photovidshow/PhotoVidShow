using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.BDMV
{
    public delegate void BDMVLogDelegate(string value);

    /// <summary>
    /// This class is used to log any BDMV errors, warnings or info messages.
    /// Other classes may add their delegate to the class events to also receive
    /// these messages.
    /// </summary>
    public class BDMVLog
    {
        public static event BDMVLogDelegate FatalErrorCallback;
        public static event BDMVLogDelegate ErrorCallback;
        public static event BDMVLogDelegate WarningCallback;
        public static event BDMVLogDelegate InfoCallback;
        
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="error"></param>
        public static void Error(string error)
        {
            ErrorCallback(error);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="warning"></param>
        public static void Warning(string warning)
        {
            WarningCallback(warning);
        }

        /// <summary>
        /// Fatal error
        /// </summary>
        /// <param name="error"></param>
        public static void FatalError(string error)
        {
            FatalErrorCallback(error);
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="info"></param>
        public static void Info(string info)
        {
            InfoCallback(info);
        }
    }
}
