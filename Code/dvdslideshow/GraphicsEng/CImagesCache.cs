using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using ManagedCore;

namespace DVDSlideshow.GraphicsEng
{
    //
	// Cache's x meg of images in ram else loads from disc (returns null if memory created image)
	// when ram limit reached, least active image is removed.
    //
	public class CImagesCache
	{
        private static RectangleF mBlankCrop = new RectangleF(0, 0, 1, 1);
        private static CImagesCache mTheCacheImageDatabase = null;

		private Object thisLock = new Object();
		private	Hashtable mCachedImages;
		private	int	mCounter =0;
        private int mUsage = 0;
        private int mMaxSize = 25 * 1024 * 1024;

		public event ManagedCore.StateChangeHandler StateChanged;

        public Hashtable CachedImages
        {
            get
            {
                return mCachedImages;
            }
        }

        public int Usage
        {
            get
            {
                return mUsage;
            }
        }

        public int MaxSize
        {
            get
            {
                return mMaxSize;
            }
        }

		//*******************************************************************
		public static CImagesCache GetInstance()
		{
			if (mTheCacheImageDatabase==null)
			{
				mTheCacheImageDatabase = new CImagesCache();
			}
			return mTheCacheImageDatabase;
		}

        //*******************************************************************
        public CImagesCache()
        {
            mCachedImages = new Hashtable();
        }

		//*******************************************************************
		public void ClearCache()
		{
			lock (this.thisLock)
			{
				mCachedImages.Clear();
				mUsage = 0;
                if (StateChanged != null)
                {
                    StateChanged();
                }
			}
		}
     
        //*******************************************************************
        private string GenerateHashString(
            string filename,
            System.Drawing.RotateFlipType rotate_flip,
            bool no_blank_area,
            CImage backgroundImage,
            float[][] colorMatrix,
            CImage.DrawImageWithAspectType aspectType,
            Size iRequestedLoadSize,
            RectangleF crop,
            bool ensureMaskColours)
        {
			string nbs = " BA ";
			if (no_blank_area==true)
			{
				nbs = " NB ";
			}

            string backimage_name = "";
            if (backgroundImage != null)
            {
                backimage_name = System.IO.Path.GetFileName(backgroundImage.ImageFilename);
            }

            StringBuilder builder = new StringBuilder(filename);
            builder.Append(rotate_flip.ToString());
            builder.Append(nbs);
            builder.Append(iRequestedLoadSize.Width);
            builder.Append("x");
            builder.Append(iRequestedLoadSize.Height);
            builder.Append(" ");
            builder.Append(backimage_name);

            if (colorMatrix != null)
            {         
                for (int matrixHIndex = 0; matrixHIndex < 5; matrixHIndex++)
                {
                    for (int matrixWIndex = 0; matrixWIndex < 5; matrixWIndex++)
                    {
                        builder.Append(colorMatrix[matrixHIndex][matrixWIndex]);
                        builder.Append(",");
                    }
                }
            }

            builder.Append(aspectType.ToString());
            if (crop != mBlankCrop)
            {
                builder.Append("Cr");
                builder.Append(crop.X);
                builder.Append(",");
                builder.Append(crop.Y);
                builder.Append(",");
                builder.Append(crop.Width);
                builder.Append(",");
                builder.Append(crop.Height);
            }

            if (ensureMaskColours == true)
            {
                builder.Append(" mask");
            }

            string key_name = builder.ToString();

            return key_name;
        }

