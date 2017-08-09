using EqumentCmds;
using System;
using System.Windows.Forms;
using System.Threading;

namespace Station.CAMFrom
{
    public partial class Form1 : Form
    {
        
        #region  通用代码
        private string res;
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            ShuJuJiaZai();
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

        private void ShuJuJiaZai()
        {
            #region   commbox 数据加载
            comBox_ZhiShiDeng_1.Items.Add("绿灯");
            comBox_ZhiShiDeng_1.Items.Add("红灯");
            comBox_ZhiShiDeng_1.Items.Add("黄灯");
            comBox_ZhiShiDeng_1.SelectedIndex = 0;

            baoJin_combox.Items.Add("01");
            baoJin_combox.Items.Add("02");
            baoJin_combox.Items.Add("04");
          
            baoJin_combox.SelectedIndex = 0;


            comboBox2.Items.Add("01");
            comboBox2.Items.Add("02");
            comboBox2.Items.Add("04");
            comboBox2.Items.Add("08");
            comboBox2.Items.Add("16");
            comboBox2.Items.Add("32");
            comboBox2.SelectedIndex = 0;

            trackBarHouBai.Maximum = 255;
            trackBarHouBai.Minimum = 0;
            trackBarHouBai.TickFrequency = 25;
            trackBarHouBai.Value = 192;

            trackBar_qianBai.Minimum = 0;
            trackBar_qianBai.Maximum = 255;
            trackBar_qianBai.TickFrequency = 25;
            trackBar_qianBai.Value = 192;



            #endregion

        }

        private void GetButtonName(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Thread thread_ = new Thread(() =>
            {
                EquipmentCmd.Instance.SendCommand(button.Text, "", out res);

                string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
                txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button.Text + "-->" + res + "    " + resHexs + "\r\n");
            }
            );
            thread_.IsBackground = true;
            thread_.Start();



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

        public static string ByteToHexString(byte[] data)
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
        /// 状态指示灯
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


        /// <summary>
        /// 清空显示消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearDispaly_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
        }


        #endregion  通用代码



        /// <summary>
        /// 后白光源调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar_HouBai_Scroll(object sender, EventArgs e)
        {
            houBaiGuangYuan.Text = trackBarHouBai.Value.ToString();
            EquipmentCmd.Instance.SendCommand(houBai_lb.Text, houBaiGuangYuan.Text, out res);
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
                    trackBar_HouBai_Scroll(sender, null);
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
        private void trackBar_QianBai_Scroll(object sender, EventArgs e)
        {
            qianBaiGuangYuan.Text = trackBar_qianBai.Value.ToString();
            EquipmentCmd.Instance.SendCommand(qianBai_lb.Text, qianBaiGuangYuan.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + qianBai_lb.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
        /// <summary>
        /// 前白光源调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void qianBaiGuangYuan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Enter)
            {
                try
                {
                    trackBar_qianBai.Value = Convert.ToInt32(qianBaiGuangYuan.Text);
                    trackBar_QianBai_Scroll(sender, null);
                }
                catch (Exception)
                {
                    MessageBox.Show("输入数据非法");
                }
            }
        }



        /// <summary>
        /// 取放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button17_Click(object sender, EventArgs e)
        {
            EquipmentCmd.Instance.SendCommand(button17.Text, comboBox2.Text, out res);

            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button17.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
       
    }
}
