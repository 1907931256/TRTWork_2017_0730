﻿
namespace Station.CommonCod
{
    class code1
    {
        #region 工具通用代码
        //private string res;
        //public Station3()
        //{
        //    InitializeComponent();
        //    Control.CheckForIllegalCrossThreadCalls = false;   //禁止.net对线程做检测
        //}


        //private void Form3_Load(object sender, EventArgs e)
        //{

        //    ShujuJiaZai();

        //    EquipmentCmd.Instance.ReportEvent += Instance_ReportEvent;

        //    if (EquipmentCmd.Instance.ConnectPort() == false)
        //    {
        //        PortConnectFlag.Text = "Port is not connect!";
        //    }
        //    else
        //    {
        //        PortConnectFlag.Text = "connect!";
        //    }

        //}

        //private void Instance_ReportEvent(CommonPortCmd.ActiveEnumData eventId)
        //{
        //    txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + eventId.ToString() + "\r\n");
        //}

        //private void ShujuJiaZai()
        //{
        //    #region   commbox 数据加载
        //    comBox_ZhiShiDeng_1.Items.Add("绿灯");
        //    comBox_ZhiShiDeng_1.Items.Add("红灯");
        //    comBox_ZhiShiDeng_1.Items.Add("黄灯");
        //    comBox_ZhiShiDeng_1.SelectedIndex = 0;

        //    trackBar_QianSe.Minimum = 0;
        //    trackBar_QianSe.Maximum = 255;
        //    trackBar_QianSe.TickFrequency = 25;//设置每一格移动的间距大小

        //    trackBar_JiaoZhun.Minimum = 0;
        //    trackBar_JiaoZhun.Maximum = 255;
        //    trackBar_JiaoZhun.TickFrequency = 25;

        //    trackBar_JinJu.Minimum = 0;
        //    trackBar_JinJu.Maximum = 255;
        //    trackBar_JinJu.TickFrequency = 25;

        //    trackBar1.Minimum = 0;
        //    trackBar1.Maximum = 255;
        //    trackBar1.TickFrequency = 25;

        //    trackBar2.Minimum = 0;
        //    trackBar2.Maximum = 255;
        //    trackBar2.TickFrequency = 25;









        //    #endregion
        //}
        //private void GetButtonName(object sender, EventArgs e)
        //{
        //    Button button = (Button)sender;
        //    EquipmentCmd.Instance.SendCommand(button.Text, "", out res);
        //    string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
        //    txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button.Text + "-->" + res + "    " + resHexs + "\r\n");

        //}
        //private void GetCheckBoxName(object sender, EventArgs e)
        //{
        //    CheckBox check = (CheckBox)sender;

        //    if (check.Checked == true)
        //    {
        //        EquipmentCmd.Instance.SendCommand(check.Text, "", out res);

        //        string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
        //        txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + check.Text + "-->" + res + "    " + resHexs + "\r\n");


        //    }
        //    else
        //    {
        //        EquipmentCmd.Instance.SendCommand(check.Tag.ToString(), "", out res);

        //        string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
        //        txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + check.Tag.ToString() + "-->" + res + "    " + resHexs + "\r\n");


        //    }
        //}
        //private static string ByteToHexString(byte[] data)
        //{
        //    string hex = string.Empty;

        //    if (data != null)
        //    {
        //        for (int i = 0; i < data.Length; i++)
        //        {
        //            hex += data[i].ToString("X2");
        //            hex += " ";
        //        }
        //    }

        //    return hex;
        //}

        ///// <summary>
        ///// 清空显示
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void clearDispaly_Click(object sender, EventArgs e)
        //{
        //    txtXianShi.Text = "";
        //}

        ///// <summary>
        ///// 3站状态灯
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void button15_Click(object sender, EventArgs e)
        //{
        //    string s = string.Empty;
        //    if (comBox_ZhiShiDeng_1.Text == "绿灯")
        //    {
        //        //00H绿 / FFH红 / AAH黄
        //        s = "00";
        //    }
        //    else if (comBox_ZhiShiDeng_1.Text == "红灯")
        //    {
        //        s = "255";
        //    }
        //    else if (comBox_ZhiShiDeng_1.Text == "黄灯")
        //    {
        //        s = "170";
        //    }
        //    EquipmentCmd.Instance.SendCommand(button15.Text, s, out res);
        //    string resHexs = ByteToHexString(EquipmentCmd.Instance.resPort);
        //    txtXianShi.AppendText(DateTime.Now.ToString("hh时mm分ss秒") + "  " + button15.Text + "-->" + res + "    " + resHexs + "\r\n");
        //}




        #endregion 通用代码




    }
}
