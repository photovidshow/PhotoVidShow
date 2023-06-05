using System;
using System.Collections.Generic;
using System.Text;
using ManagedCore;
using DVDSlideshow.BluRay.BDMV;
using DVDSlideshow.BluRay.AVCHD;
using DVDSlideshow.BluRay.ISO.UDF;

namespace DVDSlideshow.BluRay
{
    public class BluRayErrors
    {
        /// <summary>
        /// Bind the Bluray error systems to the ManagedCore Log
        /// </summary>
        public static void SetCallbacks()
        {
            BDMVLog.ErrorCallback += Log.Error;
            BDMVLog.FatalErrorCallback += Log.FatalError;
            BDMVLog.WarningCallback += Log.Warning;
            BDMVLog.InfoCallback += Log.Info;

            AVCHDLog.ErrorCallback += Log.Error;
            AVCHDLog.FatalErrorCallback += Log.FatalError;
            AVCHDLog.WarningCallback += Log.Warning;
            AVCHDLog.InfoCallback += Log.Info;

            UDFLog.ErrorCallback += Log.Error;
            UDFLog.FatalErrorCallback += Log.FatalError;
            UDFLog.WarningCallback += Log.Warning;
            UDFLog.InfoCallback += Log.Info;
        }
    }
}
