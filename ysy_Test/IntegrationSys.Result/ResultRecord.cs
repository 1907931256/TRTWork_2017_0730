using IntegrationSys.Flow;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace IntegrationSys.Result
{
	internal class ResultRecord
	{
		private static List<ResultInfo> resultList_;

		private static object lockObject_ = new object();

		public static void Load(string sn)
		{
			ResultRecord.InitResultList();
			for (int i = 0; i < 6; i++)
			{
				string path = string.Concat(new object[]
				{
					"d:\\DataFragment\\",
					sn,
					"_",
					i
				});
				try
				{
					using (StreamReader streamReader = new StreamReader(path))
					{
						using (JsonReader jsonReader = new JsonTextReader(streamReader))
						{
							JsonSerializer jsonSerializer = new JsonSerializer();
							List<ResultInfo> list = jsonSerializer.Deserialize<List<ResultInfo>>(jsonReader);
							if (list != null)
							{
								ResultRecord.resultList_.AddRange(list);
							}
						}
					}
				}
				catch (FileNotFoundException ex)
				{
					Log.Debug(ex.Message, ex);
				}
			}
		}

		public static void Clear()
		{
			if (ResultRecord.resultList_ != null)
			{
				ResultRecord.resultList_.Clear();
				ResultRecord.resultList_ = null;
			}
		}

		public static void Record(string sn)
		{
			ResultRecord.InitResultList();
			foreach (FlowItem current in FlowControl.Instance.FlowItemList)
			{
				if (current.IsFinished() && !current.Item.Property.Disable && !current.Item.Property.Hide)
				{
					ResultInfo resultInfo = new ResultInfo();
					resultInfo.Key = current.Name;
					resultInfo.Value = ResultRecord.RestoreCmdResult(current.SpecValueList);
					ResultRecord.resultList_.Add(resultInfo);
				}
			}
			if (ResultRecord.resultList_.Count > 0)
			{
				string filename = string.Concat(new object[]
				{
					"d:\\DataFragment\\",
					sn,
					"_",
					NetUtil.GetStationIndex()
				});
				using (StreamWriter streamWriter = new StreamWriter(filename))
				{
					using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
					{
						JsonSerializer jsonSerializer = new JsonSerializer();
						jsonSerializer.Serialize(jsonWriter, ResultRecord.resultList_);
					}
				}
				ThreadPool.QueueUserWorkItem(delegate
				{
					FileTransferClient fileTransferClient = new FileTransferClient(NetUtil.GetStationIp(4));
					fileTransferClient.Upload(filename, filename);
				});
			}
		}

		public static string GetResult(string name)
		{
			lock (ResultRecord.lockObject_)
			{
				if (ResultRecord.resultList_ == null)
				{
					ResultRecord.Load(AppInfo.PhoneInfo.SN);
				}
			}
			foreach (ResultInfo current in ResultRecord.resultList_)
			{
				if (current.Key == name)
				{
					return current.Value;
				}
			}
			return "Res=None";
		}

		private static void InitResultList()
		{
			if (ResultRecord.resultList_ == null)
			{
				ResultRecord.resultList_ = new List<ResultInfo>();
			}
			ResultRecord.resultList_.Clear();
		}

		private static string RestoreCmdResult(List<SpecValue> specList)
		{
			string text = string.Empty;
			if (specList != null && specList.Count != 0)
			{
				foreach (SpecValue current in specList)
				{
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						current.SpecKey,
						"=",
						current.MeasuredValue,
						";"
					});
				}
			}
			return text;
		}
	}
}
