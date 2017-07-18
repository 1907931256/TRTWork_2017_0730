using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoResetEventTest
{

    class AlarmEventArgs : EventArgs
    {
        private int eventId_;

        public AlarmEventArgs(int eventId)
        {
            eventId_ = eventId;
        }

        public int EventId
        {
            get
            {
                return eventId_;
            }
        }
    }

        delegate void AlarmEventHandler(object sender, AlarmEventArgs e);
        class Alarm
        {
            public AlarmEventHandler AlarmEventHandler;

            public void Receive()
            {
                if (true)
                {
                    if (AlarmEventHandler != null)
                    {
                        AlarmEventHandler(this, new AlarmEventArgs(3));
                    }
                }

            }
        }

    class Program
    {
        void Handler(object sender, AlarmEventArgs e)
        {
            Console.WriteLine("{0}", e.EventId);
        }


        static void Main(string[] args)
        {
            Program program = new Program();
            Alarm alarm = new Alarm();

            alarm.AlarmEventHandler += new AlarmEventHandler(program.Handler);
            alarm.Receive();


            
            #region

            //Thread payMoneyThread = new Thread(new ThreadStart(payMoneyProc));//付款线程
            //payMoneyThread.Name = "付款线程";

            //Thread getBookThread = new Thread(new ThreadStart(GetBookProc));//买书线程
            //getBookThread.Name = "买书线程";

            //payMoneyThread.Start();
            //getBookThread.Start();

            //for (int i = 1; i <= numIterations; i++)
            //{
            //    Console.WriteLine("买书线程：数量{0}", i);
            //    number = i;
            //    //Signal that a value has been written.  
            //    myResetEvent.Set();
            //    //ChangeEvent.Set();  
            //    Thread.Sleep(10);
            //}
            //payMoneyThread.Abort();
            //getBookThread.Abort();
            #endregion

            #region

            //    int step_Speed = 0;
            //    int step_frac=0;
            //    int posAdd = 0;
            //    Console.WriteLine("step_frac={0}  step_Speed={1} posAdd={2}", step_frac, step_Speed, posAdd);
            //    Console.WriteLine("****************************进入循环********************************");
            //    Stopwatch sw = new Stopwatch();
            //    sw.Start();
            //    while (step_Speed<80000)
            //  {

            //      posAdd = 0;
            //      Console.WriteLine("posAdd 开始执行时候的值{0}",posAdd);
            //      step_frac += step_Speed;
            //      posAdd = step_frac >> 16;      
            //      step_frac -= posAdd << 16;
            //      Console.WriteLine("step_frac += step_Speed;step_frac={0}   posAdd = step_frac >> 16;posAdd={1}   step_frac -= posAdd << 16;step_frac={2}", step_frac, posAdd, step_frac);

            //         step_frac+=255000;
            //         Console.WriteLine(" step_frac+=255000  step_frac={0}",step_frac);

            //       posAdd=step_frac>>17;
            //        Console.WriteLine("posAdd=step_frac>>17 posAdd={0}",posAdd);

            //       step_frac-=posAdd;
            //      Console.WriteLine("step_frac-=posAdd; step_frac={0}",step_frac);

            //    if (posAdd>0)
            //    {
            //        step_Speed=step_Speed+posAdd;
            //        Console.WriteLine("posAdd>0中数据step_Spee={0}-->posAdd={1}",step_Speed,posAdd);
            //    }
            //    Console.WriteLine("********************************************************************************************");
            //}
            //    sw.Stop();
            //    Console.WriteLine("总共运行时间{0}", sw.ElapsedMilliseconds.ToString());   
            //    Console.ReadKey();
            //}

            //static void GetMax(int x, out int y)
            //{
            //    x = 10;
            //    y = 10;


            //}

            //static void payMoneyProc()
            //{
            //    while (true)
            //    {
            //        myResetEvent.WaitOne();
            //        Console.WriteLine("{0}:,数量{1}", Thread.CurrentThread.Name, number);
            //        changeRestEvent.Set();
            //    }
            //}

            //static void GetBookProc()
            //{
            //    while (true)
            //    {
            //        changeRestEvent.WaitOne();
            //        Console.WriteLine("{0}：数量{1}", Thread.CurrentThread.Name, number);
            //        Console.WriteLine("------------------------------------------");
            //    }

            //}
            #endregion
        }
    }
}
