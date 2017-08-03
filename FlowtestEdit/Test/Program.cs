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
            //Flow flow= Flow.CreateFlow();
            // flow.DataTest();


            Program p = new Program();
            p.GetSysPath();
            Console.WriteLine(p.GetSysPath());
 
            //var engine = Python.CreateEngine();
            //var paths = engine.GetSearchPaths();
            //paths.Add(@"c:\Python27\Lib\");
            //engine.SetSearchPaths(paths);

            //ScriptRuntime pyRuntime = Python.CreateRuntime(); //创建一下运行环境
            //dynamic obj = pyRuntime.UseFile("debug.py"); //调用一个Python文件

            ////obj.show();
            //int num1, num2;
            //Console.Write("Num1:");
            //num1 = Convert.ToInt32(Console.ReadLine());
            //Console.Write("Num2:");
            //num2 = Convert.ToInt32(Console.ReadLine());
            //int sum = obj.add(num1, num2); //调用Python文件中的求和函数
            //Console.Write("Sum:");
            //Console.WriteLine(sum);





            Console.Read();
        }


        public string GetSysPath(string srcFile = "debug.py")
        {
            ScriptEngine pyEngine = Python.CreateEngine();
            ScriptScope pyScope = pyEngine.CreateScope();
            dynamic py = pyEngine.ExecuteFile(srcFile);

            //int sum= py.add(3,5);
            //Console.WriteLine("sum="+sum);

            return py.get_sys_path();
        }

    }
}
