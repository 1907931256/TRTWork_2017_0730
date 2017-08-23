using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using IntegrationSys.LogUtil;

namespace IntegrationSys.CommandLine
{
    class AdbCommand
    {
        /// <summary>
        /// 安装apk
        /// </summary>
        /// <param name="apkPath"></param>
        /// <returns></returns>
        public static bool InstallApk(string apkPath)
        {
            string param = "install -r " + apkPath;
            string result;
            bool ret = ExecuteAdbCommand(param, 20000, out result);
            Log.Debug("install " + apkPath + " return " + result);
            if (!ret) return false;

            if (!result.Contains("Success")) return false;
            return true;
        }

        public static bool UninstallApk(string packageName)
        {
            string param = "uninstall " + packageName;

            string result;
            return ExecuteAdbCommand(param, out result);
        }

        /// <summary>
        /// 安装GeneralDev.apk并启动
        /// </summary>
        /// <returns></returns>
        public static bool InstallApkAndStart()
        { 
            string apkPath = "GeneralDev.apk";
            if (!InstallApk(apkPath))
            {
                Log.Debug("install " + apkPath + " fail");
                return false;
            }
            Log.Debug("install " + apkPath + " successful");
            string result;
            ExecuteAdbCommand("shell am start -n com.qwebob.generaldev/.GeneralDevActivity", out result);

            if (result.Contains("error") || result.Contains("Error"))
            {
                Log.Debug("start " + apkPath + " fail");
                return false;
            }

            Log.Debug("start " + apkPath + " successful");
            return true;
        }

        public static bool UninstallApk()
        {
            return UninstallApk("com.qwebob.generaldev");
        }

        /// <summary>
        /// 执行adb命令,不用等待adb进程执行完成就返回
        /// </summary>
        /// <param name="param"></param>
        public static bool ExecuteAdbCommand(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {
                Process adbProcess = new Process();
                adbProcess.StartInfo.FileName = "adb.exe";
                adbProcess.StartInfo.Arguments = param;
                adbProcess.StartInfo.CreateNoWindow = true;
                adbProcess.StartInfo.UseShellExecute = false;
                adbProcess.StartInfo.RedirectStandardOutput = true;
                adbProcess.Start();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 执行adb命令，并等待命令执行完成后获取结果
        /// </summary>
        /// <param name="param"></param>
        /// <param name="result"></param>
        public static bool ExecuteAdbCommand(string param, out string result)
        {
            if (!string.IsNullOrEmpty(param))
            {
                Process adbProcess = new Process();
                adbProcess.StartInfo.FileName = "adb.exe";
                adbProcess.StartInfo.Arguments = param;
                adbProcess.StartInfo.CreateNoWindow = true;
                adbProcess.StartInfo.UseShellExecute = false;
                adbProcess.StartInfo.RedirectStandardOutput = true;
                adbProcess.Start();

                result = string.Empty;
                while (adbProcess.StandardOutput.Peek() > 0)
                {
                    result += adbProcess.StandardOutput.ReadLine();
                }
                adbProcess.WaitForExit();

                return true;
            }
            else
            {
                result = "ArgumentNull";
            }
            return false;
        }

        /// <summary>
        /// 执行adb命令，并等待（有超时）命令执行结束后获取结果
        /// </summary>
        /// <param name="param"></param>
        /// <param name="milliseconds">超时</param>
        /// <param name="result"></param>
        public static bool ExecuteAdbCommand(string param, int milliseconds, out string result)
        {
            if (!string.IsNullOrEmpty(param))
            {
                Process adbProcess = new Process();
                adbProcess.StartInfo.FileName = "adb.exe";
                adbProcess.StartInfo.Arguments = param;
                adbProcess.StartInfo.CreateNoWindow = true;
                adbProcess.StartInfo.UseShellExecute = false;
                adbProcess.StartInfo.RedirectStandardOutput = true;
                adbProcess.Start();

                result = adbProcess.StandardOutput.ReadToEnd();
                if (!adbProcess.WaitForExit(milliseconds))
                {
                    adbProcess.Kill();
                    return false;
                }
                return true;
            }
            else
            {
                result = "ArgumentNull";
            }
            return false;
        }
    }
}
