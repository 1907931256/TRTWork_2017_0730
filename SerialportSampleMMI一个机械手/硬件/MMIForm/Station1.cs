using System;
using EqumentCmds;
using System.Windows.Forms;
using System.Threading;



namespace Station
{
    public partial class Station1 : Form
    {

       


        public Station1()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        }





        #region  通用代码
        private string res;

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
            baoJin_combox.Items.Add("08");
            baoJin_combox.Items.Add("16");
            baoJin_combox.Items.Add("32");
            baoJin_combox.SelectedIndex = 0;

            comboBox1.Items.Add("01");
            comboBox1.Items.Add("02");
            comboBox1.Items.Add("04");
            comboBox1.Items.Add("08");
            comboBox1.Items.Add("16");
            comboBox1.Items.Add("32");
            comboBox1.SelectedIndex = 0;

            comboBox2.Items.Add("01");
            comboBox2.Items.Add("02");
            comboBox2.Items.Add("04");
            comboBox2.Items.Add("08");
            comboBox2.Items.Add("16");
            comboBox2.Items.Add("32");
            comboBox2.SelectedIndex = 0;

            //comboBox3.Items.Add("01");
            //comboBox3.Items.Add("02");
            //comboBox3.Items.Add("04");
            //comboBox3.Items.Add("08");
            //comboBox3.Items.Add("16");
            //comboBox3.Items.Add("32");
            //comboBox3.SelectedIndex = 0;

            //comboBox4.Items.Add("01");
            //comboBox4.Items.Add("02");
            //comboBox4.Items.Add("04");
            //comboBox4.Items.Add("08");
            //comboBox4.Items.Add("16");
            //comboBox4.Items.Add("32");
            //comboBox4.SelectedIndex = 0;

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


#endregion



        private void button11_Click(object sender, EventArgs e)
        {
            EquipmentCmd.Instance.SendCommand(button11.Text, baoJin_combox.Text, out res);

            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button11.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EquipmentCmd.Instance.SendCommand(button2.Text, comboBox1.Text, out res);

            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button2.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string s=string.Empty;
            if (comBox_ZhiShiDeng_1.Text== "绿灯")
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


            EquipmentCmd.Instance.SendCommand(button17.Text,s, out res);

            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button17.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            EquipmentCmd.Instance.SendCommand(button7.Text, comboBox2.Text, out res);

            string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
            txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button7.Text + "-->" + res + "    " + resHexs + "\r\n");
        }

        /// <summary>
        /// 清屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            txtXianShi.Text = "";
        }
      

    }

}


