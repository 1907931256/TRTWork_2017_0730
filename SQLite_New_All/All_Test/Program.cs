using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonPortCmd;

namespace All_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Common com = new Common();
            com.ConnectPort();


            string str=string.Empty;
            com.SendCommand("1站取放", "1", out str);

            com.RecDataSendEventHander += com_RecDataSendEventHander;


            Console.Read();

        }

        static void com_RecDataSendEventHander(object send, ActiveReporting e)
        {
            Console.WriteLine(e.EventId.ToString());
        }
    }
}
