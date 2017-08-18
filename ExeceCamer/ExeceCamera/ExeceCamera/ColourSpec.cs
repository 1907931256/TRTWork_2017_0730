using System;
using System.Data;
using System.Windows.Forms;
using StandardDeviations;
using ExeceCamera.CameraSpec;
using System.Collections;
using System.Collections.Generic;

namespace ExeceCamera
{
    public partial class ColorSpec : Form
    {
        /// <summary>
        /// 良品系数
        /// </summary>
        private double k;
        /// <summary>
        /// 越界下标
        /// </summary>
        private List<int> outIndexs;

        private string fileName;
        private DataTable dt;
        private string filePath_ini;
        /// <summary>
        /// 表示上限系数
        /// </summary>
        private double upRatio;
        /// <summary>
        /// 表示下限系数
        /// </summary>
        private double downRatio;
        private bool save_flag;
        private static bool checkErroStart;
        private DAPublic dapublic_;
        private ColorSpec_ colorspec_;

        private System_ini sys_ini;


        /// <summary>
        /// 表示一个复选框
        /// </summary>
        private DataGridViewCheckBoxCell checkCell;


        /// <summary>
        /// 列名 全
        /// </summary>
        private string[] columnNames =
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
                "R/G_24",

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
                "B/G_24",

                "Stdev_1",
                "Stdev_2",
                "Stdev_3",
                "Stdev_4",
                "Stdev_5",
                "Stdev_6",
                "Stdev_7",
                "Stdev_8",
                "Stdev_9",
                "Stdev_10",
                "Stdev_11",
                "Stdev_12",
                "Stdev_13",
                "Stdev_14",
                "Stdev_15",
                "Stdev_16",
                "Stdev_17",
                "Stdev_18",
                "Stdev_19",
                "Stdev_20",
                "Stdev_21",
                "Stdev_22",
                "Stdev_23",
                "Stdev_24",

                "RTV",
                "RTH",
                "RBV",
                "RBH",
                "LBV",
                "LBH",
                "LTV",
                "LTH",
                "Grey-Mean-UP",
                "Grey-Mean-Down",
                "Grey-Mean-Left",
                "Grey-Mean-Right",
                "Grey-Stdev-UP",
                "Grey-Stdev-Down",
                "Grey-Stdev-Left",
                "Grey-Stdev-Right",
                "Grey-R/G-UP",
                "Grey-R/G-Down",
                "Grey-R/G-Left",
                "Grey-R/G-Right",
                "Grey-B/G-UP",
                "Grey-B/G-Down",
                "Grey-B/G-Left",
                "Grey-B/G-Right"



            };

        /// <summary>
        /// 列名 R_G
        /// </summary>
        private string[] columnNames_RG =
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

        /// <summary>
        /// 列名 B_G
        /// </summary>
        private string[] columnNames_BG =
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

        /// <summary>
        /// 列名 STDEV
        /// </summary>
        private string[] columnNames_STDEV =
        {
             "Stdev_1",
                "Stdev_2",
                "Stdev_3",
                "Stdev_4",
                "Stdev_5",
                "Stdev_6",
                "Stdev_7",
                "Stdev_8",
                "Stdev_9",
                "Stdev_10",
                "Stdev_11",
                "Stdev_12",
                "Stdev_13",
                "Stdev_14",
                "Stdev_15",
                "Stdev_16",
                "Stdev_17",
                "Stdev_18",
                "Stdev_19",
                "Stdev_20",
                "Stdev_21",
                "Stdev_22",
                "Stdev_23",
                "Stdev_24"
        };




        public ColorSpec()
        {
            outIndexs = new List<int>();
            k = 0.3;
            sys_ini = new System_ini();
            colorspec_ = new ColorSpec_();
            dapublic_ = new DAPublic();
            upRatio = 0;
            downRatio = 0;
            save_flag = false;
            checkErroStart = false;
            filePath_ini = System.Windows.Forms.Application.StartupPath;
            InitializeComponent();

        }

        private void LoadCsvFile_Click(object sender, EventArgs e)
        {


            HildControl(save_flag);

            openFileDialog1.InitialDirectory = "e:\\";
            openFileDialog1.Filter = "(*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                fileName = openFileDialog1.FileName;

                dt = CSVFileHelper.OpenCSV(fileName);
                save_flag = true;
            }
            HildControl(save_flag);
            save_flag = false;

        }


        /// <summary>
        /// 规格计算 找出越界下标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Compute_Click(object sender, EventArgs e)
        {

            //得到每列的数据
            //找出超标照片的下标
            //根据下标找到照片
            //让用户确认照片的去留
            //根据用户的选择确定系数

            ArrayList indexsCount = new ArrayList();

            foreach (var item in columnNames)
            {
                double[] r_g_1 = dapublic_.GetDataCateReturnDouble(dt, item, "0");
                int[] indexs;
                dapublic_.GetErrorDataIndex(r_g_1, k, out indexs);
                indexsCount.AddRange(indexs);

            }
            int[] OutBounds = new int[indexsCount.Count];
            //test 可以成功得到超标数据下标
            for (int i = 0; i < OutBounds.Length; i++)
            {
                indexsCount.CopyTo(OutBounds);
            }
            //数据超标的下标具有唯一性

            int[] reaminData;
            dapublic_.GetRemoveSameData(OutBounds, out reaminData);

            DataGrewShowMethod(reaminData);

            outIndexs.AddRange(reaminData);//超标数据加入到
        }

        /// <summary>
        /// 更具用户选择，保留照片的去留 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
