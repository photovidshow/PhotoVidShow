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
    /// Summary description for CSepiaTransformDecoration
    /// </summary>
    public class CSepiaTransformDecoration : CAnimatedDecoration
    {

        public CSepiaTransformDecoration()
        {
        }

        //*******************************************************************
        public CSepiaTransformDecoration(int order)
            : base(new RectangleF(0, 0, 1, 1), order)
        {

        }

        //*******************************************************************
        public override bool IsLayer()
        {
            return true;
        }

        //*******************************************************************
        public CSepiaTransformDecoration(CSepiaTransformDecoration copy)
            : base(copy)
        {
        }

        //*******************************************************************
        public override CDecoration Clone()
        {
            return new CSepiaTransformDecoration(this);
        }


        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {

            Rectangle return_rec = new Rectangle(0, 0, (int)r.Width, (int)r.Height);

            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return return_rec;

            PixelShaderEffect sepia_shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("sepia");
            if (sepia_shader == null)
            {
                CDebugLog.GetInstance().Error("Can not find sepia pixel shader");
                return return_rec;
            }

            sepia_shader.SetParameter(0, 1);
            sepia_shader.SetParameter(1, 0);
            sepia_shader.SetParameter(2, 1);

            ge.SetPixelShaderEffect(sepia_shader);
            try
            {
                // Redraw surface with shader
                ge.DrawImage(inputSurface, new RectangleF(0, 0, 1, 1), r, false);
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

            decoration.SetAttribute("Type", "SepiaTransformDecoration");

            SaveAnimatedDecorationPart(decoration, doc);

            parent.AppendChild(decoration);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadAnimatedDecorationPart(element);
        }
    }
}
