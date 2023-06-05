using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class EpMap
    {
        private List<EpCoarseEntry> epCoarseEntries;
        private List<EpFineEntry> epFineEntries;

        public List<EpCoarseEntry> EpCoarseEntries
        {
            get { return epCoarseEntries ; }
        }

        public List<EpFineEntry> EpFineEntries
        {
            get { return epFineEntries; }
        }

        public EpMap(List<IFrameMarker> markers)
        {
            GenerateCoarseAndFineEntries(markers);
        }

        public uint GetLengthInBytes()
        {
            uint coarse = (uint)(epCoarseEntries.Count * 8);
            uint fine = (uint)(epFineEntries.Count * 4);

            uint length = coarse + fine + 16;

            return length;
        }

        public void Write(BinaryWriter writter)
        {
            ushort pid = 0x1011;
            byte epStreamType = 1;
            ushort numberCoarseEntries = (ushort) epCoarseEntries.Count;
            uint numberFineEntries = (uint)epFineEntries.Count;
            ulong nextLong = ((ulong)(pid >> 8)) << 56;
            nextLong |= ((ulong)(pid & 0xff)) << 48;
            nextLong |= ((ulong)epStreamType) << 34;
            nextLong |= ((ulong)numberCoarseEntries) << 18;
            nextLong |= (ulong)numberFineEntries;
            writter.Write(EndianSwap.Swap(nextLong));

            uint mapStartAddress = 14; // Fixed with one map
            writter.Write(EndianSwap.Swap(mapStartAddress));

            uint fineEntryOffset = (uint) (epCoarseEntries.Count * 8) +4;
            writter.Write(EndianSwap.Swap(fineEntryOffset));

            foreach (EpCoarseEntry coarseEntry in epCoarseEntries)
            {
                coarseEntry.Write(writter);
            }
            foreach (EpFineEntry fineEntry in EpFineEntries)
            {
                fineEntry.Write(writter);
            }
        }

        private void GenerateCoarseAndFineEntries(List<IFrameMarker> markers)
        {
            epCoarseEntries = new List<EpCoarseEntry>();
            epFineEntries = new List<EpFineEntry>();

            ushort lastUpperPTS = 0;
            ushort lastUpperSPN = 0;

            foreach (IFrameMarker marker in markers)
            {
                long nextPTS = marker.PresentationTimeStamp;
                uint nextSPN = marker.TsPacketNunber;

                ushort lowerPTS = (ushort)((nextPTS & 0xfffff) >> 9);   // Look at document US8554055.pdf (figure 21).  We need to AND with 20 bits not 19 (there is overlap)
                ushort upperPTS = (ushort)(nextPTS >> 19);

                uint lowerSPN = (nextSPN & 0x1ffff);
                ushort upperSPN = (ushort) (nextSPN >> 17);

                //
                // If upper part of PTS or SPN different to last fine entry then need to make new coarse entry
                //
                if (upperPTS != lastUpperPTS || upperSPN != lastUpperSPN)
                {
                    EpCoarseEntry coarseEntry = new EpCoarseEntry((uint)epFineEntries.Count, upperPTS, nextSPN); // We store whole SPN not upper part
                    epCoarseEntries.Add(coarseEntry);
                }

                //
                // Create fine Entry
                //
                EpFineEntry fineEntry = new EpFineEntry(marker.FrameSizeInBytes, lowerPTS, lowerSPN);
                epFineEntries.Add(fineEntry);

                lastUpperPTS = upperPTS;
                lastUpperSPN = upperSPN;
            }
        }
    }
}
