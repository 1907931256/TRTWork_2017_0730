using System;
using System.Collections.Generic;

namespace IntegrationSys.Net
{
	internal class IpContainer
	{
		private const int MAX_SIZE = 8;

		private List<string> ips_;

		public void Add(string ip)
		{
			if (this.ips_ == null)
			{
				this.ips_ = new List<string>();
			}
			if (this.ips_.Count >= 8)
			{
				this.ips_.RemoveAt(0);
			}
			this.ips_.Add(ip);
		}

		public string Get(int index)
		{
			string result = string.Empty;
			if (this.ips_ != null && this.ips_.Count > index)
			{
				result = this.ips_[index];
			}
			return result;
		}

		public int Size()
		{
			int result = 0;
			if (this.ips_ != null)
			{
				result = this.ips_.Count;
			}
			return result;
		}
	}
}
