using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class PlayListMPLSCreator
    {
        private const uint MPLS_SIG1 = 0x534c504d;  // 'MPLS'
        private const uint MPLS_SIG_2A = 0x30303230; // 0200

        private MPLSPlayItem playItem;
        private List<MPLSPlayMarker> playMarkers;

        private void WriteHeader(BinaryWriter writter, M2TSVideo video, int playListNumber)
        {
            writter.Write(MPLS_SIG1);
            writter.Write(MPLS_SIG_2A);

            string numberString = playListNumber.ToString("D5");

            playItem = new MPLSPlayItem(numberString, video);
            playMarkers = new List<MPLSPlayMarker>();

            //
            // Add play list marker at start
            //
            MPLSPlayMarker playMarker = new MPLSPlayMarker(0);
            playMarkers.Add(playMarker);

            //
            // Add any additional chapter markers 
            //
            if (video.ChapterMarkers != null)
            {
                foreach (DateTime chapterMarker in video.ChapterMarkers)
                {
                    uint seconds = (uint)(chapterMarker.Hour * 60 * 60);
                    seconds += (uint)(chapterMarker.Minute * 60);
                    seconds += (uint)(chapterMarker.Second);
                    MPLSPlayMarker chapterPlayMarker = new MPLSPlayMarker(seconds);
                    playMarkers.Add(chapterPlayMarker);
                }
            }
            
            // write list position
            uint listPosition = 0x3a;       // fixed
            writter.Write(EndianSwap.Swap(listPosition));

            // write mark position 
            uint markPosition = listPosition + playItem.GetLengthInBytes() + 10;
            writter.Write(EndianSwap.Swap(markPosition));

            // write ext position (currently not used and set to 0)
            uint extPosition = 0;
            writter.Write(EndianSwap.Swap(extPosition));

            // ????
            for (int i = 0; i < 23; i++)
            {
                writter.Write((byte)0);
            }
            writter.Write((byte)0x0e);
            writter.Write((byte)0x00);
            writter.Write((byte)0x01);
            for (int i = 0; i < 12; i++)
            {
                writter.Write((byte)0);
            }
        }

        private void WritePlayList(BinaryWriter writter)
        {
            // Write Length of playlist in bytes (4 bytes)
            uint length = playItem.GetLengthInBytes() + 6;
            writter.Write(EndianSwap.Swap(length));
            ushort reserved = 0;
            writter.Write(reserved);

            ushort items = 1;
            writter.Write(EndianSwap.Swap(items));
            ushort subCount = 0;
            writter.Write(EndianSwap.Swap(subCount));

            playItem.Write(writter);
        }

        private void WritePlayMarkerList(BinaryWriter writter)
        {
            ushort numberOfMarkers = (ushort)playMarkers.Count;

            // write length of play markers (4 bytes)
            writter.Write(EndianSwap.Swap((uint) (2+(numberOfMarkers*14))));    // number of markers container+ (14 * number of markers) 

            // write 
            writter.Write(EndianSwap.Swap(numberOfMarkers));

            foreach (MPLSPlayMarker marker in playMarkers)
            {
                marker.Write(writter);
            }
        }

        public bool Create(string folder, string backupFolder, M2TSVideo video, int playListNumber)
        {
            string numberString = playListNumber.ToString("D5");
            string file = folder + "\\" + numberString +".mpls";
            string backupFile = backupFolder + "\\" + numberString +".mpls";
            try
            {
                FileStream stream = new FileStream(file, FileMode.CreateNew);

                try
                {
                    using (BinaryWriter writter = new BinaryWriter(stream))
                    {
                        WriteHeader(writter, video, playListNumber);
                        WritePlayList(writter);
                        WritePlayMarkerList(writter);
                    }
                }
                finally
                {
                    stream.Close();
                }
            }
            catch (Exception exception)
            {
                AVCHDLog.Error("Failed to create '" + file + "' " + exception.Message);
                return false;
            }

            AVCHD.File.Copy(file, backupFile);

            return true;
        }
    }
}
