using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPortCmd
{
    public class StrHexToStrPram : EventArgs
    {
   
        private string commResData;//返回数据


        public StrHexToStrPram(string strHex)
        {
            commResData = RecDataToHexPram(strHex);
        }


          public string ActiveData
        {
            get
            {
                return commResData;
            }
        }
        /// <summary>
        /// 解析接受到的Hex数据
        /// </summary>
        /// <param name="strHex">串口数据回传的Hex数据，根据串口回传数据作判断，并回传判断结果</param>

        public static string RecDataToHexPram(string strHex)
        {
            string strRec ="status=OK";

            string str = strHex.Replace(" ", "");
            //取出特定的数据
            string strLength = str.ToUpper().Substring(2, 2);//数据长度
            string strAddress = str.ToUpper().Substring(4, 2);//设备地址
            string strModule = str.ToUpper().Substring(6, 2);//模块
            string strPort = str.ToUpper().Substring(8, 2);//端口
            string strStr = str.ToUpper().Substring(10, str.Length - 12);//取出数据
            /*
             * 01模块表示检测
             * 02模块气缸
             * 03模块光源调节
             * 04状态
             * 
             * 06模块电机部分
             * 
             * 特殊处理部分是主动上报部分
             * 
             * */


            #region 01模块 检测
            if (strModule == "01" && strStr == "FF")//检测
            {
                strRec ="status=NO";
            }
            #endregion

            #region 03模块 光源调节
            //else if (strAddress == "13" && strModule == "03")//光源亮度调节
            //{
            //    strRec = "status=" + Convert.ToInt32(strStr, 16).ToString();
            //}
            else if (strAddress == "14" && strModule == "08")//颜色读取
            {
                string data = strStr.Substring(0, 2);//取出00
                if (data=="00")
                {
                    string R = ShujuChuli.DiGaoHexStringToString(strStr.Substring(2, 4));
                    string G = ShujuChuli.DiGaoHexStringToString(strStr.Substring(6, 4));
                    string B = ShujuChuli.DiGaoHexStringToString(strStr.Substring(10, 4));
                    strRec ="R="+ R +";"+"G="+ G+";"+"B=" + B;
                }
            }
            #endregion
          
            #region 6站电机
            else if (strModule=="06"&& strPort=="01")
            {
                    string X = ShujuChuli.DiGaoHexStringToString(strStr.Substring(0, 4));
                    string Y = ShujuChuli.DiGaoHexStringToString(strStr.Substring(4, 4));
                    string speed =ShujuChuli.HexStringToInt(strStr.Substring(8, 2));
                    string jiaSpeed = ShujuChuli.HexStringToInt(strStr.Substring(10, 2));
                    string jianSpeed = ShujuChuli.HexStringToInt(strStr.Substring(12, 2));
                    string xiLv = ShujuChuli.DiGaoHexStringToString(strStr.Substring(14, 4));
                    strRec = "X=" + X + ";"+"Y=" + Y + ";" + "Speed=" + speed + ";" + "Acceleration=" + jiaSpeed + ";" + "Deceleration=" + jianSpeed + ";" + "Slope=" + xiLv;
                



            }
            else if (strModule == "06" && strPort == "05")
            {
                string X = ShujuChuli.DiGaoHexStringToString(strStr.Substring(0, 4));
                string Y = ShujuChuli.DiGaoHexStringToString(strStr.Substring(4, 4));
                    strRec = "X=" + X + ";"+"Y=" + Y ;
            }
            #endregion

            #region 主动上报部分
            //else if (strModule == "01" && strPort == "01" && strStr == "00")
            //{
            //    switch (strAddress)
            //    {
            //        case "11": strRec = "ProductInPlace_1_OK";
            //            break;
            //        case "12": strRec = "ProductInPlace_2_OK";
            //            break;
            //        case "13": strRec = "ProductInPlace_3_OK";
            //            break;
            //        case "14": strRec = "ProductInPlace_4_OK";
            //            break;
            //        case "15": strRec = "ProductInPlace_5_OK";
            //            break;
            //        case "16": strRec = "ProductInPlace_6_OK";
            //            break;
            //    }
            //}
            //else if (strModule == "01" && strPort == "01" && strStr == "FF")
            //{
            //    switch (strAddress)
            //    {
            //        case "11": strRec = "ProductInPlace_1_NO";
            //            break;
            //        case "12": strRec = "ProductInPlace_2_NO";
            //            break;
            //        case "13": strRec = "ProductInPlace_3_NO";
            //            break;
            //        case "14": strRec = "ProductInPlace_4_NO";
            //            break;
            //        case "15": strRec = "ProductInPlace_5_NO";
            //            break;
            //        case "16": strRec = "ProductInPlace_6_NO";
            //            break;
            //    }
            //}
            #endregion


            return strRec;
        }
    }
}
