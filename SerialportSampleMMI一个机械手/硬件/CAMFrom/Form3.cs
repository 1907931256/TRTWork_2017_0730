using EqumentCmds;
using System;
using System.Windows.Forms;

namespace Station.CAMFrom
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }
        #region 工具通用代码
        private string res;
        private void Form3_Load(object sender, EventArgs e)
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

            trackBarHouBai.Minimum = 0;
            trackBarHouBai.Maximum = 255;
            trackBarHouBai.TickFrequency = 25;//设置每一格移动的间距大小
            trackBarHouBai.Value = 192;

            trackBar_qianBai.Minimum = 0;
            trackBar_qianBai.Maximum = 255;
            trackBar_qianBai.TickFrequency = 25;
            trackBar_qianBai.Value = 192;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 255;
            trackBar1.TickFrequency = 25;
            trackBar1.Value = 192;

            baoJin_combox.SelectedIndex = 0;

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
        /// 清空显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearDispaly_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
        }

        /// <summary>
        /// 3站状态灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zhaungTaiZhiShiDeng_Click(object sender, EventArgs e)
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
            EquipmentCmd.Instance.SendCommand(zhaungTaiZhiShiDeng.Text, s, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + zhaungTaiZhiShiDeng.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JingGao_Click(object sender, EventArgs e)
        {
            EquipmentCmd.Instance.SendCommand(JingGao.Text, baoJin_combox.Text, out res);

            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + JingGao.Text + "-->" + res + "    " + resHexs + "\r\n");
        }







        #endregion 通用代码

        /// <summary>
        /// 前色光源调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarHouBai_Scroll(object sender, EventArgs e)
        {
            houBaiGuangYuan.Text = trackBarHouBai.Value.ToString();
            EquipmentCmd.Instance.SendCommand(houBai_lb.Text, houBaiGuangYuan.Text+" "+ houBaiGuangYuan.Text+" "+ houBaiGuangYuan.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + houBai_lb.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
        /// <summary>
        /// 当用用户按下enter按键后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void houBaiGuangYuan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                try
                {
                    trackBarHouBai.Value = Convert.ToInt32(houBaiGuangYuan.Text);
                    trackBarHouBai_Scroll(sender, null);
                }
                catch (Exception)
                {
                    MessageBox.Show("输入数据非法");
                }
            }
        }

        /// <summary>
        /// 前白光源调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_qianBai_Scroll(object sender, EventArgs e)
        {
            qianBaiGuangYuan.Text = trackBar_qianBai.Value.ToString();
            EquipmentCmd.Instance.SendCommand(qianBai_lb.Text, qianBaiGuangYuan.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + qianBai_lb.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
        private void qianBaiGuangYuan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                try
                {
                    trackBar_qianBai.Value = Convert.ToInt32(qianBaiGuangYuan.Text);
                    trackBar_qianBai_Scroll(sender, null);
                }
                catch (Exception)
                {
                    MessageBox.Show("输入数据非法");
                }
            }
        }
        /// <summary>
        /// 近焦光源调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label1.Text, textBox1.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label1.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
        /// <summary>
        /// 近焦光源调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                try
                {
                    trackBar1.Value = Convert.ToInt32(textBox1.Text);
                    trackBar1_Scroll(sender, null);
                }
                catch (Exception)
                {
                    MessageBox.Show("输入数据非法");
                }
            }
        }

    }
}
