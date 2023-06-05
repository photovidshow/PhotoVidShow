using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using ManagedCore;
using DVDSlideshow.GraphicsEng;
using DVDSlideshow.GraphicsEng.Direct3D;
using PhotoCruzGui;
using System.Threading;
using System.Collections;

namespace PhotoCruzCL
{
    public partial class PhotoCruzCLWindowHandle : Form
    {
        private int width;
        private int height;
        private float fps;
        private CGlobals.MP4Quality quality;
        private bool useMB;
        private bool doMp4 = true;
        private bool doDVD = false;
        private bool doBD = false;
        private bool reCreate = false;
        private PhotoCruzMainWindow mPhotoCruzMainWindow = null;

        // ****************************************************************************************
        public PhotoCruzCLWindowHandle(int width, int height, float fps, CGlobals.MP4Quality quality, bool useMB, bool doMp4, bool doDVD, bool doBD, bool reCreate)
        {
            this.width = width;
            this.height = height;
            this.fps = fps;
            this.quality = quality;
            this.useMB = useMB;
            this.doMp4 = doMp4;
            this.doDVD = doDVD;
            this.doBD = doBD;
            this.reCreate = reCreate;

            InitializeComponent();
            this.Shown += new EventHandler(this.FirstStartedCallback);
        }

        // ****************************************************************************************
        private void LoadProjectIfNeeded()
        {
            if (mPhotoCruzMainWindow == null)
            {
                mPhotoCruzMainWindow = new PhotoCruzMainWindow();
                mPhotoCruzMainWindow.LoadPhotoCruzProject(null);
            }
            else
            {
                mPhotoCruzMainWindow.LoadPhotoCruzProject(null);
                GC.Collect();
            }
        }

        // ****************************************************************************************
        private void FirstStartedCallback(object o, System.EventArgs e)
        {
            try
            {
                Panel preview = panel1;
                Panel hidden = panel2;

                //
                // Create 3d device in gui thread
                //
                Console.WriteLine("Creating DirectX device.");
                Direct3DDevice device = new Direct3DDevice();

                device.Initialise(preview, 1280, 720, hidden);
                GraphicsEngine.Current = device;

               
                System.Threading.ThreadStart s = new System.Threading.ThreadStart(this.GoGoGoCreateFromProject);
                Thread mAuthorThread = new Thread(s);
                mAuthorThread.Start();
            }
            catch (Exception exception)
            {
                PhotoCruzCL.Program.PhotoCruzError("Exception occurred when initialising: "+exception.Message, 4);
            }
        }

        // ****************************************************************************************
        private void GoGoGoCreateFromProject()
        {
            try
            {
              //  CDebugLog.GetInstance().LogLevel = CDebugLog.LoggingLevel.Error;
                //
                // SRG False if re-create set
                //
                bool useMp4Version = true;
                if (reCreate == true)
                {
                    useMp4Version = false;
                }

             //   bool doneDVD = false;

                if (this.doMp4 == true)
                {
                  
                    bool result = CreateMp4FromProject();
                    if (result == false)
                    {
                        return;
                    }
                }

                if (this.doDVD == true)
                {
                    bool result = CreateDVDFromProject(useMp4Version);
                    if (result == false)
                    {
                        return;
                    }
                }

              //  CDebugLog.GetInstance().LogLevel = CDebugLog.LoggingLevel.Info;

                if (this.doBD == true)
                {
                    bool result = CreateBDFromProject(useMp4Version);
                    if (result == false)
                    {
                        return;
                    }

                   // doneBD = true;
                }



                Console.WriteLine("All done...");
            }
            catch (Exception e)
            {
                PhotoCruzCL.Program.PhotoCruzError("Exception occurred when encoding: " + e.Message + " " + e.StackTrace, 5);
            }
            finally
            {
                this.Close();
            }
        }

