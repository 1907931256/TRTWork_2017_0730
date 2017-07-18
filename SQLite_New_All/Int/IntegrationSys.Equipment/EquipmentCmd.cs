using CommonPortCmd;
using System;
using System.Threading;

namespace IntegrationSys.Equipment
{
	internal class EquipmentCmd
	{
		public delegate void ReportEventHandler(ActiveEnumData eventId);

		private static EquipmentCmd instance_;

		private Common comport_;

		private bool connected_;

		private ManualResetEvent manualEvent_;

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
			this.comport_.RecDataSendEventHander += new Common.RecDataSend(this.EventHandler);
			this.manualEvent_ = new ManualResetEvent(false);
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
			if (!this.connected_)
			{
				this.manualEvent_.WaitOne();
			}
			return this.comport_.SendCommand(cmd, param, out resp);
		}

		public bool SendCommand(string cmd, out string resp)
		{
			if (!this.connected_)
			{
				this.manualEvent_.WaitOne();
			}
			return this.comport_.SendCommand(cmd, out resp);
		}

		private void EventHandler(object send, ActiveReporting e)
		{
			if (this.ReportEvent != null)
			{
				this.ReportEvent(e.EventId);
			}
		}
	}
}
