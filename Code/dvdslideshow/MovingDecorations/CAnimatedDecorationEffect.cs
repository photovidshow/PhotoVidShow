using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DVDSlideshow
{
    public class CAnimatedDecorationEffect
    {
        public enum CharacterDelayOrder
        {
            FIRST_TO_LAST,
            LAST_TO_FIRST,
            RANDOM
        };

        public enum Type
        {
            BOTH_MOVE_IN_AND_OUT,
            MOVE_IN,
            MOVE_OUT
        };

        private Type mType = Type.BOTH_MOVE_IN_AND_OUT;
        private CRotation mRotation = new CRotation();
        private CZoom mZoom = new CZoom();
        private CMovement mMovement = null;
        private CStretch mStretch = null;
        private CTransitionEffect mTransitionEffect = null;
        private bool mHasCharacterTimeDelay=false;
        private float mCharacerTimeDelayInSeconds = 0;
        private CharacterDelayOrder mCharacterDelayOrder = CharacterDelayOrder.FIRST_TO_LAST;
        private string mName;
        private float lengthInSeconds = 1.0f;
        private bool mOverideLengthToDecorationLife = false;    // if true the effect lasts the same length as the decoration it is in
        private int mRequiredMotionBlur = 1;
        private bool mTemplateOnlyEffect = false;  // used to indicate this is used by a template ONLY and not in the move effects list 
        private string mUVAnimatedEffectName = null;

        public bool TemplateOnlyEffect
        {
            set { mTemplateOnlyEffect = value; }
            get { return mTemplateOnlyEffect; }
        }

        public int RequiredMotionBlur
        {
            get { return mRequiredMotionBlur; }
            set { mRequiredMotionBlur = value; }
        }

        public bool LengthSetToDecorationLength
        {
            get { return mOverideLengthToDecorationLife; }
            set { mOverideLengthToDecorationLife = value; }
        }

        public CTransitionEffect TransitionEffect
        {
            get { return mTransitionEffect; }
            set { mTransitionEffect = value; }
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        public float LengthInSeconds
        {
            get { return lengthInSeconds; }
            set { lengthInSeconds = value; }
        }


        public float GetLengthInSeconds(float decorationLife)
        {
            if (mOverideLengthToDecorationLife == true)
            { 
                return decorationLife;
            }

            return lengthInSeconds;

        }

        public CZoom Zoom
        {
            get { return mZoom; }
            set { mZoom = value; }
        }

        public CRotation Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }

        public CMovement Movement
        {
            get { return mMovement; }
            set { mMovement = value; }
        }

        public CStretch Stretch
        {
            get { return mStretch; }
            set { mStretch = value; }
        }

        public bool HasCharacterTimeDelay
        {
            get { return mHasCharacterTimeDelay; }
            set { mHasCharacterTimeDelay = value; }
        }

        public float CharacerTimeDelayInSeconds
        {
            get { return mCharacerTimeDelayInSeconds; }
            set { mCharacerTimeDelayInSeconds = value; }
        }

        public CharacterDelayOrder CharacterOrder
        {
            get { return mCharacterDelayOrder; }
            set { mCharacterDelayOrder = value; }
        }

        public string UVAnimatedEffectName
        {
            get { return mUVAnimatedEffectName; }
            set { mUVAnimatedEffectName = value; }
        }

        public CAnimatedDecorationEffect UVAnimatedEffect
        {
            get
            {
                if (string.IsNullOrEmpty(this.mUVAnimatedEffectName) == false)
                {
                    return CAnimatedDecorationEffectDatabase.GetInstance().Get(mUVAnimatedEffectName);
                }
                return null;
            }
        }

        public CAnimatedDecorationEffect()
        {
        }

        public CAnimatedDecorationEffect(string name)
        {
            mName = name;
        }

        public CAnimatedDecorationEffect XMLClone()
        {
            CAnimatedDecorationEffect newEffect = null;

            try
            {
                XmlDocument saveDoc = new XmlDocument();
                XmlElement docElement = saveDoc.CreateElement("doc");
                saveDoc.AppendChild(docElement);
                this.Save(docElement, saveDoc);

                XmlNodeList list = docElement.GetElementsByTagName("DecorationEffect");

                if (list.Count != 0)
                {
                    XmlElement element = list[0] as XmlElement;

                    newEffect = new CAnimatedDecorationEffect();
                    newEffect.Load(element);
                    newEffect.Name = this.mName + " Copy";
                }
            }
            catch
            {
            }

            if (newEffect == null)
            {
                ManagedCore.Log.Error("Failed to do XML Clone on an animated decoration effect");
            }

            return newEffect;
        }


        public Type AnimatedType
        {
            get { return mType; }
            set { mType = value; }
        }

        public CRectangleReferenceFrame GetRectanglePosition(RectangleF initialRectangle, RectangleF UVCoords, float delta)
        {
            RectangleF movement = initialRectangle ;

            // Apply movement
            if (mMovement != null)
            {    
                movement = mMovement.GetPosition(movement, delta);             
            }

            // Apply strech
            if (mStretch != null)
            {
                movement = mStretch.ApplyStretch(movement, delta);
            }

            // Apply rotation
            float rotation = 0;
            float xRotOffset = 0;
            float yRotOffset = 0;

            if (mRotation != null)
            {
                rotation = mRotation.GetRotation(delta);
                xRotOffset = mRotation.XOffset;
                yRotOffset = mRotation.YOffset;
            }

            // Apply zoom
            if (mZoom != null) 
            {
                movement = mZoom.GetZoom(movement, delta);
            }

            float UVRotation = 0;
            float UVXOffset = 0;
            float UVYOffset = 0;

            CAnimatedDecorationEffect UVEffect = UVAnimatedEffect;

            if (UVEffect != null)
            {
                CRectangleReferenceFrame uvframe = UVEffect.GetRectanglePosition(UVCoords, new RectangleF(0, 0, 1, 1), delta);
                UVCoords = uvframe.position;
                UVRotation = uvframe.rotation;
                UVXOffset = uvframe.offsetRotX;
                UVYOffset = uvframe.offsetRoyY;
            }

            return new CRectangleReferenceFrame(movement, rotation, xRotOffset, yRotOffset, UVCoords, UVRotation, UVXOffset, UVYOffset);
        }


        //*******************************************************************
        private void ApplyRotation(RectangleF r1, System.Drawing.Drawing2D.Matrix mat, PointF[] out_points)
        {
            float offsetx = mat.OffsetX;
            float offsety = mat.OffsetY;

            PointF[] points = null;

            if (out_points != null)
            {
                points = out_points;
            }
            else
            {
                points = new PointF[4];
            }

            points[0] = new PointF(r1.X - offsetx, r1.Y - offsety);
            points[1] = new PointF((r1.X + r1.Width) - offsetx, r1.Y - offsety);
            points[2] = new PointF((r1.X + r1.Width) - offsetx, (r1.Y + r1.Height) - offsety);
            points[3] = new PointF(r1.X - offsetx, (r1.Y + r1.Height) - offsety);

            mat.TransformVectors(points);

            for (int i = 0; i < 4; i++)
            {
                points[i].X += offsetx;
                points[i].Y += offsety;
            }
        }


        //*******************************************************************
        public PointF[] GetRectanglePoints(RectangleF initialRectangle, RectangleF surface, float delta)
        {
            CRectangleReferenceFrame positionRF = GetRectanglePosition(initialRectangle, new RectangleF(0,0,1,1), delta);

            RectangleF r = positionRF.position;
            r = new RectangleF(r.X * surface.Width, r.Y * surface.Height, r.Width * surface.Width, r.Height * surface.Height);
     
    
            PointF [] points = new PointF[4];

            DisposableObject<Matrix> rot_mat = new DisposableObject<Matrix>();

            float xx = r.X + (r.Width / 2) + (positionRF.offsetRotX * surface.Width);
            float yy = r.Y + (r.Height / 2) + (positionRF.offsetRoyY * surface.Height);

            rot_mat.Assign(new Matrix());
            rot_mat.Object.Rotate(positionRF.rotation);
            rot_mat.Object.Translate(xx, yy, MatrixOrder.Append);

            ApplyRotation(r, rot_mat, points);
            rot_mat.Dispose();
            return points;
        }


        //*******************************************************************
        public float GetMaxZoom()
        {
            float maxzoom = 1.0f;
            CZoom zoom = this.Zoom;
            if (zoom != null)
            {
                maxzoom = 0;
                float endzoom = zoom.EndZoom;
                float startzoom = zoom.StartZoom;

                if (startzoom > maxzoom)
                {
                    maxzoom = startzoom;
                }
                if (endzoom > maxzoom)
                {
                    maxzoom = endzoom;
                }
            }
            return maxzoom;
        }

        //*******************************************************************
        public bool IsStatic()
        {
            // return true if nothing actually changes when given different
            // delta

            if (mTransitionEffect != null) return false;

            if (mRotation != null)
            {
                if (mRotation.IsStatic() == false)
                {
                    return false;
                }
            }

            if (mZoom != null)
            {
                if (mZoom.IsStatic() == false)
                {
                    return false;
                }
            }

            if (mMovement != null)
            {
                if (mMovement.IsStatic() == false)
                {
                    return false;
                }
            }

            if (mStretch != null)
            {
                if (mStretch.IsStatic() == false)
                {
                    return false;
                }
            }

            return true;
        }


        //*******************************************************************
        private static void InvertEquation(CEquation equation)
        {
            if (equation == null) return;

            CSpring spring = equation as CSpring;
            if (spring!=null)
            {
                spring.Inverted = !spring.Inverted;
            }
        }

        //*******************************************************************
        private static string ReplacePair(string name, string a, string b)
        {
            if (name.Contains(a) == true)
            {
                name = name.Replace(a, b);
            }
            else if (name.Contains(b) == true)
            {
                name = name.Replace(b, a);
            }
            return name;
        }


        //*******************************************************************
        // Utility function that creats an Out effect equivilent to the given In effect
        public static CAnimatedDecorationEffect CreateOutEffectFromInEffect(CAnimatedDecorationEffect inEffect)
        {
            CAnimatedDecorationEffect outEffect = inEffect.XMLClone();
            outEffect.Name = outEffect.Name.Replace(" Copy", "");   // remove copy part
            outEffect.AnimatedType = Type.MOVE_OUT;

            // movements are opposite
            outEffect.Name = ReplacePair(outEffect.Name,"Out","In");
            outEffect.Name = ReplacePair(outEffect.Name, "Left", "Right");
            outEffect.Name = ReplacePair(outEffect.Name, "Up", "Down");

            outEffect.Name = outEffect.Name + " "; // OK HACK, names have to be unique (so outeffect is same as ineffect plus a space character)

            if (outEffect.Movement != null)
            {
                CStraightLineMovement slm = outEffect.Movement as CStraightLineMovement;
                if (slm != null)
                {
                    PointF sp = slm.StartPoint;
                    float sz = slm.StartZ;

                    slm.StartPoint = slm.EndPoint;
                    slm.StartZ = slm.EndZ;
                    slm.EndPoint = sp;
                    slm.EndZ = sz;

                    InvertEquation(slm.Equation);               
                }

            }

            if (outEffect.Stretch != null)
            {
                CHorizontalStretch hs = outEffect.Stretch as CHorizontalStretch;
                if (hs != null)
                {
                    float sm = hs.StartMultiplier;
                    hs.StartMultiplier = hs.EndMultiplier;
                    hs.EndMultiplier = sm;

                    InvertEquation(hs.Equation);
                }

                CVerticalStretch vs = outEffect.Stretch as CVerticalStretch;
                if (vs != null)
                {
                    float sm = vs.StartMultiplier;
                    vs.StartMultiplier = vs.EndMultiplier;
                    vs.EndMultiplier = sm;
                    InvertEquation(vs.Equation);
                }
            }

            if (outEffect.Rotation != null)
            {
                float sr = outEffect.Rotation.StartRotation;
                outEffect.Rotation.StartRotation = outEffect.Rotation.EndRotation;
                outEffect.Rotation.EndRotation = sr;
                InvertEquation(outEffect.Rotation.Equation);     
            }
            if (outEffect.Zoom != null)
            {
                float sz = outEffect.Zoom.StartZoom;
                outEffect.Zoom.StartZoom = outEffect.Zoom.EndZoom;
                outEffect.Zoom.EndZoom = sz;
                InvertEquation(outEffect.Zoom.Equation);     
            }

            if (outEffect.TransitionEffect != null)
            {
                float sd = outEffect.TransitionEffect.InitialDelay;
                outEffect.TransitionEffect.InitialDelay = outEffect.TransitionEffect.EndDelay;
                outEffect.TransitionEffect.EndDelay = sd;
            }



            return outEffect;
        }

        //*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc)
		{
			XmlElement decoration = doc.CreateElement("DecorationEffect");

            decoration.SetAttribute("Name", mName);

            if (mOverideLengthToDecorationLife == true)
            {
                decoration.SetAttribute("LengthSetToDecorationLength", mOverideLengthToDecorationLife.ToString());
            }
            else
            {
                decoration.SetAttribute("Length", lengthInSeconds.ToString());
            }

            if (mRequiredMotionBlur != 1)
            {
                decoration.SetAttribute("MotionBlur", mRequiredMotionBlur.ToString());
            }

           	if (mTransitionEffect!=null)
			{
                XmlElement effect = doc.CreateElement("TransitionEffect");
                mTransitionEffect.Save(decoration, doc, effect);
			}

            if (mMovement!=null)
            {
                mMovement.Save(decoration, doc);
            }

            if (mStretch != null)
            {
                mStretch.Save(decoration, doc);
            }

            if (mRotation != null)
            {
                mRotation.Save(decoration, doc);
            }

            if (mZoom != null)
            {
                mZoom.Save(decoration, doc);
            }

            if (mHasCharacterTimeDelay == true)
            {
                decoration.SetAttribute("HasCharDelay", mHasCharacterTimeDelay.ToString());
            }

            if ( mHasCharacterTimeDelay )
            {
                decoration.SetAttribute("CharTimeOffset", mCharacerTimeDelayInSeconds.ToString() );
                decoration.SetAttribute("CharOrder", ((int)mCharacterDelayOrder).ToString());
            }

            if (mTemplateOnlyEffect == true)
            {
                decoration.SetAttribute("TemplateOnlyEffect", mTemplateOnlyEffect.ToString());
            }

            if (string.IsNullOrEmpty(mUVAnimatedEffectName) == false)
            {
                decoration.SetAttribute("UVEffect", mUVAnimatedEffectName);
            }
          
			parent.AppendChild(decoration); 
		}


		//*******************************************************************
        public void Load(XmlElement element)
        {
            string s1 = element.GetAttribute("Name");
            if (s1 != "")
            {
                mName = s1;
            }

            s1 = element.GetAttribute("Length");
            if (s1 != "")
            {
                lengthInSeconds = float.Parse(s1);
            }

            s1 = element.GetAttribute("LengthSetToDecorationLength");
            if (s1 != "")
            {
                mOverideLengthToDecorationLife = bool.Parse(s1);
            }

            s1 = element.GetAttribute("MotionBlur");
            if (s1 != "")
            {
                mRequiredMotionBlur = int.Parse(s1);
            }

            XmlNodeList list = element.GetElementsByTagName("TransitionEffect");

            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;

                mTransitionEffect = CTransitionEffect.CreateFromType(e.GetAttribute("Type"));
                if (mTransitionEffect != null)
                {
                    mTransitionEffect.Load(e);
                }
            }

            list = element.GetElementsByTagName("Movement");

            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;

                mMovement = CMovement.CreateFromType(e.GetAttribute("Type"));
                if (mMovement != null)
                {
                    mMovement.Load(e);
                }
            }

            list = element.GetElementsByTagName("Stretch");

            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;

                mStretch = CStretch.CreateFromType(e.GetAttribute("Type"));
                if (mStretch != null)
                {
                    mStretch.Load(e);
                }
            }

            list = element.GetElementsByTagName("Rotation");

            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;

                mRotation = new CRotation();
                mRotation.Load(e);
            }

            list = element.GetElementsByTagName("Zoom");

            if (list.Count > 0)
            {
                XmlElement e = list[0] as XmlElement;

                mZoom = new CZoom();
                mZoom.Load(e);
            }

            s1 = element.GetAttribute("HasCharDelay");
            if (s1 != "")
            {
                HasCharacterTimeDelay = bool.Parse(s1);
            }

            if (mHasCharacterTimeDelay)
            {
                s1 = element.GetAttribute("CharTimeOffset");
                if (s1 != "")
                {
                    mCharacerTimeDelayInSeconds = float.Parse(s1);
                }

                s1 = element.GetAttribute("CharOrder");
                if (s1 != "")
                {
                    mCharacterDelayOrder = (CAnimatedDecorationEffect.CharacterDelayOrder) int.Parse(s1);
                }
            }

            s1 = element.GetAttribute("TemplateOnlyEffect");
            if (s1 != "")
            {
                mTemplateOnlyEffect = bool.Parse(s1);
            }

            s1 = element.GetAttribute("UVEffect");
            if (s1 != "")
            {
                mUVAnimatedEffectName = s1;
            }
        }
    }
}
