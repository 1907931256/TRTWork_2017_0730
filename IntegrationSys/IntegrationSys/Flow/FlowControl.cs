using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace IntegrationSys.Flow
{
    /// <summary>
    /// 记录测试流程的数据并用于显示在DataGridView中
    /// 对该对象的所有修改只能在UI线程中进行
    /// </summary>
    sealed class FlowControl
    {
        /// <summary>
        /// 测试初始化
        /// </summary>
        public const int FLOW_STATUS_INIT = 0;
        /// <summary>
        /// 测试中
        /// </summary>
        public const int FLOW_STATUS_RUNNING = 1;
        /// <summary>
        /// 暂停
        /// </summary>
        public const int FLOW_STATUS_PAUSE = 2;
        /// <summary>
        /// 测试完成
        /// </summary>
        public const int FLOW_STATUS_COMPLETE = 3;

        public const int FLOW_COMPLETE_NORMAL = 0;   //正常测试完成
        public const int FLOW_COMPLETE_SWITCH = 1;   //switch引起的测试完成
        public const int FLOW_COMPLETE_STOP = 2;     //stop引起的测试完成

        /// <summary>
        /// 测试结果初始化
        /// </summary>
        public const int FLOW_RESULT_INIT = 0;
        /// <summary>
        /// 测试结果PASS
        /// </summary>
        public const int FLOW_RESULT_PASS = 1;
        /// <summary>
        /// 测试结果Fall
        /// </summary>
        public const int FLOW_RESULT_FAIL = 2;
        /// <summary>
        /// 测试结果异常
        /// </summary>
        public const int FLOW_RESULT_EXCEPTION = 3;   //测试中异常stop，则直接输出exception结果

        private static FlowControl instance_ = null;

        /// <summary>
        /// Item集合
        /// </summary>
        private List<FlowItem> flowItemList_;

        public static FlowControl Instance
        {
            get 
            {
                if (instance_ == null)
                {
                    instance_ = new FlowControl("Flowtest.xml");
                }
                
                return instance_;
            }
        }

        public int FlowStatus
        {
            get;
            set;
        }

        public int FlowCompleteReason
        {
            get;
            set;
        }

        public int FlowResult
        {
            get;
            set;
        }

        /// <summary>
        /// 重新加载flowItemList_
        /// </summary>
        public void Reload()
        {
            if (flowItemList_ != null)
            {
                flowItemList_.Clear();
            }

            LoadFlowFile("Flowtest.xml");
        }

        private FlowControl(string filename)
        {
            LoadFlowFile(filename);
        }

        public List<FlowItem> FlowItemList
        {
            get 
            {
                return flowItemList_;
            }
        }

        /// <summary>
        /// 根据id来获取 Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FlowItem GetFlowItem(int id)
        {
            int index = Id2Index(id);
            if (flowItemList_ == null || flowItemList_.Count <= index) return null;

            return flowItemList_[index];
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

        /// <summary>
        /// 测试脚本加载
        /// </summary>
        /// <param name="filename"></param>
        private void LoadFlowFile(string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings(); //测试脚本初始化的一些设定
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            try
            {
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    reader.MoveToContent();//移动到文本正确内容
                    while (reader.Read())
                    {
                        if (reader.IsStartElement("Item"))
                        {
                            FlowItem flowItem = ParseFlowItem(reader);
                            AddFlowItem(flowItem);
                            UpdateDependAndBedepend(flowItem);
                        }
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                
            }

        }

        /// <summary>
        /// 解析Item
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private FlowItem ParseFlowItem(XmlReader reader)
        {
            if (!reader.IsStartElement("Item")) return null;

            FlowItem flowItem = new FlowItem();
            Item item = new Item();
            item.Id = Convert.ToInt32(reader.GetAttribute("id"));
            while (reader.Read() && reader.IsStartElement("Method"))
            {
                item.AddMethod(ParseMethod(reader));
            }

            item.Property = ParseProperty(reader);

            flowItem.Item = item;

            return flowItem;
        }

        /// <summary>
        /// 解析Method节
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Method ParseMethod(XmlReader reader)
        {
            if (!reader.IsStartElement("Method")) return null;

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


        /// <summary>
        /// 解析Property节
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Property ParseProperty(XmlReader reader)
        {
            if (!reader.IsStartElement("Property")) return null;

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
            property.Switch = reader.GetAttribute(0);
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
            catch (System.FormatException)
            {

            }
            catch (System.OverflowException)
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
            catch (System.FormatException)
            {
                property.Loop = 1;
            }
            catch (System.OverflowException)
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


        /// <summary>
        /// 添加测试脚本
        /// </summary>
        /// <param name="flowItem"></param>
        private void AddFlowItem(FlowItem flowItem)
        {
            if (flowItemList_ == null)
            {
                flowItemList_ = new List<FlowItem>();
            }

            flowItemList_.Add(flowItem);
        }


        private void AddDependAndBedepend(FlowItem flowItem, int dependId)
        {
            FlowItem dependItem = GetFlowItem(dependId);

            if (dependItem != null)
            {
                if (dependItem.Item.Property.Disable)
                {
                    if (!string.IsNullOrEmpty(dependItem.Item.Property.Depend))
                    {
                        string[] dependArray = dependItem.Item.Property.Depend.Split(' ');
                        foreach (string depend in dependArray)
                        {
                            int id = Convert.ToInt32(depend);
                            AddDependAndBedepend(flowItem, id);
                        }
                    }
                }
                else
                {
                    flowItem.AddDepend(dependId);
                    dependItem.AddBedepend(flowItem.Id);
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
                string[] dependArray = flowItem.Item.Property.Depend.Split(' ');
                foreach (string depend in dependArray)
                {
                    int id = Convert.ToInt32(depend);
                    AddDependAndBedepend(flowItem, id);
                }
            }
        }
    }
}
