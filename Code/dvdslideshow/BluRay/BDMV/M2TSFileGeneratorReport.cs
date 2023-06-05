using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This class represents an output report generated when creating a m2ts file.
    /// It contains useful information needed when creating the AVCHD file system. i.e:-
    /// -IFrame markers (used to create map in .clpi files)
    /// -Number of TS packets
    /// -Record rate
    /// -Number of video frames
    /// </summary>
    public class M2TSFileGeneratorReport
    {
        private string fileName;
        private List<IFrameMarker> iVideoFrameMarkers;
        private uint numberOfTsPackets;
        private uint maxRecordRate;     // bytes per second
        private uint numberOfVideoFrames;
        private List<DateTime> chapterMarkers = null;

        public string M2TSFilename
        {
            get { return fileName; }
        }

        public List<IFrameMarker> IVideoFrameMarkers
        {
            get { return iVideoFrameMarkers; }
        }

        public uint NumberOfTsPackets
        {
            get { return numberOfTsPackets; }
        }

        public uint MaxRecordRate
        {
            get { return maxRecordRate; }
        }

        public uint NumberOfVideoFrames
        {
            get { return numberOfVideoFrames; }
        }

        public List<DateTime> ChapterMarkers
        {
            get { return chapterMarkers; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iVideoFrameMarkers"></param>
        /// <param name="numberOfTsPackets"></param>
        /// <param name="maxRecordRate"></param>
        /// <param name="numberOfVideoFrames"></param>
        public M2TSFileGeneratorReport(string filename, List<IFrameMarker> iVideoFrameMarkers, uint numberOfTsPackets, uint maxRecordRate, uint numberOfVideoFrames, List<DateTime> chapterMarkers)
        {
            this.fileName = filename;
            this.iVideoFrameMarkers = iVideoFrameMarkers;
            this.numberOfTsPackets = numberOfTsPackets;
            this.maxRecordRate = maxRecordRate;
            this.numberOfVideoFrames = numberOfVideoFrames;
            this.chapterMarkers = chapterMarkers;
        }

    }
}
