using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CustomButton
{
    public class NumericUpDownOverride : NumericUpDown
    {
        public event NumericUpDownButtonDelegate UpButtonSelected;
        public event NumericUpDownButtonDelegate DownButtonSelected;

        public override void UpButton()
        {
            base.UpButton();
            if (UpButtonSelected != null)
            {
                UpButtonSelected();
            }
        }
        public override void DownButton()
        {
            base.DownButton();
            if (DownButtonSelected != null)
            {
                DownButtonSelected();
            }
        }

    }
}
