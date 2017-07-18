using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace CommonPortCmd
{
    public class CmdPackageXml
    {

       public static List<CmdName> ReadAttribute(string filePath)
       {

           XDocument doc = XDocument.Load(filePath);
           XElement rootMain = doc.Element("CmdPackage");

           List<CmdName> cmdList = new List<CmdName>();
           List<Before> Before = new List<Before>();
           List<After> After = new List<After>();
           foreach (XElement node in rootMain.Elements("Cmd"))
           {
            
            CmdName cmd_name = new CmdName();

               //取出 loop
               cmd_name.Loop = node.Attribute("Loop").Value;
               cmd_name.Name = node.Attribute("name").Value;

               foreach (var item in node.Elements("Before"))
               {
                   Before before = new Before();


                   before.Action = item.Attribute("action").Value;

                   before.Res = item.Attribute("Res").Value;

                   Before.Add(before);

                   cmd_name.before = Before;
           
               }
               foreach (var item in node.Elements("After"))
               {
                   After after = new After();

                   //取出name action
                   after.Action = item.Attribute("action").Value;

                   after.Res = item.Attribute("Res").Value;
                   After.Add(after);
                   cmd_name.after = After;

               }
               cmdList.Add(cmd_name);
           }
           return cmdList;

       }


    }
}
