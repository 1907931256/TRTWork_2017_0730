using IntegrationSys.Flow;
using System;

namespace IntegrationSys.CommandLine
{
	internal class CommandLineCmd : IExecutable
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
			if (param.StartsWith("openwifiset"))
			{
				this.OpenWifiSettings(param, out retValue);
				return;
			}
			if (param.StartsWith("ctrlGPS"))
			{
				this.CtrlGps(param, out retValue);
				return;
			}
			if (param.StartsWith("getprop"))
			{
				this.GetProp(param, out retValue);
				return;
			}
			if (param.StartsWith("swipe"))
			{
				this.Swipe(param, out retValue);
				return;
			}
			if (param.StartsWith("touch"))
			{
				this.TouchScreen(param, out retValue);
				return;
			}
			if (param.StartsWith("snap"))
			{
				this.ScreenCapture(param, out retValue);
				return;
			}
			if (param.StartsWith("push"))
			{
				this.Push(param, out retValue);
				return;
			}
			if (param.StartsWith("pull"))
			{
				this.Pull(param, out retValue);
				return;
			}
			this.ExecuteAdbShellCmd(param, out retValue);
		}

		private void ExecuteAdbShellCmd(string param, out string retValue)
		{
			string text;
			AdbCommand.ExecuteAdbCommand("shell " + param, out text);
			retValue = "Res=Pass";
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

		private void OpenWifiSettings(string param, out string retValue)
		{
			string text;
			AdbCommand.ExecuteAdbCommand("shell am start -a android.settings.WIFI_SETTINGS", out text);
			string str = param.Substring("openwifiset".Length + 1);
			string param2 = "shell input tap " + str;
			AdbCommand.ExecuteAdbCommand(param2);
			retValue = "Res=Pass";
		}

		private void CtrlGps(string param, out string retValue)
		{
			string text;
			AdbCommand.ExecuteAdbCommand("shell am start -a android.settings.LOCATION_SOURCE_SETTINGS", out text);
			string str = param.Substring("ctrlGPS".Length + 1);
			string param2 = "shell input tap " + str;
			AdbCommand.ExecuteAdbCommand(param2);
			retValue = "Res=Pass";
		}

		private void GetProp(string param, out string retValue)
		{
			string[] array = param.Substring("getprop".Length + 1).Split(new char[]
			{
				' '
			});
			if (array.Length == 1)
			{
				this.GetProp(array[0], 0, out retValue);
				return;
			}
			if (array.Length == 2)
			{
				this.GetProp(array[0], int.Parse(array[1]), out retValue);
				return;
			}
			this.GetProp(array[0], int.Parse(array[1]), int.Parse(array[2]), out retValue);
		}

		private void GetProp(string name, int start, int length, out string retValue)
		{
			string text;
			AdbCommand.ExecuteAdbCommand("shell getprop " + name, out text);
			if (start >= text.Length)
			{
				retValue = "Res=ArgumentException";
				return;
			}
			length = Math.Min(length, text.Length - start);
			retValue = "Res=" + text.Substring(start, length);
		}

		private void GetProp(string name, int start, out string retValue)
		{
			this.GetProp(name, start, 2147483647, out retValue);
		}

		private void Swipe(string param, out string retValue)
		{
			string str = param.Substring("swipe".Length + 1);
			string text;
			AdbCommand.ExecuteAdbCommand("shell input swipe " + str, out text);
			retValue = "Res=Pass";
		}

		private void TouchScreen(string param, out string retValue)
		{
			string str = param.Substring("touch".Length + 1);
			string text;
			AdbCommand.ExecuteAdbCommand("shell input tap " + str, out text);
			retValue = "Res=Pass";
		}

		private void ScreenCapture(string param, out string retValue)
		{
			string str = param.Substring("snap".Length + 1);
			string text;
			AdbCommand.ExecuteAdbCommand("shell screencap " + str, out text);
			retValue = "Res=Pass";
		}

		private void Push(string param, out string retValue)
		{
			string str = param.Substring("push".Length);
			string text;
			AdbCommand.ExecuteAdbCommand("push" + str, out text);
			retValue = "Res=Pass";
		}

		private void Pull(string param, out string retValue)
		{
			string str = param.Substring("pull".Length);
			string text;
			AdbCommand.ExecuteAdbCommand("pull" + str, out text);
			retValue = "Res=Pass";
		}
	}
}
