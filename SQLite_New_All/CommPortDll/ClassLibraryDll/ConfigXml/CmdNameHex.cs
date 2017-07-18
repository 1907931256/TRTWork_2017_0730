using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPortCmd.ConfigXml
{
    /// <summary>
    /// 解析命令映射表
    /// </summary>
    public class CmdNameHex
    {
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="hex"></param>
        public void ReadCmdNameHex(string path,out string[] name,out string[] hex)
        {
           string model=string.Empty;

            xmldoc.Load(path + "\\CmdNameHex.xml");
            XmlNodeList nodeList = xmldoc.SelectSingleNode("Main").ChildNodes;//获取节点下的所有子节点

            foreach (var item in nodeList)
            {
                
            }








        }



    }
}
