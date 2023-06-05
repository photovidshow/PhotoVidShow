using System;
using System.Collections.Generic;
using System.Text;
using DVDSlideshow.BluRay.BDMV;

namespace BluRayBDMVTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BDMVLog.ErrorCallback += new BDMVLogDelegate(Error);
            BDMVLog.WarningCallback += new BDMVLogDelegate(Warning);
            BDMVLog.FatalErrorCallback += new BDMVLogDelegate(FatalError);
            BDMVLog.InfoCallback += new BDMVLogDelegate(Info);

            M2TSFileGenerator generator = new M2TSFileGenerator();
            M2TSFileGeneratorReport report = generator.Generate("slideshow0.264",
                                                                "slideshow0.wav",
                                                                "out.m2ts",
                                                                0x44, 24);

            Console.WriteLine("Number IFrames = " + report.IVideoFrameMarkers.Count);
            Console.WriteLine("Number of TS packets = " + report.NumberOfTsPackets);
            Console.WriteLine("Number video frames = " + report.NumberOfVideoFrames);
            Console.WriteLine("Max record rate = " + report.MaxRecordRate);

        }

        public static void Error(string error)
        {
            Console.WriteLine("Error: " + error);
        }


        public static void Warning(string warning)
        {
            Console.WriteLine("Warning: " + warning);
        }


        public static void FatalError(string error)
        {
            Console.WriteLine("Fatal error: " + error);
        }

        public static void Info(string info)
        {
            Console.WriteLine("Info: " + info);
        }

    }
}
