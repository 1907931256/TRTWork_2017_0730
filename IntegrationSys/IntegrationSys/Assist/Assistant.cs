using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.CommandLine;
using IntegrationSys.Flow;
using System.Xml;

namespace IntegrationSys.Assist
{
    class Assistant : IExecutable
    {
        const string ACTION_APK_INSTALL = "APK安装";
        const string ACTION_FAILITEMS_STATISTIC = "失败项统计";

        private static Assistant instance_ = null;

        private Assistant()
        {
 
        }

        public static Assistant Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new Assistant();
                }
                return instance_;
            }
        }

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (action == ACTION_APK_INSTALL)
            {
                ExecuteInstallCmd(param, out retValue);
            }
            else if (action == ACTION_FAILITEMS_STATISTIC)
            {
                ExecuteFailItemsStatistic(out retValue);
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }
        }

        private void ExecuteInstallCmd(string param, out string retValue)
        {
            bool ret = AdbCommand.InstallApkAndStart();

            retValue = ret ? "Res=Pass" : "Res=Fail";
        }

        private void ExecuteFailItemsStatistic(out string retValue)
        {
            FlowControl flowControl = FlowControl.Instance;

            using (XmlWriter writer = XmlWriter.Create("result.xml"))
            {
                string now = DateTime.Now.ToString("MM-dd_hhmmss");
                //写入SN
                int index = 0;
                writer.WriteStartElement("Main");
                writer.WriteStartElement("Item");
                writer.WriteAttributeString("id", Convert.ToString(index + 1));
                writer.WriteElementString("name", "SN");
                writer.WriteElementString("result", "PASS");
                writer.WriteElementString("time", now);
                writer.WriteElementString("action", "AutoTest");
                writer.WriteElementString("data", AppInfo.PhoneInfo.SN);
                writer.WriteEndElement();   //Item

                index++;

                foreach (FlowItem flowItem in flowControl.FlowItemList)
                {
                    if (!flowItem.Item.Property.Disable && !flowItem.IsAuxiliaryItem() && !flowItem.IsPass())
                    {
                        writer.WriteStartElement("Item");
                        writer.WriteAttributeString("id", Convert.ToString(index + 1));
                        writer.WriteElementString("name", flowItem.Name);
                        writer.WriteElementString("result", "FAIL");
                        writer.WriteElementString("time", now);
                        writer.WriteElementString("action", "AutoTest");
                        writer.WriteElementString("data", " ");
                        writer.WriteEndElement();   //Item

                        index++;                        
                    }
                }

                writer.WriteEndElement();   //Main
            }

            retValue = "Res=Pass";
        }
    }
}
