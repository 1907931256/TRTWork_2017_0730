using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using IntegrationSys.Equipment;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using System.Diagnostics;


//log4net写日志的必备条件
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace IntegrationSys
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetExceptionHandler();

            StartConnectPortThread();
            StartNetServer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        /// <summary>
        /// 启动串口查找线程
        /// </summary>
        private static void StartConnectPortThread()
        {
            Thread connectThread = new Thread(delegate()
                {
                    EquipmentCmd cmd = EquipmentCmd.Instance;
                    bool connected = cmd.ConnectPort();
                    Log.Debug("scan port thread " + (connected ? "succesful" : "fail"));
                });
            connectThread.IsBackground = true;
            connectThread.Start();
            Log.Debug("scan port thread start");
        }

        private static void StartLiteDataServer()
        {
            Thread liteDataServerThread = new Thread(delegate()
            {
                LiteDataServer.Instance.Start();
            });
            liteDataServerThread.IsBackground = true;
            liteDataServerThread.Start();
        }

        private static void StartFileTransferServer()
        {
            Thread fileTransferServerThread = new Thread(delegate()
                {
                    FileTransferServer.Instance.Start();
                });
            fileTransferServerThread.IsBackground = true;
            fileTransferServerThread.Start();
        }

        [Conditional("NDEBUG")]
        private static void StartNetServer()
        {
            StartLiteDataServer();
            StartFileTransferServer();
        }

        [Conditional("NDEBUG")]
        private static void SetExceptionHandler()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new ThreadExceptionEventHandler(Form1_UIThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private static void Form1_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            Log.Debug("UI thread exception", t.Exception);
            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowThreadExceptionDialog("Windows Forms Error", t.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort.

            if (result == DialogResult.Abort)
                Application.Exit();
        }


        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.Debug("Non-UI thread exception", ex);
            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowThreadExceptionDialog("Non-UI thread exception", ex);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Error",
                        "Fatal Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort.

            if (result == DialogResult.Abort)
                Application.Exit();
        }

        // Creates the error message and displays it.

        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string errorMsg = "An application error occurred. Please contact the adminstrator " +
                "with the following information:\n\n";
            errorMsg = errorMsg + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop);
        }


    }
}
