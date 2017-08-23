using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IntegrationSys.Flow;
using IntegrationSys.LogUtil;
using System.Threading;
using IntegrationSys.Equipment;
using IntegrationSys.Net;
using System.Diagnostics;
using IntegrationSys.Result;
using System.IO;

namespace IntegrationSys
{
    public partial class Form1 : Form
    {
        const int COLUMN_ITEM_COUNT = 0;
        const int COLUMN_ITEM_NAME = 1;
        const int COLUMN_ITEM_CONTENT = 2;
        const int COLUMN_ITEM_SPEC = 3;
        const int COLUMN_ITEM_VALUE = 4;
        const int COLUMN_ITEM_RESULT = 5;
        const int COLUMN_ITEM_STATUS = 6;
        const int COLUMN_ITEM_DURATION = 7;
        const int COLUMN_ITEM_MAX = 8;

        private int totalTime_ = 0;

        private delegate void ItemExecuteComplete(FlowItem flowItem);

        public Form1()
        {
            InitializeComponent();

            timer.Tick += new EventHandler(FlowTimerTickHandler);
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            LoadFlowItemList();
            SubscribeEquipmentEvent();
            SubscribeLiteDataServerEvent();

            Statistic.Instance.Load();

            this.statusStrip.SizeChanged += new EventHandler(delegate
            {
                SetStripProgressBarWidth();
            });
            InitStatusStrip();
            UpdateStatisticInfo();

            //删除图像处理软件的结果文件
            try
            {
                File.Delete(@"C:\TRT_Camera_Tester_Picture\test result\result.txt");
            }
            catch (Exception)
            {
                Log.Debug("Delete Camera result fail");
            }
        }


