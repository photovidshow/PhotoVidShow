using System;
using System.Collections.Generic;
using System.Text;
using ManagedCore;
using MangedToUnManagedWrapper.ManagedDirect3D;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    public class D3DSettings
    {
        private ManagedDirect3DSettings mManagedSettings;

        // ************************************************************************************************************
        public D3DSettings()
        {
            mManagedSettings = new ManagedDirect3DSettings();
        }

        // ************************************************************************************************************
        public bool GetUseManagedPoolForNormalTextures()
        {
            return mManagedSettings.GetUseManagedPoolForNormalTextures();
        }

        // ************************************************************************************************************
        public void SetUseManagedPoolForNormalTextures(bool value)
        {
            mManagedSettings.SetUseManagedPoolForNormalTextures(value);
        }
    }

}
