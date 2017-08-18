using IntegrationSys.LogUtil;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IntegrationSys.Net
{
	internal class FileTransferServer
	{
		public const int TRANSFER_TYPE_UPLOAD = 0;

		public const int TRANSFER_TYPE_DOWNLOAD = 1;

		private TcpListener server;

		private bool exit;

		private static FileTransferServer instance_;

		public static FileTransferServer Instance
		{
			get
			{
				if (FileTransferServer.instance_ == null)
				{
					FileTransferServer.instance_ = new FileTransferServer();
				}
				return FileTransferServer.instance_;
			}
		}

		private FileTransferServer()
		{
		}

		public void Start()
		{
			this.exit = false;
			IPAddress localaddr = IPAddress.Parse(NetUtil.LocalIp());
			this.server = new TcpListener(localaddr, 10106);
			this.server.Start();
			while (!this.exit)
			{
				TcpClient state = this.server.AcceptTcpClient();
				ThreadPool.QueueUserWorkItem(new WaitCallback(FileTransferServer.ThreadProc), state);
			}
		}

		public void Stop()
		{
			this.exit = true;
			this.server.Stop();
		}

		private static void ThreadProc(object state)
		{
			TcpClient tcpClient = (TcpClient)state;
			NetworkStream stream = tcpClient.GetStream();
			BinaryReader binaryReader = new BinaryReader(stream);
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			int num = 1;
			try
			{
				int i = 0;
				int num2 = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
				i += 4;
				int num3 = IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
				i += 4;
				byte[] array = binaryReader.ReadBytes(256);
				i += 256;
				string text = Encoding.UTF8.GetString(array, 0, array.Length);
				string arg_7E_0 = text;
				char[] trimChars = new char[1];
				text = arg_7E_0.TrimEnd(trimChars);
				if (num3 == 0)
				{
					Log.Debug("FileTransferServer upload destination path = " + text);
					using (FileStream fileStream = new FileStream(text, FileMode.Create))
					{
						byte[] buffer = new byte[4096];
						while (i < num2)
						{
							int num4 = binaryReader.Read(buffer, 0, 4096);
							fileStream.Write(buffer, 0, num4);
							i += num4;
						}
					}
				}
			}
			catch (ObjectDisposedException)
			{
				num = 0;
			}
			catch (IOException)
			{
				num = 0;
			}
			finally
			{
				num = IPAddress.HostToNetworkOrder(num);
				binaryWriter.Write(num);
				binaryReader.Close();
				binaryWriter.Close();
				tcpClient.Close();
			}
		}
	}
}
