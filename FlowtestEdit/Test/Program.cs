using FlowtestEdit;
using System;
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using IronPython.Runtime;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Flow flow = Flow.CreateFlow();
            flow.DataTest();




            Console.Read();
        }



    }
}
