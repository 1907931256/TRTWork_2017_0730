using IntegrationSys.Assist;
using IntegrationSys.CommandLine;
using IntegrationSys.Equipment;
using IntegrationSys.LogUtil;
using IntegrationSys.Phone;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace IntegrationSys.Flow
{
	internal class FlowItemExecutor
	{
		private delegate void ExecuteMatchCmd(string action, string param, out string retValue);

		private const string CMD_EQUIPMENT = "设备操作";

		private const string CMD_DELAY = "延时";

		private const string CMD_ASSISTANT = "辅助操作";

		private const string CMD_PHONE = "手机操作";

		private const string CMD_COMMANDLINE = "命令行操作";

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
		}

		public void ThreadProc(object stateInfo)
		{
			if (this.flowItem_ != null)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				Log.Debug(string.Concat(new object[]
				{
					"Item[",
					this.flowItem_.Id,
					"] ",
					this.flowItem_.Item.Property.Name,
					" start"
				}));
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
								this.ExecuteCmd(current.Name, current.Action, current.Param, out text);
								if (!string.IsNullOrEmpty(current.Compare) && !string.IsNullOrEmpty(text))
								{
									this.UpdateSpecValue(text, current.Compare, this.flowItem_.SpecValueList);
								}
							}
						}
						goto IL_2A5;
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
									goto IL_1FC;
								}
							}
							else if (i <= loopFirstLast.Item2)
							{
								goto IL_1FC;
							}
						}
						else if (num != this.flowItem_.Item.Property.Loop - 1)
						{
							if (flag)
							{
								if (i > loopFirstLast.Item2)
								{
									goto IL_1FC;
								}
							}
							else if (method.Depend || method.Bedepend)
							{
								goto IL_1FC;
							}
						}
						else if (flag)
						{
							if (i > loopFirstLast.Item2)
							{
								goto IL_1FC;
							}
						}
						else if (method.Depend || method.Bedepend || i > loopFirstLast.Item2)
						{
							goto IL_1FC;
						}
						IL_26C:
						i++;
						continue;
						IL_1FC:
						if (method.Disable)
						{
							goto IL_26C;
						}
						string text2;
						this.ExecuteCmd(method.Name, method.Action, method.Param, out text2);
						if (!string.IsNullOrEmpty(method.Compare) && !string.IsNullOrEmpty(text2))
						{
							this.UpdateSpecValue(text2, method.Compare, this.flowItem_.SpecValueList);
						}
						if (method.Bedepend && this.flowItem_.IsPass())
						{
							flag = true;
							goto IL_26C;
						}
						goto IL_26C;
					}
					num++;
				}
				IL_2A5:
				this.flowItem_.Status = 2;
				stopwatch.Stop();
				this.flowItem_.Duration = stopwatch.ElapsedMilliseconds;
				if (this.ExecuteFinish != null)
				{
					this.ExecuteFinish(this, new ExecuteFinishEventArgs(this.flowItem_));
				}
				Log.Debug(string.Concat(new object[]
				{
					"Item[",
					this.flowItem_.Id,
					"] ",
					this.flowItem_.Item.Property.Name,
					" finish"
				}));
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
				", value = ",
				retValue
			}));
		}

		private void ExecuteEquipmentCmd(string action, string param, out string retValue)
		{
			EquipmentCmd instance = EquipmentCmd.Instance;
			if (string.IsNullOrEmpty(param))
			{
				instance.SendCommand(action, out retValue);
				return;
			}
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
						dictionary.Add(key, array3[2]);
					}
					else if (array3.Length == 2)
					{
						dictionary.Add(array3[0], array3[1]);
					}
				}
			}
			foreach (SpecValue current in specValueList)
			{
				if (dictionary.ContainsKey(current.SpecKey))
				{
					current.MeasuredValue = dictionary[current.SpecKey];
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
					double num3 = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, "[^\\d]*", string.Empty));
					specValue.JudgmentResult = ((num <= num3 && num3 <= num2) ? "成功" : "失败");
					return;
				}
				specValue.JudgmentResult = "失败";
				return;
			}
			else if (compare.Equals("D>="))
			{
				if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
				{
					double num4 = Convert.ToDouble(specValue.Spec);
					double num5 = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, "[^\\d]*", string.Empty));
					specValue.JudgmentResult = ((num5 >= num4) ? "成功" : "失败");
					return;
				}
				specValue.JudgmentResult = "失败";
				return;
			}
			else
			{
				if (!compare.Equals("D<="))
				{
					if (compare.Equals("S[]"))
					{
						if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
						{
							specValue.JudgmentResult = (Regex.IsMatch(specValue.MeasuredValue, specValue.Spec) ? "成功" : "失败");
							return;
						}
						specValue.JudgmentResult = "失败";
					}
					return;
				}
				if (!string.IsNullOrEmpty(specValue.MeasuredValue) && !string.IsNullOrEmpty(specValue.Spec))
				{
					double num6 = Convert.ToDouble(specValue.Spec);
					double num7 = Convert.ToDouble(Regex.Replace(specValue.MeasuredValue, "[^\\d]*", string.Empty));
					specValue.JudgmentResult = ((num7 <= num6) ? "成功" : "失败");
					return;
				}
				specValue.JudgmentResult = "失败";
				return;
			}
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
	}
}
