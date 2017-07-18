using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TRTSpec
{
    public class Loading
    {
        List<Specs> specsList;
        List<Params> paramsList;
        /// <summary>
        /// 前缀列表 该项目的唯一代号
        /// </summary>
        List<string> strPrafixList;


        private Specs spec_;
        private Params params_;

        /// <summary>
        /// 保存文件[]里面的内容
        /// </summary>
        private string[] params_ini_Prafix;

        /// <summary>
        /// 保存文件‘[]’里面的内容
        /// </summary>
        private string[] specs_ini_Prafix;

        /// <summary>
        /// 保存ini文件内容
        /// </summary>
        private string[] contents;



        public Loading()
        {

            specsList = new List<Specs>();
            paramsList = new List<Params>();
            strPrafixList = new List<string>();
        }

        public List<Specs> ReadSpecs_INI(string file, out bool vaild)
        {
            specs_ini_Prafix = INIHelp.INIOperationClass.INIGetAllSectionNames(file);
            vaild = false;
            for (int i = 0; i < specs_ini_Prafix.Length; i++)
            {
                //获取一个section项目下的所有内容
                contents = INIHelp.INIOperationClass.INIGetAllItems(file, specs_ini_Prafix[i]);

                string desc = FindString("Describe", contents);
                string spec = FindString("Spec", contents);
                string enspecdescribe = FindString("Enspecdescribe", contents);
                string compare = FindString("Compare", contents);
                string specenable = FindString("Specenable", contents);
                string item = specs_ini_Prafix[i];

                int specCount = Regex.Matches(spec, @" ").Count;
                int enspecdescribeCount = Regex.Matches(enspecdescribe, @" ").Count;
                int compareCount = Regex.Matches(compare, @" ").Count;
                int specenableCount = Regex.Matches(specenable, @" ").Count;

                if (specCount == enspecdescribeCount && specCount == compareCount && specCount == specenableCount)
                {
                    vaild = true;
                  
                }
                else
                {
                    vaild = false;
                    break;
                }

                spec_ = new Specs(desc, spec, enspecdescribe, compare, specenable,item);
                specsList.Add(spec_);
            }

            return specsList;
        }

        /// <summary>
        /// 读取并加载 ini 文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="vaild"></param>
        /// <returns></returns>
        public List<Params> ReadParams_INI(string file, out bool vaild)
        {
            List<string> strGroupLsit;
            List<string> strDescribeList;

            List<string> strParametersList;

            List<string> prafixList;

            params_ini_Prafix = INIHelp.INIOperationClass.INIGetAllSectionNames(file);

            vaild = false;
            for (int i = 0; i < params_ini_Prafix.Length; i++)
            {
                //获取一个section项目下的所有内容
                contents = INIHelp.INIOperationClass.INIGetAllItems(file, params_ini_Prafix[i]);

                string group = FindString("Group", contents, out strGroupLsit, out prafixList);

               FindString("Describe", contents, out strDescribeList, out prafixList);
                FindString("Parameters", contents, out strParametersList, out prafixList);

                for (int j = 0; j < strParametersList.Count; j++)
                {
                    params_ = new Params(group, strDescribeList[j], strParametersList[j], prafixList[j]);
                    paramsList.Add(params_);
                }
                strPrafixList.AddRange(prafixList);
            }

            return paramsList;

        }

        /// <summary>
        /// 在selction中找到关键字str，并返回关键字对应的value.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strContents"></param>
        /// <returns>返回key对应的value</returns>
        private string FindString(string str, string[] strContents, out List<string> strList, out List<string> prefix)
        {
            string strRec = string.Empty;
            string strPrefix = string.Empty;
            List<string> list_ = new List<string>();
            List<string> prefix_ = new List<string>();
            foreach (var item in strContents)
            {
                //&& Regex.Matches(item, @"_").Count == 1
                if (item.IndexOf(str) != -1 )
                {
                    strRec = item.Substring(item.IndexOf('=') + 1);
                    strPrefix = item.Substring(0, item.IndexOf('=') - 1);
                    list_.Add(strRec);
                    prefix_.Add(strPrefix);
                }
            }
            strList = list_;
            prefix = prefix_;
            return strRec;
        }
        /// <summary>
        /// 在selction中找到关键字str，并返回关键字对应的value.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strContents"></param>
        /// <returns>返回key对应的value</returns>
        private string FindString(string str, string[] strContents)
        {
            string strRec = string.Empty;
            foreach (var item in strContents)
            {

                if (item.IndexOf(str) != -1)//&& Regex.Matches(item, @"_").Count == 1
                {
                    strRec = item.Substring(item.IndexOf('=') + 1);
                    break;
                }
            }
            return strRec;
        }

        /// <summary>
        /// 根据集合 把数据写入到ini文件中
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ParamsList"></param>
        public void SaveData_Params(string file, List<Params> ParamsList)
        {
            for (int i = 0; i < ParamsList.Count; i++)
            {
                INIHelp.INIOperationClass.INIWriteValue(file, ParamsList[i].Group, FindParfix("_", ParamsList[i].Code_name) + "_Describe", ParamsList[i].Desc);
                INIHelp.INIOperationClass.INIWriteValue(file, ParamsList[i].Group, FindParfix("_", ParamsList[i].Code_name) + "_Parameters", ParamsList[i].Parameters);

            }
        }


        public void SaveData_Specs(string file, List<Specs> SpecsList)
        {
            for (int i = 0; i < specs_ini_Prafix.Length; i++)
            {
                INIHelp.INIOperationClass.INIWriteValue(file, specs_ini_Prafix[i], specs_ini_Prafix[i] + "_Describe", SpecsList[i].Desc);
                INIHelp.INIOperationClass.INIWriteValue(file, specs_ini_Prafix[i], specs_ini_Prafix[i] + "_Spec", SpecsList[i].Spec);
                INIHelp.INIOperationClass.INIWriteValue(file, specs_ini_Prafix[i], specs_ini_Prafix[i] + "_Enspecdescribe", SpecsList[i].Enspecdescribe);
                INIHelp.INIOperationClass.INIWriteValue(file, specs_ini_Prafix[i], specs_ini_Prafix[i] + "_Compare", SpecsList[i].Compare);
                INIHelp.INIOperationClass.INIWriteValue(file, specs_ini_Prafix[i], specs_ini_Prafix[i] + "_Specenable", SpecsList[i].Specenable);
            }
        }
        /// <summary>
        /// 截取字符串指定标记的前面部分
        /// </summary>
        /// <param name="str"></param>
        /// <param name="param">被截取的字符串</param>
        /// <returns></returns>
        private string FindParfix(string str, string param)
        {
            int idex = param.IndexOf(str);
            param = param.Substring(0, idex);
            return param;
        }
    }
}
