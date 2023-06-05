using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ManagedCore;

namespace DVDSlideshow
{
    public abstract class CEquation 
    { 
        public abstract float Get(float start, float end, float delta) ;

        public abstract void Save(XmlElement parent, XmlDocument doc);
        public abstract void Load(XmlElement element);

        public static CEquation CreateFromType(string type)
        {
            if (type == "Linear") return new CLinear();
            if (type == "NonLinear") return new CNonLinear();
            if (type == "Spring") return new CSpring();
            if (type == "QuickSlow") return new CQuickSlow();
    
            CDebugLog.GetInstance().Error("Trying to create unknown equation type:" + type);

            return new CLinear();
        }

    }
}
