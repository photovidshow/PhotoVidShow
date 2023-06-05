using System;
using System.Collections.Generic;

namespace DVDSlideshow.GraphicsEng
{
    public class PixelShaderEffectDatabase
    {
        private Dictionary<string, PixelShaderEffect> mMap = new Dictionary<string, PixelShaderEffect>();
        private static PixelShaderEffectDatabase mInstance = null;

        //******************************************************************************
        private PixelShaderEffectDatabase()
        {
        }

        //******************************************************************************
        public static PixelShaderEffectDatabase GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new PixelShaderEffectDatabase();
            }
            return mInstance;
        }

        //******************************************************************************
        public PixelShaderEffect GetPixelShader(string name)
        {
            PixelShaderEffect effect = null;

            if ( mMap.TryGetValue(name, out effect) ==true)
            {
                return effect;
            }

            effect = GraphicsEngine.Current.CreatePixelShaderEffect(name);

            if (effect != null)
            {
                mMap.Add(name, effect);
            }
            return effect;
        }

        //******************************************************************************
        public void Clear()
        {
            foreach (KeyValuePair<string, PixelShaderEffect> effect in mMap)
            {
                effect.Value.Dispose();
            }
            mMap.Clear();
        }
    }
}
