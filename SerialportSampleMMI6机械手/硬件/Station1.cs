using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Station
{
    public partial class Station1 : Form
    {
        private SerialPort comm = new SerialPort();//串口程序的主要处理类
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。

        private static AutoResetEvent wEvent = new AutoResetEvent(false);  //握手等待
        private static bool ws = true;//用于控制循环寻找串口
        private static bool ChaZhaoChuanKouCiShu;

        private static Byte[] buffer = new Byte[20];//临时存放串口数据
        private static Byte[] data;//数据保存到该数组下面，用于判断操作
        private int _count;//记录数据长度
		 
       // private static bool flag_com = false;
	    private static AutoResetEvent mEvent = new AutoResetEvent(false);
      //  private static AutoResetEvent RecEvent = new AutoResetEvent(false);  //握手等待

        StationData_1 strData = new StationData_1();

       
        string strRec = "";
        #region 实现按钮缩小放大
        private float X;

        private float Y;

        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }
        private void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {

                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });
                float a = Convert.ToSingle(mytag[0]) * newx;
                con.Width = (int)a;
                a = Convert.ToSingle(mytag[1]) * newy;
                con.Height = (int)(a);
                a = Convert.ToSingle(mytag[2]) * newx;
                con.Left = (int)(a);
                a = Convert.ToSingle(mytag[3]) * newy;
                con.Top = (int)(a);
                Single currentSize = Convert.ToSingle(mytag[4]) * Math.Min(newx, newy);
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }

        }
        void Form1_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / X;
            float newy = this.Height / Y;
            setControls(newx, newy, this);
         //   this.Text = this.Width.ToString() + " " + this.Height.ToString();

        }


        #endregion




        public Station1()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }
     


        #region 支持改变窗体大小
        private const int Guying_HTLEFT = 10;
        private const int Guying_HTRIGHT = 11;
        private const int Guying_HTTOP = 12;
        private const int Guying_HTTOPLEFT = 13;
        private const int Guying_HTTOPRIGHT = 14;
        private const int Guying_HTBOTTOM = 15;
        private const int Guying_HTBOTTOMLEFT = 0x10;
        private const int Guying_HTBOTTOMRIGHT = 17;

        /// <summary>
        /// 该函数在 FormBorderState 设置成none后，支持窗体大小的改变
        /// 
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0084:
                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMLEFT;
                        else
                            m.Result = (IntPtr)Guying_HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMRIGHT;
                        else
                            m.Result = (IntPtr)Guying_HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)Guying_HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)Guying_HTBOTTOM;
                    break;
                case 0x0201://鼠标左键按下的消息
                    m.Msg = 0x00A1;//更改消息为非客户区按下鼠标
                    m.LParam = IntPtr.Zero; //默认值
                    m.WParam = new IntPtr(2);//鼠标放在标题栏内
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        #endregion


        /// <summary>
        /// 数据以16进制发送
        /// </summary>
        /// <param name="strParam"></param>
        //数据发送
        private void SendHex(string strParam)
        {
            byte[] buf = ShujuChuli.HexStringToBytes(strParam);

            try
            {
                if (comm.IsOpen)
                {
                    comm.Write(buf, 0, buf.Length);
                }
                else
                {
                    woshou();//串口线失效，调用握手函数再次握手
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

        private void Form1_Load(object sender, EventArgs e)
        {
             #region 配合上面的代码实现了窗体按钮大小可变

            this.Resize += new EventHandler(Form1_Resize);

            X = this.Width;
            Y = this.Height;

            setTag(this);
            #endregion

            ShuJuJiaZai();
            Action wo = new Action(woshou);

            wo.Invoke();


        }

        private void ShuJuJiaZai()
        {
            #region   commbox 数据加载
            comBox_ZhiShiDeng_1.Items.Add("绿灯");
            comBox_ZhiShiDeng_1.Items.Add("红灯");
            comBox_ZhiShiDeng_1.Items.Add("黄灯");
            comBox_ZhiShiDeng_1.SelectedIndex = 0;

            comBox_ZT.Items.Add("测试中");
            comBox_ZT.Items.Add("测试完成");
            comBox_ZT.SelectedIndex = 0;

            comMS.Items.Add("清仓");
            comMS.Items.Add("正常测试");
            comMS.SelectedIndex = 0;

            baoJin_combox.Items.Add("1");
            baoJin_combox.Items.Add("2");
            baoJin_combox.Items.Add("4");
            baoJin_combox.Items.Add("8");
            baoJin_combox.Items.Add("16");
            baoJin_combox.Items.Add("32");
            baoJin_combox.SelectedIndex = 0;

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;

            #endregion

        }
        private void woshou()
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
                                // Thread.Sleep(1000);
                                string s = "72 04 11 0f 00 81";
                                byte[] sbuf = ShujuChuli.HexStringToBytes(s);
                                comm.Write(sbuf, 0, sbuf.Length);

                                ChaZhaoChuanKouCiShu = wEvent.WaitOne(2000);//等待返回函数将mEvent置为mEvent.Set();

                                if (ChaZhaoChuanKouCiShu == true)
                                {
                                    //将串口名称加载在指定的控件上
                                    comboPortName.Items.Add(port);
                                    comboPortName.SelectedIndex = 0;
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
                            MessageBox.Show("无法找到串口！请确认硬件问题.");
                        }

                        a += 1;
                        if (ChaZhaoChuanKouCiShu == true)
                        {
                            break;
                        }
                        if (a == 5)
                        {
                            MessageBox.Show("无法找到串口！请确认硬件问题" + "\r\n" + "程序退出");
                            ws = false;
                            comm.Close();
                            this.Close();
                            break;
                        }
                    }
                }
            }
            #endregion
        }

        private void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
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
                    Thread.Sleep(10);
                    continue;
                }
                //将读到的一字节数据存入缓存，这里需要做一转换

                buffer[_count] = Convert.ToByte(obj);

                if (buffer[0].ToString("x2") == "34")
                {
                    _count++;
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
            

            //此处是一个线程工厂模式，利用（匿名委托）Lamd表达式，启用一个线程
            var Rec = Task.Factory.StartNew(() =>
            {

                if (builder.ToString().ToUpper().IndexOf("34 04 11 0F 00 47 ") != -1)
                {
                    strRec = "握手成功！";
                    ws = false;
                    wEvent.Set();   //此处表示握手成功
                }

                //此处是判断条件，根据不同返回数据在界面上显示不同的内容信息
                if (data.Length == 7)
                {
                    strRec = Recice(data[3], data[4], data[5], strRec);
                }


                if (data.Length == 16)
                {
                    strRec = ReciceDianJi(buffer[5], buffer[6], buffer[7], buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], strRec);
                }
                if (data.Length == 18)
                {
                    strRec = "运动到指定位置";
                }
                //在界面文本上打印信息
                this.txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + strRec + "--->" + builder.ToString() + "\r\n");

            });
            
            }

        private string ReciceDianJi(byte p1, byte p2, byte p3, byte p4, byte p5, byte p6, byte p7, byte p8, byte p9, byte p10,string s)
        {
            string d1 = ShujuChuli.DiGaoBaWei(p1, p2);
            string d2 = ShujuChuli.DiGaoBaWei(p3, p4);
            string d3 = ShujuChuli.DiGaoBaWei(p5, p6);
            string d4 = ShujuChuli.DiGaoBaWei(p7, p8);
            string d5 = ShujuChuli.DiGaoBaWei(p9, p10);
            s = "\r\n" + "电机1的位置：-->" + d1 + "\r\n" + "电机2的位置：-->" + d2 + "\r\n" + "电机3的位置：-->" + d3 + "\r\n" + "电机4的位置：-->" + d4 + "\r\n" + "电机5的位置：-->" + d5 + "\r\n";
            return s;
        }
     

        /// <summary>
        /// 数据接收判断
        /// </summary>
        /// <param name="MoKuai_buf"></param>
        /// <param name="DuanKou_buf"></param>
        /// <param name="ShuJu_buf"></param>
        /// <param name="str_txtXianShi"></param>
        /// <returns></returns>
        public string Recice(byte MoKuai_buf, byte DuanKou_buf, byte ShuJu_buf, string str_txtXianShi)
        {

            string qian = "-->OK";
            str_txtXianShi = "";
            #region  检测数据接收处理   pass/fail
            if (ShuJu_buf.ToString("x2") == "00" && MoKuai_buf.ToString("x2") == "01")
            {
              
                #region
                switch (DuanKou_buf.ToString("x2"))
                {

                    case "01": str_txtXianShi = "1站产品到位检测";
                        break;
                    case "02": str_txtXianShi = "1站防呆检测";
                        break;
                    case "03": str_txtXianShi = "1站USB拨出检测";
                        break;
                    case "04": str_txtXianShi = "1站耳机检测";
                        break;
                    case "05": str_txtXianShi = "1站关门检测";
                        break;
                    case "06": str_txtXianShi = "1站开门检测";
                        break;
                    case "07": str_txtXianShi = "1站红外感应检测";
                        break;
                    case "08": str_txtXianShi = "1站暗室打开检测";
                        break;

                    case "09": str_txtXianShi = "1站电机1原点检测";
                        break;
                    case "0a": str_txtXianShi = "1站电机1左边检测";
                        break;
                    case "0b": str_txtXianShi = "1站电机1右边检测";
                        break;
                    case "0c": str_txtXianShi = "1站上升1检测";
                        break;
                    case "0d": str_txtXianShi = "1站下降1检测";
                        break;
                    case "0e": str_txtXianShi = "1站吸嘴1检测";
                        break;

                    case "0f": str_txtXianShi = "1站电机2原点检测";
                        break;
                    case "10": str_txtXianShi = "1站电机2左边检测";
                        break;
                    case "11": str_txtXianShi = "1站电机2右边检测";
                        break;
                    case "12": str_txtXianShi = "1站上升2检测";
                        break;
                    case "13": str_txtXianShi = "1站下降2检测";
                        break;
                    case "14": str_txtXianShi = "1站吸嘴2检测";
                        break;

                    case "15": str_txtXianShi = "1站电机3原点检测";
                        break;
                    case "16": str_txtXianShi = "1站电机3左边检测";
                        break;
                    case "17": str_txtXianShi = "1站电机3右边检测";
                        break;
                    case "18": str_txtXianShi = "1站上升3检测";
                        break;
                    case "19": str_txtXianShi = "1站下降3检测";
                        break;
                    case "1a": str_txtXianShi = "1站吸嘴3检测";
                        break;

                    case "1b": str_txtXianShi = "1站电机4原点检测";
                        break;
                    case "1c": str_txtXianShi = "1站电机4左边检测";
                        break;
                    case "1d": str_txtXianShi = "1站右边4检测";
                        break;
                    case "1e": str_txtXianShi = "1站上升4检测";
                        break;
                    case "1f": str_txtXianShi = "1站下降4检测";
                        break;
                    case "20": str_txtXianShi = "1站吸嘴4检测";
                        break;

                    case "21": str_txtXianShi = "1站电机5原点到位检测";
                        break;
                    case "22": str_txtXianShi = "1站电机5左边到位检测";
                        break;
                    case "23": str_txtXianShi = "1站电机5右边到位检测";
                        break;
                    case "24": str_txtXianShi = "1站上升5到位检测";
                        break;
                    case "25": str_txtXianShi = "1站下降5到位检测";
                        break;
                    case "26": str_txtXianShi = "1站5到位检测";
                        break;

                    case "27": str_txtXianShi = "1站电机6原点到位检测";
                        break;
                    case "28": str_txtXianShi = "1站电机6左边到位检测";
                        break;
                    case "29": str_txtXianShi = "1站电机6右边到位检测";
                        break;
                    case "2a": str_txtXianShi = "1站上升6到位检测";
                        break;
                    case "2b": str_txtXianShi = "1站下降6到位检测";
                        break;
                    case "2c": str_txtXianShi = "1站6到位检测";
                        break;

                    case "2d": str_txtXianShi = "1站电机7原点到位检测";
                        break;
                    case "2e": str_txtXianShi = "1站电机7左边到位检测";
                        break;
                    case "2f": str_txtXianShi = "1站电机7右边到位检测";
                        break;
                    case "30": str_txtXianShi = "1站上升7到位检测";
                        break;
                    case "31": str_txtXianShi = "1站下降7到位检测";
                        break;
                    case "32": str_txtXianShi = "1站7到位检测";
                        break;

                    case "33": str_txtXianShi = "1站电机8原点到位检测";
                        break;
                    case "34": str_txtXianShi = "1站电机8左边到位检测";
                        break;
                    case "35": str_txtXianShi = "1站电机8右边到位检测";
                        break;
                    case "36": str_txtXianShi = "1站上升8到位检测";
                        break;
                    case "37": str_txtXianShi = "1站下降8到位检测";
                        break;
                    case "38": str_txtXianShi = "1站8到位检测";
                        break;
                    case "39": str_txtXianShi = "2-6站状态查询";
                        break;
                    case "3a": str_txtXianShi = "2-6站报警查询";
                        break;
                       
                }
#endregion
            }
            else if (ShuJu_buf.ToString("x2") == "ff" && MoKuai_buf.ToString("x2") == "01")
            {
                qian = "-->NO";
                    #region
                switch (DuanKou_buf.ToString("x2"))
                {

                    case "01": str_txtXianShi = "1站产品到位检测";
                        break;
                    case "02": str_txtXianShi = "1站防呆检测";
                        break;
                    case "03": str_txtXianShi = "1站USB拨出检测";
                        break;
                    case "04": str_txtXianShi = "1站耳机检测";
                        break;
                    case "05": str_txtXianShi = "1站关门检测";
                        break;
                    case "06": str_txtXianShi = "1站开门检测";
                        break;
                    case "07": str_txtXianShi = "1站红外感应检测";
                        break;
                    case "08": str_txtXianShi = "1站暗室打开检测";
                        break;

                    case "09": str_txtXianShi = "1站电机1原点检测";
                        break;
                    case "0a": str_txtXianShi = "1站电机1左边检测";
                        break;
                    case "0b": str_txtXianShi = "1站电机1右边检测";
                        break;
                    case "0c": str_txtXianShi = "1站上升1检测";
                        break;
                    case "0d": str_txtXianShi = "1站下降1检测";
                        break;
                    case "0e": str_txtXianShi = "1站吸嘴1检测";
                        break;

                    case "0f": str_txtXianShi = "1站电机2原点检测";
                        break;
                    case "10": str_txtXianShi = "1站电机2左边检测";
                        break;
                    case "11": str_txtXianShi = "1站电机2右边检测";
                        break;
                    case "12": str_txtXianShi = "1站上升2检测";
                        break;
                    case "13": str_txtXianShi = "1站下降2检测";
                        break;
                    case "14": str_txtXianShi = "1站吸嘴2检测";
                        break;

                    case "15": str_txtXianShi = "1站电机3原点检测";
                        break;
                    case "16": str_txtXianShi = "1站电机3左边检测";
                        break;
                    case "17": str_txtXianShi = "1站电机3右边检测";
                        break;
                    case "18": str_txtXianShi = "1站上升3检测";
                        break;
                    case "19": str_txtXianShi = "1站下降3检测";
                        break;
                    case "1a": str_txtXianShi = "1站吸嘴3检测";
                        break;

                    case "1b": str_txtXianShi = "1站电机4原点检测";
                        break;
                    case "1c": str_txtXianShi = "1站电机4左边检测";
                        break;
                    case "1d": str_txtXianShi = "1站上升4检测";
                        break;
                    case "1e": str_txtXianShi = "1站下降4检测";
                        break;
                    case "1f": str_txtXianShi = "1站下降4检测";
                        break;
                    case "20": str_txtXianShi = "1站吸嘴4检测";
                        break;

                    case "21": str_txtXianShi = "1站电机5原点到位检测";
                        break;
                    case "22": str_txtXianShi = "1站电机5左边到位检测";
                        break;
                    case "23": str_txtXianShi = "1站电机5右边到位检测";
                        break;
                    case "24": str_txtXianShi = "1站上升5到位检测";
                        break;
                    case "25": str_txtXianShi = "1站下降5到位检测";
                        break;
                    case "26": str_txtXianShi = "1站5到位检测";
                        break;

                    case "27": str_txtXianShi = "1站电机6原点到位检测";
                        break;
                    case "28": str_txtXianShi = "1站电机6左边到位检测";
                        break;
                    case "29": str_txtXianShi = "1站电机6右边到位检测";
                        break;
                    case "2a": str_txtXianShi = "1站上升6到位检测";
                        break;
                    case "2b": str_txtXianShi = "1站下降6到位检测";
                        break;
                    case "2c": str_txtXianShi = "1站6到位检测";
                        break;

                    case "2d": str_txtXianShi = "1站电机7原点到位检测";
                        break;
                    case "2e": str_txtXianShi = "1站电机7左边到位检测";
                        break;
                    case "2f": str_txtXianShi = "1站电机7右边到位检测";
                        break;
                    case "30": str_txtXianShi = "1站上升7到位检测";
                        break;
                    case "31": str_txtXianShi = "1站下降7到位检测";
                        break;
                    case "32": str_txtXianShi = "1站7到位检测";
                        break;

                    case "33": str_txtXianShi = "1站电机8原点到位检测";
                        break;
                    case "34": str_txtXianShi = "1站电机8左边到位检测";
                        break;
                    case "35": str_txtXianShi = "1站电机8右边到位检测";
                        break;
                    case "36": str_txtXianShi = "1站上升8到位检测";
                        break;
                    case "37": str_txtXianShi = "1站下降8到位检测";
                        break;
                    case "38": str_txtXianShi = "1站8到位检测";
                        break;
                    case "39": str_txtXianShi = "2-6站状态查询";
                        break;
                    case "3a": str_txtXianShi = "2-6站报警查询";
                        break;
                       
                }
                        #endregion

                
            }
            #endregion

            #region   气缸接收数据处理  //pass fail
            if (ShuJu_buf.ToString("x2")== "00" && MoKuai_buf.ToString("x2") == "02")
            {
            
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "手机固定";
                        break;
                    case "02": str_txtXianShi = "USB插入";
                        break;
                    case "03": str_txtXianShi = "耳机插入";
                        break;
                    case "04": str_txtXianShi = "N磁靠近";
                        break;
                    case "05": str_txtXianShi = "S磁靠近";
                        break;
                    case "06": str_txtXianShi = "前门开门";
                        break;
                    case "07": str_txtXianShi = "后门开门";
                        break;
                    case "08": str_txtXianShi = "升降1升起";
                        break;
                    case "09": str_txtXianShi = "取放1吸取";
                        break;

                    case "0a": str_txtXianShi = "升降2升起";
                        break;
                    case "0b": str_txtXianShi = "取放2吸取";
                        break;

                    case "0c": str_txtXianShi = "升降3升起";
                        break;
                    case "0d": str_txtXianShi = "取放3吸取";
                        break;

                    case "0e": str_txtXianShi = "升降4升起";
                        break;
                    case "0f": str_txtXianShi = "取放4吸取";
                        break;

                    case "10": str_txtXianShi = "升降5升起";
                        break;
                    case "11": str_txtXianShi = "取放5吸取";
                        break;

                    case "12": str_txtXianShi = "升降6升起";
                        break;
                    case "13": str_txtXianShi = "取放6吸取";
                        break;

                    case "14": str_txtXianShi = "升降7升起";
                        break;
                    case "15": str_txtXianShi = "取放7吸取";
                        break;

                    case "16": str_txtXianShi = "升降8升起";
                        break;
                    case "17": str_txtXianShi = "取放8吸取";
                        break;
                }
            }
            else if(ShuJu_buf.ToString("x2")== "ff" && MoKuai_buf.ToString("x2") == "02")
            {
             
                switch (DuanKou_buf.ToString("x2"))
                {
                   case "01": str_txtXianShi = "手机松开";
                        break;
                    case "02": str_txtXianShi = "USB拔出";
                        break;
                    case "03": str_txtXianShi = "耳机拔出";
                        break;
                    case "04": str_txtXianShi = "N磁远离";
                        break;
                    case "05": str_txtXianShi = "S磁远离";
                        break;
                    case "06": str_txtXianShi = "前门关门";
                        break;
                    case "07": str_txtXianShi = "后门关门";
                        break;
                    case "08": str_txtXianShi = "升降1下降";
                        break;
                    case "09": str_txtXianShi = "取放1放下";
                        break;

                    case "0a": str_txtXianShi = "升降2下降";
                        break;
                    case "0b": str_txtXianShi = "取放2放下";
                        break;

                    case "0c": str_txtXianShi = "升降3下降";
                        break;
                    case "0d": str_txtXianShi = "取放3放下";
                        break;

                    case "0e": str_txtXianShi = "升降4下降";
                        break;
                    case "0f": str_txtXianShi = "取放4放下";
                        break;

                    case "10": str_txtXianShi = "升降5下降";
                        break;
                    case "11": str_txtXianShi = "取放5放下";
                        break;

                    case "12": str_txtXianShi = "升降6下降";
                        break;
                    case "13": str_txtXianShi = "取放6放下";
                        break;

                    case "14": str_txtXianShi = "升降7下降";
                        break;
                    case "15": str_txtXianShi = "取放7放下";
                        break;

                    case "16": str_txtXianShi = "升降8下降";
                        break;
                    case "17": str_txtXianShi = "取放8放下";
                        break;
                }
            }
            #endregion

            #region  耳机接收数据处理

            if (ShuJu_buf == 0x00 && MoKuai_buf == 03)
            {
             
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "1站EAR-L";
                        break;
                    case "02": str_txtXianShi = "1站EAR-MIC打开";
                        break;
                    case "03": str_txtXianShi = "1站EAR-VOL+按下";
                        break;
                    case "04": str_txtXianShi = "1站EAR-VOL-按下";
                        break;
                    case "05": str_txtXianShi = "1站HOOK按下";
                        break;
                    case "06": str_txtXianShi = "1站国标耳机";
                        break;
                }
            }
            else if (ShuJu_buf == 0xFF && MoKuai_buf == 03)
            {
          
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "1站EAR-R";
                        break;
                    case "02": str_txtXianShi = "1站EAR-MIC关闭";
                        break;
                    case "03": str_txtXianShi = "1站EAR-VOL+弹起";
                        break;
                    case "04": str_txtXianShi = "1站EAR-VOL-弹起";
                        break;
                    case "05": str_txtXianShi = "1站HOOK弹起";
                        break;
                    case "06": str_txtXianShi = "1站美标耳机";
                        break;
                }
            }


            #endregion

            #region  机械手相关数据接收处理

            if ( MoKuai_buf == 04)
            {
                switch (DuanKou_buf.ToString("x2"))
	                {

                        case "08":str_txtXianShi="1站电机1运动";
                        break;
                        case "09":str_txtXianShi="1站电机2运动";
                        break;
                        case "0a":str_txtXianShi="1站电机3运动";
                        break;
                        case "0b":str_txtXianShi="1站电机4运动";
                        break;
                        case "0c":str_txtXianShi="1站电机5运动";
                        break;
                        
	
	                }
               
	
            }
            #endregion

            #region  状态指示数据接收处理

            if (ShuJu_buf == 0x00 && MoKuai_buf == 05)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "1站报警";
                        break;
                    case "02": str_txtXianShi = "1站状态正常";
                        break;
                }
            }
            else if (ShuJu_buf == 0xFF && MoKuai_buf == 05)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "1站正常";
                        break;
                    case "02": str_txtXianShi = "1站状态异常";
                        break;
                }
            }
            else if (MoKuai_buf == 0x05 && DuanKou_buf == 0x03)
            {
                switch (ShuJu_buf.ToString("x2"))
                {
                    case "00": str_txtXianShi = "1站状态指示灯pass";
                        break;
                    case "FF": str_txtXianShi = "1站状态指示灯fail";
                        break;
                    case "AA": str_txtXianShi = "1站状态指示灯灭";
                        break;
                }
            }


            #endregion


            //0x00所有到位正常、0x01USB拨出异常，0x02耳机拨出异常，0x04开门异常，0x08红外异常,0X10报警，0x20其余站状态异常，

            if ( MoKuai_buf == 07 && DuanKou_buf==01 )
            {
                switch (ShuJu_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "所有到位正常";
                        break;
                    case "02": str_txtXianShi = "USB拨出异常";
                        break;
                    case "04": str_txtXianShi = "开门异常";
                        break;
                    case "08": str_txtXianShi = "红外异常";
                        break;
                    case "10": str_txtXianShi = "报警";
                        break;
                    case "20": str_txtXianShi = "其余站状态异常";
                        break;
                }
            }
            else if (MoKuai_buf == 07 && DuanKou_buf == 02)
            {
                switch (ShuJu_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "清仓";
                        break;
                    case "02": str_txtXianShi = "正常测试";
                        break;
                }
            }


            if (MoKuai_buf.ToString() == "90" && DuanKou_buf.ToString() == "255")
            {
                str_txtXianShi = "复位成功";
            }
            str_txtXianShi += qian;
            return str_txtXianShi;
        }

        #region 冗余代码
 


        /// <summary>
        /// commbox和button联系
        /// </summary>
        /// <param name="com"></param>
        /// <param name="btn"></param>
        private void commboxSend(ComboBox com, Button btn)
        {


          
        }



        #endregion

        /// <summary>
        /// 串口打开按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       

        #region  状态指示


        //报警
        //private void checkBox21_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBox21.Checked)
        //    {
        //        SendHex(strData.t1);
        //        checkBox21.Text="1站报警";
        //    }
        //    else
        //    {
        //         SendHex(strData.t2);
        //         checkBox21.Text = "1站异常";
        //    }
          
        //}
        //状态
        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
           if (checkBox22.Checked)
            {
                SendHex(strData.t3);
                checkBox22.Text = "状态正常";
            }
            else
	        {
                 SendHex(strData.t4);
                checkBox22.Text="状态异常";
	        }
        }

        //指示
        private void button2_Click(object sender, EventArgs e)
        {
            switch (comBox_ZhiShiDeng_1.Text)
            {
                case "绿灯": SendHex(strData.t5);
                    break;
                case "红灯": SendHex(strData.t7);
                    break;
                case "黄灯": SendHex(strData.t6);
                    break;
            }
        }
        #endregion

      //ear- 按下
        private void button10_Click_1(object sender, EventArgs e)
        {
            SendHex(strData.r7);
        }
        //耳机-弹起
        private void button10_MouseUp(object sender, MouseEventArgs e)
        {
            SendHex(strData.r8);
        }

        //ear+ 按下
        private void button8_Click(object sender, EventArgs e)
        {
            SendHex(strData.r5);
        }
        //耳机+弹起
        private void button8_MouseUp(object sender, MouseEventArgs e)
        {
            SendHex(strData.r6);
        }

        //窗体关闭事件
        private void touMing_btn6_Click(object sender, EventArgs e)
        {
            Application.Exit();     //推出当前程序
            //  System.Environment.Exit(0);   推出当前程序，清空所有进程
        }

        //窗体最大化事件
        private void touMing_btn5_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

       

        #region   到位检测
        //1站产品到位检测
        private void button64_Click(object sender, EventArgs e)
        {
            SendHex(strData.a1);
        }
        //防呆检测
        private void button65_Click(object sender, EventArgs e)
        {
            SendHex(strData.a2);
        }

        //usb
        private void button66_Click(object sender, EventArgs e)
        {
            SendHex(strData.a3);
        }
        //耳机
        private void button67_Click(object sender, EventArgs e)
        {
            SendHex(strData.a4);
        }

        //关门
        private void button68_Click(object sender, EventArgs e)
        {
            SendHex(strData.a5);
        }
        //开门
        private void button69_Click(object sender, EventArgs e)
        {
            SendHex(strData.a6);
        }

        //红外感应
        private void button70_Click(object sender, EventArgs e)
        {
            SendHex(strData.a7);
        }

        //暗室打开
        private void button71_Click(object sender, EventArgs e)
        {
            SendHex(strData.a8);
        }
        #endregion

        #region   电机1
        //电机1原点
        private void button13_Click_1(object sender, EventArgs e)
        {
            SendHex(strData.a9);
        }

        //电机1左侧
        private void button17_Click(object sender, EventArgs e)
        {
            SendHex(strData.b1);
        }
        //电机1右侧
        private void button18_Click(object sender, EventArgs e)
        {
            SendHex(strData.b2);
        }

        //上升1
        private void button19_Click(object sender, EventArgs e)
        {
            SendHex(strData.b3);
        }

        //下降1
        private void button20_Click(object sender, EventArgs e)
        {
            SendHex(strData.b4);
        }

        //吸嘴1
        private void button21_Click(object sender, EventArgs e)
        {
            SendHex(strData.b5);
        }
        #endregion

        #region 电机2
        //电机2原点
        private void button22_Click(object sender, EventArgs e)
        {
            SendHex(strData.b6);
        }
        //电机2左侧
        private void button23_Click(object sender, EventArgs e)
        {
            SendHex(strData.b7);
        }
        //电机2右侧
        private void button24_Click(object sender, EventArgs e)
        {
            SendHex(strData.b8);
        }
        //上升2
        private void button25_Click(object sender, EventArgs e)
        {
            SendHex(strData.b9);
        }
        //下降2
        private void button26_Click(object sender, EventArgs e)
        {
            SendHex(strData.c1);
        }
        //吸嘴2
        private void button27_Click(object sender, EventArgs e)
        {
            SendHex(strData.c2);
        }

        #endregion

        #region 电机3
        //3原点
        private void button28_Click(object sender, EventArgs e)
        {
            SendHex(strData.c3);
        }
        //3左侧
        private void button29_Click(object sender, EventArgs e)
        {
            SendHex(strData.c4);
        }
        //3右侧
        private void button30_Click(object sender, EventArgs e)
        {
            SendHex(strData.c5);
        }
        //上升3
        private void button31_Click(object sender, EventArgs e)
        {
            SendHex(strData.c6);
        }
        //下降3
        private void button32_Click(object sender, EventArgs e)
        {
            SendHex(strData.c7);
        }
        //吸嘴3
        private void button33_Click(object sender, EventArgs e)
        {
            SendHex(strData.c8);
        }
        #endregion

        #region 4电机

        //原点
        private void button34_Click(object sender, EventArgs e)
        {
            SendHex(strData.c9);
        }
        //左侧
        private void button35_Click(object sender, EventArgs e)
        {
            SendHex(strData.d1);
        }
        //右侧
        private void button36_Click(object sender, EventArgs e)
        {
            SendHex(strData.d2);
        }
        //上升4
        private void button37_Click(object sender, EventArgs e)
        {
            SendHex(strData.d3);
        }
        //下降4
        private void button38_Click(object sender, EventArgs e)
        {
            SendHex(strData.d5);
        }
        //吸嘴4
        private void button39_Click(object sender, EventArgs e)
        {
            SendHex(strData.d6);
        }
        #endregion

        #region   电机5
        //原点
        private void button40_Click(object sender, EventArgs e)
        {
            SendHex(strData.d7);
        }

        //左侧
        private void button41_Click(object sender, EventArgs e)
        {
            SendHex(strData.d8);
        }
        //右侧
        private void button43_Click(object sender, EventArgs e)
        {
            SendHex(strData.d9);
        }
        //上升
        private void button52_Click(object sender, EventArgs e)
        {
            SendHex(strData.e1);
        }
        //下降
        private void button56_Click(object sender, EventArgs e)
        {
            SendHex(strData.e2);
        }
        //吸嘴
        private void button60_Click(object sender, EventArgs e)
        {
            SendHex(strData.e3);
        }
        #endregion

        #region 气缸

        //usb插入
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SendHex(strData.x3);
                checkBox1.Text = "USB拔出";
            }
            else
            {
                SendHex(strData.x4);
                checkBox1.Text = "USB插入";
            }
        }

        //手机固定
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                SendHex(strData.x1);
                checkBox2.Text = "手机松开";
            }
            else
            {
                SendHex(strData.x2);
                checkBox2.Text = "手机固定";
            }
        }

        //耳机插入
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                SendHex(strData.x5);
                checkBox3.Text = "耳机拔出";
            }
            else
            {
                SendHex(strData.x6);
                checkBox3.Text = "耳机插入";
            }
        }

        //N磁靠近
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                SendHex(strData.x7);
                checkBox4.Text = "N磁远离";
            }
            else
            {
                SendHex(strData.x8);
                checkBox4.Text = "N磁靠近";
            }
        }

        //S磁靠近
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                SendHex(strData.x9);
                checkBox5.Text = "S磁远离";
            }
            else
            {
                SendHex(strData.y1);
                checkBox5.Text = "S磁靠近";
            }
        }

        //前门开
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                SendHex(strData.y2);
                checkBox6.Text = "前门关门";
            }
            else
            {
                SendHex(strData.y3);
                checkBox6.Text = "前门开门";
            }
        }
        //后门开
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                SendHex(strData.y4);
                checkBox7.Text = "后门关门";
            }
            else
            {
                SendHex(strData.y5);
                checkBox7.Text = "后门开门";
            }
        }
        //升降1
        private void checkBox14_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox14.Checked)
            {
                SendHex(strData.y6);
                checkBox14.Text = "升降1降下";
            }
            else
            {
                SendHex(strData.y7);
                checkBox14.Text = "升降1升起";
            }
        }
        //升降2
        private void checkBox16_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox16.Checked)
            {
                SendHex(strData.z1);
                checkBox16.Text = "升降2下降";
            }
            else
            {
                SendHex(strData.z2);
                checkBox16.Text = "升降2升起";
            }
        }
        //升降3
        private void checkBox18_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox18.Checked)
            {
                SendHex(strData.z5);
                checkBox18.Text = "升降3下降";
            }
            else
            {
                SendHex(strData.z6);
                checkBox18.Text = "升降3升起";
            }
        }
        //升降4
        private void checkBox20_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox20.Checked)
            {
                SendHex(strData.z9);
                checkBox20.Text = "升降4升起";
            }
            else
            {
                SendHex(strData.q1);
                checkBox20.Text = "升降4下降";
            }
        }
        //升降5
        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                SendHex(strData.q4);
                checkBox11.Text = "升降5下降";
            }
            else
            {
                SendHex(strData.q5);
                checkBox11.Text = "升降5上升";
            }
        }
        //升降6
        private void checkBox23_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox23.Checked)
            {
                SendHex(strData.q8);
                checkBox23.Text = "升降6下降";
            }
            else
            {
                SendHex(strData.q9);
                checkBox23.Text = "升降6上升";
            }
        }
        //吸嘴1
        private void checkBox15_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox15.Checked)
            {
                SendHex(strData.y8);
                checkBox15.Text = "取放1放下";
            }
            else
            {
                SendHex(strData.y9);
                checkBox15.Text = "取放1吸取";
            }
        }
        //2
        private void checkBox17_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox17.Checked)
            {
                SendHex(strData.z3);
                checkBox17.Text = "取放2放下";
            }
            else
            {
                SendHex(strData.z4);
                checkBox17.Text = "取放2吸取";
            }
        }
        //3
        private void checkBox19_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox19.Checked)
            {
                SendHex(strData.z7);
                checkBox19.Text = "取放3放下";
            }
            else
            {
                SendHex(strData.z8);
                checkBox19.Text = "取放3吸取";
            }

        }
        //4
        private void checkBox10_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                SendHex(strData.q2);
                checkBox10.Text = "取放4放下";
            }
            else
            {
                SendHex(strData.q3);
                checkBox10.Text = "取放4吸取";
            }
        }
        //5
        private void checkBox25_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox25.Checked)
            {
                SendHex(strData.q6);
                checkBox25.Text = "取放5放下";
            }
            else
            {
                SendHex(strData.q7);
                checkBox25.Text = "取放5吸取";
            }
        }
        //6
        private void checkBox26_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox26.Checked)
            {
                SendHex(strData.q10);
                checkBox26.Text = "取放6放下";
            }
            else
            {
                SendHex(strData.q11);
                checkBox26.Text = "取放6吸取";
            }
        }
        #endregion

        //耳机
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                SendHex(strData.r1);
                checkBox8.Text = "EAR-R";
            }
            else
            {
                SendHex(strData.r2);
                checkBox8.Text = "EAR-L";
            }
        }

        //耳mic
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                SendHex(strData.r3);
                checkBox9.Text = "EAR-MIC关闭";
            }
            else
            {
                SendHex(strData.r4);
                checkBox9.Text = "EAR-MIC打开";
            }
        }
        //hook按下
        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
            {
                SendHex(strData.r9);
                checkBox12.Text = "HOOK弹起";
            }
            else
            {
                SendHex(strData.r10);
                checkBox12.Text = "HOOK按下";
            }
        }
        //国标耳机
        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox13.Checked)
            {
                SendHex(strData.r11);
                checkBox13.Text = "国标耳机";
            }
            else
            {
                SendHex(strData.r12);
                checkBox13.Text = "国标耳机";
            }
        }

       
        //清屏
        private void button1_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
            Update();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            checkAll(this);
            SendHex(strData.fuwei);
        }


        /// <summary>
        /// 使所有复选框为未选中状态
        /// </summary>
        /// <param name="c"></param>
        public void checkAll(Control c)
        {
            foreach (Control ct in c.Controls)
            {
                CheckBox cb = ct as CheckBox;
                if (cb != null)
                {
                    cb.Checked = false;
                }
                else
                {
                    checkAll(ct);
                }
            }
        }

        //测试状态
        private void button3_Click(object sender, EventArgs e)
        {
            if (comBox_ZT.Text == "测试中")
            {
                SendHex(strData.ceShiZhuangTai_1);
            }
            if (comBox_ZT.Text == "测试完成")
            {
                SendHex(strData.ceShiZhuangTai_2);
            }
        }

        //测试模式
        private void button4_Click(object sender, EventArgs e)
        {
            if (comMS.Text == "清仓")
            {
                SendHex(strData.MS_QC);
            }
            if (comMS.Text == "正常测试")
            {
                SendHex(strData.MS_ZC);
            }
        }

     
        ////1左
        //private void button5_Click(object sender, EventArgs e)
        //{
        //        SendHex(strData.yundong1_zuo);
        //}

        ////2左
        //private void button9_Click(object sender, EventArgs e)
        //{
        // SendHex(strData.yundong2_zuo);
        //}

        ////2中
        //private void button11_Click_1(object sender, EventArgs e)
        //{
        // SendHex(strData.yundong2_zhong);
        //}

        ////2右
        //private void button47_Click(object sender, EventArgs e)
        //{
        // SendHex(strData.yundong2_you);
        //}
        ////3
        //private void button14_Click(object sender, EventArgs e)
        //{
        //SendHex(strData.yundong3_zuo);
        
        //}
        ////3
        //private void button15_Click(object sender, EventArgs e)
        //{
        //SendHex(strData.yundong3_zhong);
        //}
        ////3
        //private void button48_Click(object sender, EventArgs e)
        //{
        //SendHex(strData.yundong3_you);
        //}
        ////4
        //private void button16_Click(object sender, EventArgs e)
        //{
        //SendHex(strData.yundong4_zuo);
        //}
        ////4
        //private void button42_Click(object sender, EventArgs e)
        //{
        //SendHex(strData.yundong4_zhong);
        //}
        ////4
        //private void button49_Click(object sender, EventArgs e)
        //{
        //SendHex(strData.yundong4_you);
        //}
        ////5
        //private void button44_Click(object sender, EventArgs e)
        //{
        //SendHex(strData.yundong5_zuo);
        //}
        ////5
        //private void button45_Click(object sender, EventArgs e)
        //{
        // SendHex(strData.yundong5_zhong);
        //}
        ////5
        //private void button50_Click(object sender, EventArgs e)
        //{
        // SendHex(strData.yundong5_you);
        //}
        ////报警
        //private void button53_Click(object sender, EventArgs e)
        //{
        //    SendHex(strData.BaoJin_26);
        //}
        ////状态
        //private void button51_Click(object sender, EventArgs e)
        //{
        //    SendHex(strData.ZhuangTai_26);
        //}

        private void button7_Click(object sender, EventArgs e)
        {

            string strHex = "72 05 11 04 08 " + ShujuChuli.StrToHex(comboBox1.Text) + " 81";
            SendHex(strHex);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string strHex = "72 05 11 04 09 " + ShujuChuli.StrToHex(comboBox2.Text) + " 81";
            SendHex(strHex);
        }

        private void button46_Click(object sender, EventArgs e)
        {
            string strHex = "72 05 11 04 0a " + ShujuChuli.StrToHex(comboBox3.Text) + " 81";
            SendHex(strHex);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string strHex = "72 05 11 04 0b " + ShujuChuli.StrToHex(comboBox4.Text) + " 81";
            SendHex(strHex);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string strHex = "72 05 11 02 18 " + ShujuChuli.StrToHex(baoJin_combox.Text) + " 81";
            SendHex(strHex);
        }

      
      

       

      


    }
}
 
}


                                          