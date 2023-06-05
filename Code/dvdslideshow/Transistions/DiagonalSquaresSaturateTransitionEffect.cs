using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for DiagonalSquaresSaturateTransitionEffect.
    /// </summary>
    public class DiagonalSquaresSaturateTransitionEffect : MiddleSaturateToColorTransitionEffect
    {
        private int mNumSquaresH = 6;       // squares vertical
        private int mNumSquaresW = 5;       // squares horizontal
        private float mTimeSquareComplete = 0.5f;  // delta of total transition time spent doing transition of one square (i.e must be less than 1.0 )
        private bool mTopLeftBottomRight = false;  // start top left if true else starts bottom right
        private bool mVMirror = true; // vertial mirrored of above to achieve bottom left <-> top right

        //*******************************************************************
        public DiagonalSquaresSaturateTransitionEffect()
        {
        }

        //*******************************************************************
        public DiagonalSquaresSaturateTransitionEffect(Color c, int squaresH, int squaresW, float squareDelta, bool Tlbr, bool Vmirroed, float length)
            : base(c, length)
        {
            mNumSquaresH = squaresH;
            mNumSquaresW = squaresW;
            mTimeSquareComplete = squareDelta;
            mTopLeftBottomRight = Tlbr;
            mVMirror = Vmirroed;
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CTransitionEffect t = new DiagonalSquaresSaturateTransitionEffect(mColor, mNumSquaresH,mNumSquaresW, mTimeSquareComplete,mTopLeftBottomRight, mVMirror, mLength);
            t.Index = this.Index;
            return t;
        }


        //*******************************************************************
        protected override void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
            try
            {
                int i1Width = 0; 
                int i1Height = 0;

                if (thisSlideSurface != null)
                {
                    i1Width = (int)thisSlideSurface.Width;
                    i1Height = (int)thisSlideSurface.Height;
                }
                else if (nextSlideSurface != null)
                {
                    i1Width = (int)nextSlideSurface.Width;
                    i1Height = (int)nextSlideSurface.Height;
                }
                else
                {
                    ManagedCore.Log.Error("No surface valid in ProcessDualRenderSurfaces");
                    return;
                }

                int square_x_size = (int)((((float)i1Width) / mNumSquaresW) + 0.5f);
                int square_y_size = (int)((((float)i1Height) / mNumSquaresH) + 0.5f);

                int num_squares = mNumSquaresH * mNumSquaresW;

                delta /= mTimeSquareComplete;

                int max_cycles = mNumSquaresW + (mNumSquaresH - 1);

                float step = 1 / mTimeSquareComplete;
                step -= 1.0f;
                step /= (max_cycles - 1);

                int start_row_block = 0;
                if (mTopLeftBottomRight == false)
                {
                    start_row_block = mNumSquaresH - 1;
                }

                for (int square_h = 0; square_h < mNumSquaresH; square_h++)
                {
                    int current_row_block = start_row_block;

                    if (mTopLeftBottomRight == false)
                    {
                        current_row_block = start_row_block + mNumSquaresW - 1;
                        start_row_block--;
                    }
                    else
                    {
                        start_row_block++;
                    }

                    for (int square_w = 0; square_w < mNumSquaresW; square_w++)
                    {
                        int x = square_w * square_x_size;
                        int y = square_h * square_y_size;
                        int w = square_x_size;
                        int h = square_y_size;

                        int sw = square_w;
                        if (mVMirror)
                        {
                            sw = mNumSquaresW - square_w - 1;
                            x = sw * square_x_size;
                        }

                        // ok if squares dont divide exactly, make last squares fit exactly
                        if (square_h == mNumSquaresH - 1)
                        {
                            h = i1Height - y;
                        }
                        if (sw == mNumSquaresW - 1)
                        {
                            w = i1Width - x;
                        }

                        float dd = delta - (step * current_row_block);

                        if (dd > 1) dd = 1;
                        if (dd < 0) dd = 0;


                        // check bounds as very small images can go over bounds (i.e. the int round when calculating square_x_size)
                        if (x < i1Width && y < i1Height && (x + w) <= i1Width && (y + h) <= i1Height)
                        {
                            this.ProcessRectangle(dd, x, y, w, h, thisSlideSurface, nextSlideSurface);
                        }

                        if (mTopLeftBottomRight == false)
                        {
                            current_row_block--;
                        }
                        else
                        {
                            current_row_block++;
                        }
                    }
                }
            }
            finally
            {
                GraphicsEngine.Current.SetPixelShaderEffect(null);
            }
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "DiagonalSquaresSaturateTransitionEffect");

            effect.SetAttribute("SquaresH", mNumSquaresH.ToString());
            effect.SetAttribute("SquaresW", mNumSquaresW.ToString());
            effect.SetAttribute("TimeSquareDelta", mTimeSquareComplete.ToString());
            effect.SetAttribute("Tlbr", mTopLeftBottomRight.ToString());
            effect.SetAttribute("VMirror", mVMirror.ToString());

            SaveMiddleSaturatePart(effect, doc);

            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            string s1 = element.GetAttribute("SquaresH");
            if (s1 != "") mNumSquaresH = int.Parse(s1);

            s1 = element.GetAttribute("SquaresW");
            if (s1 != "") mNumSquaresW = int.Parse(s1);

            s1 = element.GetAttribute("TimeSquareDelta");
            if (s1 != "") mTimeSquareComplete = float.Parse(s1);

            s1 = element.GetAttribute("Tlbr");
            if (s1 != "") mTopLeftBottomRight = bool.Parse(s1);

            s1 = element.GetAttribute("VMirror");
            if (s1 != "") mVMirror = bool.Parse(s1);

            base.Load(element);
        }
    }
}
