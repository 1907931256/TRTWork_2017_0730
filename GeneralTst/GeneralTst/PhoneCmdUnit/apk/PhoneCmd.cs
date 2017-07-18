using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GeneralTst.Log4Net;

namespace PhoneCmdUnit
{
   public class PhoneCmd
    {
       private TcpClient tcpClient_=new TcpClient();
       private bool sendThreadExit_=false;
       private AutoResetEvent sendEvent_=new AutoResetEvent(false);
       private bool recvThreadExit_=false;
       //private static int PORT = 6666;
       private static PhoneCmd instance_=new PhoneCmd();
       //private static int HEAD_SIZE = 4;
       private ConcurrentDictionary<string, RespPair> dict_=new ConcurrentDictionary<string,RespPair>();
       private ConcurrentQueue<string> dataQueue_=new ConcurrentQueue<string>();

       /// <summary>
       /// 断开连接
       /// </summary>
       public void Disconnect()
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

       public bool Send(string data)
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

       public void ExecuteSendData(string data, out string retValue)
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

       //private void ExecuteApkConnect(string param, out string retValue)
       //{
       //    bool flag;
       //    if (param == "Wifi")
       //    {
       //        flag = this.Connect(AppInfo.PhoneInfo.IP);
       //    }
       //    else
       //    {
       //        flag = this.Connect();
       //    }
       //    retValue = (flag ? "Res=Pass" : "Res=Fail");
       //}
       //private void ExecuteApkDisconnect(string param, out string retValue)
       //{
       //    this.Disconnect();
       //    retValue = "Res=Pass";
       //}

       /// <summary>
       /// wifi 连接
       /// </summary>
       /// <param name="ip"></param>
       /// <returns></returns>
       public bool Connect(string ip)
       {
           this.Disconnect();
           this.tcpClient_ = new TcpClient();
           this.tcpClient_.Connect(IPAddress.Parse(ip), 6666);
           if (!this.Handshake())
           {
               return false;
           }
           this.StartSendThread();
           this.StartRecvThread();
           return true;
       }

       public bool Connect()
       {
           string text;
           AdbCommand.ExecuteAdbCommand("forward tcp:6666 tcp:6666", out text);
           return this.Connect("127.0.0.1");
       }

       public void ExecutePull(string param, out string retValue)
       {
           string ip = "127.0.0.1";
           //if (AppInfo.PhoneInfo.ConnectType == 1)
           //{
           //    ip = AppInfo.PhoneInfo.IP;
           //}
           string[] array = param.Split(new char[]{' '});
           ApkFile_Transfer fileTransferCmd = new ApkFile_Transfer(ip);
           retValue = ((fileTransferCmd.Pull(array[1], array[2]) == 0) ? "Res=Pass" : "Res=Fail");
       }

       public void ExecutePush(string param, out string retValue)
       {
           string ip = "127.0.0.1";
           //if (AppInfo.PhoneInfo.ConnectType == 1)
           //{
           //    ip = AppInfo.PhoneInfo.IP;
           //}
           string[] array = param.Split(new char[] { ' ' });
           ApkFile_Transfer fileTransferCmd = new ApkFile_Transfer(ip);
           retValue = ((fileTransferCmd.Push(array[1], array[2]) == 0) ? "Res=Pass" : "Res=Fail");
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
       private void StartRecvThread()
       {
           this.recvThreadExit_ = false;
           ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadRecv));
       }

       private void StartSendThread()
       {
           this.sendThreadExit_ = false;
           ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadSend));
       }

       private void StopRecvThread()
       {
           this.recvThreadExit_ = true;
       }

       private void StopSendThread()
       {
           this.sendThreadExit_ = true;
           this.sendEvent_.Set();
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


    }
}
