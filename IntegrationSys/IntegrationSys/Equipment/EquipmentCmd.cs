using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonPortCmd;
using System.Threading;

namespace IntegrationSys.Equipment
{
    class EquipmentCmd
    {
        private static EquipmentCmd instance_ = null;
        private Common comport_;
        private bool connected_;
        private ManualResetEvent manualEvent_;

        public delegate void ReportEventHandler(ActiveEnumData eventId);

        public event ReportEventHandler ReportEvent; 

        private EquipmentCmd()
        {
            comport_ = new Common();
            comport_.RecDataSendEventHander += new Common.RecDataSend(EventHandler);

            manualEvent_ = new ManualResetEvent(false);
        }

        public static EquipmentCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new EquipmentCmd();
                }
                return instance_;
            }
        }

        public bool ConnectPort()
        {
            if (!connected_)
            {
                if (comport_.ConnectPort())
                {
                    connected_ = true;
                    manualEvent_.Set();
                }
            }

            return connected_;
        }

        public bool SendCommand(string cmd, string param, out string resp)
        {
            if (!connected_)
            {
                manualEvent_.WaitOne();
            }

            if (string.IsNullOrEmpty(param))
            {
                return comport_.SendCommand(cmd, out resp);
            }
            else
            {
                return comport_.SendCommand(cmd, param, out resp);
            }
        }

        private void EventHandler(object send, CommonPortCmd.ActiveReporting e)
        {
            if (ReportEvent != null)
            {
                ReportEvent(e.EventId);
            }
        }
    }
}
