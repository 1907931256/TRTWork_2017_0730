using IntegrationSys.Flow;
using System;
using System.IO;
using System.Text;

namespace IntegrationSys.LogUtil
{
	internal static class TimeLog
	{
		private const string FILE_NAME = "time_statistics.csv";

		public static void Save()
		{
			using (StreamWriter streamWriter = new StreamWriter("time_statistics.csv", true, Encoding.GetEncoding("GB2312")))
			{
				FlowControl instance = FlowControl.Instance;
				DateTime dateTime = DateTime.Now;
				foreach (FlowItem current in instance.FlowItemList)
				{
					if (!current.Item.Property.Disable && current.BeginTime < dateTime)
					{
						dateTime = current.BeginTime;
					}
				}
				foreach (FlowItem current2 in instance.FlowItemList)
				{
					if (!current2.Item.Property.Disable)
					{
						streamWriter.WriteLine("{0}, {1}, {2}, {3}", new object[]
						{
							current2.Name,
							(current2.BeginTime - dateTime).TotalSeconds.ToString("F3"),
							(current2.EndTime - dateTime).TotalSeconds.ToString("F3"),
							(current2.EndTime - current2.BeginTime).TotalSeconds.ToString("F3")
						});
					}
				}
			}
		}
	}
}
