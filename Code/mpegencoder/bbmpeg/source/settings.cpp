//----------------------------------------------------------------------------
// run the settings
//----------------------------------------------------------------------------

extern void DebugOutbutMessageToFile(char *fmt, ...);

#include "gui.h"

#include "main.h"

#define settingsErrNone 0
#define settingsCancel  1
#define settingsError   2

typedef enum {
   VBV_BUFFER_SIZE,           // 0
   FIXED_VBV_DELAY,           // 1
   DISPLAY_HORIZONTAL_SIZE,   // 2
   DISPLAY_VERTICAL_SIZE,     // 3
   PROG_SEQ,                  // 4
   PROG_FRAME,                // 5
   FIELDPIC,                  // 6
   TOPFIRST,                  // 7
   REPEATFIRST,               // 8
   INTRAVLC_TAB_0,            // 9
   INTRAVLC_TAB_1,            // 10
   INTRAVLC_TAB_2,            // 11
   FRAME_PRED_DCT_TAB_0,      // 12
   FRAME_PRED_DCT_TAB_1,      // 13
   FRAME_PRED_DCT_TAB_2,      // 14
   QSCALE_TAB_0,              // 15
   QSCALE_TAB_1,              // 16
   QSCALE_TAB_2,              // 17
   ALTSCAN_TAB_0,             // 18
   ALTSCAN_TAB_1,             // 19
   ALTSCAN_TAB_2,             // 20
   SEQ_DSPLY_EXT,             // 21
   SEQ_ENDCODE,               // 22
   SVCD_USER_BLOCKS,          // 23
   SLICE_HDR_MBROW,           // 24
   R,                         // 25
   AVG_ACT,                   // 26
   X_I,                       // 27
   X_P,                       // 28
   X_B,                       // 29
   D0_I,                      // 30
   D0_P,                      // 31
   D0_B,                      // 32
   MIN_FRAME_PERCENT,         // 33
   PAD_FRAME_PERCENT,         // 34
   RESET_PBD0,                // 35
   AUTOMOTION,                // 36
   XMOTION,                   // 37
   YMOTION,                   // 38
   MAXNONMOTION               // 39
   } t_non_motion_settings;

#define MAXPARENT      12 + MAXM
#define MAXMOTIONHCODE MAXM * 2
#define MAXMOTIONVCODE MAXM * 2
#define MAXMOTIONSRCH  MAXM * 4

//static char fixedMotionTxt[] = "Turn off the variable motion estimation setting above to access these settings (move the slider all the way to the left).";

//static char parentTxt[MAXPARENT][132];
/*= {
    "General settings",
    "Intended display's active region (this can be smaller or larger than the encoded frame size).",
    "Specify variable length coding tables used for intra coded blocks.",
    "Specify whether to restrict motion compensation to frame prediction and DCT to frame DCT.",
    "Specify quantization scale type.",
    "Specify entropy scanning patterns defining the order in which quantized DCT coefficients are run-length coded.",
    "Modify the behavior of the rate control scheme. Set them to 0 to have the encoder compute default values.",
    "Initial frame global complexity measure.",
    "Initial frame virtual buffer fullness.",
    "Specify fixed motion estimation vectors, only accessible if the variable motion estimation setting above is turned off.",
    "Specify the maximum horizontal and vertical movement in pels per frame.",
    "Specify the motion vectors for P frames.",
    "Specfiy the motion vectors for the B frames.",
    "Specify the motion vectors for B frame 1",
    "Specify the motion vectors for B frame 2",
    "Specify the motion vectors for B frame 3",
    "Specify the motion vectors for B frame 4",
    "Specify the motion vectors for B frame 5",
    "Specify the motion vectors for B frame 6",
    "Specify the motion vectors for B frame 7"};
	*/


//static char nonMotionTxt[MAXNONMOTION][132] ;
/*
    "Specify the size (in Kb) of the bitstream input buffer required for the sequence to be decoded without underflows or overflows.",
    "Specify whether the encoder computes a VBV delay (0) or if a fixed value of 0xFFFF is encoded (1).",
    "Specify the active region's horizontal size.",
    "Specify the active region's vertical size.",
    "Specify whether the sequence contains interlaced (0) or progressive (1) video.",
    "Specify whether the frames are interlaced (0) or progressive (1).",
    "Specify frame (0) picture coding or field (1) picture coding.",
    "Specify whether the bottom (or even) field (0) or the top (or odd) field (1) comes first in an interlaced frame.",
    "Specify whether the first field of a frame is repeated (1) after the second by the display process.",
    "Specify whether to use VLC table 0 (0) or VLC table 1 (1) for intra coded blocks in I frames.",
    "Specify whether to use VLC table 0 (0) or VLC table 1 (1) for intra coded blocks in P frames.",
    "Specify whether to use VLC table 0 (0) or VLC table 1 (1) for intra coded blocks in B frames.",
    "Specify whether to restrict (1) motion compensation to frame prediction and DCT to frame DCT in I frames.",
    "Specify whether to restrict (1) motion compensation to frame prediction and DCT to frame DCT in P frames.",
    "Specify whether to restrict (1) motion compensation to frame prediction and DCT to frame DCT in B frames.",
    "Specify linear (0) or non-linear (1) quantization scale type for I frames.",
    "Specify linear (0) or non-linear (1) quantization scale type for P frames.",
    "Specify linear (0) or non-linear (1) quantization scale type for B frames.",
    "Specify Zig-Zag Scan (0) or Alternate Scan (1) entropy scanning in I frames.",
    "Specify Zig-Zag Scan (0) or Alternate Scan (1) entropy scanning in P frames.",
    "Specify Zig-Zag Scan (0) or Alternate Scan (1) entropy scanning in B frames.",
    "Specify whether to write (1) Sequence Display Extensions in video stream or not (0).",
    "Specify whether to write (1) a Sequence End Code in video stream or not (0).",
    "Specify whether to embed SVCD user data blocks in the video stream (1) or not (0).",
    "Specify whether to write a Slice Header every macroblock row (1) or just the first row (0).",
    "Specify rate control Reaction Parameter, set to 0 to have encoder compute value.",
    "Specify the Initial Average Activity, set to 0 to have encoder compute value.",
    "Specify the Initial Global Complexity Measure for I frames, set to 0 to have encoder compute value.",
    "Specify the Initial Global Complexity Measure for P frames, set to 0 to have encoder compute value.",
    "Specify the Initial Global Complexity Measure for B frames, set to 0 to have encoder compute value.",
    "Specify the Initial Virtual Buffer Fullness for I frames, set to 0 to have encoder compute value.",
    "Specify the Initial Virtual Buffer Fullness for P frames, set to 0 to have encoder compute value.",
    "Specify the Initial Virtual Buffer Fullness for B frames, set to 0 to have encoder compute value.",
    "Specify that zero padding will occur if the actual frame size is less than this percentage of the target frame size.",
    "Specify what percentage of the target frame size to zero fill to.",
    "Specify whether the P and B frame Initial Virtual Buffer Fullness variables are reset (1) between frames.",
    "Specify whether the encoder automatically computes (1) motion vectors from pel movement settings.",
    "Specify the maximum horizontal pel movement per frame for auto motion computations.",
    "Specify the maximum vertical pel movement per frame for auto motion computations."};
*/

static int* nonMotionPtr[MAXNONMOTION] = {
	NULL,	// VBV_BUFFER_SIZE
	NULL,	// FIXED_VBV_DELAY
	NULL,	// DISPLAY_HORIZONTAL_SIZE
	NULL,	// DISPLAY_VERTICAL_SIZE
	NULL,	// PROG_SEQ
	NULL,	// PROG_FRAME
        NULL,	// FIELDPIC
	NULL,	// TOPFIRST
	NULL,	// REPEATFIRST
	NULL,	// INTRAVLC_TAB_0
	NULL,	// INTRAVLC_TAB_1
	NULL,	// INTRAVLC_TAB_2
        NULL,	// FRAME_PRED_DCT_TAB_0
	NULL,	// FRAME_PRED_DCT_TAB_1
	NULL,	// FRAME_PRED_DCT_TAB_2
	NULL,	// QSCALE_TAB_0
	NULL,	// QSCALE_TAB_1
	NULL,	// QSCALE_TAB_2
        NULL,	// ALTSCAN_TAB_0
	NULL,	// ALTSCAN_TAB_1
	NULL,	// ALTSCAN_TAB_2
	NULL,	// SEQ_DSPLY_EXT
        NULL,   // SEQ_ENDCODE
        NULL,   // SVCD_USER_BLOCKS
	NULL,   // SLICE_HDR_MBROW
	NULL,	// R
	NULL,	// AVG_ACT
        NULL,	// X_I
	NULL,	// X_P
	NULL,	// X_B
	NULL,	// D0_I
	NULL,	// D0_P
	NULL,	// D0_B
        NULL,	// MIN_FRAME_PERCENT
	NULL,	// PAD_FRAME_PERCENT
        NULL,   // RESET_PBD0
	NULL,	// AUTOMOTION
	NULL,	// XMOTION
	NULL	// YMOTION
	};

static int nonMotionRange[MAXNONMOTION][2] =
   {  {0, 0},           // VBV_BUFFER_SIZE
      {0, 1},           // FIXED_VBV_DELAY
      {0, 16383},       // DISPLAY_HORIZONTAL_SIZE
      {0, 16383},       // DISPLAY_VERTICAL_SIZE
      {0, 1},           // PROG_SEQ
      {0, 1},           // PROG_FRAME
      {0, 1},           // FIELDPIC
      {0, 1},           // TOPFIRST
      {0, 1},           // REPEATFIRST
      {0, 1},           // INTRAVLC_TAB_0
      {0, 1},           // INTRAVLC_TAB_1
      {0, 1},           // INTRAVLC_TAB_2
      {0, 1},           // FRAME_PRED_DCT_TAB_0
      {0, 1},           // FRAME_PRED_DCT_TAB_1
      {0, 1},           // FRAME_PRED_DCT_TAB_2
      {0, 1},           // QSCALE_TAB_0
      {0, 1},           // QSCALE_TAB_1
      {0, 1},           // QSCALE_TAB_2
      {0, 1},           // ALTSCAN_TAB_0
      {0, 1},           // ALTSCAN_TAB_1
      {0, 1},           // ALTSCAN_TAB_2
      {0, 1},           // SEQ_DSPLY_EXT
      {0, 1},           // SEQ_ENDCODE
{0, 1},           // SVCD_USER_BLOCKS
{0, 1},           // SLICE_HDR_MBROW
      {0, 32767},       // R
      {0, 32767},       // AVG_ACT
      {0, 32767},       // X_I
      {0, 32767},       // X_P
      {0, 32767},       // X_B
      {0, 32767},       // D0_I
      {0, 32767},       // D0_P
      {0, 32767},       // D0_B
      {0, 100},         // MIN_FRAME_PERCENT
      {0, 100},         // PAD_FRAME_PERCENT
      {0, 1},           // RESET_PBD0
      {0, 1},           // AUTOMOTION
      {0, 32767},       // XMOTION
      {0, 32767}        // YMOTION
   };

static int* motionHCodePtr[MAXMOTIONHCODE] = {NULL, NULL, NULL, NULL, NULL, NULL,
                                              NULL, NULL, NULL, NULL, NULL, NULL,
                                              NULL, NULL, NULL, NULL};

static int* motionVCodePtr[MAXMOTIONVCODE] = {NULL, NULL, NULL, NULL, NULL, NULL,
                                              NULL, NULL, NULL, NULL, NULL, NULL,
                                              NULL, NULL, NULL, NULL};

static int* motionSrchPtr[MAXMOTIONSRCH] = {NULL, NULL, NULL, NULL, NULL, NULL,
                                            NULL, NULL, NULL, NULL, NULL, NULL,
                                            NULL, NULL, NULL, NULL, NULL, NULL,
                                            NULL, NULL, NULL, NULL, NULL, NULL,
                                            NULL, NULL, NULL, NULL, NULL, NULL,
                                            NULL, NULL};

//static char motionCodeTxt[] = "Specify forward (P and B frames) and backward (B frames) motion codes for motion estimation, see help for details.";
//static char motionSrchTxt[] = "Specify forward (P and B frames) and backward (B frames) search windows for motion estimation, see help for details.";

static int vbvlim[4] = {597, 448, 112, 29};

static int horzlim[4] = {9, 9, 8, 7};
static int vertlim[4] = {5, 5, 5, 4};

static int Ndefaults[MAXM] = {1, 12, 15, 16, 15, 12, 14, 16};

static double ratetab[8]=
    {24000.0/1001.0,24.0,25.0,30000.0/1001.0,30.0,50.0,60000.0/1001.0,60.0};

static int ratetab1[4] = {62668800, 47001600, 10368000, 3041280};
static int bitratetab[4] = {80000000, 60000000, 14000000, 4000000};
static int svcdrates[15] = {0, 2568000, 2552000, 2544000, 2536000, 2520000,
                               2504000, 2488000, 2472000, 2440000, 2408000,
                               2376000, 2344000, 2280000, 2216000};

