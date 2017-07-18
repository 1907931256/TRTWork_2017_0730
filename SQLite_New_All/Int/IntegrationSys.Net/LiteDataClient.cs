using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IntegrationSys.Net
{
	internal class LiteDataClient
	{
		private static LiteDataClient instance_;

		public static LiteDataClient Instance
		{
			get
			{
				if (LiteDataClient.instance_ == null)
				{
					LiteDataClient.instance_ = new LiteDataClient();
				}
				return LiteDataClient.instance_;
			}
		}

		private LiteDataClient()
		{
		}

		public bool SendInplaceFlag(int index)
		{
			bool result = false;
			TcpClient tcpClient = new TcpClient();
			tcpClient.Connect(IPAddress.Parse(NetUtil.GetStationIp(0)), 10108);
			using (NetworkStream stream = tcpClient.GetStream())
			{
				string s = "Inplace " + index;
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				byte[] array = new byte[1024];
				int num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					Encoding.ASCII.GetString(array, 0, num);
					result = true;
				}
			}
			tcpClient.Close();
			return result;
		}

		public bool SendCompleteFlag(int index)
		{
			bool result = false;
			TcpClient tcpClient = new TcpClient();
			tcpClient.Connect(IPAddress.Parse(NetUtil.GetStationIp(0)), 10108);
			using (NetworkStream stream = tcpClient.GetStream())
			{
				string s = "Complete " + index;
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				byte[] array = new byte[1024];
				int num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					Encoding.ASCII.GetString(array, 0, num);
					result = true;
				}
			}
			tcpClient.Close();
			return result;
		}

		public bool SendPickPlace(int index)
		{
			bool result = false;
			TcpClient tcpClient = new TcpClient();
			tcpClient.Connect(IPAddress.Parse(NetUtil.GetStationIp(index)), 10108);
			using (NetworkStream stream = tcpClient.GetStream())
			{
				string s = "PickPlace " + index;
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				byte[] array = new byte[1024];
				int num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					Encoding.ASCII.GetString(array, 0, num);
					result = true;
				}
			}
			tcpClient.Close();
			return result;
		}

		public bool BroadcastPickPlace()
		{
			for (int i = 1; i < 6; i++)
			{
				this.SendPickPlace(i);
			}
			return true;
		}
	}
}
