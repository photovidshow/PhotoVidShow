using System;
using System.Collections.Generic;
using System.Text;
using ManagedCore;
using MangedToUnManagedWrapper.ManagedDirect3D;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    public class D3DCapabilities
    {
        private ManagedDirect3DCapabilities mManagedCapabilities;

        // ************************************************************************************************************
        public D3DCapabilities()
        {
            mManagedCapabilities = new ManagedDirect3DCapabilities();
        }

        // ************************************************************************************************************
        public bool GetCanDoPixelShader20()
        {
            return mManagedCapabilities.GetCanDoPixelShader20();
        }

        // ************************************************************************************************************
        public bool GetCanDo64BitTextures()
        {
            return mManagedCapabilities.GetCanDo64BitTextures();
        }

        // ************************************************************************************************************
        public bool GetCanDoTrilinearFiltering()
        {
            return mManagedCapabilities.GetCanDoTrilinearFiltering();
        }

        // ************************************************************************************************************
        public bool GetCanDoAutoMipMappingCreation()
        {
            return mManagedCapabilities.GetCanDoAutoMipMappingCreation();
        }

        // ************************************************************************************************************
        public bool GetCanDoNonPower2Textures()
        {
            return mManagedCapabilities.GetCanDoNonPower2Textures();
        }

        // ************************************************************************************************************
        public int GetMaxTextureWidth()
        {
            return mManagedCapabilities.GetMaxTextureWidth();
        }

        // ************************************************************************************************************
        public int GetMaxTextureHeight()
        {
            return mManagedCapabilities.GetMaxTextureHeight();
        }

      
    }
}
