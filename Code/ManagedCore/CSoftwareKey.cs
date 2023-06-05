using System;
using System.Collections;
using System.Security.Permissions;
namespace ManagedCore.License
{
	/// <summary>
	/// Summary description for CSoftwareKey.
	/// </summary>
	/// 

	[StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey = MyPublicKey.Value)]

	public class CSoftwareKey : CKey
	{
		private string mCorrectMacAddress="";

		public CSoftwareKey(string key_string) : base(key_string)
		{
		}
		
		public CSoftwareKey(string unique_string, int key_number, int product)
		{
			this.mKeyString = GenerateHashString(unique_string,key_number,product);
		}

		public bool IsValidForThisComputer( int product, ref int key_num)
		{
			CMacAddress mac = new CMacAddress();
			return IsValidForComputer(mac.mMacAddressStrings, product, ref key_num);
		}

		public bool IsValidForComputer(ArrayList mac_address, int product,  ref int key_num)
		{
			for (int i1=mac_address.Count-1;i1>=0;i1--)
			{
				string mac_add = (string) mac_address[i1];

				for (int i=0;i<10000;i++)
				{
					string cand = GenerateHashString(mac_add,i,product);
					if (cand==this.mKeyString)
					{
						mCorrectMacAddress = mac_add;
						key_num = i;
						return true;
					}
				}
			}

			// try host name

			string name =System.Environment.MachineName;

			for (int i=0;i<10000;i++)
			{
				string cand = GenerateHashString(name,i,product);
				if (cand==this.mKeyString)
				{
					mCorrectMacAddress = name;
					key_num = i;
					return true;
				}
			}

			return false;
		}

		/*
		public int GetOriginalKeyNumber(int product)
		{
			// is valid yet ?
			if (mCorrectMacAddress=="")
				if (IsValidForThisComputer(product)==false) return 0;

			return GetOriginalKeyNumber(mCorrectMacAddress,product);

		}
		*/

	}

	public class License
	{
		public static bool Valid = true;
		public static bool TrialValid = false;

		// use this to find the key the software is running with null means no key i.e trial version
		public static CSoftwareKey RunningWithKey=null;
		public static int RunningKeyNum =-1;
	}

}
