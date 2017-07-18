using CmdNamDAL;
using CmdNameModel;
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

namespace CommonPortCmd
{
    /// <summary>
    /// 串口操作公共类
    /// </summary>
    public class Common
    {

        /// <summary>
        /// 握手返回数据包含
        /// </summary>
        public string str_Port_Rec = "0F 00";
   
        /// <summary>
        /// 设备种类型号制定
        /// </summary>
        private static string Model;

        /// <summary>
        /// 保存串口返回的数据
        /// </summary>
        private static string recData;//保存串口返回的数据

        /// <summary>
        /// 保存串口返回的数据
        /// </summary>
        public static string RecData
        {
          get { return Common.recData; }
        }

        //定义一个委托，用于串口数据主动上报
        public delegate void RecDataSend(object send, ActiveReporting e);
        public event RecDataSend RecDataSendEventHander;

       
        /// <summary>
        /// 串口通讯类的具体对象
        /// </summary>
        private SerialPortWrapper serialWrapper;

       
        /// <summary>
        /// 保存当前站位信息
        /// </summary>
        public static string station = "";

        /// <summary>
        /// SerialPort 类的对象
        /// </summary>
        private static SerialPort comm_SerialPort;

        /// <summary>
        /// 延迟命令 锁
        /// </summary>
        private AutoResetEvent delyEvent;

        /// <summary>
        /// 保存命令对应的中文名字
        /// </summary>
        private string[] cmdNames;

        /// <summary>
        /// 保存握手指令
        /// </summary>
        private string str_woshou = "";

        //static MMI_Relationship relationship = new MMI_Relationship();//依赖项目处理

        private CmdDAl cmdDal;

        
        private CmdInfo[] cmds;

        public Common()
        {

            comm_SerialPort = new SerialPort();
            XMLConfig config = new XMLConfig();
            station = config.ReadConfing(System.IO.Directory.GetCurrentDirectory(), out Model, out str_woshou);
            serialWrapper = new SerialPortWrapper();
            serialWrapper.Report += new SerialPortWrapper.ReportEventHandler(ReportHandler);
            delyEvent = new AutoResetEvent(true);
            cmdDal=new CmdDAl();
            cmds= cmdDal.CmdByDataTable(Model);
            cmdNames = new string[cmds.Length];
            for (int i = 0; i < cmds.Length; i++)
            {
                cmdNames[i] = cmds[i].CmdName;
            }

        }

        /// <summary>
        /// 主动上报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReportHandler(object sender, ReportEventArgs e)
        {
            byte[] data = e.Data;
            recData=ShujuChuli.ByteToHexString(data);
            Log.Debug("ShujuChuli.ByteToHexString(data) recData=" + recData);

            if (RecDataSendEventHander != null)
            {

                if (DelayCmd.DelaydDat(recData))
                {
                    Log.Debug("DelayCmd.DelaydDat(recData) recData-->" + recData);

                    delyEvent.Set();
                }

                RecDataSendEventHander(this, new ActiveReporting(recData));
            }
        }

        /// <summary>
        /// 主动上报部分
        /// </summary>
       
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
            string name = string.Empty;
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                try
                {
                    serialWrapper.Open(port);
                    byte[] sbuf = ShujuChuli.HexStringToBytes(str_woshou);
                    byte[] resp;
                    serialWrapper.SendRecv(sbuf, out resp);

                   recData = ShujuChuli.ByteToHexString(resp);

                    if (recData.IndexOf("34 04 " + station + " 0F 00") != -1)
                    {
                    
                        return true;
                    }
                }
                catch (Exception)
                {
                    serialWrapper.Close();
                    name = string.Empty;

                }
                
            }
            
