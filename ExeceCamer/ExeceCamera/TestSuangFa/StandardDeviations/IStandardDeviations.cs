
namespace StandardDeviations
{
    /// 数据分析的一些常用方法接口
    /// 
    /// 


    public interface IStandardDeviations
    {
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        double GetMax(double[]  d);

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        double GetMin(double[] d);

        /// <summary>
        /// 获取数据的平均值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        double GetAvage(double[] d);

        /// <summary>
        /// 获取数据的标准偏差
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        double GetOffect(double[] d);






    }
}
