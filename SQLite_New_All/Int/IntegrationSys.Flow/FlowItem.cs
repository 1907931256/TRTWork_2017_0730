using System;
using System.Collections.Generic;

namespace IntegrationSys.Flow
{
	internal class FlowItem
	{
		public const int STATUS_INIT = 0;

		public const int STATUS_RUNNING = 1;

		public const int STATUS_FINISH = 2;

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

		public long Duration
		{
			get;
			set;
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
	}
}
