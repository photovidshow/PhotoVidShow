using System;
using System.Collections.Generic;
using System.Text;

namespace DVDSlideshow.GraphicsEng
{
    public struct GraphicsEngineCapabilities
    {
        public bool CanDoPixelShader20;
        public bool CanDo64BitTextures;
        public bool GetCanDoTrilinearFiltering;
        public bool CanDoAutoMipMappingCreation;
        public bool CanDoNonPower2Textures;			// This also includes when doing MipMapped textures
        public int MaxTextureWidth;
        public int MaxTextureHeight;
    }
}
