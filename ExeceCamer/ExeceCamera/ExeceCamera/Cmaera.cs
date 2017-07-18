using StandardDeviations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ExeceCamera
{
    public partial class Cmaera : Form
    {
        DataTable dt;
        ColorSpec colorSpec_;
        DAPublic da;
        //数据上限系数
        double up;
        //数据下限系数
        double down;
        //保存文件路径
        string filePath;

        //表示一个复选框
        DataGridViewCheckBoxCell checkCell;

        string[] naem_r_g = new string[]
        {
           "R/G_1",
           "R/G_2",
           "R/G_3",
           "R/G_4",
           "R/G_5",
           "R/G_6",
           "R/G_7",
           "R/G_8",
           "R/G_9",

           "R/G_10",
           "R/G_11",
           "R/G_12",
           "R/G_13",
           "R/G_14",
           "R/G_15",
           "R/G_16",
           "R/G_17",
           "R/G_18",
           "R/G_19",
           "R/G_20",
           "R/G_21",
           "R/G_22",
           "R/G_23",
           "R/G_24"

        };
        string[] naem_b_g = new string[]
       {
           "B/G_1",
           "B/G_2",
           "B/G_3",
           "B/G_4",
           "B/G_5",
           "B/G_6",
           "B/G_7",
           "B/G_8",
           "B/G_9",
            
           "B/G_10",
           "B/G_11",
           "B/G_12",
           "B/G_13",
           "B/G_14",
           "B/G_15",
           "B/G_16",
           "B/G_17",
           "B/G_18",
           "B/G_19",
           "B/G_20",
           "B/G_21",
           "B/G_22",
           "B/G_23",
           "B/G_24"

       };

        public Cmaera()
        {
            da = new DAPublic();
            colorSpec_ = new ColorSpec();
            InitializeComponent();
        }

        private void LoadCsvFile_Click(object sender, EventArgs e)
        {
            dt = CSVFileHelper.OpenCSV(@"e:\TRTWork\ExeceCamer\12.csv");

            //openFileDialog1.InitialDirectory = "e:\\";
            //openFileDialog1.Filter = "(*.*)|*.*";
            //openFileDialog1.RestoreDirectory = true;

            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    string fileName = openFileDialog1.FileName;

            //    filePath = System.Windows.Forms.Application.StartupPath;

            //    dt = CSVFileHelper.OpenCSV(fileName);
            //}
        }

        ///// <summary>
        ///// 关系转对象
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <param name="colorSpecList_"></param>
        //public void ConvertDataToObject(DataTable dt,out List<ColorSpec> colorSpecList_)
        //{
        //    ColorSpec cs = new ColorSpec();
        //    colorSpecList_ = new List<ColorSpec>();

        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        cs.FileName = Convert.ToString(dt.Rows[i]["Filename"]);

        //        cs.B_g_1 = Convert.ToDouble(dt.Rows[i]["B/G_1"]);
        //        cs.B_g_2 = Convert.ToDouble(dt.Rows[i]["B/G_2"]);
        //        cs.B_g_3 = Convert.ToDouble(dt.Rows[i]["B/G_3"]);
        //        cs.B_g_4 = Convert.ToDouble(dt.Rows[i]["B/G_4"]);
        //        cs.B_g_5 = Convert.ToDouble(dt.Rows[i]["B/G_5"]);
        //        cs.B_g_6 = Convert.ToDouble(dt.Rows[i]["B/G_6"]);
        //        cs.B_g_7 = Convert.ToDouble(dt.Rows[i]["B/G_7"]);
        //        cs.B_g_8 = Convert.ToDouble(dt.Rows[i]["B/G_8"]);
        //        cs.B_g_9 = Convert.ToDouble(dt.Rows[i]["B/G_9"]);

        //        cs.B_g_10 = Convert.ToDouble(dt.Rows[i]["B/G_10"]);
        //        cs.B_g_11 = Convert.ToDouble(dt.Rows[i]["B/G_11"]);
        //        cs.B_g_12 = Convert.ToDouble(dt.Rows[i]["B/G_12"]);
        //        cs.B_g_13 = Convert.ToDouble(dt.Rows[i]["B/G_13"]);
        //        cs.B_g_14 = Convert.ToDouble(dt.Rows[i]["B/G_14"]);
        //        cs.B_g_15 = Convert.ToDouble(dt.Rows[i]["B/G_15"]);
        //        cs.B_g_16 = Convert.ToDouble(dt.Rows[i]["B/G_16"]);
        //        cs.B_g_17 = Convert.ToDouble(dt.Rows[i]["B/G_17"]);
        //        cs.B_g_18 = Convert.ToDouble(dt.Rows[i]["B/G_18"]);
        //        cs.B_g_19 = Convert.ToDouble(dt.Rows[i]["B/G_19"]);
        //        cs.B_g_20 = Convert.ToDouble(dt.Rows[i]["B/G_20"]);
        //        cs.B_g_21 = Convert.ToDouble(dt.Rows[i]["B/G_21"]);
        //        cs.B_g_22 = Convert.ToDouble(dt.Rows[i]["B/G_22"]);
        //        cs.B_g_23 = Convert.ToDouble(dt.Rows[i]["B/G_23"]);
        //        cs.B_g_24 = Convert.ToDouble(dt.Rows[i]["B/G_24"]);

        //        cs.R_g_1 = Convert.ToDouble(dt.Rows[i]["R/G_1"]);
        //        cs.R_g_2 = Convert.ToDouble(dt.Rows[i]["R/G_2"]);
        //        cs.R_g_3 = Convert.ToDouble(dt.Rows[i]["R/G_3"]);
        //        cs.R_g_4 = Convert.ToDouble(dt.Rows[i]["R/G_4"]);
        //        cs.R_g_5 = Convert.ToDouble(dt.Rows[i]["R/G_5"]);
        //        cs.R_g_6 = Convert.ToDouble(dt.Rows[i]["R/G_6"]);
        //        cs.R_g_7 = Convert.ToDouble(dt.Rows[i]["R/G_7"]);
        //        cs.R_g_8 = Convert.ToDouble(dt.Rows[i]["R/G_8"]);
        //        cs.R_g_9 = Convert.ToDouble(dt.Rows[i]["R/G_9"]);

        //        cs.R_g_10 = Convert.ToDouble(dt.Rows[i]["R/G_10"]);
        //        cs.R_g_11 = Convert.ToDouble(dt.Rows[i]["R/G_11"]);
        //        cs.R_g_12 = Convert.ToDouble(dt.Rows[i]["R/G_12"]);
        //        cs.R_g_13 = Convert.ToDouble(dt.Rows[i]["R/G_13"]);
        //        cs.R_g_14 = Convert.ToDouble(dt.Rows[i]["R/G_14"]);
        //        cs.R_g_15 = Convert.ToDouble(dt.Rows[i]["R/G_15"]);
        //        cs.R_g_16 = Convert.ToDouble(dt.Rows[i]["R/G_16"]);
        //        cs.R_g_17 = Convert.ToDouble(dt.Rows[i]["R/G_17"]);
        //        cs.R_g_18 = Convert.ToDouble(dt.Rows[i]["R/G_18"]);
        //        cs.R_g_19 = Convert.ToDouble(dt.Rows[i]["R/G_19"]);
        //        cs.R_g_20 = Convert.ToDouble(dt.Rows[i]["R/G_20"]);
        //        cs.R_g_21 = Convert.ToDouble(dt.Rows[i]["R/G_21"]);
        //        cs.R_g_22 = Convert.ToDouble(dt.Rows[i]["R/G_22"]);
        //        cs.R_g_23 = Convert.ToDouble(dt.Rows[i]["R/G_23"]);
        //        cs.R_g_24 = Convert.ToDouble(dt.Rows[i]["R/G_24"]);

        //        colorSpecList_.Add(cs);
        //    }
        //}

        private void Cmaera_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            //int[] indexs;
            //List<int> indexs_count = new List<int>();
            //GetErrorDataIndex(rg_1, 0.8, out indexs);
            //indexs_count.AddRange(indexs);
            //GetErrorDataIndex(rg_2, 0.8, out indexs);
            //indexs_count.AddRange(indexs);
            //indexs = indexs_count.Distinct().ToArray();

        }


        /// <summary>
        /// 数据预处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataHandle_Click(object sender, EventArgs e)
        {
            GetErrorDataIndex();

            #region
            //indexs_Lens1 = indexs_count_Lens1.Distinct().ToArray();
            //for (int i = 0; i < 24; i++)
            //{



            //    up = Convert.ToDouble(textBox1.Text);
            //    down = Convert.ToDouble(textBox2.Text);

            //    HelpMethod.GetNewData(HelpMethod.GetDataCateDouble(dt, naem_r_g[i], "Lens0"), indexs_Lens0, out double_new);
            //    R_G_1_up += (da.GetMax(double_new) + da.GetOffect(double_new) * up).ToString("f2") + ",";//数据上限
            //    R_G_1_down += (da.GetMax(double_new) - da.GetOffect(double_new) * down).ToString("f2") + ",";


            //    HelpMethod.GetNewData(HelpMethod.GetDataCateDouble(dt, naem_b_g[i], "Lens0"), indexs_Lens0, out double_new);
            //    B_G_1_up += (da.GetMax(double_new) + da.GetOffect(double_new) * up).ToString("f2") + ",";
            //    B_G_1_down += (da.GetMax(double_new) - da.GetOffect(double_new) * down).ToString("f2") + ",";


            //    HelpMethod.GetNewData(HelpMethod.GetDataCateDouble(dt, naem_r_g[i], "Lens1"), indexs_Lens1, out double_new);
            //    R_G_0_up += (da.GetMax(double_new) + da.GetOffect(double_new) * up).ToString("f2") + ",";//数据上限
            //    R_G_0_down += (da.GetMax(double_new) - da.GetOffect(double_new) * down).ToString("f2") + ",";


            //    HelpMethod.GetNewData(HelpMethod.GetDataCateDouble(dt, naem_b_g[i], "Lens1"), indexs_Lens1, out double_new);
            //    B_G_0_up += (da.GetMax(double_new) + da.GetOffect(double_new) * up).ToString("f2") + ",";
            //    B_G_0_down += (da.GetMax(double_new) - da.GetOffect(double_new) * down).ToString("f2") + ",";
            //}

            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MaxRG", R_G_1_up );
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MinRG", R_G_1_down);
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MAXBG", B_G_1_up);
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MinBG", B_G_1_down);

            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MaxRG1", R_G_0_up);
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MinRG1", R_G_0_up);
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MAXBG1", R_G_0_up);
            //INIOperationClass.INIWriteValue(filePath + "\\system1.ini", "System", "MinBG1", R_G_0_up);

#endregion
        }

        /// <summary>
        /// 在配置文件中读取门限值
        /// </summary>
        private void GetErrorDataIndex()
        {
            string threshold_str = INIOperationClass.INIGetStringValue(@"e:\TRTWork\ExeceCamer\ExeceCamera\ExeceCamera\threshold.ini", "threshold", " threshold_start", "0");
            double threshold = Convert.ToDouble(threshold_str);//限制系数

            double k = threshold;

            string R_G_1_up = string.Empty;//保存上限
            string R_G_1_down = string.Empty;//保存下限

            string B_G_1_up = string.Empty;//保存上限
            string B_G_1_down = string.Empty;//保存下限

            string R_G_0_up = string.Empty;//保存上限
            string R_G_0_down = string.Empty;//保存下限

            string B_G_0_up = string.Empty;//保存上限
            string B_G_0_down = string.Empty;//保存下限

            int[] indexs_Lens0;//存储说有超标下标
            int[] indexs_Lens1;

            List<int> indexs_count = new List<int>();
            List<int> indexs_count_Lens1 = new List<int>();

            for (int i = 0; i < 24; i++)
            {
                double[] d_ = HelpMethod.GetDataCateDouble(dt, naem_r_g[i], "Lens0");
                HelpMethod.GetErrorDataIndex(d_, k, out indexs_Lens0);
                indexs_count.AddRange(indexs_Lens0);

                double[] d_Lens1 = HelpMethod.GetDataCateDouble(dt, naem_r_g[i], "Lens1");
                HelpMethod.GetErrorDataIndex(d_Lens1, k, out indexs_Lens1);
                indexs_count_Lens1.AddRange(indexs_Lens1);

            }
            indexs_Lens0 = indexs_count.Distinct().ToArray();
            if (indexs_Lens0.Count() != 0)
            {
                DataRow row;
                DataTable errorData_dt = new DataTable("errorData");
                errorData_dt.Columns.Add("Filename");
                errorData_dt.Columns.Add("ID");




                //添加选择框
                DataGridViewCheckBoxColumn newColumn = new DataGridViewCheckBoxColumn();
                newColumn.HeaderText = "√表示可以通过";
                newColumn.Name = "IsCheck";
                dataGridView1.Columns.Add(newColumn);

                for (int i = 0; i < indexs_Lens0.Count(); i++)
                {
                    row = errorData_dt.NewRow();
                    row["ID"] = indexs_Lens0[i];
                    row["Filename"] = dt.Rows[indexs_Lens0[i]]["Filename"];
                    errorData_dt.Rows.Add(row);
                }
                dataGridView1.DataSource = errorData_dt;
                dataGridView1.Visible = true;

                int indexs;
                int[] id;
                id = GetRowChecked(out indexs);

                //foreach (var item in id)
                //{
                //   double dd= Convert.ToDouble(dt.Rows[item][""]);
                //}

            }
        }

        /// <summary>
        /// 获得选中的行
        /// </summary>
        /// <param name="indexs">选中的行数</param>
        /// <returns>返回表中的序号 ID</returns>
        private int[] GetRowChecked(out int indexs)
        {
            indexs = 0;
          
            // DataGridCell cel=(sender as DataGridCell).
            int count = Convert.ToInt16(this.dataGridView1.Rows.Count.ToString());
            int[] id = new int[count-1];

            for (int i = 0; i < count; i++)
            {
                 checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells["IsCheck"];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true) //查找被选择的数据行
                {
                    //dataGridView1.CurrentRow.Cells[0].Value.ToString();//其中0为该行列数的index，或者你也可以这样写Cells["id"].Value
                    //int a = dataGridView1.SelectedRows.Index;
                   // dataGridView1.Rows[a].Cells["你想要的某一列的索引，想要几就写几"].Value;

                    id[i] = Convert.ToInt32(dataGridView1.Rows[i].Cells["ID"].Value);
                    indexs += 1;

                }
               
            }
            return id;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                //Console.WriteLine(e.RowIndex.ToString());
                //获取控件的值

                checkCell = (DataGridViewCheckBoxCell)this.dataGridView1.Rows[e.RowIndex].Cells["IsCheck"];
                Boolean flag = Convert.ToBoolean(checkCell.Value);

                if (flag == true)
                {
                    checkCell.Value = false;
                }
                else
                {
                    checkCell.Value = true;
                }

            }
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            int indexs;
            int[] id;

            id=GetRowChecked(out indexs);
            foreach (var item in id)
            {
                Console.WriteLine(item.ToString());
            }


            //Console.WriteLine(indexs.ToString());
        }
    }
}
