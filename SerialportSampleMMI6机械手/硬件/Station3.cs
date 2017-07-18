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
    public partial class Station3 : Form
    {

	
        private SerialPort comm = new SerialPort();//串口程序的主要处理类
        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。

        private static bool flag_com;
        private static bool ws = true;

        private static AutoResetEvent wEvent = new AutoResetEvent(false);  //握手等待
      
        private static Byte[] buffer = new Byte[20];
        private static Byte[] data;
        private int _count;

        StationData_3 strData = new StationData_3();

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
           // this.Text = this.Width.ToString() + " " + this.Height.ToString();//   打印窗体大小到控件头部

        }


        #endregion

     
        string strRec = "";

        public Station3()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }

        private void Station3_Load(object sender, EventArgs e)
        {
            #region 配合上面的代码实现了窗体按钮大小可变

            this.Resize += new EventHandler(Form1_Resize);

            X = this.Width;
            Y = this.Height;

            setTag(this);
            #endregion
           

            this.Text = Application.CompanyName + "   " + "Station3" + "   " + Application.ProductVersion;


            if (comm.IsOpen)
            {
                comm.Close();
            }
            else
            {
               
                while (ws)
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
                            string s = "72 04 13 0f 00 81";
                            byte[] sbuf = ShujuChuli.HexStringToBytes(s);
                            comm.Write(sbuf, 0, sbuf.Length);

                            bool b = wEvent.WaitOne(1000);//等待返回函数将mEvent置为mEvent.Set();


                            if (b == true)
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
                    if (comm.IsOpen == false)
                    {
                        MessageBox.Show("没有找到指定的串口，请确认是否有可用串口或程序和板块是否对应后，开关程序");
                    }
                }
         
              }
            ShuJuJiaZai();
		
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


            trackBar_QianSe.Minimum = 0;
            trackBar_QianSe.Maximum = 255;
            trackBar_QianSe.TickFrequency = 25;

            trackBar_JiaoZhun.Minimum = 0;
            trackBar_JiaoZhun.Maximum = 255;
            trackBar_JiaoZhun.TickFrequency = 25;

            trackBar_JinJu.Minimum = 0;
            trackBar_JinJu.Maximum = 255;
            trackBar_JinJu.TickFrequency = 25;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 255;
            trackBar1.TickFrequency = 25;

            trackBar2.Minimum = 0;
            trackBar2.Maximum = 255;
            trackBar2.TickFrequency = 25;

            textBox6.Text = "0";
            textBox2.Text = "0";
            textBox4.Text = "0";
            textBox1.Text = "0";
            textBox3.Text = "0";

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

            //把下面一段放在 Task.Factory.StartNew(()  里面会影响窗体登录加载事件的反应，会卡主界面
            if (builder.ToString().IndexOf("34 04 13 0F 00 47 ") != -1)
            {
                strRec = "握手成功！";
                wEvent.Set();   //此处表示握手成功
                ws = false;
            }


            var Rec = Task.Factory.StartNew(() =>
            {
               


                if (data.Length == 7)
                {
                    strRec = Recice(data[3], data[4], data[5], strRec);
                }
                this.txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + strRec + "--->" + builder.ToString() + "\r\n");

            });
           
        }

       

        public static string Recice(byte MoKuai_buf, byte DuanKou_buf, byte ShuJu_buf, string str_txtXianShi)
        {
           string  qian = "-->OK";
           str_txtXianShi = "";
            #region  检测数据接收处理
            if (ShuJu_buf == 0x00 && MoKuai_buf == 01)
            {
                switch (DuanKou_buf.ToString("x2"))
                {

                    case "01": str_txtXianShi = "3站产品到位检测";
                        break;
                    case "02": str_txtXianShi = "3站解析远离检测";
                        //flag_send = "jiexi_QG";
                        break;
                    case "03": str_txtXianShi = "3站45度放平检测";
                        break;
                    case "04": str_txtXianShi = "3站45度顶起检测";
                        break;
                    case "05": str_txtXianShi = "3站前白卡靠近检测";
                        break;
                    case "06": str_txtXianShi = "3站前白卡远离检测";
                        break;
                    case "07": str_txtXianShi = "3站前白卡上升检测";
                        break;
                    case "08": str_txtXianShi = "3站前白卡下降检测";
                        break;
                 
                   
                }
            }
            else if (ShuJu_buf == 0xFF && MoKuai_buf == 01)
            {
                qian = "-->NO";
                switch (DuanKou_buf.ToString("x2"))
                {

                    case "01": str_txtXianShi = "3站产品到位检测";
                        break;
                    case "02": str_txtXianShi = "3站解析远离检测";
                        break;
                    case "03": str_txtXianShi = "3站45度放平检测";
                        break;
                    case "04": str_txtXianShi = "3站45度顶起检测";
                        break;
                    case "05": str_txtXianShi = "3站前白卡靠近检测";
                        break;
                    case "06": str_txtXianShi = "3站前白卡远离检测";
                        break;
                    case "07": str_txtXianShi = "3站前白卡上升检测";
                        break;
                    case "08": str_txtXianShi = "3站前白卡下降检测";
                        break;
             

                }
            }
            #endregion

            #region   气缸接收数据处理
            if (ShuJu_buf == 0x00 && MoKuai_buf == 02)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "1站手机固定";
                        break;
                    case "02": str_txtXianShi = "3站45度顶起";
                        break;
                    case "03": str_txtXianShi = "3站前白灯靠近";
                        break;
                    case "04": str_txtXianShi = "3站解析靠近";
                        break;
                    case "05": str_txtXianShi = "3站前白卡上升";
                        break;
                    case "06": str_txtXianShi = "3站前白卡下降";
                        break;
                    case "07": str_txtXianShi = "3站夹具定位";
                        break;
                 
                }
            }
            else if (ShuJu_buf == 0xFF && MoKuai_buf == 02)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                     case "01": str_txtXianShi = "1站手机松开";
                        break;
                    case "02": str_txtXianShi = "3站45度放平";
                        break;
                    case "03": str_txtXianShi = "3站前白灯远离";
                        break;
                    case "04": str_txtXianShi = "3站解析远离";
                        break;
                    case "05": str_txtXianShi = "3站前白卡下降";
                        break;
                    case "06": str_txtXianShi = "3站取放下降";
                        break;
                    case "07": str_txtXianShi = "3站夹具松开";
                        break;
                }
            }
            #endregion

            #region  光源调节

            if ( MoKuai_buf == 03)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "3站前色卡上光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "02": str_txtXianShi = "3站前色卡中光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "03": str_txtXianShi = "3站前色卡下光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "04": str_txtXianShi = "3站前白光源调节-->" + ShuJu_buf.ToString();
                        break;
                    case "05": str_txtXianShi = "3站后白光源调节-->" + ShuJu_buf.ToString();
                        break;
                  
                }
            }
         

            #endregion

            #region  状态指示数据接收处理

            if (ShuJu_buf == 0x00 && MoKuai_buf == 05)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "3站报警";
                        break;
                    case "02": str_txtXianShi = "3站状态正常";
                        break;
                }
            }
            else if (ShuJu_buf == 0xFF && MoKuai_buf == 05)
            {
                switch (DuanKou_buf.ToString("x2"))
                {
                    case "01": str_txtXianShi = "3站正常";
                        break;
                    case "02": str_txtXianShi = "3站状态异常";
                        break;
                }
            }
            else if (MoKuai_buf == 0x05 && DuanKou_buf == 0x03)
            {
                switch (ShuJu_buf.ToString("x2"))
                {
                    case "00": str_txtXianShi = "3站状态指示灯pass";
                        break;
                    case "FF": str_txtXianShi = "3站状态指示灯fail";
                        break;
                    case "AA": str_txtXianShi = "3站状态指示灯灭";
                        break;
                }
            }
            #endregion

            if (DuanKou_buf == 0x01 && MoKuai_buf == 0x06)
            {

                //00H 所有到位正常/0x01解析远离不正常，0x02 45度放平不正常，0x04色卡光源远离不正常，0x08取放上升不正常
                switch (ShuJu_buf.ToString("x2"))
                {
                    case "00": str_txtXianShi = "所有到位正常";
                        break;
                    case "01": str_txtXianShi = "解析远离不正常";
                        break;
                    case "02": str_txtXianShi = "45度放平不正常";
                        break;
                    case "04": str_txtXianShi = "色卡光源远离不正常";
                        break;
                    case "08": str_txtXianShi = "取放上升不正常";
                        break;
                       
                }
                MessageBox.Show(str_txtXianShi);
            }

            str_txtXianShi += qian;
            return str_txtXianShi;
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
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region     光源调节
        string strEnd = " 81";


       // 3站色卡上光源调节
        private void trackBar_QianSe_MouseUp(object sender, MouseEventArgs e)
        {
            textBox6.Text = trackBar_QianSe.Value.ToString();
            SendHex(strData.sheKa_Shang_GuanYuanTiaoJie + " " + trackBar_QianSe.Value.ToString("x2") + strEnd);
        }


        
         //色卡 中 光源调节
        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
            SendHex(strData.sheKa_Zhaong_GuangYuanTiaoJie + " " + trackBar1.Value.ToString("x2") + strEnd);
        }

         private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //色卡 下 光源调节
        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
              textBox3.Text = trackBar2.Value.ToString();
            SendHex(strData.sheKa_XiaGuangYuanTiaoJie + " " + trackBar2.Value.ToString("x2") + strEnd);
        }

          //前白光源
        private void trackBar_JiaoZhun_MouseUp(object sender, MouseEventArgs e)
        {
            textBox2.Text = trackBar_JiaoZhun.Value.ToString();
            SendHex(strData.jiaoZhun_GuanYuan + " " + trackBar_JiaoZhun.Value.ToString("x2") + strEnd);
        }
      
         //后白光源
        private void trackBar_JinJu_MouseUp(object sender, MouseEventArgs e)
        {
            textBox4.Text = trackBar_JinJu.Value.ToString();
            SendHex(strData.jinJu_GuangYuan + " " + trackBar_JinJu.Value.ToString("x2") + strEnd);
        }

        #endregion

        #region   气缸控制

        //手机固定
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SendHex(strData.shouJiGuDing_QG);
                checkBox1.Text = "3站手机松开";
            }
            else
            {
                SendHex(strData.shouJiSongKai_QG);
                checkBox1.Text = "3站手机固定";
            }
        }


        //光源上升
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                SendHex(strData.jinJiao_ShangShen_QG);
                //System.Threading.Thread.Sleep(3000);
                //SendHex(strData.jinJiao_ShangShen_QG);
                checkBox2.Text = "3站前白卡上升";
            }
            else
            {
                SendHex(strData.jinJiao_XiaJiang_QG);
                checkBox2.Text = "3站前白卡下降";
            }
        }

        //45度顶起
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                //if (flag_send=="jiexi_QG")
                //{
                SendHex(strData.jiaoZhunGuanYuan_KaoJin_QG);

                    checkBox3.Text = "3站45度放平";
                //}
               
            }
            else
            {
                SendHex(strData.jiaoZhunGuanYuan_YuanLi_QG);
                checkBox3.Text = "3站45度顶起";
            }
        }

        //前白卡靠近
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                SendHex(strData.zhenJu_KaoJin_QG);
                checkBox4.Text = "3站前白卡远离";
            }
            else
            {
                SendHex(strData.zhenJu_YuanLi_QG);
                checkBox4.Text = "3站前白卡靠近";
            }
        }


        //解析靠近
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                SendHex(strData.jinJiao_KaoJin_QG);
                checkBox9.Text = "3站解析远离";
            }
            else
            {
                SendHex(strData.jinJiao_YuanLi_QG);
                checkBox9.Text = "3站解析靠近";
            }
        }


        //夹具定位
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                SendHex(strData.jinJiao_ShangShen_QG);
                checkBox5.Text = "3站夹具松开";
            }
            else
            {
                SendHex(strData.jinJiao_XiaJiang_QG);
                checkBox5.Text = "3站夹具定位";
            }
        }
        
        //取放上升
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                SendHex(strData.jieXi_KaoJin_QG);
                checkBox6.Text = "3站取放下降";
            }
            else
            {
                SendHex(strData.jieXi_YuanLi_QG);
                checkBox6.Text = "3站取放上升";
            }
        }

        #endregion

        #region 电机控制
        //private void checkBox10_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBox10.Checked)
        //    {
        //        SendHex(strData.xuanZhuan_QiDong_360);

        //    }
        //    else
        //    {
        //        SendHex(strData.xuanZhuan_TingZhi_360);
        //    }
        //}

        //private void checkBox11_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (checkBox11.Checked)
        //    {
        //        SendHex(strData.tianJi_ShangDian);
        //    }
        //    else
        //    {
        //        SendHex(strData.tianJi_DuanDian);
        //    }
        //}
        #endregion

        #region   状态查询
        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked)
            {
                SendHex(strData.baoJin_4);
                checkBox21.Text = "3站正常";
            }
            else
            {
                SendHex(strData.zhengChang_4);
                checkBox21.Text = "3站报警";
            }
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox22.Checked)
            {
                SendHex(strData.zhuangTaiZhengChang_4);
                checkBox22.Text = "3站状态异常";
            }
            else
            {
                SendHex(strData.zhuangTaiYiChang_4);
                checkBox22.Text = "3站状态正常";
            }
        }
        #endregion

        #region 到位查询


        //产品到位检测
        private void buttonXin1_Click(object sender, EventArgs e)
        {
            SendHex(strData.canPinDaoWei_JC);

        }
        //光线位置检测
        private void dianJi_1_YuanDian_JC(object sender, EventArgs e)
        {
            SendHex(strData.dianJi_1_YuanDian_JC);
        }

        //45顶起检测
        private void buttonXin4_Click(object sender, EventArgs e)
        {
            SendHex(strData.dianJi_2_YuanDian_JC);
        }
        //放平
        private void buttonXin3_Click(object sender, EventArgs e)
        {
            SendHex(strData.dingQi_JC);
        }

       // 3站灯色卡光源靠近检测
        private void buttonXin5_Click(object sender, EventArgs e)
        {
            SendHex(strData.dengXiangYuanLi_JC);
        }

       // 3站增距远色卡光源远离检测
        private void buttonXin6_Click(object sender, EventArgs e)
        {
            SendHex(strData.zhenJuYuanLi_JC);
        }

        //光线检测
        private void buttonXin2_Click(object sender, EventArgs e)
        {
            SendHex(strData.dianJi_1_YuanDian_JC);
        }

        //private void buttonXin3_Click(object sender, EventArgs e)
        //{
        //    SendHex(strData.dianJi_2_YuanDian_JC);
        //}
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            switch (comBox_ZhiShiDeng_1.Text)
            {
                case "绿灯": SendHex(strData.zhuangTaiZhiDeng_Pass_4);
                    break;
                case "红灯": SendHex(strData.zhuangTaiZhiDeng_Fail_4);
                    break;
                case "黄灯": SendHex(strData.zhuangTaiZhiDeng_Mie_4);
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
            Update();
        }

        //前色光源
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
        //前中光源
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
                        trackBar1.Value = a;
                        textBox1.Text = trackBar1.Value.ToString();
                        string s = strData.sheKa_Zhaong_GuangYuanTiaoJie + " " + trackBar1.Value.ToString("x2") + strEnd;
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
                    textBox1.Text = "192";
                    MessageBox.Show("输入有误");
                    throw;
                }
        }
        }
        //下光源
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
                        trackBar2.Value = a;
                        textBox3.Text = trackBar2.Value.ToString();
                        string s = strData.sheKa_XiaGuangYuanTiaoJie + " " + trackBar2.Value.ToString("x2") + strEnd;
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
        //前白光源
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
                    textBox2.Text = "192";
                    MessageBox.Show("输入有误");
                    throw;
                }
            }
        }
        //后白光源
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
                    textBox4.Text = "192";
                    MessageBox.Show("输入有误");
                    throw;
                }
        }

    }

        //取放上升检测
        private void buttonXin8_Click(object sender, EventArgs e)
        {
            SendHex(strData.quFang_SS);
        }
        //取放下降检测
        private void buttonXin9_Click(object sender, EventArgs e)
        {
            SendHex(strData.quFang_XJ);
        }

        //1站状态检测
        private void button4_Click(object sender, EventArgs e)
        {
            SendHex(strData.zhuangTai_Station_one);
        }

        //1站原点检测
        private void button5_Click(object sender, EventArgs e)
        {
            SendHex(strData.yuanDian_Station_one);
        }



        //测试状态
        private void button3_Click(object sender, EventArgs e)
        {
            if (comBox_ZT.Text=="测试中")
            {
                SendHex(strData.ceShiZhuangTai_1);
            }
            if (comBox_ZT.Text=="测试完成")
            {
                 SendHex(strData.ceShiZhuangTai_2);
            }
        }

        //复位
        private void button6_Click(object sender, EventArgs e)
        {
           
           // checkAll(this);
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

        private void label2_Click(object sender, EventArgs e)
        {

        }
}
}
