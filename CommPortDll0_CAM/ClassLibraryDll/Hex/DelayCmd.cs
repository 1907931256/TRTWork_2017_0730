using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPortCmd
{
    public class DelayCmd
    {
        /// <summary>
        /// 判断命令是否是延迟命令
        /// 延迟返回true
        /// </summary>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        public static bool WhetherCmd(string strCmd)
        {
            bool b = false;
            switch (strCmd)
            {
            /////////下面部分是 mmi部分
                case "6站运动":
                    b = true;
                    break;
                case "1站电机左运动":
                    b = true;
                    break;
                case "1站电机右运动":
                    b = true;
                    break;
                case "1站电机中运动":
                    b = true;
                    break;
                case "1站电机原点":
                    b = true;
                    break;
            /////////     下面部分是Cam 部分
                case "1站取放":
                    b = true;
                    break;
                case "1站电机回零点":
                    b = true;
                    break;
                case "3站电机":
                    b = true;
                    break;
                case "3站电机回零点":
                    b = true;
                    break;
                


            }
            return b;
        }


        /// <summary>
        /// 如果该命令是一条延迟命令
        /// 在返回数据中找到指定的数据后
        /// 返回True
        /// </summary>
        /// <param name="RecData">传入串口返回接受到的数据</param>
        /// <returns></returns>
        public static bool DelaydDat(string RecData)
        {
            bool b = false;
            //下面是MMI  部分
            if (RecData.IndexOf("16 0A 03") != -1)//电机运动第二次返回
            {
                b = true;
            }
            else if (RecData.IndexOf("11 0A 03 FF") != -1)//去放点击完成上报
            {
                 b = true;
            }
            //下面部分是 Cam 部分
            else if (RecData.IndexOf("C3 0A 03 FF") != -1)//3站运动完成主动上报
            {
                b = true;
            }
            else if (RecData.IndexOf("C1 0A 03 FF") != -1)//1站运动完成主动上报
            {
                b = true;
            }
            return b;
           

        }


    }
}
