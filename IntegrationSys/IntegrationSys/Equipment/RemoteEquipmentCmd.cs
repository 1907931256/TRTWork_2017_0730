using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;
using IntegrationSys.Net;

namespace IntegrationSys.Equipment
{
    class RemoteEquipmentCmd : IExecutable
    {
        private int station_;

        public RemoteEquipmentCmd(int index)
        {
            station_ = index;
        }



        public void ExecuteCmd(string action, string param, out string retValue)
        {
            retValue = LiteDataClient.Instance.SendEquipmentCmd(station_, action, param);
        }
    }

    class RemoteEquipmentCmd1 : RemoteEquipmentCmd
    {
        private static RemoteEquipmentCmd1 instance_;

        public static RemoteEquipmentCmd1 Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RemoteEquipmentCmd1();
                }
                return instance_;
            }
        }
        private RemoteEquipmentCmd1() : base(0) { }
    }

    class RemoteEquipmentCmd2 : RemoteEquipmentCmd
    {
        private static RemoteEquipmentCmd2 instance_;

        public static RemoteEquipmentCmd2 Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RemoteEquipmentCmd2();
                }
                return instance_;
            }
        }
        private RemoteEquipmentCmd2() : base(1) { }
    }

    class RemoteEquipmentCmd3 : RemoteEquipmentCmd
    {
        private static RemoteEquipmentCmd3 instance_;

        public static RemoteEquipmentCmd3 Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RemoteEquipmentCmd3();
                }
                return instance_;
            }
        }
        private RemoteEquipmentCmd3() : base(2) { }
    }

    class RemoteEquipmentCmd4 : RemoteEquipmentCmd
    {
        private static RemoteEquipmentCmd4 instance_;

        public static RemoteEquipmentCmd4 Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RemoteEquipmentCmd4();
                }
                return instance_;
            }
        }
        private RemoteEquipmentCmd4() : base(3) { }
    }

    class RemoteEquipmentCmd5 : RemoteEquipmentCmd
    {
        private static RemoteEquipmentCmd5 instance_;

        public static RemoteEquipmentCmd5 Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RemoteEquipmentCmd5();
                }
                return instance_;
            }
        }
        private RemoteEquipmentCmd5() : base(4) { }
    }

    class RemoteEquipmentCmd6 : RemoteEquipmentCmd
    {
        private static RemoteEquipmentCmd6 instance_;

        public static RemoteEquipmentCmd6 Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RemoteEquipmentCmd6();
                }
                return instance_;
            }
        }
        private RemoteEquipmentCmd6() : base(5) { }
    }
}