/*

class TOptPropSheet : public TPropertySheet {
  public:
    TOptPropSheet(TWindow* parent, const char far* title,
                  uint startPage = 0, bool isWizard = false,
                  uint32 flags = PSH_DEFAULT, TModule* module = 0);
};

class TOptions5Dlg : public TPropertyPage {
  public:
    TOptions5Dlg(TPropertySheet* parent);
    void Help(TNotify&);
    void SetupWindow();
    void EvShowWindow(bool show, uint status);
    void ChangeSettings();
    void EvDescription();
    void SaveAsDefault();
    void ResetDefaults();
    void LoadOptions();
    void SaveOptions();

    TEdit *OptionsName;
    TEdit *Description;

  DECLARE_RESPONSE_TABLE(TOptions5Dlg);
};

class TOptions2Dlg : public TPropertyPage {
  public:
    TOptions2Dlg(TPropertySheet* parent);
    void SetupWindow();
    void EvShowWindow(bool show, uint status);
    void ChangeSettings();
    void Help(TNotify&);
    bool CanClose();
    void EvProfileID();
    void EvLevelID();
    void EvChromaFormat();
    void EvAspectRatio();
    void EvFrameRate();
    void EvBitRateKillFocus();
    void EvAvgBitRateKillFocus();
    void EvMinBitRateKillFocus();

    void EvAutoBitrate();
    void EvPulldown();
    void EvPALDefaults();
    void EvVBRBitrate();
    bool EvMQuantBump(TNmUpDown& not);
    void EvMPEGN();
    void EvMPEGM();
    void EvMPEGType();
    bool EvTCHourBump(TNmUpDown& not);
    bool EvTCMinBump(TNmUpDown& not);
    bool EvTCSecBump(TNmUpDown& not);
    bool EvTCFrameBump(TNmUpDown& not);
    void ChangeMPEGType();
    void ChangeProfile();
    void ChangeLevel();
    void ChangeMPEGM();
    void ChangeFrameRate();
    void ChangeAutoBitrate();
    void ChangeVBRBitrate();
    void ChangeBitRate();
    void ChangeAvgBitRate();
    void ChangeMinBitRate();
    void ChangeStartTime();
    void ChangeMQuant();

    TStatic *bitrateTxt;
    TEdit *videoBitRate;
    TEdit *avgBitRate;
    TEdit *minBitRate;
    TEdit *mQuant;
    TComboBox *frameRate;
    TComboBox *profileID;
    TComboBox *levelID;
    TComboBox *aspectRatio;
    TComboBox *chromaFormat;
    TComboBox *MPEGN;
    TComboBox *MPEGM;
    TEdit *TCHour;
    TEdit *TCMin;
    TEdit *TCSec;
    TEdit *TCFrame;
    TUpDown *TCHourScroll;
    TUpDown *TCMinScroll;
    TUpDown *TCSecScroll;
    TUpDown *TCFrameScroll;
    TUpDown *mQuantScroll;
    TRadioButton *MPEG1;
    TRadioButton *MPEG2;
    TRadioButton *VideoCD;
    TRadioButton *SVCD;
    TRadioButton *DVD;
    TCheckBox *autoBitrate;
    TComboBox *videoPulldown;
    TCheckBox *palDefaults;
    TCheckBox *vbrBitrate;

  DECLARE_RESPONSE_TABLE(TOptions2Dlg);
};

class TOptions3Dlg : public TPropertyPage {
  public:
    TOptions3Dlg(TPropertySheet* parent);
    void SetupWindow();
    void EvShowWindow(bool show, uint status);
    void ChangeSettings();
    void Help(TNotify&);
    bool CanClose();
    void EvColorPrim();
    void EvTransferChar();
    void EvMatrixCoeff();
    void EvVideoFormat();
    void EvIntraDCPrec();
    void EvSettingVal();
    void EvHScroll(uint scrollCode, uint thumbPos, THandle hWndCtl);
    void SettingChange();
    bool EvSettingBump(TNmUpDown& not);
    void EvSettingTree(TTwNotify& nmHdr);
    void ChangeTree();
    void ChangeIntraDC();
    void ChangeVBVBuffer();
    void ChangeMotionEst();
    void ChangeProgressive();
    void ChangeCurrentNode();
    void ChangeOtherStuff();

    HTREEITEM currentNode;
    int *currentPtr;
    HTREEITEM parentNodes[MAXPARENT];
    HTREEITEM nonMotionNodes[MAXNONMOTION];
    HTREEITEM motionHCodeNodes[MAXMOTIONHCODE];
    HTREEITEM motionVCodeNodes[MAXMOTIONVCODE];
    HTREEITEM motionSrchNodes[MAXMOTIONSRCH];
    TComboBox *colorPrim;
    TComboBox *transferChar;
    TComboBox *matrixCoeff;
    TComboBox *videoFormat;
    TComboBox *intraDCPrec;
    TTreeWindow *settingTree;
    TEdit *settingVal;
    TUpDown *settingScroll;
    TStatic *settingTxt;
    THSlider *motionEst;
    TEdit *motionEstVal;

  DECLARE_RESPONSE_TABLE(TOptions3Dlg);
};

class TOptions4Dlg : public TPropertyPage {
  public:
    TOptions4Dlg(TPropertySheet* parent);
    void SetupWindow();
    void EvShowWindow(bool show, uint status);
    void ChangeSettings();
    void ChangeType();
    void Help(TNotify&);
    bool CanClose();
    void EvLayer1();
    void EvLayer2();
    void EvAudioBitRate();
    void EvPrivateBit();
    void EvCopyrightBit();
    void EvOriginalBit();
    void EvErrorProt();
    void EvPsychMod1();
    void EvPsychMod2();
    void EvModeStereo();
    void EvModeJStereo();
    void EvModeDChannel();
    void EvModeSChannel();
    void EvEmphNone();
    void EvEmph15us();
    void EvEmphCITT();

    TRadioButton *layer1;
    TRadioButton *layer2;
    TComboBox *audioBitRate;
    TCheckBox *privateBit;
    TCheckBox *copyrightBit;
    TCheckBox *originalBit;
    TCheckBox *errorProt;
    TRadioButton *psychMod1;
    TRadioButton *psychMod2;
    TRadioButton *modeStereo;
    TRadioButton *modeJStereo;
    TRadioButton *modeDChannel;
    TRadioButton *modeSChannel;
    TRadioButton *emphNone;
    TRadioButton *emph15us;
    TRadioButton *emphCITT;
    TGroupBox *modeGBox;

  DECLARE_RESPONSE_TABLE(TOptions4Dlg);
};

class TOptions1Dlg : public TPropertyPage {
  public:
    TOptions1Dlg(TPropertySheet* parent);
    void SetupWindow();
    void ChangeSettings();
    void EvShowWindow(bool show, uint status);
    void Help(TNotify&);
    void EvOpenPS();
    void EvOpenVS();
    void EvOpenAS();
    void EvOpenAS1();
    void EvOpenIQMatrix();
    void EvOpenNIQMatrix();
    void EvOpenStats();
    void EvSaveVideo();
    void EvSaveAudio();

    TEdit *progName;
    TCheckBox *saveVideo;
    TEdit *videoName;
    TCheckBox *saveAudio;
    TEdit *audioName;
    TEdit *audio1Name;
    TEdit *iqName;
    TEdit *niqName;
    TEdit *statsName;

  DECLARE_RESPONSE_TABLE(TOptions1Dlg);
};

class TOptions6Dlg : public TPropertyPage {
  public:
    TOptions6Dlg(TPropertySheet* parent);
    void SetupWindow();
    void ChangeSettings();
    void Help(TNotify&);
    void EvShowWindow(bool show, uint status);
    bool EvMuxRateBump(TNmUpDown& not);
    bool EvPacketSizeBump(TNmUpDown& not);
    bool EvPacketRatioBump(TNmUpDown& not);
    bool EvVBufferSizeBump(TNmUpDown& not);
    bool EvABufferSizeBump(TNmUpDown& not);
    bool EvA1BufferSizeBump(TNmUpDown& not);
    bool EvVideoDelayBump(TNmUpDown& not);
    bool EvAudioDelayBump(TNmUpDown& not);
    bool EvAudio1DelayBump(TNmUpDown& not);
    void EvPacketSize();
    void EvPacketRatio();
    void EvMuxRate();
    void EvVBufferSize();
    void EvABufferSize();
    void EvA1BufferSize();
    void EvPacketDelay();
    void EvVideoDelay();
    void EvAudioDelay();
    void EvAudio1Delay();
    void EvAddSVCDOffsets();
    void EvCompBitrate();
    void EvVCDAudioPad();
    void EvPrivate2();
    void EvAlignHeaders();
    void EvPulldown();
    void EvTimestamps();
    void EvVBRMultiplex();
    void ChangeSectorSize();
    void ChangeMPEGType();
    void ChangeSVCDOffsets();
    void EvMPEGType();
    void EvEndCode();

    TRadioButton *mpeg2Mplex;
    TRadioButton *mpeg1Mplex;
    TRadioButton *vcdMplex;
    TRadioButton *svcdMplex;
    TRadioButton *dvdMplex;
    TCheckBox *addSVCDOffsets;
    TComboBox *compBitrate;
    TCheckBox *vcdAudioPad;
    TCheckBox *private2;
    TCheckBox *alignHeaders;
    TCheckBox *vbrMultiplex;
    TCheckBox *writePEC;
    TComboBox *timeStamps;
    TComboBox *programPulldown;
    TEdit *packetSize;
    TUpDown *packetSizeScroll;
    TEdit *packetRatio;
    TUpDown *packetRatioScroll;
    TEdit *muxRate;
    TUpDown *muxRateScroll;
    TEdit *vBufferSize;
    TUpDown *vBufferSizeScroll;
    TEdit *aBufferSize;
    TUpDown *aBufferSizeScroll;
    TEdit *a1BufferSize;
    TUpDown *a1BufferSizeScroll;
    TEdit *packetDelay;
    TEdit *videoDelay;
    TUpDown *videoDelayScroll;
    TEdit *audioDelay;
    TUpDown *audioDelayScroll;
    TEdit *audio1Delay;
    TUpDown *audio1DelayScroll;

  DECLARE_RESPONSE_TABLE(TOptions6Dlg);
};

class TOptions0Dlg : public TPropertyPage
{
  public:
    TOptions0Dlg(TPropertySheet* parent);
    void SetupWindow();
    void ChangeSettings();
    void Help(TNotify&);
    void EvShowWindow(bool show, uint status);
    void EvBW();
    void EvEncodeVideo();
    void EvEncodeAudio();
    void EvMplexVideo();
    void EvMplexAudio();
    void EvMMXBox();
    void EvUseFP();
    void EvQuiet();
    void EvWriteEndCodes();
    void EvResetClocks();
    void EvSplitSize();
    void EvMuxStart();
    void EvMuxStop();
    void EvBrokenLink();
    void ChangeSplitSize();

    TCheckBox *encodeVideo;
    TCheckBox *encodeAudio;
    TCheckBox *mplexVideo;
    TCheckBox *mplexAudio;
    TComboBox *mmxBox;
    TCheckBox *useFP;
    TCheckBox *quietCheck;
    TCheckBox *bwCheck;
    TCheckBox *resetClocks;
    TCheckBox *writeEndCodes;
    TCheckBox *brokenLink;
    TEdit     *splitSize;
    TEdit     *muxStart;
    TEdit     *muxStop;

    DECLARE_RESPONSE_TABLE(TOptions0Dlg);
};

static TOptions0Dlg *options0;
static TOptions1Dlg *options1;
static TOptions2Dlg *options2;
static TOptions3Dlg *options3;
static TOptions4Dlg *options4;
static TOptions5Dlg *options5;
static TOptions6Dlg *options6;

DEFINE_RESPONSE_TABLE1(TOptions0Dlg, TPropertyPage)
  EV_WM_SHOWWINDOW,
  EV_COMMAND(IDC_USEFPCHECK, EvUseFP),
  EV_COMMAND(IDC_QUIET, EvQuiet),
  EV_COMMAND(IDC_BW, EvBW),
  EV_COMMAND(IDC_RESETCLOCKS, EvResetClocks),
  EV_COMMAND(IDC_WRITEEND, EvWriteEndCodes),
  EV_COMMAND(IDC_BROKENLINK, EvBrokenLink),
  EV_EN_KILLFOCUS(IDC_SPLITSIZE, EvSplitSize),
  EV_EN_KILLFOCUS(IDC_MUXSTART, EvMuxStart),
  EV_EN_KILLFOCUS(IDC_MUXSTOP, EvMuxStop),
  EV_CBN_CLOSEUP(IDC_MMXBOX, EvMMXBox),
  EV_CHILD_NOTIFY(IDC_ENCODEVIDEO, BN_CLICKED, EvEncodeVideo),
  EV_CHILD_NOTIFY(IDC_ENCODEAUDIO, BN_CLICKED, EvEncodeAudio),
  EV_CHILD_NOTIFY(IDC_MPLEXVIDEO, BN_CLICKED, EvMplexVideo),
  EV_CHILD_NOTIFY(IDC_MPLEXAUDIO, BN_CLICKED, EvMplexAudio),
END_RESPONSE_TABLE;


TOptions0Dlg::TOptions0Dlg(TPropertySheet* parent)
             :TPropertyPage(parent, IDD_OPTIONS0)
{
  PageInfo.dwFlags |= PSP_HASHELP;
  encodeVideo = new TCheckBox(this, IDC_ENCODEVIDEO);
  encodeAudio = new TCheckBox(this, IDC_ENCODEAUDIO);
  mplexVideo = new TCheckBox(this, IDC_MPLEXVIDEO);
  mplexAudio = new TCheckBox(this, IDC_MPLEXAUDIO);
  mmxBox = new TComboBox(this, IDC_MMXBOX);
  useFP = new TCheckBox(this, IDC_USEFPCHECK);
  quietCheck = new TCheckBox(this, IDC_QUIET);
  bwCheck = new TCheckBox(this, IDC_BW);
  resetClocks = new TCheckBox(this, IDC_RESETCLOCKS);
  writeEndCodes = new TCheckBox(this, IDC_WRITEEND);
  brokenLink = new TCheckBox(this, IDC_BROKENLINK);
  splitSize = new TEdit(this, IDC_SPLITSIZE);
  muxStart = new TEdit(this, IDC_MUXSTART);
  muxStop = new TEdit(this, IDC_MUXSTOP);
}

void TOptions0Dlg::EvShowWindow(bool show, uint status)
{
  TPropertyPage::EvShowWindow(show, status);
  if (show)
    ChangeSettings();
}

void TOptions0Dlg::SetupWindow()
{
  TPropertyPage::SetupWindow();
  mmxBox->AddString("* Off - slow (all Pentium class processors)");
  if (MMXAvail & FEATURE_MMX)
    mmxBox->AddString("* MMX - faster (MMX enabled processors)");
  else
    mmxBox->AddString("  MMX - faster (MMX enabled processors)");
  if (MMXAvail & FEATURE_3DNOW)
    mmxBox->AddString("* 3DNOW - faster (3DNOW enabled processors)");
  else
    mmxBox->AddString("  3DNOW - faster (3DNOW enabled processors)");
  if (MMXAvail & FEATURE_MMXEXT)
    mmxBox->AddString("* SSE - fastest (Pentium III and Athlon)");
  else
    mmxBox->AddString("  SSE - fastest (Pentium III and Athlon)");
  if (MMXAvail & FEATURE_3DNOWEXT)
    mmxBox->AddString("* 3DNOW EXT - fastest (Athlon and higher)");
  else
    mmxBox->AddString("  3DNOW EXT - fastest (Athlon and higher)");

  ChangeSettings();
}

void TOptions0Dlg::ChangeSettings()
{
  if (!VideoAvail)
  {
    tempSettings1.EncodeVideo = FALSE;
    encodeVideo->Uncheck();
    encodeVideo->EnableWindow(false);
    bwCheck->Uncheck();
    bwCheck->EnableWindow(false);
  }
  else
  {
    if (tempSettings1.UserEncodeVideo)
      encodeVideo->Check();
    else
      encodeVideo->Uncheck();
    if (tempSettings1.B_W)
      bwCheck->Check();
    else
      bwCheck->Uncheck();
  }

  if (!AudioAvail)
  {
    tempSettings1.EncodeAudio = FALSE;
    encodeAudio->Uncheck();
    encodeAudio->EnableWindow(false);
  }
  else
  {
    if (tempSettings1.UserEncodeAudio)
      encodeAudio->Check();
    else
      encodeAudio->Uncheck();
  }

  if (tempSettings1.MplexVideo)
    mplexVideo->Check();
  else
    mplexVideo->Uncheck();

  if (tempSettings1.MplexAudio)
    mplexAudio->Check();
  else
    mplexAudio->Uncheck();

  if (tempSettings1.useFP)
    useFP->Check();
  else
    useFP->Uncheck();

  if (tempSettings1.verbose)
    quietCheck->Check();
  else
    quietCheck->Uncheck();

  mmxBox->SetSelIndex(MMXMode);
  ChangeSplitSize();
}

void TOptions0Dlg::ChangeSplitSize()
{
  char tmpStr[30];

  if (!tempSettings1.MplexVideo)
  {
    splitSize->SetText("0");
    splitSize->EnableWindow(false);
    muxStart->SetText("0");
    muxStart->EnableWindow(false);
    muxStop->SetText("0");
    muxStop->EnableWindow(false);
    resetClocks->Uncheck();
    resetClocks->EnableWindow(false);
    writeEndCodes->Uncheck();
    writeEndCodes->EnableWindow(false);
    brokenLink->Uncheck();
    brokenLink->EnableWindow(false);
  }
  else
  {
    splitSize->EnableWindow(true);
    sprintf(tmpStr, "%d", tempSettings1.max_file_size);
    splitSize->SetText(tmpStr);
    muxStart->EnableWindow(true);
    sprintf(tmpStr, "%d", tempSettings1.mux_start_time);
    muxStart->SetText(tmpStr);
    muxStop->EnableWindow(true);
    sprintf(tmpStr, "%d", tempSettings1.mux_stop_time);
    muxStop->SetText(tmpStr);
    if (tempSettings1.max_file_size || tempSettings1.mux_start_time)
    {
      if (tempSettings1.reset_clocks)
        resetClocks->Check();
      else
        resetClocks->Uncheck();
      resetClocks->EnableWindow(true);
      if (tempSettings1.write_end_codes)
        writeEndCodes->Check();
      else
        writeEndCodes->Uncheck();
      writeEndCodes->EnableWindow(true);
      if (tempSettings1.set_broken_link)
        brokenLink->Check();
      else
        brokenLink->Uncheck();
      brokenLink->EnableWindow(true);
    }
    else
    {
      resetClocks->Uncheck();
      resetClocks->EnableWindow(false);
      writeEndCodes->Uncheck();
      writeEndCodes->EnableWindow(false);
      brokenLink->Uncheck();
      brokenLink->EnableWindow(false);
    }
  }
}

void TOptions0Dlg::EvBrokenLink()
{
  tempSettings1.set_broken_link = brokenLink->GetCheck() == BF_CHECKED;
}

void TOptions0Dlg::EvWriteEndCodes()
{
  tempSettings1.write_end_codes = writeEndCodes->GetCheck() == BF_CHECKED;
}

void TOptions0Dlg::EvResetClocks()
{
  tempSettings1.reset_clocks = resetClocks->GetCheck() == BF_CHECKED;
}

void TOptions0Dlg::EvSplitSize()
{
  char tmpStr[30];
  int i;

  splitSize->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  if (i < 0 || i > 4096)
  {
    DisplayError("Invalid split size, resetting to original");
    sprintf(tmpStr, "%d", tempSettings1.max_file_size);
    splitSize->SetText(tmpStr);
    return;
  }
  tempSettings1.max_file_size = i;
  ChangeSplitSize();
}

void TOptions0Dlg::EvMuxStart()
{
  char tmpStr[30];
  int i;

  muxStart->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  if (i > tempSettings1.mux_stop_time)
  {
    DisplayError("Mux start time must be smaller than the stop time, resetting to original");
    sprintf(tmpStr, "%d", tempSettings1.mux_start_time);
    muxStart->SetText(tmpStr);
    return;
  }
  tempSettings1.mux_start_time = i;
  ChangeSplitSize();
}

void TOptions0Dlg::EvMuxStop()
{
  char tmpStr[30];
  int i;

  muxStop->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  if (i < tempSettings1.mux_start_time)
  {
    DisplayError("Mux stop time must be larger than the start time, resetting to original");
    sprintf(tmpStr, "%d", tempSettings1.mux_stop_time);
    muxStop->SetText(tmpStr);
    return;
  }
  tempSettings1.mux_stop_time = i;
  ChangeSplitSize();
}

void TOptions0Dlg::EvQuiet()
{
  tempSettings1.verbose = quietCheck->GetCheck() == BF_CHECKED;
}

void TOptions0Dlg::EvBW()
{
  tempSettings1.B_W = bwCheck->GetCheck() == BF_CHECKED;
}

void TOptions0Dlg::EvUseFP()
{
  tempSettings1.useFP = useFP->GetCheck() == BF_CHECKED;
}

void TOptions0Dlg::EvMMXBox()
{
  char tmpStr[80];

  sprintf(tmpStr, "%s - warning", AppName);
  MMXMode = mmxBox->GetSelIndex();
  switch (MMXMode)
  {
    case MODE_MMX:
      if (!(MMXAvail & FEATURE_MMX))
        MessageBox("MMX mode not detected, I will crash if this mode is not actually supported",
                   tmpStr, MB_OK | MB_ICONWARNING);
      break;
    case MODE_3DNOW:
      if (!(MMXAvail & FEATURE_3DNOW))
        MessageBox("3DNOW mode not detected, I will crash if this mode is not actually supported",
                   tmpStr, MB_OK | MB_ICONWARNING);
      break;
    case MODE_SSE:
      if (!(MMXAvail & FEATURE_MMXEXT))
        MessageBox("SSE mode not detected, I will crash if this mode is not actually supported",
                   tmpStr, MB_OK | MB_ICONWARNING);
      break;
    case MODE_3DNOWEXT:
      if (!(MMXAvail & FEATURE_3DNOWEXT))
        MessageBox("3DNOW EXT mode not detected, I will crash if this mode is not actually supported",
                   tmpStr, MB_OK | MB_ICONWARNING);
      break;
  }
}

void TOptions0Dlg::EvMplexVideo()
{
  tempSettings1.MplexVideo = (mplexVideo->GetCheck() == BF_CHECKED);
  ChangeSplitSize();
}

void TOptions0Dlg::EvMplexAudio()
{
  tempSettings1.MplexAudio = (mplexAudio->GetCheck() == BF_CHECKED);
}

void TOptions0Dlg::EvEncodeVideo()
{
  tempSettings1.UserEncodeVideo = (encodeVideo->GetCheck() == BF_CHECKED);
  if (VideoAvail)
    tempSettings1.EncodeVideo = tempSettings1.UserEncodeVideo;
  if (tempSettings1.UserEncodeVideo)
  {
    tempSettings1.mplex_pulldown_flag = tempSettings1.video_pulldown_flag;
    switch (tempSettings1.video_type)
    {
      case MPEG_MPEG1:
        if ((tempSettings1.mplex_type == MPEG_VCD) ||
            (tempSettings1.mplex_type == MPEG_SVCD))
          SetMPEG1Mplex(&tempSettings1);
        break;

      case MPEG_VCD:
        if (tempSettings1.mplex_type != MPEG_VCD)
          SetVCDMplex(&tempSettings1);
        break;

      case MPEG_MPEG2:
        if (tempSettings1.mplex_type < MPEG_MPEG2)
          SetMPEG2Mplex(&tempSettings1);
        break;

      case MPEG_SVCD:
        if (tempSettings1.mplex_type != MPEG_SVCD)
          SetSVCDMplex(&tempSettings1);
        break;

      case MPEG_DVD:
        if (tempSettings1.mplex_type != MPEG_DVD)
          SetDVDMplex(&tempSettings1);
        break;
    }
  }
}

void TOptions0Dlg::EvEncodeAudio()
{
  tempSettings1.UserEncodeAudio = (encodeAudio->GetCheck() == BF_CHECKED);
  if (AudioAvail)
    tempSettings1.EncodeAudio = tempSettings1.UserEncodeAudio;
}

void TOptions0Dlg::Help(TNotify&)
{
  WinHelp(HelpFileName, HELP_CONTEXT, 1);
}

TOptions6Dlg::TOptions6Dlg(TPropertySheet* parent)
            :TPropertyPage(parent, IDD_OPTIONS6)
{
  PageInfo.dwFlags |= PSP_HASHELP;
  mpeg2Mplex = new TRadioButton(this, IDC_MPEG2MPLEX);
  mpeg1Mplex = new TRadioButton(this, IDC_MPEG1MPLEX);
  vcdMplex = new TRadioButton(this, IDC_VCDMPLEX);
  svcdMplex = new TRadioButton(this, IDC_SVCDMPLEX);
  dvdMplex = new TRadioButton(this, IDC_DVDMPLEX);
  compBitrate = new TComboBox(this, IDC_COMPBITRATE);
  addSVCDOffsets = new TCheckBox(this, IDC_ADDSVCDOFFSETS);
  vcdAudioPad = new TCheckBox(this, IDC_VCDAUDIOPAD);
  private2 = new TCheckBox(this, IDC_PRIVATE2);
  alignHeaders = new TCheckBox(this, IDC_ALIGNHEADERS);
  vbrMultiplex = new TCheckBox(this, IDC_VBRMPLEX);
  writePEC = new TCheckBox(this, IDC_ENDCODE);
  timeStamps = new TComboBox(this, IDC_TIMESTAMPS);
  programPulldown = new TComboBox(this, IDC_PPULLDOWN);
  packetSize = new TEdit(this, IDC_PACKETSIZETXT);
  packetSizeScroll = new TUpDown(this, IDC_PACKETSIZESCROLL);
  packetRatio = new TEdit(this, IDC_PACKETRATIOTXT);
  packetRatioScroll = new TUpDown(this, IDC_PACKETRATIOSCROLL);
  muxRate = new TEdit(this, IDC_MUXRATETXT);
  muxRateScroll = new TUpDown(this, IDC_MUXRATESCROLL);
  vBufferSize = new TEdit(this, IDC_VBUFFERSIZETXT);
  vBufferSizeScroll = new TUpDown(this, IDC_VBUFFERSIZESCROLL);
  aBufferSize = new TEdit(this, IDC_ABUFFERSIZETXT);
  aBufferSizeScroll = new TUpDown(this, IDC_ABUFFERSIZESCROLL);
  a1BufferSize = new TEdit(this, IDC_ABUFFER1SIZETXT);
  a1BufferSizeScroll = new TUpDown(this, IDC_ABUFFER1SIZESCROLL);
  packetDelay = new TEdit(this, IDC_PACKETDELAYTXT);
  videoDelay = new TEdit(this, IDC_VIDEODELAYTXT);
  videoDelayScroll = new TUpDown(this, IDC_VIDEODELAYSCROLL);
  audioDelay = new TEdit(this, IDC_AUDIODELAYTXT);
  audioDelayScroll = new TUpDown(this, IDC_AUDIODELAYSCROLL);
  audio1Delay = new TEdit(this, IDC_AUDIODELAY1TXT);
  audio1DelayScroll = new TUpDown(this, IDC_AUDIODELAY1SCROLL);
}

DEFINE_RESPONSE_TABLE1(TOptions6Dlg, TPropertyPage)
  EV_UDN_DELTAPOS(IDC_PACKETSIZESCROLL, EvPacketSizeBump),
  EV_UDN_DELTAPOS(IDC_PACKETRATIOSCROLL, EvPacketRatioBump),
  EV_UDN_DELTAPOS(IDC_MUXRATESCROLL, EvMuxRateBump),
  EV_UDN_DELTAPOS(IDC_VBUFFERSIZESCROLL, EvVBufferSizeBump),
  EV_UDN_DELTAPOS(IDC_ABUFFERSIZESCROLL, EvABufferSizeBump),
  EV_UDN_DELTAPOS(IDC_ABUFFER1SIZESCROLL, EvA1BufferSizeBump),
  EV_UDN_DELTAPOS(IDC_VIDEODELAYSCROLL, EvVideoDelayBump),
  EV_UDN_DELTAPOS(IDC_AUDIODELAYSCROLL, EvAudioDelayBump),
  EV_UDN_DELTAPOS(IDC_AUDIODELAY1SCROLL, EvAudio1DelayBump),
  EV_EN_KILLFOCUS(IDC_PACKETSIZETXT, EvPacketSize),
  EV_EN_KILLFOCUS(IDC_PACKETRATIOTXT, EvPacketRatio),
  EV_EN_KILLFOCUS(IDC_MUXRATETXT, EvMuxRate),
  EV_EN_KILLFOCUS(IDC_VBUFFERSIZETXT, EvVBufferSize),
  EV_EN_KILLFOCUS(IDC_ABUFFERSIZETXT, EvABufferSize),
  EV_EN_KILLFOCUS(IDC_ABUFFER1SIZETXT, EvA1BufferSize),
  EV_EN_KILLFOCUS(IDC_PACKETDELAYTXT, EvPacketDelay),
  EV_EN_KILLFOCUS(IDC_VIDEODELAYTXT, EvVideoDelay),
  EV_EN_KILLFOCUS(IDC_AUDIODELAYTXT, EvAudioDelay),
  EV_EN_KILLFOCUS(IDC_AUDIODELAY1TXT, EvAudio1Delay),
  EV_CBN_CLOSEUP(IDC_PPULLDOWN, EvPulldown),
  EV_CBN_CLOSEUP(IDC_TIMESTAMPS, EvTimestamps),
  EV_CBN_CLOSEUP(IDC_COMPBITRATE, EvCompBitrate),
  EV_COMMAND(IDC_PRIVATE2, EvPrivate2),
  EV_COMMAND(IDC_ALIGNHEADERS, EvAlignHeaders),
  EV_COMMAND(IDC_VCDAUDIOPAD, EvVCDAudioPad),
  EV_COMMAND(IDC_ADDSVCDOFFSETS, EvAddSVCDOffsets),
  EV_COMMAND(IDC_MPEG2MPLEX, EvMPEGType),
  EV_COMMAND(IDC_MPEG1MPLEX, EvMPEGType),
  EV_COMMAND(IDC_VCDMPLEX, EvMPEGType),
  EV_COMMAND(IDC_SVCDMPLEX, EvMPEGType),
  EV_COMMAND(IDC_DVDMPLEX, EvMPEGType),
  EV_COMMAND(IDC_VBRMPLEX, EvVBRMultiplex),
  EV_COMMAND(IDC_ENDCODE, EvEndCode),
  EV_WM_SHOWWINDOW,
END_RESPONSE_TABLE;

void TOptions6Dlg::EvShowWindow(bool show, uint status)
{
  TPropertyPage::EvShowWindow(show, status);
  if (show)
    ChangeSettings();
}

void TOptions6Dlg::SetupWindow()
{
  TPropertyPage::SetupWindow();
  programPulldown->AddString("None");
  programPulldown->AddString("2:3");
  programPulldown->AddString("3:2");
  programPulldown->AddString("Auto");
  timeStamps->AddString("All frames");
  timeStamps->AddString("I and P frames");
  timeStamps->AddString("I frames");
  compBitrate->AddString("Unused");
  compBitrate->AddString("Use Avg");
  compBitrate->AddString("Use Max");
  ChangeSettings();
}

void TOptions6Dlg::ChangeSettings()
{
  mpeg2Mplex->Uncheck();
  svcdMplex->Uncheck();
  mpeg1Mplex->Uncheck();
  vcdMplex->Uncheck();
  dvdMplex->Uncheck();
  switch (tempSettings1.mplex_type)
  {
    case MPEG_SVCD:
      svcdMplex->Check();
      break;
    case MPEG_MPEG1:
      mpeg1Mplex->Check();
      break;
    case MPEG_VCD:
      vcdMplex->Check();
      break;
    case MPEG_DVD:
      dvdMplex->Check();
      break;
    default:
      mpeg2Mplex->Check();
      break;
  }
  ChangeMPEGType();
}

void TOptions6Dlg::EvVBRMultiplex()
{
  tempSettings1.VBR_multiplex = (vbrMultiplex->GetCheck() == BF_CHECKED);
}

void TOptions6Dlg::EvPrivate2()
{
  tempSettings1.put_private2 = (private2->GetCheck() == BF_CHECKED);
}

void TOptions6Dlg::EvAlignHeaders()
{
  tempSettings1.align_sequence_headers = (alignHeaders->GetCheck() == BF_CHECKED);
}

void TOptions6Dlg::EvTimestamps()
{
  tempSettings1.frame_timestamps = timeStamps->GetSelIndex();
}

void TOptions6Dlg::EvVCDAudioPad()
{
  tempSettings1.vcd_audio_pad = (vcdAudioPad->GetCheck() == BF_CHECKED);
}

void TOptions6Dlg::EvPulldown()
{
  tempSettings1.mplex_pulldown_flag = programPulldown->GetSelIndex();
}

void TOptions6Dlg::EvMPEGType()
{
  if (mpeg1Mplex->GetCheck() == BF_CHECKED)
  {
    if (tempSettings1.mplex_type != MPEG_MPEG1)
      SetMPEG1Mplex(&tempSettings1);
  }
  else
    if (vcdMplex->GetCheck() == BF_CHECKED)
    {
      if (tempSettings1.mplex_type != MPEG_VCD)
        SetVCDMplex(&tempSettings1);
    }
    else
      if (svcdMplex->GetCheck() == BF_CHECKED)
      {
        if (tempSettings1.mplex_type != MPEG_SVCD)
          SetSVCDMplex(&tempSettings1);
      }
      else
        if (dvdMplex->GetCheck() == BF_CHECKED)
        {
          if (tempSettings1.mplex_type != MPEG_DVD)
            SetDVDMplex(&tempSettings1);
        }
        else
        {
          if (tempSettings1.mplex_type != MPEG_MPEG2)
            SetMPEG2Mplex(&tempSettings1);
        }
  ChangeMPEGType();
}

void TOptions6Dlg::EvCompBitrate()
{
  tempSettings1.use_computed_bitrate = compBitrate->GetSelIndex();
}

void TOptions6Dlg::EvAddSVCDOffsets()
{
  tempSettings1.mux_SVCD_scan_offsets = addSVCDOffsets->GetCheck() == BF_CHECKED;
  ChangeSVCDOffsets();
}

void TOptions6Dlg::ChangeSVCDOffsets()
{
  if (tempSettings1.mux_SVCD_scan_offsets)
  {
    tempSettings1.align_sequence_headers = 1;
    alignHeaders->Check();
    alignHeaders->EnableWindow(false);
  }
  else
    alignHeaders->EnableWindow(true);
}

void TOptions6Dlg::EvEndCode()
{
  tempSettings1.write_pec = writePEC->GetCheck() == BF_CHECKED;
}

void TOptions6Dlg::ChangeMPEGType()
{
  char tmpStr[20];

  if (tempSettings1.mplex_type > MPEG_VCD)
    programPulldown->SetSelIndex(tempSettings1.mplex_pulldown_flag);
  else
    programPulldown->SetSelIndex(0);

  if (tempSettings1.EncodeVideo || (tempSettings1.mplex_type < MPEG_MPEG2))
    programPulldown->EnableWindow(false);
  else
    programPulldown->EnableWindow(true);

  timeStamps->SetSelIndex(tempSettings1.frame_timestamps);

  if (tempSettings1.align_sequence_headers)
    alignHeaders->Check();
  else
    alignHeaders->Uncheck();

  if (tempSettings1.VBR_multiplex)
    vbrMultiplex->Check();
  else
    vbrMultiplex->Uncheck();

  if (tempSettings1.write_pec)
    writePEC->Check();
  else
    writePEC->Uncheck();

  if (tempSettings1.mplex_type == MPEG_DVD)
  {
    if (tempSettings1.put_private2)
      private2->Check();
    else
      private2->Uncheck();
    private2->EnableWindow(true);
  }
  else
  {
    private2->Uncheck();
    private2->EnableWindow(false);
  }

  if (tempSettings1.mplex_type > MPEG_VCD)
    packetSizeScroll->SetRange(MPEG2_MIN_SECTOR_SIZE, MAX_SECTOR_SIZE);
  else
    packetSizeScroll->SetRange(MPEG1_MIN_SECTOR_SIZE, MAX_SECTOR_SIZE);

  if (tempSettings1.user_mux_rate)
  {
    compBitrate->SetSelIndex(COMPBITRATE_NONE);
    compBitrate->EnableWindow(false);
  }
  else
  {
    compBitrate->SetSelIndex(tempSettings1.use_computed_bitrate);
    compBitrate->EnableWindow(true);
  }

  if (tempSettings1.mux_SVCD_scan_offsets)
    addSVCDOffsets->Check();
  else
    addSVCDOffsets->Uncheck();

  vcdAudioPad->Uncheck();
  if (tempSettings1.mplex_type == MPEG_VCD)
  {
    if (tempSettings1.vcd_audio_pad)
      vcdAudioPad->Check();
    vcdAudioPad->EnableWindow(true);
  }
  else
    vcdAudioPad->EnableWindow(false);

  packetSizeScroll->SetPos(tempSettings1.sector_size);
  ChangeSectorSize();
  muxRateScroll->SetRange(0, UD_MAXVAL);
  muxRateScroll->SetPos(tempSettings1.user_mux_rate);
  vBufferSizeScroll->SetRange(4, 256);
  vBufferSizeScroll->SetPos(tempSettings1.video_buffer_size);
  aBufferSizeScroll->SetRange(1, 64);
  aBufferSizeScroll->SetPos(tempSettings1.audio_buffer_size);
  a1BufferSizeScroll->SetRange(1, 64);
  a1BufferSizeScroll->SetPos(tempSettings1.audio1_buffer_size);
  sprintf(tmpStr, "%u", tempSettings1.sectors_delay);
  packetDelay->SetText(tmpStr);
  videoDelayScroll->SetRange(0, UD_MAXVAL);
  videoDelayScroll->SetPos(tempSettings1.video_delay_ms);
  audioDelayScroll->SetRange(0, UD_MAXVAL);
  audioDelayScroll->SetPos(tempSettings1.audio_delay_ms);
  audio1DelayScroll->SetRange(0, UD_MAXVAL);
  audio1DelayScroll->SetPos(tempSettings1.audio1_delay_ms);

  if (tempSettings1.EncodeVideo && tempSettings1.auto_bitrate)
  {
    vBufferSize->EnableWindow(false);
    vBufferSizeScroll->EnableWindow(false);
  }
  else
  {
    vBufferSize->EnableWindow(true);
    vBufferSizeScroll->EnableWindow(true);
  }
  ChangeSVCDOffsets();
}

void TOptions6Dlg::ChangeSectorSize()
{
  int pes_space, i;

  if (tempSettings1.mplex_type > MPEG_VCD)
  {
    pes_space = tempSettings1.sector_size - MPEG2_PACK_HEADER_SIZE - SYS_HEADER_SIZE;
    i = pes_space / MPEG2_MIN_PACKET_SIZE;
  }
  else
  {
    pes_space = tempSettings1.sector_size - MPEG1_PACK_HEADER_SIZE - SYS_HEADER_SIZE;
    i = pes_space / MPEG1_MIN_PACKET_SIZE;
  }
  packetRatioScroll->SetRange(1, i);
  if (tempSettings1.packets_per_pack > i)
    tempSettings1.packets_per_pack = 1;
  packetRatioScroll->SetPos(tempSettings1.packets_per_pack);
}

bool TOptions6Dlg::EvMuxRateBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  muxRateScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
    tempSettings1.user_mux_rate = k;
  return false;
}

bool TOptions6Dlg::EvPacketSizeBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  packetSizeScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
  {
    tempSettings1.sector_size = k;
    ChangeSectorSize();
  }
  return false;
}

bool TOptions6Dlg::EvPacketRatioBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  packetRatioScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
    tempSettings1.packets_per_pack = k;
  return false;
}

bool TOptions6Dlg::EvVBufferSizeBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  vBufferSizeScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
    tempSettings1.video_buffer_size = k;
  return false;
}

bool TOptions6Dlg::EvABufferSizeBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  aBufferSizeScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
    tempSettings1.audio_buffer_size = k;
  return false;
}

bool TOptions6Dlg::EvA1BufferSizeBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  a1BufferSizeScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
    tempSettings1.audio1_buffer_size = k;
  return false;
}

bool TOptions6Dlg::EvVideoDelayBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  videoDelayScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
    tempSettings1.video_delay_ms = k;
  return false;
}

bool TOptions6Dlg::EvAudioDelayBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  audioDelayScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
    tempSettings1.audio_delay_ms = k;
  return false;
}

bool TOptions6Dlg::EvAudio1DelayBump(TNmUpDown& not)
{
  int i, j, k;

  k = not.iPos + not.iDelta;
  audio1DelayScroll->GetRange(i, j);
  if ((k >= i) && (k <= j))
    tempSettings1.audio1_delay_ms = k;
  return false;
}

void TOptions6Dlg::EvMuxRate()
{
  int i, j, k;
  char tmpStr[80];

  muxRate->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  muxRateScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", tempSettings1.user_mux_rate);
    muxRate->SetText(tmpStr);
  }
  else
    tempSettings1.user_mux_rate = i;

  if (tempSettings1.user_mux_rate)
  {
    compBitrate->SetSelIndex(COMPBITRATE_NONE);
    compBitrate->EnableWindow(false);
  }
  else
  {
    compBitrate->SetSelIndex(tempSettings1.use_computed_bitrate);
    compBitrate->EnableWindow(true);
  }
}

void TOptions6Dlg::EvPacketSize()
{
  int i, j;
  char tmpStr[80];

  packetSize->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  if (tempSettings1.mplex_type > MPEG_VCD)
    j = MPEG2_MIN_SECTOR_SIZE;
  else
    j = MPEG1_MIN_SECTOR_SIZE;
  if ((i < j) || (i > MAX_SECTOR_SIZE))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, MAX_SECTOR_SIZE);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", tempSettings1.sector_size);
    packetSize->SetText(tmpStr);
  }
  else
    if (i != tempSettings1.sector_size)
    {
      tempSettings1.sector_size = i;
      ChangeSectorSize();
    }
}

void TOptions6Dlg::EvPacketRatio()
{
  int i, j, k;
  char tmpStr[80];

  packetRatio->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  packetRatioScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", packetRatioScroll->GetPos());
    packetRatio->SetText(tmpStr);
  }
  else
    tempSettings1.packets_per_pack = i;
}

void TOptions6Dlg::EvVBufferSize()
{
  int i, j, k;
  char tmpStr[80];

  vBufferSize->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  vBufferSizeScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", vBufferSizeScroll->GetPos());
    vBufferSize->SetText(tmpStr);
  }
  else
    tempSettings1.video_buffer_size = i;
}

void TOptions6Dlg::EvABufferSize()
{
  int i, j, k;
  char tmpStr[80];

  aBufferSize->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  aBufferSizeScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", aBufferSizeScroll->GetPos());
    aBufferSize->SetText(tmpStr);
  }
  else
    tempSettings1.audio_buffer_size = i;
}

void TOptions6Dlg::EvA1BufferSize()
{
  int i, j, k;
  char tmpStr[80];

  a1BufferSize->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  a1BufferSizeScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", a1BufferSizeScroll->GetPos());
    a1BufferSize->SetText(tmpStr);
  }
  else
    tempSettings1.audio1_buffer_size = i;
}

void TOptions6Dlg::EvPacketDelay()
{
  unsigned long i;
  char tmpStr[80];

  packetDelay->GetText(tmpStr, 30);
  sscanf(tmpStr, "%ul", &i);
  if (i > 2147483648)
  {
    MessageBox("Entry must be in the range 0 .. 2147483648", AppName, MB_OK);
    sprintf(tmpStr, "%ul", tempSettings1.sectors_delay);
    packetDelay->SetText(tmpStr);
  }
  else
    tempSettings1.sectors_delay = i;
}

void TOptions6Dlg::EvVideoDelay()
{
  int i, j, k;
  char tmpStr[80];

  videoDelay->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  videoDelayScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", videoDelayScroll->GetPos());
    videoDelay->SetText(tmpStr);
  }
  else
    tempSettings1.video_delay_ms = i;
}

void TOptions6Dlg::EvAudioDelay()
{
  int i, j, k;
  char tmpStr[80];

  audioDelay->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  audioDelayScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", audioDelayScroll->GetPos());
    audioDelay->SetText(tmpStr);
  }
  else
    tempSettings1.audio_delay_ms = i;
}

void TOptions6Dlg::EvAudio1Delay()
{
  int i, j, k;
  char tmpStr[80];

  audio1Delay->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  audio1DelayScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", audio1DelayScroll->GetPos());
    audio1Delay->SetText(tmpStr);
  }
  else
    tempSettings1.audio1_delay_ms = i;
}

void TOptions6Dlg::Help(TNotify&)
{
  WinHelp(HelpFileName, HELP_CONTEXT, 7);
}



TOptions4Dlg::TOptions4Dlg(TPropertySheet* parent)
            :TPropertyPage(parent, IDD_OPTIONS4)
{
  PageInfo.dwFlags |= PSP_HASHELP;
  layer1 = new TRadioButton(this, IDC_LAYER1);
  layer2 = new TRadioButton(this, IDC_LAYER2);
  audioBitRate = new TComboBox(this, IDC_AUDIOBITRATE);
  privateBit = new TCheckBox(this, IDC_PRIVATEBIT);
  copyrightBit = new TCheckBox(this, IDC_COPYRIGHT);
  originalBit = new TCheckBox(this, IDC_ORIGINAL);
  errorProt = new TCheckBox(this, IDC_ERRORPROT);
  psychMod1 = new TRadioButton(this, IDC_PSYCHMOD1);
  psychMod2 = new TRadioButton(this, IDC_PSYCHMOD2);
  modeGBox = new TGroupBox(this, IDC_MODEGBOX);
  modeStereo = new TRadioButton(this, IDC_MODESTEREO);
  modeJStereo = new TRadioButton(this, IDC_MODEJSTEREO);
  modeDChannel = new TRadioButton(this, IDC_MODEDCHANNEL);
  modeSChannel = new TRadioButton(this, IDC_MODESCHANNEL);
  emphNone = new TRadioButton(this, IDC_EMPHNONE);
  emph15us = new TRadioButton(this, IDC_EMPH15US);
  emphCITT = new TRadioButton(this, IDC_EMPHCITT);
}

DEFINE_RESPONSE_TABLE1(TOptions4Dlg, TPropertyPage)
  EV_CHILD_NOTIFY(IDC_LAYER1, BN_CLICKED, EvLayer1),
  EV_CHILD_NOTIFY(IDC_LAYER2, BN_CLICKED, EvLayer2),
  EV_CBN_CLOSEUP(IDC_AUDIOBITRATE, EvAudioBitRate),
  EV_CHILD_NOTIFY(IDC_PRIVATEBIT, BN_CLICKED, EvPrivateBit),
  EV_CHILD_NOTIFY(IDC_COPYRIGHT, BN_CLICKED, EvCopyrightBit),
  EV_CHILD_NOTIFY(IDC_ORIGINAL, BN_CLICKED, EvOriginalBit),
  EV_CHILD_NOTIFY(IDC_ERRORPROT, BN_CLICKED, EvErrorProt),
  EV_CHILD_NOTIFY(IDC_PSYCHMOD1, BN_CLICKED, EvPsychMod1),
  EV_CHILD_NOTIFY(IDC_PSYCHMOD2, BN_CLICKED, EvPsychMod2),
  EV_CHILD_NOTIFY(IDC_MODESTEREO, BN_CLICKED, EvModeStereo),
  EV_CHILD_NOTIFY(IDC_MODEJSTEREO, BN_CLICKED, EvModeJStereo),
  EV_CHILD_NOTIFY(IDC_MODEDCHANNEL, BN_CLICKED, EvModeDChannel),
  EV_CHILD_NOTIFY(IDC_MODESCHANNEL, BN_CLICKED, EvModeSChannel),
  EV_CHILD_NOTIFY(IDC_EMPHNONE, BN_CLICKED, EvEmphNone),
  EV_CHILD_NOTIFY(IDC_EMPH15US, BN_CLICKED, EvEmph15us),
  EV_CHILD_NOTIFY(IDC_EMPHCITT, BN_CLICKED, EvEmphCITT),
  EV_WM_SHOWWINDOW,
END_RESPONSE_TABLE;

void TOptions4Dlg::SetupWindow()
{
  TPropertyPage::SetupWindow();
  ChangeSettings();
}

void TOptions4Dlg::EvShowWindow(bool show, uint status)
{
  TPropertyPage::EvShowWindow(show, status);
  if (show)
    ChangeSettings();
}

void TOptions4Dlg::ChangeSettings()
{
  if (tempSettings1.audio_layer == 1)
    layer1->Check();
  else
    layer2->Check();
  ChangeType();
  if (tempSettings1.extension)
    privateBit->Check();
  else
    privateBit->Uncheck();
  if (tempSettings1.copyright)
    copyrightBit->Check();
  else
    copyrightBit->Uncheck();
  if (tempSettings1.original)
    originalBit->Check();
  else
    originalBit->Uncheck();
  if (tempSettings1.error_protection)
    errorProt->Check();
  else
    errorProt->Uncheck();
  if (tempSettings1.psych_model == 1)
    psychMod1->Check();
  else
    psychMod2->Check();

  if (tempSettings1.audio_mode == MPG_MD_STEREO)
    modeStereo->Check();
  else
    if (tempSettings1.audio_mode == MPG_MD_JOINT_STEREO)
      modeJStereo->Check();
    else
      if (tempSettings1.audio_mode == MPG_MD_DUAL_CHANNEL)
        modeDChannel->Check();
      else
        modeSChannel->Check();

  if (audioStereo)
  {
    modeStereo->EnableWindow(true);
    modeJStereo->EnableWindow(true);
    modeDChannel->EnableWindow(true);
    modeSChannel->EnableWindow(false);
    modeGBox->EnableWindow(true);
  }
  else
  {
    modeStereo->EnableWindow(false);
    modeJStereo->EnableWindow(false);
    modeDChannel->EnableWindow(false);
    modeSChannel->EnableWindow(false);
    modeGBox->EnableWindow(false);
  }

  if (tempSettings1.emphasis == 0)
    emphNone->Check();
  else
    if (tempSettings1.emphasis == 1)
      emph15us->Check();
    else
      emphCITT->Check();
}

void TOptions4Dlg::ChangeType()
{
  int i;
  char tmpStr[5];
  static int bitrate[2][15] = {
          {0,32,64,96,128,160,192,224,256,288,320,352,384,416,448},
          {0,32,48,56,64,80,96,112,128,160,192,224,256,320,384}};

  audioBitRate->ClearList();
  if (tempSettings1.video_type == MPEG_SVCD)
  {
    if (tempSettings1.audio_mode == MPG_MD_MONO)
      for (i = 1; i < 4; i++)
      {
        sprintf(tmpStr, "%d", bitrate[tempSettings1.audio_layer - 1][i]);
        audioBitRate->AddString(tmpStr);
      }
    for (i = 4; i < 11; i++)
    {
      sprintf(tmpStr, "%d", bitrate[tempSettings1.audio_layer - 1][i]);
      audioBitRate->AddString(tmpStr);
    }
  }
  else
  {
    audioBitRate->AddString("free format");
    for (i = 1; i < 11; i++)
    {
      sprintf(tmpStr, "%d", bitrate[tempSettings1.audio_layer - 1][i]);
      audioBitRate->AddString(tmpStr);
    }
  }
  if (tempSettings1.audio_mode == MPG_MD_MONO)
  {
    if (tempSettings1.audio_bitrate > 10)
      tempSettings1.audio_bitrate = 10;
  }
  else
    for (i = 11; i < 15; i++)
    {
      sprintf(tmpStr, "%d", bitrate[tempSettings1.audio_layer - 1][i]);
      audioBitRate->AddString(tmpStr);
    }
  if (tempSettings1.video_type == MPEG_SVCD)
  {
    if (tempSettings1.audio_mode == MPG_MD_MONO)
      audioBitRate->SetSelIndex(tempSettings1.audio_bitrate - 1);
    else
      audioBitRate->SetSelIndex(tempSettings1.audio_bitrate - 4);
  }
  else
    audioBitRate->SetSelIndex(tempSettings1.audio_bitrate);
}

void TOptions4Dlg::EvLayer1()
{
  if (layer1->GetCheck() == BF_CHECKED)
  {
    tempSettings1.audio_layer = 1;
    ChangeType();
  }
}

void TOptions4Dlg::EvLayer2()
{
  if (layer2->GetCheck() == BF_CHECKED)
  {
    tempSettings1.audio_layer = 2;
    ChangeType();
  }
}

void TOptions4Dlg::EvAudioBitRate()
{
  tempSettings1.audio_bitrate = audioBitRate->GetSelIndex();
  if (tempSettings1.video_type == MPEG_SVCD)
  {
    if (tempSettings1.audio_mode == MPG_MD_MONO)
      tempSettings1.audio_bitrate++;
    else
      tempSettings1.audio_bitrate += 4;
    AutoSetBitrateData(&tempSettings1);
  }
}

void TOptions4Dlg::EvPrivateBit()
{
  tempSettings1.extension = (privateBit->GetCheck() == BF_CHECKED);
}

void TOptions4Dlg::EvCopyrightBit()
{
  tempSettings1.copyright = (copyrightBit->GetCheck() == BF_CHECKED);
}

void TOptions4Dlg::EvOriginalBit()
{
  tempSettings1.original = (originalBit->GetCheck() == BF_CHECKED);
}

void TOptions4Dlg::EvErrorProt()
{
  tempSettings1.error_protection = (errorProt->GetCheck() == BF_CHECKED);
}

void TOptions4Dlg::EvPsychMod1()
{
  if (psychMod1->GetCheck() == BF_CHECKED)
    tempSettings1.psych_model = 1;
}

void TOptions4Dlg::EvPsychMod2()
{
  if (psychMod2->GetCheck() == BF_CHECKED)
    tempSettings1.psych_model = 2;
}

void TOptions4Dlg::EvModeStereo()
{
  if (modeStereo->GetCheck() == BF_CHECKED)
    tempSettings1.audio_mode = MPG_MD_STEREO;
}

void TOptions4Dlg::EvModeJStereo()
{
  if (modeJStereo->GetCheck() == BF_CHECKED)
    tempSettings1.audio_mode = MPG_MD_JOINT_STEREO;
}

void TOptions4Dlg::EvModeDChannel()
{
  if (modeDChannel->GetCheck() == BF_CHECKED)
    tempSettings1.audio_mode = MPG_MD_DUAL_CHANNEL;
}

void TOptions4Dlg::EvModeSChannel()
{
  if (modeSChannel->GetCheck() == BF_CHECKED)
    tempSettings1.audio_mode = MPG_MD_MONO;
}

void TOptions4Dlg::EvEmphNone()
{
  if (emphNone->GetCheck() == BF_CHECKED)
    tempSettings1.emphasis = 0;
}

void TOptions4Dlg::EvEmph15us()
{
  if (emph15us->GetCheck() == BF_CHECKED)
    tempSettings1.emphasis = 1;
}

void TOptions4Dlg::EvEmphCITT()
{
  if (emphCITT->GetCheck() == BF_CHECKED)
    tempSettings1.emphasis = 3;
}

void TOptions4Dlg::Help(TNotify&)
{
  WinHelp(HelpFileName, HELP_CONTEXT, 6);
}

bool TOptions4Dlg::CanClose()
{
  return TPropertyPage::CanClose();
}



TOptions3Dlg::TOptions3Dlg(TPropertySheet* parent)
            :TPropertyPage(parent, IDD_OPTIONS3)
{
  int i, k, l;

  PageInfo.dwFlags |= PSP_HASHELP;
  colorPrim = new TComboBox(this, IDC_PRIMARIES);
  transferChar = new TComboBox(this, IDC_TRANSFERCHAR);
  matrixCoeff = new TComboBox(this, IDC_MATRIXCOEFF);
  videoFormat = new TComboBox(this, IDC_VIDEOFORMAT);
  intraDCPrec = new TComboBox(this, IDC_INTRADCPREC);
  settingTree = new TTreeWindow(this, IDC_ADVANCEDTREE);
  settingVal = new TEdit(this, IDC_SETTINGVAL);
  settingScroll = new TUpDown(this, IDC_SETTINGSCROLL, settingVal);
  settingTxt = new TStatic(this, IDC_SETTINGTXT);
  motionEst = new THSlider(this, IDC_MOTIONEST);
  motionEstVal = new TEdit(this, IDC_MOTIONESTVAL);

  nonMotionPtr[VBV_BUFFER_SIZE]           = &tempSettings1.vbv_buffer_size;
  nonMotionPtr[FIXED_VBV_DELAY]           = &tempSettings1.fixed_vbv_delay;
  nonMotionPtr[DISPLAY_HORIZONTAL_SIZE]   =
      &tempSettings1.display_horizontal_size;
  nonMotionPtr[DISPLAY_VERTICAL_SIZE]     =
      &tempSettings1.display_vertical_size;
  nonMotionPtr[PROG_SEQ]                  = &tempSettings1.prog_seq;
  nonMotionPtr[PROG_FRAME]                = &tempSettings1.prog_frame;
  nonMotionPtr[FIELDPIC]                  = &tempSettings1.fieldpic;
  nonMotionPtr[TOPFIRST]                  = &tempSettings1.topfirst;
  nonMotionPtr[REPEATFIRST]               = &tempSettings1.repeatfirst;

  nonMotionPtr[INTRAVLC_TAB_0]            = &tempSettings1.intravlc_tab[0];
  nonMotionPtr[INTRAVLC_TAB_1]            = &tempSettings1.intravlc_tab[1];
  nonMotionPtr[INTRAVLC_TAB_2]            = &tempSettings1.intravlc_tab[2];

  nonMotionPtr[FRAME_PRED_DCT_TAB_0]      =
      &tempSettings1.frame_pred_dct_tab[0];
  nonMotionPtr[FRAME_PRED_DCT_TAB_1]      =
      &tempSettings1.frame_pred_dct_tab[1];
  nonMotionPtr[FRAME_PRED_DCT_TAB_2]      =
      &tempSettings1.frame_pred_dct_tab[2];

  nonMotionPtr[QSCALE_TAB_0]              = &tempSettings1.qscale_tab[0];
  nonMotionPtr[QSCALE_TAB_1]              = &tempSettings1.qscale_tab[1];
  nonMotionPtr[QSCALE_TAB_2]              = &tempSettings1.qscale_tab[2];

  nonMotionPtr[ALTSCAN_TAB_0]             = &tempSettings1.altscan_tab[0];
  nonMotionPtr[ALTSCAN_TAB_1]             = &tempSettings1.altscan_tab[1];
  nonMotionPtr[ALTSCAN_TAB_2]             = &tempSettings1.altscan_tab[2];

  nonMotionPtr[SEQ_DSPLY_EXT]             = &tempSettings1.write_sde;
  nonMotionPtr[SEQ_ENDCODE]               = &tempSettings1.write_sec;
  nonMotionPtr[SVCD_USER_BLOCKS]          = &tempSettings1.embed_SVCD_user_blocks;
  nonMotionPtr[SLICE_HDR_MBROW]           = &tempSettings1.slice_hdr_every_MBrow;

  nonMotionPtr[R]                         = &tempSettings1.r;
  nonMotionPtr[AVG_ACT]                   = &tempSettings1.avg_act;

  nonMotionPtr[X_I]                       = &tempSettings1.Xi;
  nonMotionPtr[X_P]                       = &tempSettings1.Xp;
  nonMotionPtr[X_B]                       = &tempSettings1.Xb;

  nonMotionPtr[D0_I]                      = &tempSettings1.d0i;
  nonMotionPtr[D0_P]                      = &tempSettings1.d0p;
  nonMotionPtr[D0_B]                      = &tempSettings1.d0b;

  nonMotionPtr[MIN_FRAME_PERCENT]         = &tempSettings1.min_frame_percent;
  nonMotionPtr[PAD_FRAME_PERCENT]         = &tempSettings1.pad_frame_percent;
  nonMotionPtr[RESET_PBD0]                = &tempSettings1.reset_d0pb;

  nonMotionPtr[AUTOMOTION]                = &tempSettings1.automotion;
  nonMotionPtr[XMOTION]                   = &tempSettings1.xmotion;
  nonMotionPtr[YMOTION]                   = &tempSettings1.ymotion;

  motionHCodePtr[0] = &tempSettings1.motion_data[0].forw_hor_f_code;
  motionVCodePtr[0] = &tempSettings1.motion_data[0].forw_vert_f_code;
  motionSrchPtr[0] = &tempSettings1.motion_data[0].sxf;
  motionSrchPtr[1] = &tempSettings1.motion_data[0].syf;

  k = 1;
  l = 2;
  for (i = 1; i < MAXM; i++)
  {
    motionHCodePtr[k] = &tempSettings1.motion_data[i].forw_hor_f_code;
    motionVCodePtr[k++] = &tempSettings1.motion_data[i].forw_vert_f_code;

    motionSrchPtr[l++] = &tempSettings1.motion_data[i].sxf;
    motionSrchPtr[l++] = &tempSettings1.motion_data[i].syf;

    motionHCodePtr[k] = &tempSettings1.motion_data[i].back_hor_f_code;
    motionVCodePtr[k++] = &tempSettings1.motion_data[i].back_vert_f_code;

    motionSrchPtr[l++] = &tempSettings1.motion_data[i].sxb;
    motionSrchPtr[l++] = &tempSettings1.motion_data[i].syb;
  }

  nonMotionRange[XMOTION][1] = horizontal_size;
  nonMotionRange[YMOTION][1] = vertical_size;
}

DEFINE_RESPONSE_TABLE1(TOptions3Dlg, TPropertyPage)
  EV_CBN_CLOSEUP(IDC_PRIMARIES, EvColorPrim),
  EV_CBN_CLOSEUP(IDC_TRANSFERCHAR, EvTransferChar),
  EV_CBN_CLOSEUP(IDC_MATRIXCOEFF, EvMatrixCoeff),
  EV_CBN_CLOSEUP(IDC_VIDEOFORMAT, EvVideoFormat),
  EV_CBN_CLOSEUP(IDC_INTRADCPREC, EvIntraDCPrec),
  EV_EN_KILLFOCUS(IDC_SETTINGVAL, EvSettingVal),
  EV_UDN_DELTAPOS(IDC_SETTINGSCROLL, EvSettingBump),
  EV_TVN_SELCHANGED(IDC_ADVANCEDTREE, EvSettingTree),
  EV_WM_HSCROLL,
  EV_WM_SHOWWINDOW,
END_RESPONSE_TABLE;

void TOptions3Dlg::SetupWindow()
{
  TPropertyPage::SetupWindow();
  ChangeSettings();
}

void TOptions3Dlg::EvShowWindow(bool show, uint status)
{
  TPropertyPage::EvShowWindow(show, status);
  if (show)
    ChangeSettings();
}

void TOptions3Dlg::EvHScroll(uint scrollCode, uint thumbPos, THandle hWndCtl)
{
  int i, j, k;
  char tmpStr[10];

  TDialog::EvHScroll(scrollCode, thumbPos, hWndCtl);

  if (scrollCode != SB_THUMBTRACK)
  {
    if (hWndCtl == motionEst->HWindow)
    {
      i = motionEst->GetPosition();
      motionEst->GetRange(j, k);
      if (i < j)
        i = j;
      if (i > k)
        i = k;
      tempSettings1.maxmotion = i;
      if (i > 7)
        sprintf(tmpStr, "%d", i);
      else
        strcpy(tmpStr, "Off");
      motionEstVal->SetText(tmpStr);
    }
  }
}

void TOptions3Dlg::ChangeSettings()
{
  ChangeOtherStuff();
  ChangeIntraDC();
  ChangeVBVBuffer();
  ChangeMotionEst();
  ChangeProgressive();

  TTreeNode root = settingTree->GetRoot();
  TTreeNode general = root.AddChild(TTreeNode(*settingTree, "General"));
  ChangeTree();
}

void TOptions3Dlg::ChangeOtherStuff()
{
  int i;

  colorPrim->ClearList();
  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    colorPrim->AddString("Not applicable");
    colorPrim->SetSelIndex(0);
    colorPrim->EnableWindow(false);
  }
  else
  {
    colorPrim->EnableWindow(true);
    colorPrim->AddString("ITU-R Rec. 709 (1990)");
    colorPrim->AddString("unspecified");
    colorPrim->AddString("ITU-R Rec. 624-4 System M");
    colorPrim->AddString("ITU-R Rec. 624-4 System B, G");
    colorPrim->AddString("SMPTE 170M");
    colorPrim->AddString("SMPTE 240M (1987)");
    i = tempSettings1.color_primaries;
    if (i < 3)
      i--;
    else
      i -= 2;
    colorPrim->SetSelIndex(i);
  }
  transferChar->ClearList();
  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    transferChar->AddString("Not applicable");
    transferChar->SetSelIndex(0);
    transferChar->EnableWindow(false);
  }
  else
  {
    transferChar->EnableWindow(true);
    transferChar->AddString("ITU-R Rec. 709 (1990)");
    transferChar->AddString("unspecified");
    transferChar->AddString("ITU-R Rec. 624-4 System M");
    transferChar->AddString("ITU-R Rec. 624-4 System B, G");
    transferChar->AddString("SMPTE 170M");
    transferChar->AddString("SMPTE 240M (1987)");
    transferChar->AddString("linear transfer characteristics");
    i = tempSettings1.transfer_characteristics;
    if (i < 3)
      i--;
    else
      i -= 2;
    transferChar->SetSelIndex(i);
  }

  matrixCoeff->ClearList();
  matrixCoeff->AddString("ITU-R Rec. 709 (1990)");
  matrixCoeff->AddString("unspecified");
  matrixCoeff->AddString("FCC");
  matrixCoeff->AddString("ITU-R Rec. 624-4 System B, G");
  matrixCoeff->AddString("SMPTE 170M");
  matrixCoeff->AddString("SMPTE 240M (1987)");
  i = tempSettings1.matrix_coefficients;
  if (i < 3)
    i--;
  else
    i -= 2;
  matrixCoeff->SetSelIndex(i);

  videoFormat->ClearList();
  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    videoFormat->AddString("N/A");
    videoFormat->SetSelIndex(0);
    videoFormat->EnableWindow(false);
  }
  else
  {
    videoFormat->EnableWindow(true);
    videoFormat->AddString("composite");
    videoFormat->AddString("PAL");
    videoFormat->AddString("NTSC");
    videoFormat->AddString("SECAM");
    videoFormat->AddString("MAC");
    videoFormat->AddString("unspecified");
    videoFormat->SetSelIndex(tempSettings1.video_format);
  }
}

void TOptions3Dlg::ChangeIntraDC()
{
  intraDCPrec->ClearList();
  intraDCPrec->AddString(" 8 bit");
  if (tempSettings1.video_type < MPEG_MPEG2)
    intraDCPrec->EnableWindow(false);
  else
  {
    intraDCPrec->EnableWindow(true);
    intraDCPrec->AddString(" 9 bit");
    intraDCPrec->AddString("10 bit");
    if (tempSettings1.profile < 4)
      intraDCPrec->AddString("11 bit");
  }
  intraDCPrec->SetSelIndex(tempSettings1.dc_prec);
}

void TOptions3Dlg::ChangeVBVBuffer()
{
  int i;

  if (tempSettings1.auto_bitrate)
  {
    nonMotionRange[VBV_BUFFER_SIZE][0] = tempSettings1.vbv_buffer_size;
    nonMotionRange[VBV_BUFFER_SIZE][1] = tempSettings1.vbv_buffer_size;
  }
  else
  {
    nonMotionRange[VBV_BUFFER_SIZE][0] = 0;
    if (tempSettings1.video_type < MPEG_MPEG2)
      i = 1023;
    else
      i = vbvlim[(tempSettings1.level - 4) >> 1];
    nonMotionRange[VBV_BUFFER_SIZE][1] = i;
    if (tempSettings1.vbv_buffer_size > i)
      tempSettings1.vbv_buffer_size = i;
  }
}

void TOptions3Dlg::ChangeMotionEst()
{
  int i, hor, vert;
  char tmpStr[10];

  if (tempSettings1.constrparms)
  {
    hor = 4;
    vert = 4;
    i = 58;
  }
  else
    if (tempSettings1.video_type < MPEG_MPEG2)
    {
      hor = 7;
      vert = 7;
      i = 506;
    }
    else
    {
      hor = horzlim[(tempSettings1.level - 4) >> 1];
      vert = vertlim[(tempSettings1.level - 4) >> 1];
      if (vert == 5)
        i = 122;
      else
        i = 58;
    }
  if (tempSettings1.maxmotion > i)
    tempSettings1.maxmotion = i;
  motionEst->SetRange(7, i);
  motionEst->SetRuler((i - 7) / 10, false);
  motionEst->SetLineMagnitude(1);
  motionEst->SetPageMagnitude(i / 10);
  if (tempSettings1.maxmotion < 8)
  {
    strcpy(tmpStr, "Off");
    motionEst->SetPosition(7);
  }
  else
  {
    sprintf(tmpStr, "%d", tempSettings1.maxmotion);
    motionEst->SetPosition(tempSettings1.maxmotion);
  }
  motionEstVal->SetText(tmpStr);
  for (i = 0; i < MAXM; i++)
  {
    if (tempSettings1.motion_data[i].forw_hor_f_code > hor)
      tempSettings1.motion_data[i].forw_hor_f_code = hor;
    if (tempSettings1.motion_data[i].forw_vert_f_code > vert)
      tempSettings1.motion_data[i].forw_vert_f_code = vert;
    if (i != 0)
    {
      if (tempSettings1.motion_data[i].back_hor_f_code > hor)
        tempSettings1.motion_data[i].back_hor_f_code = hor;
      if (tempSettings1.motion_data[i].back_vert_f_code > vert)
        tempSettings1.motion_data[i].back_vert_f_code = vert;
    }
  }
}

void TOptions3Dlg::ChangeProgressive()
{
  int i;
  bool constrain_repeat;

  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    for (i = DISPLAY_HORIZONTAL_SIZE; i <= INTRAVLC_TAB_2; i++)
      nonMotionRange[i][1] = 0;
    for (i = FRAME_PRED_DCT_TAB_0; i <= FRAME_PRED_DCT_TAB_2; i++)
      nonMotionRange[i][0] = 1;
    for (i = QSCALE_TAB_0; i <= ALTSCAN_TAB_2; i++)
      nonMotionRange[i][1] = 0;
  }
  else
  {
    nonMotionRange[DISPLAY_HORIZONTAL_SIZE][1] = 16383;
    nonMotionRange[DISPLAY_VERTICAL_SIZE][1] = 16383;
    for (i = PROG_SEQ; i <= ALTSCAN_TAB_2; i++)
    {
      nonMotionRange[i][1] = 1;
      nonMotionRange[i][0] = 0;
    }

    constrain_repeat = false;
    if (tempSettings1.profile != 1)
    {
      if (tempSettings1.frame_rate_code < 3)
      {
        constrain_repeat = true;
        tempSettings1.repeatfirst = 0;
      }
      else
        if ((tempSettings1.frame_rate_code < 7) && (tempSettings1.prog_seq))
        {
          constrain_repeat = true;
          tempSettings1.repeatfirst = 0;
        }
    }
    if (constrain_repeat)
    {
      tempSettings1.repeatfirst = 0;
      nonMotionRange[REPEATFIRST][1] = 0;
    }

    if (tempSettings1.prog_seq)
    {
      tempSettings1.prog_frame = 1;
      nonMotionRange[PROG_FRAME][0] = 1;
      if (constrain_repeat)
      {
        tempSettings1.topfirst = 0;
        nonMotionRange[TOPFIRST][1] = 0;
      }
      else
      {
        if (tempSettings1.topfirst)
        {
          tempSettings1.repeatfirst = 1;
          nonMotionRange[REPEATFIRST][0] = 1;
        }
      }
    }
    if (tempSettings1.prog_frame)
    {
      tempSettings1.fieldpic = 0;
      nonMotionRange[FIELDPIC][1] = 0;
      for (i = 0; i < 3; i++)
      {
        tempSettings1.frame_pred_dct_tab[i] = 1;
        nonMotionRange[i + FRAME_PRED_DCT_TAB_0][0] = 1;
      }
    }
    else
    {
      if (!constrain_repeat)
      {
        tempSettings1.repeatfirst = 0;
        nonMotionRange[REPEATFIRST][1] = 0;
      }
    }
    if (tempSettings1.fieldpic)
    {
      for (i = 0; i < 3; i++)
      {
        tempSettings1.frame_pred_dct_tab[i] = 0;
        nonMotionRange[i + FRAME_PRED_DCT_TAB_0][1] = 0;
      }
    }
    else
    {
      for (i = 0; i < 3; i++)
        nonMotionRange[i + FRAME_PRED_DCT_TAB_0][1] = 1;
    }
  }
}

void TOptions3Dlg::ChangeTree()
{
  int i, j, k, l;
  char tmpStr[30];

  settingTree->DeleteAllItems();

  TTreeNode root = settingTree->GetRoot();
  TTreeNode general = root.AddChild(TTreeNode(*settingTree, "General"));
  parentNodes[0] = general.GetHTreeItem();
  TTreeNode temp = general.AddChild(TTreeNode(*settingTree, "VBV Buffer Size"));
  nonMotionNodes[VBV_BUFFER_SIZE] = temp.GetHTreeItem();
  temp = general.AddChild(TTreeNode(*settingTree, "Force VBV Delay"));
  nonMotionNodes[FIXED_VBV_DELAY] = temp.GetHTreeItem();
  TTreeNode displaySize = general.AddChild(TTreeNode(*settingTree, "Display Size"));
  parentNodes[1] = displaySize.GetHTreeItem();

  temp = displaySize.AddChild(TTreeNode(*settingTree, "Horizontal"));
  nonMotionNodes[DISPLAY_HORIZONTAL_SIZE] = temp.GetHTreeItem();
  temp = displaySize.AddChild(TTreeNode(*settingTree, "Vertical"));
  nonMotionNodes[DISPLAY_VERTICAL_SIZE] = temp.GetHTreeItem();
  temp = general.AddChild(TTreeNode(*settingTree, "Progressive Sequence"));
  nonMotionNodes[PROG_SEQ] = temp.GetHTreeItem();
  temp = general.AddChild(TTreeNode(*settingTree, "Progressive Frame"));
  nonMotionNodes[PROG_FRAME] = temp.GetHTreeItem();
  temp = general.AddChild(TTreeNode(*settingTree, "Field Pictures"));
  nonMotionNodes[FIELDPIC] = temp.GetHTreeItem();
  temp = general.AddChild(TTreeNode(*settingTree, "Top Field First"));
  nonMotionNodes[TOPFIRST] = temp.GetHTreeItem();
  temp = general.AddChild(TTreeNode(*settingTree, "Repeat First Field"));
  nonMotionNodes[REPEATFIRST] = temp.GetHTreeItem();

  TTreeNode intraVLC = general.AddChild(TTreeNode(*settingTree, "Intra VLC Format"));
  parentNodes[2] = intraVLC.GetHTreeItem();
  temp = intraVLC.AddChild(TTreeNode(*settingTree, "I Frame"));
  nonMotionNodes[INTRAVLC_TAB_0] = temp.GetHTreeItem();
  temp = intraVLC.AddChild(TTreeNode(*settingTree, "P Frame"));
  nonMotionNodes[INTRAVLC_TAB_1] = temp.GetHTreeItem();
  temp = intraVLC.AddChild(TTreeNode(*settingTree, "B Frame"));
  nonMotionNodes[INTRAVLC_TAB_2] = temp.GetHTreeItem();

  TTreeNode framePred = general.AddChild(TTreeNode(*settingTree, "Use Frame Prediction and Frame DCT"));
  parentNodes[3] = framePred.GetHTreeItem();
  temp = framePred.AddChild(TTreeNode(*settingTree, "I Frame"));
  nonMotionNodes[FRAME_PRED_DCT_TAB_0] = temp.GetHTreeItem();
  temp = framePred.AddChild(TTreeNode(*settingTree, "P Frame"));
  nonMotionNodes[FRAME_PRED_DCT_TAB_1] = temp.GetHTreeItem();
  temp = framePred.AddChild(TTreeNode(*settingTree, "B Frame"));
  nonMotionNodes[FRAME_PRED_DCT_TAB_2] = temp.GetHTreeItem();

  TTreeNode qScale = general.AddChild(TTreeNode(*settingTree, "Quantization Scale Type"));
  parentNodes[4] = qScale.GetHTreeItem();
  temp = qScale.AddChild(TTreeNode(*settingTree, "I Frame"));
  nonMotionNodes[QSCALE_TAB_0] = temp.GetHTreeItem();
  temp = qScale.AddChild(TTreeNode(*settingTree, "P Frame"));
  nonMotionNodes[QSCALE_TAB_1] = temp.GetHTreeItem();
  temp = qScale.AddChild(TTreeNode(*settingTree, "B Frame"));
  nonMotionNodes[QSCALE_TAB_2] = temp.GetHTreeItem();

  TTreeNode altScan = general.AddChild(TTreeNode(*settingTree, "Use Alternate Scanning Pattern"));
  parentNodes[5] = altScan.GetHTreeItem();
  temp = altScan.AddChild(TTreeNode(*settingTree, "I Frame"));
  nonMotionNodes[ALTSCAN_TAB_0] = temp.GetHTreeItem();
  temp = altScan.AddChild(TTreeNode(*settingTree, "P Frame"));
  nonMotionNodes[ALTSCAN_TAB_1] = temp.GetHTreeItem();
  temp = altScan.AddChild(TTreeNode(*settingTree, "B Frame"));
  nonMotionNodes[ALTSCAN_TAB_2] = temp.GetHTreeItem();

  temp = general.AddChild(TTreeNode(*settingTree, "Sequence Display Extension"));
  nonMotionNodes[SEQ_DSPLY_EXT] = temp.GetHTreeItem();

  temp = general.AddChild(TTreeNode(*settingTree, "Sequence End Code"));
  nonMotionNodes[SEQ_ENDCODE] = temp.GetHTreeItem();

  temp = general.AddChild(TTreeNode(*settingTree, "Embed SVCD User Blocks"));
  nonMotionNodes[SVCD_USER_BLOCKS] = temp.GetHTreeItem();

  temp = general.AddChild(TTreeNode(*settingTree, "Write Slice Headers"));
  nonMotionNodes[SLICE_HDR_MBROW] = temp.GetHTreeItem();

  TTreeNode rateControl = root.AddChild(TTreeNode(*settingTree, "Rate Control"));
  parentNodes[6] = rateControl.GetHTreeItem();
  temp = rateControl.AddChild(TTreeNode(*settingTree, "Reaction Parameter"));
  nonMotionNodes[R] = temp.GetHTreeItem();
  temp = rateControl.AddChild(TTreeNode(*settingTree, "Initial Average Activity"));
  nonMotionNodes[AVG_ACT] = temp.GetHTreeItem();

  TTreeNode xMeasure = rateControl.AddChild(TTreeNode(*settingTree, "Initial Global Complexity Measure"));
  parentNodes[7] = xMeasure.GetHTreeItem();
  temp = xMeasure.AddChild(TTreeNode(*settingTree, "I Frame"));
  nonMotionNodes[X_I] = temp.GetHTreeItem();
  temp = xMeasure.AddChild(TTreeNode(*settingTree, "P Frame"));
  nonMotionNodes[X_P] = temp.GetHTreeItem();
  temp = xMeasure.AddChild(TTreeNode(*settingTree, "B Frame"));
  nonMotionNodes[X_B] = temp.GetHTreeItem();

  TTreeNode d0 = rateControl.AddChild(TTreeNode(*settingTree, "Initial Virtual Buffer Fullness"));
  parentNodes[8] = d0.GetHTreeItem();
  temp = d0.AddChild(TTreeNode(*settingTree, "I Frame"));
  nonMotionNodes[D0_I] = temp.GetHTreeItem();
  temp = d0.AddChild(TTreeNode(*settingTree, "P Frame"));
  nonMotionNodes[D0_P] = temp.GetHTreeItem();
  temp = d0.AddChild(TTreeNode(*settingTree, "B Frame"));
  nonMotionNodes[D0_B] = temp.GetHTreeItem();

  temp = rateControl.AddChild(TTreeNode(*settingTree, "Minimum Frame Percentage"));
  nonMotionNodes[MIN_FRAME_PERCENT] = temp.GetHTreeItem();
  temp = rateControl.AddChild(TTreeNode(*settingTree, "Pad Frame Percentage"));
  nonMotionNodes[PAD_FRAME_PERCENT] = temp.GetHTreeItem();
  temp = rateControl.AddChild(TTreeNode(*settingTree, "Reset P and B IVBF"));
  nonMotionNodes[RESET_PBD0] = temp.GetHTreeItem();

  TTreeNode motionEst = root.AddChild(TTreeNode(*settingTree, "Fixed Motion Estimation"));
  parentNodes[9] = motionEst.GetHTreeItem();
  temp = motionEst.AddChild(TTreeNode(*settingTree, "Auto set Vector Lengths"));
  nonMotionNodes[AUTOMOTION] = temp.GetHTreeItem();
  TTreeNode pelMove = motionEst.AddChild(TTreeNode(*settingTree, "Pel Movement"));
  parentNodes[10] = pelMove.GetHTreeItem();
  temp = pelMove.AddChild(TTreeNode(*settingTree, "Horizontal"));
  nonMotionNodes[XMOTION] = temp.GetHTreeItem();
  temp = pelMove.AddChild(TTreeNode(*settingTree, "Vertical"));
  nonMotionNodes[YMOTION] = temp.GetHTreeItem();

  TTreeNode pFrame = motionEst.AddChild(TTreeNode(*settingTree, "P Frame Vector Length"));
  parentNodes[11] = pFrame.GetHTreeItem();
  temp = pFrame.AddChild(TTreeNode(*settingTree, "Forward Horz F Code"));
  motionHCodeNodes[0] = temp.GetHTreeItem();
  temp = pFrame.AddChild(TTreeNode(*settingTree, "Forward Vert F Code"));
  motionVCodeNodes[0] = temp.GetHTreeItem();
  temp = pFrame.AddChild(TTreeNode(*settingTree, "Forward Search Width"));
  motionSrchNodes[0] = temp.GetHTreeItem();
  temp = pFrame.AddChild(TTreeNode(*settingTree, "Forward Search Height"));
  motionSrchNodes[1] = temp.GetHTreeItem();

  TTreeNode bFrames = motionEst.AddChild(TTreeNode(*settingTree, "B Frame Vector Lengths"));
  parentNodes[12] = bFrames.GetHTreeItem();
  j = 13;
  k = 1;
  l = 2;
  for (i = 1; i < tempSettings1.M; i++)
  {
    sprintf(tmpStr, "B Frame %d", i);
    TTreeNode thisB = bFrames.AddChild(TTreeNode(*settingTree, tmpStr));
    parentNodes[j++] = thisB.GetHTreeItem();

    temp = thisB.AddChild(TTreeNode(*settingTree, "Forward Horz F Code"));
    motionHCodeNodes[k] = temp.GetHTreeItem();
    temp = thisB.AddChild(TTreeNode(*settingTree, "Forward Vert F Code"));
    motionVCodeNodes[k++] = temp.GetHTreeItem();

    temp = thisB.AddChild(TTreeNode(*settingTree, "Forward Search Width"));
    motionSrchNodes[l++] = temp.GetHTreeItem();
    temp = thisB.AddChild(TTreeNode(*settingTree, "Forward Search Height"));
    motionSrchNodes[l++] = temp.GetHTreeItem();

    temp = thisB.AddChild(TTreeNode(*settingTree, "Backward Horz F Code"));
    motionHCodeNodes[k] = temp.GetHTreeItem();
    temp = thisB.AddChild(TTreeNode(*settingTree, "Backward Vert F Code"));
    motionVCodeNodes[k++] = temp.GetHTreeItem();

    temp = thisB.AddChild(TTreeNode(*settingTree, "Backward Search Width"));
    motionSrchNodes[l++] = temp.GetHTreeItem();
    temp = thisB.AddChild(TTreeNode(*settingTree, "Backward Search Height"));
    motionSrchNodes[l++] = temp.GetHTreeItem();
  }

  for (i = tempSettings1.M; i < MAXM; i++)
  {
    motionHCodeNodes[k] = NULL;
    motionVCodeNodes[k++] = NULL;

    motionSrchNodes[l++] = NULL;
    motionSrchNodes[l++] = NULL;

    motionHCodeNodes[k] = NULL;
    motionVCodeNodes[k++] = NULL;

    motionSrchNodes[l++] = NULL;
    motionSrchNodes[l++] = NULL;
  }

  settingTree->Update();
  currentNode = NULL;
  ChangeCurrentNode();
}

void TOptions3Dlg::EvSettingTree(TTwNotify& nmHdr)
{
  if (settingVal->HWindow == GetFocus() && currentPtr)
  {
    int i, j, k;
    char tmpStr[80];

    settingVal->GetText(tmpStr, 30);
    sscanf(tmpStr, "%d", &i);
    settingScroll->GetRange(j, k);
    if ((i >= j) && (i <= k))
      *currentPtr = i;
  }
  currentNode = nmHdr.itemNew.hItem;
  ChangeCurrentNode();
}

void TOptions3Dlg::ChangeCurrentNode()
{
  int i, low, high;
  bool found;
  char tmpStr[15];

  currentPtr = NULL;
  settingVal->EnableWindow(false);
  settingScroll->EnableWindow(false);
  if (currentNode == NULL)
  {
    settingVal->SetText("");
    return;
  }

  i = 0;
  found = false;
  while ((i < MAXPARENT) && (!found))
  {
    if (parentNodes[i] == currentNode)
      found = true;
    else
      i++;
  }
  if (found)
  {
    settingTxt->SetText(parentTxt[i]);
    settingVal->SetText("");
    return;
  }
  i = 0;
  while ((i < MAXNONMOTION) && (!found))
  {
    if (nonMotionNodes[i] == currentNode)
      found = true;
    else
      i++;
  }
  if (found)
  {
    currentPtr = nonMotionPtr[i];
    low = nonMotionRange[i][0];
    high = nonMotionRange[i][1];
    if ((i == TOPFIRST) && (tempSettings1.fieldpic))
      settingTxt->SetText("Specify whether the bottom (or even) field (0) or the top (or odd) field (1) is encoded first in the sequence.");
    else
      if ((tempSettings1.maxmotion > 7) &&
         ((i == AUTOMOTION) || (i == XMOTION) || (i == YMOTION)))
        settingTxt->SetText(fixedMotionTxt);
      else
        settingTxt->SetText(nonMotionTxt[i]);
    settingScroll->SetRange(low, high);
    settingScroll->SetPos(*currentPtr);
    sprintf(tmpStr, "%d", *currentPtr);
    settingVal->SetText(tmpStr);
    if (low == high)
      return;
    if ((tempSettings1.maxmotion > 7) &&
       ((i == AUTOMOTION) || (i == XMOTION) || (i == YMOTION)))
      return;
    if ((!tempSettings1.automotion) && ((i == XMOTION) || (i == YMOTION)))
      return;
    settingScroll->EnableWindow(true);
    settingVal->EnableWindow(true);
    return;
  }
  i = 0;
  while ((i < MAXMOTIONHCODE) && (!found))
  {
    if (motionHCodeNodes[i] == currentNode)
      found = true;
    else
      i++;
  }
  if (found)
  {
    currentPtr = motionHCodePtr[i];
    if (tempSettings1.maxmotion > 7)
      settingTxt->SetText(fixedMotionTxt);
    else
      settingTxt->SetText(motionCodeTxt);
    if (tempSettings1.constrparms)
      settingScroll->SetRange(1, 4);
    else
      if (tempSettings1.video_type < MPEG_MPEG2)
        settingScroll->SetRange(1, 7);
      else
        settingScroll->SetRange(1, horzlim[(tempSettings1.level - 4) >> 1]);
    settingScroll->SetPos(*currentPtr);
    if (tempSettings1.automotion || (tempSettings1.maxmotion > 7))
      return;
    settingVal->EnableWindow(true);
    settingScroll->EnableWindow(true);
    return;
  }
  i = 0;
  while ((i < MAXMOTIONVCODE) && (!found))
  {
    if (motionVCodeNodes[i] == currentNode)
      found = true;
    else
      i++;
  }
  if (found)
  {
    currentPtr = motionVCodePtr[i];
    if (tempSettings1.maxmotion > 7)
      settingTxt->SetText(fixedMotionTxt);
    else
      settingTxt->SetText(motionCodeTxt);
    if (tempSettings1.constrparms)
      settingScroll->SetRange(1, 4);
    else
      if (tempSettings1.video_type < MPEG_MPEG2)
        settingScroll->SetRange(1, 5);
      else
        settingScroll->SetRange(1, vertlim[(tempSettings1.level - 4) >> 1]);
    settingScroll->SetPos(*currentPtr);
    if (tempSettings1.automotion || (tempSettings1.maxmotion > 7))
      return;
    settingVal->EnableWindow(true);
    settingScroll->EnableWindow(true);
    return;
  }
  i = 0;
  while ((i < MAXMOTIONSRCH) && (!found))
  {
    if (motionSrchNodes[i] == currentNode)
      found = true;
    else
      i++;
  }
  if (found)
  {
    currentPtr = motionSrchPtr[i];
    if (tempSettings1.maxmotion > 7)
      settingTxt->SetText(fixedMotionTxt);
    else
      settingTxt->SetText(motionSrchTxt);
    settingScroll->SetRange(1, 32767);
    settingScroll->SetPos(*currentPtr);
    if (tempSettings1.automotion || (tempSettings1.maxmotion > 7))
      return;
    settingVal->EnableWindow(true);
    settingScroll->EnableWindow(true);
  }
}

void TOptions3Dlg::EvColorPrim()
{
  int i;

  i = colorPrim->GetSelIndex();
  if (i < 2)
    i++;
  else
    i += 2;
  tempSettings1.color_primaries = i;
}

void TOptions3Dlg::EvTransferChar()
{
  int i;

  i = transferChar->GetSelIndex();
  if (i < 2)
    i++;
  else
    i += 2;
  tempSettings1.transfer_characteristics = i;
}

void TOptions3Dlg::EvMatrixCoeff()
{
  int i;

  i = matrixCoeff->GetSelIndex();
  if (i < 2)
    i++;
  else
    i += 2;
  tempSettings1.matrix_coefficients = i;
}

void TOptions3Dlg::EvVideoFormat()
{
  tempSettings1.video_format = videoFormat->GetSelIndex();
}

void TOptions3Dlg::EvIntraDCPrec()
{
  tempSettings1.dc_prec = intraDCPrec->GetSelIndex();
}

void TOptions3Dlg::EvSettingVal()
{
  int i, j, k;
  char tmpStr[80];

  settingVal->GetText(tmpStr, 30);
  sscanf(tmpStr, "%d", &i);
  settingScroll->GetRange(j, k);
  if ((i < j) || (i > k))
  {
    sprintf(tmpStr, "Entry must be in the range %d .. %d.", j, k);
    MessageBox(tmpStr, AppName, MB_OK);
    sprintf(tmpStr, "%d", settingScroll->GetPos());
    settingVal->SetText(tmpStr);
  }
  else
    if (currentPtr)
    {
      *currentPtr = i;
      SettingChange();
    }
}

bool TOptions3Dlg::EvSettingBump(TNmUpDown& not)
{
  int i, j, k;

  settingScroll->GetRange(i, j);
  k = not.iPos + not.iDelta;
  if ((k >= i) && (k <= j))
  {
    *currentPtr = k;
    SettingChange();
  }
  return false;
}

void TOptions3Dlg::SettingChange()
{
  if ((nonMotionNodes[PROG_SEQ] == currentNode) ||
      (nonMotionNodes[PROG_FRAME] == currentNode) ||
      (nonMotionNodes[TOPFIRST] == currentNode) ||
      (nonMotionNodes[FIELDPIC] == currentNode))
    ChangeProgressive();
  else
    if ((nonMotionNodes[AUTOMOTION] == currentNode) ||
        (nonMotionNodes[XMOTION] == currentNode) ||
        (nonMotionNodes[YMOTION] == currentNode))
    {
      if (tempSettings1.automotion)
        AutoSetMotionData(&tempSettings1);
    }
}

void TOptions3Dlg::Help(TNotify&)
{
  WinHelp(HelpFileName, HELP_CONTEXT, 5);
}

bool TOptions3Dlg::CanClose()
{
  return TPropertyPage::CanClose();
}



TOptions5Dlg::TOptions5Dlg(TPropertySheet* parent)
            :TPropertyPage(parent, IDD_OPTIONS5)
{
  PageInfo.dwFlags |= PSP_HASHELP;
  OptionsName = new TEdit(this, IDC_OPTIONSNAME);
  Description = new TEdit(this, IDC_DESCRIPTION);
}

DEFINE_RESPONSE_TABLE1(TOptions5Dlg, TPropertyPage)
  EV_WM_SHOWWINDOW,
  EV_COMMAND(IDC_SAVEASDEFAULT, SaveAsDefault),
  EV_COMMAND(IDC_RESETDEFAULTS, ResetDefaults),
  EV_COMMAND(IDC_LOADOPTIONS, LoadOptions),
  EV_COMMAND(IDC_SAVEOPTIONS, SaveOptions),
  EV_EN_KILLFOCUS(IDC_DESCRIPTION, EvDescription),
END_RESPONSE_TABLE;

void TOptions5Dlg::SetupWindow()
{
  TPropertyPage::SetupWindow();
  ChangeSettings();
}

void TOptions5Dlg::EvShowWindow(bool show, uint status)
{
  TPropertyPage::EvShowWindow(show, status);
  if (show)
    ChangeSettings();
}

void TOptions5Dlg::ChangeSettings()
{
  OptionsName->SetText(ParamFilename);
  Description->SetText(tempSettings1.id_string);
}

void TOptions5Dlg::Help(TNotify&)
{
  WinHelp(HelpFileName, HELP_CONTEXT, 3);
}


void TOptions5Dlg::EvDescription()
{
  Description->GetText(tempSettings1.id_string, MAXPATH);
}

void TOptions5Dlg::SaveAsDefault()
{
  char tmpStr[MAXPATH], tmpStr1[MAXPATH];

  strcpy(tmpStr1, StartupDir);
  strcat(tmpStr1, "default.ini");
  if (!WriteSettings(tmpStr1, &tempSettings1))
  {
    sprintf(tmpStr, "Unable to save settings to %s.", tmpStr1);
    MessageBox(tmpStr, AppName, MB_OK);
  }
  else
  {
    strcpy(ParamFilename, "");
    OptionsName->SetText("");
    MessageBox("Settings saved as default.", AppName, MB_OK);
  }
}

void TOptions5Dlg::ResetDefaults()
{
  char tmpStr[MAXPATH], tmpStr1[MAXPATH];

  SetMPEG2Defaults(&tempSettings1, PALDefaults);
  ChangeSettings();
  strcpy(tmpStr, StartupDir);
  strcat(tmpStr, "default.ini");
  if (!remove(tmpStr))
    MessageBox("Original default settings restored", AppName, MB_OK);
  else
    if (errno == EACCES)
    {
      sprintf(tmpStr1, "Unable to delete default file %s.", tmpStr);
      MessageBox(tmpStr1, AppName, MB_OK);
    }
    else
      MessageBox("Original default settings restored", AppName, MB_OK);
}

void TOptions5Dlg::LoadOptions()
{
  char tmpStr[MAXPATH];

  static TOpenSaveDialog::TData data (
         OFN_PATHMUSTEXIST|OFN_FILEMUSTEXIST,
         "INI Files (*.INI)|*.ini|All Files (*.*)|*.*|",
         0, "", "INI");

  sprintf(tmpStr, "%s - open settings file", AppName);
  if ((new TFileOpenDialog(this, data, 0, tmpStr))->Execute() == IDOK)
  {
    tempSettings2 = tempSettings1;
    if (ReadSettings(data.FileName, &tempSettings2))
    {
      strcpy(ParamFilename, data.FileName);
      strlwr(ParamFilename);
      tempSettings1 = tempSettings2;
      if (!VideoAvail)
        tempSettings1.EncodeVideo = FALSE;
      if (!AudioAvail)
        tempSettings1.EncodeAudio = FALSE;
      ChangeSettings();
      sprintf(tmpStr, "Settings loaded from file %s.", ParamFilename);
      MessageBox(tmpStr, AppName, MB_OK);
    }
  }
}

void TOptions5Dlg::SaveOptions()
{
  char tmpStr[MAXFILE];
  static TOpenSaveDialog::TData data (
         OFN_PATHMUSTEXIST|OFN_OVERWRITEPROMPT|OFN_HIDEREADONLY|OFN_NOREADONLYRETURN,
         "INI Files (*.INI)|*.ini|All Files (*.*)|*.*|",
         0, "", "INI");

  sprintf(tmpStr, "%s - save settings to file", AppName);
  if ((new TFileSaveDialog(this, data, 0, tmpStr))->Execute() == IDOK)
  {
    strcpy(ParamFilename, data.FileName);
    strlwr(ParamFilename);
    if (!WriteSettings(ParamFilename, &tempSettings1))
    {
      sprintf(tmpStr, "Unable to save settings to file %s.", ParamFilename);
      MessageBox(tmpStr, AppName, MB_OK);
      strcpy(ParamFilename, "");
      OptionsName->SetText("");
    }
    else
    {
      sprintf(tmpStr, "Settings saved to file %s.", ParamFilename);
      OptionsName->SetText(ParamFilename);
      MessageBox(tmpStr, AppName, MB_OK);
    }
  }
}


TOptions2Dlg::TOptions2Dlg(TPropertySheet* parent)
            :TPropertyPage(parent, IDD_OPTIONS2)
{
  PageInfo.dwFlags |= PSP_HASHELP;
  autoBitrate = new TCheckBox(this, IDC_AUTOBITRATE);
  vbrBitrate = new TCheckBox(this, IDC_VBRCHECK);
  videoPulldown = new TComboBox(this, IDC_VPULLDOWN);
  palDefaults = new TCheckBox(this, IDC_PALDEFAULTS);
  videoBitRate = new TEdit(this, IDC_VIDEOBITRATE);
  avgBitRate = new TEdit(this, IDC_AVGBITRATE);
  minBitRate = new TEdit(this, IDC_MINBITRATE);
  bitrateTxt = new TStatic(this, IDC_BITRATETXT);
  mQuant = new TEdit(this, IDC_MQUANT);
  frameRate = new TComboBox(this, IDC_FRAMERATE);
  profileID = new TComboBox(this, IDC_PROFILEID);
  levelID = new TComboBox(this, IDC_LEVELID);
  chromaFormat = new TComboBox(this, IDC_CHROMAFORMAT);
  aspectRatio = new TComboBox(this, IDC_ASPECTRATIO);
  MPEGN = new TComboBox(this, IDC_MPEGN);
  MPEGM = new TComboBox(this, IDC_MPEGM);
  MPEG1 = new TRadioButton(this, IDC_MPEG1);
  MPEG2 = new TRadioButton(this, IDC_MPEG2);
  VideoCD = new TRadioButton(this, IDC_VIDEOCD);
  SVCD = new TRadioButton(this, IDC_SVCD);
  DVD = new TRadioButton(this, IDC_DVD);
  TCHour = new TEdit(this, IDC_TCHOUR);
  TCHourScroll = new TUpDown(this, IDC_TCHOURSCROLL, TCHour);
  TCMin = new TEdit(this, IDC_TCMIN);
  TCMinScroll = new TUpDown(this, IDC_TCMINSCROLL, TCMin);
  TCSec = new TEdit(this, IDC_TCSEC);
  TCSecScroll = new TUpDown(this, IDC_TCSECSCROLL, TCSec);
  TCFrame = new TEdit(this, IDC_TCFRAME);
  TCFrameScroll = new TUpDown(this, IDC_TCFRAMESCROLL, TCFrame);
  mQuantScroll = new TUpDown(this, IDC_MQUANTSCROLL);
}

DEFINE_RESPONSE_TABLE1(TOptions2Dlg, TPropertyPage)
  EV_COMMAND(IDC_PALDEFAULTS, EvPALDefaults),
  EV_COMMAND(IDC_VBRCHECK, EvVBRBitrate),
  EV_CBN_CLOSEUP(IDC_VPULLDOWN, EvPulldown),
  EV_COMMAND(IDC_AUTOBITRATE, EvAutoBitrate),
  EV_CBN_CLOSEUP(IDC_PROFILEID, EvProfileID),
  EV_CBN_CLOSEUP(IDC_LEVELID, EvLevelID),
  EV_CBN_CLOSEUP(IDC_CHROMAFORMAT, EvChromaFormat),
  EV_CBN_CLOSEUP(IDC_ASPECTRATIO, EvAspectRatio),
  EV_CBN_CLOSEUP(IDC_FRAMERATE, EvFrameRate),
  EV_CBN_CLOSEUP(IDC_MPEGM, EvMPEGM),
  EV_CBN_CLOSEUP(IDC_MPEGN, EvMPEGN),
  EV_EN_KILLFOCUS(IDC_VIDEOBITRATE, EvBitRateKillFocus),
  EV_EN_KILLFOCUS(IDC_AVGBITRATE, EvAvgBitRateKillFocus),
  EV_EN_KILLFOCUS(IDC_MINBITRATE, EvMinBitRateKillFocus),
  EV_UDN_DELTAPOS(IDC_TCHOURSCROLL, EvTCHourBump),
  EV_UDN_DELTAPOS(IDC_TCMINSCROLL, EvTCMinBump),
  EV_UDN_DELTAPOS(IDC_TCSECSCROLL, EvTCSecBump),
  EV_UDN_DELTAPOS(IDC_TCFRAMESCROLL, EvTCFrameBump),
  EV_UDN_DELTAPOS(IDC_MQUANTSCROLL, EvMQuantBump),
  EV_CHILD_NOTIFY(IDC_MPEG1, BN_CLICKED, EvMPEGType),
  EV_CHILD_NOTIFY(IDC_MPEG2, BN_CLICKED, EvMPEGType),
  EV_CHILD_NOTIFY(IDC_VIDEOCD, BN_CLICKED, EvMPEGType),
  EV_CHILD_NOTIFY(IDC_SVCD, BN_CLICKED, EvMPEGType),
  EV_CHILD_NOTIFY(IDC_DVD, BN_CLICKED, EvMPEGType),
  EV_WM_SHOWWINDOW,
END_RESPONSE_TABLE;

void TOptions2Dlg::SetupWindow()
{
  TPropertyPage::SetupWindow();
  videoPulldown->AddString("None");
  videoPulldown->AddString("2:3");
  videoPulldown->AddString("3:2");
  mQuantScroll->SetRange(1, 31);
  ChangeSettings();
}

void TOptions2Dlg::EvShowWindow(bool show, uint status)
{
  TPropertyPage::EvShowWindow(show, status);
  if (show)
    ChangeSettings();
}

void TOptions2Dlg::ChangeSettings()
{
  TCHourScroll->SetRange(0, 23);
  TCHourScroll->SetPos(tempSettings1.hours);
  TCMinScroll->SetRange(0, 59);
  TCMinScroll->SetPos(tempSettings1.mins);
  TCSecScroll->SetRange(0, 59);
  TCSecScroll->SetPos(tempSettings1.sec);
  MPEG1->Uncheck();
  VideoCD->Uncheck();
  MPEG2->Uncheck();
  SVCD->Uncheck();
  DVD->Uncheck();
  switch (tempSettings1.video_type)
  {
    case MPEG_VCD:
      VideoCD->Check();
      break;
    case MPEG_MPEG1:
      MPEG1->Check();
      break;
    case MPEG_SVCD:
      SVCD->Check();
      break;
    case MPEG_DVD:
      DVD->Check();
      break;
    default:
      MPEG2->Check();
      break;
  }
  if (PALDefaults)
    palDefaults->Check();
  else
    palDefaults->Uncheck();
  ChangeMPEGType();
}

void TOptions2Dlg::EvVBRBitrate()
{
  tempSettings1.constant_bitrate = (vbrBitrate->GetCheck() != BF_CHECKED);
  if (tempSettings1.constant_bitrate)
  {
    tempSettings1.fixed_vbv_delay = 0;
    tempSettings1.VBR_multiplex = 0;
  }
  else
  {
    tempSettings1.fixed_vbv_delay = 1;
    tempSettings1.VBR_multiplex = 1;
  }
  ChangeVBRBitrate();
}

void TOptions2Dlg::EvAutoBitrate()
{
  tempSettings1.auto_bitrate = (autoBitrate->GetCheck() == BF_CHECKED);
  ChangeAutoBitrate();
}

void TOptions2Dlg::EvPulldown()
{
  tempSettings1.video_pulldown_flag = videoPulldown->GetSelIndex();
  if (tempSettings1.MplexVideo)
    tempSettings1.mplex_pulldown_flag = tempSettings1.video_pulldown_flag;
}

void TOptions2Dlg::EvPALDefaults()
{
  int i;

  i = (palDefaults->GetCheck() == BF_CHECKED);
  if (i != PALDefaults)
  {
    PALDefaults = i;
    switch (tempSettings1.video_type)
    {
      case MPEG_VCD:
        SetVCDDefaults(&tempSettings1, PALDefaults);
        break;
      case MPEG_MPEG1:
        SetMPEG1Defaults(&tempSettings1, PALDefaults);
        break;
      case MPEG_SVCD:
        SetSVCDDefaults(&tempSettings1, PALDefaults);
        break;
      case MPEG_DVD:
        SetDVDDefaults(&tempSettings1, PALDefaults);
        break;
      default:
        SetMPEG2Defaults(&tempSettings1, PALDefaults);
        break;
    }
    ChangeSettings();
  }
}

void TOptions2Dlg::EvMPEGType()
{
  if (MPEG1->GetCheck() == BF_CHECKED)
  {
    if (tempSettings1.video_type != MPEG_MPEG1)
      SetMPEG1Defaults(&tempSettings1, PALDefaults);
  }
  else
    if (VideoCD->GetCheck() == BF_CHECKED)
    {
      if (tempSettings1.video_type != MPEG_VCD)
        SetVCDDefaults(&tempSettings1, PALDefaults);
    }
    else
      if (SVCD->GetCheck() == BF_CHECKED)
      {
        if (tempSettings1.video_type != MPEG_SVCD)
          SetSVCDDefaults(&tempSettings1, PALDefaults);
      }
      else
        if (DVD->GetCheck() == BF_CHECKED)
        {
          if (tempSettings1.video_type != MPEG_DVD)
            SetDVDDefaults(&tempSettings1, PALDefaults);
        }
        else
        {
          if (tempSettings1.video_type != MPEG_MPEG2)
            SetMPEG2Defaults(&tempSettings1, PALDefaults);
        }
  ChangeMPEGType();
}

void TOptions2Dlg::EvProfileID()
{
  int i;

  i = profileID->GetSelIndex();
  if (i == 0)
  {
    if (tempSettings1.profile == 1)
      return;
    tempSettings1.profile = 1;
  }
  else
  {
    if (i == (tempSettings1.profile + 3))
      return;
    else
      tempSettings1.profile = i + 3;
  }
  ChangeProfile();
}

void TOptions2Dlg::EvLevelID()
{
  int i;

  if (tempSettings1.profile == 5)
    return;

  i = (levelID->GetSelIndex() << 1) + 4;
  if (i == tempSettings1.level)
    return;
  tempSettings1.level = i;
  ChangeLevel();
  AutoSetMotionData(&tempSettings1);
}

void TOptions2Dlg::EvChromaFormat()
{
  tempSettings1.chroma_format = chromaFormat->GetSelIndex() + 1;
}

void TOptions2Dlg::EvAspectRatio()
{
  tempSettings1.aspectratio = aspectRatio->GetSelIndex() + 1;
}

void TOptions2Dlg::EvFrameRate()
{
  tempSettings1.frame_rate_code = frameRate->GetSelIndex() + 1;
  tempSettings1.frame_rate = ratetab[tempSettings1.frame_rate_code - 1];
  ChangeFrameRate();
}

void TOptions2Dlg::EvBitRateKillFocus()
{
  unsigned int i;
  char tmpStr[30];

  videoBitRate->GetText(tmpStr, 30);
  sscanf(tmpStr, "%ld", &i);
  if (tempSettings1.constant_bitrate)
    tempSettings1.bit_rate = (double)i;
  else
    tempSettings1.max_bit_rate = (double)i;
  ChangeBitRate();
}

void TOptions2Dlg::EvAvgBitRateKillFocus()
{
  unsigned int i;
  char tmpStr[30];

  avgBitRate->GetText(tmpStr, 30);
  sscanf(tmpStr, "%ld", &i);
  tempSettings1.avg_bit_rate = (double)i;
  ChangeBitRate();
}

void TOptions2Dlg::EvMinBitRateKillFocus()
{
  unsigned int i;
  char tmpStr[30];

  minBitRate->GetText(tmpStr, 30);
  sscanf(tmpStr, "%ld", &i);
  tempSettings1.min_bit_rate = (double)i;
  ChangeBitRate();
}


bool TOptions2Dlg::EvMQuantBump(TNmUpDown& not)
{
  int i, j, k;

  mQuantScroll->GetRange(i, j);
  k = not.iPos + not.iDelta;
  if ((k >= i) && (k <= j))
    tempSettings1.mquant_value = k;
  return false;
}

void TOptions2Dlg::EvMPEGN()
{
  char tmpStr[30];

  MPEGN->GetString(tmpStr, MPEGN->GetSelIndex());
  sscanf(tmpStr, "%d", &tempSettings1.N);
}

void TOptions2Dlg::EvMPEGM()
{
  tempSettings1.M = MPEGM->GetSelIndex() + 1;
  ChangeMPEGM();
}

bool TOptions2Dlg::EvTCHourBump(TNmUpDown& not)
{
  int i, j, k;

  TCHourScroll->GetRange(i, j);
  k = not.iPos + not.iDelta;
  if ((k >= i) && (k <= j))
  {
    tempSettings1.hours = k;
    ChangeStartTime();
  }
  return false;
}

bool TOptions2Dlg::EvTCMinBump(TNmUpDown& not)
{
  int i, j, k;

  TCMinScroll->GetRange(i, j);
  k = not.iPos + not.iDelta;
  if ((k >= i) && (k <= j))
  {
    tempSettings1.mins = k;
    ChangeStartTime();
  }
  return false;
}

bool TOptions2Dlg::EvTCSecBump(TNmUpDown& not)
{
  int i, j, k;

  TCSecScroll->GetRange(i, j);
  k = not.iPos + not.iDelta;
  if ((k >= i) && (k <= j))
  {
    tempSettings1.sec = k;
    ChangeStartTime();
  }
  return false;
}

bool TOptions2Dlg::EvTCFrameBump(TNmUpDown& not)
{
  int i, j, k;

  TCFrameScroll->GetRange(i, j);
  k = not.iPos + not.iDelta;
  if ((k >= i) && (k <= j))
  {
    tempSettings1.tframe = k;
    ChangeStartTime();
  }
  return false;
}

void TOptions2Dlg::ChangeMPEGType()
{
  profileID->ClearList();
  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    profileID->AddString("Not applicable");
    profileID->SetSelIndex(0);
    profileID->EnableWindow(false);
  }
  else
  {
    profileID->AddString("High Profile");
    profileID->AddString("Main Profile");
    profileID->AddString("Low Profile");
    if (tempSettings1.profile == 1)
      profileID->SetSelIndex(0);
    else
      profileID->SetSelIndex(tempSettings1.profile - 3);
    profileID->EnableWindow(true);

  }
  ChangeProfile();
  aspectRatio->ClearList();
  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    if (tempSettings1.aspectratio > 14)
      tempSettings1.aspectratio = 12;
    aspectRatio->AddString("1.0000 - Square Pels");
    aspectRatio->AddString("0.6735");
    aspectRatio->AddString("0.7031 - 16:9 625 lines");
    aspectRatio->AddString("0.7615");
    aspectRatio->AddString("0.8055");
    aspectRatio->AddString("0.8437 - 16:9 525 lines");
    aspectRatio->AddString("0.8935");
    aspectRatio->AddString("0.9375 - CCIR601 625 lines (PAL)");
    aspectRatio->AddString("0.9815");
    aspectRatio->AddString("1.0255");
    aspectRatio->AddString("1.0695");
    aspectRatio->AddString("1.1250 - CCIR601 525 lines (NTSC)");
    aspectRatio->AddString("1.1575");
    aspectRatio->AddString("1.2015");
  }
  else
  {
    if (tempSettings1.aspectratio > 4)
      tempSettings1.aspectratio = 2;
    aspectRatio->AddString("Square Pels");
    aspectRatio->AddString("4:3 Display");
    aspectRatio->AddString("16:9 Display");
    aspectRatio->AddString("2.21:1 Display");
  }
  aspectRatio->SetSelIndex(tempSettings1.aspectratio - 1);
}

void TOptions2Dlg::ChangeProfile()
{
  int i;
  char tmpStr[10];

  levelID->ClearList();
  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    levelID->AddString("Not applicable");
    levelID->SetSelIndex(0);
    levelID->EnableWindow(false);
  }
  else
  {
    if (tempSettings1.profile < 5)
    {
      levelID->AddString("High Level");
      if (horizontal_size < 1441)
        levelID->AddString("High 1440 Level");
    }
    if ((horizontal_size < 721) && (vertical_size < 577))
      levelID->AddString("Main Level");
    if ((tempSettings1.profile == 4) && (horizontal_size < 353) && (vertical_size < 289))
      levelID->AddString("Low Level");
    levelID->EnableWindow(true);

  }
  chromaFormat->ClearList();
  chromaFormat->AddString("4:2:0");
  chromaFormat->EnableWindow(false);
  MPEGM->ClearList();
  MPEGM->AddString("1");
  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    tempSettings1.chroma_format = 1;
    if (tempSettings1.dc_prec > 0)
      tempSettings1.dc_prec = 0;
    for (i = 2; i <= MAXM; i++)
    {
      sprintf(tmpStr, "%d", i);
      MPEGM->AddString(tmpStr);
    }
    chromaFormat->SetSelIndex(0);
    MPEGM->SetSelIndex(tempSettings1.M - 1);
  }
  else
  {
    switch (tempSettings1.profile)
    {
      case 1:
        if (horizontal_size > 1440)
          tempSettings1.level = 4;
        else
          if (((horizontal_size > 720) || (vertical_size > 576)) && (tempSettings1.level > 6))
            tempSettings1.level = 6;
          else
            if (((horizontal_size > 352) || (vertical_size > 288))&& (tempSettings1.level > 8))
              tempSettings1.level = 8;
        if (tempSettings1.level > 8)
          tempSettings1.level = 6;
        levelID->SetSelIndex((tempSettings1.level - 4) >> 1);
        chromaFormat->AddString("4:2:2");
        chromaFormat->SetSelIndex(tempSettings1.chroma_format - 1);
        chromaFormat->EnableWindow(true);
        for (i = 2; i <= MAXM; i++)
        {
          sprintf(tmpStr, "%d", i);
          MPEGM->AddString(tmpStr);
        }
        MPEGM->SetSelIndex(tempSettings1.M - 1);
        break;
      case 4:
        levelID->SetSelIndex((tempSettings1.level - 4) >> 1);
        tempSettings1.chroma_format = 1;
        chromaFormat->SetSelIndex(0);
        if (tempSettings1.dc_prec > 2)
          tempSettings1.dc_prec = 2;
        for (i = 2; i <= MAXM; i++)
        {
          sprintf(tmpStr, "%d", i);
          MPEGM->AddString(tmpStr);
        }
        MPEGM->SetSelIndex(tempSettings1.M - 1);
        break;
      case 5:
        tempSettings1.level = 8;
        levelID->SetSelIndex(0);
        tempSettings1.chroma_format = 1;
        tempSettings1.M = 1;
        chromaFormat->SetSelIndex(0);
        if (tempSettings1.dc_prec > 2)
          tempSettings1.dc_prec = 2;
        MPEGM->SetSelIndex(0);
    }
  }
  ChangeLevel();
  ChangeMPEGM();
}

void TOptions2Dlg::ChangeLevel()
{
  int i, maxval;

  frameRate->ClearList();
  if (tempSettings1.video_type < MPEG_MPEG2)
  {
    frameRate->AddString("23.976 fps - NTSC encapsulated film rate");
    frameRate->AddString("24 fps - Standard international cinema film rate");
    frameRate->AddString("25 fps - PAL (625/50) video frame rate");
    frameRate->AddString("29.97 fps - NTSC video frame rate");
    frameRate->AddString("30 fps - NTSC drop-frame (525/60) video frame rate");
    frameRate->AddString("50 fps - double frame rate/progressive PAL");
    frameRate->AddString("59.94 fps - double frame rate NTSC");
    frameRate->AddString("60 fps - double frame rate drop-frame NTSC");
    frameRate->SetSelIndex(tempSettings1.frame_rate_code - 1);
    frameRate->EnableWindow(true);
  }
  else
  {
    frameRate->AddString("23.976 fps - NTSC encapsulated film rate");
    frameRate->AddString("24 fps - Standard international cinema film rate");
    frameRate->AddString("25 fps - PAL (625/50) video frame rate");
    i = horizontal_size * vertical_size;
    maxval = ratetab1[(tempSettings1.level - 4) >> 1];
    if ((double) i * ratetab[3] <= (double) maxval)
      frameRate->AddString("29.97 fps - NTSC video frame rate");
    else
      if (tempSettings1.frame_rate_code > 3)
        tempSettings1.frame_rate_code = 3;
    if (i * 30 <= maxval)
      frameRate->AddString("30 fps - NTSC drop-frame (525/60) video frame rate");
    else
      if (tempSettings1.frame_rate_code > 4)
        tempSettings1.frame_rate_code = 4;
    if (tempSettings1.level < 8)
    {
      if (i * 50 <= maxval)
        frameRate->AddString("50 fps - double frame rate/progressive PAL");
      else
        if (tempSettings1.frame_rate_code > 5)
          tempSettings1.frame_rate_code = 5;
      if ((double) i * ratetab[6] <= (double) maxval)
        frameRate->AddString("59.94 fps - double frame rate NTSC");
      else
        if (tempSettings1.frame_rate_code > 6)
          tempSettings1.frame_rate_code = 6;
      if (i * 60 <= maxval)
        frameRate->AddString("60 fps - double frame rate drop-frame NTSC");
      else
        if (tempSettings1.frame_rate_code > 7)
          tempSettings1.frame_rate_code = 7;
    }
    else
      if (tempSettings1.frame_rate_code > 5)
        tempSettings1.frame_rate_code = 4;
    frameRate->SetSelIndex(tempSettings1.frame_rate_code - 1);
  }
  tempSettings1.frame_rate = ratetab[tempSettings1.frame_rate_code - 1];
  ChangeFrameRate();
}

void TOptions2Dlg::ChangeFrameRate()
{
  int i;

  i = (int)(tempSettings1.frame_rate - 1.0);
  TCFrameScroll->SetRange(0, i);
  if (tempSettings1.tframe > i)
    tempSettings1.tframe = i;
  TCFrameScroll->SetPos(tempSettings1.tframe);

  if (tempSettings1.video_type > MPEG_VCD)
  {
    videoPulldown->SetSelIndex(tempSettings1.video_pulldown_flag);
    if ((tempSettings1.frame_rate_code == 1) ||
        (tempSettings1.frame_rate_code == 2))
    {
      tempSettings1.M = 3;
      tempSettings1.N = 12;
      videoPulldown->EnableWindow(true);
      ChangeMPEGM();
    }
    else
      videoPulldown->EnableWindow(false);
  }
  else
  {
    tempSettings1.video_pulldown_flag = PULLDOWN_NONE;
    videoPulldown->SetSelIndex(0);
    videoPulldown->EnableWindow(false);
  }
  
  ChangeVBRBitrate();
  ChangeStartTime();
}

void TOptions2Dlg::ChangeVBRBitrate()
{
  if (tempSettings1.constant_bitrate)
  {
    autoBitrate->EnableWindow(true);
    bitrateTxt->SetText("CBR:");
    mQuant->EnableWindow(false);
    mQuantScroll->EnableWindow(false);
  }
  else
  {
    autoBitrate->EnableWindow(false);
    bitrateTxt->SetText("Max:");
    mQuant->EnableWindow(true);
    mQuantScroll->EnableWindow(true);
  }
  ChangeMQuant();
  ChangeAutoBitrate();
}

void TOptions2Dlg::ChangeMQuant()
{
  char tmpStr[30];

  if (tempSettings1.constant_bitrate)
  {
    mQuant->SetText("0");
    vbrBitrate->Uncheck();
  }
  else
  {
    vbrBitrate->Check();
    mQuantScroll->SetPos(tempSettings1.mquant_value);
    sprintf(tmpStr, "%d", tempSettings1.mquant_value);
    mQuant->SetText(tmpStr);
  }
}

void TOptions2Dlg::ChangeAutoBitrate()
{
  if (tempSettings1.auto_bitrate && tempSettings1.constant_bitrate)
  {
    autoBitrate->Check();
    AutoSetBitrateData(&tempSettings1);
  }
  else
    autoBitrate->Uncheck();
  ChangeBitRate();
}

void TOptions2Dlg::ChangeBitRate()
{
  double maxval;
  char tmpStr[30];

  if (tempSettings1.auto_bitrate && tempSettings1.constant_bitrate)
    videoBitRate->EnableWindow(false);
  else
  {
    videoBitRate->EnableWindow(true);
    switch (tempSettings1.video_type)
    {
      case MPEG_MPEG1:
        maxval = 104857200.0;
        break;
      case MPEG_VCD:
        maxval = 1151929.0;
        break;
      case MPEG_SVCD:
        maxval = 2600000.0; //svcdtab[tempSettings1.audio_bitrate];
        break;
      case MPEG_DVD:
        maxval = 9800000.0;
        break;
      default:
        maxval = bitratetab[(tempSettings1.level - 4) >> 1];
    }
    if (tempSettings1.constant_bitrate)
    {
      avgBitRate->EnableWindow(false);
      minBitRate->EnableWindow(false);
      if (tempSettings1.bit_rate <= 0)
      {
        MessageBox("Video bit rate must be greater then 0, setting to maximum.", AppName, MB_OK);
        tempSettings1.bit_rate = maxval;
      }
      if (tempSettings1.bit_rate > maxval)
      {
        MessageBox("Video bit rate greater than maximum, setting to maximum.", AppName, MB_OK);
        tempSettings1.bit_rate = maxval;
      }
    }
    else
    {
      avgBitRate->EnableWindow(true);
      minBitRate->EnableWindow(true);
      if (tempSettings1.max_bit_rate > maxval)
      {
        MessageBox("Max video bit rate greater than allowed maximum, setting to maximum.", AppName, MB_OK);
        tempSettings1.max_bit_rate = maxval;
      }
      if (tempSettings1.avg_bit_rate > tempSettings1.max_bit_rate)
      {
        MessageBox("Avg video bit rate greater than specified maximum, setting to maximum.", AppName, MB_OK);
        tempSettings1.avg_bit_rate = tempSettings1.max_bit_rate;
      }
      if ((tempSettings1.avg_bit_rate > 0) && (tempSettings1.min_bit_rate > tempSettings1.avg_bit_rate))
      {
        MessageBox("Min video bit rate greater than specified average, setting to average.", AppName, MB_OK);
        tempSettings1.min_bit_rate = tempSettings1.avg_bit_rate;
      }
      if (tempSettings1.min_bit_rate > tempSettings1.max_bit_rate)
      {
        MessageBox("Min video bit rate greater than specified maximum, setting to maximum.", AppName, MB_OK);
        tempSettings1.min_bit_rate = tempSettings1.max_bit_rate;
      }
    }
  }
  if (tempSettings1.constant_bitrate)
  {
    avgBitRate->SetText("0");
    minBitRate->SetText("0");
    sprintf(tmpStr, "%ld", (int)floor(tempSettings1.bit_rate));
  }
  else
  {
    sprintf(tmpStr, "%ld", (int)floor(tempSettings1.avg_bit_rate));
    avgBitRate->SetText(tmpStr);
    sprintf(tmpStr, "%ld", (int)floor(tempSettings1.min_bit_rate));
    minBitRate->SetText(tmpStr);
    sprintf(tmpStr, "%ld", (int)floor(tempSettings1.max_bit_rate));
  }
  videoBitRate->SetText(tmpStr);
}

void TOptions2Dlg::ChangeMPEGM()
{
  int i;
  char tmpStr[5];

  MPEGN->ClearList();
  for (i = tempSettings1.M; i <= MAXN; i++)
  {
    if (i % tempSettings1.M == 0)
    {
      sprintf(tmpStr, "%d", i);
      MPEGN->AddString(tmpStr);
    }
  }
  if (tempSettings1.N % tempSettings1.M != 0)
    tempSettings1.N = Ndefaults[tempSettings1.M - 1];
  MPEGN->SetSelIndex(tempSettings1.N / tempSettings1.M - 1);
  AutoSetMotionData(&tempSettings1);
}

void TOptions2Dlg::ChangeStartTime()
{
  tempSettings1.tc0 = tempSettings1.hours;
  tempSettings1.tc0 = 60 * tempSettings1.tc0 + tempSettings1.mins;
  tempSettings1.tc0 = 60 * tempSettings1.tc0 + tempSettings1.sec;
  tempSettings1.tc0 =
    (int)(tempSettings1.frame_rate + 0.5) * tempSettings1.tc0 + tempSettings1.tframe;
}

void TOptions2Dlg::Help(TNotify&)
{
  WinHelp(HelpFileName, HELP_CONTEXT, 4);
}

bool TOptions2Dlg::CanClose()
{
  return TPropertyPage::CanClose();
}

TOptions1Dlg::TOptions1Dlg(TPropertySheet* parent)
            :TPropertyPage(parent, IDD_OPTIONS1)
{
  PageInfo.dwFlags |= PSP_HASHELP;
  progName = new TEdit(this, IDC_PROGRAMNAME);
  saveVideo = new TCheckBox(this, IDC_SAVEVIDEO);
  videoName = new TEdit(this, IDC_VIDEONAME);
  saveAudio = new TCheckBox(this, IDC_SAVEAUDIO);
  audioName = new TEdit(this, IDC_AUDIONAME);
  audio1Name = new TEdit(this, IDC_AUDIONAME1);
  iqName = new TEdit(this, IDC_IQMATRIXNAME);
  niqName = new TEdit(this, IDC_NIQMATRIXNAME);
  statsName = new TEdit(this, IDC_STATSNAME);
}

DEFINE_RESPONSE_TABLE1(TOptions1Dlg, TPropertyPage)
  EV_WM_SHOWWINDOW,
  EV_COMMAND(IDC_OPENPS, EvOpenPS),
  EV_COMMAND(IDC_OPENVS, EvOpenVS),
  EV_COMMAND(IDC_OPENAS, EvOpenAS),
  EV_COMMAND(IDC_OPENAS1, EvOpenAS1),
  EV_COMMAND(IDC_OPENIQMATRIX, EvOpenIQMatrix),
  EV_COMMAND(IDC_OPENNIQMATRIX, EvOpenNIQMatrix),
  EV_COMMAND(IDC_OPENSTATS, EvOpenStats),
  EV_CHILD_NOTIFY(IDC_SAVEVIDEO, BN_CLICKED, EvSaveVideo),
  EV_CHILD_NOTIFY(IDC_SAVEAUDIO, BN_CLICKED, EvSaveAudio),
END_RESPONSE_TABLE;

void TOptions1Dlg::SetupWindow()
{
  TPropertyPage::SetupWindow();
  progName->SetText(ProgramFilename);
  videoName->SetText(VideoFilename);
  audioName->SetText(AudioFilename);
  audio1Name->SetText(Audio1Filename);
  ChangeSettings();
}

void TOptions1Dlg::EvShowWindow(bool show, uint status)
{
  TPropertyPage::EvShowWindow(show, status);
  if (show)
    ChangeSettings();
}

void TOptions1Dlg::ChangeSettings()
{
  if (tempSettings1.EncodeVideo)
  {
    if (!tempSettings1.MplexVideo || tempSettings1.SaveTempVideo)
      saveVideo->Check();
    else
      saveVideo->Uncheck();
    saveVideo->EnableWindow(true);
  }
  else
  {
    saveVideo->Check();
    saveVideo->EnableWindow(false);
  }
  if (tempSettings1.EncodeAudio)
  {
    if (!tempSettings1.MplexAudio || tempSettings1.SaveTempAudio)
      saveAudio->Check();
    else
      saveAudio->Uncheck();
    saveAudio->EnableWindow(true);
  }
  else
  {
    saveAudio->Check();
    saveAudio->EnableWindow(false);
  }
  videoName->SetText(VideoFilename);
  iqName->SetText(tempSettings1.iqname);
  niqName->SetText(tempSettings1.niqname);
  statsName->SetText(tempSettings1.statname);
}

void TOptions1Dlg::EvSaveVideo()
{
  tempSettings1.SaveTempVideo = (saveVideo->GetCheck() == BF_CHECKED);
}

void TOptions1Dlg::EvSaveAudio()
{
  tempSettings1.SaveTempAudio = (saveAudio->GetCheck() == BF_CHECKED);
}

void TOptions1Dlg::EvOpenPS()
{
  char tmpStr[MAXPATH];

  static TOpenSaveDialog::TData data (
         OFN_PATHMUSTEXIST|OFN_OVERWRITEPROMPT|OFN_HIDEREADONLY|OFN_NOREADONLYRETURN,
         "MPEG Program Streams (*.MPG,*.M2P,*.VOB)|*.mpg;*.m2p;*.vob|MPEG Program Streams (*.MPG)|*.mpg|MPEG-2 Program Streams (*.M2P)|*.m2p|Video Object Units (*.VOB)|*.vob|All Files (*.*)|*.*|",
         0, "", "");

  sprintf(tmpStr, "%s - output program stream file", AppName);
  if ((new TFileSaveDialog(this, data, 0, tmpStr))->Execute() == IDOK)
  {
    strcpy(ProgramFilename, data.FileName);
    strlwr(ProgramFilename);
    progName->SetText(ProgramFilename);
  }
}

void TOptions1Dlg::EvOpenVS()
{
  char tmpStr[MAXPATH];

  static TOpenSaveDialog::TData saveData (
         OFN_PATHMUSTEXIST|OFN_OVERWRITEPROMPT|OFN_HIDEREADONLY|OFN_NOREADONLYRETURN,
         "MPEG Video Streams (*.M2V,*.M1V,*.MPV)|*.m2v;*.m1v;*.mpv|MPEG-2 Video Streams (*.M2V)|*.m2v|MPEG-1 Video Streams (*.M1V)|*.m1v|MPEG Video Streams (*.MPV)|*.mpv|All Files (*.*)|*.*|",
         0, "", "");
  static TOpenSaveDialog::TData openData (
         OFN_PATHMUSTEXIST|OFN_FILEMUSTEXIST,
         "MPEG Video Streams (*.M2V,*.M1V,*.MPV)|*.m2v;*.m1v;*.mpv|MPEG-2 Video Streams (*.M2V)|*.m2v|MPEG-1 Video Streams (*.M1V)|*.m1v|MPEG Video Streams (*.MPV)|*.mpv|All Files (*.*)|*.*|",
         0, "", "");

  if (!tempSettings1.EncodeVideo)
  {
    sprintf(tmpStr, "%s - input video stream file", AppName);
    if ((new TFileOpenDialog(this, openData, 0, tmpStr))->Execute() == IDOK)
    {
      strcpy(VideoFilename, openData.FileName);
      strlwr(VideoFilename);
      videoName->SetText(VideoFilename);
    }
  }
  else
  {
    sprintf(tmpStr, "%s - output video stream file", AppName);
    if ((new TFileSaveDialog(this, saveData, 0, tmpStr))->Execute() == IDOK)
    {
      strcpy(VideoFilename, saveData.FileName);
      strlwr(VideoFilename);
      videoName->SetText(VideoFilename);
    }
  }
}

void TOptions1Dlg::EvOpenAS()
{
  char tmpStr[MAXPATH];

  static TOpenSaveDialog::TData saveData (
         OFN_PATHMUSTEXIST|OFN_OVERWRITEPROMPT|OFN_HIDEREADONLY|OFN_NOREADONLYRETURN,
         "MPEG Audio Streams (*.MP2,*.MP1, *.MPA)|*.mp2;*.mp1;*.mpa|MPEG Layer 2 Audio Streams (*.MP2)|*.mp2|MPEG Layer 1 Audio Streams (*.MP1)|*.mp1|MPEG Audio Streams (*.MPA)|*.mpa|All Files (*.*)|*.*|",
         0, "", "");
  static TOpenSaveDialog::TData openData (
         OFN_PATHMUSTEXIST|OFN_FILEMUSTEXIST,
         "Supported Audio Streams (*.MP2,*.MP1,*.MPA,*.AC3)|*.mp2;*.mp1;*.mpa;*.ac3|MPEG Layer 2 Audio Streams (*.MP2)|*.mp2|MPEG Layer 1 Audio Streams (*.MP1)|*.mp1|MPEG Audio Streams (*.MPA)|*.mpa|AC3 Audio Streams (*.AC3)|*.ac3|All Files (*.*)|*.*|",
         0, "", "");

  if (!tempSettings1.EncodeAudio)
  {
    sprintf(tmpStr, "%s - input audio stream file", AppName);
    if ((new TFileOpenDialog(this, openData, 0, tmpStr))->Execute() == IDOK)
    {
      strcpy(AudioFilename, openData.FileName);
      strlwr(AudioFilename);
      audioName->SetText(AudioFilename);
    }
  }
  else
  {
    sprintf(tmpStr, "%s - output audio stream file", AppName);
    if ((new TFileSaveDialog(this, saveData, 0, tmpStr))->Execute() == IDOK)
    {
      strcpy(AudioFilename, saveData.FileName);
      strlwr(AudioFilename);
      audioName->SetText(AudioFilename);
    }
  }
}

void TOptions1Dlg::EvOpenAS1()
{
  char tmpStr[MAXPATH];

  static TOpenSaveDialog::TData saveData (
         OFN_PATHMUSTEXIST|OFN_OVERWRITEPROMPT|OFN_HIDEREADONLY|OFN_NOREADONLYRETURN,
         "MPEG Audio Streams (*.MP2,*.MP1, *.MPA)|*.mp2;*.mp1;*.mpa|MPEG Layer 2 Audio Streams (*.MP2)|*.mp2|MPEG Layer 1 Audio Streams (*.MP1)|*.mp1|MPEG Audio Streams (*.MPA)|*.mpa|All Files (*.*)|*.*|",
         0, "", "");
  static TOpenSaveDialog::TData openData (
         OFN_PATHMUSTEXIST|OFN_FILEMUSTEXIST,
         "Supported Audio Streams (*.MP2,*.MP1,*.MPA,*.AC3)|*.mp2;*.mp1;*.mpa;*.ac3|MPEG Layer 2 Audio Streams (*.MP2)|*.mp2|MPEG Layer 1 Audio Streams (*.MP1)|*.mp1|MPEG Audio Streams (*.MPA)|*.mpa|AC3 Audio Streams (*.AC3)|*.ac3|All Files (*.*)|*.*|",
         0, "", "");

  sprintf(tmpStr, "%s - input audio stream file", AppName);
  if ((new TFileOpenDialog(this, openData, 0, tmpStr))->Execute() == IDOK)
  {
    strcpy(Audio1Filename, openData.FileName);
    strlwr(Audio1Filename);
    audio1Name->SetText(Audio1Filename);
  }
}

void TOptions1Dlg::EvOpenIQMatrix()
{
  char tmpStr[MAXPATH];

  static TOpenSaveDialog::TData data (
         OFN_PATHMUSTEXIST|OFN_FILEMUSTEXIST,
         "Matrix Files (*.MAT)|*.mat|All Files (*.*)|*.*|",
         0, "", "MAT");

  sprintf(tmpStr, "%s - open intra quant matrix file", AppName);
  if ((new TFileOpenDialog(this, data, 0, tmpStr))->Execute() == IDOK)
  {
    strcpy(tempSettings1.iqname, data.FileName);
    strlwr(tempSettings1.iqname);
    iqName->SetText(tempSettings1.iqname);
  }
}

void TOptions1Dlg::EvOpenNIQMatrix()
{
  char tmpStr[MAXPATH];

  static TOpenSaveDialog::TData data (
         OFN_PATHMUSTEXIST|OFN_FILEMUSTEXIST,
         "Matrix Files (*.MAT)|*.mat|All Files (*.*)|*.*|",
         0, "", "MAT");

  sprintf(tmpStr, "%s - open non-intra quant matrix file", AppName);
  if ((new TFileOpenDialog(this, data, 0, tmpStr))->Execute() == IDOK)
  {
    strcpy(tempSettings1.niqname, data.FileName);
    strlwr(tempSettings1.niqname);
    niqName->SetText(tempSettings1.niqname);
  }
}

void TOptions1Dlg::EvOpenStats()
{
  char tmpStr[MAXPATH];

  static TOpenSaveDialog::TData data (
         OFN_PATHMUSTEXIST|OFN_OVERWRITEPROMPT|OFN_HIDEREADONLY|OFN_NOREADONLYRETURN,
         "Stats Files (*.STS)|*.sts|All Files (*.*)|*.*|",
         0, "", "STS");

  sprintf(tmpStr, "%s - output statistics file", AppName);
  if ((new TFileSaveDialog(this, data, 0, tmpStr))->Execute() == IDOK)
  {
    strcpy(tempSettings1.statname, data.FileName);
    strlwr(tempSettings1.statname);
    statsName->SetText(tempSettings1.statname);
  }
}

void TOptions1Dlg::Help(TNotify&)
{
  WinHelp(HelpFileName, HELP_CONTEXT, 2);
}


TOptPropSheet::TOptPropSheet(TWindow* parent, const char far* title,
                  uint startPage, bool isWizard,
                  uint32 flags, TModule* module) :
               TPropertySheet(parent, title, startPage, isWizard, flags, module)
{
  Attr.Style &= (~DS_CONTEXTHELP);
  Attr.ExStyle &= (~WS_EX_CONTEXTHELP);
}

*/

