using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CommonPortCmd.Net
{
   public class SendEqumentCmd:ISendCmd
    {
        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetStationIp(int index)
        {
            string result = "0.0.0.0";
            switch (index)
            {
                case 0:
                    result = "192.168.0.19";
                    break;
                case 1:
                    result = "192.168.0.101";
                    break;
                case 2:
                    result = "192.168.0.102";
                    break;
                case 3:
                    result = "192.168.0.103";
                    break;
                case 4:
                    result = "192.168.0.104";
                    break;
                case 5:
                    result = "192.168.0.105";
                    break;
                case 6:
                    result = "192.168.0.106";
                    break;
            }
            return result;
        }
        public string SendCmd(int index, string action, string paramter)
        {
            string result = string.Empty;
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(GetStationIp(index)), 10108);
            using (NetworkStream stream = tcpClient.GetStream())
            {
                string s = JsonConvert.SerializeObject(new JSonCmd
                {
                    Name = "RemoteEquipmentCmd",
                    Paramters = new string[]
                        {
                            action,
                            paramter
                        }
                });
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                stream.Write(bytes, 0, bytes.Length);
                byte[] array = new byte[1024];
                int num = stream.Read(array, 0, array.Length);
                if (num > 0)
                {
                    result = Encoding.UTF8.GetString(array, 0, num);
                }
            }
            Log.Debug("action=" + action + "   " + "paramter="+paramter);
            tcpClient.Close();
            return result;
        }

    }
}
