using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents an AtscCa descriptor which is transmitted as part of
    /// the PMT.
    /// </summary>
    public class AtscCaDescriptor : Descriptor
    {
        //
        // CA_System_ID (2 bytes) shall be set to 0FFF (AACS blu-ray)
        //
        private ushort CaSystemId = 0x0fff;

        /*
        * 11111 10011111100   (sintel)  (i.e. sets RetentionMoveMode and RetentionState to all 1111s)
        * 10000 10011111100   (proshow)
        */
        private byte reserved = 0x1;            // 1 bit
        private byte RetentionMoveMode = 0x1;   // 1 bit (not used for blu-ray set to 1?)
        private byte RetentionState = 0x7;      // 3 bits (not used for blu-ray set to 111?)
        private byte EPN = 0x1;                 // 1 bit
        private byte CCA = 0;                   // 2 bits
        private byte reserved2 = 0x1f;          // 5 bits
        private byte ImageConstraintToken = 0x1;// 1 bit
        private byte APS = 0;                   // 2 bits

        private const byte TAG = 0x88;

        /// <summary>
        /// Constructor
        /// </summary>
        public AtscCaDescriptor()
            : base(TAG)
        {
        }

        /// <summary>
        /// Gets the length in bytes
        /// </summary>
        /// <returns></returns>
        public override uint GetLengthInBytes()
        {
            return 6; // Fixed
        }

        /// <summary>
        /// Writes this descriptor to a given writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write((byte)4); // Length
            writer.Write(EndianSwap.Swap(CaSystemId));
            byte nextByte = (byte)(reserved << 7);
            nextByte |= (byte)(RetentionMoveMode << 6);
            nextByte |= (byte)(RetentionState << 3);
            nextByte |= (byte)(EPN << 2);
            nextByte |= CCA;
            writer.Write(nextByte);
            nextByte = (byte)(reserved2 << 3);
            nextByte |= (byte)(ImageConstraintToken << 2);
            nextByte |= APS;
            writer.Write(nextByte);
        }
    }
}
