using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Drawing;

namespace DVDSlideshow
{
    /// <summary>
    /// This class can generate a 'menu.txt' data file that is used to drive a Blu-Ray menu.
    /// This resulting 'menu.txt' file will be copied onto the blu-ray disc along with the JAR code
    /// that will read it when the disc is played in a blu-ray player.
    /// This class will also generate any .png graphics that are needed by the menu.
    /// </summary>
    public class BluRayMenuDataFileCreator
    {
        // ************************************************************************************************
        public BluRayMenuDataFileCreator()
        {
        }

        // ************************************************************************************************
        public bool GenerateMenuFileAndGraghics(string folder, CProject forProject, bool ignoreMenus, int width, int height)
        {
            string file = folder + "\\menu.txt";
            bool result = ManagedCore.IO.ForceDeleteIfExists(file, false);
            if (result == false)
            {
                return false;
            }

            StreamWriter writter = new StreamWriter(file);
            try
            {
                writter.WriteLine("PhotoVidShowBDMenu,1");
                writter.WriteLine("Resolution," + width + "," + height);
                writter.WriteLine("Style,DVD"); // only type supported at mo

                CSlideShow preslideshow = forProject.PreMenuSlideshow;
    
                int slideshowCount = forProject.GetAllProjectSlideshows(true).Count;
                int normalSlideshowCount = forProject.GetAllProjectSlideshows(false).Count;

                writter.WriteLine("Slideshows," + normalSlideshowCount);

                //
                // Are we just looping all slideshows one after another
                //
                if (ignoreMenus == true)
                {
                    //
                    // Has to have two parameters, second not used though
                    //
                    writter.WriteLine("PlayAllLooped,1");    
                    return true;    // will also close the file
                }

                Rectangle rd = forProject.DiskPreferences.GetFinalDiskCropRectangle(width, height);
                CMenuSubPictureRenderer renderer = new CMenuSubPictureRenderer();
               
                if (preslideshow == null)
                {
                    writter.WriteLine("PreMenu,-1");

                }
                else
                {
                    int preMenuIndex = slideshowCount - 1;// pre menu will always been last slideshow
                    writter.WriteLine("PreMenu," + preMenuIndex);  
                }
                int firstMenuIndex = slideshowCount;

                ArrayList menus = forProject.MainMenu.GetSelfAndAllSubMenus();

                writter.WriteLine("NumMenus," + menus.Count);
                
                int buttonNum = 0;

                for (int i = 0; i < menus.Count; i++)
                {
                    CMainMenu menu = (CMainMenu)menus[i];
                    int menuNum = i + 1;
                    int menuPlayListNumber = firstMenuIndex + i;
                    writter.WriteLine("Menu," + menuNum+","+menuPlayListNumber);

                  
                    foreach (CDecoration d in menu.BackgroundSlide.GetAllAndSubDecorations())
                    {
                        CMenuSlideshowButton slideshowButton = d as CMenuSlideshowButton;
                        if (slideshowButton != null)
                        {
                            buttonNum++;
                            int slideShowPlaylistNum = forProject.GetSlideshowIndex(slideshowButton.GetInnerImageStringId());
                            GenerateMenuTextLineAndButtonGraphics(folder, slideshowButton, renderer, menu, menuNum, writter, forProject, slideShowPlaylistNum, 1, buttonNum);
                        }

                        CMenuLinkButton linkButton = d as CMenuLinkButton;
                        if (linkButton != null)
                        {
                            CMainMenu link_menu = CGlobals.mCurrentProject.GetMenu(linkButton.MenuLinkID);
                            if (link_menu != null)
                            {
                                buttonNum++;
                                int menuNumberLink = menus.IndexOf(link_menu) + 1;
                                GenerateMenuTextLineAndButtonGraphics(folder, linkButton, renderer, menu, menuNum, writter, forProject, menuNumberLink, 2, buttonNum);
                            }
                            else
                            {
                                ManagedCore.Log.Error("Can not find menu for menu link id " + linkButton.MenuLinkID);
                            }
                        }

                        CMenuPlayAllButton playAllButton = d as CMenuPlayAllButton;
                        if (playAllButton != null)
                        {
                            buttonNum++;
                            GenerateMenuTextLineAndButtonGraphics(folder, playAllButton, renderer, menu, menuNum, writter, forProject, 0, 3, buttonNum);
                        }

                        CMenuPlayAllLoopedButton playAllLoopedButton = d as CMenuPlayAllLoopedButton;
                        if (playAllLoopedButton != null)
                        {
                            buttonNum++;
                            GenerateMenuTextLineAndButtonGraphics(folder, playAllLoopedButton, renderer, menu, menuNum, writter, forProject, 0, 4, buttonNum);
                        }
                    }
                }
            }
            finally
            {
                writter.Close();
                writter.Dispose();
            }

            return true;              
        }

        // ************************************************************************************************
        private void GenerateMenuTextLineAndButtonGraphics(string folder,
                                                           CMenuButton button, 
                                                           CMenuSubPictureRenderer renderer,
                                                           CMainMenu menu, 
                                                           int menuNum, 
                                                           StreamWriter writer,
                                                           CProject forProject,
                                                           int linkNum,
                                                           int buttonType,
                                                           int buttonNum)
        {
            RectangleF coverage = button.CoverageArea;

            CDecorationSubtitle ds = renderer.RenderButton(button, menu);
            Rectangle dest = ds.mSubtitleRegion;
            Bitmap highlightBitmap = ds.mCreatedBitmap;
            Bitmap selectBitmap = highlightBitmap.Clone() as Bitmap;
            
            string highlightName = "highlight" + buttonNum + ".png";
            string selectName =  "select" + buttonNum + ".png";

            string highlightLocation = folder + "\\" + highlightName ;
            string selectLocation = folder + "\\" + selectName; 
            
            try
            {
                Color highlightColor = menu.SubPictureStyle.SubPictureColor;
                Color selectColor = Color.FromArgb(highlightColor.R / 2, highlightColor.G / 2, highlightColor.B / 2);

                BitmapUtils.MakeTransparentOneColor(highlightBitmap, highlightColor, 255);
                BitmapUtils.MakeTransparentOneColor(selectBitmap, selectColor, 255);

                highlightBitmap.Save(highlightLocation, System.Drawing.Imaging.ImageFormat.Png);
                selectBitmap.Save(selectLocation, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception e)
            {
                ManagedCore.Log.Error("Excpetion when generating BD image. " + e.Message + " "+e.StackTrace);
            }
            finally
            {        
                highlightBitmap.Dispose();
                selectBitmap.Dispose();
            }

            string line = GenerateButtonString(dest, menuNum, buttonType, linkNum, highlightName, selectName);
            writer.WriteLine(line);
        }


        // ************************************************************************************************
        private string GenerateButtonString(RectangleF dest, int menuNum, int type, int playListNum, string highlightName, string selectName)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Button,");
            builder.Append(menuNum);
            builder.Append(",");
            builder.Append(type);       // 1= slideshow (video), 2 = link to menu, 3 play all button, 4 play all looped button
            builder.Append(",");
            builder.Append(dest.X);
            builder.Append(",");
            builder.Append(dest.Y);
            builder.Append(",");
            builder.Append(dest.Width);
            builder.Append(",");
            builder.Append(dest.Height);
            builder.Append(",");
            builder.Append(highlightName);
            builder.Append(",");
            builder.Append(selectName);
            builder.Append(",");
            builder.Append(playListNum);        // video BD playlist number OR menu number (depending on type)

            return builder.ToString();
        }
    }
}
