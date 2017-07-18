using System;
using System.Windows.Forms;


namespace ExeceCamera
{


     
    public partial class Form1 : Form
    {

        private static string fileName;
        private static string filePath;
        public Form1()
        {
            filePath = System.Windows.Forms.Application.StartupPath;
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
           
        }


        /// <summary>
        /// 选中文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "e:\\";
            openFileDialog1.Filter = "(*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                int nameIndex = fileName.LastIndexOf("\\");
                int lastname = fileName.LastIndexOf(".");
            }


        }  

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
        //    if (excelEdit_ != null)
        //    {
        //        RGmax = Calculation_RGMax();
        //        RGmin = Calculation_RGMin();
        //        RGstdev = Calculation_RGSTDEV();

        //        BGmax = Calculation_BGMax();
        //        BGmin = Calculation_BGMin();
        //        BGstdev = Calculation_BGSTDEV();

        //        double[] RG_aver = new double[25];
        //        RG_aver = Calculation_RGaver();
        //        double[] BG_aver = new double[25];
        //        BG_aver = Calculation_BGaver();

        //        if (DataCheckBox.Checked)
        //        {
        //            for (int i = 1; i < RGmax.Length; i++)
        //            {
        //                if (RGstdev[i] > RG_aver[i] * Convert.ToDouble(yc_xs.Text) || BGstdev[i] > BG_aver[i] * Convert.ToDouble(yc_xs.Text))
        //                {
        //                    MessageBox.Show("数据源异常，请检查照片来源！");
        //                    GB_SeKa.Visible = false;
        //                    break;
        //                }
        //            }

        //        }
        //        else
        //        {
        //            GB_SeKa.Visible = true;
        //        }
        //    }
        //       else
        //{
        //    MessageBox.Show("请先加载文档");
        //}


          

        }
        private void qian_btn_Click(object sender, EventArgs e)
        {
            //if (Convert.ToDouble(s_txt.Text) <= 0 || Convert.ToDouble(x_txt.Text) <= 0)
            //{
            //    MessageBox.Show("系数输入有误!");
            //}
            //else
            //{
               
            //     string rg_max=string.Empty;
            //     string rg_min = string.Empty;

            //     string bg_max = string.Empty;
            //     string bg_min = string.Empty;



            //    for (int i = 1; i < RGmax.Length; i++)
            //    {
            //        RGmax[i] = RGmax[i] + RGstdev[i] * Convert.ToDouble(s_txt.Text);
            //        BGmax[i] = BGmax[i] + BGstdev[i] * Convert.ToDouble(s_txt.Text);
                   
            //        RGmin[i] = RGmin[i] - RGstdev[i] * Convert.ToDouble(x_txt.Text);
            //        BGmin[i] = BGmin[i] - BGstdev[i] * Convert.ToDouble(x_txt.Text);

            //        if (RGmin[i]<0)
            //        {
            //            RGmin[i] = 0;
            //        }
            //        if (BGmin[i]<0)
            //        {
            //            BGmin[i] = 0;
            //        }

            //        rg_max += string.Format("{0:F}", RGmax[i]) + ",";
            //        rg_min += string.Format("{0:F}", RGmin[i]) + ",";
            //        bg_max += string.Format("{0:F}", BGmax[i]) + ",";
            //        bg_min += string.Format("{0:F}", BGmin[i]) + ",";

            //    }
            //    INIOperationClass.INIWriteValue(filePath+"\\system1.ini", "System", "MaxRG", rg_max);
            //    INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MinRG", rg_min);
            //    INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MaxBG", bg_max);
            //    INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MinBG", bg_min);
            //    MessageBox.Show("操作完毕");

            //}
        }
        //private double[] Calculation_RGMax()
        //{
        //    //double[] RG_max = new double[25];

        //    //double[] RB_max = new double[25];
        //    //double[] RB_min = new double[25];
        //    //double[] RB_skew_p = new double[25];

        //    //RG_max[1] = excelEdit_.Max("D1");
        //    //RG_max[2] = excelEdit_.Max("G1");
        //    //RG_max[3] = excelEdit_.Max("J1");
        //    //RG_max[4] = excelEdit_.Max("M1");
        //    //RG_max[5] = excelEdit_.Max("P1");
        //    //RG_max[6] = excelEdit_.Max("S1");
        //    //RG_max[7] = excelEdit_.Max("V1");
        //    //RG_max[8] = excelEdit_.Max("Y1");
        //    //RG_max[9] = excelEdit_.Max("AB1");
        //    //RG_max[10] = excelEdit_.Max("AE1");
        //    //RG_max[11] = excelEdit_.Max("AH1");
        //    //RG_max[12] = excelEdit_.Max("AK1");
        //    //RG_max[13] = excelEdit_.Max("AN1");
        //    //RG_max[14] = excelEdit_.Max("AQ1");
        //    //RG_max[15] = excelEdit_.Max("AT1");
        //    //RG_max[16] = excelEdit_.Max("AW1");
        //    //RG_max[17] = excelEdit_.Max("AZ1");
        //    //RG_max[18] = excelEdit_.Max("BC1");
        //    //RG_max[19] = excelEdit_.Max("BF1");
        //    //RG_max[20] = excelEdit_.Max("BI1");
        //    //RG_max[21] = excelEdit_.Max("BL1");
        //    //RG_max[22] = excelEdit_.Max("BO1");
        //    //RG_max[23] = excelEdit_.Max("BR1");
        //    //RG_max[24] = excelEdit_.Max("BU1");
        //    ////RG_max[25] = RG_max[26] = RG_max[27] = RG_max[28] = 1.5;
        //    //return RG_max;
        //}

        //private double[] Calculation_RGMin()
        //{
        //    double[] RG_min = new double[25];
        //    RG_min[1] = excelEdit_.Min("D1");
        //    RG_min[2] = excelEdit_.Min("G1");
        //    RG_min[3] = excelEdit_.Min("J1");
        //    RG_min[4] = excelEdit_.Min("M1");
        //    RG_min[5] = excelEdit_.Min("P1");
        //    RG_min[6] = excelEdit_.Min("S1");
        //    RG_min[7] = excelEdit_.Min("V1");
        //    RG_min[8] = excelEdit_.Min("Y1");
        //    RG_min[9] = excelEdit_.Min("AB1");
        //    RG_min[10] = excelEdit_.Min("AE1");
        //    RG_min[11] = excelEdit_.Min("AH1");
        //    RG_min[12] = excelEdit_.Min("AK1");
        //    RG_min[13] = excelEdit_.Min("AN1");
        //    RG_min[14] = excelEdit_.Min("AQ1");
        //    RG_min[15] = excelEdit_.Min("AT1");
        //    RG_min[16] = excelEdit_.Min("AW1");
        //    RG_min[17] = excelEdit_.Min("AZ1");
        //    RG_min[18] = excelEdit_.Min("BC1");
        //    RG_min[19] = excelEdit_.Min("BF1");
        //    RG_min[20] = excelEdit_.Min("BI1");
        //    RG_min[21] = excelEdit_.Min("BL1");
        //    RG_min[22] = excelEdit_.Min("BO1");
        //    RG_min[23] = excelEdit_.Min("BR1");
        //    RG_min[24] = excelEdit_.Min("BU1");
        //    return RG_min;

        //}

        //private double[] Calculation_RGSTDEV()
        //{
        //    double[] RG_skew_p = new double[25];
        //    RG_skew_p[1] = excelEdit_.STDEV("D1");
        //    RG_skew_p[2] = excelEdit_.STDEV("G1");
        //    RG_skew_p[3] = excelEdit_.STDEV("J1");
        //    RG_skew_p[4] = excelEdit_.STDEV("M1");
        //    RG_skew_p[5] = excelEdit_.STDEV("P1");
        //    RG_skew_p[6] = excelEdit_.STDEV("S1");
        //    RG_skew_p[7] = excelEdit_.STDEV("V1");
        //    RG_skew_p[8] = excelEdit_.STDEV("Y1");
        //    RG_skew_p[9] = excelEdit_.STDEV("AB1");
        //    RG_skew_p[10] = excelEdit_.STDEV("AE1");
        //    RG_skew_p[11] = excelEdit_.STDEV("AH1");
        //    RG_skew_p[12] = excelEdit_.STDEV("AK1");
        //    RG_skew_p[13] = excelEdit_.STDEV("AN1");
        //    RG_skew_p[14] = excelEdit_.STDEV("AQ1");
        //    RG_skew_p[15] = excelEdit_.STDEV("AT1");
        //    RG_skew_p[16] = excelEdit_.STDEV("AW1");
        //    RG_skew_p[17] = excelEdit_.STDEV("AZ1");
        //    RG_skew_p[18] = excelEdit_.STDEV("BC1");
        //    RG_skew_p[19] = excelEdit_.STDEV("BF1");
        //    RG_skew_p[20] = excelEdit_.STDEV("BI1");
        //    RG_skew_p[21] = excelEdit_.STDEV("BL1");
        //    RG_skew_p[22] = excelEdit_.STDEV("BO1");
        //    RG_skew_p[23] = excelEdit_.STDEV("BR1");
        //    RG_skew_p[24] = excelEdit_.STDEV("BU1");
        //    return RG_skew_p;
        //}

        //private double[] Calculation_RGaver()
        //{
        //    double[] RG_aver= new double[25];
        //    RG_aver[1] = excelEdit_.Average("D1");
        //    RG_aver[2] = excelEdit_.Average("G1");
        //    RG_aver[3] = excelEdit_.Average("J1");
        //    RG_aver[4] = excelEdit_.Average("M1");
        //    RG_aver[5] = excelEdit_.Average("P1");
        //    RG_aver[6] = excelEdit_.Average("S1");
        //    RG_aver[7] = excelEdit_.Average("V1");
        //    RG_aver[8] = excelEdit_.Average("Y1");
        //    RG_aver[9] = excelEdit_.Average("AB1");
        //    RG_aver[10] = excelEdit_.Average("AE1");
        //    RG_aver[11] = excelEdit_.Average("AH1");
        //    RG_aver[12] = excelEdit_.Average("AK1");
        //    RG_aver[13] = excelEdit_.Average("AN1");
        //    RG_aver[14] = excelEdit_.Average("AQ1");
        //    RG_aver[15] = excelEdit_.Average("AT1");
        //    RG_aver[16] = excelEdit_.Average("AW1");
        //    RG_aver[17] = excelEdit_.Average("AZ1");
        //    RG_aver[18] = excelEdit_.Average("BC1");
        //    RG_aver[19] = excelEdit_.Average("BF1");
        //    RG_aver[20] = excelEdit_.Average("BI1");
        //    RG_aver[21] = excelEdit_.Average("BL1");
        //    RG_aver[22] = excelEdit_.Average("BO1");
        //    RG_aver[23] = excelEdit_.Average("BR1");
        //    RG_aver[24] = excelEdit_.Average("BU1");
        //    return RG_aver;

        //}


        //private double[] Calculation_BGMax()
        //{
        //    double[] BG_max = new double[25];
                    
        //    BG_max[1] = excelEdit_.Max("E1");
        //    BG_max[2] = excelEdit_.Max("H1");
        //    BG_max[3] = excelEdit_.Max("L1");
        //    BG_max[4] = excelEdit_.Max("N1");
        //    BG_max[5] = excelEdit_.Max("Q1");
        //    BG_max[6] = excelEdit_.Max("T1");
        //    BG_max[7] = excelEdit_.Max("W1");
        //    BG_max[8] = excelEdit_.Max("Z1");
        //    BG_max[9] = excelEdit_.Max("AC1");
        //    BG_max[10] = excelEdit_.Max("AF1");
        //    BG_max[11] = excelEdit_.Max("AI1");
        //    BG_max[12] = excelEdit_.Max("AL1");
        //    BG_max[13] = excelEdit_.Max("AO1");
        //    BG_max[14] = excelEdit_.Max("AR1");
        //    BG_max[15] = excelEdit_.Max("AV1");
        //    BG_max[16] = excelEdit_.Max("AZ1");
        //    BG_max[17] = excelEdit_.Max("BA1");
        //    BG_max[18] = excelEdit_.Max("BD1");
        //    BG_max[19] = excelEdit_.Max("BH1");
        //    BG_max[20] = excelEdit_.Max("BJ1");
        //    BG_max[21] = excelEdit_.Max("BM1");
        //    BG_max[22] = excelEdit_.Max("BP1");
        //    BG_max[23] = excelEdit_.Max("BS1");
        //    BG_max[24] = excelEdit_.Max("BV1");
        //    return BG_max;
        //}

        //private double[] Calculation_BGMin()
        //{
        //    double[] BG_min = new double[25];
        //    BG_min[1] = excelEdit_.Max("E1");
        //    BG_min[2] = excelEdit_.Max("H1");
        //    BG_min[3] = excelEdit_.Max("L1");
        //    BG_min[4] = excelEdit_.Max("N1");
        //    BG_min[5] = excelEdit_.Max("Q1");
        //    BG_min[6] = excelEdit_.Max("T1");
        //    BG_min[7] = excelEdit_.Max("W1");
        //    BG_min[8] = excelEdit_.Max("Z1");
        //    BG_min[9] = excelEdit_.Max("AC1");
        //    BG_min[10] = excelEdit_.Max("AF1");
        //    BG_min[11] = excelEdit_.Max("AI1");
        //    BG_min[12] = excelEdit_.Max("AL1");
        //    BG_min[13] = excelEdit_.Max("AO1");
        //    BG_min[14] = excelEdit_.Max("AR1");
        //    BG_min[15] = excelEdit_.Max("AV1");
        //    BG_min[16] = excelEdit_.Max("AZ1");
        //    BG_min[17] = excelEdit_.Max("BA1");
        //    BG_min[18] = excelEdit_.Max("BD1");
        //    BG_min[19] = excelEdit_.Max("BH1");
        //    BG_min[20] = excelEdit_.Max("BJ1");
        //    BG_min[21] = excelEdit_.Max("BM1");
        //    BG_min[22] = excelEdit_.Max("BP1");
        //    BG_min[23] = excelEdit_.Max("BS1");
        //    BG_min[24] = excelEdit_.Max("BV1");
        //    return BG_min;

        //}

        //private double[] Calculation_BGSTDEV()
        //{
        //    double[] BG_skew_p = new double[25];
        //    BG_skew_p[1] = excelEdit_.STDEV("E1");
        //    BG_skew_p[2] = excelEdit_.STDEV("H1");
        //    BG_skew_p[3] = excelEdit_.STDEV("L1");
        //    BG_skew_p[4] = excelEdit_.STDEV("N1");
        //    BG_skew_p[5] = excelEdit_.STDEV("Q1");
        //    BG_skew_p[6] = excelEdit_.STDEV("T1");
        //    BG_skew_p[7] = excelEdit_.STDEV("W1");
        //    BG_skew_p[8] = excelEdit_.STDEV("Z1");
        //    BG_skew_p[9] = excelEdit_.STDEV("AC1");
        //    BG_skew_p[10] = excelEdit_.STDEV("AF1");
        //    BG_skew_p[11] = excelEdit_.STDEV("AI1");
        //    BG_skew_p[12] = excelEdit_.STDEV("AL1");
        //    BG_skew_p[13] = excelEdit_.STDEV("AO1");
        //    BG_skew_p[14] = excelEdit_.STDEV("AR1");
        //    BG_skew_p[15] = excelEdit_.STDEV("AV1");
        //    BG_skew_p[16] = excelEdit_.STDEV("AZ1");
        //    BG_skew_p[17] = excelEdit_.STDEV("BA1");
        //    BG_skew_p[18] = excelEdit_.STDEV("BD1");
        //    BG_skew_p[19] = excelEdit_.STDEV("BH1");
        //    BG_skew_p[20] = excelEdit_.STDEV("BJ1");
        //    BG_skew_p[21] = excelEdit_.STDEV("BM1");
        //    BG_skew_p[22] = excelEdit_.STDEV("BP1");
        //    BG_skew_p[23] = excelEdit_.STDEV("BS1");
        //    BG_skew_p[24] = excelEdit_.STDEV("BV1");
        //    return BG_skew_p;
        //}

        //private double[] Calculation_BGaver()
        //{
        //    double[] BG_Average = new double[25];
        //    BG_Average[1] = excelEdit_.Average("E1");
        //    BG_Average[2] = excelEdit_.Average("H1");
        //    BG_Average[3] = excelEdit_.Average("L1");
        //    BG_Average[4] = excelEdit_.Average("N1");
        //    BG_Average[5] = excelEdit_.Average("Q1");
        //    BG_Average[6] = excelEdit_.Average("T1");
        //    BG_Average[7] = excelEdit_.Average("W1");
        //    BG_Average[8] = excelEdit_.Average("Z1");
        //    BG_Average[9] = excelEdit_.Average("AC1");
        //    BG_Average[10] = excelEdit_.Average("AF1");
        //    BG_Average[11] = excelEdit_.Average("AI1");
        //    BG_Average[12] = excelEdit_.Average("AL1");
        //    BG_Average[13] = excelEdit_.Average("AO1");
        //    BG_Average[14] = excelEdit_.Average("AR1");
        //    BG_Average[15] = excelEdit_.Average("AV1");
        //    BG_Average[16] = excelEdit_.Average("AZ1");
        //    BG_Average[17] = excelEdit_.Average("BA1");
        //    BG_Average[18] = excelEdit_.Average("BD1");
        //    BG_Average[19] = excelEdit_.Average("BH1");
        //    BG_Average[20] = excelEdit_.Average("BJ1");
        //    BG_Average[21] = excelEdit_.Average("BM1");
        //    BG_Average[22] = excelEdit_.Average("BP1");
        //    BG_Average[23] = excelEdit_.Average("BS1");
        //    BG_Average[24] = excelEdit_.Average("BV1");
        //    return BG_Average;

        //}


        //预定值读取
        private void yuDu_save_Click(object sender, EventArgs e)
        {
            ////            WhiteScreen=.9
            ////LightSceen=.8
            ////DarkScreen=.2
            ////BlackScreen=.1

            ////Back Lens
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "WhiteScreen", (numericUpDown1.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "LightSceen", (numericUpDown2.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "DarkScreen", (numericUpDown3.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "BlackScreen", (numericUpDown4.Value / 100).ToString());

            ////Lcd Bringtens
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "LCDWhiteScreen", (numericUpDown8.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "LCDLightSceen", (numericUpDown7.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "LCDDarkScreen", (numericUpDown6.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "LCDBlackScreen", (numericUpDown5.Value / 100).ToString());

            ////Front Bringtens
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "WhiteScreen1", (numericUpDown33.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "LightSceen1", (numericUpDown11.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "DarkScreen1", (numericUpDown10.Value / 100).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "BlackScreen1", (numericUpDown9.Value / 100).ToString());

            ////CamWhiteBadPixelsThres=120
            ////CamWhiteBadPixelsThres1=110
            ////CamWhiteBadPixelsCount=2
            ////CamWhiteBadPixelsCount1=1
            ////.......
            ////CamWhiteAreaLimit=50
            ////CamWhiteAreaLimit1=60

            ////Cam White bad pixels
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteBadPixelsThres", (numericUpDown19.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteBadPixelsThres1", (numericUpDown21.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteBadPixelsCount", (numericUpDown18.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteBadPixelsCount1", (numericUpDown16.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteAreaLimit", (numericUpDown20.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteAreaLimit1", (numericUpDown17.Value).ToString());


            ////CamWhiteScreenDirtyXRatio=.03
            ////CamWhiteScreenDirtyYRatio=.09
            ////CamWhiteScreenDirty1XRatio=.03
            ////CamWhiteScreenDirty1YRatio=.09
            ////CamWhiteScreenDirtyCount=0
            ////CamWhiteScreenDirtyCount1=0
            ////.......
            ////CamDirtyDiffYThres=138
            ////........
            ////FrontCamDirtyDiffYThres=138
            ////Cam Dirty Sample Ratio
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteScreenDirtyXRatio", (numericUpDown27.Value ).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteScreenDirtyYRatio", (numericUpDown25.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteScreenDirty1XRatio", (numericUpDown29.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteScreenDirty1YRatio", (numericUpDown23.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteScreenDirtyCount", (numericUpDown26.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamWhiteScreenDirtyCount1", (numericUpDown24.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamDirtyDiffYThres", (numericUpDown28.Value ).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "FrontCamDirtyDiffYThres", (numericUpDown22.Value).ToString());

            ////NyquistValue=.3
            ////NyquistValue
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "NyquistValue", (numericUpDown32.Value).ToString());


            ////CamBlackBadPixelsThres=60
            ////CamBlackBadPixelsThres1=60
            ////CamBadPixelsCount=1
            ////CamBadPixelsCount1=1
            ////Cam Black  bad
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamBlackBadPixelsThres", (numericUpDown12.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamBlackBadPixelsThres1", (numericUpDown15.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamBadPixelsCount", (numericUpDown13.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "CamBadPixelsCount1", (numericUpDown14.Value).ToString());

            ////BlackEdgeThres=80
            ////BlackEdgeThres1=80
            ////WhiteEdgeThres=40
            ////WhiteEdgeThres1=40
            ////Edge
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "BlackEdgeThres", (numericUpDown37.Value ).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "BlackEdgeThres1", (numericUpDown36.Value ).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "WhiteEdgeThres", (numericUpDown31.Value).ToString());
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "WhiteEdgeThres1", (numericUpDown30.Value ).ToString());







        }


        //特例计算
        private void button6_Click(object sender, EventArgs e)
        {
            //if (excelEdit_ != null)
            //{
            //    string s_rg = INIOperationClass.INIGetStringValue(filePath + "\\system1.ini", "System", "MaxRG", "0");

            //    string s_bg = INIOperationClass.INIGetStringValue(filePath + "\\system1.ini", "System", "MaxRG", "0");
            //    string[] str_rg_ini = s_rg.Split(',');
            //    string[] str_bg_ini = s_bg.Split(',');

            //    double[] MaxRG_ini = new double[str_rg_ini.Length - 1];

            //    double[] MaxBG_ini = new double[str_bg_ini.Length - 1];

            //    for (int i = 0; i < MaxRG_ini.Length; i++)
            //    {
            //        MaxRG_ini[i] = Convert.ToDouble(str_rg_ini[i]);
            //    }
            //    for (int i = 0; i < MaxBG_ini.Length; i++)
            //    {
            //        MaxBG_ini[i] = Convert.ToDouble(str_bg_ini[i]);
            //    }

            //    double[] MaxRG_exc = Calculation_RGMax();
            //    double[] MaxBG_exc = Calculation_BGMax();

            //    for (int i = 0; i < MaxRG_ini.Length; i++)
            //    {
            //        if (MaxRG_ini[i] < MaxRG_exc[i + 1])
            //        {
            //            MaxRG_ini[i] = MaxRG_exc[i + 1];
            //        }
            //    }
            //    for (int i = 0; i < MaxBG_ini.Length; i++)
            //    {
            //        if (MaxBG_ini[1] < MaxBG_exc[i + 1])
            //        {
            //            MaxBG_ini[i] = MaxBG_exc[i + 1];
            //        }
            //    }


            //}
            //else
            //{
            //    MessageBox.Show("请先加载文档");
            //}
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //excelEdit_.Save();
        }
       


    }

}
