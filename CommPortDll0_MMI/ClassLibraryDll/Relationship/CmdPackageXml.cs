using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace CommonPortCmd
{
    public class CmdPackageXml
    {

       public static List<CmdName> LoadFlowtest(string filePath)
       {

           XDocument doc = XDocument.Load(filePath);
           XElement rootMain = doc.Element("CmdPackage");


           //新建和命令个数匹配的类
           //int count_cmd = rootMain.Elements("Cmd").Count();
           //CmdName[] cmd = new CmdName[count_cmd];
           

           List<CmdName> cmdList = new List<CmdName>();
           List<Action_> actList = new List<Action_>();
          
           int i = 0;
           foreach (XElement node in rootMain.Elements("Cmd"))
           {
            
            CmdName cmd_name = new CmdName();

               //取出 loop
               cmd_name.Loop = node.Attribute("Loop").Value;
               //取出 name
               string name = node.Attribute("name").Value;
               cmd_name.Name = node.Attribute("name").Value;

               int count_act = node.Elements("action").Count();
               int j = 0;
               foreach (var item in node.Elements("action"))
               {
                   Action_ act = new Action_();

                   //取出name action
                   act.Name = item.Attribute("name").Value;

                   act.Res = item.Attribute("Res").Value;
                   actList.Add(act);
                   cmd_name.action = actList;
           
                   j++;
               }
               cmdList.Add(cmd_name);
               i++;

           }
           return cmdList;

       }


    }
}
