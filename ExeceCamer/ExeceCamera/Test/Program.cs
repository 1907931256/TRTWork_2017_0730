using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Statistics;
using System.Collections;
using System.Data;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            DataTable dt = CSVFileHelper.OpenCSV(@"e:\TRTWork\ExeceCamer\1234.csv");

            double rg_max;
            double rg_min;
            double rg_eviation;
            double rg_mean;
            GetData(dt, "R/G_1", out rg_max, out rg_min,out rg_eviation,out rg_mean);
            Console.WriteLine(rg_max.ToString());




            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    R_G_1.Add(Convert.ToDouble(dt.Rows[i]["R/G_1"]));
            //    R_G_2.Add(Convert.ToDouble(dt.Rows[i]["R/G_2"]));
            //    R_G_3.Add(Convert.ToDouble(dt.Rows[i]["R/G_3"]));
            //}

            //double[] R_G_1_value = R_G_1.Cast<double>().ToArray();
            //double[] R_G_2_value = R_G_2.Cast<double>().ToArray();
            //double[] R_G_3_value = R_G_3.Cast<double>().ToArray();



            ////计算每列数据的 最大值 最小值 平均值 标准偏差值

            //Console.WriteLine("**********************************************");

            //Console.WriteLine("最大值="+R_G_1_value.Maximum().ToString());
            //Console.WriteLine("最小值="+R_G_1_value.Minimum().ToString());
            //Console.WriteLine("均值=" + R_G_1_value.Mean().ToString());
            //Console.WriteLine("标准偏差值="+R_G_1_value.StandardDeviation().ToString());//标准偏差值
          
            //Console.WriteLine("**********************************************");

            //Console.WriteLine("最大值=" + R_G_2_value.Maximum().ToString());
            //Console.WriteLine("最小值=" + R_G_2_value.Minimum().ToString());
            //Console.WriteLine("均值=" + R_G_2_value.Mean().ToString());
            //Console.WriteLine("标准偏差值=" + R_G_2_value.StandardDeviation().ToString());//标准偏差值

            //Console.WriteLine("**********************************************");

            //Console.WriteLine("最大值=" + R_G_3_value.Maximum().ToString());
            //Console.WriteLine("最小值=" + R_G_3_value.Minimum().ToString());
            //Console.WriteLine("均值=" + R_G_3_value.Mean().ToString());
            //Console.WriteLine("标准偏差值=" + R_G_3_value.StandardDeviation().ToString());//标准偏差值

            Console.Read();

        }
        public static void GetData(DataTable dt,string coloumnName,out double max,out double min,out double eviation,out double mean)
        {
                max=min=eviation=mean=0;
               ArrayList aList=new ArrayList();
               for (int i = 0; i < dt.Rows.Count; i++)
                {
                    aList.Add(Convert.ToDouble(dt.Rows[i][coloumnName]));
                }
               double[] value = aList.Cast<double>().ToArray();
            max=value.Maximum();
            min=value.Minimum();
            mean=value.Mean();
            eviation=value.StandardDeviation();

        }
    }
}
