using CommonPortCmd;
using IntegrationSys.Equipment;
using IntegrationSys.Flow;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace IntegrationSys
{
	public class Form1 : Form
	{
		private delegate void ItemExecuteComplete(FlowItem flowItem);

		private const int COLUMN_ITEM_COUNT = 0;

		private const int COLUMN_ITEM_NAME = 1;

		private const int COLUMN_ITEM_CONTENT = 2;

		private const int COLUMN_ITEM_SPEC = 3;

		private const int COLUMN_ITEM_VALUE = 4;

		private const int COLUMN_ITEM_RESULT = 5;

		private const int COLUMN_ITEM_STATUS = 6;

		private const int COLUMN_ITEM_DURATION = 7;

		private const int COLUMN_ITEM_MAX = 8;

		private IContainer components;

		private DataGridView flowGridView;

		private MenuStrip menuStrip1;

		private ToolStripMenuItem 文件FToolStripMenuItem;

		private ToolStripMenuItem 新建NToolStripMenuItem;

		private ToolStripMenuItem 打开OToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator;

		private ToolStripMenuItem 保存SToolStripMenuItem;

		private ToolStripMenuItem 另存为AToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripMenuItem 打印PToolStripMenuItem;

		private ToolStripMenuItem 打印预览VToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem 退出XToolStripMenuItem;

		private ToolStripMenuItem 编辑EToolStripMenuItem;

		private ToolStripMenuItem 撤消UToolStripMenuItem;

		private ToolStripMenuItem 重复RToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripMenuItem 剪切TToolStripMenuItem;

		private ToolStripMenuItem 复制CToolStripMenuItem;

		private ToolStripMenuItem 粘贴PToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripMenuItem 全选AToolStripMenuItem;

		private ToolStripMenuItem 工具TToolStripMenuItem;

		private ToolStripMenuItem 自定义CToolStripMenuItem;

		private ToolStripMenuItem 选项OToolStripMenuItem;

		private ToolStripMenuItem 帮助HToolStripMenuItem;

		private ToolStripMenuItem 内容CToolStripMenuItem;

		private ToolStripMenuItem 索引IToolStripMenuItem;

		private ToolStripMenuItem 搜索SToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator5;

		private ToolStripMenuItem 关于AToolStripMenuItem;

		public Form1()
		{
			this.InitializeComponent();
		}

		private void MainFormLoad(object sender, EventArgs e)
		{
			this.LoadFlowItemList();
			this.SubscribeEquipmentEvent();
		}

		private void LoadFlowItemList()
		{
			FlowControl instance = FlowControl.Instance;
			this.flowGridView.ColumnCount = 8;
			this.flowGridView.Columns[0].Name = "测试项";
			this.flowGridView.Columns[1].Name = "测试项名称";
			this.flowGridView.Columns[2].Name = "测试内容";
			this.flowGridView.Columns[3].Name = "规格值";
			this.flowGridView.Columns[4].Name = "实测值";
			this.flowGridView.Columns[5].Name = "结果";
			this.flowGridView.Columns[6].Name = "状态";
			this.flowGridView.Columns[7].Name = "测试时间";
			for (int i = 0; i < 8; i++)
			{
				this.flowGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
			}
			this.flowGridView.Columns[0].DefaultCellStyle = this.flowGridView.ColumnHeadersDefaultCellStyle;
			this.flowGridView.Columns[1].DefaultCellStyle = this.flowGridView.ColumnHeadersDefaultCellStyle;
			this.flowGridView.Columns[2].DefaultCellStyle = this.flowGridView.ColumnHeadersDefaultCellStyle;
			this.flowGridView.Columns[3].DefaultCellStyle = this.flowGridView.ColumnHeadersDefaultCellStyle;
			if (instance.FlowItemList != null)
			{
				int num = 1;
				for (int j = 0; j < instance.FlowItemList.Count; j++)
				{
					FlowItem flowItem = instance.FlowItemList[j];
					flowItem.RowIndex = this.flowGridView.RowCount;
					if (!flowItem.Item.Property.Hide && !flowItem.Item.Property.Disable)
					{
						if (flowItem.SpecValueList != null)
						{
							using (List<SpecValue>.Enumerator enumerator = flowItem.SpecValueList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									SpecValue current = enumerator.Current;
									if (!current.Disable)
									{
										List<string> list = new List<string>();
										list.Add(Convert.ToString(num));
										list.Add(flowItem.Name);
										list.Add(current.SpecDescription);
										list.Add(current.Spec);
										list.Add(string.IsNullOrEmpty(current.MeasuredValue) ? "" : current.MeasuredValue);
										list.Add(string.IsNullOrEmpty(current.JudgmentResult) ? "" : current.JudgmentResult);
										list.Add("");
										list.Add("");
										this.flowGridView.Rows.Add(list.ToArray());
									}
								}
								goto IL_37A;
							}
							goto IL_2F8;
						}
						goto IL_2F8;
						IL_37A:
						num++;
						goto IL_37E;
						IL_2F8:
						List<string> list2 = new List<string>();
						list2.Add(Convert.ToString(num));
						list2.Add(flowItem.Name);
						list2.Add("");
						list2.Add("");
						list2.Add("");
						list2.Add("");
						list2.Add("");
						list2.Add("");
						this.flowGridView.Rows.Add(list2.ToArray());
						goto IL_37A;
					}
					IL_37E:;
				}
			}
		}

		private void ReloadFlowTest()
		{
			if (FlowControl.Instance.FlowStatus == 2)
			{
				FlowControl.Instance.Reload();
				this.flowGridView.Rows.Clear();
				this.LoadFlowItemList();
				FlowControl.Instance.FlowStatus = 0;
			}
		}

		private void StartFlowTest()
		{
			FlowControl instance = FlowControl.Instance;
			if (instance.FlowStatus == 0)
			{
				instance.FlowStatus = 1;
				foreach (FlowItem current in instance.FlowItemList)
				{
					if (current.Status == 0 && !current.Item.Property.Disable && this.CheckDepend(current.DependSet))
					{
						this.StartFlowItem(current);
					}
				}
			}
		}

		private void StartFlowItem(FlowItem flowItem)
		{
			if (flowItem != null && !flowItem.Item.Property.Disable)
			{
				FlowItemExecutor flowItemExecutor = new FlowItemExecutor(flowItem);
				flowItemExecutor.ExecuteFinish += new ExecuteFinishEventHandler(this.ItemExecuteFinish);
				ThreadPool.QueueUserWorkItem(new WaitCallback(flowItemExecutor.ThreadProc));
				flowItem.Status = 1;
				this.UpdateData(flowItem);
			}
		}

		private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.StartFlowTest();
		}

		private void 搜索SToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.ReloadFlowTest();
		}

		private void ItemExecuteFinish(object sender, ExecuteFinishEventArgs e)
		{
			e.FlowItem.Status = 2;
			base.BeginInvoke(new Form1.ItemExecuteComplete(this.ItemExecuteCompleteHandler), new object[]
			{
				e.FlowItem
			});
		}

		private void ItemExecuteCompleteHandler(FlowItem flowItem)
		{
			this.UpdateData(flowItem);
			this.UpdatePhoneInfo(flowItem);
			if (this.FlowComplete())
			{
				FlowControl.Instance.FlowStatus = 2;
			}
		}

		private void UpdatePhoneInfo(FlowItem flowItem)
		{
			if (flowItem.Name == "SN")
			{
				if (flowItem.SpecValueList != null && flowItem.SpecValueList.Count == 1)
				{
					AppInfo.PhoneInfo.SN = flowItem.SpecValueList[0].MeasuredValue;
					return;
				}
			}
			else if (flowItem.Name == "IP" && flowItem.SpecValueList != null && flowItem.SpecValueList.Count == 1)
			{
				AppInfo.PhoneInfo.IP = flowItem.SpecValueList[0].MeasuredValue;
			}
		}

		private void UpdateData(FlowItem flowItem)
		{
			switch (flowItem.Status)
			{
			case 1:
				this.UpdateRunning(flowItem);
				return;
			case 2:
				this.UpdateFinish(flowItem);
				return;
			default:
				return;
			}
		}

		private void UpdateRunning(FlowItem flowItem)
		{
			if (flowItem.Status == 1)
			{
				DataGridViewCell dataGridViewCell = this.flowGridView.Rows[flowItem.RowIndex].Cells[6];
				if (dataGridViewCell != null)
				{
					dataGridViewCell.Value = "测试中";
					dataGridViewCell.Style.BackColor = Color.Lime;
				}
			}
		}

		private void UpdateFinish(FlowItem flowItem)
		{
			if (flowItem.Status == 2)
			{
				if (flowItem.SpecValueList != null)
				{
					int num = 0;
					for (int i = 0; i < flowItem.SpecValueList.Count; i++)
					{
						if (!flowItem.SpecValueList[i].Disable)
						{
							DataGridViewCell dataGridViewCell = this.flowGridView.Rows[flowItem.RowIndex + num].Cells[4];
							if (dataGridViewCell != null)
							{
								dataGridViewCell.Value = flowItem.SpecValueList[i].MeasuredValue;
							}
							DataGridViewCell dataGridViewCell2 = this.flowGridView.Rows[flowItem.RowIndex + num].Cells[5];
							if (dataGridViewCell2 != null)
							{
								dataGridViewCell2.Value = flowItem.SpecValueList[i].JudgmentResult;
								if (!"成功".Equals(flowItem.SpecValueList[i].JudgmentResult))
								{
									dataGridViewCell2.Style.BackColor = Color.Red;
								}
							}
							num++;
						}
					}
				}
				DataGridViewCell dataGridViewCell3 = this.flowGridView.Rows[flowItem.RowIndex].Cells[6];
				if (dataGridViewCell3 != null)
				{
					dataGridViewCell3.Value = "完成";
					dataGridViewCell3.Style.BackColor = this.flowGridView.DefaultCellStyle.BackColor;
				}
				DataGridViewCell dataGridViewCell4 = this.flowGridView.Rows[flowItem.RowIndex].Cells[7];
				if (dataGridViewCell4 != null)
				{
					dataGridViewCell4.Value = Convert.ToString(flowItem.Duration) + "ms";
				}
				if (flowItem.BedependSet != null)
				{
					FlowControl instance = FlowControl.Instance;
					foreach (int current in flowItem.BedependSet)
					{
						FlowItem flowItem2 = instance.GetFlowItem(current);
						if (flowItem2 != null && flowItem2.Status == 0 && this.CheckDepend(flowItem2.DependSet))
						{
							this.StartFlowItem(flowItem2);
						}
					}
				}
			}
		}

		private bool CheckDepend(HashSet<int> dependSet)
		{
			if (dependSet == null)
			{
				return true;
			}
			FlowControl instance = FlowControl.Instance;
			foreach (int current in dependSet)
			{
				FlowItem flowItem = instance.GetFlowItem(current);
				if (flowItem.Item.Property.Disable)
				{
					if (!this.CheckDepend(flowItem.DependSet))
					{
						bool result = false;
						return result;
					}
				}
				else if (flowItem.Status != 2)
				{
					bool result = false;
					return result;
				}
			}
			return true;
		}

		private bool FlowComplete()
		{
			foreach (FlowItem current in FlowControl.Instance.FlowItemList)
			{
				if (!current.Item.Property.Disable && current.Status != 2)
				{
					return false;
				}
			}
			return true;
		}

		private void SubscribeEquipmentEvent()
		{
			EquipmentCmd.Instance.ReportEvent += new EquipmentCmd.ReportEventHandler(this.EquipmentEventHandler);
		}

		private void EquipmentEventHandler(ActiveEnumData eventId)
		{
			IntegrationSys.LogUtil.Log.Debug("Equipment report event " + eventId);
			if (eventId == ActiveEnumData.ProductInPlace_OK && NetUtil.GetStationIndex() == 0)
			{
				this.ReloadFlowTest();
				this.StartFlowTest();
				this.InplaceHandler(0);
			}
		}

		private void SubscribeLiteDataServerEvent()
		{
			LiteDataServer.Instance.InplaceEvent += new LiteDataServer.InplaceEventHandler(this.InplaceHandler);
			LiteDataServer.Instance.CompleteEvent += new LiteDataServer.CompleteEventHandler(this.CompleteHandler);
			LiteDataServer.Instance.PickPlaceEvent += new LiteDataServer.PickPlaceEventHandler(this.PickPlaceHandler);
		}

		private void CompleteHandler(int index)
		{
			IntegrationSys.LogUtil.Log.Debug("CompleteHandler " + index);
			AppInfo.EquipmentInfo.GetStationInfo(index).Complete = true;
			AppInfo.TryPickPlace();
		}

		private void InplaceHandler(int index)
		{
			IntegrationSys.LogUtil.Log.Debug("InplaceHandler " + index);
			AppInfo.EquipmentInfo.GetStationInfo(index).Work = true;
		}

		private void PickPlaceHandler()
		{
			IntegrationSys.LogUtil.Log.Debug("PickPlaceHandler");
			if (NetUtil.GetStationIndex() != 0)
			{
				this.ReloadFlowTest();
				this.StartFlowTest();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Form1));
			this.flowGridView = new DataGridView();
			this.menuStrip1 = new MenuStrip();
			this.文件FToolStripMenuItem = new ToolStripMenuItem();
			this.新建NToolStripMenuItem = new ToolStripMenuItem();
			this.打开OToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator = new ToolStripSeparator();
			this.保存SToolStripMenuItem = new ToolStripMenuItem();
			this.另存为AToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.打印PToolStripMenuItem = new ToolStripMenuItem();
			this.打印预览VToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.退出XToolStripMenuItem = new ToolStripMenuItem();
			this.编辑EToolStripMenuItem = new ToolStripMenuItem();
			this.撤消UToolStripMenuItem = new ToolStripMenuItem();
			this.重复RToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator3 = new ToolStripSeparator();
			this.剪切TToolStripMenuItem = new ToolStripMenuItem();
			this.复制CToolStripMenuItem = new ToolStripMenuItem();
			this.粘贴PToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator4 = new ToolStripSeparator();
			this.全选AToolStripMenuItem = new ToolStripMenuItem();
			this.工具TToolStripMenuItem = new ToolStripMenuItem();
			this.自定义CToolStripMenuItem = new ToolStripMenuItem();
			this.选项OToolStripMenuItem = new ToolStripMenuItem();
			this.帮助HToolStripMenuItem = new ToolStripMenuItem();
			this.内容CToolStripMenuItem = new ToolStripMenuItem();
			this.索引IToolStripMenuItem = new ToolStripMenuItem();
			this.搜索SToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator5 = new ToolStripSeparator();
			this.关于AToolStripMenuItem = new ToolStripMenuItem();
			((ISupportInitialize)this.flowGridView).BeginInit();
			this.menuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.flowGridView.AllowUserToAddRows = false;
			this.flowGridView.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.flowGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle.BackColor = SystemColors.Control;
			dataGridViewCellStyle.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.True;
			this.flowGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
			this.flowGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.flowGridView.Location = new Point(0, 28);
			this.flowGridView.Name = "flowGridView";
			this.flowGridView.ReadOnly = true;
			dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.BackColor = SystemColors.Control;
			dataGridViewCellStyle2.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
			this.flowGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.flowGridView.RowHeadersVisible = false;
			dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
			this.flowGridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
			this.flowGridView.RowTemplate.Height = 23;
			this.flowGridView.Size = new Size(829, 344);
			this.flowGridView.TabIndex = 0;
			this.menuStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.文件FToolStripMenuItem,
				this.编辑EToolStripMenuItem,
				this.工具TToolStripMenuItem,
				this.帮助HToolStripMenuItem
			});
			this.menuStrip1.Location = new Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new Size(829, 25);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			this.文件FToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.新建NToolStripMenuItem,
				this.打开OToolStripMenuItem,
				this.toolStripSeparator,
				this.保存SToolStripMenuItem,
				this.另存为AToolStripMenuItem,
				this.toolStripSeparator1,
				this.打印PToolStripMenuItem,
				this.打印预览VToolStripMenuItem,
				this.toolStripSeparator2,
				this.退出XToolStripMenuItem
			});
			this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
			this.文件FToolStripMenuItem.Size = new Size(58, 21);
			this.文件FToolStripMenuItem.Text = "文件(&F)";
			this.新建NToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("新建NToolStripMenuItem.Image");
			this.新建NToolStripMenuItem.ImageTransparentColor = Color.Magenta;
			this.新建NToolStripMenuItem.Name = "新建NToolStripMenuItem";
			this.新建NToolStripMenuItem.ShortcutKeys = (Keys)131150;
			this.新建NToolStripMenuItem.Size = new Size(165, 22);
			this.新建NToolStripMenuItem.Text = "新建(&N)";
			this.打开OToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("打开OToolStripMenuItem.Image");
			this.打开OToolStripMenuItem.ImageTransparentColor = Color.Magenta;
			this.打开OToolStripMenuItem.Name = "打开OToolStripMenuItem";
			this.打开OToolStripMenuItem.ShortcutKeys = (Keys)131151;
			this.打开OToolStripMenuItem.Size = new Size(165, 22);
			this.打开OToolStripMenuItem.Text = "打开(&O)";
			this.toolStripSeparator.Name = "toolStripSeparator";
			this.toolStripSeparator.Size = new Size(162, 6);
			this.保存SToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("保存SToolStripMenuItem.Image");
			this.保存SToolStripMenuItem.ImageTransparentColor = Color.Magenta;
			this.保存SToolStripMenuItem.Name = "保存SToolStripMenuItem";
			this.保存SToolStripMenuItem.ShortcutKeys = (Keys)131155;
			this.保存SToolStripMenuItem.Size = new Size(165, 22);
			this.保存SToolStripMenuItem.Text = "保存(&S)";
			this.另存为AToolStripMenuItem.Name = "另存为AToolStripMenuItem";
			this.另存为AToolStripMenuItem.Size = new Size(165, 22);
			this.另存为AToolStripMenuItem.Text = "另存为(&A)";
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(162, 6);
			this.打印PToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("打印PToolStripMenuItem.Image");
			this.打印PToolStripMenuItem.ImageTransparentColor = Color.Magenta;
			this.打印PToolStripMenuItem.Name = "打印PToolStripMenuItem";
			this.打印PToolStripMenuItem.ShortcutKeys = (Keys)131152;
			this.打印PToolStripMenuItem.Size = new Size(165, 22);
			this.打印PToolStripMenuItem.Text = "打印(&P)";
			this.打印预览VToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("打印预览VToolStripMenuItem.Image");
			this.打印预览VToolStripMenuItem.ImageTransparentColor = Color.Magenta;
			this.打印预览VToolStripMenuItem.Name = "打印预览VToolStripMenuItem";
			this.打印预览VToolStripMenuItem.Size = new Size(165, 22);
			this.打印预览VToolStripMenuItem.Text = "打印预览(&V)";
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(162, 6);
			this.退出XToolStripMenuItem.Name = "退出XToolStripMenuItem";
			this.退出XToolStripMenuItem.Size = new Size(165, 22);
			this.退出XToolStripMenuItem.Text = "退出(&X)";
			this.编辑EToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.撤消UToolStripMenuItem,
				this.重复RToolStripMenuItem,
				this.toolStripSeparator3,
				this.剪切TToolStripMenuItem,
				this.复制CToolStripMenuItem,
				this.粘贴PToolStripMenuItem,
				this.toolStripSeparator4,
				this.全选AToolStripMenuItem
			});
			this.编辑EToolStripMenuItem.Name = "编辑EToolStripMenuItem";
			this.编辑EToolStripMenuItem.Size = new Size(59, 21);
			this.编辑EToolStripMenuItem.Text = "编辑(&E)";
			this.撤消UToolStripMenuItem.Name = "撤消UToolStripMenuItem";
			this.撤消UToolStripMenuItem.ShortcutKeys = (Keys)131162;
			this.撤消UToolStripMenuItem.Size = new Size(161, 22);
			this.撤消UToolStripMenuItem.Text = "撤消(&U)";
			this.重复RToolStripMenuItem.Name = "重复RToolStripMenuItem";
			this.重复RToolStripMenuItem.ShortcutKeys = (Keys)131161;
			this.重复RToolStripMenuItem.Size = new Size(161, 22);
			this.重复RToolStripMenuItem.Text = "重复(&R)";
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new Size(158, 6);
			this.剪切TToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("剪切TToolStripMenuItem.Image");
			this.剪切TToolStripMenuItem.ImageTransparentColor = Color.Magenta;
			this.剪切TToolStripMenuItem.Name = "剪切TToolStripMenuItem";
			this.剪切TToolStripMenuItem.ShortcutKeys = (Keys)131160;
			this.剪切TToolStripMenuItem.Size = new Size(161, 22);
			this.剪切TToolStripMenuItem.Text = "剪切(&T)";
			this.复制CToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("复制CToolStripMenuItem.Image");
			this.复制CToolStripMenuItem.ImageTransparentColor = Color.Magenta;
			this.复制CToolStripMenuItem.Name = "复制CToolStripMenuItem";
			this.复制CToolStripMenuItem.ShortcutKeys = (Keys)131139;
			this.复制CToolStripMenuItem.Size = new Size(161, 22);
			this.复制CToolStripMenuItem.Text = "复制(&C)";
			this.粘贴PToolStripMenuItem.Image = (Image)componentResourceManager.GetObject("粘贴PToolStripMenuItem.Image");
			this.粘贴PToolStripMenuItem.ImageTransparentColor = Color.Magenta;
			this.粘贴PToolStripMenuItem.Name = "粘贴PToolStripMenuItem";
			this.粘贴PToolStripMenuItem.ShortcutKeys = (Keys)131158;
			this.粘贴PToolStripMenuItem.Size = new Size(161, 22);
			this.粘贴PToolStripMenuItem.Text = "粘贴(&P)";
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new Size(158, 6);
			this.全选AToolStripMenuItem.Name = "全选AToolStripMenuItem";
			this.全选AToolStripMenuItem.Size = new Size(161, 22);
			this.全选AToolStripMenuItem.Text = "全选(&A)";
			this.工具TToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.自定义CToolStripMenuItem,
				this.选项OToolStripMenuItem
			});
			this.工具TToolStripMenuItem.Name = "工具TToolStripMenuItem";
			this.工具TToolStripMenuItem.Size = new Size(59, 21);
			this.工具TToolStripMenuItem.Text = "工具(&T)";
			this.自定义CToolStripMenuItem.Name = "自定义CToolStripMenuItem";
			this.自定义CToolStripMenuItem.Size = new Size(128, 22);
			this.自定义CToolStripMenuItem.Text = "自定义(&C)";
			this.选项OToolStripMenuItem.Name = "选项OToolStripMenuItem";
			this.选项OToolStripMenuItem.Size = new Size(128, 22);
			this.选项OToolStripMenuItem.Text = "选项(&O)";
			this.帮助HToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.内容CToolStripMenuItem,
				this.索引IToolStripMenuItem,
				this.搜索SToolStripMenuItem,
				this.toolStripSeparator5,
				this.关于AToolStripMenuItem
			});
			this.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem";
			this.帮助HToolStripMenuItem.Size = new Size(61, 21);
			this.帮助HToolStripMenuItem.Text = "帮助(&H)";
			this.内容CToolStripMenuItem.Name = "内容CToolStripMenuItem";
			this.内容CToolStripMenuItem.Size = new Size(152, 22);
			this.内容CToolStripMenuItem.Text = "内容(&C)";
			this.索引IToolStripMenuItem.Name = "索引IToolStripMenuItem";
			this.索引IToolStripMenuItem.Size = new Size(152, 22);
			this.索引IToolStripMenuItem.Text = "索引(&I)";
			this.搜索SToolStripMenuItem.Name = "搜索SToolStripMenuItem";
			this.搜索SToolStripMenuItem.Size = new Size(152, 22);
			this.搜索SToolStripMenuItem.Text = "搜索(&S)";
			this.搜索SToolStripMenuItem.Click += new EventHandler(this.搜索SToolStripMenuItem_Click);
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new Size(149, 6);
			this.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem";
			this.关于AToolStripMenuItem.Size = new Size(152, 22);
			this.关于AToolStripMenuItem.Text = "关于(&A)...";
			this.关于AToolStripMenuItem.Click += new EventHandler(this.关于AToolStripMenuItem_Click);
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(829, 372);
			base.Controls.Add(this.flowGridView);
			base.Controls.Add(this.menuStrip1);
			base.MainMenuStrip = this.menuStrip1;
			base.Name = "Form1";
			this.Text = "TRT";
			base.Load += new EventHandler(this.MainFormLoad);
			((ISupportInitialize)this.flowGridView).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
