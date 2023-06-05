using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class ClipInfoCLPICreator
    {
        private const uint HDMV_SIG1 = 0x564d4448;  // 'HDMV'
        private const uint HDMV_SIG_2A = 0x30303230; // 0200

        private ClipInfoATCSequence ATCSequence;
        private ClipInfoProgram program;
        private EpMap map;


        private void WriteHeader(BinaryWriter writter)
        {
            writter.Write(HDMV_SIG1);
            writter.Write(HDMV_SIG_2A);

            uint sequenceInfoStartAddr = 0xdc;      // fixed
            writter.Write(EndianSwap.Swap(sequenceInfoStartAddr));

            uint programStartInfoAddr = sequenceInfoStartAddr + ATCSequence.GetLengthInBytes() + 6;           
            writter.Write(EndianSwap.Swap(programStartInfoAddr));

            uint cpiStartAddr = programStartInfoAddr + program.GetLengthInBytes() + 6;    
            writter.Write(EndianSwap.Swap(cpiStartAddr));

            uint clipMarkStartAddr = cpiStartAddr + map.GetLengthInBytes() + 8 ; 
            writter.Write(EndianSwap.Swap(clipMarkStartAddr));

            uint extDataStartAddr = 0;
            writter.Write(EndianSwap.Swap(extDataStartAddr));

            for (int i = 0; i < 3; i++)
            {
                writter.Write((uint)0);
            }
        }

        private void WriteClipInfo(BinaryWriter writter, M2TSVideo video)
        {
            uint length = 0xb0;
            writter.Write(EndianSwap.Swap(length));

            ushort reserved = 0;
            writter.Write(reserved);

            byte streamType = 1;
            writter.Write(streamType);

            byte applicationType = 1;
            writter.Write(applicationType);

            uint isCc5 = 0;
            writter.Write(EndianSwap.Swap(isCc5));

            uint tsRecordingRate = video.MaxRecordRate;
            writter.Write(EndianSwap.Swap(tsRecordingRate));

            uint numberOfSourcePackets = video.NumberOfPackets;
            writter.Write(EndianSwap.Swap(numberOfSourcePackets));

            for (int i = 0; i < 32; i++)
            {
                writter.Write((uint)0);
            }

            // ts type info
            writter.Write((byte)0x00); // ??
            writter.Write((byte)0x1e); // length
            writter.Write((byte)0x80); // Validity flags ???
            writter.Write(0x564d4448); // format id 'HDMV'
            for (int i = 0; i < 25; i++)
            {
                writter.Write((byte)0);
            }
        }

        private void WriteSequenceInfo(BinaryWriter writter)
        {
            uint length = ATCSequence.GetLengthInBytes() + 2; 
            writter.Write(EndianSwap.Swap(length));
            byte reserved = 0;
            writter.Write(reserved);
            byte numberOfSequences = 1;
            writter.Write(numberOfSequences);

            ATCSequence.Write(writter);
        }

        private void WriteProgram(BinaryWriter writter)
        {
            uint length = program.GetLengthInBytes() + 2;
            writter.Write(EndianSwap.Swap(length));
            byte reserved = 0;
            writter.Write(reserved);
            byte numberOfPrograms = 1;
            writter.Write(numberOfPrograms);

            program.Write(writter);
        }

        private void WriteCPI(BinaryWriter writter)
        {
            uint length = map.GetLengthInBytes() + 4;
            writter.Write(EndianSwap.Swap(length));

            byte reserved = 0;
            writter.Write(reserved);
            byte cpiType = 1;
            writter.Write(cpiType);
            byte reserved1 = 0;
            writter.Write(reserved1);
            byte numStreamPid = 1;
            writter.Write(numStreamPid);

            map.Write(writter);

          
        }

        public bool Create(string folder, string backupFolder, M2TSVideo video, int playListNumber)
        {
            string numberString = playListNumber.ToString("D5");

            string file = folder + "\\"+numberString+".clpi";
            string backupFile = backupFolder + "\\"+numberString+".clpi";
            try
            {
                FileStream stream = new FileStream(file, FileMode.CreateNew);
                try
                {
                    using (BinaryWriter writter = new BinaryWriter(stream))
                    {
                        ATCSequence = new ClipInfoATCSequence(video.LengthInSeconds);
                        program = new ClipInfoProgram(video.ContainsAudio, video); 
                        map = new EpMap(video.IFrameMarkers);

                        WriteHeader(writter);
                        WriteClipInfo(writter, video);
                        WriteSequenceInfo(writter);
                        WriteProgram(writter);
                        WriteCPI(writter);
                        writter.Write((uint)0); // Write 0 clip markers
                    }
                }
                finally
                {
                    stream.Close();
                }
            }
            catch (Exception exception)
            {
                AVCHDLog.Error("Failed to create '" + file + "' " + exception.Message);
                return false;
            }

            AVCHD.File.Copy(file, backupFile);
            return true;
        }

    }
}
