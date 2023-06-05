using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DVDSlideshow;
using System.Drawing.Imaging;

namespace CustomButton
{
    public partial class CropUserControl : UserControl
    {
        enum TRACKERSELECTED// keep track of which border of the box is to be resized.
        {
            RB_NONE = 0,
            RB_TOP = 1,
            RB_RIGHT = 2,
            RB_BOTTOM = 3,
            RB_LEFT = 4,
            RB_TOPLEFT = 5,
            RB_TOPRIGHT = 6,
            RB_BOTTOMLEFT = 7,
            RB_BOTTOMRIGHT = 8,
            RB_MOVE =9
        }

        public delegate void ImageChangedCallbackDelegate();

        public event ImageChangedCallbackDelegate ImageChanged;
        public event ImageChangedCallbackDelegate MovingCropArea; 

        private List<CImageDecoration> mForDecorations;     // all the same template image
        private PictureBox mForPictureBox;
        private CRectTracker mRecTracker = null;
        private bool mMouseDown = false;
        private TRACKERSELECTED mCurrentAction = TRACKERSELECTED.RB_NONE;
        private Point mLastActionPoint = new Point(0,0);

        private float mUseAspect = -1;

        RectangleF mClipRec = new RectangleF(0,0,1,1);

        public CheckBox CropEnabledCheckBox
        {
            get { return mCropEnabledCheckBox; }
        }

        private Bitmap mCachedBitmap = null;
        private Bitmap mCachedBitmapDark = null;

        //*************************************************************************************************
        public CropUserControl()
        {
            InitializeComponent();

            mRecTracker =null;
        }

        //*************************************************************************************************   
        public void SetForDecoration(List<CImageDecoration> forDecorations, PictureBox forPB)
        {
            mForDecorations = forDecorations;

            if (mForPictureBox == null)
            {
                mForPictureBox = forPB;
                mForPictureBox.MouseMove += new MouseEventHandler(mForPictureBox_MouseMove);
                mForPictureBox.MouseDown += new MouseEventHandler(mForPictureBox_MouseDown);
                mForPictureBox.MouseUp += new MouseEventHandler(mForPictureBox_MouseUp);
            }

            if (forDecorations[0].GetCrop() != new RectangleF(0, 0, 1, 1))
            {
                mClipRec = RotateFlipFromDecoration(forDecorations[0].GetCrop());

                this.mRecTracker = new CRectTracker(true, false);
                mRecTracker.Offset = 0;

                mCropEnabledCheckBox.Checked = true;
            }
            else
            {
                mCropEnabledCheckBox.Checked = false;
            }

            EnableDisableControls();
            ClearCachedBitmaps();
            DrawClipPreviewPB();

        }
             
        
        //*************************************************************************************************   
        private void EnableDisableControls()
        {
            bool enabled = false;

            if (mCropEnabledCheckBox.Checked == true)
            {
                enabled = true;
            }

            foreach (Control c in this.Controls)
            {
                if (c != mCropEnabledCheckBox)
                {
                    c.Enabled = enabled;
                }
            }
            UpdateTextBoxes();
        }

        //************************************************************************************************* 
        public void DisableCrop()
        {
            mRecTracker = null;
            this.ClearCachedBitmaps();
        }

        //*************************************************************************************************   
        private void ClearCachedBitmaps()
        {
            if (mCachedBitmap != null)
            {
                mCachedBitmap.Dispose();
                mCachedBitmap = null;
            }

            if (mCachedBitmapDark != null)
            {
                mCachedBitmapDark.Dispose();
                mCachedBitmapDark = null;
            }
        }

        //*************************************************************************************************   
        private TRACKERSELECTED GetCurrentMouseOver(MouseEventArgs e, ref Cursor cursor)
        {
            Point ee = e.Location;
            ee.X -= (mForPictureBox.ClientRectangle.Width - mForPictureBox.Image.Width) / 2;
            ee.Y -= (mForPictureBox.ClientRectangle.Height - mForPictureBox.Image.Height) / 2;

            Rectangle r = CalcRect(new Size(mForPictureBox.Image.Width ,
                                            mForPictureBox.Image.Height));
            int result = 0;

            Cursor c = mRecTracker.CursorCheck(r, ee, ref result);

            if (result ==0 && r.Contains(ee))
            {
                result = 9;
            }

            if (cursor!=null)
            {
                cursor = c;
            }

            return (TRACKERSELECTED)result;
        }