            return false;
        }
        /// <summary>
        ///串口数据发送
        /// </summary>
        /// <param name="strParam"></param>
        public void SendHex(string strParam, out string recStr)
        {
            byte[] data = ShujuChuli.HexStringToBytes(strParam);
            byte[] resp;
            try
            {
                Log.Debug("serialWrapper start use" + strParam);
                serialWrapper.SendRecv(data, out resp);
                recData = ShujuChuli.ByteToHexString(resp);
                recStr = StrHexToStrPram.RecDataToHexPram(recData);//返回解析结果
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
                    recData = ShujuChuli.ByteToHexString(resp);
                    recStr = StrHexToStrPram.RecDataToHexPram(recData);//返回解析结果

                }
                catch (TimeoutException)
                {
                    Log.Debug("TimeoutException  already happen");
                    recStr = "status=timeout";
                }
                catch (InvalidDataException)
                {
                    Log.Debug("InvalidDataException  already happen");
                    recStr = "status=DtatERR";
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
                    recData = ShujuChuli.ByteToHexString(resp);
                    recStr = StrHexToStrPram.RecDataToHexPram(recData);//返回解析结果

                }
                catch (TimeoutException)
                {
                    Log.Debug("TimeoutException  already happen");
                    recStr = "status=timeout";
                }
                catch (InvalidDataException)
                {
                    Log.Debug("InvalidDataException  already happen");
                    recStr = "status=DtatERR";
                }

            }
        }


        public bool SendCommand(string strCmd)
        {
            string recStr = string.Empty;
            int i;
            string cmdString = string.Empty;
            if ((i = Array.IndexOf(cmdNames, strCmd)) != -1)
            {
                cmdString = cmds[i].Start + " " + cmds[i].Length + " " + cmds[i].Adress + " " + cmds[i].Model + " " + cmds[i].Port + " " + cmds[i].StrPram + " " + cmds[i].End;

                SendHex(cmdString, out recStr);
                Log.Debug(strCmd + "==" + recStr);
            }
            else
            {
                cmdString = "NOT";
            }
           
            return true;
        }


        public bool SendCommand(string strCmd, out string recStr)
        {
            int i;
            string cmdString = string.Empty;
            if ((i = Array.IndexOf(cmdNames, strCmd)) != -1)
            {
                cmdString = cmds[i].Start + " " + cmds[i].Length + " " + cmds[i].Adress + " " + cmds[i].Model + " " + cmds[i].Port + " " + cmds[i].StrPram + " " + cmds[i].End;
                SendHex(cmdString, out recStr);

                Log.Debug(strCmd + "==" + recStr);

            }
            else
            {
                if (Model=="MMI")
                {
                    MMI_Package(strCmd, out recStr);
                }
                else if (Model=="CAM")
                {
                   CAM_Package(strCmd, out recStr);
                }
                else
                {
                    recStr = "NOT";
                }
               

            }

            return true;
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="strParam">接受到的命令</param>
        ///// <param name="param">接受到的，参数</param>
        ///// <returns>返回命令执行的结果数据</returns>
        public bool SendCommand(string strCmd, string param, out string recStr)
        {
            int i;
            string cmdString = string.Empty;
            if ((i = Array.IndexOf(cmdNames, strCmd)) != -1)
            {
                cmdString = cmds[i].Start + " " + cmds[i].Length + " " + cmds[i].Adress + " " + cmds[i].Model + " " + cmds[i].Port + " " + ShujuChuli.StrToHex(param) + " " + cmds[i].End;


                if (DelayCmd.WhetherCmd(strCmd))//命令是一条延迟命令
                {
                    delyEvent.WaitOne();//等待延迟数据返回

                    Log.Debug("Cmd is delay cmd -->" + strCmd + param);

                    SendHex(cmdString, out recStr);//发送命令
                   
                }
                else
                {

                    SendHex(cmdString, out recStr);//发送命令
                    Log.Debug(strCmd + "==" + recStr);
                    return true;
                }
            }
            else
            {
                if (Model == "MMI")
                {
                    MMI_Package(strCmd, param, out recStr); 
                }
                else if (Model == "CAM")
                {
                    CAM_Package(strCmd, param, out recStr);
                }
                else
                {
                    recStr = "NOT";
                }
                
            }
            return true;
        }


/*****************************************************************************************************************/
/*****************************************************************************************************************/

        #region   Room MMI处理类
   
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
            else if (strCmd == "复位")
            {
                strHex = "72 04 " + station + " 0F 00 81";

                SendHex(strHex, out recStr);
                Log.Debug(strCmd + "==" + recStr);
                return true;
            }
            //else if (strCmd == "报警")
            //{
            //    if (station != "11")
            //    {
            //        strHex = "72 05 " + station + " 05 01 00 81";
            //    }
            //    else
            //    {
            //        strHex = "72 05 11 05 18 00 81";
            //    }


            //    SendHex(strHex, out recStr);
            //    Log.Debug(strCmd + "==" + recStr);
            //    return true;
            //}
            else
            {
                recStr = "status=NOT";
                return true;
            }

        }
        /// <summary>
        /// MMI组合命令处理类
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        private bool MMI_Package(string strCmd, string param, out string recStr)
        {
            string strHex = string.Empty;
            if (strCmd == "1站取放")
            {
                MMI_LeftDo(param);
                MMI_RightDo(param);
                MMI_MidDo(param);
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
        private void MMI_LeftDo(string pram)
        {
            string str = string.Empty;
                MMI_OrginOK(pram);
                SendCommand("1站电机左运动",pram, out str);
                Thread.Sleep(500);
                MMI_LeftOK(pram);
                MMI_LiftDownUpAbsorb(pram);
        }

        /// <summary>
        /// 右运动
        /// </summary>
        /// <param name="pram"></param>
        private void MMI_RightDo(string pram)
        {
            string str = string.Empty;
            MMI_LeftOK(pram);
            SendCommand("1站电机右运动",pram, out str);
            Thread.Sleep(500);
            MMI_RightOK(pram);
            MMI_RightDownAbsorb(pram);

        }
        /// <summary>
        /// 中运动
        /// </summary>
        /// <param name="pram"></param>
        private void MMI_MidDo(string pram)
        {
            string str = string.Empty;
            MMI_RightOK(pram);
            SendCommand("1站电机中运动",pram, out str);
            Thread.Sleep(500);
            MMI_OrginOK(pram);

            
        }

        /// <summary>
        /// 原点检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string MMI_OrginOK(string pram)
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
        private string MMI_LeftOK(string pram)
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
        private string MMI_RightOK(string pram)
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
        private string MMI_LiftDownUpAbsorb(string pram)
        {
            #region
            int timeAbsorb = 500;//吸取时间
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
                    Thread.Sleep(timeAbsorb);
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);

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
        private string MMI_RightDownAbsorb(string pram)
        {
            #region
            // int timeAbsorb = 500;//吸取时间

            int timeUp = 150;//间隔多久放气
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
                #region
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

                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸4升起", out str);

                    break;
                case "C"://3 4电机
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


                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);


                    break;

                case "D"://1 3 4电机

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

                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);


                    break;
                case "E"://2 3 4电机
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

                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);


                    break;
                #endregion
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
                    SendCommand("1站升降气缸1升起", out str);
                    SendCommand("1站升降气缸2升起", out str);
                    SendCommand("1站升降气缸3升起", out str);
                    SendCommand("1站升降气缸4升起", out str);
                    SendCommand("1站升降气缸5升起", out str);
                    break;
            }
            #endregion
            return res;
        }


        /// <summary>
        /// 检测电机是否可以运动
        /// </summary>
        /// <returns></returns>
        private string MMI_MotorMotionCanDo()
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
        #endregion

