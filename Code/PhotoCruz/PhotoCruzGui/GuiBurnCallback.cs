using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PhotoCruz;
using System.IO;
using System.Collections;
using MangedToUnManagedWrapper;

namespace PhotoCruzGui
{ 
    public class BurnerProgressCallback : MangedToUnManagedWrapper.IVideoDiskCallback
    {

        private PhotoCruzMainWindow mAu;
        private bool mDoingPadding = false;
        public BurnerProgressCallback(PhotoCruzMainWindow au)
        {
            mAu = au;
        }

        public override void PadStartedCallback()
        {
            mAu.SetCurrentProcessText("Padding disk to 1Gb...Please wait");
            mDoingPadding = true;
            //	mAu.GetPercentageDone().Value = 0;
        }

        public override void BurnPercentageComplete(int amount)
        {
            if (amount < 0) amount = 0;
            if (amount > 100) amount = 100;

            if (amount > 0 && amount < 99 && mDoingPadding == false)
            {
                mAu.SetCurrentProcessText("Burning...");
            }

            if (amount >= 99 && mDoingPadding == true)
            {
                mAu.SetCurrentProcessText("Finalizing... Wait for disk to finish!!");
                mAu.mBurnReachFinalizeStage = true;
            }

            mAu.SetProgressBarValue(amount);
        }

        public override void BufferStatusCallback(int percent_done, int bufferFreeSizeInUCHARs, int BufferSizeInUCHARs)
        {
            if (percent_done < 0) percent_done = 0;
            if (percent_done > 100) percent_done = 100;

            mAu.SetBurnBufferValue(percent_done);
        }
    }
}