        private bool CreateMp4FromProject()
        {
            Console.WriteLine("Creating mp4 video");

            CGlobals.OutputAspectRatio outputAspect = CGlobals.OutputAspectRatio.TV16_9;
            float aspect = ((float)width) / ((float)height);
            if (Math.Abs(1.333 - aspect) < 0.1)
            {
                outputAspect = CGlobals.OutputAspectRatio.TV4_3;
            }

            Console.WriteLine("Resolution:" + width + "x" + height);

            if (outputAspect == CGlobals.OutputAspectRatio.TV16_9)
            {
                Console.WriteLine("Output aspect:16x9 widescreen");
            }
            else
            {
                Console.WriteLine("Output aspect:4x3 standard");
            }

            Console.WriteLine("Fps:" + fps);
            if (useMB == true)
            {
                Console.WriteLine("Motion blur: On");
            }
            else
            {
                Console.WriteLine("Motion blur: Off");
            }
            Console.WriteLine("Quality:" + quality.ToString());


            LoadProjectIfNeeded();

            CProject project = CGlobals.mCurrentProject;
            if (project == null)
            {
                return false;
            }

            ArrayList allSlideshows = project.GetAllProjectSlideshows(false);
            if (allSlideshows.Count <= 0)
            {
                PhotoCruzCL.Program.PhotoCruzError("No slideshows in project", 3);
                return false;
            }

            CSlideShow mainslideshow = null;
            for (int i = allSlideshows.Count - 1; i >= 0; i--)
            {
                CSlideShow cand = allSlideshows[i] as CSlideShow;
                if (cand.Name == "MainSlideshow")
                {
                    mainslideshow = cand;
                }
            }

            if (mainslideshow == null)
            {
                PhotoCruzCL.Program.PhotoCruzError("Failed to find MainSlideshow in project", 3);
                return false;
            }

            CProjectAuthor pa = new CProjectAuthor(project, "", mainslideshow);
            CGlobals.mCurrentPA = pa;

            CProject currentProject = CGlobals.mCurrentProject;

            currentProject.DiskPreferences.CustomVideoOutputWidth = width;
            currentProject.DiskPreferences.CustomVideoOutputHeight = height;
            currentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.MP4;
            currentProject.DiskPreferences.OutputVideoFramesPerSecond = fps;
            currentProject.DiskPreferences.TurnOffMotionBlur = !useMB;
            currentProject.DiskPreferences.OutputVideoQuality = quality;

            Console.WriteLine("Encoding video please wait...");

            pa.Author(false, false, false, true);

            Console.WriteLine("");

            string mpeg4_fn = CGlobals.GetTempDirectory() + "\\slideshow0.mp4";
            if (System.IO.File.Exists(mpeg4_fn) == true)
            {
                string dest = mPhotoCruzMainWindow.Root + "\\memorials\\" + mPhotoCruzMainWindow.ProjectName + "\\" + mPhotoCruzMainWindow.ProjectName + ".mp4";

                Console.WriteLine("Moving mp4 video to " + dest);

                ManagedCore.IO.ForceDeleteIfExists(dest, false);
                try
                {
                    System.IO.File.Move(mpeg4_fn, dest);
                }
                catch (Exception e)
                {
                    PhotoCruzCL.Program.PhotoCruzError("Failed to move mp4 video " + mpeg4_fn + " " + e.Message, 6);
                    return false;
                }
            }
            else
            {
                PhotoCruzCL.Program.PhotoCruzError("Failed to move mp4 video " + mpeg4_fn + " file does not exist", 6);
                return false;
            }

            return true;
            
        }

        // ****************************************************************************************************
        private void ReplaceMainSlideshowWithMp4(string sourcefile, CSlideShow oldMainSlideshow)
        {
            oldMainSlideshow.FadeIn = false;
            oldMainSlideshow.FadeOut = false;
            ArrayList slides = new ArrayList();

            CVideoDecoration vd = new CVideoDecoration(sourcefile, new RectangleF(0, 0, 1, 1), 0);

            //
            // SRG  use to speed up testing
            //
            //vd.EndVideoOffset = 21.5 * 60;

            CSlide slide = new CBlankStillPictureSlide(vd);
            slides.Add(slide);

            ArrayList oldSlides = (ArrayList) oldMainSlideshow.mSlides.Clone();

            oldMainSlideshow.RemoveSlides(oldSlides, false);
            oldMainSlideshow.AddSlides(slides);
        }

        // ****************************************************************************************************
        private bool CreateDVDFromProject(bool useMp4Video)
        {
            Console.WriteLine("Creating DVD folder");

            CGlobals.OutputAspectRatio outputAspect = CGlobals.OutputAspectRatio.TV16_9;

            if (outputAspect == CGlobals.OutputAspectRatio.TV16_9)
            {
                Console.WriteLine("Output aspect:16x9 widescreen");
            }
            else
            {
                Console.WriteLine("Output aspect:4x3 standard");
            }

            if (useMB == true)
            {
                Console.WriteLine("Motion blur:On");
            }
            else
            {
                Console.WriteLine("Motion blur:Off");
            }

            LoadProjectIfNeeded();

            CProject project = CGlobals.mCurrentProject;
            if (project == null)
            {
                return false;
            }

            ArrayList allSlideshows = project.GetAllProjectSlideshows(false);
            if (allSlideshows.Count <= 0)
            {
                PhotoCruzCL.Program.PhotoCruzError("No slideshows in project", 3);
                return false;
            }

            CSlideShow mainslideshow = null;
            for (int i = allSlideshows.Count - 1; i >= 0; i--)
            {
                CSlideShow cand = allSlideshows[i] as CSlideShow;
                if (cand.Name == "MainSlideshow")
                {
                    mainslideshow = cand;
                }
            }

            if (mainslideshow == null)
            {
                PhotoCruzCL.Program.PhotoCruzError("Failed to find MainSlideshow in project", 3);
                return false;
            }


            string dest = mPhotoCruzMainWindow.Root + "\\memorials\\" + mPhotoCruzMainWindow.ProjectName +"\\DVDROOT";

            CProjectAuthor pa = new CProjectAuthor(project, dest, mainslideshow);
            CGlobals.mCurrentPA = pa;

            CProject currentProject = CGlobals.mCurrentProject;

            if (useMp4Video == true)
            {
                string mp4file = mPhotoCruzMainWindow.Root + "\\memorials\\" + mPhotoCruzMainWindow.ProjectName + "\\" + mPhotoCruzMainWindow.ProjectName + ".mp4";

                //
                // Replace main slideshow with are own one
                //
                if (System.IO.File.Exists(mp4file) == false)
                {
                    Console.WriteLine("Warning could not find " + mp4file + " Re-creating DVD from original project");
                }
                else
                {
                    Console.WriteLine("Using " + mp4file + " as DVD main slideshow");
                    ReplaceMainSlideshowWithMp4(mp4file, mainslideshow);
                }   
               
            }

            //
            // Set the DVD output settings
            // Aspect should already be set after the project was loaded by original PhotoCruz
            //
            currentProject.DiskPreferences.SetToNTSC();
            currentProject.DiskPreferences.FinalDiskCropPercent = 0;
            currentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.DVD;
            currentProject.DiskPreferences.TurnOffMotionBlur = !useMB;

            //
            // Turn off mb if re-creating from mp4
            //
            if (useMp4Video == true)
            {
                currentProject.DiskPreferences.TurnOffMotionBlur = true;
            }

            Console.WriteLine("Encoding video please wait...");

            pa.Author(false, false, false, false);

            Console.WriteLine("");

            return true;

        }


