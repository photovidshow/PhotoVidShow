using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DVDSlideshow;

namespace TestControls
{
    public class ColorListBox : ListBox
    {
        public List<Color> ColorList;

        public ColorListBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
            ColorList = new List<Color>();
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)((18.0f * (CGlobals.dpiX / 96.0f))+0.4999f);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();

            e.DrawFocusRectangle();

            // Draw the current item text based on the current Font and the custom brush settings.
            if ((e.Index >= 0) && (e.Index < Items.Count))
            {
                Font f = new Font("Segeo UI", 10, FontStyle.Regular);

                e.Graphics.DrawString(Items[e.Index].ToString(), f,
                    new SolidBrush(ColorList[e.Index]), e.Bounds, StringFormat.GenericDefault);
            }
            base.OnDrawItem(e);
        }

        public void Swap(int indexa, int indexb)
        {
            if (indexa >= 0 && indexb >= 0 && indexa < ColorList.Count && indexb < ColorList.Count && indexa != indexb)
            {
                Color temp = ColorList[indexa];
                ColorList[indexa] = ColorList[indexb];
                ColorList[indexb] = temp;

                object temp2 = Items[indexa];
                Items[indexa] = Items[indexb];
                Items[indexb] = temp2;
            }
        }


        public void AddTop(object item, Color color)
        {
            ColorList.Insert(0, color);
            Items.Insert(0, item);
        }

        public void Clear()
        {
            ColorList.Clear();
            Items.Clear();
        }

        public void RemoveAt(int index)
        {
            ColorList.RemoveAt(index);
            Items.RemoveAt(index);
        }

        public void Remove(object item)
        {
            ColorList.RemoveAt(Items.IndexOf(item));
            Items.Remove(item);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ColorListBox
            // 
            this.ResumeLayout(false);
        }

    }

}
