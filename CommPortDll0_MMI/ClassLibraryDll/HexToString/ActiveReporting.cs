using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPortCmd
{
    public enum ActiveEnumData
    {
        #region   产品到位检测
        /// <summary>
        /// 产品到位
        /// </summary>
        ProductInPlace_OK = 1,
        ProductInPlace_NO = 2,

        /// <summary>
        /// 站报警
        /// </summary>
        AlarmClear_OK = 3,
        AlarmClear_NO = 4,
        ///// <summary>
        ///// 原点检测到位
        ///// </summary>
        //OriginDetection_OK = 4,
        //OriginDetection_NO = 5,

        ///// <summary>
        ///// 状态检测
        ///// </summary>
        //StateDetection_2_OK = 6,
        //StateDetection_2_NO = 7,
       NULL=10

        #endregion

    };
    public class ActiveReporting : EventArgs
    {
        //private ActiveEnumData commActiveReportingData;//得到的數據
        private ActiveEnumData eventId;


        public ActiveReporting(string strHex)
        {
            eventId = ActiveReportingDataToEnum(strHex);
                 
        }

        public ActiveEnumData EventId
        {
            get
            {
                return eventId;
            }
        }

        private ActiveEnumData ActiveReportingDataToEnum(string strHex)
        {
            ActiveEnumData actEnumData;

            //if (strHex.IndexOf() != -1) ///产品检测
            //{
            //    actEnumData = ActiveEnumData.ProductInPlace_OK;
            //}
            if (strHex.IndexOf("01 01 00") != -1) ///产品检测   "0A 01 00"
            {
                actEnumData = ActiveEnumData.ProductInPlace_OK;
            }
            else if (strHex.IndexOf("01 01 FF") != -1)//"0A 01 FF"
            {
                actEnumData = ActiveEnumData.ProductInPlace_NO;
            }
            else if (strHex.IndexOf("0A 02 FF") != -1)
            {
                actEnumData = ActiveEnumData.AlarmClear_NO;
            }
            else if (strHex.IndexOf("0A 02 00") != -1)
            {
                actEnumData = ActiveEnumData.AlarmClear_OK;
            }
            //else if (strHex.IndexOf("0A 03 FF") != -1)//1站运动完成
            //{
            //    actEnumData = ActiveEnumData.AlarmClear_OK;
            //}
            //else if (strHex.IndexOf("16 0A 03") != -1)//6站运动完成
            //{
            //    actEnumData = ActiveEnumData.AlarmClear_OK;
            //}
            else
            {
                actEnumData = ActiveEnumData.NULL;
            }

            return actEnumData;

        }


    }

}

