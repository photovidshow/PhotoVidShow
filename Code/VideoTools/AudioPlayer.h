#ifndef AUDIO_PLAYER_INCLUDE
#define AUDIO_PLAYER_INCLUDE

#include "AudioGraphRendererBase.h"
#include <stdio.h>
#include <windows.h>
#include "streams.h"
#include "qedit.h"
#include <atlbase.h>
#include <string>

class AudioPlayer : public CAudioGraphRendererBase
{
public:
	std::string mName ;

	IGraphBuilder*  mGraphBuilder;
	IMediaControl*  mControl;
	IMediaSeeking*  mMediaSeeking;
	REFERENCE_TIME  mDuration;
	IBasicAudio*	mBasicAudio;

	AudioPlayer(std::string& name);
	~AudioPlayer();

	long GetVolume();
	void SetVolume(long vol);

	LONGLONG GetDuration();
	void Play();
	void Stop();
	void SeekToTime(double time, bool wait_for_completion);
	double GetPlaybackPositionInSeconds();
	void Pause();

	bool BuildGraph();
	void ReleaseAndNullAll();

};


#endif

