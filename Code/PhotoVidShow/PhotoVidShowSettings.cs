using System;
using System.Collections.Generic;
using System.Text;
using ManagedCore;
using DVDSlideshow;

namespace PhotoVidShow
{
    public class PhotoVidShowSettings : UserSettings
    {
        // *********************************************************************************************************************************
        public PhotoVidShowSettings()
        {
            _settings = this;

            //
            // Attemp to load the settings xml file if it exists
            //
            LoadFromFile(CGlobals.GetUserDirectory() + "\\UserSettings.xml", FileType.Xml);

            //
            // Add the default values if they are not already set to something
            //
            SetDefaultFolders();
            SetVideoSettingDefaults();
            SetOutputTypeDefaults();
        }

        private string MyPictures = "MyPictures"; 
        private string MyMusic = "MyMusic"; 
        private string MyProjects = "MyProjects";
        private string AuthoredFolder = "AuthoredFolder";
        private string DefaultFoldersSettings = "DefaultFoldersSettings";

        private string GetDefaultProjectsDirectory()
        {
            return CGlobals.GetUserDirectory() + @"\Projects";
        }

        private string GetDefaultAuthoredRootDirectory()
        {
            return CGlobals.GetUserDirectory() + @"\Authored";
        }

        private string GetDefaultPicturesAndVideoDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures).ToString();
        }

        private string GetDefaultMusicAndAudioDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).ToString();
        }

        // *********************************************************************************************************************************
        protected override void ResetCategoryInternal(string name)
        {
            //
            // Can only reset default folders currently
            //
            if (name == DefaultFoldersSettings)
            {
                DefaultFolders.SetFolder(MyPictures, GetDefaultPicturesAndVideoDirectory(), false);
                SetStringValue(name, MyPictures, GetDefaultPicturesAndVideoDirectory());
                DefaultFolders.SetFolder(MyMusic, GetDefaultMusicAndAudioDirectory(), false);
                SetStringValue(name, MyMusic, GetDefaultMusicAndAudioDirectory());
                DefaultFolders.SetFolder(MyProjects, GetDefaultProjectsDirectory(), false);  // dont bother checking for write access
                SetStringValue(name, MyProjects, GetDefaultProjectsDirectory());
                DefaultFolders.SetFolder(AuthoredFolder, GetDefaultAuthoredRootDirectory(), false);  // dont bother checking for write access
                SetStringValue(name, AuthoredFolder, GetDefaultAuthoredRootDirectory());
            }

            base.ResetCategoryInternal(name);
        }

        // *********************************************************************************************************************************
        private void SetDefaultFolders()
        {
            AddSettingsFolderToDefaultFolder(DefaultFoldersSettings, MyPictures, GetDefaultPicturesAndVideoDirectory(), false);
            AddSettingsFolderToDefaultFolder(DefaultFoldersSettings, MyMusic, GetDefaultMusicAndAudioDirectory(), false);
            AddSettingsFolderToDefaultFolder(DefaultFoldersSettings, MyProjects, GetDefaultProjectsDirectory(), true);
            AddSettingsFolderToDefaultFolder(DefaultFoldersSettings, AuthoredFolder, GetDefaultAuthoredRootDirectory(), true);
        }


        // *********************************************************************************************************************************
        private void AddSettingsFolderToDefaultFolder(string category, string settings, string defaultPath, bool requireWriteAccess)
        { 
            if (EntryExists(category, settings) == true)
            {
                string folder = GetStringValue(category, settings);
                if (folder != defaultPath)
                {
                    if (DefaultFolders.SetFolder(settings, folder, requireWriteAccess) == true)
                    {
                        return;
                    }
                }
            }
        
            DefaultFolders.SetFolder(settings, defaultPath, false); // default path always has write access
        }

        // *********************************************************************************************************************************
        private void SetVideoSettingDefaults()
        {
            string videoSettingsCategory = "VideoSettings";
            string framepersecond = "FramesPerSecondForUnknownVideos";
            string ffdshowmp4 = "ForceUseFFDShowIfInstalledForMp4";
            string ffdshowmov = "ForceUseFFDShowIfInstalledForMov";
            string ffdshowmts = "ForceUseFFDShowIfInstalledForMts";
            string maxVideoPlayers = "MaximumVideoPlayersCached";
            string ReduceHDResolutionVideosWhenEditing = "ReduceHDResolutionVideosWhenEditing";

            bool useFFDShow = false;

            // If windows vista or younger, force ffdshow on mov and mp4 files as default settings
            if (!CGlobals.IsWin7OrHigher())
            {
                useFFDShow = true;
            }

            if (EntryExists(videoSettingsCategory, framepersecond) == false)
            {
                SetFloatValue(videoSettingsCategory, framepersecond, 23.976f);
            }

            if (EntryExists(videoSettingsCategory, ffdshowmp4) == false)
            {
                SetBoolValue(videoSettingsCategory, ffdshowmp4, useFFDShow);
            }

            if (EntryExists(videoSettingsCategory, ffdshowmov) == false)
            {
                SetBoolValue(videoSettingsCategory, ffdshowmov, useFFDShow);
            }

            if (EntryExists(videoSettingsCategory, ffdshowmts) == false)
            {
                SetBoolValue(videoSettingsCategory, ffdshowmts, useFFDShow);
            }

            int maxDefaultPlayers = 4;
            //
            // If pc running with 2 gig or less, then reduce default players caches at once
            // If Windows WMI is broken then this can throw an exception, if so ignore
            //
            try
            {
                ulong memory = ManagedCore.CSystem.GetTotalComputerMemory() / 1024 / 1024;
                if (memory <= 2048)
                {
                    maxDefaultPlayers = 2;
                }
            }
            catch
            {
            }


            if (EntryExists(videoSettingsCategory, maxVideoPlayers) == false)
            {
                SetIntValue(videoSettingsCategory, maxVideoPlayers, maxDefaultPlayers);
            }

            if (EntryExists(videoSettingsCategory, ReduceHDResolutionVideosWhenEditing) == false)
            {
                SetBoolValue(videoSettingsCategory, ReduceHDResolutionVideosWhenEditing, true);
            }
        }

        // *********************************************************************************************************************************
        private void SetOutputTypeDefaults()
        {
            string outputVideoCategory = "OutputType";
            string showOptionsAtStartup = "ShowOptionsAtStartup";
            string defaultOutputType = "DefaultOutputType";

            if (EntryExists(outputVideoCategory, showOptionsAtStartup) == false)
            {
                SetBoolValue(outputVideoCategory, showOptionsAtStartup, true);
            }

            if (EntryExists(outputVideoCategory, defaultOutputType) == false)
            {
                SetIntValue(outputVideoCategory, defaultOutputType, (int)CGlobals.VideoType.MP4);
            }
        }
    }
}
