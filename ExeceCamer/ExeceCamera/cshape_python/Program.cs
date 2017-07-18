using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;

namespace cshape_python
{
    class Program
    {
        static void Main(string[] args)
        {
            //第一句代码创建了一个Python的运行环境，第二句则使用.net4.0的语法创建了一个动态的对象， OK，
            //下面就可以用这个dynamic类型的对象去调用刚才在定义的welcome方法了。
            ScriptRuntime pyRuntime = Python.CreateRuntime();
            dynamic obj = pyRuntime.UseFile("hello.py");

            Console.WriteLine(obj.welecom("nICK"));
            Console.WriteLine(obj.add(10,8));
            Console.ReadKey();
        }
    }
}
