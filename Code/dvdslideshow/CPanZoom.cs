using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ManagedCore;
using System.Xml;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for CPanZoom.
	/// </summary>
	public class CPanZoom
	{
        private float mInitialDelay = 0.0f;
        private float mEndDelay = 1.0f;

        // rotation only used when converted to animateddecoration effect
        private float mStartRotation = 0;
        private float mEndRotation = 0;
        private bool mPanZoomOnAll = false; // if true, indicates that all the slide's decorations should be inside a group and we pan zoom on that group
                                            // if false, we simply "move/zoom" the first and only decoration.  Quicker way and most common 

        private bool mReGenerateOnImageChange = false;  // this is a template thing which means when we define image1 (the only image) we should re-generate a random pan zoom region
        private CEquation mMovementEquation = new CLinear();
        private CEquation mRotationEquation = new CNonLinear();

        private static Random mRandom = null ; 

        public CEquation MovementEquation
        {
            get { return mMovementEquation; }
            set
            {
                mMovementEquation = value; 

                // if spring set rotation to this too
                if (mMovementEquation is CLinear == false)
                {
                    mRotationEquation = mMovementEquation;
                }
                else
                {
                    mRotationEquation = new CNonLinear();
                } 
            }
        }

        public bool ReGenerateOnImageChange
        {
            get { return mReGenerateOnImageChange; }
            set { mReGenerateOnImageChange = true; }
        }

        public bool PanZoomOnAll
        {
            get { return mPanZoomOnAll; }
            set { mPanZoomOnAll = value; }
        }

        public float StartRotation
        {
            get { return mStartRotation; }
            set { mStartRotation = value; }
        }

        public float EndRotation
        {
            get { return mEndRotation; }
            set { mEndRotation = value; }
        }

        public float InitialDelay
        {
            get { return mInitialDelay; }
            set 
            {
                mInitialDelay = value; 
            }
        }

        public float EndDelay
        {
            get { return mEndDelay; }
            set { mEndDelay = value; }
        }


		private RectangleF mStartArea;
	

		public RectangleF StartArea
		{
			get
			{
				return mStartArea;
			}
			set
			{
				mStartArea = value ;
			}
		}

		private RectangleF mEndArea ;

		public RectangleF EndArea
		{
			get
			{
				return mEndArea;
			}
			set
			{
				mEndArea = value ;
			}
		}

		private bool mCalcedImage=false;
		

		public CPanZoom()
		{
			mStartArea = new RectangleF(0,0,1.0f,1.0f);
			mEndArea = new RectangleF(0,0,1.0f,1.0f);

			//
			// TODO: Add constructor logic here
			//
		}

		//*******************************************************************
		public CPanZoom Clone()
		{
			CPanZoom pz = new CPanZoom();
			pz.mStartArea = this.mStartArea;
			pz.mEndArea = this.mEndArea;
			mCalcedImage= true;
			return pz;
		}

		//*******************************************************************
		public void CalcBestRectangleAreas(CImage i1)
		{
			CalcBestRectangleAreas(i1,false);
		}


        //*******************************************************************
        // this private method returns true if a original 4:3 image is being
        // drawn on a 16:9 canvis.  This is very common if someone is trying
        // to take normal digital camera images and make them into a tv
        // widescreen DVD video
        private bool is4x3ImageOn16x9canvis(CImage i1)
        {
            if ( (Math.Abs((1.0f / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction()) -1.77777f)) > 0.001f)
            {
                return false;
            }

            // any image that has big black bars either side will do if canvis is 16x9
            if (i1.Aspect > 1.334 || i1.Aspect < 1.332)
            {
                return false;
            }

            return true;
        }

        private static int seedCount = 0;

        //*******************************************************************
        private int GetSeed()
        {
            // seed defined?
            if (seedCount == 0)
            {
                DateTime now = DateTime.Now;
                string s = now.ToLongDateString() + now.ToLongTimeString() + now.Millisecond.ToString();
                int hash = s.GetHashCode();
                hash = Math.Abs(hash);
                seedCount = hash;
            }
            else
            {
                seedCount++;
            }

            return seedCount;
        }

		//*******************************************************************
        // initial calculation is based on 4:3 canvis
		public void CalcBestRectangleAreas(CImage i1, bool re_randomize)
		{
			int pan_type = 0;
			int reverse = 0 ;

            if (mRandom == null)
            {
                mRandom = new Random(GetSeed());
            }

			if (mCalcedImage!=true || re_randomize==true) 
			{
                pan_type = mRandom.Next() % 4;
                reverse = mRandom.Next() % 2;
			}
			else
			{
				return ;
			}

			mCalcedImage= true;

            // As we normalise later, the width,height values aren't actually important.  But set to the canvis width
            // and height to make debug easier.
            // The aspect of the canvis is calculated a completely different way.
            float width = CGlobals.mCurrentProject.DiskPreferences.CanvasWidth;
            float height = CGlobals.mCurrentProject.DiskPreferences.CanvasHeight;

            float ff1 = ((float)mRandom.Next() % 1000) / 5000.0f;
			ff1=ff1-0.1f;

			float w_percent = width*(0.3f+ff1) ;
			float w_half_percent = w_percent/2;

			float h_percent = height*(0.3f+ff1) ;
			float h_half_percent = h_percent/2;
      
			// ok if image was taken sideways default to pan zoon type 1
			bool sideways = false;
            bool needs_normalizing = true;

            bool is43on169 = false;

            if (i1 != null)
            {
                is43on169 = is4x3ImageOn16x9canvis(i1);

                // 2 = pan down or up zoomed in a lot (used for potrait piccies)
                if ((i1.Aspect) < 0.76f &&
                    re_randomize == false && (mRandom.Next() % 2) == 0)
                {
                    pan_type = 2;
                    sideways = true;
                }
            }
 
			// zoom in and out
			if (pan_type==0 || pan_type==1)
			{
                // if 16x9 cavis with 4/3 image make it zoom less
                if (is43on169 == true)
                {
                    float ff2 = ((float)mRandom.Next() % 1000) / 10000.0f;
                    ff2 = ff2 - 0.05f;
                    w_percent = width * (0.2f + ff2);
                    w_half_percent = w_percent / 2;

                    h_percent = height * (0.2f + ff2);
                    h_half_percent = h_percent / 2;
                }

				mStartArea = new RectangleF(w_half_percent,h_half_percent,width-w_percent,height-h_percent);
				mEndArea = new RectangleF(0,0,width,height);

                // 1 in 3 zooms have some kind of rotation
                if ((mRandom.Next() % 3) == 0)
                {
                    int rot = 0;
                    while (Math.Abs(rot) <= 2)
                    {
                        rot = mRandom.Next(11) - 5;
                        this.mStartRotation = rot;
                    }
                }
			}
 
			if (!sideways)
			{
				w_percent*=0.5f;
				w_half_percent*=0.5f;
				h_percent*=0.5f;
				h_half_percent*=0.5f;
			}

        

            float aspect = 1.0f / CGlobals.mCurrentProject.DiskPreferences.GetAspectRatioFraction();

            float imageAspect = aspect;
            if (i1 != null)
            {
                imageAspect = i1.Aspect;
            }

            float canvisWidth = aspect * 900.0f;
            float imageWidth = imageAspect * 900.0f;

            float canvisHeight = 1600 / aspect;
            float imageHeight = 1600 / imageAspect;

            float a1 = (imageWidth / canvisWidth);
            float a2 = (imageHeight / canvisHeight);

			// if doing a pan choose one with less blank area
			if (pan_type==2 && a2 < a1) 
			{
				pan_type =3 ;
			}
			if (pan_type==3 && a2 > a1) 
			{
				pan_type =2 ;
			}
 
			// up and down
			if (pan_type==2)
			{
                if ((mRandom.Next() % 3) == 0 && sideways == false && a1 > 0.9 && a2 > 0.9)
				{
					mStartArea = new RectangleF(0,0,		width-w_percent,height-h_percent);
					mEndArea = new RectangleF(w_percent,h_percent,	width-w_percent,height-h_percent);
				}
				else
				{
                    // special case for 16x9 canvis
                    // if 4x3 or thinner image on 16x9 canvis reduce to 0.75 and pan up/down
                    if (is43on169 && a2 >= 0.999f && a1 < 0.751)
                    {
                        float m_x = (1.0f - 0.75f) / 2;
                        float m_w = 0.75f;
                        float m_h = 0.75f;
                        float m_y = 1.0f-0.75f;


                        mStartArea = new RectangleF(m_x, 0, m_w, m_h) ;
                        mEndArea = new RectangleF(m_x, m_y, m_w, m_h);
                        needs_normalizing = false;

                    }
                    else
                    {
                        mStartArea = new RectangleF(w_half_percent, 0, width - w_percent, height - h_percent);
                        mEndArea = new RectangleF(w_half_percent, h_percent, width - w_percent, height - h_percent);
                    }
				}
			}
			// left and right

			if(pan_type==3)
			{
                if (((mRandom.Next() % 3) == 0) && sideways == false && a1 > 0.9 && a2 > 0.9)
				{
					mEndArea = new RectangleF(0,h_percent,				width-w_percent,height-h_percent);
					mStartArea = new RectangleF(w_percent,0,	width-w_percent,height-h_percent);
				}
				else
				{
					mEndArea = new RectangleF(0,h_half_percent,				width-w_percent,height-h_percent);
					mStartArea = new RectangleF(w_percent,h_half_percent,	width-w_percent,height-h_percent);
				}
			}


			if (reverse==0)
			{
				RectangleF temp = mEndArea;
				mEndArea = mStartArea;
				mStartArea = temp ;

                float tempRot = mStartRotation;
                mStartRotation = mEndRotation;
                mEndRotation = tempRot;
			}

            if (needs_normalizing == true)
            {
                mStartArea.X /= width;
                mStartArea.Y /= height;
                mStartArea.Width /= width;
                mStartArea.Height /= height;

                mEndArea.X /= width;
                mEndArea.Y /= height;
                mEndArea.Width /= width;
                mEndArea.Height /= height;
            }


            // correction for 16:9 canvis and zoom in/out
            if (is43on169 == true && (pan_type==0 || pan_type==1))
            {
                float half_correction = ((4.0f / 3.0f) / (16.0f / 9.0f));

                mStartArea.Width *= half_correction;
                mStartArea.X = (1.0f-mStartArea.Width) / 2;
                mStartArea.Height *= half_correction;
                mStartArea.Y = (1.0f-mStartArea.Height) / 2;

                mEndArea.Width *= half_correction;
                mEndArea.X = (1.0f - mEndArea.Width) / 2;
                mEndArea.Height *= half_correction;
                mEndArea.Y = (1.0f - mEndArea.Height) / 2;

            }


			if (mStartArea.Height > 1.00001 || mStartArea.Width > 1.00001 ||
				mStartArea.X < -0.00001 || mStartArea.Y < -0.00001 ||
				mEndArea.Height > 1.00001 || mEndArea.Width > 1.00001 ||
				mEndArea.X < -0.00001 || mEndArea.Y < -0.00001 )

			{
                CDebugLog.GetInstance().Error("Pan/Zoom area invalid after call to CalcBestRectangleAreas");
			}
		}

		//*******************************************************************
		public float GetMaxZoom()
		{
			float zoom = 1.0f;

			if (mEndArea.Width < mStartArea.Width)
			{
				zoom = 1.0f/mEndArea.Width;
			}
			else
			{
				zoom = 1.0f /mStartArea.Width;
			}

      //      zoom -= 0.3f;

			if (zoom < 1.0) 
			{
				zoom = 1.0f;
			}

			return zoom;
		}


		//*******************************************************************
		public RectangleF Process(float delta, int width, int height, ref float rotation)
		{
            // adjust delta to take into account initial/end delay points
            if (mInitialDelay > 0.0001 || mEndDelay < 0.9999)
            {
                if (delta < mInitialDelay)
                {
                    delta = 0.0f;
                }
                else if (delta > mEndDelay)
                {
                    delta = 1.0f;
                }
                else
                {
                    delta -= mInitialDelay;

                    delta /= (mEndDelay - mInitialDelay);

                    // just in case
                    if (delta < 0.0f) delta = 0.0f;
                    if (delta > 1.0f) delta = 1.0f;             
                }
            }

            // just in case
            if (delta < 0.0f) delta = 0.0f;
            if (delta > 1.0f) delta = 1.0f;

            float rotateDelta = mRotationEquation.Get(0, 1, delta);

            // apply rotation
            rotation = mStartRotation + ((mEndRotation - mStartRotation) * rotateDelta);

            delta = mMovementEquation.Get(0, 1, delta);

			RectangleF sa = mStartArea;
			RectangleF ea = mEndArea;

			sa.X *= width;
			sa.Y *= height;
			sa.Width *= width;
			sa.Height *= height;

			ea.X *= width;
			ea.Y *= height;
			ea.Width *= width;
			ea.Height *= height;
	
			float x = ((((ea.X - sa.X)*delta))+sa.X);
			float y = ((((ea.Y - sa.Y)*delta))+sa.Y);


			float wwidth = ((((ea.Width - sa.Width)*delta))+sa.Width);
			float hheight = ((((ea.Height - sa.Height)*delta))+sa.Height);

            return new RectangleF(x, y, wwidth, hheight);
			
		}

        //*******************************************************************
        public int GetRequiredMotionBlur()
        {
            if (mMovementEquation is CQuickSlow)
            {
                return 30;
            }
            return 1;
        }

		//*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement pz = doc.CreateElement("PanZoom");

			CGlobals.SaveRectangle(pz,doc,"StartArea",mStartArea);
			CGlobals.SaveRectangle(pz,doc,"EndArea",mEndArea);

            if (mInitialDelay != 0.0)
            {
                pz.SetAttribute("InitialDelay", mInitialDelay.ToString());
            }

            if (mEndDelay != 1.0)
            {
                pz.SetAttribute("EndDelay", mEndDelay.ToString());
            }

            mMovementEquation.Save(pz, doc);

            if (mStartRotation != 0)
            {
                pz.SetAttribute("StartRotation", mStartRotation.ToString());
            }

            if (mEndRotation != 0)
            {
                pz.SetAttribute("EndRotation", mEndRotation.ToString());
            }

            if (mPanZoomOnAll == true)
            {
                pz.SetAttribute("PanZoomOnAll", mPanZoomOnAll.ToString());
            }

            if (mReGenerateOnImageChange == true)
            {
                pz.SetAttribute("ReGenerateOnImageChange", mReGenerateOnImageChange.ToString());
            }

			parent.AppendChild(pz); 
		}


		//*******************************************************************
		public void Load(XmlElement element)
		{
			mStartArea = CGlobals.LoadRectangle(element,"StartArea");
			mEndArea = CGlobals.LoadRectangle(element,"EndArea");

            string s = element.GetAttribute("InitialDelay");
            if (s != "")
            {
                this.mInitialDelay = float.Parse(s);
            }

            s = element.GetAttribute("EndDelay");
            if (s != "")
            {
                this.EndDelay = float.Parse(s);
            }

            // legacy
            s = element.GetAttribute("Smoothing");
            if (s != "")
            {
                bool use = bool.Parse(s);
                if (use == true)
                {
                    mMovementEquation = new CNonLinear();
                }
            }

            XmlNodeList list = element.GetElementsByTagName("Equation");
            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;

                CEquation equation = CEquation.CreateFromType(e.GetAttribute("Type"));
                if (equation != null)
                {
                    equation.Load(e);
                    mMovementEquation = equation;
                }
            }

            s = element.GetAttribute("StartRotation");
            if (s != "")
            {
                this.mStartRotation = float.Parse(s);
            }

            s = element.GetAttribute("EndRotation");
            if (s != "")
            {
                this.mEndRotation = float.Parse(s);
            }

            s = element.GetAttribute("PanZoomOnAll");
            if (s!="")
            {
                mPanZoomOnAll = bool.Parse(s);
            }

            s = element.GetAttribute("ReGenerateOnImageChange");
            if (s != "")
            {
                mReGenerateOnImageChange = bool.Parse(s);
            }
       
			mCalcedImage=true;
		}


	}
}
