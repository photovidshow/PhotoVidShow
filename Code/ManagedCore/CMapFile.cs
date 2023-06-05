using System;
using System.IO;
using System.Security.Permissions;
namespace ManagedCore.License
{
	/// <summary>
	/// Summary description for CMapFile.
	/// </summary>
	/// 
	[StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey = MyPublicKey.Value)]


	public class CMapFile
	{
		private byte[] mData;

		private CMapFile()
		{
		}

		public static CMapFile FromFile(string filename)
		{
			CMapFile mf = new CMapFile();
			try
			{
				StreamReader reader = new System.IO.StreamReader(filename);

				byte[] data = ManagedCore.IO.ReadFully(reader.BaseStream,1250);

				reader.Close();

				if (data==null) return null;
				mf.MangleData(data);
				mf.mData = data;

				return mf;

				
			}
			catch
			{
				return null;
			}
		}

		public void Save(string filename)
		{
			MangleData(this.mData);
			System.IO.StreamWriter sw = new StreamWriter(filename);
			for (int i=0;i<mData.Length;i++)
			{
				sw.BaseStream.WriteByte(mData[i]);
			}
			sw.Close();
			MangleData(this.mData);
		}


		public int GetValue(int number)
		{
			int b = number >> 3;
			if (b> mData.Length) return 1;

			int d = mData[b];

			int d1 = number - (b<<3);

			int b2 =1;
			int shift=0;
			while (d1 > 0)
			{
				b2 =b2<<1;
				d1--;
				shift++;
			}

			int val = d & b2;
			val = val >> shift;

			return val;
		}

		public void SetValue(int number, int to_val)
		{
			int b = number >> 3;
			if (b> mData.Length) return ;

			int d = mData[b];

			int d1 = number - (b<<3);

			int b2 =1;
			int shift=0;
			while (d1 > 0)
			{
				b2 =b2<<1;
				d1--;
				shift++;
			}

			b2 =~b2;
			to_val <<= shift;
		
			int s2 = b2 & d;
			s2|= to_val;

			mData[b] =(byte)s2;
		}

		private void MangleData(byte[] data)
		{
			int j=69;

			for (int i=0;i<data.Length;i++)
			{
				byte s = data[i];
				s^= (byte)j;
				data[i] = s;
				j+=35;
				j = j%256;
			}
		}

		static public void GenerateBlank(string filename)
		{
			CMapFile mf = new CMapFile();
			mf.mData =new byte[1250];
			mf.MangleData(mf.mData);

			System.IO.StreamWriter sw = new StreamWriter(filename);
			for (int i=0;i<mf.mData.Length;i++)
			{
				sw.BaseStream.WriteByte(mf.mData[i]);
			}
			sw.Close();
		}

		public void BanNumber(int i)
		{
			SetValue(i,1);
		}

		public void UnBanNumber(int i)
		{
			SetValue(i,0);
		}


	}
}
