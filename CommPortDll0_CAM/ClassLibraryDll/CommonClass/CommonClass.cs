
using ComPort;
using log4net;
using log4net.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CmdFile;

namespace CommonPortCmd
{
    public class Common
    {

        /// <summary>
        /// 握手返回数据包含
        /// </summary>
        public string str_Port_Rec = "0F 00";
        //private string str_wo_shou = "34 04";
        /// <summary>
        /// 串口接收到错误数据命令头
        /// </summary>
        //private static string DATAERROString = "34 04";
        public static string Model = "";//设备种类型号制定


        //定义一个委托，用于串口数据主动上报
        public delegate void RecDataSend(object send, ActiveReporting e);
        public event RecDataSend RecDataSendEventHander;
        private INICmds INICmds;

        #region 类的私有变量
        private SerialPortWrapper serialWrapper;


        private string station = "";//保存当前站位信息

        //标识一条命令是否是这个互动上报
        //private bool activeData = false;
        private static string sendStringCmdToHex = string.Empty;//保存接受到的命令
        private static string recByteToHex = string.Empty;//保存上位机返回的数据
        private static string recActiyData = string.Empty;//保存组东上报数据

        private static SerialPort comm = new SerialPort();//串口程序的主要处理类

        //保存接受到的串口数据
        private static string recDataStr = "";

        /// <summary>
        /// 延迟数据发送锁
        /// </summary>
        private AutoResetEvent delyEvent;

        /// <summary>
        /// 电机运动完成
        /// </summary>
        private AutoResetEvent DianJiYuDongOkEvent ;

        //握手事件锁
        AutoResetEvent woShouEvent = new AutoResetEvent(false);

        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private static AutoResetEvent wEvent = new AutoResetEvent(false);  //握手等待

        private static Byte[] buffer = new Byte[1024];//临时存放串口数据

        /// <summary>
        /// 动态保存第二次数据
        /// </summary>
        private ArrayList secondRecList = new ArrayList();//数据接收不完整时，长度大于命令长度



        private string str_woshou = "";//保存握手指令Hex

        static MMI_Relationship relationship = new MMI_Relationship();//依赖项目处理


        #endregion

        public Common()
        {
            INICmds = new INICmds();
            INICmds.FielPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "cmds.ini";
            XMLConfig config = new XMLConfig();
            station = config.ReadConfing(System.IO.Directory.GetCurrentDirectory(), out Model, out str_woshou);
            serialWrapper = new SerialPortWrapper();
            serialWrapper.Report += new SerialPortWrapper.ReportEventHandler(ReportHandler);
            delyEvent = new AutoResetEvent(true);
            DianJiYuDongOkEvent = new AutoResetEvent(false);
        }
        /// <summary>
        /// 主动上报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReportHandler(object sender, ReportEventArgs e)
        {
            byte[] data = e.Data;
            recActiyData = ShujuChuli.ByteToHexString(data);
            Log.Debug("ShujuChuli.ByteToHexString(data) recActiyData=" + recActiyData);
            if (RecDataSendEventHander != null)
            {

                if (DelayCmd.DelaydDat(recActiyData))//recActiyData 主动上报数据
                {
                    Log.Debug("DelayCmd.DelaydDat(recActiyData) recActiyData-->" + recActiyData);

                    delyEvent.Set();
                    //DianJiYuDongOkEvent.Set();
                }

                RecDataSendEventHander(this, new ActiveReporting(recActiyData));
            }
        }


        private static void InitLog4Net()
        {
            //读取配置文件
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "LowerPC.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);

        }

        /// <summary>
        /// 握手函数，调用该函数可以发送握手指令，和串口握手，只有握手成功后 才可以发送其它的串口数据
        /// </summary>
        public bool ConnectPort()
        {

            string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    try
                    {
                        Log.Debug("portname" + port);

                        serialWrapper.Open(port);
                        byte[] sbuf = ShujuChuli.HexStringToBytes(str_woshou);
                        byte[] resp;

                        serialWrapper.SendRecv(sbuf, out resp);

                        string hex = ShujuChuli.ByteToHexString(resp);

                        if (hex.IndexOf("34 04 " + station + " 0F 00") != -1)
                        {
                            return true;
                        }




                    }
                    catch (Exception)
                    {
                        serialWrapper.Close();

                    }

                }
            return false;
        }

        #region 数据发送
        /// <summary>
        ///串口数据发送
        /// </summary>
        /// <param name="strParam"></param>
        private void SendHex(string strParam, out string recStr)
        {
            byte[] data = ShujuChuli.HexStringToBytes(strParam);
            byte[] resp;
            string recStrHex;
            try
            {
                Log.Debug("serialWrapper start use" + strParam);
                serialWrapper.SendRecv(data, out resp);
                recStrHex = ShujuChuli.ByteToHexString(resp);
                recStr = StrHexToStrPram.RecDataToHexPram(recStrHex);//返回解析结果
            }
            catch (TimeoutException)
            {
                Log.Debug("TimeoutException  already happen");
                recStr = "status=timeout";
                Thread.Sleep(1000);
                try
                {
                    Log.Debug("Send again");
                    serialWrapper.SendRecv(data, out resp);
                    recStrHex = ShujuChuli.ByteToHexString(resp);
                    recStr = StrHexToStrPram.RecDataToHexPram(recStrHex);//返回解析结果

                }
                catch (TimeoutException)
                {
                    Log.Debug("TimeoutException  already happen");
                    recStr = "status=timeout";
                }


            }
            catch (InvalidDataException)//数据出错
            {
                Log.Debug("InvalidDataException already happen");
                Thread.Sleep(500);
                try
                {
                    Log.Debug("Send again");
                    serialWrapper.SendRecv(data, out resp);
                    recStrHex = ShujuChuli.ByteToHexString(resp);
                    recStr = StrHexToStrPram.RecDataToHexPram(recStrHex);//返回解析结果

                }
                catch (TimeoutException)
                {
                    Log.Debug("TimeoutException  already happen");
                    recStr = "status=timeout";
                }

            }
        }

        #endregion

        public bool SendCommand(string strCmd)
        {
            string recStr = string.Empty;
            string strHex = string.Empty;
            INICmds.GetEqumentCommand(strCmd,out strHex);
            SendHex(strHex, out recStr);
            Log.Debug(strCmd + "==" + recStr);
            return true;
        }