int DoSettings(HWND parent, int initPage)
{
	/*
  char tmpProg[MAXPATH], tmpAud[MAXPATH], tmpVid[MAXPATH];
  int i, j, k, startPage;
  TWindow *parentWindow;

  if (parent)
    parentWindow = new TWindow(parent);
  else
    parentWindow = new TWindow(NULL, "", module);
  if (!parentWindow)
  {
    DisplayError("Construct parent window failed");
    return settingsError;
  }

  startPage = 0; // display the general settings page by default
  switch (initPage)
  {
    case 1:   // display the input and output files page
      startPage = 1;
      break;
    case 2:   // display the video settings page
      if (VideoAvail)
        startPage = 2;
      break;
    case 3:   // display the audio settings page
      if (AudioAvail)
        if (VideoAvail)
          startPage = 4;
        else
          startPage = 2;
      break;
  }

  TOptPropSheet* ps = new TOptPropSheet(parentWindow,
                       "MPEG Output Settings", startPage, false,
                       PSH_DEFAULT | PSH_HASHELP | PSH_NOAPPLYNOW, module);

  if (!ps)
  {
    DisplayError("Create property sheet failed");
    delete parentWindow;
    return settingsError;
  }
  options0 = new TOptions0Dlg(ps);
  if (!options0)
  {
    DisplayError("Create settings page 0 failed");
    ps->Destroy();
    delete parentWindow;
    return settingsError;
  }
  options1 = new TOptions1Dlg(ps);
  if (!options1)
  {
    DisplayError("Create settings page 1 failed");
    ps->Destroy();
    delete parentWindow;
    return settingsError;
  }
  if (VideoAvail)
  {
    options2 = new TOptions2Dlg(ps);
    if (!options2)
    {
      DisplayError("Create settings page 2 failed");
      ps->Destroy();
      delete parentWindow;
      return settingsError;
    }
    options3 = new TOptions3Dlg(ps);
    if (!options3)
    {
      DisplayError("Create settings page 3 failed");
      ps->Destroy();
      delete parentWindow;
      return settingsError;
    }
  }
  else
  {
    options2 = NULL;
    options3 = NULL;
  }

  if (AudioAvail)
  {
    options4 = new TOptions4Dlg(ps);
    if (!options4)
    {
      DisplayError("Create settings page 4 failed");
      ps->Destroy();
      delete parentWindow;
      return settingsError;
    }
  }
  else
    options4 = NULL;

  options5 = new TOptions5Dlg(ps);
  if (!options5)
  {
    DisplayError("Create settings page 5 failed");
    ps->Destroy();
    delete parentWindow;
    return settingsError;
  }
  options6 = new TOptions6Dlg(ps);
  if (!options6)
  {
    DisplayError("Create settings page 6 failed");
    ps->Destroy();
    delete parentWindow;
    return settingsError;
  }

  PutTempSettings(&tempSettings1);
  strcpy(tmpProg, ProgramFilename);
  strcpy(tmpVid, VideoFilename);
  strcpy(tmpAud, AudioFilename);
  j = PALDefaults;
  k = MMXMode;
  i = ps->Execute();
  if (i == IDOK)
    GetTempSettings(&tempSettings1);
  else
  {
    strcpy(ProgramFilename, tmpProg);
    strcpy(VideoFilename, tmpVid);
    strcpy(AudioFilename, tmpAud);
    PALDefaults = j;
    MMXMode = k;
    if (i == -1)
      DisplayError("Unable to create settings dialog");
  }
  sprintf(tmpProg, "%d", PALDefaults);
  WriteProfileString(AppName, "PALDefaults", tmpProg);
  sprintf(tmpProg, "%d", MMXMode);
  WriteProfileString(AppName, "MMXMode", tmpProg);
  ps->Destroy();
  delete parentWindow;
  if (i == IDOK)
    return settingsErrNone;
  else
    if (i == -1)
      return settingsError;
	  */
  return settingsCancel;
}


