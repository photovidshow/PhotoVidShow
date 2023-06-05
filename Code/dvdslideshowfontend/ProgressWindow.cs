using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ManagedCore.Progress;
using DVDSlideshow;

namespace dvdslideshowfontend
{
	/// <summary>
	/// Summary description for ProgressWindow.
	/// </summary>
	public class ProgressWindow : System.Windows.Forms.Form, IProgressCallback
	{
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.ProgressBar progressBar;

		public bool safe_to_kill=false;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public delegate void SetTextInvoker(String text);
		public delegate void IncrementInvoker( int val );
		public delegate void StepToInvoker( int val );
		public delegate void RangeInvoker( int minimum, int maximum );

		private String titleRoot = "";
		private System.Threading.ManualResetEvent initEvent = new System.Threading.ManualResetEvent(false);
		private System.Threading.ManualResetEvent abortEvent = new System.Threading.ManualResetEvent(false);
		public System.Windows.Forms.Button CancelButtons;
		private bool requiresClose = true;
        private bool mEnd = false;
        private bool mCancelled = false;

        public bool Cancelled
        {
            get { return mCancelled;  }
        }

     //   private IntPtr handle;

        public delegate void ProgressWindowStartMenthodDelegate( object o);

 

        private ProgressWindowStartMenthodDelegate startDelegate = null;
        private object startDelegateObject = null;

        public object StartDelegateObject
        {
            set { startDelegateObject = value; }
            get { return startDelegateObject; }
        }

        public ProgressWindow(Form parent_window, ProgressWindowStartMenthodDelegate del, object delobj)
        {
            mCancelled = false;
            Rectangle pos = parent_window.Bounds;

            Location = new Point(pos.X + pos.Width / 2 - 190, pos.Y + pos.Height / 2 - 30);
            InitializeComponent();

            startDelegate = del;
            startDelegateObject = delobj;

            this.Shown +=new EventHandler(ProgressWindow_Shown);
        }