        public bool SendCommand(string strCmd, out string recStr)
        {
            Log.Debug("Rrs no pram cmd  SendCommand=" + strCmd);

            //string strHex = StrPramToHexPram.StrToHex(strCmd);
            //string recStr = string.Empty;
            string strHex = string.Empty;
            INICmds.GetEqumentCommand(strCmd, out strHex);
            if (strHex != "")
            {

                SendHex(strHex, out recStr);

                Log.Debug(strCmd + "==" + recStr);

                return true;
            }
            else if (strCmd == "到位检测")
            {
                strHex = "72 05 " + station + " 01 01 FF 81";

                SendHex(strHex, out recStr);
                Log.Debug(strCmd + "==" + recStr);
                //recStr = recDataStr;

                return true;
            }
            else if (strCmd == "复位")
            {
                strHex = "72 04 " + station + " 0F 00 81";

                SendHex(strHex, out recStr);
                Log.Debug(strCmd + "==" + recStr);
                return true;
            }
            else if (strCmd == "报警")
            {
                if (station != "11")
                {
                    strHex = "72 05 " + station + " 05 01 00 81";
                }
                else
                {
                    strHex = "72 05 11 05 18 00 81";
                }


                SendHex(strHex, out recStr);
                Log.Debug(strCmd + "==" + recStr);
                return true;
            }
            else if (strCmd.IndexOf("测试完成状态") != -1)
            {
                RecTestOK(strCmd, out recStr);
                return true;
            }
            else if (strCmd.IndexOf("3站取放开始") != -1)
            {
                StartStation_3(strCmd, out recStr);
                return true;
            }
            else if (strCmd.IndexOf("4站取放开始") != -1)
            {
                recStr = "status=OK";
                StartStation_4(strCmd, out recStr);
                return true;
            }
            else
            {
                recStr = "status=NOT";
                return true;
            }


        }

