using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.CommandLine;
using System.IO;
using System.Net.Sockets;
using System.Net;
using IntegrationSys.LogUtil;
using System.Threading;

namespace IntegrationSys.Phone
{
    class FileTransferCmd
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
            ip_ = ip;
            port_ = PORT;

            if (ip == LOOPBACK_IP)
            {
                string adbResult;
                AdbCommand.ExecuteAdbCommand("forward tcp:6661 tcp:6661", out adbResult);
            }
        }

        public int Pull(string srcfilename, string destfilename)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Log.Debug("connect ip = " + ip_ + " port = " + port_ + " start");
                    client.Connect(ip_, port_);
                    Log.Debug("connect ip = " + ip_ + " port = " + port_ + " end");

                    using (NetworkStream stream = client.GetStream())
                    {
                        BinaryWriter writer = new BinaryWriter(stream);
                        // buf 结构为 总长度(4byte) + pull/push标志(4byte) + 目标文件全路径名(256byte)
                        int length = 4 + 4 + 256;
                        writer.Write(IPAddress.HostToNetworkOrder(length));
                        writer.Write(IPAddress.HostToNetworkOrder(TRANSFER_TYPE_PULL));
                        byte[] filename = System.Text.Encoding.UTF8.GetBytes(srcfilename);
                        writer.Write(filename);
                        byte[] filenameRemaining = new byte[256 - filename.Length];
                        writer.Write(filenameRemaining);
                        Log.Debug("write finish");

                        using (FileStream fs = new FileStream(destfilename, FileMode.Create))
                        {
                            Log.Debug("create dest file");

                            BinaryReader reader = new BinaryReader(stream);

                            Log.Debug("create binary reader");

                            int totalBytes = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                            Log.Debug("read totalBytes = " + totalBytes);
                            int readTotalBytes = 4;
                            const int BUFFER_SIZE = 4096;
                            byte[] buffer = new byte[BUFFER_SIZE];
                         
                            while (readTotalBytes < totalBytes)
                            {
                                int readBytes = reader.Read(buffer, 0, BUFFER_SIZE);
                                fs.Write(buffer, 0, readBytes);
                                readTotalBytes += readBytes;
                                Log.Debug("TotalBytes = " + totalBytes + ", readBytes = " + readBytes + ", readTotalBytes = " + readTotalBytes);
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

        public int Push(string srcfilename, string destfilename)
        {
            try
            {
                using (FileStream fs = new FileStream(srcfilename, FileMode.Open, FileAccess.Read))
                {
                    using (TcpClient client = new TcpClient())
                    {
                        client.Connect(ip_, port_);

                        using (NetworkStream stream = client.GetStream())
                        {
                            BinaryWriter writer = new BinaryWriter(stream);
                            // buf 结构为 总长度(4byte) + pull/push标志(4byte) + 目标文件全路径名(256byte) + 文件字节流
                            int length = 4 + 4 + 256 + (int)fs.Length;
                            writer.Write(IPAddress.HostToNetworkOrder(length));
                            writer.Write(IPAddress.HostToNetworkOrder(TRANSFER_TYPE_PUSH));
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
