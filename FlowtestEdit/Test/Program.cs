using FlowtestEdit;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
           Flow flow= Flow.CreateFlow();
            flow.DataTest();

            Console.Read();
        }
    }
}
