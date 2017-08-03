using EqumentCmds;
using System;
using System.Windows.Forms;

namespace Station
{
    public partial class Station2 : Form
    {
        #region 通用代码
        private string res;
        public Station2()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }


        private void Form2_Load(object sender, EventArgs e)
        {

            ShujuJiaZai();

            EquipmentCmd.Instance.ReportEvent += Instance_ReportEvent;

            if (EquipmentCmd.Instance.ConnectPort() == false)
            {
                PortConnectFlag.Text = "Port is not connect!";
            }
            else
            {
                PortConnectFlag.Text = "connect!";
            }

        }

        private void Instance_ReportEvent(CommonPortCmd.ActiveEnumData eventId)
        {
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + eventId.ToString() + "\r\n");
        }

        private void ShujuJiaZai()
        {
            #region   commbox 数据加载
            comBox_ZhiShiDeng_1.Items.Add("绿灯");
            comBox_ZhiShiDeng_1.Items.Add("红灯");
            comBox_ZhiShiDeng_1.Items.Add("黄灯");
            comBox_ZhiShiDeng_1.SelectedIndex = 0;

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
            trackBar1.Maximum = 25;
            trackBar1.TickFrequency = 5;

            trackBar2.Minimum = 0;
            trackBar2.Maximum = 25;
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


        private void GetButtonName(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            EquipmentCmd.Instance.SendCommand(button.Text, "", out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button.Text + "-->" + res + "    " + resHexs + "\r\n");

        }
        private void GetCheckBoxName(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;

            if (check.Checked == true)
            {
                EquipmentCmd.Instance.SendCommand(check.Text, "", out res);

                string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
                txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + check.Text + "-->" + res + "    " + resHexs + "\r\n");


            }
            else
            {
                EquipmentCmd.Instance.SendCommand(check.Tag.ToString(), "", out res);

                string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
                txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + check.Tag.ToString() + "-->" + res + "    " + resHexs + "\r\n");


            }
        }
        private static string ByteToHexString(byte[] data)
        {
            string hex = string.Empty;

            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    hex += data[i].ToString("X2");
                    hex += " ";
                }
            }

            return hex;
        }

        /// <summary>
        /// 清屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearDispaly_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
        }


        #endregion  通用代码
        //色卡上光源
        private void trackBar_QianSe_Scroll(object sender, EventArgs e)
        {
            textBox6.Text=trackBar_QianSe.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label7.Text, textBox6.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label7.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
        /// <summary>
        /// 色卡中光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_YuanJu_Scroll(object sender, EventArgs e)
        {
            textBox3.Text = trackBar_YuanJu.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label4.Text, textBox3.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label4.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
        /// <summary>
        /// 色卡下光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_BaiGuang_Scroll(object sender, EventArgs e)
        {
            textBox5.Text = trackBar_BaiGuang.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label6.Text, textBox5.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label6.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
        /// <summary>
        /// 校准光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_JiaoZhun_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = trackBar_JiaoZhun.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label3.Text, textBox2.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label3.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 内光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_JinJu_Scroll(object sender, EventArgs e)
        {
            textBox4.Text = trackBar_JinJu.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label5.Text, textBox4.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label5.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 外光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar3.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label10.Text, textBox1.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label10.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 指示灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            string s = string.Empty;
            if (comBox_ZhiShiDeng_1.Text == "绿灯")
            {
                //00H绿 / FFH红 / AAH黄
                s = "00";
            }
            else if (comBox_ZhiShiDeng_1.Text == "红灯")
            {
                s = "255";
            }
            else if (comBox_ZhiShiDeng_1.Text == "黄灯")
            {
                s = "170";
            }


            EquipmentCmd.Instance.SendCommand(button1.Text, s, out res);

            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button1.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 防抖XY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            fangdou_1.Text = trackBar1.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label8.Text, fangdou_1.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label8.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 防抖Z
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            fangdou_2.Text = trackBar2.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label9.Text, fangdou_2.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label9.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
       
    }
}