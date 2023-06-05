using System;
using System.Collections;
using System.Text;

namespace DVDSlideshow
{
    //
    // Class is typically used in the aiding of creating error or waring messages.  It is useful to inform the user of which slide a problem is occurring in.
    //
    public class SlideshowAndSlideIdentifier
    {
        private string mSlideshow;
        private int mSlideNumber;

        public string Slideshow
        {
            get { return mSlideshow; }
            set { mSlideshow = value; }
        }

        public int SlideNumber
        {
            get { return mSlideNumber; }
            set { mSlideNumber = value; }
        }

        // **********************************************************************************************************
        public SlideshowAndSlideIdentifier(string slideshow, int slideNumber)
        {
            mSlideshow = slideshow;
            mSlideNumber = slideNumber;
        }

        // **********************************************************************************************************
        public static SlideshowAndSlideIdentifier FromDecoration(CDecoration decoration)
        {
            SlideshowAndSlideIdentifier message = new SlideshowAndSlideIdentifier("Unknown", 0);

            ArrayList list = CGlobals.mCurrentProject.GetAllProjectSlideshows(true);

            foreach (CSlideShow ss in list)
            {
                int number = 1;
                foreach (CSlide s in ss.mSlides)
                {
                    CImageSlide imageSlide = s as CImageSlide;
                    if (imageSlide !=null)
                    {
                        foreach (CDecoration d in imageSlide.GetAllAndSubDecorations())
                        {
                            if (d == decoration)
                            {
                                message.SlideNumber = number;

                                if (ss == CGlobals.mCurrentProject.PreMenuSlideshow)
                                {
                                    message.Slideshow = "Disc intro slideshow";
                                }
                                else
                                {
                                    message.Slideshow = ss.GetSlideshowLabelName();
                                }
                                return message;
                            }
                        }
                    }
                    number++;
                }
            }

            if (CGlobals.mCurrentProject.MainMenu != null)
            {
                ArrayList menusList = CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();
                foreach (CMainMenu menu in menusList)
                {
                    int number = 1;
                    foreach (CDecoration d in menu.BackgroundSlide.GetAllAndSubDecorations())
                    {
                        if (d == decoration)
                        {
                            message.mSlideshow = "Menu " + number;
                            message.SlideNumber = 1;
                        }
                    }
                    number++;
                }
            }

            return message;

        }

        // **********************************************************************************************************
        public static SlideshowAndSlideIdentifier FromSlide(CSlide slide)
        {
            SlideshowAndSlideIdentifier message = new SlideshowAndSlideIdentifier("Unknown", 0);

            ArrayList list = CGlobals.mCurrentProject.GetAllProjectSlideshows(true);

            foreach (CSlideShow ss in list)
            {
                int number = 1;
                foreach (CSlide s in ss.mSlides)
                {
                    if (s == slide)
                    {
                        message.SlideNumber = number;

                        if (ss == CGlobals.mCurrentProject.PreMenuSlideshow)
                        {
                            message.Slideshow = "Disc intro slideshow";
                        }
                        else
                        {
                            message.Slideshow = ss.GetSlideshowLabelName();
                        }
                        return message;
                    }
                    number++;
                }
            }

            ArrayList menusList =CGlobals.mCurrentProject.MainMenu.GetSelfAndAllSubMenus();
            foreach (CMainMenu menu in menusList)
            {
                int number = 1;
                if (menu.BackgroundSlide == slide)
                {
                    message.mSlideshow = "Menu " + number;
                    message.SlideNumber = 1;
                }
                number++;
            }

            return message;
        }
    }
}
