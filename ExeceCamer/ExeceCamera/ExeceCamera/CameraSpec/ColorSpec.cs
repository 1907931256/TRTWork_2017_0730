
namespace ExeceCamera
{
    /// <summary>
    /// 24块色卡类
    /// </summary>
    public class ColorSpec_
    {
        //保存照片的名字
        string[] fileName;
        //保存 csv文件中的最大值
        private double[] max_RG_1;
        private double[] max_RG_0;
        private double[] max_BG_1;
        private double[] max_BG_0;
        private double[] max_Stdev_1;
        private double[] max_Stdev_0;
        //保存csv文件中的最小值
        private double[] min_RG_1;
        private double[] min_RG_0;
        private double[] min_BG_1;
        private double[] min_BG_0;
        private double[] min_Stdev_1;
        private double[] min_Stdev_0;

        public string[] FileName { get => fileName; set => fileName = value; }
        public double[] Max_RG_1 { get => max_RG_1; set => max_RG_1 = value; }
        /// <summary>
        /// 保存rg的上限
        /// </summary>
        public double[] Max_RG_0 { get => max_RG_0; set => max_RG_0 = value; }
        public double[] Max_BG_1 { get => max_BG_1; set => max_BG_1 = value; }
        public double[] Max_BG_0 { get => max_BG_0; set => max_BG_0 = value; }
        public double[] Max_Stdev_1 { get => max_Stdev_1; set => max_Stdev_1 = value; }
        public double[] Max_Stdev_0 { get => max_Stdev_0; set => max_Stdev_0 = value; }
        public double[] Min_RG_1 { get => min_RG_1; set => min_RG_1 = value; }
        public double[] Min_RG_0 { get => min_RG_0; set => min_RG_0 = value; }
        public double[] Min_BG_1 { get => min_BG_1; set => min_BG_1 = value; }
        public double[] Min_BG_0 { get => min_BG_0; set => min_BG_0 = value; }
        public double[] Min_Stdev_1 { get => min_Stdev_1; set => min_Stdev_1 = value; }
        public double[] Min_Stdev_0 { get => min_Stdev_0; set => min_Stdev_0 = value; }


        #region


        //double[] r_g_1;
        //double[] b_g_1;
        //double[] r_g_2;
        //double[] b_g_2;
        //double[] r_g_3;
        //double[] b_g_3;
        //double[] r_g_4;
        //double[] b_g_4;
        //double[] r_g_5;
        //double[] b_g_5;
        //double[] r_g_6;
        //double[] b_g_6;
        //double[] r_g_7;
        //double[] b_g_7;
        //double[] r_g_8;
        //double[] b_g_8;
        //double[] r_g_9;
        //double[] b_g_9;
        //double[] r_g_10;
        //double[] b_g_10;

        //double[] r_g_11;
        //double[] b_g_11;
        //double[] r_g_12;
        //double[] b_g_12;
        //double[] r_g_13;
        //double[] b_g_13;
        //double[] r_g_14;
        //double[] b_g_14;
        //double[] r_g_15;
        //double[] b_g_15;
        //double[] r_g_16;
        //double[] b_g_16;
        //double[] r_g_17;
        //double[] b_g_17;
        //double[] r_g_18;
        //double[] b_g_18;
        //double[] r_g_19;
        //double[] b_g_19;
        //double[] r_g_20;
        //double[] b_g_20;

        //double[] r_g_21;
        //double[] b_g_21;
        //double[] r_g_22;
        //double[] b_g_22;
        //double[] r_g_23;
        //double[] b_g_23;
        //double[] r_g_24;
        //double[] b_g_24;


        //public string[] FileName { get => fileName; set => fileName = value; }



