using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using DVDSlideshow;

namespace DVDSlideshow
{
    public class CStraightLineMovement : CMovement
    {
        private PointF mStartPoint;
        private float mStartZ = 0;
        private PointF mEndPoint;
        private float mEndZ = 0;

        // If set true, then the z vanishing point is as 0.5, 0.5 screen space else 
        // it is the centre of the decoration
        private bool mTranslateXYIfZSet = false;

        public PointF StartPoint
        {
            get { return mStartPoint; }
            set { mStartPoint = value; }
        }

        public float StartZ
        {
            get { return mStartZ; }
            set { mStartZ = value; }
        }

        public float EndZ
        {
            get { return mEndZ; }
            set { mEndZ = value; }
        }

        public PointF EndPoint
        {
            get { return mEndPoint; }
            set { mEndPoint = value; }
        }

        public bool TranslateXYIfZSet
        {
            get { return mTranslateXYIfZSet; }
            set { mTranslateXYIfZSet = value; }
        }

        public override string ToMovementString()
        {
            return MovementString();
        }

        public static string MovementString()
        {
             return "Straight Line";
        }

        public CStraightLineMovement()
        {
            mStartPoint = new PointF(0, 0);
            mEndPoint = new PointF(0, 0);
        }

        public CStraightLineMovement(PointF startPoint, PointF endPoint)
        {
            this.mStartPoint = startPoint;
            this.mEndPoint = endPoint;
        }

        public override RectangleF GetPosition(RectangleF initialRec, float delta)
        {
            RectangleF position = initialRec;

            float zoom =1;
            float delay_delta = GetDeltaAferDelay(delta);

            float translateX = 0;
            float translateY = 0;

            if (mStartZ != 0 && mEndZ != 0)
            {
                // ok find translate in XY plane of initialRec
                if (mTranslateXYIfZSet == true)
                {
                    float centreX = (initialRec.Width / 2.0f) + initialRec.X;
                    float centerY = (initialRec.Height / 2.0f) + initialRec.Y;

                    translateX = 0.5f - centreX;
                    translateY = 0.5f - centerY;

                    position.X += translateX;
                    position.Y += translateY;
                }
             
                float one_over_zoom = equation.Get(mStartZ, mEndZ, delay_delta);

                zoom = 1 / one_over_zoom;

                position.X -= (((position.Width * zoom) - position.Width) / 2);
                position.Y -= (((position.Height * zoom) - position.Height) / 2);

                position.Width *= zoom;
                position.Height *= zoom;

                if (zoom < 0) return new RectangleF(-1, -1, 0.1f, 0.1f);
            }

            float startX = mStartPoint.X - translateX;
            float endX = mEndPoint.X - translateX;

            float startY = mStartPoint.Y - translateY;
            float endY = mEndPoint.Y - translateY;

            float x = equation.Get(startX, endX, delay_delta);
            float y = equation.Get(startY, endY, delay_delta);

            x *= zoom;
            y *= zoom;

            RectangleF final_position = new RectangleF(position.X + x, position.Y + y, position.Width, position.Height);

            return final_position;
        }

        public override bool IsStatic()
        {
            if (mStartPoint != mEndPoint || mStartZ != mEndZ)
            {
                return false;
            }
            return true;
        }

        public override void Save(XmlElement parent, XmlDocument doc)
        {
            if (mStartPoint.X == 0 && mStartPoint.Y == 0 &&
                mEndPoint.X == 0 && mEndPoint.Y == 0 &&
                mStartZ == 0 && mEndZ ==0)
            {
                return;
            }

            XmlElement movement = doc.CreateElement("Movement");

            movement.SetAttribute("Type", "StraightLineMovement");

            CGlobals.Save3dPointF(movement, doc, "Start", mStartPoint, mStartZ);
            CGlobals.Save3dPointF(movement, doc, "End", mEndPoint, mEndZ);

            if (mTranslateXYIfZSet == true)
            {
                movement.SetAttribute("TranslateXY", mTranslateXYIfZSet.ToString());
            }

            SaveMovementPart(movement, doc);

            parent.AppendChild(movement);
        }

        public override void Load(XmlElement element)
        {
            mStartPoint = CGlobals.Load3dPointF(element, "Start", ref mStartZ);
            mEndPoint = CGlobals.Load3dPointF(element, "End", ref mEndZ);

            string s = element.GetAttribute("TranslateXY");
            if (s != "")
            {
                mTranslateXYIfZSet = bool.Parse(s);
            }   

            LoadMovementPart(element);
        }
    }
}
