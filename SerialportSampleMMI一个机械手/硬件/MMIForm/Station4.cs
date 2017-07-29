using EqumentCmds;
using System;
using System.Windows.Forms;

namespace Station
{
    public partial class Station4 : Form
    {
        private string res;

        public Station4()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;  
        }
        private void Form4_Load(object sender, EventArgs e)
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


            trackBar_BaiGuang.Minimum = 0;
            trackBar_BaiGuang.Maximum = 255;
            trackBar_BaiGuang.TickFrequency = 25;

            trackBar1.Minimum = 0;
            trackBar1.Maximum = 255;
            trackBar1.TickFrequency = 5;
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
        /// 后白光源调节
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
        /// 备用光源调节
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = trackBar1.Value.ToString();
            EquipmentCmd.Instance.SendCommand(label3.Text, textBox1.Text, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + label3.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 清屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonXin6_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
        }

        /// <summary>
        /// 状态指示灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
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
            EquipmentCmd.Instance.SendCommand(button9.Text, s, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button9.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
    }
}
