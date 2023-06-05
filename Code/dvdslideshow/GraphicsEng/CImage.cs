using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;
using System.IO;
using ManagedCore;

namespace DVDSlideshow.GraphicsEng
{
	/// <summary>
	/// Summary description for CImage.
	/// </summary>
    public class CImage : DrawableObject
	{
        public enum DrawImageWithAspectType
        {
            Stretch,
            KeepAspectByAddingPillarBox,  // legacy used by slides not decorations
            KeepAspectByClipping,
            KeepAspectByChangingCoverageArea
        }

		private Image		mRawImage;
		private RectangleF	mRectangle ;		// the viewable area of the raw image
		private string		mImageFileName = "";
        private DrawImageWithAspectType mDrawImageAspectType = DrawImageWithAspectType.Stretch;
		private RotateFlipType	mRotateFlipType = RotateFlipType.RotateNoneFlipNone;
		private bool mNoBlankArea = false;
        private CImage mBackgroundImage;
        private bool mBlackAndWhite = false;
        private float [] mBrightness = {1,1,1};
        private float [] mContrast= {1,1,1};
        private float mAspect =-1;
        private Size mMaxSizeImageAfterCropAndBlankArea;   // used so we can increase resolution if needed
        private Size mMaxSizeImageFromFile;
        private RectangleF mCrop = new RectangleF(0,0,1,1);
        private bool mImageIsUsedAsMask = false;


        public string ImageFilename
        {
            get { return mImageFileName; }
            set { mImageFileName = value; }
        }

        public bool ImageIsUsedAsMask
        {
            get { return mImageIsUsedAsMask; }
            set { mImageIsUsedAsMask = value; }
        }

        public RectangleF Crop
        {
            get { return mCrop; }
            set 
            { 
                mCrop = value;
                CalcFileImageAspectAndMaxImageSize();
                CImagesCache.GetInstance().ClearCache();
            }
        }

        // ok 2560 x 1440 x 4
        // this matches max size of group decoration width at 2560
        private const float mMaxSizeImageInBytes = (2560 * 1440 * 4);

        public Size MaxSizeImageFromFile
        {
            get { return mMaxSizeImageFromFile; }
        }

        public float Aspect
        {
            get
            {
                if (mAspect == -1)
                {
                    CalcFileImageAspectAndMaxImageSize();
                }

                return mAspect;
            }
        }
      
        public bool BlackAndWhite
        {
            get { return mBlackAndWhite; }
            set { mBlackAndWhite = value; }
        }

        public float[] Brightness
        {
            get { return mBrightness; }
            set { mBrightness = value; }
        }

        public float[] Contrast
        {
            get { return mContrast; }
            set { mContrast = value; }
        }

        private IImageCreator mImageCreator = null;

        public DrawImageWithAspectType DrawImageWithAspect
        {
            get { return mDrawImageAspectType; }
            set { mDrawImageAspectType = value; }
        }

        public bool NoBlankArea
        {
            get { return mNoBlankArea; }
            set { mNoBlankArea = value; }
        }

        public CImage BackgroundImage
        {
            set { mBackgroundImage = value; }
        }

		public RotateFlipType RotateFlipType
		{
			get
			{
				return mRotateFlipType;
			}
			set
			{
				mRotateFlipType= value;
			}	
		}

		public CImage()
		{
		}

		//*******************************************************************
		public CImage(Image image)
		{
			if (image==null)
			{
				CDebugLog.GetInstance().Error("Tried to created CImage with a null raw image");
			}

			mRawImage=image;
			mImageFileName="";
			mRectangle.X = 0;
			mRectangle.Y = 0;
			int w1 = image.Width;
			mRectangle.Width = w1;
			mRectangle.Height = image.Height;
		}

        //*******************************************************************
        public CImage(IImageCreator creator, int desiredWith, int desiredHeight)
        {
            mImageCreator = creator;
            mRectangle = new RectangleF(0, 0, desiredWith, desiredHeight);
            mNoBlankArea = true;
            mImageFileName = "";
        }
		
