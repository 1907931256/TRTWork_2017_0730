using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommonPortCmd.Test
{
   public class SerialPort:PortCommon
    {
        public string portName;
        private Thread recThread;//串口数据接收线程
        private AutoResetEvent delayRecEvent;//延迟接收锁

       public bool SendCmd(string strCmd, out string str)
       {

           string strHex = StrPramToHexPram.StrToHex(strCmd);
           if (strHex != "")
           {
               relationship.RelShip(strCmd);

               SendHex(strHex);

               Console.WriteLine(strHex);//测试用

               recStr = recDataStr;
               Log.Debug(strCmd + "-->命令返回-->" + recStr);
               return true;
           }
           else
           {
               recStr = "status=NOT";
               return true;
           }
       }
       public bool SendCmd(string strCmd, string strPram,out string str)
       {
           delay = DelayCmd.WhetherCmd(strCmd);

           //解析发送过来的字符串命令
           string strHex = StrPramToHexPram.StrToHex(strCmd);
           string sendHex = (strHex + " " + ShujuChuli.StrToHex(param) + " 81");

           relationship.RelShip(strCmd);

           if (strHex != "")
           {
               if (delay == true)//命令是一条延迟命令
               {
                   lock (delaylook)
                   {

                       SendHex(sendHex);//发送命令
                       recStr = recDataStr;
                       Log.Debug(strCmd + "-->命令返回-->" + recStr);
                       delyEvent.WaitOne();//等待延迟数据返回
                       return true;
                   }
               }
               else
               {

                   SendHex(sendHex);//发送命令

                   recStr = recDataStr;
                   Log.Debug(strCmd + "-->命令返回-->" + recStr);
                   return true;
               }

           }
           else
           {
               recStr = "status=NOT";
               return true;
           }

       }
       public bool ConnectPort()
       {
           //握手状态指示
           bool connectOK = false;
           #region 握手

           while (ws)
           {
               if (comm.IsOpen)
               {
                   comm.Close();
                   comm.Dispose();
               }
               else
               {
                   int a = 0;
                   for (int i = 0; i < 5; i++)
                   {

                       string[] ports = SerialPort.GetPortNames();
                       foreach (string port in ports)
                       {
                           comm.PortName = port;
                           comm = new SerialPort(port);

                           comm.BaudRate = 19200;
                           comm.Parity = Parity.None;
                           comm.StopBits = StopBits.One;
                           comm.DataBits = 8;


                           // comm.DataReceived += new SerialDataReceivedEventHandler(comm_DataReceived);//串口数据回调函数

                           //comm.DataReceived +=comm_DataReceived;
                           try
                           {

                               comm.Open();

                               //开启一个线程读取串口数据
                               //把读取线程设置为后台线程

                               _readThread.Start();


                               byte[] sbuf = ShujuChuli.HexStringToBytes(str_woshou);
                               comm.Write(sbuf, 0, sbuf.Length);

                               ChaZhaoChuanKouCiShu = woShouEvent.WaitOne(2000);//等待返回函数将mEvent置为mEvent.Set();
                               if (ChaZhaoChuanKouCiShu == true)
                               {
                                   //将串口名称加载在指定的控件上
                                   // strPort = port;
                                   connectOK = true;
                                   ws = false;
                                   break;
                               }
                               else
                               {

                                   _readThread.Abort();
                                   comm.Close();
                                   comm.Dispose();
                               }

                           }
                           catch (Exception)
                           {
                               _readThread.Abort();
                               comm.Close();
                               comm.Dispose();
                               //现实异常信息给客户。
                               //  MessageBox.Show(ex.Message);
                           }
                       }
                       a += 1;
                       if (ChaZhaoChuanKouCiShu == true)
                       {
                           //  MessageBox.Show("无法找到串口！请确认硬件问题.");
                           ws = false;
                           break;
                       }
                       if (a == 5)
                       {
                           ws = false;
                           comm.Close();
                           connectOK = false;
                           break;
                       }
                   }
               }

           }
           #endregion
           return connectOK;
       }

       public void ReadThreadProc()
       {
           throw new NotImplementedException();
       }

       public void CloseProt()
       {
           throw new NotImplementedException();
       }

       public void WriteHexData(string strHex)
       {

           byte[] buf = ShujuChuli.HexStringToBytes(strParam.ToUpper());
           try
           {
               if (comm.IsOpen)
               {

                   comm.Write(buf, 0, buf.Length);

                   Console.WriteLine(strParam);
                   Log.Debug("CmdHex-->" + strParam);//问题调查时可用
               }
               else
               {
                   ConnectPort();
                   try
                   {
                       if (comm.IsOpen)
                       {
                           //  recEvent.WaitOne();
                           comm.Write(buf, 0, buf.Length);

                       }
                   }
                   catch (Exception)
                   {
                       comm = new SerialPort();
                       //   MessageBox.Show(ex.Message);
                   }
               }
           }
           catch (Exception)
           {

               throw;
           }
       }
    }
}
