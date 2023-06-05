using System;
using System.IO;
using System.Security.Permissions;
namespace ManagedCore.License
{
	[StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey = MyPublicKey.Value)]

	public class CActivateKey : CKey
	{

		public CActivateKey(string key_string) : base(key_string)
		{
		}

		/*
		public CActivateKey(int key_number, int software_product)
		{
		
			this.mKeyString = GenerateHashString("",key_number, software_product);
			Console.WriteLine(mKeyString);

		}
		*/


		// is valid formation.  can still be invalid if map file on internet has blocked code
		public bool IsValid(int product, ref bool could_not_connect)
		{
			for (int i=0;i<10000;i++)
			{
				string cand = GenerateHashString("",i,product);
				if (cand==this.mKeyString)
				{
					bool map_valid = IsValidInMapFile(product, ref could_not_connect);
					if (map_valid==true)
					{
						return true;
					}
					return false;
				}
			}

			return false;
		}
				
		private bool IsValidInMapFile(int product, ref bool could_not_connect)
		{
            string user_folder =  Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToString();

			ManagedCore.CNetFile net_file = new ManagedCore.CNetFile("MapFile1.htm");
            string local_copy = user_folder + "\\MapFile1.dat";
			ManagedCore.IO.ForceDeleteIfExists(local_copy,false);
			bool ok = net_file.CopyToHarddrive(local_copy, false);
			if (ok==false)
			{
				could_not_connect=true;
				ManagedCore.IO.ForceDeleteIfExists(local_copy,false);
				return false;
			}

			CMapFile mf = CMapFile.FromFile(local_copy);
			if (mf==null) 
			{
				ManagedCore.IO.ForceDeleteIfExists(local_copy,false);
				return false;
			}

			int orginal_key_num = this.GetOriginalKeyNumber("",1);
			int val =mf.GetValue(orginal_key_num);
			if (val==0) 
			{
				ManagedCore.IO.ForceDeleteIfExists(local_copy,false);
				return true;
			}

			ManagedCore.IO.ForceDeleteIfExists(local_copy,false);
			return false;
		}


		private void NotityRegistrationToNetFile()
		{
		}

		
		public CSoftwareKey GenerateSoftkey(string mac_address, int product)
		{
			int key_number = GetOriginalKeyNumber("",product);
			return new CSoftwareKey(mac_address, key_number,product);
		}
		
		public CSoftwareKey GenerateSoftkey(CMacAddress address, int product)
		{
			string one_to_use ="123456789";

			if (address.mMacAddressStrings.Count>0)
			{
				one_to_use=(string) address.mMacAddressStrings[address.mMacAddressStrings.Count-1];
			}

			return GenerateSoftkey(one_to_use,product);
		}
		
	}
}
