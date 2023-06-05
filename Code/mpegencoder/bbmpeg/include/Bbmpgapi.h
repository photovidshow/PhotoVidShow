
#ifndef BBMPEGAPI_INCLUDE
#define BBMPEGAPI_INCLUDE

// bbMPEG's API header

// bbMPEG return codes

#define bbErrNone         0 	  // no error
/* video errors */
#define bbInternalError1  1       // internal error 1
#define bbInternalError2  2       // internal error 2
#define bbInternalError3  3       // internal error 3
#define bbInternalError4  4       // internal error 4
#define bbInternalError5  5       // internal error 5
#define bbInternalError6  6       // internal error 6
#define bbInternalError7  7       // internal error 7
#define bbInternalError8  8       // internal error 8
#define bbInternalError9  9       // internal error 9
#define bbInternalError10 10      // internal error 10
#define bbInternalError11 11      // internal error 11
#define bbInternalError12 12      // internal error 12
#define bbInternalError13 13      // internal error 13
#define bbInternalError14 14      // internal error 14
#define bbInternalError15 15      // internal error 15

/* audio errors */

#define bbInternalError16 16      // internal error 16
#define bbInternalError17 17      // internal error 17
#define bbInternalError18 18      // internal error 18

/* other codes */

#define bbMakeAbort   	  30      // user aborted the make
#define bbMakeDone    	  31	  // make finished normally
#define bbInternalError   32      // undefined error

// audio callback function
//  inputs: sampleNum = the current frame of audio data to get. This number
//                      will be in the startSample..endSample range specified
//                      in the compile options. This number will increment
//                      sequentially.
//
// outputs: numSamples = the number of samples actually retrieved
//          bufferSize = the number of bytes (not samples) actually retrieved
//          buffer = pointer to the audio data. The audio data must be 16 bit
//                   uncompressed samples, (mono or stereo). The buffer must
//                   consist of the number of samples that are present in one
//                   frame of video i.e if video frame rate is 30fps and the
//                   audio sample rate is 48000 then one frame of audio should
//                   consist of 48000/30 = 1600 samples.
//
// return: bbErrNone if audio data is retrieved successfully
//         anything else to abort

typedef int (*bbGetAudio)(unsigned int sampleNum, unsigned int *numSamples,
                          unsigned int *bufferSize, char **buffer);

// video callback function
//  inputs: frameNum = the video frame number requested, this number will be
//                     in the startFrame..endFrame range specified in the
//                     compile options. The frame may or may not be requested
//                     in order depending on the RequestInOrder setting
//                     in the win.ini file. If the RequestInOrder setting
//                     is zero the frames will probably be requested out
//                     of order, if the settings is one the frames will be
//                     requested in order.
//
// outputs: bytesInRow = number of bytes in each row of pixels. This number
//                       can ge different than the width * 4
//          buffer = pointer to the video frame buffer. The frame buffer must
//                   be a 32 bit per pixel (bgra) image in MS windows bmp format
//                   (only the pixel data, does not include the bmp header data)
//                   the first byte in the buffer is the blue byte of the pixel
//                   in the first column of the bottom row of the image
//
//  return: bbErrNone if a frame of video data is retrieved successfully
//          anything else to abort

typedef int (*bbGetVideo)(unsigned int frameNum, unsigned int *bytesInRow,
                          char **buffer);

struct makeMPEGRecInfo {    // make info
  char doVideo;             // if non-zero, video encoding is performed
  unsigned int width;       // video width, if not even the last column is ignored
  unsigned int height;      // video height, if not even the last row is ignored
  unsigned int startFrame;  // first video frame number
  unsigned int endFrame;    // last video frame number
  unsigned int startSample; // first audio sample
  unsigned int endSample;   // last audio sample
  char doAudio;             // if non-zero, audio encoding is performed
  unsigned int audioRate;   // audio sample rate, only 32000, 44100 and 48000 allowed
  char stereo;              // non-zero indicates stereo samples
  unsigned int sampleSize;  // size in bits of samples, must be 16
  char *outputFilename;     // pointer to a NULL terminated output filename
  bbGetVideo getVideo;      // pointer to a get audio callback function
  bbGetAudio mgetAudio;      // pointer to a get video callback function
  unsigned int pal;			// if 1 then do pal defaults else do ntsc
};

// call to initialize the settings, this function must be called before
// the compile function is called
//
//  inputs: none
// outputs: none
//  return: bbErrNone if successfull, bbInternalError if not

extern int MPEGEncoderInit(int pal, int widescreen, int disk_type, bool encode_audio, bool do_3_2_pulldown);

typedef int (*bbMPEGInitEntry)();

// call to encode audio and/or video, this will not return until the
// encoding process is complete
//
//  inputs: a pointer to a filled in makeMPEGRecInfo structure
// outputs: MPEG-2 encoded bitstream
//  return: bbMakeDone if successfull, bbMakeAbort if aborted by user or
//          bbInternalError if an error occurs

int MPEGEncoderStart(makeMPEGRecInfo *makeInfo);

//typedef int (*bbMPEGMakeMPEGEntry)(makeMPEGRecInfo *makeInfo);

// call to shutdown bbMPEG, not crucial it just saves the current bbMPEG
// window position
//
//  inputs: none
// outputs: none
//  return: none

extern  void MPEGEncoderShutdown();
typedef void (*bbMPEGShutdownEntry)();

// call to have the main dialog box set focus to itself
//
//  inputs: none
// outputs: none
//  return: none

typedef void (*bbMPEGSetFocusEntry)();

// call to set the parent window of bbMPEG's dialog to appWnd
//
//  inputs: an HWND which is going to be the parent window
// outputs: none
//  return: none

typedef void (*bbMPEGAppWindowEntry)(HWND appWnd);


#endif

