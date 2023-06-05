using System;
using System.Collections;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace ManagedCore
{
	public class CNetFile
	{
        private enum ConnectionToInternetResult
        {
            failedToConnect =0,
            failedMoreThanOnceIn30Seconds=1,
            success=2
        }

		public static  ArrayList mUpdateURLList = new ArrayList();
		private static System.Net.WebClient mWebClient;
		private string mNetFilename;
        private DownloadFileForm mDownloadFileForm;
        private bool mAsyncDownloadSuccess = false;
        private Exception mAsyncDownloadError = null;
        private static int mShownNeedToConnectToInterenetError = 0;
		
		public CNetFile(string filename)
		{
			mNetFilename=filename;
	
		}

        private static DateTime mLastInternetCheck = new DateTime();
        private CheckingInternetForm cfiform = null;

        private void CheckForInternetInOwnThread()
        {
			mWebClient = new WebClient();

			try
			{
				// Initialize the WebRequest.
				WebRequest myRequest = WebRequest.Create("http://www.google.com");
				myRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
				// Return the response. 
				WebResponse myResponse = myRequest.GetResponse();	
				myResponse.Close();

				mWebClient.BaseAddress = (string)mUpdateURLList[0];
			}
			catch (Exception )
			{
				mWebClient = null;
			}

            cfiform.DeclareFinishedChecking();
		}

        //*******************************************************************
        private ConnectionToInternetResult ConnectToLatestServer()
        {
            if (mUpdateURLList.Count < 1) return ConnectionToInternetResult.failedToConnect;

            if (mWebClient != null) return ConnectionToInternetResult.success;

            TimeSpan timeSinceLastCheck = DateTime.Now - mLastInternetCheck;
            if (timeSinceLastCheck < new TimeSpan(0, 0, 30))
            {
                return ConnectionToInternetResult.failedMoreThanOnceIn30Seconds;
            }

            mLastInternetCheck = DateTime.Now;

            cfiform = null;

            cfiform = new CheckingInternetForm();

            System.Threading.ThreadStart myThreadStart = new System.Threading.ThreadStart(CheckForInternetInOwnThread);
            System.Threading.Thread thres = new System.Threading.Thread(myThreadStart);
            thres.Start();

            cfiform.ShowDialog();
            cfiform = null;

            if (mWebClient == null)
            {
                return ConnectionToInternetResult.failedToConnect;
            }

            return ConnectionToInternetResult.success;
        }

		public void CallWebCGIScript(string cgi_script, string request_string)
		{
			if (ConnectToLatestServer() != ConnectionToInternetResult.success) return;
		
			try
			{
				int current =0;
			
				if (mUpdateURLList.Count==0) return ;
				// keep trying through all the servers listed
				while (current < mUpdateURLList.Count)
				{
					try
					{
						string url =(string)mUpdateURLList[current]+"/cgi-bin/"+cgi_script+"?"+request_string;
						WebRequest wr = WebRequest.Create(url);
						WebResponse wresp=(HttpWebResponse)wr.GetResponse();		
						wresp.Close();
					}
					catch
					{
					}
					current++;
				}

			}
			catch
			{
				return ;
			}

			return ;
			
		}
		

        // ***************************************************************************************************
        private void ShowFailedToDownMediaWindow(string net_f, string sub_message)
        {
            // Only show this error 10 times per session, can gvet annoying otherwise, or situations where this is repeatedly called
            if (mShownNeedToConnectToInterenetError < 10)
            {
                mShownNeedToConnectToInterenetError++;
                UserMessage.Show("Could not download '" + Path.GetFileName(net_f) + "' from the PhotoVidShow web site.  This media file does not exist " +
                "on the local computer and needs to be downloaded.  Please ensure internet is available on this computer and no firewall is blocking this application.\r\n\r\n" + sub_message, "Download error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        
        // ***************************************************************************************************
      	public bool CopyToHarddrive(string dest_dir, bool asyncWithprogressWindow)
		{
            ConnectionToInternetResult result = ConnectToLatestServer();
            if (result != ConnectionToInternetResult.success)
            {
                if (asyncWithprogressWindow == true)
                {
                    if (result == ConnectionToInternetResult.failedToConnect)
                    {
                        UserMessage.Show("Could not connect to the internet to download media file", "Download error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                    else
                    {
                        ShowFailedToDownMediaWindow(mNetFilename, "");
                    }
                }
                return false;
            }

            bool downloaded = false;

			try
			{
				string dest_file = dest_dir;

				string net_f = mNetFilename.ToLower();
				if (net_f.EndsWith(".exe")==true)
				{
					net_f = net_f.Substring(0,net_f.Length-4);
					net_f = net_f +".sgs" ;
				}

				int current =0;
			
				if (mUpdateURLList.Count==0) return false;

				// keep trying through all the servers listed
				while (current < mUpdateURLList.Count &&
					downloaded ==false)
				{

					mWebClient.BaseAddress = (string)mUpdateURLList[current];

					try
					{
                        if (asyncWithprogressWindow == false)
                        {
                            mWebClient.DownloadFile(net_f, dest_file);
                            downloaded = true;
                        }
                        else
                        {
                            mAsyncDownloadSuccess = false;
                            mAsyncDownloadError = null;

                            Uri address = new Uri(mWebClient.BaseAddress + net_f);
                            mDownloadFileForm = new DownloadFileForm(mWebClient.CancelAsync);
                            mDownloadFileForm.Text = "Downloading '" + Path.GetFileNameWithoutExtension(net_f) + "' media file";
                            mWebClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(mWebClient_DownloadFileCompleted);
                            mWebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(mWebClient_DownloadProgressChanged);
                            mWebClient.DownloadFileAsync(address, dest_file);

                            mDownloadFileForm.ShowDialog();
                            downloaded =mAsyncDownloadSuccess;

                            if (downloaded == false)
                            {
                                IO.ForceDeleteIfExists(dest_file, false);
                            }

                            if (mAsyncDownloadError!=null)
                            {
                                ShowFailedToDownMediaWindow(net_f, mAsyncDownloadError.Message);
                            }
                        }
						
					}
					catch (Exception e2)
					{
                        Log.Error("Exception when downloading file:"+e2.Message);
					}
					current++;
				}

			}
			catch
			{
				return false;
			}

            return downloaded;
		}

        // ************************************************************************************************************************
        private void mWebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            mDownloadFileForm.SetProgressPercent(e.ProgressPercentage);
        }

        // ************************************************************************************************************************
        private void mWebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == false && e.Error == null)
            {
                mAsyncDownloadSuccess = true;
            }
            else if ( e.Error != null && e.Cancelled==false)
            {
                mAsyncDownloadError = e.Error;
            }

            mDownloadFileForm.Close();
        }
	}

}
