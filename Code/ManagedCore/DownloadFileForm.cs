using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ManagedCore
{
    public partial class DownloadFileForm : Form
    {
     
        public delegate void CancelDownloadDelegate();

        private CancelDownloadDelegate mCancelDelegate;

        //*******************************************************************
        public DownloadFileForm(CancelDownloadDelegate cancelDelegate)
        {
            mCancelDelegate = cancelDelegate;
            InitializeComponent();
        }

        //*******************************************************************
        public void SetProgressPercent(int value)
        {
            this.mProgressLabel.Text = "Progress " + value.ToString() + "%";
            this.mProgressBar.Value = value;
        }

        //*******************************************************************
        private void mCancelButton_Click(object sender, EventArgs e)
        {
            if (mCancelDelegate != null)
            {
                mCancelDelegate();
            }
        }

    }
}
