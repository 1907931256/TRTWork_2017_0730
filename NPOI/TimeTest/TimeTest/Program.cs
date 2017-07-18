using System.IO;
using System.Text;
using System.Diagnostics;
using System;
using NPOI.XSSF.UserModel;

namespace TimeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            XSSFWorkbook wk = Read(@"e:\TRTWork\NPOI\2017.xlsx");

            Write(@"e:\TRTWork\NPOI\2017_1.xlsx", wk);


            sw.Stop();
            long time = sw.ElapsedMilliseconds;
            Console.WriteLine("运行耗时=" + time.ToString());

            Console.Read();


        }

        private static XSSFWorkbook Read(string filePath)
        {
            StringBuilder sbr = new StringBuilder();
            XSSFWorkbook wk;
            using (FileStream fs = File.OpenRead(filePath))
            {
                //HSSFWorkbook wk = new HSSFWorkbook(fs);
                wk = new XSSFWorkbook(fs);
                //ISheet sheet = wk.CreateSheet("Sheet1");
            }
            return wk;
        }

        private static void Write(string filePath, XSSFWorkbook book)
        {

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                book.Write(fs);
            }
            book = null;

        }



    }
}
