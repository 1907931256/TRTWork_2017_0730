using IntegrationSys.Assist;
using IntegrationSys.Audio;
using IntegrationSys.CommandLine;
using IntegrationSys.CommandUtils;
using IntegrationSys.Equipment;
using IntegrationSys.FileUtil;
using IntegrationSys.Image;
using IntegrationSys.LogUtil;
using IntegrationSys.Phone;
using IntegrationSys.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace IntegrationSys.Flow
{
	internal class FlowItemExecutor
	{
		private delegate void ExecuteMatchCmd(string action, string param, out string retValue);

		private const string CMD_EQUIPMENT = "设备操作";

		private const string CMD_EQUIPMENT1 = "1站设备操作";

		private const string CMD_EQUIPMENT2 = "2站设备操作";

		private const string CMD_EQUIPMENT3 = "3站设备操作";

		private const string CMD_EQUIPMENT4 = "4站设备操作";

		private const string CMD_EQUIPMENT5 = "5站设备操作";

		private const string CMD_EQUIPMENT6 = "6站设备操作";

		private const string CMD_DELAY = "延时";

		private const string CMD_ASSISTANT = "辅助操作";

		private const string CMD_PHONE = "手机操作";

		private const string CMD_COMMANDLINE = "命令行操作";

		private const string CMD_FILE = "文件操作";

		private const string CMD_RSTECH = "RStechCmd";

		private const string CMD_AUDIO = "音频操作";

		private const string CMD_RESULT = "测试结果";

		private const string CMD_IMAGE_PROCESS = "图像处理操作";

		private const string CMD_IMAGE = "图像操作";

		private Dictionary<string, FlowItemExecutor.ExecuteMatchCmd> cmdDict_;

		private FlowItem flowItem_;

		public event ExecuteFinishEventHandler ExecuteFinish;

		public FlowItemExecutor(FlowItem flowItem)
		{
			this.flowItem_ = flowItem;
			this.cmdDict_ = new Dictionary<string, FlowItemExecutor.ExecuteMatchCmd>();
			this.cmdDict_.Add("设备操作", new FlowItemExecutor.ExecuteMatchCmd(this.ExecuteEquipmentCmd));
			this.cmdDict_.Add("延时", new FlowItemExecutor.ExecuteMatchCmd(this.ExecuteDelayCmd));
			this.cmdDict_.Add("辅助操作", new FlowItemExecutor.ExecuteMatchCmd(Assistant.Instance.ExecuteCmd));
			this.cmdDict_.Add("手机操作", new FlowItemExecutor.ExecuteMatchCmd(PhoneCmd.Instance.ExecuteCmd));
			this.cmdDict_.Add("命令行操作", new FlowItemExecutor.ExecuteMatchCmd(CommandLineCmd.Instance.ExecuteCmd));
			this.cmdDict_.Add("1站设备操作", new FlowItemExecutor.ExecuteMatchCmd(RemoteEquipmentCmd1.Instance.ExecuteCmd));
			this.cmdDict_.Add("2站设备操作", new FlowItemExecutor.ExecuteMatchCmd(RemoteEquipmentCmd2.Instance.ExecuteCmd));
			this.cmdDict_.Add("3站设备操作", new FlowItemExecutor.ExecuteMatchCmd(RemoteEquipmentCmd3.Instance.ExecuteCmd));
			this.cmdDict_.Add("4站设备操作", new FlowItemExecutor.ExecuteMatchCmd(RemoteEquipmentCmd4.Instance.ExecuteCmd));
			this.cmdDict_.Add("5站设备操作", new FlowItemExecutor.ExecuteMatchCmd(RemoteEquipmentCmd5.Instance.ExecuteCmd));
			this.cmdDict_.Add("6站设备操作", new FlowItemExecutor.ExecuteMatchCmd(RemoteEquipmentCmd6.Instance.ExecuteCmd));
			this.cmdDict_.Add("文件操作", new FlowItemExecutor.ExecuteMatchCmd(FileCmd.Instance.ExecuteCmd));
			this.cmdDict_.Add("RStechCmd", new FlowItemExecutor.ExecuteMatchCmd(RStechCmd.Instance.ExecuteCmd));
			this.cmdDict_.Add("音频操作", new FlowItemExecutor.ExecuteMatchCmd(AudioCmd.Instance.ExecuteCmd));
			this.cmdDict_.Add("测试结果", new FlowItemExecutor.ExecuteMatchCmd(ResultCmd.Instance.ExecuteCmd));
			this.cmdDict_.Add("图像处理操作", new FlowItemExecutor.ExecuteMatchCmd(ImageProcessCmd.Instance.ExecuteCmd));
			//this.cmdDict_.Add("图像操作", new FlowItemExecutor.ExecuteMatchCmd(ImageCmd.Instance.ExecuteCmd));
		}


        /// <summary>
        /// 队列消息
        /// </summary>
        /// <param name="stateInfo"></param>
		public void ThreadProc(object stateInfo)
		{
			if (this.flowItem_ != null)
			{
				this.flowItem_.BeginTime = DateTime.Now;
				Log.Debug(string.Concat(new object[]
				{"Item[",this.flowItem_.Id,"] ", this.flowItem_.Item.Property.Name," start" }));


                List<Method> methodList = this.flowItem_.Item.MethodList;
				Tuple<int, int> loopFirstLast = this.GetLoopFirstLast(methodList);
				if (loopFirstLast.Item1 == -1 || loopFirstLast.Item2 == -1 || loopFirstLast.Item1 >= loopFirstLast.Item2 || this.flowItem_.Item.Property.Loop < 2)
				{
					using (List<Method>.Enumerator enumerator = methodList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Method current = enumerator.Current;
							if (!current.Disable)
							{
								string text;
								this.ExecuteCmd(current.Name, current.Action, this.ReplaceSymbol(current.Param), out text);
								if (!string.IsNullOrEmpty(current.Compare) && !string.IsNullOrEmpty(text))
								{
									this.UpdateSpecValue(text, current.Compare, this.flowItem_.SpecValueList);
								}
							}
						}
						goto IL_2B6;
					}
				}
				bool flag = false;
				int num = 0;
				while (num < this.flowItem_.Item.Property.Loop && !flag)
				{
					int i = 0;
					while (i < methodList.Count)
					{
						Method method = methodList[i];
						if (num == 0)
						{
							if (flag)
							{
								if (!method.Depend)
								{
									goto IL_207;
								}
							}
							else if (i <= loopFirstLast.Item2)
							{
								goto IL_207;
							}
						}
						else if (num != this.flowItem_.Item.Property.Loop - 1)
						{
							if (flag)
							{
								if (i > loopFirstLast.Item2)
								{
									goto IL_207;
								}
							}
							else if (method.Depend || method.Bedepend)
							{
								goto IL_207;
							}
						}
						else if (flag)
						{
							if (i > loopFirstLast.Item2)
							{
								goto IL_207;
							}
						}
						else if (method.Depend || method.Bedepend || i > loopFirstLast.Item2)
						{
							goto IL_207;
						}
						IL_27D:
						i++;
						continue;
						IL_207:
						if (method.Disable)
						{
							goto IL_27D;
						}
						string text2;
						this.ExecuteCmd(method.Name, method.Action, this.ReplaceSymbol(method.Param), out text2);
						if (!string.IsNullOrEmpty(method.Compare) && !string.IsNullOrEmpty(text2))
						{
							this.UpdateSpecValue(text2, method.Compare, this.flowItem_.SpecValueList);
						}
						if (method.Bedepend && this.flowItem_.IsPass())
						{
							flag = true;
							goto IL_27D;
						}
						goto IL_27D;
					}
					num++;
				}
				IL_2B6:
				this.flowItem_.EndTime = DateTime.Now;
				this.flowItem_.Duration = (long)(this.flowItem_.EndTime - this.flowItem_.BeginTime).TotalMilliseconds;
				Log.Debug(string.Concat(new object[]
				{
					"Item[",
					this.flowItem_.Id,
					"] ",
					this.flowItem_.Item.Property.Name,
					" finish"
				}));
				if (this.ExecuteFinish != null)
				{
					this.ExecuteFinish(this, new ExecuteFinishEventArgs(this.flowItem_));
				}
			}
		}

		private void ExecuteCmd(string name, string action, string param, out string retValue)
		{
			Log.Debug(string.Concat(new string[]
			{
				"ExecuteCmd name = ",
				name,
				", action = ",
				action,
				", param = ",
				param,
				" start"
			}));
			if (this.cmdDict_.ContainsKey(name))
			{
				this.cmdDict_[name](action, param, out retValue);
			}
			else
			{
				retValue = "Res=CmdNotSupport";
			}
			Log.Debug(string.Concat(new string[]
			{
				"ExecuteCmd name = ",
				name,
				", action = ",
				action,
				", param = ",
				param,
				", ",
				retValue
			}));
		}

		private void ExecuteEquipmentCmd(string action, string param, out string retValue)
		{
			EquipmentCmd instance = EquipmentCmd.Instance;
			instance.SendCommand(action, param, out retValue);
		}

		private void ExecuteDelayCmd(string action, string param, out string retValue)
		{
			int millisecondsTimeout = Convert.ToInt32(param);
			Thread.Sleep(millisecondsTimeout);
			retValue = "Res=Pass";
		}

		private void UpdateSpecValue(string retValue, string compare, List<SpecValue> specValueList)
		{
			if (specValueList == null || specValueList.Count == 0)
			{
				return;
			}
			if (string.IsNullOrEmpty(retValue) || string.IsNullOrEmpty(compare))
			{
				return;
			}
			string[] array = retValue.Split(new char[]
			{
				';'
			});
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!string.IsNullOrEmpty(text))
				{
					string[] array3 = text.Split(new char[]
					{
						'='
					});
					if (array3.Length == 3)
					{
						string key = array3[1].Substring(array3[1].IndexOf(':') + 1);
						dictionary[key] = array3[2];
					}
					else if (array3.Length == 2)
					{
						dictionary[array3[0]] = array3[1];
					}
				}
			}
			foreach (SpecValue current in specValueList)
			{
				if (dictionary.ContainsKey(current.SpecKey))
				{
					current.MeasuredValue = dictionary[current.SpecKey];
				}
				else
				{
					current.MeasuredValue = "None";
				}
			}
			string[] array4 = compare.Split(new char[]
			{
				' '
			});
			for (int j = 0; j < array4.Length; j++)
			{
				this.UpdateJudgmentResult(array4[j], specValueList[j]);
			}
		}

		private void UpdateJudgmentResult(string compare, SpecValue specValue)
		{
			if (compare.Equals("S=="))
			{
				specValue.JudgmentResult = (specValue.Spec.Equals(specValue.MeasuredValue) ? "成功" : "失败");
				return;
			}
			if (compare.Equals("S!="))
			{
				specValue.JudgmentResult = ((specValue.Spec != specValue.MeasuredValue) ? "成功" : "失败");
				return;
			}
			if (!compare.Equals("S[]"))
			{
				if (compare.Equals("D[]"))
				{
					if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec) && specValue.Spec.Contains('~'))
					{
						string[] array = specValue.Spec.Split(new char[]
						{
							'~'
						});
						double num = Convert.ToDouble(array[0]);
						double num2 = Convert.ToDouble(array[1]);
						try
						{
							double num3 = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, "[^\\d\\.\\-\\+]*", string.Empty));
							specValue.JudgmentResult = ((num <= num3 && num3 <= num2) ? "成功" : "失败");
							return;
						}
						catch (Exception)
						{
							specValue.JudgmentResult = "失败";
							return;
						}
					}
					specValue.JudgmentResult = "失败";
					return;
				}
				if (compare.Equals("D>="))
				{
					if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
					{
						double num4 = Convert.ToDouble(specValue.Spec);
						try
						{
							double num5 = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, "[^\\d\\.\\-\\+]*", string.Empty));
							specValue.JudgmentResult = ((num5 >= num4) ? "成功" : "失败");
							return;
						}
						catch (Exception)
						{
							specValue.JudgmentResult = "失败";
							return;
						}
					}
					specValue.JudgmentResult = "失败";
					return;
				}
				if (compare.Equals("D<="))
				{
					if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
					{
						double num6 = Convert.ToDouble(specValue.Spec);
						try
						{
							double num7 = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, "[^\\d\\.\\-\\+]*", string.Empty));
							specValue.JudgmentResult = ((num7 <= num6) ? "成功" : "失败");
							return;
						}
						catch (Exception)
						{
							specValue.JudgmentResult = "失败";
							return;
						}
					}
					specValue.JudgmentResult = "失败";
					return;
				}
				if (compare.Equals("D=="))
				{
					if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
					{
						double num8 = Convert.ToDouble(specValue.Spec);
						try
						{
							double num9 = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, "[^\\d\\.\\-\\+]*", string.Empty));
							specValue.JudgmentResult = ((num9 == num8) ? "成功" : "失败");
							return;
						}
						catch (Exception)
						{
							specValue.JudgmentResult = "失败";
							return;
						}
					}
					specValue.JudgmentResult = "失败";
				}
				return;
			}
			if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
			{
				specValue.JudgmentResult = (Regex.IsMatch(specValue.MeasuredValue, specValue.Spec) ? "成功" : "失败");
				return;
			}
			specValue.JudgmentResult = "失败";
		}

		private Tuple<int, int> GetLoopFirstLast(List<Method> methodList)
		{
			int num = -1;
			int item = -1;
			if (methodList != null)
			{
				for (int i = 0; i < methodList.Count; i++)
				{
					Method method = methodList[i];
					if (!method.Disable)
					{
						if (method.Bedepend && -1 == num)
						{
							num = i;
						}
						else if (method.Depend)
						{
							item = i;
						}
					}
				}
			}
			return new Tuple<int, int>(num, item);
		}

		private string ReplaceSymbol(string source)
		{
			string text = source.Replace("手机SN号", AppInfo.PhoneInfo.SN);
			string newValue = DateTime.Now.ToString("yyyyMMddhhmmss");
			return text.Replace("时间", newValue);
		}
	}
}
