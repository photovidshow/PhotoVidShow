using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ManagedCore
{
    public partial class CheckingInternetForm : Form
    {
        private bool mFinishedChecking = false;
        private Timer mCheckForFinishedTimer = new Timer();

        public CheckingInternetForm()
        {
            InitializeComponent();

            mCheckForFinishedTimer.Interval = 100;
            mCheckForFinishedTimer.Start();
            mCheckForFinishedTimer.Tick += new EventHandler(mCheckForFinishedTimer_Tick);
        }
         
        private void mCheckForFinishedTimer_Tick(object sender, EventArgs e)
        {
            if (mFinishedChecking == true)
            {
                mCheckForFinishedTimer.Stop();
                mCheckForFinishedTimer.Dispose();
                this.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void DeclareFinishedChecking()
        {
            mFinishedChecking = true;
        }

    }
}
