using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegrationSys.Utility
{
    static class Utils
    {
        /// <summary>
        /// 逻辑表达式求值
        /// 通过转化为逆波兰式来求值
        /// </summary>
        /// <param name="expression">类似 true || false && true</param>
        /// <returns></returns>
        public static bool IsTrue(string expression)
        {
            List<string> tokenList = Parse(expression);

            tokenList = ReversePolish(tokenList);

            return Eval(tokenList);
        }

        /// 1）如果遇到操作数，我们就直接将其输出
        /// 2）如果遇到操作符，则我们将其放入到栈中，遇到左括号时我们也将其放入栈中
        /// 3）如果遇到一个右括号，则将栈元素弹出，将弹出的操作符输出直到遇到左括号为止。注意，左括号只弹出并不输出
        /// 4）如果遇到任何其他的操作符，如（“+”， “*”，“（”， “&&”， “||”）等，从栈中弹出元素直到遇到发现更低优先级的元素(或者栈为空)为止
        /// 5）如果我们读到了输入的末尾，则将栈中所有元素依次弹出
        private static List<string> ReversePolish(List<string> tokenList)
        {
            List<string> notationTokenList = new List<string>();

            if (tokenList != null && tokenList.Count > 0)
            {
                Stack<string> operatorStack = new Stack<string>();
                foreach (string token in tokenList)
                {
                    if (IsLogicOperator(token))
                    {
                        if (operatorStack.Count == 0)
                        {
                            operatorStack.Push(token);
                        }
                        else
                        {
                            if (token == "(")
                            {
                                operatorStack.Push(token);
                            }
                            else if (token == ")")
                            {
                                string top;

                                try
                                {
                                    while ((top = operatorStack.Pop()) != "(")
                                    {
                                        notationTokenList.Add(top);
                                    }
                                }
                                catch (InvalidOperationException)
                                {
                                }
                            }
                            else
                            {
                                string top;

                                try
                                {
                                    while ((top = operatorStack.Peek()) != "(")
                                    {
                                        top = operatorStack.Pop();
                                        notationTokenList.Add(top);
                                    }
                                }
                                catch (InvalidOperationException)
                                {

                                }

                                operatorStack.Push(token);
                            }
                        }
                    }
                    else
                    {
                        notationTokenList.Add(token);
                    }
                }

                while (operatorStack.Count > 0)
                {
                    notationTokenList.Add(operatorStack.Pop());
                }
            }

            return notationTokenList;
        }

        /// <summary>
        /// 逆波兰式求值
        /// </summary>
        /// <param name="tokenList">逆波兰式表达式</param>
        /// <returns></returns>
        private static bool Eval(List<string> tokenList)
        {
            Stack<string> stack = new Stack<string>();

            foreach (string token in tokenList)
            {
                if (IsLogicOperator(token))
                {
                    string rhs = stack.Pop();
                    string lhs = stack.Pop();

                    string result = Boolean.FalseString;
                    if (token == "+")
                    {
                        try
                        {
                            result = (Boolean.Parse(lhs) && Boolean.Parse(rhs)).ToString();
                        }
                        catch (Exception)
                        {
                            result = Boolean.FalseString;
                        }
                    }
                    else if (token == "-")
                    {
                        try
                        {
                            result = (Boolean.Parse(lhs) || Boolean.Parse(rhs)).ToString();
                        }
                        catch (Exception)
                        {
                            result = Boolean.FalseString;
                        }
                    }
                    stack.Push(result);
                }
                else
                {
                    stack.Push(token);
                }
            }

            bool pass = false;
            if (stack.Count == 1)
            {
                try
                {
                    pass = Boolean.Parse(stack.Pop());
                }
                catch (Exception)
                {
                }
            }

            return pass;
        }

        /// <summary>
        /// 表达式解析，将每个token存在list中
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static List<string> Parse(string expression)
        {
            List<string> tokenList = new List<string>();

            expression = expression.Replace(" ", string.Empty);

            string token = string.Empty;
            foreach (char s in expression)
            {
                if ('+' == s || '-' == s)
                {
                    if (token == "+" || token == "-")
                    {
                        throw new ArgumentException();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(token))
                        {
                            tokenList.Add(token);
                            token = string.Empty;
                        }
                        tokenList.Add(s.ToString());
                    }
                }
                else if ('(' == s || ')' == s)
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        tokenList.Add(token);
                        token = string.Empty;
                    }

                    tokenList.Add(s.ToString());
                }
                else
                {
                    token += s;
                }
            }

            if (!string.IsNullOrEmpty(token))
            {
                tokenList.Add(token);
                token = string.Empty;
            }


            return tokenList;
        }

        /// <summary>
        /// 是否是逻辑操作符
        /// </summary>
        /// <returns></returns>
        private static bool IsLogicOperator(string str)
        {
            if ("+" == str)
            {
                return true;
            }
            else if ("-" == str)
            {
                return true;
            }
            else if ("(" == str)
            {
                return true;
            }
            else if (")" == str)
            {
                return true;
            }
            else
            {
                return false;
            }
        }        
    }
}
