﻿using EqumentCmds;
using System;
using System.Threading;
using System.Windows.Forms;

using CommonClass;

namespace Station
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CAMFrom.Form1());
            //必须放在窗体运行之后，握手才能正常握手
            StartConnectPortThread();

        }

        private static void StartConnectPortThread()
        {
            Thread thread_ = new Thread(() =>
            {
                EquipmentCmd instance = EquipmentCmd.Instance;
                bool flag = instance.ConnectPort();
            }
            );
            thread_.IsBackground = true;
            thread_.Start();
        }

    }
}