        /// <summary>
        /// 将FlowItem加载到DataGridView中
        /// </summary>
        private void LoadFlowItemList()
        {
            FlowControl flowControl = FlowControl.Instance;

            flowGridView.ColumnCount = COLUMN_ITEM_MAX;

            //flowGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            //flowGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            //flowGridView.ColumnHeadersDefaultCellStyle.Font =
            //    new Font(flowGridView.Font, FontStyle.Bold);


            flowGridView.Columns[COLUMN_ITEM_COUNT].Name = "测试项";
            flowGridView.Columns[COLUMN_ITEM_NAME].Name = "测试项名称";
            flowGridView.Columns[COLUMN_ITEM_CONTENT].Name = "测试内容";
            flowGridView.Columns[COLUMN_ITEM_SPEC].Name = "规格值";
            flowGridView.Columns[COLUMN_ITEM_VALUE].Name = "实测值";
            flowGridView.Columns[COLUMN_ITEM_RESULT].Name = "结果";
            flowGridView.Columns[COLUMN_ITEM_STATUS].Name = "状态";
            flowGridView.Columns[COLUMN_ITEM_DURATION].Name = "测试时间";

            for (int i = 0; i < COLUMN_ITEM_MAX; i++)   
            {
                flowGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            flowGridView.Columns[COLUMN_ITEM_COUNT].DefaultCellStyle = flowGridView.ColumnHeadersDefaultCellStyle;
            flowGridView.Columns[COLUMN_ITEM_NAME].DefaultCellStyle = flowGridView.ColumnHeadersDefaultCellStyle;
            flowGridView.Columns[COLUMN_ITEM_CONTENT].DefaultCellStyle = flowGridView.ColumnHeadersDefaultCellStyle;
            flowGridView.Columns[COLUMN_ITEM_SPEC].DefaultCellStyle = flowGridView.ColumnHeadersDefaultCellStyle;

            if (flowControl.FlowItemList != null)
            {
                int itemCount = 1;
                for (int i = 0; i < flowControl.FlowItemList.Count; i++)
                {
                    FlowItem flowItem = flowControl.FlowItemList[i];
                    flowItem.RowIndex = flowGridView.RowCount;

                    if (!flowItem.Item.Property.Hide && !flowItem.Item.Property.Disable)
                    {
                        if (flowItem.SpecValueList != null)
                        {
                            foreach (SpecValue specValue in flowItem.SpecValueList)
                            {
                                if (!specValue.Disable)
                                {
                                    List<string> rowValueList = new List<string>();
                                    rowValueList.Add(Convert.ToString(itemCount));
                                    rowValueList.Add(flowItem.Name);
                                    rowValueList.Add(specValue.SpecDescription);
                                    rowValueList.Add(specValue.Spec);
                                    rowValueList.Add(string.IsNullOrEmpty(specValue.MeasuredValue) ? "" : specValue.MeasuredValue);
                                    rowValueList.Add(string.IsNullOrEmpty(specValue.JudgmentResult) ? "" : specValue.JudgmentResult);
                                    rowValueList.Add("");
                                    rowValueList.Add("");
                                    flowGridView.Rows.Add(rowValueList.ToArray());
                                }
                            }

                            if (flowGridView.RowCount - flowItem.RowIndex > 1)
                            {
                                flowGridView.Merge(flowItem.RowIndex, COLUMN_ITEM_COUNT, flowGridView.RowCount - flowItem.RowIndex, 1);
                                flowGridView.Merge(flowItem.RowIndex, COLUMN_ITEM_NAME, flowGridView.RowCount - flowItem.RowIndex, 1);
                                flowGridView.Merge(flowItem.RowIndex, COLUMN_ITEM_STATUS, flowGridView.RowCount - flowItem.RowIndex, 1);
                                flowGridView.Merge(flowItem.RowIndex, COLUMN_ITEM_DURATION, flowGridView.RowCount - flowItem.RowIndex, 1);
                            }
                        }
                        else
                        {
                            List<string> rowValueList = new List<string>();
                            rowValueList.Add(Convert.ToString(itemCount));
                            rowValueList.Add(flowItem.Name);
                            rowValueList.Add("");
                            rowValueList.Add("");
                            rowValueList.Add("");
                            rowValueList.Add("");
                            rowValueList.Add("");
                            rowValueList.Add("");
                            flowGridView.Rows.Add(rowValueList.ToArray());
                        }

                        itemCount++;
                    }
                }                  
            }
        }

        private void ReloadFlowTest()
        {
            if (FlowControl.Instance.FlowStatus == FlowControl.FLOW_STATUS_COMPLETE)
            {
                FlowControl.Instance.Reload();
                flowGridView.Rows.Clear();
                LoadFlowItemList();
                ShowSN(string.Empty);
                InitStatusStrip();
                ResultRecord.Clear();

                FlowControl.Instance.FlowStatus = FlowControl.FLOW_STATUS_INIT;
                FlowControl.Instance.FlowCompleteReason = FlowControl.FLOW_COMPLETE_NORMAL;
                FlowControl.Instance.FlowResult = FlowControl.FLOW_RESULT_INIT;
            }
        }

        /// <summary>
        /// 开始启动测试
        /// </summary>
        private void StartFlowTest()
        {
            FlowControl flowControl = FlowControl.Instance;

            if (flowControl.FlowStatus == FlowControl.FLOW_STATUS_INIT)
            {
                GetPhoneIp();

                flowControl.FlowStatus = FlowControl.FLOW_STATUS_RUNNING;
                foreach (FlowItem flowItem in flowControl.FlowItemList)
                {
                    if (flowItem.Status == FlowItem.STATUS_INIT && !flowItem.Item.Property.Disable && CheckDepend(flowItem.DependSet))
                    {
                        StartFlowItem(flowItem);
                    }
                }

                StartFlowTimer();
            }
        }

        private void StartFlowItem(FlowItem flowItem)
        {
            if (flowItem != null && !flowItem.Item.Property.Disable)
            {
                if (!flowItem.IsSkip())
                {
                    FlowItemExecutor executor = new FlowItemExecutor(flowItem);
                    executor.ExecuteFinish += ItemExecuteFinish;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(executor.ThreadProc));
                    flowItem.Status = FlowItem.STATUS_RUNNING;
                    UpdateRunning(flowItem);
                }
                else
                {
                    //此项不需要执行，直接跳过执行，并将实测值赋值为Skip，测试结果赋值为失败
                    if (flowItem.SpecValueList != null && flowItem.SpecValueList.Count > 0)
                    {
                        foreach (SpecValue specValue in flowItem.SpecValueList)
                        {
                            specValue.MeasuredValue = "Skip";
                            specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                        }
                    }
                    flowItem.Status = FlowItem.STATUS_FINISH;
                    UpdateFinish(flowItem);
                    NextFlowItem(flowItem);
                }          
            }
        }

