using System;
using System.Windows.Forms;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Log4Net
{
    [System.Security.SecuritySafeCritical]

    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            for (int i = 0; i < 10; i++)
            {
                log.Debug("Application.EnableVisualStyles() ");
            }


            MessageBox.Show("加载完成");

            Console.Read();
        }
    }
}
