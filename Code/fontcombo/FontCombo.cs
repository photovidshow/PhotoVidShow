using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace SquidgySoft.UI.Controls
{  
	public class FontComboBox : ComboBox
	{
		int    _maxWidth        = 0;
		bool  _displayNameNormalFont  = true;

		const string SAMPLE        = " - Hello World";
		const int DEFAULT_SIZE      = 11;

        private float default_size = 11;

        Font _arial = null;

		public FontComboBox()
		{        
            _arial = new Font("Arial", 11);

			MaxDropDownItems = 20;
			IntegralHeight = false;
			Sorted = false;
			DropDownStyle = ComboBoxStyle.DropDownList;
			DrawMode = DrawMode.OwnerDrawVariable;              
		}

		public void Populate(bool displayNameNormalFont)
		{
			_displayNameNormalFont = displayNameNormalFont;

			foreach (FontFamily ff in FontFamily.Families)
			{
				if(ff.IsStyleAvailable(FontStyle.Regular))
				{
					Items.Add(ff.Name);                        
				}
			}      
      
			if(Items.Count > 0)
			{
				SelectedIndex=0;
			}
		}

		protected override void OnMeasureItem(System.Windows.Forms.MeasureItemEventArgs e)
		{  
			if(e.Index > -1)
			{
				int w = 0;
				string fontName  = Items[e.Index].ToString();
                Font tmpFont = new Font(fontName, default_size);
				Graphics g    = CreateGraphics();
				if( _displayNameNormalFont )
				{
					SizeF fontSize    = g.MeasureString(SAMPLE, tmpFont);
					SizeF captionSize  = g.MeasureString(fontName, _arial);
					e.ItemHeight = (int)Math.Max(fontSize.Height, captionSize.Width);
					w = (int)(fontSize.Width + captionSize.Width);
				}
				else
				{
					SizeF s      = g.MeasureString(fontName, tmpFont);
					e.ItemHeight  = (int)s.Height;
					w = (int)s.Width;
				}
				_maxWidth = Math.Max(_maxWidth, w);
				e.ItemHeight = Math.Min(e.ItemHeight, 28);
			}
			base.OnMeasureItem(e);
		}

		protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
		{  
			if(e.Index > -1)
			{
				string fontName  = Items[e.Index].ToString();
                Font tmpFont = new Font(fontName, default_size);
    
				if(_displayNameNormalFont)
				{
					Graphics g = CreateGraphics();
					int w = (int)g.MeasureString(fontName, _arial).Width;

					if((e.State & DrawItemState.Focus)==0)
					{
						e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), e.Bounds);
						e.Graphics.DrawString(fontName, _arial, new SolidBrush(SystemColors.WindowText), e.Bounds.X*2, e.Bounds.Y);  
						e.Graphics.DrawString(SAMPLE,tmpFont,new SolidBrush(SystemColors.WindowText), e.Bounds.X*2+w, e.Bounds.Y);  
					}
					else
					{
						e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
						e.Graphics.DrawString(fontName, _arial, new SolidBrush(SystemColors.HighlightText), e.Bounds.X*2, e.Bounds.Y);  
						e.Graphics.DrawString(SAMPLE, tmpFont,new SolidBrush(SystemColors.HighlightText), e.Bounds.X*2+w, e.Bounds.Y);  
					}  
				}
				else
				{
					if((e.State & DrawItemState.Focus)==0)
					{
						e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), e.Bounds);
						e.Graphics.DrawString(fontName,tmpFont,new SolidBrush(SystemColors.WindowText), e.Bounds.X*2, e.Bounds.Y);
					}
					else
					{
						e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
						e.Graphics.DrawString(fontName,tmpFont,new SolidBrush(SystemColors.HighlightText), e.Bounds.X*2, e.Bounds.Y);
					}   
				}
			}
			base.OnDrawItem(e);
		}

		protected override void OnDropDown(System.EventArgs e)
		{
			this.DropDownWidth = _maxWidth+30;
		}    
	}
}

