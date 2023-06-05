using System;
using System.Drawing;
using System.Drawing.Imaging;
using MangedToUnManagedWrapper;
using ManagedCore;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for BitmapUtils.
	/// </summary>
	public class BitmapUtils
	{


        //*******************************************************************
        public static void EnsureMaskColours(Image i)
        {
            Bitmap bitmap = i as Bitmap;
            if (bitmap == null)
            {
                return;
            }
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                return;
            }

            int xx = 0;
            int yy = 0;
            int ww = bitmap.Width;
            int hh = bitmap.Height;

            BitmapData iImageData = bitmap.LockBits(new Rectangle(xx, yy, ww, hh),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            System.IntPtr Scan0 = iImageData.Scan0;

            unsafe
            {
                byte* pPixel = (byte*)(void*)Scan0;
                int nOffset = (iImageData.Stride - ww * 4);

                for (int y = 0; y < hh; y++)
                {
                    for (int x = 0; x < ww; x++)
                    {
                        byte b = *(pPixel + 0);
                        byte g = *(pPixel + 1);
                        byte r = *(pPixel + 2);

                        if (b > 250)
                        {
                            b = 255;
                        }
                        if (g > 250)
                        {
                            g = 255;
                        }
                        if (r > 250)
                        {
                            r = 255;
                        }

                        *(pPixel + 0) = b;
                        *(pPixel + 1) = g;
                        *(pPixel + 2) = r;
                        
                        pPixel += 4;
                    }
                    pPixel += nOffset;
                }
            }

            bitmap.UnlockBits(iImageData);
        }


		//*******************************************************************
		public static void FadeToBackColor(Bitmap image, Color c)
		{
			if (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
			{
				return ;
			}

			int rr = c.R*2;
			int gg = c.G*2;
			int bb = c.B*2;

			int xx= 0;
			int yy=0;
			int ww=image.Width;
			int hh=image.Height;

			BitmapData iImageData = image.LockBits(new Rectangle(xx, yy, ww, hh), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			System.IntPtr Scan0 = iImageData.Scan0;
		
			unsafe
			{
				byte * pPixel = (byte *)(void *)Scan0;
				int nOffset = (iImageData.Stride  - ww*4);

				for (int y = 0; y < hh; y++)
				{
					for (int x = 0; x < ww; x++)
					{
						byte b = *(pPixel+0);
						byte g = *(pPixel+1);
						byte r = *(pPixel+2);
					
						int bb1 = (bb +b) /3;
						int gg1 =(gg +g) /3;
						int rr1 =(rr+r) /3; 

						*(pPixel+0) =(byte)bb1;
						*(pPixel+1) =(byte)gg1;
						*(pPixel+2) =(byte)rr1;

						pPixel+=4;
					}
					pPixel += nOffset;
				}
			}

			image.UnlockBits(iImageData);
		}

		//*******************************************************************
		public static void FadeToBackColorLight(Bitmap image, Color c)
		{
			if (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
			{
				return ;
			}

			int rr = c.R;
			int gg = c.G;
			int bb = c.B;

			int xx= 0;
			int yy=0;
			int ww=image.Width;
			int hh=image.Height;

			BitmapData iImageData = image.LockBits(new Rectangle(xx, yy, ww, hh), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			System.IntPtr Scan0 = iImageData.Scan0;
		
			unsafe
			{
				byte * pPixel = (byte *)(void *)Scan0;
				int nOffset = (iImageData.Stride  - ww*4);

				for (int y = 0; y < hh; y++)
				{
					for (int x = 0; x < ww; x++)
					{
						byte b = *(pPixel+0);
						byte g = *(pPixel+1);
						byte r = *(pPixel+2);
					
						int bb1 = (bb +b+b) /3;
						int gg1 =(gg +g+g) /3;
						int rr1 =(rr+r+r) /3; 

						*(pPixel+0) =(byte)bb1;
						*(pPixel+1) =(byte)gg1;
						*(pPixel+2) =(byte)rr1;

						pPixel+=4;
					}
					pPixel += nOffset;
				}
			}

			image.UnlockBits(iImageData);
		}



		//*******************************************************************
		public static void MakeBitmapGrey(Bitmap image, int add_offset)
		{
			if (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
			{
				return ;
			}

			int xx= 0;
			int yy=0;
			int ww=image.Width;
			int hh=image.Height;

			BitmapData iImageData = image.LockBits(new Rectangle(xx, yy, ww, hh), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			System.IntPtr Scan0 = iImageData.Scan0;
		
			unsafe
			{
				byte * pPixel = (byte *)(void *)Scan0;
				int nOffset = (iImageData.Stride  - ww*4);

				for (int y = 0; y < hh; y++)
				{
					for (int x = 0; x < ww; x++)
					{
						byte b = *(pPixel+0);
						byte g = *(pPixel+1);
						byte r = *(pPixel+2);
						int all = b+g+r;
						all /=3;
						all +=add_offset;
						if (all<0) all=0;
						if (all>255) all=255;

						byte bb = (byte)all;
						*(pPixel+0) = bb;
						*(pPixel+1) = bb;
						*(pPixel+2) = bb;
						pPixel+=4;
					}
					pPixel += nOffset;
				}
			}

			image.UnlockBits(iImageData);
		}


		//*******************************************************************
		public static void MakeBitmapGreyDelta(Bitmap image, float delta, int add)
		{
			if (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
			{
				return ;
			}

			int xx= 0;
			int yy=0;
			int ww=image.Width;
			int hh=image.Height;

			BitmapData iImageData = image.LockBits(new Rectangle(xx, yy, ww, hh), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			System.IntPtr Scan0 = iImageData.Scan0;

			int add2 = add*2;
		
			unsafe
			{
				byte * pPixel = (byte *)(void *)Scan0;
				int nOffset = (iImageData.Stride  - ww*4);

				for (int y = 0; y < hh; y++)
				{
					for (int x = 0; x < ww; x++)
					{
						byte b = *(pPixel+0);
						byte g = *(pPixel+1);
						byte r = *(pPixel+2);
						int all = b+g+r;
						all /=3;
						if (all<0) all=0;
						if (all>255) all=255;

						byte bb = (byte)all;

						int ra = ((int)(((float)(bb - r))*delta)) + r+add2;
						int rg = ((int)(((float)(bb - g))*delta)) + g+add2;
						int rb = ((int)(((float)(bb - b))*delta)) + b+add;
						//	if (all >128) ra+=add; else ra-=add;
						//	if (all >128) rg+=add;else rg-=add;
						//	if (all>128) rb+=add;else rb-=add;

						if (ra>255) ra=255;
						if (rg>255) rg=255;
						if (rb>255) rb=255;

						//	if (ra<0) ra=0;
						//	if (rg<0) rg=0;
						//	if (rb<0) rb=0;

						//	r = (byte) ra;
						//	g = (byte) rg;
						//	b = (byte) rb;

						*(pPixel+0) = (byte) rb;
						*(pPixel+1) = (byte) rg;
						*(pPixel+2) = (byte) ra;
						pPixel+=4;
					}
					pPixel += nOffset;
				}
			}

			image.UnlockBits(iImageData);
		}



		//*******************************************************************
		public static void BitmapBilinearCopy(Bitmap a, Bitmap b)
		{
			//MangedToUnManagedWrapper.CDVDEncode.BilnearBitmapCopy(a,b);
		}

		//*******************************************************************
		public static void unsafe_copy(BitmapData bmSrc, BitmapData bmData, int x,int y, int w,int h)
		{

			//	CDebugLog.GetInstance().Warning("Called unsafe_copy"+x+" "+y+" "+w+" "+h);


			int scanline = bmData.Stride;
    
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcScan0 = bmSrc.Scan0;
    
			unsafe
			{
				int * p = (int *)(void *)Scan0;
				int * pSrc = (int *)(void *)SrcScan0;
        
				int nOffset = (bmData.Stride  - w*4)/4;

				p+=x+(y*nOffset);
				pSrc+=x+(y*nOffset);

				for(int yy=0;yy < h;++yy)
				{
					for(int xx=0; xx < w; ++xx )
					{   
						*p = *pSrc;
						p++;
						pSrc++;
					}
					p += nOffset;
					pSrc +=nOffset;
				}
			}
  
		}

        //*******************************************************************
        public static void unsafe_copy_offsets(BitmapData bmSrc, BitmapData bmData, int src_x, int src_y, int dest_x, int dest_y, int w, int h)
        {
            int line_mult = bmData.Stride >> 2;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                int* p = (int*)(void*)Scan0;
                int* pSrc = (int*)(void*)SrcScan0;

                int nSrcOffset = (bmSrc.Stride - w * 4) / 4;
                int nDstOffset = (bmData.Stride - w * 4) / 4;

                p += dest_x + (dest_y * line_mult);
                pSrc += src_x + (src_y * line_mult);

                for (int yy = 0; yy < h; ++yy)
                {
                    for (int xx = 0; xx < w; ++xx)
                    {
                        *p = *pSrc;
                        p++;
                        pSrc++;
                    }
                    p += nDstOffset;
                    pSrc += nSrcOffset;
                }
            }

        }

		//*******************************************************************
		public static void alpha_blend(BitmapData bmSrca, BitmapData bmSrcb, BitmapData bmData,int alpha, int x,int y, int w,int h)
		{
			int scanline = bmData.Stride;
    
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcaScan0 = bmSrca.Scan0;
			System.IntPtr SrcbScan0 = bmSrcb.Scan0;

			if (x+w<0) return ;

			
			int tt=0;

			if (x<0)
			{
				w+=x;
				tt-=x;
				x=0;
			}
			
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * pSrca = (byte *)(void *)SrcaScan0;
				byte * pSrcb = (byte *)(void *)SrcbScan0;
        
				int nOffset = (bmData.Stride  - w*4);

				p+=x*4+(y*nOffset);
				pSrca+=x*4+(y*nOffset);
				pSrcb+=x*4+(y*nOffset);

		
				int inv_a = 255-alpha;

				for(int yy=0;yy < h;++yy)
				{
					for(int xx=0; xx < w; ++xx )
					{  
						// abg

                        // SRG TODO ( IS THIS THE FASTEST, SAME PROBLEM ON OTHER EFFECTS?)
                        if (*pSrca != *pSrcb)
                        {
                            *p++ = (byte)((((*pSrca++) * alpha) >> 8) + (((*pSrcb++) * inv_a) >> 8));
                        }
                        else
                        {
                            *p++ = *pSrca++; ;
                            pSrcb++;
                        }

                        if (*pSrca != *pSrcb)
                        {
                            *p++ = (byte)((((*pSrca++) * alpha) >> 8) + (((*pSrcb++) * inv_a) >> 8));
                        }
                        else
                        {
                            *p++ = *pSrca++; ;
                            pSrcb++;
                        }

                        if (*pSrca != *pSrcb)
                        {
                            *p++ = (byte)((((*pSrca++) * alpha) >> 8) + (((*pSrcb++) * inv_a) >> 8));
                        }
                        else
                        {
                            *p++ = *pSrca++; ;
                            pSrcb++;
                        }


                        //*p++ =  (byte) ((((*pSrca++)*alpha)>>8) + (((*pSrcb++)*inv_a) >>8));
					    //*p++  = (byte) ((((*pSrca++)*alpha)>>8) + (((*pSrcb++)*inv_a) >>8)); 
						//*p++  = (byte) ((((*pSrca++)*alpha)>>8) + (((*pSrcb++)*inv_a) >>8)); 
						*p++  = 255;

						++pSrca;++pSrcb;
					}
					p += nOffset;
					pSrca +=nOffset;
					pSrcb +=nOffset;
				}
			}
  
		}

		//*******************************************************************
		public static void alpha_blend_color(BitmapData bmSrca, Color c, BitmapData bmData,int alpha, int x,int y, int w,int h)
		{
			int scanline = bmData.Stride;
    
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcaScan0 = bmSrca.Scan0;
			
			if (x+w<0) return ;

			int tt=0;

			if (x<0)
			{
				w+=x;
				tt-=x;
				x=0;
			}
			
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * pSrca = (byte *)(void *)SrcaScan0;

				byte r = c.R;
				byte g = c.G;
				byte b = c.B;
			
				int nOffset = (bmData.Stride  - w*4);

				p+=x*4+(y*nOffset);
				pSrca+=x*4+(y*nOffset);
			
				int inv_a = 255-alpha;

				for(int yy=0;yy < h;++yy)
				{
					for(int xx=0; xx < w; ++xx )
					{  
						// bgr
						*p++  = (byte) ((((*pSrca++)*alpha)>>8) + (((b)*inv_a) >>8)); 
						*p++  = (byte) ((((*pSrca++)*alpha)>>8) + (((g)*inv_a) >>8)); 
						*p++  = (byte) ((((*pSrca++)*alpha)>>8) + (((r)*inv_a) >>8)); 
						*p++  = 255;

						++pSrca;
					}
					p += nOffset;
					pSrca +=nOffset;
				}
			}
  
		}


		//*******************************************************************
		public static void add_color(Bitmap in_image, Bitmap out_image, Color c)
		{
			BitmapData inData = in_image.LockBits(new Rectangle(0, 0, 
				in_image.Width, in_image.Height), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			BitmapData outData = out_image.LockBits(new Rectangle(0, 0, 
				out_image.Width, out_image.Height), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


			add_color(inData,c, outData, 0,0,out_image.Width,out_image.Height);

			in_image.UnlockBits(inData);

			out_image.UnlockBits(outData);

		}


        //*******************************************************************
        public static void add_color(Bitmap in_image, Color c)
        {
            BitmapData inData = in_image.LockBits(new Rectangle(0, 0,
                in_image.Width, in_image.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            add_color(inData, c, inData, 0, 0, in_image.Width, in_image.Height);

            in_image.UnlockBits(inData);

        }


		//*******************************************************************
		public static void add_color(BitmapData bmSrca, Color c, BitmapData bmData,int x,int y, int w,int h)
		{
			int scanline = bmData.Stride;
    
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcaScan0 = bmSrca.Scan0;
			
			if (x+w<0) return ;

			int tt=0;

			if (x<0)
			{
				w+=x;
				tt-=x;
				x=0;
			}
			
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * pSrca = (byte *)(void *)SrcaScan0;

				byte r = c.R;
				byte g = c.G;
				byte b = c.B;
			
				int nOffset = (bmData.Stride  - w*4);

                p += x * 4 + (y * bmData.Stride);
                pSrca += x * 4 + (y * bmData.Stride);
			
	
				for(int yy=0;yy < h;++yy)
				{
					for(int xx=0; xx < w; ++xx )
					{  
						// bgr

						if (0xff - (*pSrca) < b) *p++=0xff; else *p++=(byte)((*pSrca)+b);
						++pSrca;
						if (0xff - (*pSrca) < g) *p++=0xff; else *p++=(byte)((*pSrca)+g);
						++pSrca;
						if (0xff - (*pSrca) < r) *p++=0xff; else *p++=(byte)((*pSrca)+r);
						++pSrca;
						*p++  = 255;
						++pSrca;
					}
					p += nOffset;
					pSrca +=nOffset;
				}
			}
  
		}


        //*******************************************************************
        public static void add_images(Bitmap bmDest, Bitmap bmSrc)
        {
            // bitmaps must have same width/height
            if (bmDest.Width != bmSrc.Width ||
                bmDest.Height != bmSrc.Height)
            {
                return;
            }

            int width = bmDest.Width;
            int height = bmDest.Height;

            BitmapData iSrcData = bmSrc.LockBits(new Rectangle(0, 0,
                bmSrc.Width, bmSrc.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            BitmapData iDestData = bmDest.LockBits(new Rectangle(0, 0,
              bmDest.Width, bmDest.Height),
              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            System.IntPtr DestScan0 = iDestData.Scan0;
            System.IntPtr SrcScan0 = iSrcData.Scan0;

            unsafe
            {
                byte* pDest = (byte*)(void*)DestScan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = (iDestData.Stride - width * 4);

                int r = 0;

                for (int yy = 0; yy < height; ++yy)
                {
                    for (int xx = 0; xx < width; ++xx)
                    {
                        // bgr

                        r = (*pSrc++) + (*pDest);
                        if (r >=255) *pDest++ = 255; else *pDest++ = (byte)r;

                        r = (*pSrc++) + (*pDest);
                        if (r >= 255) *pDest++ = 255; else *pDest++ = (byte)r;

                        r = (*pSrc++) + (*pDest);
                        if (r >= 255) *pDest++ = 255; else *pDest++ = (byte)r;

                        *pDest++ = 255;
                        ++pSrc;
                    }
                    pDest += nOffset;
                    pSrc += nOffset;
                }
            }

            bmSrc.UnlockBits(iSrcData);
            bmDest.UnlockBits(iDestData);
        }

        //*******************************************************************
        public static void copy_image_to_alpha_channel(Bitmap srcImg, Bitmap DstImg)
        {
            // bitmaps must have same width/height
            if (srcImg.Width != DstImg.Width ||
                srcImg.Height != DstImg.Height)
            {
                return;
            }

            int width = DstImg.Width;
            int height = DstImg.Height;

            BitmapData iSrcData = srcImg.LockBits(new Rectangle(0, 0,
                srcImg.Width, srcImg.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            BitmapData iDestData = DstImg.LockBits(new Rectangle(0, 0,
              DstImg.Width, DstImg.Height),
              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            System.IntPtr DestScan0 = iDestData.Scan0;
            System.IntPtr SrcScan0 = iSrcData.Scan0;

            unsafe
            {
                byte* pDest = (byte*)(void*)DestScan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                pDest += 3;

                int nOffset = (iDestData.Stride - width * 4);

                for (int yy = 0; yy < height; ++yy)
                {
                    for (int xx = 0; xx < width; ++xx)
                    {
                        // bgr

                        *pDest = *pSrc;
                        pDest += 4;
                        pSrc += 4;
                    }
                    pDest += nOffset;
                    pSrc += nOffset;
                }
            }

            srcImg.UnlockBits(iSrcData);
            DstImg.UnlockBits(iDestData);
        }

        //*******************************************************************
        public static void sub_images_dest_minus_src(Bitmap bmDest, Bitmap bmSrc)
        {
            // bitmaps must have same width/height
            if (bmDest.Width != bmSrc.Width ||
                bmDest.Height != bmSrc.Height)
            {
                return;
            }

            int width = bmDest.Width;
            int height = bmDest.Height;

            BitmapData iSrcData = bmSrc.LockBits(new Rectangle(0, 0,
                bmSrc.Width, bmSrc.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            BitmapData iDestData = bmDest.LockBits(new Rectangle(0, 0,
              bmDest.Width, bmDest.Height),
              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            System.IntPtr DestScan0 = iDestData.Scan0;
            System.IntPtr SrcScan0 = iSrcData.Scan0;

            unsafe
            {
                byte* pDest = (byte*)(void*)DestScan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = (iDestData.Stride - width * 4);

                int r = 0;

                for (int yy = 0; yy < height; ++yy)
                {
                    for (int xx = 0; xx < width; ++xx)
                    {
                        // bgr

                        r = (*pDest)- (*pSrc++);
                        if (r <= 0) *pDest++ = 0; else *pDest++ = (byte)r;

                        r = (*pDest) - (*pSrc++);
                        if (r <= 0) *pDest++ = 0; else *pDest++ = (byte)r;

                        r = (*pDest) - (*pSrc++);
                        if (r <= 0) *pDest++ = 0; else *pDest++ = (byte)r;

                        *pDest++ = 255;
                        ++pSrc;
                    }
                    pDest += nOffset;
                    pSrc += nOffset;
                }
            }

            bmSrc.UnlockBits(iSrcData);
            bmDest.UnlockBits(iDestData);
        }


        //*******************************************************************
        public static void sub_images_src_minus_dest(Bitmap bmDest, Bitmap bmSrc)
        {
            // bitmaps must have same width/height
            if (bmDest.Width != bmSrc.Width ||
                bmDest.Height != bmSrc.Height)
            {
                return;
            }

            int width = bmDest.Width;
            int height = bmDest.Height;

            BitmapData iSrcData = bmSrc.LockBits(new Rectangle(0, 0,
                bmSrc.Width, bmSrc.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            BitmapData iDestData = bmDest.LockBits(new Rectangle(0, 0,
              bmDest.Width, bmDest.Height),
              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            System.IntPtr DestScan0 = iDestData.Scan0;
            System.IntPtr SrcScan0 = iSrcData.Scan0;

            unsafe
            {
                byte* pDest = (byte*)(void*)DestScan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = (iDestData.Stride - width * 4);

                int r = 0;

                for (int yy = 0; yy < height; ++yy)
                {
                    for (int xx = 0; xx < width; ++xx)
                    {
                        // bgr

                        r = (*pSrc++) - (*pDest) ;
                        if (r <= 0) *pDest++ = 0; else *pDest++ = (byte)r;

                        r = (*pSrc++) - (*pDest);
                        if (r <= 0) *pDest++ = 0; else *pDest++ = (byte)r;

                        r = (*pSrc++) - (*pDest);
                        if (r <= 0) *pDest++ = 0; else *pDest++ = (byte)r;

                        *pDest++ = 255;
                        ++pSrc;
                    }
                    pDest += nOffset;
                    pSrc += nOffset;
                }
            }

            bmSrc.UnlockBits(iSrcData);
            bmDest.UnlockBits(iDestData);
        }

        //*******************************************************************
        public static void modulate_images(Bitmap bmDest, Bitmap bmSrc, int modulate_shift)
        {
            // bitmaps must have same width/height
            if (bmDest.Width != bmSrc.Width ||
                bmDest.Height != bmSrc.Height)
            {
                return;
            }

            int width = bmDest.Width;
            int height = bmDest.Height;

            BitmapData iSrcData = bmSrc.LockBits(new Rectangle(0, 0,
                bmSrc.Width, bmSrc.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            BitmapData iDestData = bmDest.LockBits(new Rectangle(0, 0,
              bmDest.Width, bmDest.Height),
              ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            System.IntPtr DestScan0 = iDestData.Scan0;
            System.IntPtr SrcScan0 = iSrcData.Scan0;

            unsafe
            {
                byte* pDest = (byte*)(void*)DestScan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = (iDestData.Stride - width * 4);

                int r = 0;

                for (int yy = 0; yy < height; ++yy)
                {
                    for (int xx = 0; xx < width; ++xx)
                    {
                        // bgr

                        r = ((*pSrc++) * (*pDest));
                        r /= 255;
                        r <<= modulate_shift;
                        if (r >=255) *pDest++ = 255; else *pDest++ = (byte)r;

                        r = ((*pSrc++) * (*pDest));
                        r /= 255;
                        r <<= modulate_shift;
 
                        if (r >= 255) *pDest++ = 255; else *pDest++ = (byte)r;

                        r = ((*pSrc++) * (*pDest));
                        r /= 255;
                        r <<= modulate_shift;
                        if (r >= 255) *pDest++ = 255; else *pDest++ = (byte)r;

                        *pDest++ = 255;
                        ++pSrc;
                    }
                    pDest += nOffset;
                    pSrc += nOffset;
                }
            }

            bmSrc.UnlockBits(iSrcData);
            bmDest.UnlockBits(iDestData);
        }

		//*******************************************************************
		public static void sub_color(BitmapData bmSrca, Color c, BitmapData bmData,int x,int y, int w,int h)
		{
			int scanline = bmData.Stride;
    
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcaScan0 = bmSrca.Scan0;
			
			if (x+w<0) return ;

			int tt=0;

			if (x<0)
			{
				w+=x;
				tt-=x;
				x=0;
			}
			
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * pSrca = (byte *)(void *)SrcaScan0;

				byte r = c.R;
				byte g = c.G;
				byte b = c.B;
			
				int nOffset = (bmData.Stride  - w*4);

                p += x * 4 + (y * bmData.Stride);
                pSrca += x * 4 + (y * bmData.Stride);

				for(int yy=0;yy < h;++yy)
				{
					for(int xx=0; xx < w; ++xx )
					{  
						// bgr

						if ((*pSrca) < b) *p++=0x00; else *p++=(byte)((*pSrca)-b);
						++pSrca;
						if ((*pSrca) < g) *p++=0x00; else *p++=(byte)((*pSrca)-g);
						++pSrca;
						if ((*pSrca) < r) *p++=0x00; else *p++=(byte)((*pSrca)-r);
						++pSrca;
						*p++  = 255;
						++pSrca;
					}
					p += nOffset;
					pSrca +=nOffset;
				}
			}
  
		}

		//*******************************************************************
		public static void AddAlpha(BitmapData image, BitmapData mask, int x,int y, int w,int h)
		{
			System.IntPtr imgScan0 = image.Scan0;
			System.IntPtr maskScan0 = mask.Scan0;
    
			unsafe
			{
				uint * pimg = (uint *)(void *)imgScan0;
				uint * pmask = (uint *)(void *)maskScan0;
        
				int nOffset = (image.Stride  - w*4)/4;

				pimg+=x+(y*nOffset);
				pmask+=x+(y*nOffset);

				for(int yy=0;yy < h;++yy)
				{
					for(int xx=0; xx < w; ++xx )
					{   
						uint m = (*pmask) <<8 ;
						m&=0xff000000;
						uint im = *pimg;
						im &= 0x00ffffff;
						im |= m;

						*pimg =im;

						pimg++;
						pmask++;
					}
					pimg += nOffset;
					pmask +=nOffset;
				}
			}
  
		}

		//*******************************************************************
		public static void AddAlpha(Bitmap image, Bitmap m, int x,int y, int w,int h)
		{

			BitmapData iImageData = image.LockBits(new Rectangle(x, y, 
				w, h), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


			BitmapData iMaskData = m.LockBits(new Rectangle(x, y, 
				w, h), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			AddAlpha(iImageData,iMaskData,x,y,w,h);

			image.UnlockBits(iImageData);
			m.UnlockBits(iMaskData);

		}

		//*******************************************************************
		// convert a bitmap into one color, with a max alpha (must be 64,128 or 192)
		// used to create subtitle dvd menu bitmaps etc
		public static void MakeTransparentOneColor(Bitmap image, Color c, int max_alpha)
		{
			int w = image.Width;
			int h = image.Height;

			BitmapData iImageData = image.LockBits(new Rectangle(0, 0, 
				w, h), 
				ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			System.IntPtr imgScan0 = iImageData.Scan0;

			unsafe
			{
				uint col = (uint) c.ToArgb();
				col &=0x00ffffff;

				uint * pimg = (uint *)(void *)imgScan0;
			
				int nOffset = (iImageData.Stride  - w*4)/4;

				for(int yy=0;yy < h;++yy)
				{
					for(int xx=0; xx < w; ++xx )
					{   
						uint m = *pimg;
						uint a = m >> 24;
	
						if (a > 0 || m>0)
						{
                            if (a > max_alpha)
                            {
                                a = (uint)max_alpha;
                            }

                            a = a >> 6;
                            a = a <<6;

							*pimg = a<<24 | col;
						}
						else
						{
							*pimg = 0;
						}
						
						pimg++;
					
					}
					pimg += nOffset;
				
				}
			}

			image.UnlockBits(iImageData);
	
		}


		//*******************************************************************
		public static void unsafe_copy_alpha(BitmapData bmSrca, BitmapData bmSrcb, BitmapData bmData, byte [] alpha, int alpha_width, int x,int y, int w,int h)
		{

		//	CDebugLog.GetInstance().Warning("Called unsafe_copy_alpha"+x+" "+y+" "+w+" "+h);

			int scanline = bmData.Stride;
    
			System.IntPtr Scan0 = bmData.Scan0;
			System.IntPtr SrcaScan0 = bmSrca.Scan0;
			System.IntPtr SrcbScan0 = bmSrcb.Scan0;

			if (x+w<0) return ;

			
			int tt=0;

			if (x<0)
			{
				w+=x;
				tt-=x;
				x=0;
			}
			
			unsafe
			{
				byte * p = (byte *)(void *)Scan0;
				byte * pSrca = (byte *)(void *)SrcaScan0;
				byte * pSrcb = (byte *)(void *)SrcbScan0;
        
				int nOffset = (bmData.Stride  - w*4);

				p+=x*4+(y*nOffset);
				pSrca+=x*4+(y*nOffset);
				pSrcb+=x*4+(y*nOffset);

		
				for(int yy=0;yy < h;++yy)
				{
					for(int xx=0; xx < w; ++xx )
					{   
						int a =alpha[tt++];
						int inv_a = 255-a;

						// abg
						*p++  = (byte) ((((*pSrca++)*a)>>8) + (((*pSrcb++)*inv_a) >>8)); 
						*p++  = (byte) ((((*pSrca++)*a)>>8) + (((*pSrcb++)*inv_a) >>8)); 
						*p++  = (byte) ((((*pSrca++)*a)>>8) + (((*pSrcb++)*inv_a) >>8)); 
						*p++  = 255;

						++pSrca;++pSrcb;
					}
					p += nOffset;
					pSrca +=nOffset;
					pSrcb +=nOffset;
					if (w<alpha_width) 
					{
						tt+= alpha_width-w;
					}
				}
			}
  
		}
	}
}
