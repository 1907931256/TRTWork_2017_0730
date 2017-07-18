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
    public partial class Station5 : Form
    {
        private SerialPort comm = new SerialPort();//串口程序的主要处理类
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private static AutoResetEvent SendEvent = new AutoResetEvent(false);
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

        StationData_5 strData = new StationData_5();

        ShujuChuli ChuLi = new ShujuChuli();
        
  
      

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
            this.Text = this.Width.ToString() + " " + this.Height.ToString();

        }


        #endregion

        public Station5()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }

        /// <summary>
        /// 数据以16进制发送
        /// </summary>
        /// <param name="strParam"></param>
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

        private void Station5_Load(object sender, EventArgs e)
        {
            #region 配合上面的代码实现了窗体按钮大小可变

            this.Resize += new EventHandler(Form1_Resize);

            X = this.Width;
            Y = this.Height;

            setTag(this);
            #endregion


            JiaZai();
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
                                string s = "72 04 15 0f 00 81";
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

        public void JiaZai()
        {
            #region 初始化数据加载
            comBox_ZhiShiDeng_1.Items.Add("红灯");
            comBox_ZhiShiDeng_1.Items.Add("绿灯");
            comBox_ZhiShiDeng_1.Items.Add("黄灯");
            comBox_ZhiShiDeng_1.SelectedIndex = 1;

            comBox_ZT.Items.Add("测试中");
            comBox_ZT.Items.Add("测试完成");
            comBox_ZT.SelectedIndex = 0;
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
                if (builder.ToString().ToUpper().IndexOf("34 04 15 0F 00 47 ") != -1)
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
                //在界面文本上打印信息
                this.txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + strRec + "--->" + builder.ToString() + "\r\n");
            });


        }

        public static string Recice(byte MoKuai_buf, byte DuanKou_buf, byte ShuJu_buf, string str_txtXianShi)
        {
            str_txtXianShi = "";
            string qian = "-->OK";
            #region 状态检测
            if (MoKuai_buf.ToString("x2") == "01" && ShuJu_buf.ToString("x2") == "00")
            {
               switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "5站产品到位检测";
                        break;
                    case "02": str_txtXianShi = "5站人工嘴下降检测";
                        break;
                    case "03": str_txtXianShi = "5站人工耳上升检测";
                        break;
                    case "04": str_txtXianShi = "5站隔离上升检测";
                        break;
                    case "05": str_txtXianShi = "5站耳机拨出检测";
                        break;
                    case "06": str_txtXianShi = "5站隔离关闭检测";
                        break;
                    case "07": str_txtXianShi = "1站原点状态检测";
                        break;
                    case "08": str_txtXianShi = "1站状态检测";
                        break;
                    case "09": str_txtXianShi = "报警清除";
                        break;
                   
                }
            }
            else if (MoKuai_buf.ToString("x2") == "01" && ShuJu_buf.ToString("x2") == "ff")
            {
                qian = "-->NO";
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "5站产品到位检测";
                        break;
                    case "02": str_txtXianShi = "5站人工嘴下降检测";
                        break;
                    case "03": str_txtXianShi = "5站人工耳上升检测";
                        break;
                    case "04": str_txtXianShi = "5站隔离上升检测";
                        break;
                    case "05": str_txtXianShi = "5站耳机拨出检测";
                        break;
                    case "06": str_txtXianShi = "5站隔离关闭检测";
                        break;
                    case "07": str_txtXianShi = "1站原点状态检测";
                        break;
                    case "08": str_txtXianShi = "1站状态检测";
                        break;
                    case "09": str_txtXianShi = "报警清除";
                        break;
                }
            }
            
            #endregion

            #region 气缸
            if (MoKuai_buf.ToString("x2") == "02" && ShuJu_buf.ToString("x2") == "00")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "5站手机固定";
                        break;
                    case "02": str_txtXianShi = "5站USB插入";
                        break;
                    case "03": str_txtXianShi = "5站隔音箱上升";
                        break;
                    case "04": str_txtXianShi = "5站人工嘴上升";
                        break;
                    case "05": str_txtXianShi = "5站人工嘴左移";
                        break;
                    case "06": str_txtXianShi = "5站人工耳上升";
                        break;
                    case "07": str_txtXianShi = "5站耳机插入";
                        break;
                }
            }
            else if (MoKuai_buf.ToString("x2") == "02" && ShuJu_buf.ToString("x2") == "ff")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "5站手机松开";
                        break;
                    case "02": str_txtXianShi = "5站USB拔出";
                        break;
                    case "03": str_txtXianShi = "5站隔音箱下降";
                        break;
                    case "04": str_txtXianShi = "5站人工嘴下降";
                        break;
                    case "05": str_txtXianShi = "5站人工嘴右移";
                        break;
                    case "06": str_txtXianShi = "5站人工耳下降";
                        break;
                    case "07": str_txtXianShi = "5站耳机拔出";
                        break;
                }
            }
            #  endregion

            #region  状态指示数据接收处理

            if (ShuJu_buf.ToString("x2") == "00" && MoKuai_buf.ToString("x2") =="05")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "5站报警";
                        break;
                    case "02": str_txtXianShi = "5站状态正常";
                        break;
                    case "03": str_txtXianShi = "5站状态指示绿灯";
                        break;
                }
            }
            else if (ShuJu_buf.ToString("x2") == "ff" && MoKuai_buf.ToString("x2") == "05")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "5站报警正常";
                        break;
                    case "02": str_txtXianShi = "5站状态异常";
                        break;
                    case "03": str_txtXianShi = "5站状态指示红灯";
                        break;
                }
            }
            else if (DuanKou_buf.ToString("x2") == "03" && MoKuai_buf.ToString("x2") == "05" && ShuJu_buf.ToString("x2")=="aa")
            {
                str_txtXianShi = "5站状态指示黄灯";
                       
            }


            #endregion

            if (DuanKou_buf.ToString("x2") == "01" && MoKuai_buf.ToString("x2") == "06")
            {
                //0X00状态正常，0x01USB异常，0x02隔音箱异常，0x04人工嘴上升异常，0x08
                switch (ShuJu_buf.ToString("x2"))
                {
                    case "00": str_txtXianShi = "5站状态正常";
                        break;
                    case "01": str_txtXianShi = "5站1USB异常";
                        break;
                    case "03": str_txtXianShi = "5站隔音箱异常";
                        break;
                    case "04": str_txtXianShi = "5站人工嘴上升异常";
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


        #region 气缸
        //手机固定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SendHex(strData.a1);
                checkBox1.Text = "4站手机松开";
            }
            else
            {
                SendHex(strData.b1);
                checkBox1.Text = "4站手机固定";
            }
        }
        //usb插拔
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                SendHex(strData.a2);
                checkBox2.Text = "5站USB拔出";
            }
            else
            {
                SendHex(strData.b2);
                checkBox2.Text = "5站USB插入";
            }
        }

        //隔离箱上升
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                SendHex(strData.a3);
                checkBox3.Text = "5站隔音箱下降";
            }
            else
            {
                SendHex(strData.b3);
                checkBox3.Text = "5站隔音箱上升";
            }
        }

        //人工嘴上升
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                SendHex(strData.a4);
                checkBox4.Text = "5站人工嘴下降";
            }
            else
            {
                SendHex(strData.b4);
                checkBox4.Text = "5站人工嘴上升";
            }
        }

        //人工嘴左
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                SendHex(strData.a5);
                checkBox5.Text = "5站人工嘴右移";
            }
            else
            {
                SendHex(strData.b5);
                checkBox5.Text = "5站人工嘴左移";
            }
        }

        //人工嘴上升
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                SendHex(strData.a6);
                checkBox6.Text = "5站人工耳下降";
            }
            else
            {
                SendHex(strData.b6);
                checkBox6.Text = "5站人工耳上升";
            }
        }

        //耳机插入
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                SendHex(strData.a7);
                checkBox7.Text = "5站耳机拔出";
            }
            else
            {
                SendHex(strData.b7);
                checkBox7.Text = "5站耳机插入";
            }
        }
