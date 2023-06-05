using System;
using System.Collections.Generic;
using System.Text;
using MangedToUnManagedWrapper;

namespace DVDSlideshow
{
    public class CAudioRecorder : IDisposable
    {
        private CManagedAudioRecorder mManagedAudioRecorder;

        public CAudioRecorder()
        {
            mManagedAudioRecorder = new CManagedAudioRecorder();
        }

        public void Record(string outputWavFilename, int selectedDevice, bool echoRecording)
        {
            int intecho = 0;
            if (echoRecording == true) intecho = 1;

            mManagedAudioRecorder.Record(outputWavFilename, selectedDevice, intecho);
        }

        public void Stop()
        {
            mManagedAudioRecorder.Stop();
        }

        public void Pause()
        {
            mManagedAudioRecorder.Pause();
        }

        public void Continue()
        {
            mManagedAudioRecorder.Continue();
        }

        public void Dispose()
        {
            mManagedAudioRecorder.Dispose();
        }

        public string GetFirstDeviceName()
        {
            return mManagedAudioRecorder.GetFirstDeviceName();
        }

        public string GetNextDeviceName()
        {
            return mManagedAudioRecorder.GetNextDeviceName();
        }
    }
}
