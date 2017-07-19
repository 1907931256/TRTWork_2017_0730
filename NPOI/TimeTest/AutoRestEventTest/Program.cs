using System;
using System.Threading;


namespace AutoRestEventTest
{
    class Program
    {
        AutoResetEvent testEvent = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            Program p = new Program();

            Thread thread_ = new Thread(() =>
            {
                p.Test_();
            }
            );

            while (true)
            {
                Console.WriteLine("Test start!");
                p.testEvent.WaitOne();
            }


        }
        private void Test_()
        {
            while (true)
            {
                string read = Console.ReadLine();
                if (read == "11")
                {
                    testEvent.Set();
                    Console.WriteLine("testEvent.Se");
                }
            }
            
        }

    }
}
