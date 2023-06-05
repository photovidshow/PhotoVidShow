using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    //
    // According to internet this is how NAL values for frame types should be:-
    //
    // NAL Units start code: 00 00 00 01 X Y
    // X = IDR Picture NAL Units (0x25, 0x45, 0x65)
    // X = Non IDR Picture NAL Units (0x01, 0x21, 0x41, 0x61) ; 0x01 = b-frames, 0x41 = p-frames
    // since frames can be splitted over multiple NAL Units only count the NAL Units with Y != 00
    //
    // This is how it seems to work with PVS h264 encoder
    //
    // 00 01 09        = access unit deleminator (AUD)
    // 00 01 68        = picture parameter  
    // 00 01 65 X =    = P-frame (Typically X = 154 but not 100% also seen as 136)
    // 00 01 06        = SEI information
    // 00 01 01 158    = B frame 
    // 00 01 103 100 0 = I-frame 
    //  
    //  I-frames are key frames (complete info for a frame) and we start with this and with a gop
    //  P-frames are preditive from the I-frame
    //  B-frames (bi-directional are determined from I past frames and P future frames.
    //  Becuase of this p-frames are transimitted before b-frames although presentation
    //  will occur in a different order.
    // 
    //  e.g. for gob (group of pictures) it may transmit
    //  ipbbpbb
    // 
    //  this is actually presented in order
    //  ibbpbbp

    /// <summary>
    /// This class will convert a raw h264 stream into PES packets.
    /// It currently will only work with the PVS H264 encoder and assumes this stream
    /// has been created with the bluray setting which will add an AUD NAL type to the
    /// start of each video frame.  It will also ensure friendly I/B/P ordered frames.
    /// </summary>
    public class H264Packetizer : Packetizer
    {
        private byte streamID;
       
        private int PTStimeOffset = 0;
        private int DTStimeOffset = 0;

        //
        // PTS and frame step time are based on a 90khz clock
        //
        private double clockFrequency = 90000;
        private long basePTS = 54000000;    // 10 mins i.e. (54000000 / 90000) seconds
        private int frameStepTime; // 24 fps = 3750; // 25 fps = 3600

        //
        // This packetizer will buffer future frames in this list
        //
        private List<PES> bufferedFrames = new List<PES>();

        public H264Packetizer(string rawh264Filename, byte streamID, double fps) :
            base(rawh264Filename)
        {
            this.streamID = streamID;
            frameStepTime = (int)((1 / fps) * clockFrequency);
        }

        /// <summary>
        /// Returns the next PES packet, or null if the stream has ended
        /// </summary>
        /// <returns></returns>
        public PES GetNextPacket()
        {
            if (Reader == null)
            {
                return null;
            }
       
            //
            // Buffer to at least 8 frames
            //
            while (bufferedFrames.Count < 8)
            {
                PES nextPacket = GetNextPacketInternal();
                if (nextPacket == null)
                {
                    break;
                }
                bufferedFrames.Add(nextPacket);
            }

            if (bufferedFrames.Count == 0)
            {
                return null;
            }

            //
            // If next frame is an I or P frame then check there is no B frames after
            //           
            if (bufferedFrames[0].FrameType == PES.H264FrameType.IFrame ||
                bufferedFrames[0].FrameType == PES.H264FrameType.PFrame)
            {
                int nextCheckFrameIndex = 1;
                bool foundNextIOrPFrame = false;
                while (foundNextIOrPFrame == false)
                {
                    if (bufferedFrames.Count >= nextCheckFrameIndex + 1)
                    {
                        if (bufferedFrames[nextCheckFrameIndex].FrameType == PES.H264FrameType.BFrame)
                        {
                            nextCheckFrameIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        //
                        // No more frames to check
                        //
                        break;
                    }
                }

                if (nextCheckFrameIndex != 1)
                {
                    //
                    // Fill next b-frames with PTS
                    //
                    for (int i = 1; i < nextCheckFrameIndex; i++)
                    {
                        if (bufferedFrames[i].FrameType != PES.H264FrameType.BFrame)
                        {
                            BDMVLog.Error("Received a " + bufferedFrames[i].FrameType.ToString() + " but was expected a B-frame"); 
                        }
                        bufferedFrames[i].PresentTimeStamp = basePTS + PTStimeOffset;
                        PTStimeOffset += frameStepTime; ;
                    }
                }

                bufferedFrames[0].PresentTimeStamp = basePTS + PTStimeOffset;
                PTStimeOffset += frameStepTime;
            }

            PES nextToSend = bufferedFrames[0];
            bufferedFrames.RemoveAt(0);

            if (nextToSend.PresentTimeStamp == 0)
            {
                BDMVLog.Error("PTS not set for PES in h264 Packetizer");
            }

            return nextToSend;
        }

        //
        // Internal method to generate the next PES packet
        //
        private PES GetNextPacketInternal()
        {
            //
            // No data left? end now
            //
            if (PeekBytes(0, 1) == null)
            {
                return null;
            }

            bool completePez = false;
            bool foundStart = false;
            List<byte> PesData = new List<byte>();

            PES.H264FrameType nextFrameType = PES.H264FrameType.NotSet;      

            while (completePez == false)
            {
                byte[] nal = PeekBytes(0,4);

                //
                // End of file?
                //
                if (nal == null || nal.Length <4)
                {
                    if (nal != null)
                    {
                        AddBytes(PesData, nal.Length);
                    }

                    if (foundStart==true)
                    {
                        completePez = true;
                    }
                    else
                    {
                        BDMVLog.Warning("Unexpected data found at end of h264 file");
                    }
                    break;
                }

                bool foundFrame = false;        
                //
                // Is NAL header?
                //
                if (nal[1] == 0 && nal[2] == 0 && nal[3] == 1)
                {
                    //
                    // Peak next 3 bytes to get type
                    //
                    byte[] naltype = PeekBytes(4, 3);

                    if (naltype != null)
                    {
                        if (naltype.Length== 3)
                        {                         
                            //
                            // Is this AUD generated by h264 encoder
                            // 
                            if (nal[0] == 0 && naltype[0] == 9 )
                            {
                                //
                                // If start already found, then this is start of next PES, 
                                // end now as we have got a complete PES
                                //
                                if (foundStart == true)
                                {
                                    completePez = true;
                                    break;
                                }
                                foundStart = true;
                            }
                            
                           //
                           // Is this a H264 I/B/P frame?
                           //
                           if ( ( naltype[0] == 0x41 )||
                                ( naltype[1] == 0x9a || naltype[1] == 0x9e) || 
                                ( naltype[0] == 0x67 && naltype[1] == 0x64 && naltype[2] == 0) )
                            {
                                foundFrame = true;

                                if (naltype[0] == 1)
                                {
                                    nextFrameType = PES.H264FrameType.BFrame;
                                }
                                else if (naltype[0] == 0x41)
                                {
                                    nextFrameType = PES.H264FrameType.PFrame;
                                }
                                else
                                {
                                    nextFrameType = PES.H264FrameType.IFrame;
                                }

                                //
                                // Copy NAL into PES data
                                //
                                AddBytes(PesData, 7);
                            }                     
                        }                    
                    }
                }
                if (foundFrame == false)
                {
                    AddBytes(PesData, 1);
                }

                //
                // It is impossible to have a PES 8 meg or bigger.
                // If we have reached this limit then bail
                //
                if (PesData.Count > 1024 * 1024 * 8)
                {
                    BDMVLog.Error("PES too large");
                    break;
                }
            }

            if (PesData.Count > 0)
            {
                PES pes = new PES(streamID, PesData.ToArray(), 0, basePTS - frameStepTime + DTStimeOffset, false); // PTS is set later

                if (nextFrameType == PES.H264FrameType.NotSet)
                {
                    BDMVLog.Error("Frame type has no reconised NAL type when creating PES");
                }
                pes.FrameType = nextFrameType;
                DTStimeOffset += frameStepTime;

                return pes;
            }

            return null;
        }
    }
}
