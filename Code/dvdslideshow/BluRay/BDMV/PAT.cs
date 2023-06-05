using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// PAT stands for Program Association Table. It lists all programs available in the transport stream. 
    /// Each of the listed programs is identified by a 16-bit value called program_number. Each of the 
    /// programs listed in PAT has an associated value of PID for its Program Map Table (PMT).
    /// The value 0x0000 of program_number is reserved to specify the PID where to look for Network Information
    /// Table (NIT). If such a program is not present in PAT the default PID value (0x0010) shall be used for
    /// NIT.
    /// TS Packets containing PAT information always have PID 0x0000.
    /// </summary>
    public class PAT : Payload
    {
        private byte sectionSyntaxIndicator;        // 1  bit
        private byte zero;                          // 1  bit
        private byte reserved = 0x3;                // 2  bits
        private ushort sectionLength;               // 12 bits
        private ushort transportStreamId;           // 16 bits
        private byte reserved2 = 0x3;               // 2  bits
        private byte versionNumber;                 // 5  bits
        private byte currentNextIndicator;          // 1  bit
        private byte sectionNumber;                 // 8  bits
        private byte lastSectionNumber;             // 8  bits;
        private List<ProgramInfo> programInfos;     // N * 32 bits

        private const byte PAT_ID = 0;
   
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="programInfos"></param>
        public PAT(List<ProgramInfo> programInfos)
        {    
            sectionSyntaxIndicator = 1;
            zero = 0;
            sectionLength = (ushort)((4 * programInfos.Count) + 9);
            transportStreamId = 1;
            versionNumber = 0;
            currentNextIndicator = 1;
            sectionNumber = 0;
            lastSectionNumber = 0;
            this.programInfos = programInfos;
        }

        /// <summary>
        /// Gets the PAT length in bytes
        /// </summary>
        /// <returns></returns>
        public override uint GetLengthInBytes()
        {
            uint size = 12;
            foreach (ProgramInfo programInfo in programInfos)
            {
                size += programInfo.GetLengthInBytes();
            }
            return size;
        }

        /// <summary>
        /// Writes the PAT to the given writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(PAT_ID);

            ushort nextWord = (ushort)(sectionSyntaxIndicator << 15);
            nextWord |= (ushort)(zero << 14);
            nextWord |= (ushort)(reserved << 12);
            nextWord |= sectionLength;
            nextWord = EndianSwap.Swap(nextWord);
            writer.Write(nextWord);

            writer.Write(EndianSwap.Swap(transportStreamId));

            byte nextByte = (byte)(reserved2 << 6);
            nextByte |= (byte)(versionNumber << 1);
            nextByte |= currentNextIndicator;
            writer.Write(nextByte);

            writer.Write(sectionNumber);
            writer.Write(lastSectionNumber);
            foreach (ProgramInfo programInfo in programInfos)
            {
                programInfo.Write(writer);
            }

            //
            // Write CRC, currently we only ever create 1 type of PAT, so no need to calculate CRC
            //
            writer.Write((byte)0x24);
            writer.Write((byte)0xAC);
            writer.Write((byte)0x48);
            writer.Write((byte)0x84);
        }
    }
}
