using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class M2TSVideo
    {
        //
        // These enums map esactly to thoes found in AVCDH .mpls and .clpi files
        //
        public enum VideoResolution
        {
            ResolutionNotDefined = 0,
            Resolution480i,     
            Resolution576i,
            Resoultion480p,
            Resoultion1080i,
            Resoultion720p,
            Resoultion1080p,
            Resoultion576p
        }

        public enum VideoFPS
        {
            FpsNotDefined,
            Fps23Point976,
            Fps24,
            Fps25,
            Fps29Point97,
            FpsReserved,         
            Fps50,
            Fps59Point94
        }

        public enum VidepAspect
        {
            AspectNotDefined,
            AspectNotReserved,
            Aspect4x3,
            Aspect16x9
        }
            
        private string filename;
        private List<IFrameMarker> iFrameMarkers;
        private uint numberOfPackets;
        private uint maxRecordRate;         // bytes per second
        private float lengthInSeconds;
        private bool containsAudio;
        private VideoResolution resolution;
        private VideoFPS fps;
        private VidepAspect aspect;
        private List<DateTime> chapterMarkers;

        public string Filename
        {
            get { return filename; }
        }

        public List<IFrameMarker> IFrameMarkers
        {
            get { return iFrameMarkers; }
        }

        public uint NumberOfPackets
        {
            get { return numberOfPackets; }
        }

        public uint MaxRecordRate
        {
            get { return maxRecordRate; }
        }

        public float LengthInSeconds
        {
            get { return lengthInSeconds; }
        }

        public bool ContainsAudio
        {
            get { return containsAudio; }
        }

        public VideoResolution Resolution
        {
            get { return resolution; }
        }

        public VideoFPS FPS
        {
            get { return fps; }
        }

        public VidepAspect Aspect
        {
            get { return aspect; }
        }

        public List<DateTime> ChapterMarkers
        {
            get { return chapterMarkers; }
        }

        public M2TSVideo(string filename, List<IFrameMarker> markers, uint numberOfPackets, uint maxRecordRate, float lengthInSeconds, VideoResolution resolution, VideoFPS fps, VidepAspect aspect, List<DateTime> chapterMarkers)
        {
            this.filename = filename;
            this.iFrameMarkers = markers;
            this.numberOfPackets = numberOfPackets;
            this.maxRecordRate = maxRecordRate;
            this.chapterMarkers = chapterMarkers;

            // 
            if (this.maxRecordRate > 6000000)
            {
                this.maxRecordRate = 6000000;
                AVCHDLog.Warning("Record rate of m2ts file '" + maxRecordRate + " bigger than blu-ray max of 6000000");
            }
            this.lengthInSeconds = lengthInSeconds;
            containsAudio = true;           // Set to false to debug things else this is always true in final version
            this.resolution = resolution;
            this.fps = fps;
            this.aspect = aspect;
        }
    }
}