        //*************************************************************************************************   
        void mForPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mMouseDown = false;
            if (mRecTracker == null) return;
            StoreClipRec();
        }

        

        //*************************************************************************************************   
        void mForPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (mMouseDown == true) return;
            if (mRecTracker == null) return ;

            Cursor c = Cursor.Current;
            TRACKERSELECTED selected = GetCurrentMouseOver(e,ref c);
            mCurrentAction = selected;
            mLastActionPoint = e.Location;

            mMouseDown = true;
        }



        //*************************************************************************************************   
        public Rectangle PictureBoxImagePosInClient()
        {
            int x = (mForPictureBox.ClientRectangle.Width - mForPictureBox.Image.Width) / 2;
            int y = (mForPictureBox.ClientRectangle.Height - mForPictureBox.Image.Height) / 2;
            if (x < 0) x = 0;
            if (y < 0) y = 0;

            return new Rectangle(x, y, mForPictureBox.Image.Width, mForPictureBox.Image.Height);
        }


        //*************************************************************************************************   
        void mForPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mForPictureBox.Image == null) return;
            if (mRecTracker == null) return;

            if (MovingCropArea != null)
            {
                MovingCropArea();
            }

            Rectangle imagePos = PictureBoxImagePosInClient();
         
            int elocx = e.X;
            int elocy = e.Y;
            
            // is mouse outside picture box, then clamp certain positions
            if (imagePos.Contains(e.Location)== false &&
                imagePos.Contains(mLastActionPoint) == false)
            {
                if (e.X < imagePos.X && mClipRec.X <=0)
                {
                    elocx = imagePos.X;
                    mLastActionPoint.X = elocx;
                }
                if (e.Y < imagePos.Y && mClipRec.Y <=0)
                {
                    elocy = imagePos.Y;
                    mLastActionPoint.Y = elocy;
                }

                if ((e.X > (imagePos.X + imagePos.Width)) && ((mClipRec.X+mClipRec.Width) >=1))
                {
                    elocx = imagePos.Width + imagePos.X;
                    mLastActionPoint.X = elocx;
                }
  
                if ((e.Y > (imagePos.Y + imagePos.Height)) && ((mClipRec.Y+mClipRec.Height) >=1))
                {
                    elocy = imagePos.Height + imagePos.Y;
                    mLastActionPoint.Y = elocy;
                }
            }

            if (mMouseDown == false)
            {
               Cursor c = Cursor.Current;
               GetCurrentMouseOver(e, ref c);
               mForPictureBox.Cursor = c;
            }
            else
            {
                if (mCurrentAction == TRACKERSELECTED.RB_NONE) return;

                float imageWidth = this.mForPictureBox.Image.Width;
                float imageHeight = this.mForPictureBox.Image.Height;

                float aspect = mUseAspect;

                float xdiff = mLastActionPoint.X - elocx;
                float ydiff = mLastActionPoint.Y - elocy;

                xdiff /= imageWidth;
                ydiff /= imageHeight;

                if (mCurrentAction == TRACKERSELECTED.RB_MOVE)
                {
                    float newX = mClipRec.X;
                    float newY = mClipRec.Y;
                    float newW = mClipRec.Width;
                    float newH = mClipRec.Height;
                    newX = CGlobals.ClampF(mClipRec.X - xdiff, 0, 1);
                    newY = CGlobals.ClampF(mClipRec.Y - ydiff, 0, 1);
                    if (newX + mClipRec.Width > 1) newX = 1 - mClipRec.Width;
                    if (newY + mClipRec.Height > 1) newY = 1 - mClipRec.Height;

                    mClipRec = new RectangleF(newX, newY, newW, newH);
                }
                else
                {
                    bool switchx = false;

                    if (aspect > 0)
                    {
                        if (Math.Abs(xdiff) < Math.Abs(ydiff))
                        {
                            xdiff = ydiff * aspect;
                            switchx = true;
                        }
                        else
                        {
                            ydiff = xdiff / aspect;
                        }
                    }


                    if (mCurrentAction == TRACKERSELECTED.RB_TOPLEFT)
                    {
                        AdjustRecXY(xdiff, ydiff, aspect);
                    }
                    if (mCurrentAction == TRACKERSELECTED.RB_LEFT)
                    {
                        if (aspect <= 0)
                        {
                            ydiff = 0;
                        }
                        AdjustRecXY(xdiff, ydiff, aspect);
                    }

                   
                    if (mCurrentAction == TRACKERSELECTED.RB_BOTTOMRIGHT)
                    {
                        AdjustRecWidthHeight(xdiff, ydiff, aspect);
                    }
                    if( mCurrentAction == TRACKERSELECTED.RB_RIGHT)
                    {
                        if (aspect <=0)
                        {
                            ydiff=0;
                        }
                         AdjustRecWidthHeight(xdiff, ydiff, aspect);
                    }

                   
                    if (mCurrentAction == TRACKERSELECTED.RB_TOPRIGHT ||
                        mCurrentAction == TRACKERSELECTED.RB_TOP)
                    {
                        if (aspect > 0)
                        {
                            if (switchx) xdiff = -xdiff; else ydiff = -ydiff;
                        }

                        if (mCurrentAction == TRACKERSELECTED.RB_TOP &&
                            aspect <= 0)
                        {
                            xdiff = 0;
                        }


                        AdjustRecWidthY(xdiff, ydiff, aspect);
                    }

                    if (mCurrentAction == TRACKERSELECTED.RB_BOTTOMLEFT ||
                        mCurrentAction == TRACKERSELECTED.RB_BOTTOM)
                    {
                        if (aspect > 0)
                        {
                            if (switchx) xdiff = -xdiff; else ydiff = -ydiff;
                        }
                        if (mCurrentAction == TRACKERSELECTED.RB_BOTTOM &&
                            aspect <= 0)
                        {
                            xdiff = 0;
                        }

                        AdjustRecXHeight(xdiff, ydiff, aspect);
                    }
                }

                mLastActionPoint = new Point(elocx, elocy);

                DrawClipPreviewPB();
                UpdateTextBoxes();
            }
        }

        //************************************************************************************************* 
        private void DoTransFormClip(ref float newX, ref float newY, ref float newW, ref float newH, float xdiff, float ydiff, float aspect)
        {
            newW = newW - xdiff;
            newH = newH - ydiff;

            if (newW < 0.1f)
            {
                newW = 0.1f;
                if (aspect > 0) newH = newW / aspect;
            }
            if (newH < 0.1f)
            {
                newH = 0.1f;
                if (aspect > 0) newW = newH * aspect;
            }

            if (newX + newW > 1)
            {
                newW = 1 - newX;
                if (aspect > 0) newH = newW / aspect;
            }
            if (newY + newH > 1)
            {
                newH = 1 - newY;
                if (aspect > 0) newW = newH * aspect;
            }
        }

        //*************************************************************************************************   
        private void AdjustRecXY(float xdiff, float ydiff, float aspect)
        {
            float newX = 1.0f- (mClipRec.X + mClipRec.Width);
            float newY = 1.0f- (mClipRec.Y + mClipRec.Height);
            float newW = mClipRec.Width;
            float newH = mClipRec.Height;

            xdiff = -xdiff;
            ydiff = -ydiff;

            DoTransFormClip(ref newX, ref newY, ref newW, ref newH, xdiff, ydiff, aspect);

            newX +=newW;
            newY += newH;
            newX = 1 - newX;
            newY = 1 - newY;

            mClipRec = new RectangleF(newX, newY, newW, newH);
        }

        //*************************************************************************************************   
        private void AdjustRecWidthHeight(float xdiff, float ydiff, float aspect)
        {
            float newX = mClipRec.X;
            float newY = mClipRec.Y;
            float newW = mClipRec.Width;
            float newH = mClipRec.Height;

            DoTransFormClip(ref newX, ref newY, ref newW, ref newH, xdiff, ydiff, aspect);
       
            mClipRec = new RectangleF(newX, newY, newW, newH);
        }

        //*************************************************************************************************   
        private void AdjustRecWidthY(float xdiff, float ydiff, float aspect)
        {
            float newX = mClipRec.X;
            float newY = 1.0f - (mClipRec.Y + mClipRec.Height);
            float newW = mClipRec.Width;
            float newH = mClipRec.Height;

            ydiff = -ydiff;

            DoTransFormClip(ref newX, ref newY, ref newW, ref newH, xdiff, ydiff, aspect);

            newY += newH;
            newY = 1 - newY;

            mClipRec = new RectangleF(newX, newY, newW, newH);
        }


        //*************************************************************************************************   
        private void AdjustRecXHeight(float xdiff, float ydiff, float aspect)
        {
            float newX = 1.0f - (mClipRec.X + mClipRec.Width);
            float newY = mClipRec.Y;
            float newW = mClipRec.Width;
            float newH = mClipRec.Height;

            xdiff = -xdiff;

            DoTransFormClip(ref newX, ref newY, ref newW, ref newH, xdiff, ydiff, aspect);

            newX += newW;
            newX = 1 - newX;

            mClipRec = new RectangleF(newX, newY, newW, newH);
        }

        //*************************************************************************************************   
        private int Round(float val)
        {
            return (int)(val + 0.49999f);
        }

        //*************************************************************************************************   
        private Rectangle CalcRect(Size b)
        {
            float x = (((float)b.Width) * mClipRec.X);
            int xx = Round(x);
            float y = (((float)b.Height) * mClipRec.Y);
            int yy = Round(y);

            float w = (((float)b.Width) * mClipRec.Width);
            int ww = Round(w);
            float h = (((float)b.Height) * mClipRec.Height);
            int hh = Round(h);

            return new Rectangle(xx, yy, ww, hh);
        }


        //*************************************************************************************************   
        private void DrawClipPreviewPB()
        {
            if (mCachedBitmap == null)
            {
                bool drawAlphaMap = mRecTracker == null;

                Image i = DecorationThumbnailGenerator.GenerateThumbnailForDecoration(mForDecorations[0], mForPictureBox.Width, mForPictureBox.Height, null, drawAlphaMap, true, true);

                mCachedBitmap = new Bitmap(i.Width, i.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(mCachedBitmap))
                {
                    g.Clear(mForPictureBox.BackColor);
                    g.DrawImage(i, new Point(0, 0));
                }

                mCachedBitmapDark = new Bitmap(i.Width, i.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(mCachedBitmapDark))
                {
                    g.Clear(mForPictureBox.BackColor);

                    float[][] ptsArray ={ 
                        new float[] {1, 0, 0, 0, 0},
                        new float[] {0, 1, 0, 0, 0},
                        new float[] {0, 0, 1, 0, 0},
                        new float[] {-.2f, -.2f, -.2f, 1, 0}, 
                        new float[] {0, 0, 0, 0, 1}};

                    ColorMatrix cm = new ColorMatrix(ptsArray);
                    ImageAttributes ia = new ImageAttributes();
                    ia.ClearColorMatrix();
                    ia.SetColorMatrix(cm);

                    g.DrawImage(i, new Rectangle(0, 0, mCachedBitmapDark.Width, mCachedBitmapDark.Height), 0, 0, mCachedBitmapDark.Width, mCachedBitmapDark.Height, GraphicsUnit.Pixel, ia);
                }

                i.Dispose();
            }

            Rectangle cliprec = new Rectangle(0, 0, mCachedBitmap.Width, mCachedBitmap.Height);
 
            if (mRecTracker != null)
            {
                cliprec = CalcRect(new Size(mCachedBitmap.Width, mCachedBitmap.Height));
            }

            Bitmap bb = mCachedBitmapDark.Clone() as Bitmap;

            using (Graphics g = Graphics.FromImage(bb))
            {
                g.DrawImage(mCachedBitmap, cliprec, cliprec.X, cliprec.Y, cliprec.Width, cliprec.Height, GraphicsUnit.Pixel);
            }

            if (mRecTracker != null)
            {
                mRecTracker.DrawSelectionGhostRect(cliprec, bb);
                mRecTracker.DrawSelectionTrackers(cliprec, bb);
            }

            Image oldImage = mForPictureBox.Image;
            mForPictureBox.Image = bb;
            if (oldImage != null)
            {
                oldImage.Dispose();
            }
        }

        //*************************************************************************************************
        private void CropUserControl_VisibleChanged(object sender, EventArgs e)
        {
            if (mForDecorations == null) return;

            if (this.Visible == true)
            {
                ClearCachedBitmaps();
                DrawClipPreviewPB();
            }
        }

        //*************************************************************************************************
        private void mLockAspectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (mLockAspectCheckBox.Checked == true)
            {
                mUseAspect = mClipRec.Width / mClipRec.Height;
                return;
            }

            this.mUseAspect = -1;
        }

        //*************************************************************************************************
        private void mUseSquareAspectButton_Click(object sender, EventArgs e)
        {
            RectangleF i1 = new RectangleF(0, 0, 1, 1);
            RectangleF i2 = new RectangleF(0, 0, mForPictureBox.Image.Width, mForPictureBox.Image.Height);

            RectangleF r = CGlobals.CalcBestFitRectagleF(i2, i1,false);

            r.X /= mForPictureBox.Image.Width;
            r.Y /= mForPictureBox.Image.Height;
            r.Width /= mForPictureBox.Image.Width;
            r.Height /= mForPictureBox.Image.Height;

            mClipRec = r;
            mUseAspect = r.Width / r.Height;
            StoreClipRec();
            DrawClipPreviewPB();
            if (mLockAspectCheckBox.Checked == false)
            {
                mLockAspectCheckBox.Checked = true;
            }
        }

        //*************************************************************************************************
        private void mUseImageAspectButton_Click(object sender, EventArgs e)
        {
            float width = mForPictureBox.Image.Width;
            float height = mForPictureBox.Image.Height;

            mUseAspect = 1;
            mClipRec = new RectangleF(0.2f,0.2f,0.6f,0.6f);
            StoreClipRec();
            DrawClipPreviewPB();
            if (mLockAspectCheckBox.Checked == false)
            {
                mLockAspectCheckBox.Checked = true;
            }           
        }

        //*************************************************************************************************
        private void mUse4by3AspectButton_Click(object sender, EventArgs e)
        {
            RectangleF i1 = new RectangleF(0, 0, 1.33333f, 1);
            RectangleF i2 = new RectangleF(0, 0, mForPictureBox.Image.Width, mForPictureBox.Image.Height);

            RectangleF r = CGlobals.CalcBestFitRectagleF(i2, i1, false);

            r.X /= mForPictureBox.Image.Width;
            r.Y /= mForPictureBox.Image.Height;
            r.Width /= mForPictureBox.Image.Width;
            r.Height /= mForPictureBox.Image.Height;

            mClipRec = r;
            mUseAspect = r.Width / r.Height;
            StoreClipRec();
            DrawClipPreviewPB();
            if (mLockAspectCheckBox.Checked == false)
            {
                mLockAspectCheckBox.Checked = true;
            }
        }

        //*************************************************************************************************
        private void mUse16by9AspectButton_Click(object sender, EventArgs e)
        {

            RectangleF i1 = new RectangleF(0, 0, 1.7777f, 1);
            RectangleF i2 = new RectangleF(0, 0, mForPictureBox.Image.Width, mForPictureBox.Image.Height);

            RectangleF r = CGlobals.CalcBestFitRectagleF(i2, i1, false);

            r.X /= mForPictureBox.Image.Width;
            r.Y /= mForPictureBox.Image.Height;
            r.Width /= mForPictureBox.Image.Width;
            r.Height /= mForPictureBox.Image.Height;

            mClipRec = r;
            mUseAspect = r.Width / r.Height;
            StoreClipRec();
            DrawClipPreviewPB();
            if (mLockAspectCheckBox.Checked == false)
            {
                mLockAspectCheckBox.Checked = true;
            }
        }

        //*************************************************************************************************
        private void mCropEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.ClearCachedBitmaps();

            if (mCropEnabledCheckBox.Checked == true)
            {
                if (mRecTracker !=null) return ;

                this.mRecTracker = new CRectTracker(true, false);
                mRecTracker.Offset = 0;
                mClipRec = new RectangleF(0.2f, 0.2f, 0.6f, 0.6f);
                StoreClipRec();
            }
            else
            {
                if (mRecTracker==null) return ;

                mRecTracker = null;
                mClipRec = new RectangleF(0, 0, 1, 1);
                StoreClipRec();
            }

            DrawClipPreviewPB();
            EnableDisableControls();
            
        }

        //*************************************************************************************************
        private void UpdateTextBoxes()
        {
            int w=0;
            int h=0;

            if (mForDecorations != null)
            {
                Size s = this.mForDecorations[0].GetOriginalFileDimension();
                w = s.Width;
                h = s.Height;
                if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.CW90 ||
                    mForDecorations[0].Orientation == CImageDecoration.OrientationType.CW270)
                {
                    w = s.Height;
                    h = s.Width;
                }

                mOriginalSizeTextBox.Text = w + " x " + h;
            }
            else
            {
                mOriginalSizeTextBox.Text = "";
            }

            if (mCropEnabledCheckBox.Checked == false)
            {
                mCroppedLeftTextBox.Text = "";
                mCroppedRightTextBox.Text = "";
                mCroppedTopTextBox.Text = "";
                mCroppedBottomTextBox.Text = "";
                mCroppedSizeTextBox.Text = "";
            }
            else
            {

                Size s = new Size(w, h);
                Rectangle r = this.CalcRect(s);

                mCroppedLeftTextBox.Text = r.X.ToString();
                mCroppedRightTextBox.Text = (r.X + r.Width).ToString();
                mCroppedTopTextBox.Text = r.Y.ToString();
                mCroppedBottomTextBox.Text = (r.Y + r.Height).ToString();
                mCroppedSizeTextBox.Text = r.Width + " x " + r.Height;

            }
        }

        //*************************************************************************************************
        // when we draw slide in DirectX way, crop is applied first then rotation.
        // In this editor it is the other way around....
        private RectangleF RotateFlipFromDecoration(RectangleF crop)
        {
            if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.NONE &&
                mForDecorations[0].XFlipped == false &&
                mForDecorations[0].YFlipped == false)
            {
                return crop;
            }

            if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.CW90)
            {
                crop = Rotate90(crop);
            }


            if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.CW180)
            {
                crop = Rotate90(crop);
                crop = Rotate90(crop);
            }

            if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.CW270)
            {
                crop = Rotate90(crop);
                crop = Rotate90(crop);
                crop = Rotate90(crop);
            }

            if (mForDecorations[0].XFlipped == true)
            {
                crop = FlipX(crop);
            }

            if (mForDecorations[0].YFlipped == true)
            {
                crop = FlipY(crop);
            }

            return crop;

        }

        //*************************************************************************************************
        private RectangleF RotateFlipToDecoration(RectangleF crop)
        {

            if (mForDecorations[0].YFlipped == true)
            {
                crop = FlipY(crop);
            }

            if (mForDecorations[0].XFlipped == true)
            {
                crop = FlipX(crop);
            }


            if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.NONE &&
                mForDecorations[0].XFlipped == false &&
                mForDecorations[0].YFlipped == false)
            {
                return crop;
            }

            if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.CW90)
            {
                // inverse so do 270
                crop = Rotate90(crop);
                crop = Rotate90(crop);
                crop = Rotate90(crop);
            }


            if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.CW180)
            {
                crop = Rotate90(crop);
                crop = Rotate90(crop);
            }

            if (mForDecorations[0].Orientation == CImageDecoration.OrientationType.CW270)
            {
                // inverse so do 90
                crop = Rotate90(crop);
            }


            return crop;

        }

        //*************************************************************************************************
        private RectangleF Rotate90(RectangleF crop)
        {
            float new_height = crop.Width ;
            float new_width = crop.Height ;

            float newX = 1 - (crop.Y + crop.Height);
            float newY = crop.X;

            return new RectangleF(newX, newY, new_width, new_height);
        }

        //*************************************************************************************************
        private RectangleF FlipX(RectangleF crop)
        {
            float newX = 1 - (crop.X + crop.Width);
            return new RectangleF(newX, crop.Y, crop.Width, crop.Height);
        }

        //*************************************************************************************************
        private RectangleF FlipY(RectangleF crop)
        {
            float newY = 1 - (crop.Y + crop.Height);
            return new RectangleF(crop.X, newY, crop.Width, crop.Height);
        }


        //*************************************************************************************************
        private void StoreClipRec()
        {
            if (mForDecorations != null)
            {
                UpdateTextBoxes();

                RectangleF cropToStore = RotateFlipToDecoration(mClipRec);

                foreach (CImageDecoration dec in mForDecorations)
                {
                    if (dec.GetCrop() != cropToStore)
                    {
                        dec.SetCrop(cropToStore);
                    }
                }
                if (ImageChanged != null)
                {
                    ImageChanged();
                }
           }
        }   
    }
}
