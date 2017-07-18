using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace IntegrationSys.Flow
{
	internal sealed class FlowControl
	{
		public const int FLOW_STATUS_INIT = 0;

		public const int FLOW_STATUS_RUNNING = 1;

		public const int FLOW_STATUS_COMPLETE = 2;

		private static FlowControl instance_;

		private List<FlowItem> flowItemList_;

		public static FlowControl Instance
		{
			get
			{
				if (FlowControl.instance_ == null)
				{
					FlowControl.instance_ = new FlowControl("Flowtest.xml");
				}
				return FlowControl.instance_;
			}
		}

		public int FlowStatus
		{
			get;
			set;
		}

		public List<FlowItem> FlowItemList
		{
			get
			{
				return this.flowItemList_;
			}
		}

		public void Reload()
		{
			if (this.flowItemList_ != null)
			{
				this.flowItemList_.Clear();
			}
			this.LoadFlowFile("Flowtest.xml");
		}

		private FlowControl(string filename)
		{
			this.LoadFlowFile(filename);
		}

		public FlowItem GetFlowItem(int id)
		{
			int num = this.Id2Index(id);
			if (this.flowItemList_ == null || this.flowItemList_.Count <= num)
			{
				return null;
			}
			return this.flowItemList_[num];
		}

		public int Index2Id(int index)
		{
			return index + 1;
		}

		public int Id2Index(int id)
		{
			if (id > 0)
			{
				return id - 1;
			}
			return 0;
		}

		private void LoadFlowFile(string filename)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreProcessingInstructions = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			try
			{
				using (XmlReader xmlReader = XmlReader.Create(filename, xmlReaderSettings))
				{
					xmlReader.MoveToContent();
					while (xmlReader.Read())
					{
						if (xmlReader.IsStartElement("Item"))
						{
							FlowItem flowItem = this.ParseFlowItem(xmlReader);
							this.AddFlowItem(flowItem);
							this.UpdateDependAndBedepend(flowItem);
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
			}
		}

		private FlowItem ParseFlowItem(XmlReader reader)
		{
			if (!reader.IsStartElement("Item"))
			{
				return null;
			}
			FlowItem flowItem = new FlowItem();
			Item item = new Item();
			item.Id = Convert.ToInt32(reader.GetAttribute("id"));
			while (reader.Read() && reader.IsStartElement("Method"))
			{
				item.AddMethod(this.ParseMethod(reader));
			}
			item.Property = this.ParseProperty(reader);
			flowItem.Item = item;
			return flowItem;
		}

		private Method ParseMethod(XmlReader reader)
		{
			if (!reader.IsStartElement("Method"))
			{
				return null;
			}
			Method method = new Method();
			method.Name = reader.GetAttribute(0);
			method.Action = reader.GetAttribute(1);
			method.Param = reader.GetAttribute(2);
			method.Compare = reader.GetAttribute(3);
			method.Disable = reader.GetAttribute(4).Equals("1");
			method.Bedepend = reader.GetAttribute(5).Equals("1");
			method.Depend = reader.GetAttribute(6).Equals("1");
			reader.Read();
			return method;
		}

		private Property ParseProperty(XmlReader reader)
		{
			if (!reader.IsStartElement("Property"))
			{
				return null;
			}
			Property property = new Property();
			property.Name = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.Spec = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.SpecDescription = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.SpecKey = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.ErrorCode = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.SpecPrefix = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.SpecSuffix = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.Switch = reader.GetAttribute(0).Equals("1");
			reader.Read();
			reader.Read();
			property.Alarm = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.Disable = reader.GetAttribute(0).Equals("1");
			reader.Read();
			reader.Read();
			property.SpecEnable = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.Brother = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			try
			{
				property.Timeout = Convert.ToInt32(reader.GetAttribute(0));
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			reader.Read();
			reader.Read();
			property.Editable = reader.GetAttribute(0).Equals("1");
			reader.Read();
			reader.Read();
			try
			{
				property.Loop = Convert.ToInt32(reader.GetAttribute(0));
			}
			catch (FormatException)
			{
				property.Loop = 1;
			}
			catch (OverflowException)
			{
				property.Loop = 1;
			}
			reader.Read();
			reader.Read();
			property.Hide = reader.GetAttribute(0).Equals("1");
			reader.Read();
			reader.Read();
			property.Condition = reader.GetAttribute(0);
			reader.Read();
			reader.Read();
			property.Depend = reader.GetAttribute(0);
			reader.Read();
			return property;
		}

		private void AddFlowItem(FlowItem flowItem)
		{
			if (this.flowItemList_ == null)
			{
				this.flowItemList_ = new List<FlowItem>();
			}
			this.flowItemList_.Add(flowItem);
		}

		private void AddDependAndBedepend(FlowItem flowItem, int dependId)
		{
			FlowItem flowItem2 = this.GetFlowItem(dependId);
			if (flowItem2 != null)
			{
				if (!flowItem2.Item.Property.Disable)
				{
					flowItem.AddDepend(dependId);
					flowItem2.AddBedepend(flowItem.Id);
					return;
				}
				if (!string.IsNullOrEmpty(flowItem2.Item.Property.Depend))
				{
					string[] array = flowItem2.Item.Property.Depend.Split(new char[]
					{
						' '
					});
					string[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						string value = array2[i];
						int dependId2 = Convert.ToInt32(value);
						this.AddDependAndBedepend(flowItem, dependId2);
					}
					return;
				}
			}
			else
			{
				flowItem.AddDepend(dependId);
			}
		}

		private void UpdateDependAndBedepend(FlowItem flowItem)
		{
			if (!string.IsNullOrEmpty(flowItem.Item.Property.Depend))
			{
				string[] array = flowItem.Item.Property.Depend.Split(new char[]
				{
					' '
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string value = array2[i];
					int dependId = Convert.ToInt32(value);
					this.AddDependAndBedepend(flowItem, dependId);
				}
			}
		}
	}
}
