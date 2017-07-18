using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.IO;
using CommonPortCmd;

namespace ComPort
{
   public class ReportEventArgs : EventArgs
    {
        private byte[] data_;

        public ReportEventArgs(byte[] data)
        {
            data_ = data;
        }

        public byte[] Data
        {
            get
            {
                return data_;
            }
        }
    }

    class SerialPortWrapper
    {
        /// <summary>
        /// 波特率
        /// </summary>
        const int BAUD_RATE = 19200;
        /// <summary>
        /// 数据位
        /// </summary>
        const int DATA_BITS = 8;
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        const int BUFFER_SIZE = 4096;
        /// <summary>
        /// 接收数据头
        /// </summary>
        const int RECV_HEAD = 0x34;

        public delegate void ReportEventHandler(object sender, ReportEventArgs e);
        public event ReportEventHandler Report;

        private SerialPort serialPort_;

        /// <summary>
        /// 发送数据锁
        /// </summary>
        private AutoResetEvent sendEvent_;
        /// <summary>
        /// 接受数据锁
        /// </summary>
        private AutoResetEvent recvEvent_;

        private byte[] readBuffer_;
        private byte[] recvBuffer_;

        /// <summary>
        /// 创建其支持存储区为内存的流
        /// </summary>
        private MemoryStream byteBuffer_;

        private short sendId_;
        /// <summary>
        /// 数据错误标志
        /// </summary>
        private bool errorFlag_;

        private bool keepReadinf_;

        private Thread thread_Rec;

        public SerialPortWrapper()
        {
            sendEvent_ = new AutoResetEvent(true);
            recvEvent_ = new AutoResetEvent(false);
            serialPort_ = new SerialPort();

            keepReadinf_ = true;
            readBuffer_ = new byte[BUFFER_SIZE];
            sendId_ = 0;

        }

        public void Open(string portName)
        {
            Close();
            serialPort_ = new SerialPort(portName);
            serialPort_.BaudRate = BAUD_RATE;
            serialPort_.Parity = Parity.None;
            serialPort_.DataBits = DATA_BITS;
            serialPort_.StopBits = StopBits.One;
            serialPort_.Open();
            byteBuffer_ = new MemoryStream();


            if (serialPort_.IsOpen)
            {
                keepReadinf_ = true;
                if (thread_Rec == null)
                {
                    thread_Rec = new Thread(DataReceivedHandler);
                    thread_Rec.IsBackground = true;
                    thread_Rec.Start();
                }
                
                    
            }


            //serialPort_.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);


        }

        public void Close()
        {
            if (serialPort_ != null)
            {
                keepReadinf_ = false;
                serialPort_.Close();
                serialPort_ = null;
            }

            if (byteBuffer_ != null)
            {
                byteBuffer_.Close();
                byteBuffer_ = null;
            }
            
            //thread_Rec.DisableComObjectEagerCleanup();
            //dataReceivedFlag = false;
        }


