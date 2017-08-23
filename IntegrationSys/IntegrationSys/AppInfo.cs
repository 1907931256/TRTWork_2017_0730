using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Equipment;
using IntegrationSys.Net;
using IntegrationSys.LogUtil;
using System.Diagnostics;
using System.Threading;

namespace IntegrationSys
{
    /// <summary>
    /// 保存应用程序的全局信息
    /// </summary>
    static class AppInfo
    {
        public const int STATION_SERVER = 5;

        const int APP_TYPE_MMI = 0;
        const int APP_TYPE_CAMERA = 1;

        private static int appType_ = APP_TYPE_MMI;

        private static PhoneInfo phoneInfo_;

        private static EquipmentInfo equipmentInfo_;

        public static PhoneInfo PhoneInfo
        {
            get
            {
                if (phoneInfo_ == null)
                {
                    phoneInfo_ = new PhoneInfo();
                }
                return phoneInfo_;
            }
        }

        public static EquipmentInfo EquipmentInfo
        {
            get
            {
                if (equipmentInfo_ == null)
                {
                    equipmentInfo_ = new EquipmentInfo();
                }
                return equipmentInfo_;
            }
        }

        public static int AppType
        {
            get
            {
                return appType_;
            }
        }


        public static bool CheckPickPlaceCondition()
        {
            string work = string.Empty;
            string complete = string.Empty;
            for (int i = 0; i < EquipmentInfo.STATION_NUM; i++)
            {
                StationInfo stationInfo = EquipmentInfo.GetStationInfo(i);
                if (stationInfo.Work)
                {
                    work = "1" + work;
                }
                else
                {
                    work = "0" + work;
                }
                if (stationInfo.Complete)
                {
                    complete = "1" + complete;
                }
                else
                {
                    complete = "0" + complete;
                }
            }
            Log.Debug("CheckPickPlaceCondition work = " + work + ", complete = " + complete);

            for (int i = 0; i < EquipmentInfo.STATION_NUM; i++)
            {
                StationInfo stationInfo = EquipmentInfo.GetStationInfo(i);

                if (stationInfo.Work && !stationInfo.Complete)
                {
                    return false;
                }
            }

            return true;        
        }

        /// <summary>
        /// 检查取放条件，满足则取放
        /// </summary>
        [Conditional("NDEBUG")]
        public static void TryPickPlace()
        {
            if (CheckPickPlaceCondition())
            {
                int num = EquipmentInfo.STATION_NUM - 2;
                if (AppInfo.AppType == AppInfo.APP_TYPE_CAMERA)
                {
                    num = 2;
                }
                string bin = "";
                for (int i = num; i >= 0; i--)
                {
                    StationInfo stationInfo = EquipmentInfo.GetStationInfo(i);

                    if (stationInfo.Work)
                    {
                        bin += '1';
                    }
                    else
                    {
                        bin += '0';
                    }
                }

                int d = Convert.ToInt32(bin, 2);
                string param = Convert.ToString(d);
                string resp;

                ThreadPool.QueueUserWorkItem(delegate {
                    LiteDataClient.Instance.BroadcastPickPlace();

                    Log.Debug("PickPlace " + "1站取放 " + bin);
                    EquipmentCmd.Instance.SendCommand("1站取放", param, out resp);
                });

                for (int i = 0; i < EquipmentInfo.STATION_NUM; i++)
                {
                    StationInfo stationInfo = EquipmentInfo.GetStationInfo(i);
                    stationInfo.Complete = false;
                }
            }
        }
    }

    /// <summary>
    /// 手机信息
    /// </summary>
    class PhoneInfo
    {
        public string SN
        {
            get;
            set;
        }

        public string IP
        {
            get;
            set;
        }

        /// <summary>
        /// 0：Usb
        /// 1: Wifi
        /// </summary>
        public int ConnectType
        {
            get;
            set;
        }
    }
}
