using System;

namespace IntegrationSys.Equipment
{
	internal class RemoteEquipmentCmd2 : RemoteEquipmentCmd
	{
		private static RemoteEquipmentCmd2 instance_;

		public static RemoteEquipmentCmd2 Instance
		{
			get
			{
				if (RemoteEquipmentCmd2.instance_ == null)
				{
					RemoteEquipmentCmd2.instance_ = new RemoteEquipmentCmd2();
				}
				return RemoteEquipmentCmd2.instance_;
			}
		}

		private RemoteEquipmentCmd2() : base(1)
		{
		}
	}
}
