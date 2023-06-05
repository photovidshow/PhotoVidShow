using System;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.Management;
using System.Collections.Generic;

namespace ManagedCore
{
	/// <summary>
	/// Summary description for IO.
	/// </summary>
	/// 
	public delegate void StateChangeHandler();

	public class IO
	{

        // *********************************************************************************************************
        private static string mRootDir = @"c:\dev\dvdslideshow";

        private static List<String> mAcceptedDrives = new List<string>();

        // *********************************************************************************************************
        public static string GetRootDirectory()
        {
            return mRootDir;
        }

        // *********************************************************************************************************
        public static void SetRootDirectory(string value)
        {
            mRootDir = value;
        }

        // *********************************************************************************************************
        public static string StripRootHeader(string inputfilename)
        {
            inputfilename = inputfilename.ToLower();
            string root = GetRootDirectory().ToLower();

            if (inputfilename.StartsWith(root) == true)
            {
                inputfilename = inputfilename.Substring(root.Length + 1);
            }

            return inputfilename;
        }

        // *********************************************************************************************************
        public static string GetUserDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToString() + "\\PhotoVidShow";
        }

        // *********************************************************************************************************
        public static string GetDownloadMediaDirectory()
        {
            return GetUserDirectory() + @"\Media";
        }

		public IO()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        // *********************************************************************************************************
        public static void DeleteAllFileInDirectory(string directory)
        {
            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(directory);

            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                ForceDeleteIfExists(file.FullName, false);
            }
        }

        // *********************************************************************************************************
		public static bool ForceDeleteIfExists(string filename, bool pop_up_message_if_cant_delete)
		{
			try
			{
				if (File.Exists(filename)==true)
				{
					System.IO.FileInfo fi = new System.IO.FileInfo(filename);

					// remove readonly attribute
					if ((fi.Attributes & System.IO.FileAttributes.ReadOnly) != 0)
						fi.Attributes -= System.IO.FileAttributes.ReadOnly;

					File.Delete(filename);
				}
			}
			catch
			{
				CDebugLog.GetInstance().Error("Could not delete file '"+filename+"'. Please delete file manually");

				if (pop_up_message_if_cant_delete)
				{
					DialogResult dr = UserMessage.Show("PhotoVidShow can not delete file '"+filename+"'. Please delete this file manually and select ok, else select cancel","Error: Unable to delete file",
						MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
			
					if (dr == DialogResult.OK)
					{
						return ForceDeleteIfExists(filename, true);
					}
				}

				return false;
			}
			return true;
		}

        // *********************************************************************************************************
        public static long GetDirectorySize(string parentDir)
        {
            long totalFileSize = 0;

            string[] dirFiles = Directory.GetFiles(parentDir, "*.*", System.IO.SearchOption.AllDirectories);

            foreach (string fileName in dirFiles)
            {
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(fileName);
                totalFileSize = totalFileSize + info.Length;
            }
            return totalFileSize;
        }

        // *********************************************************************************************************
        public static bool VertifyEnoughDiskSpace(string path, long spaceRequired, string operation, bool estimation)
        {
            FileInfo file = new FileInfo(path);
            DriveInfo drive = new DriveInfo(file.Directory.Root.FullName);
            long spaceLeft = drive.TotalFreeSpace;
            long oneGig = 1024 * 1024 * 1024;
 
            while (spaceLeft < (spaceRequired + oneGig))
            {
                double spaceLeftGB = ((double)spaceLeft) / oneGig;
                double spaceRequiredGb = ((double)spaceRequired) / oneGig;

                StringBuilder builder = new StringBuilder();
                builder.Append("The amount of disc space left on drive ");
                builder.Append(drive.Name);
                builder.Append(" is too low when ");
                builder.Append(operation);
                builder.Append("\r\n\r\n");
                builder.Append("The amount left is ");
                builder.Append(spaceLeftGB.ToString("0.#"));
                if (estimation == false)
                {
                    builder.Append(" GB, amount required is ");
                }
                else
                {
                    builder.Append(" GB, amount estimated required is ");
                }
                builder.Append(spaceRequiredGb.ToString("0.#"));
                builder.Append(" GB.");
                builder.Append("\r\n\r\n");
                builder.Append("It is recommended to free some space then select retry.");

                DialogResult r = UserMessage.Show(builder.ToString(), "Low disc space", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);

                if (r == DialogResult.Ignore)
                {
                    return true;
                }
                if (r == DialogResult.Abort)
                {
                    return false;
                }

                drive = new DriveInfo(file.Directory.Root.FullName);
                spaceLeft = drive.TotalFreeSpace;
            }

            return true;
        }


        // *********************************************************************************************************
		public static byte[] ReadFully (Stream stream, int initialLength)
		{
			// If we've been passed an unhelpful initial length, just
			// use 32K.
			if (initialLength < 1)
			{
				initialLength = 32768;
			}
    
			byte[] buffer = new byte[initialLength];
			int read=0;
    
			int chunk;
			while ( (chunk = stream.Read(buffer, read, buffer.Length-read)) > 0)
			{
				read += chunk;
        
				// If we've reached the end of our buffer, check to see if there's
				// any more information
				if (read == buffer.Length)
				{
					int nextByte = stream.ReadByte();
            
					// End of stream? If so, we're done
					if (nextByte==-1)
					{
						return buffer;
					}
            
					// Nope. Resize the buffer, put in the byte we've just
					// read, and continue
					byte[] newBuffer = new byte[buffer.Length*2];
					Array.Copy(buffer, newBuffer, buffer.Length);
					newBuffer[read]=(byte)nextByte;
					buffer = newBuffer;
					read++;
				}
			}
			// Buffer is now too big. Shrink it.
			byte[] ret = new byte[read];
			Array.Copy(buffer, ret, read);
			return ret;
		}

        private static bool ignoreDriveTypeMessage = false;


        public static bool IsDriveAnExternUSBDrive(string driveLetter)
        {
            bool retVal = false;

            try
            {             
                driveLetter = driveLetter.TrimEnd('\\');

                // browse all USB WMI physical disks
                foreach (ManagementObject drive in new ManagementObjectSearcher("select DeviceID, MediaType,InterfaceType from Win32_DiskDrive").Get())
                {
                    // associate physical disks with partitions
                    ManagementObjectCollection partitionCollection = new ManagementObjectSearcher(String.Format("associators of {{Win32_DiskDrive.DeviceID='{0}'}} " + "where AssocClass = Win32_DiskDriveToDiskPartition", drive["DeviceID"])).Get();

                    foreach (ManagementObject partition in partitionCollection)
                    {
                        if (partition != null)
                        {
                            // associate partitions with logical disks (drive letter volumes)
                            ManagementObjectCollection logicalCollection = new ManagementObjectSearcher(String.Format("associators of {{Win32_DiskPartition.DeviceID='{0}'}} " + "where AssocClass= Win32_LogicalDiskToPartition", partition["DeviceID"])).Get();

                            foreach (ManagementObject logical in logicalCollection)
                            {
                                if (logical != null)
                                {
                                    // finally find the logical disk entry
                                    ManagementObjectCollection.ManagementObjectEnumerator volumeEnumerator = new ManagementObjectSearcher(String.Format("select DeviceID from Win32_LogicalDisk " + "where Name='{0}'", logical["Name"])).Get().GetEnumerator();

                                    volumeEnumerator.MoveNext();

                                    ManagementObject volume = (ManagementObject)volumeEnumerator.Current;

                                    if (driveLetter.ToLowerInvariant().Equals(volume["DeviceID"].ToString().ToLowerInvariant()) &&
                                        (drive["MediaType"].ToString().ToLowerInvariant().Contains("external") || drive["InterfaceType"].ToString().ToLowerInvariant().Contains("usb")))
                                    {
                                        retVal = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return retVal;
        }


        // *********************************************************************************************************
        public static bool IsDriveOkToUse(string path)
        {
            if (ignoreDriveTypeMessage == true)
            {
                return true;
            }

            bool result = true;

            try
            {
                string drive = System.IO.Path.GetPathRoot(path);


                if (mAcceptedDrives.Contains(drive) == true)
                {
                    return true;
                }

                //
                // If C drive, assume this is acceptable and continue
                //
                if (drive.ToLower().StartsWith("c:") == true)
                {
                    mAcceptedDrives.Add(drive);
                    return true;
                }

                //
                // Is drive a network location?
                //
                if (drive.StartsWith("\\\\") == true)
                {
                    result = false;
                }
                else
                {
                    DriveInfo d = new DriveInfo(drive);

                    //
                    // Is local drive fixed?
                    //
                    if (d.DriveType != DriveType.Fixed)
                    {
                        result = false;
                    }
                    else
                    {
                        result = !IsDriveAnExternUSBDrive(drive);
                    }
                }

                if (result == true)
                {
                    mAcceptedDrives.Add(drive);
                }
            }
            catch 
            {
                result = false;
            }

            if (result == false)
            {

                DialogResult r = UserMessage.Show("The location of this media is from a non local fixed drive (e.g. a CD drive, external USB hard-drive, USB stick or remote network drive).  " +
                                                           "For PhotoVidShow to work properly all media files must be imported from the local computer hard-drive." +
                                                           "\n\r\n\rIt is strongly recommended to copy your media files onto the local hard-drive first." +
                                                           "\n\r\n\rSelect 'Cancel' to abort or 'Retry' to import the media anyway.", "Non local fixed drive",
                                                           MessageBoxButtons.RetryCancel,
                                                           MessageBoxIcon.Exclamation);

                if (r != DialogResult.Retry)
                {
                    return false;
                }

                ignoreDriveTypeMessage = true;
            }
           
            return true;
        }

        private static string kksw = @"3476*&j-";

        // *********************************************************************************************************
        public static void ef(string inputFile, string outputFile)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] kk = UE.GetBytes(kksw);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(kk, kk),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                // write extra bytes to pad
                for (int i = 0; i < 5502; i++)
                {
                    cs.WriteByte(0);
                }

                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch (Exception e)
            {
                UserMessage.Show("Encryption failed! " + e.Message, "Error");
            }
        }

        // *********************************************************************************************************
        public static void df(string inputFile, string outputFile)
        {
            // Flag indicating we had to copy the pvso file before we could read it.
            bool forcedToCopyFileFirst = false;

            if (System.IO.File.Exists(outputFile) == true)
            {
                return;
            }

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] kk = UE.GetBytes(kksw);
            string copyFilename="";

            FileStream fsCrypt = null;
            try
            {
                fsCrypt = new FileStream(inputFile, FileMode.Open);
            }
            catch (Exception e)
            {
                Log.Info("Exception occurred when reading decrypt file '"+inputFile +"' :"+e.Message);
                if (System.IO.File.Exists(inputFile) == true)
                {
                    // On Windows7, we get file access denied, if we are for example the Guest account and the pvso lives 
                    // in a sub folder of c:\dev\pvs
                    // Why this happens for .pvso files and not images i do not know.  If we copy the pvso file to temppath
                    // Then read it, it seems to work!
                    copyFilename = System.IO.Path.GetDirectoryName(outputFile) + "\\" + System.IO.Path.GetFileName(inputFile);

                    try
                    {
                        System.IO.File.Copy(inputFile, copyFilename);
                        fsCrypt = new FileStream(copyFilename, FileMode.Open);
                        forcedToCopyFileFirst= true;
                    }
                    catch (Exception e2)
                    {
                        Log.Error("Exception occurred in df, input file '" + inputFile + "' outputfile '" + outputFile +"' ," + e2.Message);
                        return;
                    }
                }
                else
                {
                    Log.Error("Exception occurred in df, input file does not exist '" + inputFile + "' outputfile '" + outputFile + "' ," + e.Message);
                    return;
                }
            }

            RijndaelManaged RMCrypto = new RijndaelManaged();

            CryptoStream cs = new CryptoStream(fsCrypt,
                RMCrypto.CreateDecryptor(kk, kk),
                CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            int data;
            while ((data = cs.ReadByte()) != -1)
                fsOut.WriteByte((byte)data);

            fsOut.Close();
            cs.Close();
            fsCrypt.Close();

            // If we had to copy the pvso file, delete the copy
            if (forcedToCopyFileFirst==true)
            {
                IO.ForceDeleteIfExists(copyFilename, false);
            }
            
        }

        // **********************************************************************************************************
        //
        // The safesest way is to physically try and create a file at the folder and check if it throws an exception
        // or not.
        //
        public static bool HasWriteAccessToDirectory(string directory)
        {
            try
            {
                using (FileStream fs = File.Create(
                    Path.Combine(
                        directory,
                        Path.GetRandomFileName()
                    ),
                    1,
                    FileOptions.DeleteOnClose)
                )
                { }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