        //public double[] B_g_1 { get => b_g_1; set => b_g_1 = value; }
        //public double[] R_g_2 { get => r_g_2; set => r_g_2 = value; }
        //public double[] B_g_2 { get => b_g_2; set => b_g_2 = value; }
        //public double[] R_g_3 { get => r_g_3; set => r_g_3 = value; }
        //public double[] B_g_3 { get => b_g_3; set => b_g_3 = value; }
        //public double[] R_g_4 { get => r_g_4; set => r_g_4 = value; }
        //public double[] B_g_4 { get => b_g_4; set => b_g_4 = value; }
        //public double[] R_g_5 { get => r_g_5; set => r_g_5 = value; }
        //public double[] B_g_5 { get => b_g_5; set => b_g_5 = value; }
        //public double[] R_g_6 { get => r_g_6; set => r_g_6 = value; }
        //public double[] B_g_6 { get => b_g_6; set => b_g_6 = value; }
        //public double[] R_g_7 { get => r_g_7; set => r_g_7 = value; }
        //public double[] B_g_7 { get => b_g_7; set => b_g_7 = value; }
        //public double[] R_g_8 { get => r_g_8; set => r_g_8 = value; }
        //public double[] B_g_8 { get => b_g_8; set => b_g_8 = value; }
        //public double[] R_g_9 { get => r_g_9; set => r_g_9 = value; }
        //public double[] B_g_9 { get => b_g_9; set => b_g_9 = value; }
        //public double[] R_g_10 { get => r_g_10; set => r_g_10 = value; }
        //public double[] B_g_10 { get => b_g_10; set => b_g_10 = value; }
        //public double[] R_g_11 { get => r_g_11; set => r_g_11 = value; }
        //public double[] B_g_11 { get => b_g_11; set => b_g_11 = value; }
        //public double[] R_g_12 { get => r_g_12; set => r_g_12 = value; }
        //public double[] B_g_12 { get => b_g_12; set => b_g_12 = value; }
        //public double[] R_g_13 { get => r_g_13; set => r_g_13 = value; }
        //public double[] B_g_13 { get => b_g_13; set => b_g_13 = value; }
        //public double[] R_g_14 { get => r_g_14; set => r_g_14 = value; }
        //public double[] B_g_14 { get => b_g_14; set => b_g_14 = value; }
        //public double[] R_g_15 { get => r_g_15; set => r_g_15 = value; }
        //public double[] B_g_15 { get => b_g_15; set => b_g_15 = value; }
        //public double[] R_g_16 { get => r_g_16; set => r_g_16 = value; }
        //public double[] B_g_16 { get => b_g_16; set => b_g_16 = value; }
        //public double[] R_g_17 { get => r_g_17; set => r_g_17 = value; }
        //public double[] B_g_17 { get => b_g_17; set => b_g_17 = value; }
        //public double[] R_g_18 { get => r_g_18; set => r_g_18 = value; }
        //public double[] B_g_18 { get => b_g_18; set => b_g_18 = value; }
        //public double[] R_g_19 { get => r_g_19; set => r_g_19 = value; }
        //public double[] B_g_19 { get => b_g_19; set => b_g_19 = value; }
        //public double[] R_g_20 { get => r_g_20; set => r_g_20 = value; }
        //public double[] B_g_20 { get => b_g_20; set => b_g_20 = value; }
        //public double[] R_g_21 { get => r_g_21; set => r_g_21 = value; }
        //public double[] B_g_21 { get => b_g_21; set => b_g_21 = value; }
        //public double[] R_g_22 { get => r_g_22; set => r_g_22 = value; }
        //public double[] B_g_22 { get => b_g_22; set => b_g_22 = value; }
        //public double[] R_g_23 { get => r_g_23; set => r_g_23 = value; }
        //public double[] B_g_23 { get => b_g_23; set => b_g_23 = value; }
        //public double[] R_g_24 { get => r_g_24; set => r_g_24 = value; }
        //public double[] B_g_24 { get => b_g_24; set => b_g_24 = value; }
        //public double[] R_g_1 { get => r_g_1; set => r_g_1 = value; }
        //public double R_g_1_up { get => r_g_1_up; set => r_g_1_up = value; }
        //public double B_g_1_up { get => b_g_1_up; set => b_g_1_up = value; }
        //public double R_g_2_up { get => r_g_2_up; set => r_g_2_up = value; }
        //public double B_g_2_up { get => b_g_2_up; set => b_g_2_up = value; }
        //public double R_g_3_up { get => r_g_3_up; set => r_g_3_up = value; }
        //public double B_g_3_up { get => b_g_3_up; set => b_g_3_up = value; }
        //public double R_g_4_up { get => r_g_4_up; set => r_g_4_up = value; }
        //public double B_g_4_up { get => b_g_4_up; set => b_g_4_up = value; }
        //public double R_g_5_up { get => r_g_5_up; set => r_g_5_up = value; }
        //public double B_g_5_up { get => b_g_5_up; set => b_g_5_up = value; }
        //public double R_g_6_up { get => r_g_6_up; set => r_g_6_up = value; }
        //public double B_g_6_up { get => b_g_6_up; set => b_g_6_up = value; }
        //public double R_g_7_up { get => r_g_7_up; set => r_g_7_up = value; }
        //public double B_g_7_up { get => b_g_7_up; set => b_g_7_up = value; }
        //public double R_g_8_up { get => r_g_8_up; set => r_g_8_up = value; }
        //public double B_g_8_up { get => b_g_8_up; set => b_g_8_up = value; }
        //public double R_g_9_up { get => r_g_9_up; set => r_g_9_up = value; }
        //public double B_g_9_up { get => b_g_9_up; set => b_g_9_up = value; }
        //public double R_g_10_up { get => r_g_10_up; set => r_g_10_up = value; }
        //public double B_g_10_up { get => b_g_10_up; set => b_g_10_up = value; }
        //public double R_g_11_up { get => r_g_11_up; set => r_g_11_up = value; }
        //public double B_g_11_up { get => b_g_11_up; set => b_g_11_up = value; }
        //public double R_g_12_up { get => r_g_12_up; set => r_g_12_up = value; }
        //public double B_g_12_up { get => b_g_12_up; set => b_g_12_up = value; }
        //public double R_g_13_up { get => r_g_13_up; set => r_g_13_up = value; }
        //public double B_g_13_up { get => b_g_13_up; set => b_g_13_up = value; }
        //public double R_g_14_up { get => r_g_14_up; set => r_g_14_up = value; }
        //public double B_g_14_up { get => b_g_14_up; set => b_g_14_up = value; }
        //public double R_g_15_up { get => r_g_15_up; set => r_g_15_up = value; }
        //public double B_g_15_up { get => b_g_15_up; set => b_g_15_up = value; }
        //public double R_g_16_up { get => r_g_16_up; set => r_g_16_up = value; }
        //public double B_g_16_up { get => b_g_16_up; set => b_g_16_up = value; }
        //public double R_g_17_up { get => r_g_17_up; set => r_g_17_up = value; }
        //public double B_g_17_up { get => b_g_17_up; set => b_g_17_up = value; }
        //public double R_g_18_up { get => r_g_18_up; set => r_g_18_up = value; }
        //public double B_g_18_up { get => b_g_18_up; set => b_g_18_up = value; }
        //public double R_g_19_up { get => r_g_19_up; set => r_g_19_up = value; }
        //public double B_g_19_up { get => b_g_19_up; set => b_g_19_up = value; }
        //public double R_g_20_up { get => r_g_20_up; set => r_g_20_up = value; }
        //public double B_g_20_up { get => b_g_20_up; set => b_g_20_up = value; }
        //public double R_g_21_up { get => r_g_21_up; set => r_g_21_up = value; }
        //public double B_g_21_up { get => b_g_21_up; set => b_g_21_up = value; }
        //public double R_g_22_up { get => r_g_22_up; set => r_g_22_up = value; }
        //public double B_g_22_up { get => b_g_22_up; set => b_g_22_up = value; }
        //public double R_g_23_up { get => r_g_23_up; set => r_g_23_up = value; }
        //public double B_g_23_up { get => b_g_23_up; set => b_g_23_up = value; }
        //public double R_g_24_up { get => r_g_24_up; set => r_g_24_up = value; }
        //public double B_g_24_up { get => b_g_24_up; set => b_g_24_up = value; }
        //public double R_g_1_down { get => r_g_1_down; set => r_g_1_down = value; }
        //public double B_g_1_down { get => b_g_1_down; set => b_g_1_down = value; }
        //public double R_g_2_down { get => r_g_2_down; set => r_g_2_down = value; }
        //public double B_g_2_down { get => b_g_2_down; set => b_g_2_down = value; }
        //public double R_g_3_down { get => r_g_3_down; set => r_g_3_down = value; }
        //public double B_g_3_down { get => b_g_3_down; set => b_g_3_down = value; }
        //public double R_g_4_down { get => r_g_4_down; set => r_g_4_down = value; }
        //public double B_g_4_down { get => b_g_4_down; set => b_g_4_down = value; }
        //public double R_g_5_down { get => r_g_5_down; set => r_g_5_down = value; }
        //public double B_g_5_down { get => b_g_5_down; set => b_g_5_down = value; }
        //public double R_g_6_down { get => r_g_6_down; set => r_g_6_down = value; }
        //public double B_g_6_down { get => b_g_6_down; set => b_g_6_down = value; }
        //public double R_g_7_down { get => r_g_7_down; set => r_g_7_down = value; }
        //public double B_g_7_down { get => b_g_7_down; set => b_g_7_down = value; }
        //public double R_g_8_down { get => r_g_8_down; set => r_g_8_down = value; }
        //public double B_g_8_down { get => b_g_8_down; set => b_g_8_down = value; }
        //public double R_g_9_down { get => r_g_9_down; set => r_g_9_down = value; }
        //public double B_g_9_down { get => b_g_9_down; set => b_g_9_down = value; }
        //public double R_g_10_down { get => r_g_10_down; set => r_g_10_down = value; }
        //public double B_g_10_down { get => b_g_10_down; set => b_g_10_down = value; }
        //public double R_g_11_down { get => r_g_11_down; set => r_g_11_down = value; }
        //public double B_g_11_down { get => b_g_11_down; set => b_g_11_down = value; }
        //public double R_g_12_down { get => r_g_12_down; set => r_g_12_down = value; }
        //public double B_g_12_down { get => b_g_12_down; set => b_g_12_down = value; }
        //public double R_g_13_down { get => r_g_13_down; set => r_g_13_down = value; }
        //public double B_g_13_down { get => b_g_13_down; set => b_g_13_down = value; }
        //public double R_g_14_down { get => r_g_14_down; set => r_g_14_down = value; }
        //public double B_g_14_down { get => b_g_14_down; set => b_g_14_down = value; }
        //public double R_g_15_down { get => r_g_15_down; set => r_g_15_down = value; }
        //public double B_g_15_down { get => b_g_15_down; set => b_g_15_down = value; }
        //public double R_g_16_down { get => r_g_16_down; set => r_g_16_down = value; }
        //public double B_g_16_down { get => b_g_16_down; set => b_g_16_down = value; }
        //public double R_g_17_down { get => r_g_17_down; set => r_g_17_down = value; }
        //public double B_g_17_down { get => b_g_17_down; set => b_g_17_down = value; }
        //public double R_g_18_down { get => r_g_18_down; set => r_g_18_down = value; }
        //public double B_g_18_down { get => b_g_18_down; set => b_g_18_down = value; }
        //public double R_g_19_down { get => r_g_19_down; set => r_g_19_down = value; }
        //public double B_g_19_down { get => b_g_19_down; set => b_g_19_down = value; }
        //public double R_g_20_down { get => r_g_20_down; set => r_g_20_down = value; }
        //public double B_g_20_down { get => b_g_20_down; set => b_g_20_down = value; }
        //public double R_g_21_down { get => r_g_21_down; set => r_g_21_down = value; }
        //public double B_g_21_down { get => b_g_21_down; set => b_g_21_down = value; }
        //public double R_g_22_down { get => r_g_22_down; set => r_g_22_down = value; }
        //public double B_g_22_down { get => b_g_22_down; set => b_g_22_down = value; }
        //public double R_g_23_down { get => r_g_23_down; set => r_g_23_down = value; }
        //public double B_g_23_down { get => b_g_23_down; set => b_g_23_down = value; }
        //public double R_g_24_down { get => r_g_24_down; set => r_g_24_down = value; }
        //public double B_g_24_down { get => b_g_24_down; set => b_g_24_down = value; }