        protected void ProgressWindow_Shown(object sender, EventArgs e)
        {
            if (startDelegate != null)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(startDelegate), startDelegateObject);

            }

        }

        public ProgressWindow(ProgressWindowStartMenthodDelegate del, object delobj)
        {
            //
            // Required for Windows Form Designer support

            InitializeComponent();
            mCancelled = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            startDelegate = del;
            startDelegateObject = delobj;

            this.Shown += new EventHandler(ProgressWindow_Shown);
        }

		#region Implementation of IProgressCallback
		/// <summary>
		/// Call this method from the worker thread to initialize
		/// the progress meter.
		/// </summary>
		/// <param name="minimum">The minimum value in the progress range (e.g. 0)</param>
		/// <param name="maximum">The maximum value in the progress range (e.g. 100)</param>
		public void Begin( int minimum, int maximum )
		{
            try
            {
                initEvent.WaitOne();
                Invoke(new RangeInvoker(DoBegin), new object[] { minimum, maximum });
            }
            catch
            {
            }
		}

		/// <summary>
		/// Call this method from the worker thread to initialize
		/// the progress callback, without setting the range
		/// </summary>
		public void Begin()
		{
            try
            {
                initEvent.WaitOne();
                Invoke(new MethodInvoker(DoBegin));
            }
            catch
            {
            }
		}

		/// <summary>
		/// Call this method from the worker thread to reset the range in the progress callback
		/// </summary>
		/// <param name="minimum">The minimum value in the progress range (e.g. 0)</param>
		/// <param name="maximum">The maximum value in the progress range (e.g. 100)</param>
		/// <remarks>You must have called one of the Begin() methods prior to this call.</remarks>
		public void SetRange( int minimum, int maximum )
		{
			initEvent.WaitOne();
            if (mCancelled == false)
            {
                Invoke(new RangeInvoker(DoSetRange), new object[] { minimum, maximum });
            }
		}

		/// <summary>
		/// Call this method from the worker thread to update the progress text.
		/// </summary>
		/// <param name="text">The progress text to display</param>
		public void SetText( String text )
		{
            if (mCancelled == false)
            {
                Invoke(new SetTextInvoker(DoSetText), new object[] { text });
            }
		}

		/// <summary>
		/// Call this method from the worker thread to increase the progress counter by a specified value.
		/// </summary>
		/// <param name="val">The amount by which to increment the progress indicator</param>
		public void Increment( int val )
		{
            if (mCancelled == false)
            {
                Invoke(new IncrementInvoker(DoIncrement), new object[] { val });
            }
		}

		/// <summary>
		/// Call this method from the worker thread to step the progress meter to a particular value.
		/// </summary>
		/// <param name="val"></param>
		public void StepTo( int val )
		{
            if (mCancelled == false)
            {
                Invoke(new StepToInvoker(DoStepTo), new object[] { val });
            }
		}

		
		/// <summary>
		/// If this property is true, then you should abort work
		/// </summary>
		public bool IsAborting
		{
			get
			{
				return abortEvent.WaitOne( 0, false );
			}
		}

		/// <summary>
		/// Call this method from the worker thread to finalize the progress meter
		/// </summary>
		public void End()
		{
            try
            {
                if (requiresClose)
                {
                    Invoke(new MethodInvoker(DoEnd));
                }

                safe_to_kill = true;
            }
            catch
            {
                mEnd = true;
            }
		}
		#endregion

		#region Implementation members invoked on the owner thread
		private void DoSetText( String text )
		{
			label.Text = text;
		}

		private void DoIncrement( int val )
		{
			progressBar.Increment( val );
			UpdateStatusText();
		}

		private void DoStepTo( int val )
		{
			progressBar.Value = val;
			UpdateStatusText();
		}

		private void DoBegin( int minimum, int maximum )
		{
			DoBegin();
			DoSetRange( minimum, maximum );
		}

		private void DoBegin()
		{
			//cancelButton.Enabled = true;
			//ControlBox = true;
		}

		private void DoSetRange( int minimum, int maximum )
		{
			progressBar.Minimum = minimum;
			progressBar.Maximum = maximum;
			progressBar.Value = minimum;
			titleRoot = Text;
		}

		private void DoEnd()
		{
			Close();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Handles the form load, and sets an event to ensure that
		/// intialization is synchronized with the appearance of the form.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad( e );
			ControlBox = false;
			initEvent.Set();
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

		/// <summary>
		/// Handler for 'Close' clicking
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			requiresClose = false;
			AbortWork();
			base.OnClosing( e );
		}
		#endregion
		
		#region Implementation Utilities
		/// <summary>
		/// Utility function that formats and updates the title bar text
		/// </summary>
		private void UpdateStatusText()
		{
			Text = titleRoot + String.Format( " - {0}% complete", (progressBar.Value * 100 ) / (progressBar.Maximum - progressBar.Minimum) );
		}
		
		/// <summary>
		/// Utility function to terminate the thread
		/// </summary>
		private void AbortWork()
		{
			abortEvent.Set();
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label = new System.Windows.Forms.Label();
            this.CancelButtons = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(8, 24);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(360, 23);
            this.progressBar.TabIndex = 1;
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label.Location = new System.Drawing.Point(8, 8);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(3058, 16);
            this.label.TabIndex = 0;
            this.label.Text = "Starting operation...";
            // 
            // CancelButtons
            // 
            this.CancelButtons.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.CancelButtons.Location = new System.Drawing.Point(296, 56);
            this.CancelButtons.Name = "CancelButtons";
            this.CancelButtons.Size = new System.Drawing.Size(75, 23);
            this.CancelButtons.TabIndex = 2;
            this.CancelButtons.Text = "Cancel";
            this.CancelButtons.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ProgressWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(376, 86);
            this.ControlBox = false;
            this.Controls.Add(this.CancelButtons);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ProgressWindow";
            this.Load += new System.EventHandler(this.ProgressWindow_Load);
            this.Closed += new System.EventHandler(this.ProgressWindow_Closed);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ProgressWindow_Closing);
            this.ResumeLayout(false);

		}
		#endregion

		private void ProgressWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			
		}

		private void ProgressWindow_Closed(object sender, System.EventArgs e)
		{
			
		}

		private void ProgressWindow_Load(object sender, System.EventArgs e)
		{
		
		}

		private void CancelButton_Click(object sender, System.EventArgs e)
		{
            mCancelled = true;
            this.Close();
		}

        private void ShowDialogThreadStart(object o)
        {
            if (mEnd == true) return;
            ShowDialog();
        }

        public void ShowDialogInOwnThread()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ShowDialogThreadStart));
        }

	}
}
