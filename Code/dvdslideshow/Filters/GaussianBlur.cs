using System;
using System.Drawing;
using System.Drawing.Imaging;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for LinearBlur.
	/// </summary>
	public class GaussianBlur
	{
     
        static float[] BlurWeights = new float[13] 
        {
         1.0f / 4096.0f,
         12.0f / 4096.0f,
         66.0f / 4096.0f,
         220.0f / 4096.0f,
         495.0f / 4096.0f,
         792.0f / 4096.0f,
         924.0f / 4096.0f,
         792.0f / 4096.0f,
         495.0f / 4096.0f,
         220.0f / 4096.0f,
         66.0f / 4096.0f,
         12.0f / 4096.0f,
         1.0f / 4096.0f,
        };


        public GaussianBlur()
		{
		}

		public void Process(CImage image, int amount)
		{	
			Bitmap s1 = (Bitmap) image.GetRawImage();

            if (s1.Width < 1) return;
            if (s1.Height < 1) return;

			Bitmap t1 = new Bitmap(s1.Width,s1.Height,PixelFormat.Format32bppArgb);

			unsafe
			{
				BitmapData src = s1.LockBits(new Rectangle(0, 0, s1.Width, s1.Height),
					ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

				BitmapData temp = t1.LockBits(new Rectangle(0, 0, s1.Width, s1.Height),
					ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

				int temp_stride = temp.Stride;
				int src_stride = src.Stride;

                int maxWidthIndex = s1.Width - 1;
                int maxHeightIndex = s1.Height - 1;

                int tempstrideoffset = (s1.Width * 4) - temp_stride;

                for (int times = 0; times < amount; times++)
                {

                    uint* pqsrc = (uint*)(void*)src.Scan0;
                    byte* pqsrcbyte = (byte*)(void*)src.Scan0;

                    uint* pqtemp = (uint*)(void*)temp.Scan0;
                    byte* pqtempbyte = (byte*)(void*)temp.Scan0;


                    for (int hh = 0; hh < s1.Height; hh++)
                    {
                        for (int ww = 0; ww < s1.Width; ww++)
                        {
                            float outaa = 0;
                            float outrr = 0;
                            float outgg = 0;
                            float outbb = 0;

                            for (int uu = -6; uu <= 6; uu++)
                            {
                                int xpixel = ww + (uu*2);
                                if (xpixel < 0) xpixel = 0;
                                if (xpixel > maxWidthIndex) xpixel = maxWidthIndex;

                                uint pixel = *(pqsrc + xpixel);
                                float weight = BlurWeights[uu + 6];

                                uint alpha = pixel >> 24;
                                if (alpha > 0)
                                {
                                    float alphaweight = weight * alpha;
                                    outbb += (((float)(pixel & 0x000000ff))) * alphaweight;
                                    outgg += (((float)((pixel & 0x0000ff00) >> 8))) * alphaweight;
                                    outrr += (((float)((pixel & 0x00ff0000) >> 16))) * alphaweight;
                                    outaa += (((float)(alpha))) * weight;
                                }
                            }

                            if (outaa > 0)
                            {
                                outbb /= outaa;
                                outrr /= outaa;
                                outgg /= outaa;
                            }

                            *pqtempbyte++ = (byte)outbb;
                            *pqtempbyte++ = (byte)outgg;
                            *pqtempbyte++ = (byte)outrr;
                            *pqtempbyte++ = (byte)outaa;
                        }

                        pqsrc += (src_stride >> 2);
                        pqtempbyte += tempstrideoffset;
                    }

                    int wordTempStride = temp_stride >> 2;

                    for (int ww = 0; ww < s1.Width; ww++)
                    {
                        int woffset = ww * 4;
                        int hoffset = 0;

                        for (int hh = 0; hh < s1.Height; hh++)
                        {
                            float outaa = 0;
                            float outrr = 0;
                            float outgg = 0;
                            float outbb = 0;

                            for (int uu = -6; uu <= 6; uu++)
                            {
                                int xpixel = hh + (uu*2);
                                if (xpixel < 0) xpixel = 0;
                                if (xpixel > maxHeightIndex) xpixel = maxHeightIndex;

                                xpixel *= wordTempStride;

                                uint pixel = *(pqtemp + xpixel + ww);

                                float weight = BlurWeights[uu + 6];

                                uint alpha = pixel >>24;
                                if (alpha > 0)
                                {
                                    float alphaweight = weight * alpha;

                                    outbb += (((float)(pixel & 0x000000ff))) * alphaweight;
                                    outgg += (((float)((pixel & 0x0000ff00) >> 8))) * alphaweight;
                                    outrr += (((float)((pixel & 0x00ff0000) >> 16))) * alphaweight;
                                    outaa += (((float)(alpha))) * weight;
                                }
                            }

                            if (outaa > 0)
                            {
                                outbb /= outaa;
                                outrr /= outaa;
                                outgg /= outaa;
                            }

                            byte* dest = pqsrcbyte + hoffset + woffset;

                            *dest++ = (byte)outbb;
                            *dest++ = (byte)outgg;
                            *dest++ = (byte)outrr;
                            *dest++ = (byte)outaa;

                            hoffset += src_stride;
                        }
                    }
                }

				s1.UnlockBits(src); 
				t1.UnlockBits(temp); 

                t1.Dispose();

				return ;
			}
		}
	}
}
