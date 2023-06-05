using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class TimeStamp
    {
        //
        // ECMA 167 1/7.3 
        //
        private ushort TypeAndTimezone;
        private short Year;
        private byte Month;
        private byte Day;
        private byte Hour;
        private byte Minute;
        private byte Second;
        private byte Centiseconds;
        private byte HundredsofMicroseconds;
        private byte Microseconds;

        public TimeStamp(DateTime stamp)
        {
            TypeAndTimezone = 0x103c; // not totally sure what this means
            Year = (short)stamp.Year;
            Month = (byte)stamp.Month;
            Day = (byte)stamp.Day;
            Hour = (byte)stamp.Hour;
            Minute = (byte)stamp.Minute;
            Second = (byte)stamp.Second;
            Centiseconds = 0;
            HundredsofMicroseconds = 0;
            Microseconds = 0;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(TypeAndTimezone);
            writer.Write(Year);
            writer.Write(Month);
            writer.Write(Day);
            writer.Write(Hour);
            writer.Write(Minute);
            writer.Write(Second);
            writer.Write(Centiseconds);
            writer.Write(HundredsofMicroseconds);
            writer.Write(Microseconds);
        }
    }
}
