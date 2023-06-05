
#define _CRT_SECURE_NO_WARNINGS 1
#include <windows.h>
/*
#define STRICT
//#include <owl/pch.h>
#if !defined(OWL_PROPSHT_H)
# include <owl/propsht.h>
#endif
#if !defined(OWL_COMMCTRL_H)
# include <owl/commctrl.h>
#endif
#if !defined(OWL_CHOOSECO_H)
# include <owl/chooseco.h>
#endif
#include <owl\gdiobjec.h>
#include <owl\window.h>

#ifndef OWLNEXT
//#include <owl\owlpch.h>
#endif

#include <owl\dc.h>
#include <owl\framewin.h>
#include <owl\listbox.h>
#include <owl\checkbox.h>
#include <owl\groupbox.h>
#include <owl\radiobut.h>
#include <owl\button.h>
#include <owl\combobox.h>
#include <owl\scrollba.h>
#include <owl\static.h>
#include <owl\edit.h>
#include <owl\validate.h>
#include <owl\opensave.h>
#include <owl\gauge.h>
#include <owl\updown.h>
#include <owl\slider.h>
#include <owl\treewind.h>
#include "gui1.h"

class TMPEGDialog : public TDialog
{
  public:
    TMPEGDialog(TWindow* parent, int resourceID, TModule* module = 0);
    void SetupWindow();
    void ShowInfo();
    void CmCancel();
    void CmHelp();
    void CmSuspend();
    void StartMPEG();
    void RunSettings();
    void YieldTime();
    void AddMessage(char *txt);
    void EvWriteMess();
    void EvBatchMode();

    TStatic *progressText;
    TButton *cancelButton;
    TButton *suspendButton;
    TButton *startButton;
    TButton *settingsButton;
    TButton *helpButton;
    TButton *writeMess;
    TListBox *messageLBox;
    TGauge *progressBar;
    TCheckBox *batchMode;

    int SuspendMPEG;

    DECLARE_RESPONSE_TABLE(TMPEGDialog);
};

#ifdef MPEGMAIN
#define EXTERN
#else
#define EXTERN extern
#endif



EXTERN TModule *module;
EXTERN TMPEGDialog *MPEGDialog;

*/
int startGui();

#ifndef MAXPATH
#define MAXPATH 260
#endif

extern char HelpFileName[MAXPATH];
extern char AppName[MAXPATH];
extern char About[MAXPATH];
extern char Version[20];
extern char StartupDir[MAXPATH];
extern char DefaultExt[5];
extern char ParamFilename[MAXPATH];
