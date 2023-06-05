using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class BluRayFolderCreator
    {
        private string bdmvFolder;
        private string auxDataFolder;
        private string backupFolder;
        private string backupBDJOFolder;
        private string backupClipInfFolder;
        private string backupJarFolder;
        private string backupJarDataFolder;
        private string backupPlaylistFolder;
        private string BDJOFolder;
        private string ClipInfFolder;
        private string JarFolder;
        private string JarDataFolder;
        private string MetaFolder;
        private string PlaylistFolder;
        private string StreamFolder;
        private string certificateFolder;
        private string certificateBackUpFolder;
        private string rootFolder;

        private static bool abort = false;

        public static bool Abort
        {
            set { abort = value ;}
        }

        public void Create(string folderLocation, string applicationRootFolder, List<M2TSVideo> mt2sVideo)
        {
            abort = false;
            rootFolder = applicationRootFolder; 
            GenerateFolderStrings(folderLocation);
            CreateFolderHiearchy(folderLocation);
            MoveVideosToStreamsFolder(folderLocation, mt2sVideo);
            CreatFiles(folderLocation, mt2sVideo);
        }

        private void GenerateFolderStrings(string folderLocation)
        {
             bdmvFolder = folderLocation + "\\BDMV";
             auxDataFolder = folderLocation + "\\BDMV\\AUXDATA";
             backupFolder = folderLocation + "\\BDMV\\BACKUP";
             backupBDJOFolder = folderLocation + "\\BDMV\\BACKUP\\BDJO";
             backupClipInfFolder = folderLocation + "\\BDMV\\BACKUP\\CLIPINF";
             backupJarFolder = folderLocation + "\\BDMV\\BACKUP\\JAR";
             backupJarDataFolder = folderLocation + "\\BDMV\\BACKUP\\JAR\\00000";
             backupPlaylistFolder = folderLocation + "\\BDMV\\BACKUP\\PLAYLIST";
             BDJOFolder = folderLocation + "\\BDMV\\BDJO";
             ClipInfFolder = folderLocation + "\\BDMV\\CLIPINF";
             JarFolder = folderLocation + "\\BDMV\\JAR";
             JarDataFolder = folderLocation + "\\BDMV\\JAR\\00000";
             MetaFolder = folderLocation + "\\BDMV\\META";
             PlaylistFolder = folderLocation + "\\BDMV\\PLAYLIST";
             StreamFolder = folderLocation + "\\BDMV\\STREAM";
             certificateFolder = folderLocation + "\\CERTIFICATE";
             certificateBackUpFolder = folderLocation + "\\CERTIFICATE\\BACKUP";
        }

        private void CreateFolderHiearchy(string folderLocation)
        {
            //
            // Create folder hiearchy
            //
            CreateFolder(bdmvFolder);
            CreateFolder(auxDataFolder);
            CreateFolder(backupFolder);
            CreateFolder(backupBDJOFolder);
            CreateFolder(backupClipInfFolder);
            CreateFolder(backupJarFolder);
            CreateFolder(backupJarDataFolder);
            CreateFolder(backupPlaylistFolder);
            CreateFolder(BDJOFolder);
            CreateFolder(ClipInfFolder);
            CreateFolder(JarFolder);
            CreateFolder(JarDataFolder);
            CreateFolder(MetaFolder);
            CreateFolder(PlaylistFolder);
            CreateFolder(StreamFolder);
            CreateFolder(certificateFolder);
            CreateFolder(certificateBackUpFolder);
        }

        private void CreatFiles(string folderLocation, List<M2TSVideo> mt2sVideo)
        {
            IndexBDMVCreator indexCreator = new IndexBDMVCreator();
            indexCreator.CreateIndexBDMV(bdmvFolder);
            indexCreator.CreateIndexBDMV(backupFolder);

            MovieObjectBDMVCreator movieObjectBDMVCreator = new MovieObjectBDMVCreator();
            movieObjectBDMVCreator.CreateMovieObjectBDMV(bdmvFolder);
            movieObjectBDMVCreator.CreateMovieObjectBDMV(backupFolder);

            PlayListMPLSCreator playListCreator = new PlayListMPLSCreator();
            ClipInfoCLPICreator clipCreator = new ClipInfoCLPICreator();

            BDJOBDMVCreator bdjoCreator = new BDJOBDMVCreator();
            bdjoCreator.CreateBDJO(BDJOFolder);
            bdjoCreator.CreateBDJO(backupBDJOFolder);

            CopyJarAndCertificateFiles();
           
            int videoNumber = 0;
            foreach (M2TSVideo video in mt2sVideo)
            {
                if (abort == true)
                {
                    return;
                }
                playListCreator.Create(PlaylistFolder, backupPlaylistFolder, video, videoNumber);
                clipCreator.Create(ClipInfFolder, backupClipInfFolder, video, videoNumber);
                videoNumber++;
            }
        }
      
        private void MoveVideosToStreamsFolder(string folderLocation, List<M2TSVideo> mt2sVideo)
        {
            //
            // Copy videos to stream folder
            //
            int videoNum = 0;

            foreach (M2TSVideo video in mt2sVideo)
            {
                string filename = video.Filename;
                if (System.IO.File.Exists(filename))
                {
                    string file = Path.GetFileName(filename);
                    string dest = StreamFolder + "\\" + videoNum.ToString("D5") + ".m2ts";
                    AVCHD.File.Move(filename, dest);
                }
                else
                {
                    AVCHDLog.Error("Could not find file '" + video.Filename + "'");
                }
                videoNum++;
            }
        }

        private void CopyJarAndCertificateFiles()
        {
            AVCHD.File.Copy(rootFolder+"\\blurayfiles\\00000.jar", backupJarFolder + "\\00000.jar");
            AVCHD.File.Copy(rootFolder+"\\blurayfiles\\00000.jar", JarFolder + "\\00000.jar");
            AVCHD.File.Copy(rootFolder+"\\blurayfiles\\app.discroot.crt", certificateBackUpFolder + "\\app.discroot.crt");
            AVCHD.File.Copy(rootFolder+"\\blurayfiles\\app.discroot.crt", certificateFolder + "\\app.discroot.crt");
        }

        private void CreateFolder(string folder)
        {
            if (Directory.Exists(folder) == true)
            {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);
        }  
    }
}
