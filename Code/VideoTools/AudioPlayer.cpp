
#include "AudioPlayer.h"
#include "UnmanagedErrors.h"

//***************************************************************************
AudioPlayer::AudioPlayer(std::string& name)
{	
	mGraphBuilder=NULL;
	mControl=NULL;
	mMediaSeeking=NULL;
	mBasicAudio=NULL;
	mName = name ;
	mDuration =-1;
}


//***************************************************************************
AudioPlayer::~AudioPlayer()
{
	ReleaseAndNullAll();
}


//***************************************************************************
void AudioPlayer::ReleaseAndNullAll()
{
	if (mMediaSeeking) mMediaSeeking->Release();
	if (mControl) mControl->Release();
	if (mGraphBuilder) mGraphBuilder->Release();
	if (mBasicAudio) mBasicAudio->Release();
	mMediaSeeking=NULL;
	mControl=NULL;
	mGraphBuilder=NULL;
	mBasicAudio=NULL;
}


//***************************************************************************
long AudioPlayer::GetVolume()
{
	if (mBasicAudio==NULL) return 0;

	long vol=0;
	mBasicAudio->get_Volume(&vol);
	return vol;
}

//***************************************************************************
void  AudioPlayer::SetVolume(long vol)
{
	if (mBasicAudio!=NULL) 
	{
		mBasicAudio->put_Volume(vol);
	}
}


//***************************************************************************
void AudioPlayer::SeekToTime(double time, bool wait_for_completion)
{
#ifdef FULL_VIDEO_TRACE
	Trace("Seeking to time %f",time);
#endif

	if (this->mMediaSeeking==NULL)
	{
		Error("Audio can't seek as Seeking interface was NULL");
		return ;
	}

	GetDuration();

	REFERENCE_TIME Start = (REFERENCE_TIME) (time * 10000000.0f);
	{
		mMediaSeeking->SetPositions( &Start,    AM_SEEKING_AbsolutePositioning, 
									 &mDuration, AM_SEEKING_AbsolutePositioning );
	}
}

//***************************************************************************
double AudioPlayer::GetPlaybackPositionInSeconds()
{

	if (this->mMediaSeeking==NULL)
	{
		Error("Ausio can't get playback position as Seeking interface was NULL");
		return 0;
	}

	//GetDuration();

	LONGLONG current = 0;
	LONGLONG stop =0;
	{
		mMediaSeeking->GetPositions(&current,&stop);
	}

	double r = ((double)current) /10000000.0f;

	return r;
}


//***************************************************************************
bool AudioPlayer::BuildGraph()
{
	HRESULT hr;
	hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, 
                        IID_IGraphBuilder, (void **)&mGraphBuilder);
    if (FAILED(hr))
    {
        Error(" AudioPlayer Could not create the Filter Graph Manager.");
        return false ;
    }

	BOOL result = CreateAndRenderGraph((char*)mName.c_str(), mGraphBuilder, TRUE);
	if (result == FALSE)
	{
		return false;
	}

	hr = mGraphBuilder->QueryInterface(IID_IMediaControl, (void **)&mControl);
    if (FAILED(hr))
    {
        Error("Could not get media control interface.");
        return false ;
	}

	hr = mGraphBuilder->QueryInterface(IID_IMediaSeeking, (void **)&mMediaSeeking);
    if (FAILED(hr))
    {
        Error("Could not get seeking interface to media");
        return false ;
	}
	
	hr = mGraphBuilder->QueryInterface(IID_IBasicAudio, (void **)&mBasicAudio);
	if (FAILED(hr))
    {
        Warning("Could not get Basic Audio interface");
	}

	return true ;
}

//***************************************************************************
LONGLONG AudioPlayer::GetDuration()
{
	if (mDuration!=-1)
	{
		return mDuration;
	}

	mDuration=0;
	if (mMediaSeeking==NULL)
	{
		Error("Called AudioPlayer::GetDuration wich has a NULL mMediaSeeking Interface");
		return -1;
	}

	if (S_OK != mMediaSeeking->IsFormatSupported(&TIME_FORMAT_MEDIA_TIME))
	{
		Warning("Call to AudioPlayer::GetDuration could not detemine length");
		return -1;
	}

    HRESULT hr = mMediaSeeking->GetDuration(&mDuration);
    if (FAILED(hr))
	{
		Error("call to AudioPlayer::GetDuration failed ");
		mDuration=0;

        return -1;
	}

	return mDuration;
}


//***************************************************************************
void AudioPlayer::Play()
{
	mControl->Run();
}


//***************************************************************************
void AudioPlayer::Stop()
{

	mControl->Stop();
	
}

//***************************************************************************
void AudioPlayer::Pause()
{
	mControl->Pause();
}







