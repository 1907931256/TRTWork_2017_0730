using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IntegrationSys.Flow;

namespace IntegrationSys.LogUtil
{
    static class TimeLog
    {
        const string FILE_NAME = @"time_statistics.csv";

        public static void Save()
        {
            using (StreamWriter writer = new StreamWriter(FILE_NAME, true, UnicodeEncoding.GetEncoding("GB2312")))
            {
                FlowControl flowControl = FlowControl.Instance;

                DateTime timeOrigin = DateTime.Now; ;

                foreach (FlowItem flowItem in flowControl.FlowItemList)
                {
                    if (!flowItem.Item.Property.Disable)
                    {
                        if (flowItem.BeginTime < timeOrigin)
                        {
                            timeOrigin = flowItem.BeginTime;
                        }
                    }
                }

                foreach (FlowItem flowItem in flowControl.FlowItemList)
                {
                    if (!flowItem.Item.Property.Disable)
                    {
                        writer.WriteLine("{0}, {1}, {2}, {3}", flowItem.Name, (flowItem.BeginTime - timeOrigin).TotalSeconds.ToString("F3"),
                            (flowItem.EndTime - timeOrigin).TotalSeconds.ToString("F3"), (flowItem.EndTime - flowItem.BeginTime).TotalSeconds.ToString("F3"));
                    }
                }
            }
        }
    }
}
