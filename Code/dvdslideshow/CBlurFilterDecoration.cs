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
    /// Summary description for CVideoDecoration .
    /// </summary>
    public class CBlurFilterDecoration : CAnimatedDecoration
    {
        private float mStrength = 1f;  // 0.0 = soft, 1 = medium, 2.0 = strong

        // used by transitions do have more control over blur
        private float mBlurStep = 1.0f / 1920.0f;

        public enum BlurType
        {
            NORMAL =0 ,
            LENS
        }

        private BlurType mBlurType = BlurType.NORMAL;

        public BlurType BlurFilterType
        {
            get { return mBlurType; }
            set { mBlurType = value; }
        }

        public float Strength
        {
            get { return mStrength; }
            set { mStrength = value; }
        }

        public float BlurStep
        {
            get { return mBlurStep; }
            set { mBlurStep = value; }
        }

        public CBlurFilterDecoration()
        {
        }

        //*******************************************************************
        public CBlurFilterDecoration(int order)
            : base(new RectangleF(0,0,1,1) , order)
        {

        }

        //*******************************************************************
        public CBlurFilterDecoration(CBlurFilterDecoration copy)
            : base(copy)
        {
        }

        //*******************************************************************
        public override bool IsLayer()
        {
            return true;
        }

        //*******************************************************************
        public override CDecoration Clone()
        {
            return new CBlurFilterDecoration(this);
        }

     
        //*******************************************************************
        public override Rectangle RenderToGraphics(RectangleF r, int frame_num, CSlide originating_slide, RenderSurface inputSurface)
        {
            float w_step = 0.0f;
            float h_step = 0.0f;

            float strength = mStrength;

            if (mBlurType == BlurType.LENS)
            {
                // if lens blur increase strength
                strength += 1.0f;
            }

            if (mBlurStep != 0)
            {
                float blur_w = 1.0f / mBlurStep;
                float blur_h = blur_w / (r.Width / r.Height);

                w_step = (1.0f / blur_w) + (strength / blur_w);
                h_step = (1.0f / blur_h) + (strength / blur_h);
            }
           
            Rectangle return_rec = new Rectangle(0, 0, (int)r.Width, (int)r.Height);

            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge == null) return return_rec;

            RenderSurface original_surface = ge.GetRenderTarget();

            string prefix = "";
            if (mBlurType == BlurType.LENS)
            {
                prefix = "l";
            }
         
            PixelShaderEffect h_blur_shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader(prefix+"blurh");
            if (h_blur_shader == null)
            {
                CDebugLog.GetInstance().Error("Can not find "+prefix+"+blurh pixel shader");
                return return_rec;
            }

            PixelShaderEffect v_blur_shader = PixelShaderEffectDatabase.GetInstance().GetPixelShader(prefix+"blurv");
            if (v_blur_shader == null)
            {
                CDebugLog.GetInstance().Error("Can not find "+prefix+"blurv pixel shader");
                return return_rec;
            }
      
            RenderSurface rs = ge.GenerateRenderSurface((uint)r.Width, (uint)r.Height, this.ToString()+"::RenderToGraphics");
            ge.SetRenderTarget(rs);
            try
            {
                RectangleF srcRec = new RectangleF(0, 0, 1, 1);
                // Ok two passes, first we do a horizontal blur, then we do a vertical blur
 
                h_blur_shader.SetParameter(0, w_step);
                h_blur_shader.SetParameter(1, w_step * 6f);           // 6 because it's half the value 13 -1 defined in the shader code
                ge.SetPixelShaderEffect(h_blur_shader);

                ge.DrawImage(inputSurface, srcRec, r, false);
                ge.SetRenderTarget(original_surface);
         
                v_blur_shader.SetParameter(0, h_step);
                v_blur_shader.SetParameter(1, h_step * 6f);  // 6 because it's half the value 13 -1 defined in the shader code
                ge.SetPixelShaderEffect(v_blur_shader);

                ge.DrawImage(rs, srcRec, r, false);

                rs.Dispose();
                rs = null; 
            }
            catch (Exception exception)
            {
                CDebugLog.GetInstance().Error("Exception occurred in BlurFilterDecoration::RenderToGraphics: " + exception.Message);
                ge.SetRenderTarget(original_surface);
                if (rs != null) rs.Dispose();
            }
            finally
            {
                GraphicsEngine.Current.SetPixelShaderEffect(null);
            }

            return return_rec;
        }

    

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc)
        {
            XmlElement decoration = doc.CreateElement("Decoration");

            decoration.SetAttribute("Type", "BlurFilterDecoration");

            if (mStrength != 1f)
            {
                decoration.SetAttribute("StrengthF", mStrength.ToString());
            }

            if (mBlurType != BlurType.NORMAL)
            {
                decoration.SetAttribute("BlurFilterType", ((int)mBlurType).ToString());
            }

            SaveAnimatedDecorationPart(decoration, doc);

            parent.AppendChild(decoration);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadAnimatedDecorationPart(element);

            string s = element.GetAttribute("StrengthF");
            if (s != "")
            {
                mStrength = float.Parse(s);
            }

            s = element.GetAttribute("BlurFilterType");
            if (s != "")
            {
                mBlurType = (BlurType)int.Parse(s);
            }

        }
    }
}
