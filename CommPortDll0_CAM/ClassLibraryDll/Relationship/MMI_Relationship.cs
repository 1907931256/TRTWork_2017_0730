using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommonPortCmd
{
    public class MMI_Relationship
    {

      private Common comm_= new Common();

        

        public void RelShip(string strCmd)
       {
           List<CmdName> cmdList = new List<CmdName>();
           List<Action_> actList = new List<Action_>();
            cmdList=CmdPackageXml.LoadFlowtest("CmdPackage.xml");
            foreach (var item in cmdList)
            {
                for (int i = 0; i < Convert.ToInt32(item.Loop); i++)
                {
                    if (item.Name == strCmd)
                    {
                        actList = item.action;
                        foreach (var act in actList)
                        {
                            if (act.Name.IndexOf("延时") != -1)
                            {
                                int time =Convert.ToInt32( act.Name.Substring(act.Name.IndexOf(" ") + 1));
                                Thread.Sleep(time);
                            }
                            else
                            {
                                string res=act.Res;
                               
                             comm_.SendCommand(act.Name, out res);
                                
                            }
                                
                        }
                    }
                }

            }
       }


        //private static void SendCommand(string strCmd, out string recStr)
        //{
        //    string strHex = StrPramToHexPram.StrToHex(strCmd);
        //    if (strHex != "")
        //    {

        //       SendHex(strHex);

        //        Console.WriteLine(strHex);//测试用

        //        Log.Debug(strCmd + "-->命令返回-->" + recStr);
               
        //    }
        //    else
        //    {
        //        recStr = "status=NOT";
              
        //    }
        //}








        /// <summary>
        /// 设备动作危险项目
        /// </summary>
        /// <param name="strCmd">设备动作命令</param>
        //public static void GanRouXian(string strCmd)
        //{

        //    string[] strRec = new string[10];
        //    for (int i = 0; i < strRec.Length; i++)
        //    {
        //        strRec[i] = "0";
        //    }

        //    switch (strCmd)
        //    {
        //        #region 测试完成
        //        case "1站测试完成":
        //            {
        //                //1站后门上到位检测
        //                //1站USB拨出检测
        //               comm.SendCommand("1站后门上到位检测", out strRec[0]);

        //               comm.SendCommand("1站USB拨出检测", out strRec[1]);

        //                while (strRec[0] == "OK" && strRec[1] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;
        //        case "2站测试完成":
        //            {
        //                //2站防抖电机1原点检测
        //                //2站防抖电机2原点检测
        //                //2站灯箱远离检测
        //                comm.SendCommand("2站防抖电机1原点检测", out strRec[0]);
        //                comm.SendCommand("2站防抖电机2原点检测", out strRec[1]);
        //                comm.SendCommand("2站灯箱远离检测", out strRec[2]);

        //                while (strRec[0] == "OK" && strRec[1] == "OK" && strRec[2] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;
        //        case "3站测试完成":
        //            {
        //                //3站45度放平检测
        //                //3站前白光源远离检测
        //                comm.SendCommand("3站45度放平检测", out strRec[0]);
        //                comm.SendCommand("3站前白光源远离检测", out strRec[1]);
        //                while (strRec[0] == "OK" && strRec[1] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;
        //        case "4站测试完成":
        //            {
        //                //4站天板远离检测
        //                comm.SendCommand("4站天板远离检测", out strRec[0]);
        //                while (strRec[0] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;
        //        case "5站测试完成":
        //            {   //5站人工嘴下降检测
        //                //5站隔音上升检测
        //                //5站人工耳远离检测
        //                //5站手机托板下降检测

        //                comm.SendCommand("5站人工嘴下降检测", out strRec[0]);
        //                comm.SendCommand("5站隔音上升检测", out strRec[1]);
        //                comm.SendCommand("5站人工耳远离检测", out strRec[2]);
        //                comm.SendCommand("5站手机托板下降检测", out strRec[3]);


        //                while (strRec[0] == "OK" && strRec[1] == "OK" && strRec[2] == "OK" && strRec[3] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;
        //        case "6站产品到位检测":
        //            {
        //                //6站耳机拔出检测
        //                //6站usb拔出检测
        //                comm.SendCommand("6站耳机拔出检测", out strRec[0]);
        //                comm.SendCommand("6站usb拔出检测", out strRec[1]);
        //                while (strRec[0] == "OK" && strRec[1] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;
        //        #endregion

        //        #region 气缸动作
        //        case "1站前后门关门":
        //            {
        //                //1站电机1原点检测
        //                comm.SendCommand("1站电机1原点检测", out strRec[0]);
        //                while (strRec[0] == "OK")
        //                {
        //                    break;
        //                }
        //            };
        //            break;
        //        case "2站校准靠近":
        //            {
        //                //1站电机2原点检测
        //                //1站电机1原点检测
        //                comm.SendCommand("1站电机2原点检测", out strRec[0]);
        //                comm.SendCommand("1站电机1原点检测", out strRec[1]);
        //                while (strRec[0] == "OK" && strRec[1] == "OK")
        //                {
        //                    break;
        //                }
        //            };
        //            break;
        //        case "2站防抖"://1取放上升，2站取放上升
        //            {
        //                comm.SendCommand("1站电机2原点检测", out strRec[0]);
        //                comm.SendCommand("1站电机1原点检测", out strRec[1]);
        //                while (strRec[0] == "OK" && strRec[1] == "OK")
        //                {
        //                    break;
        //                }
        //            };
        //            break;
        //        case "3站45度顶起":
        //            {
        //                //3站前白光源远离检测
        //                //2，3站机械手原点
        //                comm.SendCommand("1站电机2原点检测", out strRec[0]);
        //                comm.SendCommand("1站电机3原点检测", out strRec[1]);
        //                comm.SendCommand("3站前白光源远离检测", out strRec[1]);
        //                while (strRec[0] == "OK" && strRec[1] == "OK" && strRec[3] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;
        //        case "4站天板靠近":
        //            {
        //                //3站前白光源远离检测
        //                //2，3站机械手原点
        //                comm.SendCommand("1站电机2原点检测", out strRec[0]);
        //                comm.SendCommand("1站电机3原点检测", out strRec[1]);
        //                comm.SendCommand("3站前白光源远离检测", out strRec[1]);
        //                while (strRec[0] == "OK" && strRec[1] == "OK" && strRec[3] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;
        //        case "5站隔音箱下降":
        //            {
        //                //3站前白光源远离检测
        //                //2，3站机械手原点
        //                comm.SendCommand("1站电机4原点检测", out strRec[0]);
        //                comm.SendCommand("1站电机5原点检测", out strRec[1]);
        //                while (strRec[0] == "OK" && strRec[1] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;




        //        #endregion

        //        #region 取放逻辑
        //            //正常测试
        //        case "6站取放":
        //            {
        //                //6站耳机拔出检测
        //                //6站usb拔出检测
        //                comm.SendCommand("6站耳机拔出检测", out strRec[0]);
        //                comm.SendCommand("6站usb拔出检测", out strRec[1]);
        //                while (strRec[0] == "OK" && strRec[1] == "OK")
        //                {
        //                    break;
        //                }
        //            }
        //            break;

        //        #endregion
        //    }

        //}

        


    }
}
