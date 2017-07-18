using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Hyet.Dal;
using Hxj.Model;
using Hxj.Data;
using System.Collections;
using Hyet.BLL;

namespace DigitalFactory.ReportForms
{
    public partial class ChartInfo : Form
    {
        public ChartInfo()
        {
            InitializeComponent();

            InitializeChart();

            this.myChart.GetToolTipText += new EventHandler<ToolTipEventArgs>(myChart_GetToolTipText);
        }

        private void InitializeChart()
        {
            myChart.ChartAreas.Clear();
            myChart.Series.Clear();

            #region 设置图表的属性
            //图表的背景色
            myChart.BackColor = Color.FromArgb(211, 223, 240);
            //图表背景色的渐变方式
            myChart.BackGradientStyle = GradientStyle.TopBottom;
            //图表的边框颜色、
            myChart.BorderlineColor = Color.FromArgb(26, 59, 105);
            //图表的边框线条样式
            myChart.BorderlineDashStyle = ChartDashStyle.Solid;
            //图表边框线条的宽度
            myChart.BorderlineWidth = 2;
            //图表边框的皮肤
            myChart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            #endregion

            #region 设置图表的标题
            Title title = new Title();
            //标题内容
            title.Text = "曲线图";
            //标题的字体
            title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            //标题字体颜色
            title.ForeColor = Color.FromArgb(26, 59, 105);
            //标题阴影颜色
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            //标题阴影偏移量
            title.ShadowOffset = 3;
            myChart.Titles.Add(title);
            #endregion

            #region 设置图例的属性
            //注意，需要把原来控件自带的图例删除掉
            this.myChart.Legends.Clear();

            Legend legend = new Legend("Default");
            legend.Alignment = StringAlignment.Center;
            legend.Docking = Docking.Bottom;
            legend.LegendStyle = LegendStyle.Column;
            this.myChart.Legends.Add(legend);

            // Add header separator of type line
            legend.HeaderSeparator = LegendSeparatorStyle.Line;
            legend.HeaderSeparatorColor = Color.Gray;

            LegendCellColumn firstColumn = new LegendCellColumn();
            firstColumn.ColumnType = LegendCellColumnType.SeriesSymbol;
            firstColumn.HeaderText = "Color";
            firstColumn.HeaderBackColor = Color.WhiteSmoke;
            myChart.Legends["Default"].CellColumns.Add(firstColumn);

            // Add Legend Text column
            LegendCellColumn secondColumn = new LegendCellColumn();
            secondColumn.ColumnType = LegendCellColumnType.Text;
            secondColumn.HeaderText = "Name";
            secondColumn.Text = "#LEGENDTEXT";
            secondColumn.HeaderBackColor = Color.WhiteSmoke;
            myChart.Legends["Default"].CellColumns.Add(secondColumn);

            // Add AVG cell column
            LegendCellColumn avgColumn = new LegendCellColumn();
            avgColumn.Text = "#AVG{N2}";
            avgColumn.HeaderText = "Avg";
            avgColumn.Name = "AvgColumn";
            avgColumn.HeaderBackColor = Color.WhiteSmoke;
            myChart.Legends["Default"].CellColumns.Add(avgColumn);

            // Add Total cell column
            LegendCellColumn totalColumn = new LegendCellColumn();
            totalColumn.Text = "#TOTAL{N1}";
            totalColumn.HeaderText = "Total";
            totalColumn.Name = "TotalColumn";
            totalColumn.HeaderBackColor = Color.WhiteSmoke;
            myChart.Legends["Default"].CellColumns.Add(totalColumn);

            // Set Min cell column attributes
            LegendCellColumn minColumn = new LegendCellColumn();
            minColumn.Text = "#MIN{N1}";
            minColumn.HeaderText = "Min";
            minColumn.Name = "MinColumn";
            minColumn.HeaderBackColor = Color.WhiteSmoke;
            myChart.Legends["Default"].CellColumns.Add(minColumn);

            // Set Max cell column attributes
            LegendCellColumn maxColumn = new LegendCellColumn();
            maxColumn.Text = "#MAX{N1}";
            maxColumn.HeaderText = "Max";
            maxColumn.Name = "MaxColumn";
            maxColumn.HeaderBackColor = Color.WhiteSmoke;
            myChart.Legends["Default"].CellColumns.Add(maxColumn);

            #endregion

            #region 设置图表区属性
            ChartArea chartArea = new ChartArea("Default");
            //设置Y轴刻度间隔大小
            chartArea.AxisY.Interval = 5;
            //设置Y轴的数据类型格式
            //chartArea.AxisY.LabelStyle.Format = "C";
            //设置背景色
            chartArea.BackColor = Color.FromArgb(64, 165, 191, 228);
            //设置背景渐变方式
            chartArea.BackGradientStyle = GradientStyle.TopBottom;
            //设置渐变和阴影的辅助背景色
            chartArea.BackSecondaryColor = Color.White;
            //设置边框颜色
            chartArea.BorderColor = Color.FromArgb(64, 64, 64, 64);
            //设置阴影颜色
            chartArea.ShadowColor = Color.Transparent;
            //设置X轴和Y轴线条的颜色
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            //设置X轴和Y轴线条的宽度
            chartArea.AxisX.LineWidth = 1;
            chartArea.AxisY.LineWidth = 1;
            //设置X轴和Y轴的标题
            chartArea.AxisX.Title = "时间";
            chartArea.AxisY.Title = "数值";
            //设置图表区网格横纵线条的颜色
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            //设置图表区网格横纵线条的宽度
            chartArea.AxisX.MajorGrid.LineWidth = 1;
            chartArea.AxisY.MajorGrid.LineWidth = 1;
            //设置坐标轴刻度线不延长出来
            chartArea.AxisX.MajorTickMark.Enabled = false;
            chartArea.AxisY.MajorTickMark.Enabled = false;
            //开启下面两句能够隐藏网格线条
            //chartArea.AxisX.MajorGrid.Enabled = false;
            //chartArea.AxisY.MajorGrid.Enabled = false;
            //设置X轴的显示类型及显示方式
            chartArea.AxisX.Interval = 0; //设置为0表示由控件自动分配
            chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Minutes;
            chartArea.AxisX.LabelStyle.IsStaggered = true;
            //chartArea.AxisX.MajorGrid.IntervalType = DateTimeIntervalType.Minutes;
            //chartArea.AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Minutes;
            chartArea.AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm:ss";
            //设置文本角度
            //chartArea.AxisX.LabelStyle.Angle = 45;
            //设置文本自适应
            chartArea.AxisX.IsLabelAutoFit = true;
            //设置X轴允许拖动放大
            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorX.Interval = 0;
            chartArea.CursorX.IntervalOffset = 0;
            chartArea.CursorX.IntervalType = DateTimeIntervalType.Minutes;
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisX.ScrollBar.IsPositionedInside = false;

            //设置中短线（还没看到效果）
            //chartArea.AxisY.ScaleBreakStyle.Enabled = true;
            //chartArea.AxisY.ScaleBreakStyle.CollapsibleSpaceThreshold = 47;
            //chartArea.AxisY.ScaleBreakStyle.BreakLineStyle = BreakLineStyle.Wave;
            //chartArea.AxisY.ScaleBreakStyle.Spacing = 2;
            //chartArea.AxisY.ScaleBreakStyle.LineColor = Color.Red;
            //chartArea.AxisY.ScaleBreakStyle.LineWidth = 10;

            myChart.ChartAreas.Add(chartArea);
            #endregion

            //线条2：主要曲线
            Series series = new Series("Default");
            //设置线条类型
            series.ChartType = SeriesChartType.Line;
            //线条宽度
            series.BorderWidth = 1;
            //阴影宽度
            series.ShadowOffset = 0;
            //是否显示在图例集合Legends
            series.IsVisibleInLegend = true;
            //线条上数据点上是否有数据显示
            series.IsValueShownAsLabel = true;
            //线条颜色
            series.Color = Color.MediumPurple;
            //设置曲线X轴的显示类型
            series.XValueType = ChartValueType.DateTime;
            //设置数据点的类型
            series.MarkerStyle = MarkerStyle.Circle;
            //线条数据点的大小
            series.MarkerSize = 5;
            myChart.Series.Add(series);

            //手动构造横坐标数据
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("TheTime", typeof(DateTime)); //注意typeof
            dataTable.Columns.Add("TheValue", typeof(double)); //注意typeof
            Random random = new Random(); //随机数
            DateTime dateTime = System.DateTime.Now;
            for (int n = 0; n < 3; n++)
            {
                dateTime = dateTime.AddSeconds(10);
                DataRow dr = dataTable.NewRow();
                dr["TheTime"] = dateTime;
                dr["TheValue"] = random.Next(0, 101);
                dataTable.Rows.Add(dr);
            }
            for (int n = 3; n < 1000; n++)
            {
                dateTime = dateTime.AddSeconds(30);
                DataRow dr = dataTable.NewRow();
                dr["TheTime"] = dateTime;
                dr["TheValue"] = random.Next(0, 101);
                dataTable.Rows.Add(dr);
            }

            //线条1：下限横线
            Series seriesMin = new Series("Min");
            seriesMin.ChartType = SeriesChartType.Line;
            seriesMin.BorderWidth = 1;
            seriesMin.ShadowOffset = 0;
            seriesMin.IsVisibleInLegend = true;
            seriesMin.IsValueShownAsLabel = false;
            seriesMin.Color = Color.Red;
            seriesMin.XValueType = ChartValueType.DateTime;
            seriesMin.MarkerStyle = MarkerStyle.None;
            myChart.Series.Add(seriesMin);

            //线条3：上限横线
            Series seriesMax = new Series("Max");
            seriesMax.ChartType = SeriesChartType.Line;
            seriesMax.BorderWidth = 1;
            seriesMax.ShadowOffset = 0;
            seriesMax.IsVisibleInLegend = true;
            seriesMax.IsValueShownAsLabel = false;
            seriesMax.Color = Color.Red;
            seriesMax.XValueType = ChartValueType.DateTime;
            seriesMax.MarkerStyle = MarkerStyle.None;
            myChart.Series.Add(seriesMax);

            //设置X轴的最小值为第一个点的X坐标值
            chartArea.AxisX.Minimum = Convert.ToDateTime(dataTable.Rows[0]["TheTime"]).ToOADate();

            //开始画线
            foreach (DataRow dr in dataTable.Rows)
            {
                series.Points.AddXY(dr["TheTime"], dr["TheValue"]);

                seriesMin.Points.AddXY(dr["TheTime"], 15); //设置下线为15
                seriesMax.Points.AddXY(dr["TheTime"], 30); //设置上限为30
            }
        }

        private void myChart_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                DataPoint dp = e.HitTestResult.Series.Points[i];
                e.Text = string.Format("时间:{0}，数值:{1:F1} ", DateTime.FromOADate(dp.XValue), dp.YValues[0]);
            }
        }

        private void ChartInfo_Load(object sender, EventArgs e)
        {

        }
    }
}