int InitSettings(HINSTANCE instance)
{
	/*
  // We are using the DLL and want to use the DLL's resources

#ifdef OWLNEXT
  // This ensures that the application dictionary holds an application
  // (workaround for OWLNext, does no harm for standard OWL
  // We'll #ifdef it just the same

  TApplication *Tapp;
  Tapp = GetApplicationObject();
  if (!Tapp)
    return FALSE;
#endif
	*/

//  module = new TModule(0, instance);
 // if (!module)
  //  return FALSE;
  return TRUE;
  
}


void PutTempSettings(mpegOutSettings *set)
{
  int i;

  set->useFP = UseFP;
  set->verbose = verbose;
  set->MplexVideo = MplexVideo;
  set->MplexAudio = MplexAudio;
  set->UserEncodeVideo = UserEncodeVideo;
  set->UserEncodeAudio = UserEncodeAudio;
  set->EncodeVideo = EncodeVideo;
  set->EncodeAudio = EncodeAudio;
  set->SaveTempVideo = SaveTempVideo;
  set->SaveTempAudio = SaveTempAudio;
  set->B_W = B_W;

  strcpy(set->id_string, id_string);
  strcpy(set->iqname, iqname);
  strcpy(set->niqname, niqname);
  strcpy(set->statname, statname);
  set->video_type = video_type;
  set->video_pulldown_flag = video_pulldown_flag;
  set->constrparms = constrparms;
  set->N = N; set->M = M;
  set->fieldpic = fieldpic;
  set->aspectratio = aspectratio;
  set->frame_rate_code = frame_rate_code;
  set->frame_rate = frame_rate;
  set->tc0 = tc0;
  set->hours = hours;
  set->mins = mins;
  set->sec = sec;
  set->tframe = tframe;
  set->bit_rate = bit_rate;
  set->max_bit_rate = max_bit_rate;
  set->avg_bit_rate = avg_bit_rate;
  set->min_bit_rate = min_bit_rate;
  set->auto_bitrate = auto_bitrate;
  set->vbv_buffer_size = vbv_buffer_size;
  set->fixed_vbv_delay = fixed_vbv_delay;
  set->constant_bitrate = constant_bitrate;
  set->mquant_value = mquant_value;
  set->low_delay = low_delay;
  set->profile = profile;
  set->level = level;
  set->prog_seq = prog_seq;
  set->chroma_format = chroma_format;
  set->write_sde = write_sde;
  set->write_sec = write_sec;
  set->video_format = video_format;
  set->color_primaries = color_primaries;
  set->transfer_characteristics = transfer_characteristics;
  set->matrix_coefficients = matrix_coefficients;
  set->display_horizontal_size = display_horizontal_size;
  set->display_vertical_size = display_vertical_size;
  set->dc_prec = dc_prec;
  set->topfirst = topfirst;
  set->embed_SVCD_user_blocks = embed_SVCD_user_blocks;
  set->write_pde = write_pde;
  set->frame_centre_horizontal_offset = frame_centre_horizontal_offset;
  set->frame_centre_vertical_offset = frame_centre_vertical_offset;
  set->slice_hdr_every_MBrow = slice_hdr_every_MBrow;

  for (i = 0; i < 3; i++)
  {
    set->frame_pred_dct_tab[i] = frame_pred_dct_tab[i];
    set->conceal_tab[i] = conceal_tab[i];
    set->qscale_tab[i] = qscale_tab[i];
    set->intravlc_tab[i] = intravlc_tab[i];
    set->altscan_tab[i] = altscan_tab[i];
  }

  set->repeatfirst = repeatfirst;
  set->prog_frame = prog_frame;
  set->P = P;
  set->r = init_r;
  set->avg_act = (int) init_avg_act;
  set->Xi = init_Xi;
  set->Xp = init_Xp;
  set->Xb = init_Xb;
  set->d0i = init_d0i;
  set->d0p = init_d0p;
  set->d0b = init_d0b;
  set->reset_d0pb = reset_d0pb;
  set->min_frame_percent = min_frame_percent;
  set->pad_frame_percent = pad_frame_percent;

  set->xmotion = xmotion;
  set->ymotion = ymotion;
  set->automotion = automotion;
  set->maxmotion = maxmotion;
  for (i = 0; i < MAXM; i++)
  {
    set->motion_data[i].forw_hor_f_code = motion_data[i].forw_hor_f_code;
    set->motion_data[i].forw_vert_f_code = motion_data[i].forw_vert_f_code;
    set->motion_data[i].sxf = motion_data[i].sxf;
    set->motion_data[i].syf = motion_data[i].syf;
    set->motion_data[i].back_hor_f_code = motion_data[i].back_hor_f_code;
    set->motion_data[i].back_vert_f_code = motion_data[i].back_vert_f_code;
    set->motion_data[i].sxb = motion_data[i].sxb;
    set->motion_data[i].syb = motion_data[i].syb;
  }
  // audio stuff 
  set->audio_mode = audio_mode;
  set->audio_layer = audio_layer;
  set->psych_model = psych_model;
  set->audio_bitrate = audio_bitrate;
  set->emphasis = emphasis;
  set->extension = extension;
  set->error_protection = error_protection;
  set->copyright = copyright;
  set->original = original;

  // multiplex stuff 
  set->sectors_delay = sectors_delay;
  set->video_delay_ms = video_delay_ms;
  set->audio_delay_ms = audio_delay_ms;
  set->audio1_delay_ms = audio1_delay_ms;
  set->sector_size = sector_size;
  set->packets_per_pack = packets_per_pack;
  set->audio_buffer_size = init_audio_buffer_size;
  set->audio1_buffer_size = init_audio1_buffer_size;
  set->video_buffer_size = init_video_buffer_size;
  set->always_sys_header = always_sys_header;
  set->use_computed_bitrate = use_computed_bitrate;
  set->mplex_type = mplex_type;
  set->mplex_pulldown_flag = mplex_pulldown_flag;
  set->vcd_audio_pad = vcd_audio_pad;
  set->user_mux_rate = user_mux_rate;
  set->align_sequence_headers = align_sequence_headers;
  set->put_private2 = put_private2;
  set->frame_timestamps = frame_timestamps;
  set->VBR_multiplex = VBR_multiplex;
  set->write_pec = write_pec;
  set->mux_SVCD_scan_offsets = mux_SVCD_scan_offsets;
  set->max_file_size = max_file_size;
  set->mux_start_time = mux_start_time;
  set->mux_stop_time = mux_stop_time;
  set->reset_clocks = reset_clocks;
  set->write_end_codes = write_end_codes;
  set->set_broken_link = set_broken_link;
}

