using IntegrationSys.CommandLine;
using IntegrationSys.LogUtil;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IntegrationSys.Phone
{
	internal class FileTransferCmd
	{
		public const int TRANSFER_TYPE_PULL = 0;

		public const int TRANSFER_TYPE_PUSH = 1;

		public const int TRANSFER_ERROR_NONE = 0;

		public const int TRANSFER_ERROR_FILE_NOT_EXIST = 1;

		public const int TRANSFER_ERROR_NETWORK = 2;

		private const string LOOPBACK_IP = "127.0.0.1";

		private const int PORT = 6661;

		private string ip_;

		private int port_;

		public FileTransferCmd(string ip)
		{
			this.ip_ = ip;
			this.port_ = 6661;
			if (ip == "127.0.0.1")
			{
				string text;
				AdbCommand.ExecuteAdbCommand("forward tcp:6661 tcp:6661", out text);
			}
		}

		public int Pull(string srcfilename, string destfilename)
		{
			try
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					Log.Debug(string.Concat(new object[]
					{
						"connect ip = ",
						this.ip_,
						" port = ",
						this.port_,
						" start"
					}));
					tcpClient.Connect(this.ip_, this.port_);
					Log.Debug(string.Concat(new object[]
					{
						"connect ip = ",
						this.ip_,
						" port = ",
						this.port_,
						" end"
					}));
					using (NetworkStream stream = tcpClient.GetStream())
					{
						BinaryWriter binaryWriter = new BinaryWriter(stream);
						int host = 264;
						binaryWriter.Write(IPAddress.HostToNetworkOrder(host));
						binaryWriter.Write(IPAddress.HostToNetworkOrder(0));
						byte[] bytes = Encoding.UTF8.GetBytes(srcfilename);
						binaryWriter.Write(bytes);
						byte[] buffer = new byte[256 - bytes.Length];
						binaryWriter.Write(buffer);
						Log.Debug("write finish");
						using (FileStream fileStream = new FileStream(destfilename, FileMode.Create))
						{
							Log.Debug("create dest file");
							BinaryReader binaryReader = new BinaryReader(stream);
							Log.Debug("create binary reader");
							int num = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
							Log.Debug("read totalBytes = " + num);
							int i = 4;
							byte[] buffer2 = new byte[4096];
							while (i < num)
							{
								int num2 = binaryReader.Read(buffer2, 0, 4096);
								fileStream.Write(buffer2, 0, num2);
								i += num2;
								Log.Debug(string.Concat(new object[]
								{
									"TotalBytes = ",
									num,
									", readBytes = ",
									num2,
									", readTotalBytes = ",
									i
								}));
							}
						}
					}
				}
			}
			catch (Exception)
			{
				return 1;
			}
			return 0;
		}

		public int Push(string srcfilename, string destfilename)
		{
			try
			{
				using (FileStream fileStream = new FileStream(srcfilename, FileMode.Open, FileAccess.Read))
				{
					using (TcpClient tcpClient = new TcpClient())
					{
						tcpClient.Connect(this.ip_, this.port_);
						using (NetworkStream stream = tcpClient.GetStream())
						{
							BinaryWriter binaryWriter = new BinaryWriter(stream);
							int host = 264 + (int)fileStream.Length;
							binaryWriter.Write(IPAddress.HostToNetworkOrder(host));
							binaryWriter.Write(IPAddress.HostToNetworkOrder(1));
							byte[] bytes = Encoding.UTF8.GetBytes(destfilename);
							binaryWriter.Write(bytes);
							byte[] buffer = new byte[256 - bytes.Length];
							binaryWriter.Write(buffer);
							byte[] array = new byte[fileStream.Length];
							fileStream.Read(array, 0, array.Length);
							binaryWriter.Write(array);
							BinaryReader binaryReader = new BinaryReader(stream);
							int num = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
							if (num != 1)
							{
								int result = 1;
								return result;
							}
						}
					}
				}
			}
			catch (Exception)
			{
				int result = 1;
				return result;
			}
			return 0;
		}
	}
}
