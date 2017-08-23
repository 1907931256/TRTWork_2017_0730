using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;
using Newtonsoft.Json;
using System.IO;
using IntegrationSys.Net;
using System.Threading;
using IntegrationSys.LogUtil;
using IntegrationSys.Equipment;

namespace IntegrationSys.Result
{
    class ResultRecord
    {
        private static List<ResultInfo> resultList_;

        private static Object lockObject_ = new Object();

        /// <summary>
        /// 加载sn的所有测试结果
        /// </summary>
        /// <param name="sn"></param>
        public static void Load(string sn)
        {
            InitResultList();

            for (int i = 0; i < EquipmentInfo.STATION_NUM; i++)
            {
                string filename = @"d:\DataFragment\" + sn + "_" + i;

                try
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        using (JsonReader reader = new JsonTextReader(sr))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            List<ResultInfo> infoList = serializer.Deserialize<List<ResultInfo>>(reader);
                            if (infoList != null)
                            {
                                resultList_.AddRange(infoList);
                            }
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    Log.Debug(e.Message, e);
                }
            }
        }

        public static void Clear()
        {
            if (resultList_ != null)
            {
                resultList_.Clear();
                resultList_ = null;
            }
        }

        /// <summary>
        /// 测试完成后，调用此接口，用来记录测试结果
        /// </summary>
        /// <param name="sn"></param>
        public static void Record(string sn)
        {
            InitResultList();

            foreach (FlowItem flowItem in FlowControl.Instance.FlowItemList)
            {
                if (flowItem.IsFinished() && !flowItem.Item.Property.Disable && !flowItem.Item.Property.Hide)
                {
                    ResultInfo info = new ResultInfo();
                    info.Key = flowItem.Name;
                    info.Value = RestoreCmdResult(flowItem.SpecValueList);
                    resultList_.Add(info);
                }
            }

            if (resultList_.Count > 0)
            {
                string filename = @"d:\DataFragment\" + sn + "_" + NetUtil.GetStationIndex();
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(writer, resultList_);
                    }
                }

                ThreadPool.QueueUserWorkItem(delegate
                {
                    FileTransferClient client = new FileTransferClient(NetUtil.GetStationIp(AppInfo.STATION_SERVER));
                    client.Upload(filename, filename);
                });
            }

        }

        public static string GetResult(string name)
        {
            lock (lockObject_)
            {
                if (resultList_ == null)
                {
                    Load(AppInfo.PhoneInfo.SN);
                }
            }

            foreach (ResultInfo info in resultList_)
            {
                if (info.Key == name)
                {
                    return info.Value;
                }
            }

            return "Res=None";
        }

        private static void InitResultList()
        {
            if (resultList_ == null)
            {
                resultList_ = new List<ResultInfo>();
            }
            resultList_.Clear(); 
        }

        /// <summary>
        /// 从规格值和实测值中恢复命令的返回结果
        /// </summary>
        private static string RestoreCmdResult(List<SpecValue> specList)
        {
            string result = string.Empty;

            if (specList != null && specList.Count != 0)
            {
                foreach (SpecValue specValue in specList)
                {
                    result += specValue.SpecKey + "=" + specValue.MeasuredValue + ";"; 
                }
            }

            return result;
        }
    }
}
