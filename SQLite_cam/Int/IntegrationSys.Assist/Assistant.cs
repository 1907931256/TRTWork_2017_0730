using IntegrationSys.CommandLine;
using System;

namespace IntegrationSys.Assist
{
	internal class Assistant
	{
		private const string ACTION_APK_INSTALL = "APK安装";

		private static Assistant instance_;

		public static Assistant Instance
		{
			get
			{
				if (Assistant.instance_ == null)
				{
					Assistant.instance_ = new Assistant();
				}
				return Assistant.instance_;
			}
		}

		private Assistant()
		{
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if (action == "APK安装")
			{
				this.ExecuteInstallCmd(param, out retValue);
				return;
			}
			retValue = "Res=CmdNotSupport";
		}

		private void ExecuteInstallCmd(string param, out string retValue)
		{
			retValue = (AdbCommand.InstallApkAndStart() ? "Res=Pass" : "Res=Fail");
		}
	}
}
