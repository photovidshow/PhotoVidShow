using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DVDSlideshow;
using DVDSlideshow.GraphicsEng;
using DVDSlideshow.GraphicsEng.Direct3D;
using System.Collections.Generic;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for ImageCacheViewer.
	/// </summary>
	public class ImageCacheViewer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox mImageCacheListBox;
        private Panel panel1;
        private Button mClearImageCacheButton;
        private Button mClearDXTexturesButton;
        private ListBox mTexturesLoadedListBox;
        private ListBox mD3DSurfaceCacheListBox;
        private Button mRefreshButton;
        private Button mClearCachedPixelShadersButton;
        private CheckBox mForcePower2Textures;
        private CheckBox mManagedPoolTexturesCheckBox;
        private Label mMaxTextureSizeLabel;
        private Label mDoes64BitTexturesLabel;
        private Label mAutoMipmapGenLabel;
        private Label mPixelShader2SupportLabel;
        private Label mTriLinearSupportLabel;
        private Label mNonPower2TexturesSupportLabel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        //*******************************************************************
		public ImageCacheViewer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            /*
			CImagesCache.GetInstance().StateChanged+=new ManagedCore.StateChangeHandler(this.ImageCacheStateChange);

            Direct3DDevice device = GraphicsEngine.Current as Direct3DDevice ;
            if (device != null)
            {
                device.D3DSurfaceCache.StateChanged += new ManagedCore.StateChangeHandler(this.D3DSurfaceCacheStateChange);
                device.TextureManager.StateChanged += new ManagedCore.StateChangeHandler(this.TextureManagerStateChange);
            }
             */

            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge!=null)
            {

                mForcePower2Textures.Checked = ge.ForcePower2Textures;
           
                Direct3DDevice d3ddevice = ge as Direct3DDevice;
                if (d3ddevice != null)
                {
                    mManagedPoolTexturesCheckBox.Checked = d3ddevice.Settings.GetUseManagedPoolForNormalTextures();

                    mMaxTextureSizeLabel.Text = "MaxTex size:" + d3ddevice.Capabilities.GetMaxTextureWidth().ToString() + "," + d3ddevice.Capabilities.GetMaxTextureHeight().ToString();
                    mDoes64BitTexturesLabel.Text = "64 bit Tex:" + d3ddevice.Capabilities.GetCanDo64BitTextures().ToString();
                    mAutoMipmapGenLabel.Text = "Auto mipmap:" + d3ddevice.Capabilities.GetCanDoAutoMipMappingCreation().ToString();
                    mPixelShader2SupportLabel.Text = "ps 2.0:" + d3ddevice.Capabilities.GetCanDoPixelShader20().ToString();
                    mTriLinearSupportLabel.Text = "Tex Tri filter:" + d3ddevice.Capabilities.GetCanDoTrilinearFiltering().ToString();
                    mNonPower2TexturesSupportLabel.Text = "NonP2 Tex:" + d3ddevice.Capabilities.GetCanDoNonPower2Textures().ToString();

                }
            }


			ReDrawImageCacheListBox();
            ReDrawD3DSurfaceCacheListBox();
            ReDrawTextureManagerListBox();




			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        //*******************************************************************
		public void ImageCacheStateChange()
		{
			ReDrawImageCacheListBox();
		}

        //*******************************************************************
        public void D3DSurfaceCacheStateChange()
        {
            ReDrawD3DSurfaceCacheListBox();
        }

        public void TextureManagerStateChange()
        {
            ReDrawTextureManagerListBox();
        }


        private delegate void ReDrawListBoxDelegate();

        //*******************************************************************
		public void ReDrawImageCacheListBox()
		{
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new ReDrawListBoxDelegate(ReDrawImageCacheListBox));
                return;
            }

			this.mImageCacheListBox.Items.Clear(); 

			Hashtable h =CImagesCache.GetInstance().CachedImages;
			
			int used = CImagesCache.GetInstance().Usage;
			int max = CImagesCache.GetInstance().MaxSize;

			string ss = "Used ="+used+" Max ="+max;

			this.mImageCacheListBox.Items.Add(ss);

			ICollection c = h.Values;

			foreach (CMemoryCachedImage xmi in c)
			{
                ss = xmi.mName + " " + xmi.GetSizeInBytes().ToString();
				this.mImageCacheListBox.Items.Add(ss);
			}      
		}

        //*******************************************************************
        public void ReDrawTextureManagerListBox()
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new ReDrawListBoxDelegate(ReDrawTextureManagerListBox));
                return;
            }

            this.mTexturesLoadedListBox.SuspendLayout();
            try
            {
                this.mTexturesLoadedListBox.Items.Clear();

                Direct3DDevice device = GraphicsEngine.Current as Direct3DDevice;
                if (device == null) return;

                Dictionary<CImage, Texture> textures = device.TextureManager.GetCurrentTextures();

                int totalusage = 0;

                mTexturesLoadedListBox.Items.Add("");

                foreach (KeyValuePair<CImage, Texture> pair in textures)
                {
                    Texture t  = pair.Value;

                    mTexturesLoadedListBox.Items.Add(t.GetDetailsString() + "(" + pair.Key.ImageFilename + ")");
                    totalusage += t.GetMemoryUsage();
                }

                totalusage /= 1024;
                totalusage /=1024;

                mTexturesLoadedListBox.Items[0] = textures.Count.ToString() + " Textures loaded, memory=" + totalusage.ToString()+"MB , D3D available=" +device.GetAvailableTextureMem().ToString();

            }
            finally
            {
                this.mTexturesLoadedListBox.ResumeLayout();
            }
        }

        //*******************************************************************
        public void ReDrawD3DSurfaceCacheListBox()
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new ReDrawListBoxDelegate(ReDrawD3DSurfaceCacheListBox));
                return;
            }

            mD3DSurfaceCacheListBox.SuspendLayout();

            try
            {
                this.mD3DSurfaceCacheListBox.Items.Clear();

                Direct3DDevice device = GraphicsEngine.Current as Direct3DDevice;
                if (device == null) return;

                int totalusage = 0;

                mD3DSurfaceCacheListBox.Items.Add("");

                mD3DSurfaceCacheListBox.Items.Add("----IN USE----");

                List<D3DSurface> currentSurface = D3DSurface.GetCurrentSurfaces();
                foreach (D3DSurface surface in currentSurface)
                {
                    mD3DSurfaceCacheListBox.Items.Add(surface.GetDetailsString());
                    totalusage += surface.GetMemoryUsage();
                }

                mD3DSurfaceCacheListBox.Items.Add("----CACHED----");

                List<D3DSurface> cachedSurfaces = device.D3DSurfaceCache.GetCachedSurfaces();
                foreach (D3DSurface surface in cachedSurfaces)
                {
                    mD3DSurfaceCacheListBox.Items.Add(surface.GetDetailsString());
                    totalusage += surface.GetMemoryUsage();
                }

                int surfaces_in_use = currentSurface.Count + cachedSurfaces.Count;

                totalusage /= 1024;
                totalusage /= 1024;

                mD3DSurfaceCacheListBox.Items[0] = surfaces_in_use.ToString() + " Surfaces cached, memory="+totalusage.ToString()+"MB";
            }
            finally
            {
                mD3DSurfaceCacheListBox.ResumeLayout();
            }
        }


        //*******************************************************************
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
        /// 
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.mImageCacheListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mTexturesLoadedListBox = new System.Windows.Forms.ListBox();
            this.mD3DSurfaceCacheListBox = new System.Windows.Forms.ListBox();
            this.mClearImageCacheButton = new System.Windows.Forms.Button();
            this.mClearDXTexturesButton = new System.Windows.Forms.Button();
            this.mRefreshButton = new System.Windows.Forms.Button();
            this.mClearCachedPixelShadersButton = new System.Windows.Forms.Button();
            this.mForcePower2Textures = new System.Windows.Forms.CheckBox();
            this.mManagedPoolTexturesCheckBox = new System.Windows.Forms.CheckBox();
            this.mMaxTextureSizeLabel = new System.Windows.Forms.Label();
            this.mDoes64BitTexturesLabel = new System.Windows.Forms.Label();
            this.mAutoMipmapGenLabel = new System.Windows.Forms.Label();
            this.mPixelShader2SupportLabel = new System.Windows.Forms.Label();
            this.mTriLinearSupportLabel = new System.Windows.Forms.Label();
            this.mNonPower2TexturesSupportLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mImageCacheListBox
            // 
            this.mImageCacheListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.mImageCacheListBox.Location = new System.Drawing.Point(0, 0);
            this.mImageCacheListBox.Name = "mImageCacheListBox";
            this.mImageCacheListBox.Size = new System.Drawing.Size(762, 264);
            this.mImageCacheListBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.mTexturesLoadedListBox);
            this.panel1.Controls.Add(this.mD3DSurfaceCacheListBox);
            this.panel1.Controls.Add(this.mImageCacheListBox);
            this.panel1.Location = new System.Drawing.Point(12, 77);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(762, 1106);
            this.panel1.TabIndex = 1;
            // 
            // mTexturesLoadedListBox
            // 
            this.mTexturesLoadedListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.mTexturesLoadedListBox.Location = new System.Drawing.Point(0, 489);
            this.mTexturesLoadedListBox.Name = "mTexturesLoadedListBox";
            this.mTexturesLoadedListBox.Size = new System.Drawing.Size(762, 342);
            this.mTexturesLoadedListBox.TabIndex = 2;
            // 
            // mD3DSurfaceCacheListBox
            // 
            this.mD3DSurfaceCacheListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.mD3DSurfaceCacheListBox.Location = new System.Drawing.Point(0, 264);
            this.mD3DSurfaceCacheListBox.Name = "mD3DSurfaceCacheListBox";
            this.mD3DSurfaceCacheListBox.Size = new System.Drawing.Size(762, 225);
            this.mD3DSurfaceCacheListBox.TabIndex = 1;
            // 
            // mClearImageCacheButton
            // 
            this.mClearImageCacheButton.Location = new System.Drawing.Point(12, 11);
            this.mClearImageCacheButton.Name = "mClearImageCacheButton";
            this.mClearImageCacheButton.Size = new System.Drawing.Size(113, 23);
            this.mClearImageCacheButton.TabIndex = 2;
            this.mClearImageCacheButton.Text = "Clear image cache";
            this.mClearImageCacheButton.UseVisualStyleBackColor = true;
            this.mClearImageCacheButton.Click += new System.EventHandler(this.mClearImageCacheButton_Click);
            // 
            // mClearDXTexturesButton
            // 
            this.mClearDXTexturesButton.Location = new System.Drawing.Point(131, 11);
            this.mClearDXTexturesButton.Name = "mClearDXTexturesButton";
            this.mClearDXTexturesButton.Size = new System.Drawing.Size(107, 23);
            this.mClearDXTexturesButton.TabIndex = 3;
            this.mClearDXTexturesButton.Text = "Clear DX textures";
            this.mClearDXTexturesButton.UseVisualStyleBackColor = true;
            this.mClearDXTexturesButton.Click += new System.EventHandler(this.mClearDXTexturesButton_Click);
            // 
            // mRefreshButton
            // 
            this.mRefreshButton.Location = new System.Drawing.Point(677, 11);
            this.mRefreshButton.Name = "mRefreshButton";
            this.mRefreshButton.Size = new System.Drawing.Size(97, 23);
            this.mRefreshButton.TabIndex = 4;
            this.mRefreshButton.Text = "Refresh Lists";
            this.mRefreshButton.UseVisualStyleBackColor = true;
            this.mRefreshButton.Click += new System.EventHandler(this.mRefreshButton_Click);
            // 
            // mClearCachedPixelShadersButton
            // 
            this.mClearCachedPixelShadersButton.Location = new System.Drawing.Point(244, 11);
            this.mClearCachedPixelShadersButton.Name = "mClearCachedPixelShadersButton";
            this.mClearCachedPixelShadersButton.Size = new System.Drawing.Size(147, 23);
            this.mClearCachedPixelShadersButton.TabIndex = 5;
            this.mClearCachedPixelShadersButton.Text = "Clear cached pixel shaders";
            this.mClearCachedPixelShadersButton.UseVisualStyleBackColor = true;
            this.mClearCachedPixelShadersButton.Click += new System.EventHandler(this.mClearCachedPixelShadersButton_Click);
            // 
            // mForcePower2Textures
            // 
            this.mForcePower2Textures.AutoSize = true;
            this.mForcePower2Textures.Location = new System.Drawing.Point(397, 15);
            this.mForcePower2Textures.Name = "mForcePower2Textures";
            this.mForcePower2Textures.Size = new System.Drawing.Size(131, 17);
            this.mForcePower2Textures.TabIndex = 6;
            this.mForcePower2Textures.Text = "Force power2 textures";
            this.mForcePower2Textures.UseVisualStyleBackColor = true;
            this.mForcePower2Textures.CheckedChanged += new System.EventHandler(this.mForcePower2Textures_CheckedChanged);
            // 
            // mManagedPoolTexturesCheckBox
            // 
            this.mManagedPoolTexturesCheckBox.AutoSize = true;
            this.mManagedPoolTexturesCheckBox.Location = new System.Drawing.Point(534, 15);
            this.mManagedPoolTexturesCheckBox.Name = "mManagedPoolTexturesCheckBox";
            this.mManagedPoolTexturesCheckBox.Size = new System.Drawing.Size(135, 17);
            this.mManagedPoolTexturesCheckBox.TabIndex = 7;
            this.mManagedPoolTexturesCheckBox.Text = "Managed Pool textures";
            this.mManagedPoolTexturesCheckBox.UseVisualStyleBackColor = true;
            this.mManagedPoolTexturesCheckBox.CheckedChanged += new System.EventHandler(this.mManagedPoolTexturesCheckBox_CheckedChanged);
            // 
            // mMaxTextureSizeLabel
            // 
            this.mMaxTextureSizeLabel.AutoSize = true;
            this.mMaxTextureSizeLabel.Location = new System.Drawing.Point(13, 41);
            this.mMaxTextureSizeLabel.Name = "mMaxTextureSizeLabel";
            this.mMaxTextureSizeLabel.Size = new System.Drawing.Size(120, 13);
            this.mMaxTextureSizeLabel.TabIndex = 8;
            this.mMaxTextureSizeLabel.Text = "MaxTex size 8000,8000";
            // 
            // mDoes64BitTexturesLabel
            // 
            this.mDoes64BitTexturesLabel.AutoSize = true;
            this.mDoes64BitTexturesLabel.Location = new System.Drawing.Point(148, 41);
            this.mDoes64BitTexturesLabel.Name = "mDoes64BitTexturesLabel";
            this.mDoes64BitTexturesLabel.Size = new System.Drawing.Size(75, 13);
            this.mDoes64BitTexturesLabel.TabIndex = 9;
            this.mDoes64BitTexturesLabel.Text = "64 bit Tex:true";
            // 
            // mAutoMipmapGenLabel
            // 
            this.mAutoMipmapGenLabel.AutoSize = true;
            this.mAutoMipmapGenLabel.Location = new System.Drawing.Point(239, 41);
            this.mAutoMipmapGenLabel.Name = "mAutoMipmapGenLabel";
            this.mAutoMipmapGenLabel.Size = new System.Drawing.Size(89, 13);
            this.mAutoMipmapGenLabel.TabIndex = 10;
            this.mAutoMipmapGenLabel.Text = "Auto mipmap:true";
            // 
            // mPixelShader2SupportLabel
            // 
            this.mPixelShader2SupportLabel.AutoSize = true;
            this.mPixelShader2SupportLabel.Location = new System.Drawing.Point(334, 41);
            this.mPixelShader2SupportLabel.Name = "mPixelShader2SupportLabel";
            this.mPixelShader2SupportLabel.Size = new System.Drawing.Size(57, 13);
            this.mPixelShader2SupportLabel.TabIndex = 11;
            this.mPixelShader2SupportLabel.Text = "ps 2.0:true";
            // 
            // mTriLinearSupportLabel
            // 
            this.mTriLinearSupportLabel.AutoSize = true;
            this.mTriLinearSupportLabel.Location = new System.Drawing.Point(406, 41);
            this.mTriLinearSupportLabel.Name = "mTriLinearSupportLabel";
            this.mTriLinearSupportLabel.Size = new System.Drawing.Size(83, 13);
            this.mTriLinearSupportLabel.TabIndex = 12;
            this.mTriLinearSupportLabel.Text = "Tex Tri filter:true";
            // 
            // mNonPower2TexturesSupportLabel
            // 
            this.mNonPower2TexturesSupportLabel.AutoSize = true;
            this.mNonPower2TexturesSupportLabel.Location = new System.Drawing.Point(506, 41);
            this.mNonPower2TexturesSupportLabel.Name = "mNonPower2TexturesSupportLabel";
            this.mNonPower2TexturesSupportLabel.Size = new System.Drawing.Size(86, 13);
            this.mNonPower2TexturesSupportLabel.TabIndex = 13;
            this.mNonPower2TexturesSupportLabel.Text = "NonP2 Tex:True";
            // 
            // ImageCacheViewer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(781, 941);
            this.Controls.Add(this.mNonPower2TexturesSupportLabel);
            this.Controls.Add(this.mTriLinearSupportLabel);
            this.Controls.Add(this.mPixelShader2SupportLabel);
            this.Controls.Add(this.mAutoMipmapGenLabel);
            this.Controls.Add(this.mDoes64BitTexturesLabel);
            this.Controls.Add(this.mMaxTextureSizeLabel);
            this.Controls.Add(this.mManagedPoolTexturesCheckBox);
            this.Controls.Add(this.mForcePower2Textures);
            this.Controls.Add(this.mClearCachedPixelShadersButton);
            this.Controls.Add(this.mRefreshButton);
            this.Controls.Add(this.mClearDXTexturesButton);
            this.Controls.Add(this.mClearImageCacheButton);
            this.Controls.Add(this.panel1);
            this.Name = "ImageCacheViewer";
            this.Text = "Image and texture caches";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        //*******************************************************************
        private void mClearDXTexturesButton_Click(object sender, EventArgs e)
        {
            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge != null)
            {
                Direct3DDevice d3ddevice = ge as Direct3DDevice;
                if (d3ddevice != null)
                {
                    d3ddevice.TextureManager.Clean();
                }
            }
        }

        //*******************************************************************
        private void mClearImageCacheButton_Click(object sender, EventArgs e)
        {
            CImagesCache.GetInstance().ClearCache();
        }

        //*******************************************************************
        private void mRefreshButton_Click(object sender, EventArgs e)
        {
            ReDrawImageCacheListBox();
            ReDrawD3DSurfaceCacheListBox();
            ReDrawTextureManagerListBox();
        }

        //*******************************************************************
        private void mClearCachedPixelShadersButton_Click(object sender, EventArgs e)
        {
            PixelShaderEffectDatabase.GetInstance().Clear();
        }

        //*******************************************************************
        private void mForcePower2Textures_CheckedChanged(object sender, EventArgs e)
        {
            GraphicsEngine.Current.ForcePower2Textures = mForcePower2Textures.Checked;
        }

        //*******************************************************************
        private void mManagedPoolTexturesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GraphicsEngine ge = GraphicsEngine.Current;
            if (ge != null)
            {
                Direct3DDevice d3ddevice = ge as Direct3DDevice;
                if (d3ddevice != null)
                {
                    d3ddevice.Settings.SetUseManagedPoolForNormalTextures(mManagedPoolTexturesCheckBox.Checked);
                }
            }
        }
	}
}
