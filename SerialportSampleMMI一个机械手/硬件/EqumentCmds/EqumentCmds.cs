using CommonPortCmd;
using ComPort;
using System.Threading;

namespace EqumentCmds
{
    internal class EquipmentCmd
    {
        public delegate void ReportEventHandler(ActiveEnumData eventId);

        /// <summary>
        /// 串口接收到的数据
        /// </summary>
        public byte[] resPort;

        private static EquipmentCmd instance_;

        private Common comport_;

        private bool connected_;

        private ManualResetEvent manualEvent_;

        /// <summary>
        /// 主动上报部分
        /// </summary>
        public event EquipmentCmd.ReportEventHandler ReportEvent;

        public static EquipmentCmd Instance
        {
            get
            {
                if (EquipmentCmd.instance_ == null)
                {
                    EquipmentCmd.instance_ = new EquipmentCmd();
                }
                return EquipmentCmd.instance_;
            }
        }

        private EquipmentCmd()
        {
            this.comport_ = new Common();
            //this.comport_.add_RecDataSendEventHander(new Common.RecDataSend(this.EventHandler));
            comport_.RecDataSendEventHander += Comport__RecDataSendEventHander;
            this.manualEvent_ = new ManualResetEvent(false);
            //ReportEvent += EquipmentCmd_ReportEvent;
        }

        //private void EquipmentCmd_ReportEvent(ActiveEnumData eventId)
        //{
        //    string s = eventId.ToString();
        //}

        private void Comport__RecDataSendEventHander(object send, ActiveReporting e)
        {
            if (this.ReportEvent != null)
            {
                this.ReportEvent(e.EventId);
                
            }
        }
        public bool ConnectPort()
        {
            if (!this.connected_ && this.comport_.ConnectPort())
            {
                this.connected_ = true;
                this.manualEvent_.Set();
            }
            return this.connected_;
        }

        public bool SendCommand(string cmd, string param, out string resp)
        {
            bool b;
            resp = null;
            if (!this.connected_)
            {
                this.manualEvent_.WaitOne();
            }
            if (string.IsNullOrEmpty(param))
            {
                b=this.comport_.SendCommand(cmd, out resp);
                resPort = comport_.resp;
                return b;
            }
                    
            b= this.comport_.SendCommand(cmd, param, out resp);
            resPort = comport_.resp;
            return b;



        }

    }
}
