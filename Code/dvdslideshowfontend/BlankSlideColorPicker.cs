using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace dvdslideshowfontend
{
    public partial class BlankSlideColorPicker : Form
    {
        public Color mColor = Color.Black;
        public BlankSlideColorPicker(bool edit)
        {
            InitializeComponent();

            if (edit == true)
            {
                this.Text = "Edit color";
                this.button1.Text = "Apply color";
            }  
        }

        public void SetComboColor(Color color)
        {
            this.colorComboBox1.SelectedColor = color;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            mColor = this.colorComboBox1.SelectedColor;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
