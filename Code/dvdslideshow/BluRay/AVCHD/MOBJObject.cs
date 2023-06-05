using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class MOBJObject
    {
        public byte resumeIntentionFlag;    // 1 bit
        public byte menuCallMask;           // 1 bit
        public byte titleSearchMask;        // 1 bit   

        public byte reserved;

        ushort nummberOfCommands;           // 16 bit
        List<MOBJCommand> commands = new List<MOBJCommand>();

        public MOBJObject(List<MOBJCommand> commands, bool resumeIntentionFlag, bool menuCallMask, bool titleSearchMask)
        {
            this.commands = commands;
            this.resumeIntentionFlag = (byte)(resumeIntentionFlag == true ? 1 : 0);
            this.menuCallMask = (byte)(menuCallMask == true ? 1 : 0);
            this.titleSearchMask = (byte)(titleSearchMask == true ? 1 : 0);
            reserved = 0;
            nummberOfCommands = (ushort) commands.Count;
        }

        public int GetLengthInBytes()
        {
            int length = 4 + (commands.Count * 12);
            return length;
        }

        public void Write(BinaryWriter writter)
        {
            byte nextByte = (byte)(resumeIntentionFlag << 7);
            nextByte |= (byte)(menuCallMask << 6);
            nextByte |= (byte)(titleSearchMask << 5);
            writter.Write(nextByte);

            writter.Write(reserved);

            writter.Write(EndianSwap.Swap(nummberOfCommands));
            foreach (MOBJCommand command in commands)
            {
                command.Write(writter);
            }
        }
    }
}
