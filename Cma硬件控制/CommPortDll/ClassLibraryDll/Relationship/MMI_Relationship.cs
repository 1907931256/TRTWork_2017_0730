using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommonPortCmd
{
    public class MMI_Relationship
    {

       // Common comm_ = Common.NewCommon();
        

       // public void RelShip(string strCmd)
       //{
       //    string[] res=new string[10];
       //    List<CmdName> cmdList = new List<CmdName>();
       //    List<Before> before = new List<Before>();
       //    List<After> after = new List<After>();
       //     cmdList=CmdPackageXml.ReadAttribute("CmdPackage.xml");
       //     foreach (var item in cmdList)
       //     {
       //         for (int i = 0; i < Convert.ToInt32(item.Loop); i++)
       //         {
       //             if (item.Name == strCmd)
       //             {
       //                 before = item.before;
       //                 foreach (var bf in before)
       //                 {
       //                     if (bf.Action.IndexOf("延时") != -1)
       //                     {
       //                         int time = Convert.ToInt32(bf.Action.Substring(bf.Action.IndexOf(" ") + 1));
       //                         Thread.Sleep(time);
       //                     }
       //                     else
       //                     {
       //                       res[i]=bf.Res;
       //                      comm_.SendCommand(bf.Action, out res[i]);
       //                     }
                                
       //                 }
                        
       //                 if (Array.IndexOf(res,"OK")!=-1)
       //                 {
       //                     comm_.SendCommand(item.Name, out res[0]);
       //                 }
       //                 else if(res[0].IndexOf("OK")!=-1)
       //                 {
       //                     after = item.after;

       //                     foreach (var af in after)
       //                     {
       //                         if (af.Action.IndexOf("延时") != -1)
       //                         {
       //                             int time = Convert.ToInt32(af.Action.Substring(item.Name.IndexOf(" ") + 1));
       //                             Thread.Sleep(time);
       //                         }
       //                         else
       //                         {
       //                             res[i] = af.Res;
       //                             comm_.SendCommand(af.Action, out res[i]);

       //                         }
       //                     }

       //                 }
                       
       //             }
       //         }

       //     }
       //}



    }
}
