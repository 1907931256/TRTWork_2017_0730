using System;

namespace IntegrationSys.Equipment
{
	internal class RemoteEquipmentCmd1 : RemoteEquipmentCmd
	{
		private static RemoteEquipmentCmd1 instance_;

		public static RemoteEquipmentCmd1 Instance
		{
			get
			{
				if (RemoteEquipmentCmd1.instance_ == null)
				{
					RemoteEquipmentCmd1.instance_ = new RemoteEquipmentCmd1();
				}
				return RemoteEquipmentCmd1.instance_;
			}
		}

		private RemoteEquipmentCmd1() : base(0)
		{
		}
	}
}
