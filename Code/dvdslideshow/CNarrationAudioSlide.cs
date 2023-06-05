using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DVDSlideshow
{
    public class CNarrationAudioSlide : CMusicSlide
    {
        private float mStartNarrationTime =0;
        private CMusicPeformanceAudioUnit mPreviewAudioUnit; // reference to audio unit in music performance
   
        public CMusicPeformanceAudioUnit PreviewAudioUnit
        {
            set { mPreviewAudioUnit = value; }
            get { return mPreviewAudioUnit; }
        }

        public float StartNarrationTime
        {
            get { return mStartNarrationTime; }
            set { mStartNarrationTime = value; }
        }

        //*******************************************************************
        public CNarrationAudioSlide()
        {
        }

        //*******************************************************************
        public CNarrationAudioSlide(string filename)
            : base(filename, false, 0, false, 0, false, 1.0f)
        {
        }

        //*******************************************************************
        public void CalcLengthInFrame()
        {
            mStartFrameOffset =(int)  ((mStartNarrationTime * CGlobals.mCurrentProject.DiskPreferences.AudioFramesPerSecond)+0.4999f);
        }	

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement track = doc.CreateElement("NarrationAudioSlide");

            track.SetAttribute("StartNarrarionTime", this.mStartNarrationTime.ToString());

            SaveMusicSlidePart(track);

            parent.AppendChild(track);
        }

        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s = element.GetAttribute("StartNarrarionTime");
            if (s != "")
            {
                mStartNarrationTime = float.Parse(s);
            }

            base.Load(element);
        }

        //*******************************************************************
        public bool Overlaps(float startTime, float endTime, ref float hangingBefore, ref float hangingAfter)
        {
            // allow a little bit of tollerance
            float tolleranceStartTime = startTime + 0.05f;
            float tolleranceEndTime = endTime - 0.05f;

            float candidateStartTime = this.StartNarrationTime;
            float candidateEndTime = candidateStartTime + ((float)GetDurationInSeconds());

            bool overlap = false;

            hangingBefore = 0;
            hangingAfter = 0;

            // completely in middle
            if (tolleranceStartTime > candidateStartTime && tolleranceEndTime < candidateEndTime)
            {
                overlap = true;
                // make is move to the right
                hangingBefore = 0;
                hangingAfter = 0;
            }
            else
            {
                if (tolleranceStartTime < candidateStartTime && tolleranceEndTime > candidateStartTime)
                {
                    hangingBefore = candidateStartTime - startTime;
                    overlap = true;
                }

                if (tolleranceStartTime < candidateEndTime && tolleranceEndTime > candidateEndTime)
                {
                    hangingAfter = endTime - candidateEndTime;
                    overlap = true;
                }
            }

            return overlap;
        }
    }
}
