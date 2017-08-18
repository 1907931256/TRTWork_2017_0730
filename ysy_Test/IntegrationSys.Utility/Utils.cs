using System;
using System.Collections.Generic;

namespace IntegrationSys.Utility
{
	internal static class Utils
	{
		public static bool IsTrue(string expression)
		{
			List<string> tokenList = Utils.Parse(expression);
			tokenList = Utils.ReversePolish(tokenList);
			return Utils.Eval(tokenList);
		}

		private static List<string> ReversePolish(List<string> tokenList)
		{
			List<string> list = new List<string>();
			if (tokenList != null && tokenList.Count > 0)
			{
				Stack<string> stack = new Stack<string>();
				using (List<string>.Enumerator enumerator = tokenList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						if (Utils.IsLogicOperator(current))
						{
							if (stack.Count == 0)
							{
								stack.Push(current);
							}
							else if (current == "(")
							{
								stack.Push(current);
							}
							else
							{
								if (current == ")")
								{
									try
									{
										string item;
										while ((item = stack.Pop()) != "(")
										{
											list.Add(item);
										}
										continue;
									}
									catch (InvalidOperationException)
									{
										continue;
									}
								}
								try
								{
									while (stack.Peek() != "(")
									{
										string item2 = stack.Pop();
										list.Add(item2);
									}
								}
								catch (InvalidOperationException)
								{
								}
								stack.Push(current);
							}
						}
						else
						{
							list.Add(current);
						}
					}
					goto IL_FB;
				}
				IL_EF:
				list.Add(stack.Pop());
				IL_FB:
				if (stack.Count > 0)
				{
					goto IL_EF;
				}
			}
			return list;
		}

		private static bool Eval(List<string> tokenList)
		{
			Stack<string> stack = new Stack<string>();
			foreach (string current in tokenList)
			{
				if (Utils.IsLogicOperator(current))
				{
					string value = stack.Pop();
					string value2 = stack.Pop();
					string item = bool.FalseString;
					if (current == "+")
					{
						try
						{
							item = (bool.Parse(value2) && bool.Parse(value)).ToString();
							goto IL_A5;
						}
						catch (Exception)
						{
							item = bool.FalseString;
							goto IL_A5;
						}
						goto IL_70;
					}
					goto IL_70;
					IL_A5:
					stack.Push(item);
					continue;
					IL_70:
					if (current == "-")
					{
						try
						{
							item = (bool.Parse(value2) || bool.Parse(value)).ToString();
						}
						catch (Exception)
						{
							item = bool.FalseString;
						}
						goto IL_A5;
					}
					goto IL_A5;
				}
				else
				{
					stack.Push(current);
				}
			}
			bool result = false;
			if (stack.Count == 1)
			{
				try
				{
					result = bool.Parse(stack.Pop());
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		private static List<string> Parse(string expression)
		{
			List<string> list = new List<string>();
			expression = expression.Replace(" ", string.Empty);
			string text = string.Empty;
			string text2 = expression;
			for (int i = 0; i < text2.Length; i++)
			{
				char c = text2[i];
				if ('+' == c || '-' == c)
				{
					if (text == "+" || text == "-")
					{
						throw new ArgumentException();
					}
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
						text = string.Empty;
					}
					list.Add(c.ToString());
				}
				else if ('(' == c || ')' == c)
				{
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
						text = string.Empty;
					}
					list.Add(c.ToString());
				}
				else
				{
					text += c;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(text);
				text = string.Empty;
			}
			return list;
		}

		private static bool IsLogicOperator(string str)
		{
			return "+" == str || "-" == str || "(" == str || ")" == str;
		}
	}
}
