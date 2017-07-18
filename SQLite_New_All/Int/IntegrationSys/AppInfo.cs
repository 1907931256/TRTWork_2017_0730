using IntegrationSys.Equipment;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using System;

namespace IntegrationSys
{
	internal static class AppInfo
	{
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

		public static bool CheckPickPlaceCondition()
		{
			for (int i = 0; i < 6; i++)
			{
				StationInfo stationInfo = AppInfo.EquipmentInfo.GetStationInfo(i);
				if (stationInfo.Work && !stationInfo.Complete)
				{
					return false;
				}
			}
			return true;
		}

		public static void TryPickPlace()
		{
			if (AppInfo.CheckPickPlaceCondition())
			{
				string text = "";
				for (int i = 5; i >= 0; i--)
				{
					StationInfo stationInfo = AppInfo.EquipmentInfo.GetStationInfo(i);
					if (stationInfo.Work)
					{
						text += '1';
					}
					else
					{
						text += '0';
					}
				}
				int value = Convert.ToInt32(text, 2);
				string param = Convert.ToString(value);
				for (int j = 0; j < 6; j++)
				{
					StationInfo stationInfo2 = AppInfo.EquipmentInfo.GetStationInfo(j);
					stationInfo2.Complete = false;
				}
				Log.Debug("PickPlace 1站取放 " + text);
				string text2;
				EquipmentCmd.Instance.SendCommand("1站取放", param, out text2);
				LiteDataClient.Instance.BroadcastPickPlace();
			}
		}
	}
}
