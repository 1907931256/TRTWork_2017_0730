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
    public partial class Station2 : Form
    {

        private SerialPort comm = new SerialPort();//串口程序的主要处理类
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
       
     

	    private static AutoResetEvent wEvent = new AutoResetEvent(false);  //握手等待
        private static bool ws = true;
        private static bool ChaZhaoChuanKouCiShu;

        private static Byte[] buffer = new Byte[20];
        private static Byte[] data;
        private int _count;
       // private static AutoResetEvent RecEvent = new AutoResetEvent(false);  //握手等待
     
       
        string strRec = "";




        StationData_2 strData = new StationData_2();

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
           // this.Text = this.Width.ToString() + " " + this.Height.ToString();

        }


        #endregion


        public Station2()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }

     
        private void Station2_Load(object sender, EventArgs e)
        {
            #region 配合上面的代码实现了窗体按钮大小可变

            this.Resize += new EventHandler(Form1_Resize);

            X = this.Width;
            Y = this.Height;

            setTag(this);
            #endregion

            this.Text = Application.CompanyName + "   " + "Station2" + "   " + Application.ProductVersion;
            Console.WriteLine("Station_Load    " + Thread.CurrentThread.ManagedThreadId.ToString());

            ShujuJiaZai();
            woshou();
              
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
                                string s = "72 04 12 0f 00 81";
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

        private void ShujuJiaZai()
        {
            #region   commbox 数据加载
            comBox_ZhiShiDeng_1.Items.Add("绿灯");
            comBox_ZhiShiDeng_1.Items.Add("红灯");
            comBox_ZhiShiDeng_1.Items.Add("黄灯");
            comBox_ZhiShiDeng_1.SelectedIndex = 0;

            comBox_ZT.Items.Add("测试中");
            comBox_ZT.Items.Add("测试完成");
            comBox_ZT.SelectedIndex = 0;



            trackBar_QianSe.Minimum = 0;
            trackBar_QianSe.Maximum = 255;
            trackBar_QianSe.TickFrequency = 25;//设置每一格移动的间距大小

            trackBar_JiaoZhun.Minimum = 0;
            trackBar_JiaoZhun.Maximum = 255;
            trackBar_JiaoZhun.TickFrequency = 25;

            trackBar_JinJu.Minimum = 0;
            trackBar_JinJu.Maximum = 255;
            trackBar_JinJu.TickFrequency = 25;

            trackBar_YuanJu.Minimum = 0;
            trackBar_YuanJu.Maximum = 255;
            trackBar_YuanJu.TickFrequency = 25;

            trackBar_BaiGuang.Minimum = 0;
            trackBar_BaiGuang.Maximum = 255;
            trackBar_BaiGuang.TickFrequency = 25;

            trackBar3.Minimum = 0;
            trackBar3.Maximum = 255;
            trackBar3.TickFrequency = 25;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 15;
            trackBar1.TickFrequency = 5;


            trackBar2.Minimum = 0;
            trackBar2.Maximum = 15;
            trackBar2.TickFrequency = 5;

            textBox1.Text = "0";
            textBox2.Text = "0";
            textBox3.Text = "0";
            textBox4.Text = "0";
            textBox5.Text = "0";
            textBox6.Text = "0";
            fangdou_2.Text = "0";
            fangdou_1.Text = "0";

            trackBar1.Value = Convert.ToInt32(fangdou_1.Text);
            trackBar2.Value = Convert.ToInt32(fangdou_2.Text);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("comm_DataReceived   " + Thread.CurrentThread.ManagedThreadId.ToString());

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



            var Rec = Task.Factory.StartNew(() =>
            {

                if (builder.ToString().ToUpper().IndexOf("34 04 12 0F 00 47 ") != -1)
                {
                    strRec = "握手成功！";
                    ws = false;
                    wEvent.Set();   //此处表示握手成功
                }


                if (data.Length == 7)
                {
                    strRec = Recice(data[3], data[4], data[5], strRec);
                }
                this.txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + strRec + "--->" + builder.ToString() + "\r\n");

            });

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
            //flag_send = "";
            str_txtXianShi = "";
            #region  检测数据接收处理
            if (ShuJu_buf == 0x00 && MoKuai_buf == 01)
            {
              

                switch (DuanKou_buf.ToString("x2"))
                {

                    case "01": str_txtXianShi = "2站产品到位检测";
                      
                        break;
                    case "02": str_txtXianShi = "2站防抖电机X原点检测";
                        break;
                    case "03": str_txtXianShi = "2站防抖电机Y原点检测";
                        break;
                    case "04": str_txtXianShi = "2站灯箱远离检测";
                        break;
                    case "05": str_txtXianShi = "2站增距远离检测";
                        break;
                    case "06": str_txtXianShi = "2站近焦远离检测";
                        break;
                    case "07": str_txtXianShi = "2站解析远离检测";
                        break;
                    case "08": str_txtXianShi = "2站SFR远离检测";
                        break;
                    case "09": str_txtXianShi = "2站近焦上升到位检测";
                        break;
                    case "0a": str_txtXianShi = "1站原点检测";
                        break;
                    case "0b": str_txtXianShi = "1站状态检测";
                        break;




                }
            }
            else if (ShuJu_buf == 0xFF && MoKuai_buf == 01)
            {
                qian = "-->NO";
                switch (DuanKou_buf.ToString("x2"))
                {


                    case "01": str_txtXianShi = "2站产品到位检测";
                      
                        break;
                    case "02": str_txtXianShi = "2站防抖电机X原点检测";
                        break;
                    case "03": str_txtXianShi = "2站防抖电机Y原点检测";
                        break;
                    case "04": str_txtXianShi = "2站灯箱远离检测";
                        break;
                    case "05": str_txtXianShi = "2站增距远离检测";
                        break;
                    case "06": str_txtXianShi = "2站近焦远离检测";
                        break;
                    case "07": str_txtXianShi = "2站解析远离检测";
                        break;
                    case "08": str_txtXianShi = "2站SFR远离检测";
                        break;
                    case "09": str_txtXianShi = "2站近焦上升到位检测";
                        break;
                    case "0a": str_txtXianShi = "1站原点检测";
                        break;
                    case "0b": str_txtXianShi = "1站状态检测";
                        break;

                }
            }
            #endregion

            #region   气缸接收数据处理
            if (ShuJu_buf == 0x00 && MoKuai_buf == 02)
            {
        
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "2站手机固定";
                        break;
                    case "02": str_txtXianShi = "2站光源靠近";
                        break;
                    case "03": str_txtXianShi = "2站增距靠近";
                        break;
                    case "04": str_txtXianShi = "2站近焦靠近";
                        break;
                    case "05": str_txtXianShi = "2站近焦上升";
                        break;
                    case "06": str_txtXianShi = "2站解析靠近";
                        break;
                    case "07": str_txtXianShi = "2站SFR靠近";
                        break;
                    //case "09": str_txtXianShi = "2站手机定位";
                    //    break;
                    //case "03": str_txtXianShi = "2站后白灯箱靠近";
                    //    break;
                }
            }
            else if (ShuJu_buf == 0xFF && MoKuai_buf == 02)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "2站手机松开";
                        break;
                    case "02": str_txtXianShi = "2站光源远离";
                        break;
                    case "03": str_txtXianShi = "2站增距远离";
                        break;
                    case "04": str_txtXianShi = "2站近焦远离";
                        break;
                    case "05": str_txtXianShi = "2站近焦下降";
                        break;
                    case "06": str_txtXianShi = "2站解析远离";
                        break;
                    case "07": str_txtXianShi = "2站SFR远离";
                        break;
                }
            }
            #endregion

            #region  光源调节
            if (MoKuai_buf == 0x03)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "2站色卡上光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "02": str_txtXianShi = "2站色卡中光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "03": str_txtXianShi = "2站色卡下光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "04": str_txtXianShi = "2站校准光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "05": str_txtXianShi = "2站近距内光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "06": str_txtXianShi = "2站近距外光源调节-->" + ShuJu_buf.ToString();
                        break;
                }
            }


            #endregion

            #region  机械手相关数据接收处理

            if (ShuJu_buf == 0x00 && MoKuai_buf == 04)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "2站XY防抖停止";
                        break;
                    case "02": str_txtXianShi = "2站Z防抖停止";
                        break;
                    case "03": str_txtXianShi = "2站电机电源通电";
                        break;
                }
            }
            else if ( MoKuai_buf == 04)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "2站XY防抖-->" + ShuJu_buf.ToString();
                        break;
                    case "02": str_txtXianShi = "2站Z防抖-->" + ShuJu_buf.ToString();
                        break;
                    case "03": str_txtXianShi = "2站电机电源断电";
                        break;
                }
            }




            #endregion

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

                //0x00所有到位正常、0x01防抖电机1原点异常，0x02防抖电机2原点异常，0x04灯箱远离异常，0x08增距远离异常，0x10近焦远离异常，0x20解析远离异常，0x40近焦上升异常

                switch (ShuJu_buf.ToString("x2"))
                {
                    case "00": str_txtXianShi = "所有到位正常";
                        break;
                    case "01": str_txtXianShi = "防抖电机1原点异常";
                        break;
                    case "02": str_txtXianShi = "防抖电机2原点异常";
                        break;
                    case "04": str_txtXianShi = "灯箱远离异常";
                        break;
                    case "08": str_txtXianShi = "增距远离异常";
                        break;
                    case "10": str_txtXianShi = "近焦远离异常";
                        break;
                    case "20": str_txtXianShi = "解析远离异常";
                        break;
                    case "40": str_txtXianShi = "近焦上升异常";
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



        #region     到位检测
        private void buttonXin1_Click(object sender, EventArgs e)
        {
            SendHex(strData.canPinDaoWei_JC);
        }
  
        private void buttonXin2_Click(object sender, EventArgs e)
        {
            SendHex(strData.zhenJuYuanLi_JC);
        }

        private void buttonXin3_Click(object sender, EventArgs e)
        {
            SendHex(strData.dianJi_1_YuanDian_JC);
        }

        private void buttonXin4_Click(object sender, EventArgs e)
        {
            SendHex(strData.dianJi_2_YuanDian_JC);
        }

        private void buttonXin5_Click(object sender, EventArgs e)
        {
            SendHex(strData.dengXiangYuanLi_JC);
        }
        private void buttonXin6_Click(object sender, EventArgs e)
        {
            SendHex(strData.jinJiaoYuanLi_JC);
        }

        private void buttonXin7_Click(object sender, EventArgs e)
        {
            SendHex(strData.jieXi_JC);
        }

        private void buttonXin8_Click(object sender, EventArgs e)
        {
            SendHex(strData.SFR_JC);
        }



        #endregion

        #region   气缸控制
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
               
                    SendHex(strData.shouJiGuDing_QG);
                    checkBox1.Text = "2站手机固定松开";
                
            }
            else
            {
                SendHex(strData.shouJiSongKai_QG);
                checkBox1.Text = "2站手机固定";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                SendHex(strData.jiaoZhunGuanYuan_KaoJin_QG);
                checkBox2.Text = "2站光源靠近";
            }
            else
            {
                SendHex(strData.jiaoZhunGuanYuan_YuanLi_QG);
                checkBox2.Text = "2站光源远离";
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                SendHex(strData.zhenJu_KaoJin_QG);
                checkBox4.Text = "2站增距远离";
            }
            else
            {
                SendHex(strData.zhenJu_YuanLi_QG);
                checkBox4.Text = "2站增距靠近";
            }
        }

        /// <summary>
        /// 近焦
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                SendHex(strData.jinJiao_XiaJiang_QG);
                checkBox9.Text = "2站近焦上升";
            }
            else
            {
                SendHex(strData.jinJiao_ShangShen_QG);
                checkBox9.Text = "2站近焦下降";
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                SendHex(strData.jinJiao_KaoJin_QG);
                checkBox5.Text = "2站近焦远离";
            }
            else
            {
                SendHex(strData.jinJiao_YuanLi_QG);
                checkBox5.Text = "2站近焦靠近";
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                SendHex(strData.jieXi_KaoJin_QG);
                checkBox6.Text = "2站解析远离";
            }
            else
            {
                SendHex(strData.jieXi_YuanLi_QG);
                checkBox6.Text = "2站解析靠近";
            }
        }

        /// <summary>
        /// SFR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                SendHex(strData.SFR_KaoJin_QG);
                checkBox7.Text = "2站SFR远离";
            }
            else
            {
                SendHex(strData.SFR_YuanLi_QG);
                checkBox7.Text = "2站SFR远离";
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkBox8.Checked)
            //{
            //    SendHex(strData.shouJi_DingWei_QG);
            //    checkBox8.Text = "2站手机松开";
            //}
            //else
            //{
            //    SendHex(strData.shouJi_SongKai_QG);
            //    checkBox8.Text = "2站手机定位";
            //}
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkBox3.Checked)
            //{
            //    SendHex(strData.baiDengXiang_KaoJin_QG);
            //    checkBox3.Text = "2站后白灯箱远离";
            //}
            //else
            //{
            //    SendHex(strData.baiDengXiang_YuanLi_QG);
            //    checkBox3.Text = "2站后白灯箱靠近";
            //}
        }


        #endregion

        #region    电机控制部分


        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
            {
                SendHex(strData.tianJi_ShangDian);
                checkBox12.Text = "2站电机电源断电";
            }
            else
            {
                SendHex(strData.tianJi_DuanDian);
                checkBox12.Text = "2站电机电源上电";
            }
        }



        #endregion

        #region    光源控制
        string strEnd = " 81";

        /// <summary>
        /// 色卡上光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_QianSe_MouseUp(object sender, MouseEventArgs e)
        {

            textBox6.Text = trackBar_QianSe.Value.ToString();
            string s = strData.sheKa_Shang_GuanYuanTiaoJie + " " + trackBar_QianSe.Value.ToString("x2") + strEnd;
            SendHex(s);

        }
       

        /// <summary>
        /// 校准光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_JiaoZhun_MouseUp(object sender, MouseEventArgs e)
        {
            textBox2.Text = trackBar_JiaoZhun.Value.ToString();
            string s = strData.jiaoZhun_GuanYuan + " " + trackBar_JiaoZhun.Value.ToString("x2") + strEnd;
            SendHex(s);
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int a = Convert.ToInt32(textBox2.Text);

                if (a < 256 && a >= 0)
                {
                    trackBar_JiaoZhun.Value = a;
                }
                else
                {
                    MessageBox.Show("你输入的数据有误，请输入 0~255 之间的数据");
                    textBox2.Text = "";
                }
            }
            catch (Exception)
            {
                textBox2.Text = "192";
                MessageBox.Show("输入有误");
                throw;
            }
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 近距内光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_JinJu_MouseUp(object sender, MouseEventArgs e)
        {
            textBox4.Text = trackBar_JinJu.Value.ToString();
            string s = strData.jinJu_GuangYuan + " " + trackBar_JinJu.Value.ToString("x2") + strEnd;
            SendHex(s);
        }
        //近距外光源
        private void trackBar3_MouseUp_1(object sender, MouseEventArgs e)
        {

            textBox1.Text = trackBar3.Value.ToString();
            string s = strData.wai_GuangYuan + " " + trackBar3.Value.ToString("x2") + strEnd;
            SendHex(s);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
            if (e.KeyChar == 13)
            {
                try
                {
                    int a = Convert.ToInt32(textBox1.Text);

                    if (a < 256 && a >= 0)
                    {
                        trackBar3.Value = a;

                        textBox1.Text = trackBar3.Value.ToString();
                        string s = strData.wai_GuangYuan + " " + trackBar3.Value.ToString("x2") + strEnd;
                        SendHex(s);

                    }
                    else
                    {
                        MessageBox.Show("你输入的数据有误，请输入 0~255 之间的数据");
                        textBox1.Text = "0";
                    }
                }
                catch (Exception)
                {
                    textBox1.Text = "0";
                    MessageBox.Show("输入有误");
                    throw;
                }
            }

        }
        /// <summary>
        /// 色卡中光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_YuanJu_MouseUp(object sender, MouseEventArgs e)
        {
            textBox3.Text = trackBar_YuanJu.Value.ToString();
            string s = strData.sheKa_Zhaong_GuangYuanTiaoJie + " " + trackBar_YuanJu.Value.ToString("x2") + strEnd;
            SendHex(s);
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }


        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 色卡下光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_BaiGuang_MouseUp(object sender, MouseEventArgs e)
        {
            textBox5.Text = trackBar_BaiGuang.Value.ToString();
            string s = strData.sheKa_XiaGuangYuanTiaoJie + " " + trackBar_BaiGuang.Value.ToString("x2") + strEnd;
            SendHex(s);
        }



        #endregion

        #region 状态显示
        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked)
            {
                SendHex(strData.baoJin_3);
                checkBox21.Text = "2站报警正常";
            }
            else
            {
                SendHex(strData.zhengChang_3);
                checkBox21.Text = "2站报警";
            }
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox22.Checked)
            {
                SendHex(strData.zhuangTaiZhengChang_3);
                checkBox22.Text = "2站状态正常";
            }
            else
            {
                SendHex(strData.zhuangTaiYiChang_3);
                checkBox22.Text = "2站状态异常";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (comBox_ZhiShiDeng_1.Text)
            {
                case "绿灯": SendHex(strData.zhuangTaiZhiDeng_Pass_3);
                    break;
                case "红灯": SendHex(strData.zhuangTaiZhiDeng_Fail_3);
                    break;
                case "黄灯": SendHex(strData.zhuangTaiZhiDeng_Mie_3);
                    break;
            }
        }


        #endregion

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            fangdou_1.Text = trackBar1.Value.ToString();
            string s = strData.fangDou_1_TiaoJie + " " + trackBar1.Value.ToString("x2") + " 81";
            SendHex(s);

        }

        private void fangdou_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
            if (e.KeyChar==13)
            {
                try
                {
                    trackBar1.Value = Convert.ToInt32(fangdou_1.Text);

                    fangdou_1.Text = trackBar1.Value.ToString();
                    string s = strData.fangDou_1_TiaoJie + " " + trackBar1.Value.ToString("x2") + " 81";
                    SendHex(s);

                }
                catch (Exception)
                {

                    throw;
                }
            }
            

        }

        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
            fangdou_2.Text = trackBar2.Value.ToString();
            string s = strData.fangDou_2_TiaoJie + " " + trackBar2.Value.ToString("x2") + " 81";
            SendHex(s);

        }

        private void fangdou_2_KeyPress(object sender, KeyPressEventArgs e)
        {
              if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
            if (e.KeyChar==13)
            {
                trackBar2.Value = Convert.ToInt32(fangdou_2.Text);

                fangdou_2.Text = trackBar2.Value.ToString();
                string s = strData.fangDou_2_TiaoJie + " " + trackBar2.Value.ToString("x2") + " 47";
                SendHex(s);
            
            }
        }

        private void fuwei_Click(object sender, EventArgs e)
        {
            SendHex(strData.fuwei);
            checkAll(this);
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
        private void button1_Click(object sender, EventArgs e)
        {

            txtXianShi.Clear();
            Update();
        }

        /// <summary>
        /// 色卡上光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
            if (e.KeyChar == 13)
            {
                try
                {
                    int a = Convert.ToInt32(textBox6.Text);

                    if (a < 256 && a >= 0)
                    {
                        trackBar_QianSe.Value = a;

                        textBox6.Text = trackBar_QianSe.Value.ToString();
                        string s = strData.sheKa_Shang_GuanYuanTiaoJie + " " + trackBar_QianSe.Value.ToString("x2") + strEnd;
                        SendHex(s);

                    }
                    else
                    {
                        MessageBox.Show("你输入的数据有误，请输入 0~255 之间的数据");
                        textBox6.Text = "0";
                    }
                }
                catch (Exception)
                {
                    textBox6.Text = "192";
                    MessageBox.Show("输入有误");
                    throw;
                }
            }

        }

        /// <summary>
        /// 近距光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }

            if (e.KeyChar == 13)
            {
                try
                {
                    int a = Convert.ToInt32(textBox4.Text);

                    if (a < 256 && a >= 0)
                    {
                        trackBar_JinJu.Value = a;

                        textBox4.Text = trackBar_JinJu.Value.ToString();
                        string s = strData.jinJu_GuangYuan + " " + trackBar_JinJu.Value.ToString("x2") + strEnd;
                        SendHex(s);
                    }
                    else
                    {
                        MessageBox.Show("你输入的数据有误，请输入 0~255 之间的数据");
                        textBox4.Text = "0";
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }


        /// <summary>
        /// 校准光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }

            if (e.KeyChar == 13)
            {
                try
                {
                    int a = Convert.ToInt32(textBox2.Text);

                    if (a < 256 && a >= 0)
                    {
                        trackBar_JiaoZhun.Value = a;

                        textBox2.Text = trackBar_JiaoZhun.Value.ToString();
                        string s = strData.jiaoZhun_GuanYuan + " " + trackBar_JiaoZhun.Value.ToString("x2") + strEnd;
                        SendHex(s);
                    }
                    else
                    {
                        MessageBox.Show("你输入的数据有误，请输入 0~255 之间的数据");
                        textBox2.Text = "0";
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        /// <summary>
        /// 色卡中光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {


            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }

            if (e.KeyChar == 13)
            {
                try
                {
                    int a = Convert.ToInt32(textBox3.Text);

                    if (a < 256 && a >= 0)
                    {
                        trackBar_YuanJu.Value = a;
                        textBox3.Text = trackBar_YuanJu.Value.ToString();
                        string s = strData.sheKa_Zhaong_GuangYuanTiaoJie + " " + trackBar_YuanJu.Value.ToString("x2") + strEnd;
                        SendHex(s);
                    }
                    else
                    {
                        MessageBox.Show("你输入的数据有误，请输入 0~255 之间的数据");
                        textBox3.Text = "0";
                    }
                }
                catch (Exception)
                {
                    textBox3.Text = "192";
                    MessageBox.Show("输入有误");
                    throw;
                }
            }

        }


        //色卡下光源
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }

            if (e.KeyChar == 13)
            {
                try
                {
                    int a = Convert.ToInt32(textBox5.Text);

                    if (a < 256 && a >= 0)
                    {
                        trackBar_BaiGuang.Value = a;

                        textBox5.Text = trackBar_BaiGuang.Value.ToString();
                        string s = strData.sheKa_XiaGuangYuanTiaoJie + " " + trackBar_BaiGuang.Value.ToString("x2") + strEnd;
                        SendHex(s);
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

        //2站近焦上升到位检测
        private void buttonXin11_Click(object sender, EventArgs e)
        {
            SendHex(strData.jinJiao);
        }
        //yuan
        private void buttonXin10_Click(object sender, EventArgs e)
        {
            SendHex(strData.a);
        }
        //zhuangtai
        private void buttonXin9_Click(object sender, EventArgs e)
        {
            SendHex(strData.b);
        }

      

      
       

      







    }
}