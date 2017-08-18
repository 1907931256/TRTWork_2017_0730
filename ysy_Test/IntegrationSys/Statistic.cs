using IntegrationSys.LogUtil;
using Newtonsoft.Json;
using System;
using System.IO;

namespace IntegrationSys
{
	internal class Statistic
	{
		private StatisticInfo info_;

		private static Statistic instance_;

		public static Statistic Instance
		{
			get
			{
				if (Statistic.instance_ == null)
				{
					Statistic.instance_ = new Statistic();
				}
				return Statistic.instance_;
			}
		}

		public StatisticInfo StatisticInfo
		{
			get
			{
				return this.info_;
			}
		}

		private Statistic()
		{
		}

		public void Load()
		{
			string path = "statistic";
			try
			{
				using (StreamReader streamReader = new StreamReader(path))
				{
					using (JsonReader jsonReader = new JsonTextReader(streamReader))
					{
						JsonSerializer jsonSerializer = new JsonSerializer();
						this.info_ = jsonSerializer.Deserialize<StatisticInfo>(jsonReader);
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				this.info_ = new StatisticInfo();
				Log.Debug(ex.Message, ex);
			}
		}

		public void Save()
		{
			string path = "statistic";
			try
			{
				using (StreamWriter streamWriter = new StreamWriter(path))
				{
					using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
					{
						JsonSerializer jsonSerializer = new JsonSerializer();
						jsonSerializer.Serialize(jsonWriter, this.info_);
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				Log.Debug(ex.Message, ex);
			}
		}

		public void IncreaseTotalNum()
		{
			this.info_.TotalNum++;
		}

		public void IncreaseFailNum()
		{
			this.info_.FailNum++;
		}
	}
}
