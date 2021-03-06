﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TRTCamera
{
    public partial class Form1 : Form
    {
        private static commPortHelp commProtHelp_;
        private static string protName;

        #region 支持改变窗体大小
        private const int Guying_HTLEFT = 10;
        private const int Guying_HTRIGHT = 11;
        private const int Guying_HTTOP = 12;
        private const int Guying_HTTOPLEFT = 13;
        private const int Guying_HTTOPRIGHT = 14;
        private const int Guying_HTBOTTOM = 15;
        private const int Guying_HTBOTTOMLEFT = 0x10;
        private const int Guying_HTBOTTOMRIGHT = 17;


        private float X;

        private float Y;
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
        void Form_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / X;
            float newy = this.Height / Y;
            setControls(newx, newy, this);
            //   this.Text = this.Width.ToString() + " " + this.Height.ToString();

        }
        #endregion

        public Form1()
        {
          
            InitializeComponent();
            commProtHelp_ = commPortHelp.CreatCommPort();
           
        }
      


        private void Form1_Load(object sender, EventArgs e)
        {
            #region 配合上面的代码实现了窗体按钮大小可变

            //this.Resize += new EventHandler(Form_Resize);

            //X = this.Width;
            //Y = this.Height;

            //setTag(this);
            #endregion

            commProtHelp_.ChuanZhiEvent += com_ChuanZhiEvent;
            
            FromInitializationCommBox();

            Thread thread = new Thread(delegate()
            {

                PortDo(out protName);

            });
            thread.IsBackground = true;
            thread.Start();

           
        }
        private  void PortDo(out string protName)
        {
            bool b = commProtHelp_.ConnectPort(out protName);
        }
        private void FromInitializationCommBox()
        {
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
        }

        void com_ChuanZhiEvent(object send, MyEventArgs e)
        {
            txtXianShi.AppendText(System.DateTime.Now + "   " + e.Cmd + "---->" + e.Res + "\r\n");
        }

     
        private void button1_Click(object sender, EventArgs e)
        {
           
            string str = string.Empty;
            commProtHelp_.SendCmd(button1.Text,ShujuChuli.StrToHex( comboBox1.Text), out str);
            //txtXianShi.AppendText(System.DateTime.Now + "   " + button1.Text + "---->" + str + "\r\n");
            trackBar1.Value = Convert.ToInt32(comboBox1.Text);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            string str = string.Empty;
            commProtHelp_.SendCmd(button2.Text,ShujuChuli.StrToHex( comboBox2.Text), out str);
            //txtXianShi.AppendText(System.DateTime.Now + "   " + button2.Text + "---->" + str + "\r\n");
            trackBar2.Value = Convert.ToInt32(comboBox2.Text);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
          
            string str = string.Empty;
            commProtHelp_.SendCmd(button1.Text,ShujuChuli.StrToHex( trackBar1.Value.ToString()), out str);
            //txtXianShi.AppendText(System.DateTime.Now + "   " + button1.Text + "---->" + str + "\r\n");
            comboBox1.Text = trackBar1.Value.ToString();
            Thread.Sleep(100);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            string str = string.Empty;
            commProtHelp_.SendCmd(button2.Text,ShujuChuli.StrToHex( trackBar2.Value.ToString()), out str);
            //txtXianShi.AppendText(System.DateTime.Now + "   " + button2.Text + "---->" + str + "\r\n");
            comboBox2.Text = trackBar2.Value.ToString();
            Thread.Sleep(100);
        }

     

        private void button3_Click_1(object sender, EventArgs e)
        {
            string sendCmdHex = string.Empty;
            string outstr = string.Empty;
            string pram = string.Empty;
            if (comboBox3.Text == "红灯")
            {
                pram = "FF";
            }
            else if (comboBox3.Text == "绿灯")
            {
                pram = "00";
            }
            else
            {
                pram = "AA";
            }
            commProtHelp_.SendCmd(button3.Text, pram, out outstr);
            //txtXianShi.AppendText(System.DateTime.Now + "   " + button3.Text + "---->" + outstr + "\r\n");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sendCmdHex = string.Empty;
            string outstr = string.Empty;
            string pram = string.Empty;
            if (comboBox4.Text == "前面")
            {
                pram = "00";
            }
            else if (comboBox4.Text == "中间")
            {
                pram = "AA";
            }
            else
            {
                pram = "FF";
            }
            commProtHelp_.SendCmd(button4.Text, pram, out outstr);
            //txtXianShi.AppendText(System.DateTime.Now + "   " + button4.Text + "---->" + outstr + "\r\n");
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            comboPortName.Text = protName;
         
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sendCmdHex = string.Empty;
            string outstr = string.Empty;
            string pram = string.Empty;
            commProtHelp_.SendCmd(button5.Text,string.Format("{0:X2}",Convert.ToInt32(comboBox5.Text)), out outstr);
            //txtXianShi.AppendText(System.DateTime.Now + "   " + button5.Text + "---->" + outstr + "\r\n");
        }

        private void button6_Click(object sender, EventArgs e)
        {
           //string s= LiteData.SendEquipmentCmd(2, "3站产品到位检测", "");
           txtXianShi.Text = "";
            Update();
        }

      
        
    }
}
