using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Station
{
   public  class Common
    {

       public string str_woshou = "";


        private SerialPort comm = new SerialPort();//串口程序的主要处理类
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private static AutoResetEvent wEvent = new AutoResetEvent(false);  //握手等待
        private static bool ws = true;//用于控制循环寻找串口
        private static bool ChaZhaoChuanKouCiShu;
        private string strPort;
        private  Byte[] buffer = new Byte[1024];//临时存放串口数据
        private static Byte[] data;//数据保存到该数组下面，用于判断操作
        private int _count;//记录数据长度

      



       /// <summary>
       /// 握手
       /// </summary>
       private string woshou()
       {
           
           #region 握手
           if (comm.IsOpen)
           {
               comm.Close();
           }
           else
           {
               while (ws)
               {
                   int a = 0;
                   for (int i = 0; i < 5; i++)
                   {
                       string[] ports = SerialPort.GetPortNames();
                       foreach (string port in ports)
                       {
                           comm.PortName = port;
                           comm.BaudRate = 19200;
                           comm = new SerialPort(port, 19200);
                           comm.DataReceived += comm_DataReceived;//串口数据回调函数

                           try
                           {
                               comm.Open();
                               byte[] sbuf = ShujuChuli.HexStringToBytes(str_woshou);
                               comm.Write(sbuf, 0, sbuf.Length);

                               ChaZhaoChuanKouCiShu = wEvent.WaitOne(1000);//等待返回函数将mEvent置为mEvent.Set();

                               if (ChaZhaoChuanKouCiShu == true)
                               {
                                   //将串口名称加载在指定的控件上
                              strPort = port;
                                   break;
                               }
                               else
                               {
                                   //   RecEvent.WaitOne();
                                   comm.Close();
                               }

                           }
                           catch (Exception ex)
                           {
                               //现实异常信息给客户。
                               MessageBox.Show(ex.Message);
                           }
                       }
                       a += 1;
                       if (ChaZhaoChuanKouCiShu == true)
                       {
                           MessageBox.Show("无法找到串口！请确认硬件问题.");
                  
                           break;
                       }
                       if (a == 5)
                       {
                           ws = false;
                           comm.Close();
                           break;
                       }
                   }
               }
             
           }
           #endregion

           return strPort;

       }
		
	    
       /// <summary>
       ///数据发送
       /// </summary>
       /// <param name="strParam"></param>
       private void SendHex(string strParam)
       {
           byte[] buf = ShujuChuli.HexStringToBytes(strParam.ToUpper());
           try
           {
               if (comm.IsOpen)
               {
                   comm.Write(buf, 0, buf.Length);
               }
               else
               {
                   woshou();
                   try
                   {
                       if (comm.IsOpen)
                       {
                           comm.Write(buf, 0, buf.Length);
                       }
                   }
                   catch (Exception ex)
                   {
                       this.comm = new SerialPort();
                       MessageBox.Show(ex.Message);
                   }


               }
           }
           catch (Exception)
           {

               throw;
           }
       }


       private void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
       {
           #region 数据读取  到串口中读取指定的数据
           while (true)
           {
               Object obj = null;
               try
               {
                   //每隔一定时间，从串口读入一字节
                   //如未读到，obj为null
                   obj = comm.ReadByte();
               }
               catch
               {
               }
               if (obj == null)
               { //未读到数据，线程休眠
                   Thread.Sleep(1);
                   continue;
               }
               //将读到的一字节数据存入缓存，这里需要做一转换

               buffer[_count] = Convert.ToByte(obj);

               if (buffer[0].ToString("x2") == "34")
               {
                   _count++;
               }
               if (_count > 1)
               {
                   if (buffer[_count - 1].ToString("x2") == "47")
                   {
                       break;
                   }
               }

           }
           while (true)
           {
               if (_count == Convert.ToInt32(buffer[1].ToString("d")) + 2)//我的接收规则是6位长度
               {

                   builder.Clear();
                   //复制数据，并清空缓存，计数器也置零

                   data = new Byte[_count];//bufCount                        

                   //Array.Copy(buffer, data, bufCount);

                   Array.Copy(buffer, 0, data, 0, _count);
                   Array.Clear(buffer, 0, _count);

                   _count = 0;

                   foreach (byte b in data)
                   {
                       builder.Append(b.ToString("X2") + " ");
                   }
                   //通知处理器处理数据


                   break;
               }
           }
           #endregion


           //此处是一个线程工厂模式，利用（匿名委托）Lamd表达式，启用一个线程
           var Rec = Task.Factory.StartNew(() =>
           {
               if (builder.ToString().ToUpper().IndexOf("04 16 0F 00") != -1)
               {
                   ws = false;
                   wEvent.Set();   //此处表示握手成功
               }
           });

       }


    }
}
