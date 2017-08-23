using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Utility;
using IntegrationSys.Net;

namespace IntegrationSys.Flow
{

    /// <summary>
    /// 记录规格值和实测值
    /// </summary>
    class SpecValue
    {
        public string SpecDescription
        {
            get;
            set;
        }

        public string Spec
        {
            get;
            set;
        }

        public String SpecKey
        {
            get;
            set;
        }

        public string MeasuredValue
        {
            get;
            set;
        }

        public string JudgmentResult
        {
            get;
            set;
        }

        public bool Disable
        {
            get;
            set;
        }

    }


    class FlowItem
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
                return item_;
            }
            set
            {
                item_ = value;
                if (item_.Property != null && !string.IsNullOrEmpty(item_.Property.Spec))
                {
                    string[] specDescriptions = item_.Property.SpecDescription.Split(' ');
                    string[] specs = item_.Property.Spec.Split(' ');
                    string[] specKeys = item_.Property.SpecKey.Split(' ');
                    string[] specsEnable = null;
                    if (!string.IsNullOrEmpty(item_.Property.SpecEnable))
                    {
                        specsEnable = item_.Property.SpecEnable.Split(' ');
                    }

                    for (int i = 0; i < specs.Length; i++)
                    {
                        SpecValue specValue = new SpecValue();
                        specValue.SpecDescription = specDescriptions[i];
                        specValue.Spec = specs[i];
                        specValue.SpecKey = specKeys[i];
                        if (specsEnable != null && specsEnable.Length > i && specsEnable[i] == "0")
                        {
                            specValue.Disable = true;
                        }
                        AddSpecValue(specValue);
                    }
                }
            }
        }

        public int Id
        {
            get 
            {
                return Item.Id;
            }
        }

        public string Name
        {
            get
            {
                return Item.Property.Name;            
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
                int type = 0;
                try
                {
                    type = Int32.Parse(Item.Property.Alarm);
                }
                catch (Exception)
                {
                    type = 0;
                }

                return type;
            }
        }

        public int SwitchType
        {
            get
            {
                int type = 0;
                try
                {
                    type = Int32.Parse(Item.Property.Switch);

                    if (type == 2 && NetUtil.GetStationIndex() != 0)
                    {
                        type = 1;
                    }
                }
                catch (Exception)
                {
                    type = 0;
                }

                return type;
            }
        }

        /// <summary>
        /// 此测试项在DataGridView表格中的行索引
        /// </summary>
        public int RowIndex
        {
            get;
            set;
        }

        public HashSet<int> DependSet
        {
            get 
            {
                return dependSet_;
            }
        }

        public HashSet<int> BedependSet
        {
            get
            {
                return bedependSet_;
            }
        }

        /// <summary>
        /// 增加依赖项
        /// </summary>
        /// <param name="depend"></param>
        public void AddDepend(int depend)
        {
            if (dependSet_ == null)
            {
                dependSet_ = new HashSet<int>();
            }
            dependSet_.Add(depend);
        }

        public void AddBedepend(int bedepend)
        {
            if (bedependSet_ == null)
            {
                bedependSet_ = new HashSet<int>();
            }
            bedependSet_.Add(bedepend);
        }

        public List<SpecValue> SpecValueList
        {
            get
            {
                return specValueList_;
            }
        }

        public void AddSpecValue(SpecValue specValue)
        {
            if (specValueList_ == null)
            {
                specValueList_ = new List<SpecValue>();
            }
            specValueList_.Add(specValue);
        }

        public bool IsPass()
        {
            if (specValueList_ != null)
            {
                foreach (SpecValue specValue in specValueList_)
                {
                    if (specValue.Disable) continue;

                    if (string.IsNullOrEmpty(specValue.JudgmentResult)
                    || !specValue.JudgmentResult.Equals(CommonString.RESULT_SUCCEED))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsFinished()
        {
            return Status == STATUS_FINISH;
        }

        /// <summary>
        /// 是否需要跳过测试
        /// 如果跳过测试指令，则直接将实测值赋值为Skip，测试结果赋值为失败
        /// </summary>
        /// <returns></returns>
        public bool IsSkip()
        {
            if (!String.IsNullOrEmpty(Item.Property.Condition))
            {
                string condition = Item.Property.Condition.Replace(" ", String.Empty);

                string expression = String.Empty;
                string operand = String.Empty;
                bool not = false;
                foreach (char c in condition)
                {
                    if (c == '+' || c == '-' || c == '(' || c == ')')
                    {
                        if (!String.IsNullOrEmpty(operand))
                        {
                            int id = Int32.Parse(operand);

                            FlowItem flowItem = FlowControl.Instance.GetFlowItem(id);

                            bool pass = false;
                            if (flowItem != null && flowItem.IsPass())
                            {
                                pass = true;
                            }

                            if (not)
                            {
                                pass = !pass;
                            }

                            expression += pass ? Boolean.TrueString : Boolean.FalseString;

                            operand = String.Empty;
                            not = false;
                        }

                        expression += c;
                    }
                    else if (c == '!')
                    {
                        not = true;
                    }
                    else
                    {
                        operand += c;
                    }
                }

                if (!String.IsNullOrEmpty(operand))
                {
                    int id = Int32.Parse(operand);

                    FlowItem flowItem = FlowControl.Instance.GetFlowItem(id);

                    bool pass = false;
                    if (flowItem != null && flowItem.IsPass())
                    {
                        pass = true;
                    }

                    if (not)
                    {
                        pass = !pass;
                    }

                    expression += pass ? Boolean.TrueString : Boolean.FalseString;
                    operand = String.Empty;
                    not = false;
                }

                return !Utils.IsTrue(expression);
            }

            return false;
        }

        /// <summary>
        /// Item分为真正的测试项和辅助项两类
        /// </summary>
        /// <returns></returns>
        public bool IsAuxiliaryItem()
        {
            return Item.Property.Hide;
        }
    }
}
