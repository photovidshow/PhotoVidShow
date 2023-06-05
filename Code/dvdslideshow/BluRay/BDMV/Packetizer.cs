using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This abstract class provides basic file reading capabilites for packetization
    /// i.e. peeking/reading x bytes from a file (stream)
    /// It also allows a zero stream to be generated (e.g. used by LPCM packetizer if there is no audio)
    /// </summary>
    public abstract class Packetizer : IDisposable
    {
        private FileStream reader;
        private long totalIndex = 0;
        private static readonly int BufferSize = 1024 * 512;
        private byte[] buffer = new byte[BufferSize];
        private int fileEndMarker = -1;
        private int index = 0;
        private bool zeroStream;    // if set to true we always return 0's for PeekBytes

        protected FileStream Reader
        {
            get { return reader; }
        }

        /// <summary>
        /// Constructor for zero stream packetizer
        /// </summary>
        /// <param name="zeroStream"></param>
        public Packetizer()
        {
            this.zeroStream = true;
        }

        /// <summary>
        /// Constructor a packetizer from a given stream file
        /// </summary>
        /// <param name="filename"></param>
        public Packetizer(string filename)
        {
            zeroStream = false;
            try
            {
                reader = File.OpenRead(filename);
                int read = reader.Read(buffer, 0, BufferSize);
                if (read < BufferSize)
                {
                    fileEndMarker = read;
                }
            }
            catch
            {
                BDMVLog.Error("Could not open file '" + filename+"'");
            }
        }

        /// <summary>
        /// Returns 'amount' bytes from the current poistion + the given offset.
        /// It does not move the current position forward.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        protected byte[] PeekBytes(int offset, int amount)
        {
            if (zeroStream == true)
            {
                byte[] returnBytes = new byte[amount];
                return returnBytes;
            }

            if (reader == null)
            {
                return null;
            }

            //
            // Does peek go beyond end of file?
            //
            if ((fileEndMarker != -1) && (index + offset + amount > fileEndMarker))
            {
                //
                // Is the 0 bytes left in file, return null
                //
                if (index == fileEndMarker)
                {
                    return null;
                }

                byte[] returnBytes = new byte[fileEndMarker - index];
                for (int i = 0; i < returnBytes.Length; i++)
                {
                    returnBytes[i] = buffer[index + i];
                }
                return returnBytes;

            }

            //
            // Need to read more data
            // 
            if (fileEndMarker == -1 && (index + offset + amount > buffer.Length))
            {
                int start = 0;
                int leftInBuffer = buffer.Length - index;
                for (int i = 0; i < leftInBuffer; i++)
                {
                    buffer[start++] = buffer[index++];
                }
                index = 0;

                int toRead = BufferSize - leftInBuffer;
                int readBytes = reader.Read(buffer, leftInBuffer, toRead);
                if (readBytes < toRead)
                {
                    //
                    // Could not read all bytes (eof near, mark fileEndMarker in buffer
                    //  
                    fileEndMarker = leftInBuffer + readBytes;
                }


                return PeekBytes(offset, amount);
            }

            // return data
            byte[] returnData = new byte[amount];
            for (int i = 0; i < amount; i++)
            {
                returnData[i] = buffer[index + offset + i];
            }
            return returnData;

        }

        /// <summary>
        /// Gets the given 'amount' bytes from the current position.
        /// It the moves the current postion forward the same amount of bytes.
        /// A peek must be done before a call to this method.
        /// </summary>
        /// <param name="pesData"></param>
        /// <param name="amount"></param>
        protected void AddBytes(List<byte> pesData, int amount)
        {
            //
            // Assume peek done i.e. assume there is ebough data in buffer
            //
            for (int i = 0; i < amount; i++)
            {
                pesData.Add(buffer[index++]);
            }

            totalIndex += amount;
        }

        /// <summary>
        /// Moves the current position forwad the given amount.
        /// A peek must be done before a call to this method.
        /// </summary>
        /// <param name="amount"></param>
        protected void MoveIndex(int amount)
        {
            //
            // Assume peek done i.e. assume there is ebough data in buffer
            //
            index += amount;
            totalIndex += amount;
        }

        /// <summary>
        /// Closes the stream file if exists and open
        /// </summary>
        public void Dispose()
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
    }
}