        /// <summary>
        /// 阻塞调用，向串口发送数据，并等待回复。如果超时1S，抛出TimeoutException
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        public void SendRecv(byte[] req, out byte[] resp)
        {
            sendEvent_.WaitOne();
            Log.Debug("SerialPortWrapper send data = " + ByteArrayToHexString(req));
            serialPort_.Write(req, 0, req.Length);
            sendId_ = BitConverter.ToInt16(req, 3);
            //数据发送间隔时间
            int timeSet = 10;

            if (!recvEvent_.WaitOne(2000))
            {
                Log.Debug("SerialPort recv timeout");

                sendId_ = 0;
                Thread.Sleep(timeSet);
                sendEvent_.Set();
                throw new TimeoutException();
            }

            if (errorFlag_)
            {
                //通信错误
                Log.Debug("SerialPort communication error");
                sendId_ = 0;
                Thread.Sleep(timeSet);
                sendEvent_.Set();

                throw new InvalidDataException();
            }

            resp = recvBuffer_;
            Log.Debug("SerialPortWrapper recv data = " + ByteArrayToHexString(resp));

            sendId_ = 0;
            Thread.Sleep(timeSet);
            sendEvent_.Set();
        }
        //Object sender, SerialDataReceivedEventArgs e
        private void DataReceivedHandler()
        {
            while (keepReadinf_)
            {
                Log.Debug("DataReceivedHandler start");

                int readBytes = serialPort_.BytesToRead;

                if (readBytes > BUFFER_SIZE)
                {
                    throw new OverflowException();
                }
                if (readBytes <= 0)
                {
                    //Log.Debug("readBytes=" + readBytes);
                    Thread.Sleep(100);
                    continue;
                }

                serialPort_.Read(readBuffer_, 0, readBytes);

                Log.Debug("DataReceivedHandler data = " + ByteArrayToHexString(readBuffer_, 0, readBytes));

                int remaining = readBytes;
                int offset = 0;

                if (byteBuffer_.Length > 0)
                {
                    //bytebuffer_有内容，表示上一次数据接收不完整
                    int size = 0;

                    if (byteBuffer_.Length >= 2)
                    {
                        byteBuffer_.Position = 1;
                        size = byteBuffer_.ReadByte();

                        byteBuffer_.Position = byteBuffer_.Length;
                    }
                    else
                    {
                        size = readBuffer_[0];
                        byteBuffer_.Position = byteBuffer_.Length;
                        byteBuffer_.WriteByte(readBuffer_[0]);
                        remaining--;
                        offset++;
                    }

                    if (remaining >= size + 2 - byteBuffer_.Length)
                    {
                        int writeBytes = size + 2 - (int)byteBuffer_.Length;
                        byteBuffer_.Write(readBuffer_, offset, writeBytes);
                        remaining -= writeBytes;
                        offset += writeBytes;

                        HandlerData(byteBuffer_.ToArray());

                        byteBuffer_.Position = 0;
                        byteBuffer_.SetLength(0);
                    }
                    else if (remaining > 0)
                    {
                        byteBuffer_.Write(readBuffer_, offset, remaining);
                        remaining = 0;
                        offset += remaining;
                    }
                }

                //半截命令的情况已处理完成
                //重新开始查找RECV_HEAD
                while (remaining > 0)
                {

                    int index = FindHead(readBuffer_, offset, remaining);

                    if (index == -1) break;

                    if (index == offset + remaining - 1)
                    {
                        byteBuffer_.Write(readBuffer_, index, 1);
                        remaining = 0;
                    }
                    else
                    {
                        int size = readBuffer_[index + 1];
                        byteBuffer_.Write(readBuffer_, index, 2);
                        remaining -= index - offset + 2;
                        offset = index + 2;

                        if (remaining >= size)
                        {
                            byteBuffer_.Write(readBuffer_, offset, size);
                            remaining -= size;
                            offset += size;

                            HandlerData(byteBuffer_.ToArray());

                            byteBuffer_.Position = 0;
                            byteBuffer_.SetLength(0);
                        }
                        else if (remaining > 0)
                        {
                            byteBuffer_.Write(readBuffer_, offset, remaining);
                            remaining = 0;
                            offset += remaining;
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// 处理一条下位机的完整数据
        /// 下位机数据格式类似34 05 01 10 00 FF 47
        /// 首字节和尾字节为首尾标志，第2个字节为命令的长度
        /// </summary>
        private void HandlerData(byte[] data, int offset, int count)
        {
            Log.Debug("HandlerData start   ");

            if (offset < 0 || count <= 0)
            {
                throw new ArgumentException();
            }

            if (offset + count > data.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            Log.Debug("SerialPortWrapper HandlerData data = " + ByteArrayToHexString(data, offset, count));

            byte mod = data[offset + 3];
            short sendId = BitConverter.ToInt16(data, offset + 3);
            if (mod == 0xFF)
            {
                errorFlag_ = true;
                recvEvent_.Set();
            }
            else
            {
                errorFlag_ = false;
                if (sendId_ != 0 && sendId == sendId_)
                {
                    sendId_ = 0;
                    recvBuffer_ = new byte[count];
                    Array.Copy(data, offset, recvBuffer_, 0, count);
                    recvEvent_.Set();
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        byte[] reportData = new byte[count];
                        Array.Copy(data, offset, reportData, 0, count);

                        Log.Debug("SerialPortWrapper report data = " + ByteArrayToHexString(reportData));

                        if (Report != null)
                        {
                            ReportEventArgs args = new ReportEventArgs(reportData);
                            Report(this, args);
                        }
                    });
                }
            }

        }

        private void HandlerData(byte[] data)
        {
            HandlerData(data, 0, data.Length);
        }

        /// <summary>
        /// 将byte数组以16进制形式输出
        /// </summary>
        /// <returns></returns>
        private string ByteArrayToHexString(byte[] data, int offset, int count)
        {
            if (offset < 0 || count <= 0)
            {
                throw new ArgumentException();
            }

            if (offset + count > data.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            string hex = string.Empty;

            for (int i = offset; i < offset + count; i++)
            {
                hex += data[i].ToString("X2");
                hex += " ";
            }

            return hex;
        }

        private string ByteArrayToHexString(byte[] data)
        {
            return ByteArrayToHexString(data, 0, data.Length);
        }

        /// <summary>
        /// 从data中查找RECV_HEAD,返回其索引
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private int FindHead(byte[] data, int offset, int count)
        {
            if (offset < 0 || count <= 0)
            {
                throw new ArgumentException();
            }

            if (offset + count > data.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            int index = -1;
            for (int i = offset; i < offset + count; i++)
            {
                if (data[i] == RECV_HEAD)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
    }
}
