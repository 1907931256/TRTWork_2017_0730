using MathNet.Numerics.Statistics;
using System;
using Sort;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Collections;

namespace StandardDeviations
{

    public class DAPublic : IStandardDeviations
    {
        /// <summary>
        /// 数据分析一些常用的方法
        /// </summary>
        public DAPublic()
        {

        }

        /// <summary>
        /// 获得平均值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public double GetAvage(double[] d)
        {
            return d.Mean();
        }

        /// <summary>
        /// 获得最大值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public double GetMax(double[] d)
        {
            return d.Maximum();
        }

        /// <summary>
        /// 获得最小值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public double GetMin(double[] d)
        {
            return d.Minimum();
        }

        /// <summary>
        /// 获得标准偏差
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public double GetOffect(double[] d)
        {
            return d.StandardDeviation();
        }

        /// <summary>
        /// 从数组最小值开始移除元素，直至标准偏差达到指定值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <param name="new_sd_min"></param>
        /// <param name="remain"></param>
        public void DelErroData_Min(double[] data, double k, out double new_sd_min, out double[] remain)
        {
            if (k < 0)
            {
                throw new Exception("k is erro");
            }
            data = ShellSorter.Sort(data);
            double sd = this.GetOffect(data);

            int i = 1;
            while (true)
            {
                if (sd < k)
                {
                    new_sd_min = sd;
                    remain = data;
                    break;
                }
                else
                {
                    double[] remain_data = new double[data.Length - i];
                    Array.Copy(data, i, remain_data, 0, remain_data.Length);
                    sd = this.GetOffect(remain_data);
                    i++;
                    if (sd < k)
                    {
                        new_sd_min = sd;
                        remain = remain_data;
                        break;
                    }

                }
            }
        }

        /// <summary>
        /// 按照指定的门限删除数据
        /// </summary>
        /// <param name="new_data">需要处理的数据</param>
        /// <param name="k">目标门限值</param>
        /// <param name="flagNumbers">数据在的下标位置</param>
        public void GetThresholdDatas(double[] new_data, double k,string max, out int[] flagNumbers)
        {
            double[] data_ = new double[new_data.Length];
            Array.Copy(new_data, data_, new_data.Length);

            data_=ShellSorter.Sort(data_);
            double sd;
            while (true)
            {
                sd = this.GetOffect(data_);
                List<double> list = data_.ToList();
                if (max=="max")
                {
                    list.Remove(this.GetMax(data_));
                }
                else
                {
                    list.Remove(this.GetMin(data_));
                }
               
                double[] remain_data = new double[list.Count];
                Array.Copy(list.ToArray(), remain_data, remain_data.Length);
                sd = this.GetOffect(remain_data);
                data_ = list.ToArray();
                if (sd <= k)
                {
                    double[] remainDatas;
                    GetDifferentDatas(new_data, data_, out flagNumbers, out remainDatas);
                    break;
                }

            }
        }

        /// <summary>
        /// 找出两组数据的不同点
        /// </summary>
        /// <param name="new_data">原始数据</param>
        /// <param name="data_">和原始数据比较的数据，</param>
        /// <param name="flagNumbers">在原始数据中的下标</param>
        /// <param name="remainDatas">和原始数据存在的差异数据</param>
        private void GetDifferentDatas(double[] new_data, double[] data_, out int[] flagNumbers, out double[] remainDatas)
        {
            List<double> datas = new_data.ToList();
            List<double> data_list = data_.ToList();
            List<int> flags = new List<int>();
            remainDatas = datas.Intersect(data_list).ToArray();



            //找出不同的元素(即交集的补集)
            var diffArr = datas.Where(c => !data_list.Contains(c)).ToArray();
            for (int i = 0; i < new_data.Length; i++)
            {
                for (int j = 0; j < diffArr.Length; j++)
                {
                    if (new_data[i] == diffArr[j])
                    {
                        flags.Add(i);
                    }
                }
                
            }
            flagNumbers = flags.ToArray();

        }

        /// <summary>
        /// 从最大值开始移除
        /// </summary>
        /// <param name="data"></param>
        /// <param name="k"></param>
        /// <param name="new_sd_min"></param>
        /// <param name="remain"></param>
        public void DelErroData_Max(double[] data, double k, out double new_sd_min, out double[] remain)
        {
            if (k < 0)
            {
                throw new Exception("k is erro");
            }
            double[] data_ = new double[data.Length];
            Array.Copy(data, data_, data.Length);

            double sd = this.GetOffect(data_);

            //int i = 1;

            if (sd < k)
            {
                new_sd_min = sd;
                remain = data;
            }
            else
            {
                while (true)
                {

                    double avage = this.GetAvage(data_);

                    double[] offest_ = new double[data_.Length];//定义一个差值集合
                    for (int j = 0; j < data_.Length; j++)
                    {
                        offest_[j] = Math.Abs(avage - data_[j]);
                    }
                    double max_ = GetMax(offest_);//得到最大偏差
                    List<double> list = data_.ToList();
                    if (max_ + avage == this.GetMax(data_))
                    {
                        list.Remove(this.GetMax(data_));
                    }
                    else
                    {
                        list.Remove(this.GetMin(data_));
                    }
                    double[] remain_data = new double[list.Count];

                    Array.Copy(list.ToArray(), remain_data, remain_data.Length);

                    sd = this.GetOffect(remain_data);

                    data_ = list.ToArray();

                    if (sd < k)
                    {
                        new_sd_min = sd;
                        remain = remain_data;
                        break;
                    }

                }
            }

        }