		//*******************************************************************
		public CImage(string file)
		{
			Init(file,true, System.Drawing.RotateFlipType.RotateNoneFlipNone, 1.0f);
		}

		//*******************************************************************
		public CImage(string file, bool resize_to_dvd)
		{
			Init(file,resize_to_dvd, System.Drawing.RotateFlipType.RotateNoneFlipNone, 1.0f);
		}
  
		//*******************************************************************
		public CImage(string file, bool resize_to_dvd, System.Drawing.RotateFlipType rotate_type)
		{
			Init(file,resize_to_dvd, rotate_type, 1.0f);
		}

		//*******************************************************************
		public CImage(string file, bool resize_to_dvd, System.Drawing.RotateFlipType rotate_type, float req_zoom)
		{
			Init(file,resize_to_dvd, rotate_type, req_zoom);
		}

        //*******************************************************************
        public CImage Clone()
        {
            // SRG FIX ME, think this ant right.... obviosly the comment bit was before
            // the image chaching.  The copy constructor does a shallow copy.  
            CImage i = new CImage((Image)this.GetRawImage().Clone());
            //	i.mRawImage=(Image)mRawImage.Clone();
            return i;
        }

        //*******************************************************************
        public void DisposeRawImage()
        {
            if (mRawImage != null)
            {
                mRawImage.Dispose();
                mRawImage = null;
            }
        }

        //*******************************************************************
        public static FileStream GenerateFileStreamForImage(string imageFilename)
        {
            imageFilename = CImage.GetRealFilenameFromTemplateFilename(imageFilename);
            imageFilename = CGlobals.GetFullPathFilename(imageFilename);

            bool downloadIfMissing = CGlobals.mVideoGenerateMode == VideoGenerateMode.EDIT_PRIVIEW_MODE;

            string pfilename = ManagedCore.CryptoFS.GetNonCryptoFilename(imageFilename, downloadIfMissing);
            if (pfilename=="")
            {
                throw new Exception("No phiscal image file to load from '" + imageFilename + "'"); 
            }

            FileStream fs = new FileStream(pfilename, FileMode.Open, FileAccess.Read);

            return fs;
        }
    
        //*******************************************************************
        public static bool VerifyMediaExistsForFilename(string imageFilename)
        {
            imageFilename = CImage.GetRealFilenameFromTemplateFilename(imageFilename);
            imageFilename = CGlobals.GetFullPathFilename(imageFilename);

            bool downloadIfMissing = CGlobals.mVideoGenerateMode == VideoGenerateMode.EDIT_PRIVIEW_MODE;

            // This will download/decrypt as needed
            string pfilename = ManagedCore.CryptoFS.GetNonCryptoFilename(imageFilename, downloadIfMissing);
            if (pfilename == "")
            {
                return false;
            }
            return true;
        }

        //*******************************************************************
        private void CalcFileImageAspectAndMaxImageSize()
        {
            if (mImageFileName != "")
            {
                try
                {
                    FileStream fs = GenerateFileStreamForImage(mImageFileName);
                     
                    Image ii = Image.FromStream(fs, true, false);
                    mMaxSizeImageFromFile = new Size(ii.Width, ii.Height);
                    RectangleF croprec = CImagesCache.CalcCropRec(ii, mCrop);

                    Rectangle i = new Rectangle((int)croprec.X, (int)croprec.Y, (int)croprec.Width, (int)croprec.Height);

                    mAspect = ((float)i.Width) / ((float)i.Height);

                    if (mDrawImageAspectType != DrawImageWithAspectType.KeepAspectByAddingPillarBox)
                    {
                        mMaxSizeImageAfterCropAndBlankArea.Width = i.Width;
                        mMaxSizeImageAfterCropAndBlankArea.Height = i.Height;
                    }
                    else
                    {
                        float file_aspect = ((float)i.Width) / ((float)i.Height);
                        float canvis_aspect = 1.0f / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

                        if (file_aspect < canvis_aspect)
                        {
                            mMaxSizeImageAfterCropAndBlankArea.Width = (int)((((float)i.Height) * canvis_aspect) + 0.499f);
                            mMaxSizeImageAfterCropAndBlankArea.Height = i.Height;
                        }
                        else if (file_aspect > canvis_aspect)
                        {
                            mMaxSizeImageAfterCropAndBlankArea.Height = (int)((((float)i.Width) / canvis_aspect) + 0.499f);
                            mMaxSizeImageAfterCropAndBlankArea.Width = i.Width;
                        }
                        else
                        {
                            mMaxSizeImageAfterCropAndBlankArea.Width = i.Width;
                            mMaxSizeImageAfterCropAndBlankArea.Height = i.Height;
                        }
                    }

                    fs.Close();
                    fs.Dispose();
                    ii.Dispose();
                }
                catch (Exception exception)
                {
                    ManagedCore.Log.Error("Exception occurred in CalcFileImageAspectAndMaxImageSize, filename '" + mImageFileName +"', exception:"+exception.Message);
                    mAspect = 1;
                }
            }
            else
            {
                ManagedCore.Log.Warning("Calling CalcFileImageAspectAndMaxImageSize on a non file based CImage");
                mAspect = mRectangle.Width / mRectangle.Height;
            }
        }


