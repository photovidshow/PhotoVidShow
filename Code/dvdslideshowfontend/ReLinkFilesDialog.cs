using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using dvdslideshowfontend;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using ManagedCore;
using System.Threading;
using DVDSlideshow;


namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for ReLinkFilesDialog.
	/// </summary>
	/// 

	public class ReLinkFilesDialog : System.Windows.Forms.Form
	{
		private SquidgySoft.UI.Controls.TreeViewFolderBrowser treeViewFolderBrowser1;
		private SquidgySoft.UI.Controls.TreeViewFolderBrowserDataProviderShell32 mShell32Provider;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Button Search;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button DoneButton;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Label NumOutstandingFiles;
		private System.Windows.Forms.Label d;
		private System.Windows.Forms.Button RemoveMissingFilesButton;
		private System.Windows.Forms.Panel UpperLabelPanel;
		private System.Windows.Forms.Panel upperpanel;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel lowepanel;
		private System.Windows.Forms.Panel lowerButtonPanel;

		private ArrayList mMissingFiles;

		private System.Threading.Thread mSearchThread = null;
		private System.Windows.Forms.Timer mTimer = new System.Windows.Forms.Timer();
		private System.Windows.Forms.Label CurrentDirSearchLabel;
		private bool mThreadStopped=false;
		private bool mNoNeedToKeepOnSearching=false;

        //*******************************************************************
		public ReLinkFilesDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

	    	mTimer.Interval=(100);
			mTimer.Tick+=new EventHandler(myTick);
			mTimer.Start();
	
		}

        //*******************************************************************
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
			mTimer.Stop();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReLinkFilesDialog));
            this.treeViewFolderBrowser1 = new SquidgySoft.UI.Controls.TreeViewFolderBrowser();
            this.DoneButton = new System.Windows.Forms.Button();
            this.Search = new System.Windows.Forms.Button();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.RemoveMissingFilesButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.NumOutstandingFiles = new System.Windows.Forms.Label();
            this.d = new System.Windows.Forms.Label();
            this.UpperLabelPanel = new System.Windows.Forms.Panel();
            this.upperpanel = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.lowepanel = new System.Windows.Forms.Panel();
            this.CurrentDirSearchLabel = new System.Windows.Forms.Label();
            this.lowerButtonPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.UpperLabelPanel.SuspendLayout();
            this.upperpanel.SuspendLayout();
            this.lowepanel.SuspendLayout();
            this.lowerButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewFolderBrowser1
            // 
            this.treeViewFolderBrowser1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewFolderBrowser1.CheckboxBehaviorMode = SquidgySoft.UI.Controls.CheckboxBehaviorMode.RecursiveChecked;
            this.treeViewFolderBrowser1.DataSource = null;
            this.treeViewFolderBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewFolderBrowser1.HideSelection = false;
            this.treeViewFolderBrowser1.Location = new System.Drawing.Point(0, 0);
            this.treeViewFolderBrowser1.Name = "treeViewFolderBrowser1";
            this.treeViewFolderBrowser1.SelectedDirectories = ((System.Collections.Specialized.StringCollection)(resources.GetObject("treeViewFolderBrowser1.SelectedDirectories")));
            this.treeViewFolderBrowser1.ShowLines = false;
            this.treeViewFolderBrowser1.ShowRootLines = false;
            this.treeViewFolderBrowser1.Size = new System.Drawing.Size(870, 280);
            this.treeViewFolderBrowser1.TabIndex = 0;
            // 
            // DoneButton
            // 
            this.DoneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DoneButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.DoneButton.Enabled = false;
            this.DoneButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DoneButton.Location = new System.Drawing.Point(790, 8);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(75, 23);
            this.DoneButton.TabIndex = 1;
            this.DoneButton.Text = "Apply";
            this.DoneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // Search
            // 
            this.Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Search.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Search.Location = new System.Drawing.Point(8, 8);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(96, 23);
            this.Search.TabIndex = 2;
            this.Search.Text = "Start search";
            this.Search.Click += new System.EventHandler(this.Search_Click);
            // 
            // dataGrid1
            // 
            this.dataGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid1.BackgroundColor = System.Drawing.SystemColors.Menu;
            this.dataGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid1.CaptionFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGrid1.CaptionVisible = false;
            this.dataGrid1.DataMember = "";
            this.dataGrid1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGrid1.HeaderFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(0, 24);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.PreferredColumnWidth = 500;
            this.dataGrid1.ReadOnly = true;
            this.dataGrid1.RowHeadersVisible = false;
            this.dataGrid1.Size = new System.Drawing.Size(870, 214);
            this.dataGrid1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Outstanding missing files:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RemoveMissingFilesButton
            // 
            this.RemoveMissingFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveMissingFilesButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoveMissingFilesButton.Location = new System.Drawing.Point(112, 8);
            this.RemoveMissingFilesButton.Name = "RemoveMissingFilesButton";
            this.RemoveMissingFilesButton.Size = new System.Drawing.Size(144, 23);
            this.RemoveMissingFilesButton.TabIndex = 4;
            this.RemoveMissingFilesButton.Text = "Remove missing files";
            this.RemoveMissingFilesButton.Click += new System.EventHandler(this.RemoveMissingFilesButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButton.Location = new System.Drawing.Point(710, 8);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(80, 23);
            this.CancelButton.TabIndex = 5;
            this.CancelButton.Text = "Cancel";
            // 
            // NumOutstandingFiles
            // 
            this.NumOutstandingFiles.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumOutstandingFiles.Location = new System.Drawing.Point(130, 4);
            this.NumOutstandingFiles.Name = "NumOutstandingFiles";
            this.NumOutstandingFiles.Size = new System.Drawing.Size(26, 16);
            this.NumOutstandingFiles.TabIndex = 6;
            this.NumOutstandingFiles.Text = "23";
            this.NumOutstandingFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // d
            // 
            this.d.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.d.Location = new System.Drawing.Point(4, 5);
            this.d.Name = "d";
            this.d.Size = new System.Drawing.Size(224, 23);
            this.d.TabIndex = 7;
            this.d.Text = "Please select directories to search in";
            // 
            // UpperLabelPanel
            // 
            this.UpperLabelPanel.Controls.Add(this.d);
            this.UpperLabelPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.UpperLabelPanel.Location = new System.Drawing.Point(0, 0);
            this.UpperLabelPanel.Name = "UpperLabelPanel";
            this.UpperLabelPanel.Size = new System.Drawing.Size(870, 22);
            this.UpperLabelPanel.TabIndex = 8;
            // 
            // upperpanel
            // 
            this.upperpanel.Controls.Add(this.treeViewFolderBrowser1);
            this.upperpanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.upperpanel.Location = new System.Drawing.Point(0, 22);
            this.upperpanel.Name = "upperpanel";
            this.upperpanel.Size = new System.Drawing.Size(870, 280);
            this.upperpanel.TabIndex = 9;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 302);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(870, 5);
            this.splitter1.TabIndex = 10;
            this.splitter1.TabStop = false;
            // 
            // lowepanel
            // 
            this.lowepanel.Controls.Add(this.NumOutstandingFiles);
            this.lowepanel.Controls.Add(this.CurrentDirSearchLabel);
            this.lowepanel.Controls.Add(this.label1);
            this.lowepanel.Controls.Add(this.dataGrid1);
            this.lowepanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lowepanel.Location = new System.Drawing.Point(0, 307);
            this.lowepanel.Name = "lowepanel";
            this.lowepanel.Size = new System.Drawing.Size(870, 259);
            this.lowepanel.TabIndex = 11;
            // 
            // CurrentDirSearchLabel
            // 
            this.CurrentDirSearchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentDirSearchLabel.Location = new System.Drawing.Point(176, 4);
            this.CurrentDirSearchLabel.Name = "CurrentDirSearchLabel";
            this.CurrentDirSearchLabel.Size = new System.Drawing.Size(678, 14);
            this.CurrentDirSearchLabel.TabIndex = 7;
            this.CurrentDirSearchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lowerButtonPanel
            // 
            this.lowerButtonPanel.Controls.Add(this.RemoveMissingFilesButton);
            this.lowerButtonPanel.Controls.Add(this.Search);
            this.lowerButtonPanel.Controls.Add(this.CancelButton);
            this.lowerButtonPanel.Controls.Add(this.DoneButton);
            this.lowerButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lowerButtonPanel.Location = new System.Drawing.Point(0, 527);
            this.lowerButtonPanel.Name = "lowerButtonPanel";
            this.lowerButtonPanel.Size = new System.Drawing.Size(870, 39);
            this.lowerButtonPanel.TabIndex = 12;
            // 
            // ReLinkFilesDialog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(870, 566);
            this.ControlBox = false;
            this.Controls.Add(this.lowerButtonPanel);
            this.Controls.Add(this.lowepanel);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.upperpanel);
            this.Controls.Add(this.UpperLabelPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReLinkFilesDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Relink missing files";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.UpperLabelPanel.ResumeLayout(false);
            this.upperpanel.ResumeLayout(false);
            this.lowepanel.ResumeLayout(false);
            this.lowerButtonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        //*******************************************************************
		private ArrayList GetSelectedDirs()
		{
			ArrayList names = new ArrayList();
			foreach(string s in treeViewFolderBrowser1.SelectedDirectories)
			{
				names.Add(s);
			}	
			return names;
		}

        //*******************************************************************
		private void FillDataProviderCombo()
		{
    		mShell32Provider =new SquidgySoft.UI.Controls.TreeViewFolderBrowserDataProviderShell32();
		}

        //*******************************************************************
		protected override void OnLoad(EventArgs e)
		{
			this.Search.Enabled =false; 
			this.DoneButton.Enabled =false;

			if(!DesignMode)
			{
                string fn = CGlobals.GetTempDirectory() + @"/list";
                FileStream stream = new System.IO.FileStream(fn, FileMode.OpenOrCreate);
				try
				{
					BinaryFormatter binary = new BinaryFormatter();			
					this.treeViewFolderBrowser1.SelectedDirectories = binary.Deserialize(stream) as System.Collections.Specialized.StringCollection;
				} 
				catch{}
				stream.Close();	
				//
				FillDataProviderCombo();
				//
				this.treeViewFolderBrowser1.DataSource = mShell32Provider as SquidgySoft.UI.Controls.ITreeViewFolderBrowserDataProvider;
				//
				this.treeViewFolderBrowser1.Populate(System.Environment.SpecialFolder.ApplicationData);
				//
				this.treeViewFolderBrowser1.SelectedDirectoriesChanged +=new SquidgySoft.UI.Controls.SelectedDirectoriesChangedDelegate(this.DirectoriesChangedCallback);

				this.Text = "Relink missing files";
			}
			
			FillDataBox();

			base.OnLoad (e);
		}

        //*******************************************************************
		public void HandoverMissingFilesList(ArrayList missing_files)
		{
			mMissingFiles = missing_files;
		}

        //*******************************************************************
		public void DirectoriesChangedCallback(object o, SquidgySoft.UI.Controls.SelectedDirectoriesChangedEventArgs a)
		{
			int num = treeViewFolderBrowser1.SelectedDirectories.Count;
			if (num==0)
			{ 
				this.Search.Enabled =false; ;
			}
			else
			{
				this.Search.Enabled = true ;
			}
		}

        //*******************************************************************
		private void FillDataBox()
		{
			// Create sample data for the DataGrid control.
			DataTable dt = new DataTable();
			
			DataRow dr;
 
			// Define the columns of the table.
			dt.Columns.Add(new DataColumn("File", typeof(string)));

			DataColumn dc2 = new DataColumn("State", typeof(string));
			dt.Columns.Add(dc2);
	//		DataColumn dc3 = new DataColumn("", typeof(string));
	//		dt.Columns.Add(dc3);

			if (mMissingFiles==null) return ;
			if (mMissingFiles.Count<1) return ;

			// Populate the table with sample values.
			for (int i = 0; i < mMissingFiles.Count; i++) 
			{
				dr = dt.NewRow();

                string filename = (mMissingFiles[i] as MissingFile).mFullName; 
                if (filename.Length > 60)
                {
                    filename = filename.Substring(filename.Length - 60, 60);
                }

                dr[0] = filename;

				string ssd = (mMissingFiles[i] as MissingFile).mFoundDirecotry;
				if (ssd=="") dr[1] = "Missing";
				else if (ssd.StartsWith("Removed")==true) dr[1] = ssd;
				else dr[1] = "Relinked to '"+ssd+"'";
				dt.Rows.Add(dr);
			
			}
 
			DataView dv = new DataView(dt);
	
			dataGrid1.DataSource = dv;

			int on = ManagedCore.MissingFilesDatabase.GetInstance().GetUnlinkedFiles().Count;
			this.NumOutstandingFiles.Text = on.ToString();
			if (on==0)
			{
				this.RemoveMissingFilesButton.Enabled = false ;
			}
			else
			{
				this.RemoveMissingFilesButton.Enabled = true ;
			}

			if (ManagedCore.MissingFilesDatabase.GetInstance().GetUnlinkedFiles().Count==0)
			{
				this.DoneButton.Enabled =true;
			}
			else
			{
				this.DoneButton.Enabled=false;
			}

			this.SizeColumnsToContent(this.dataGrid1,-1);
		}


        //*******************************************************************
		private System.Data.DataColumn CreateBoundColumn(String DataFieldValue, 
			String HeaderTextValue)
		{

			// Create a BoundColumn.
			DataColumn column = new DataColumn();

			return column;
		}


        //*******************************************************************
        private delegate void SetCurrentSearchDirectoryStringDelegate(string s);
        private void SetCurrentSearchDirectoryString(string s)
        {
            if (this.InvokeRequired == true)
            {
                BeginInvoke(new SetCurrentSearchDirectoryStringDelegate(SetCurrentSearchDirectoryString),
                                            new object[] { s });
                return;
            }

            CurrentDirSearchLabel.Text = s;
        }


		//*******************************************************************
		private void RecursiveCheck(string dir)
		{
			if (mNoNeedToKeepOnSearching==true) 
			{
				return ;
			}

			string[] Files = null;

			try
			{
				Files = Directory.GetFiles(dir);
			}
			catch (Exception)
			{
				return ;
			}

			if (Files==null)
			{
				return ;
			}

            SetCurrentSearchDirectoryString(dir);
		
			try
			{
				foreach(string s in Files) 
				{
					if (mNoNeedToKeepOnSearching==false)
					{
						int still_missing=0;
						string s2 = Path.GetFileName(s).ToLower();
						foreach (MissingFile mf in this.mMissingFiles)
						{
							if (mf.mFoundDirecotry=="")
							{
								still_missing++;
								if (mf.mName.ToLower() == s2)
								{
									mf.mFoundDirecotry = dir;
								}
							}
						}
						if (still_missing==0)
						{
							mNoNeedToKeepOnSearching=true;
						}
					}

				
				}
			}
			catch (Exception)
			{
			}

			try
			{
				string[] Dirs = Directory.GetDirectories(dir);

				foreach(string s in Dirs) 
				{
					RecursiveCheck(s);
				}
			}
			catch (Exception)
			{
				return ;
			}
		}

        //*******************************************************************
		private void StartSearch()
		{
			ArrayList dirs = GetSelectedDirs();

			if (dirs.Count <1) return ;

			for (int i=0; i<dirs.Count;i++)
			{
				if (mNoNeedToKeepOnSearching==false)
				{
					try
					{
						RecursiveCheck(dirs[i] as string);
					}
					catch
					{
					}
				}
			}
			mThreadStopped=true;

		}

		//*******************************************************************
		private void myTick(System.Object o, System.EventArgs e)
		{
			if (mThreadStopped==true &&
				mSearchThread!=null)
			{
				SearchThreadEnd();
				FillDataBox();
				mThreadStopped=false;
				mSearchThread=null;
			}

		}

		//*******************************************************************
		private void SearchThreadEnd()
		{
			CurrentDirSearchLabel.Text="";
			this.Search.Text = "Start search";	// we cause text change in main thread
			this.RemoveMissingFilesButton.Enabled=true;
			this.CancelButton.Enabled=true;
			this.DoneButton.Enabled=true;
			this.dataGrid1.Enabled=true;
			this.treeViewFolderBrowser1.Enabled=true;
		}


		//*******************************************************************
		private void Search_Click(object sender, System.EventArgs e)
		{
			// is search going on
			if (mSearchThread!=null)
			{
				mThreadStopped=true;
                mNoNeedToKeepOnSearching = true;
			}
			// no search thread is going on start it
			else
			{
				this.Search.Text = "Stop search";
				this.RemoveMissingFilesButton.Enabled=false;
				this.CancelButton.Enabled=false;
				this.DoneButton.Enabled=false;
				this.dataGrid1.Enabled=false;
				this.treeViewFolderBrowser1.Enabled=false;

				mThreadStopped=false;
				mNoNeedToKeepOnSearching=false;

				System.Threading.ThreadStart st = new System.Threading.ThreadStart(StartSearch);
				mSearchThread= new System.Threading.Thread(st);
				mSearchThread.Start();
			}
		}

        //*******************************************************************
		private void DoneButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}

        //*******************************************************************
		private void CancelButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}


        //*******************************************************************
		private void RemoveMissingFilesButton_Click(object sender, System.EventArgs e)
		{
			ArrayList a = MissingFilesDatabase.GetInstance().GetUnlinkedFiles();

			foreach (MissingFile mf in a)
			{
				mf.mFoundDirecotry="Removed";
			}

			FillDataBox();
		}

        //*******************************************************************
		public void SizeColumnsToContent(DataGrid dataGrid, int nRowsToScan) 
		{
	
			DataGridTableStyle tableStyle = new DataGridTableStyle();

			tableStyle.RowHeadersVisible=false;


			DataTable dataTable = ((DataView)dataGrid.DataSource).Table;

			
			if (-1 == nRowsToScan)
			{
				nRowsToScan = dataTable.Rows.Count;
			}
			else
			{
				// Can only scan rows if they exist.
				nRowsToScan = System.Math.Min(nRowsToScan, 
					dataTable.Rows.Count);
			}

			// Clear any existing table styles.
			dataGrid.TableStyles.Clear();

			// Use mapping name that is defined in the data source.
			tableStyle.MappingName = dataTable.TableName;

			// Now create the column styles within the table style.
			

			DataGridTextBoxColumn columnStyle;
			int iWidth;

			for (int iCurrCol = 0; iCurrCol < dataTable.Columns.Count; 
				iCurrCol++)
			{

				DataColumn dataColumn = dataTable.Columns[iCurrCol];

				columnStyle = new DataGridTextBoxColumn();

				columnStyle.TextBox.Enabled = true;
				columnStyle.HeaderText = dataColumn.ColumnName;
				columnStyle.MappingName = dataColumn.ColumnName;

				if (iCurrCol==0)
				{
					iWidth = 450;
				}
				else
				{
					iWidth=4000;
				}

				columnStyle.ReadOnly=true;
			
				// Change width, if data width is wider than header text width.
				// Check the width of the data in the first X rows.
			
				columnStyle.Width = iWidth + 4;

				// Add the new column style to the table style.
				tableStyle.GridColumnStyles.Add(columnStyle);
			}    
			// Add the new table style to the data grid.
			dataGrid.TableStyles.Add(tableStyle);
			
		}
	}
}
