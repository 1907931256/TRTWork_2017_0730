using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Station
{
    class Conent
    {
        internal class CommReader
        {
            //单个缓冲区最大长度
            private const int max = 6;
            //数据计数器
            private int count = 0;
            private int countData = 1;
            //变长标志数
            //private int bufCount = 0;
            //数字缓冲区
            private Byte[] buffer = new Byte[max];
            /// 
            /// 串口控件
            /// 
            private SerialPort _Comm;
            /// 

            /// 扫描的时间间隔 单位毫秒

            /// 

            private Int32 _interval;

            //数据处理函数

            public delegate void HandleCommData(Byte[] data, SerialPort sPort);

            //事件侦听
            public event HandleCommData Handlers;
            //负责读写Comm的线程
            private Thread _workerThread;


            /// <summary>
            /// 
            /// </summary>
            /// <param name="comm"></param>
            /// <param name="interval"></param>
            internal CommReader(SerialPort comm, Int32 interval)
            {

                _Comm = comm;

                //创建读取线程

                _workerThread = new Thread(new ThreadStart(ReadComm));

                //确保扫描时间间隔不要太小，造成线程长期占用cpu

                if (interval < 10)

                    _interval = 10;

                else

                    _interval = interval;

            }



            //读取串口数据，为线程执行函数

            public void ReadComm()
            {
                while (true)
                {

                    Object obj = null;

                    try
                    {

                        //每隔一定时间，从串口读入一字节

                        //如未读到，obj为null

                        obj = _Comm.ReadByte();

                    }

                    catch
                    {

                    }



                    if (obj == null)
                    { //未读到数据，线程休眠

                        Thread.Sleep(_interval);

                        continue;

                    }

                    //将读到的一字节数据存入缓存，这里需要做一转换

                    buffer[count] = Convert.ToByte(obj);

                    if (buffer[0] == 0xFE)
                    {
                        count++;
                    }



                    //计算接收数据的结束位                    

                    //当达到指定长度时，这里的判断条件可以根据要求变为：

                    // 判断当前读到的字节是否为结束位，等等

                    //计算结束标志位，协议为除了开始标志位的其他数据值的异或值



                    if (count ==Convert.ToInt32(buffer[1].ToString("x2"))+2 && buffer[1] == 0x04)//我的接收规则是6位长度，第二个字节是0x04
                    {

                        //复制数据，并清空缓存，计数器也置零

                        Byte[] data = new Byte[6];//bufCount                        

                        //Array.Copy(buffer, data, bufCount);

                        Array.Copy(buffer, 0, data, 0, 6);

                        count = 0;

                        Array.Clear(buffer, 0, max);

                        //通知处理器处理数据

                        if (Handlers != null)

                            Handlers(data, _Comm);

                    }



                    if (count == 6 && buffer[1] != 0x04)
                    {

                        Array.Clear(buffer, 0, max);

                        count = 0;

                    }

                }

            }



            //启动读入器

            public void Start()
            {

                //启动读取线程

                if (_workerThread.IsAlive)

                    return;

                if (!_Comm.IsOpen)

                    _Comm.Open();

                _workerThread.Start();

                while (!_workerThread.IsAlive) ;

            }

            //停止读入

            public void Stop()
            {

                //停止读取线程

                if (_workerThread.IsAlive)
                {

                    _workerThread.Abort();

                    _workerThread.Join();

                }

                _Comm.Close();

            }

        }

        // 把十六进制字符串转换成字节型和把字节型转换成十六进制字符串

        public static string ByteToString(byte[] InBytes)
        {

            string StringOut = "";

            foreach (byte InByte in InBytes)
            {

                StringOut = StringOut + String.Format("{0:X2} ", InByte);

            }

            return StringOut;

        }



        public static byte[] StringToByte(string InString)
        {
            string[] ByteStrings;

            ByteStrings = InString.Split(" ".ToCharArray());

            byte[] ByteOut;

            ByteOut = new byte[ByteStrings.Length - 1];

            for (int i = 0; i == ByteStrings.Length - 1; i++)
            {
                ByteOut[i] = Convert.ToByte(("0x" + ByteStrings[i]));
            }
            return ByteOut;
        }




    }



}
