using IntegrationSys.LogUtil;
using System;
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
				string @string = Encoding.ASCII.GetString(array, 0, num);
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
				}
				else if (@string.StartsWith("PickPlace") && this.PickPlaceEvent != null)
				{
					this.PickPlaceEvent();
				}
				byte[] array2 = new byte[4];
				stream.Write(array2, 0, array2.Length);
			}
			stream.Close();
			tcpClient.Close();
		}
	}
}
