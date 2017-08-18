using IntegrationSys.CommandLine;
using IntegrationSys.Flow;
using System;
using System.Xml;

namespace IntegrationSys.Assist
{
	internal class Assistant : IExecutable
	{
		private const string ACTION_APK_INSTALL = "APK安装";

		private const string ACTION_FAILITEMS_STATISTIC = "失败项统计";

		private static Assistant instance_;

		public static Assistant Instance
		{
			get
			{
				if (Assistant.instance_ == null)
				{
					Assistant.instance_ = new Assistant();
				}
				return Assistant.instance_;
			}
		}

		private Assistant()
		{
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if (action == "APK安装")
			{
				this.ExecuteInstallCmd(param, out retValue);
				return;
			}
			if (action == "失败项统计")
			{
				this.ExecuteFailItemsStatistic(out retValue);
				return;
			}
			retValue = "Res=CmdNotSupport";
		}

		private void ExecuteInstallCmd(string param, out string retValue)
		{
			retValue = (AdbCommand.InstallApkAndStart() ? "Res=Pass" : "Res=Fail");
		}

		private void ExecuteFailItemsStatistic(out string retValue)
		{
			FlowControl instance = FlowControl.Instance;
			using (XmlWriter xmlWriter = XmlWriter.Create("result.xml"))
			{
				string value = DateTime.Now.ToString("MM-dd_hhmmss");
				int num = 0;
				xmlWriter.WriteStartElement("Main");
				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("id", Convert.ToString(num + 1));
				xmlWriter.WriteElementString("name", "SN");
				xmlWriter.WriteElementString("result", "PASS");
				xmlWriter.WriteElementString("time", value);
				xmlWriter.WriteElementString("action", "AutoTest");
				xmlWriter.WriteElementString("data", AppInfo.PhoneInfo.SN);
				xmlWriter.WriteEndElement();
				num++;
				foreach (FlowItem current in instance.FlowItemList)
				{
					if (!current.Item.Property.Disable && !current.IsAuxiliaryItem() && !current.IsPass())
					{
						xmlWriter.WriteStartElement("Item");
						xmlWriter.WriteAttributeString("id", Convert.ToString(num + 1));
						xmlWriter.WriteElementString("name", current.Name);
						xmlWriter.WriteElementString("result", "FAIL");
						xmlWriter.WriteElementString("time", value);
						xmlWriter.WriteElementString("action", "AutoTest");
						xmlWriter.WriteElementString("data", " ");
						xmlWriter.WriteEndElement();
						num++;
					}
				}
				xmlWriter.WriteEndElement();
			}
			string empty = string.Empty;
			AdbCommand.ExecuteAdbCommand("push result.xml /sdcard/", out empty);
			retValue = "Res=Pass";
		}
	}
}
