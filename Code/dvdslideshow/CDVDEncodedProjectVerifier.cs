using System;
using System.Collections;
using System.Text;
using System.IO;
using ManagedCore;

namespace DVDSlideshow
{
    //
    // This class is used to verify that an valid DVD encoded project was created.  
    // This class is typically used if the burn stage fails because it could not burn from a VIDEO_TS folder becuase the VIDEO_TS
    // folder was invalid.
    // 
    public class CDVDEncodedProjectVerifier
    {
        private CProject mProject;
        private string mMpeg2fileslocation;
        private string mDVDroot;

        public CDVDEncodedProjectVerifier(CProject project, string mpeg2fileslocation, string DVDroot)
        {
            mProject = project;
            mMpeg2fileslocation = mpeg2fileslocation;
            mDVDroot = DVDroot;
        }

        // ***********************************************************************
        private void ListAllFilesInDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                try
                {
                    FileInfo fi = new FileInfo(file);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(file);
                    sb.Append(" ");
                    sb.Append(fi.LastWriteTime);
                    sb.Append(" ");
                    sb.Append(fi.Length);
                    Log.Error(sb.ToString());
                }
                catch (Exception e)
                {
                    Log.Error("Could not read file:" + file + " " + e.Message);
                }
            }
        }

        // *************************************************************************************
        public void Check()
        {
            try
            {
                if (mProject.PreMenuSlideshow != null)
                {
                    Log.Error("Project contains intro slideshow");
                    Log.Error("Intro slideshow slide count = " + mProject.PreMenuSlideshow.mSlides.Count);
                }

                ArrayList slideshows = mProject.GetAllProjectSlideshows(false);
                Log.Error("Slideshow count = " + slideshows.Count);
                int slideshowCount = 0;
                foreach (CSlideShow ss in slideshows)
                {
                    Log.Error("Slideshow " + slideshowCount + " slide count = " + ss.mSlides.Count);
                    slideshowCount++;
                }

                ArrayList menus = mProject.MainMenu.GetSelfAndAllSubMenus();
                Log.Error("Menus count = " + menus.Count);

                Log.Error("Verifying VIDEO_TS folder");

                string video_Ts_folder = mDVDroot + "\\VIDEO_TS";
                if (Directory.Exists(video_Ts_folder) == false)
                {
                    Log.Error("No VIDEO_TS folder at " + mDVDroot);
                    return;
                }

                Log.Error("Dumping VIDEO_TS folder");
                ListAllFilesInDirectory(video_Ts_folder);
                if (mMpeg2fileslocation != "")
                {
                    Log.Error("Dumping mpeg folder");
                    ListAllFilesInDirectory(mMpeg2fileslocation);
                }
                Log.Error("Done....");
            }
            catch (Exception exception)
            {
                Log.Error("Error on video_ts check: "+exception.ToString());
            }
        }
    }
}
