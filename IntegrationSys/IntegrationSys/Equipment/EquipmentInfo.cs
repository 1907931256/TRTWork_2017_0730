using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationSys.Equipment
{
    //记录设备信息
    class StationInfo
    {
        /// <summary>
        /// 站位索引
        /// </summary>
        public int StationIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 是否工作中
        /// 开始测试时，手机依次进仓
        /// 停止测试时，手机依次出仓
        /// 空闲状态只会发生在进仓和出仓这两个阶段
        /// </summary>
        public bool Work
        {
            get;
            set;
        }

        /// <summary>
        /// 一轮测试是否完成
        /// </summary>
        public bool Complete
        {
            get;
            set;
        }
    }

    class EquipmentInfo
    {
        public const int STATION_NUM = 6;

        private StationInfo[] stationsInfo_ = new StationInfo[STATION_NUM];

        public EquipmentInfo()
        {
            for (int i = 0; i < STATION_NUM; i++)
            {
                stationsInfo_[i] = new StationInfo();
            }
        }

        public StationInfo GetStationInfo(int index)
        {
            return stationsInfo_[index]; 
        }
    }
}