void GetTempSettings(mpegOutSettings *set)
{
  int i;

  UseFP = set->useFP;
  verbose = set->verbose;
  MplexVideo = set->MplexVideo;
  MplexAudio = set->MplexAudio;
  UserEncodeVideo = set->UserEncodeVideo;
#ifdef _DEBUG
  DebugOutbutMessageToFile("UserEncodeVideo = %d\n", UserEncodeVideo);
#endif

  UserEncodeAudio = set->UserEncodeAudio;
  EncodeVideo = set->EncodeVideo;
  EncodeAudio = set->EncodeAudio;
  SaveTempVideo = set->SaveTempVideo;
  SaveTempAudio = set->SaveTempAudio;
  B_W = set->B_W;

  strcpy(id_string, set->id_string);
  strcpy(iqname, set->iqname);
  strcpy(niqname, set->niqname);
  strcpy(statname, set->statname);
  video_type = set->video_type;
  video_pulldown_flag = set->video_pulldown_flag;
  constrparms = set->constrparms;
  N = set->N; M = set->M;
  fieldpic = set->fieldpic;
  aspectratio = set->aspectratio;
  frame_rate_code = set->frame_rate_code;
  frame_rate = set->frame_rate;
  tc0 = set->tc0;
  hours = set->hours;
  mins = set->mins;
  sec = set->sec;
  tframe = set->tframe;
  bit_rate = set->bit_rate;
  max_bit_rate = set->max_bit_rate;
  avg_bit_rate = set->avg_bit_rate;
  min_bit_rate = set->min_bit_rate;
  auto_bitrate = set->auto_bitrate;
  vbv_buffer_size = set->vbv_buffer_size;
  fixed_vbv_delay = set->fixed_vbv_delay;
  constant_bitrate = set->constant_bitrate;
  mquant_value = set->mquant_value;
  low_delay = set->low_delay;
  profile = set->profile;
  level = set->level;
  prog_seq = set->prog_seq;
  chroma_format = set->chroma_format;
  write_sde = set->write_sde;
  write_sec = set->write_sec;
  video_format = set->video_format;
  color_primaries = set->color_primaries;
  transfer_characteristics = set->transfer_characteristics;
  matrix_coefficients = set->matrix_coefficients;
  display_horizontal_size = set->display_horizontal_size;
  display_vertical_size = set->display_vertical_size;
  dc_prec = set->dc_prec;
  topfirst = set->topfirst;
  embed_SVCD_user_blocks = set->embed_SVCD_user_blocks;
  write_pde = set->write_pde;
  frame_centre_horizontal_offset = set->frame_centre_horizontal_offset;
  frame_centre_vertical_offset = set->frame_centre_vertical_offset; 
  slice_hdr_every_MBrow = set->slice_hdr_every_MBrow;

  for (i = 0; i< 3; i++)
  {
    frame_pred_dct_tab[i] = set->frame_pred_dct_tab[i];
    conceal_tab[i] = set->conceal_tab[i];
    qscale_tab[i] = set->qscale_tab[i];
    intravlc_tab[i] = set->intravlc_tab[i];
    altscan_tab[i] = set->altscan_tab[i];
  }

  repeatfirst = set->repeatfirst;
  prog_frame = set->prog_frame;
  P = set->P;
  init_r = set->r;
  init_avg_act = (double) set->avg_act;
  init_Xi = set->Xi;
  init_Xp = set->Xp;
  init_Xb = set->Xb;
  init_d0i = set->d0i;
  init_d0p = set->d0p;
  init_d0b = set->d0b;
  reset_d0pb = set->reset_d0pb;
  min_frame_percent = set->min_frame_percent;
  pad_frame_percent = set->pad_frame_percent;

  xmotion = set->xmotion;
  ymotion = set->ymotion;
  automotion = set->automotion;
  maxmotion = set->maxmotion;
  for (i = 0; i < MAXM; i++)
  {
    motion_data[i].forw_hor_f_code = set->motion_data[i].forw_hor_f_code;
    motion_data[i].forw_vert_f_code = set->motion_data[i].forw_vert_f_code;
    motion_data[i].sxf = set->motion_data[i].sxf;
    motion_data[i].syf = set->motion_data[i].syf;
    motion_data[i].back_hor_f_code = set->motion_data[i].back_hor_f_code;
    motion_data[i].back_vert_f_code = set->motion_data[i].back_vert_f_code;
    motion_data[i].sxb = set->motion_data[i].sxb;
    motion_data[i].syb = set->motion_data[i].syb;
  }
  // audio stuff 
  audio_mode = set->audio_mode;
  audio_layer = set->audio_layer;
  psych_model = set->psych_model;
  audio_bitrate = set->audio_bitrate;
  emphasis = set->emphasis;
  extension = set->extension;
  error_protection = set->error_protection;
  copyright = set->copyright;
  original = set->original;

  // multiplex stuff 
  sectors_delay = set->sectors_delay;
  video_delay_ms = set->video_delay_ms;
  audio_delay_ms = set->audio_delay_ms;
  audio1_delay_ms = set->audio1_delay_ms;
  sector_size = set->sector_size;
  packets_per_pack = set->packets_per_pack;
  init_audio_buffer_size = set->audio_buffer_size;
  init_audio1_buffer_size = set->audio1_buffer_size;
  init_video_buffer_size = set->video_buffer_size;
  always_sys_header = set->always_sys_header;
  use_computed_bitrate = set->use_computed_bitrate;
  mplex_type = set->mplex_type;
  mplex_pulldown_flag = set->mplex_pulldown_flag;
  vcd_audio_pad = set->vcd_audio_pad;
  user_mux_rate = set->user_mux_rate;
  align_sequence_headers = set->align_sequence_headers;
  put_private2 = set->put_private2;
  frame_timestamps = set->frame_timestamps;
  VBR_multiplex = set->VBR_multiplex;
  write_pec = set->write_pec;
  mux_SVCD_scan_offsets = set->mux_SVCD_scan_offsets;
  max_file_size = set->max_file_size;
  mux_start_time = set->mux_start_time;
  mux_stop_time = set->mux_stop_time;
  reset_clocks = set->reset_clocks;
  write_end_codes = set->write_end_codes;
  set_broken_link = set->set_broken_link;
}



