using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class MOBJCommand
    {
        private HDMVInstruction instruction;
        private uint destination;
        private uint source;


        public MOBJCommand(HDMVInstruction instruction, uint source, uint destination)
        {
            this.instruction = instruction;
            this.destination = destination;
            this.source = source;
        }

        public void Write(BinaryWriter writter)
        {
            instruction.Write(writter);
            writter.Write(EndianSwap.Swap(destination));
            writter.Write(EndianSwap.Swap(source));
        }
    }
}
