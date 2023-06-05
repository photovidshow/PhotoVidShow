using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DVDSlideshow.GraphicsEng.Direct3D
{
    public class TextureManager
    {
        private object mLock = new object();

        // Dictionary for fast lookup
        private Dictionary<CImage, Texture> mMap = new Dictionary<CImage, Texture>();
        // Linked list to represent age of texture
        private List<CImage> mCacheImageOrder = new List<CImage>();

        public event ManagedCore.StateChangeHandler StateChanged;
        private bool mForcePower2Textures = false;

        private const int MaxTextureUsageWhenEncoding               = 128;   // mb when encoding don't exceed this
        private const int UseD3dMangedPoolThreashold                = 128;   // mb if less than this available use d3d managed pool
        private const int MaxTextureUsage                           = 320;   // mb don't exceed this (i.e. when editing)
        private const int TextureMemoryCushion                      = 200;   // mb, akways try to leave d3d this amount
        private const int MinAmountOfTextureLoadedBeforeEvicting  = 128;     // mb, 

        //******************************************************************************************************
        // If set, it will force textures to be created at sizes of power2, this us usefull to test this system
        // to imitate graphics cards that would otherwise force this anyway.
        public bool ForcePower2Textures
        {
            get { return mForcePower2Textures; }
            set { mForcePower2Textures = value; }
        }
   
        //******************************************************************************************************
        public Dictionary<CImage, Texture> GetCurrentTextures()
        {
            lock (mLock)
            {
                Dictionary<CImage, Texture> clonedDictionary = new Dictionary<CImage, Texture>(mMap);
                return clonedDictionary;
            }
        }

        //******************************************************************************************************
        public Texture GetTextureForImage(CImage image, SizeF requestedDimension)
        {
            lock (mLock)
            {
                Texture texture = null;

                if (mMap.TryGetValue(image, out texture) == false)
                {
                    texture = CreateTexture(image, requestedDimension);

                    return texture;
                }

                if (image.ImageFilename != "")
                {
                    bool usesTooMuchMemory = false;

                    SizeF dimensionToLoad = image.CalcImageDimensionToLoad(requestedDimension, out usesTooMuchMemory);
                    if (((int)dimensionToLoad.Width) > (texture.Width+1) || ((int)dimensionToLoad.Height) > (texture.Height+1))
                    {
                        // ok if we've going to reload texture try and make it 50% bigger
                        requestedDimension.Height *= 1.5f;
                        requestedDimension.Width *= 1.5f;

                        // recalc dimention to load
                        dimensionToLoad = image.CalcImageDimensionToLoad(requestedDimension, out usesTooMuchMemory);

                        ManagedCore.Log.Info("Re-loading texture at higher res, image = " + image.ImageFilename + " old=" + texture.Width + "," + texture.Height + " new=" + dimensionToLoad.Width + "," + dimensionToLoad.Height);
                        if (mMap.Remove(image))
                        {
                            texture.Dispose();
                        }
                        mCacheImageOrder.Remove(image);

                        texture = CreateTexture(image, dimensionToLoad);

                    }
                }

                return texture;
            }
        }

        //******************************************************************************************************
        public Texture GetTextureForImage(CImage image)
        {
            lock (mLock)
            {
                Texture texture = null;

                if (mMap.TryGetValue(image, out texture) == false)
                {
                    texture = CreateTexture(image, new Size());
                }

                return texture;
            }
        }

        //******************************************************************************************************
        // Gets or creates a texture for the given Cimage filename.  I.e. if any textures exists that has the
        // same CImage filename, then that is returned, else a new texture is created
        // Note : If the cimage has no filename a new texture is created.
        public Texture GetTextureForMatchingImageFilename(CImage image)
        {
            lock (mLock)
            {
                string filename = image.ImageFilename;
                if (filename == "")
                {
                    return GetTextureForImage(image);
                }

                foreach (KeyValuePair<CImage, Texture> pair in mMap)
                {
                    CImage keyImage = pair.Key;
                    if (keyImage.ImageFilename.ToLower() == filename.ToLower())
                    {
                        return pair.Value;
                    }
                }

                Texture texture = CreateTexture(image, new Size());

                return texture;
            }
             
        }


        //******************************************************************************************************
        // returns current texture usage in mb
        private int GetMemoryUsage()
        {
            int totalusage = 0;

            foreach (KeyValuePair<CImage, Texture> pair in mMap)
            {
                Texture t = pair.Value;
                totalusage += t.GetMemoryUsage();
            }

            totalusage /= 1024;
            totalusage /= 1024;

            return totalusage;
        }


        //******************************************************************************************************
        private Texture CreateTexture(CImage image, SizeF requestedDimesion)
        {
            uint textureMemoryReportedLeft = GetAvailableTextureMem();

            // If reports less that UseD3dMangedPoolThreashold uses d3d managed system (hopefully we don't need this)
            if (textureMemoryReportedLeft < UseD3dMangedPoolThreashold)
            {
                ManagedCore.Log.Warning("Using managed pool textures");
                (GraphicsEngine.Current as Direct3DDevice).Settings.SetUseManagedPoolForNormalTextures(true);
            }

            lock (mLock)
            {
                int usage = GetMemoryUsage();
                int maxRemoveal = 5;
                // Do we need to free up mem, set max cap to 1 gig textures.  Remove oldest one?

                if (usage >= MinAmountOfTextureLoadedBeforeEvicting)
                {
                    while ((usage > MaxTextureUsage || textureMemoryReportedLeft <= TextureMemoryCushion) && maxRemoveal > 0)
                    {
                        CImage first = mCacheImageOrder[0];
                        RemoveCachedTexuresForImage(first);
                      
                        usage = GetMemoryUsage();

                        if (usage < MinAmountOfTextureLoadedBeforeEvicting)
                        {
                            break;
                        }

                        textureMemoryReportedLeft = GetAvailableTextureMem();
                        maxRemoveal--;
                    }
                }

                Texture texture = new Texture(image, requestedDimesion, mForcePower2Textures);

                // The image may of changed now, such that the dictionary hash code for it already exists in out map.
                if (mMap.ContainsKey(image) == true)
                {
                    texture.Dispose();
                    return mMap[image];
                }

                try
                {
                    mMap.Add(image, texture);
                }
                catch (Exception e)
                {
                    if (mMap.ContainsKey(image))
                    {
                        ManagedCore.Log.Error("Texture manager map already contains texture for CImage");
                        return mMap[image];
                    }
                }

                mCacheImageOrder.Add(image);

                if (StateChanged != null) StateChanged();

                return texture;
            }
        }

        //******************************************************************************************************
        public void Clean()
        {
            lock (mLock)
            {
                foreach (KeyValuePair<CImage, Texture> pair in mMap)
                {
                    pair.Value.Dispose();
                }

                mMap.Clear();
                mCacheImageOrder.Clear();
                if (StateChanged != null) StateChanged();
            }
        }

        //******************************************************************************************************
        private uint GetAvailableTextureMem()
        {  
            Direct3DDevice device = GraphicsEngine.Current as Direct3DDevice;
            if (device != null)
            {
                uint amount =  device.GetAvailableTextureMem() /1024 /1024;

                // if encoding at high res, reduce to save memory
                if (CGlobals.mVideoGenerateMode == VideoGenerateMode.FINAL_OUTPUT_MODE)
                {
                    if (amount > MaxTextureUsageWhenEncoding)
                    {
                        amount = MaxTextureUsageWhenEncoding;
                    }
                }
                else
                {
                    // If using managed pool, set memory left to memory cushion,
                    if (device.Settings.GetUseManagedPoolForNormalTextures() == true)
                    {
                        amount = TextureMemoryCushion;
                    }
                }

                return amount;
            }
            return 0;
        }

        //*******************************************************************
        public void RemoveCachedTexuresForImage(CImage image)
        {
            mCacheImageOrder.Remove(image);

            Texture remove_texture = null;
            if (mMap.TryGetValue(image, out remove_texture))
            {
                mMap.Remove(image);
                remove_texture.Dispose();
            }
        }


    }
}
