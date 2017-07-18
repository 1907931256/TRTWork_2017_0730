using System;
using System.Diagnostics;
using GeneralTst.Log4Net;

namespace PhoneCmdUnit
{
    public class AdbCommand
    {

        /// <summary>
        /// 运行cmd命令
        /// 会显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
         public static bool RunCmd(string cmdExe, string cmdStr)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                    myPro.StartInfo = psi;
                    myPro.Start();
                    myPro.WaitForExit();
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }


        /// <summary>
        /// adb 命令
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
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
                result = string.Empty;
                while (process.StandardOutput.Peek() > 0)
                {
                    result += process.StandardOutput.ReadLine();

                }
               
                process.WaitForExit();
                process.Close();
                return true;
            }
            result = "ArgumentNull";
            return false;
        }





        /// <summary>
        /// adb 命令
        /// </summary>
        /// <param name="param"></param>
        /// <param name="milliseconds">等待指定的时间后结束进程</param>
        /// <param name="result"></param>
        /// <returns></returns>
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

        /// <summary>
        /// apk 安装
        /// </summary>
        /// <param name="apkPath"></param>
        /// <returns></returns>
        public static bool InstallApk(string apkPath)
        {
            string param = "install -r " + apkPath;
            string text;
            bool flag = AdbCommand.ExecuteAdbCommand(param, 20000, out text);
            Log.Debug("install " + apkPath + " return " + text);
            return flag && text.Contains("Success");
        }
        /// <summary>
        ///安装并启动
        /// </summary>
        /// <returns></returns>
        public static bool InstallApkAndStart()
        {
            //string text = "GeneralDev.apk";
            //if (!AdbCommand.InstallApk(text))
            //{
            //    return false;
            //}
            string text2;
            AdbCommand.ExecuteAdbCommand("shell am start -n com.qwebob.generaldev/.GeneralDevActivity", out text2);
            if (text2.Contains("error") || text2.Contains("Error"))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 卸载Apk
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static bool UninstallApk(string packageName)
        {
            string param = "uninstall " + packageName;
            string text;
            return AdbCommand.ExecuteAdbCommand(param, out text);
        }
        /// <summary>
        /// 卸载apk
        /// </summary>
        /// <returns></returns>
        public static bool UninstallApk()
        {
            return AdbCommand.UninstallApk("com.qwebob.generaldev");
        }

        /// <summary>
        /// 划屏幕指令
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        public static void Swipe(string param, out string retValue)
        {
            string str = param.Substring("swipe".Length + 1);
            string text;
            AdbCommand.ExecuteAdbCommand("shell input swipe " + str, out text);
            retValue = "Res=Pass";
        }
        /// <summary>
        /// 打开Wifi设置界面
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        public static void OpenWifiSettings(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand("shell am start -a android.settings.WIFI_SETTINGS", out text);
            string str = param.Substring("openwifiset".Length + 1);
            string param2 = "shell input tap " + str;
            AdbCommand.ExecuteAdbCommand(param2);
            retValue = "Res=Pass";
        }
        /// <summary>
        /// Gps设置界面
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        public static void CtrlGps(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand("shell am start -a android.settings.LOCATION_SOURCE_SETTINGS", out text);
            string str = param.Substring("ctrlGPS".Length + 1);
            string param2 = "shell input tap " + str;
            AdbCommand.ExecuteAdbCommand(param2);
            retValue = "Res=Pass";
        }

        /// <summary>
        /// 包名和类名获取
        /// </summary>
        /// <param name="retValue"></param>
        public static void GetFocusedActivity(out string retValue)
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

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        public static void TouchScreen(string param, out string retValue)
        {
            string str = param.Substring("touch".Length + 1);
            string text;
            AdbCommand.ExecuteAdbCommand("shell input tap " + str, out text);
            retValue = "Res=Pass";
        }


        /// <summary>
        /// SIM卡（adbcmd getval SIM1_prop_0=gsm.sim.state）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="retValue"></param>
        public static void GetProp(string name, int start, int length, out string retValue)
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
        public static void GetProp(string param, out string retValue)
        {
            string[] array = param.Substring("getprop".Length + 1).Split(new char[]
	            {
		            ' '
	            });
            if (array.Length == 1)
            {
                GetProp(array[0], 0, out retValue);
                return;
            }
            if (array.Length == 2)
            {
                GetProp(array[0], int.Parse(array[1]), out retValue);
                return;
            }
            GetProp(array[0], int.Parse(array[1]), int.Parse(array[2]), out retValue);
        }
        public static void GetProp(string name, int start, out string retValue)
        {
            GetProp(name, start, 2147483647, out retValue);
        }

        /// <summary>
        /// 截屏
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        public static void ScreenCapture(string param, out string retValue)
        {
            string str = param.Substring("snap".Length + 1);
            string text;
            AdbCommand.ExecuteAdbCommand("shell screencap " + str, out text);
            retValue = text + "\r\n                Res=Pass";
        }

        public static void Remove(string param, out string retValue)
        {
            string str = param.Substring("rm".Length + 1);
            string text;
            AdbCommand.ExecuteAdbCommand("shell rm " + str, out text);
            retValue = text + "\r\n                Res=Pass";
        }


        public static void ExecuteAdbShellCmd(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand("shell " + param, out text);
            retValue = text + "\r\n                Res=Pass";
        }

        public static void Push(string param, out string retValue)
        {
            string str = param.Substring("push".Length);
            string text;
            AdbCommand.ExecuteAdbCommand("push" + str, out text);
            retValue = text + "\r\n                Res=Pass";
        }

        public static void Pull(string param, out string retValue)
        {
            string str = param.Substring("pull".Length);
            string text;
            AdbCommand.ExecuteAdbCommand("pull" + str, out text);
            retValue = text + "\r\n                Res=Pass";
        }

        /// <summary>
        /// keyevent 
        /// adb shell input keyevent KeyCode_Home
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        public static void KeyEvrnt(string param,out string retValue)
        {
            string keyEventFile = System.Environment.CurrentDirectory + "\\KeyEvent.ini";
            string[] str_key = IniUnit.INIOperationClass.INIGetAllItems(keyEventFile, "Key");
            string val = string.Empty;
            if (param.IndexOf("key")!=-1)
            {
                foreach (var item in str_key)
                {
                    if (item.IndexOf("Key_Back") != -1 && param.IndexOf("Back") != -1)
                    {
                        val = item.Substring(item.IndexOf("=") + 1);
                        break;
                    }
                    else if (item.IndexOf("Key_Home") != -1 && param.IndexOf("Home") != -1)
                    {
                         val = item.Substring(item.IndexOf("=") + 1);
                        break;
                    }
                    else if (item.IndexOf("Key_Menu") != -1 && param.IndexOf("Menu") != -1)
                    {
                        val = item.Substring(item.IndexOf("=") + 1);
                        break;
                    }
                }   
            }
            AdbCommand.ExecuteAdbCommand("shell input keyevent " + val, out retValue);
            retValue = "Res=Pass";

        }

        public static void AdbWifiConnect(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param , out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void TcpIp(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void AdbDisconnect(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }

        public static void StartServer(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void KillServer(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void Devices(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void Install(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void Uninstall(string param, out string retValue)
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void GetSN(string param, out string retValue)//返回设备序列号SN值
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void Reboot(string param, out string retValue)//返回设备序列号SN值
        {
            string text;
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
        public static void LogCat(string param, out string retValue)//返回设备序列号SN值
        {
            //-v time qwebob >D:\logfile.txt
            string text;//+ @" -v time qwebob >D:\logfile.txt"
            AdbCommand.ExecuteAdbCommand(param, out text);
            retValue = text + "\r\n                Res=Pass";
        }
    }
}
