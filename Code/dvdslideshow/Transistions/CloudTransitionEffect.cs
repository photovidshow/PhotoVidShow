using System;
using System.Xml;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    /// <summary>
    /// Summary description for CloudTransitionEffect.
    /// </summary>
    public class CloudTransitionEffect : HeightMapTransition
    {
        public CloudTransitionEffect()
        {

        }

        public CloudTransitionEffect(float length, int spread)
            : base(length, spread)
        {
        }

        //*******************************************************************
        public override CTransitionEffect Clone()
        {
            CloudTransitionEffect t = new CloudTransitionEffect(mLength, mSpread);
            t.Index = this.Index;
            return t;
        }

        //*******************************************************************
        public override string GetIdString()
        {
            return "CloudTransitionEffect1";
        }

        //*******************************************************************
        public override void BuildMap(int width, int height, BitmapData bitmapData)
        {
            float real_spread = mSpread * 256.0f;
   
            float half_spread = real_spread / 2;

            float max = 65535 / 8;
            max /= 2;

            unsafe
            {
                uint* ptr = (uint*)bitmapData.Scan0.ToPointer();
                int pitch = bitmapData.Stride;
                pitch >>= 2;


                for (int j = 0; j < width; j++)
                {
                    float fw = ((float)j)/width;

                    for (int i = 0; i < height; i++)
                    {
                        float fh = ((float)i)/height;

                        float x1 = (float)((Math.Sin(fh * 19) + 1) * max);
                        float x2 = (float)((Math.Cos(fh * 3) + 1) * max);
                        float x3 = (float)((Math.Cos(fh * 43)+1) * max);
                        float x4 = (float)((Math.Sin(fh * 23) + 1) * max);
                                              
                        float y1 = (float)((Math.Sin(fw * 15)+1) * max);
                        float y2 = (float)((Math.Cos(fw * 7) + 1) * max);
                        float y3 = (float)((Math.Cos(fw * 61) + 1) * max);
                        float y4 = (float)((Math.Sin(fw * 53) + 1) * max);

                        ptr[(i * pitch)+j] = (uint)(x1+x2+x3+x4 + y1+y2+y3+y4);
       
                    }
                }
            }
        }

        //*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
        {
            effect.SetAttribute("Type", "CloudTransitionEffect");
            SaveHeightMapTransitionPart(effect, doc);
            parent.AppendChild(effect);
        }


        //*******************************************************************
        public override void Load(XmlElement element)
        {
            LoadHeightMapTransitionPart(element);
        }
    }
}
