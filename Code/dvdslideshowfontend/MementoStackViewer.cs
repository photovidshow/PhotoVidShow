using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DVDSlideshow;
using DVDSlideshow.Memento;
using ManagedCore;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for MementoStackViewer.
	/// </summary>
	public class MementoStackViewer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MementoStackViewer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			DVDSlideshow.Memento.Caretaker.GetInstance().StateChanged+= new ManagedCore.StateChangeHandler(this.CareTakeChangedCallback);

			ReDrawListBox();

		}

		public void CareTakeChangedCallback()
		{
			ReDrawListBox();
		}

		public void ReDrawListBox()
		{
			this.listBox1.Items.Clear(); 

			Stack s =DVDSlideshow.Memento.Caretaker.GetInstance().SnapshotStack;

			Array a = s.ToArray();

			int count =a.Length-1;

			int si = DVDSlideshow.Memento.Caretaker.GetInstance().CurrentStackIndex;

			foreach (DVDSlideshow.Memento.Snapshot snap in a)
			{

				// assume 1 memento per snapshot

				Memento m = (Memento) snap.mMementos[0];
				string ss = "Snapshot "+count.ToString()+" ";

				ss += snap.mDescription;
				if (si == count+1) ss+=" <-----";

				this.listBox1.Items.Add(ss);
				count --;
			}
		}



		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(292, 264);
			this.listBox1.TabIndex = 0;
			// 
			// MementoStackViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.listBox1);
			this.Name = "MementoStackViewer";
			this.Text = "MementoStackViewer";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
