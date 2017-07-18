using System;

namespace IntegrationSys.CommandLine
{
	internal class CommandLineCmd
	{
		private const string ACTION_ADBCMD = "ADB命令";

		private static CommandLineCmd instance_;

		public static CommandLineCmd Instance
		{
			get
			{
				if (CommandLineCmd.instance_ == null)
				{
					CommandLineCmd.instance_ = new CommandLineCmd();
				}
				return CommandLineCmd.instance_;
			}
		}

		private CommandLineCmd()
		{
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if (!(action == "ADB命令"))
			{
				retValue = "Res=CmdNotSupport";
				return;
			}
			if (param == "getFocusedActivity")
			{
				this.GetFocusedActivity(out retValue);
				return;
			}
			if (param == "devices")
			{
				this.Devices(out retValue);
				return;
			}
			retValue = "Res=CmdNotSupport";
		}

		private void GetFocusedActivity(out string retValue)
		{
			string text;
			AdbCommand.ExecuteAdbCommand("shell dumpsys activity | grep mResumedActivity", out text);
			int num = text.IndexOf("/.");
			if (num != -1)
			{
				string text2 = text.Substring(0, num);
				string text3 = text.Substring(num);
				int num2 = text2.LastIndexOf(' ') + 1;
				int num3 = text3.IndexOf(' ') + num;
				retValue = "Res=" + text.Substring(num2, num3 - num2);
				return;
			}
			retValue = "Res=ActivityNotFound";
		}

		private void Devices(out string retValue)
		{
			string text;
			AdbCommand.ExecuteAdbCommand("get-state", out text);
			retValue = "Res=" + text.Trim();
		}
	}
}
