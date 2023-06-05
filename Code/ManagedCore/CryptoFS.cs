using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ManagedCore;

namespace ManagedCore
{
    public class CryptoFS
    {
        static Dictionary<string, string> mNonCryptoToPhsicalMap = new Dictionary<string, string>();

        // *************************************************************************************************************
        public static string GetNonCryptoFilename(string name, bool downloadIfMissing)
        {
            string lowerName = name.ToLower();

            if (lowerName.EndsWith("pvsv") == false &&
                lowerName.EndsWith("pvsi") == false)
            {
                return name;
            }

            if (mNonCryptoToPhsicalMap.ContainsKey(lowerName) == false)
            {
                string physicalName = CreateNonCryptoVersionOfFile(lowerName, downloadIfMissing);

                if (physicalName == "")
                {
                    return "";
                }
                else
                {
                    mNonCryptoToPhsicalMap.Add(lowerName, physicalName);
                }

                return physicalName;
            }

            
            string fn = mNonCryptoToPhsicalMap[lowerName];

            if (System.IO.File.Exists(fn)==false)
            {
                Log.Warning("Missing DFile, re-creating '" + lowerName+"'");
                string result = CreateNonCryptoVersionOfFile(lowerName, downloadIfMissing);

                if (result == "")
                {
                    Log.Error("Failed to create DFile '" + lowerName + "'");
                    return "";
                }

                if (result != fn)
                {
                    Log.Warning("PFile,DFile map wrong '" + result + "' '" + fn +"'");

                    mNonCryptoToPhsicalMap.Remove(lowerName);

                    mNonCryptoToPhsicalMap.Add(lowerName, result);
                    fn = result;
                }
            }

            return fn;

        }

        // *************************************************************************************************************
        private static string CreateNonCryptoVersionOfFile(string cryptofn, bool downloadIfMissing)
        {
            string result = "";

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(cryptofn);

                XmlNodeList list = doc.GetElementsByTagName("Link");

                if (list.Count > 0)
                {
                    XmlElement e = list[0] as XmlElement;
                    if (e != null)
                    {
                        string s = e.GetAttribute("PFilename");
                        if (s != "")
                        {
                            string path = System.IO.Path.GetDirectoryName(cryptofn);

                            string pfilename = path + "\\" + s;
                            if (System.IO.File.Exists(pfilename) == false)
                            {                               
                                // See if it already exists in download media folder
                                string fileFromRoot = IO.StripRootHeader(pfilename);
                                pfilename = IO.GetDownloadMediaDirectory() + "\\" + fileFromRoot;

                                if (System.IO.File.Exists(pfilename) == false && downloadIfMissing)
                                {
                                    // ok try downloaded it

                                    CNetFile nf = new CNetFile("media\\" + fileFromRoot);
                                    bool downloaded = nf.CopyToHarddrive(pfilename, true);

                                    if (downloaded == false)
                                    {
                                        Log.Warning("Failed to download file '" + pfilename + "'");
                                        return result;
                                    }
                                }
                            }

                            if (System.IO.File.Exists(pfilename))
                            {
                                // Does it have encrpyed filename
                                s = e.GetAttribute("DFilename");
                                if (s != "")
                                {
                                    string dir = System.IO.Path.GetTempPath() + "pvs\\";
                                    if (System.IO.Directory.Exists(dir) == false)
                                    {
                                        System.IO.Directory.CreateDirectory(dir);
                                    }

                                    result = dir + s;

                                    IO.df(pfilename, result);
                                }
                                else
                                {
                                    // non ecrypted (i.e. pfilename is the one to use
                                    result = pfilename;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return result;
        }

        // ******************************************************************************************************
        public static void CreateVideoPFile(string filename)
        {
            CreatePFile(filename, ".pvsv");
        }


        // ******************************************************************************************************
        public static void CreateImagePFile(string filename)
        {
            CreatePFile(filename, ".pvsi");
        }

        // ******************************************************************************************************
        private static void CreatePFile(string filename, string pExtension)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(filename);
            string dname =  System.IO.Path.GetFileName(filename);
            string pname = name + ".pvso";
            string path = System.IO.Path.GetDirectoryName(filename);
            string xmlfile = path + "\\" + name + pExtension;

            XmlDocument d = new XmlDocument();
            XmlElement e = d.CreateElement("Link");

            if (System.IO.File.Exists(pname) == true)
            {
                ManagedCore.IO.ForceDeleteIfExists(pname, false);
            }

            if (System.IO.File.Exists(xmlfile) == true)
            {
                ManagedCore.IO.ForceDeleteIfExists(xmlfile, false);
            }

            IO.ef(filename,path +"\\"+ pname);

            e.SetAttribute("PFilename", pname);
            e.SetAttribute("DFilename", dname);

            d.AppendChild(e);
            d.Save(xmlfile);
        }


        // ********************************************************************************************************
        public static void ClearDFiles()
        {
            string dir = System.IO.Path.GetTempPath() + "pvs";
            if (System.IO.Directory.Exists(dir) == true)
            {
                IO.DeleteAllFileInDirectory(dir);
            }
        }


    }
}
