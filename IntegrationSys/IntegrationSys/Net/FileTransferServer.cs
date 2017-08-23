using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using IntegrationSys.LogUtil;

namespace IntegrationSys.Net
{
    class FileTransferServer
    {
        public const int TRANSFER_TYPE_UPLOAD = 0;
        public const int TRANSFER_TYPE_DOWNLOAD = 1;

        private TcpListener server;
        private bool exit = false;

        private static FileTransferServer instance_;

        public static FileTransferServer Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new FileTransferServer();
                }

                return instance_;
            }
        }

        private FileTransferServer() { }

        public void Start()
        {
            exit = false;

            IPAddress localAddr = IPAddress.Parse(NetUtil.LocalIp());
            server = new TcpListener(localAddr, NetUtil.PORT_FILE_TRANSFER_SERVER);

            server.Start();

            while (!exit)
            {
                TcpClient client = server.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), client);
            }
        }

        public void Stop()
        {
            exit = true;
            server.Stop();
        }

        private static void ThreadProc(Object state)
        {
            TcpClient client = (TcpClient)state;
            NetworkStream networkStream = client.GetStream();
            BinaryReader binReader = new BinaryReader(networkStream);
            BinaryWriter binWriter = new BinaryWriter(networkStream);
            int rescode = 1;
            try
            {
                int totalByteRead = 0;
                int length = IPAddress.NetworkToHostOrder(binReader.ReadInt32());
                totalByteRead += 4;
                int type = IPAddress.NetworkToHostOrder(binReader.ReadInt32());
                totalByteRead += 4;
                byte[] byteFilename = binReader.ReadBytes(256);
                totalByteRead += 256;
                string filename = System.Text.Encoding.UTF8.GetString(byteFilename, 0, byteFilename.Length);
                filename = filename.TrimEnd('\0');

                if (type == TRANSFER_TYPE_UPLOAD)
                {
                    Log.Debug("FileTransferServer upload destination path = " + filename);
                    using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                    {
                        const int BUFFER_SIZE = 4096;
                        byte[] buffer = new byte[BUFFER_SIZE];
                        while (totalByteRead < length)
                        {
                            int byteRead = binReader.Read(buffer, 0, BUFFER_SIZE);
                            fileStream.Write(buffer, 0, byteRead);
                            totalByteRead += byteRead;
                        }

                    }
                }
            }
            catch (ObjectDisposedException)
            {
                rescode = 0;
            }
            catch (IOException)
            {
                rescode = 0;
            }
            finally
            {
                rescode = IPAddress.HostToNetworkOrder(rescode);
                binWriter.Write(rescode);
                binReader.Close();
                binWriter.Close();
                client.Close();
            }
        }

    }
}