/*
void SetMPEG2Defaults(mpegOutSettings *set, int palDefaults)
{
  int i;

  set->useFP = 0;
  set->verbose = 0;
  set->MplexVideo = TRUE;
  set->MplexAudio = TRUE;
  set->UserEncodeVideo = TRUE;
  set->UserEncodeAudio = TRUE;
  set->EncodeVideo = TRUE;
  set->EncodeAudio = TRUE;
  set->SaveTempVideo = FALSE;
  set->SaveTempAudio = FALSE;
  set->write_sde = 1;
  set->write_sec = 1;
  set->B_W = 0;
  if (palDefaults)
  {
    strcpy(set->id_string, "MPEG-2 PAL video and MPEG audio");
    set->frame_rate_code = 3;
    set->display_vertical_size = 576;
    set->color_primaries = 5;
    set->transfer_characteristics = 5;
    set->video_format = 1;
  }
  else
  {
    strcpy(set->id_string, "MPEG-2 NTSC video and MPEG audio");
    set->frame_rate_code = 4;
    set->display_vertical_size = 480;
    set->color_primaries = 4;
    set->transfer_characteristics = 4;
    set->video_format = 2;
  }
  strcpy(set->iqname, "");
  strcpy(set->niqname, "");
  strcpy(set->statname, "");
  set->video_type = MPEG_MPEG2;
  set->video_pulldown_flag = PULLDOWN_NONE;
  set->constrparms = FALSE;
  set->N = 15;
  set->M = 3;
  set->fieldpic = 0;
  set->aspectratio = 2;
  set->frame_rate = ratetab[set->frame_rate_code - 1];
  set->tc0 = 0;
  set->hours = 0;
  set->mins = 0;
  set->sec = 0;
  set->tframe = 0;
  set->bit_rate = 6000000;
  set->max_bit_rate = 0;
  set->avg_bit_rate = 0;
  set->min_bit_rate = 0;
  set->auto_bitrate = 0;
  set->vbv_buffer_size = 112;
  set->fixed_vbv_delay = 1;
  set->constant_bitrate = 0;
  set->mquant_value = 4;
  set->low_delay = 0;
  set->profile = 4;
  set->level = 8;
  set->prog_seq = 0;
  set->chroma_format = 1;
  set->matrix_coefficients = 5;
  set->display_horizontal_size = 720;
  set->dc_prec = 1;
  set->topfirst = 1;
  set->embed_SVCD_user_blocks = 0;
  set->write_pde = 0;
  set->frame_centre_horizontal_offset = 0;
  set->frame_centre_vertical_offset = 0;
  set->slice_hdr_every_MBrow = 1;

  for (i = 0; i < 3; i++)
  {
    set->frame_pred_dct_tab[i] = 1;
    set->conceal_tab[i] = 0;
    set->qscale_tab[i] = 1;
    set->intravlc_tab[i] = 0;
    set->altscan_tab[i] = 1;
  }
  set->intravlc_tab[0] = 1;
  set->repeatfirst = 0;
  set->prog_frame = 0;
  set->P = 0;
  set->r = 0;
  set->avg_act = 0;
  set->Xi = 0;
  set->Xp = 0;
  set->Xb = 0;
  set->d0i = 0;
  set->d0p = 0;
  set->d0b = 0;
  set->reset_d0pb = 1;
  set->min_frame_percent = 25;
  set->pad_frame_percent = 90;

  set->xmotion = 3;
  set->ymotion = 3;
  set->automotion = 1;
  set->maxmotion = 58;
  AutoSetMotionData(set);

  // audio stuff 
  if (audioStereo)
    set->audio_mode = MPG_MD_STEREO;
  else
    set->audio_mode = MPG_MD_MONO;
  set->audio_layer = 2;
  set->psych_model = 2;
  set->audio_bitrate = 11;
  set->emphasis = 0;
  set->extension = 0;
  set->error_protection = 0;
  set->copyright = 0;
  set->original = 0;
  SetMPEG2Mplex(set);
  ChangeVideoFilename(set);
}

void SetMPEG2Mplex(mpegOutSettings *set)
{
  // multiplex stuff 
  set->sectors_delay = 0;
  set->video_delay_ms = 180;
  set->audio_delay_ms = 180;
  set->audio1_delay_ms = 180;
  set->sector_size = 2048;
  set->packets_per_pack = 1;
  set->audio_buffer_size = 4;
  set->audio1_buffer_size = 4;
  set->video_buffer_size = 224;
  set->always_sys_header = FALSE;
  set->mplex_type = MPEG_MPEG2;
  set->mplex_pulldown_flag = PULLDOWN_AUTO;
  set->vcd_audio_pad = FALSE;
  set->user_mux_rate = 0;
  set->align_sequence_headers = 0;
  set->put_private2 = 0;
  set->frame_timestamps = TIMESTAMPS_ALL;
  set->VBR_multiplex = !set->constant_bitrate;
  set->use_computed_bitrate = COMPBITRATE_MAX;
  set->write_pec = 1;
  set->mux_SVCD_scan_offsets = 0;
  set->max_file_size = 0;
  set->mux_start_time = 0;
  set->mux_stop_time = 0;
  set->reset_clocks = 1;
  set->write_end_codes = 1;
  set->set_broken_link = 1;
  AutoSetBitrateData(set);
}

*/


