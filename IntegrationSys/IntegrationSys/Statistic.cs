using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using IntegrationSys.LogUtil;

namespace IntegrationSys
{
    class StatisticInfo
    {
        public int TotalNum
        {
            get;
            set;
        }

        public int FailNum
        {
            get;
            set;
        }
    }

    class Statistic
    {
        private StatisticInfo info_;

        private static Statistic instance_;

        public static Statistic Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new Statistic();
                }

                return instance_;
            }
        }

        private Statistic() { }

        public StatisticInfo StatisticInfo
        {
            get
            {
                return info_;
            }
        }

        public void Load()
        {
            string filename = @"statistic";

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        JsonSerializer serializer = new JsonSerializer();

                        info_ = serializer.Deserialize<StatisticInfo>(reader);
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                info_ = new StatisticInfo();
                Log.Debug(e.Message, e);
            }
        }

        public void Save()
        {
            string filename = @"statistic";

            try
            {
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(writer, info_);
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Log.Debug(e.Message, e);
            }
        }

        /// <summary>
        /// 增加测试总数
        /// </summary>
        public void IncreaseTotalNum()
        {
            info_.TotalNum++;
        }

        /// <summary>
        /// 增加不良数
        /// </summary>
        public void IncreaseFailNum()
        {
            info_.FailNum++;
        }
    }
}
