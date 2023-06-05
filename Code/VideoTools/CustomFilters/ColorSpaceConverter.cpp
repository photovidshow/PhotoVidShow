#include "ColorSpaceConverter.h"
#include "../UnmanagedErrors.h"

static long int crv_tab[256];
static long int cbu_tab[256];
static long int cgu_tab[256];
static long int cgv_tab[256];
static long int tab_76309[256];
static unsigned char clp[1024];	


static void InitConvertTable()
{
   long int crv,cbu,cgu,cgv;
   int i,ind;   
     
   crv = 104597; cbu = 132201;
   cgu = 25675;  cgv = 53279;
  
   for (i = 0; i < 256; i++) 
   {
      crv_tab[i] = (i-128) * crv;
      cbu_tab[i] = (i-128) * cbu;
      cgu_tab[i] = (i-128) * cgu;
      cgv_tab[i] = (i-128) * cgv;
      tab_76309[i] = 76309*(i-16);
   }
	 
   for (i=0; i<384; i++)
	  clp[i] =0;
   ind=384;
   for (i=0;i<256; i++)
	   clp[ind++]=i;
   ind=640;
   for (i=0;i<384;i++)
	   clp[ind++]=255;
}

static inline void NV12_to_RGB32(BYTE* luma, BYTE* uv, BYTE* dst, int width, int height)
{
	int y1,y2,u,v; 
	unsigned char *py1,*py2;
	int i,j, c1, c2, c3, c4;
	unsigned char *d1, *d2;

	py1 = luma;
	py2 = py1 + width;
	d1 = dst + ((4*width*height) - (4*width));
	d2 = d1 - 4 * width;
 	for (j = 0; j < height; j += 2)
	{ 
		for (i = 0; i < width; i += 2) 
		{
			v = *uv++;
			u = *uv++;

			c1 = crv_tab[v];
			c2 = cgu_tab[u];
			c3 = cgv_tab[v];
			c4 = cbu_tab[u];

			//up-left
            y1 = tab_76309[*py1++];	
			*d1++ = clp[384+((y1 + c1)>>16)];  
			*d1++ = clp[384+((y1 - c2 - c3)>>16)];
            *d1++ = clp[384+((y1 + c4)>>16)];
			*d1++ = 0xff;

			//down-left
			y2 = tab_76309[*py2++];
			*d2++ = clp[384+((y2 + c1)>>16)];  
			*d2++ = clp[384+((y2 - c2 - c3)>>16)];
            *d2++ = clp[384+((y2 + c4)>>16)];
			*d2++ = 0xff;

			//up-right
			y1 = tab_76309[*py1++];
			*d1++ = clp[384+((y1 + c1)>>16)];  
			*d1++ = clp[384+((y1 - c2 - c3)>>16)];
			*d1++ = clp[384+((y1 + c4)>>16)];
			*d1++ = 0xff;

			//down-right
			y2 = tab_76309[*py2++];
			*d2++ = clp[384+((y2 + c1)>>16)];  
			*d2++ = clp[384+((y2 - c2 - c3)>>16)];
            *d2++ = clp[384+((y2 + c4)>>16)];
			*d2++ = 0xff;
		}
		 
		d1 -= 12 * width;
		d2 -= 12 * width;
		py1+=   width;
		py2+=   width;
	}       
}


static inline void NV12_to_RGB32_Quarter(BYTE* luma, BYTE* uv, BYTE* dst, int width, int height)
{
	int y1, u, v;
	unsigned char *py1;
	int i, j, c1, c2, c3, c4;
	unsigned char *d1;

	int half_width = width >> 1;
	int half_height = height >> 1;

	py1 = luma;
	d1 = dst + ((4 * half_width*half_height) - (4 * half_width));

	for (j = 0; j < height; j += 2)
	{
		for (i = 0; i < width; i += 2)
		{
			v = *uv++;
			u = *uv++;

			c1 = crv_tab[v];
			c2 = cgu_tab[u];
			c3 = cgv_tab[v];
			c4 = cbu_tab[u];

			//up-left
			y1 = tab_76309[*py1++];

			*d1++ = clp[384 + ((y1 + c1) >> 16)];
			*d1++ = clp[384 + ((y1 - c2 - c3) >> 16)];
			*d1++ = clp[384 + ((y1 + c4) >> 16)];
			*d1++ = 0xff;
			*py1++;
		}

		d1 -= 8 * half_width;
		py1 += width;
	}
}


//----------------------------------------------------------------------------
// Constructor
//----------------------------------------------------------------------------
CColorSpaceConverter::CColorSpaceConverter(const GUID mediaType, int width, int height)
{
	InitConvertTable();

	m_mediaType = mediaType;
	m_width = width;
	m_height = height;
	m_uPlanePos = width * height;
	m_vPlanePos = m_uPlanePos + m_uPlanePos / 4;
}


//----------------------------------------------------------------------------
// Destructor
//----------------------------------------------------------------------------
CColorSpaceConverter::~CColorSpaceConverter(void)
{
}

//----------------------------------------------------------------------------
// convert_to_rgb32
//----------------------------------------------------------------------------
void CColorSpaceConverter::convert_to_rgb32(BYTE* outFrameBuffer, BYTE* inSampleBuffer, int outFrameBufferSize, int inSampleBufferSize, BOOL quarter)
{
	if (m_mediaType == MEDIASUBTYPE_RGB32)
	{
		//
		// Just in case we're passed in crap
		//
		if ( outFrameBufferSize > inSampleBufferSize)
		{
			WarningOnce("ColorSpaceConverter::convert_to_rgb32 output buffer bigger than input buffer size"));
			memset(outFrameBuffer,0xd8,outFrameBufferSize);
			return;
		}	

		if (quarter == TRUE)
		{
			unsigned int* ofb = (unsigned int* )outFrameBuffer;
			unsigned int* ifb = (unsigned int* )inSampleBuffer;

			for (int h = 0; h < m_height; h += 2)
			{
				for (int w = 0; w < m_width; w+=2)
				{
					*ofb++ = *ifb;
					ifb+=2;
				}
				ifb += m_width;
			}
		}
		else
		{
			memcpy(outFrameBuffer, inSampleBuffer, outFrameBufferSize);
		}
	}
	else if(m_mediaType == MEDIASUBTYPE_NV12)
	{
		if (((m_width*m_height) & 0x7) == 0)
		{
			//
			// Just in case we're passed in crap
			//
			if ((((m_width*m_height) >> 3)*12) > inSampleBufferSize)
			{
				WarningOnce("ColorSpaceConverter::convert_to_rgb32 width and height of video does not match input buffer size"));
				memset(outFrameBuffer,0x78,outFrameBufferSize);			
				return;
			}

			if (quarter == TRUE)
			{
				NV12_to_RGB32_Quarter(inSampleBuffer, &inSampleBuffer[m_uPlanePos], outFrameBuffer, m_width, m_height);
			}
			else
			{
				NV12_to_RGB32(inSampleBuffer, &inSampleBuffer[m_uPlanePos], outFrameBuffer, m_width, m_height);
			}
		}	
	}
	
	return ;
}
