using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using ManagedCore;
using MangedToUnManagedWrapper;
using OrganicBit.Zip;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Text;
using DVDSlideshow.BluRay.AVCHD;
using DVDSlideshow.BluRay.BDMV;


// this class can author a given project 

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CProjectAuthor.
    /// This class creats a DVD/VCD/SVCD/VIDEO FILE from a given project.
    /// The output is stored on the harddrive in the user temp directory
    /// </summary>
    ///  
    public enum ECurrentAuthorProcess
    {
        SETTING_UP,
        ENCODING_AUDIO,
        ENCODING_VIDEO,
        ADDING_DVD_SUBTITLES,
        BULIDING_DISK_STRUCTURE,
        BUILDING_ZIP_FILE,
        COPYING_OUTPUT_FILES,
        COMPLETE
    } 

    [StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey = MyPublicKey.Value)]


   

    public class CProjectAuthor
    {
        public CProject mForProject;

        // authoring stats

        public CMusicPerformance mCurrentMusicPerformance;
        public string mCurrentOutName;
        public float mPercentageCompete;
        private int mTotalFramesToEncode = 0;
        private string mOutputFolder;
        private bool mDontEncodeMpeg = false;
        private bool mIgnoreMenus = false;
        private CVideo mCurrentEncodeVideo = null;

        private ECurrentAuthorProcess mCurrentProcess = ECurrentAuthorProcess.SETTING_UP;

        // debug stuff

        public bool mDebugEncodeMenus = true;
        public bool mDebugEncodeBestQuality = true;
        public bool mDebugEncodeSlideshow1 = true;
        public bool mDebugEncodeSlideshow2 = true;
        public bool mDebugEncodeSlideshow3 = true;
        public bool mDebugEncodeVideos = true;
        public bool mDebugEncodeSubtitles = true;
        public bool mDebugCreateDiscFileStructure = true;
        private bool mAbort = false;
        private CSlideShow mCurrentSelectedSlideshow = null; // If creating a video only, this will be set to the slideshow to encode

        private ArrayList mCurrentDecorationMenuSubtitleList = new ArrayList();
        private List<M2TSFileGeneratorReport> mt2sReports;

        public bool Aborted
        {
            get { return mAbort; }
        }

        public int TotalFramesToEncode
        {
            get
            {
                return mTotalFramesToEncode;
            }
        }

         //*******************************************************************
        public CProjectAuthor(CProject for_project, string output_folder, CSlideShow selectedSlideshow)
        {
            mOutputFolder = output_folder;
            mCurrentSelectedSlideshow = selectedSlideshow;
            mForProject = for_project;
        }

        //*******************************************************************
        // Call back to tell us to generate the raw slideshowN.pcm file
        public void AudioGenerateCallback()
        {
            CGlobals.DeclareEncodeCheckpoint('4');
            if (mCurrentMusicPerformance != null && CGlobals.mEncodeAudio == true)
            {
                mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.ENCODING_AUDIO;

                if (mCurrentMusicPerformance.IsDolbyAC3 == true)
                {
                    string out_audio_name = mCurrentOutName + ".ac3";
                    if (ManagedCore.IO.ForceDeleteIfExists(out_audio_name, true) == false)
                    {
                        CGlobals.DeclareEncodeCheckpoint('9');
                        return;
                    }

                    CGlobals.DeclareEncodeCheckpoint('5');
                    mCurrentMusicPerformance.RipDolbyAC3(out_audio_name);
                }
                else
                {
                    string out_audio_name = mCurrentOutName + ".pcm";

                    if (ManagedCore.IO.ForceDeleteIfExists(out_audio_name, true) == false)
                    {
                        CGlobals.DeclareEncodeCheckpoint('8');
                        return;
                    }

                    Log.Info("Encoding audio");

                    CGlobals.DeclareEncodeCheckpoint('6');
                    mCurrentMusicPerformance.EncodeIntoSingleStereoStream(out_audio_name, CGlobals.mCurrentProject.DiskPreferences.AudioFrequency);
                    Log.Info("Done encoding audio");
                }
            }
            CGlobals.DeclareEncodeCheckpoint('7');
        }

        //******************************************************************
        public void MultiplexCallback()
        {
            //
            // Currently this will only mean m2ts files
            //
            Log.Info("Muxing '"+mCurrentOutName+"' into m2ts file");
            BluRay.BDMV.M2TSFileGenerator generator = new DVDSlideshow.BluRay.BDMV.M2TSFileGenerator();
            string h264Name = mCurrentOutName + ".264";

            if (System.IO.File.Exists(h264Name) == false)
            {
                Log.Error("Could not find file '" + h264Name + "'");
                return;
            }
           
            string auidoName = mCurrentOutName + ".pcm";

            if (System.IO.File.Exists(auidoName) == false)
            {
                //
                // If not audio in slideshow, then no file will exist
                //
                if (mCurrentMusicPerformance.ContainsAudio==false)
                {
                    auidoName = ""; // This tells the generator to genrate its own blank audio
                }
                else
                {
                    Log.Error("Could not find file '" + auidoName + "'");
                    return;
                }
            }

            List<DateTime> chapterMarkers = null;
            //
            // If encoding slideshow, get chapter markers
            //
            CSlideShow ss = mCurrentEncodeVideo as CSlideShow;
            if (ss != null)
            {
                chapterMarkers = ss.ChapterMarkers;
            }

            string outputName = mCurrentOutName + ".m2ts";
            M2TSFileGeneratorReport report = generator.Generate(h264Name, auidoName, outputName, 0, CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond, chapterMarkers);
            if (report != null)
            {
                mt2sReports.Add(report);
            }

            ManagedCore.IO.ForceDeleteIfExists(h264Name, false);
            ManagedCore.IO.ForceDeleteIfExists(h264Name, false);

        }

        //*******************************************************************
        private bool CopyPreEncodedSlideshow(CSlideShow ss, string out_name)
        {

            ArrayList slides = ss.mSlides;
            if (slides.Count <= 0) return false;

            CVideoSlide vs = slides[0] as CVideoSlide;
            if (vs == null) return false;

            string fn = vs.SourceFilename;
            if (System.IO.File.Exists(fn) == false) return false;

            if (vs.Player.IsVideoInCorrrectOutputDVDFormat() == false)
            {
                return false;
            }

            ManagedCore.IO.ForceDeleteIfExists(out_name, false);

            try
            {
                System.IO.File.Copy(fn, out_name);
            }
            catch
            {
                int i = 0;
                i++;
            }

            return true;
        }


        //*******************************************************************
        private void EncodeVideo(CVideo video, string out_name)
        {
            CGlobals.DeclareEncodeCheckpoint('X');
            mCurrentMusicPerformance = null;

            mCurrentOutName = out_name;

            Encoder mEncoder = new Encoder(video);

            mCurrentEncodeVideo = video;
            MangedToUnManagedWrapper.CDVDEncode.ResetProgressState();

            mCurrentMusicPerformance = CMusicPerformance.FromVideo(mCurrentEncodeVideo);

            string outVideoNameWithExtension = mCurrentOutName;

            //
            // For apps that use command line
            //
            Console.WriteLine("Encoding video "+out_name+"...                                                ");
          
            if (this.mAbort == true)
            {
                CGlobals.DeclareEncodeCheckpoint('Z');
                return;
            }

            int pal = 0;
            if (CGlobals.mCurrentProject.DiskPreferences.TVType == CGlobals.OutputTVStandard.PAL)
            {
                pal = 1;
            }

            int widescreen = 0;
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9)
            {
                widescreen = 1;
            }

            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV221_1)
            {
                widescreen = 2;
            }

            int disk_type = 0;

            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD)
            {
                outVideoNameWithExtension = mCurrentOutName + ".mpg";
            }

            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.SVCD)
            {
                disk_type = 1;
                outVideoNameWithExtension = mCurrentOutName + ".mpg";
            }
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.VCD)
            {
                disk_type = 2;
                outVideoNameWithExtension = mCurrentOutName + ".mpg";
            }
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4)
            {
                disk_type = 3;
                outVideoNameWithExtension = mCurrentOutName + ".mp4";
            }
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY)
            {
                disk_type = 4;
                outVideoNameWithExtension = mCurrentOutName + ".m2ts";
            }

            if (ManagedCore.IO.ForceDeleteIfExists(outVideoNameWithExtension, true) == false)
            {
                CGlobals.DeclareEncodeCheckpoint('Y');
                this.Abort();
                return;
            }

            bool do_3_2_pulldown = false;
            if (CGlobals.mCurrentProject.DiskPreferences.frames_per_second == 23.976f)
            {
                do_3_2_pulldown = true;
            }

            // mplex from ac3 file rather than encode mp2 audio
            bool mplex_ac3 = false;

            if (mCurrentMusicPerformance.IsDolbyAC3 == true)
            {
                mplex_ac3 = true;

                // have to do it before video i'm afraid as callback will never occur
                // because bbmpeg does not need to encode mpeg2 audio
                CGlobals.DeclareEncodeCheckpoint('0');
                AudioGenerateCallback();
            }

            mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.ENCODING_VIDEO;

            CGlobals.DeclareEncodeCheckpoint('1');

            //
            // Generate frames in background ready for encoder (runs is seperate thread).
            //
            mEncoder.StartGenerateFramesInBackground();

            try
            {
                int fpsoption = 0;

                if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4 ||
                    CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY)
                {
                    fpsoption = GetMp4VideoIfpsOptionFromFps(CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond);
                }

                MangedToUnManagedWrapper.CDVDEncode.Encode(mEncoder,
                                                           outVideoNameWithExtension,
                                                           CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth,
                                                           CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight,
                                                           CGlobals.mCurrentProject.DiskPreferences.AudioFrequency,
                                                           pal,
                                                           widescreen,
                                                           disk_type,
                                                           fpsoption, // mp4 only
                                                           (int)CGlobals.mCurrentProject.DiskPreferences.OutputVideoQuality, //mp4 only
                                                           mplex_ac3,
                                                           do_3_2_pulldown,
                                                           mDontEncodeMpeg,
                                                           CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond);
            }
            catch (Exception e)
            {
                ManagedCore.CDebugLog.GetInstance().Error("The encoder threw an exception:" + e.StackTrace + "\r\n\r\n" + e.Message);
            }

            mEncoder.StopGeneratingInBackGroundAndCleanup();

            CGlobals.DeclareEncodeCheckpoint('2');

            GC.Collect();

            //
            // Clean up
            //
            string path = System.IO.Path.GetDirectoryName(mCurrentOutName);

            string out_video_name_temp = mCurrentOutName + ".temp";
            ManagedCore.IO.ForceDeleteIfExists(out_video_name_temp, false);
            string out_video_name_pcm = mCurrentOutName + ".pcm";
            ManagedCore.IO.ForceDeleteIfExists(out_video_name_pcm, false);
            string out_video_name_ac3 = mCurrentOutName + ".ac3";
            ManagedCore.IO.ForceDeleteIfExists(out_video_name_ac3, false);
            string out_video_name_m4a = mCurrentOutName + ".m4a";
            ManagedCore.IO.ForceDeleteIfExists(out_video_name_m4a, false);

            CGlobals.DeclareEncodeCheckpoint('3');
        }


        //*******************************************************************
        private int GetMp4VideoIfpsOptionFromFps(float fps)
        {
            if (fps == 12) return 0;
            if (fps == 15) return 1;
            if (fps == 20) return 2;
            if (fps == 24) return 3;
            if (fps == 25) return 4;
            if (fps == 30) return 5;
            if (fps == 50) return 6;
            if (fps == 60) return 7;
            if (Math.Abs(23.976 - fps) <0.001) return 8;
            if (Math.Abs(59.94 - fps) < 0.001) return 9;

            Log.Warning("Unknown fps " + fps + " for h264 encoder, setting to 30");
            return 5; // default 30fps
        }

        //*******************************************************************
        private ArrayList GetSlideshowsToEncode()
        {
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.MP4)
            {
                ArrayList slideshows = mForProject.GetAllProjectSlideshows(true) ;
                return slideshows;
            }

            ArrayList list = new ArrayList();
            list.Add(mCurrentSelectedSlideshow);

            return list;
        }

        //*******************************************************************

        private void EncodeAllVideos()
        {
            CGlobals.DeclareEncodeCheckpoint('Q');
            // make sure we don't start sound playing
            bool mute_sound = CGlobals.MuteSound;
            CGlobals.MuteSound = true;

            CGlobals.VideoSeeking = false; // JUST in case

            // reset all media streams;
            CGlobals.mCurrentProject.ResetAllMediaStreams();

            // encode slideshowss

            // calc total frame calc based on final possible motion blur fps;
            CGlobals.mCurrentProject.RecalcAllFrameLengths();

            int i = 0;
            ArrayList slideshowsToEncde = GetSlideshowsToEncode();
            foreach (CSlideShow s in slideshowsToEncde)
            {  
                CGlobals.mCurrentProject.DiskPreferences.RenderToEncodeFPSRatio = s.GetMaxNumberRequiredMotionBlurSubFrames();

                if (mAbort == true) break;
                CGlobals.DeclareEncodeCheckpoint('R');

                CGlobals.mCurrentProject.ResetAllMediaStreams();


                if ((i == 0 && mDebugEncodeSlideshow1 == true) ||
                    (i == 1 && mDebugEncodeSlideshow2 == true) ||
                    (i == 2 && mDebugEncodeSlideshow3 == true) ||
                    i > 2)
                {
                    if (mDebugEncodeSlideshow1 == true ||
                        mDebugEncodeSlideshow2 == true ||
                        mDebugEncodeSlideshow3 == true)
                    {
                        string filename = CGlobals.GetTempDirectory() + "\\slideshow" + i;

                        // must be last slideshow in list
                        if (s.Name == CProject.PreMenuIdentityString)
                        {
                            if (i != slideshowsToEncde.Count - 1)
                            {
                                Log.Error("PreMenu slideshow not last slideshow to encode");
                            }

                            filename = CGlobals.GetTempDirectory() + "\\premenu";
                        }

                        if (s.PreEncodedSlideshow == true)
                        {
                            string filename2 = CGlobals.GetTempDirectory() + "\\slideshow" + i + ".mpg";
                            bool res = CopyPreEncodedSlideshow(s, filename2);

                            // did not work just generated slideshow instead
                            if (res == false)
                            {
                                EncodeVideo(s, filename);
                                s.PreEncodedSlideshow = false;
                            }
                        }

                        /* SRG 28/6/2018 THIS DOES NOT WORK, i.e. pulldown makes a 23fps slideshow but creates a 29fps VOB FILE !!!!!
                        else if (CGlobals.mCurrentProject.DiskPreferences.TVType == CGlobals.OutputTVStandard.NTSC &&
                                 (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD ||
                                  CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.SVCD) &&
                                 s.IsNTSCFilmSlideshow())
                        {
                            // convert to ntsc 23.976 with 3:2 pulldown
                            CGlobals.mCurrentProject.DiskPreferences.SetToNTSC32Pulldown();
                            s.CalcLengthOfAllSlides();  // need to re-calc frames
                            EncodeVideo(s, filename);

                        }
                        */
                        else
                        {
                            EncodeVideo(s, filename);
                        }
                    }
                }
                i++;
                CGlobals.DeclareEncodeCheckpoint('S');

            }

            CGlobals.DeclareEncodeCheckpoint('T');
            CGlobals.mCurrentProject.ResetAllMediaStreams();

            // encode menu
            if (mDebugEncodeMenus == true && this.mIgnoreMenus == false)
            {
                ArrayList a = mForProject.MainMenu.GetSelfAndAllSubMenus();

                for (int ii = 0; ii < a.Count; ii++)
                {
                    CMainMenu mm = a[ii] as CMainMenu;
                    if (mm != null)
                    {
                        CGlobals.mCurrentProject.DiskPreferences.RenderToEncodeFPSRatio = mm.GetMaxNumberRequiredMotionBlurSubFrames();

                        CGlobals.DeclareEncodeCheckpoint('U');
                        CGlobals.mCurrentProject.ResetAllMediaStreams();

                        string menu_name = CGlobals.GetTempDirectory() + "\\mainmenu" + ii;
                        EncodeVideo(mm, menu_name);
                        CGlobals.DeclareEncodeCheckpoint('V');
                    }
                }

            }

            CGlobals.DeclareEncodeCheckpoint('W');

            CGlobals.MuteSound = mute_sound;
            CGlobals.mCurrentProject.ResetAllMediaStreams();
        }

        //*******************************************************************
        public string GetCurrentProcessText()
        {

            if (mCurrentProcess == ECurrentAuthorProcess.SETTING_UP)
            {
                return "Setting up...";
            }
            if (mCurrentProcess == ECurrentAuthorProcess.ENCODING_VIDEO)
            {
                int tot = mTotalFramesToEncode;
                int cur = CGlobals.mCurrentProcessFrame;
                if (cur > tot) cur = tot;

                int sub = CGlobals.mCurrentProcessSubFrame;
                int subt = CGlobals.mCurrentProcessSubFrameTotal;
                int sl = CGlobals.mCurrentProcessSlide + 1;
#if (DEBUG)
                return "Enc " + sl + " f:" + cur + "/" + tot + " s:" + sub + "/" + subt;
#else
                return "Encoding video frame " + cur + "/" + tot;
#endif

            }

            if (mCurrentProcess == ECurrentAuthorProcess.ENCODING_AUDIO)
            {
                // are we creating audfio buffer

                if (CMusicPerformaceChannel.mCreatingAudioBuffer == true)
                {
                    return "Creating audio buffer...";
                }

                if (CMusicPerformaceChannel.mReSamplingAudio == true)
                {
                    return "Resampling audio...";
                }

                // are we ripping
                if (MangedToUnManagedWrapper.CManagedAudioRipper.mDoingRip == true)
                {
                    if (MangedToUnManagedWrapper.CManagedAudioRipper.mBuildingCodecArray == true)
                    {
                        return "Building codec array...";
                    }

                    int percent = 0;

                    return "Ripping audio '" +
                        System.IO.Path.GetFileName(MangedToUnManagedWrapper.CManagedAudioRipper.mRippingFile) + "'";

                }

                // we are encoding audio
                int encodeAudioPercentage = GetEncodeAudioPercentage();

                string encodeAudioMessage = "Encoding audio";
                if (encodeAudioPercentage != 0)
                {
                    encodeAudioMessage += " " + encodeAudioPercentage.ToString() + "%";
                }
                else
                {
                    encodeAudioMessage += "...";
                }


                return encodeAudioMessage;
            }

            if (mCurrentProcess == ECurrentAuthorProcess.ADDING_DVD_SUBTITLES)
            {
                return "Adding DVD subtitles";
            }

            if (mCurrentProcess == ECurrentAuthorProcess.BULIDING_DISK_STRUCTURE)
            {
                return "Building final disk structure";
            }

            if (mCurrentProcess == ECurrentAuthorProcess.BUILDING_ZIP_FILE)
            {
                return "Building zip file";
            }

            if (mCurrentProcess == ECurrentAuthorProcess.COPYING_OUTPUT_FILES)
            {
                return "Copying output files";
            }

            return "Complete.";
        }

        //*******************************************************************
        private int GetEncodeAudioPercentage()
        {
            // only support aac encoding at mo, if not aac then 0 is always returned
            return MangedToUnManagedWrapper.CDVDEncode.GetAACCurentEncodeFrame();
        }


        //*******************************************************************
        public void UpdateStats()
        {
            mPercentageCompete = 0;

            // calculate percentage of author complete

            bool found_current = false;

            int total = 0;
            int done = 0;

            ArrayList slideshowsToEncde = GetSlideshowsToEncode();

            foreach (CSlideShow s in slideshowsToEncde)
            {
                int frames = s.GetFinalOutputLengthInFrames();

                if (s.PreEncodedSlideshow == true)
                {
                    continue;
                }

                total += frames;
                if (mCurrentEncodeVideo != null && mCurrentEncodeVideo == s)
                {
                    done += (int)(((float)frames) * (MangedToUnManagedWrapper.CDVDEncode.GetPercentageDone() / 100.0f));
                    found_current = true;
                }
                else if (found_current == false)
                {
                    done += frames;
                }
            }

            if (this.mIgnoreMenus == false)
            {
                ArrayList sub_menus = mForProject.MainMenu.GetSelfAndAllSubMenus();

                for (int id = 0; id < sub_menus.Count; id++)
                {
                    int menu_length = mForProject.MainMenu.GetFinalOutputLengthInFrames();
                    total += menu_length;

                    if (mCurrentEncodeVideo != null && sub_menus[id] as CMainMenu == mCurrentEncodeVideo)
                    {
                        done += (int)(((float)menu_length) * (MangedToUnManagedWrapper.CDVDEncode.GetPercentageDone() / 100.0f));
                        found_current = true;
                    }
                    else if (found_current == false)
                    {
                        done += menu_length;
                    }
                }
            }

            if (total != 0)
            {
                mPercentageCompete = ((float)done) / ((float)total) * 100.0f;
            }

            if (mPercentageCompete < 0) mPercentageCompete = 0.0f;
            if (mPercentageCompete > 100.0f) mPercentageCompete = 100.0f;

            mTotalFramesToEncode = total;

            if (this.mCurrentProcess >= ECurrentAuthorProcess.BULIDING_DISK_STRUCTURE)
            {
                mPercentageCompete = 100;
            }

            if (mCurrentEncodeVideo == null)
            {
                mPercentageCompete = 0;
            }

            // calcpercentage of dvd construction
        }


        //*******************************************************************
        private void AddSubtitlesToMainMenus()
        {
            // if not a dvd ignore this part
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType != CGlobals.VideoType.DVD)
            {
                return;
            }

            mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.ADDING_DVD_SUBTITLES;

            CGlobals.DeclareEncodeCheckpoint('a');

            ArrayList a = this.mForProject.MainMenu.GetSelfAndAllSubMenus();

            for (int id = 0; id < a.Count; id++)
            {
                if (mAbort == true) break;
                CMainMenu for_menu = a[id] as CMainMenu;

                System.Xml.XmlDocument my_doc = new XmlDocument();

                XmlElement sub_pic = my_doc.CreateElement("subpictures");
                my_doc.AppendChild(sub_pic);

                XmlElement stream = my_doc.CreateElement("stream");
                sub_pic.AppendChild(stream);

                XmlElement spu = my_doc.CreateElement("spu");

                spu.SetAttribute("force", "yes");
                spu.SetAttribute("start", @"00:00:00.00");
                spu.SetAttribute("autooutline", "infer");
                spu.SetAttribute("outlinewidth", "6");
                spu.SetAttribute("autoorder", "rows");

                DisposableObject<Bitmap> bb2 = new DisposableObject<Bitmap>();

                using (Bitmap bb = new Bitmap(CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth, CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                using (Bitmap bb1 = new Bitmap(CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth, CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                using (bb2)
                {
                    List<CMenuButton> buttons = new List<CMenuButton>();
                    foreach (CDecoration d in for_menu.BackgroundSlide.Decorations)
                    {
                        CMenuButton mb = d as CMenuButton;
                        if (mb != null)
                        {
                            buttons.Add(mb);
                        }
                    }

                    CMenuSubPictureRenderer renderer = new CMenuSubPictureRenderer();
                    CMenuDecorationSubtitles currentMenuDecorationSubtitle = renderer.Render(buttons, bb1, for_menu);

                    mCurrentDecorationMenuSubtitleList.Add(currentMenuDecorationSubtitle);

                    Color highlightColor = for_menu.SubPictureStyle.SubPictureColor;
                    Color selectColor = Color.FromArgb(highlightColor.R / 2, highlightColor.G / 2, highlightColor.B / 2);

                    //bb1.Save("test1before.bmp");

                    // This step only verfies things are 1 color, else we can get assets in dvd author.
                    BitmapUtils.MakeTransparentOneColor(bb1, for_menu.SubPictureStyle.SubPictureColor, 255);

                    bb2.Assign(bb1.Clone() as Bitmap);
                    BitmapUtils.MakeTransparentOneColor(bb2.Object, selectColor, 255);

                    //bb.Save("test1.bmp");
                    //bb1.Save("test1after.bmp");
                    //bb2.Object.Save("test2after.bmp");

                    stream.AppendChild(spu);

                    String xml_buffer = my_doc.InnerXml;

                    string fie = CGlobals.GetTempDirectory() + "\\mainmenusub" + id + ".mpg";
                    if (ManagedCore.IO.ForceDeleteIfExists(fie, true) == false)
                    {
                        Abort();
                        CGlobals.DeclareEncodeCheckpoint('c');
                        return;
                    }

                    int the_mode = 0;
                    if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.SVCD)
                    {
                        the_mode = 2;
                    }
                    if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.VCD)
                    {
                        the_mode = 1;
                    }

                    SubtitleButtonPositions mysubpos = new SubtitleButtonPositions();

                    string s1 = CGlobals.GetTempDirectory() + "\\mainmenu" + id + ".mpg";
                    string s2 = CGlobals.GetTempDirectory() + "\\mainmenusub" + id + ".mpg";
               
                    DVDAuthor.AddSubtitleToVideo(xml_buffer,
                        s1,
                        s2,
                        bb, bb1, bb2, the_mode, CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth,
                        CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight,
                        mysubpos);
                 


                    bool subtitle_button_error = false;

                    ArrayList sort_list = new ArrayList();

                    for (int jj = 0; jj < mysubpos.x0.Count; jj++)
                    {
                        bool found = false;

                        int x0 = 0;
                        int y0 = 0;

                        x0 = (int)mysubpos.x0[jj];
                        y0 = (int)mysubpos.y0[jj];

                        foreach (CDecorationSubtitle ds in currentMenuDecorationSubtitle.mDecorationSubtitles)
                        {
                            Point pp = new Point(x0, y0);
                            Rectangle r2 = ds.mSubtitleRegion;
                            if (r2.Contains(pp) == true &&
                                sort_list.Contains(ds) == false)
                            {
                                sort_list.Add(ds);
                                found = true;
                                Log.Info("Found decoration for subtitle Button position " + jj + "= " + x0 + "," + y0);
                            }
                        }
                        if (found == false)
                        {
                            subtitle_button_error = true;
                            Log.Error("Could not fund decoration for subtitle button position " + jj + "= " + x0 + "," + y0);
                        }
                    }

                    if (subtitle_button_error == false)
                    {
                        currentMenuDecorationSubtitle.mDecorationSubtitles = sort_list;
                    }
                    else
                    {
                        string message = "An error occurred when trying to order the subtitle slideshow buttons. The final menu may have incorrectly ordered buttons.";
                        Log.Error(message);
                        UserMessage.Show(message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

                    }
                }
            }
            CGlobals.DeclareEncodeCheckpoint('b');
        }


        //*******************************************************************
        private void CreateFileStructure()
        {      
            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD)
            {
                mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.BULIDING_DISK_STRUCTURE;
                CreateISO9660FileStructureForDVD();
            }
            else if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.SVCD ||
                     CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.VCD)
            {
                mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.BULIDING_DISK_STRUCTURE;
                CreateISO9660FileStructureForVCD();
            }
            else if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY)
            {
                mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.BULIDING_DISK_STRUCTURE;
                CreateBluRayFileStructure();
            }
        }

        //*******************************************************************
        private bool CreateBluRayFileStructure()
        {
            BluRayFolderCreator BDFolderCreator = new BluRayFolderCreator();

            ArrayList slideshows = mForProject.GetAllProjectSlideshows(true);
            ArrayList menus = mForProject.MainMenu.GetSelfAndAllSubMenus();

            int expectedM2tsVideos = slideshows.Count;

            if (mIgnoreMenus == false)
            {
                expectedM2tsVideos += menus.Count;
            }

            if (mt2sReports.Count != expectedM2tsVideos)
            {
                Log.Error("Can not create a Blu-ray disk structure, received " + mt2sReports.Count + " m2ts video, expecting " + expectedM2tsVideos);
                return false;
            }    
            
            List<M2TSVideo> videoList = new List<M2TSVideo>();
            foreach (M2TSFileGeneratorReport report in mt2sReports)
            {
                //
                // Convert BDMV I-Frame makers into AVCHD I-Frame markers
                // 
                List<DVDSlideshow.BluRay.AVCHD.IFrameMarker> markers = new List<DVDSlideshow.BluRay.AVCHD.IFrameMarker>(report.IVideoFrameMarkers.Count);
                foreach (DVDSlideshow.BluRay.BDMV.IFrameMarker BDMVMarker in report.IVideoFrameMarkers)
                {
                    markers.Add( new DVDSlideshow.BluRay.AVCHD.IFrameMarker(BDMVMarker.TsPacketNunber, BDMVMarker.PresentationTimeStamp, BDMVMarker.FrameSizeInBytes));
                }

                float lengthInSeconds = ((float)report.NumberOfVideoFrames) / CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond;     

                M2TSVideo.VideoResolution outputResoultion = M2TSVideo.VideoResolution.Resoultion1080p;
                if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight == 720)
                {
                    outputResoultion = M2TSVideo.VideoResolution.Resoultion720p;
                }

                M2TSVideo.VideoFPS outputFPS = M2TSVideo.VideoFPS.Fps24;

                if (Math.Abs(CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond - 23.976) < 0.001)
                {
                    outputFPS = M2TSVideo.VideoFPS.Fps23Point976;
                }
                else if (Math.Abs(CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond - 50) < 0.001)
                {
                    outputFPS = M2TSVideo.VideoFPS.Fps50;
                }
                else if (Math.Abs(CGlobals.mCurrentProject.DiskPreferences.OutputVideoFramesPerSecond - 59.94) < 0.001)
                {
                    outputFPS = M2TSVideo.VideoFPS.Fps59Point94;
                }

                M2TSVideo video = new M2TSVideo(report.M2TSFilename, markers, report.NumberOfTsPackets, report.MaxRecordRate, lengthInSeconds, outputResoultion, outputFPS, M2TSVideo.VidepAspect.Aspect16x9, report.ChapterMarkers);
                videoList.Add(video);
            }

            string outputFolder = CGlobals.GetTempDirectory() + "\\blurayfolder";

            if (this.mOutputFolder != "")
            {
                outputFolder = mOutputFolder + "\\BDMVROOT";
            }

            if (Directory.Exists(outputFolder) == true)
            {
                try
                {
                    Directory.Delete(outputFolder + "\\BDMV", true);
                    Directory.Delete(outputFolder + "\\CERTIFICATE", true);
                }
                catch (Exception e)
                {
                }
            }

            try
            {
                Directory.CreateDirectory(outputFolder);
            }
            catch
            {
            }


            //
            // For apps that use command line
            //
            Console.WriteLine("Creating the Bluray BDMV folder image...                                        ");

            //
            // Create folder structure
            //
            BDFolderCreator.Create(outputFolder, CGlobals.GetRootDirectory(), videoList);

            //
            // Ok create menu.txt file 
            //
            BluRayMenuDataFileCreator menuTextFileCreator = new BluRayMenuDataFileCreator();
            if (menuTextFileCreator.GenerateMenuFileAndGraghics(outputFolder + "\\BDMV\\JAR\\00000",
                                                                mForProject,
                                                                mIgnoreMenus,
                                                                CGlobals.mCurrentProject.DiskPreferences.OutputVideoWidth,
                                                                CGlobals.mCurrentProject.DiskPreferences.OutputVideoHeight) == false)
            {
                Log.Error("Failed to create menu.txt file for blu-ray disc");
                return false;
            }

            return true;
        }


        //*******************************************************************
        private void CreateISO9660FileStructureForVCD()
        {
            CGlobals.DeclareEncodeCheckpoint('d');
            System.Xml.XmlDocument my_doc2 = new XmlDocument();

            XmlElement vcd_author_ele = my_doc2.CreateElement("videocd");
            my_doc2.AppendChild(vcd_author_ele);

            vcd_author_ele.SetAttribute("xmlns", "Slideshow");

            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.SVCD)
            {
                vcd_author_ele.SetAttribute("class", "svcd");
                vcd_author_ele.SetAttribute("version", "1.0");
            }
            else
            {
                vcd_author_ele.SetAttribute("class", "vcd");
                vcd_author_ele.SetAttribute("version", "2.0");
            }

            // info
            XmlElement info_ele = my_doc2.CreateElement("info");
            vcd_author_ele.AppendChild(info_ele);

            XmlElement albumid_ele = my_doc2.CreateElement("album-id");
            info_ele.AppendChild(albumid_ele);

            XmlElement volume_count_ele = my_doc2.CreateElement("volume-count");
            volume_count_ele.InnerText = "1";
            info_ele.AppendChild(volume_count_ele);

            XmlElement volume_no_ele = my_doc2.CreateElement("volume-number");
            volume_no_ele.InnerText = "1";
            info_ele.AppendChild(volume_no_ele);

            XmlElement restriction_ele = my_doc2.CreateElement("restriction");
            restriction_ele.InnerText = "0";
            info_ele.AppendChild(restriction_ele);


            // pvd
            XmlElement pvd_ele = my_doc2.CreateElement("pvd");
            vcd_author_ele.AppendChild(pvd_ele);


            XmlElement volume_id_ele = my_doc2.CreateElement("volume-id");
            volume_id_ele.InnerText = "VIDEOCD";
            pvd_ele.AppendChild(volume_id_ele);

            XmlElement system_id_ele = my_doc2.CreateElement("system-id");
            system_id_ele.InnerText = "CD-RTOS CD-BRIDGE";
            pvd_ele.AppendChild(system_id_ele);

            pvd_ele.AppendChild(my_doc2.CreateElement("application-id"));
            pvd_ele.AppendChild(my_doc2.CreateElement("preparer-id"));
            pvd_ele.AppendChild(my_doc2.CreateElement("publisher-id"));

            //segment items

            /*
            XmlElement segments_ele = my_doc2.CreateElement("segment-items");
            vcd_author_ele.AppendChild(segments_ele);
			
			
            ArrayList menu_list  =  mForProject.MainMenu.GetSelfAndAllSubMenus();

            int menus = menu_list.Count;
            string temp_dir =CGlobals.GetTempDirectory();

            for (int ii=0;ii<menus;ii++)
            {
				
                XmlElement segment_ele = my_doc2.CreateElement("segment-item");
                string s1 = temp_dir+@"\mainmenu"+ii+".mpg";
                string s2= "seg-mainmenu"+ii;
                segment_ele.SetAttribute("src",s1);
                segment_ele.SetAttribute("id",s2);
                segments_ele.AppendChild(segment_ele);
            }
            */


            XmlElement sequences_ele = my_doc2.CreateElement("sequence-items");
            vcd_author_ele.AppendChild(sequences_ele);

            // add menus to sequence if we need them

            ArrayList menu_list = mForProject.MainMenu.GetSelfAndAllSubMenus();
            int menus = menu_list.Count;
            string temp_dir = CGlobals.GetTempDirectory();

            if (mIgnoreMenus == false)
            {
                for (int ii = 0; ii < menus; ii++)
                {

                    XmlElement sequence_ele = my_doc2.CreateElement("sequence-item");
                    string s1 = temp_dir + @"\mainmenu" + ii + ".mpg";
                    string s2 = "seq-mainmenu" + ii;
                    sequence_ele.SetAttribute("src", s1);
                    sequence_ele.SetAttribute("id", s2);
                    sequences_ele.AppendChild(sequence_ele);
                }
            }


            // slideshow sequence


            int slideshows = mForProject.GetAllProjectSlideshows(false).Count;

            for (int ii = 0; ii < slideshows; ii++)
            {
                XmlElement sequence_ele = my_doc2.CreateElement("sequence-item");
                string s1 = temp_dir + @"\slideshow" + ii + ".mpg";
                string s2 = "sequence" + ii;
                sequence_ele.SetAttribute("src", s1);
                sequence_ele.SetAttribute("id", s2);
                sequences_ele.AppendChild(sequence_ele);
            }


            //do pbc if we have menus
            if (mIgnoreMenus == false)
            {
                XmlElement pbc_ele = my_doc2.CreateElement("pbc");
                vcd_author_ele.AppendChild(pbc_ele);


                int bsn = 1;

                int current_slideshow_link = 0;

                for (int ii = 0; ii < menus; ii++)
                {
                    XmlElement selection_ele = my_doc2.CreateElement("selection");
                    selection_ele.SetAttribute("id", "select-mainmenu" + ii);
                    pbc_ele.AppendChild(selection_ele);

                    XmlElement bsn_ele = my_doc2.CreateElement("bsn");
                    bsn_ele.InnerText = bsn.ToString();
                    selection_ele.AppendChild(bsn_ele);

                    XmlElement timeout_ele = my_doc2.CreateElement("timeout");
                    timeout_ele.SetAttribute("ref", "select-mainmenu" + ii);
                    selection_ele.AppendChild(timeout_ele);

                    XmlElement wait_ele = my_doc2.CreateElement("wait");
                    wait_ele.InnerText = "1";
                    selection_ele.AppendChild(wait_ele);

                    XmlElement loop_ele = my_doc2.CreateElement("loop");
                    loop_ele.SetAttribute("jump-timing", "immediate");
                    loop_ele.InnerText = "1";
                    selection_ele.AppendChild(loop_ele);

                    XmlElement play_item_ele = my_doc2.CreateElement("play-item");
                    play_item_ele.SetAttribute("ref", "seq-mainmenu" + ii);
                    selection_ele.AppendChild(play_item_ele);

                    // selection buttons

                    CMainMenu m = (CMainMenu)menu_list[ii];
                    int num_sub_slideshows = m.GetSlideshowsSelectableFromMenu().Count;

                    for (int ssf = 0; ssf < num_sub_slideshows; ssf++)
                    {
                        XmlElement select_ele = my_doc2.CreateElement("select");
                        select_ele.SetAttribute("ref", "select-chapter" + current_slideshow_link);
                        current_slideshow_link++;
                        bsn++;
                        selection_ele.AppendChild(select_ele);
                    }

                    if (m.ParentMenu != null)
                    {
                        XmlElement select_ele = my_doc2.CreateElement("select");
                        int parent_menu = ii - 1;
                        select_ele.SetAttribute("ref", "select-mainmenu" + parent_menu);
                        bsn++;
                        selection_ele.AppendChild(select_ele);
                    }

                    if (m.SubMenus.Count != 0)
                    {
                        XmlElement select_ele = my_doc2.CreateElement("select");
                        int child_menu = ii + 1;
                        select_ele.SetAttribute("ref", "select-mainmenu" + child_menu);
                        bsn++;
                        selection_ele.AppendChild(select_ele);
                    }
                }

                for (int ii = 0; ii < slideshows; ii++)
                {
                    XmlElement selection_ele = my_doc2.CreateElement("selection");
                    selection_ele.SetAttribute("id", "select-chapter" + ii);
                    pbc_ele.AppendChild(selection_ele);


                    XmlElement next_ele = my_doc2.CreateElement("next");
                    next_ele.SetAttribute("ref", "select-mainmenu0");
                    selection_ele.AppendChild(next_ele);

                    XmlElement ret_ele = my_doc2.CreateElement("return");
                    ret_ele.SetAttribute("ref", "select-mainmenu0");
                    selection_ele.AppendChild(ret_ele);

                    XmlElement timeout_ele = my_doc2.CreateElement("timeout");
                    timeout_ele.SetAttribute("ref", "select-mainmenu0");
                    selection_ele.AppendChild(timeout_ele);

                    XmlElement wait_ele = my_doc2.CreateElement("wait");
                    wait_ele.InnerText = "1";
                    selection_ele.AppendChild(wait_ele);

                    XmlElement loop_ele = my_doc2.CreateElement("loop");
                    loop_ele.SetAttribute("jump-timing", "immediate");
                    loop_ele.InnerText = "1";
                    selection_ele.AppendChild(loop_ele);

                    XmlElement play_item_ele = my_doc2.CreateElement("play-item");
                    play_item_ele.SetAttribute("ref", "sequence" + ii);
                    selection_ele.AppendChild(play_item_ele);
                }
            }

            // create pbc which plays slideshow 1 after another then loops to start
            else
            {
                XmlElement pbc_ele = my_doc2.CreateElement("pbc");
                vcd_author_ele.AppendChild(pbc_ele);

                XmlElement selection_ele = my_doc2.CreateElement("playlist");
                selection_ele.SetAttribute("id", "playlist1");
                pbc_ele.AppendChild(selection_ele);

                XmlElement next_ele = my_doc2.CreateElement("next");
                next_ele.SetAttribute("ref", "playlist1");
                selection_ele.AppendChild(next_ele);

                XmlElement ret_ele = my_doc2.CreateElement("return");
                ret_ele.SetAttribute("ref", "playlist1");
                selection_ele.AppendChild(ret_ele);

                XmlElement wait_ele = my_doc2.CreateElement("wait");
                wait_ele.InnerText = "1";
                selection_ele.AppendChild(wait_ele);

                XmlElement loop_ele = my_doc2.CreateElement("loop");
                loop_ele.SetAttribute("jump-timing", "immediate");
                loop_ele.InnerText = "1";
                selection_ele.AppendChild(loop_ele);


                for (int ii = 0; ii < slideshows; ii++)
                {
                    XmlElement play_item_ele = my_doc2.CreateElement("play-item");
                    play_item_ele.SetAttribute("ref", "sequence" + ii);
                    selection_ele.AppendChild(play_item_ele);
                }

            }


            string output_folder;

            if (this.mOutputFolder != "")
            {
                output_folder = mOutputFolder;
            }
            else
            {
                output_folder = CGlobals.GetTempDirectory() + "\\vcdfolder";
            }


            if (Directory.Exists(output_folder) == false)
            {
                try
                {
                    Directory.CreateDirectory(output_folder);
                }
                catch (Exception e5)
                {
                }
            }
            else
            {
                ManagedCore.IO.ForceDeleteIfExists(mOutputFolder + "\\vcd.cue", true);
                ManagedCore.IO.ForceDeleteIfExists(mOutputFolder + "\\vcd.bin", true);
            }


            if (CGlobals.mIsDebug == true)
            {
                try
                {
                    my_doc2.Save(output_folder + "\\vcdauthor.xml");
                }
                catch (Exception e6)
                {
                }
            }


            String xml_buffer2 = my_doc2.InnerXml;




            MangedToUnManagedWrapper.VCDAuthor.AuthorVCD(xml_buffer2, output_folder);
            CGlobals.DeclareEncodeCheckpoint('e');

        }


        //*******************************************************************
        private void AddPreMenuToDVDStructure(System.Xml.XmlDocument my_doc2, XmlElement vmgm, string filename, string chapterMarkersString)
        {
            XmlElement pre_menus_elem = my_doc2.CreateElement("menus");
            vmgm.AppendChild(pre_menus_elem);

            XmlElement pre_pgc_elem = my_doc2.CreateElement("pgc");
            pre_menus_elem.AppendChild(pre_pgc_elem);

            XmlElement pre_vob_elem = my_doc2.CreateElement("vob");
            pre_pgc_elem.AppendChild(pre_vob_elem);

            pre_vob_elem.SetAttribute("file", filename);

            //
            // Set any pre menu chapter markers
            //
            if (chapterMarkersString != "")
            {
                pre_vob_elem.SetAttribute("chapters", chapterMarkersString);
            }

            XmlElement pre_post_elem = my_doc2.CreateElement("post");
            pre_pgc_elem.AppendChild(pre_post_elem);

            pre_post_elem.InnerText = "jump titleset 1 menu;";

           

        }

        //*******************************************************************
        private void CreateISO9660FileStructureForDVD()
        {
            CGlobals.DeclareEncodeCheckpoint('f');
            System.Xml.XmlDocument my_doc2 = new XmlDocument();

            XmlElement dvd_author_ele = my_doc2.CreateElement("dvdauthor");
            my_doc2.AppendChild(dvd_author_ele);

            XmlElement vmgm = my_doc2.CreateElement("vmgm");
            dvd_author_ele.AppendChild(vmgm);

            //
            // Add old photcruz premenu?
            //
            if (mForProject.PreMenuMovieFileName != "" &&
                System.IO.File.Exists(this.mForProject.PreMenuMovieFileName) == true)
            {
                AddPreMenuToDVDStructure(my_doc2, vmgm, this.mForProject.PreMenuMovieFileName, "");
            }

            // 
            // Add generated premenu slideshow?
            //
            else if (mForProject.PreMenuSlideshow != null)
            {
                Log.Info("Adding premenu to dvd");
                string markersString = GenerateDVDChapterMarkersString(mForProject.PreMenuSlideshow);
                AddPreMenuToDVDStructure(my_doc2, vmgm, CGlobals.GetTempDirectory() + "\\premenu.mpg", markersString);
            }

            int current_title_set = 1;
            int menu_id = 0;

            ArrayList menu_list = null;
            if (mForProject.MainMenu != null)
            {
                menu_list = mForProject.MainMenu.GetSelfAndAllSubMenus();
            }

            XmlElement titleset_elem = my_doc2.CreateElement("titleset");
            dvd_author_ele.AppendChild(titleset_elem);

            ArrayList slide_shows_title_order = new ArrayList();
            ArrayList non_return_to_menu_slideshow_index = new ArrayList();

            if (mIgnoreMenus == false)
            {
                XmlElement menus_elem = my_doc2.CreateElement("menus");
                titleset_elem.AppendChild(menus_elem);

                XmlElement video_elem = my_doc2.CreateElement("video");
                menus_elem.AppendChild(video_elem);

                if (CGlobals.mCurrentProject.DiskPreferences.TVType == CGlobals.OutputTVStandard.NTSC)
                {
                    video_elem.SetAttribute("format", "ntsc");
                    video_elem.SetAttribute("resolution", "720x480");
                }
                else
                {
                    video_elem.SetAttribute("format", "pal");
                    video_elem.SetAttribute("resolution", "720x576");
                }

                if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9 ||
                    CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV221_1)
                {
                    video_elem.SetAttribute("aspect", "16:9");
                }
                if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
                {
                    video_elem.SetAttribute("aspect", "4:3");
                }

                int title_num = 1;
                int current_menu = 1;

                foreach (CMainMenu menu in menu_list)
                {
                    CMenuDecorationSubtitles mds = (CMenuDecorationSubtitles)mCurrentDecorationMenuSubtitleList[current_menu - 1];

                    XmlElement pgc_elem = my_doc2.CreateElement("pgc");
                    menus_elem.AppendChild(pgc_elem);

                    if (current_menu == 1)
                    {
                        pgc_elem.SetAttribute("entry", "root");
                    }

                    // if more than 1 menu make sure we jump to the last menu seen
                    // as after each slideshow it will jump back to menu 1
                    // (just the way dvd author works)
                    if (menu_list.Count > 1 && current_menu == 1)
                    {
                        XmlElement pre_elem = my_doc2.CreateElement("pre");
                        pgc_elem.AppendChild(pre_elem);

                        string pre_command = "{ ";
                        for (int hyi = 2; hyi <= menu_list.Count; hyi++)
                        {
                            pre_command += " if (g1 eq " + hyi + ") {jump menu " + hyi + ";} ";
                        }

                        pre_command += "}";
                        pre_elem.InnerText = pre_command;
                    }

                    // add vob
                    XmlElement vob_elem = my_doc2.CreateElement("vob");
                    pgc_elem.AppendChild(vob_elem);

                    // the main menu mpeg2
                    vob_elem.SetAttribute("file", CGlobals.GetTempDirectory() + "\\mainmenusub" + menu_id + ".mpg");

                    // do menu slideshows first

                    foreach (CDecorationSubtitle ds in mds.mDecorationSubtitles)
                    {
                        CDecoration dec = ds.mForDecoration;

                        if (dec is CMenuSlideshowButton)
                        {

                            CMenuSlideshowButton ssbut = dec as CMenuSlideshowButton;
                            XmlElement button_elem = my_doc2.CreateElement("button");
                            pgc_elem.AppendChild(button_elem);

                            button_elem.InnerText = "g2 = 1; jump title " + title_num + ";";
                      
                            title_num++;
                            int slideshow_index = CGlobals.mCurrentProject.GetSlideshowIndex(ssbut.GetInnerImageStringId());
                            slide_shows_title_order.Add(slideshow_index);

                            // used in photocruz
                            if (ssbut.ReturnToMenuAfterPlay == false)
                            {
                                non_return_to_menu_slideshow_index.Add(slideshow_index);
                            }
                        }

                        if (dec is CMenuLinkPreviousNextButton ||
                            dec is CMenuLinkSubMenuButton)
                        {
                            // work out menu id it links to
                            ArrayList menu_list2 = mForProject.MainMenu.GetSelfAndAllSubMenus();

                            for (int menu_id2 = 0; menu_id2 < menu_list2.Count; menu_id2++)
                            {
                                CMainMenu mm = menu_list2[menu_id2] as CMainMenu;
                                if (mm.ID == ds.CMenuLinkID)
                                {
                                    int parent_vml = 1;
                                    if (mm.ParentMenu != null)
                                    {
                                        parent_vml = mm.ParentMenu.VmlMenuAuthorNumber;
                                    }

                                    XmlElement button_elem = my_doc2.CreateElement("button");
                                    pgc_elem.AppendChild(button_elem);
                                    int link_menu = menu_id2 + 1;

                                    // if multiple menus set the g1 varibale to selected menu
                                    if (menu_list.Count > 1)
                                    {
                                        button_elem.InnerText = "g1 = " + link_menu + "; jump menu " + link_menu + ";";
                                    }
                                    else
                                    {
                                        button_elem.InnerText = "jump menu " + link_menu + ";";
                                    }

                                    break;
                                }
                            }
                        }

                        if (dec is CMenuPlayAllButton)
                        {
                            XmlElement button_elem = my_doc2.CreateElement("button");
                            pgc_elem.AppendChild(button_elem);
                            button_elem.InnerText = "g2 = 2; jump title " + "1" + ";";
                        }

                        if (dec is CMenuPlayAllLoopedButton)
                        {
                            XmlElement button_elem = my_doc2.CreateElement("button");
                            pgc_elem.AppendChild(button_elem);
                            button_elem.InnerText = "g2 = 3; jump title " + "1" + ";";
                        }

                    }

                    XmlElement post_menu_ele = my_doc2.CreateElement("post");
                    pgc_elem.AppendChild(post_menu_ele);
                    post_menu_ele.InnerText = "jump cell 1;";

                    current_menu++;
                    menu_id++;
                }
            }
            // ignore menu
            else if (menu_list != null)
            {
                // if ignore menus build up slideshow list in correct order
                foreach (CMainMenu menu in menu_list)
                {
                    for (int d = 0; d < menu.BackgroundSlide.Decorations.Count; d++)
                    {
                        CDecoration dec = (CDecoration)menu.BackgroundSlide.Decorations[d];
                        if (dec is CMenuSlideshowButton)
                        {
                            CMenuSlideshowButton ssbut = dec as CMenuSlideshowButton;
                            int slideshow_index = CGlobals.mCurrentProject.GetSlideshowIndex(ssbut.GetInnerImageStringId());
                            slide_shows_title_order.Add(slideshow_index);
                        }
                    }
                }
            }
            // No menu defined, just select slideshows direct (used in things like Memory Guard DVD burn)
            else
            {
                foreach (CSlideShow slideshow in CGlobals.mCurrentProject.GetAllProjectSlideshows(false))
                {
                    slide_shows_title_order.Add(slideshow.ID);
                }
            }

            XmlElement titles_elem = my_doc2.CreateElement("titles");
            titleset_elem.AppendChild(titles_elem);

            XmlElement video_elem2 = my_doc2.CreateElement("video");
            titles_elem.AppendChild(video_elem2);

            if (CGlobals.mCurrentProject.DiskPreferences.TVType == CGlobals.OutputTVStandard.NTSC)
            {
                video_elem2.SetAttribute("format", "ntsc");
                video_elem2.SetAttribute("resolution", "720x480");

            }
            else
            {
                video_elem2.SetAttribute("format", "pal");
                video_elem2.SetAttribute("resolution", "720x576");
            }

            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV16_9 ||
                CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV221_1)
            {
                video_elem2.SetAttribute("aspect", "16:9");
            }
            if (CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio == CGlobals.OutputAspectRatio.TV4_3)
            {
                video_elem2.SetAttribute("aspect", "4:3");
            }


            // CREATE TITLES

            // create the titles in the order the menus are expting

            int title_count = 1;

            foreach (int idd in slide_shows_title_order)
            {
                XmlElement pgc_ele_2 = my_doc2.CreateElement("pgc");
                titles_elem.AppendChild(pgc_ele_2);

                XmlElement vob_elem2 = my_doc2.CreateElement("vob");
                vob_elem2.SetAttribute("file", CGlobals.GetTempDirectory() + "\\slideshow" + idd + ".mpg");

                // 
                // Add Chapter markers if set
                //
                CSlideShow slideshow = CGlobals.mCurrentProject.GetSlideshowFromIndex(idd);
                if (slideshow != null)
                {
                    string markersString = GenerateDVDChapterMarkersString(slideshow);
                    if (markersString != "")
                    {
                        vob_elem2.SetAttribute("chapters", markersString);
                    }
                }

                pgc_ele_2.AppendChild(vob_elem2);

                XmlElement post_menu_ele2 = my_doc2.CreateElement("post");
                pgc_ele_2.AppendChild(post_menu_ele2);

                // for photocruz
                if (non_return_to_menu_slideshow_index.Contains(idd) == true)
                {
                    // ASSUME THIS IS MAIN SLIDEHOW AND IN CHAPEL MENU
                    // just jump to current title
                    post_menu_ele2.InnerText = "jump title " + title_count + ";";

                    // REMOVE REPEAT STUFF BY REMOVING FROM LIST
                    non_return_to_menu_slideshow_index.Clear();
                }
                else if (mIgnoreMenus == false)
                {
                    // get next title
                    int next_title = title_count + 1;

                    //  Last title? jump to menu or title one, base on global register g2
                    if (title_count >= slide_shows_title_order.Count)
                    {
                        post_menu_ele2.InnerText = "{ if (g2 lt 3) {call menu;} jump title 1;}";
                    }
                    else
                    {
                        // if g2 == 1 go back to menu else must be play/loop all option, so go to next title
                        post_menu_ele2.InnerText = "{ if (g2 eq 1) {call menu;} jump title " + next_title + ";}";
                    }
                }
                else
                {
                    // get next title
                    int next_title = title_count + 1;

                    // if our last title loop to first
                    if (title_count >= slide_shows_title_order.Count)
                    {
                        next_title = 1;
                    }

                    post_menu_ele2.InnerText = "jump title " + next_title + ";";
                }

                title_count++;
            }


            String xml_buffer2 = my_doc2.InnerXml;

            string output_folder;

            if (this.mOutputFolder != "")
            {
                output_folder = mOutputFolder;
            }
            else
            {
                output_folder = CGlobals.GetTempDirectory() + "\\dvdfolder";
            }

            if (Directory.Exists(output_folder) == true)
            {
                try
                {
                    Directory.Delete(output_folder + "\\VIDEO_TS", true);
                    Directory.Delete(output_folder + "\\AUDIO_TS", true);
                }
                catch (Exception e)
                {
                }
            }

            try
            {
                Directory.CreateDirectory(output_folder);
            }
            catch
            {
            }


            if (CGlobals.mIsDebug == true)
            {
                try
                {
                    my_doc2.Save(output_folder + "\\dvdauthor.xml");
                }
                catch (Exception e2)
                {
                }
            }

            //
            // For apps that use command line
            //
            Console.WriteLine("Creating the DVD VIDEO_TS folder image...");

            RunDVDAuthorInSeperateExe(xml_buffer2, output_folder);
            CGlobals.DeclareEncodeCheckpoint('g');

        }

        //*******************************************************************
        private string GenerateDVDChapterMarkersString(CSlideShow slideshow)
        {
            // Always have a chapter marker at time 0 
            string result = "00:00:00";
            List<DateTime> chapterMarkers = slideshow.ChapterMarkers;
            if (chapterMarkers.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (DateTime dateTime in chapterMarkers)
                {
                    stringBuilder.Append(",");
                    string time = System.String.Format("{0:d2}:{1:d2}:{2:d2}", dateTime.Hour, dateTime.Minute, dateTime.Second);
                    stringBuilder.Append(time);
                }

                result = stringBuilder.ToString();
            }
            return result;
        }

        //*******************************************************************
        void RunDVDAuthorInSeperateExe(string xml_buffer2, string output_folder)
        {

            MangedToUnManagedWrapper.DVDAuthor.AuthorDVD(xml_buffer2, output_folder);

            CGlobals.DeclareEncodeCheckpoint('i');

        }


        //*******************************************************************
        // this will author the prject 
        // i.e. encode the project and then write out to dvd/vcd/svcd etc.

        // If creating a video file, this method returns the final output full filename
        public string Author(bool include_orginal_pictures,
                             bool include_original_vidoes,
                             bool dont_mpeg_encode,
                             bool ignore_menus)
        {

            try
            {

                //
                // Turn off all project change events from firing off (i.e. used by the gui to update controls)
                //
                CGlobals.mCurrentProject.IgnoreProjectChanges = true;

                string videoFileCreated = "";

                CGlobals.DeclareEncodeCheckpoint('O');

                //
                // Used when creating blurays disks
                //
                mt2sReports = new List<M2TSFileGeneratorReport>();

                this.mAbort = false;

                // clear decoration subtitle link
                mCurrentDecorationMenuSubtitleList = new ArrayList();

                mIgnoreMenus = ignore_menus;

                mDontEncodeMpeg = dont_mpeg_encode;

                CGlobals.mCurrentProcessFrame = 0;

                this.UpdateStats();

                mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.SETTING_UP;

                CGlobals.PlayVideoRealTime = false;

                CGlobals.mVideoGenerateMode = VideoGenerateMode.FINAL_OUTPUT_MODE;

                // clean video cache, as video cache size is smaller when encoding	
                CVideoPlayerCache.GetInstance().CleanCache();

                // rebuild all slides canvas area as we are now in higher res 
                CGlobals.mCurrentProject.RebuildToNewCanvas(CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio);

                if (mDebugEncodeVideos == true && mAbort == false)
                {
                    EncodeAllVideos();
                }


                if (mIgnoreMenus == false && mDebugEncodeSubtitles == true && mAbort == false)
                {
                    AddSubtitlesToMainMenus();
                }

                if (mDebugCreateDiscFileStructure == true && mAbort == false)
                {
                    //
                    // Builds VIDEO_TS, Cue/Bin or BDMV folder for appropriate optical media output file structure
                    //
                    CreateFileStructure();
                }

                if ((include_orginal_pictures == true ||
                    include_original_vidoes == true) &&
                    mAbort == false)
                {
                    BuildOriginalFilesZip(include_orginal_pictures, include_original_vidoes);
                }

                // copy slideshow over
                if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.MP4 && mAbort == false && mOutputFolder != "")
                {
                    mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.COPYING_OUTPUT_FILES;

                    int attempts = 0;
                    bool moved = false;
                    while (attempts < 10 && moved == false)
                    {
                        string file = mOutputFolder;
                        try
                        {
                            videoFileCreated = file;
                            System.IO.File.Move(CGlobals.GetTempDirectory() + "\\slideshow0.mp4", file);
                            moved = true;
                        }
                        catch (Exception exception)
                        {
                            attempts++;
                            if (attempts == 10)
                            {
                                ManagedCore.UserMessage.Show("Failed to move created slideshow from temp directory : " + exception.Message, "Failed to move slideshow", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(500);
                            }
                        }
                    }
                }

                mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.COMPLETE;

                CVideoPlayerCache.GetInstance().CleanCache();

                // back to preview lower res canvas area
                CGlobals.mVideoGenerateMode = VideoGenerateMode.EDIT_PRIVIEW_MODE;
                CGlobals.mCurrentProject.RecalcAllFrameLengths();
                CGlobals.PlayVideoRealTime = true;
                CGlobals.mCurrentProject.RebuildToNewCanvas(CGlobals.mCurrentProject.DiskPreferences.OutputAspectRatio);

                CGlobals.DeclareEncodeCheckpoint('P');

                return videoFileCreated;
            }
            catch (Exception error)
            {
                Log.Error(error.Message + "\r\n\r\n" + error.StackTrace);
                throw;
            }
            finally
            {
                CGlobals.mCurrentProject.IgnoreProjectChanges = false;
            }
        }


        //*******************************************************************
        private void BuildOriginalFilesZip(bool include_orginal_pictures, bool include_original_vidoes)
        {
            mCurrentProcess = DVDSlideshow.ECurrentAuthorProcess.BUILDING_ZIP_FILE;

            CGlobals.DeclareEncodeCheckpoint('j');

            string org_files_name = "";

            if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.DVD)
            {
                org_files_name = CGlobals.GetTempDirectory() + "\\dvdfolder\\VIDEO_TS\\" + CGlobals.mOriginalFilesZipFilename;

                if (this.mOutputFolder != "")
                {
                    org_files_name = mOutputFolder + "\\VIDEO_TS\\" + CGlobals.mOriginalFilesZipFilename;
                }
            }
            else if (CGlobals.mCurrentProject.DiskPreferences.OutputVideoType == CGlobals.VideoType.BLURAY)
            {
                org_files_name = CGlobals.GetTempDirectory() + "\\blurayfolder\\" + CGlobals.mOriginalFilesZipFilename;

                if (this.mOutputFolder != "")
                {
                    org_files_name = mOutputFolder + "\\BDMVROOT\\" +CGlobals.mOriginalFilesZipFilename;
                }
            }
            else
            {
                Log.Error("Unknown end burn type when creating zip file");
                return;
            }

            // delete old zip files
            int delete_file_no = 1;
            string to_delete = org_files_name + delete_file_no.ToString() + ".zip";
            while (System.IO.File.Exists(to_delete))
            {
                if (ManagedCore.IO.ForceDeleteIfExists(to_delete, true) == false)
                {
                    CGlobals.DeclareEncodeCheckpoint('k');
                    return;
                }
                delete_file_no++;
                to_delete = org_files_name + delete_file_no.ToString() + ".zip";
            }

            // create new zip files
            int zip_file = 1;

            OrganicBit.Zip.ZipWriter writer = new OrganicBit.Zip.ZipWriter(org_files_name + zip_file.ToString() + ".zip");

            ArrayList filenames = mForProject.GetSlidesSourceFilesNames();

            byte[] buffer = new byte[4096];
            int byteCount;

            long zip_file_size = 0;
            foreach (string s in filenames)
            {
                bool add = false;

                if (CGlobals.IsImageFilename(s) && include_orginal_pictures == true)
                {
                    add = true;
                }
                else if (CGlobals.IsVideoFilename(s) && include_original_vidoes == true)
                {
                    add = true;
                }

                if (add == true)
                {

                    // zip file too big? (i.e. over 300 meg), create the next one.  This is because
                    // dvd does not like big files, say 1 gig
                    if (zip_file_size > 1024 * 1024 * 300)
                    {
                        writer.Close();
                        zip_file++;
                        writer = new OrganicBit.Zip.ZipWriter(org_files_name + zip_file.ToString() + ".zip");
                        zip_file_size = 0;
                    }

                    try
                    {

                        string filename_f = System.IO.Path.GetFileName(s);

                        ZipEntry entry = new ZipEntry(filename_f);
                        entry.ModifiedTime = System.IO.File.GetLastWriteTime(s);
                        entry.Comment = "";
                        writer.AddEntry(entry);

                        FileStream reader = System.IO.File.OpenRead(s);
                        while ((byteCount = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, byteCount);
                        }
                        reader.Close();

                        System.IO.FileInfo fileinfo = new FileInfo(s);
                        zip_file_size += fileinfo.Length;
                    }
                    catch (Exception exc)
                    {
                    }
                }
            }

            writer.Close();
            CGlobals.DeclareEncodeCheckpoint('l');
        }



        //*******************************************************************
        public void Abort()
        {
            CGlobals.DeclareEncodeCheckpoint('!');
            this.mAbort = true;
            MangedToUnManagedWrapper.CDVDEncode.Abort();
            MangedToUnManagedWrapper.CManagedAudioRipper.Abort();
            CMusicPerformance.Abort();
            MangedToUnManagedWrapper.CManagedMemoryBufferCache.Abort();
            MangedToUnManagedWrapper.CManagedAudioMixer.Abort();
            CGlobals.DeclareEncodeCheckpoint('$');
            M2TSFileGenerator.Abort = true;
            BluRayFolderCreator.Abort = true;
        }
    }
}
