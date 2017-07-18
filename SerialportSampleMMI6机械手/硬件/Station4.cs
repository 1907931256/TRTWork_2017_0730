using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Station
{
    public partial class Station4 : Form
    {
        private static AutoResetEvent wEvent = new AutoResetEvent(false);  
        private SerialPort comm = new SerialPort();//串口程序的主要处理类
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private static bool ChaZhaoChuanKouCiShu;

        private static bool ws = true;//用于控制循环寻找串口

        private static Byte[] buffer = new Byte[20];//临时存放串口数据
        private static Byte[] data;//数据保存到该数组下面，用于判断操作
        private int _count;//记录数据长度
	
        private static bool flag_com = false;


        StationData_4 strData = new StationData_4();
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
            //this.Text = this.Width.ToString() + " " + this.Height.ToString();

        }


        #endregion

        public Station4()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }

     
        string strRec = "";
      

        private void Station4_Load(object sender, EventArgs e)
        {
            Action wo = new Action(woshou);

            wo.Invoke();

            #region 配合上面的代码实现了窗体按钮大小可变

            this.Resize += new EventHandler(Form1_Resize);

            X = this.Width;
            Y = this.Height;

            setTag(this);
            #endregion

            this.Text = Application.CompanyName + "   " + "Station4"+ "   " + Application.ProductVersion;

            #region  窗口数据初始化加载
            button2.Enabled = false;
            button2.BackColor = Color.Black;
            button4.Enabled = false;
            button4.BackColor = Color.Black;
            button6.Enabled = false;
            button6.BackColor = Color.Black;
            button8.Enabled = false;
            button8.BackColor = Color.Black;

            comBox_ZhiShiDeng_1.Items.Add("红灯");
            comBox_ZhiShiDeng_1.Items.Add("绿灯");
            comBox_ZhiShiDeng_1.Items.Add("黄灯");
            comBox_ZhiShiDeng_1.SelectedIndex = 1;

            comBox_ZT.Items.Add("测试中");
            comBox_ZT.Items.Add("测试完成");
            comBox_ZT.SelectedIndex = 0;

            textBox1.Text = "0";
            textBox5.Text = "0";

            trackBar_BaiGuang.Minimum = 0;
            trackBar_BaiGuang.Maximum = 255;
            trackBar_BaiGuang.TickFrequency = 25;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 255;
            trackBar1.TickFrequency = 25;

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
                                string s = "72 04 14 0f 00 81";
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

        /// <summary>
        /// 接受串口返回数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                /*
                1.判断数据的  头 尾 数据
                2.判断数据长度
                3.一系列的判断
				
                */
                if (buffer[0].ToString("x2") == "34")
                {
                    _count++;

                    if (_count == Convert.ToInt32(buffer[1].ToString("d")) + 2 && buffer[_count - 1].ToString("x2") == "47")
                    {

                        break;

                    }
                    if (_count>Convert.ToInt32(buffer[1].ToString("d")) + 2)
                    {
                        break;
                    }
                }
            }
     
                

           
         
               
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
                  
                
                
            
			//下面这段代码放在  Task.Factory.StartNew里面会影响窗体程序性能
			//造成窗体无法登陆
            if (builder.ToString().IndexOf("34 04 14 0F 00 47 ") != -1)
            {
                strRec = "握手成功！";
                wEvent.Set();   //此处表示握手成功
                ws = false;
            }

			//此处是一个线程工厂模式，利用（匿名委托）Lamd表达式，启用一个线程
            var Rec = Task.Factory.StartNew(() =>
            {
				
				//此处是判断条件，根据不同返回数据在界面上显示不同的内容信息
                
                    if (data.Length  == 13)     //颜色检测
                    {
                        strRec = DataRGB(data[6], data[7], data[8], data[9], data[10], data[11], data[4], strRec);
                    }
                    else if (data.Length  == 7)
                    {
                        strRec = Recice(data[3], data[4], data[5], strRec);
                    }
                    else if (data.Length == 3)
                    {
                        if (data[2].ToString("x2") == "01")
                        {
                            strRec = "红外数据发送";
                        }

                    }

			//在界面文本上打印信息
            this.txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + strRec + "--->" + builder.ToString() + "\r\n");

            });

          }
        

        /// <summary>
        /// 颜色读取  RGB
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="p5"></param>
        /// <param name="p6"></param>
        /// <param name="strXianshi"></param>
        /// <returns></returns>
        private string DataRGB(byte p1, byte p2, byte p3, byte p4, byte p5, byte p6,byte p7,string strXianshi)
        {
            strXianshi = "";
            string R = ShujuChuli.DiGaoBaWei(p1, p2);
            string G = ShujuChuli.DiGaoBaWei(p3, p4);
            string B = ShujuChuli.DiGaoBaWei(p5, p6);

            switch (p7.ToString("x2"))
            {

                case "01": strXianshi = "前闪光灯数据读取RGB-->";
                    break;
                case "02": strXianshi = "LCD背光数据读取RGB-->" ;
                    break;
                case "03": strXianshi = "后闪光灯数据读取RGB-->" ;
                    break;
                case "04": strXianshi = "信号灯数据读取RGB-->";
                    break;
            }
            string s="R="+ R + " "+"G=" + G + " "+"B=" + B;
           return strXianshi+s;
        }


        /// <summary>
        /// 数据以16进制发送
        /// </summary>
        /// <param name="strParam"></param>
        public void SendHex(string strParam)
        {
            byte[] buf = ShujuChuli.HexStringToBytes(strParam);


            try
            {
                if (comm.IsOpen)
                {
                    comm.Write(buf, 0, buf.Length);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


       
        
        /// <summary>
        /// 数据接收判断
        /// </summary>
        /// <param name="MoKuai_buf"></param>
        /// <param name="DuanKou_buf"></param>
        /// <param name="ShuJu_buf"></param>
        /// <param name="str_txtXianShi"></param>
        /// <returns></returns>
        public static string Recice(byte MoKuai_buf, byte DuanKou_buf, byte ShuJu_buf, string str_txtXianShi)
        {
            string qian = "-->OK";
            str_txtXianShi = "";
         #region 状态检测
            if (MoKuai_buf==0x01 && DuanKou_buf==0x01)
            {
                if (ShuJu_buf.ToString("x2")=="00")
                {
                    str_txtXianShi = "4站产品到位";
                  
                }
                else
                {
                    qian = "-->NO";
                    str_txtXianShi = "4站产品未到位";
                }
            }
            else if (MoKuai_buf==0x01 && DuanKou_buf==0x02)
            {
                if (ShuJu_buf.ToString("x2")=="00")
                {
                    str_txtXianShi = "4站天板远离";
                }
                else
                {
                    qian = "-->NO";
                    str_txtXianShi = "4站天板远离未到位";
                }
            }
            else if (MoKuai_buf == 0x01 && DuanKou_buf == 0x03)
            {
                if (ShuJu_buf.ToString("x2") == "00")
                {
                    str_txtXianShi = "4站天板靠近到位";
                }
                else
                {
                    qian = "-->NO";
                    str_txtXianShi = "4站天板靠近未到位";
                }
            }
            else if (MoKuai_buf == 0x01 && DuanKou_buf == 0x04)
            {
                if (ShuJu_buf.ToString("x2") == "00")
                {
                    str_txtXianShi = "1站原点检测";
                }
                else
                {
                    qian = "-->NO";
                    str_txtXianShi = "1站原点检测";
                }
            }
            else if (MoKuai_buf == 0x01 && DuanKou_buf == 0x05)
            {
                if (ShuJu_buf.ToString("x2") == "00")
                {
                    str_txtXianShi = "1站状态检测";
                }
                else
                {
                    qian = "-->NO";
                    str_txtXianShi = "1站状态检测";
                }
            }
            #endregion
                
         #region 气缸
             if (MoKuai_buf.ToString("x2")=="02" && ShuJu_buf.ToString("x2")=="00")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "4站手机固定";
                        break;
                    case "02": str_txtXianShi = "4站距离1遮挡";
                        break;
                    case "03": str_txtXianShi = "4站距离2遮挡";
                        break;
                    case "04": str_txtXianShi = "4站天板靠近";
                        break;
                    case "05": str_txtXianShi = "4站NFC靠近";
                        break;
                    case "06": str_txtXianShi = "4站返回靠近";
                        break;
                    case "07": str_txtXianShi = "4站主页靠近";
                        break;
                    case "08": str_txtXianShi = "4站菜单靠近";
                        break;
                    case "09": str_txtXianShi = "4站指纹靠近";
                        break;
                    case "0a": str_txtXianShi = "4站后白靠近";
                        break;
                }
            }
             else if (MoKuai_buf.ToString("x2") == "02" && ShuJu_buf.ToString("x2") == "ff")
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "4站手机松开";
                        break;
                    case "02": str_txtXianShi = "4站距离1远离";
                        break;
                    case "03": str_txtXianShi = "4站距离2远离";
                        break;
                    case "04": str_txtXianShi = "4站天板远离";
                        break;
                    case "05": str_txtXianShi = "4站NFC远离";
                        break;
                    case "06": str_txtXianShi = "4站返回远离";
                        break;
                    case "07": str_txtXianShi = "4站主页远离";
                        break;
                    case "08": str_txtXianShi = "4站菜单远离";
                        break;
                    case "09": str_txtXianShi = "4站指纹远离";
                        break;
                    case "0a": str_txtXianShi = "4站后白远离";
                        break;
                }
            }
        #  endregion

         #region  状态指示数据接收处理

             if (ShuJu_buf == 0x00 && MoKuai_buf == 05)
             {
                 switch (DuanKou_buf.ToString("x2"))
                 {
                     case "01": str_txtXianShi = "2站报警";
                         break;
                     case "02": str_txtXianShi = "2站状态正常";
                         break;
                 }
             }
             else if (ShuJu_buf == 0xFF && MoKuai_buf == 05)
             {
                 switch (DuanKou_buf.ToString("x2"))
                 {
                     case "01": str_txtXianShi = "2站正常";
                         break;
                     case "02": str_txtXianShi = "2站状态异常";
                         break;
                 }
             }
             else if (MoKuai_buf == 0x05 && DuanKou_buf == 0x03)
             {
                 switch (ShuJu_buf.ToString("x2"))
                 {
                     case "00": str_txtXianShi = "2站状态指示绿灯";
                         break;
                     case "FF": str_txtXianShi = "2站状态指示红灯";
                         break;
                     case "AA": str_txtXianShi = "2站状态指示黄灯";
                         break;
                 }
             }


             #endregion

          if (DuanKou_buf == 0x01 && MoKuai_buf == 0x06)
             {

                 //0x00到位正常/0x01 天板远离异常
                 switch (ShuJu_buf.ToString("x2"))
                 {
                     case "00": str_txtXianShi = "所有到位正常";
                         break;
                     case "01": str_txtXianShi = "天板远离异常";
                         break;
                       
                 }
                 MessageBox.Show(str_txtXianShi);
             }

         if (MoKuai_buf.ToString() == "90" && DuanKou_buf.ToString() == "255")
         {
               str_txtXianShi = "复位成功";
         }
         str_txtXianShi += qian;
            return str_txtXianShi;
        }

  

        private void button1_Click(object sender, EventArgs e)
        {
           
            SendHex(strData.qianShangGuang_JC);
            button2.Enabled = true;
            button2.BackColor = Color.Gold;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendHex(strData.lCDBeiGuang_JC);
            button4.Enabled = true;
            button4.BackColor = Color.Gold;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendHex(strData.houShangGuang_JC);
            button6.Enabled = true;
            button6.BackColor = Color.Gold;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SendHex(strData.xinHaoDeng_JC);
            button8.Enabled = true;
            button8.BackColor = Color.Gold;
        }

        //前闪光灯颜色读取
        private void button2_Click(object sender, EventArgs e)
        {
            SendHex(strData.qianShangGuang_DuQu);
        }

        #region 气缸

        //手机固定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SendHex(strData.guDing_QG);
                checkBox1.Text = "4站手机松开";
            }
            else
            {
                SendHex(strData.songKai_QG);
                checkBox1.Text = "4站手机固定";
            }
           
        }

        //距离近遮挡
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                SendHex(strData.juLi_1_ZheTang_QG);
                checkBox2.Text = "4站距离1远离";
            }
            else
            {
                SendHex(strData.juLi_1_YuanLi_QG);
                checkBox2.Text = "4站距离1遮挡";
            }
        }
          //距离中遮档
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                SendHex(strData.juLi_2_ZheTang_QG );
                checkBox3.Text = "4站距离2遮挡";
            }
            else
            {
                SendHex(strData.juLi_2_YuanLi_QG);
                checkBox3.Text = "4站距离2远离";
            }

        }
         //天板靠近
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                SendHex(strData.tianBan_2_ZheTang_QG);
                checkBox4.Text = "4站天板远离";
            }
            else
            {
                SendHex(strData.tianBan_2_YuanLi_QG);
                checkBox4.Text = "4站天板靠近";
            }
        }
        
        //NFC靠近
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                SendHex(strData.nFC_2_ZheTang_QG);
                checkBox5.Text = "4站NFC远离";
            }
            else
            {
                SendHex(strData.nFC_2_YuanLi_QG);
                checkBox5.Text = "4站NFC靠近";
            }
        }

        //返回靠近
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                SendHex(strData.FH_YL_QG);
                checkBox6.Text = "4站返回远离";
            }
            else
            {
                SendHex(strData.FH_KJ_QG);
                checkBox6.Text = "4站返回靠近";
            }
        }

        //主页靠近
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                SendHex(strData.ZY_KJ_QG);
                checkBox7.Text = "4站主页远离";
            }
            else
            {
                SendHex(strData.ZY_YL_QG);
                checkBox7.Text = "4站主页靠近";
            }
        }

        //菜单
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                SendHex(strData.CD_KJ_QG);
                checkBox8.Text = "4站菜单远离";
            }
            else
            {
                SendHex(strData.CD_YL_QG);
                checkBox8.Text = "4站菜单靠近";
            }
        }

        //指纹
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                SendHex(strData.ZW_KJ_QG);
                checkBox9.Text = "4站指纹远离";
            }
            else
            {
                SendHex(strData.ZW_YL_QG);
                checkBox9.Text = "4站指纹靠近";
            }
        }

        //后白
        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                SendHex(strData.HB_KJ_QG);
                checkBox10.Text = "4站后白远离";
            }
            else
            {
                SendHex(strData.HB_YL_QG);
                checkBox10.Text = "4站后白靠近";
            }
        }

        #endregion

        #region 到位查询 检测
        //产品到位检测
        private void buttonXin1_Click(object sender, EventArgs e)
        {
            SendHex(strData.canPinDaoWei_JC);
        }
          //天板靠近检测
        private void buttonXin2_Click(object sender, EventArgs e)
        {
            SendHex(strData.tianBanYuanLI_JC);
        }
        //天板靠近检测
        private void buttonXin5_Click(object sender, EventArgs e)
        {
            SendHex(strData.tianBan_KaoJin_JC);
        }
   

        #endregion

        //红外
        private void buttonXin3_Click(object sender, EventArgs e)
        {
            SendHex(strData.hongWaiFaSong);
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked)
            {
                SendHex(strData.baoJin_2);
                checkBox21.Text = "4站报警";
            }
            else
            {
                SendHex(strData.zhengChang_2);
                checkBox21.Text = "4站正常";
            }
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox22.Checked)
            {
                SendHex(strData.zhuangTaiZhengChang_2);
                checkBox22.Text = "4站状态异常";
            }
            else
            {
                SendHex(strData.zhuangTaiYiChang_2);
                checkBox22.Text = "4站状态正常";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            switch (comBox_ZhiShiDeng_1.Text)
            {
                case "绿灯": SendHex(strData.zhuangTaiZhiDeng_Pass_2);
                    break;
                case "红灯": SendHex(strData.zhuangTaiZhiDeng_Fail_2);
                    break;
                case "黄灯": SendHex(strData.zhuangTaiZhiDeng_Mie_2);
                    break;
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            SendHex(strData.lCDBeiGuang_DuQu);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SendHex(strData.houShangGuang_DuQu);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SendHex(strData.xinHaoDeng_DuQu);
        }

        //复位
        private void buttonXin4_Click(object sender, EventArgs e)
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

        //清屏
        private void buttonXin6_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
            Update();
        }


        //后白光源调节
        private void trackBar_BaiGuang_MouseUp(object sender, MouseEventArgs e)
        {
            textBox5.Text = trackBar_BaiGuang.Value.ToString();
            string s = strData.aa1 + " " + trackBar_BaiGuang.Value.ToString("x2") + " 81";
            SendHex(s);
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int a = Convert.ToInt32(textBox5.Text);

                if (a < 256 && a >= 0)
                {
                    trackBar_BaiGuang.Value = a;
                }
                else
                {
                    MessageBox.Show("你输入的数据有误，请输入 0~255 之间的数据");
                    textBox5.Text = "";
                }
            }
            catch (Exception)
            {
                textBox5.Text = "192";
                MessageBox.Show("输入有误");
                throw;
            }
        }


        //备用光源调节
        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
            string s = strData.aa2 + " " + trackBar1.Value.ToString("x2") + " 81";
            SendHex(s);

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int a = Convert.ToInt32(textBox1.Text);

                if (a < 256 && a >= 0)
                {
                    trackBar1.Value = a;
                }
                else
                {
                    MessageBox.Show("你输入的数据有误，请输入 0~255 之间的数据");
                    textBox1.Text = "";
                }
            }
            catch (Exception)
            {
                textBox1.Text = "192";
                MessageBox.Show("输入有误");
                throw;
            }
        }

        //原点检测
        private void button10_Click(object sender, EventArgs e)
        {
            SendHex(strData.yuanDian_JC);


        }

        //状态检测
        private void button11_Click(object sender, EventArgs e)
        {
            SendHex(strData.zhuangTai_JC);

        }
        //状态指示
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

     
      

       

       





    }
}