#endregion

        #region  状态检测
        //产品到位检测
        private void button1_Click(object sender, EventArgs e)
        {
            SendHex(strData.canPingDaoWei_JC);
        }

        //人工耳上升检测
        private void button3_Click(object sender, EventArgs e)
        {
            SendHex(strData.guanXiang_JC);
        }

        //耳机拔出检测
        private void button5_Click(object sender, EventArgs e)
        {
            SendHex(strData.renGongZui_JC);
        }

        //人工耳下降检测
        private void button2_Click(object sender, EventArgs e)
        {
            SendHex(strData.aa2);
        }

        //人工耳下降检测
        private void button4_Click(object sender, EventArgs e)
        {
            SendHex(strData.kaiXiang_JC);

        }

        //usb拔出检测
        private void button6_Click(object sender, EventArgs e)
        {
            SendHex(strData.aa1);
        }
        #endregion

        #region 状态
        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked)
            {
                SendHex(strData.baoJin);
                checkBox21.Text = "5站报警正常";
            }
            else
            {
                SendHex(strData.zhengChang);
                checkBox21.Text = "5站报警";
            }
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox22.Checked)
            {
                SendHex(strData.zhuangTaiZhengChang);
                checkBox22.Text = "5站状态正常";
            }
            else
            {
                SendHex(strData.zhuangTaiYiChang);
                checkBox22.Text = "5站状态异常";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            switch (comBox_ZhiShiDeng_1.Text)
            {
                case "绿灯": SendHex(strData.zhuangTaiZhiDeng_Pass);
                    break;
                case "红灯": SendHex(strData.zhuangTaiZhiDeng_Fail);
                    break;
                case "黄灯": SendHex(strData.zhuangTaiZhiDeng_Mie);
                    break;
            }
        }

        private void buttonXin4_Click(object sender, EventArgs e)
        {
            SendHex(strData.fuwei);
        }
        #endregion

        
        private void buttonXin6_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
            Update();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SendHex(strData.renEr_JC);
        }

        private void button8_Click(object sender, EventArgs e)
        {

            SendHex(strData.aa3);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SendHex(strData.aa4);
        }

        private void button12_Click(object sender, EventArgs e)
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

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                SendHex(strData.A1);
                checkBox8.Text = "5站振动打开";
            }
            else
            {
                SendHex(strData.B1);
                checkBox8.Text = "5站振动关闭";
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                SendHex(strData.A2);
                checkBox9.Text = "5站听筒打开";
            }
            else
            {
                SendHex(strData.B2);
                checkBox9.Text = "5站听筒关闭";
            }

        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                SendHex(strData.A3);
                checkBox10.Text = "5站喇叭打开";
            }
            else
            {
                SendHex(strData.B3);
                checkBox10.Text = "5站喇叭关闭";
            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                SendHex(strData.A4);
                checkBox11.Text = "5站备用打开";
            }
            else
            {
                SendHex(strData.B4);
                checkBox11.Text = "5站备用关闭";
            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
            {
                SendHex(strData.A5);
                checkBox12.Text = "5站MIC打开";
            }
            else
            {
                SendHex(strData.B5);
                checkBox12.Text = "5站MIC关闭";
            }
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox13.Checked)
            {
                SendHex(strData.A6);
                checkBox13.Text = "5站MIC1打开";
            }
            else
            {
                SendHex(strData.B6);
                checkBox13.Text = "5站MIC1关闭";
            }
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox14.Checked)
            {
                SendHex(strData.A7);
                checkBox14.Text = "5站备用输出打开";
            }
            else
            {
                SendHex(strData.B7);
                checkBox14.Text = "5站备用输出关闭";
            }

        }



    }
       



    
}
