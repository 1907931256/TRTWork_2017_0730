using System;
using System.Linq;
using System.Net;

namespace IntegrationSys.Net
{
	internal static class NetUtil
	{
		public const int PORT_FILE_TRANSFER_SERVER = 10106;

		public const int PORT_LITE_DATA_SERVER = 10108;

		public static string LocalIp()
		{
			return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault((IPAddress a) => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
		}

		public static string GetStationIp(int index)
		{
			string result = "0.0.0.0";
			switch (index)
			{
			case 0:
				result = "192.168.0.101";
				break;
			case 1:
				result = "192.168.0.102";
				break;
			case 2:
				result = "192.168.0.103";
				break;
			case 3:
				result = "192.168.0.104";
				break;
			case 4:
				result = "192.168.0.105";
				break;
			case 5:
				result = "192.168.0.106";
				break;
			}
			return result;
		}

		public static int GetStationIndex()
		{
			string a = NetUtil.LocalIp();
			int result = 0;
			if (a == "192.168.0.101")
			{
				result = 0;
			}
			else if (a == "192.168.0.102")
			{
				result = 1;
			}
			else if (a == "192.168.0.103")
			{
				result = 2;
			}
			else if (a == "192.168.0.104")
			{
				result = 3;
			}
			else if (a == "192.168.0.105")
			{
				result = 4;
			}
			else if (a == "192.168.0.106")
			{
				result = 5;
			}
			return result;
		}
	}
}