        //double r_g_1_up;
        //double b_g_1_up;
        //double r_g_2_up;
        //double b_g_2_up;
        //double r_g_3_up;
        //double b_g_3_up;
        //double r_g_4_up;
        //double b_g_4_up;
        //double r_g_5_up;
        //double b_g_5_up;
        //double r_g_6_up;
        //double b_g_6_up;
        //double r_g_7_up;
        //double b_g_7_up;
        //double r_g_8_up;
        //double b_g_8_up;
        //double r_g_9_up;
        //double b_g_9_up;

        //double r_g_10_up;
        //double b_g_10_up;
        //double r_g_11_up;
        //double b_g_11_up;
        //double r_g_12_up;
        //double b_g_12_up;
        //double r_g_13_up;
        //double b_g_13_up;
        //double r_g_14_up;
        //double b_g_14_up;
        //double r_g_15_up;
        //double b_g_15_up;
        //double r_g_16_up;
        //double b_g_16_up;
        //double r_g_17_up;
        //double b_g_17_up;
        //double r_g_18_up;
        //double b_g_18_up;
        //double r_g_19_up;
        //double b_g_19_up;
        //double r_g_20_up;
        //double b_g_20_up;
        //double r_g_21_up;
        //double b_g_21_up;
        //double r_g_22_up;
        //double b_g_22_up;
        //double r_g_23_up;
        //double b_g_23_up;
        //double r_g_24_up;
        //double b_g_24_up;



