using System;
using System.Drawing;
using System.Drawing.Imaging;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for LinearBlur.
	/// </summary>
	public class VGBlur
	{
		public int m_width;
	
		public VGBlur()
		{
		}


		unsafe public byte get_p(byte* p,int nOffset, int x, int y, int z)
		{	
			if (x<0) x=0;
			if (x>=m_width) x=m_width-1;

			if (y<0) y=0;
		
			//	byte * p = (byte *)(void *)d.Scan0;
			//	int nOffset = (d.Stride);

			byte* pp = p+(((x*4)+(y*nOffset))+z);
			return *pp;
		}
		
		unsafe public void set_p(byte* p, int nOffset, int x, int y, int z, int val)
		{
			//	if (x<0) x=0;
			//	if (x>719) x=719;

			//	if (y<0) y=0;
			//	if (y>575) y=575;

			//	byte * p = (byte *)(void *)d.Scan0;
			//	int nOffset = (d.Stride);

			byte* pp = p+(((x*4)+(y*nOffset))+z);
			*pp = (byte)val;
		}




		public CImage Process(CImage image, float delta)
		{	
		
		
			//Category:"FM Tutorial"
			//Title:   "Box Blur 2"
			//Author:  "Ilyich the Toad; Alex Hunter"

			//ctl(0):"V Blur (pixels)",val=10,range=(0,scaleFactor*Y/2)
			//ctl(1):"H Blur (pixels)",val=10,range=(0,scaleFactor*X/2)

			//ForEveryTile:{

			// Compute rounded, scaled blur radius.
			//	int vBlurRadius = (2*ctl(0)+scaleFactor)/(2*scaleFactor);  //Vertical blur radius
			//	int hBlurRadius = (2*ctl(1)+scaleFactor)/(2*scaleFactor);  //Horizontal blur radius



			// If unscaled radius was nonzero, force scaled radius
			// to be nonzero also, to show some effect in preview.
			//if (vBlurRadius == 0 && ctl(0) > 0) vBlurRadius = 1;
			//	if (hBlurRadius == 0 && ctl(1) > 0) hBlurRadius = 1;

			Bitmap s1 = (Bitmap) image.GetRawImage();

			m_width = s1.Width;

			delta = 1 - delta;
			
			float d1 =delta;

			float inv_d1 = 1.0f - d1;

			Bitmap t1 = new Bitmap(s1.Width,s1.Height,PixelFormat.Format32bppArgb);
			Bitmap p1 = new Bitmap(s1.Width,s1.Height,PixelFormat.Format32bppArgb);


			unsafe
			{

				BitmapData src = s1.LockBits(new Rectangle(0, 0, s1.Width, s1.Height),
					ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

				BitmapData temp = t1.LockBits(new Rectangle(0, 0, s1.Width, s1.Height),
					ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

				BitmapData dest = p1.LockBits(new Rectangle(0, 0, s1.Width, s1.Height),
					ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);


				int offset_stride = dest.Stride - (s1.Width*4);
				int dest_stride = dest.Stride;
				int temp_stride = temp.Stride;
				int src_stride = src.Stride;

				uint * pq = (uint *)(void *)dest.Scan0;

				byte * pqsrc = (byte *)(void *)src.Scan0;
				byte * pqtemp = (byte *)(void *)temp.Scan0;
				byte * pqdest = (byte *)(void *)dest.Scan0;

				for (int xx=0;xx<s1.Width;xx++)
				{
					for (int yy=0;yy<s1.Height;yy++)
					{
						*pq++ = 0xffffffff ;
					}
					pq+=offset_stride;
				}

				int x_end = s1.Width;
				int x_start = 0;
				int y_start = 0;
				int y_end = s1.Height;

				int Z=3;

		
				double omiga = 9.0;

				double one_over_sqr_2pi_omega = 1/ System.Math.Sqrt(System.Math.PI *2*omiga);

		
			//	int vWeight = vBlurRadius*2 + 1;
			
				int total = (x_end-x_start) + (y_end-y_start);
				int x, y, z;

				for (y=y_start; y < y_end; y++)
				{
					for (z=0; z < Z; z++) 
					{
						float a = (float)get_p(pqsrc,src_stride,(x_end/2)+1, y, z);
						for (x=(x_end/2); x >= x_start; x--) 
						{
							float b = (float)get_p(pqsrc,src_stride,x, y, z);

							float aa =  ((inv_d1*a) + (d1*b));
							if (aa<0) aa =0;
							if (aa>255) aa = 255;

							set_p(pqsrc, src_stride,x,y,z,(byte)aa);
							a = (byte) aa;
							set_p(pqdest, dest_stride,x,y,z,(byte)aa);
						}

						for (x=(x_end/2)+1; x < x_end; x++) 
						{
					
							a = (float)get_p(pqsrc,src_stride,x-1, y, z);
							float b = (float)get_p(pqsrc,src_stride,x, y, z);

							float aa =  ((inv_d1*a) + (d1*b));
							if (aa<0) aa =0;
							if (aa>255) aa = 255;

							set_p(pqsrc, src_stride,x,y,z,(byte)aa);
							set_p(pqdest, dest_stride,x,y,z,(byte)aa);
							
						}
					}
				}
			

				//				updateProgress(0,0);
				s1.UnlockBits(src); 
				t1.UnlockBits(temp); 
				p1.UnlockBits(dest); 


			}

	

			return new CImage(p1);  //Done!
		}
	}
}
