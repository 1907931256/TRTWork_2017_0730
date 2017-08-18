using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IntegrationSys.Net
{
	internal class FileTransferClient
	{
		public const int TRANSFER_ERROR_NONE = 0;

		public const int TRANSFER_ERROR_FILE_NOT_EXIST = 1;

		public const int TRANSFER_ERROR_NETWORK = 2;

		private string ip_;

		public FileTransferClient(string ip)
		{
			this.ip_ = ip;
		}

		public int Upload(string srcfilename, string destfilename)
		{
			try
			{
				using (TcpClient tcpClient = new TcpClient())
				{
					tcpClient.Connect(this.ip_, 10106);
					using (NetworkStream stream = tcpClient.GetStream())
					{
						using (FileStream fileStream = new FileStream(srcfilename, FileMode.Open, FileAccess.Read))
						{
							BinaryWriter binaryWriter = new BinaryWriter(stream);
							int host = 264 + (int)fileStream.Length;
							binaryWriter.Write(IPAddress.HostToNetworkOrder(host));
							binaryWriter.Write(IPAddress.HostToNetworkOrder(0));
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
