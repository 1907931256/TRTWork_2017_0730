using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Station
{
   public class Contrue
    {

        /*	以认为AutoResetEvent就是一个公共的变量（尽管它是一个事件），
       创建的时候可以设置为false，然后在要等待的线程使用它的WaitOne方法，
       那么线程就一直会处于等待状态，只有这个AutoResetEvent被别的线程使用了Set方法
       ，也就是要发通知的线程使用了它的Set方法，那么等待的线程就会往下执行了，
       Set就是发信号，WaitOne是等待信号，只有发了信号，等待的才会执行。
       如果不发的话，WaitOne后面的程序就永远不会执行。
       好下面看用AutoResetEvent改造上面的程序：
       */
       
        #region  属性

        private static AutoResetEvent WaitRecEvent = new AutoResetEvent(false);
        private static SerialPort comm = new SerialPort();//串口程序的主要处理类
       
        //数据处理函数

        public delegate void HandleCommData(Byte[] data, SerialPort sPort);
     

        //事件侦听
        public event HandleCommData Handlers;
        //负责读写Comm的线程
        private Thread _workerThread;


        //字段
        private SerialPort _Comm;
        private Byte[] buffer = new Byte[20];
        private int count = 0;
        private string name;
        private string text;
        private string hex;
        private int _interval;  //设定串口等待时间


        #endregion

        #region 方法



       //类的构造函数
        public Contrue(SerialPort comm,int interval)
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
     



        /// <summary>
        /// 串口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadComm()
        {
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
                    Thread.Sleep(_interval);
                    continue;
                }
                //将读到的一字节数据存入缓存，这里需要做一转换

                buffer[count] = Convert.ToByte(obj);

                if (buffer[0] == 0xEF)
                {
                    count++;
                }
                if (buffer[count]==0x47)
                {
                    break;
                }


                //计算接收数据的结束位                    

                //当达到指定长度时，这里的判断条件可以根据要求变为：

                // 判断当前读到的字节是否为结束位，等等

                //计算结束标志位，协议为除了开始标志位的其他数据值的异或值



                if (count == Convert.ToInt32(buffer[1].ToString("x2")) + 2)//我的接收规则是6位长度
                {

                    //复制数据，并清空缓存，计数器也置零

                    Byte[] data = new Byte[count];//bufCount                        

                    //Array.Copy(buffer, data, bufCount);

                    Array.Copy(buffer, 0, data, 0, count);

                    count = 0;

                    Array.Clear(buffer, 0, 1024);

                    //通知处理器处理数据

                    if (Handlers != null)

                        Handlers(data, comm);

                }

            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 数据以16进制发送
        /// </summary>
        /// <param name="strParam"></param>
        public static void SendHex(string strParam)
        {
            byte[] buf = ShujuChuli.HexStringToBytes(strParam);
            try
            {
                if (comm.IsOpen)
                {
                    comm.Write(buf, 0, buf.Length);
                    //等待返回数据将该信号WaitRecEvent.Set();
                    WaitRecEvent.WaitOne();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
