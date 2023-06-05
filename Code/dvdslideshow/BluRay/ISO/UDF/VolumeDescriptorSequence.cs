using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    public class VolumeDescriptorSequence
    {
        /* MVDS */
        /* -------------------------------------------------------- */
        /* | PVD | IUVD | PD | LVD | USD | TD | ...   | LVID | TD | */
        /* -------------------------------------------------------- */

        /* RVDS */
        /* ------------------------------------ */
        /* | PVD | IUVD | PD | LVD | USD | TD | */
        /* ------------------------------------ */

        private PrimaryVolumeDescriptor primaryVolumeDescriptor ;
        private ImplementationUseVolumeDescriptor implementationUseVolumeDescriptor;
        private PartitionDescritor partitionDescritor;
        private LogicalVolumeDescriptor logicalVolumeDescriptor;
        private UnallocatedSpaceDescriptor unallocatedSpaceDescriptor = new UnallocatedSpaceDescriptor();
        private TerminatingDescriptor terminatingDescritor = new TerminatingDescriptor(0);
        private LogicalVolumeIntegrityDescriptor logicalVolumeIntegrityDescriptor;

        public PartitionDescritor PartitionDescritor
        {
            get
            {
                return partitionDescritor;
            }
        }

        public LogicalVolumeDescriptor LogicalVolumeDescriptor
        {
            get
            {
                return logicalVolumeDescriptor;
            }
        }

        private PrimaryVolumeDescriptor PrimaryVolumeDescriptor
        {
            get
            {
                return primaryVolumeDescriptor;
            }
        }

        public ImplementationUseVolumeDescriptor ImplementationUseVolumeDescriptor
        {
            get
            {
                return implementationUseVolumeDescriptor;
            }
        }

        public UnallocatedSpaceDescriptor UnallocatedSpaceDescriptor
        {
            get
            {
                return unallocatedSpaceDescriptor;
            }
        }

        public TerminatingDescriptor TerminatingDescritor
        {
            get
            {
                return terminatingDescritor;
            }
        }

        public LogicalVolumeIntegrityDescriptor LogicalVolumeIntegrityDescriptor
        {
            get
            {
                return logicalVolumeIntegrityDescriptor;
            }
        }

        public VolumeDescriptorSequence(string rootFolder, string volumeSetIdentifier, string volumeIdentifier, string applicationName, bool streamFolderFirst)
        {
            primaryVolumeDescriptor = new PrimaryVolumeDescriptor(volumeSetIdentifier, volumeIdentifier, applicationName);
            implementationUseVolumeDescriptor = new ImplementationUseVolumeDescriptor(volumeIdentifier, applicationName);
            partitionDescritor = new PartitionDescritor(rootFolder, volumeIdentifier, applicationName, streamFolderFirst);
            logicalVolumeIntegrityDescriptor = new LogicalVolumeIntegrityDescriptor(partitionDescritor.FileSystemTree.NumberOfFiles, partitionDescritor.FileSystemTree.NumberOfDirectories, partitionDescritor.PartitionLengthInSectors, applicationName);
            logicalVolumeDescriptor = new LogicalVolumeDescriptor(0, partitionDescritor.FileSystemTree.MetadataMirrorFileLocation, volumeIdentifier, applicationName);
        }


        public void Write(BinaryWriter writer, bool reserveTable)
        {
            //
            // Sector 1 of VDS contains primary volume descriptor PVD
            //
            primaryVolumeDescriptor.Write(writer);

            //
            // Write out rest of sector 1 as PVD is only 512 bytes
            //
            byte[] blank = new byte[1536];
            writer.Write(blank);

            //
            // Sector 2 IUVD
            //
            ImplementationUseVolumeDescriptor.Write(writer);

            //
            // Write out rest of sector 2 as IUVD is only 512 bytes
            //
            writer.Write(blank);

            //
            // Sector 3 is Partition descriptor
            //
            partitionDescritor.Write(writer);

            //
            // Write out rest of sector 3 as PD is only 512 bytes
            //
            writer.Write(blank);

            //
            // Sector 4 is LVD
            //
            logicalVolumeDescriptor.Write(writer);

            //
            // Write out remainder of sector as blank
            //
            int lengthLVD = logicalVolumeDescriptor.LengthInBytes();
            Byte [] blankLVDPart = new byte[2048 - lengthLVD];
            writer.Write(blankLVDPart);

            //
            // Sector 5 is Unallocated Space Descriptor
            //
            UnallocatedSpaceDescriptor.Write(writer);

            //
            // Write out rest of sector 1 as USD is only 24 bytes
            //
            byte[] usdBlank = new byte[2048-24];
            writer.Write(usdBlank);

            //
            // Sector 6 is Terminating Descritor
            //
            terminatingDescritor.Write(writer);
            //
            // Write out rest of sector 6 as TD is only 512 bytes
            //
            writer.Write(blank);

            if (reserveTable == false)
            {
                // 
                //  next 26 sectors are blank 
                //
                byte[] blankSector = new byte[2048];

                for (int i = 0; i < 26; i++)
                {
                    writer.Write(blankSector);
                }

                //
                // Write out Logical Volume Integrity Descriptor
                //
                logicalVolumeIntegrityDescriptor.Write(writer);

                //
                // Write out rest of sector as blank
                //
                byte[] lviBlank = new byte[2048 - logicalVolumeIntegrityDescriptor.GetLengthInBytes()];
                writer.Write(lviBlank);

                // Write out TD again
                terminatingDescritor.Write(writer);
                //
                // Write out rest of sector 6 as TD is only 512 bytes
                //
                writer.Write(blank);
            }
        }
    }
}
