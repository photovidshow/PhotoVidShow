using System;
using System.Collections.Generic;
using System.Text;

namespace dvdslideshowfontend
{
    public abstract class StoryboardComponent
    {
        public int CalculatedLeft =0;
    
        protected int mLeft;
        protected int mTop;
        protected int mWidth;
        protected int mHeight;

        public int Left
        {
            get { return mLeft; }
            set { mLeft = value;}
        }

        public int Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        public int Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

        public int Top
        {
            get { return mTop; }
            set { mTop = value;  }
        }
    }
}
