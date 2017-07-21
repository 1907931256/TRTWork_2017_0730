using CmdFile;
using CommonPortCmd.Net;
using ComPort;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace CommonPortCmd
{
    public class Common
    {

        /// <summary>
        /// 握手返回数据包含
        /// </summary>
        public string str_Port_Rec = "0F 00";

        public static string Model;//设备种类型号制定

        /// <summary>
        /// 接受到的数据
        /// </summary>
        public byte[] resp;

        /// <summary>
        /// 主动上报数据事件
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        public delegate void RecDataSend(object send, ActiveReporting e);
        public event RecDataSend RecDataSendEventHander;


        /// <summary>
        /// 串口通讯类 封装
        /// </summary>
        private SerialPortWrapper serialWrapper;

        /// <summary>
        /// 保存当前站位信息
        /// </summary>
        private string station;//保存当前站位信息

        /// <summary>
        /// 保存主动上报的数据
        /// </summary>
        private static string recActiyData;

        
        /// <summary>
        /// 延迟命令返回锁
        /// </summary>
        private AutoResetEvent delyEvent;

       /// <summary>
       /// 握手事件锁
       /// </summary>
        AutoResetEvent woShouEvent;
        /// <summary>
        /// 避免在事件处理方法中反复的创建，定义到外面
        /// </summary>
        private StringBuilder builder;

        /// <summary>
        /// 临时存放数组
        /// </summary>
        private static Byte[] buffer; 

        /// <summary>
        /// 保存串口的握手指令
        /// </summary>
        private string str_woshou;//保存握手指令Hex


        /// <summary>
        /// 发送网络命令
        /// </summary>
        private ISendCmd sendNetEqumentCmd;


        private INICmds INICmds_;


        public Common()
        {

            string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "cmds.ini";
            INICmds_ = new INICmds(path);

            buffer = new byte[1024];
            XMLConfig config = new XMLConfig();
            serialWrapper = new SerialPortWrapper();
            delyEvent = new AutoResetEvent(true);
            woShouEvent = new AutoResetEvent(false);
            sendNetEqumentCmd = new SendEqumentCmd();
            builder = new StringBuilder();

            serialWrapper.Report += new SerialPortWrapper.ReportEventHandler(ReportHandler);
            station = config.ReadConfing(System.IO.Directory.GetCurrentDirectory(), out Model, out str_woshou);

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
            if (RecDataSendEventHander != null)
            {

                if (DelayCmd.DelaydDat(recActiyData))//recActiyData 主动上报数据
                {
                    Log.Debug("DelayCmd.DelaydDat(recActiyData) recActiyData-->" + recActiyData);
                    delyEvent.Set();
                }

                RecDataSendEventHander(this, new ActiveReporting(recActiyData));
            }
        }


        /// <summary>
        /// 握手函数，调用该函数可以发送握手指令，和串口握手，只有握手成功后 才可以发送其它的串口数据
        /// </summary>
        public bool ConnectPort()
        {
            for (int i = 0; i < 2; i++)
            {
                string[] ports = SerialPort.GetPortNames();

                foreach (string port in ports)
                {
                    try
                    {
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
            }
           
            return false;
        }


  
        /// <summary>
        ///串口数据发送
        /// </summary>
        /// <param name="strParam"></param>
        private void SendHex(string strParam, out string recStr)
        {
            byte[] data = ShujuChuli.HexStringToBytes(strParam);
          
            string recStrHex;
            try
            {
                //Log.Debug("serialWrapper start use" + strParam);
                serialWrapper.SendRecv(data, out resp);
                recStrHex = ShujuChuli.ByteToHexString(resp);
                recStr = StrHexToStrPram.RecDataToHexPram(recStrHex);//返回解析结果
            }
            catch(InvalidOperationException)
            {
                //Log.Debug("InvalidOperationException  already happen");
                
                    Thread.Sleep(100);
                    ConnectPort();
                    recStr = "status=PortClosed";
                
               
            }
            catch (TimeoutException)
            {
                //Log.Debug("TimeoutException  already happen");
                recStr = "status=timeout";
                Thread.Sleep(100);
                try
                {
                    Log.Debug("Send again");
                    serialWrapper.SendRecv(data, out resp);
                    recStrHex = ShujuChuli.ByteToHexString(resp);
                    recStr = StrHexToStrPram.RecDataToHexPram(recStrHex);//返回解析结果

                }
                catch (TimeoutException)
                {
                    //Log.Debug("TimeoutException  already happen");
                    recStr = "status=timeout";
                }
                catch (InvalidDataException)
                {
                    //Log.Debug("InvalidDataException  already happen");
                    recStr = "status=DtatERR";
                }


            }
            catch (InvalidDataException)//数据出错
            {
                //Log.Debug("InvalidDataException already happen");
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
                    //Log.Debug("TimeoutException  already happen");
                    recStr = "status=timeout";
                }
                catch (InvalidDataException)
                {
                    //Log.Debug("InvalidDataException  already happen");
                    recStr = "status=DtatERR";
                }

            }
        }

        public bool SendCommand(string strCmd)
        {
            string recStr = string.Empty;
            string strHex = string.Empty;
            INICmds_.GetEqumentCommand(strCmd, out strHex);
            SendHex(strHex, out recStr);
            Log.Debug(strCmd + "==" + recStr);
            return true;
        }

        public bool SendCommand(string strCmd, out string recStr)
        {

            string strHex = string.Empty;
            INICmds_.GetEqumentCommand(strCmd, out strHex);
            Log.Debug("strHex="+strHex);

            if (strHex != "")
            {

                SendHex(strHex, out recStr);

                Log.Debug(strCmd + "==" + recStr);

                return true;
            }
            else
            {
                if (Model == "MMI")
                {
                    MMI_Package(strCmd, out recStr);
                }
                else if (Model == "CAM")
                {
                    Cam_Package(strCmd, out recStr);
                }
                else
                {
                    recStr = "NOT";
                }

                return true;
            }
           

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

            string strHex = string.Empty;
            INICmds_.GetEqumentCommand(strCmd, out strHex);
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
                    return true;
                }

            }
            else
            {
                if (Model == "MMI")
                {
                    Log.Debug("MMI_Package start");
                    MMI_Package(strCmd, param, out recStr);
                }
                else if (Model == "CAM")
                {
                    Log.Debug("Cam_Package start");
                    Cam_Package(strCmd, param, out recStr);
                }
                else
                {
                    recStr = "NOT";
                }
            }
            return true;
        }

        #region    /**********************************  龙门结构运动控制   *****************************************/

        /// <summary>
        /// 龙门结构MMI指令分解
        /// </summary>
        /// <param name="strCmd"></param>
        /// <param name="recStr"></param>
        /// <returns></returns>
        private bool MMI_Package(string strCmd, out string recStr)
        {
            string strHex = string.Empty;

            if (strCmd == "到位检测")
            {
                strHex = "72 05 " + station + " 01 01 FF 81";

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
            else if (strCmd == "复位")
            {
                strHex = "72 04 " + station + " 5A FF 81";

                SendHex(strHex, out recStr);
                Log.Debug(strCmd + "==" + recStr);
                return true;
            }
            else if (strCmd.IndexOf("测试完成状态") != -1)
            {
                Room_RecTestOK(strCmd, out recStr);
                return true;
            }
            else if (strCmd.IndexOf("2站手机固定") != -1)
            {
                while (true)
                {
                    recStr = sendNetEqumentCmd.SendCmd(1, "1站电机1原点检测", "");
                    if (recStr == "status=OK")
                    {
                        break;

                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                strHex = "72 05 12 02 01 00 81";
                SendHex(strHex, out recStr);

                return true;
            }
            else if (strCmd.IndexOf("3站45度顶起") != -1)
            {
                while (true)
                {
                    recStr = sendNetEqumentCmd.SendCmd(1, "1站电机1原点检测", "");
                    if (recStr == "status=OK")
                    {
                        break;
                       
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                //while (true)
                //{
                //    SendCommand("3站前白卡上升检测",out recStr);
                //    if (recStr == "status=OK")
                //    {
                //        break;

                //    }
                //    else
                //    {
                //        Thread.Sleep(100);
                //    }
                //}

                strHex = "72 05 13 02 02 00 81";
                SendHex(strHex, out recStr);

                return true;
            }
            else if (strCmd.IndexOf("3站前白卡下降") != -1)
            {
                while (true)
                {
                    recStr = sendNetEqumentCmd.SendCmd(1, "1站电机1原点检测", "");
                    if (recStr == "status=OK")
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
                    SendCommand("3站45度放平检测", out recStr);
                    if (recStr == "status=OK")
                    {
                        break;

                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                strHex = "72 05 13 02 05 00 81";
                SendHex(strHex, out recStr);

                return true;
            }
            else if (strCmd.IndexOf("4站天板靠近") != -1)
            {
                while (true)
                {
                    recStr = sendNetEqumentCmd.SendCmd(1, "1站电机1原点检测", "");
                    if (recStr == "status=OK")
                    {
                        break;

                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                strHex = "72 05 14 02 04 00 81";
                SendHex(strHex, out recStr);
                return true;
            }
            else if (strCmd.IndexOf("5站隔音箱下降") != -1)
            {
                while (true)
                {
                    recStr = sendNetEqumentCmd.SendCmd(1, "1站电机1原点检测", "");
                    if (recStr == "status=OK")
                    {
                        break;

                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }

                strHex = "72 05 15 02 03 FF 81";
                SendHex(strHex, out recStr);
                return true;
            }
            else
            {
                recStr = "status=NOT";
                return true;
            }

        }

        private bool MMI_Package(string strCmd, string param, out string recStr)
        {
            string strHex = string.Empty;

            if (strCmd == "1站取放")
            {
                delyEvent.WaitOne();//等待延迟数据返回
                Room_MotorCheckBeforeDo();
                //Room_LeftDo(param);
                //Room_RightDo(param);
                //Room_MidDo(param);
                if (param!="0")
                {
                    strHex="72 05 11 04 0C " + ShujuChuli.StrToHex(param) + " 81";
                    SendHex(strHex, out recStr);//发送命令
                    recStr = "status=OK";
                }
                else
                {
                    recStr = "status=error";
                }
               
                return true;
            }
            else if (strCmd == "警告")
            {
                if (station != "11")
                {
                    strHex = "72 05 " + station + " 05 01 00 81";
                }
                else
                {
                    strHex = "72 05 11 02 1b " + ShujuChuli.StrToHex(param) + " 81";
                }

                SendHex(strHex, out recStr);//发送命令
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
                    strHex = "72 05 11 02 18 " + ShujuChuli.StrToHex(param) + " 81";
                }

                SendHex(strHex, out recStr);//发送命令
                Log.Debug(strCmd + "==" + recStr);

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
                    strHex = "72 05 11 02 18 00 81";
                }

                SendHex(strHex, out recStr);//发送命令
                Log.Debug(strCmd + "==" + recStr);

                return true;
            }

            else
            {
                recStr = "status=NOT";
                return true;
            }


        }


        private void Room_RecTestOK(string strCmd, out string recStr)
        {
            switch (strCmd)
            {
                case "1站测试完成状态":
                    //SendCommand("1站手机固定", out recStr);
                    break;
                case "2站测试完成状态":
                    //SendCmmond("1站手机固定", out recStr);
                    break;
                case "3站测试完成状态":
                    string[] rec_3 = new string[5];
                    while (true)
                    {
                        SendCommand("3站45度放平检测", out rec_3[0]);
                        //SendCommand("3站前白卡远离检测", out rec_3[1]);
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
                    //string[] rec_6 = new string[5];
                    //while (true)
                    //{
                    //    //SendCommand("6站电机X位置检测", out rec[0]);
                    //    //SendCommand("6站电机Y位置检测", out rec[1]);
                    //    //SendCommand("3站前白卡上升检测", out rec[2]);

                    //    if (rec[0] == "status=OK" && rec[1] == "status=OK"/* && rec[2] == "status=OK"*/)
                    //    {
                    //        break;
                    //    }
                    //    Thread.Sleep(50);
                    //}

                    break;
            }
            recStr = "status=OK";
        }


        /// <summary>
        /// 电机运动前必须具备的状态
        /// 跨站位访问
        /// </summary>
        private void Room_MotorCheckBeforeDo()
        {
            
            /* 3站45度放平检测
             * 3站前白卡上升检测   富士康设备注销该命令
             * 4站天板远离检测
             * 5站隔离上升检测
             * 5站人工耳远离检测
             * 
             * 
             * */
            //检测不到位从新发送时间间隔
            int timeNO = 100;
            string result = string.Empty;

            while (true)
            {
               result= sendNetEqumentCmd.SendCmd(3, "3站45度放平检测","" );
               if (result=="status=OK")
               {
                   break;
               }
               else
               {
                   Thread.Sleep(timeNO);
               }
            }
            while (true)
            {
                result = sendNetEqumentCmd.SendCmd(3,"3站前白卡上升检测", "");
                if (result == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }

            while (true)
            {
                result = sendNetEqumentCmd.SendCmd(4,  "4站天板远离检测","");
                if (result == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }


            while (true)
            {
                result = sendNetEqumentCmd.SendCmd(5, "5站隔离上升检测","" );
                if (result == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                result = sendNetEqumentCmd.SendCmd(5, "5站人工耳远离检测", "");
                if (result == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }


        }
        /// <summary>
        /// 左运动
        /// </summary>
        /// <param name="pram"></param>
        private void Room_LeftDo(string pram)
        {
                string str = "OK";
                Room_OrginOK(pram);
                Room_MotorCheckBeforeDo();
                SendCommand("1站56站靠近", out str);
                Room_MotorMotionCanDo();
                SendCommand("1站电机左运动", pram, out str);
                Thread.Sleep(600);
                Room_LeftOK(pram);
                Room_LiftDownUpAbsorb(pram);
        }

        /// <summary>
        /// 右运动
        /// </summary>
        /// <param name="pram"></param>
        private void Room_RightDo(string pram)
        {
            string str;
            Room_LeftOK(pram);
            SendCommand("1站45站靠近", out str);
            SendCommand("1站56站远离", out str);
            Room_MotorMotionCanDo();
            SendCommand("1站电机右运动", pram, out str);
            Thread.Sleep(600);
            Room_RightOK(pram);
            Room_RightDownAbsorb(pram);

        }
        /// <summary>
        /// 中运动
        /// </summary>
        /// <param name="pram"></param>
        private void Room_MidDo(string pram)
        {
                string str;
                Room_RightOK(pram);
                SendCommand("1站45站远离", out str);
                SendCommand("1站56站远离", out str);
                Room_MotorMotionCanDo();
                SendCommand("1站电机中运动", pram, out str);
                Thread.Sleep(600);
                Room_OrginOK(pram);

        }

        /// <summary>
        /// 原点检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Room_OrginOK(string pram)
        {
            string str = "";

            pram = pram.ToUpper();
            while (true)
            {
                SendCommand("1站56站远离检测", out str);
                if (str == "status=OK")
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
                SendCommand("1站45站远离检测", out str);
                if (str == "status=OK")
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
                SendCommand("1站电机1原点检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
	            {
                    Thread.Sleep(100);
	            }
            }
            return str;
        }

        /// <summary>
        /// 左检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Room_LeftOK(string pram)
        {

            string str;           

            while (true)
            {
                SendCommand("1站电机1左边检测", out str);
                if (str == "status=OK")
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
                SendCommand("1站45站远离检测", out str);
                if (str == "status=OK")
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
                SendCommand("1站56站靠近检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            return str;
        }

        /// <summary>
        /// 右检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Room_RightOK(string pram)
        {

            string res = "";
            string str;
            int timeNO = 100;

            while (true)
            {
                SendCommand("1站电机1右边检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
               
            }
            while (true)
            {
                SendCommand("1站45站靠近检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                Thread.Sleep(timeNO);
            }
            while (true)
            {
                SendCommand("1站56站远离检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                Thread.Sleep(timeNO);
            }
            return res;
        }

        /// <summary>
        /// 左边吸取动作
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Room_LiftDownUpAbsorb(string pram)
        {
            int timeAbsorb = 300;//吸取时间

            int timeUp = 100;
            int timeNO = 100;
            string res = "";
            string str;
            pram = pram.ToUpper();
            switch (pram)
            {
                case "1"://1电机

                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站取放吸嘴1吸取", out str);
             
                    while (true)
                    {

                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }


                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    break;
                case "2"://2电机

                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站取放吸嘴2吸取", out str);
              
                    while (true)
                    {

                        SendCommand("1站下降2检测", out str);
                        // SendCommand("1站吸嘴2检测", out str[9]);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸2升起", out str);

                    break;
                case "3"://1 2电机
                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站取放吸嘴1吸取", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站取放吸嘴2吸取", out str);
                  
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                        
                    }
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str== "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);

                    break;

                case "4"://3电机
                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站取放吸嘴3吸取", out str);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                       
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸3升起", out str);

                    break;

                case "7"://1 2 3电机
                    SendCommand("1站取放吸嘴2吸取", out str);
                    SendCommand("1站取放吸嘴1吸取", out str);
                    SendCommand("1站取放吸嘴3吸取", out str);

                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸3降下", out str);

              

                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);

                    break;
                case "8"://4电机
                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站取放吸嘴4吸取", out str);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        
                        SendCommand("1站下降4检测", out str);
                      
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }


                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸4升起", out str);

                    break;
                case "15"://1 2 3 4电机
                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站取放吸嘴3吸取", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站取放吸嘴4吸取", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站取放吸嘴2吸取", out str);
                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站取放吸嘴1吸取", out str);
             

                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                      
                    }
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    break;
                case "16"://5电机
                    SendCommand("1站升降气缸5降下", out str);
                    SendCommand("1站取放吸嘴5吸取", out str);
                 
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸5升起", out str);
                    break;
                case "24"://4 5电机

                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站取放吸嘴4吸取", out str);

                    SendCommand("1站升降气缸5降下", out str);
                    SendCommand("1站取放吸嘴5吸取", out str);
                
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }



                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

                    break;
                case "28"://3 4 5电机

                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站取放吸嘴3吸取", out str);

                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站取放吸嘴4吸取", out str);

                    SendCommand("1站升降气缸5降下", out str);
                    SendCommand("1站取放吸嘴5吸取", out str);
                


                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }


                    Thread.Sleep(timeAbsorb);

                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

                    break;
                case "30":// 2 3 4 5电机


                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站取放吸嘴2吸取", out str);

                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站取放吸嘴3吸取", out str);

                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站取放吸嘴4吸取", out str);

                    SendCommand("1站升降气缸5降下", out str);
                    SendCommand("1站取放吸嘴5吸取", out str);
                  
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }

                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

                    break;
                case "31"://1 2 3 4 5电机

                    
                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站升降气缸5降下", out str);
                    
                    SendCommand("1站取放吸嘴1吸取", out str);
                    SendCommand("1站取放吸嘴2吸取", out str);
                    SendCommand("1站取放吸嘴3吸取", out str);
                    SendCommand("1站取放吸嘴4吸取", out str);
                    SendCommand("1站取放吸嘴5吸取", out str);

                    
                     while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str== "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                        
                    }
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

                    break;
            }
            return res;
        }

        /// <summary>
        /// 右边吸取动作
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Room_RightDownAbsorb(string pram)
        {
            int timeNOAbsorb = 300;//吸取时间
            int timeUp = 80;//间隔多久放气
            int timeNO = 100;
            string res = "";
            string str;
            
            pram = pram.ToUpper();
            switch (pram)
            {
                case "1"://1电机

                    SendCommand("1站升降气缸1升起", out str);

                    while (true)
                    {
                        SendCommand("1站上升1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }



                    SendCommand("1站升降气缸1降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str);
                    while (true)
                    {

                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                        
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸1升起", out str);

                    break;

                case "2"://2电机

                    SendCommand("1站升降气缸2升起", out str);

                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str== "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    SendCommand("1站升降气缸2降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴2放气", out str);
                    while (true)
                    {

                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸2升起", out str);

                    break;
                case "3"://1 2电机

                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }




                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str);
                    SendCommand("1站取放吸嘴2放气", out str);
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);

                    break;
                case "4"://3电机
                    SendCommand("1站升降气缸3升起", out str);
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    SendCommand("1站升降气缸3降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴3放气", out str);
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸3升起", out str);

                    break;
                case "7"://1 2 3电机
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    

                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸3降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str);
                    SendCommand("1站取放吸嘴2放气", out str);
                    SendCommand("1站取放吸嘴3放气", out str);
                     while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);



                    break;
                case "8"://4电机

                    SendCommand("1站升降气缸4升起", out str);
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    SendCommand("1站升降气缸4降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴4放气", out str);

                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    SendCommand("1站升降气缸4升起", out str);

                    break;
         
                case "9"://1 4电机
                  SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                   
                  
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                
                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    Thread.Sleep(timeUp);
              
                    SendCommand("1站取放吸嘴1放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                   
                   
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                 
                 
                    break;
                case "10"://2 4电机
               
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                   
                  
                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    Thread.Sleep(timeUp);
              
                    SendCommand("1站取放吸嘴2放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                   
                   
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                 
                    break;
                case "11"://1 2 4电机
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                   
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str);
                        if (str== "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str);
                    SendCommand("1站取放吸嘴2放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                   
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                 
                    break;
                case "12"://3 4电机
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                   
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str== "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴3放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                   
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                 
              
                    break;

                case "13"://1 3 4电机

                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站上升1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str== "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str);
                    SendCommand("1站取放吸嘴3放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                 

                    break;
                case "14"://2 3 4电机
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str== "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴2放气", out str);
                    SendCommand("1站取放吸嘴3放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                 

                    break;
            
                case "15"://1 2 3 4电机
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站上升1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str== "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }



                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str);
                    SendCommand("1站取放吸嘴2放气", out str);
                    SendCommand("1站取放吸嘴3放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);

                    break;
                case "16"://5电机
                    SendCommand("1站升降气缸1升起", out str);

                    while (true)
                    {
                        SendCommand("1站上升5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    SendCommand("1站升降气缸5降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴5放气", out str);
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸5升起", out str);
                    break;
                case "24"://4 5电机

                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                   
                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站升降气缸5降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴4放气", out str);
                    SendCommand("1站取放吸嘴5放气", out str);

                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

                    break;
                case "28"://3 4 5电机

                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }


                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站升降气缸5降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴3放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                    SendCommand("1站取放吸嘴5放气", out str);

                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);
                   

                    break;
                case "30":// 2 3 4 5电机


                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);


                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }


                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站上升5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站升降气缸5降下", out str);
                    Thread.Sleep(timeUp);

                    SendCommand("1站取放吸嘴2放气", out str);
                    SendCommand("1站取放吸嘴3放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                    SendCommand("1站取放吸嘴5放气", out str);

                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    while (true)
                    {
                        SendCommand("1站上升5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

                    break;
                case "31"://1 2 3 4 5电机

                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

                    while (true)
                    {
                        SendCommand("1站上升1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }  
                    while (true)
                    {
                        SendCommand("1站上升4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站上升5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }

                    SendCommand("1站升降气缸1降下", out str);
                    SendCommand("1站升降气缸2降下", out str);
                    SendCommand("1站升降气缸3降下", out str);
                    SendCommand("1站升降气缸4降下", out str);
                    SendCommand("1站升降气缸5降下", out str);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str);
                    SendCommand("1站取放吸嘴2放气", out str);
                    SendCommand("1站取放吸嘴3放气", out str);
                    SendCommand("1站取放吸嘴4放气", out str);
                    SendCommand("1站取放吸嘴5放气", out str);
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降2检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降3检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    while (true)
                    {
                        SendCommand("1站下降5检测", out str);
                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }
                    }
                    Thread.Sleep(timeNOAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);
                    break;
            }
            return res;
        }

        /// <summary>
        /// 检测电机是否可以运动
        /// </summary>
        /// <returns></returns>
        private string Room_MotorMotionCanDo()
        {
            int timeNO = 100;
            string str = string.Empty;
            while (true)
            {
                SendCommand("1站上升1检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站上升2检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站上升3检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站上升4检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站上升5检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            return str;
        }

        #endregion    /**********************************  龙门结构运动控制   *****************************************/

        #region /*************  Cam  *********************************************************************/

        /// <summary>
        /// Cam 组合命令控制
        /// </summary>
        /// <param name="strCmd"></param>
        /// <param name="recStr"></param>
        /// <returns></returns>
        private bool Cam_Package(string strCmd, out string recStr)
        {
            string strHex = string.Empty;

            if (strCmd == "到位检测")
            {
                strHex = "72 05 " + station + " 01 01 FF 81";

                SendHex(strHex, out recStr);
                Log.Debug(strCmd + "==" + recStr);
                //recStr = recDataStr;

                return true;
            }
            else if (strCmd == "复位")
            {
                strHex = "72 04 " + station + " 5A FF 81";

                SendHex(strHex, out recStr);
                Log.Debug(strCmd + "==" + recStr);
                return true;
            }
            //else if (strCmd.IndexOf("测试完成状态") != -1)
            //{
            //    RecTestOK(strCmd, out recStr);
            //    return true;
            //}
            else if (strCmd.IndexOf("3站取放开始") != -1)
            {
                CamStartStation_3(strCmd, out recStr);
                return true;
            }
            else if (strCmd.IndexOf("4站取放开始") != -1)
            {
                recStr = "status=OK";
                CamStartStation_4(strCmd, out recStr);
                return true;
            }
            else
            {
                recStr = "status=NOT";
                return true;
            }

        }

        /// <summary>
        /// 4站电机取放
        /// </summary>
        /// <param name="strCmd"></param>
        /// <param name="recStr"></param>
        private void CamStartStation_4(string strCmd, out string recStr)
        {
            string fangStation = "19400";
            string yuanStation = "19400";
            string s1;
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

        /// <summary>
        /// 3站电机取放动作
        /// </summary>
        /// <param name="strCmd"></param>
        /// <param name="recStr"></param>
        private void CamStartStation_3(string strCmd, out string recStr)
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

        private bool Cam_Package(string strCmd, string param, out string recStr)
        {
            string strHex = string.Empty;

            if (strCmd == "1站取放")
            {
                Cam_LeftDo(param);
                Cam_RightDo(param);
                Cam_MidDo(param);
                recStr = "status=OK";
                return true;
            }
            else
            {
                recStr = "status=NOT";
                return true;
            }


        }

        /// <summary>
        /// 左运动
        /// </summary>
        /// <param name="pram"></param>
        private void Cam_LeftDo(string pram)
        {
            string str = "OK";
            str = Cam_OrginOK(pram);
            if (str == "status=OK")//电机在原点，且上到位
            {

                SendCommand("1站电机左运动", out str);

               
                Cam_LeftOK(pram);
                
                CamLiftDownUpAbsorb(pram);
            }
        }

        /// <summary>
        /// 右运动
        /// </summary>
        /// <param name="pram"></param>
        private void Cam_RightDo(string pram)
        {
            string str = "OK";
            Cam_LeftOK(pram);
            SendCommand("1站电机右运动", out str);
            while (true)
            {
                str = Cam_RightOK(pram);
                if (str == "status=OK")
                {
                    break;
                }
            }
            Cam_RightDownAbsorb(pram);

        }
        /// <summary>
        /// 中运动
        /// </summary>
        /// <param name="pram"></param>
        private void Cam_MidDo(string pram)
        {
            string str = "OK";
            str = Cam_RightOK(pram);
            if (str == "status=OK")//
            {
                SendCommand("1站电机中运动", out str);
                while (true)
                {
                    string s = Cam_OrginOK(pram);
                    if (s == "status=OK")
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                    

                }

            }
        }

        /// <summary>
        /// 原点检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Cam_OrginOK(string pram)
        {
            string str = "";
            int timeNO = 100;
            pram = pram.ToUpper();

            while (true)
            {
                SendCommand("1站取放原点检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站取放1抬起检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站取放2抬起检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            return str;
        }

        /// <summary>
        /// 左检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Cam_LeftOK(string pram)
        {

            string str = "";
            int timeNO = 100;
            pram = pram.ToUpper();
            while (true)
            {
                SendCommand("1站前到位检测", out str);
                if (str== "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站取放1抬起检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站取放2抬起检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }

            return str;
        }

        /// <summary>
        /// 右检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Cam_RightOK(string pram)
        {

            string str = "";
            int timeNO = 100;
            pram = pram.ToUpper();
            while (true)
            {
                SendCommand("1站后到位检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站取放1抬起检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }
            while (true)
            {
                SendCommand("1站取放2抬起检测", out str);
                if (str == "status=OK")
                {
                    break;
                }
                else
                {
                    Thread.Sleep(timeNO);
                }
            }


            return str;
        }

        /// <summary>
        /// 左边吸取动作
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string CamLiftDownUpAbsorb(string pram)
        {
            int timeAbsorb = 200;//吸取时间
            int timeNO = 100;
            int timeUP = 200;
            string str="" ;
          
            pram = pram.ToUpper();

            switch (pram)
            {
                case "1"://1电机
                    
                        SendCommand("1站取放1下压", out str);
                        SendCommand("1站取放1吸取", out str);
                        Thread.Sleep(timeAbsorb);

                        while (true)
                        {

                            SendCommand("1站取放1下压检测", out str);

                            if (str == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeNO);
                            }
                        }
                        SendCommand("1站取放1抬起", out str);
                        Thread.Sleep(timeUP);
                        while (true)
                        {
                            SendCommand("1站取放1抬起检测", out str);

                            if (str == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeNO);
                            }

                        }
                    break;
                case "2"://2电机

                        SendCommand("1站取放2下压", out str);
                        SendCommand("1站取放2吸取", out str);

                        Thread.Sleep(timeAbsorb);
                        while (true)
                        {

                            SendCommand("1站取放2下压检测", out str);

                            if (str == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeNO);
                            }
                        }
                        SendCommand("1站取放2抬起", out str);

                        Thread.Sleep(timeUP);
                        while (true)
                        {
                            SendCommand("1站取放2抬起检测", out str);

                            if (str == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeNO);
                            }
                        }

                    break;
                case "3"://1 2电机

                        SendCommand("1站取放1下压", out str);
                        SendCommand("1站取放1吸取", out str);
                        SendCommand("1站取放2下压", out str);
                        SendCommand("1站取放2吸取", out str);
                        Thread.Sleep(timeAbsorb);

                        while (true)
                        {

                            SendCommand("1站取放1下压检测", out str);

                            if (str == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeNO);
                            }
                        }
                        while (true)
                        {

                            SendCommand("1站取放2下压检测", out str);

                            if (str == "status=OK")
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(timeNO);
                            }
                        }


                        SendCommand("1站取放1抬起", out str);
                        SendCommand("1站取放2抬起", out str);
                        Thread.Sleep(timeUP);
                    while (true)
                    {
                        SendCommand("1站取放1抬起检测", out str);

                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {
                        SendCommand("1站取放2抬起检测", out str);

                        if (str == "status=OK")
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    break;
            }

            return str;
        }

        /// <summary>
        /// 右边吸取动作
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string Cam_RightDownAbsorb(string pram)
        {
            int timeDownAbsorb = 100;//手机多久抬起
            int timeNO = 100;
            int timeDown = 200;
            string str = "";

            pram = pram.ToUpper();

            switch (pram)
            {
                case "1"://1电机
                    SendCommand("1站取放1下压", out str);

                    Thread.Sleep(timeDown);
                    SendCommand("1站取放1松开", out str);
                    while (true)
                    {

                        SendCommand("1站取放1下压检测", out str);
                        if (str == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeDownAbsorb);
                    SendCommand("1站取放1抬起", out str);

                    break;

                case "2"://2电机

                    SendCommand("1站取放2下压", out str);
                    Thread.Sleep(timeDown);
                    SendCommand("1站取放2松开", out str);
                    while (true)
                    {

                        SendCommand("1站取放2下压检测", out str);
                        if (str == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeDown);

                    SendCommand("1站取放2抬起", out str);
                    break;
                case "3"://1 2电机


                    SendCommand("1站取放1下压", out str);
                    SendCommand("1站取放2下压", out str);
                    Thread.Sleep(timeDown);
                    SendCommand("1站取放1松开", out str);
                    SendCommand("1站取放2松开", out str);
                    while (true)
                    {

                        SendCommand("1站取放1下压检测", out str);
                        if (str == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    while (true)
                    {

                        SendCommand("1站取放2下压检测", out str);
                        if (str == "status=OK")
                        {

                            break;
                        }
                        else
                        {
                            Thread.Sleep(timeNO);
                        }

                    }
                    Thread.Sleep(timeDown);
                    SendCommand("1站取放1抬起", out str);
                    SendCommand("1站取放2抬起", out str);
                    break;
            }

            return str;

        }

        private void Cam_StartStation_4(string strCmd, out string recStr)
        {
            string fangStation = "19400";
            string yuanStation = "19400";
            string s1;
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
            recStr = "status=OK";
        }

        private void Cam_StartStation_3(string strCmd, out string recStr)
        {
            string quStation = "10900";
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

        #endregion /*************  Cam  *********************************************************************/


    }
}
