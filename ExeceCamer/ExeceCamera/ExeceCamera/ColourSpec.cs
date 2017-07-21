using System;
using System.Data;
using System.Windows.Forms;
using StandardDeviations;
using ExeceCamera.CameraSpec;
using System.Collections;

namespace ExeceCamera
{
    public partial class ColorSpec : Form
    {
        /// <summary>
        /// 良品系数
        /// </summary>
        private double k;

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
        /// 列名
        /// </summary>
        string[] columnNames =
        {
                "CH",
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





        public ColorSpec()
        {

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

       

        private void SaveIni_Click(object sender, EventArgs e)
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
            int[] id = new int[count - 1];

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
                GetSpace.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
            }
            else
            {
                GetSpace.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;

                label1.Visible = false;
                label2.Visible = false;
            }
        }


    }
}
