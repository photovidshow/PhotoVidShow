using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.AVCHD
{
    public delegate void AVCHDLogDelegate(string value);

    /// <summary>
    /// This class is used to log any AVCHD errors, warnings or info messages.
    /// Other classes may add their delegate to the class events to also receive
    /// these messages.
    /// </summary>
    public class AVCHDLog
    {
        public static event AVCHDLogDelegate FatalErrorCallback;
        public static event AVCHDLogDelegate ErrorCallback;
        public static event AVCHDLogDelegate WarningCallback;
        public static event AVCHDLogDelegate InfoCallback;

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
