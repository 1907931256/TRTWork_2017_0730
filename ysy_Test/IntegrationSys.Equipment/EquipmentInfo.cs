using System;

namespace IntegrationSys.Equipment
{
	internal class EquipmentInfo
	{
		public const int STATION_NUM = 6;

		private StationInfo[] stationsInfo_ = new StationInfo[6];

		public EquipmentInfo()
		{
			for (int i = 0; i < 6; i++)
			{
				this.stationsInfo_[i] = new StationInfo();
			}
		}

		public StationInfo GetStationInfo(int index)
		{
			return this.stationsInfo_[index];
		}
	}
}
