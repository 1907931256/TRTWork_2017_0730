using IntegrationSys.CommandLine;
using IntegrationSys.Flow;
using IntegrationSys.LogUtil;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IntegrationSys.Phone
{
	internal class PhoneCmd : IExecutable
	{
		private const string ACTION_APK_CONNECT = "APK通信连接";

		private const string ACTION_APK_DISCONNECT = "APK通信断开";

		private const string ACTION_PUSH = "push";

		private const string ACTION_PULL = "pull";

		private const int PORT = 6666;

		private const int HEAD_SIZE = 4;

		private static PhoneCmd instance_;

		private TcpClient tcpClient_;

		private ConcurrentQueue<string> dataQueue_;

		private ConcurrentDictionary<string, RespPair> dict_;

		private AutoResetEvent sendEvent_;

		private bool sendThreadExit_;

		private bool recvThreadExit_;

		public static PhoneCmd Instance
		{
			get
			{
				if (PhoneCmd.instance_ == null)
				{
					PhoneCmd.instance_ = new PhoneCmd();
				}
				return PhoneCmd.instance_;
			}
		}

		private PhoneCmd()
		{
			this.dataQueue_ = new ConcurrentQueue<string>();
			this.dict_ = new ConcurrentDictionary<string, RespPair>();
			this.sendEvent_ = new AutoResetEvent(false);
			this.sendThreadExit_ = true;
			this.recvThreadExit_ = true;
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if (action == "APK通信连接")
			{
				this.ExecuteApkConnect(param, out retValue);
				return;
			}
			if (action == "APK通信断开")
			{
				this.ExecuteApkDisconnect(param, out retValue);
				return;
			}
			if (action == "push")
			{
				this.ExecutePush(param, out retValue);
				return;
			}
			if (action == "pull")
			{
				this.ExecutePull(param, out retValue);
				return;
			}
			string data;
			if (string.IsNullOrEmpty(param))
			{
				data = action;
			}
			else
			{
				data = action + " " + param;
			}
			this.ExecuteSendData(data, out retValue);
		}

		private void ExecuteApkConnect(string param, out string retValue)
		{
			bool flag;
			if (param == "Wifi")
			{
				flag = this.Connect(AppInfo.PhoneInfo.IP);
				AppInfo.PhoneInfo.ConnectType = 1;
			}
			else
			{
				flag = this.Connect();
				AppInfo.PhoneInfo.ConnectType = 0;
			}
			retValue = (flag ? "Res=Pass" : "Res=Fail");
		}

		private void ExecuteApkDisconnect(string param, out string retValue)
		{
			this.Disconnect();
			retValue = "Res=Pass";
		}

		private void ExecutePush(string param, out string retValue)
		{
			string ip = "127.0.0.1";
			if (AppInfo.PhoneInfo.ConnectType == 1)
			{
				ip = AppInfo.PhoneInfo.IP;
			}
			string[] array = param.Split(new char[]
			{
				' '
			});
			FileTransferCmd fileTransferCmd = new FileTransferCmd(ip);
			retValue = ((fileTransferCmd.Push(array[0], array[1]) == 0) ? "Res=Pass" : "Res=Fail");
		}

		private void ExecutePull(string param, out string retValue)
		{
			string ip = "127.0.0.1";
			if (AppInfo.PhoneInfo.ConnectType == 1)
			{
				ip = AppInfo.PhoneInfo.IP;
			}
			string[] array = param.Split(new char[]
			{
				' '
			});
			FileTransferCmd fileTransferCmd = new FileTransferCmd(ip);
			retValue = ((fileTransferCmd.Pull(array[0], array[1]) == 0) ? "Res=Pass" : "Res=Fail");
		}

		private void ExecuteSendData(string data, out string retValue)
		{
			this.dataQueue_.Enqueue(data);
			this.sendEvent_.Set();
			ManualResetEvent respEvent = new ManualResetEvent(false);
			RespPair respPair = new RespPair();
			respPair.RespEvent = respEvent;
			RespPair orAdd = this.dict_.GetOrAdd(data, respPair);
			orAdd.RespEvent.WaitOne();
			RespPair respPair2;
			if (this.dict_.TryRemove(data, out respPair2))
			{
				retValue = respPair2.Resp;
				return;
			}
			retValue = "Res=PhoneCmdConcurrentConflict";
		}

		private bool Handshake()
		{
			if (!this.tcpClient_.Connected)
			{
				return false;
			}
			if (!this.Send("TEST"))
			{
				return false;
			}
			BinaryReader binaryReader = new BinaryReader(this.tcpClient_.GetStream());
			int num = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
			byte[] bytes = binaryReader.ReadBytes(num - 4);
			string @string = Encoding.UTF8.GetString(bytes);
			return @string.Contains("Res=PASS");
		}

		private bool Connect()
		{
			string text;
			AdbCommand.ExecuteAdbCommand("forward tcp:6666 tcp:6666", out text);
			return this.Connect("127.0.0.1");
		}

		private bool Connect(string ip)
		{
			try
			{
				Log.Debug("Connect " + ip + " enter");
				this.Disconnect();
				this.tcpClient_ = new TcpClient();
				Log.Debug("Connect method called before");
				this.tcpClient_.Connect(IPAddress.Parse(ip), 6666);
				Log.Debug("Connect method called after");
				Log.Debug("Handshake before");
				if (!this.Handshake())
				{
					bool result = false;
					return result;
				}
				Log.Debug("Handshake after");
				this.StartSendThread();
				this.StartRecvThread();
				Log.Debug("Connect " + ip + " leave");
			}
			catch (Exception e)
			{
				Log.Debug("Connect exception", e);
				bool result = false;
				return result;
			}
			return true;
		}

		private void Disconnect()
		{
			this.StopSendThread();
			this.StopRecvThread();
			if (this.tcpClient_ != null)
			{
				if (this.tcpClient_.Connected)
				{
					this.tcpClient_.GetStream().Close();
				}
				this.tcpClient_.Close();
				this.tcpClient_ = null;
			}
		}

		private bool Send(string data)
		{
			if (!this.tcpClient_.Connected)
			{
				return false;
			}
			MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(data);
				int host = 4 + bytes.Length;
				binaryWriter.Write(IPAddress.HostToNetworkOrder(host));
				binaryWriter.Write(bytes);
				byte[] array = memoryStream.ToArray();
				NetworkStream stream = this.tcpClient_.GetStream();
				stream.Write(array, 0, array.Length);
			}
			return true;
		}

		private void StartSendThread()
		{
			this.sendThreadExit_ = false;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadSend));
		}

		private void StopSendThread()
		{
			this.sendThreadExit_ = true;
			this.sendEvent_.Set();
		}

		private void StartRecvThread()
		{
			this.recvThreadExit_ = false;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadRecv));
		}

		private void StopRecvThread()
		{
			this.recvThreadExit_ = true;
		}

		private void ThreadSend(object stateInfo)
		{
			while (!this.sendThreadExit_)
			{
				string data;
				if (this.dataQueue_.Count == 0)
				{
					this.sendEvent_.WaitOne();
				}
				else if (this.dataQueue_.TryDequeue(out data))
				{
					this.Send(data);
				}
			}
		}

		private void ThreadRecv(object stateInfo)
		{
			try
			{
				while (!this.recvThreadExit_)
				{
					if (this.tcpClient_ != null && this.tcpClient_.Connected)
					{
						BinaryReader binaryReader = new BinaryReader(this.tcpClient_.GetStream());
						int num = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
						byte[] bytes = binaryReader.ReadBytes(num - 4);
						string @string = Encoding.UTF8.GetString(bytes);
						int num2 = @string.IndexOf("::Rsp]");
						if (num2 != -1)
						{
							string key = @string.Substring(1, num2 - 1);
							string resp = @string.Substring(num2 + "::Rsp]".Length);
							RespPair respPair;
							if (this.dict_.TryGetValue(key, out respPair))
							{
								respPair.Resp = resp;
								respPair.RespEvent.Set();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Debug(ex.Message, ex);
			}
		}
	}
}
