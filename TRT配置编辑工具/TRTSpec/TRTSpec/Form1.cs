using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TRTSpec
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// params路径存放位置
        /// </summary>
        private string file_pram;

        /// <summary>
        /// specs存放路径
        /// </summary>
        private string file_specs;

        private Loading loading_;

        public Form1()
        {
            loading_ = new Loading();

            InitializeComponent();

            file_pram = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "params.ini";
            file_specs = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "specs.ini";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool vaild;
            rowMergeView1.DataSource = loading_.ReadParams_INI(file_pram, out vaild);
            this.rowMergeView1.MergeColumnNames.Add("Group");

            dataGridView2.DataSource = loading_.ReadSpecs_INI(file_specs, out vaild);
            if (vaild==false)
            {
                MessageBox.Show("规格不匹配,请查找下一行规格！");
            }


        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var IL = rowMergeView1.DataSource;
            List<Params> ILS = (List<Params>)IL;
            loading_.SaveData_Params(file_pram, ILS);

            var spec = dataGridView2.DataSource;
            List<Specs> specList = (List<Specs>)spec;
            loading_.SaveData_Specs(file_specs, specList);

            //bool vaild;
            //dataGridView1.DataSource = loading_.ReadSpecs_INI(file_pram, out vaild);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //bool vaild;
            //dataGridView1.DataSource = loading_.ReadSpecs_INI(file_pram, out vaild);
        }

       
    }
}
