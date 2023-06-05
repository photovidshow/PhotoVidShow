using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// A packet is the basic unit of data in a transport stream. It starts with a sync byte and a header.
    /// Additional optional transport fields, as signaled in the optional adaptation field, may follow. 
    /// The rest of the packet consists of payload.
    /// 
    /// TS packets are 192 bytes for blu-ray (BDAV). Normal 188 bytes + 4 byte pre header timestamp
    /// 
    /// </summary>
    public class TSPacket
    {
        private const byte syncByte = 0x47;

        private byte transportErrorIndicator;       // 1  bit
        private byte payloadUnitStartIndictator;    // 1  bit
        private byte transportPriority;             // 1  bit
        private ushort PID;                         // 13 bits

        private byte transportScramblingControl;    // 2  bits
        private byte adaptationFieldControl;          // 2  bits
        private byte continuityCounter;             // 4  bits

        private Payload payload;
        private AdaptationField adaptationField;

        public AdaptationField AdaptationField
        {
            get { return adaptationField; }
            set { adaptationField = value; }
        }

        public byte AdaptationFieldControl
        {
            get { return adaptationFieldControl; }
            set { adaptationFieldControl = value; }
        }

        public byte ContinuityCounter
        {
            get { return continuityCounter; }
            set { continuityCounter = value; }
        }

        public byte PayloadUnitStartIndictator
        {
            get { return payloadUnitStartIndictator; }
            set { payloadUnitStartIndictator = value; }
        }
                
        public TSPacket(ushort PID, Payload payload)
        {
            Initialise(PID);
            adaptationFieldControl = 1;
            payloadUnitStartIndictator = 1;
            this.payload = payload;
        }

        public TSPacket(ushort PID, AdaptationField adaptationField)
        {
            Initialise(PID);
            this.adaptationField = adaptationField;
            payloadUnitStartIndictator = 0;
            adaptationFieldControl = 2;
        }

        private void Initialise(ushort PID)
        {
            transportErrorIndicator = 0;       
            transportPriority = 0;
            this.PID = PID;
            transportScramblingControl = 0;     
            continuityCounter = 0;
        }

        public void Write(BinaryWriter writer)
        {
            Write(writer, true);
        }

        public void Write(BinaryWriter writer, bool writeZeroAdaptation)
        {
            //
            // The arrival_time_stamp is really just the truncated (and normalized) PCR of each TS packet
            // (minus 10 bytes, since the PCR is the timestamp of the PCR itself and the arrival_time_stamp 
            // is the PCR of the beginning of the 188 byte packet).
            //

            //
            // All the bits of a PES packet must arrive (be placed in the stream) before the PCR becomes equal to the DTS of the PES packet.
            //
            uint timestamp = 0;            
            writer.Write(timestamp);
            writer.Write(syncByte);

            ushort nextWord = (ushort)(transportErrorIndicator << 15);
            nextWord |= (ushort)(payloadUnitStartIndictator << 14);
            nextWord |= (ushort)(transportPriority << 13);
            nextWord |= PID;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            byte nextByte = (byte)(transportScramblingControl << 6);
            nextByte |= (byte)(adaptationFieldControl << 4);
            nextByte |= continuityCounter;
            writer.Write(nextByte);

            uint sizeTSPacket = 8;

            if (adaptationField != null)
            {
                adaptationField.Write(writer);
                sizeTSPacket += adaptationField.GetLengthInBytes();
            }
            else if (adaptationFieldControl == 1 && writeZeroAdaptation==true)
            {
                writer.Write((byte)0x00);
                sizeTSPacket++;
            }

            if (payload != null)
            {
                payload.Write(writer);
               
                PES pes = payload as PES;
                if (pes != null)
                {
                    sizeTSPacket += pes.GetLastDataWrittenSizeInBytes();
                }
                else
                {
                    sizeTSPacket += payload.GetLengthInBytes();
                }   
            }

            
            // 
            // Write stuffing 0xff to align 192 bytes
            //
            uint used = (uint)(sizeTSPacket % 192);

            /*
            //
            // Sanity check, make sure 'used' matches allignment with file.
            // This call is slow hence removed in build (left for debug purposes)
            //
            uint fileUsed = (uint)(writer.BaseStream.Position % 192);
            if (used != fileUsed)
            {
                BDMVLog.Error("TS packet not alligned correctly with file");
            }
            */

            if (used != 0)
            {
                byte stuffingByte = 0xff;
                uint remainder = 192 - used;
                for (uint i = 0; i < remainder; i++)
                {
                    writer.Write(stuffingByte);
                }          
            }     
        }
    }
}
