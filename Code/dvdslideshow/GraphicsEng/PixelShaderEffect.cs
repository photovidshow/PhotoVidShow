using System;

namespace DVDSlideshow.GraphicsEng
{
    public abstract class PixelShaderEffect : IDisposable
    {

        public PixelShaderEffect(string name)
        {
            mName = name;
        }

        public abstract void SetParameter(int index, float value);
        public abstract void SetInputImage1(DrawableObject drawableObject);

        private string mName; // name of shader (and shader file )

        public abstract void Dispose();

    }
}
