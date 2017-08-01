using FlowtestEdit;
using System;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Flow flow= Flow.CreateFlow();
            // flow.DataTest();

            ScriptRuntime pyRuntime = Python.CreateRuntime(); //创建一下运行环境
            dynamic obj = pyRuntime.UseFile("debug.py"); //调用一个Python文件
            int num1, num2;
            Console.Write("Num1:");
            num1 = Convert.ToInt32(Console.ReadLine());
            Console.Write("Num2:");
            num2 = Convert.ToInt32(Console.ReadLine());
            int sum = obj.add(num1, num2); //调用Python文件中的求和函数
            Console.Write("Sum:");
            Console.WriteLine(sum);





            Console.Read();
        }
    }
}
