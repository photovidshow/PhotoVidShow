using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{

    /// <summary>
    /// Program Map Tables (PMTs) contain information about programs. For each program, there is one PMT. 
    /// While the MPEG-2 standard permits more than one PMT section to be transmitted on a single PID 
    /// (Single Transport stream PID contains PMT information of more than one program), most MPEG-2 "users"
    /// such as ATSC and SCTE require each PMT to be transmitted on a separate PID that is not used for any 
    /// other packets. The PMTs provide information on each program present in the transport stream, including
    /// the program_number, and list the elementary streams that comprise the described MPEG-2 program. 
    /// There are also locations for optional descriptors that describe the entire MPEG-2 program, as well as 
    /// an optional descriptor for each elementary stream. Each elementary stream is labeled with a stream_type
    /// value.
    /// </summary>
    public class PMT : Payload
    {   
        private byte sectionSyntaxIndicator;        // 1  bit
        private byte zero;                          // 1  bit
        private byte reserved = 0x3;                // 2  bits
        private ushort sectionLength;               // 12 bits (size of rest of payload + crc)
        private ushort programNumber;               // 16 bits
        private byte reserved2 = 0x3;               // 2  bits
        private byte versionNumber;                 // 5  bits
        private byte currentNextIndicator;          // 1  bit
        private byte sectionNumber;                 // 8  bits  
        private byte lastSectionNumber;             // 8  bits
        private byte reserved3 =0x7;                // 3  bits
        private ushort PCR_PID;                     // 13 bits
        private byte reserved4 = 15;                // 4  bits
        private ushort programInfoLength;           // 12 bits
        private List<Descriptor> programDescriptors = new List<Descriptor>();
        private List<ProgramMapEntry> mapEntries = new List<ProgramMapEntry>();

        private bool includeAudio = true;  // Debug use 

        private const byte PMT_ID = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="includeAudio"></param>
        public PMT(bool includeAudio)
        {
            this.includeAudio = includeAudio;
            sectionSyntaxIndicator = 1;
            zero = 0;
            sectionLength = 0x24;    
            programNumber = 1;
            versionNumber = 0;
            currentNextIndicator = 1;
            sectionNumber = 0;
            lastSectionNumber = 0;
            PCR_PID = 4097;
            programInfoLength = 12; // Fixed length

            //
            // PMT for blu-ray must contain these descriptors for the program
            //
            programDescriptors.Add(new RegistrationDescriptor(0x564d4448));  // 'HDMI'
            programDescriptors.Add(new AtscCaDescriptor());
         
            //
            // Our blu-ray program contains two entries; 1 audio stream and 1 video stream
            //
            ProgramMapEntry videoStreamEntry = new VideoStreamProgramMapEntry();

            mapEntries.Add(videoStreamEntry);

            if (includeAudio == true)
            {
                ProgramMapEntry audioStreamEntry = new AudioStreamProgramMapEntry();
                mapEntries.Add(audioStreamEntry);
                sectionLength = 0x33;
            }
        }

        /// <summary>
        /// Gets the length of the PMT in bytes
        /// </summary>
        /// <returns></returns>
        public override uint GetLengthInBytes()
        {
            uint size = 16;
            foreach (Descriptor d in programDescriptors)
            {
                size += d.GetLengthInBytes();
            }

            foreach (ProgramMapEntry entry in mapEntries)
            {
               size += entry.GetLengthInBytes();
            }

            return size;
        }

        /// <summary>
        /// Writes the PMT to the writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(PMT_ID);

            ushort nextWord = (ushort)(sectionSyntaxIndicator<<15);
            nextWord |= (ushort) (zero << 14);
            nextWord |= (ushort) (reserved << 12);
            nextWord |= sectionLength;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            writer.Write(EndianSwap.Swap(programNumber));

            byte nextByte = (byte)(reserved2 << 6);
            nextByte |= (byte)(versionNumber << 1);
            nextByte |= currentNextIndicator;
            writer.Write(nextByte);

            writer.Write(sectionNumber);
            writer.Write(lastSectionNumber);

            nextWord = (ushort)(reserved3 << 13);
            nextWord |= PCR_PID;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            nextWord = (ushort)(reserved4 << 12);
            nextWord |= programInfoLength;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            foreach (Descriptor d in programDescriptors)
            {
                d.Write(writer);
            }

            foreach (ProgramMapEntry entry in mapEntries)
            {
                entry.Write(writer);
            }

            //
            // write CRC (one of two possibilies, no point calculating it)
            //
            if (includeAudio == false)
            {
                writer.Write((byte)0xDB);
                writer.Write((byte)0x3E);
                writer.Write((byte)0xCC);
                writer.Write((byte)0x7C);
            }
            else
            {
                writer.Write((byte)0x7C);
                writer.Write((byte)0x61);
                writer.Write((byte)0x4C);
                writer.Write((byte)0x1C);
            }
        }
    }
}
