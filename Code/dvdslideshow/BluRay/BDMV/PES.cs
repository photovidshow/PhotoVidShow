using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents a packetized elementary stream.
    /// Transport Streams (TS) and Program Streams are each logically constructed from PES packets.
    /// PES packets may be much larger than the size of a Transport Stream packet.
    /// E.g. a single PES may be transmitted over many Transport Stream packets.
    /// </summary>
    public class PES : Payload
    {
        public enum H264FrameType
        {
            NotSet,
            IFrame,
            PFrame,
            BFrame
        };

        // 
        // ITU-T Rec. H.262 | ISO/IEC 13818-2 or ISO/IEC 11172-2 or ISO/IEC
        // 14496-2 video stream number xxxx
        //
        public const byte MPEGStreamID = 0xE0; // 111 00000   
        public const byte LPCMStreamID = 0xBD;
       
        private byte[] packetStartCodePrefix = { 0x00, 0x01 };              // 24 bits  ( 00 00 01) but first 00 is from adaptation
        private byte streamID;                                              // 8 bits
        private ushort PESPacketLength;                                     // 16 bits
        private byte oneZero = 0x2;                                         // 2 bits
        private byte PESScramblingControl;                                  // 2 bits
        private byte PESPriority;                                           // 1 bit
        private byte dataAlignmentIndicator;                                // 1 bit
        private byte copyright;                                             // 1 bit
        private byte originalOrCopy;                                        // 1 bit
        private byte PTSDTSFlag;                                            // 2 bits
        private byte ESCRFlag;                                              // 1 bit
        private byte ESRateFlag;                                            // 1 bit
        private byte DSMTrickModeFlag;                                      // 1 bit
        private byte additionalCopyInfoFlag;                                // 1 bit
        private byte PESCRCFlag;                                            // 1 bit
        private byte PESExtensionFlag;                                      // 1 bit
        private byte PESHeaderDataLength;                                   // 8 bits

        //
        // PTS = Presentation Time Stamp
        // DTS = Decoding Time Stamp
        // When PTDDTS Flag = 11
        //
        private byte zeroZeroOneOne = 0x3;                                  // 4 bits
        private byte zeroZeroOneZero = 0x2;                                 // 4 bits   // use when PTS ONLY
        private byte PTS_32_30;                                             // 3 bits
        private byte PTS_32_30_MarketBit = 1;                               // 1 bit
        private ushort PTS_29_15;                                           // 15 bits
        private byte PTS_29_15_MarketBit = 1;                               // 1 bit
        private ushort PTS_14_0;                                            // 15 bits
        private byte PTS_14_0_MarkerBit = 1;                                // 1 bit                     

        private byte zeroZeroZeroOne = 1;                                   // 4 bits
        private byte DTS_32_30;                                             // 3 bits
        private byte DTS_32_30_MarketBit = 1;                               // 1 bit
        private ushort DTS_29_15;                                           // 15 bits
        private byte DTS_29_15_MarketBit = 1;                               // 1 bit
        private ushort DTS_14_0;                                            // 15 bits
        private byte DTS_14_0_MarkerBit = 1;                                // 1 bit      

        private byte[] data;
        private int dataOffset = 0;

        private H264FrameType frameType = H264FrameType.NotSet;
        private long DTS = 0;
        private long PTS = 0;
        private bool audio = false;
        private bool firstAudio = false;
        private uint lastSizeDataWritten = 0;

        public H264FrameType FrameType
        {
            get { return frameType; }
            set { frameType = value; }
        }

        public byte[] Data
        {
            get { return data; }
        }

        public int DataOffset
        {
            get { return dataOffset; }
        }

        public long DecodeTimeStamp
        {
            get { return DTS; }
            set
            {
                DTS = value;
                DTS_32_30 = (byte)(value >> 30);
                DTS_29_15 = (ushort)((value >> 15) & 0x7FFF);
                DTS_14_0 = (ushort)(value & 0x7FFF);

                CheckPTSDTS();
            }
        }

        public long PresentTimeStamp
        {
            get { return PTS;}
            set
            {
                PTS = value;
                PTS_32_30 = (byte)(value >> 30);
                PTS_29_15 = (ushort)((value >> 15) & 0x7FFF);
                PTS_14_0 = (ushort)(value & 0x7FFF);

                CheckPTSDTS();
            }
        }

        public bool FirstAudio
        {
            get { return firstAudio; }
            set { firstAudio = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="streamID"></param>
        /// <param name="data"></param>
        /// <param name="PTS"></param>
        /// <param name="DTS"></param>
        /// <param name="audio"></param>
        public PES(byte streamID, byte [] data, long PTS, long DTS, bool audio) 
        {
            this.audio = audio;
            this.data = data;
            this.streamID = streamID;

            PESScramblingControl = 0;
            PESPriority = 0;
            dataAlignmentIndicator = 1;
            copyright = 0;
            originalOrCopy = 0;
            PTSDTSFlag = 0x3;
            ESCRFlag = 0;
            ESRateFlag = 0;
            DSMTrickModeFlag = 0;
            additionalCopyInfoFlag = 0;
            PESCRCFlag = 0;
            PESExtensionFlag = 0;
            PESHeaderDataLength = 10;   // (PTS + DTS length ??)

           // long PTS = 54000000;
          //  long DTS = 53996250;

            this.PTS = PTS;
            PTS_32_30 = (byte)(PTS >> 30);
            PTS_29_15 = (ushort)((PTS >> 15) & 0x7FFF);
            PTS_14_0 = (ushort)(PTS & 0x7FFF);

            this.DTS = DTS;
            DTS_32_30 = (byte)(DTS >> 30);
            DTS_29_15 = (ushort)((DTS >> 15) & 0x7FFF);
            DTS_14_0 = (ushort)(DTS & 0x7FFF);

            CheckPTSDTS();
        }

        private void CheckPTSDTS()
        {
            if (DTS == PTS)
            {
                PESHeaderDataLength = 5;
                PTSDTSFlag = 0x2;
            }
            else
            {
                PESHeaderDataLength = 10;
                PTSDTSFlag = 0x3;
            }

            CalcPesLength();
        }

        private void CalcPesLength()
        {
            int header = 3 + PESHeaderDataLength;

            int length = data.Length + header;

            if (audio == true)
            {
                length += 4;
            }

            //
            // If length bigger than 65535 we set the field to 0 (i.e. not set)
            //
            if (length > 65535)
            {
                PESPacketLength = 0;
            }
            else
            {
                PESPacketLength = (ushort)length;
            }
        }

        public override void Write(BinaryWriter writer)
        {
            Write(writer, dataOffset);
        }

        public int nextSize = -1;

        public int GetNextSize()
        {
            if (nextSize != -1)
            {
                int a =nextSize;
                nextSize =-1;
                return a;
            }

            if (dataOffset == 0)
            {
                //
                // Calc size based on header
                //
                if (PESHeaderDataLength == 5)
                {
                    return 192 - (18 + 4);
                }

                return 192 - (23 + 4);
            }
            else
            {
                return 192 - 8;
            }
        }

        private void Write(BinaryWriter writer, int offset)
        {
            int nextSize = GetNextSize();
            lastSizeDataWritten=0;

            if (offset == 0)
            {
                WriteHeader(writer);

                if (audio == false)
                {
                    WriteData(writer, 0, nextSize);
                }
                else
                {
                    writer.Write((byte)0x03);
                    writer.Write((byte)0xc0);
                    writer.Write((byte)0x31);

                    if (firstAudio == true)
                    {
                        writer.Write((byte)0x60);
                    }
                    else
                    {
                        writer.Write((byte)0x40);
                    }
                    WriteData(writer, 0, nextSize - 4);
                    lastSizeDataWritten += 4;
                }
            }
            else
            {
                WriteData(writer, offset, nextSize);
            }
        }

        private void WriteData(BinaryWriter writer, int offset, int length)
        {
            if (offset >=data.Length)
            {
                BDMVLog.Error("Invalid offet ("+offset+") in PES write data, length="+data.Length);
                return;
            }

            if (offset + length > data.Length)
            {
                length = data.Length - offset;
            }

            writer.Write(data, offset, length);
            dataOffset += length;

            lastSizeDataWritten += (uint)length;
        }

        private void WriteHeader(BinaryWriter writer)
        {
            writer.Write(packetStartCodePrefix);

            writer.Write(streamID);
            writer.Write(EndianSwap.Swap(PESPacketLength));

            byte nextByte = (byte)(oneZero<<6);
            nextByte |= (byte)(PESScramblingControl<<4);
            nextByte |= (byte)(PESPriority<<3);
            nextByte |= (byte)(dataAlignmentIndicator<<2);
            nextByte |= (byte)(copyright<<1);
            nextByte |= originalOrCopy;
            writer.Write(nextByte);

            nextByte = (byte)(PTSDTSFlag<<6);
            nextByte |= (byte)(ESCRFlag<<5);
            nextByte |= (byte)(ESRateFlag<<4);
            nextByte |= (byte)(DSMTrickModeFlag<<3);
            nextByte |= (byte)(additionalCopyInfoFlag<<2);
            nextByte |= (byte)(PESCRCFlag<<1);
            nextByte |= PESExtensionFlag;
            writer.Write(nextByte);

            writer.Write(PESHeaderDataLength);

            if (PTSDTSFlag == 0x3)
            {
                nextByte = (byte)(zeroZeroOneOne << 4); 
            }
            else
            {
                nextByte = (byte)(zeroZeroOneZero << 4);
            }

            nextByte |= (byte)(PTS_32_30 << 1);
            nextByte |= PTS_32_30_MarketBit;
            writer.Write(nextByte);

            ushort nextWord = (ushort)(PTS_29_15 << 1);
            nextWord |= PTS_29_15_MarketBit;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            nextWord = (ushort)(PTS_14_0 << 1);
            nextWord |= PTS_14_0_MarkerBit;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            lastSizeDataWritten += 13;

            if (PTSDTSFlag == 0x3)
            {
                nextByte = (byte)(zeroZeroZeroOne << 4);
                nextByte |= (byte)(DTS_32_30 << 1);
                nextByte |= DTS_32_30_MarketBit;
                writer.Write(nextByte);

                nextWord = (ushort)(DTS_29_15 << 1);
                nextWord |= DTS_29_15_MarketBit;
                nextWord = EndianSwap.Swap(nextWord);
                writer.Write(nextWord);

                nextWord = (ushort)(DTS_14_0 << 1);
                nextWord |= DTS_14_0_MarkerBit;
                nextWord = EndianSwap.Swap(nextWord);
                writer.Write(nextWord);

                lastSizeDataWritten+=5;
            }
        }

        public uint GetLastDataWrittenSizeInBytes()
        {
            return lastSizeDataWritten;
        }

        public override uint GetLengthInBytes()
        {
            BDMVLog.Error("GetLengthInBytes not supported for PES");
            return 0;
        }
    }
}
