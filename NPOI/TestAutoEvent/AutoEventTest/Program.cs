using System;
using System.Threading;

namespace AutoEventTest
{
    class Program
    {

        /// <summary>
        /// 如果初始状态为终止状态，则为 true
        /// </summary>
        private AutoResetEvent testEvent=new AutoResetEvent(false);

        private int a = 1;
        static void Main(string[] args)
        {
            Program p = new Program();
            Thread thread_ = new Thread(() =>
            {
                p.TestSet();
            }
            );
            //thread_.Start();



            Console.WriteLine(p.testEvent.WaitOne().ToString());
                p.a = 1;
               
                Console.WriteLine("testEvent.WaitOne()");

                Console.Read();
        }

        private void TestSet()
        {
            while (true)
            {
                if (a==1)
                {
                    Thread.Sleep(500);
                    //testEvent.Set();
                    Console.WriteLine("testEvent.Set()");
                }
                a = 0;
            }        
        
        }

    }
}
