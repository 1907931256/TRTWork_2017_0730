
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
    public class Common
    {

        /// <summary>
        /// 握手返回数据包含
        /// </summary>
        public string str_Port_Rec = "0F 00";
   
        public static string Model = "";//设备种类型号制定


        //定义一个委托，用于串口数据主动上报
        public delegate void RecDataSend(object send, ActiveReporting e);
        public event RecDataSend RecDataSendEventHander;

       
        #region 类的私有变量
        private SerialPortWrapper serialWrapper;


        private string station = "";//保存当前站位信息

        //标识一条命令是否是这个互动上报
        private static string sendStringCmdToHex = string.Empty;//保存接受到的命令
        private static string recByteToHex = string.Empty;//保存上位机返回的数据
        private static string recActiyData = string.Empty;//保存组东上报数据

        private static SerialPort comm = new SerialPort();//串口程序的主要处理类

        //保存接受到的串口数据
        private static string recDataStr = "";

        private AutoResetEvent delyEvent;

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

        private CmdDAl cmdDal;

        private static Common common;
        #endregion

        private Common()
        {
          
            XMLConfig config = new XMLConfig();
            station = config.ReadConfing(System.IO.Directory.GetCurrentDirectory(), out Model, out str_woshou);
            serialWrapper = new SerialPortWrapper();
            serialWrapper.Report += new SerialPortWrapper.ReportEventHandler(ReportHandler);
            delyEvent = new AutoResetEvent(true);
            cmdDal=new CmdDAl();
        }

        public static Common CreatCommon()
        {
            if (common == null)
            {
                return common = new Common();
            }
            else
            {
                return common;
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
            recActiyData = ShujuChuli.ByteToHexString(data);
            Log.Debug("ShujuChuli.ByteToHexString(data) recActiyData=" + recActiyData);
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
                    //string str = System.Text.Encoding.Default.GetString(resp);

                    string hex = ShujuChuli.ByteToHexString(resp);

                    if (hex.IndexOf("34 04 " + station + " 0F 00") != -1)
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
                recStr="status=timeout";
            }
            catch (InvalidDataException)//数据出错
            {
                Log.Debug("InvalidDataException already happen");
                Thread.Sleep(500);
                try 
	            {
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
            CmdInfo cmdInfo = new CmdInfo();
            cmdInfo = cmdDal.CmdByCmdName_DAL(strCmd);
            if (cmdInfo==null)
            {
                recStr = "status=NOT";
                return true;
            }
            string strHex = cmdInfo.Start + " " + cmdInfo.Length + " " + cmdInfo.Adress + " " + cmdInfo.Model + " " + cmdInfo.Port + " " + cmdInfo.StrPram + " " + cmdInfo.End;

            SendHex(strHex, out recStr);
            Log.Debug(strCmd + "==" + recStr);
            return true;
        }


        public bool SendCommand(string strCmd, out string recStr)
        {
            Log.Debug("Rrc cmd  SendCommand=" + strCmd);

          CmdInfo cmdInfo=new CmdInfo();
            cmdInfo= cmdDal.CmdByCmdName_DAL(strCmd);
            if (cmdInfo==null)
            {
                recStr = "status=NOT";
                return true;
            }
            string strHex = cmdInfo.Start + " " + cmdInfo.Length + " " + cmdInfo.Adress + " " + cmdInfo.Model + " " + cmdInfo.Port + " " + cmdInfo.StrPram + " " + cmdInfo.End;

           if (strHex != "")
            {
              

                SendHex(strHex, out recStr);

                Log.Debug(strCmd +"=="+ recStr);

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

            else
            {
                recStr = "status=NOT";
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
            Log.Debug("Rrc cmd  SendCommand=" + strCmd);

             CmdInfo cmdInfo=new CmdInfo();
            cmdInfo= cmdDal.CmdByCmdName_DAL(strCmd);
            if (cmdInfo==null)
            {
                recStr = "status=NOT";
                return true;
            }
            string strHex = cmdInfo.Start + " " + cmdInfo.Length + " " + cmdInfo.Adress + " " + cmdInfo.Model + " " + cmdInfo.Port + " " + param + " " + cmdInfo.End;


            if (strHex != "")
            {
                if (DelayCmd.WhetherCmd(strCmd))//命令是一条延迟命令
                {
                         delyEvent.WaitOne();//等待延迟数据返回

                         Log.Debug("Cmd is delay cmd -->" + strCmd + param);

                         SendHex(strHex, out recStr);//发送命令

                    return true;
                }
                else
                {

                    SendHex(strHex, out recStr);//发送命令
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
                SendCommand("1站电机左运动", pram, out str);
                Thread.Sleep(1000);
                while (true)
                {
                    string s = RightOK(pram);
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
            this.RightOK(pram);

            SendCommand("1站电机右运动", pram, out str);
            Thread.Sleep(1500);
            while (true)
            {
                str = LeftOK(pram);
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
            str = this.LeftOK(pram);
            if (str == "status=OK")//
            {
                // LiftDownUpAbsorb(pram);
                SendCommand("1站电机中运动", pram, out str);
                Thread.Sleep(1000);
                while (true)
                {

                    string s = OrginOK(pram);
                    if (s == "status=OK")
                    {

                        break;
                    }
                    SendCommand("1站电机原点", out str);
                    Thread.Sleep(1000);
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
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);
                        if (str[0] == "status=OK" && str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER1";
                        }
                        Thread.Sleep(100);
                    }


                    break;
                case "2"://2电机
                    while (true)
                    {

                        SendCommand("1站电机2原点检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);
                        if (str[3] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER2";
                        }
                        Thread.Sleep(100);
                    }


                    break;
                case "3"://1 2电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);
                        SendCommand("1站电机2原点检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        if (str[0] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER12";
                        }
                        Thread.Sleep(100);
                    }



                    break;

                case "4"://3电机
                    while (true)
                    {
                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);
                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "5"://1 3电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "6"://2 3电机
                    while (true)
                    {
                        SendCommand("1站电机2原点检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);
                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "7"://1 2 3电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2原点检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "8"://4电机
                    while (true)
                    {
                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "9"://1 4电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "10"://2 4电机
                    while (true)
                    {
                        SendCommand("1站电机2原点检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "11"://1 2 4电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2原点检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "12"://3 4电机
                    while (true)
                    {
                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);


                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "13"://1 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "14"://2 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[3]);
                        SendCommand("1站升降气缸1升起", out str[4]);
                        SendCommand("1站上升1检测", out str[5]);

                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "15"://1 2 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机1原点检测", out str[3]);
                        SendCommand("1站升降气缸1升起", out str[4]);
                        SendCommand("1站上升1检测", out str[5]);

                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "16"://5电机
                    while (true)
                    {
                        SendCommand("1站电机5原点检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);
                        if (str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER5";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "24":// 4 5电机
                    while (true)
                    {
                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5原点检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER5";
                        }
                        Thread.Sleep(100);
                    }
                    break;
                case "28":// 3 4 5电机
                    while (true)
                    {

                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5原点检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER5";
                        }
                        Thread.Sleep(100);
                    }
                    break;
                case "30"://2 3 4 5电机
                    while (true)
                    {

                        SendCommand("1站电机2原点检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5原点检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER5";
                        }
                        Thread.Sleep(100);
                    }
                    break;
                case "31"://1 2 3 4 5电机
                    while (true)
                    {
                        SendCommand("1站电机1原点检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机1原点检测", out str[3]);
                        SendCommand("1站升降气缸1升起", out str[4]);
                        SendCommand("1站上升1检测", out str[5]);

                        SendCommand("1站电机3原点检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4原点检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5原点检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER5";
                        }
                        Thread.Sleep(100);
                    }

                    break;
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
            #region
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
                        SendCommand("1站电机1左边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);
                        if (str[0] == "status=OK" && str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER1";
                        }
                        Thread.Sleep(100);
                    }
                    break;
                case "2"://2电机
                    while (true)
                    {
                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);
                        if (str[3] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER2";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "3"://1 2电机
                    while (true)
                    {
                        SendCommand("1站电机1左边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        if (str[0] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER12";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                #region
                case "4"://3电机
                    while (true)
                    {
                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);
                        if (str[6] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                    }

                    break;
                case "5"://1 3电机
                    while (true)
                    {
                        SendCommand("1站电机1左边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                    }

                    break;
                case "6"://2 3电机
                    while (true)
                    {
                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                    }

                    break;
                #endregion
                case "7"://1 2 3电机
                    while (true)
                    {
                        SendCommand("1站电机1左边检测", out str[0]);
                        // SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2左边检测", out str[3]);
                        // SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3左边检测", out str[6]);
                        // SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[0] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "8"://4电机
                    while (true)
                    {
                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "9"://1 4电机
                    while (true)
                    {
                        SendCommand("1站电机1左边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "10"://2 4电机
                    while (true)
                    {
                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "11"://1 2 4电机
                    while (true)
                    {
                        SendCommand("1站电机1左边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "12"://3 4电机
                    while (true)
                    {
                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);


                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "13"://1 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机1左边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);


                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[0] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "14"://2 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "15"://1 2 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机1左边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "16"://5电机
                    while (true)
                    {
                        SendCommand("1站电机5左边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);
                        if (str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER5";
                        }
                        Thread.Sleep(100);
                    }

                    break;
                case "24"://4 5电机
                    while (true)
                    {
                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5左边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                        Thread.Sleep(100);
                    }
                    break;
                case "28"://3 4 5电机
                    while (true)
                    {

                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5左边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                        Thread.Sleep(100);
                    }
                    break;
                case "30":// 2 3 4 5电机
                    while (true)
                    {

                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5左边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                        Thread.Sleep(100);
                    }
                    break;
                case "31"://1 2 3 4 5电机
                    while (true)
                    {
                        SendCommand("1站电机1左边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2左边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3左边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4左边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5左边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                        Thread.Sleep(100);
                    }
                    break;
            }
            #endregion
            return res;
        }

        /// <summary>
        /// 右检测OK
        /// </summary>
        /// <param name="pram"></param>
        /// <returns></returns>
        private string RightOK(string pram)
        {
            #region
            string res = "";
            string[] str = new string[20];
            for (int i = 0; i < 20; i++)
            {
                str[i] = "";
            }
            switch (pram)
            {
                case "1"://1电机
                    while (true)
                    {
                        SendCommand("1站升降气缸1升起", out str[0]);
                        SendCommand("1站电机1右边检测", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);
                        if (str[1] == "status=OK" && str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }
                    break;
                case "2"://2电机
                    while (true)
                    {
                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);
                        if (str[3] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER2";
                        }

                    }


                    break;
                case "3"://1 2电机
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER12";
                        }
                    }

                    break;

                case "4"://3电机
                    while (true)
                    {
                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);
                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                    }

                    break;
                case "5"://1 3电机
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                    }

                    break;
                case "6"://2 3电机
                    while (true)
                    {
                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                    }

                    break;
                case "7"://1 2 3电机
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        if (str[0] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[8] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER3";
                        }
                    }

                    break;
                case "8"://4电机
                    while (true)
                    {
                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                    }

                    break;
                case "9"://1 4电机
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                    }

                    break;
                case "10"://2 4电机
                    while (true)
                    {
                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                    }

                    break;
                case "11"://1 2 4电机
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                    }

                    break;
                case "12"://3 4电机
                    while (true)
                    {
                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);


                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                    }

                    break;
                case "13"://1 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);


                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);
                        if (str[0] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                    }

                    break;
                case "14"://2 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                    }

                    break;
                case "15"://1 2 3 4电机
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER4";
                        }
                    }

                    break;
                case "16"://5电机
                    while (true)
                    {
                        SendCommand("1站电机5右边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);
                        if (str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER5";
                        }
                    }

                    break;
                case "24":// 4 5电机
                    while (true)
                    {

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5右边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER45";
                        }
                    }

                    break;
                case "28":// 3 4 5电机
                    while (true)
                    {
                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5右边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                    }

                    break;
                case "30":// 2 3 4 5电机
                    while (true)
                    {

                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5右边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                    }

                    break;
                case "31"://1 2 3 4 5电机
                    while (true)
                    {
                        SendCommand("1站电机1右边检测", out str[0]);
                        SendCommand("1站升降气缸1升起", out str[1]);
                        SendCommand("1站上升1检测", out str[2]);

                        SendCommand("1站电机2右边检测", out str[3]);
                        SendCommand("1站升降气缸2升起", out str[4]);
                        SendCommand("1站上升2检测", out str[5]);

                        SendCommand("1站电机3右边检测", out str[6]);
                        SendCommand("1站升降气缸3升起", out str[7]);
                        SendCommand("1站上升3检测", out str[8]);

                        SendCommand("1站电机4右边检测", out str[9]);
                        SendCommand("1站升降气缸4升起", out str[10]);
                        SendCommand("1站上升4检测", out str[11]);

                        SendCommand("1站电机5右边检测", out str[12]);
                        SendCommand("1站升降气缸5升起", out str[13]);
                        SendCommand("1站上升5检测", out str[14]);

                        if (str[0] == "status=OK" && str[1] == "status=OK" && str[2] == "status=OK" && str[3] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[6] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                        else
                        {
                            res = "status=ER";
                        }
                    }

                    break;
            }
            #endregion
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
            int timeUp = 200;
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

                    SendCommand("1站升降气缸1降下", out str[0]);
                    SendCommand("1站取放吸嘴1吸取", out str[1]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降1检测", out str[2]);
                        // SendCommand("1站吸嘴1检测", out str[3]);
                        if (str[2] == "status=OK")
                        {
                            break;
                        }

                    }

                    SendCommand("1站升降气缸1升起", out str[4]);

                    break;
                case "2"://2电机

                    SendCommand("1站升降气缸2降下", out str[6]);
                    SendCommand("1站取放吸嘴2吸取", out str[7]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降2检测", out str[8]);
                        // SendCommand("1站吸嘴2检测", out str[9]);
                        if (str[8] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸2升起", out str[10]);

                    break;
                case "3"://1 2电机
                    SendCommand("1站升降气缸1降下", out str[0]);
                    SendCommand("1站取放吸嘴1吸取", out str[1]);
                    SendCommand("1站升降气缸2降下", out str[6]);
                    SendCommand("1站取放吸嘴2吸取", out str[7]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        Thread.Sleep(timeAbsorb);
                        SendCommand("1站下降1检测", out str[2]);
                        //SendCommand("1站吸嘴1检测", out str[3]);
                        SendCommand("1站下降2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[9]);
                        if (str[2] == "status=OK" && str[8] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1升起", out str[4]);
                    SendCommand("1站升降气缸2升起", out str[10]);

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
                    SendCommand("1站升降气缸1降下", out str[0]);
                    SendCommand("1站取放吸嘴1吸取", out str[0]);

                    SendCommand("1站升降气缸2降下", out str[0]);
                    SendCommand("1站取放吸嘴2吸取", out str[0]);

                    SendCommand("1站升降气缸3降下", out str[0]);
                    SendCommand("1站取放吸嘴3吸取", out str[0]);
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

                    //while (true)
                    //{
                    //    Thread.Sleep(timeUp);
                    //    SendCommand("1站吸嘴1检测", out str[17]);
                    //    SendCommand("1站吸嘴1检测", out str[18]);
                    //    SendCommand("1站吸嘴3检测", out str[19]);
                    //    if (str[17] == "status=OK" && str[18] == "status=OK" && str[19] == "status=OK")
                    //    {
                    //        res = "status=OK";
                    //        break;
                    //    }
                    //}

                    break;
                case "8"://4电机
                    SendCommand("1站升降气缸4降下", out str[1]);
                    SendCommand("1站取放吸嘴4吸取", out str[2]);
                    Thread.Sleep(timeUp);
                    while (true)
                    {
                        SendCommand("1站下降4检测", out str[8]);
                        //SendCommand("1站吸嘴4检测", out str[9]);
                        if (str[8] == "status=OK")
                        {
                            break;
                        }
                        Thread.Sleep(timeAbsorb);
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
                        SendCommand("1站下降5检测", out str[8]);
                        //SendCommand("1站吸嘴5检测", out str[9]);
                        if (str[8] == "status=OK")
                        {
                            break;
                        }
                        Thread.Sleep(timeAbsorb);
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
            int timeUp = 200;//间隔多久放气
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

                    SendCommand("1站升降气缸1升起", out str[0]);

                    while (true)
                    {
                        //   Thread.Sleep(timeAbsorb);
                        SendCommand("1站上升1检测", out str[2]);
                        //  SendCommand("1站吸嘴1检测", out str[3]);
                        SendCommand("1站电机1左边检测", out str[4]);
                        if (str[2] == "status=OK" && str[4] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1降下", out str[5]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str[1]);
                    while (true)
                    {

                        SendCommand("1站下降1检测", out str[6]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸1升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }
                    break;
                case "2"://2电机

                    SendCommand("1站升降气缸2升起", out str[0]);
                    while (true)
                    {
                        //   Thread.Sleep(timeAbsorb);
                        SendCommand("1站上升2检测", out str[2]);
                        //SendCommand("1站吸嘴2检测", out str[3]);
                        SendCommand("1站电机2左边检测", out str[4]);
                        if (str[2] == "status=OK" && str[4] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸2降下", out str[5]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴2放气", out str[3]);
                    while (true)
                    {

                        SendCommand("1站下降2检测", out str[6]);
                        if (str[5] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸2升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升2检测", out str[2]);
                        if (str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
                        }
                    }
                    break;
                case "3"://1 2电机

                    SendCommand("1站升降气缸1升起", out str[0]);
                    SendCommand("1站升降气缸2升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升1检测", out str[2]);
                        //SendCommand("1站吸嘴1检测", out str[3]);
                        SendCommand("1站电机1左边检测", out str[4]);

                        SendCommand("1站上升2检测", out str[5]);
                        //SendCommand("1站吸嘴2检测", out str[6]);
                        SendCommand("1站电机2左边检测", out str[7]);

                        if (str[2] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[7] == "status=OK")
                        {
                            break;
                        }

                    }
                    SendCommand("1站升降气缸1降下", out str[5]);
                    SendCommand("1站升降气缸2降下", out str[6]);
                    Thread.Sleep(timeUp);
                    SendCommand("1站取放吸嘴1放气", out str[3]);
                    SendCommand("1站取放吸嘴2放气", out str[3]);
                    while (true)
                    {
                        SendCommand("1站下降1检测", out str[6]);
                        SendCommand("1站下降2检测", out str[7]);
                        if (str[6] == "status=OK" && str[7] == "status=OK")
                        {

                            break;
                        }
                    }
                    SendCommand("1站升降气缸1升起", out str[0]);
                    SendCommand("1站升降气缸2升起", out str[0]);
                    while (true)
                    {
                        SendCommand("1站上升2检测", out str[2]);
                        SendCommand("1站上升1检测", out str[1]);
                        if (str[1] == "status=OK" && str[2] == "status=OK")
                        {
                            res = "status=OK";
                            break;
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
                        SendCommand("1站电机3左边检测", out str[4]);
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
                        SendCommand("1站电机1左边检测", out str[4]);

                        SendCommand("1站上升2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[9]);
                        SendCommand("1站电机2左边检测", out str[10]);

                        SendCommand("1站上升3检测", out str[5]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        SendCommand("1站电机3左边检测", out str[7]);

                        if (str[2] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[10] == "status=OK")
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
                        SendCommand("1站电机4左边检测", out str[4]);
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
                        SendCommand("1站电机1左边检测", out str[4]);

                        SendCommand("1站上升2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[9]);
                        SendCommand("1站电机2左边检测", out str[10]);

                        SendCommand("1站上升3检测", out str[5]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        SendCommand("1站电机3左边检测", out str[7]);

                        SendCommand("1站上升4检测", out str[11]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        SendCommand("1站电机4左边检测", out str[13]);


                        if (str[2] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[13] == "status=OK")
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
                        SendCommand("1站电机5左边检测", out str[4]);
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
                        SendCommand("1站电机4左边检测", out str[13]);

                        SendCommand("1站上升5检测", out str[14]);
                        //SendCommand("1站吸嘴5检测", out str[15]);
                        SendCommand("1站电机5左边检测", out str[16]);

                        if (str[11] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK" && str[16] == "status=OK")
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
                        SendCommand("1站电机3左边检测", out str[7]);

                        SendCommand("1站上升4检测", out str[11]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        SendCommand("1站电机4左边检测", out str[13]);

                        SendCommand("1站上升5检测", out str[14]);
                        //SendCommand("1站吸嘴5检测", out str[15]);
                        SendCommand("1站电机5左边检测", out str[16]);

                        if (str[5] == "status=OK" && str[7] == "status=OK" && str[11] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK" && str[16] == "status=OK")
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
                        SendCommand("1站电机2左边检测", out str[9]);

                        SendCommand("1站上升3检测", out str[10]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        SendCommand("1站电机3左边检测", out str[11]);

                        SendCommand("1站上升4检测", out str[12]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        SendCommand("1站电机4左边检测", out str[13]);

                        SendCommand("1站上升5检测", out str[14]);
                        //SendCommand("1站吸嘴5检测", out str[15]);
                        SendCommand("1站电机5左边检测", out str[15]);

                        if (str[8] == "status=OK" && str[9] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[12] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK" && str[15] == "status=OK")
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
                        SendCommand("1站电机1左边检测", out str[4]);

                        SendCommand("1站上升2检测", out str[8]);
                        //SendCommand("1站吸嘴2检测", out str[9]);
                        SendCommand("1站电机2左边检测", out str[10]);

                        SendCommand("1站上升3检测", out str[5]);
                        //SendCommand("1站吸嘴3检测", out str[6]);
                        SendCommand("1站电机3左边检测", out str[7]);

                        SendCommand("1站上升4检测", out str[11]);
                        //SendCommand("1站吸嘴4检测", out str[12]);
                        SendCommand("1站电机4左边检测", out str[13]);

                        SendCommand("1站上升5检测", out str[14]);
                        //SendCommand("1站吸嘴5检测", out str[15]);
                        SendCommand("1站电机5左边检测", out str[16]);

                        if (str[2] == "status=OK" && str[4] == "status=OK" && str[5] == "status=OK" && str[7] == "status=OK" && str[8] == "status=OK" && str[10] == "status=OK" && str[11] == "status=OK" && str[13] == "status=OK" && str[14] == "status=OK" && str[16] == "status=OK")
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