void SetDVDDefaults(mpegOutSettings *set, int palDefaults, int widescreen, bool encode_audio, bool do_3_2_pulldown)
{
  int i;

  set->useFP = 0;
  set->verbose = 0;
  set->MplexVideo = TRUE;
  set->MplexAudio = TRUE;
  set->UserEncodeVideo = TRUE;
  set->UserEncodeAudio = encode_audio;
  set->EncodeVideo = TRUE;
  set->EncodeAudio = encode_audio;
  set->SaveTempVideo = FALSE;
  set->SaveTempAudio = FALSE;
  set->write_sde = 1;
  set->write_sec = 1;
  set->B_W = 0;
  if (palDefaults)
  {

  //  strcpy(set->id_string, "MPEG-2 DVD PAL video and MPEG audio");
    strcpy(set->id_string, "");

    set->N = 30; //12;
    set->frame_rate_code = 3;
    set->display_vertical_size = 576;
    set->color_primaries = 5;
    set->transfer_characteristics = 5;
    set->video_format = 1;
  }
  else
  {
   // strcpy(set->id_string, "MPEG-2 DVD NTSC video and MPEG audio");
     strcpy(set->id_string, "");
    set->N = 15;
	if (do_3_2_pulldown==TRUE)
    {
      set->frame_rate_code = 1;
	}
	else
	{
		set->frame_rate_code = 4;
	}

    set->display_vertical_size = 480;
    set->color_primaries = 4;
    set->transfer_characteristics = 4;
    set->video_format = 2;
  }
  strcpy(set->iqname, "");
  strcpy(set->niqname, "");
  strcpy(set->statname, "");
  set->video_type = MPEG_DVD;
  if (do_3_2_pulldown==TRUE)
  {
    set->video_pulldown_flag = PULLDOWN_32;
  }
  else
  {
    set->video_pulldown_flag=PULLDOWN_NONE;
  }

  set->constrparms = FALSE;
  set->M = 5; //3;
  set->fieldpic = 0;
  if (widescreen==0)
  {
	set->aspectratio = 2;
  }
  else if (widescreen==1)
  {
	  set->aspectratio=3;
  }
  else if (widescreen==2)
  {
	   set->aspectratio=4;
  }


  set->frame_rate = ratetab[set->frame_rate_code - 1];
  set->tc0 = 0;
  set->hours = 0;
  set->mins = 0;
  set->sec = 0;
  set->tframe = 0;
  set->bit_rate = 6000000;
  set->max_bit_rate = 9800000;

  set->avg_bit_rate = 0;
  set->min_bit_rate = 0;
  set->auto_bitrate = 0;
  set->vbv_buffer_size = 112;
  set->fixed_vbv_delay = 1;
  set->constant_bitrate = 0;
  set->mquant_value = 4;
  set->low_delay = 0;
  set->profile = 4;
  set->level = 8;
  set->prog_seq = 0;
  set->chroma_format = 1;
  set->matrix_coefficients = 5;
  set->display_horizontal_size = 720;
  set->dc_prec = 1;
  set->topfirst = 1;
  set->embed_SVCD_user_blocks = 0;
  set->write_pde = 0;
  set->frame_centre_horizontal_offset = 0;
  set->frame_centre_vertical_offset = 0;
  set->slice_hdr_every_MBrow = 1;

  for (i = 0; i < 3; i++)
  {
    set->frame_pred_dct_tab[i] = 1;
    set->conceal_tab[i] = 0;
    set->qscale_tab[i] = 1;
    set->intravlc_tab[i] = 0;
    set->altscan_tab[i] = 1;
  }
  set->intravlc_tab[0] = 1;
  set->repeatfirst = 0;
  set->prog_frame = 0;
  set->P = 0;
  set->r = 0;
  set->avg_act = 0;
  set->Xi = 0;
  set->Xp = 0;
  set->Xb = 0;
  set->d0i = 0;
  set->d0p = 0;
  set->d0b = 0;
  set->reset_d0pb = 1;
  set->min_frame_percent = 0;
  set->pad_frame_percent = 0;

    // normal settings
  /*
  set->xmotion = 3;
  set->ymotion = 3;
  set->automotion = 1;
  set->maxmotion = 58;
  
*/
  
  // seems good
  
  set->xmotion = 5;
  set->ymotion = 5;
  set->automotion = 1;
  set->maxmotion = 75;

  AutoSetMotionData(set);

  // audio stuff 
  if (audioStereo)
    set->audio_mode = MPG_MD_STEREO;
  else
    set->audio_mode = MPG_MD_MONO;
  set->audio_layer = 2;
  set->psych_model = 2;
  set->audio_bitrate = 11;
  set->emphasis = 0;
  set->extension = 0;
  set->error_protection = 1;
  set->copyright = 0;
  set->original = 0;
  SetDVDMplex(set);
  ChangeVideoFilename(set);
}

void SetDVDMplex(mpegOutSettings *set)
{
  // multiplex stuff 
  set->sectors_delay = 0;
  set->video_delay_ms = 180;
  set->audio_delay_ms = 180;
  set->audio1_delay_ms = 180;
  set->sector_size = 2048;
  set->packets_per_pack = 1;
  set->audio_buffer_size = 4;
  set->audio1_buffer_size = 4;
  set->video_buffer_size = 232;
  set->always_sys_header = FALSE;
  set->use_computed_bitrate = COMPBITRATE_MAX;
  set->mplex_type = MPEG_DVD;
  set->mplex_pulldown_flag = PULLDOWN_AUTO;
  set->vcd_audio_pad = FALSE;
  set->user_mux_rate = 25200;
  set->align_sequence_headers = TRUE;
  set->put_private2 = TRUE;
  set->frame_timestamps = TIMESTAMPS_IONLY;
  set->VBR_multiplex = !set->constant_bitrate;
  set->write_pec = 1;
  set->mux_SVCD_scan_offsets = 0;
  set->max_file_size = 0;
  set->mux_start_time = 0;
  set->mux_stop_time = 0;
  set->reset_clocks = 0;
  set->write_end_codes = 0;
  set->set_broken_link = 0;
  AutoSetBitrateData(set);
}


void SetSVCDDefaults(mpegOutSettings *set, int palDefaults, int widescreen, bool encode_audio, bool do_3_2_pulldown)
{
  int i;

  set->useFP = FALSE;
  set->verbose = 0;
  set->MplexVideo = TRUE;
  set->MplexAudio = TRUE;
  set->UserEncodeVideo = TRUE;
  set->UserEncodeAudio = encode_audio;
  set->EncodeVideo = TRUE;
  set->EncodeAudio = encode_audio;
  set->SaveTempVideo = FALSE;
  set->SaveTempAudio = FALSE;
  strcpy(set->iqname, "");
  strcpy(set->niqname, "");
  strcpy(set->statname, "");
  set->video_type = MPEG_SVCD;
  if (do_3_2_pulldown==TRUE)
  {
     set->video_pulldown_flag = PULLDOWN_32;
  }
  else
  {
	  set->video_pulldown_flag = PULLDOWN_NONE;
  }

  set->constrparms = FALSE;
  set->write_sde = 0;
  set->write_sec = 1;
  set->B_W = 0;
  if (palDefaults)
  {
  //  strcpy(set->id_string, "MPEG-2 SuperVCD PAL video and MPEG audio");
	strcpy(set->id_string, "");
    set->frame_rate_code = 3;
    set->video_format = 1;
    set->color_primaries = 5;
    set->transfer_characteristics = 5;
    set->display_vertical_size = 576;
  }
  else
  {
  //  strcpy(set->id_string, "MPEG-2 SuperVCD NTSC video and MPEG audio");
    strcpy(set->id_string, "");
    if (do_3_2_pulldown==TRUE)
	{
  	  set->frame_rate_code = 1;
	}
	else
	{ 
		set->frame_rate_code = 4;
	}

    set->video_format = 2;
    set->color_primaries = 4;
    set->transfer_characteristics = 4;
    set->display_vertical_size = 480;
  }
  set->N = 15;
  set->M = 3;
  if (widescreen==0)
  {
	 set->aspectratio = 2;
  }
  else if (widescreen==1)
  {
     set->aspectratio = 3;
  }
  else if (widescreen==2)
  {
	 set->aspectratio = 4;
  }


  set->fieldpic = 0;
  set->frame_rate = ratetab[set->frame_rate_code - 1];
  set->tc0 = 0;
  set->hours = 0;
  set->mins = 0;
  set->sec = 0;
  set->tframe = 0;
  set->bit_rate = 2300000;
  set->max_bit_rate = 2376000;
  set->avg_bit_rate = 0;
  set->min_bit_rate = 0;
  set->auto_bitrate = 0;
  set->vbv_buffer_size = 112;
  set->fixed_vbv_delay = 1;
  set->constant_bitrate = 0;
  set->mquant_value = 10;
  set->low_delay = 0;
  set->profile = 4;
  set->level = 8;
  set->prog_seq = 0;
  set->chroma_format = 1;
  set->matrix_coefficients = 5;
  set->display_horizontal_size = 720;
  set->dc_prec = 1;
  set->topfirst = 1;
  set->embed_SVCD_user_blocks = 0;
  set->write_pde = 0;
  set->frame_centre_horizontal_offset = 0;
  set->frame_centre_vertical_offset = 0;
  set->slice_hdr_every_MBrow = 1;

  for (i = 0; i < 3; i++)
  {
    set->frame_pred_dct_tab[i] = 1;
    set->conceal_tab[i] = 0;
    set->qscale_tab[i] = 1;
    set->intravlc_tab[i] = 0;
    set->altscan_tab[i] = 1;
  }
  set->intravlc_tab[0] = 1;
  set->repeatfirst = 0;
  set->prog_frame = 0;
  set->P = 0;
  set->r = 0;
  set->avg_act = 0;
  set->Xi = 0;
  set->Xp = 0;
  set->Xb = 0;
  set->d0i = 0;
  set->d0p = 0;
  set->d0b = 0;
  set->reset_d0pb = 1;
  set->min_frame_percent = 25;
  set->pad_frame_percent = 90;

  set->xmotion = 3;
  set->ymotion = 3;
  set->automotion = 1;
  set->maxmotion = 58;
  AutoSetMotionData(set);

  // audio stuff 
  if (audioStereo)
    set->audio_mode = MPG_MD_STEREO;
  else
    set->audio_mode = MPG_MD_MONO;
  set->audio_layer = 2;
  set->psych_model = 2;
  set->audio_bitrate = 11;
  set->emphasis = 0;
  set->extension = 0;
  set->error_protection = 1;
  set->copyright = 0;
  set->original = 0;
  SetSVCDMplex(set);
  ChangeVideoFilename(set);
}

void SetSVCDMplex(mpegOutSettings *set)
{
  // multiplex stuff 
  set->sectors_delay = 0;
  set->video_delay_ms = 180;
  set->audio_delay_ms = 180;
  set->audio1_delay_ms = 180;
  set->sector_size = SVCD_SECTOR_SIZE;
  set->packets_per_pack = 1;
  set->audio_buffer_size = 4;
  set->audio1_buffer_size = 4;
  set->video_buffer_size = 230;
  set->always_sys_header = FALSE;
  set->use_computed_bitrate = COMPBITRATE_MAX;
  set->mplex_type = MPEG_SVCD;
  set->mplex_pulldown_flag = PULLDOWN_AUTO;
  set->vcd_audio_pad = FALSE;
  set->user_mux_rate = 6972;
  set->align_sequence_headers = 1;
  set->put_private2 = 0;
  set->frame_timestamps = TIMESTAMPS_ALL;
  set->VBR_multiplex = TRUE;
  set->write_pec = 1;
  set->mux_SVCD_scan_offsets = 1;
  set->max_file_size = 0;
  set->mux_start_time = 0;
  set->mux_stop_time = 0;
  set->reset_clocks = 1;
  set->write_end_codes = 1;
  set->set_broken_link = 1;
  AutoSetBitrateData(set);
}

/*
void SetMPEG1Defaults(mpegOutSettings *set, int palDefaults)
{
  int i;

  set->useFP = 0;
  set->verbose = 0;
  set->MplexVideo = TRUE;
  set->MplexAudio = TRUE;
  set->UserEncodeVideo = TRUE;
  set->UserEncodeAudio = TRUE;
  set->EncodeVideo = TRUE;
  set->EncodeAudio = TRUE;
  set->SaveTempVideo = FALSE;
  set->SaveTempAudio = FALSE;
  set->write_sde = 1;
  set->write_sec = 1;
  set->B_W = 0;
  if (palDefaults)
  {
    strcpy(set->id_string, "MPEG-1 PAL video and MPEG audio");
    set->aspectratio = 8;
    set->frame_rate_code = 3;
    set->display_vertical_size = 288;
    set->video_format = 1;
    set->color_primaries = 5;
    set->transfer_characteristics = 5;
  }
  else
  {
    strcpy(set->id_string, "MPEG-1 NTSC video and MPEG audio");
    set->aspectratio = 12;
    set->frame_rate_code = 4;
    set->display_vertical_size = 240;
    set->video_format = 2;
    set->color_primaries = 4;
    set->transfer_characteristics = 4;
  }
  strcpy(set->iqname, "");
  strcpy(set->niqname, "");
  strcpy(set->statname, "");
  set->video_type = MPEG_MPEG1;
  set->video_pulldown_flag = PULLDOWN_NONE;
  set->constrparms = FALSE;
  set->N = 15;
  set->M = 3;
  set->fieldpic = 0;
  set->frame_rate = ratetab[set->frame_rate_code - 1];
  set->tc0 = 0;
  set->hours = 0;
  set->mins = 0;
  set->sec = 0;
  set->tframe = 0;
  set->bit_rate = 1800000;
  set->max_bit_rate = 0;
  set->avg_bit_rate = 0;
  set->min_bit_rate = 0;
  set->auto_bitrate = 0;
  set->vbv_buffer_size = 23;
  set->fixed_vbv_delay = 0;
  set->constant_bitrate = 0;
  set->mquant_value = 4;
  set->low_delay = 0;
  set->profile = 4;
  set->level = 8;
  set->prog_seq = 1;
  set->chroma_format = 1;
  set->matrix_coefficients = 5;
  set->display_horizontal_size = 352;
  set->dc_prec = 0;
  set->topfirst = 0;
  set->embed_SVCD_user_blocks = 0;
  set->write_pde = 0;
  set->frame_centre_horizontal_offset = 0;
  set->frame_centre_vertical_offset = 0;
  set->slice_hdr_every_MBrow = 0;

  for (i = 0; i < 3; i++)
  {
    set->frame_pred_dct_tab[i] = 1;
    set->conceal_tab[i] = 0;
    set->qscale_tab[i] = 0;
    set->intravlc_tab[i] = 0;
    set->altscan_tab[i] = 0;
  }
  set->repeatfirst = 0;
  set->prog_frame = 1;
  set->P = 0;
  set->r = 0;
  set->avg_act = 0;
  set->Xi = 0;
  set->Xp = 0;
  set->Xb = 0;
  set->d0i = 0;
  set->d0p = 0;
  set->d0b = 0;
  set->reset_d0pb = 1;
  set->min_frame_percent = 25;
  set->pad_frame_percent = 90;

  set->xmotion = 3;
  set->ymotion = 3;
  set->automotion = 1;
  set->maxmotion = 58;
  AutoSetMotionData(set);

  // audio stuff 
  if (audioStereo)
    set->audio_mode = MPG_MD_STEREO;
  else
    set->audio_mode = MPG_MD_MONO;
  set->audio_layer = 2;
  set->psych_model = 2;
  set->audio_bitrate = 11;
  set->emphasis = 0;
  set->extension = 0;
  set->error_protection = 0;
  set->copyright = 0;
  set->original = 0;
  SetMPEG1Mplex(set);
  ChangeVideoFilename(set);
}

void SetMPEG1Mplex(mpegOutSettings *set)
{
  // multiplex stuff 
  set->sectors_delay = 0;
  set->video_delay_ms = 180;
  set->audio_delay_ms = 180;
  set->audio1_delay_ms = 180;
  set->sector_size = 2048;
  set->packets_per_pack = 1;
  set->audio_buffer_size = 4;
  set->audio1_buffer_size = 4;
  set->video_buffer_size = 46;
  set->always_sys_header = FALSE;
  set->mplex_type = MPEG_MPEG1;
  set->mplex_pulldown_flag = PULLDOWN_NONE;
  set->vcd_audio_pad = FALSE;
  set->user_mux_rate = 0;
  set->align_sequence_headers = 0;
  set->put_private2 = 0;
  set->frame_timestamps = TIMESTAMPS_ALL;
  set->VBR_multiplex = !set->constant_bitrate;
  set->use_computed_bitrate = COMPBITRATE_MAX;
  set->write_pec = 1;
  set->mux_SVCD_scan_offsets = 0;
  set->max_file_size = 0;
  set->mux_start_time = 0;
  set->mux_stop_time = 0;
  set->reset_clocks = 1;
  set->write_end_codes = 1;
  set->set_broken_link = 1;
  AutoSetBitrateData(set);
}

*/

void SetVCDDefaults(mpegOutSettings *set, int palDefaults, int widescreen, bool encode_audio)
{
#ifdef _DEBUG
	DebugOutbutMessageToFile("Setting to vcd defaukts\n");
#endif

  int i;

  set->useFP = 0;
  set->verbose = 0;
  set->MplexVideo = TRUE;
  set->MplexAudio = TRUE;
  set->UserEncodeVideo = TRUE;
  set->UserEncodeAudio = encode_audio;
  set->EncodeVideo = TRUE;
  set->EncodeAudio = encode_audio;
  set->SaveTempVideo = FALSE;
  set->SaveTempAudio = FALSE;
  strcpy(set->iqname, "");
  strcpy(set->niqname, "");
  strcpy(set->statname, "");
  set->video_type = MPEG_VCD;
  set->video_pulldown_flag = PULLDOWN_NONE;
  set->constrparms = TRUE;
  set->write_sde = 1;
  set->write_sec = 1;
  set->B_W = 0;
  if (palDefaults)
  {
  //  strcpy(set->id_string, "MPEG-1 VideoCD PAL video and MPEG audio");
	  strcpy(set->id_string, "");
    set->aspectratio = 8;
    set->frame_rate_code = 3;
    set->video_format = 1;
    set->color_primaries = 5;
    set->transfer_characteristics = 5;
    set->display_vertical_size = 288;
  }
  else
  {
  //  strcpy(set->id_string, "MPEG-1 VideoCD NTSC video and MPEG audio");
	  strcpy(set->id_string, "");
    set->aspectratio = 12;
    set->frame_rate_code = 4;
    set->video_format = 2;
    set->color_primaries = 4;
    set->transfer_characteristics = 4;
    set->display_vertical_size = 240;
  }
  set->N = 15;
  set->M = 3;
  set->fieldpic = 0;
  set->frame_rate = ratetab[set->frame_rate_code - 1];
  set->tc0 = 0;
  set->hours = 0;
  set->mins = 0;
  set->sec = 0;
  set->tframe = 0;
  set->bit_rate = 1150000; // max = 1151929;
  set->max_bit_rate = 1151929;
  set->avg_bit_rate = 0;
  set->min_bit_rate = 0;
  set->auto_bitrate = 0;
  set->vbv_buffer_size = 20;
  set->fixed_vbv_delay = 0;
  set->constant_bitrate = 1;
  set->mquant_value = 4;
  set->low_delay = 0;
  set->profile = 4;
  set->level = 8;
  set->prog_seq = 1;
  set->chroma_format = 1;
  set->matrix_coefficients = 5;
  set->display_horizontal_size = 352;
  set->dc_prec = 0;
  set->topfirst = 0;
  set->embed_SVCD_user_blocks = 0;
  set->write_pde = 0;
  set->frame_centre_horizontal_offset = 0;
  set->frame_centre_vertical_offset = 0;
  set->slice_hdr_every_MBrow = 0;

  for (i = 0; i < 3; i++)
  {
    set->frame_pred_dct_tab[i] = 1;
    set->conceal_tab[i] = 0;
    set->qscale_tab[i] = 0;
    set->intravlc_tab[i] = 0;
    set->altscan_tab[i] = 0;
  }
  set->repeatfirst = 0;
  set->prog_frame = 1;
  set->P = 0;
  set->r = 0;
  set->avg_act = 0;
  set->Xi = 0;
  set->Xp = 0;
  set->Xb = 0;
  set->d0i = 0;
  set->d0p = 0;
  set->d0b = 0;
  set->reset_d0pb = 1;
  set->min_frame_percent = 25;
  set->pad_frame_percent = 90;

  set->xmotion = 3;
  set->ymotion = 3;
  set->automotion = 1;
  set->maxmotion = 90; // 58;
  AutoSetMotionData(set);
 
  // audio stuff 
  if (audioStereo)
    set->audio_mode = MPG_MD_STEREO;
  else
    set->audio_mode = MPG_MD_MONO;
  set->audio_layer = 2;
  set->psych_model = 2;
  set->audio_bitrate = 11;
  set->emphasis = 0;
  set->extension = 0;
  set->error_protection = 0;
  set->copyright = 0;
  set->original = 0;
  SetVCDMplex(set);
  ChangeVideoFilename(set);
}

void SetVCDMplex(mpegOutSettings *set)
{
  // multiplex stuff 
  set->sectors_delay = 400;
  set->video_delay_ms = 344;
  set->audio_delay_ms = 344;
  set->audio1_delay_ms = 344;
  set->sector_size = VIDEOCD_SECTOR_SIZE;
  set->packets_per_pack = 1;
  set->audio_buffer_size = 4;
  set->audio1_buffer_size = 4;
  set->video_buffer_size = 46;
  set->always_sys_header = FALSE;
  set->use_computed_bitrate = COMPBITRATE_NONE;
  set->mplex_type = MPEG_VCD;
  set->mplex_pulldown_flag = PULLDOWN_NONE;
  set->vcd_audio_pad = FALSE;
  set->user_mux_rate = 3528;
  set->align_sequence_headers = 0;
  set->put_private2 = 0;
  set->frame_timestamps = TIMESTAMPS_ALL;
  set->VBR_multiplex = 0;
  set->write_pec = 1;
  set->mux_SVCD_scan_offsets = 0;
  set->max_file_size = 0;
  set->mux_start_time = 0;
  set->mux_stop_time = 0;
  set->reset_clocks = 1;
  set->write_end_codes = 1;
  set->set_broken_link = 1;
  AutoSetBitrateData(set);
}