        private void StopFlowTest()
        {
            FlowControl flowControl = FlowControl.Instance;

            if (flowControl.FlowStatus == FlowControl.FLOW_STATUS_RUNNING
             || flowControl.FlowStatus == FlowControl.FLOW_STATUS_PAUSE)
            {
                foreach (FlowItem flowItem in flowControl.FlowItemList)
                {
                    if (flowItem.Status == FlowItem.STATUS_INIT && !flowItem.Item.Property.Disable)
                    {
                        if (flowItem.SpecValueList != null && flowItem.SpecValueList.Count > 0)
                        {
                            foreach (SpecValue specValue in flowItem.SpecValueList)
                            {
                                specValue.MeasuredValue = CommonString.NOT_TEST;
                                specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                            }
                        }
                        flowItem.Status = FlowItem.STATUS_FINISH;
                        UpdateFinish(flowItem);
                    }
                }
            }
        }

        /// <summary>
        /// 手动点击流程测试开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartFlowTest();
        }

        private void 搜索SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadFlowTest();
        }

        /// <summary>
        /// FlowItem测试项完成时间响应函数,此函数运行在Item执行线程中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="?"></param>
        private void ItemExecuteFinish(object sender, ExecuteFinishEventArgs e)
        {
            Log.Debug("Item[" + e.FlowItem.Id + "] " + e.FlowItem.Item.Property.Name + " ItemExecuteFinish");
            this.BeginInvoke(new ItemExecuteComplete(ItemExecuteCompleteHandler), e.FlowItem);
        }

        /// <summary>
        /// 在主线程中运行的测试项完成事件响应函数
        /// </summary>
        /// <param name="flowItem"></param>
        private void ItemExecuteCompleteHandler(FlowItem flowItem)
        {
            Log.Debug("Item[" + flowItem.Id + "] " + flowItem.Item.Property.Name + " ItemExecuteCompleteHandler");
            flowItem.Status = FlowItem.STATUS_FINISH;

            UpdatePhoneInfo(flowItem);
            UpdateFinish(flowItem);

            if (!flowItem.IsPass() && flowItem.SwitchType > 0)  //检查是否需要switch
            {
                Switch(flowItem);
            }
            else if (!flowItem.IsPass() && flowItem.AlarmType > 0)  //检查是否需要alarm
            {

            }
            else
            {
                NextFlowItem(flowItem);
                toolStripProgressBar.PerformStep();
            }

            if (FlowComplete())
            {
                FlowCompleteHandler();
            }
        }

        /// <summary>
        /// 在主线程中运行的流程测试完成事件响应函数
        /// </summary>
        private void FlowCompleteHandler()
        {
            StopFlowTimer();
            FlowControl.Instance.FlowStatus = FlowControl.FLOW_STATUS_COMPLETE;

            if (FlowControl.Instance.FlowResult == FlowControl.FLOW_RESULT_INIT)
            {
                bool pass = true;
                foreach (FlowItem flowItem in FlowControl.Instance.FlowItemList)
                {
                    if (!flowItem.Item.Property.Disable && !flowItem.IsAuxiliaryItem() && !flowItem.IsPass())
                    {
                        pass = false;
                        break;
                    }
                }

                FlowControl.Instance.FlowResult = pass ? FlowControl.FLOW_RESULT_PASS : FlowControl.FLOW_RESULT_FAIL;
            }
            else if (FlowControl.Instance.FlowResult == FlowControl.FLOW_RESULT_EXCEPTION)
            {
                //复位设备
                string resp;
                EquipmentCmd.Instance.SendCommand(CommonString.CMD_RESET, String.Empty, out resp);
            }

            ShowResult();

            if (FlowControl.Instance.FlowResult == FlowControl.FLOW_RESULT_FAIL)
            {
                Statistic.Instance.IncreaseFailNum();
            }
            Statistic.Instance.IncreaseTotalNum();

            UpdateStatisticInfo();
            Statistic.Instance.Save();

            if (FlowControl.Instance.FlowCompleteReason != FlowControl.FLOW_COMPLETE_STOP)
            {
                ResultRecord.Record(AppInfo.PhoneInfo.SN);

                if (0 == NetUtil.GetStationIndex())
                {
                    CompleteHandler(0);
                }
                else
                {
                    LiteDataClient.Instance.SendCompleteFlag(NetUtil.GetStationIndex());
                }
            }

            TimeLog.Save();

            DataReport.Save();
        }

        private void ShowSN(string sn)
        {
            labelSN.Text = sn;
        }


        /// <summary>
        /// 测试结果显示
        /// </summary>
        private void ShowResult()
        {
            if (FlowControl.Instance.FlowResult == FlowControl.FLOW_RESULT_PASS)
            {
                ShowSN("PASS");
            }
            else if (FlowControl.Instance.FlowResult == FlowControl.FLOW_RESULT_FAIL)
            {
                ShowSN("FAIL");
            }
            else if (FlowControl.Instance.FlowResult == FlowControl.FLOW_RESULT_EXCEPTION)
            {
                ShowSN("STOP");
            }
        }

        /// <summary>
        /// 更新手机状态信息
        /// </summary>
        /// <param name="flowItem"></param>
        private void UpdatePhoneInfo(FlowItem flowItem)
        {
            if (flowItem.Name == "SN")
            {
                //更新SN
                if (flowItem.SpecValueList != null && flowItem.SpecValueList.Count == 1)
                {
                    AppInfo.PhoneInfo.SN = flowItem.SpecValueList[0].MeasuredValue;
                    ShowSN(AppInfo.PhoneInfo.SN);
                }
            }
            else if (flowItem.Name == "IP")
            {
                //更新IP
                if (flowItem.SpecValueList != null && flowItem.SpecValueList.Count == 1)
                {
                    AppInfo.PhoneInfo.IP = flowItem.SpecValueList[0].MeasuredValue;
                    PhoneIpManager.Instance.Add(AppInfo.PhoneInfo.IP);
                }
            }
        }

        /// <summary>
        /// 在UI线程中更新控件并更新FlowControl数据
        /// </summary>
        /// <param name="flowItem"></param>
        private void UpdateData(FlowItem flowItem)
        {
            switch (flowItem.Status)
            {
                case FlowItem.STATUS_RUNNING:
                    UpdateRunning(flowItem);
                    break;

                case FlowItem.STATUS_FINISH:
                    UpdateFinish(flowItem);
                    break;

                default:
                    break;
            }
        }

        private void UpdateRunning(FlowItem flowItem)
        {
            if (flowItem.Status == FlowItem.STATUS_RUNNING && !flowItem.Item.Property.Hide)
            {
                DataGridViewCell cell = flowGridView.Rows[flowItem.RowIndex].Cells[COLUMN_ITEM_STATUS];
                if (cell != null)
                {
                    cell.Value = "测试中";
                    cell.Style.BackColor = Color.Lime;
                }
            }
        }


        /// <summary>
        /// 测试完成后数据更新
        /// </summary>
        /// <param name="flowItem"></param>
        private void UpdateFinish(FlowItem flowItem)
        {
            if (flowItem.Status == FlowItem.STATUS_FINISH && !flowItem.Item.Property.Hide)
            {
                if (flowItem.SpecValueList != null)
                {
                    int rowOffset = 0;
                    for (int i = 0; i < flowItem.SpecValueList.Count; i++)
                    {
                        if (!flowItem.SpecValueList[i].Disable)
                        {
                            DataGridViewCell valueCell = flowGridView.Rows[flowItem.RowIndex + rowOffset].Cells[COLUMN_ITEM_VALUE];
                            if (valueCell != null)
                            {
                                valueCell.Value = flowItem.SpecValueList[i].MeasuredValue;
                            }

                            DataGridViewCell resultCell = flowGridView.Rows[flowItem.RowIndex + rowOffset].Cells[COLUMN_ITEM_RESULT];
                            if (resultCell != null)
                            {
                                resultCell.Value = flowItem.SpecValueList[i].JudgmentResult;
                                if (!CommonString.RESULT_SUCCEED.Equals(flowItem.SpecValueList[i].JudgmentResult))
                                {
                                    resultCell.Style.BackColor = Color.Red;
                                }
                            }
                            rowOffset++;
                        }
                    }
                }

                DataGridViewCell statusCell = flowGridView.Rows[flowItem.RowIndex].Cells[COLUMN_ITEM_STATUS];
                if (statusCell != null)
                {
                    statusCell.Value = "完成";
                    statusCell.Style.BackColor = flowGridView.DefaultCellStyle.BackColor;
                }
                DataGridViewCell durationCell = flowGridView.Rows[flowItem.RowIndex].Cells[COLUMN_ITEM_DURATION];
                if (durationCell != null)
                {
                    durationCell.Value = Convert.ToString(flowItem.Duration) + "ms";
                }
            }
        }

        /// <summary>
        /// 依赖项目检测
        /// </summary>
        /// <param name="dependSet"></param>
        /// <returns></returns>
        private bool CheckDepend(HashSet<int> dependSet)
        {
            if (dependSet == null) return true;
            FlowControl flowControl = FlowControl.Instance;
            foreach (int id in dependSet)
            {
                FlowItem flowItem = flowControl.GetFlowItem(id);
                if (flowItem == null)
                {
                    return false;
                }
                else if (flowItem.Item.Property.Disable)
                {
                    if (!CheckDepend(flowItem.DependSet)) return false;
                }
                else if (flowItem.Status != FlowItem.STATUS_FINISH)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 执行下一条流程
        /// </summary>
        /// <param name="flowItem"></param>
        private void NextFlowItem(FlowItem flowItem)
        {
            if (flowItem.BedependSet != null)
            {
                FlowControl flowControl = FlowControl.Instance;
                foreach (int id in flowItem.BedependSet)
                {
                    FlowItem bedependItem = flowControl.GetFlowItem(id);
                    if (bedependItem != null && bedependItem.Status == FlowItem.STATUS_INIT && CheckDepend(bedependItem.DependSet))
                    {
                        StartFlowItem(bedependItem);
                    }
                }
            }
        }

        /// <summary>
        /// switch后的处理函数
        /// </summary>
        /// <returns></returns>
        private void Switch(FlowItem flowItem)
        {
            if (flowItem.SwitchType > 0 && !flowItem.IsPass())
            {
                StopFlowTest();

                if (flowItem.SwitchType == FlowItem.SWITCH_TYPE_SWITCH)
                {
                    FlowControl.Instance.FlowCompleteReason = FlowControl.FLOW_COMPLETE_SWITCH;
                    FlowControl.Instance.FlowResult = FlowControl.FLOW_RESULT_FAIL;
                }
                else if (flowItem.SwitchType == FlowItem.SWITCH_TYPE_STOP)
                {
                    FlowControl.Instance.FlowCompleteReason = FlowControl.FLOW_COMPLETE_STOP;
                    FlowControl.Instance.FlowResult = FlowControl.FLOW_RESULT_EXCEPTION;
                }
            }
        }

        private bool FlowComplete()
        {
            foreach (FlowItem flowItem in FlowControl.Instance.FlowItemList)
            {
                if (!flowItem.Item.Property.Disable && flowItem.Status != FlowItem.STATUS_FINISH)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 订阅下位机主动上报事件
        /// </summary>
        private void SubscribeEquipmentEvent()
        {
            EquipmentCmd.Instance.ReportEvent += EquipmentEventHandler;
        }

        private void EquipmentEventHandler(CommonPortCmd.ActiveEnumData eventId)
        {
            Log.Debug("Equipment report event " + eventId);

            if (eventId == CommonPortCmd.ActiveEnumData.ProductInPlace_OK)
            {
                if (0 == NetUtil.GetStationIndex())
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new System.Windows.Forms.MethodInvoker(delegate()
                            {
                                StartFlowTest();
                            }));
                    }
                    else
                    {
                        StartFlowTest();
                    }                    
                    InplaceHandler(0);
                }
                else
                {
                    LiteDataClient.Instance.SendInplaceFlag(NetUtil.GetStationIndex());
                }
            }
            else if (eventId == CommonPortCmd.ActiveEnumData.ProductInPlace_NO)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                    {
                        ReloadFlowTest();
                    }));
            }
        }

        /// <summary>
        /// 订阅LiteDataServer事件
        /// </summary>
        private void SubscribeLiteDataServerEvent()
        {
            LiteDataServer.Instance.InplaceEvent += new LiteDataServer.InplaceEventHandler(InplaceHandler);
            LiteDataServer.Instance.CompleteEvent += new LiteDataServer.CompleteEventHandler(CompleteHandler);
            LiteDataServer.Instance.PickPlaceEvent += new LiteDataServer.PickPlaceEventHandler(PickPlaceHandler);
        }

        private void CompleteHandler(int index)
        {
            Log.Debug("CompleteHandler " + index);
            AppInfo.EquipmentInfo.GetStationInfo(index).Complete = true;
            AppInfo.TryPickPlace();
        }

        private void InplaceHandler(int index)
        {
            Log.Debug("InplaceHandler " + index);
            AppInfo.EquipmentInfo.GetStationInfo(index).Work = true;
        }

        private void PickPlaceHandler()
        {
            Log.Debug("PickPlaceHandler");

            if (this.InvokeRequired)
            {
                this.Invoke(new System.Windows.Forms.MethodInvoker(delegate()
                    {
                        ReloadFlowTest();
                        if (0 != NetUtil.GetStationIndex())
                        {
                            StartFlowTest();
                        }
                    }));
            }
            else 
            {
                ReloadFlowTest();
                if (0 != NetUtil.GetStationIndex())
                {
                    StartFlowTest();
                }
            }
        }

        /// <summary>
        /// 2~5站从第一站获取手机ip
        /// </summary>
        [Conditional("NDEBUG")]
        private void GetPhoneIp()
        {
            //获取手机IP地址
            if (NetUtil.GetStationIndex() > 0)
            {
                AppInfo.PhoneInfo.IP = LiteDataClient.Instance.GetPhoneIP(NetUtil.GetStationIndex());
                Log.Debug("phone ip = " + AppInfo.PhoneInfo.IP);
            }
        }

        /// <summary>
        /// 开启流程计时器
        /// </summary>
        private void StartFlowTimer()
        {
            totalTime_ = 0;
            timer.Start();
        }

        private void StopFlowTimer()
        {
            timer.Stop();
        }

        private void FlowTimerTickHandler(Object sender, EventArgs e)
        {
            totalTime_++;
            this.toolStripStatusLabelTime.Text = "测试时间 : " + totalTime_ + " s";
            SetStripProgressBarWidth();
        }

        private void InitStatusStrip()
        {
            SetStripProgressBarWidth();
            InitStatusLabelTime();
            InitStatusProgressBar();
        }

        private void InitStatusLabelTime()
        {
            this.toolStripStatusLabelTime.Text = "测试时间 : 0 s";
        }

        private void InitStatusProgressBar()
        {
            toolStripProgressBar.Minimum = 0;
            List<FlowItem> flowItemList = FlowControl.Instance.FlowItemList;
            toolStripProgressBar.Maximum = flowItemList.Count(i => !i.Item.Property.Disable);
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Step = 1;
        }

        private void SetStripProgressBarWidth()
        {
            this.toolStripProgressBar.Width = this.statusStrip.Width - this.toolStripStatusLabelTime.Width - this.toolStripStatusLabelStatistic.Width - 15;
        }

        private void UpdateStatisticInfo()
        {
            int total = Statistic.Instance.StatisticInfo.TotalNum;
            int fail = Statistic.Instance.StatisticInfo.FailNum;
            double rate = (total - fail) / (double)total;
            this.toolStripStatusLabelStatistic.Text = "总数 : " + total + "   不良数 : " + fail + "   良率 : " + rate.ToString("P") ;
            SetStripProgressBarWidth();
        }
    }
}
