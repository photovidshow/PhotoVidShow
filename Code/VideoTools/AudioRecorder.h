#pragma once

#include <stdio.h>
#include <windows.h>
#include "streams.h"
#include "qedit.h"
#include <atlbase.h>
#include <string>
#include <vector>



class AudioRecordDevice
{
public:

	IMoniker * mMoniker;
	std::string mName;
};


class AudioRecorder
{
public:
	AudioRecorder(void);
	~AudioRecorder(void);

	void Record(char* outputWavFile, int selectedDevice, int echoRecording) ;
	void Stop();
	void Pause();
	void Continue();

	std::string GetFirstDeviceName();
	std::string GetNextDeviceName();

private:
	HRESULT SetAudioProperties();
	HRESULT GetRecordDevices() ;
	HRESULT SetupInterfaces();
	HRESULT RenderCaptureStream(char* outputWavFilename);
	HRESULT DestroyCaptureStream();
	HRESULT RenderPreviewStream();
	HRESULT DestroyPreviewStream();
	void ResetCapture();
	void FreeInterfaces();

	IGraphBuilder *m_pGB;
    ICaptureGraphBuilder2 *m_pCapture;
	IMediaControl *m_pMC;
	IBaseFilter *m_pInputDevice;
	IBaseFilter *m_pWAVDest;
	IBaseFilter *m_pFileWriter;
    IBaseFilter *m_pSplitter;
	IBaseFilter *m_pRenderer;

	std::vector<AudioRecordDevice> mRecordDevicesList;

	int mDeviceIterator;
};
