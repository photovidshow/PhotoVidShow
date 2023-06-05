using System;
using System.Collections;
using ManagedCore;

namespace DVDSlideshow
{
	/// <summary>
    /// Summary description for CMenuButtonStyleDatabase.
	/// </summary>
	public class CMenuButtonStyleDatabase
	{
		private static  CMenuButtonStyleDatabase mInstance;
	
		public static CMenuButtonStyleDatabase GetInstance()
		{
			if (mInstance==null)
			{
				mInstance = new CMenuButtonStyleDatabase();
			}
			return mInstance;
		}


		//*******************************************************************
		//*******************************************************************
		//*******************************************************************

		private ArrayList mEntries ;

		
		//*******************************************************************
		private CMenuButtonStyleDatabase()
		{
			BuildDatabase();
		}


		//*******************************************************************
		private void BuildDatabase()
		{
			mEntries = new ArrayList();


			string prv = CGlobals.GetRootDirectory();

			for (int i=1;i<93;i++)
			{
				try 
				{
					string frame_name = prv+"\\MenuButtonStyles\\frame"+i+".png";
					string frame_mask = prv+"\\MenuButtonStyles\\framemask"+i+".png";

                    CMenuButtonStyle style = new CMenuButtonStyle(frame_name, frame_mask);
					mEntries.Add(style);
				}
				catch 
				{
				}

			}
		}
	
		//*******************************************************************
		public CMenuButtonStyle	GetStyle(int index)
		{
			if (index > mEntries.Count-1)
			{
				CDebugLog.GetInstance().Error("Entry "+index+" not in menu button style database");
				index =0;
			}

            return (CMenuButtonStyle)mEntries[index];
		}


		//*******************************************************************
		public int	GetNumStyles()
		{
			return mEntries.Count;
		}

	}
}
