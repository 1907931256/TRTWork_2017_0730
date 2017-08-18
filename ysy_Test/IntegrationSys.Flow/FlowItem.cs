using IntegrationSys.Net;
using IntegrationSys.Utility;
using System;
using System.Collections.Generic;

namespace IntegrationSys.Flow
{
	internal class FlowItem
	{
		public const int STATUS_INIT = 0;

		public const int STATUS_RUNNING = 1;

		public const int STATUS_FINISH = 2;

		public const int ALARM_TYPE_NONE = 0;

		public const int ALARM_TYPE_WARN = 1;

		public const int ALARM_TYPE_ERROR = 2;

		public const int SWITCH_TYPE_NONE = 0;

		public const int SWITCH_TYPE_SWITCH = 1;

		public const int SWITCH_TYPE_STOP = 2;

		private Item item_;

		private HashSet<int> dependSet_;

		private HashSet<int> bedependSet_;

		private List<SpecValue> specValueList_;

		public Item Item
		{
			get
			{
				return this.item_;
			}
			set
			{
				this.item_ = value;
				if (this.item_.Property != null && !string.IsNullOrEmpty(this.item_.Property.Spec))
				{
					string[] array = this.item_.Property.SpecDescription.Split(new char[]
					{
						' '
					});
					string[] array2 = this.item_.Property.Spec.Split(new char[]
					{
						' '
					});
					string[] array3 = this.item_.Property.SpecKey.Split(new char[]
					{
						' '
					});
					string[] array4 = null;
					if (!string.IsNullOrEmpty(this.item_.Property.SpecEnable))
					{
						array4 = this.item_.Property.SpecEnable.Split(new char[]
						{
							' '
						});
					}
					for (int i = 0; i < array2.Length; i++)
					{
						SpecValue specValue = new SpecValue();
						specValue.SpecDescription = array[i];
						specValue.Spec = array2[i];
						specValue.SpecKey = array3[i];
						if (array4 != null && array4.Length > i && array4[i] == "0")
						{
							specValue.Disable = true;
						}
						this.AddSpecValue(specValue);
					}
				}
			}
		}

		public int Id
		{
			get
			{
				return this.Item.Id;
			}
		}

		public string Name
		{
			get
			{
				return this.Item.Property.Name;
			}
		}

		public int Status
		{
			get;
			set;
		}

		public DateTime BeginTime
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public long Duration
		{
			get;
			set;
		}

		public int AlarmType
		{
			get
			{
				int result = 0;
				try
				{
					result = int.Parse(this.Item.Property.Alarm);
				}
				catch (Exception)
				{
					result = 0;
				}
				return result;
			}
		}

		public int SwitchType
		{
			get
			{
				int num = 0;
				try
				{
					num = int.Parse(this.Item.Property.Switch);
					if (num == 2 && NetUtil.GetStationIndex() != 0)
					{
						num = 1;
					}
				}
				catch (Exception)
				{
					num = 0;
				}
				return num;
			}
		}

		public int RowIndex
		{
			get;
			set;
		}

		public HashSet<int> DependSet
		{
			get
			{
				return this.dependSet_;
			}
		}

		public HashSet<int> BedependSet
		{
			get
			{
				return this.bedependSet_;
			}
		}

		public List<SpecValue> SpecValueList
		{
			get
			{
				return this.specValueList_;
			}
		}

		public void AddDepend(int depend)
		{
			if (this.dependSet_ == null)
			{
				this.dependSet_ = new HashSet<int>();
			}
			this.dependSet_.Add(depend);
		}

		public void AddBedepend(int bedepend)
		{
			if (this.bedependSet_ == null)
			{
				this.bedependSet_ = new HashSet<int>();
			}
			this.bedependSet_.Add(bedepend);
		}

		public void AddSpecValue(SpecValue specValue)
		{
			if (this.specValueList_ == null)
			{
				this.specValueList_ = new List<SpecValue>();
			}
			this.specValueList_.Add(specValue);
		}

		public bool IsPass()
		{
			if (this.specValueList_ != null)
			{
				foreach (SpecValue current in this.specValueList_)
				{
					if (!current.Disable && (string.IsNullOrEmpty(current.JudgmentResult) || !current.JudgmentResult.Equals("成功")))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public bool IsFinished()
		{
			return this.Status == 2;
		}

		public bool IsSkip()
		{
			if (!string.IsNullOrEmpty(this.Item.Property.Condition))
			{
				string text = this.Item.Property.Condition.Replace(" ", string.Empty);
				string text2 = string.Empty;
				string text3 = string.Empty;
				bool flag = false;
				string text4 = text;
				for (int i = 0; i < text4.Length; i++)
				{
					char c = text4[i];
					if (c == '+' || c == '-' || c == '(' || c == ')')
					{
						if (!string.IsNullOrEmpty(text3))
						{
							int id = int.Parse(text3);
							FlowItem flowItem = FlowControl.Instance.GetFlowItem(id);
							bool flag2 = false;
							if (flowItem != null && flowItem.IsPass())
							{
								flag2 = true;
							}
							if (flag)
							{
								flag2 = !flag2;
							}
							text2 += (flag2 ? bool.TrueString : bool.FalseString);
							text3 = string.Empty;
							flag = false;
						}
						text2 += c;
					}
					else if (c == '!')
					{
						flag = true;
					}
					else
					{
						text3 += c;
					}
				}
				if (!string.IsNullOrEmpty(text3))
				{
					int id2 = int.Parse(text3);
					FlowItem flowItem2 = FlowControl.Instance.GetFlowItem(id2);
					bool flag3 = false;
					if (flowItem2 != null && flowItem2.IsPass())
					{
						flag3 = true;
					}
					if (flag)
					{
						flag3 = !flag3;
					}
					text2 += (flag3 ? bool.TrueString : bool.FalseString);
					text3 = string.Empty;
				}
				return !Utils.IsTrue(text2);
			}
			return false;
		}

		public bool IsAuxiliaryItem()
		{
			return this.Item.Property.Hide;
		}
	}
}
