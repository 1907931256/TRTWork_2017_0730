using IntegrationSys.Flow;
using IntegrationSys.Net;
using System;

namespace IntegrationSys.Equipment
{
	internal class RemoteEquipmentCmd : IExecutable
	{
		private int station_;

		public RemoteEquipmentCmd(int index)
		{
			this.station_ = index;
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			retValue = LiteDataClient.Instance.SendEquipmentCmd(this.station_, action, param);
		}
	}
}
