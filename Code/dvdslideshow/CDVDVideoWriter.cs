using System;
using MangedToUnManagedWrapper;
using System.IO;
using System.Windows.Forms;
using ManagedCore;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CDVDVideoWriter.
	/// Burns a DVD video stored on hard drive to a blank DVD disk
	/// Directory required is a standard DVD VIDEO_TS directory 
	/// (+ all correct VOB BUP IFO files etc stored in it)
	/// Also an Original_files.zip may be included in route directory
	/// </summary>
	/// 

	public class CDVDVideoWriter : CVideoWriter
	{
		private CManagedVideoDisk	mDVDBurnerDevice;
		private string mRoutePath;
		private string mVolumeName;
		private bool mDoPadding;

        public const long DVDSizeInBytes = 4700373992;

		public CDVDVideoWriter(CManagedVideoDisk device, string path, 
							   string volume_name,
						       bool do_padding)
		{
			mDVDBurnerDevice=device;
			mRoutePath=path;
			mVolumeName= volume_name;
			mDoPadding=do_padding;


			//
			// TODO: Add constructor logic here
			//
		}

        private long DirSize(DirectoryInfo d)
        {

            long Size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }


		// returns 0 if there was an error
		public override int BurnFromPath(bool do_orig)
		{
			if (this.mDVDBurnerDevice==null) return 10;

			string org_files_name = "";
            int num_orig_files_count = 0;
				
			if (do_orig==true)
			{
				org_files_name = mRoutePath+"\\VIDEO_TS\\"+CGlobals.mOriginalFilesZipFilename;

                num_orig_files_count = 1;
                string ss = org_files_name + num_orig_files_count.ToString() + ".zip";

				while (System.IO.File.Exists(ss)==true)
				{
                    num_orig_files_count++;
                    ss = org_files_name + num_orig_files_count.ToString() + ".zip";
				}
                num_orig_files_count--;
                if (num_orig_files_count == 0) org_files_name = "";

			}
			string dvd_path  = mRoutePath ; // +"\\VIDEO_TS";

            // get size of video_ts folder

            DirectoryInfo d = new DirectoryInfo(mRoutePath + "\\VIDEO_TS");

            long size = DirSize(d);
            if (size >  CDVDVideoWriter.DVDSizeInBytes)
            {
                DialogResult res = UserMessage.Show("The disc image is over 4.7gb, this is unlikely to burn correctly, burn anyway?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res == DialogResult.Cancel)
                {
                    return 32;
                }
            }

			DateTime now = DateTime.Now;

			int errors = mDVDBurnerDevice.Burn(dvd_path,
										 mVolumeName,
										 org_files_name,
                                         num_orig_files_count,
										 now.Minute,
										 now.Hour,
										 now.Day,
										 now.Month,
										 now.Year,
										 mDoPadding,
									     false) ;

			if (errors ==0) return 1;
            if (errors == 1) return 0;
			return errors;
		}

		public override void AbortBurn()
		{
			mDVDBurnerDevice.AbortBurn();
		}

		public override int GetWriteSpeed()
		{
			return mDVDBurnerDevice.GetWriteSpeed();
		}


		// how long da ya recond to burn this folder
		public override double GetBurnTimeEstimation()
		{
            // Burn estimation times currently turned off, becuase estimation does not work (G919)
            return 0;
 
            /*
			// get all files in directory
			long total_bytes =0;

			string path = mRoutePath+"\\VIDEO_TS";

            // ### SRG TO DO this is nuts just get all files under root

			System.IO.DirectoryInfo dirInfo = new DirectoryInfo(path);
			foreach(System.IO.FileInfo file in dirInfo.GetFiles())
			{
				total_bytes += file.Length;
			}

            // get all the original zip files

            int original_files_count = 1;
            string of = path + "\\" + CGlobals.mOriginalFilesZipFilename + original_files_count.ToString() + ".zip";
       
            while (File.Exists(of) == true)
            {
                System.IO.FileInfo fileinfo = new FileInfo(of);
                total_bytes += fileinfo.Length;
                original_files_count++;
                of = path + "\\" + CGlobals.mOriginalFilesZipFilename + original_files_count.ToString() + ".zip";      
            }

			
			// size to burn
			long size = total_bytes;
			if (size==0) return 0;

			// in k
			size /=1024;

			if (mDoPadding && size < 1024 * 1024)
			{
				size = 1024*1024;
			}

			int kilo_bytes_a_second = GetWriteSpeed();

			double ss = size;
			ss /=kilo_bytes_a_second;
			return ss+CGlobals.mBurnLeadInandOutTime;
             */
		}
	}
}