        private void StartStation_4(string strCmd, out string recStr)
        {
            //string quStation = "10000";
            string fangStation = "19400";
            string yuanStation = "19400";
            string s1;
            //int timeAbrsore = 500;//吸取时间
            while (true)
            {
                SendCommand("3站出仓原点检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            while (true)
            {
                SendCommand("3站取放3抬起检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            SendCommand("3站电机放", fangStation, out s1);//放
            //DianJiYuDongOkEvent.WaitOne();
            Thread.Sleep(500);
            while (true)
            {
                SendCommand("3站取放3出仓检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            //3站取放3松开
            SendCommand("3站取放3下压", out s1);
            while (true)
            {
                SendCommand("3站取放3下压检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }

            }
        
            SendCommand("3站取放3松开", out s1);
            Thread.Sleep(100);
            SendCommand("3站取放3抬起", out s1);

            while (true)
            {
                SendCommand("3站取放3抬起检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            //3站取放3出仓检测
            SendCommand("3站电机", yuanStation, out s1);//原点
            Thread.Sleep(500);
            //DianJiYuDongOkEvent.WaitOne();
            recStr = "status=OK";
        }

        //3站测试开始
        //
        //
        private void StartStation_3(string strCmd, out string recStr)
        {
            string quStation = "10900";
            //string fangStation = "30000";
            string yuanStation = "10900";
            string s1;
            int timeAbrsore = 500;//吸取时间
            while (true)
            {
                SendCommand("3站出仓原点检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            while (true)
            {
                //3站取放3抬起检测
                SendCommand("3站取放3抬起检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }

            }

            SendCommand("3站电机取", quStation, out s1);//取
            //DianJiYuDongOkEvent.WaitOne();
            Thread.Sleep(500);
            while (true)
            {
                SendCommand("3站右取放检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }

            }
            
            //Thread.Sleep(2000);
            SendCommand("3站取放3下压", out s1);
            while (true)
            {
                SendCommand("3站取放3下压检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }

            }
            SendCommand("3站取放3吸取", out s1);
            Thread.Sleep(timeAbrsore);
            SendCommand("3站取放3抬起", out s1);
            while (true)
            {
                //3站取放3抬起检测
                SendCommand("3站取放3抬起检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }

            }
            while (true)
            {
                SendCommand("3站取放3吸取检测", out s1);
                if (s1 == "status=OK")
                {
                    break;
                }
                else
                {
                    SendCommand("3站取放3下压", out s1);
                    while (true)
                    {
                        SendCommand("3站取放3下压检测", out s1);
                        if (s1 == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }

                    }
                    SendCommand("3站取放3吸取", out s1);
                    Thread.Sleep(timeAbrsore);
                    SendCommand("3站取放3抬起", out s1);
                    Thread.Sleep(1000);
                }
            }


            SendCommand("3站电机放", yuanStation, out s1);//放
            Thread.Sleep(500);

            recStr = "status=OK";
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="strParam">接受到的命令</param>
        ///// <param name="param">接受到的，参数</param>
        ///// <returns>返回命令执行的结果数据</returns>
        public bool SendCommand(string strCmd, string param, out string recStr)
        {
            Log.Debug("Rrs cmd  SendCommand=" + strCmd);

            //解析发送过来的字符串命令
            string strHex = string.Empty;
            INICmds.GetEqumentCommand(strCmd, out strHex);
            string sendHex = (strHex + " " + ShujuChuli.StrToHex(param) + " 81");

            if (strHex != "")
            {


                if (DelayCmd.WhetherCmd(strCmd))//命令是一条延迟命令
                {
                    Log.Debug("Cmd is delay cmd -->" + strCmd + param);
                    delyEvent.WaitOne();//等待延迟数据返回
                    SendHex(sendHex, out recStr);//发送命令

                    return true;
                }
                else
                {

                    SendHex(sendHex, out recStr);//发送命令
                    Log.Debug(strCmd + "==" + recStr);
                    return true;
                }

            }
            else if (strCmd == "1站取放")
            {
                LeftDo(param);
                RightDo(param);
                MidDo(param);
                recStr = "status=OK";
                return true;
            }
            else if (strCmd == "报警")
            {
                if (station != "11")
                {
                    strHex = "72 05 " + station + " 05 01 00 81";
                }
                else
                {
                    strHex = "72 05 11 05 18 " + ShujuChuli.StrToHex(param) + " 81";
                }

                SendHex(strHex, out recStr);//发送命令
                Log.Debug(strCmd + "==" + recStr);
                recStr = recDataStr;

                return true;
            }
            else if (strCmd == "报警取消")
            {
                if (station != "11")
                {
                    strHex = "72 05 " + station + " 05 01 FF 81";
                }
                else
                {
                    strHex = "72 05 11 05 18 00 81";
                }

                SendHex(strHex, out recStr);//发送命令
                Log.Debug(strCmd + "==" + recStr);
                recStr = recDataStr;

                return true;
            }
            else
            {
                recStr = "status=NOT";
                return true;
            }


        }

        private void RecTestOK(string strCmd, out string recStr)
        {
            switch (strCmd)
            {
                case "1站测试完成状态":
                    SendCommand("1站手机固定", out recStr);
                    break;
                case "2站测试完成状态":
                    //SendCmmond("1站手机固定", out recStr);
                    break;
                case "3站测试完成状态":
                    string[] rec_3 = new string[5];
                    while (true)
                    {
                        SendCommand("3站手机松开", out rec_3[4]);
                        SendCommand("3站45度放平检测", out rec_3[0]);
                        SendCommand("3站前白卡远离检测", out rec_3[1]);
                        SendCommand("3站前白卡上升检测", out rec_3[2]);

                        if (rec_3[0] == "status=OK" && rec_3[1] == "status=OK" && rec_3[2] == "status=OK")
                        {
                            break;
                        }
                        Thread.Sleep(50);
                    }


                    break;
                case "4站测试完成状态":
                    string[] rec_4 = new string[5];
                    while (true)
                    {
                        SendCommand("4站天板远离检测", out rec_4[0]);
                        //SendCommand("3站前白卡远离检测", out rec[1]);
                        //SendCommand("3站前白卡上升检测", out rec[2]);

                        if (rec_4[0] == "status=OK" /* && rec[1] == "status=OK" && rec[2] == "status=OK"*/)
                        {
                            break;
                        }
                        Thread.Sleep(50);
                    }
                    break;
                case "5站测试完成状态":
                    string[] rec_5 = new string[5];
                    while (true)
                    {
                        SendCommand("5站隔离上升检测", out rec_5[0]);
                        SendCommand("5站人工耳远离检测", out rec_5[1]);
                        //SendCommand("3站前白卡上升检测", out rec[2]);

                        if (rec_5[0] == "status=OK" && rec_5[1] == "status=OK"/* && rec[2] == "status=OK"*/)
                        {
                            break;
                        }
                        Thread.Sleep(50);
                    }
                    break;
                case "6站测试完成状态":
                    string[] rec_6 = new string[5];
                    while (true)
                    {
                        SendCommand("6站电机X位置检测", out rec_6[0]);
                        SendCommand("6站电机Y位置检测", out rec_6[1]);
                        //SendCommand("3站前白卡上升检测", out rec[2]);

                        if (rec_6[0] == "status=OK" && rec_6[1] == "status=OK"/* && rec[2] == "status=OK"*/)
                        {
                            break;
                        }
                        Thread.Sleep(50);
                    }

                    break;
            }
            recStr = "status=OK";
        }

      
        /// <summary>
        /// 左运动
        /// </summary>
        /// <param name="pram"></param>
        private void LeftDo(string pram)
        {
            string str = "OK";
            str = this.OrginOK(pram);
            if (str == "status=OK")//电机在原点，且上到位
            {
                SendCommand("1站电机左运动", out str);
                Thread.Sleep(500);

                while (true)
                {
                    string s = LeftOK(pram);
                    if (s == "status=OK")
                    {
                        break;
                    }
                }
                LiftDownUpAbsorb(pram);



            }
        }

        /// <summary>
        /// 右运动
        /// </summary>
        /// <param name="pram"></param>
        private void RightDo(string pram)
        {
            string str = "OK";
            this.LeftOK(pram);
            SendCommand("1站电机右运动", out str);
            //Thread.Sleep(2000);
            Thread.Sleep(500);
            //Thread.Sleep(500);
            while (true)
            {
                str = RightOK(pram);
                if (str == "status=OK")
                {
                    break;
                }
            }
            RightDownAbsorb(pram);

        }
        /// <summary>
        /// 中运动
        /// </summary>
        /// <param name="pram"></param>
        private void MidDo(string pram)
        {
            string str = "OK";
            str = this.RightOK(pram);
            if (str == "status=OK")//
            {
                SendCommand("1站电机中运动", out str);
                Thread.Sleep(500);
                Thread.Sleep(500);
                while (true)
                {
                    string s = OrginOK(pram);
                    if (s == "status=OK")
                    {
                        break;
                    }
                    Thread.Sleep(100);

                }

            }
        }

        /// <summary>
        /// 原点检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string OrginOK(string pram)
        {
            string res = "";
            string str = "";

            pram = pram.ToUpper();

            while (true)
            {
                SendCommand("1站取放原点检测", out str);
                if (str == "status=OK")
                {
                    res = "status=OK";
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            return res;
        }

        /// <summary>
        /// 左检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string LeftOK(string pram)
        {

            string res = "";
            string[] str = new string[20];
            for (int i = 0; i < 20; i++)
            {
                str[i] = "";
            }

            pram = pram.ToUpper();
            while (true)
            {

                SendCommand("1站前到位检测", out str[3]);

                SendCommand("1站取放1抬起检测", out str[1]);
                SendCommand("1站取放2抬起检测", out str[2]);

                //str[1] == "status=OK" &&


                if (str[2] == "status=OK" && str[3] == "status=OK" && str[1] == "status=OK")
                {
                    res = "status=OK";
                    break;
                }
                Thread.Sleep(100);
            }
            return res;
        }

        /// <summary>
        /// 右检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string RightOK(string pram)
        {

            string res = "";
            string[] str = new string[20];
            for (int i = 0; i < 20; i++)
            {
                str[i] = "";
            }
            while (true)
            {
                SendCommand("1站后到位检测", out str[1]);
                if (str[1] == "status=OK")
                {
                    res = "status=OK";
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }

            }
            return res;
        }

        /// <summary>
        /// 左边吸取动作
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string LiftDownUpAbsorb(string pram)
        {
            #region
            int timeAbsorb = 500;//吸取时间
            int timeErr = 500;
            int timeUp = 100;
            string res = "";
            string[] str = new string[20];
            for (int i = 0; i < 20; i++)
            {
                str[i] = "";
            }
            pram = pram.ToUpper();
            switch (pram)
            {
                case "1"://1电机
                    while (true)
                    {
                        SendCommand("1站取放1下压", out str[1]);
                        SendCommand("1站取放1吸取", out str[1]);
                        Thread.Sleep(timeAbsorb);

                        while (true)
                        {

                            SendCommand("1站取放1下压检测", out str[2]);

                            if (str[2] == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeErr);
                            }
                        }
                        SendCommand("1站取放1抬起", out str[4]);
                        Thread.Sleep(timeErr);
                        SendCommand("1站取放1抬起检测", out str[2]);

                        if (str[2] == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeErr);
                        }

                    }
                    break;
                case "2"://2电机

                    while (true)
                    {

                        SendCommand("1站取放2下压", out str[1]);
                        SendCommand("1站取放2吸取", out str[1]);

                        Thread.Sleep(timeAbsorb);
                        while (true)
                        {

                            SendCommand("1站取放2下压检测", out str[2]);

                            if (str[2] == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeErr);
                            }
                        }
                        SendCommand("1站取放2抬起", out str[4]);

                        Thread.Sleep(timeErr);


                        SendCommand("1站取放2抬起检测", out str[2]);

                        if (str[2] == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeErr);
                        }

                    }
                    break;
                case "3"://1 2电机

                    while (true)
                    {
                        SendCommand("1站取放1下压", out str[1]);
                        SendCommand("1站取放1吸取", out str[2]);
                        SendCommand("1站取放2下压", out str[3]);
                        SendCommand("1站取放2吸取", out str[4]);
                        Thread.Sleep(timeAbsorb);

                        while (true)
                        {

                            SendCommand("1站取放1下压检测", out str[2]);

                            if (str[2] == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeErr);
                            }
                        }
                        while (true)
                        {

                            SendCommand("1站取放2下压检测", out str[2]);

                            if (str[2] == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeErr);
                            }
                        }


                        SendCommand("1站取放1抬起", out str[4]);
                        SendCommand("1站取放2抬起", out str[4]);
                        Thread.Sleep(timeErr);
                        SendCommand("1站取放1抬起检测", out str[2]);
                        SendCommand("1站取放2抬起检测", out str[1]);

                        if (str[2] == "status=OK" && str[1] == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeErr);
                        }

                    }
                    break;




























                case "4"://3电机
                    SendCommand("1站升降气缸3降下", out str[12]);
                    SendCommand("1站取放吸嘴3吸取", out str[13]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降3检测", out str[14]);
                        //  SendCommand("1站吸嘴3检测", out str[15]);
                        if (str[14] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸3升起", out str[16]);



                    break;

                case "7"://1 2 3电机
                    SendCommand("1站取放吸嘴2吸取", out str[0]);
                    SendCommand("1站取放吸嘴1吸取", out str[0]);
                    SendCommand("1站取放吸嘴3吸取", out str[0]);

                    SendCommand("1站升降气缸1降下", out str[0]);
                    SendCommand("1站升降气缸2降下", out str[0]);
                    SendCommand("1站升降气缸3降下", out str[0]);

                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降1检测", out str[8]);
                        //SendCommand("1站吸嘴1检测", out str[9]);

                        SendCommand("1站下降2检测", out str[10]);
                        //SendCommand("1站吸嘴2检测", out str[11]);

                        SendCommand("1站下降3检测", out str[12]);
                        //SendCommand("1站吸嘴3检测", out str[13]);

                        if (str[8] == "status=OK" && str[10] == "status=OK" && str[12] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1升起", out str[14]);
                    SendCommand("1站升降气缸2升起", out str[15]);
                    SendCommand("1站升降气缸3升起", out str[16]);

                    break;
                case "8"://4电机
                    SendCommand("1站升降气缸4降下", out str[1]);
                    SendCommand("1站取放吸嘴4吸取", out str[2]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降4检测", out str[8]);
                        //SendCommand("1站吸嘴4检测", out str[9]);
                        if (str[8] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸4升起", out str[10]);

                    //while (true)
                    //{
                    //    SendCommand("1站吸嘴4检测", out str[11]);
                    //    if (str[11] == "status=OK")
                    //    {
                    //        res = "status=OK";
                    //        break;
                    //    }
                    //}

                    break;
                case "15"://1 2 3 4电机
                    SendCommand("1站升降气缸3降下", out str[0]);
                    SendCommand("1站取放吸嘴3吸取", out str[1]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);
                    SendCommand("1站升降气缸2降下", out str[8]);
                    SendCommand("1站取放吸嘴2吸取", out str[9]);

                    SendCommand("1站升降气缸1降下", out str[1]);
                    SendCommand("1站取放吸嘴1吸取", out str[2]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);

                        SendCommand("1站下降1检测", out str[16]);
                        //SendCommand("1站吸嘴1检测", out str[17]);

                        SendCommand("1站下降2检测", out str[10]);
                        //SendCommand("1站吸嘴2检测", out str[11]);
                        SendCommand("1站下降2检测", out str[12]);
                        //SendCommand("1站吸嘴3检测", out str[13]);
                        SendCommand("1站下降4检测", out str[14]);
                        //SendCommand("1站吸嘴4检测", out str[15]);
                        if (str[10] == "status=OK" && str[12] == "status=OK" && str[14] == "status=OK" && str[16] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1升起", out str[4]);
                    SendCommand("1站升降气缸2升起", out str[4]);
                    SendCommand("1站升降气缸3升起", out str[5]);
                    SendCommand("1站升降气缸4升起", out str[6]);

                    //while (true)
                    //{
                    //    Thread.Sleep(timeUp);

                    //    SendCommand("1站吸嘴1检测", out str[20]);
                    //    SendCommand("1站吸嘴2检测", out str[21]);
                    //    SendCommand("1站吸嘴3检测", out str[22]);
                    //    SendCommand("1站吸嘴4检测", out str[23]);
                    //    if (str[20] == "status=OK" && str[21] == "status=OK" && str[22] == "status=OK" && str[23] == "status=OK")
                    //    {
                    //        res = "status=OK";
                    //        break;
                    //    }
                    //}


                    break;
                case "16"://5电机
                    SendCommand("1站升降气缸5降下", out str[6]);
                    SendCommand("1站取放吸嘴5吸取", out str[7]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降5检测", out str[8]);
                        //SendCommand("1站吸嘴5检测", out str[9]);
                        if (str[8] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸5升起", out str[10]);

                    //while (true)
                    //{
                    //    SendCommand("1站吸嘴5检测", out str[11]);
                    //    if (str[11] == "status=OK")
                    //    {
                    //        res = "status=OK";
                    //        break;
                    //    }
                    //}

                    break;
                case "24"://4 5电机

                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);

                    SendCommand("1站升降气缸5降下", out str[3]);
                    SendCommand("1站取放吸嘴5吸取", out str[4]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);

                        SendCommand("1站下降4检测", out str[12]);
                        //SendCommand("1站吸嘴4检测", out str[15]);

                        SendCommand("1站下降5检测", out str[8]);
                        //SendCommand("1站吸嘴5检测", out str[9]);

                        if (str[8] == "status=OK" && str[12] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸4升起", out str[6]);
                    SendCommand("1站升降气缸5升起", out str[7]);

                    break;
                case "28"://3 4 5电机

                    SendCommand("1站升降气缸3降下", out str[0]);
                    SendCommand("1站取放吸嘴3吸取", out str[1]);

                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);

                    SendCommand("1站升降气缸5降下", out str[3]);
                    SendCommand("1站取放吸嘴5吸取", out str[4]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);



                        SendCommand("1站下降3检测", out str[11]);
                        //SendCommand("1站吸嘴3检测", out str[13]);

                        SendCommand("1站下降4检测", out str[12]);
                        //SendCommand("1站吸嘴4检测", out str[15]);

                        SendCommand("1站下降5检测", out str[8]);
                        //SendCommand("1站吸嘴5检测", out str[9]);

                        if (str[8] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸3升起", out str[5]);
                    SendCommand("1站升降气缸4升起", out str[6]);
                    SendCommand("1站升降气缸5升起", out str[7]);

                    break;
                case "30":// 2 3 4 5电机


                    SendCommand("1站升降气缸2降下", out str[8]);
                    SendCommand("1站取放吸嘴2吸取", out str[9]);

                    SendCommand("1站升降气缸3降下", out str[0]);
                    SendCommand("1站取放吸嘴3吸取", out str[1]);

                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);

                    SendCommand("1站升降气缸5降下", out str[3]);
                    SendCommand("1站取放吸嘴5吸取", out str[4]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);


                        SendCommand("1站下降2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[11]);

                        SendCommand("1站下降3检测", out str[9]);
                        //SendCommand("1站吸嘴3检测", out str[13]);

                        SendCommand("1站下降4检测", out str[10]);
                        //SendCommand("1站吸嘴4检测", out str[15]);

                        SendCommand("1站下降5检测", out str[11]);
                        //SendCommand("1站吸嘴5检测", out str[9]);

                        if (str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸2升起", out str[4]);
                    SendCommand("1站升降气缸3升起", out str[5]);
                    SendCommand("1站升降气缸4升起", out str[6]);
                    SendCommand("1站升降气缸5升起", out str[7]);

                    break;
                case "31"://1 2 3 4 5电机

                    SendCommand("1站升降气缸3降下", out str[0]);
                    SendCommand("1站取放吸嘴3吸取", out str[1]);

                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);

                    SendCommand("1站升降气缸2降下", out str[8]);
                    SendCommand("1站取放吸嘴2吸取", out str[9]);

                    SendCommand("1站升降气缸1降下", out str[8]);
                    SendCommand("1站取放吸嘴1吸取", out str[9]);

                    SendCommand("1站升降气缸5降下", out str[3]);
                    SendCommand("1站取放吸嘴5吸取", out str[4]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);

                        SendCommand("1站下降1检测", out str[10]);
                        //SendCommand("1站吸嘴1检测", out str[11]);

                        SendCommand("1站下降2检测", out str[9]);
                        //SendCommand("1站吸嘴2检测", out str[11]);

                        SendCommand("1站下降3检测", out str[11]);
                        //SendCommand("1站吸嘴3检测", out str[13]);

                        SendCommand("1站下降4检测", out str[12]);
                        //SendCommand("1站吸嘴4检测", out str[15]);

                        SendCommand("1站下降5检测", out str[8]);
                        //SendCommand("1站吸嘴5检测", out str[9]);

                        if (str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1升起", out str[4]);
                    SendCommand("1站升降气缸2升起", out str[4]);
                    SendCommand("1站升降气缸3升起", out str[5]);
                    SendCommand("1站升降气缸4升起", out str[6]);
                    SendCommand("1站升降气缸5升起", out str[7]);

                    break;
            }
            #endregion
            return res;
        }

        /// <summary>
        /// 右边吸取动作
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string RightDownAbsorb(string pram)
        {
            #region
            // int timeAbsorb = 500;//吸取时间
            int timeUp_ = 100;//间隔多久放气
            int timeUp = 100;//间隔多久放气
            string res = "";
            string[] str = new string[20];
            for (int i = 0; i < 20; i++)
            {
                str[i] = "";
            }
            pram = pram.ToUpper();

            switch (pram)
            {
                case "1"://1电机
                    SendCommand("1站取放1下压", out str[5]);

                    Thread.Sleep(timeUp_);
                    SendCommand("1站取放1松开", out str[1]);
                    while (true)
                    {

                        SendCommand("1站取放1下压检测", out str[5]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }

                    }
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放1抬起", out str[0]);
                    Thread.Sleep(100);

                    while (true)
                    {
                        SendCommand("1站取放1抬起检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    break;




                case "2"://2电机

                    SendCommand("1站取放2下压", out str[5]);
                    Thread.Sleep(timeUp_);
                    SendCommand("1站取放2松开", out str[1]);
                    while (true)
                    {

                        SendCommand("1站取放2下压检测", out str[5]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }

                    }
                    Thread.Sleep(timeUp);

                    SendCommand("1站取放2抬起", out str[0]);
                    Thread.Sleep(100);

                    while (true)
                    {
                        SendCommand("1站取放2抬起检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    break;
                case "3"://1 2电机


                    SendCommand("1站取放1下压", out str[5]);
                    SendCommand("1站取放2下压", out str[5]);
                    Thread.Sleep(timeUp_);
                    SendCommand("1站取放1松开", out str[1]);
                    SendCommand("1站取放2松开", out str[1]);
                    while (true)
                    {

                        SendCommand("1站取放1下压检测", out str[5]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }

                    }
                    while (true)
                    {

                        SendCommand("1站取放2下压检测", out str[5]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }

                    }
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放1抬起", out str[0]);
                    SendCommand("1站取放2抬起", out str[0]);
                    Thread.Sleep(100);

                    while (true)
                    {
                        SendCommand("1站取放1抬起检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站取放2抬起检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    break;



































                case "4"://3电机
                    SendCommand("1站升降气缸3升起", out str[0]);
                    while (true)
                    {
                        //   Thread.Sleep(timeAbsorb);
                        SendCommand("1站上升3检测", out str[2]);
                        // SendCommand("1站吸嘴3检测", out str[3]);
                        SendCommand("1站电机1右边检测", out str[4]);
                        if (str[2] == "status=OK" && str[4] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸3降下", out str[5]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴3放气", out str[31]);
                    while (true)
                    {

                        SendCommand("1站下降3检测", out str[6]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸3升起", out str[0]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "7"://1 2 3电机
                    SendCommand("1站升降气缸1升起", out str[0]);
                    SendCommand("1站升降气缸2升起", out str[0]);
                    SendCommand("1站升降气缸3升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str[2]);
                        //SendCommand("1站吸嘴1检测", out str[3]);
                        SendCommand("1站电机1右边检测", out str[4]);

                        SendCommand("1站上升2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[9]);
                        // SendCommand("1站电机2左边检测", out str[10]);

                        SendCommand("1站上升3检测", out str[5]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        // SendCommand("1站电机3左边检测", out str[7]);

                        if (str[2] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[8] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1降下", out str[5]);
                    SendCommand("1站升降气缸2降下", out str[5]);
                    SendCommand("1站升降气缸3降下", out str[6]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str[1]);
                    SendCommand("1站取放吸嘴2放气", out str[1]);
                    SendCommand("1站取放吸嘴3放气", out str[1]);
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str[6]);
                        SendCommand("1站下降2检测", out str[8]);
                        SendCommand("1站下降3检测", out str[7]);
                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸1升起", out str[0]);
                    SendCommand("1站升降气缸2升起", out str[0]);
                    SendCommand("1站升降气缸3升起", out str[0]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str[1]);
                        SendCommand("1站上升2检测", out str[2]);
                        SendCommand("1站上升3检测", out str[3]);
                        if (str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }
                    break;
                case "8"://4电机

                    SendCommand("1站升降气缸4升起", out str[0]);
                    while (true)
                    {
                        //   Thread.Sleep(timeAbsorb);
                        SendCommand("1站上升4检测", out str[2]);
                        //SendCommand("1站吸嘴4检测", out str[3]);
                        SendCommand("1站电机1右边检测", out str[4]);
                        if (str[2] == "status=OK" && str[4] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸4降下", out str[5]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴4放气", out str[31]);
                    while (true)
                    {

                        SendCommand("1站下降4检测", out str[6]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸4升起", out str[0]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                #region
                case "9"://1 4电机
                    SendCommand("1站升降气缸1降下", out str[0]);
                    SendCommand("1站取放吸嘴1吸取", out str[1]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);

                    while (true)
                    {
                        // Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降1检测", out str[2]);
                        SendCommand("1站吸嘴1检测", out str[3]);
                        SendCommand("1站下降4检测", out str[8]);
                        SendCommand("1站吸嘴4检测", out str[9]);
                        if (str[2] == "status=OK" && str[3] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1升起", out str[4]);
                    SendCommand("1站升降气缸4升起", out str[10]);

                    while (true)
                    {
                        Thread.Sleep(timeUp);
                        SendCommand("1站吸嘴1检测", out str[5]);
                        SendCommand("1站吸嘴4检测", out str[9]);
                        if (str[5] == "status=OK" && str[9] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "A"://2 4电机
                    SendCommand("1站升降气缸2降下", out str[0]);
                    SendCommand("1站取放吸嘴2吸取", out str[1]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);

                    while (true)
                    {
                        // Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降2检测", out str[2]);
                        SendCommand("1站吸嘴2检测", out str[3]);
                        SendCommand("1站下降4检测", out str[8]);
                        SendCommand("1站吸嘴4检测", out str[9]);
                        if (str[2] == "status=OK" && str[3] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸2升起", out str[4]);
                    SendCommand("1站升降气缸4升起", out str[10]);

                    while (true)
                    {
                        Thread.Sleep(timeUp);
                        SendCommand("1站吸嘴2检测", out str[5]);
                        SendCommand("1站吸嘴4检测", out str[9]);
                        if (str[5] == "status=OK" && str[9] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "B"://1 2 4电机


                    SendCommand("1站升降气缸2降下", out str[0]);
                    SendCommand("1站取放吸嘴2吸取", out str[1]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);
                    SendCommand("1站升降气缸1降下", out str[8]);
                    SendCommand("1站取放吸嘴1吸取", out str[9]);

                    while (true)
                    {
                        //  Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降1检测", out str[10]);
                        SendCommand("1站吸嘴1检测", out str[11]);
                        SendCommand("1站下降2检测", out str[12]);
                        SendCommand("1站吸嘴2检测", out str[13]);
                        SendCommand("1站下降4检测", out str[14]);
                        SendCommand("1站吸嘴4检测", out str[15]);
                        if (str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK" && str[15] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1升起", out str[4]);
                    SendCommand("1站升降气缸2升起", out str[5]);
                    SendCommand("1站升降气缸4升起", out str[6]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        Thread.Sleep(timeUp);

                        SendCommand("1站吸嘴1检测", out str[16]);
                        SendCommand("1站吸嘴2检测", out str[17]);
                        SendCommand("1站吸嘴4检测", out str[18]);
                        if (str[16] == "status=OK" && str[17] == "status=OK" && str[18] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "C"://3 4电机
                    SendCommand("1站升降气缸4降下", out str[0]);
                    SendCommand("1站取放吸嘴4吸取", out str[1]);
                    SendCommand("1站升降气缸3降下", out str[6]);
                    SendCommand("1站取放吸嘴3吸取", out str[7]);

                    while (true)
                    {
                        //  Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降4检测", out str[2]);
                        SendCommand("1站吸嘴4检测", out str[3]);
                        SendCommand("1站下降3检测", out str[8]);
                        SendCommand("1站吸嘴3检测", out str[9]);
                        if (str[2] == "status=OK" && str[3] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸4升起", out str[4]);
                    SendCommand("1站升降气缸3升起", out str[10]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        Thread.Sleep(timeUp);
                        SendCommand("1站吸嘴3检测", out str[5]);
                        SendCommand("1站吸嘴4检测", out str[9]);
                        if (str[5] == "status=OK" && str[9] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }


                    break;
                case "D"://1 3 4电机

                    SendCommand("1站升降气缸3降下", out str[0]);
                    SendCommand("1站取放吸嘴3吸取", out str[1]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);
                    SendCommand("1站升降气缸1降下", out str[8]);
                    SendCommand("1站取放吸嘴1吸取", out str[9]);

                    while (true)
                    {
                        // Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降1检测", out str[10]);
                        SendCommand("1站吸嘴1检测", out str[11]);
                        SendCommand("1站下降3检测", out str[12]);
                        SendCommand("1站吸嘴3检测", out str[13]);
                        SendCommand("1站下降4检测", out str[14]);
                        SendCommand("1站吸嘴4检测", out str[15]);
                        if (str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK" && str[15] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1升起", out str[4]);
                    SendCommand("1站升降气缸2升起", out str[5]);
                    SendCommand("1站升降气缸4升起", out str[6]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        Thread.Sleep(timeUp);

                        SendCommand("1站吸嘴1检测", out str[16]);
                        SendCommand("1站吸嘴3检测", out str[17]);
                        SendCommand("1站吸嘴4检测", out str[18]);
                        if (str[16] == "status=OK" && str[17] == "status=OK" && str[18] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "E"://2 3 4电机
                    SendCommand("1站升降气缸3降下", out str[0]);
                    SendCommand("1站取放吸嘴3吸取", out str[1]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站取放吸嘴4吸取", out str[7]);
                    SendCommand("1站升降气缸2降下", out str[8]);
                    SendCommand("1站取放吸嘴2吸取", out str[9]);

                    while (true)
                    {
                        // Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降2检测", out str[10]);
                        SendCommand("1站吸嘴2检测", out str[11]);
                        SendCommand("1站下降2检测", out str[12]);
                        SendCommand("1站吸嘴3检测", out str[13]);
                        SendCommand("1站下降4检测", out str[14]);
                        SendCommand("1站吸嘴4检测", out str[15]);
                        if (str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK" && str[15] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸2升起", out str[4]);
                    SendCommand("1站升降气缸3升起", out str[5]);
                    SendCommand("1站升降气缸4升起", out str[6]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        Thread.Sleep(timeUp);

                        SendCommand("1站吸嘴2检测", out str[16]);
                        SendCommand("1站吸嘴3检测", out str[17]);
                        SendCommand("1站吸嘴4检测", out str[18]);
                        if (str[16] == "status=OK" && str[17] == "status=OK" && str[18] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }


                    break;
                #endregion
                case "15"://1 2 3 4电机
                    SendCommand("1站升降气缸1升起", out str[0]);
                    SendCommand("1站升降气缸2升起", out str[0]);
                    SendCommand("1站升降气缸3升起", out str[0]);
                    SendCommand("1站升降气缸4升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str[2]);
                        //SendCommand("1站吸嘴1检测", out str[3]);
                        SendCommand("1站电机1右边检测", out str[4]);

                        SendCommand("1站上升2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[9]);
                        //  SendCommand("1站电机2左边检测", out str[10]);

                        SendCommand("1站上升3检测", out str[5]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        //  SendCommand("1站电机3左边检测", out str[7]);

                        SendCommand("1站上升4检测", out str[11]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        // SendCommand("1站电机4左边检测", out str[13]);


                        if (str[2] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[8] == "status=OK" && str[11] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1降下", out str[5]);
                    SendCommand("1站升降气缸2降下", out str[5]);
                    SendCommand("1站升降气缸3降下", out str[6]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str[1]);
                    SendCommand("1站取放吸嘴2放气", out str[1]);
                    SendCommand("1站取放吸嘴3放气", out str[1]);
                    SendCommand("1站取放吸嘴4放气", out str[1]);
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str[6]);
                        SendCommand("1站下降2检测", out str[8]);
                        SendCommand("1站下降3检测", out str[7]);
                        SendCommand("1站下降4检测", out str[9]);
                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸1升起", out str[0]);
                    SendCommand("1站升降气缸2升起", out str[0]);
                    SendCommand("1站升降气缸3升起", out str[0]);
                    SendCommand("1站升降气缸4升起", out str[0]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str[1]);
                        SendCommand("1站上升2检测", out str[2]);
                        SendCommand("1站上升3检测", out str[3]);
                        SendCommand("1站上升4检测", out str[4]);
                        if (str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "16"://5电机
                    SendCommand("1站升降气缸1升起", out str[0]);
                    while (true)
                    {
                        //   Thread.Sleep(timeAbsorb);
                        SendCommand("1站上升5检测", out str[2]);
                        //SendCommand("1站吸嘴5检测", out str[3]);
                        SendCommand("1站电机1右边检测", out str[4]);
                        if (str[2] == "status=OK" && str[4] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸5降下", out str[5]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴5放气", out str[1]);
                    while (true)
                    {

                        SendCommand("1站下降5检测", out str[6]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸5升起", out str[0]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        SendCommand("1站上升5检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "24"://4 5电机

                    SendCommand("1站升降气缸4升起", out str[0]);
                    SendCommand("1站升降气缸5升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str[11]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        // SendCommand("1站电机4左边检测", out str[13]);

                        SendCommand("1站上升5检测", out str[14]);
                        //SendCommand("1站吸嘴5检测", out str[15]);
                        SendCommand("1站电机1右边检测", out str[16]);

                        if (str[11] == "status=OK" && str[14] == "status=OK" && str[16] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站升降气缸5降下", out str[6]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴4放气", out str[1]);
                    SendCommand("1站取放吸嘴5放气", out str[3]);
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str[9]);
                        SendCommand("1站下降5检测", out str[10]);
                        if (str[9] == "status=OK" && str[10] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸4升起", out str[0]);
                    SendCommand("1站升降气缸5升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str[4]);
                        SendCommand("1站上升5检测", out str[5]);
                        if (str[4] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "28"://3 4 5电机

                    SendCommand("1站升降气缸3升起", out str[0]);
                    SendCommand("1站升降气缸4升起", out str[0]);
                    SendCommand("1站升降气缸5升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str[5]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        // SendCommand("1站电机3左边检测", out str[7]);

                        SendCommand("1站上升4检测", out str[11]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        // SendCommand("1站电机4左边检测", out str[13]);

                        SendCommand("1站上升5检测", out str[14]);
                        //SendCommand("1站吸嘴5检测", out str[15]);
                        SendCommand("1站电机1右边检测", out str[16]);

                        if (str[5] == "status=OK" && str[11] == "status=OK" && str[14] == "status=OK" && str[16] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸3降下", out str[6]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站升降气缸5降下", out str[6]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴3放气", out str[1]);
                    SendCommand("1站取放吸嘴4放气", out str[1]);
                    SendCommand("1站取放吸嘴5放气", out str[1]);
                    while (true)
                    {

                        SendCommand("1站下降3检测", out str[7]);
                        SendCommand("1站下降4检测", out str[9]);
                        SendCommand("1站下降5检测", out str[10]);
                        if (str[7] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK")
                        {

                            break;
                        }
                    }

                    SendCommand("1站升降气缸3升起", out str[0]);
                    SendCommand("1站升降气缸4升起", out str[0]);
                    SendCommand("1站升降气缸5升起", out str[0]);
                    while (true)
                    {

                        SendCommand("1站上升3检测", out str[3]);
                        SendCommand("1站上升4检测", out str[4]);
                        SendCommand("1站上升5检测", out str[5]);
                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "30":// 2 3 4 5电机


                    SendCommand("1站升降气缸2升起", out str[0]);
                    SendCommand("1站升降气缸3升起", out str[0]);
                    SendCommand("1站升降气缸4升起", out str[0]);
                    SendCommand("1站升降气缸5升起", out str[0]);
                    while (true)
                    {


                        SendCommand("1站上升2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[9]);
                        // SendCommand("1站电机2左边检测", out str[9]);

                        SendCommand("1站上升3检测", out str[10]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        //  SendCommand("1站电机3左边检测", out str[11]);

                        SendCommand("1站上升4检测", out str[12]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        //  SendCommand("1站电机4左边检测", out str[13]);

                        SendCommand("1站上升5检测", out str[14]);
                        //SendCommand("1站吸嘴5检测", out str[15]);
                        SendCommand("1站电机1右边检测", out str[15]);

                        if (str[8] == "status=OK" && str[10] == "status=OK" && str[12] == "status=OK" && str[14] == "status=OK" && str[15] == "status=OK")
                        {
                            break;
                        }

                    }

                    SendCommand("1站升降气缸2降下", out str[5]);
                    SendCommand("1站升降气缸3降下", out str[6]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站升降气缸5降下", out str[6]);
                    Thread.Sleep(timeUp);

                    SendCommand("1站取放吸嘴2放气", out str[1]);
                    SendCommand("1站取放吸嘴3放气", out str[2]);
                    SendCommand("1站取放吸嘴4放气", out str[3]);
                    SendCommand("1站取放吸嘴5放气", out str[4]);
                    while (true)
                    {

                        SendCommand("1站下降2检测", out str[8]);
                        SendCommand("1站下降3检测", out str[7]);
                        SendCommand("1站下降4检测", out str[9]);
                        SendCommand("1站下降5检测", out str[10]);
                        if (str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK")
                        {

                            break;
                        }
                    }

                    SendCommand("1站升降气缸2升起", out str[0]);
                    SendCommand("1站升降气缸3升起", out str[0]);
                    SendCommand("1站升降气缸4升起", out str[0]);
                    SendCommand("1站升降气缸5升起", out str[0]);
                    Thread.Sleep(100);
                    while (true)
                    {

                        SendCommand("1站上升2检测", out str[2]);
                        SendCommand("1站上升3检测", out str[3]);
                        SendCommand("1站上升4检测", out str[4]);
                        SendCommand("1站上升5检测", out str[5]);
                        if (str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
                case "31"://1 2 3 4 5电机

                    SendCommand("1站升降气缸1升起", out str[0]);
                    SendCommand("1站升降气缸2升起", out str[0]);
                    SendCommand("1站升降气缸3升起", out str[0]);
                    SendCommand("1站升降气缸4升起", out str[0]);
                    SendCommand("1站升降气缸5升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str[2]);
                        //SendCommand("1站吸嘴1检测", out str[3]);
                        SendCommand("1站电机1右边检测", out str[4]);

                        SendCommand("1站上升2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[9]);
                        // SendCommand("1站电机2左边检测", out str[10]);

                        SendCommand("1站上升3检测", out str[5]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        // SendCommand("1站电机3左边检测", out str[7]);

                        SendCommand("1站上升4检测", out str[11]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        // SendCommand("1站电机4左边检测", out str[13]);

                        SendCommand("1站上升5检测", out str[14]);
                        //SendCommand("1站吸嘴5检测", out str[15]);
                        // SendCommand("1站电机5左边检测", out str[16]);

                        if (str[2] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[8] == "status=OK" && str[11] == "status=OK" && str[14] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1降下", out str[5]);
                    SendCommand("1站升降气缸2降下", out str[5]);
                    SendCommand("1站升降气缸3降下", out str[6]);
                    SendCommand("1站升降气缸4降下", out str[6]);
                    SendCommand("1站升降气缸5降下", out str[6]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str[1]);
                    SendCommand("1站取放吸嘴2放气", out str[1]);
                    SendCommand("1站取放吸嘴3放气", out str[1]);
                    SendCommand("1站取放吸嘴4放气", out str[1]);
                    SendCommand("1站取放吸嘴5放气", out str[1]);
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str[6]);
                        SendCommand("1站下降2检测", out str[8]);
                        SendCommand("1站下降3检测", out str[7]);
                        SendCommand("1站下降4检测", out str[9]);
                        SendCommand("1站下降5检测", out str[10]);
                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸1升起", out str[0]);
                    SendCommand("1站升降气缸2升起", out str[0]);
                    SendCommand("1站升降气缸3升起", out str[0]);
                    SendCommand("1站升降气缸4升起", out str[0]);
                    SendCommand("1站升降气缸5升起", out str[0]);
                    Thread.Sleep(100);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str[1]);
                        SendCommand("1站上升2检测", out str[2]);
                        SendCommand("1站上升3检测", out str[3]);
                        SendCommand("1站上升4检测", out str[4]);
                        SendCommand("1站上升5检测", out str[5]);
                        if (str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }

                    break;
            }
            #endregion
            return res;
        }
    }
}
