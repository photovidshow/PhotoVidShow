using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using ManagedCore;

namespace DVDSlideshow
{
    public class CAnimatedDecorationEffectDatabase
    {
        private List<CAnimatedDecorationEffect> mEffects = new List<CAnimatedDecorationEffect>();

        private static CAnimatedDecorationEffectDatabase mInstance = null;

        //*******************************************************************
        private CAnimatedDecorationEffectDatabase()
        {
        }

        //*******************************************************************
        public static CAnimatedDecorationEffectDatabase GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new CAnimatedDecorationEffectDatabase();
            }
            return mInstance;
        }

        //*******************************************************************
        public void AddEffect(CAnimatedDecorationEffect newEffect)
        {
            int insert =0;
            
            /*
            //
            // Add in alphabetic order
            //
            foreach (CAnimatedDecorationEffect effect in mEffects)
            {
                int r = string.Compare(newEffect.Name, effect.Name, true);
                if ( r < 0)
                {
                    break;
                }
                insert++;
            }

            mEffects.Insert(insert, newEffect);
             */

            mEffects.Add(newEffect);
        }

        
        //*******************************************************************
        public void AddEffects(List<CAnimatedDecorationEffect> effects)
        {
            foreach (CAnimatedDecorationEffect effect in effects)
            {
                AddEffect(effect);
            }
        }

        //*******************************************************************
        public void RemoveEffect(string name)
        {
            foreach (CAnimatedDecorationEffect effect in mEffects)
            {
                if (effect.Name == name)
                {
                    mEffects.Remove(effect);
                    return;
                }
            }
        }

        //*******************************************************************
        public List<CAnimatedDecorationEffect> Effects
        {
            get
            {
                return mEffects;
            }
        }

        //*******************************************************************
        public List<CAnimatedDecorationEffect> GetInEffects(bool getTemplateEffects)
        {
            List<CAnimatedDecorationEffect> newList = new List<CAnimatedDecorationEffect>();
            foreach (CAnimatedDecorationEffect animation in mEffects)
            {
                if (animation.AnimatedType == CAnimatedDecorationEffect.Type.BOTH_MOVE_IN_AND_OUT ||
                    animation.AnimatedType == CAnimatedDecorationEffect.Type.MOVE_IN ||
                    animation.TemplateOnlyEffect == true )
                {
                    if (getTemplateEffects == false && animation.TemplateOnlyEffect == true)
                    {
                        continue;
                    }

                    newList.Add(animation);
                }
            }
            return newList;
  
        }

        //*******************************************************************
        public List<CAnimatedDecorationEffect> GetOutEffects(bool getTemplateEffects)
        {
           
            List<CAnimatedDecorationEffect> newList = new List<CAnimatedDecorationEffect>();
            foreach (CAnimatedDecorationEffect animation in mEffects)
            {
                if (animation.AnimatedType == CAnimatedDecorationEffect.Type.BOTH_MOVE_IN_AND_OUT ||
                    animation.AnimatedType == CAnimatedDecorationEffect.Type.MOVE_OUT ||
                    animation.TemplateOnlyEffect == true)
                {
                    if (getTemplateEffects == false && animation.TemplateOnlyEffect == true)
                    {
                        continue;
                    }
                    newList.Add(animation);
                }
            }
            return newList;
     
        }

        //*******************************************************************
        public CAnimatedDecorationEffect Get(string name)
        {
            foreach (CAnimatedDecorationEffect effect in mEffects)
            {
                if (effect.Name == name)
                {
                    return effect;
                }
            }
            return null;
        }

        //*******************************************************************
        public bool EffectExists(string name)
        {
            if (Get(name) == null) return false;
            return true;
        }


        /*
        //*******************************************************************
        public void Export()
        {
            string effectsToSave = CGlobals.GetAnimatedDecorationsEffectsDirectory() + "\\moveeffects.xml";

            System.Xml.XmlDocument my_doc = new XmlDocument();

            XmlElement element = my_doc.CreateElement("MoveEffects");

            my_doc.AppendChild(element);

            Save(element, my_doc);

            my_doc.Save(effectsToSave);
        }
         */



        //*******************************************************************
        public void Inport()
        {
            string effectsToLoad = CGlobals.GetAnimatedDecorationsEffectsDirectory() + "\\moveeffects.xml";

            System.Xml.XmlDocument my_doc = new XmlDocument();
            try
            {
                my_doc.Load(effectsToLoad);
            }
            catch (Exception exception1)
            {
                Log.Error("Could not read move effects file:" + effectsToLoad + " " + exception1.Message);
                return;
            }

            // ok load decoration effects
            XmlNodeList decorationEffectList = my_doc.GetElementsByTagName("DecorationEffectsDatabase");

            if (decorationEffectList.Count > 0)
            {
                XmlElement element = decorationEffectList[0] as XmlElement;
                this.Append(element, false, false);
            }

            // create out effect from those loaded
            CrateOutEffectsFromNonTemplateInEffects();
        }

        //*******************************************************************
        public List<CAnimatedDecorationEffect> GetDependanciesForSlide(CSlideShow slideshow)
        {
            List<CAnimatedDecorationEffect> effects = new List<CAnimatedDecorationEffect>();
            foreach (CSlide slide in slideshow.mSlides)
            {
                CImageSlide imageSlide = slide as CImageSlide;
                if (imageSlide != null)
                {
                    ArrayList list = imageSlide.GetAllAndSubDecorations();

                    // groups also may contain animted effects
                    List<CGroupedDecoration> groups = imageSlide.GetAllGroupDecorations();
                    list.AddRange(groups.ToArray());

                    foreach (CDecoration d in list)
                    {
                        CAnimatedDecoration animatedDecoration = d as CAnimatedDecoration;

                        CAnimatedDecorationEffect moveInEffect = animatedDecoration.MoveInEffect;
                        if (moveInEffect != null && effects.Contains(moveInEffect) == false)
                        {
                            effects.Add(moveInEffect);

                            CAnimatedDecorationEffect UVeffect = moveInEffect.UVAnimatedEffect;
                            if (UVeffect != null && effects.Contains(UVeffect) == false)
                            {
                                effects.Add(UVeffect);
                            }
                        }

                        CAnimatedDecorationEffect moveOutEffect = animatedDecoration.MoveOutEffect;
                        if (moveOutEffect != null && effects.Contains(moveOutEffect) == false)
                        {
                            effects.Add(moveOutEffect);
                            CAnimatedDecorationEffect UVeffect = moveOutEffect.UVAnimatedEffect;
                            if (UVeffect != null && effects.Contains(UVeffect) == false)
                            {
                                effects.Add(UVeffect);
                            }
                        }
                    }
                }
            }

            return effects;
        }

        //*******************************************************************
        public void SaveOnlyRequiredBySlideshow(CSlideShow slideshow, XmlElement parent, XmlDocument doc)
        {
            XmlElement decorationEffectsDatabase = doc.CreateElement("DecorationEffectsDatabase");

            SaveEffectsRequiredBySlideshow(decorationEffectsDatabase,doc,slideshow);

            parent.AppendChild(decorationEffectsDatabase); 
        }

        //*******************************************************************
        private void SaveEffectsRequiredBySlideshow(XmlElement decorationEffectsDatabase, XmlDocument doc, CSlideShow slideshow)
        {
            List<CAnimatedDecorationEffect> list = GetDependanciesForSlide(slideshow);

            foreach (CAnimatedDecorationEffect effect in mEffects)
            {
                if (list.Contains(effect)== true)
                {
                    effect.Save(decorationEffectsDatabase, doc);
                }
            }
        }

        //*******************************************************************
		public void Save(XmlElement parent, XmlDocument doc)
		{
            XmlElement decorationEffectsDatabase = doc.CreateElement("DecorationEffectsDatabase");

            // ### SRG TODO for menu?

            foreach (CSlideShow slideshow in CGlobals.mCurrentProject.GetAllProjectSlideshows(true))
            {
                SaveEffectsRequiredBySlideshow(decorationEffectsDatabase, doc, slideshow);
            }

            parent.AppendChild(decorationEffectsDatabase); 
		}


		//*******************************************************************
        public void Load(XmlElement element)
        {
           // Effects.Clear();

            Append(element, false, false);

        }


        //*******************************************************************
        public void ClearAllButDefaults()
        {
            Effects.Clear();
            this.Inport();

        }

        //*******************************************************************
        public List<CAnimatedDecorationEffect> GetAllTemplateEffects()
        {
            List<CAnimatedDecorationEffect> returnList = new List<CAnimatedDecorationEffect>();
            foreach (CAnimatedDecorationEffect effect in mEffects)
            {
                if (effect.TemplateOnlyEffect == true)
                {
                    returnList.Add(effect);
                }
            }

            return returnList;
        }

        //*******************************************************************
        public void Append(XmlElement element, bool reportDuplicatesAsWarnings, bool appendFromTemplate)
        {
            XmlNodeList list = element.GetElementsByTagName("DecorationEffect");

            foreach (XmlElement e in list)
            {
                CAnimatedDecorationEffect effect = new CAnimatedDecorationEffect();

                effect.Load(e);

                if (EffectExists(effect.Name) == false)
                {
                    if (effect.TemplateOnlyEffect == false)
                    {
                        effect.TemplateOnlyEffect = appendFromTemplate;

                        if (appendFromTemplate == false)
                        {
                            effect.AnimatedType = CAnimatedDecorationEffect.Type.MOVE_IN;
                        }
                    }

                    Effects.Add(effect);
                }
                else if (reportDuplicatesAsWarnings == true)
                {
                    ManagedCore.Log.Warning("Animated effect '" + effect.Name + "' already exists in database (ingoring)");
                }
            }
        }

        //*******************************************************************
        private void CrateOutEffectsFromNonTemplateInEffects()
        {
            List<CAnimatedDecorationEffect> newEffects = new List<CAnimatedDecorationEffect>();

            foreach (CAnimatedDecorationEffect effect in mEffects)
            {
                if (effect.TemplateOnlyEffect == true || 
                    effect.AnimatedType != CAnimatedDecorationEffect.Type.MOVE_IN ||
                    effect.LengthSetToDecorationLength == true)
                {
                    continue;
                }

               CAnimatedDecorationEffect newEffect = CAnimatedDecorationEffect.CreateOutEffectFromInEffect(effect);

               newEffects.Add(newEffect);
            }

            mEffects.AddRange(newEffects);
        }
    }
}
