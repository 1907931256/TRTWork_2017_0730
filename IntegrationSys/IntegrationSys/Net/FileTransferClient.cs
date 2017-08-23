using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace IntegrationSys.Net
{
    class FileTransferClient
    {
        public const int TRANSFER_ERROR_NONE = 0;
        public const int TRANSFER_ERROR_FILE_NOT_EXIST = 1;
        public const int TRANSFER_ERROR_NETWORK = 2;

        private string ip_;

        public FileTransferClient(string ip)
        {
            ip_ = ip;
        }

        public int Upload(string srcfilename, string destfilename)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(ip_, NetUtil.PORT_FILE_TRANSFER_SERVER);

                    using (NetworkStream stream = client.GetStream())
                    {
                        using (FileStream fs = new FileStream(srcfilename, FileMode.Open, FileAccess.Read))
                        {
                            BinaryWriter writer = new BinaryWriter(stream);
                            // buf 结构为 总长度(4byte) + pull/push标志(4byte) + 目标文件全路径名(256byte) + 文件字节流
                            int length = 4 + 4 + 256 + (int)fs.Length;
                            writer.Write(IPAddress.HostToNetworkOrder(length));
                            writer.Write(IPAddress.HostToNetworkOrder(FileTransferServer.TRANSFER_TYPE_UPLOAD));
                            byte[] filename = System.Text.Encoding.UTF8.GetBytes(destfilename);
                            writer.Write(filename);
                            byte[] filenameRemaining = new byte[256 - filename.Length];
                            writer.Write(filenameRemaining);
                            byte[] fileContent = new byte[fs.Length];
                            fs.Read(fileContent, 0, fileContent.Length);
                            writer.Write(fileContent);

                            BinaryReader reader = new BinaryReader(stream);
                            int rescode = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                            if (rescode != 1)
                            {
                                return TRANSFER_ERROR_FILE_NOT_EXIST;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return TRANSFER_ERROR_FILE_NOT_EXIST;
            }

            return TRANSFER_ERROR_NONE;
        }
    }
}
