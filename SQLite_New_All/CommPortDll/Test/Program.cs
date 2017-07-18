using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PortCmdDAL;


namespace Test
{
    class Program
    {
          //private static AutoResetEvent MyEvent = new AutoResetEvent(false);
        static void Main(string[] args)
        {


            //Common com = new Common();
            //com.str_Port_Rec = "0F 00";
            //bool b = com.ConnectPort();
            //string str = "OK";
            //Console.WriteLine(b.ToString());

            //com.SendCommand("6站电机X位置检测", out str);

            CmdDAl cmdDal = new CmdDAl();
            CmdInfo cmd = cmdDal.CmdByCmdName_DAL("6站电机X位置检测");
            string strHex = cmd.Start + cmd.Length + cmd.Adress + cmd.Model + cmd.Port + cmd.StrPram + cmd.End;
            Console.WriteLine(strHex);
            Console.Read();
            //while (true)
            //{
            //    com.SendCommand("1站取放", "31", out str);
            //    Thread.Sleep(5000);
            //}
          
           

          //  Console.WriteLine(str);


          //  Thread thread = new Thread(Dosthing);

          //  thread.Start();
          //bool b=  MyEvent.WaitOne(1000);

          //  if (b)
          //  {
          //      Console.WriteLine("主要线程");
          //  }

          //  Console.WriteLine("完成");
          //  Console.ReadKey();
        }

        //private static void Dosthing()
        //{
        //   Thread.Sleep(2000);
        //    Console.WriteLine("000000000");
        //    MyEvent.Set();
        //}
       
    }
}
