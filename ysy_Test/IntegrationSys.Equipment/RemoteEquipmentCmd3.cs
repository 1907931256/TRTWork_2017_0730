using System;

namespace IntegrationSys.Equipment
{
	internal class RemoteEquipmentCmd3 : RemoteEquipmentCmd
	{
		private static RemoteEquipmentCmd3 instance_;

		public static RemoteEquipmentCmd3 Instance
		{
			get
			{
				if (RemoteEquipmentCmd3.instance_ == null)
				{
					RemoteEquipmentCmd3.instance_ = new RemoteEquipmentCmd3();
				}
				return RemoteEquipmentCmd3.instance_;
			}
		}

		private RemoteEquipmentCmd3() : base(2)
		{
		}
	}
}
