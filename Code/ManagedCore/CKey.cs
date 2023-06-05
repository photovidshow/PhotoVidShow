using System;
using System.Security.Permissions;
namespace ManagedCore.License
{
	/// <summary>
	/// Summary description for CKey.
	/// </summary>\
	/// 

	[StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey = MyPublicKey.Value)]

	public class CKey
	{
		protected string mKeyString;

		public string KeySyring
		{
			get
			{
				return mKeyString;
			}
		}
		

		protected string GenerateHashString(string unique_string, int key_number, int software_product)
		{
			string pre_string = unique_string;
			string name1 = System.String.Format(pre_string+"(SquidgySoft)Num{0:D5}Prd{1:D3}", key_number, software_product);
			string hash_string = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(name1, "md5");    // ### SRG FIX ME
			return hash_string;
		}


		public CKey(string key_string)
		{
            // allow user to enter G instead of 0 and H instead of 1 ( i.e not to get 
            // confused with letter o's and l's )
            key_string =key_string.Replace('G', '0');
            key_string =key_string.Replace('H', '1');
			mKeyString=key_string;
		}

		public CKey()
		{
		}

		public void Load(string file)
		{
			System.IO.StreamReader reader = new System.IO.StreamReader(file);
			this.mKeyString = reader.ReadLine();
			reader.Close();
		}

		public void Save(string file)
		{
			System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
			writer.WriteLine(this.mKeyString);
			writer.Close();
		}

		public int GetOriginalKeyNumber(string unique_string, int product)
		{
			for (int i=0;i<10000;i++)
			{
				string cand = GenerateHashString(unique_string,i,product);
				if (cand==this.mKeyString)
				{
					return i;
				}
			}
			
			return 0;
		}
	}

}
