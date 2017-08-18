using IntegrationSys.Equipment;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using System;
using System.Diagnostics;
using System.Threading;

namespace IntegrationSys
{
	internal static class AppInfo
	{
		public const int STATION_SERVER = 4;

		private const int APP_TYPE_MMI = 0;

		private const int APP_TYPE_CAMERA = 1;

		private static int appType_ = 1;

		private static PhoneInfo phoneInfo_;

		private static EquipmentInfo equipmentInfo_;

		public static PhoneInfo PhoneInfo
		{
			get
			{
				if (AppInfo.phoneInfo_ == null)
				{
					AppInfo.phoneInfo_ = new PhoneInfo();
				}
				return AppInfo.phoneInfo_;
			}
		}

		public static EquipmentInfo EquipmentInfo
		{
			get
			{
				if (AppInfo.equipmentInfo_ == null)
				{
					AppInfo.equipmentInfo_ = new EquipmentInfo();
				}
				return AppInfo.equipmentInfo_;
			}
		}

		public static int AppType
		{
			get
			{
				return AppInfo.appType_;
			}
		}

		public static bool CheckPickPlaceCondition()
		{
			string str = string.Empty;
			string text = string.Empty;
			for (int i = 0; i < 6; i++)
			{
				StationInfo stationInfo = AppInfo.EquipmentInfo.GetStationInfo(i);
				if (stationInfo.Work)
				{
					str = "1" + str;
				}
				else
				{
					str = "0" + str;
				}
				if (stationInfo.Complete)
				{
					text = "1" + text;
				}
				else
				{
					text = "0" + text;
				}
			}
			Log.Debug("CheckPickPlaceCondition work = " + str + ", complete = " + text);
			for (int j = 0; j < 6; j++)
			{
				StationInfo stationInfo2 = AppInfo.EquipmentInfo.GetStationInfo(j);
				if (stationInfo2.Work && !stationInfo2.Complete)
				{
					return false;
				}
			}
			return true;
		}

		//[Conditional("NDEBUG")]
		//public static void TryPickPlace()
		//{
		//	if (AppInfo.CheckPickPlaceCondition())
		//	{
		//		AppInfo.<>c__DisplayClass1 <>c__DisplayClass = new AppInfo.<>c__DisplayClass1();
		//		int num = 4;
		//		if (AppInfo.AppType == 1)
		//		{
		//			num = 1;
		//		}
		//		<>c__DisplayClass.bin = "";
		//		for (int i = num; i >= 0; i--)
		//		{
		//			StationInfo stationInfo = AppInfo.EquipmentInfo.GetStationInfo(i);
		//			if (stationInfo.Work)
		//			{
		//				AppInfo.<>c__DisplayClass1 expr_43 = <>c__DisplayClass;
		//				expr_43.bin += '1';
		//			}
		//			else
		//			{
		//				AppInfo.<>c__DisplayClass1 expr_5E = <>c__DisplayClass;
		//				expr_5E.bin += '0';
		//			}
		//		}
		//		int value = Convert.ToInt32(<>c__DisplayClass.bin, 2);
		//		<>c__DisplayClass.param = Convert.ToString(value);
		//		ThreadPool.QueueUserWorkItem(delegate
		//		{
		//			LiteDataClient.Instance.BroadcastPickPlace();
		//			Log.Debug("PickPlace 1站取放 " + <>c__DisplayClass.bin);
		//			EquipmentCmd.Instance.SendCommand("1站取放", <>c__DisplayClass.param, out <>c__DisplayClass.resp);
		//		});
		//		for (int j = 0; j < 6; j++)
		//		{
		//			StationInfo stationInfo2 = AppInfo.EquipmentInfo.GetStationInfo(j);
		//			stationInfo2.Complete = false;
		//		}
		//	}
		//}
	}
}
