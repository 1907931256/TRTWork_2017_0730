
using CommonClass;
using EqumentCmds;
using System;
using System.Windows.Forms;

namespace Station
{
    public partial class Station6 : Form
    {
        /// <summary>
        /// 表示电机当前位置
        /// </summary>
        private string dangQianWeiZhi_X = "";
        private string dangQianWeiZhi_Y = "";


        #region 通用代码
        private string res;
        public Station6()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }


        private void Form6_Load(object sender, EventArgs e)
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



        #endregion  通用代码



        /// <summary>
        /// 运动到指定位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSport_Click(object sender, EventArgs e)
        {
            string pram = X_txt.Text + " " + Y_txt.Text + " " + SuDu_txt.Text + " "+JiaSuDu_txt.Text+" "+ JianSuDu_txt.Text + " " + XieLv.Text;//运动所需参数

            EquipmentCmd.Instance.SendCommand(btnSport.Text, pram, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + btnSport.Text + "-->" + res + "    " + resHexs + "\r\n");

        }

        /// <summary>
        /// 斜率配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
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
        /// <summary>
        /// 当前位置获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            EquipmentCmd.Instance.SendCommand(button.Text, "", out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);

            if (resHexs.IndexOf("34 0A 16 06 05 ") != -1)
            {
                dangQianWeiZhi_X = ShujuChuli.DiGaoBaWei(ShujuChuli.HexStringToBytes((ShujuChuli.GetIndexString(resHexs,5)))[0], ShujuChuli.HexStringToBytes((ShujuChuli.GetIndexString(resHexs, 6)))[0]);
                dangQianWeiZhi_Y = ShujuChuli.DiGaoBaWei(ShujuChuli.HexStringToBytes((ShujuChuli.GetIndexString(resHexs, 7)))[0],ShujuChuli.HexStringToBytes((ShujuChuli.GetIndexString(resHexs, 8)))[0]);
            }


            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 清屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
        }
        /// <summary>
        /// 状态灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zhuangTaiDeng_Click(object sender, EventArgs e)
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
            EquipmentCmd.Instance.SendCommand(button15.Text, s, out res);
            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button15.Text + "-->" + res + "    " + resHexs + "\r\n");
        }
    }
}
