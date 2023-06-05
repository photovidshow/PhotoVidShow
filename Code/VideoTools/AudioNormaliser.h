// AudioNormaliser.h: interface for the AudioNormaliser class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_AudioNORMALISER_H__4DF17E68_9222_4FFC_93FE_23DA333D040E__INCLUDED_)
#define AFX_AudioNORMALISER_H__4DF17E68_9222_4FFC_93FE_23DA333D040E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


class CBuffer;

// normalises a Audio buffer

class AudioNormaliser  
{
public:
	AudioNormaliser(CBuffer* buffer);
	virtual ~AudioNormaliser();

	static bool mAbort;

	void Process();


private:
	CBuffer* mBuffer;

};

#endif // !defined(AFX_AudioNORMALISER_H__4DF17E68_9222_4FFC_93FE_23DA333D040E__INCLUDED_)
