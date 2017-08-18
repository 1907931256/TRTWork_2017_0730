using System;

namespace IntegrationSys.Equipment
{
	internal class RemoteEquipmentCmd4 : RemoteEquipmentCmd
	{
		private static RemoteEquipmentCmd4 instance_;

		public static RemoteEquipmentCmd4 Instance
		{
			get
			{
				if (RemoteEquipmentCmd4.instance_ == null)
				{
					RemoteEquipmentCmd4.instance_ = new RemoteEquipmentCmd4();
				}
				return RemoteEquipmentCmd4.instance_;
			}
		}

		private RemoteEquipmentCmd4() : base(3)
		{
		}
	}
}
