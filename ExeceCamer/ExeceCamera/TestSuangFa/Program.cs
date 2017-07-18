using System;
using StandardDeviations;
using System.Collections;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace TestSuangFa
{
    class Program
    {
        static void Main(string[] args)
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("name", System.Type.GetType("System.String"));
            dt.Columns.Add("rg_1", System.Type.GetType("System.Double"));
            dt.Columns.Add("rg_2", System.Type.GetType("System.Double"));
            dt.Columns.Add("rg_3", System.Type.GetType("System.Double"));

            DataRow dr_1 = dt.NewRow();
            dr_1["name"] = "pic_1";
            dr_1["rg_1"] = 20;
            dr_1["rg_2"] = 7;
            dr_1["rg_3"] = 1.1;
            dt.Rows.Add(dr_1);

            DataRow dr_2 = dt.NewRow();
            dr_2["name"] = "pic_2";
            dr_2["rg_1"] = 1.1;
            dr_2["rg_2"] = 6;
            dr_2["rg_3"] = 2.1;
            dt.Rows.Add(dr_2);

            DataRow dr_3 = dt.NewRow();
            dr_3["name"] = "pic_3";
            dr_3["rg_1"] = 5;
            dr_3["rg_2"] = 1.5;
            dr_3["rg_3"] = 4;
            dt.Rows.Add(dr_3);

            DataRow dr_4 = dt.NewRow();
            dr_4["name"] = "pic_4";
            dr_4["rg_1"] = 1.1;
            dr_4["rg_2"] = 1.5;
            dr_4["rg_3"] = 4;
            dt.Rows.Add(dr_4);

            double[] rg_1 = GetDataCateDouble(dt, "rg_1");
            double[] rg_2 = GetDataCateDouble(dt, "rg_2");
            double[] rg_3 = GetDataCateDouble(dt, "rg_3");


            foreach (var item in rg_1)
            {
                Console.WriteLine("rg_1 初始值=" + item.ToString());
            }

            Console.WriteLine();
            foreach (var item in rg_2)
            {
                Console.WriteLine("rg_2 初始值=" + item.ToString());
            }
      
            

            int[] indexs;
            List<int> indexs_count = new List<int>();
            GetErrorDataIndex(rg_1, 0.8,out indexs);
            indexs_count.AddRange(indexs);
            GetErrorDataIndex(rg_2, 0.8, out indexs);
            indexs_count.AddRange(indexs);
            indexs = indexs_count.Distinct().ToArray();



            foreach (var item in indexs)
            {
                Console.WriteLine(" rg_1 item=" + item.ToString());
            }

            double[] remain;
            GetNewData(rg_1, indexs, out remain);
            

            foreach (var item in remain)
            {
                Console.WriteLine(" rg_1 移除后的值=" + item.ToString());
            }


            GetNewData(rg_2, indexs, out remain);
            
            Console.WriteLine();

            foreach (var item in remain)
            {
                Console.WriteLine(" rg_2 移除后的值=" + item.ToString());
            }



            Console.Read();
        }

        /// <summary>
        /// 更具指定的系数找出超标数据的下标
        /// </summary>
        /// <param name="t">输入的数组</param>
        /// <param name="k">输入的系数</param>
        /// <param name="indexs">输出的下标</param>
        private static void GetErrorDataIndex(double[] t,double k,out int[] indexs)
        {
            DAPublic dp = new DAPublic();

            double offect;//获得输出后的偏差
            double[] remain_rg_1;//按照 系数移除后剩余的 数
            dp.DelErroData_Max(t, k, out offect, out remain_rg_1);

            var data_ = t.Except(remain_rg_1);//原始数据中移除 超出规格的数据
            double[] data_expect = new double[data_.Count()];


            int[] indexs_ = new int[10];//保存超出标准数据的下标
            int i = 0;
            foreach (var item in data_)
            {

                data_expect[i] = Convert.ToDouble(item);
                Console.WriteLine(data_expect[i].ToString());
                indexs_[i] = Array.IndexOf(t, data_expect[i]);
                i++;
            }
            //移除indexs 中多余的0
            int[] s = new int[i];
            Array.Copy(indexs_, s, i);
            indexs = s;

        }

        /// <summary>
        /// 根据下标删除数组中元素
        /// </summary>
        /// <param name="t"></param>
        /// <param name="indexs"></param>
        /// <param name="new_t"></param>
        private static void GetNewData(double[] t,int[] indexs,out double[] new_t)
        {

            List<double> t_List = t.ToList();
            double[] BeRemove = new double[indexs.Length];
            for (int m = 0; m < BeRemove.Length; m++)
            {
                BeRemove[m] = t[indexs[m]];
            }
            //在rg_2中删除rg_1中超标的数据，不纳入计算
            foreach (var item in BeRemove)
            {
                t_List.Remove(item);
            }
            new_t = t_List.ToArray();

        }
        
        /// <summary>
        /// 在表中取出列名称对应的数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="coloumnName"></param>
        /// <returns></returns>
        public static double[] GetDataCateDouble(DataTable dt, string coloumnName)
        {
            ArrayList aList = new ArrayList();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                
                    aList.Add(Convert.ToDouble(dt.Rows[i][coloumnName]));
                
            }
            double[] value = aList.Cast<double>().ToArray();
            return value;
        }



    }
}
