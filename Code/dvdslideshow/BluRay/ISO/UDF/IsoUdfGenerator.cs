using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class can generate a UDF v2.5 ISO file
    /// Currently this is only used to generate a Blu-Ray ISO and may contain hard coded values to suit this purpose.
    /// </summary>
    public class IsoUdfGenerator
    {
        //
        // Static functions, used to test
        //
        public static bool UsePSTimeStamps = false; // PS = Proshow
        public static bool GenerateExtendedAttributes = false;

        public static  DateTime GetTimeStamp()
        {
            if (UsePSTimeStamps == true)
            {
                DateTime proShowDataTime = new DateTime(2014, 12, 7, 12, 5, 24);
                return proShowDataTime;
            }

            return DateTime.Now;
        }

        static public DateTime GetTimeStamp1(DateTime time)
        {
            if (UsePSTimeStamps == true)
            {
                return new DateTime(time.Year, time.Month, time.Day, time.Hour + 1, time.Minute, time.Second);
            }
            return time;
        }

        public static DateTime GetAccessTime(DateTime time, string name)
        {
            if (UsePSTimeStamps == true)
            {
                if (name == "00000.m2ts")
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 49);
                }
                if (name == "00001.m2ts")
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 04, 56);
                }
                if (name == "00002.m2ts")
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 8);
                }
            }
            return time;
        }


        public static DateTime GetCreatingTime(DateTime time, string name)
        {
            if (UsePSTimeStamps == true)
            {
                if (name == "CLIPINF" || name == "JAR" || name == "PLAYLIST")
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 4, 49);
                }

                if (name == "00000.m2ts")
                {
                    return new DateTime(time.Year, time.Month, time.Day, 11, 54, 56);
                }

                if (name == "00001.m2ts")
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 04, 56);
                }

                if (name == "00002.m2ts")
                {
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 8);
                }
            }

            return time;
        }


        private string volumeSetIdentifier = "";
        private string volumeIdentifier = "PHOTOVIDSHOW_BD";
        private string applicationName = "PhotoVidShow";
        private string rootFolder = "";   
     
        //
        //  If set to true the STREAM folder on a blu-ray disc is set as the first file entry; else it is ordered in alphabetical order
        //
        private bool blurayStreamFolderFirst = false;

        private VolumeRegconitionSequence volumeRegconitionSequence = new VolumeRegconitionSequence();
        private AnchorVolumeDescriptorPointer anchorVolumePointer = null;
        private VolumeDescriptorSequence volumeDescriptorSequence = null;

        public static bool AbortCreation = false;

        public string VolumeSetIdentifier
        {
            get { return volumeSetIdentifier; }
            set 
            {
                if (value.Length > 16)
                {
                    volumeSetIdentifier = value.Substring(0, 16);
                    // UDFLog.Warning("Volume set identifer name too long (truncating)");
                }
                else
                {
                    volumeSetIdentifier = value;
                }
            }
        }

        public string VolumeIdentifier
        {
            get { return volumeIdentifier; }
            set 
            {
                if (value.Length > 16)
                {
                    volumeIdentifier = value.Substring(0, 16);
                    // UDFLog.Warning("Volume identifer name too long (truncating)");
                }
                else
                {
                    volumeIdentifier = value;
                }
            }
        }

        public string ApplicationName
        {
            get { return applicationName; }
            set
            {
                if (value.Length > 16)
                {
                    applicationName = value.Substring(0, 16);
                    // UDFLog.Warning("Application name too long (truncating)");
                }
                else
                {
                    applicationName = value;
                }
            }
        }

        public string RootFolder
        {
            get { return rootFolder; }
            set { rootFolder = value; }
        }

        public bool BlurayStreamFolderFirst
        {
            get { return blurayStreamFolderFirst; }
            set { blurayStreamFolderFirst = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private VolumeRegconitionSequence VolumeRegconitionSequence
        {
            get
            {
                return volumeRegconitionSequence;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private AnchorVolumeDescriptorPointer AnchorVolumeDescriptorPointer
        {
            get
            {
                return anchorVolumePointer;
            }
        }

        private VolumeDescriptorSequence VolumeDescriptorSequence
        {
            get
            {
                return volumeDescriptorSequence;
            }
          
        }

        public IsoUdfGenerator()
        {
            //
            // Set volumeSetIdentifier to something unique by default. e.g. starts with a timestamp
            //
            DateTime now = DateTime.Now;
            StringBuilder builder = new StringBuilder();
            builder.Append(now.Year.ToString("D2"));
            builder.Append(now.Month.ToString("D2"));
            builder.Append(now.Day.ToString("D2"));
            builder.Append(now.Hour.ToString("D2"));
            builder.Append(now.Minute.ToString("D2"));
            builder.Append(now.Second.ToString("D2"));
            builder.Append("UDF0001");
            volumeSetIdentifier = builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isoFilename"></param>
        /// <returns></returns>
        public void GenerateIso(string isoFilename)
        {
            AbortCreation = false;

            if (rootFolder == "")
            {
                UDFLog.Error("Root folder not set on call to generate");
                throw new Exception("Root folder not set");
            }

            if (Directory.Exists(rootFolder) == false)
            {
                UDFLog.Error("Root folder does not exist on call to generate");
                throw new Exception("Root folder not found");
            }

            IsoFile file = IsoFile.CreateNewFromFilename(isoFilename);
            try
            {
                volumeDescriptorSequence = new VolumeDescriptorSequence(rootFolder, volumeSetIdentifier, volumeIdentifier, applicationName, blurayStreamFolderFirst);

                BinaryWriter writer = file.Writer;

                //
                // First 16 sectors are blank
                //
                file.WriteBlankSectors(16);

                //
                // Sectors 16...32 contain volume regconition sequence
                //
                volumeRegconitionSequence.Write(file);

                //
                // Sectors 32...48 contain volume descriptor sequence
                //
                volumeDescriptorSequence.Write(writer, false);

                // 48 to ... stores LVID  (done in above step)

                //
                //  Sector ... to 255 contain blank RVRS
                //
                file.WriteBlankSectors(190);

                //
                // Sector 256 contains anchor volume descriptor pointer
                //

                anchorVolumePointer = new AnchorVolumeDescriptorPointer(volumeDescriptorSequence.PartitionDescritor.PartitionLengthInSectors + 288);
                anchorVolumePointer.Write(writer);

                //
                // Write out rest of sector 1 as AVP is only 512 bytes
                //
                byte[] blank = new byte[1536];
                writer.Write(blank);

                //
                // Next 31 sectors blank 0x80800 to 0x90000
                //
                file.WriteBlankSectors(31);

                ExtendedFileEntry topEntry = volumeDescriptorSequence.PartitionDescritor.FileSystemTree.TopFileEntry;

                //
                // Write file system tree
                //
                UDFLog.Info("Writing FS Tree");
                topEntry.Write(writer);
                topEntry.LinkedEntry.PartitionOffset = topEntry.PartitionOffset + 32;
                topEntry.LinkedEntry.PadAndWrite(writer);

                //
                // Write files
                //
                topEntry.WriteFiles(288, writer);

                if (IsoUdfGenerator.AbortCreation == true)
                {
                    UDFLog.Info("Aborted Blu-ray iso creation");
                    return;
                }

                //
                //  Pad out sector
                //
                long currentPosition = writer.BaseStream.Position;
                if ((currentPosition % 2048) != 0)
                {
                    byte[] pad = new byte[2048 - (currentPosition % 2048)];
                    writer.Write(pad);
                }

                //
                // Continue write until we reach the next 32 aligned sector
                //
                while ((writer.BaseStream.Position % (32 * 2048)) != 0)
                {
                    file.WriteBlankSectors(1);
                }

                //
                // Write out mirror file system tree
                // First Change the partition map to mirror file typel
                //
                topEntry.Icb.ICBFileType = 251;

                //
                // Apply mirror offset sector to top entries
                // 
                uint mirrorSector = this.volumeDescriptorSequence.PartitionDescritor.FileSystemTree.MetadataMirrorFileLocation;
                ShortAd vdsLocation = topEntry.StoredAddress;
                vdsLocation.SectorLocation = vdsLocation.SectorLocation + mirrorSector;
                uint adSectorLocation = topEntry.AllocationDescriptorSet[0].SectorLocation;
                topEntry.AllocationDescriptorSet[0].SectorLocation = adSectorLocation + mirrorSector;

                //
                //
                // Write file system tree again
                //
                UDFLog.Info("Writing mirror FS Tree");

                topEntry.Write(writer);
                topEntry.LinkedEntry.PartitionOffset = mirrorSector + 288 + 32;
                topEntry.LinkedEntry.PadAndWrite(writer);

                //
                // Write out blank until end of partition
                //
                while ((writer.BaseStream.Position / 2048) < volumeDescriptorSequence.PartitionDescritor.PartitionLengthInSectors + 288)
                {
                    file.WriteBlankSectors(1);
                }

                //
                // Write reserve vds
                //
                volumeDescriptorSequence.Write(writer, true);

                //
                // Continue write until we reach last sector of a 32 align sector
                //
                while ((writer.BaseStream.Position % (32 * 2048)) != (31 * 2048))
                {
                    file.WriteBlankSectors(1);
                }

                //
                // Last sector contains repeat of anchor volume descriptor pointer
                //
                anchorVolumePointer.Write(writer);
                //
                // Write out rest of sector 1 as AVP is only 512 bytes
                //
                writer.Write(blank);
            }
            finally
            {
                file.Close();
            }
        }
    }
}
