using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CustomButton
{
    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            this.AutoScroll = false;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }
    }
}
