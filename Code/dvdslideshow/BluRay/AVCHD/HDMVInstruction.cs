using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class HDMVInstruction
    {
        private byte subGroup;       // 3 bits command sub-group 
        private byte operandCount;   // 3 bits operand count 
        private byte group;          // 2 bits command group

        private byte branchOption;   // 4 bits
        private byte reserved1;      // 2 bits
        private byte iFlagForOp2;    // 1 bit I-flag for operand 2
        private byte iFlagForOp1;    // 1 bit I-flag for operand 1

        private byte compareOption;  // 4 bits;
        private byte reserved2;      // 4 bits;

        private byte setOption;      // 5 bits;
        private byte reserved3;      // 3 bits;


        // alternative
        private uint opcode=0;

        public HDMVInstruction(uint opcode)
        {
            this.opcode = opcode;
        }

        public HDMVInstruction(byte group, byte operandCount, byte subGroup, byte branchOption, byte IFlag1, byte iFlag2, byte compareOption, byte setOption)
        {
            this.group = group;
            this.operandCount = operandCount;
            this.subGroup = subGroup;
            this.branchOption = branchOption;
            this.iFlagForOp1 = IFlag1;
            this.iFlagForOp2 = iFlag2;
            this.compareOption = compareOption;
            this.setOption = setOption;

            reserved1 = 0;
            reserved2 = 0;
            reserved3 = 0;
        }

        public void Write(BinaryWriter writer)
        {
            if (opcode != 0)
            {
                writer.Write(EndianSwap.Swap(opcode));
            }
            else
            {
                byte nextByte = (byte)(subGroup << 5);
                nextByte |= (byte)(operandCount << 2);
                nextByte |= (byte)(group);
                writer.Write(nextByte);

                nextByte = (byte)(branchOption << 4);
                nextByte |= (byte)(reserved1 << 2);
                nextByte |= (byte)(iFlagForOp2 << 1);
                nextByte |= (byte)(iFlagForOp1 << 1);
                writer.Write(nextByte);

                nextByte = (byte)(compareOption << 4);
                nextByte |= (byte)(reserved2);
                writer.Write(nextByte);

                nextByte = (byte)(setOption << 3);
                nextByte |= (byte)(reserved3);
                writer.Write(nextByte);
            }
        }

    }
}
