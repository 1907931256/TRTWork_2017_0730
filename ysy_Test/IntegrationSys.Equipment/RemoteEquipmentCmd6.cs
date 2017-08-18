using System;

namespace IntegrationSys.Equipment
{
	internal class RemoteEquipmentCmd6 : RemoteEquipmentCmd
	{
		private static RemoteEquipmentCmd6 instance_;

		public static RemoteEquipmentCmd6 Instance
		{
			get
			{
				if (RemoteEquipmentCmd6.instance_ == null)
				{
					RemoteEquipmentCmd6.instance_ = new RemoteEquipmentCmd6();
				}
				return RemoteEquipmentCmd6.instance_;
			}
		}

		private RemoteEquipmentCmd6() : base(5)
		{
		}
	}
}
