using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using IntegrationSys.LogUtil;
using IntegrationSys.Equipment;
using IntegrationSys.Assist;
using IntegrationSys.Phone;
using IntegrationSys.CommandLine;
using System.Text.RegularExpressions;
using IntegrationSys.FileUtil;
using IntegrationSys.CommandUtils;
using IntegrationSys.Audio;
using IntegrationSys.Result;
using IntegrationSys.Image;

namespace IntegrationSys.Flow
{
    interface IExecutable
    {
        void ExecuteCmd(string action, string param, out string retValue);
    }

    class ExecuteFinishEventArgs : EventArgs
    {
        private FlowItem flowItem_;

        public ExecuteFinishEventArgs(FlowItem flowItem)
        {
            flowItem_ = flowItem;
        }

        public FlowItem FlowItem
        {
            get
            {
                return flowItem_;
            }
        }
    }

    delegate void ExecuteFinishEventHandler(object sender, ExecuteFinishEventArgs e);

    /// <summary>
    /// 此类用于在工作线程中执行具体的测试
    /// </summary>
    class FlowItemExecutor
    {
        const string CMD_EQUIPMENT = "设备操作";
        const string CMD_EQUIPMENT1 = "1站设备操作";
        const string CMD_EQUIPMENT2 = "2站设备操作";
        const string CMD_EQUIPMENT3 = "3站设备操作";
        const string CMD_EQUIPMENT4 = "4站设备操作";
        const string CMD_EQUIPMENT5 = "5站设备操作";
        const string CMD_EQUIPMENT6 = "6站设备操作";
        const string CMD_DELAY = "延时";
        const string CMD_ASSISTANT = "辅助操作";
        const string CMD_PHONE = "手机操作";
        const string CMD_COMMANDLINE = "命令行操作";
        const string CMD_FILE = "文件操作";
        const string CMD_RSTECH = "RStechCmd";
        const string CMD_AUDIO = "音频操作";
        const string CMD_RESULT = "测试结果";
        const string CMD_IMAGE_PROCESS = "图像处理操作";
        const string CMD_IMAGE = "图像操作";
        

        private delegate void ExecuteMatchCmd(string action, string param, out string retValue);

        public event ExecuteFinishEventHandler ExecuteFinish;

        private Dictionary<string, ExecuteMatchCmd> cmdDict_;
        private FlowItem flowItem_;

