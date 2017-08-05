using EqumentCmds;
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
            Application.Run(new CAMFrom.Form3());
            //必须放在窗体运行之后，握手才能正常握手
            

        }

       

    }
}