        // ****************************************************************************************************
        private bool CreateBDFromProject(bool useMp4Video)
        {
            Console.WriteLine("Creating BD folder");

            CGlobals.OutputAspectRatio outputAspect = CGlobals.OutputAspectRatio.TV16_9;

            if (outputAspect == CGlobals.OutputAspectRatio.TV16_9)
            {
                Console.WriteLine("Output aspect:16x9 widescreen");
            }
            else
            {
                PhotoCruzCL.Program.PhotoCruzError("Blu-Ray does not support 4 x 3 outputs", 8);
                return false;
            }

            if (useMB == true)
            {
                Console.WriteLine("Motion blur:On");
            }
            else
            {
                Console.WriteLine("Motion blur:Off");
            }

            LoadProjectIfNeeded();

            CProject project = CGlobals.mCurrentProject;
            if (project == null)
            {
                return false;
            }

            ArrayList allSlideshows = project.GetAllProjectSlideshows(false);
            if (allSlideshows.Count <= 0)
            {
                PhotoCruzCL.Program.PhotoCruzError("No slideshows in project", 3);
                return false;
            }

            CSlideShow mainslideshow = null;
            for (int i = allSlideshows.Count - 1; i >= 0; i--)
            {
                CSlideShow cand = allSlideshows[i] as CSlideShow;
                if (cand.Name == "MainSlideshow")
                {
                    mainslideshow = cand;
                }
            }

            if (mainslideshow == null)
            {
                PhotoCruzCL.Program.PhotoCruzError("Failed to find MainSlideshow in project", 3);
                return false;
            }


            string dest = mPhotoCruzMainWindow.Root + "\\memorials\\" + mPhotoCruzMainWindow.ProjectName;

            CProjectAuthor pa = new CProjectAuthor(project, dest, mainslideshow);
            CGlobals.mCurrentPA = pa;

            CProject currentProject = CGlobals.mCurrentProject;

            //
            // Use mp4 movie instead of from slides?
            //
            if (useMp4Video == true )
            {
                string mp4file = mPhotoCruzMainWindow.Root + "\\memorials\\" + mPhotoCruzMainWindow.ProjectName + "\\" + mPhotoCruzMainWindow.ProjectName + ".mp4";
 
                //
                // Replace main slideshow with our one
                //
                if (System.IO.File.Exists(mp4file) == false)
                {
                    Console.WriteLine("Warning could not find " + mp4file + " Re-creating BD from original project");
                }
                else
                {
                    Console.WriteLine("Using " + mp4file + " as Blu-ray main slideshow");
                    ReplaceMainSlideshowWithMp4(mp4file, mainslideshow);
                }
            }
              

            //
            // Set the Blu-ray output settings
            // Aspect should already be set after the project was loaded by original PhotoCruz
            //
            currentProject.DiskPreferences.FinalDiskCropPercent = 0;
            currentProject.DiskPreferences.CustomVideoOutputWidth = 1920; // 426;  //  1920;
            currentProject.DiskPreferences.CustomVideoOutputHeight = 1080; //240;   // 1080
            currentProject.DiskPreferences.OutputVideoType = CGlobals.VideoType.BLURAY;     // also sets to resolution above
            currentProject.DiskPreferences.OutputVideoFramesPerSecond = 23.976f;
            currentProject.DiskPreferences.TurnOffMotionBlur = !useMB;
            currentProject.DiskPreferences.OutputVideoQuality = quality;

            //
            // Turn off mb if re-creating from mp4
            //
            if (useMp4Video == true)
            {
                currentProject.DiskPreferences.TurnOffMotionBlur = true;
            }

            Console.WriteLine("Encoding video please wait...");

            pa.Author(false, false, false, false);
           
            Console.WriteLine("");

            return true;

        }
    }
}
