using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TRTCamera
{
   public class LiteData
    {
        public string Name
        {
            get;
            set;
        }

        public string[] Paramters
        {
            get;
            set;
        }
        public static string SendEquipmentCmd(int index, string action, string paramter)
        {
            string result = string.Empty;
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(GetStationIp(index)), 10108);
            //tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 10108);
            using (NetworkStream stream = tcpClient.GetStream())
            {
                string s = JsonConvert.SerializeObject(new LiteData
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
            tcpClient.Close();
            return result;
        }
        public static string GetStationIp(int index)
        {
            string result = "0.0.0.0";
            switch (index)
            {
                case 0:
                    result = "192.168.0.19";
                    break;
                case 1:
                    result = "192.168.0.102";
                    break;
                case 2:
                    result = "192.168.0.103";
                    break;
                case 3:
                    result = "192.168.0.104";
                    break;
                case 4:
                    result = "192.168.0.105";
                    break;
                case 5:
                    result = "192.168.0.106";
                    break;
            }
            return result;
        }
    }
}
