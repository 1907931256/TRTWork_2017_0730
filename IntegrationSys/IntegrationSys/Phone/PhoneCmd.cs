using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using IntegrationSys.LogUtil;
using IntegrationSys.CommandLine;
using IntegrationSys.Flow;

namespace IntegrationSys.Phone
{
    class RespPair
    {
        public ManualResetEvent RespEvent
        {
            get;
            set;
        }

        public string Resp
        {
            get;
            set;
        }
    }

    class PhoneCmd : IExecutable
    {
        const string ACTION_APK_CONNECT = "APK通信连接";
        const string ACTION_APK_DISCONNECT = "APK通信断开";
        const string ACTION_PUSH = "push";
        const string ACTION_PULL = "pull";

        const int PORT = 6666;
        const int HEAD_SIZE = 4;

        private static PhoneCmd instance_ = null;
        private TcpClient tcpClient_;

        private BlockingCollection<string> dataQueue_;
        private ConcurrentDictionary<string, RespPair> dict_;

        private bool sendThreadExit_;
        private bool recvThreadExit_;

        private PhoneCmd()
        {
            dataQueue_ = new BlockingCollection<string>();
            dict_ = new ConcurrentDictionary<string, RespPair>();

            sendThreadExit_ = true;
            recvThreadExit_ = true;
        }

