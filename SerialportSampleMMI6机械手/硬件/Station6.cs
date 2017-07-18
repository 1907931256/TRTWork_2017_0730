using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Station
{
    public partial class Station6 : Form
    {
        private SerialPort comm = new SerialPort();//串口程序的主要处理类
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private static AutoResetEvent Send_JiXi_Event = new AutoResetEvent(false);
        private static AutoResetEvent wEvent = new AutoResetEvent(false);  //握手等待
        private static bool ws = true;//用于控制循环寻找串口
        private static bool ChaZhaoChuanKouCiShu;

        private static Byte[] buffer = new Byte[1024];//临时存放串口数据
        private static Byte[] data;//数据保存到该数组下面，用于判断操作
        private int _count;//记录数据长度

        private string strRec = "";
        private string dangQianWeiZhi_X = "";
        private string dangQianWeiZhi_Y = "";
        private static string qian = "";

        StationData_6 strData = new StationData_6();

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
            //this.Text = this.Width.ToString() + " " + this.Height.ToString();

        }


        #endregion

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

        public Station6()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }

        private void Station6_Load(object sender, EventArgs e)
        {
            #region 配合上面的代码实现了窗体按钮大小可变

            this.Resize += new EventHandler(Form1_Resize);

            X = this.Width;
            Y = this.Height;

            setTag(this);
            #endregion

            ChuShiHua();

            this.Text = Application.CompanyName + "   " + "Station6" + "   " + Application.ProductVersion;

            Action wo = new Action(woshou);

            wo.Invoke();

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
                                string s = "72 04 16 0f 00 81";
                                byte[] sbuf = ShujuChuli.HexStringToBytes(s);
                                comm.Write(sbuf, 0, sbuf.Length);

                                ChaZhaoChuanKouCiShu = wEvent.WaitOne(1000);//等待返回函数将mEvent置为mEvent.Set();

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
                        }
                        MessageBox.Show("无法找到串口！请确认硬件问题.");
                        a += 1;
                        if (ChaZhaoChuanKouCiShu == true)
                        {
                            break;
                        }
                        if (a == 5)
                        {
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
		
		
        private void ChuShiHua()
        {
            X_txt.Text = "0";
            Y_txt.Text = "10";
            SuDu_txt.Text = "3";
            JiaSuDu_txt.Text = "5";
            JianSuDu_txt.Text = "5";
            XieLv.Text = "0";
            comBox_ZhiShiDeng_1.Items.Add("绿灯");
            comBox_ZhiShiDeng_1.Items.Add("红灯");
            comBox_ZhiShiDeng_1.Items.Add("黄灯");
            comBox_ZhiShiDeng_1.SelectedIndex = 0;

            comBox_ZT.Items.Add("测试中");
            comBox_ZT.Items.Add("测试完成");
            comBox_ZT.SelectedIndex = 0;

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
                    //  Thread.Sleep(1);
                    continue;
                }
                //将读到的一字节数据存入缓存，这里需要做一转换
                buffer[_count] = Convert.ToByte(obj);
                /*
                1.判断数据的  头 尾 数据
                2.判断数据长度
                3.一系列的判断
				
                */

                if (buffer[0].ToString("x2") == "34")
                {
                    _count++;


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
                        break;
                    }

                }
            }



            //此处是一个线程工厂模式，利用（匿名委托）Lamd表达式，启用一个线程
            var Rec = Task.Factory.StartNew(() =>
            {
                if (builder.ToString().IndexOf("34 04 16 0F 00 47") != -1)
                {

                    strRec = "握手成功！";
                    ws = false;
                    wEvent.Set();   //此处表示握手成功
                }
                if (data.Length == 12)
                {
                    if (builder.ToString().IndexOf("34 0A 16 06 05 ") != -1)
                    {

                        dangQianWeiZhi_X = ShujuChuli.DiGaoBaWei(data[5], data[6]);
                        dangQianWeiZhi_Y = ShujuChuli.DiGaoBaWei(data[7], data[8]);
                        this.txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "X=" + dangQianWeiZhi_X + "\r\n" + "Y=" + dangQianWeiZhi_Y);
                        double x = Math.Abs(Convert.ToDouble(X_txt.Text) - Convert.ToDouble(dangQianWeiZhi_X));
                        double y = Math.Abs(Convert.ToDouble(Y_txt.Text) - Convert.ToDouble(dangQianWeiZhi_Y));
                     Send_JiXi_Event.Set();
                    }
                }


                if (data.Length == 7)
                {
                    strRec = Recice(data[3], data[4], data[5], strRec);
                }
                if (data.Length == 10)
                {
                    strRec = Recice_8(data[3], data[4], data[7], data[8], strRec);
                }
                if (_count == 6 && data[3].ToString("x2") == "5a" && data[4].ToString("x2") == "ff")
                {
                    strRec = "复位成功";
                }

                this.txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + strRec + "--->" + builder.ToString() + "\r\n");

            });

        }
        private string Recice_8(byte p1, byte p2, byte p3, byte p4, string str_txtXianShi)
        {
            if (p1.ToString("x2") == "04")
            {
                switch (p2.ToString("x2"))
                {
                    case "10": str_txtXianShi = "电压读取-->" + ShujuChuli.DiGaoBaWei(p3, p4) + "mv";
                        break;
                    case "11": str_txtXianShi = "充电电流读取-->" + ShujuChuli.DiGaoBaWei(p3, p4) + "mA";
                        break;
                    case "12": str_txtXianShi = "负载电流读取-->" + ShujuChuli.DiGaoBaWei(p3, p4) + "mA";
                        break;

                }

            }
            return str_txtXianShi;
        }

        private static string Recice(byte MoKuai_buf, byte DuanKou_buf, byte ShuJu_buf, string str_txtXianShi)
        {
            qian = "-->OK";
            str_txtXianShi = "";
            #region 到位检测
            if (MoKuai_buf.ToString("x2") == "01" && ShuJu_buf.ToString("x2") == "00")
            {

                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "6站产品到位检测";
                        break;
                    case "02": str_txtXianShi = "6站电机X位置到位检测";
                        break;
                    case "03": str_txtXianShi = "6站电机Y位置到位检测";
                        break;
                    case "04": str_txtXianShi = "6站电机Z位置到位检测";
                        break;
                    case "05": str_txtXianShi = "6站USB拨出到位检测";
                        break;
                    case "06": str_txtXianShi = "6站耳机拨出到位检测";
                        break;
                    case "07": str_txtXianShi = "6站取放下压到位检测";
                        break;
                    case "08": str_txtXianShi = "6站取放抬起到位检测";
                        break;
                    case "09": str_txtXianShi = "TP抬起到位检测";
                        break;
                    case "0a": str_txtXianShi = "吸合状态到位检测";
                        break;
                    case "0b": str_txtXianShi = "6站取放到位检测";
                        break;
                    case "0c": str_txtXianShi = "1站原点检测";
                        break;
                    case "0d": str_txtXianShi = "1站状态检测";
                        break;
                    case "0e": str_txtXianShi = "报警清除检测";
                        break;
                    case "0f": str_txtXianShi = "6站取放中间到位";
                        break;
                    case "10": str_txtXianShi = "TP按下到位";
                        break;





                }
            }
            else if (MoKuai_buf.ToString("x2") == "01" && ShuJu_buf.ToString("x2") == "ff")
            {
                qian = "-->NO";
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "6站产品到位检测";
                        break;
                    case "02": str_txtXianShi = "6站电机X位置到位检测";
                        break;
                    case "03": str_txtXianShi = "6站电机Y位置到位检测";
                        break;
                    case "04": str_txtXianShi = "6站电机Z位置到位检测";
                        break;
                    case "05": str_txtXianShi = "6站USB拨出到位检测";
                        break;
                    case "06": str_txtXianShi = "6站耳机拨出到位检测";
                        break;
                    case "07": str_txtXianShi = "6站取放下压到位检测";
                        break;
                    case "08": str_txtXianShi = "6站取放抬起到位检测";
                        break;
                    case "09": str_txtXianShi = "TP抬起到位检测";
                        break;
                    case "0a": str_txtXianShi = "吸合状态到位检测";
                        break;
                    case "0b": str_txtXianShi = "6站取放到位检测";
                        break;
                    case "0c": str_txtXianShi = "1站原点检测";
                        break;
                    case "0d": str_txtXianShi = "1站状态检测";
                        break;
                    case "0e": str_txtXianShi = "报警清除检测";
                        break;
                    case "0f": str_txtXianShi = "6站取放中间到位";
                        break;
                    case "10": str_txtXianShi = "TP按下到位";
                        break;
                }

            }
            #endregion

            #region 气缸
            if (MoKuai_buf.ToString("x2") == "02" && ShuJu_buf.ToString("x2") == "00")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "6站手机固定";
                        break;
                    case "02": str_txtXianShi = "站音量-按下";
                        break;
                    case "03": str_txtXianShi = "6站音量+按下";
                        break;
                    case "04": str_txtXianShi = "6站TP按下";
                        break;
                    case "05": str_txtXianShi = "6站USB插入";
                        break;
                    case "06": str_txtXianShi = "6站吸盘吸合";
                        break;
                    case "07": str_txtXianShi = "6站取放靠近";
                        break;
                    case "08": str_txtXianShi = "6站耳机插入";
                        break;
                    case "09": str_txtXianShi = "6站开机按下";
                        break;
                    case "0a": str_txtXianShi = "6站TOUCH按下";
                        break;
                }
            }
            else if (MoKuai_buf.ToString("x2") == "02" && ShuJu_buf.ToString("x2") == "ff")
            {

                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "6站手机松开";
                        break;
                    case "02": str_txtXianShi = "站音量-弹起";
                        break;
                    case "03": str_txtXianShi = "6站音量+弹起";
                        break;
                    case "04": str_txtXianShi = "6站TP弹起";
                        break;
                    case "05": str_txtXianShi = "6站USB拔出";
                        break;
                    case "06": str_txtXianShi = "6站吸盘松开";
                        break;
                    case "07": str_txtXianShi = "6站取放远离";
                        break;
                    case "08": str_txtXianShi = "6站耳机拔出";
                        break;
                    case "09": str_txtXianShi = "6站开机弹起";
                        break;
                    case "0a": str_txtXianShi = "6站TOUCH弹起";
                        break;
                }
            }
            else if (MoKuai_buf.ToString("x2") == "02" && DuanKou_buf.ToString("x2") == "07" && ShuJu_buf.ToString("x2") == "aa")
            {
                str_txtXianShi = "6站取放中";
            }



            #endregion

            #region 耳机
            if (MoKuai_buf.ToString("x2") == "03" && ShuJu_buf.ToString("x2") == "00")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "6站EAR-L";
                        break;
                    case "02": str_txtXianShi = "6站EAR-MIC打开";
                        break;
                    case "03": str_txtXianShi = "6站EAR-VOL+按下";
                        break;
                    case "04": str_txtXianShi = "6站EAR-VOL-按下";
                        break;
                    case "05": str_txtXianShi = "6站HOOK按下";
                        break;
                    case "06": str_txtXianShi = "6站国标耳机";
                        break;
                }
            }
            else if (MoKuai_buf.ToString("x2") == "03" && ShuJu_buf.ToString("x2") == "ff")
            {

                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "6站EAR-R";
                        break;
                    case "02": str_txtXianShi = "6站EAR-MIC关闭";
                        break;
                    case "03": str_txtXianShi = "6站EAR-VOL+弹起";
                        break;
                    case "04": str_txtXianShi = "6站EAR-VOL-弹起";
                        break;
                    case "05": str_txtXianShi = "6站HOOK弹起";
                        break;
                    case "06": str_txtXianShi = "6站美标耳机";
                        break;
                }
            }

            #endregion

            #region OTG-USB-电脑

            if (MoKuai_buf.ToString("x2") == "04" && ShuJu_buf.ToString("x2") == "00")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "00": str_txtXianShi = "电脑断开";
                        break;
                    case "01": str_txtXianShi = "充电关闭";
                        break;
                    case "02": str_txtXianShi = "OTG断开";
                        break;
                    case "03": str_txtXianShi = "OTG正插";
                        break;
                    case "04": str_txtXianShi = "负载连接";
                        break;

                }
            }
            else if (MoKuai_buf.ToString("x2") == "04" && ShuJu_buf.ToString("x2") == "ff")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "00": str_txtXianShi = "电脑连接";
                        break;
                    case "01": str_txtXianShi = "充电打开";
                        break;
                    case "02": str_txtXianShi = "OTG连接";
                        break;
                    case "03": str_txtXianShi = "OTG正插";
                        break;
                    case "04": str_txtXianShi = "负载断开";
                        break;

                }
            }



            #endregion

            #region  状态
            if (MoKuai_buf.ToString("x2") == "05" && ShuJu_buf.ToString("x2") == "00")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "6站报警";
                        break;
                    case "02": str_txtXianShi = "6站状态正常";
                        break;
                    case "03": str_txtXianShi = "2站绿灯";
                        break;
                }
            }
            else if (MoKuai_buf.ToString("x2") == "05" && ShuJu_buf.ToString("x2") == "ff")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "6站正常";
                        break;
                    case "02": str_txtXianShi = "6站状态异常";
                        break;
                    case "03": str_txtXianShi = "2站红灯";
                        break;
                }
            }
            else if (MoKuai_buf.ToString("x2") == "05" && ShuJu_buf.ToString("x2") == "aa" && DuanKou_buf.ToString("x2") == "03")
            {
                str_txtXianShi = "2站黄灯";
            }



            #endregion

            #region 电机




            #endregion
            str_txtXianShi += qian;
            return str_txtXianShi;
        }

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

        /// <summary>
        /// 获取当前位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            SendHex(strData.weiZhiHuoQu);
        }

        /// <summary>
        /// 运动指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            //button13_Click(null,null);
            //System.Threading.Thread.Sleep(200);

            string s = ShujuChuli.StrToGaoDiWei(X_txt.Text) + " " + ShujuChuli.StrToGaoDiWei(Y_txt.Text) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
            SendHex(strData.yunDong + " " + s + " " + 81);

        }

        private void JianSuDu_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (JianSuDu_txt.Text == "")
                    {
                        JianSuDu_txt.Text = "5";
                    }


                }
            }
        }

        private void SuDu_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (SuDu_txt.Text == "")
                    {
                        SuDu_txt.Text = "5";
                    }


                }
            }
        }

        private void JaSuDu_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (JiaSuDu_txt.Text == "")
                    {
                        JiaSuDu_txt.Text = "5";
                    }


                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            SendHex(strData.huiLingDian);
        }

        /// <summary>
        /// 斜率配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {

            Console.WriteLine("X=" + dangQianWeiZhi_X);
            Console.WriteLine("Y=" + dangQianWeiZhi_Y);
            double x = Math.Abs(Convert.ToDouble(X_txt.Text) - Convert.ToDouble(dangQianWeiZhi_X));
            double y = Math.Abs(Convert.ToDouble(Y_txt.Text) - Convert.ToDouble(dangQianWeiZhi_Y));

            if (x == 0 || y == 0)
            {
                XieLv.Text = "0";
            }
            else if (x - y > 0)
            {

                XieLv.Text = Convert.ToString(Math.Round(65536 * y / x));
                //XieLv.Text = Convert.ToString(Math.Round(((y / x)>>16) &255));
            }
            else
            {
                XieLv.Text = Convert.ToString(Math.Round(65536 * x / y));
            }
        }

        private void Y_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (char)Keys.Enter)
                {
                    button14_Click(null, null);
                }
            }
        }

        private void Test_Click(object sender, EventArgs e)
        {
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                //当后台线程操作完成时发执行
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkComper);
                //调用 bw.RunWorkerAsync时发生  及调用后台线程是开始执行
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                //开始执行后台操作
                bw.RunWorkerAsync("后台操作执行完成--测试次数时：");
            }
            #region


            //string s = ShujuChuli.StrToGaoDiWei(x1.Text) + " " + ShujuChuli.StrToGaoDiWei(y1.Text) + " " + Convert.ToInt16(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JianSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(XieLv.Text).ToString("x2");
            //  SendHex(strData.yunDong + " " + s + " " + 81);
            //  System.Threading.Thread.Sleep(2000);


            //  s = ShujuChuli.StrToGaoDiWei(x2.Text) + " " + ShujuChuli.StrToGaoDiWei(y2.Text) + " " + Convert.ToInt16(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JianSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(XieLv.Text).ToString("x2");
            //  SendHex(strData.yunDong + " " + s + " " + 81);
            //  System.Threading.Thread.Sleep(2000);

            //  s = ShujuChuli.StrToGaoDiWei(x2.Text) + " " + ShujuChuli.StrToGaoDiWei(y1.Text) + " " + Convert.ToInt16(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JianSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(XieLv.Text).ToString("x2");
            //  SendHex(strData.yunDong + " " + s + " " + 81);
            //  System.Threading.Thread.Sleep(2000);


            //  s = ShujuChuli.StrToGaoDiWei(x1.Text) + " " + ShujuChuli.StrToGaoDiWei(y2.Text) + " " + Convert.ToInt16(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JianSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(XieLv.Text).ToString("x2");
            //  SendHex(strData.yunDong + " " + s + " " + 81);
            //  System.Threading.Thread.Sleep(2000);

            //  s = ShujuChuli.StrToGaoDiWei(x1.Text) + " " + ShujuChuli.StrToGaoDiWei(y1.Text) + " " + Convert.ToInt16(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(JianSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt16(XieLv.Text).ToString("x2");
            //  SendHex(strData.yunDong + " " + s + " " + 81);
            //  System.Threading.Thread.Sleep(2000);
            #endregion
        }


        /// <summary>
        /// 执行后天程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        /// <summary>
        /// 后台线程完成时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkComper(object sender, RunWorkerCompletedEventArgs e)
        {

        }



        //复位按钮
        private void button18_Click(object sender, EventArgs e)
        {
            SendHex(strData.fuwei);
        }

        #region  状态检测
        //产品到位
        private void button6_Click(object sender, EventArgs e)
        {
            SendHex(strData.canPinDaoWei_JC);
        }

        //电机X位置
        private void button7_Click(object sender, EventArgs e)
        {
            SendHex(strData.dianJi_X_JC);
        }

        //电机Y位置
        private void button9_Click(object sender, EventArgs e)
        {
            SendHex(strData.dianJi_Y_JC);
        }

        //电机Z位置
        private void button21_Click(object sender, EventArgs e)
        {
            SendHex(strData.erJi_JC);
        }

        //usb检测
        private void button12_Click(object sender, EventArgs e)
        {
            SendHex(strData.USB_JC);
        }

        //耳机拔出检测
        private void button11_Click(object sender, EventArgs e)
        {
            SendHex(strData.erJi_BaChu_JC);
        }

        //取放下压检查
        private void button22_Click(object sender, EventArgs e)
        {
            SendHex(strData.quFang_XiaYa_JC);
        }

        //取放抬起
        private void button23_Click(object sender, EventArgs e)
        {
            SendHex(strData.quFang_TaiQI_JC);
        }

        //吸合状态检测
        private void button25_Click(object sender, EventArgs e)
        {
            SendHex(strData.xiHeZhuangTai_JC);
        }

        //tp抬起到位
        private void button24_Click(object sender, EventArgs e)
        {
            SendHex(strData.TpTaiQi_DaoWei_JC);
        }

        //去放到位
        private void button26_Click(object sender, EventArgs e)
        {
            SendHex(strData.quFangDaoWei_JC);
        }
        #endregion

        #region 气缸控制

        //手机固定
        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox19.Checked)
            {
                SendHex(strData.shouJiGuDing_QG);
                checkBox19.Text = "6站手机松开";
            }
            else
            {
                SendHex(strData.shouJiSongKai_QG);
                checkBox19.Text = "6站手机固定";
            }
        }

        //TP按下
        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox18.Checked)
            {
                SendHex(strData.jinJiao_KaoJin_QG);
                checkBox18.Text = "6站TP按下";
            }
            else
            {
                SendHex(strData.jinJiao_YuanLi_QG);
                checkBox18.Text = "6站TP弹起";
            }
        }

        //Usb插入
        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox17.Checked)
            {
                SendHex(strData.jinJiao_ShangShen_QG);
                checkBox17.Text = "6站USB拔出";
            }
            else
            {
                SendHex(strData.jinJiao_XiaJiang_QG);
                checkBox17.Text = "6站USB插入";
            }
        }

        //吸盘吸和
        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox15.Checked)
            {
                SendHex(strData.xiHe_KaoJin_QG);
                checkBox15.Text = "6站吸盘松开";
            }
            else
            {
                SendHex(strData.xiHe_YuanLi_QG);
                checkBox15.Text = "6站吸盘吸合";
            }
        }

       

        // 耳机插入
        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox14.Checked)
            {
                SendHex(strData.erJi_KaoJin_QG);
                checkBox14.Text = "6站耳机拔出";
            }
            else
            {
                SendHex(strData.erJi_YuanLi_QG);
                checkBox14.Text = "6站耳机插入";
            }
        }

        //开机按下
        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                SendHex(strData.kaiJi_KaoJin_QG);
                checkBox11.Text = "6站开机弹起";
            }
            else
            {
                SendHex(strData.kaiJi_YuanLi_QG);
                checkBox11.Text = "6站开机按下";
            }
        }

        //touch按下
        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                SendHex(strData.touch_KaoJin_QG);
                checkBox10.Text = "6站TOUCH按下";
            }
            else
            {
                SendHex(strData.touch_YuanLi_QG);
                checkBox10.Text = "6站TOUCH弹起";
            }
        }

        //音量—按下
        private void button27_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SendHex(strData.jiaoZhunGuanYuan_KaoJin_QG);
            }
           
        }
        private void button27_MouseUp(object sender, MouseEventArgs e)
        {
            SendHex(strData.jiaoZhunGuanYuan_YuanLi_QG);
        }
        //音量加
        private void button28_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SendHex(strData.zhenJu_KaoJin_QG);
            }
        }
        private void button28_MouseUp(object sender, MouseEventArgs e)
        {
            SendHex(strData.zhenJu_YuanLi_QG);
        }
      
        #endregion

        #region  OTG-电脑-电压
        //电脑连接
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SendHex(strData.dianNao_LianJie);
                checkBox1.Text = "电脑断开";
            }
            else
            {
                SendHex(strData.dianNao_DuanKai);
                checkBox1.Text = "电脑连接";
            }
        }

        //充电打开
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                SendHex(strData.congDian_DaKai);
                checkBox2.Text = "充电关闭";
            }
            else
            {
                SendHex(strData.congDian_GuanBi);
                checkBox2.Text = "充电打开";
            }
        }

        //otg连接
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                SendHex(strData.OTG_LianJie);
                checkBox3.Text = "OTG断开";
            }
            else
            {
                SendHex(strData.OTG_DuanKia);
                checkBox3.Text = "OTG连接";
            }
        }

        //otg反插
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                SendHex(strData.OTG_ZhengCha);
                checkBox4.Text = "OTG反插";
            }
            else
            {
                SendHex(strData.OTG_FanCha);
                checkBox4.Text = "OTG正插";
            }
        }

        //负载连接
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                SendHex(strData.fuZai_LianJie);
                checkBox5.Text = "负载断开";
            }
            else
            {
                SendHex(strData.fuZai_DuanKai);
                checkBox5.Text = "负载连接";
            }

        }

        //负载电流读取
        private void button4_Click(object sender, EventArgs e)
        {
            SendHex(strData.fuZai_DianLu_DuQu);
        }

        //充电电流读取
        private void button5_Click(object sender, EventArgs e)
        {
            SendHex(strData.congDian_DianLu_DuQu);
        }

        //电压读取
        private void button1_Click(object sender, EventArgs e)
        {
            SendHex(strData.dianYa_DuQu);
        }
        #endregion


        #region   耳机控制

        //耳机L-R
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                SendHex(strData.ear_R);
                checkBox8.Text = "6站EAR-R";
            }
            else
            {
                SendHex(strData.ear_L);
                checkBox8.Text = "6站EAR-L";
            }

        }

        //耳机MIC 打开关闭
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                SendHex(strData.ear_Mic_Open);
                checkBox9.Text = "6站EAR-MIC关闭";
            }
            else
            {
                SendHex(strData.ear_Mic_Close);
                checkBox9.Text = "6站EAR-MIC打开";
            }
        }

        //HOOK按下
        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
            {
                SendHex(strData.HOOK_TanQi);
                checkBox12.Text = "6站HOOK按下";
            }
            else
            {
                SendHex(strData.HOOK_AnXia);
                checkBox12.Text = "6站HOOK抬起";
            }
        }

        //耳机切换
        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox13.Checked)
            {
                SendHex(strData.guoBiao_EAR);
                checkBox13.Text = "6站美标耳机";
            }
            else
            {
                SendHex(strData.meiBiao_EAR);
                checkBox13.Text = "6站国标耳机";
            }
        }

        //耳机+按下
        private void button8_Click(object sender, EventArgs e)
        {
            SendHex(strData.ear_VolJia_Down);

        }

        //耳机-按下
        private void button10_Click(object sender, EventArgs e)
        {
            SendHex(strData.ear_VOLJian_Down);
        }

        #endregion



        //报警
        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked)
            {
                SendHex(strData.baoJin_BaoJin);
                checkBox21.Text = "6站报警正常";
            }
            else
            {
                SendHex(strData.zhengChang_BaoJin);
                checkBox21.Text = "6站报警正常";
            }
        }

        //清屏
        private void button3_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
            Update();
        }

        //状态
        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox22.Checked)
            {
                SendHex(strData.zhenChang_ZhuangTai);
                checkBox22.Text = "2站状态正常";
            }
            else
            {
                SendHex(strData.yiChang_ZhuangTai);
                checkBox22.Text = "2站状态异常";
            }
        }


        //状态指示
        private void button2_Click(object sender, EventArgs e)
        {
            switch (comBox_ZhiShiDeng_1.Text)
            {
                case "绿灯": SendHex(strData.pass_SanSeDeng);
                    break;
                case "红灯": SendHex(strData.fali_SanSeDeng);
                    break;
                case "黄灯": SendHex(strData.stop_SanSeDeng);
                    break;
            }
        }

        private void comboBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboPortName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        //原点
        private void button18_Click_1(object sender, EventArgs e)
        {
            SendHex(strData.zhaungtai_1);
        }
        //状态
        private void button17_Click(object sender, EventArgs e)
        {
            SendHex(strData.zhaungtai_2);
        }
        //报警清除检测
        private void button19_Click(object sender, EventArgs e)
        {
            SendHex(strData.zhaungtai_3);
        }

        private void button20_Click(object sender, EventArgs e)
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

        private void button29_Click(object sender, EventArgs e)
        {
            SendHex(strData.zhaungtai_4);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            SendHex(strData.zhaungtai_5);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            SendHex(strData.xiFang_zj_QG);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            SendHex(strData.xiFang_KaoJin_QG);
        }

        private void button32_Click(object sender, EventArgs e)
        {
                SendHex(strData.xiFang_YuanLi_QG);
          
        }

        //电机向上移动指定距离
        //private void button34_Click(object sender, EventArgs e)
        //{
        //    SendHex(strData.weiZhiHuoQu);
        //    Send_JiXi_Event.WaitOne();
        //   int move=Convert.ToInt32(Move_txt.Text);
        //   string x= (Convert.ToInt32(dangQianWeiZhi_X) + move).ToString();
        //    string s = ShujuChuli.StrToGaoDiWei(x) + " " + ShujuChuli.StrToGaoDiWei(Y_txt.Text) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
        //    SendHex(strData.yunDong + " " + s + " " + 81);
        //}

        //private void button35_Click(object sender, EventArgs e)
        //{
        //    SendHex(strData.weiZhiHuoQu);
        //    Send_JiXi_Event.WaitOne();
        //    int move = Convert.ToInt32(Move_txt.Text);
        //    string x = (Convert.ToInt32(dangQianWeiZhi_X) -move).ToString();
        //    string s = ShujuChuli.StrToGaoDiWei(x) + " " + ShujuChuli.StrToGaoDiWei(Y_txt.Text) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
        //    SendHex(strData.yunDong + " " + s + " " + 81);
        //}

        //private void button36_Click(object sender, EventArgs e)
        //{
        //    SendHex(strData.weiZhiHuoQu);
        //    Send_JiXi_Event.WaitOne();
        //    int move = Convert.ToInt32(Move_txt.Text);
        //    string x = (Convert.ToInt32(dangQianWeiZhi_Y) + move).ToString();
        //    string s = ShujuChuli.StrToGaoDiWei(dangQianWeiZhi_X) + " " + ShujuChuli.StrToGaoDiWei(x) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
        //    SendHex(strData.yunDong + " " + s + " " + 81);
        //}

        //private void button37_Click(object sender, EventArgs e)
        //{
        //    SendHex(strData.weiZhiHuoQu);
        //    Send_JiXi_Event.WaitOne();
        //    int move = Convert.ToInt32(Move_txt.Text);
        //    string x = (Convert.ToInt32(dangQianWeiZhi_Y) - move).ToString();
        //    string s = ShujuChuli.StrToGaoDiWei(dangQianWeiZhi_X) + " " + ShujuChuli.StrToGaoDiWei(x) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
        //    SendHex(strData.yunDong + " " + s + " " + 81);
        //}


        //private void Move_txt_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Up)//上
        //    {
        //        SendHex(strData.weiZhiHuoQu);
        //        Send_JiXi_Event.WaitOne();
        //        int move = Convert.ToInt32(Move_txt.Text);
        //        string x = (Convert.ToInt32(dangQianWeiZhi_X) - move).ToString();
        //        string s = ShujuChuli.StrToGaoDiWei(x) + " " + ShujuChuli.StrToGaoDiWei(Y_txt.Text) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
        //        SendHex(strData.yunDong + " " + s + " " + 81);
        //    }
        //    else if (e.KeyCode == Keys.Down)//下
        //    {
        //        SendHex(strData.weiZhiHuoQu);
        //        Send_JiXi_Event.WaitOne();
        //        int move = Convert.ToInt32(Move_txt.Text);
        //        string x = (Convert.ToInt32(dangQianWeiZhi_X) + move).ToString();
        //        string s = ShujuChuli.StrToGaoDiWei(x) + " " + ShujuChuli.StrToGaoDiWei(Y_txt.Text) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
        //        SendHex(strData.yunDong + " " + s + " " + 81);
        //    }
        //    else if (e.KeyCode==Keys.Left)//左
        //    {

        //        SendHex(strData.weiZhiHuoQu);
        //        Send_JiXi_Event.WaitOne();
        //        int move = Convert.ToInt32(Move_txt.Text);
        //        string x = (Convert.ToInt32(dangQianWeiZhi_Y) - move).ToString();
        //        string s = ShujuChuli.StrToGaoDiWei(X_txt.Text) + " " + ShujuChuli.StrToGaoDiWei(x) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
        //        SendHex(strData.yunDong + " " + s + " " + 81);
        //    }
        //    else if (e.KeyCode==Keys.Right)//右
        //    {
        //        SendHex(strData.weiZhiHuoQu);
        //        Send_JiXi_Event.WaitOne();
        //        int move = Convert.ToInt32(Move_txt.Text);
        //        string x = (Convert.ToInt32(dangQianWeiZhi_Y) + move).ToString();
        //        string s = ShujuChuli.StrToGaoDiWei(X_txt.Text) + " " + ShujuChuli.StrToGaoDiWei(x) + " " + Convert.ToInt32(SuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JiaSuDu_txt.Text).ToString("x2") + " " + Convert.ToInt32(JianSuDu_txt.Text).ToString("x2") + " " + ShujuChuli.StrToGaoDiWei(XieLv.Text);
        //        SendHex(strData.yunDong + " " + s + " " + 81);
        //    }
        //}

       


    }
}