        /// <summary>
        /// 移除指定的数据后返回一个新的数据
        /// 按照最大最小个移除指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="new_data"></param>
        /// <param name="max">指示是否从最大值处开始移除</param>
        public void GetRemoveNumData(double[] data ,out double[] new_data,bool max)
        {
            int k =(int) Math.Round(data.Length * 0.1);
            int num = data.Length;
            data=ShellSorter.Sort(data);
            List<double> list = data.ToList();
            for (int i = 0; i < k; i++)
            {
                if (max==true)
                {
                    list.RemoveAt(list.Count - 1);
                }
                else
                {
                    list.RemoveAt(0);
                }
            }

            new_data = list.ToArray();

        }

        /// <summary>
        /// 读取指定列的内容，返回一个该列的数组
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="coloumnName"></param>
        /// <param name="new_data"></param>
        /// <param name="flag"></param>
        public void GetDataCate(DataTable dt, string coloumnName, out double[] new_data, string flag)
        {
            ArrayList aList = new ArrayList();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToString(dt.Rows[i]["Cate"]).IndexOf(flag) != -1)
                {
                    aList.Add(Convert.ToDouble(dt.Rows[i][coloumnName]));
                }
            }
            new_data = aList.Cast<double>().ToArray();

        }

        /// <summary>
        /// 如果数据在范围内返回
        /// </summary>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="data"></param>
        private bool CheckDataInRange(double max, double min, double data)
        {
            if (data <= max && data >= min)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更具指定的系数找出超标数据的下标
        /// </summary>
        /// <param name="t">输入的数组</param>
        /// <param name="k">输入的系数</param>
        /// <param name="indexs">输出的下标</param>
        public  void GetErrorDataIndex(double[] t, double k, out int[] indexs)
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
        public static void GetNewData(double[] t, int[] indexs, out double[] new_t)
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
        public static double[] GetDataCateReturnDouble(DataTable dt, string coloumnName, string flag)
        {
            ArrayList aList = new ArrayList();
            ArrayList aList_ = new ArrayList();
            bool b = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToString(dt.Rows[i]["Cate"]).IndexOf(flag) != -1)
                {
                    aList.Add(Convert.ToDouble(dt.Rows[i][coloumnName]));
                    b = false;
                }
                else
                {
                    aList_.Add(Convert.ToDouble(dt.Rows[i][coloumnName]));
                    b = true;
                }
            }
            if (b)
            {
                double[] value = aList_.Cast<double>().ToArray();
                return value;
            }
            else
            {
                double[] value = aList.Cast<double>().ToArray();
                return value;
            }

        }

        public static string[] GetDataCateReturnString(DataTable dt, string coloumnName)
        {
            ArrayList aList = new ArrayList();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToString(dt.Rows[i]["Cate"]).IndexOf("0") != -1)
                {
                    aList.Add(Convert.ToString(dt.Rows[i][coloumnName]));
                }
            }
            string[] value = aList.Cast<string>().ToArray();
            return value;
        }

        /// <summary>
        /// 根据行号得到数据
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDataFromIndexsReturnString(int[] indexs)
        {
            DataTable table = new DataTable("ErrorDatas");
            table.Columns.Add("Filename");
            table.Columns.Add("ID");



        }






        ///// <summary>
        ///// 流程控制
        ///// </summary>
        ///// <param name="ratio">门限值</param>
        ///// <param name="flagNumbers">找出异常数据的下标</param>
        ///// <param name="columns">行号</param>
        //private void GetErrorDataIndexs(double ratio, out int[] flagNumbers, string columnNames)
        //{
        //    flagNumbers = null;
        //    //int[] flagNumbers;//异常数据下标
        //    double[] new_data;
        //    double k;
        //    double max, min, avage, offect;
        //    dapublic_.GetDataCate(dt, columnNames, out new_data, "0");
        //    max = dapublic_.GetMax(new_data);
        //    min = dapublic_.GetMax(new_data);
        //    avage = dapublic_.GetAvage(new_data);
        //    offect = dapublic_.GetOffect(new_data);
        //    if (Math.Abs((max - avage) / avage) > Math.Abs((min - avage) / avage))
        //    {
        //        k = Math.Abs((max - avage) / avage);
        //        if (k > ratio)//存在异常数据,在最大方向
        //        {
        //            dapublic_.GetThresholdDatas(new_data, ratio, "max", out flagNumbers);//按照指定的门限去掉部分数据
        //        }

        //    }
        //    else
        //    {
        //        k = Math.Abs((min - avage) / avage);
        //        if (k > ratio)//存在异常数据,在最小方向
        //        {
        //            dapublic_.GetThresholdDatas(new_data, ratio, "min", out flagNumbers);//按照指定的门限去掉部分数据
        //        }
        //    }


        //}

    }
}
