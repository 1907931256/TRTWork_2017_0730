using IntegrationSys.Equipment;
using IntegrationSys.LogUtil;
using Newtonsoft.Json;
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
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				byte[] array = new byte[1024];
				int num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					Encoding.UTF8.GetString(array, 0, num);
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
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				byte[] array = new byte[1024];
				int num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					Encoding.UTF8.GetString(array, 0, num);
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
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				byte[] array = new byte[1024];
				int num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					Encoding.UTF8.GetString(array, 0, num);
					result = true;
				}
			}
			tcpClient.Close();
			return result;
		}

		public string GetPhoneIP(int index)
		{
			string result = string.Empty;
			TcpClient tcpClient = new TcpClient();
			tcpClient.Connect(IPAddress.Parse(NetUtil.GetStationIp(0)), 10108);
			using (NetworkStream stream = tcpClient.GetStream())
			{
				string s = "TargetIp " + index;
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				byte[] array = new byte[1024];
				int num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					result = Encoding.UTF8.GetString(array, 0, num);
				}
			}
			tcpClient.Close();
			return result;
		}

		public string SendEquipmentCmd(int index, string action, string paramter)
		{
			string result = string.Empty;
			TcpClient tcpClient = new TcpClient();
			tcpClient.Connect(IPAddress.Parse(NetUtil.GetStationIp(index)), 10108);
			using (NetworkStream stream = tcpClient.GetStream())
			{
				string s = JsonConvert.SerializeObject(new LiteData
				{
					Name = "RemoteEquipmentCmd",
					Paramters = new string[]
					{
						action,
						paramter
					}
				});
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				byte[] array = new byte[1024];
				int num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					result = Encoding.UTF8.GetString(array, 0, num);
				}
			}
			tcpClient.Close();
			return result;
		}

		public bool BroadcastPickPlace()
		{
			Log.Debug("SendPickPlace " + 0);
			this.SendPickPlace(0);
			for (int i = 0; i < 5; i++)
			{
				StationInfo stationInfo = AppInfo.EquipmentInfo.GetStationInfo(i);
				if (!stationInfo.Work)
				{
					break;
				}
				Log.Debug("SendPickPlace " + (i + 1));
				this.SendPickPlace(i + 1);
			}
			return true;
		}
	}
}
