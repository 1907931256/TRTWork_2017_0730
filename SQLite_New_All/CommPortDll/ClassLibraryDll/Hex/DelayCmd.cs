using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPortCmd
{
    public class DelayCmd
    {
        ///// <summary>
        ///// 判断命令是否是延迟命令
        ///// 延迟返回true
        ///// </summary>
        ///// <param name="strCmd"></param>
        ///// <returns></returns>
        public static bool WhetherCmd(string strCmd)
        {
            bool b = false;
            switch (strCmd)
            {
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

            }
            return b;
        }


         //<summary>
         //如果该命令是一条延迟命令
         //在返回数据中找到指定的数据后
         //返回True
         //</summary>
         //<param name="RecData">传入串口返回接受到的数据</param>
         //<returns></returns>
        public static bool DelaydDat(string RecData)
        {
            bool b = false;
            if (RecData.IndexOf("34 08 16 08 01") != -1)//电机运动第二次返回
            {
                b = true;
            }
            else if (RecData.IndexOf("34 05 11 04 0C FF 47") != -1)
            {
                 b = true;
            }
            return b;
        }


    }
}