#if false  //人工确认数据
            int index_;
            int[] ids = GetRowChecked(out index_);
            //outIndexs.AddRange(ids);//选中代表移除，不选择代表留下
            foreach (var item in ids)
            {
                outIndexs.Remove(item);//选中代表保留
            }
            
            int i = 0;
            foreach (var item in columnNames_RG)
            {
                double[] remainData;//剩下的数据
                double[] data_ = dapublic_.GetDataCateReturnDouble(dt, item, "0");
                
                dapublic_.GetNewData(data_, outIndexs.ToArray(), out remainData);

                sys_ini.MaxRG += dapublic_.GetMax(remainData).ToString() + ",";
                sys_ini.MinRG += dapublic_.GetMin(remainData).ToString() + ",";
                i++;
            }
            foreach (var item in columnNames_BG)
            {
                double[] remainData;//剩下的数据
                double[] data_ = dapublic_.GetDataCateReturnDouble(dt, item, "0");

                dapublic_.GetNewData(data_, outIndexs.ToArray(), out remainData);

                sys_ini.MAXBG += dapublic_.GetMax(remainData).ToString() + ",";
                sys_ini.MinBG += dapublic_.GetMin(remainData).ToString() + ",";
                i++;
            }
#endif

            SaveInIni();

        }


        /// <summary>
        /// 根据下标在控件上显示越界的行
        /// </summary>
        /// <param name="reaminData"></param>
        private void DataGrewShowMethod(int[] reaminData)
        {
            DataRow row;
            DataTable errorData_dt = new DataTable("errorData");
            errorData_dt.Columns.Add("Filename");
            errorData_dt.Columns.Add("ID");

            //添加选择框
            DataGridViewCheckBoxColumn newColumn = new DataGridViewCheckBoxColumn();
            newColumn.HeaderText = "√表示可以通过";
            newColumn.Name = "IsCheck";

            if (dataGridView1.ColumnCount < 3)
            {
                dataGridView1.Columns.Add(newColumn);
            }

            for (int i = 0; i < reaminData.Length; i++)
            {
                row = errorData_dt.NewRow();
                row["ID"] = reaminData[i];
                row["Filename"] = dt.Rows[reaminData[i]]["Filename"];
                errorData_dt.Rows.Add(row);
            }

            dataGridView1.DataSource = errorData_dt;
            dataGridView1.Visible = true;
        }




        /// <summary>
        /// 获得选中的行
        /// </summary>
        /// <param name="indexs">一共选中多少行</param>
        /// <returns>返回表中的序号 ID</returns>
        private int[] GetRowChecked(out int indexs)
        {
            indexs = 0;

            // DataGridCell cel=(sender as DataGridCell).
            int count = Convert.ToInt16(this.dataGridView1.Rows.Count.ToString());
            int[] id = new int[count];

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



        //private void GetSfrCatDate()
        //{
        //    upRatio = Convert.ToDouble(textBox1.Text);
        //    downRatio = Convert.ToDouble(textBox2.Text);

        //    //保存 标准偏差
        //    double stdev;
        //    double[] data;

        //    string SFR4BackColor = string.Empty;
        //    string SFR4BackColorFlash = string.Empty;
        //    string SFR4FrontColor = string.Empty;
        //    string SFR4FrontColorFlash = string.Empty;






        //    for (int i = 0; i < columnNames.Length; i++)
        //    {
        //        if (i<=12 && i>=0)
        //        {
        //            //SFR4FrontColor 前后
        //            GetDataCate(dt, columnNames[i], out data, "0");
        //            double[] new_data;
        //            double down;
        //            if (data.Length!=0)
        //            {
        //                dapublic_.GetRemoveNumData(data, out new_data);

        //                stdev = dapublic_.GetOffect(new_data);


        //                down = new_data[0] + stdev * downRatio;
        //                SFR4BackColor += down.ToString("f2") + ",";//数据上限
        //            }




        //            GetDataCate(dt, columnNames[i], out data, "1");

        //            if (data.Length!=0)
        //            {
        //                dapublic_.GetRemoveNumData(data, out new_data);

        //                stdev = dapublic_.GetOffect(new_data);


        //                down = new_data[0] + stdev * downRatio;
        //                SFR4FrontColor += down.ToString("f2") + ",";//数据上限
        //            }


        //        }
        //        else if (i >=13)
        //        {
        //            GetDataCate(dt, columnNames[i], out data, "0");
        //            double[] new_data;
        //            double down;
        //            if (data.Length!=0)
        //            {
        //                dapublic_.GetRemoveNumData(data, out new_data);


        //                stdev = dapublic_.GetOffect(new_data);


        //                down = new_data[new_data.Length - 1] + stdev * downRatio;
        //                SFR4BackColor += down.ToString("f2") + ",";//数据上限
        //            }


        //            GetDataCate(dt, columnNames[i], out data, "1");
        //            if (data.Length!=0)
        //            {
        //                dapublic_.GetRemoveNumData(data, out new_data);

        //                stdev = dapublic_.GetOffect(new_data);


        //                down = new_data[new_data.Length - 1] + stdev * downRatio;
        //                SFR4FrontColor += down.ToString("f2") + ",";//数据上限
        //            }
        //        }

        //    }
        //    if (SFR4BackColor!=string.Empty)
        //    {
        //        INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "SFR", "SFR4BackColor", SFR4BackColor);
        //    }
        //    if (SFR4FrontColor!=string.Empty)
        //    {
        //        INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "SFR", "SFR4FrontColor", SFR4FrontColor);
        //    }

        //}


        private void ColourSpec_Load(object sender, EventArgs e)
        {
            HildControl(false);
        }


        private void HildControl(bool bl)
        {
            if (bl)
            {
                //GetSpace.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                label1.Visible = true;
                label2.Visible = true;

                button1.Visible = false;

            }
            else
            {
                //GetSpace.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;

                label1.Visible = false;
                label2.Visible = false;

                button1.Visible = false;
            }
        }


        /// <summary>
        /// 按比例缩放数据
        /// 默认采用10%的比例缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProportionBtn_Click(object sender, EventArgs e)
        {
            sys_ini.MaxRG = sys_ini.MinRG = sys_ini.MaxRG1 = sys_ini.MinRG1 = sys_ini.MAXBG = sys_ini.MinBG = sys_ini.MAXBG1 = sys_ini.MinBG1="";

            foreach (var item in columnNames_RG)
            {
                double[] remainData;//剩下的数据
                double[] data_ = dapublic_.GetDataCateReturnDouble(dt, item, "0");

                dapublic_.RemoveProportionData(data_, 0.1, out remainData);

                sys_ini.MaxRG += dapublic_.GetMax(remainData).ToString() + ",";
                sys_ini.MinRG += dapublic_.GetMin(remainData).ToString() + ",";


                data_ = dapublic_.GetDataCateReturnDouble(dt, item, "1");

                dapublic_.RemoveProportionData(data_, 0.1, out remainData);

                sys_ini.MaxRG1 += dapublic_.GetMax(remainData).ToString() + ",";
                sys_ini.MinRG1 += dapublic_.GetMin(remainData).ToString() + ",";


            }
            sys_ini.MaxRG = sys_ini.MaxRG + "1.5,1.5,1.5,1.5,";//补全后面部分数据
            sys_ini.MinRG = sys_ini.MinRG + "1.5,1.5,1.5,1.5,";

            sys_ini.MaxRG1 = sys_ini.MaxRG1 + "1.5,1.5,1.5,1.5,";//补全后面部分数据
            sys_ini.MinRG1 = sys_ini.MinRG1 + "1.5,1.5,1.5,1.5,";

            foreach (var item in columnNames_BG)
            {
                double[] remainData;//剩下的数据
                double[] data_ = dapublic_.GetDataCateReturnDouble(dt, item, "0");

                dapublic_.RemoveProportionData(data_, 0.1, out remainData);

                sys_ini.MAXBG += dapublic_.GetMax(remainData).ToString() + ",";
                sys_ini.MinBG += dapublic_.GetMin(remainData).ToString() + ",";

                data_ = dapublic_.GetDataCateReturnDouble(dt, item, "1");

                dapublic_.RemoveProportionData(data_, 0.1, out remainData);

                sys_ini.MAXBG1 += dapublic_.GetMax(remainData).ToString() + ",";
                sys_ini.MinBG1 += dapublic_.GetMin(remainData).ToString() + ",";


            }
            sys_ini.MAXBG = sys_ini.MAXBG + "1.5,1.5,1.5,1.5,";//补全后面部分数据
            sys_ini.MinBG = sys_ini.MinBG + "1.5,1.5,1.5,1.5,";
            sys_ini.MAXBG1 = sys_ini.MAXBG1 + "1.5,1.5,1.5,1.5,";//补全后面部分数据
            sys_ini.MinBG1 = sys_ini.MinBG1 + "1.5,1.5,1.5,1.5,";

            button1.Visible = true;





        }

        /// <summary>
        /// 保存数据到ini文件
        /// </summary>
        private void SaveInIni()
        {
            INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "System", "MinRG", sys_ini.MinRG);
            INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "System", "MaxRG", sys_ini.MaxRG);
            INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "System", "MAXBG", sys_ini.MAXBG);
            INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "System", "MinBG", sys_ini.MinBG);

            INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "System", "MinRG1", sys_ini.MinRG1);
            INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "System", "MaxRG1", sys_ini.MaxRG1);
            INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "System", "MAXBG1", sys_ini.MAXBG1);
            INIOperationClass.INIWriteValue(filePath_ini + "\\system1.ini", "System", "MinBG1", sys_ini.MinBG1);

            MessageBox.Show("执行完毕！");
        }



    }
}
