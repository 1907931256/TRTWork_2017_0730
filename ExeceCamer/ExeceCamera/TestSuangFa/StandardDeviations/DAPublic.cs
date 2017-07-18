using MathNet.Numerics.Statistics;
using System;
using Sort;
using System.Collections.Generic;
using System.Linq;

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


        public double GetAvage(double[] d)
        {
            return d.Mean();
        }

        public double GetMax(double[] d)
        {
            return d.Maximum();
        }

        public double GetMin(double[] d)
        {
            return d.Minimum();
        }

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
                        list.Remove(max_ + avage);
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




    }
}