        public static PhoneCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new PhoneCmd();
                }
                return instance_;
            }
        }

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (action == ACTION_APK_CONNECT)
            {
                ExecuteApkConnect(param, out retValue);
            }
            else if (action == ACTION_APK_DISCONNECT)
            {
                ExecuteApkDisconnect(param, out retValue);
            }
            else if (action == ACTION_PUSH)
            {
                ExecutePush(param, out retValue);
            }
            else if (action == ACTION_PULL)
            {
                ExecutePull(param, out retValue);
            }
            else
            {
                string data;
                if (string.IsNullOrEmpty(param))
                {
                    data = action;
                }
                else
                {
                    data = action + " " + param;
                }
                ExecuteSendData(data, out retValue);
            }
        }


        /// <summary>
        /// wifi通讯连接
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        private void ExecuteApkConnect(string param, out string retValue)
        {
            bool result = false;

            if (param == "Wifi")
            {
                //result = Connect(AppInfo.PhoneInfo.IP, 3000);
                result = Connect("192.168.0.53", 3000);
                AppInfo.PhoneInfo.ConnectType = 1;
            }
            else
            {
                result = Connect();
                AppInfo.PhoneInfo.ConnectType = 0;
            }

            retValue = result ? "Res=Pass" : "Res=Fail";
        }

        private void ExecuteApkDisconnect(string param, out   string retValue)
        {
            Disconnect();
            retValue = "Res=Pass";
        }

        private void ExecutePush(string param, out string retValue)
        {
            string ip = "127.0.0.1";
            if (AppInfo.PhoneInfo.ConnectType == 1)
            {
                ip = AppInfo.PhoneInfo.IP;
            }

            string[] paths = param.Split(' ');
            FileTransferCmd transferCmd = new FileTransferCmd(ip);
            retValue = FileTransferCmd.TRANSFER_ERROR_NONE == transferCmd.Push(paths[0], paths[1]) ? "Res=Pass" : "Res=Fail";
        }

        private void ExecutePull(string param, out string retValue)
        {
            string ip = "127.0.0.1";
            if (AppInfo.PhoneInfo.ConnectType == 1)
            {
                ip = AppInfo.PhoneInfo.IP;
            }

            string[] paths = param.Split(' ');
            FileTransferCmd transferCmd = new FileTransferCmd(ip);
            retValue = FileTransferCmd.TRANSFER_ERROR_NONE == transferCmd.Pull(paths[0], paths[1]) ? "Res=Pass" : "Res=Fail";            
        }

        private void ExecuteSendData(string data, out string retValue)
        {
            dataQueue_.Add(data);

            RespPair pair = new RespPair();
            pair.RespEvent = new ManualResetEvent(false);;
            RespPair getOrAddPair = dict_.GetOrAdd(data, pair);
            getOrAddPair.RespEvent.WaitOne();

            RespPair respPair;
            if (dict_.TryRemove(data, out respPair))
            {
                retValue = respPair.Resp;
            }
            else
            {
                retValue = "Res=PhoneCmdConcurrentConflict";
            }
            
        }

        /// <summary>
        /// 发送TEST，测试通信是否正常
        /// </summary>
        /// <returns></returns>
        private bool Handshake()
        {
            if (!tcpClient_.Connected) return false;

            if (!Send("TEST")) return false;

            BinaryReader reader = new BinaryReader(tcpClient_.GetStream());
            int dataSize = IPAddress.NetworkToHostOrder(reader.ReadInt32());
            byte[] dataBytes = reader.ReadBytes(dataSize - HEAD_SIZE);
            string resp = System.Text.Encoding.UTF8.GetString(dataBytes);
            if (!resp.Contains("Res=PASS")) return false;

            return true;
        }

        /// <summary>
        /// 通过数据线连接Apk server
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            string result;
            AdbCommand.ExecuteAdbCommand("forward tcp:6666 tcp:6666", out result);
            return Connect("127.0.0.1");
        }

        /// <summary>
        /// 通过wifi连接apk server
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool Connect(string ip)
        {
            try
            {
                Log.Debug("Connect " + ip + " enter");
                Disconnect();
                tcpClient_ = new TcpClient();
                Log.Debug("Connect method called before");
                tcpClient_.Connect(IPAddress.Parse(ip), PORT);
                Log.Debug("Connect method called after");

                Log.Debug("Handshake before");
                if (!Handshake()) return false;
                Log.Debug("Handshake after");

                StartSendThread();
                StartRecvThread();
                Log.Debug("Connect " + ip + " leave");
            }
            catch (Exception e)
            {
                Log.Debug("Connect exception", e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 连接指定IP，并设置超时时间
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private bool Connect(string ip, int timeout)
        {
            Log.Debug("Connect " + ip + " enter");

            Disconnect();

            tcpClient_ = new TcpClient();
            IAsyncResult asyncResult = tcpClient_.BeginConnect(IPAddress.Parse(ip), PORT, null, null);
            Log.Debug("BeginConnect method call");

            if (asyncResult.AsyncWaitHandle.WaitOne(timeout))
            {
                tcpClient_.EndConnect(asyncResult);
                Log.Debug("Handshake before");
                if (!Handshake()) return false;
                Log.Debug("Handshake after");

                StartSendThread();
                StartRecvThread();
            }
            else
            {
                Log.Debug("BeginConnect timeout");
                Disconnect();
                return false;
            }

            return true;
        }


        /// <summary>
        /// 断开与Apk server的连接
        /// </summary>
        private void Disconnect()
        {
            StopSendThread();
            StopRecvThread();

            if (tcpClient_ != null)
            {
                if (tcpClient_.Connected)
                {
                    tcpClient_.GetStream().Close();
                }
                tcpClient_.Close();
                tcpClient_ = null;
            }
        }

        /// <summary>
        /// 发送具体测试命令
        /// </summary>
        /// <param name="data">例如Wifi state</param>
        private bool Send(string data)
        {
            if (!tcpClient_.Connected) return false;
            MemoryStream memstream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(memstream))
            {
                byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
                int dataSize = HEAD_SIZE + dataBytes.Length;
                writer.Write(IPAddress.HostToNetworkOrder(dataSize));
                writer.Write(dataBytes);

                byte[] bytes = memstream.ToArray();
                NetworkStream netstream = tcpClient_.GetStream();
                netstream.Write(bytes, 0, bytes.Length);
            }

            return true;
        }

        private void StartSendThread()
        {
            sendThreadExit_ = false;

            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadSend));
        }

        private void StopSendThread()
        {
            sendThreadExit_ = true;
        }

        private void StartRecvThread()
        {
            recvThreadExit_ = false;

            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadRecv));
        }

        private void StopRecvThread()
        {
            recvThreadExit_ = true;
        }

        private void ThreadSend(Object stateInfo)
        {
            string data;
            while (dataQueue_.TryTake(out data, Timeout.Infinite))
            {
                Send(data);
            }
        }

        private void ThreadRecv(Object stateInfo)
        {
            try
            {
                while (!recvThreadExit_)
                {
                    if (tcpClient_ != null && tcpClient_.Connected)
                    {
                        BinaryReader reader = new BinaryReader(tcpClient_.GetStream());

                        int dataSize = IPAddress.NetworkToHostOrder(reader.ReadInt32());
                        byte[] dataBytes = reader.ReadBytes(dataSize - HEAD_SIZE);
                        string resp = System.Text.Encoding.UTF8.GetString(dataBytes);

                        int pos = resp.IndexOf("::Rsp]");
                        if (pos != -1)
                        {
                            string respKey = resp.Substring(1, pos - 1);
                            string respValue = resp.Substring(pos + "::Rsp]".Length);

                            RespPair pair;
                            if (dict_.TryGetValue(respKey, out pair))
                            {
                                pair.Resp = respValue;
                                pair.RespEvent.Set();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message, e);
            }
        }
    }
}
