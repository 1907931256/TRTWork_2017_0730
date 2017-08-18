using IntegrationSys.Equipment;
using IntegrationSys.LogUtil;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IntegrationSys.Net
{
	internal class LiteDataServer
	{
		public delegate void InplaceEventHandler(int index);

		public delegate void CompleteEventHandler(int index);

		public delegate void PickPlaceEventHandler();

		private static LiteDataServer instance_;

		private TcpListener server;

		private bool exit;

		public event LiteDataServer.InplaceEventHandler InplaceEvent;

		public event LiteDataServer.CompleteEventHandler CompleteEvent;

		public event LiteDataServer.PickPlaceEventHandler PickPlaceEvent;

		public static LiteDataServer Instance
		{
			get
			{
				if (LiteDataServer.instance_ == null)
				{
					LiteDataServer.instance_ = new LiteDataServer();
				}
				return LiteDataServer.instance_;
			}
		}

		private LiteDataServer()
		{
		}

		public void Start()
		{
			this.exit = false;
			IPAddress localaddr = IPAddress.Parse(NetUtil.LocalIp());
			this.server = new TcpListener(localaddr, 10108);
			this.server.Start();
			while (!this.exit)
			{
				TcpClient state = this.server.AcceptTcpClient();
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadProc), state);
			}
		}

		public void Stop()
		{
			this.exit = true;
			this.server.Stop();
		}

		private void ThreadProc(object state)
		{
			TcpClient tcpClient = (TcpClient)state;
			NetworkStream stream = tcpClient.GetStream();
			byte[] array = new byte[1024];
			int num = stream.Read(array, 0, array.Length);
			if (num > 0)
			{
				string @string = Encoding.UTF8.GetString(array, 0, num);
				Log.Debug("LiteDataServer receive " + @string);
				if (@string.StartsWith("Inplace"))
				{
					int num2 = @string.IndexOf(' ');
					if (num2 != -1)
					{
						string s = @string.Substring(num2 + 1);
						int index = int.Parse(s);
						if (this.InplaceEvent != null)
						{
							this.InplaceEvent(index);
						}
					}
					byte[] array2 = new byte[4];
					stream.Write(array2, 0, array2.Length);
				}
				else if (@string.StartsWith("Complete"))
				{
					int num3 = @string.IndexOf(' ');
					if (num3 != -1)
					{
						string s2 = @string.Substring(num3 + 1);
						int index2 = int.Parse(s2);
						if (this.CompleteEvent != null)
						{
							this.CompleteEvent(index2);
						}
					}
					byte[] array3 = new byte[4];
					stream.Write(array3, 0, array3.Length);
				}
				else if (@string.StartsWith("PickPlace"))
				{
					if (this.PickPlaceEvent != null)
					{
						this.PickPlaceEvent();
					}
					byte[] array4 = new byte[4];
					stream.Write(array4, 0, array4.Length);
				}
				else if (@string.StartsWith("TargetIp"))
				{
					int num4 = @string.IndexOf(' ');
					if (num4 != -1)
					{
						string s3 = @string.Substring(num4 + 1);
						int index3 = int.Parse(s3);
						string ip = LiteDataServer.GetIp(index3);
						Log.Debug("phone ip = " + ip);
						byte[] bytes = Encoding.UTF8.GetBytes(ip);
						stream.Write(bytes, 0, bytes.Length);
					}
				}
				else
				{
					LiteData liteData = JsonConvert.DeserializeObject<LiteData>(@string);
					if (liteData.Name == "RemoteEquipmentCmd" && liteData.Paramters != null && liteData.Paramters.Length >= 2)
					{
						string s4;
						EquipmentCmd.Instance.SendCommand(liteData.Paramters[0], liteData.Paramters[1], out s4);
						byte[] bytes2 = Encoding.UTF8.GetBytes(s4);
						stream.Write(bytes2, 0, bytes2.Length);
					}
				}
			}
			stream.Close();
			tcpClient.Close();
		}

		private static string GetIp(int index)
		{
			string result = "";
			using (StreamReader streamReader = new StreamReader("IpList.txt"))
			{
				ArrayList arrayList = new ArrayList();
				string value;
				while ((value = streamReader.ReadLine()) != null)
				{
					arrayList.Add(value);
				}
				if (arrayList.Count >= index)
				{
					result = (string)arrayList[arrayList.Count - index];
				}
			}
			return result;
		}
	}
}
