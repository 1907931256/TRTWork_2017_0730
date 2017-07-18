using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommonPortCmd
{
    /// <summary>
    /// 握手文件配置
    /// </summary>
    public class XMLConfig
    {
        private  XmlDocument xmldoc;
        private  XmlElement xmlelem;

        /// <summary>
        /// 创建配置文件
        /// </summary>
        public void WriteConfig()
        {
            xmldoc = new XmlDocument();
            //加入Xml的段落申明
            XmlDeclaration xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmldoc.AppendChild(xmldecl);
            //加入一个根元素
            xmlelem = xmldoc.CreateElement("", "Main", "");
            xmldoc.AppendChild(xmlelem);

            XmlNode root = xmldoc.SelectSingleNode("Main");
            XmlElement Port = xmldoc.CreateElement("Type");//创建一个Node节点
            Port.SetAttribute("PC-Stance", "1");//设置该节点的属性
            Port.SetAttribute("Model", "MMI");//设置该节点的属性
            root.AppendChild(Port);

            xmldoc.Save("DataConfig.xml");
        }

        /// <summary>
        /// 读取设备信息 握手指令
        /// </summary>
        /// <param name="path">配置文件存放路径</param>
        /// <param name="Model">设备产品类别</param>
        /// <param name="PortConnect">发送的握手指令</param>
        public string ReadConfing(string path, out string Model, out string PortConnect)
        {
            string station = "";
            xmldoc = new XmlDocument();

            string model = "";
            string connect = "";

            xmldoc.Load(path + "\\DataConfig.xml");
            XmlNodeList nodeList = xmldoc.SelectSingleNode("Main").ChildNodes;//获取节点下的所有子节点

            foreach (XmlNode xn in nodeList)
            {
                #region Type解析
                if (xn.Name.ToString() == "Type")
                {
                    XmlElement xe = (XmlElement)xn;
                    model = xe.GetAttribute("Model");
                    station = xe.GetAttribute("PC-Stance");
                    if (xe.GetAttribute("Model") == "MMI")
                    {

                        switch (station)
                        {
                            case "1": connect = "72 04 11 0f 00 81";
                                station = "11";
                                break;
                            case "2": connect = "72 04 12 0f 00 81";
                                station = "12";
                                break;
                            case "3": connect = "72 04 13 0f 00 81";
                                station = "13";
                                break;
                            case "4": connect = "72 04 14 0f 00 81";
                                station = "14";
                                break;
                            case "5": connect = "72 04 15 0f 00 81";
                                station = "15";
                                break;
                            case "6": connect = "72 04 16 0f 00 81";
                                station = "16";
                                break;
                        }
                    }
                    else if (xe.GetAttribute("Model") == "CAM")
                    {
                        switch (station)
                        {
                            case "1": connect = "72 04 C1 0f 00 81";
                                station = "C1";
                                break;
                            case "2": connect = "72 04 C2 0f 00 81";
                                station = "C2";
                                break;
                            case "3": connect = "72 04 C3 0f 00 81";
                                station = "C3";
                                break;
                        }
                    }
                }
                #endregion


            }

            Model = model;
            PortConnect = connect;
            return station;
        }
    }
}
