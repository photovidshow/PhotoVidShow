#ifndef AUDIO_GRAPH_RENDERER_BASE_INCLUDE
#define AUDIO_GRAPH_RENDERER_BASE_INCLUDE

#include <atlbase.h>
#include <Dshow.h>


//
// This class is the base class for our direct show audio renderers.
// i.e. our AudioPlayer and AudioRipper.
// It simply contains the common "building graph" functionality.
//
// This class uses the lav audio decoder or if lav turned off our mp3 tag stripper 
// source filter for mp3 files.
//
// This class should only be created and used by child classes.
//
class CAudioGraphRendererBase
{
public:

	static BOOL CreateAndRenderGraph(char* filename,
		IGraphBuilder* graphBuilder,
		BOOL useLav);

private:

	static void CreateDefaultFilters(IGraphBuilder* graphBuilder);

	static BOOL RenderGraph(char* filename,
		IGraphBuilder* graphBuilder,
		BOOL useLav);

	static BOOL IsMp3File(char* filename);

	static BOOL RenderFromMp3TagStripperSourceFilter(IGraphBuilder* graphBuilder, WCHAR* filename);


};

#endif
