

namespace Sort
{
    /// <summary>
    /// 冒泡排序
    /// </summary>
    public class bubblesort
    {
        public void BubbleSort(int[] R)
        {
            int i, j, temp; //交换标志 
            bool exchange;
            for (i = 0; i < R.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false; //本趟排序开始前，交换标志应为假
                for (j = R.Length - 2; j >= i; j--)
                {
                    if (R[j + 1] < R[j]) //交换条件
                    {
                        temp = R[j + 1];
                        R[j + 1] = R[j];
                        R[j] = temp;
                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }
                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
        }
    }
}