using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /*
       * Here are some constraints for .m2ts file:
- divided to aligned units (6144 bytes / 32 packets)
- last unit is padded with null packets, ts pid 0x1fff
- ts pids are fixed:
0x0100 PMT
0x1001 PCR
0x1011 primary video
0x1100- 0x111f primary audio
0x1200- 0x121f PGS
0x1400- 0x141f IGS
0x1800         text subtitles
0x1a00- 0x1a1f secondary audio
0x1b00- 0x1b1f secondary video
- no gaps in PID values are allowed
- PCR must be present, max interval 100ms
- transport scrambling control set to 00
- following tables must be present: PAT, PMT, SIT
- max intervals: PAT 100ms, PMT 100ms, SIT 1 second

Movie application main .m2ts file:
- 1 primary video stream
MPEG2, MPEG4 AVC or SMTPE VC-1
- 0-32 primary audio streams
LPCM, AC-3, DD+, Dolby Lossless, DTS, DTS-HD
- 0-32 primary HDMV PG streams
- 0-32 primary HDMV IG streams
- 0-32 secondary video streams
MPEG2, MPEG4 AVC or SMTPE VC-1  
- 0-32 secondary audio streams
DD+, DTS-HD LBR
[Note that additional streams may be muxed to separate .m2ts file]

PAT:
- network PID: 0x001f
- program number: 1 (only one program allowed)
- program map pid: 0x100
PMT:
- program number: 1
- stream types:
video 0x02, 0x1b, 0xea
audio 0x80...0x86, 0xa1..0xa2
graphics 0x90..0x92
- program info shall contain descriptors 0x05 (value "HDMV") and 0x88
- program info may contain descriptor 0x89
- stream info shall contain descriptors 0x05, 0x81, 0x86, 0x28
- stream info may contain descriptor 0x2a
SIT:
- only one service
- running status: 0
- shall contain descriptor 0x63

> The difference is that each ISO-TS packet is
> preceded by a 32-bit timestamp value, signaling when the packet is to be
> delivered to the demuxer. 

2 first bits are copy-control bits (see aacs spec).
next 30 bits are timestamp (ATC, arrival time counter, 27 MHz)

> The purpose of the timestamp is to avoid having
> to add padding packets.

and sync reading sub play items (when some substreams are in
separate .m2ts file)
- Petri
     */

    /// <summary>
    /// This class can create (and mux) a bluray .M2TS file from a raw h264 stream and an LPCM audio stream.
    /// It then returns a report with informatoin about the muxing process.
    /// If no audio stream exists then set the 'aacAudioFilename' to an empty string "".  The generator
    /// will then create blank audio when muxing.
    /// This given audio stream is assumed to be 48khz 16 bit stereo.
    /// </summary>
    public class M2TSFileGenerator
    {
        private H264Packetizer videoPackertizer = null;
        private LPCMPacketizer audioPackertizer = null;
        private List<IFrameMarker> iFrameMarkers;
        private uint numberOfTsPackets;
        private uint maxRecordRate;             // max record rate
        private uint numberOfVideoFrames;
            
        private ProgramInfo program0;
        private ProgramInfo program1;
        private PAT pat;
        private PMT pmt;
        private SIT sit;
        private int patCount;
        private int nextPatCC;

        //
        // Used to calc max record rate
        //
        private int maxRateIndex = 0;
        private uint[] maxRateHistory;
        private uint lastVideoFramePacketNumber;

        private double clockFrequency = 90000;
        private int frameStepTime = 0;
        private static bool abort = false;

        //
        // Used to snapshot all time stamps when a PCR is generated. 
        // These are used later as a sanity check and also when calculating arrivel time for 
        // TS packets
        //
        private struct TimeStampMarkers
        {
            public uint PESAfterPCRPacketNumber;    // TS packet number of PES after PCR packet
            public long PCR;    // PCR before PES packet timestamp
            public long DTS;    // of PES after PCR
            public long PTS;    // of PES after PCR

            public TimeStampMarkers(uint PESAfterPCRPacketNumber, long PCR, long DTS, long PTS)
            {
                this.PESAfterPCRPacketNumber = PESAfterPCRPacketNumber;
                this.PCR = PCR;
                this.DTS = DTS;
                this.PTS = PTS;
            }
        }

        public static bool Abort
        {
            set { abort = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="h264VideoFilename"></param>
        /// <param name="aacAudioFilename"></param>
        /// <param name="outFilename"></param>
        /// <param name="wavHeaderSize"></param>
        /// <param name="fps"></param>
        /// <returns></returns>
        public M2TSFileGeneratorReport Generate(string h264VideoFilename, string aacAudioFilename, string outFilename, uint wavHeaderSize, double fps, List<DateTime> chapterMarkers)
        {
            abort = false;

            //
            // Needed to calculate arrival times in first 4 bytes of the TS packets.
            // These values are are then used to calculae arrival time on a second pass 
            // of the m2ts file
            //
            List<TimeStampMarkers> timeStampMarkers = new List<TimeStampMarkers>();
            long sizeFile = 0;

            M2TSFile file = M2TSFile.CreateNewFromFilename(outFilename);

            try
            {

                iFrameMarkers = new List<IFrameMarker>();
                numberOfTsPackets = 0;
                maxRecordRate = 0;
                numberOfVideoFrames = 0;
                lastVideoFramePacketNumber = 0;

                frameStepTime = (int)((1 / fps) * clockFrequency);

                //
                // Store last 1 second rate (round to nearest whole fps)
                //
                int rateFrameRate = (int)(fps + 0.4999);
                maxRateHistory = new uint[rateFrameRate];
                maxRateIndex = 0;

                //
                // Create two programs 0 and 1(main program)
                //
                program0 = new ProgramInfo();
                program0.programNumber = 0;
                program0.networkPID = 31;

                //
                // Main program
                //
                program1 = new ProgramInfo();
                program1.programNumber = 1;
                program1.programMapPID = 256;

                List<ProgramInfo> programInfos = new List<ProgramInfo>();
                programInfos.Add(program0);
                programInfos.Add(program1);

                //
                // Packet 1 write PAT
                //
                pat = new PAT(programInfos);
                TSPacket firstPATPacket = new TSPacket(0, pat); // pat always has PID 0
                firstPATPacket.Write(file.Writer);
                numberOfTsPackets++;

                //
                // Packet 2 write PMT for program 1 (PID 256)
                //
                pmt = new PMT(true);
                TSPacket firstPMTPacket = new TSPacket(program1.programMapPID, pmt);
                firstPMTPacket.Write(file.Writer);
                numberOfTsPackets++;

                //
                // Packet 3 write SIT 
                //
                sit = new SIT();
                TSPacket firstSITPacket = new TSPacket(SIT.PID, sit);
                firstSITPacket.ContinuityCounter = 1;
                firstSITPacket.Write(file.Writer);
                numberOfTsPackets++;

                int PCRTimeCount = 0;
                //
                // Packet 4 write only Adaptation Field with PCR time at 0
                //
                TSPacket firstPCRPacket = GeneratePCRTSPacket(4097, PCRTimeCount);
                firstPCRPacket.Write(file.Writer);
                numberOfTsPackets++;

                long PCRTime = 0;

                //
                // Create a PES packetizer for h264 stream and LPCM audio
                //
                videoPackertizer = new H264Packetizer(h264VideoFilename, PES.MPEGStreamID, fps);

                if (aacAudioFilename != "")
                {
                    audioPackertizer = new LPCMPacketizer(aacAudioFilename, PES.LPCMStreamID, wavHeaderSize);
                }
                else
                {
                    //
                    // No audio, so create a zero stream packetizer
                    //
                    audioPackertizer = new LPCMPacketizer(PES.LPCMStreamID);
                }

                using (videoPackertizer)
                using (audioPackertizer)
                {
                    PES videoPes = videoPackertizer.GetNextPacket();
                    PES audioPes = audioPackertizer.GetNextPacket();
                    int videoCC = 1;
                    int audioCC = 1;
                    patCount = 0;
                    nextPatCC = 1;
                    long lastPCRTimestamp = 0;
                    long nextAudioPTS = 54000000 - frameStepTime;
                    audioPes.FirstAudio = true;

                    //
                    // Store first time stamp marker
                    //
                    timeStampMarkers.Add(new TimeStampMarkers(numberOfTsPackets, firstPCRPacket.AdaptationField.PCR, videoPes.DecodeTimeStamp, videoPes.PresentTimeStamp));
               
                    //
                    // Loop though all video PESs in packetizer
                    //    
                    while (videoPes != null || audioPes != null)
                    {
                        //
                        // If aborted return now
                        //
                        if (abort == true)
                        {
                            return null;
                        }

                        if (((PCRTime + frameStepTime) - lastPCRTimestamp) > 7800)
                        {
                            //
                            // Write PCR and possibly PMT/PAT/SIT 
                            //
                            if (videoPes != null)
                            {
                                WritePCRPacket(file, timeStampMarkers, PCRTime + frameStepTime, videoPes.DecodeTimeStamp, videoPes.PresentTimeStamp);
                            }
                            lastPCRTimestamp = PCRTime + frameStepTime;
                        }

                        if (audioPes != null && audioPes.PresentTimeStamp < nextAudioPTS)
                        {
                            //
                            // Write audio 
                            //
                            WritePesOverMultipleTsPackets(file, audioPes, 4352, ref audioCC);
                            audioPes = audioPackertizer.GetNextPacket();
                            PCRTime += 450;
                        }
                        else
                        {
                            if (videoPes == null)
                            {
                                // 
                                // No video frame? force end of muxing.
                                //
                                break;
                            }

                            //
                            // Write video
                            //
                            numberOfVideoFrames++;
                            WritePesOverMultipleTsPackets(file, videoPes, 4113, ref videoCC);
                            videoPes = videoPackertizer.GetNextPacket();
                            nextAudioPTS += frameStepTime;

                            //
                            // Check what are max rec rate is
                            //
                            CheckRecordRate();

                            //
                            // If no audio the move PCR time on
                            //
                            if (audioPes == null)
                            {
                                PCRTime += frameStepTime;
                            }
                        }
                    }
                }

                //
                // Sanity check that numberOfTsPackets should be current stream position divided by 192
                //
                if (numberOfTsPackets != file.Writer.BaseStream.Position / 192)
                {
                    BDMVLog.Error("Size of file '" + outFilename + "' does not match expected number of TS packets recorded");
                }

                // 
                // Record time stamp marker at end position
                //
                timeStampMarkers.Add(new TimeStampMarkers(numberOfTsPackets, PCRTime + frameStepTime + AdaptationField.PCRTimeBase, 0, 0));

                sizeFile = file.Writer.BaseStream.Position;
            }
            finally
            {
                file.Close();
            }

            //
            // Second pass, write arrival time stamps bases on our time stamp list
            //
            WriteArrivalTimeStamps(outFilename, timeStampMarkers.ToArray(), sizeFile);
        
            //
            // Generate output report
            //
            M2TSFileGeneratorReport report = new M2TSFileGeneratorReport(outFilename, iFrameMarkers, numberOfTsPackets, maxRecordRate, numberOfVideoFrames, chapterMarkers);

            return report;
        }

        /// <summary>
        /// Private method to check what the recorded rate was over the last 1 second
        /// </summary>
        private void CheckRecordRate()
        {
            maxRateIndex++;
            if (maxRateIndex == maxRateHistory.Length)
            {
                maxRateIndex = 0;
            }

            uint packetsWriten = numberOfTsPackets - lastVideoFramePacketNumber;
            uint bytesWritten = packetsWriten * 192;
            uint thisRate = bytesWritten;
            maxRateHistory[maxRateIndex] = (uint)thisRate;

            uint rate = 0;
            //
            // Calculate the average rate over the last 1 second
            //
            for (int i = 0; i < maxRateHistory.Length; i++)
            {
                rate += maxRateHistory[i];
            }

            //
            // If bigger than max rate then set a new max rate
            //
            if (rate > maxRecordRate)
            {
                maxRecordRate = (uint)rate;
            }

            lastVideoFramePacketNumber = numberOfTsPackets;
        }

        /// <summary>
        /// Private method to write a PCR based TS packet and possibly write the PAT/PMT/SIT
        /// </summary>
        /// <param name="file"></param>
        /// <param name="timeStampMarkers"></param>
        /// <param name="PCRTime"></param>
        /// <param name="DTS"></param>
        /// <param name="PTS"></param>
        private void WritePCRPacket(M2TSFile file, List<TimeStampMarkers> timeStampMarkers, long PCRTime, long DTS, long PTS)
        {
            //
            // Generate PCR every 0.1 seconds
            //
            TSPacket PCRPacket = GeneratePCRTSPacket(4097, PCRTime);
            PCRPacket.AdaptationFieldControl = 2;
            PCRPacket.ContinuityCounter = 0;
            PCRPacket.Write(file.Writer);
            numberOfTsPackets++;

            timeStampMarkers.Add(new TimeStampMarkers(numberOfTsPackets, PCRPacket.AdaptationField.PCR, DTS, PTS));
            
            //
            // Do we need to generate a PAT/MPT/SIT (every 1 second)
            //
            patCount++;
            if (patCount == 10) 
            {
                patCount = 0;
                WritePatPmtSit(file);
            }
        }

        /// <summary>
        /// Private method to write the PAT/PMT/SIT
        /// </summary>
        /// <param name="file"></param>
        private void WritePatPmtSit(M2TSFile file)
        {
            TSPacket PatPacket = new TSPacket(0, pat); // PAT always has PID 0
            PatPacket.ContinuityCounter = (byte)(nextPatCC % 16);
            PatPacket.Write(file.Writer);
            numberOfTsPackets++;

            TSPacket PmtPacket = new TSPacket(program1.programMapPID, pmt);
            PmtPacket.ContinuityCounter = (byte)(nextPatCC % 16);
            PmtPacket.Write(file.Writer);
            numberOfTsPackets++;

            TSPacket SitPacket = new TSPacket(SIT.PID, sit);
            SitPacket.ContinuityCounter = (byte)((nextPatCC + 1) % 16);
            SitPacket.Write(file.Writer);
            numberOfTsPackets++;
            nextPatCC++;
        }


        /// <summary>
        /// Private method to write a complete PES over multiple TS packets.
        /// It also generates 'padding' adaptation fields in the TS packets to ensure
        /// the PES ends on a complete TS packet.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="pes"></param>
        /// <param name="PID"></param>
        /// <param name="cc"></param>
        private void WritePesOverMultipleTsPackets(M2TSFile file, PES pes, ushort PID, ref int cc)
        {
            bool writtenCompletPes = false;
            bool firstPesTSPacket = true;

            int totalToWrite = pes.Data.Length;

            while (writtenCompletPes == false)
            {
                int leftToWrite = totalToWrite - pes.DataOffset;

                if (leftToWrite <= 0)
                {
                    break;
                }

                int nextSize = pes.GetNextSize();

                TSPacket TsPesPacket5 = new TSPacket(PID, pes);
                TsPesPacket5.ContinuityCounter = (byte)(cc % 16);

                if (firstPesTSPacket == true)
                {
                    //
                    // Is this an iframe? mark this as this will be needed later in the the AVCHD ClipInfo file
                    //
                    if (pes.FrameType == PES.H264FrameType.IFrame)
                    {
                        uint packetNumber = (uint)(file.Writer.BaseStream.Position / 192);
                        iFrameMarkers.Add(new IFrameMarker(packetNumber, pes.PresentTimeStamp, (uint)pes.Data.Length));
                    }

                    if (leftToWrite <= 192 - 27)
                    {
                        //
                        // This would happen if a whole picture frame is less than 165 bytes? impossible ?
                        // I suppose possible with audio
                        //
                        WriteRemainder(TsPesPacket5, file, 192 - 27, leftToWrite);
                    }
                    else
                    {
                        TsPesPacket5.Write(file.Writer, true);
                        firstPesTSPacket = false;
                    }
                }
                else
                {
                    TsPesPacket5.PayloadUnitStartIndictator = 0;

                    if (leftToWrite <= 184)
                    {
                        WriteRemainder(TsPesPacket5, file, 184, leftToWrite);
                    }
                    else
                    {
                        TsPesPacket5.Write(file.Writer, false);
                    }
                }
                numberOfTsPackets++;
                cc++;
            }
        }

        /// <summary>
        /// Private method to create the padding adaptation TS packets
        /// </summary>
        /// <param name="TsPesPacket5"></param>
        /// <param name="file"></param>
        /// <param name="TsPacketBytesRemaining"></param>
        /// <param name="pesLeftToWrite"></param>
        private void WriteRemainder(TSPacket TsPesPacket5, M2TSFile file, int TsPacketBytesRemaining, int pesLeftToWrite)
        {
            if (pesLeftToWrite == TsPacketBytesRemaining)
            {
                TsPesPacket5.AdaptationFieldControl = 1;
                TsPesPacket5.AdaptationField = null;
                TsPesPacket5.Write(file.Writer, false);
            }
            else
            {
                AdaptationField af2 = new AdaptationField((byte)((TsPacketBytesRemaining - 1) - pesLeftToWrite), false, 0);
                TsPesPacket5.AdaptationFieldControl = 3;
                TsPesPacket5.AdaptationField = af2;
                TsPesPacket5.Write(file.Writer, false);
            }
        }

        /// <summary>
        /// Private method to generate a PCR based adaptation field TS packet
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="PCRTime"></param>
        /// <returns></returns>
        private TSPacket GeneratePCRTSPacket(ushort pid, long PCRTime)
        {
            AdaptationField adaptationField = new AdaptationField(183, true, PCRTime); 
            TSPacket packet = new TSPacket(pid, adaptationField);
            return packet;
        }

        private void WriteArrivalTimeStamps(string filename, TimeStampMarkers[] timeStampMarkers, long sizeFile)
        {      
            M2TSFile file = M2TSFile.OpenFromFilename(filename);
            if (file == null)
            {
                return;
            }

            bool warningShown = false;
            double timeBetweenPCR;

            double baseTime = 80372640;           // Not toally sure why this number is used (copied for txmuxer)
            double PCRbase = baseTime;         // Not toally sure why this number is used (copied for txmuxer)
            double currentTime =baseTime;

            //
            // First 4 TS packets have same base arrival time   (PAT/PMT/SIT/PCR Adaptation)
            //
            for (int i = 0; i < 4; i++)
            {           
                file.Writer.Write(EndianSwap.Swap((uint)baseTime));
                file.Writer.BaseStream.Position += 188;
            }

            for (int i = 0; i < timeStampMarkers.Length - 1; i++)
            {
               

                uint nextPacketSetLength = timeStampMarkers[i + 1].PESAfterPCRPacketNumber - timeStampMarkers[i].PESAfterPCRPacketNumber;

                long PCRDiff = timeStampMarkers[i + 1].PCR - timeStampMarkers[i].PCR;
                timeBetweenPCR = PCRDiff * 300;

                double timeDiffencePerPacket = timeBetweenPCR / nextPacketSetLength;

                //
                // Convert timestamps into seconds equivalent
                //
                double thisDTS = ((((double)timeStampMarkers[i].DTS) - 54000000) * 300) ;
                double thisPTS = ((((double)timeStampMarkers[i].PTS) - 54000000) * 300) ;
                double thisPCR = ((((double)timeStampMarkers[i].PCR) - 53955000) * 300);
                double DTSSeconds = thisDTS / 27000000;
                double thisPCRSeconds = ((((double)timeStampMarkers[i].PCR) - 54000000) / 90000);
                double nextPCRSeconds = ((((double)timeStampMarkers[i + 1].PCR) - 54000000) / 90000);

                if (thisPTS < thisDTS && warningShown==false)
                {
                    BDMVLog.Warning("PTS (" + thisPTS + ") smaller than DTS(" + thisDTS + ")");
                    warningShown = true;
                }
                if (thisPCRSeconds > nextPCRSeconds && warningShown==false)
                {
                    BDMVLog.Warning("This PCR (" + thisPCRSeconds + ") after next PCR (" + nextPCRSeconds+")");
                    warningShown = true;
                }
                if (thisPCRSeconds > DTSSeconds && warningShown == false)
                {
                    BDMVLog.Warning("Last PCR'" + thisPCRSeconds + "' after DTS '" + DTSSeconds);
                    warningShown = true;
                }
                if (nextPCRSeconds > DTSSeconds && warningShown == false)
                {
                    BDMVLog.Warning("Next PCR'" + thisPCRSeconds + "' after DTS '" + DTSSeconds);
                    warningShown = true;
                }

                PCRbase = thisPCR + baseTime;

                for (int packet = 0; packet < nextPacketSetLength; packet++)
                {
                    //
                    // If aborted return now
                    //
                    if (abort == true)
                    {
                        return;
                    }

                    currentTime = PCRbase + (timeDiffencePerPacket * (packet + 1));
                    uint arrivalTimeStamp = (uint)(currentTime);

                    //
                    // Only 30 bits, top 2 bits not used by us (copy permission indicator)
                    //
                    arrivalTimeStamp &= 0x3FFFFFFF;

                    file.Writer.Write(EndianSwap.Swap(arrivalTimeStamp));
                    file.Writer.BaseStream.Position += 188;

                    double arrivalTimeSeconds = ((currentTime - baseTime) / 27000000) - 0.5; // Assume arrival time base starts at -0.5 seconds

                    //
                    // check arrival time between PCR and next PCR and before DTS
                    //
                    if (arrivalTimeSeconds > DTSSeconds && warningShown==false)
                    {
                        BDMVLog.Warning("Arrival stamp '" + arrivalTimeSeconds + "' after DTS '" + DTSSeconds);
                        warningShown = true;
                    }
                    if (arrivalTimeSeconds < thisPCRSeconds && warningShown == false)
                    {
                        BDMVLog.Warning("Arrival stamp '" + arrivalTimeSeconds + "' before last PCR '" + thisPCRSeconds);
                        warningShown = true;
                    }
                    if (i != timeStampMarkers.Length - 2 && arrivalTimeSeconds > (nextPCRSeconds + 0.0001) && warningShown == false) // Do not check if last packet set
                    {
                        BDMVLog.Warning("Arrival stamp '" + arrivalTimeSeconds + "' after next PCR '" + nextPCRSeconds);
                        warningShown = true;
                    }
                }
            }

            if (file.Writer.BaseStream.Position != sizeFile)
            {
                BDMVLog.Error("Something went wrong when writting .m2ts arrival times, file size different than calculated");
            }
            file.Close();
        }
    }
}
