using System;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Globalization;

namespace ManagedCore
{
	/// <summary>
	/// Summary description for CAppUpdater.
	/// </summary>
	public class Updater
	{	
		public static double mVersion = 0.1f;

		public Updater()
		{
		}


		private bool IsUpdateRequired(ref string new_version)
		{
			string current_dir = Environment.CurrentDirectory;

            string update_folder = "";
            try
            {    
#if (DEBUG)
                CNetFile nf = new CNetFile("versiontest.txt");
#else
				CNetFile nf= new CNetFile("version.txt");
#endif
                update_folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToString() + "\\PhotoVidShow";

                ManagedCore.IO.ForceDeleteIfExists(update_folder + "\\version.txt", false);

                bool g = nf.CopyToHarddrive(update_folder + "\\version.txt", false);
                if (g == false) return false;
                StreamReader reader = File.OpenText(update_folder + "\\" + "version.txt");
                string text = reader.ReadToEnd();
                reader.Close();
                int version = 100000000;
                try
                {
                    version = int.Parse(text);
                }
                catch
                {
                }

                if (version > (mVersion + 0.5))
                {
                    new_version = text;
                    return true;               
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                ManagedCore.IO.ForceDeleteIfExists(update_folder + "\\version.txt", false);
            }

		}

        //**************************************************************************************
		private void DownloadUpdateFiles(string root_dir)
		{
            /*
			string current_dir = root_dir;
			current_dir += "\\" + "update";
			if (Directory.Exists(current_dir)==true)
			{
				Directory.Delete(current_dir,true);
			}

			Directory.CreateDirectory(current_dir);

			XmlNodeList list= this.mUpdateDoc.GetElementsByTagName("Update");
			foreach (XmlElement e in list)
			{
				float from_v = float.Parse(e.GetAttribute("FromVersion"),CultureInfo.InvariantCulture);
			//	if (from_v == mVersion)
				{
					XmlNodeList file_list = e.ChildNodes;
					foreach (XmlElement e1 in file_list)
					{
						string file = e1.GetAttribute("name");
						Console.WriteLine("Getting file"+file+"...");

						CNetFile nf = new CNetFile(file);
						string f1 = System.IO.Path.GetFileNameWithoutExtension(file)+".zip";
						nf.CopyToHarddrive(current_dir+"\\"+f1);
						return ;
					}	
				}
			}
             */
		}

        //****************************************************************************************
		public bool CheckIfUpdateRequired(ref string new_version)
		{
			if (IsUpdateRequired(ref new_version)==false) 
			{
				return false;
			}
			return true;
		}

        //*******************************************************************************************
		// only call this you just called IsUpdateRequired and it returned true!! uses file it just downloaded
		public bool Upgrade(string root_directory)
		{
            return true;

            /*
            string update_folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToString() + "\\PhotoVidShow";

            DownloadUpdateFiles(update_folder);

			if (mUpdateDoc!=null)
			{
				mUpdateDoc=null;
			}
				
			try
			{
				File.Delete(update_folder+"\\update.xml");
			}
			catch (Exception e)
			{
				int i=1;
				i++;
			}

			string ss ="PhotoVidShow.exe";

			// ok if we get this far inform server of update

			ManagedCore.CNetFile netfile = new ManagedCore.CNetFile("");

			if (ManagedCore.License.License.RunningWithKey!=null &&
				mCachedNewVersion!=null)
			{
				int key_num = ManagedCore.License.License.RunningKeyNum;
#if (DEBUG)
                netfile.CallWebCGIScript("update.pl", "regnum=" + "Test" + " " + mCachedNewVersion);
#else

				netfile.CallWebCGIScript("update.pl","regnum="+key_num.ToString()+" "+mCachedNewVersion);
#endif
			}

			try
			{
				Directory.SetCurrentDirectory(root_directory);
				string prog = root_directory+"\\ReLauncher.exe";
			
				Process.Start(prog,ss+" update");
				Process p = Process.GetCurrentProcess();
				p.Kill();
			}
			catch (Exception)
			{
			}

			return true;
             */
			
		}
	}
}
