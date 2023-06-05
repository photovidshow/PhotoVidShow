using System;
using System.Collections.Generic;
using System.Text;
using DVDSlideshow;
using ManagedCore;

namespace PhotoCruzMain
{
    public class PhotoCruzSettings : UserSettings
    {
        // *********************************************************************************************************************************
        public PhotoCruzSettings()
        {
            _settings = this;

            //
            // Attempt to load the settings xml file if it exists
            //
            LoadFromFile(CGlobals.GetUserDirectory() + "\\UserSettings.xml", FileType.Xml);

            //
            // Add the default values if they are not already set to something
            //
            SetVideoSettingDefaults();
        }

        // *********************************************************************************************************************************
        private void SetVideoSettingDefaults()
        {
            string videoSettingsCategory = "VideoSettings";
            string framepersecond = "FramesPerSecondForUnknownVideos";
            string ffdshowmp4 = "ForceUseFFDShowIfInstalledForMp4";
            string ffdshowmov = "ForceUseFFDShowIfInstalledForMov";
            string ffdshowmts = "ForceUseFFDShowIfInstalledForMts";
            string haalimp4 = "ForceUseHaaliSplitterIfInstalledForMp4";
            string haaliwmov = "ForceUseHaaliSplitterIfInstalledForMov";
            string haalimts = "ForceUseHaaliSplitterIfInstalledForMts";
            string maxVideoPlayers = "MaximumVideoPlayersCached";
            string ReduceHDResolutionVideosWhenEditing = "ReduceHDResolutionVideosWhenEditing";

            bool useFFDShow = false;

            // In windows vista or younger, force ffdshow on mov and mp4 files as default settings
            OperatingSystem OS = Environment.OSVersion;
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

            if (EntryExists(videoSettingsCategory, haalimp4) == false)
            {
                SetBoolValue(videoSettingsCategory, haalimp4, true);
            } 

            if (EntryExists(videoSettingsCategory, haaliwmov) == false)
            {
                SetBoolValue(videoSettingsCategory, haaliwmov, true);
            }

            if (EntryExists(videoSettingsCategory, haalimts) == false)
            {
                SetBoolValue(videoSettingsCategory, haalimts, true);
            }

            if (EntryExists(videoSettingsCategory, maxVideoPlayers) == false)
            {
                SetIntValue(videoSettingsCategory, maxVideoPlayers, 0);
            }

            if (EntryExists(videoSettingsCategory, ReduceHDResolutionVideosWhenEditing) == false)
            {
                SetBoolValue(videoSettingsCategory, ReduceHDResolutionVideosWhenEditing, true);
            }
        }
    }
}
