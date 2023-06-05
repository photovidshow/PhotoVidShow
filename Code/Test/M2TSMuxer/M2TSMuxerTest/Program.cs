using System;
using System.Collections.Generic;
using System.Text;
using DVDSlideshow.BluRay.BDMV;
using DVDSlideshow.BluRay.AVCHD;
using MangedToUnManagedWrapper;

namespace M2TSMuxerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BDMVLog.ErrorCallback += Program.Error;
            BDMVLog.FatalErrorCallback += Program.FatalError;
            BDMVLog.WarningCallback += Program.Warning;
            BDMVLog.InfoCallback += Program.Info;
            MangedToUnManagedWrapper.CManagedErrors.SetUnmanagedCallbacks();
      

            M2TSFileGenerator generator = new M2TSFileGenerator();

            string pcmfilename = "e:\\warmbodiesdemux\\audio.pcm";

            Console.WriteLine("Starting mux");

            CManagedFileBufferCache buffer = new CManagedFileBufferCache(pcmfilename);
            int size_of_header = DVDSlideshow.CWavToRawPCMFileConverter.GetSizeOfHeader(pcmfilename, buffer);

            //M2TSFileGeneratorReport report = generator.Generate("e:\\warmbodiesdemux\\video.264", pcmfilename, "e:\\warmbodiesdemux\\muxed.m2ts", (uint)size_of_header, 24);
      
            Console.WriteLine("Done mux");   
        }

        static void Error(string error)
        {
            Console.WriteLine("Error:" + error);
        }

        static void FatalError(string error)
        {
            Console.WriteLine("Fatal error:" + error);
        }

        static void Warning(string error)
        {
            Console.WriteLine("Warning:" + error);
        }

        static void Info(string error)
        {
            Console.WriteLine("Info:" + error);
        }
    }
}