		//*******************************************************************
		// Returs cached image else loads it from disc
        public Image GetFileImage(string filename,
                                  CImage.DrawImageWithAspectType aspectType,
                                  SizeF requestedLoadSize,
                                  bool no_blank_area, 
                                  System.Drawing.RotateFlipType rotate_flip,
                                  ref float aspect,
                                  ref Size maxSizeOfImageFromFile,
                                  CImage backgroundImage,
                                  float[][] colorMatrix,
                                  Image useImageInsteadOfFilename,
                                  bool doNotCache,
                                  out bool imageNeedsDisposing,
                                  RectangleF crop,
                                  bool ensureMaskColours)
		{ 
			lock (this.thisLock)
			{
                imageNeedsDisposing = false;
                Size iRequestedLoadSize = new Size( (int) (requestedLoadSize.Width+0.4999f), (int) (requestedLoadSize.Height+0.4999f));

                ImageAttributes imageAttributes = null;
                if (colorMatrix != null)
                {
                    imageAttributes = new ImageAttributes();
                    imageAttributes.ClearColorMatrix();
                    imageAttributes.SetColorMatrix(new ColorMatrix(colorMatrix), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                }

                string key_name = GenerateHashString(filename, rotate_flip, no_blank_area, backgroundImage, colorMatrix, aspectType, iRequestedLoadSize, crop, ensureMaskColours);

				CMemoryCachedImage i = mCachedImages[key_name] as CMemoryCachedImage;
				if (i!=null)
				{
					i.mCounter = mCounter++;
                    maxSizeOfImageFromFile = i.mMaxSizeImageFromFile;
                    aspect = i.mAspect;
                    return i.GetImage();
				}

				//	Console.WriteLine("Did not find Cached image for "+filename+". loading from disk");

                CMemoryCachedImage ici = LoadImageFromDisk(filename, key_name, aspectType, iRequestedLoadSize, no_blank_area, rotate_flip, backgroundImage, imageAttributes, useImageInsteadOfFilename, doNotCache, crop);

                if (ici == null)
                {
                    return null;
                }

                //
                // Only set these if we loaded from a file
                //
                if (useImageInsteadOfFilename != null)
                {
                    aspect = ici.mAspect;
                    maxSizeOfImageFromFile = ici.mMaxSizeImageFromFile;
                }

                if (doNotCache)
                {
                    imageNeedsDisposing = true;
                }

                Image ii = ici.GetImage();

                //
                // When loading a mask png file using .net sometimes (e.g. windows xp) it thinks pure red is 254 not 255.
                // If this a mask image make sure image uses mask colours
                //
                if (ii!=null && ensureMaskColours == true)
                {
                    BitmapUtils.EnsureMaskColours(ii);
                }

                return ii;
			}
		}

		//*******************************************************************
		// returns null if image not cached
		private Image GetImage(string name)
		{
			lock (this.thisLock)
			{
				CMemoryCachedImage i = mCachedImages[name] as CMemoryCachedImage;
				if (i!=null)
				{
					i.mCounter = mCounter++;

					//Console.WriteLine("Found Cached image for "+name);
                    return i.GetImage(); ;
				}
			}

			return null;
		}

		//*******************************************************************
		private CMemoryCachedImage LoadImageFromDisk(string filename, 
                                                     string key_name,
                                                     CImage.DrawImageWithAspectType aspectType,
                                                     Size requestedLoadSize,
                                                     bool no_blank_area,
                                                     System.Drawing.RotateFlipType rotate_flip,
                                                     CImage backgroundImage,
                                                     ImageAttributes imageAttributes,
                                                     Image useImageInsteadOfFilename,
                                                     bool doNotCache,
                                                     RectangleF crop)
		{
            //
            // Default image dimension is the size of the canvis screen
            //
			int cw = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
			int ch = CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;

			CMemoryCachedImage memCachedImage = new CMemoryCachedImage();
            ImageFormat originalFormat = ImageFormat.Jpeg;
            FileStream fs = null;

            Image newImage = null;

            try
            {
                if (useImageInsteadOfFilename != null)
                {
                    newImage = useImageInsteadOfFilename.Clone() as Bitmap;
                    originalFormat = ImageFormat.Bmp; ;
                    newImage = CropImage(newImage, crop);
                }
                else
                {
                    fs = CImage.GenerateFileStreamForImage(filename);
                    Image iii = Image.FromStream(fs, true, false);

                    //
                    // If not jpg, then basically we say the original format is a 32 bitmap with possible alpha
                    //
                    if (filename.ToLower().EndsWith(".jpg") == false &&
                        filename.ToLower().EndsWith(".jpeg") == false)
                    {
                        originalFormat = ImageFormat.Bmp;
                    }

                    newImage = CropImage(iii, crop);
                }

                memCachedImage.mMaxSizeImageFromFile.Width = newImage.Width;
                memCachedImage.mMaxSizeImageFromFile.Height = newImage.Height;
                memCachedImage.mAspect = ((float)newImage.Width) / ((float)newImage.Height);
                
                if (requestedLoadSize.Width > 0 && requestedLoadSize.Height > 0)
                {
                    cw = requestedLoadSize.Width;
                    ch = requestedLoadSize.Height;
                }

                // ManagedCore.Log.Info("Loading " + filename + "+ at resolution " + cw+","+ch);

                //
                // Clip art
                //
                if (aspectType == CImage.DrawImageWithAspectType.Stretch ||
                    aspectType == CImage.DrawImageWithAspectType.KeepAspectByClipping)
                {
                    int width = requestedLoadSize.Width;
                    int height = requestedLoadSize.Height;

                    //
                    // This means we're in edit mode, do old method for now (which is generaly good enough)
                    //
                    if (requestedLoadSize.Width == 0 || requestedLoadSize.Height == 0)
                    {
                        int max = cw; // ### SRG FIX ME, for texture effects we just set same as cw

                        width = newImage.Width;
                        height = newImage.Height;

                        //
                        // Determine the size for the thumbnail image
                        //
                        if (newImage.Width > max || newImage.Height > max)
                        {
                            if (newImage.Width > newImage.Height)
                            {
                                width = max;
                                height = (int)(newImage.Height * max / newImage.Width);
                            }
                            else
                            {
                                width = (int)(newImage.Width * max / newImage.Height);
                                height = max;
                            }
                        }
                    }

                    if (imageAttributes == null)
                    {
                        //
                        // If a mask image then we don't want to do bilinear filtering
                        //
                        if (filename.Contains("overlays\\masks") == true)
                        {
                            newImage = ReScaleImageWithoutBilinearFiltering(newImage, width, height);
                        }
                        else
                        {
                            Image oldImage = newImage;
                            newImage = new Bitmap(oldImage, width, height);
                            oldImage.Dispose();
                        }
                    }
                    else
                    {
                        Bitmap copiedImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                        //
                        // If applying imagea ttributes, it's a lot quicker to downsize then apply the effect.
                        //
                        if (imageAttributes != null)
                        {
                            Image memcached = newImage;
                            newImage = new Bitmap(newImage, new Size(width, height));
                            memcached.Dispose();
                        }

                        using (Graphics g = Graphics.FromImage(copiedImage))
                        {
                            g.DrawImage(newImage, new Rectangle(0, 0, width, height), 0, 0, newImage.Width, newImage.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        Image b = newImage;
                        newImage = copiedImage;
                        b.Dispose();
                    }

                    if (rotate_flip != RotateFlipType.RotateNoneFlipNone)
                    {
                        newImage.RotateFlip(rotate_flip);
                    }

                    width = newImage.Width;
                    height = newImage.Height;
                }

                //
                // a slide image  THIS IMAGE WILL MATCH GLOBAL CANVIS ASPECT (with possible pillar boxes )
                //
                else
                {
                    //
                    // Currently does not support wmf files
                    //
                    try
                    {
                        if (rotate_flip != RotateFlipType.RotateNoneFlipNone)
                        {
                            newImage.RotateFlip(rotate_flip);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Invalid rotate on image, was it a wmf? " + exception.Message);
                    }

                    //
                    // SRG reduce for preview speed up.
                    // Normally smallest 1.25 ratio box that fits compelte image
                    //
                    int ww = cw;
                    int hh = ch;

                    Image oThumbNail = new Bitmap(ww, hh, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                    //
                    // Draw background image (covers the black area)
                    //
                    if (backgroundImage != null)
                    {
                        Image backgroundRawImage = backgroundImage.GetRawImage();
                        using (Graphics gg = Graphics.FromImage(oThumbNail))
                        {
                            gg.DrawImage(backgroundRawImage, new Rectangle(0, 0, ww, hh), 0, 0, backgroundRawImage.Width, backgroundRawImage.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                    }

                    Rectangle r1 = new Rectangle(0, 0, ww, hh);
                    Rectangle oRectangle = new Rectangle(0, 0, newImage.Width, newImage.Height);

                    Rectangle r = CGlobals.CalcBestFitRectagle(r1, oRectangle);
                    if (no_blank_area == true)
                    {
                        r = CGlobals.CalcBestFitRectagleNoBlank(r1, oRectangle);
                    }

                    using (Graphics gp = Graphics.FromImage(oThumbNail))
                    {
                        //
                        // Draw the base image
                        //
                        gp.DrawImage(newImage, r,
                            0, 0, newImage.Width, newImage.Height,
                            GraphicsUnit.Pixel, imageAttributes);

                        newImage.Dispose();
                        newImage = oThumbNail;
                    }

                    float file_aspect = ((float)memCachedImage.mMaxSizeImageFromFile.Width) / ((float)memCachedImage.mMaxSizeImageFromFile.Height);
                    float canvis_aspect = 1.0f / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

                    if (file_aspect < canvis_aspect)
                    {
                        memCachedImage.mMaxSizeImageFromFile.Width = (int)((((float)memCachedImage.mMaxSizeImageFromFile.Height) * canvis_aspect) + 0.499f);
                    }
                    else if (file_aspect > canvis_aspect)
                    {
                        memCachedImage.mMaxSizeImageFromFile.Height = (int)((((float)memCachedImage.mMaxSizeImageFromFile.Width) / canvis_aspect) + 0.499f);
                    }
                }

                memCachedImage.mName = key_name;
                memCachedImage.SetImage(newImage, originalFormat);

                if (doNotCache == false)
                {
                    memCachedImage.mCounter = mCounter++;

                    try
                    {
                        mCachedImages.Add(key_name, memCachedImage);
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Exception occurred in LoadImageFromDisk. Filename '" + filename+ "', exception:" + exception.Message);
                    }

                    mUsage += (int)memCachedImage.GetSizeInBytes();

                    CheckCacheSize();
                }

                if (StateChanged != null)
                {
                    StateChanged();
                }
            }
            catch (Exception exception)
            {
                //
                // If exception was 'Could not find file', then do a popup window informing the user.
                // This can happen if the user deleted the file whist PhotoVidShow was running and if it was already loaded in as a slide.
                //
                if (exception.Message.StartsWith("Could not find file") == true)
                {
                    UserMessage.Show(exception.Message, "Could not find file", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                } 
                Log.Error("Failed to load image '" + filename + "', exception:" + exception.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
     
			return memCachedImage;
		}

        //*******************************************************************
        private Image ReScaleImageWithoutBilinearFiltering(Image input, int width, int height)
        {
            Bitmap b = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(input,  new Rectangle(0, 0, width, height), new Rectangle(0, 0, input.Width, input.Height), GraphicsUnit.Pixel);
            }

            input.Dispose();

            return b;
        }

		//*******************************************************************
		private void RemoveCachedImage(CMemoryCachedImage i)
		{
		//	Console.WriteLine("Image cache is removing image "+i.mName);
			mUsage-= (int)i.GetSizeInBytes();
			mCachedImages.Remove(i.mName);
            i.Dispose();
            if (StateChanged != null)
            {
                StateChanged();
            }
		}

		//*******************************************************************
		private void CheckCacheSize()
		{
			while (mUsage > mMaxSize)
			{
                //
				// Remove oldest image
                //
				CMemoryCachedImage oldest=null;

				IDictionaryEnumerator myEnumerator = mCachedImages.GetEnumerator();

				while ( myEnumerator.MoveNext() )
				{
					CMemoryCachedImage j = myEnumerator.Value as CMemoryCachedImage;
					if (oldest==null ||j.mCounter < oldest.mCounter)
					{
						oldest = j;
					}
				}

				if (oldest!=null)
				{
				//	Console.WriteLine("Cache to big removed "+oldest.mName);

					if (oldest.mCounter == mCounter-1)
					{
						Log.Error("Image cache to small");
						break ;
					}
					RemoveCachedImage(oldest);
				}
				else
				{
					Log.Error("No image removed from overfull cache");
					break;
				}
			}

		//	Console.WriteLine("Cache size :"+mUsage/1024/1024);
		}

        //*******************************************************************
        public static RectangleF CalcCropRec(Image i, RectangleF crop)
        {
            if (crop == mBlankCrop) return new RectangleF(0,0,i.Width, i.Height);

            float width = (float)i.Width;
            float height = (float)i.Height;

            float x = width * crop.X;
            float y = height * crop.Y;
            float w = width * crop.Width;
            float h = height * crop.Height;

            return new RectangleF(x,y,w,h);
        }

        //*******************************************************************
        private Image CropImage(Image i, RectangleF crop)
        {
            if (crop == mBlankCrop)
            {
                return i;
            }

            RectangleF cropped = CalcCropRec(i, crop);

            Bitmap b = new Bitmap((int)cropped.Width, (int)cropped.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(b))
            {           
                g.DrawImage(i, new RectangleF(0, 0, b.Width, b.Height), cropped ,GraphicsUnit.Pixel);
            }
            i.Dispose();
            return b;
        }
	}
}
