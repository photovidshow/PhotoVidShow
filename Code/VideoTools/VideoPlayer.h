#ifndef VIDEO_PLAYER_INCLUDE
#define VIDEO_PLAYER_INCLUDE

#include <streams.h>     // Active Movie (includes windows.h)
#include <initguid.h>    // declares DEFINE_GUID to declare an EXTERN_C const.


#include "qedit.h"


#include <stdio.h>
#include <windows.h>
#include <control.h>
//#include <qedit.h>
#include <atlbase.h>
#include <string>
#include <vfwmsgs.h>
#include "Errors.h"
#include "CustomFilters/grabber.h"


class CDirectShowPlayerCallback;

class CDirectShowPlayer
{
public:

	int mID;

	IPVSGrabber *pGrabber;
	IBaseFilter *pGrabberF;
	ISampleGrabberCB* pCB;
	IMediaSeeking* pSeeking;
	IBaseFilter *pFilter;
	IGraphBuilder *pGraph ;
    IMediaFilter *pMediaFilter;
	IMediaControl *pControl ;
	IMediaEvent   *pEvent ;
	IBaseFilter   *pNullRenderer;
	IBaseFilter	  *pDSoundRenderer;
	IBasicAudio	  *pBasicAudio;
	REFERENCE_TIME mDuration;
	CDirectShowPlayerCallback* pCallback;
	char* mFilename;

	HANDLE	mNextFrameReadySignal;
	HANDLE  mWaitForAdvanceFrameSingnal;
	IEnumFilters* mFiltersListIterator;
	
	bool	hack_just_seeked_waiting_to_render;
	bool	next_frame_ready ;
	bool	mFrameStepping ;
	int		mWidth;
	int		mHeight;
	int		mBpp;
	int		mPlanes;
	bool	mStopped ;
	bool	mStoppedAtStart;
	long    mCachedVolume;
	bool	mHasAudio;
	bool	mHasAC3;
	double	mFrameRate;
	double	mLastSampleTime;
	double  mLastSeekTime;
	bool    mRunningNullClock;

	int		mInternalVideoFrameNum; // the number of frames after start the video is on


	int hack_counter;
	int framesCountedOnFrameStepping;	// used to note number of frames retrieved when encoding
	

	CDirectShowPlayer(int id) ;
	~CDirectShowPlayer();

	void ReleaseAndNullAll();

	int Load(char* filename, bool useffdshowformp4, bool useffdshowformov, bool useffdshowformts, bool quarter1080pVideos);
	void Start(bool mute_sound);
	bool IsStopped();
	void Stop(double reset_time, bool do_reset, bool do_pause);
	void SeekToTime(double time, bool wait_for_completion);
	void GetDuration(hyper* h) ;
	void RemoveFilterIfNotUsed(IBaseFilter* filter);
	void AdvanceFrame();
	bool ReachedEnd();
    int GetWidth();
    int GetHeight();
	
	void GetCurrentVideoBuffer(unsigned char* data, int pitch, unsigned int width, unsigned int height, double* sampleTime, BOOL quartered);
	bool HasDolbyAC3Sound();

	long GetVolume();
	void SetVolume(long vol);
	bool ContainsAudio();
	double GetFrameRate();
	void CleanInternalBuffers();
	void SetFrameStepping(bool value);
    double GetLastSeekTime();
	std::string GetFirstFilterDetails();
	std::string GetNextFilterDetails();

protected:
	
	OAFilterState GetControlState(); 
	bool GetImageInfo( ) ;
	int LoadWithResults(char* name, int try_filter,  int filters_mask, bool quarter1080pVideos);
    void CopyCurrentBufferContents(unsigned char* data, int pitch, unsigned int width, unsigned int height, double* sampleTime, BOOL quartered);
};



#endif
