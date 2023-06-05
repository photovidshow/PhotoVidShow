// DSCodec.h: interface for the CDSCodec class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_DSCODEC_H__08AD6127_616C_4925_9BC6_61DB24964EF3__INCLUDED_)
#define AFX_DSCODEC_H__08AD6127_616C_4925_9BC6_61DB24964EF3__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

//#include "dshow.h"
#include "DSCodecFormat.h"

class CDSCodec 
{

	
public:
	CDSCodec();
	virtual ~CDSCodec();

	//unsigned short m_szCodecName;
	IMoniker	*m_pMoniker;
	CDSCodecFormat *mTheCodecFormat44100 ;
	CDSCodecFormat *mTheCodecFormat48000 ;

	void BuildCodecFormatArray();
};

#endif // !defined(AFX_DSCODEC_H__08AD6127_616C_4925_9BC6_61DB24964EF3__INCLUDED_)
