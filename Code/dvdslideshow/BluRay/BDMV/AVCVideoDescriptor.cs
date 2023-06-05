using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// For AVC video streams, the AVC video descriptor provides basic information for identifying coding parameters 
    /// of the associated AVC video stream.
    /// </summary>
    public class AVCVideoDescriptor : Descriptor
    {
        private byte descriptorLength;   // 8 bits
        private byte profileIdc;         // 8 bits
        private byte constraintSet0Flag; // 1 bit
        private byte constraintSet1Flag; // 1 bit
        private byte constraintSet2Flag; // 1 bit
        private byte constraintSet3Flag; // 1 bit
        private byte constraintSet4Flag; // 1 bit
        private byte constraintSet5Flag; // 1 bit
        private byte AVCCompatibleFlags; // 2 bits
        private byte levelIdc;           // 8 bits          
        private byte AVCStillPresent;    // 1 bit    
        private byte AVC24HourPictureFlag; // 1 bit
        private byte FramePackingSEINotPresentFlag;  // 1 bit
        private byte reserved = 0x1f;    // 5 bits

        private const byte TAG = 0x28;

        /// <summary>
        /// Constructor
        /// </summary>
        public AVCVideoDescriptor()
            : base(TAG)
        {
            descriptorLength = 4;
            profileIdc = 0x64;          // SRG ???
            constraintSet0Flag = 0;
            constraintSet1Flag = 0;
            constraintSet2Flag = 0;
            constraintSet3Flag = 0;
            constraintSet4Flag = 0;
            constraintSet5Flag = 0;
            AVCCompatibleFlags = 0;
            levelIdc = 0x28;            // SRG ???
            AVCStillPresent = 1;    // ???
            AVC24HourPictureFlag = 0;
            FramePackingSEINotPresentFlag = 1; // Set if no SEI present ?
        }

        /// <summary>
        /// Gets the length of the AVC video descriptor length in bytes
        /// </summary>
        /// <returns></returns>
        public override uint GetLengthInBytes()
        {
            return 6;
        }

        /// <summary>
        /// Writes the AVC video descriptor to the writer
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(descriptorLength);
            writer.Write(profileIdc);

            byte nextByte = (byte)(constraintSet0Flag << 7);
            nextByte |= (byte)(constraintSet1Flag << 6);
            nextByte |= (byte)(constraintSet2Flag << 5);
            nextByte |= (byte)(constraintSet3Flag << 4);
            nextByte |= (byte)(constraintSet4Flag << 3);
            nextByte |= (byte)(constraintSet5Flag << 2);
            nextByte |= AVCCompatibleFlags;
            writer.Write(nextByte);
            writer.Write(levelIdc);

            nextByte = (byte)(AVCStillPresent << 7);
            nextByte |= (byte)(AVC24HourPictureFlag << 6);
            nextByte |= (byte)(FramePackingSEINotPresentFlag << 5);
            nextByte |= reserved;

            writer.Write(nextByte);
        }
    }
}
