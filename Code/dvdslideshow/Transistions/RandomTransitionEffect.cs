using System;
using System.Xml;
using System.Drawing;
using DVDSlideshow.GraphicsEng;

namespace DVDSlideshow
{
	/// <summary>
	/// Summary description for RandomTransitionEffect.
	/// </summary>
	public class RandomTransitionEffect : CTransitionEffect
	{
		private CTransitionEffect mTheEffect = null;
		static int seed =-1;

        //*******************************************************************
        public override bool NeedsDuelRenderSurface
        {
            get { return mTheEffect.NeedsDuelRenderSurface; }
        }

        //*******************************************************************
        public override bool UseImage2Alpha
        {
            get { return mTheEffect.UseImage2Alpha; }
            set { mTheEffect.UseImage2Alpha = value; }
        }

        //*******************************************************************
        public override int ForcedMB
        {
            get { return mTheEffect.ForcedMB; }
            set { mTheEffect.ForcedMB = value; }
        }

        //*******************************************************************
        public override float Length
        {
            get 
            {
                return mTheEffect.Length; 
            }
            set 
            { 
                mLength = value;
                mTheEffect.Length = value;
            }
        }

        //*******************************************************************
        public override bool RenderNextSlideBeforeCurrentSlide
        {
            get { return mTheEffect.RenderNextSlideBeforeCurrentSlide; }
        }

		//*******************************************************************
		public RandomTransitionEffect()
		{
            GenerateRandomEffect();
		}

		//*******************************************************************
		public RandomTransitionEffect(float length) : base(length)
		{
            GenerateRandomEffect();
		}

		//*******************************************************************
		private void GenerateRandomEffect()
		{
			if (seed==-1)
			{
				seed = DateTime.Now.Second * DateTime.Now.Minute;
			}

			Random r = new Random(seed);
			seed+=3433;

            CTransitionEffect[] effects = CTransitionEffectTable.GetInstance().GetEffects();
			int effect = r.Next()%(effects.Length-1);
            effect++;

            // If running PhotoCruz limit out selection of transition effects
            while (CGlobals.RunningPhotoCruz == true &&
                   ((effect >= 20 && effect <= 42) ||
                    (effect >= 68 && effect <= 70) ||
                    (effect >= 121)))
            {
                effect = r.Next() % (effects.Length - 1);
                effect++;
            }
           
            mTheEffect = effects[effect].Clone();

            // don't really need to set these but just in case
            this.mNeedsDualRenderSurface = mTheEffect.NeedsDuelRenderSurface;
            mLength = mTheEffect.Length;
            mRenderNextSlideBeforeCurrentSlide = mTheEffect.RenderNextSlideBeforeCurrentSlide;
		}

		//*******************************************************************
		public override CTransitionEffect Clone()
		{
			RandomTransitionEffect t= new RandomTransitionEffect(mLength) ;
			t.Index = this.Index;
			return t;
		}

		//*******************************************************************
        protected override CImage Process(float delta, CImage ii1, CImage ii2, int frame, CImage render_to_this)
		{
            return null;		
		}

        //*******************************************************************
        protected override void Process2(float delta, RenderSurface nextSlideSurface, int frame, Rectangle? innerRegion)
        {
            if (mLength != mTheEffect.Length)
            {
                mTheEffect.Length = mLength;
            }

            mTheEffect.SimpleProcess2(delta, nextSlideSurface, frame, innerRegion);
        }

         //*******************************************************************
        protected override void ProcessDualRenderSurfaces(float delta, RenderSurface thisSlideSurface, RenderSurface nextSlideSurface, int frame)
        {
            if (mLength != mTheEffect.Length)
            {
                mTheEffect.Length = mLength;
            }

            mTheEffect.SimpleProcessDualRenderSurfaces(delta, thisSlideSurface, nextSlideSurface, frame);
        }

		//*******************************************************************
        public override void Save(XmlElement parent, XmlDocument doc, XmlElement effect)
		{
			effect.SetAttribute("Type","RandomTransitionEffect");
			SaveTransitionPart(effect,doc);
			parent.AppendChild(effect); 
		}

		
		//*******************************************************************
		public override void Load(XmlElement element)
		{
			LoadTransitionPart(element);

            // re-generate again, to get lengths and that correct etc
            GenerateRandomEffect();
		}
	}
}
