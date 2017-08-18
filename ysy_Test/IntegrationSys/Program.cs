using IntegrationSys.Equipment;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;



[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace IntegrationSys
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
            Log.Debug("scan port thread start");



            //Program.SetExceptionHandler();
            //Program.StartConnectPortThread();
            //Program.StartNetServer();
            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}

		private static void StartConnectPortThread()
		{
            Thread thread_ = new Thread(() =>
              {
                  EquipmentCmd instance = EquipmentCmd.Instance;
                  bool flag = instance.ConnectPort();
                  Log.Debug("scan port thread " + (flag ? "succesful" : "fail"));
              });
            thread_.IsBackground = true;
            thread_.Start();
        }

        private static void StartLiteDataServer()
        {
            Thread thread_ = new Thread(() =>
            {
                LiteDataServer.Instance.Start();
            });
             thread_.IsBackground = true;
            thread_.Start();
        }

        private static void StartFileTransferServer()
        {
            Thread thread_ = new Thread(() =>
            {
                FileTransferServer.Instance.Start();
            });
            thread_.IsBackground = true;
            thread_.Start();
        }

        [Conditional("NDEBUG")]
        private static void StartNetServer()
        {
            Program.StartLiteDataServer();
            Program.StartFileTransferServer();
        }

        [Conditional("NDEBUG")]
        private static void SetExceptionHandler()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new ThreadExceptionEventHandler(Program.Form1_UIThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
        }

        private static void Form1_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            Log.Debug("UI thread exception", t.Exception);
            DialogResult dialogResult = DialogResult.Cancel;
            try
            {
                dialogResult = Program.ShowThreadExceptionDialog("Windows Forms Error", t.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error", "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Hand);
                }
                finally
                {
                    Application.Exit();
                }
            }
            if (dialogResult == DialogResult.Abort)
            {
                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception e2 = (Exception)e.ExceptionObject;
            Log.Debug("Non-UI thread exception", e2);
            DialogResult dialogResult = DialogResult.Cancel;
            try
            {
                dialogResult = Program.ShowThreadExceptionDialog("Non-UI thread exception", e2);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Error", "Fatal Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Hand);
                }
                finally
                {
                    Application.Exit();
                }
            }
            if (dialogResult == DialogResult.Abort)
            {
                Application.Exit();
            }
        }

        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string text = "An application error occurred. Please contact the adminstrator with the following information:\n\n";
            text = text + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(text, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Hand);
        }
    }
}
