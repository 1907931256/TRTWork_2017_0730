using IntegrationSys.Equipment;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using System;
using System.Threading;
using System.Windows.Forms;

namespace IntegrationSys
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Program.StartConnectPortThread();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}

		private static void StartConnectPortThread()
		{
            Thread thread_ = new Thread(Connect_);
		    thread_.IsBackground=true;
            thread_.Start();
            Log.Debug("scan port thread start");
		}
        private static void Connect_()
        {
                EquipmentCmd instance = EquipmentCmd.Instance;
				bool flag = instance.ConnectPort();
				Log.Debug("scan port thread " + (flag ? "succesful" : "fail"));
        }
		private static void StartLiteDataServer()
		{
            //Thread thread__ = new Thread(() => LiteDataServer.Instance.Start());

            Thread thread_ = new Thread(DoSomthing);
            thread_.IsBackground = true;
            thread_.Start();
		}
        private static void DoSomthing()
        {
            LiteDataServer.Instance.Start();
        }
	}
}
