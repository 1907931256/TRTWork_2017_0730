using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace IntegrationSys.Net
{
    static class NetUtil
    {
        public const int PORT_FILE_TRANSFER_SERVER = 10106;
        public const int PORT_LITE_DATA_SERVER = 10108;

        public static string LocalIp()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
        }

        /// <summary>
        /// 获取站位IP
        /// </summary>
        /// <returns></returns>
        public static string GetStationIp(int index)
        {
            string ip = "0.0.0.0";
            switch (index)
            {
                case 0:
                    ip = "192.168.0.101";
                    break;

                case 1:
                    ip = "192.168.0.102";
                    break;

                case 2:
                    ip = "192.168.0.103";
                    break;

                case 3:
                    ip = "192.168.0.104";
                    break;

                case 4:
                    ip = "192.168.0.105";
                    break;

                case 5:
                    ip = "192.168.0.106";
                    break;

                default:
                    break;
            }

            return ip;
        }

        public static int GetStationIndex()
        {
            string localIp = LocalIp();
            int index = 0;

            if (localIp == "192.168.0.101")
            {
                index = 0;
            }
            else if (localIp == "192.168.0.102")
            {
                index = 1;
            }
            else if (localIp == "192.168.0.103")
            {
                index = 2;
            }
            else if (localIp == "192.168.0.104")
            {
                index = 3;
            }
            else if (localIp == "192.168.0.105")
            {
                index = 4;
            }
            else if (localIp == "192.168.0.106")
            {
                index = 5;
            }

            return index;
        }
    }
}
