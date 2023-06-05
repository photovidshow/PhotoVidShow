using System;
using System.Collections.Generic;
using System.Text;
using MangedToUnManagedWrapper;
using DVDSlideshow.BluRay.ISO;
using DVDSlideshow.BluRay.ISO.UDF;
using System.IO;
using System.Windows.Forms;
using ManagedCore;

namespace DVDSlideshow
{
    public class CBluRayVideoWriter : CVideoWriter
    {
        private CManagedVideoDisk	mBluRayBurnerDevice;
        private string mAVCHDFolder;
        private string mVolumeName;
        private bool mDoPadding;
        private CGlobals.VideoType mDiscType;
        private bool mCreatingIso = false;

        public const long BluRaySizeInBytes = 25000000000;

        //*******************************************************************
        public CBluRayVideoWriter(CManagedVideoDisk device, string path, string volumeName, bool doPadding, CGlobals.VideoType discDetectedType)
		{
            mBluRayBurnerDevice = device;
            mAVCHDFolder = path;
            mVolumeName = volumeName;
            mDoPadding = doPadding;
            mDiscType = discDetectedType;
		}

        private string GetTempIsoFile()
        {
            string tempIso = CGlobals.GetTempDirectory() + "\\temp.iso";
            return tempIso;
        }


        //*******************************************************************
        public override bool CreateIso()
        {
            mCreatingIso= true;
            try
            {
                return CreateBlurayIso(mAVCHDFolder, GetTempIsoFile(), mVolumeName);
            }
            finally
            {
                mCreatingIso = false;
            }
        }

        //*******************************************************************
        public override bool CreatingIso()
        {
            return mCreatingIso;
        }

        //*******************************************************************
		// returns 0 if there was an error
		public override int BurnFromPath(bool do_orig)
		{
            if (this.mBluRayBurnerDevice == null) return 10;

            string tempIso = GetTempIsoFile();
            //
            // First make an iso from the AVCHD folders
            //
          
            FileInfo f = new FileInfo(tempIso);
            long size = f.Length;

            long diskSize = CBluRayVideoWriter.BluRaySizeInBytes;
            string diskSizeString = "23gb";

            //
            // Are we trying to burn a bluray is to a DVD which is allowed
            //
            if (mDiscType == CGlobals.VideoType.DVD)
            {
                diskSize = 4700373992;
                diskSizeString = "4.3gb";
            }

            if (size > diskSize)
            {
                DialogResult res = UserMessage.Show("The disc image is over " + diskSizeString + ", this is unlikely to burn correctly, burn anyway?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                 if (res == DialogResult.Cancel)
                 {
                     return 33;
                 }
             }

            DateTime now = DateTime.Now;

            int errors = mBluRayBurnerDevice.Burn(tempIso,
                                      "",
                                      "",
                                      0,
                                      now.Minute,
                                      now.Hour,
                                      now.Day,
                                      now.Month,
                                      now.Year,
                                      mDoPadding,
                                      false);

             ManagedCore.IO.ForceDeleteIfExists(tempIso, false);

             if (errors == 0) return 1;
             if (errors == 1) return 0;
             return errors;
		}


        //*******************************************************************
        private bool CreateBlurayIso(string root, string isoFilename, string volumeName)
        {
            IsoUdfGenerator generator = new IsoUdfGenerator();
            generator.RootFolder = root;
            generator.VolumeIdentifier = volumeName;
         
            try
            {
                ManagedCore.IO.ForceDeleteIfExists(isoFilename, false);

                long sizeOfBluRayFileStructure = ManagedCore.IO.GetDirectorySize(root);
                bool result = ManagedCore.IO.VertifyEnoughDiskSpace(isoFilename, sizeOfBluRayFileStructure, "building the blu-ray iso file.", false);
                if (result == false)
                {
                    ManagedCore.Log.Warning("User abandoned building blu-ray iso, as disc space too low..");
                    return false;
                }
                generator.GenerateIso(isoFilename);
            }
            catch (Exception e)
            {
                ManagedCore.Log.Error("Failed to create Blu-ray iso from path '" + root + "' "+e.ToString());
                return false;
            }
            return true;
        }

        //*******************************************************************
		public override void AbortBurn()
		{
            BluRay.ISO.UDF.IsoUdfGenerator.AbortCreation = true;
            mBluRayBurnerDevice.AbortBurn();
		}

        //*******************************************************************
		public override int GetWriteSpeed()
		{
            return mBluRayBurnerDevice.GetWriteSpeed();
		}

        //*******************************************************************
		public override double GetBurnTimeEstimation()
		{
            // Burn estimation times currently turned off, becuase estimation does not work (G919)
            return 0;
		}
    }
}
