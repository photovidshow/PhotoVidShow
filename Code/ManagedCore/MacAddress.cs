using System;
using System.Web.Security;
using System.Management;
using System.Collections;
using System.Security.Permissions;
namespace ManagedCore
{
	[StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey = MyPublicKey.Value)]

	public class CMacAddress
	{
		public ArrayList mMacAddressStrings = new ArrayList();
		public CMacAddress()
		{
			ManagementObjectSearcher query = null; 
			ManagementObjectCollection queryCollection = null; 

			try 
			{ 
				query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration") ; 

				queryCollection = query.Get(); 

				string macAdd ="";

				foreach( ManagementObject mo in queryCollection ) 
				{ 
					try
					{
                        object obj = mo["MACAddress"];
                        if (obj != null)
                        {
                            macAdd = obj.ToString();
                            if (macAdd != "")
                            {
                                mMacAddressStrings.Add(macAdd);
                                //	Console.WriteLine("Found mac address "+ macAdd); 
                                macAdd = "";
                            }
                        }
					}
					catch
					{
					}
				} 
			} 
			catch(Exception ex) 
			{ 
				Console.WriteLine(ex.Source); 
				Console.WriteLine(ex.Message); 
			} 


		}
	}
}
