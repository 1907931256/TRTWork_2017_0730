using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using CommonPortCmd;
using TRTCamera;
namespace Test
{
    class Program
    {
       // private static readonly string str = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

        static void Main(string[] args)
        {
            //Common common_ = new Common();
            string str = string.Empty;
            //common_.ConnectPort(out str);
            commPortHelp comHelp = new commPortHelp();

            comHelp.ConnectPort(out str);

            //Console.WriteLine(str);
            //com.SendCommand("6站取放靠近", out str);
            //Console.WriteLine(str);
            ////com.SendCommand("6站运动", "23000 10000 20 255 255 65535", out str);
            //com.SendCommand("6站运动", "0 0 20 255 255 65535", out str);
            ////Console.WriteLine(str);
            //com.SendCommand("1站USB插入", out str);
            //Console.WriteLine(str);
            //com.SendCommand("1站前后门开门", out str);
            //Console.WriteLine(str);
           
           //CmdDAl dal=new CmdDAl();
           // CmdInfo cmd=dal.CmdByCmdName_DAL("6站电机X位置检测");

           // string strHex = cmd.Start + cmd.Length + cmd.Adress + cmd.Model + cmd.Port + cmd.StrPram + cmd.End;
           // Console.WriteLine(strHex);
            Console.ReadKey();

        }
    }
}