        //*******************************************************************
        // Get a sub CImage from a CImage 
        // note pixel data is copied accross

        public CImage GetSubImage(Rectangle r)
        {
            Bitmap bm = GetRawImage() as Bitmap;
            
            Bitmap new_bitmap = new Bitmap(r.Width,r.Height, bm.PixelFormat);

            Graphics gp = Graphics.FromImage(new_bitmap);
            CGlobals.SetGPQuality(gp);

            gp.DrawImage(bm,
                        new Rectangle(0,0,r.Width,r.Height),
                        r,
                        GraphicsUnit.Pixel);

            gp.Dispose();

            return new CImage(new_bitmap);
        }

        //*******************************************************************
        private float[][] Mult3x3Matrix(float[][] a, float[][] b)
        {
            float[][] result ={ new float[] {0,0,0}, new float[] {0,0,0}, new float[] {0,0,0} };

            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    for(int k = 0; k < 3; k++)
                    {
                        result[i][j] +=  a[i][k] *  b[k][j];
                    }
                }
            }

            return result;
        }

        //*******************************************************************
        private float[][] GetColorMatrix()
        {
            if (mBlackAndWhite == true)
            {
                float[][] bw ={
                    new float[] {0.299f, 0.299f, 0.299f}, // scale red
                    new float[] {0.587f, 0.587f, 0.587f}, // scale green
                    new float[] {0.114f, 0.114f, 0.114f} }; // scale blue

                float [][] contr ={
                    new float[] {mContrast[0], 0, 0}, // scale red
                    new float[] {0, mContrast[1], 0}, // scale green
                    new float[] {0, 0, mContrast[2]} }; // scale blue

                // apply brightness/contrast levels to black and white matrix
                float[][] result = Mult3x3Matrix(contr, bw);

                float[][] ptsArray ={
                    new float[] {result[0][0], result[0][1], result[0][2], 0, 0}, // scale red
                    new float[] {result[1][0], result[1][1], result[1][2], 0, 0}, // scale green
                    new float[] {result[2][0], result[2][1], result[2][2], 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                    new float[] {mBrightness[0] -1, mBrightness[1] -1, mBrightness[2] -1, 0, 1}};


                return ptsArray;
            }

            if (mBrightness[0] != 1 || mBrightness[1] != 1 || mBrightness[2] != 1 ||
                mContrast[0] != 1 || mContrast[1] != 1 || mContrast[2] != 1)
            {
                // create matrix that will brighten and contrast the image
                float[][] ptsArray ={
                    new float[] {mContrast[0], 0, 0, 0, 0}, // scale red
                    new float[] {0, mContrast[1], 0, 0, 0}, // scale green
                    new float[] {0, 0, mContrast[2], 0, 0}, // scale blue
                    new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                    new float[] {mBrightness[0] -1, mBrightness[1] -1, mBrightness[2] -1, 0, 1}};

                return ptsArray;
            }

            return null;
        }

        //*******************************************************************
        public SizeF CalcImageDimensionToLoad(SizeF requestedDimension, out bool usesTooMuchMemory)
        {
            // ### SRG TO DO FOR EDIT MODE SWITCH OFF FOR NOW
            if (CGlobals.mVideoGenerateMode == VideoGenerateMode.EDIT_PRIVIEW_MODE)
            {
                usesTooMuchMemory = false;
                return new SizeF(0, 0);
            }

            usesTooMuchMemory = false;

            if (mMaxSizeImageAfterCropAndBlankArea.Width == 0 && mMaxSizeImageAfterCropAndBlankArea.Height == 0)
            {
                // image not loaded yet, do a load now
                if (mImageFileName != "" && VerifyMediaExistsForFilename(mImageFileName) == true)
                {
                    CalcFileImageAspectAndMaxImageSize();
                }
                else
                {
                    mAspect = 1;
                }

            }

            // ok loaded 
            if (mMaxSizeImageAfterCropAndBlankArea.Width > 0 && mMaxSizeImageAfterCropAndBlankArea.Height > 0)
            {
                // ok make sure we don't request anything bigger than it is on disc
                if (requestedDimension.Width > mMaxSizeImageAfterCropAndBlankArea.Width ||
                     requestedDimension.Height > mMaxSizeImageAfterCropAndBlankArea.Height)
                {
                    requestedDimension = mMaxSizeImageAfterCropAndBlankArea;
                }
            }

            // ok ensure image not too big 
            float size = (requestedDimension.Width * requestedDimension.Height * 4);
          
            if (size > (mMaxSizeImageInBytes+4))
            {
                // ok calc max size we can do

                // x * y * 4 = MaxSizeImageFromFile

                float fr = requestedDimension.Height / requestedDimension.Width;
                
                float newWidth = (float) Math.Sqrt((mMaxSizeImageInBytes / 4) / fr);
                float newHeight = newWidth * fr;

                requestedDimension.Width = (int)(newWidth+0.4999);
                requestedDimension.Height = (int)(newHeight + 0.4999); 
                usesTooMuchMemory = true;
            }        

            return requestedDimension;
        }


        //*******************************************************************
        public Image GetRawImage()
        {
            return GetRawImage(new Size());
        }

        //*******************************************************************
        public Image GetRawImage(SizeF requestedDimension)
        {
            bool needsDisposing; // this call will always return false
            return GetRawImage(requestedDimension, true, null, false, out needsDisposing);
        }

		//*******************************************************************
        public Image GetRawImage(SizeF requestedDimension, bool increase20percent, Image useImageInsteadOfFilename, bool doNotCache, out bool imageNeedsDisposing)
		{
            imageNeedsDisposing = false;

            // CImage was created with a raw bitmap, return that
            if (mRawImage != null)
            {
                return mRawImage;
            }

            // CImage was created with a handle to an image creator, generate the image using that
            if (mImageCreator != null)
            {
                return mImageCreator.CreateImage((int)mRectangle.Width, (int)mRectangle.Height, ref mCreateMipMaps);
            }

     
            // If CImage does not have filename defined, then return null
            if (mImageFileName == "")
            {
                return null;
            }

            // Ok load in the image from a file increase request size a little bit

            SizeF candRequest = requestedDimension;

            if (increase20percent == true)
            {
                candRequest.Width = candRequest.Width * 1.2f;
                candRequest.Height = candRequest.Height * 1.2f;
            }

            bool usesTooMuchMemory = false;

            SizeF dimensionToLoadSize = CalcImageDimensionToLoad(candRequest, out usesTooMuchMemory);

			CImagesCache s = CImagesCache.GetInstance();

            float[][] colorMatrix = GetColorMatrix();

            Image  i = s.GetFileImage(mImageFileName,
                                      this.mDrawImageAspectType,
                                      dimensionToLoadSize,
                                      mNoBlankArea,
                                      mRotateFlipType,
                                      ref mAspect,
                                      ref mMaxSizeImageAfterCropAndBlankArea,
                                      mBackgroundImage,
                                      colorMatrix,
                                      useImageInsteadOfFilename,
                                      doNotCache,
                                      out imageNeedsDisposing,
                                      mCrop,
                                      mImageIsUsedAsMask);

			return i;

		}

		public bool ThumbnailCallback()
		{
			return false;
		}

		//*******************************************************************
		public Image GetThumbnailImage(int width, int height)
		{
            Image i = GetRawImage(new Size(width, height));

			System.Drawing.Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
			Image it = i.GetThumbnailImage(width,height,myCallback, IntPtr.Zero);
			return it;

		}

		//*******************************************************************
		public RectangleF GetRectangle()
		{
            // already defined
            if (mImageCreator != null)
            {
                return mRectangle;
            }

            GraphicsEngine engine = GraphicsEngine.Current;
            return engine.GetRawImageDimension(this);
		}

     

		//*******************************************************************
		public void Init(string file, bool resize_to_dvd, System.Drawing.RotateFlipType rotate_type, float max_zoom)
		{
            mImageFileName = file;
          
			mRotateFlipType = rotate_type;

            if (resize_to_dvd == true)
            {
                this.mDrawImageAspectType = DrawImageWithAspectType.KeepAspectByAddingPillarBox;
            }

			return ;
		}
 
		//*******************************************************************
		public CImage(CImage copy)
		{
		//	mRawImage = copy.mRawImage;	// (not hard copy)
			mImageFileName = copy.mImageFileName;
			if (mImageFileName=="")
			{
				mRawImage = copy.mRawImage;
			}

			mRectangle = copy.mRectangle ;
            mDrawImageAspectType = copy.mDrawImageAspectType;
			mRotateFlipType = copy.mRotateFlipType;
			mNoBlankArea = copy.mNoBlankArea;
		}

		//*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc, string name)
		{
			XmlElement image = doc.CreateElement(name);

            // Ok if this is a photovidshow file, i.e. store in sub folder of PhotoVidShow.
            // Then strip header
            string filename = CGlobals.StripRootHeader(this.mImageFileName);

            image.SetAttribute("Filename", filename);

            if (mNoBlankArea == true)
            {
                image.SetAttribute("NoBlankAera", this.mNoBlankArea.ToString());
            }

            if (mCrop.X !=0 || mCrop.Y !=0 || mCrop.Width !=1 || mCrop.Height !=1)
            {
                CGlobals.SaveRectangle(image, doc, "Crop", mCrop);
            }

			parent.AppendChild(image); 
		}

        //*******************************************************************
        protected static string GetRealFilenameFromTemplateFilename(string filename)
        {
            if (filename.ToLower().StartsWith("image") == true)
            {
                string ss = filename.Substring(5);
                int number = 1;
                ss = ss.Replace(".jpg", "");
                try
                {
                    number = int.Parse(ss);
                }
                catch
                {
                    return filename;
                }

                filename = CGlobals.GetTemplateDirectory() + "\\media\\blank" + number.ToString() + ".jpg";
            }
  
            return filename;
        }

        //*******************************************************************
        public static bool IsTemplateFilename(string filename)
        {
            string s = GetRealFilenameFromTemplateFilename(filename);
            if (s != filename) return true;
            return false;
        }

		//*******************************************************************
		public void Load(XmlElement element, bool resize_to_dvd)
		{
			string filename = element.GetAttribute("Filename");

            string realFile = GetRealFilenameFromTemplateFilename(filename);
            if (realFile == filename)
            {
                string fullFilename = CGlobals.GetFullPathFilename(filename);
                filename = CGlobals.CheckFileExistsElseThrow(fullFilename);
            }

			Init(filename, resize_to_dvd, mRotateFlipType, 1.0f);

			string s = element.GetAttribute("NoBlankAera");
			if (s!="")
			{
				mNoBlankArea = bool.Parse(s);
			}

            XmlNodeList list = element.GetElementsByTagName("Crop");
            if (list.Count > 0)
            {
                mCrop = CGlobals.LoadRectangle(element, "Crop");
            }
		}
	}
}
