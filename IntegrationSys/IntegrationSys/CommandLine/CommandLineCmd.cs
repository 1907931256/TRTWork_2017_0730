using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;

namespace IntegrationSys.CommandLine
{
    class CommandLineCmd : IExecutable
    {
        const string ACTION_ADBCMD = "ADB命令";

        private static CommandLineCmd instance_;

        private CommandLineCmd()
        { 
        }

        public static CommandLineCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new CommandLineCmd();
                }

                return instance_;
            }
        }

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (action == ACTION_ADBCMD)
            {
                if (param == "getFocusedActivity")
                {
                    GetFocusedActivity(out retValue);
                }
                else if (param == "devices")
                {
                    Devices(out retValue);
                }
                else if (param.StartsWith("openwifiset"))
                {
                    OpenWifiSettings(param, out retValue);
                }
                else if (param.StartsWith("ctrlGPS"))
                {
                    CtrlGps(param, out retValue);
                }
                else if (param.StartsWith("getprop"))
                {
                    GetProp(param, out retValue);
                }
                else if (param.StartsWith("swipe"))
                {
                    Swipe(param, out retValue);
                }
                else if (param.StartsWith("touch"))
                {
                    TouchScreen(param, out retValue);
                }
                else if (param.StartsWith("snap"))
                {
                    ScreenCapture(param, out retValue);
                }
                else if (param.StartsWith("push"))
                {
                    Push(param, out retValue);
                }
                else if (param.StartsWith("pull"))
                {
                    Pull(param, out retValue);
                }
                else
                {
                    ExecuteAdbShellCmd(param, out retValue);
                }
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }
        }

        private void ExecuteAdbShellCmd(string param, out string retValue)
        {
            string adbResult;
            AdbCommand.ExecuteAdbCommand("shell " + param, out adbResult);
            retValue = "Res=Pass";
        }


        private void GetFocusedActivity(out string retValue)
        {
            string adbResult;

            AdbCommand.ExecuteAdbCommand("shell dumpsys activity | grep mResumedActivity", out adbResult);

            int pos = adbResult.IndexOf("/.");

            if (pos != -1)
            {
                string firstHalf = adbResult.Substring(0, pos);
                string lastHalf = adbResult.Substring(pos);

                int begin = firstHalf.LastIndexOf(' ') + 1;
                int end = lastHalf.IndexOf(' ') + pos;

                retValue = "Res=" + adbResult.Substring(begin, end - begin);
            }
            else
            {
                retValue = "Res=ActivityNotFound";
            }
        }

        private void Devices(out string retValue)
        {
            string adbResult;
            AdbCommand.ExecuteAdbCommand("get-state", out adbResult);
            retValue = "Res=" + adbResult.Trim();
        }

        /// <summary>
        /// start android.settings.WIFI_SETTINGS activity, then touch wifi switch button
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="retValue"></param>
        private void OpenWifiSettings(string param, out string retValue)
        {
            string adbResult;
            AdbCommand.ExecuteAdbCommand("shell am start -a android.settings.WIFI_SETTINGS", out adbResult);

            string locationXY = param.Substring("openwifiset".Length + 1);
            string command = "shell input tap " + locationXY;
            AdbCommand.ExecuteAdbCommand(command);
            retValue = "Res=Pass";
        }

        /// <summary>
        /// start android.settings.LOCATION_SOURCE_SETTINGS activity, then touch gps switch button
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        private void CtrlGps(string param, out string retValue)
        {
            string adbResult;
            AdbCommand.ExecuteAdbCommand("shell am start -a android.settings.LOCATION_SOURCE_SETTINGS", out adbResult);

            string locationXY = param.Substring("ctrlGPS".Length + 1);
            string command = "shell input tap " + locationXY;
            AdbCommand.ExecuteAdbCommand(command);
            retValue = "Res=Pass";
        }

        private void GetProp(string param, out string retValue)
        {
            string[] paramArray = param.Substring("getprop".Length + 1).Split(' ');
            if (paramArray.Length == 1)
            {
                GetProp(paramArray[0], 0, out retValue);
            }
            else if (paramArray.Length == 2)
            {
                GetProp(paramArray[0], int.Parse(paramArray[1]), out retValue);
            }
            else
            {
                GetProp(paramArray[0], int.Parse(paramArray[1]), int.Parse(paramArray[2]), out retValue);
            }
        }

        private void GetProp(string name, int start, int length, out string retValue)
        {
            string adbResult;
            AdbCommand.ExecuteAdbCommand("shell getprop " + name, out adbResult);

            if (start >= adbResult.Length)
            {
                retValue = "Res=ArgumentException";
            }
            else
            {
                length = Math.Min(length, adbResult.Length - start);
                retValue = "Res=" + adbResult.Substring(start, length);
            }
        }

        private void GetProp(string name, int start, out string retValue)
        {
            GetProp(name, start, int.MaxValue, out retValue);
        }

        /// <summary>
        /// execute adb shell input swipe x1 y1 x2 y2
        /// </summary>
        /// <param name="param"></param>
        /// <param name="retValue"></param>
        private void Swipe(string param, out string retValue)
        {
            string swipeParam = param.Substring("swipe".Length + 1);
            string adbResult;
            AdbCommand.ExecuteAdbCommand("shell input swipe " + swipeParam, out adbResult);
            retValue = "Res=Pass";
        }

        /// <summary>
        /// execute adb shell input tap x y
        /// </summary>
        private void TouchScreen(string param, out string retValue)
        {
            string tapParam = param.Substring("touch".Length + 1);
            string adbResult;
            AdbCommand.ExecuteAdbCommand("shell input tap " + tapParam, out adbResult);
            retValue = "Res=Pass";
        }

        private void ScreenCapture(string param, out string retValue)
        {
            string filename = param.Substring("snap".Length + 1);
            string adbResult;
            AdbCommand.ExecuteAdbCommand("shell screencap " + filename, out adbResult);
            retValue = "Res=Pass";
        }

        private void Push(string param, out string retValue)
        {
            string pushParam = param.Substring("push".Length);
            string adbResult;
            AdbCommand.ExecuteAdbCommand("push" + pushParam, out adbResult);
            retValue = "Res=Pass";
        }

        private void Pull(string param, out string retValue)
        {
            string pullParam = param.Substring("pull".Length);
            string adbResult;
            AdbCommand.ExecuteAdbCommand("pull" + pullParam, out adbResult);
            retValue = "Res=Pass";
        }
    }
}
