using IntegrationSys.LogUtil;
using System;
using System.Diagnostics;

namespace IntegrationSys.CommandLine
{
	internal class AdbCommand
	{
		public static bool InstallApk(string apkPath)
		{
			string param = "install -r " + apkPath;
			string text;
			bool flag = AdbCommand.ExecuteAdbCommand(param, 20000, out text);
			Log.Debug("install " + apkPath + " return " + text);
			return flag && text.Contains("Success");
		}

		public static bool UninstallApk(string packageName)
		{
			string param = "uninstall " + packageName;
			string text;
			return AdbCommand.ExecuteAdbCommand(param, out text);
		}

		public static bool InstallApkAndStart()
		{
			string text = "GeneralDev.apk";
			if (!AdbCommand.InstallApk(text))
			{
				Log.Debug("install " + text + " fail");
				return false;
			}
			Log.Debug("install " + text + " successful");
			string text2;
			AdbCommand.ExecuteAdbCommand("shell am start -n com.qwebob.generaldev/.GeneralDevActivity", out text2);
			if (text2.Contains("error") || text2.Contains("Error"))
			{
				Log.Debug("start " + text + " fail");
				return false;
			}
			Log.Debug("start " + text + " successful");
			return true;
		}

		public static bool UninstallApk()
		{
			return AdbCommand.UninstallApk("com.qwebob.generaldev");
		}

		public static bool ExecuteAdbCommand(string param)
		{
			if (!string.IsNullOrEmpty(param))
			{
				new Process
				{
					StartInfo = 
					{
						FileName = "adb.exe",
						Arguments = param,
						CreateNoWindow = true,
						UseShellExecute = false,
						RedirectStandardOutput = true
					}
				}.Start();
				return true;
			}
			return false;
		}

		public static bool ExecuteAdbCommand(string param, out string result)
		{
			if (!string.IsNullOrEmpty(param))
			{
				Process process = new Process();
				process.StartInfo.FileName = "adb.exe";
				process.StartInfo.Arguments = param;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.Start();
				result = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				return true;
			}
			result = "ArgumentNull";
			return false;
		}

		public static bool ExecuteAdbCommand(string param, int milliseconds, out string result)
		{
			if (string.IsNullOrEmpty(param))
			{
				result = "ArgumentNull";
				return false;
			}
			Process process = new Process();
			process.StartInfo.FileName = "adb.exe";
			process.StartInfo.Arguments = param;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.Start();
			result = process.StandardOutput.ReadToEnd();
			if (!process.WaitForExit(milliseconds))
			{
				process.Kill();
				return false;
			}
			return true;
		}
	}
}
