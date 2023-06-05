using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Extender provider that shows a FolderBrowserDialog and automatically sets the
	/// appropriate control's text to the selected folder.
	/// </summary>
	[ToolboxBitmap(typeof(FolderBrowserDialog))]
	[ProvideProperty("BrowseButton", typeof(Control))]
	public class FolderBrowserExtenderProvider : Component, IExtenderProvider
	{
		private Hashtable extendees;
		protected FolderBrowserDialog folderDialog;

		public FolderBrowserExtenderProvider()
		{
			extendees = new Hashtable();
			folderDialog = new FolderBrowserDialog();
			folderDialog.Description="Find DVD video folder";
		
		}

		// Save the contents of the FolderBrowserDialog
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The FolderBrowserDialog shown when the assigned BrowseButton is clicked")]
		public System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog
		{
			get {  return folderDialog; }
		}
		
		#region IExtenderProvider Member

		public bool CanExtend(object extendee)
		{
			return (extendee is Control);
		}

		[DefaultValue(null)]
		public Control GetBrowseButton(Control extendee)
		{
			if (extendees.Contains(extendee))
				return (Control)extendees[extendee];
			else
				return null;
		}

		public void SetBrowseButton(Control extendee, ToolStripItem browseButton)
		{
		//	if (DesignMode && (extendee == browseButton))
	///		{
	//			string correctCode = "SetBrowseButton("+extendee.Name+", "+browseButton.Name+");";
	//			throw new ApplicationException("Because of a bug in the code generation you cannot set a control to be its own BrowseButton in the visual designer.\r\nAdding the call to SetBrowseButton() to code does work, though.\r\n\r\nYou might consider adding this line to your code:\r\n"+correctCode);
	//		}

			if (extendees.Contains(extendee))
			{
				Control btn = extendees[extendee] as Control;
				btn.Click -= new EventHandler(browseButton_Click);
				extendees.Remove(extendee);
			}

			if (browseButton == null)
				return;

            extendees[extendee] = browseButton;
			browseButton.Click += new EventHandler(browseButton_Click);
		}


		#endregion

		private void browseButton_Click(object sender, EventArgs e)
		{
            ToolStripItem btn = sender as ToolStripItem;

			if (folderDialog.ShowDialog() == DialogResult.OK)
			{
				// Find the accompanying target and set it's text
				foreach (DictionaryEntry ent in extendees)
				{
					if (ent.Value == btn)
					{
						Control target = ent.Key as Control;
						target.Text = folderDialog.SelectedPath;
						return;
					}
				}
			}
		}
	}
}