/*****************************************************************************************************************/
/*****************************************************************************************************************/

        #region CAM 处理类
         private bool CAM_Package(string strCmd, out string recStr)
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
                 strHex = "72 04 " + station + " 0F 00 81";

                 SendHex(strHex, out recStr);
                 Log.Debug(strCmd + "==" + recStr);
                 return true;
             }
             //else if (strCmd == "报警")
             //{
             //    if (station != "11")
             //    {
             //        strHex = "72 05 " + station + " 05 01 00 81";
             //    }
             //    else
             //    {
             //        strHex = "72 05 11 05 18 00 81";
             //    }


             //    SendHex(strHex, out recStr);
             //    Log.Debug(strCmd + "==" + recStr);
             //    return true;
             //}
             //else if (strCmd.IndexOf("测试完成状态") != -1)
             //{
             //    CAM_RecTestOK(strCmd, out recStr);
             //    return true;
             //}
             else if (strCmd.IndexOf("3站取放开始") != -1)
             {
                 CAM_StartStation_3(strCmd, out recStr);
                 return true;
             }
             else if (strCmd.IndexOf("4站取放开始") != -1)
             {
                 recStr = "status=OK";
                 CAM_StartStation_4(strCmd, out recStr);
                 return true;
             }
             else
             {
                 recStr = "status=NOT";
                 return true;
             }

         }

         private bool CAM_Package(string strCmd, string param, out string recStr)
         {
             string strHex = string.Empty;
             if (strCmd == "1站取放")
             {
                 CAM_LeftDo(param);
                 CAM_RightDo(param);
                 CAM_MidDo(param);
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

                 return true;
             }
             else
             {
                 recStr = "status=NOT";
                 return true;
             }





         }
        private void CAM_StartStation_4(string strCmd, out string recStr)
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
        private void CAM_StartStation_3(string strCmd, out string recStr)
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

        /// <summary>
        /// 左运动
        /// </summary>
        /// <param name="pram"></param>
        private void CAM_LeftDo(string pram)
        {
            string str = "OK";
            str = CAM_OrginOK(pram);
            if (str == "status=OK")//电机在原点，且上到位
            {
                SendCommand("1站电机左运动", out str);
                Thread.Sleep(500);

                while (true)
                {
                    string s = CAM_LeftOK(pram);
                    if (s == "status=OK")
                    {
                        break;
                    }
                }
                CAM_LiftDownUpAbsorb(pram);



            }
        }

        /// <summary>
        /// 右运动
        /// </summary>
        /// <param name="pram"></param>
        private void CAM_RightDo(string pram)
        {
            string str = "OK";
            CAM_LeftOK(pram);
            SendCommand("1站电机右运动", out str);
            //Thread.Sleep(2000);
            Thread.Sleep(500);
            //Thread.Sleep(500);
            while (true)
            {
                str = CAM_RightOK(pram);
                if (str == "status=OK")
                {
                    break;
                }
            }
            CAM_RightDownAbsorb(pram);

        }
        /// <summary>
        /// 中运动
        /// </summary>
        /// <param name="pram"></param>
        private void CAM_MidDo(string pram)
        {
            string str = "OK";
            str = CAM_RightOK(pram);
            if (str == "status=OK")//
            {
                SendCommand("1站电机中运动", out str);
                Thread.Sleep(500);
                Thread.Sleep(500);
                while (true)
                {
                    string s =CAM_OrginOK(pram);
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
        private string CAM_OrginOK(string pram)
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
        private string CAM_LeftOK(string pram)
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
        private string CAM_RightOK(string pram)
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
        private string CAM_LiftDownUpAbsorb(string pram)
        {
            #region
            int timeAbsorb = 500;//吸取时间
            int timeErr = 500;
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
            }
            #endregion
            return res;
        }

        /// <summary>
        /// 右边吸取动作
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string CAM_RightDownAbsorb(string pram)
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
            }
            #endregion
            return res;
        }

        #endregion  
        
/*****************************************************************************************************************/
/*****************************************************************************************************************/
    }
}
