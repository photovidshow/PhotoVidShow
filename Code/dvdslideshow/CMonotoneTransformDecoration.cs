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
    /// Summary description for CMonotoneTransformDecoration
    /// </summary>
    public class CMonotoneTransformDecoration : CAnimatedDecoration
    {

        public CMonotoneTransformDecoration()
        {
        }

        //*******************************************************************
        public CMonotoneTransformDecoration(int order)
            : base(new RectangleF(0, 0, 1, 1), order)
        {

        }

        //*******************************************************************
        public override bool IsLayer()
        {
            return true;
        }

        //*******************************************************************
        public CMonotoneTransformDecoration(CMonotoneTransformDecoration copy)
            : base(copy)
        {
        }

        //*******************************************************************
        public override CDecoration Clone()
        {
            return new CMonotoneTransformDecoration(this);
        }


        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {
          
            Rectangle return_rec = new Rectangle(0, 0, (int)r.Width, (int)r.Height);

            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return return_rec;

            PixelShaderEffect mono_shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader("monotone");
            if (mono_shader == null)
            {
                CDebugLog.GetInstance().Error("Can not find monotone pixel shader");
                return return_rec;
            }

            ge.SetPixelShaderEffect(mono_shader);
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

            decoration.SetAttribute("Type", "MonotoneTransformDecoration");

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
