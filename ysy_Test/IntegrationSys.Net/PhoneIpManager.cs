using System;
using System.IO;

namespace IntegrationSys.Net
{
	internal class PhoneIpManager
	{
		private static PhoneIpManager instance_;

		private IpContainer ips_;

		public static PhoneIpManager Instance
		{
			get
			{
				if (PhoneIpManager.instance_ == null)
				{
					PhoneIpManager.instance_ = new PhoneIpManager();
				}
				return PhoneIpManager.instance_;
			}
		}

		private PhoneIpManager()
		{
			this.Init();
		}

		public void Add(string ip)
		{
			if (this.ips_.Size() > 0 && this.ips_.Get(this.ips_.Size() - 1) == ip)
			{
				return;
			}
			this.ips_.Add(ip);
			this.Save();
		}

		private void Init()
		{
			this.ips_ = new IpContainer();
			string path = "IpList.txt";
			try
			{
				using (StreamReader streamReader = new StreamReader(path))
				{
					string ip;
					while ((ip = streamReader.ReadLine()) != null)
					{
						this.ips_.Add(ip);
					}
				}
			}
			catch (FileNotFoundException)
			{
			}
		}

		private void Save()
		{
			string path = "IpList.txt";
			using (StreamWriter streamWriter = new StreamWriter(path, false))
			{
				for (int i = 0; i < this.ips_.Size(); i++)
				{
					streamWriter.WriteLine(this.ips_.Get(i));
				}
			}
		}
	}
}
