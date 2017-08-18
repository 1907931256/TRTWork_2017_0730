using IntegrationSys.Flow;
using IntegrationSys.LibWrap;
using System;

namespace IntegrationSys.CommandUtils
{
	internal class RStechCmd : IExecutable
	{
		private static RStechCmd instance_;

		private string path_ = string.Empty;

		public static RStechCmd Instance
		{
			get
			{
				if (RStechCmd.instance_ == null)
				{
					RStechCmd.instance_ = new RStechCmd();
				}
				return RStechCmd.instance_;
			}
		}

		private RStechCmd()
		{
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if (action == "SetAppPath")
			{
				this.SetPath(param);
				retValue = "Res=Pass";
				return;
			}
			if (action == "TSEnd")
			{
				int num = TrustSystem.TSEnd(this.path_);
				retValue = "Res=" + num;
				return;
			}
			if (action == "Demo")
			{
				int num2 = TrustSystem.Demo(this.path_);
				retValue = "Res=" + num2;
				return;
			}
			if (action == "FLogin")
			{
				int num3 = (int)TrustSystem.FLogin(this.path_, param);
				retValue = "Res=" + num3;
				return;
			}
			if (action == "GetDataLen")
			{
				int dataLen = TrustSystem.GetDataLen(this.path_, param);
				retValue = "Res=" + dataLen;
				return;
			}
			if (action == "Goto")
			{
				int num4 = TrustSystem.Goto(this.path_, param);
				retValue = "Res=" + num4;
				return;
			}
			if (action == "LoadPro")
			{
				TrustSystem.LoadPro(this.path_, param);
				retValue = "Res=Pass";
				return;
			}
			if (action == "LoadProOver")
			{
				int num5 = TrustSystem.LoadProOver(this.path_);
				retValue = "Res=" + num5;
				return;
			}
			if (action == "Result")
			{
				int num6 = TrustSystem.Result(this.path_);
				retValue = "Res=" + num6;
				return;
			}
			if (action == "SN_Number")
			{
				TrustSystem.SN_Number(this.path_, param);
				retValue = "Res=Pass";
				return;
			}
			if (action == "TSCommand")
			{
				TrustSystem.TSCommand(this.path_, param);
				retValue = "Res=Pass";
				return;
			}
			if (action == "ShowTheWindow")
			{
				TrustSystem.ShowTheWindow(this.path_, param);
				retValue = "Res=Pass";
				return;
			}
			if (action == "Test")
			{
				retValue = "Res=-1.09";
				return;
			}
			retValue = "Res=CmdNotSupport";
		}

		private void SetPath(string path)
		{
			this.path_ = path;
		}
	}
}
