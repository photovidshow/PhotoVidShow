using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CustomButton
{
    public partial class AnimatedDecorationNamer : Form
    {
        public enum AnimatedDecorationNamerType
        {
            Create,
            Rename
        }

        private AnimatedDecorationNamerType mType = AnimatedDecorationNamerType.Create;

        public AnimatedDecorationNamer( AnimatedDecorationNamerType type )
        {
            mType = type;
            InitializeComponent();
            if (mType == AnimatedDecorationNamerType.Rename)
            {
                this.Text = "Rename Effect";
                this.mCreateApplyButton.Text = "Apply";
            }
        }

        public string EffectName
        {
            get { return this.mNameTextBox.Text; }
            set { this.mNameTextBox.Text = value; }
        }
    }
}