        public FlowItemExecutor(FlowItem flowItem)
        {
            flowItem_ = flowItem;
            cmdDict_ = new Dictionary<string, ExecuteMatchCmd>();
            cmdDict_.Add(CMD_EQUIPMENT, new ExecuteMatchCmd(ExecuteEquipmentCmd));
            cmdDict_.Add(CMD_DELAY, new ExecuteMatchCmd(ExecuteDelayCmd));
            cmdDict_.Add(CMD_ASSISTANT, new ExecuteMatchCmd(Assistant.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_PHONE, new ExecuteMatchCmd(PhoneCmd.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_COMMANDLINE, new ExecuteMatchCmd(CommandLineCmd.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_EQUIPMENT1, new ExecuteMatchCmd(RemoteEquipmentCmd1.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_EQUIPMENT2, new ExecuteMatchCmd(RemoteEquipmentCmd2.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_EQUIPMENT3, new ExecuteMatchCmd(RemoteEquipmentCmd3.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_EQUIPMENT4, new ExecuteMatchCmd(RemoteEquipmentCmd4.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_EQUIPMENT5, new ExecuteMatchCmd(RemoteEquipmentCmd5.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_EQUIPMENT6, new ExecuteMatchCmd(RemoteEquipmentCmd6.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_FILE, new ExecuteMatchCmd(FileCmd.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_RSTECH, new ExecuteMatchCmd(RStechCmd.Instance.ExecuteCmd));
            //cmdDict_.Add(CMD_AUDIO, new ExecuteMatchCmd(AudioCmd.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_RESULT, new ExecuteMatchCmd(ResultCmd.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_IMAGE_PROCESS, new ExecuteMatchCmd(ImageProcessCmd.Instance.ExecuteCmd));
            cmdDict_.Add(CMD_IMAGE, new ExecuteMatchCmd(ImageCmd.Instance.ExecuteCmd));
        }

        public void ThreadProc(Object stateInfo)
        { 
            if (flowItem_ != null)
            {
                flowItem_.BeginTime = DateTime.Now;
                Log.Debug("Item[" + flowItem_.Id + "] " + flowItem_.Item.Property.Name + " start");

                List<Method> methodList = flowItem_.Item.MethodList;
                Tuple<int, int> firstLast = GetLoopFirstLast(methodList);
                if (firstLast.Item1 == -1 || firstLast.Item2 == -1
                    || firstLast.Item1 >= firstLast.Item2
                    || flowItem_.Item.Property.Loop < 2)
                {
                    //不需要循环
                    foreach (Method method in methodList)
                    {
                        if (!method.Disable)
                        {
                            string retValue;
                            ExecuteCmd(method.Name, method.Action, ReplaceSymbol(method.Param), out retValue);
                            if (!string.IsNullOrEmpty(method.Compare) && !string.IsNullOrEmpty(retValue))
                            {
                                UpdateSpecValue(retValue, method.Compare, flowItem_.SpecValueList);
                            }   
                        }
                    }
                }
                else
                {
                    bool loopExit = false;
                    for (int i = 0; i < flowItem_.Item.Property.Loop && !loopExit; i++)
                    {
                        for (int j = 0; j < methodList.Count; j++)
                        {
                            Method method = methodList[j];
                            if (i == 0)
                            {
                                if (loopExit)
                                {
                                    if (method.Depend) continue;
                                }
                                else
                                {
                                    if (j > firstLast.Item2) continue;
                                }
                            }
                            else if (i != flowItem_.Item.Property.Loop - 1)
                            {
                                if (loopExit)
                                {
                                    if (j <= firstLast.Item2) continue;
                                }
                                else
                                {
                                    if (!method.Depend && !method.Bedepend) continue;
                                }
                            }
                            else
                            {
                                if (loopExit)
                                {
                                    if (j <= firstLast.Item2) continue;
                                }
                                else
                                {
                                    if (!method.Depend && !method.Bedepend && j <= firstLast.Item2) continue;
                                }
                            }

                            if (!method.Disable)
                            {
                                string retValue;
                                ExecuteCmd(method.Name, method.Action, ReplaceSymbol(method.Param), out retValue);
                                if (!string.IsNullOrEmpty(method.Compare) && !string.IsNullOrEmpty(retValue))
                                {
                                    UpdateSpecValue(retValue, method.Compare, flowItem_.SpecValueList);
                                }

                                if (method.Bedepend && flowItem_.IsPass())
                                {
                                    loopExit = true;
                                }
                            }

                        }
                    }
                }

                flowItem_.EndTime = DateTime.Now;
                flowItem_.Duration = (long)(flowItem_.EndTime - flowItem_.BeginTime).TotalMilliseconds;

                Log.Debug("Item[" + flowItem_.Id + "] " + flowItem_.Item.Property.Name + " finish");

                if (ExecuteFinish != null)
                {
                    ExecuteFinish(this, new ExecuteFinishEventArgs(flowItem_));
                }
            }
        }

        /// <summary>
        /// 执行具体的测试命令
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="param"></param>
        /// <param name="retValue">命令返回结果</param>
        private void ExecuteCmd(string name, string action, string param, out string retValue)
        {
            Log.Debug("ExecuteCmd name = " + name + ", action = " + action + ", param = " + param + " start");
            if (cmdDict_.ContainsKey(name))
            {
                cmdDict_[name](action, param, out retValue);
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }
            Log.Debug("ExecuteCmd name = " + name + ", action = " + action + ", param = " + param + ", " + retValue);
        }

        private void ExecuteEquipmentCmd(string action, string param, out string retValue)
        {
            EquipmentCmd cmd = EquipmentCmd.Instance;
            cmd.SendCommand(action, param, out retValue);
        }

        private void ExecuteDelayCmd(string action, string param, out string retValue)
        {
            int timeout = Convert.ToInt32(param);
            Thread.Sleep(timeout);

            retValue = "Res=Pass";
        }

        /// <summary>
        /// 根据命令返回值和比较方法来更新FlowItem中的SpecValueList
        /// </summary>
        /// <param name="retValue">类似key1=value1;key2=value2;key3=value3的字串</param>
        /// <param name="compare">类似S==;D[];D>=的字串</param>
        /// <param name="specValueList"></param>
        private void UpdateSpecValue(string retValue, string compare, List<SpecValue> specValueList)
        {
            if (specValueList == null || specValueList.Count == 0) return;

            if (string.IsNullOrEmpty(retValue) || string.IsNullOrEmpty(compare)) return;

            //parse key-value pair
            string[] keyValueArray = retValue.Split(';');
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string keyValue in keyValueArray)
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    string[] pair = keyValue.Split('=');
                    if (pair.Length == 3)
                    {
                        //针对类似返回值Res=PASS:key=value;的特殊处理
                        string key = pair[1].Substring(pair[1].IndexOf(':') + 1);
                        dict[key] = pair[2];
                    }
                    else if (pair.Length == 2)
                    {
                        dict[pair[0]] = pair[1];
                    }
                }
            }

            foreach (SpecValue specValue in specValueList)
            {
                if (dict.ContainsKey(specValue.SpecKey))
                {
                    specValue.MeasuredValue = dict[specValue.SpecKey];
                }
                else
                {
                    specValue.MeasuredValue = "None";
                }
            }

            string[] compareArray = compare.Split(' ');
            for (int i = 0; i < compareArray.Length; i++)
            {
                UpdateJudgmentResult(compareArray[i], specValueList[i]);
            }

        }

        private void UpdateJudgmentResult(string compare, SpecValue specValue)
        {
            if (compare.Equals("S=="))
            {
                bool pass = specValue.Spec.Equals(specValue.MeasuredValue);
                specValue.JudgmentResult = pass ? CommonString.RESULT_SUCCEED : CommonString.RESULT_FAILURE;
            }
            else if (compare.Equals("S!="))
            {
                bool pass = specValue.Spec != specValue.MeasuredValue;
                specValue.JudgmentResult = pass ? CommonString.RESULT_SUCCEED : CommonString.RESULT_FAILURE;
            }
            else if (compare.Equals("S[]"))   //正则表达式
            {
                if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
                {
                    bool pass = Regex.IsMatch(specValue.MeasuredValue, specValue.Spec);
                    specValue.JudgmentResult = pass ? CommonString.RESULT_SUCCEED : CommonString.RESULT_FAILURE;
                }
                else
                {
                    specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                }
            }
            else if (compare.Equals("D[]"))   //上下限
            {
                if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec) && specValue.Spec.Contains('~'))
                {
                    string[] bound = specValue.Spec.Split('~');
                    double lower = Convert.ToDouble(bound[0]);
                    double upper = Convert.ToDouble(bound[1]);
                    try
                    {
                        double value = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, @"[^\d\.\-\+]*", string.Empty));
                        bool pass = lower <= value && value <= upper;
                        specValue.JudgmentResult = pass ? CommonString.RESULT_SUCCEED : CommonString.RESULT_FAILURE;
                    }
                    catch (Exception)
                    {
                        specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                    }

                }
                else
                {
                    specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                }
            }
            else if (compare.Equals("D>="))   //下限
            {
                if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
                {
                    double lower = Convert.ToDouble(specValue.Spec);
                    try
                    {
                        double value = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, @"[^\d\.\-\+]*", string.Empty));
                        bool pass = value >= lower;
                        specValue.JudgmentResult = pass ? CommonString.RESULT_SUCCEED : CommonString.RESULT_FAILURE;
                    }
                    catch (Exception)
                    {
                        specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                    }
                }
                else
                {
                    specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                }
            }
            else if (compare.Equals("D<="))   //上限
            {
                if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
                {
                    double upper = Convert.ToDouble(specValue.Spec);
                    try
                    {
                        double value = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, @"[^\d\.\-\+]*", string.Empty));
                        bool pass = value <= upper;
                        specValue.JudgmentResult = pass ? CommonString.RESULT_SUCCEED : CommonString.RESULT_FAILURE;
                    }
                    catch (Exception)
                    {
                        specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                    }
                }
                else
                {
                    specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                }
            }
            else if (compare.Equals("D=="))
            {
                if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
                {
                    double spec = Convert.ToDouble(specValue.Spec);
                    try
                    {
                        double value = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, @"[^\d\.\-\+]*", string.Empty));
                        bool pass = value == spec;
                        specValue.JudgmentResult = pass ? CommonString.RESULT_SUCCEED : CommonString.RESULT_FAILURE;
                    }
                    catch (Exception)
                    {
                        specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                    }
                }
                else
                {
                    specValue.JudgmentResult = CommonString.RESULT_FAILURE;
                }
            }
        }

        //获取methodList中需要循环执行的第一项和最后一项索引
        private Tuple<int, int> GetLoopFirstLast(List<Method> methodList)
        {
            int first = -1;
            int last = -1;

            if (methodList != null)
            {
                for (int i = 0; i < methodList.Count; i++)
                {
                    Method m = methodList[i];
                    if (!m.Disable)
                    {
                        if (m.Bedepend && -1 == first)
                        {
                            first = i;
                        }
                        else if (m.Depend)
                        {
                            last = i;
                        }
                    }
                }
            }

            return new Tuple<int, int>(first, last);
        }

        /// <summary>
        /// 替换source中带有的 "手机SN号" "时间" 特殊标志
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string ReplaceSymbol(string source)
        {
            string dest = source.Replace("手机SN号", AppInfo.PhoneInfo.SN);

            string time = DateTime.Now.ToString("yyyyMMddhhmmss");
            dest = dest.Replace("时间", time);

            return dest;
        }
    }
}