/*

int ReadSettings(char *filename, mpegOutSettings *set)
{
  int i;
  char tmpStr[40];

  // load/save settings
  ::GetPrivateProfileString("loadsave", "description", set->id_string, set->id_string, MAXPATH, filename);

  // input/output settings
  set->SaveTempVideo = ::GetPrivateProfileInt("iofiles", "saveTempVideo", set->SaveTempVideo, filename);
  set->SaveTempAudio = ::GetPrivateProfileInt("iofiles", "saveTempAudio", set->SaveTempAudio, filename);
  ::GetPrivateProfileString("iofiles", "intraQuantMatrix", set->iqname, set->iqname, MAXPATH, filename);
  ::GetPrivateProfileString("iofiles", "nonIntraQuantMatrix", set->niqname, set->niqname, MAXPATH, filename);
  ::GetPrivateProfileString("iofiles", "statisticsFilename", set->statname, set->statname, MAXPATH, filename);

  // general settings
  set->useFP = ::GetPrivateProfileInt("general", "useFloatingPoint", set->useFP, filename);
  set->verbose = ::GetPrivateProfileInt("general", "verbose", set->verbose, filename);
  set->B_W = ::GetPrivateProfileInt("general", "blackAndWhite", set->B_W, filename);
  set->UserEncodeVideo = ::GetPrivateProfileInt("general", "userEncodeVideo", set->UserEncodeVideo, filename);
  set->UserEncodeAudio = ::GetPrivateProfileInt("general", "userEncodeAudio", set->UserEncodeAudio, filename);
  set->MplexVideo = ::GetPrivateProfileInt("general", "multiplexVideo", set->MplexVideo, filename);
  set->MplexAudio = ::GetPrivateProfileInt("general", "multiplexAudio", set->MplexAudio, filename);
  set->mux_start_time = ::GetPrivateProfileInt("general", "muxStartTime", set->mux_start_time, filename);
  set->mux_stop_time = ::GetPrivateProfileInt("general", "muxStopTime", set->mux_stop_time, filename);
  set->max_file_size = ::GetPrivateProfileInt("general", "maxFileSize", set->max_file_size, filename);
  set->reset_clocks = ::GetPrivateProfileInt("general", "resetClocks", set->reset_clocks, filename);
  set->set_broken_link = ::GetPrivateProfileInt("general", "setBrokenLink", set->set_broken_link, filename);
  set->write_end_codes = ::GetPrivateProfileInt("general", "writeEndCodes", set->write_end_codes, filename);

  // video settings
  set->video_type = ::GetPrivateProfileInt("video", "videoType", set->video_type, filename);
  set->video_pulldown_flag = ::GetPrivateProfileInt("video", "videoPulldownFlag", set->video_pulldown_flag, filename);
  set->profile = ::GetPrivateProfileInt("video", "profile", set->profile, filename);
  set->level = ::GetPrivateProfileInt("video", "level", set->level, filename);
  set->chroma_format = ::GetPrivateProfileInt("video", "chromaFormat", set->chroma_format, filename);
  set->aspectratio = ::GetPrivateProfileInt("video", "aspectRatio", set->aspectratio, filename);
  set->frame_rate_code = ::GetPrivateProfileInt("video", "frameRateCode", set->frame_rate_code, filename);
  set->N = ::GetPrivateProfileInt("video", "iFramesInGOP", set->N, filename);
  set->M = ::GetPrivateProfileInt("video", "ipFrameDistance", set->M, filename);
  set->auto_bitrate = ::GetPrivateProfileInt("video", "autoBitrate", set->auto_bitrate, filename);
  set->constant_bitrate = ::GetPrivateProfileInt("video", "constBitrateFlag", set->constant_bitrate, filename);
  set->mquant_value = ::GetPrivateProfileInt("video", "mquantValue", set->mquant_value, filename);
  set->bit_rate = ::GetPrivateProfileInt("video", "constBitrate", (int)set->bit_rate, filename);
  set->max_bit_rate = ::GetPrivateProfileInt("video", "maxBitrate", (int)set->max_bit_rate, filename);
  set->avg_bit_rate = ::GetPrivateProfileInt("video", "avgBitrate", (int)set->avg_bit_rate, filename);
  set->min_bit_rate = ::GetPrivateProfileInt("video", "minBitrate", (int)set->min_bit_rate, filename);
  set->hours = ::GetPrivateProfileInt("video", "firstFrameHours", set->hours, filename);
  set->mins = ::GetPrivateProfileInt("video", "firstFrameMinutes", set->mins, filename);
  set->sec = ::GetPrivateProfileInt("video", "firstFrameSeconds", set->sec, filename);
  set->tframe = ::GetPrivateProfileInt("video", "firstFrameFrame", set->tframe, filename);

  // audio settings
  set->audio_layer = ::GetPrivateProfileInt("audio", "audioLayer", set->audio_layer, filename);
  set->audio_mode = ::GetPrivateProfileInt("audio", "audioMode", set->audio_mode, filename);
  set->emphasis = ::GetPrivateProfileInt("audio", "deEmphasis", set->emphasis, filename);
  set->audio_bitrate = ::GetPrivateProfileInt("audio", "audioBitrate", set->audio_bitrate, filename);
  set->extension = ::GetPrivateProfileInt("audio", "privateBit", set->extension, filename);
  set->copyright = ::GetPrivateProfileInt("audio", "copyrightBit", set->copyright, filename);
  set->original = ::GetPrivateProfileInt("audio", "originalBit", set->original, filename);
  set->error_protection = ::GetPrivateProfileInt("audio", "errorProtection", set->error_protection, filename);
  set->psych_model = ::GetPrivateProfileInt("audio", "psychModel", set->psych_model, filename);

  // program stream settings
  set->mplex_type = ::GetPrivateProfileInt("program", "programStreamType", set->mplex_type, filename);
  set->VBR_multiplex = ::GetPrivateProfileInt("program", "vbrMultiplex", set->VBR_multiplex, filename);
  set->vcd_audio_pad = ::GetPrivateProfileInt("program", "padVCDAudio", set->vcd_audio_pad, filename);
  set->write_pec = ::GetPrivateProfileInt("program", "writeProgramEndCode", set->write_pec, filename);
  set->mux_SVCD_scan_offsets = ::GetPrivateProfileInt("program", "muxSVCDScanOffsets", set->mux_SVCD_scan_offsets, filename);
  set->align_sequence_headers = ::GetPrivateProfileInt("program", "alignSequenceHeaders", set->align_sequence_headers, filename);
  set->put_private2 = ::GetPrivateProfileInt("program", "usePrivateStream2", set->put_private2, filename);
  set->use_computed_bitrate = ::GetPrivateProfileInt("program", "useComputedBitrate", set->use_computed_bitrate, filename);
  set->frame_timestamps = ::GetPrivateProfileInt("program", "frameTimestamps", set->frame_timestamps, filename);
  set->mplex_pulldown_flag = ::GetPrivateProfileInt("program", "muxPulldownFlag", set->mplex_pulldown_flag, filename);
  set->user_mux_rate = ::GetPrivateProfileInt("program", "userMuxRate", set->user_mux_rate, filename);
  set->sector_size = ::GetPrivateProfileInt("program", "sectorSize", set->sector_size, filename);
  set->packets_per_pack = ::GetPrivateProfileInt("program", "packetsPerPack", set->packets_per_pack, filename);
  set->video_buffer_size = ::GetPrivateProfileInt("program", "videoBufferSize", set->video_buffer_size, filename);
  set->audio_buffer_size = ::GetPrivateProfileInt("program", "audioBufferSize", set->audio_buffer_size, filename);
  set->audio1_buffer_size = ::GetPrivateProfileInt("program", "audio1BufferSize", set->audio1_buffer_size, filename);
  set->sectors_delay = ::GetPrivateProfileInt("program", "sectorDelay", set->sectors_delay, filename);
  set->video_delay_ms = ::GetPrivateProfileInt("program", "videoDelay", set->video_delay_ms, filename);
  set->audio_delay_ms = ::GetPrivateProfileInt("program", "audioDelay", set->audio_delay_ms, filename);
  set->audio1_delay_ms = ::GetPrivateProfileInt("program", "audio1Delay", set->audio1_delay_ms, filename);
  set->always_sys_header = ::GetPrivateProfileInt("program", "alwaysWriteSysHeader", set->always_sys_header, filename);

  // advanced video settings
  set->color_primaries = ::GetPrivateProfileInt("advVideo", "colorPrimaries", set->color_primaries, filename);
  set->transfer_characteristics = ::GetPrivateProfileInt("advVideo", "transferCharacteristics", set->transfer_characteristics, filename);
  set->matrix_coefficients = ::GetPrivateProfileInt("advVideo", "matrixCoefficients", set->matrix_coefficients, filename);
  set->video_format = ::GetPrivateProfileInt("advVideo", "videoFormat", set->video_format, filename);
  set->dc_prec = ::GetPrivateProfileInt("advVideo", "intraDCPrec", set->dc_prec, filename);
  set->maxmotion = ::GetPrivateProfileInt("advVideo", "variableMaxMotion", set->maxmotion, filename);
  set->vbv_buffer_size = ::GetPrivateProfileInt("advVideo", "vbvBufferSize", set->vbv_buffer_size, filename);
  set->fixed_vbv_delay = ::GetPrivateProfileInt("advVideo", "forceVBVDelay", set->fixed_vbv_delay, filename);
  set->display_horizontal_size = ::GetPrivateProfileInt("advVideo", "displayHorizontalSize", set->display_horizontal_size, filename);
  set->display_vertical_size = ::GetPrivateProfileInt("advVideo", "displayVerticalSize", set->display_vertical_size, filename);
  set->prog_seq = ::GetPrivateProfileInt("advVideo", "progressiveSequence", set->prog_seq, filename);
  set->prog_frame = ::GetPrivateProfileInt("advVideo", "progressiveFrame", set->prog_frame, filename);
  set->fieldpic = ::GetPrivateProfileInt("advVideo", "fieldPictures", set->fieldpic, filename);
  set->topfirst = ::GetPrivateProfileInt("advVideo", "topFieldFirst", set->topfirst, filename);
  set->repeatfirst = ::GetPrivateProfileInt("advVideo", "repeatFirstField", set->repeatfirst, filename);
  set->intravlc_tab[0] = ::GetPrivateProfileInt("advVideo", "intraVLCFormatI", set->intravlc_tab[0], filename);
  set->intravlc_tab[1] = ::GetPrivateProfileInt("advVideo", "intraVLCFormatP", set->intravlc_tab[1], filename);
  set->intravlc_tab[2] = ::GetPrivateProfileInt("advVideo", "intraVLCFormatB", set->intravlc_tab[2], filename);
  set->frame_pred_dct_tab[0] = ::GetPrivateProfileInt("advVideo", "framePredDCTI", set->frame_pred_dct_tab[0], filename);
  set->frame_pred_dct_tab[1] = ::GetPrivateProfileInt("advVideo", "framePredDCTP", set->frame_pred_dct_tab[1], filename);
  set->frame_pred_dct_tab[2] = ::GetPrivateProfileInt("advVideo", "framePredDCTB", set->frame_pred_dct_tab[2], filename);//
  set->qscale_tab[0] = ::GetPrivateProfileInt("advVideo", "quantizationScaleI", set->qscale_tab[0], filename);
  set->qscale_tab[1] = ::GetPrivateProfileInt("advVideo", "quantizationScaleP", set->qscale_tab[1], filename);
  set->qscale_tab[2] = ::GetPrivateProfileInt("advVideo", "quantizationScaleB", set->qscale_tab[2], filename);
  set->altscan_tab[0] = ::GetPrivateProfileInt("advVideo", "alternateScanI", set->altscan_tab[0], filename);
  set->altscan_tab[1] = ::GetPrivateProfileInt("advVideo", "alternateScanP", set->altscan_tab[1], filename);
  set->altscan_tab[2] = ::GetPrivateProfileInt("advVideo", "alternateScanB", set->altscan_tab[2], filename);
  set->write_sde = ::GetPrivateProfileInt("advVideo", "writeSequenceDisplayExt", set->write_sde, filename);
  set->write_sec = ::GetPrivateProfileInt("advVideo", "writeSequenceEndCode", set->write_sec, filename);
  set->embed_SVCD_user_blocks = ::GetPrivateProfileInt("advVideo", "embedSVCDUserBlocks", set->embed_SVCD_user_blocks, filename);
  set->write_pde = ::GetPrivateProfileInt("advVideo", "writePictureDisplayExt", set->write_pde, filename);
  set->frame_centre_horizontal_offset = ::GetPrivateProfileInt("advVideo", "frameCentreHorizontalOffset", set->frame_centre_horizontal_offset, filename);
  set->frame_centre_vertical_offset = ::GetPrivateProfileInt("advVideo", "frameCentreVerticalOffset", set->frame_centre_vertical_offset, filename);
  set->slice_hdr_every_MBrow = ::GetPrivateProfileInt("advVideo", "sliceHdrEveryMBrow", set->slice_hdr_every_MBrow, filename);

  set->r = ::GetPrivateProfileInt("advVideo", "reactionParameter", set->r, filename);
  set->avg_act = ::GetPrivateProfileInt("advVideo", "initialAverageActivity", set->avg_act, filename);
  set->Xi = ::GetPrivateProfileInt("advVideo", "initialIComplexity", set->Xi, filename);
  set->Xp = ::GetPrivateProfileInt("advVideo", "initialPComplexity", set->Xp, filename);
  set->Xb = ::GetPrivateProfileInt("advVideo", "initialBComplexity", set->Xb, filename);
  set->d0i = ::GetPrivateProfileInt("advVideo", "initialIFullness", set->d0i, filename);
  set->d0p = ::GetPrivateProfileInt("advVideo", "initialPFullness", set->d0p, filename);
  set->d0b = ::GetPrivateProfileInt("advVideo", "initialBFullness", set->d0b, filename);
  set->min_frame_percent = ::GetPrivateProfileInt("advVideo", "minFramePercentage", set->min_frame_percent, filename);
  set->pad_frame_percent = ::GetPrivateProfileInt("advVideo", "padFramePercentage", set->pad_frame_percent, filename);
  set->reset_d0pb = ::GetPrivateProfileInt("advVideo", "resetPBIVBF", set->reset_d0pb, filename);

  set->automotion = ::GetPrivateProfileInt("advVideo", "autoVectorLengths", set->automotion, filename);
  set->xmotion = ::GetPrivateProfileInt("advVideo", "horzPelMovement", set->xmotion, filename);
  set->ymotion = ::GetPrivateProfileInt("advVideo", "vertPelMovement", set->ymotion, filename);

  set->motion_data[0].forw_hor_f_code = ::GetPrivateProfileInt("advVideo", "forwHorzFCodeP", set->motion_data[0].forw_hor_f_code, filename);
  set->motion_data[0].forw_vert_f_code = ::GetPrivateProfileInt("advVideo", "forwVertFCodeP", set->motion_data[0].forw_vert_f_code, filename);
  set->motion_data[0].sxf = ::GetPrivateProfileInt("advVideo", "forwHorzSearchP", set->motion_data[0].sxf, filename);
  set->motion_data[0].syf = ::GetPrivateProfileInt("advVideo", "forwVertSearchP", set->motion_data[0].syf, filename);

  for (i = 1; i < MAXM; i++)
  {
    sprintf(tmpStr, "forwHorzFCodeB%d", i);
    set->motion_data[i].forw_hor_f_code = ::GetPrivateProfileInt("advVideo", tmpStr, set->motion_data[i].forw_hor_f_code, filename);
    sprintf(tmpStr, "forwVertFCodeB%d", i);
    set->motion_data[i].forw_vert_f_code = ::GetPrivateProfileInt("advVideo", tmpStr, set->motion_data[i].forw_vert_f_code, filename);
    sprintf(tmpStr, "forwHorzSearchB%d", i);
    set->motion_data[i].sxf = ::GetPrivateProfileInt("advVideo", tmpStr, set->motion_data[i].sxf, filename);
    sprintf(tmpStr, "forwVertSearchB%d", i);
    set->motion_data[i].syf = ::GetPrivateProfileInt("advVideo", tmpStr, set->motion_data[i].syf, filename);
    sprintf(tmpStr, "backHorzFCodeB%d", i);
    set->motion_data[i].back_hor_f_code = ::GetPrivateProfileInt("advVideo", tmpStr, set->motion_data[i].back_hor_f_code, filename);
    sprintf(tmpStr, "backVertFCodeB%d", i);
    set->motion_data[i].back_vert_f_code = ::GetPrivateProfileInt("advVideo", tmpStr, set->motion_data[i].back_vert_f_code, filename);
    sprintf(tmpStr, "backHorzSearchB%d", i);
    set->motion_data[i].sxb = ::GetPrivateProfileInt("advVideo", tmpStr, set->motion_data[i].sxb, filename);
    sprintf(tmpStr, "backVertSearchB%d", i);
    set->motion_data[i].syb = ::GetPrivateProfileInt("advVideo", tmpStr, set->motion_data[i].syb, filename);
  }

  set->EncodeVideo = set->UserEncodeVideo;
  set->EncodeAudio = set->UserEncodeAudio;
  set->frame_rate = ratetab[set->frame_rate_code - 1];
  if (set->video_type == MPEG_VCD)
    set->constrparms = TRUE;
  set->tc0 = set->hours;
  set->tc0 = 60 * set->tc0 + set->mins;
  set->tc0 = 60 * set->tc0 + set->sec;
  set->tc0 = (int)(set->frame_rate + 0.5) * set->tc0 + set->tframe;

  if (CheckVideoSettings(set))
  {
    if (CheckAudioSettings(set))
      return TRUE;
    else
    {
      sprintf(tmpStr, "Invalid audio settings in file %s.", filename);
      MessageBox(NULL, tmpStr, AppName, MB_OK);
    }
  }
  else
  {
    sprintf(tmpStr,"Invalid video settings in file %s.", filename);
    MessageBox(NULL, tmpStr, AppName, MB_OK);
  }
  return FALSE;
}

/*

int WritePrivateProfileInt(char *section, char *key, int value, char *filename)
{
  char tmpStr[20];

  sprintf(tmpStr, "%d", value);
  return ::WritePrivateProfileString(section, key, tmpStr, filename);
}

int WriteSettings(char *filename, mpegOutSettings *set)
{
  int i;
  char tmpStr[40];

  // load/save settings
  if (!::WritePrivateProfileString("loadsave", "description", set->id_string, filename))
    return FALSE;

  // input/output settings
  if (!WritePrivateProfileInt("iofiles", "saveTempVideo", set->SaveTempVideo, filename))
    return FALSE;
  if (!WritePrivateProfileInt("iofiles", "saveTempAudio", set->SaveTempAudio, filename))
    return FALSE;
  if (!::WritePrivateProfileString("iofiles", "intraQuantMatrix", set->iqname, filename))
    return FALSE;
  if (!::WritePrivateProfileString("iofiles", "nonIntraQuantMatrix", set->niqname, filename))
    return FALSE;
  if (!::WritePrivateProfileString("iofiles", "statisticsFilename", set->statname, filename))
    return FALSE;

  // general settings
  if (!WritePrivateProfileInt("general", "useFloatingPoint", set->useFP, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "verbose", set->verbose, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "blackAndWhite", set->B_W, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "userEncodeVideo", set->UserEncodeVideo, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "userEncodeAudio", set->UserEncodeAudio, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "multiplexVideo", set->MplexVideo, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "multiplexAudio", set->MplexAudio, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "muxStartTime", set->mux_start_time, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "muxStopTime", set->mux_stop_time, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "maxFileSize", set->max_file_size, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "resetClocks", set->reset_clocks, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "setBrokenLink", set->set_broken_link, filename))
    return FALSE;
  if (!WritePrivateProfileInt("general", "writeEndCodes", set->write_end_codes, filename))
    return FALSE;

  // video settings
  if (!WritePrivateProfileInt("video", "videoType", set->video_type, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "videoPulldownFlag", set->video_pulldown_flag, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "profile", set->profile, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "level", set->level, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "chromaFormat", set->chroma_format, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "aspectRatio", set->aspectratio, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "frameRateCode", set->frame_rate_code, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "iFramesInGOP", set->N, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "ipFrameDistance", set->M, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "autoBitrate", set->auto_bitrate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "constBitrateFlag", set->constant_bitrate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "mquantValue", set->mquant_value, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "constBitrate", (int)set->bit_rate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "maxBitrate", (int)set->max_bit_rate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "avgBitrate", (int)set->avg_bit_rate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "minBitrate", (int)set->min_bit_rate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "firstFrameHours", set->hours, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "firstFrameMinutes", set->mins, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "firstFrameSeconds", set->sec, filename))
    return FALSE;
  if (!WritePrivateProfileInt("video", "firstFrameFrame", set->tframe, filename))
    return FALSE;

  // audio settings
  if (!WritePrivateProfileInt("audio", "audioLayer", set->audio_layer, filename))
    return FALSE;
  if (!WritePrivateProfileInt("audio", "audioMode", set->audio_mode, filename))
    return FALSE;
  if (!WritePrivateProfileInt("audio", "deEmphasis", set->emphasis, filename))
    return FALSE;
  if (!WritePrivateProfileInt("audio", "audioBitrate", set->audio_bitrate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("audio", "privateBit", set->extension, filename))
    return FALSE;
  if (!WritePrivateProfileInt("audio", "copyrightBit", set->copyright, filename))
    return FALSE;
  if (!WritePrivateProfileInt("audio", "originalBit", set->original, filename))
    return FALSE;
  if (!WritePrivateProfileInt("audio", "errorProtection", set->error_protection, filename))
    return FALSE;
  if (!WritePrivateProfileInt("audio", "psychModel", set->psych_model, filename))
    return FALSE;

  // program stream settings
  if (!WritePrivateProfileInt("program", "programStreamType", set->mplex_type, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "vbrMultiplex", set->VBR_multiplex, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "padVCDAudio", set->vcd_audio_pad, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "writeProgramEndCode", set->write_pec, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "muxSVCDScanOffsets", set->mux_SVCD_scan_offsets, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "alignSequenceHeaders", set->align_sequence_headers, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "usePrivateStream2", set->put_private2, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "useComputedBitrate", set->use_computed_bitrate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "frameTimestamps", set->frame_timestamps, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "muxPulldownFlag", set->mplex_pulldown_flag, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "userMuxRate", set->user_mux_rate, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "sectorSize", set->sector_size, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "packetsPerPack", set->packets_per_pack, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "videoBufferSize", set->video_buffer_size, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "audioBufferSize", set->audio_buffer_size, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "audio1BufferSize", set->audio1_buffer_size, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "sectorDelay", set->sectors_delay, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "videoDelay", set->video_delay_ms, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "audioDelay", set->audio_delay_ms, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "audio1Delay", set->audio1_delay_ms, filename))
    return FALSE;
  if (!WritePrivateProfileInt("program", "alwaysWriteSysHeader", set->always_sys_header, filename))
    return FALSE;

  // advanced video settings
  if (!WritePrivateProfileInt("advVideo", "colorPrimaries", set->color_primaries, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "transferCharacteristics", set->transfer_characteristics, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "matrixCoefficients", set->matrix_coefficients, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "videoFormat", set->video_format, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "intraDCPrec", set->dc_prec, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "variableMaxMotion", set->maxmotion, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "vbvBufferSize", set->vbv_buffer_size, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "forceVBVDelay", set->fixed_vbv_delay, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "displayHorizontalSize", set->display_horizontal_size, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "displayVerticalSize", set->display_vertical_size, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "progressiveSequence", set->prog_seq, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "progressiveFrame", set->prog_frame, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "fieldPictures", set->fieldpic, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "topFieldFirst", set->topfirst, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "repeatFirstField", set->repeatfirst, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "intraVLCFormatI", set->intravlc_tab[0], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "intraVLCFormatP", set->intravlc_tab[1], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "intraVLCFormatB", set->intravlc_tab[2], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "framePredDCTI", set->frame_pred_dct_tab[0], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "framePredDCTP", set->frame_pred_dct_tab[1], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "framePredDCTB", set->frame_pred_dct_tab[2], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "quantizationScaleI", set->qscale_tab[0], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "quantizationScaleP", set->qscale_tab[1], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "quantizationScaleB", set->qscale_tab[2], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "alternateScanI", set->altscan_tab[0], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "alternateScanP", set->altscan_tab[1], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "alternateScanB", set->altscan_tab[2], filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "writeSequenceDisplayExt", set->write_sde, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "writeSequenceEndCode", set->write_sec, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "embedSVCDUserBlocks", set->embed_SVCD_user_blocks, filename))
    return FALSE;

  if (!WritePrivateProfileInt("advVideo", "writePictureDisplayExt", set->write_pde, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "frameCentreHorizontalOffset", set->frame_centre_horizontal_offset, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "frameCentreVerticalOffset", set->frame_centre_vertical_offset, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "sliceHdrEveryMBrow", set->slice_hdr_every_MBrow, filename))
	return FALSE;

  if (!WritePrivateProfileInt("advVideo", "reactionParameter", set->r, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "initialAverageActivity", set->avg_act, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "initialIComplexity", set->Xi, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "initialPComplexity", set->Xp, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "initialBComplexity", set->Xb, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "initialIFullness", set->d0i, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "initialPFullness", set->d0p, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "initialBFullness", set->d0b, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "minFramePercentage", set->min_frame_percent, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "padFramePercentage", set->pad_frame_percent, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "resetPBIVBF", set->reset_d0pb, filename))
    return FALSE;

  if (!WritePrivateProfileInt("advVideo", "autoVectorLengths", set->automotion, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "horzPelMovement", set->xmotion, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "vertPelMovement", set->ymotion, filename))
    return FALSE;

  if (!WritePrivateProfileInt("advVideo", "forwHorzFCodeP", set->motion_data[0].forw_hor_f_code, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "forwVertFCodeP", set->motion_data[0].forw_vert_f_code, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "forwHorzSearchP", set->motion_data[0].sxf, filename))
    return FALSE;
  if (!WritePrivateProfileInt("advVideo", "forwVertSearchP", set->motion_data[0].syf, filename))
    return FALSE;

  for (i = 1; i < MAXM; i++)
  {
    sprintf(tmpStr, "forwHorzFCodeB%d", i);
    if (!WritePrivateProfileInt("advVideo", tmpStr, set->motion_data[i].forw_hor_f_code, filename))
      return FALSE;
    sprintf(tmpStr, "forwVertFCodeB%d", i);
    if (!WritePrivateProfileInt("advVideo", tmpStr, set->motion_data[i].forw_vert_f_code, filename))
      return FALSE;
    sprintf(tmpStr, "forwHorzSearchB%d", i);
    if (!WritePrivateProfileInt("advVideo", tmpStr, set->motion_data[i].sxf, filename))
      return FALSE;
    sprintf(tmpStr, "forwVertSearchB%d", i);
    if (!WritePrivateProfileInt("advVideo", tmpStr, set->motion_data[i].syf, filename))
      return FALSE;
    sprintf(tmpStr, "backHorzFCodeB%d", i);
    if (!WritePrivateProfileInt("advVideo", tmpStr, set->motion_data[i].back_hor_f_code, filename))
      return FALSE;
    sprintf(tmpStr, "backVertFCodeB%d", i);
    if (!WritePrivateProfileInt("advVideo", tmpStr, set->motion_data[i].back_vert_f_code, filename))
      return FALSE;
    sprintf(tmpStr, "backHorzSearchB%d", i);
    if (!WritePrivateProfileInt("advVideo", tmpStr, set->motion_data[i].sxb, filename))
      return FALSE;
    sprintf(tmpStr, "backVertSearchB%d", i);
    if (!WritePrivateProfileInt("advVideo", tmpStr, set->motion_data[i].syb, filename))
      return FALSE;
  }
  return TRUE;
}
*/

void ChangeVideoFilename(mpegOutSettings *set)
{
  char *tmpPtr;

  tmpPtr = strrchr(VideoFilename, 0x2e); // look for a .
  if (strlen(VideoFilename) && tmpPtr)
  {
    if (set->video_type < MPEG_MPEG2)
    {
      if (!strcmp(tmpPtr, ".temp"))
        tmpPtr[2] = 0x31;
    }
    else
    {
      if (!strcmp(tmpPtr, ".temp"))
        tmpPtr[2] = 0x32;
    }
  }
}

int HorzMotionCode(mpegOutSettings *set, int i)
{
  if (i < 8)
    return 1;
  if (i < 16)
    return 2;
  if (i < 32)
    return 3;
  if ((i < 64) || (set->constrparms))
    return 4;
  if (i < 128)
    return 5;
  if (i < 256)
    return 6;
  if ((i < 512) || (set->level == 10) || (set->video_type < MPEG_MPEG2))
    return 7;
  if ((i < 1024) || (set->level == 8))
    return 8;
  if (i < 2048)
    return 9;
  return 1;
}

int VertMotionCode(mpegOutSettings *set, int i)
{
  if (i < 8)
    return 1;
  if (i < 16)
    return 2;
  if (i < 32)
    return 3;
  if ((i < 64) || (set->level == 10) || (set->constrparms))
    return 4;
  return 5;
}


void AutoSetMotionData(mpegOutSettings *set)
{
  int i;

  if (set->M != 1)
  {
    for (i = 1; i < set->M; i++)
    {
      set->motion_data[i].sxf = set->xmotion * i;
      set->motion_data[i].forw_hor_f_code = HorzMotionCode(set, set->motion_data[i].sxf);
      set->motion_data[i].syf = set->ymotion * i;
      set->motion_data[i].forw_vert_f_code = VertMotionCode(set, set->motion_data[i].syf);
      set->motion_data[i].sxb = set->xmotion * (set->M - i);
      set->motion_data[i].back_hor_f_code = HorzMotionCode(set, set->motion_data[i].sxb);
      set->motion_data[i].syb = set->ymotion * (set->M - i);
      set->motion_data[i].back_vert_f_code = VertMotionCode(set, set->motion_data[i].syb);
    }
  }
  set->motion_data[0].sxf = set->xmotion * set->M;
  set->motion_data[0].forw_hor_f_code = HorzMotionCode(set, set->motion_data[0].sxf);
  set->motion_data[0].syf = set->ymotion * set->M;
  set->motion_data[0].forw_vert_f_code = VertMotionCode(set, set->motion_data[0].syf);
}

void AutoSetBitrateData(mpegOutSettings *set)
{
  if (!set->auto_bitrate)
    return;

  if (horizontal_size && vertical_size)
  {
    if (set->video_type == MPEG_VCD)
    {
      set->bit_rate = 1150000;
      set->vbv_buffer_size = 20;
      set->video_buffer_size = 46;
      return;
    }
    if (set->video_type == MPEG_SVCD)
    {
      set->bit_rate = svcdrates[set->audio_bitrate];
      set->vbv_buffer_size = 112;
      set->video_buffer_size = 230;
      return;
    }
    set->bit_rate = floor((double)horizontal_size * (double)vertical_size * (double)set->frame_rate * 0.74);
    set->vbv_buffer_size = (int)floor(((double)set->bit_rate * 0.20343) / 16384.0);
    if (set->video_type < MPEG_MPEG2)
    {
      if (set->vbv_buffer_size > 1023)
        set->vbv_buffer_size = 1023;
    }
    else
    {
      if (set->vbv_buffer_size > vbvlim[(set->level - 4) >> 1])
        set->vbv_buffer_size = vbvlim[(set->level - 4) >> 1];
    }
    if (set->mplex_type < MPEG_DVD)
      set->video_buffer_size = set->vbv_buffer_size << 1;
    else
      set->video_buffer_size = 232;
  }
}

