using StandardDeviations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TestSuangFa.Help
{
    class HelpMethod
    {
        /// <summary>
        /// 更具指定的系数找出超标数据的下标
        /// </summary>
        /// <param name="t">输入的数组</param>
        /// <param name="k">输入的系数</param>
        /// <param name="indexs">输出的下标</param>
        private static void GetErrorDataIndex(double[] t, double k, out int[] indexs)
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
        private static void GetNewData(double[] t, int[] indexs, out double[] new_t)
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
