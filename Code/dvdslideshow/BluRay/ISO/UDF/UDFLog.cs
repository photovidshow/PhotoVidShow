using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public delegate void UDFLogDelegate(string value);

    /// <summary>
    /// This class is used to log any UDF errors, warnings or info messages.
    /// Other classes may add their delegate to the class events to also receive
    /// these messages.
    /// </summary>
    public class UDFLog
    {
        public static event UDFLogDelegate FatalErrorCallback;
        public static event UDFLogDelegate ErrorCallback;
        public static event UDFLogDelegate WarningCallback;
        public static event UDFLogDelegate InfoCallback;

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
