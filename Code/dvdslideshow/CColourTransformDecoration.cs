using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using ManagedCore;
using System.Globalization;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CColourTransformDecoration
    /// </summary>
    public class CColourTransformDecoration : CAnimatedDecoration
    {
        private float mBrightnessR = 1;
        private float mBrightnessG = 1;
        private float mBrightnessB = 1;
        private float mContrastR = 1;
        private float mContrastG = 1;
        private float mContrastB = 1;
        private float mGamma = 1;

        public float BrightnessR
        {
            get { return mBrightnessR; }
            set { mBrightnessR = value; }
        }

        public float BrightnessG
        {
            get { return mBrightnessG; }
            set { mBrightnessG = value; }
        }

        public float BrightnessB
        {
            get { return mBrightnessB; }
            set { mBrightnessB = value; }
        }

        public float ContrastR
        {
            get { return mContrastR; }
            set { mContrastR = value; }
        }

        public float ContrastG
        {
            get { return mContrastG; }
            set { mContrastG = value; }
        }

        public float ContrastB
        {
            get { return mContrastB; }
            set { mContrastB = value; }
        }
    
        public CColourTransformDecoration()
        {
        }

        //*******************************************************************
        public CColourTransformDecoration(int order)
            : base(new RectangleF(0,0,1,1) , order)
        {

        }

        //*******************************************************************
        public override bool IsLayer()
        {
            return true;
        }

        //*******************************************************************
        public CColourTransformDecoration(CColourTransformDecoration copy)
            : base(copy)
        {
        }

        //*******************************************************************
        public override CDecoration Clone()
        {
            return new CColourTransformDecoration(this);
        }

     
        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {
            /* old GDI+ method

            Bitmap b = original_image.GetRawImage() as Bitmap;
            if (b == null) return new Rectangle(0, 0, 1,1);

            BrightnessContrast.ApplyBrightnessContrastGamma(b, mBrightnessR, mBrightnessG, mBrightnessB, mContrastR, mContrastG, mContrastB, mGamma);
            */

            Rectangle return_rec = new Rectangle(0, 0, (int)r.Width, (int)r.Height);

            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return return_rec;
    
            PixelShaderEffect b_c_shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("brightnesscontrast");
            if (b_c_shader == null)
            {
                CDebugLog.GetInstance().Error("Can not find brightness contrast shader");
                return return_rec;
            }

            b_c_shader.SetParameter(0, mBrightnessR-1.0f);
            b_c_shader.SetParameter(1, mBrightnessG-1.0f);
            b_c_shader.SetParameter(2, mBrightnessB-1.0f);
            b_c_shader.SetParameter(3, mContrastR);
            b_c_shader.SetParameter(4, mContrastG);
            b_c_shader.SetParameter(5, mContrastB);

            // ### SRG TODO no longer support gamma

            ge.SetPixelShaderEffect(b_c_shader);
            try
            {
                // Redraw surface with shader
                ge.DrawImage(inputSurface, new RectangleF(0,0,1,1), r, false);
            }
            finally
            {
                ge.SetPixelShaderEffect(null);
            }
            
            return return_rec;
        }

    

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement decoration = doc.CreateElement("Decoration");

            decoration.SetAttribute("Type", "ColourTransformDecoration");

            decoration.SetAttribute("br", mBrightnessR.ToString());
            decoration.SetAttribute("bg", mBrightnessG.ToString());
            decoration.SetAttribute("bb", mBrightnessB.ToString());
            decoration.SetAttribute("cr", mContrastR.ToString());
            decoration.SetAttribute("cg", mContrastG.ToString());
            decoration.SetAttribute("cb", mContrastB.ToString());
            decoration.SetAttribute("gamma", mGamma.ToString());

            SaveAnimatedDecorationPart(decoration, doc);

            parent.AppendChild(decoration);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s = element.GetAttribute("br");
            if (s != "") mBrightnessR = float.Parse(s);

            s = element.GetAttribute("bg");
            if (s != "") mBrightnessG = float.Parse(s);

            s = element.GetAttribute("bb");
            if (s != "") mBrightnessB = float.Parse(s);

            s = element.GetAttribute("cr");
            if (s != "") mContrastR = float.Parse(s);

            s = element.GetAttribute("cg");
            if (s != "") mContrastG = float.Parse(s);

            s = element.GetAttribute("cb");
            if (s != "") mContrastB = float.Parse(s);

            s = element.GetAttribute("gamma");
            if (s != "") mGamma = float.Parse(s);

            LoadAnimatedDecorationPart(element);
        }
    }
}
