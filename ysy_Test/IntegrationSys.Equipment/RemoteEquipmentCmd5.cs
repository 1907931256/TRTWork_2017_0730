using System;

namespace IntegrationSys.Equipment
{
	internal class RemoteEquipmentCmd5 : RemoteEquipmentCmd
	{
		private static RemoteEquipmentCmd5 instance_;

		public static RemoteEquipmentCmd5 Instance
		{
			get
			{
				if (RemoteEquipmentCmd5.instance_ == null)
				{
					RemoteEquipmentCmd5.instance_ = new RemoteEquipmentCmd5();
				}
				return RemoteEquipmentCmd5.instance_;
			}
		}

		private RemoteEquipmentCmd5() : base(4)
		{
		}
	}
}