        //double r_g_1_down;
        //double b_g_1_down;
        //double r_g_2_down;
        //double b_g_2_down;
        //double r_g_3_down;
        //double b_g_3_down;
        //double r_g_4_down;
        //double b_g_4_down;
        //double r_g_5_down;
        //double b_g_5_down;
        //double r_g_6_down;
        //double b_g_6_down;
        //double r_g_7_down;
        //double b_g_7_down;
        //double r_g_8_down;
        //double b_g_8_down;
        //double r_g_9_down;
        //double b_g_9_down;

        //double r_g_10_down;
        //double b_g_10_down;
        //double r_g_11_down;
        //double b_g_11_down;
        //double r_g_12_down;
        //double b_g_12_down;
        //double r_g_13_down;
        //double b_g_13_down;
        //double r_g_14_down;
        //double b_g_14_down;
        //double r_g_15_down;
        //double b_g_15_down;
        //double r_g_16_down;
        //double b_g_16_down;
        //double r_g_17_down;
        //double b_g_17_down;
        //double r_g_18_down;
        //double b_g_18_down;
        //double r_g_19_down;
        //double b_g_19_down;
        //double r_g_20_down;
        //double b_g_20_down;
        //double r_g_21_down;
        //double b_g_21_down;
        //double r_g_22_down;
        //double b_g_22_down;
        //double r_g_23_down;
        //double b_g_23_down;
        //double r_g_24_down;
        //double b_g_24_down;
        #endregion
    }
}
