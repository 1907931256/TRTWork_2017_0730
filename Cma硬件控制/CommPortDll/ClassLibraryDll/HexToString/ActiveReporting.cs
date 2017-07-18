using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPortCmd
{
    public enum ActiveEnumData
    {
        /// <summary>
        /// 1站报警清除
        /// </summary>
        AlarmClear_1_OK = 12,
        AlarmClear_1_NO = 13,

        #region   产品到位检测
        /// <summary>
        /// 1站产品到位
        /// </summary>
        ProductInPlace_1_OK = 0,
        ProductInPlace_2_OK = 1,
        ProductInPlace_3_OK = 2,
        ProductInPlace_4_OK = 3,
        ProductInPlace_5_OK = 4,
        ProductInPlace_6_OK = 5,
        ProductInPlace_1_NO = 6,
        ProductInPlace_2_NO = 7,
        ProductInPlace_3_NO = 8,
        ProductInPlace_4_NO = 9,
        ProductInPlace_5_NO = 10,
        ProductInPlace_6_NO = 11,
        #endregion

        #region 原点到位检测
        /// <summary>
        /// 1站原点2检测到位
        /// </summary>
        OriginDetection_2_OK = 14,
        OriginDetection_2_NO = 15,
        OriginDetection_3_OK = 16,
        OriginDetection_3_NO = 17,
        OriginDetection_4_OK = 18,
        OriginDetection_4_NO = 19,
        OriginDetection_5_OK = 20,
        OriginDetection_5_NO = 21,
        OriginDetection_6_OK = 22,
        OriginDetection_6_NO = 23,
        #endregion

        #region  状态检测
        /// <summary>
        /// 1站状态检测2
        /// </summary>
        StateDetection_2_OK = 24,
        StateDetection_2_NO = 25,
        StateDetection_3_OK = 26,
        StateDetection_3_NO = 27,
        StateDetection_4_OK = 28,
        StateDetection_4_NO = 29,
        StateDetection_5_OK = 30,
        StateDetection_5_NO = 31,
        StateDetection_6_OK = 32,
        StateDetection_6_NO = 33,
        #endregion



    };
    public class ActiveReporting : EventArgs
    {
        private string commActiveReportingData;//得到的數據

       
        public ActiveReporting(string strHex)
        {
            commActiveReportingData = ActiveReportingDataToEnum(strHex);
        }

        private string ActiveReportingDataToEnum(string strHex)
        {
            string commRecHexData = "";

            string str = strHex.Replace(" ", "");
            //取出特定的数据
            string strLength = str.ToUpper().Substring(2, 2);//数据长度
            string strAddress = str.ToUpper().Substring(4, 2);//设备地址
            string strModule = str.ToUpper().Substring(6, 2);//模块
            string strPort = str.ToUpper().Substring(8, 2);//端口
            string strStr = str.ToUpper().Substring(10, str.Length - 12);//取出数据


            #region 产品到位
            if (strModule == "01" && strPort == "01" && strStr == "00")
            {
                switch (strAddress)
                {
                    case "11": commRecHexData = ActiveEnumData.ProductInPlace_1_OK.ToString();
                        break;
                    case "12": commRecHexData = ActiveEnumData.ProductInPlace_2_OK.ToString();
                        break;
                    case "13": commRecHexData = ActiveEnumData.ProductInPlace_3_OK.ToString();
                        break;
                    case "14": commRecHexData = ActiveEnumData.ProductInPlace_4_OK.ToString();
                        break;
                    case "15": commRecHexData = ActiveEnumData.ProductInPlace_5_OK.ToString();
                        break;
                    case "16": commRecHexData = ActiveEnumData.ProductInPlace_6_OK.ToString();
                        break;
                }
            }
            else if (strModule == "01" && strPort == "01" && strStr == "FF")
            {
                switch (strAddress)
                {
                    case "11": commRecHexData = ActiveEnumData.ProductInPlace_1_NO.ToString();
                        break;
                    case "12": commRecHexData = ActiveEnumData.ProductInPlace_2_NO.ToString();
                        break;
                    case "13": commRecHexData = ActiveEnumData.ProductInPlace_3_NO.ToString();
                        break;
                    case "14": commRecHexData = ActiveEnumData.ProductInPlace_4_NO.ToString();
                        break;
                    case "15": commRecHexData = ActiveEnumData.ProductInPlace_5_NO.ToString();
                        break;
                    case "16": commRecHexData = ActiveEnumData.ProductInPlace_6_NO.ToString();
                        break;
                }
            }
            #endregion

            if (strAddress == "11" && strModule == "01" && strPort == "08")//报警清除
            {
                if (strStr=="00")
                {
                    commRecHexData = ActiveEnumData.AlarmClear_1_OK.ToString();
                }
                else
	            {
                    commRecHexData = ActiveEnumData.AlarmClear_1_NO.ToString();
                }

            }
            #region  2站主动上报除去  产品到位
            if (strAddress=="12"&& strModule=="01"&& strPort=="0A")//1站原点检测2
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.OriginDetection_2_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.OriginDetection_2_NO.ToString();
                }

            }
            else if (strAddress == "12" && strModule == "01" && strPort == "0B")
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.StateDetection_2_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.StateDetection_2_OK.ToString();
                }

            }

            #endregion

            #region  3站主动上报除去  产品到位
            if (strAddress == "12" && strModule == "01" && strPort == "09")//1站原点检测2
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.OriginDetection_3_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.OriginDetection_3_NO.ToString();
                }

            }
            else if (strAddress == "12" && strModule == "01" && strPort == "0A")
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.StateDetection_3_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.StateDetection_3_OK.ToString();
                }

            }

            #endregion

            #region  4站主动上报除去  产品到位
            if (strAddress == "12" && strModule == "01" && strPort == "04")//1站原点检测2
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.OriginDetection_4_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.OriginDetection_4_NO.ToString();
                }

            }
            else if (strAddress == "12" && strModule == "01" && strPort == "05")
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.StateDetection_4_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.StateDetection_4_OK.ToString();
                }

            }

            #endregion

            #region  5站主动上报除去  产品到位
            if (strAddress == "12" && strModule == "01" && strPort == "07")//1站原点检测2
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.OriginDetection_5_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.OriginDetection_5_NO.ToString();
                }

            }
            else if (strAddress == "12" && strModule == "01" && strPort == "08")
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.StateDetection_5_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.StateDetection_5_OK.ToString();
                }

            }

            #endregion

            #region  6站主动上报除去  产品到位
            if (strAddress == "12" && strModule == "01" && strPort == "0C")//1站原点检测2
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.OriginDetection_6_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.OriginDetection_6_NO.ToString();
                }

            }
            else if (strAddress == "12" && strModule == "01" && strPort == "0D")
            {
                if (strStr == "00")
                {
                    commRecHexData = ActiveEnumData.StateDetection_6_OK.ToString();
                }
                else
                {
                    commRecHexData = ActiveEnumData.StateDetection_6_OK.ToString();
                }

            }

            #endregion

            return commRecHexData;

        }

        public string ActiveData
        {
            get
            {
                return commActiveReportingData;
            }
        }

    }

}

