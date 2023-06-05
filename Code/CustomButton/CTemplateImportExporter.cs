using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DVDSlideshow;
using ManagedCore;

namespace CustomButton
{
    public partial class CTemplateImportExporter : Form
    {
        private static string mCurrentName;
        private static string mCurrentDescription;
        private static string mCurrentTemplateFile;

        public delegate void NewSlideshowButtonDelegate(object sender, System.EventArgs e);

        //*******************************************************************
        public static void ExportCurrentSlideshow()
        {
            CTemplateImportExporter form = new CTemplateImportExporter();
            form.ShowDialog();
        }

        //*******************************************************************
        public static void ImportTemplate(NewSlideshowButtonDelegate newProjectCallback)
        {
            System.Windows.Forms.OpenFileDialog openTemplateDialog = new OpenFileDialog();

            openTemplateDialog.Filter = "Template Files (*.xml)|";
            openTemplateDialog.FilterIndex = 1;
            openTemplateDialog.RestoreDirectory = true;
            openTemplateDialog.InitialDirectory = CGlobals.GetTemplateDirectory();

            if (openTemplateDialog.ShowDialog() == DialogResult.OK)
            {

                newProjectCallback(new object(), new EventArgs());

                string filename = openTemplateDialog.FileName;

                System.Xml.XmlDocument my_doc = new XmlDocument();
                try
                {
                    my_doc.Load(filename);
                }
                catch (Exception exception1)
                {
                    Log.Error("Could not read template file:" + filename + " " + exception1.Message);
                    return;
                }

                XmlNodeList templateList =  my_doc.GetElementsByTagName("Template");

                if (templateList.Count > 0)
                {
                    XmlElement templateElement = templateList[0] as XmlElement;
                    mCurrentName = templateElement.GetAttribute("Name");
                    mCurrentDescription = templateElement.GetAttribute("Description");
                    mCurrentTemplateFile = filename;
                }

                // ok load decoration effects
                XmlNodeList decorationEffectList = my_doc.GetElementsByTagName("DecorationEffectsDatabase");

                if (decorationEffectList.Count > 0)
                {
                    XmlElement element = decorationEffectList[0] as XmlElement;
                    CAnimatedDecorationEffectDatabase.GetInstance().Append(element, false, true);
                }

                XmlNodeList list = my_doc.GetElementsByTagName("Slideshow");

                if (list.Count > 0)
                {
                    XmlElement element = list[0] as XmlElement;
                    try
                    {
                        CSlideShow slideshow = CGlobals.mCurrentProject.GetAllProjectSlideshows(false)[0] as CSlideShow;

                        slideshow.Load(element);

                        // if template is multislideslide, uncovert it now
                        if (slideshow.mSlides.Count > 0)
                        {
                            CMultiSlideSlide mss = slideshow.mSlides[0] as CMultiSlideSlide;
                            if (mss != null)
                            {
                                slideshow.ConvertMultiSlideSlideToSlides(mss);
                            }

                        }

                        // turn off fade effects
                        slideshow.FadeIn = false;
                        slideshow.FadeOut = false;

                    }
                    catch (Exception exception2)
                    {
                        Log.Error("Could not create slide from template:" + filename + " " + exception2.Message);
                        return;
                    }
                }
                else
                {
                    Log.Error("No slideshow defined in template:" + filename);
                    return;
                }

                CGlobals.mCurrentProject.DeclareChange("Imported template");
            }

         
        }


        //*******************************************************************
        private static void ConvertMultiSlideSlideToSlides(CMultiSlideSlide slide, CSlideShow slideshow)
        {
            slideshow.mSlides.Remove(slide);

            List<CImageSlide> subSlides = slide.SubSlides;

            foreach (CImageSlide subSlide in subSlides)
            {
                slideshow.mSlides.Add(subSlide);
            }

            slideshow.CalcLengthOfAllSlides();
        }


        //*******************************************************************
        public CTemplateImportExporter()
        {
            InitializeComponent();
            mNameTextBox.Text = mCurrentName;
            mDescriptionTextBox.Text = mCurrentDescription;

            CSlideShow slideshow = CGlobals.mCurrentProject.GetAllProjectSlideshows(false)[0] as CSlideShow;
            List<CAnimatedDecorationEffect> effects =CAnimatedDecorationEffectDatabase.GetInstance().GetDependanciesForSlide(slideshow);

            foreach (CAnimatedDecorationEffect effect in effects)
            {
                mDetectedAnimatedEffects.Text += effect.Name + "\r\n";
            }
        }
       
        //*******************************************************************
        private void mExportButton_Click(object sender, EventArgs e)
        {
            System.Xml.XmlDocument my_doc = new XmlDocument();

            XmlElement template = my_doc.CreateElement("Template");

            mCurrentName = mNameTextBox.Text;
            mCurrentDescription = mDescriptionTextBox.Text;

            template.SetAttribute("Version", "1.0.0");
            template.SetAttribute("Name", mCurrentName);
            template.SetAttribute("Description", mCurrentDescription);

            my_doc.AppendChild(template);

            CSlideShow slideshow = CGlobals.mCurrentProject.GetAllProjectSlideshows(false)[0] as CSlideShow;

            // multiple slides, covert into a multislideslide
            if (slideshow.mSlides.Count > 1)
            {
                slideshow.ConvertAllSlidesToMultiSlideSlide();
            }

            slideshow.Save(template, my_doc, true);

            CAnimatedDecorationEffectDatabase.GetInstance().SaveOnlyRequiredBySlideshow(slideshow, template, my_doc);

            System.Windows.Forms.SaveFileDialog saveProjectDialog  = new SaveFileDialog();

            saveProjectDialog.Filter = "Template Files (*.xml)|";
            saveProjectDialog.FilterIndex = 1;
            saveProjectDialog.RestoreDirectory = true;
            saveProjectDialog.InitialDirectory = CGlobals.GetTemplateDirectory();

            if (mCurrentTemplateFile != "")
            {
                saveProjectDialog.FileName = mCurrentTemplateFile;
            }

            if (saveProjectDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveProjectDialog.FileName;

                if (filename.EndsWith(".xml") == false)
                {
                    filename += ".xml";
                }

                my_doc.Save(filename) ;

                MessageBox.Show("All done... Re-start PhotoVidShow", "Done", MessageBoxButtons.OK);

                mCurrentTemplateFile = filename;

            }

            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
