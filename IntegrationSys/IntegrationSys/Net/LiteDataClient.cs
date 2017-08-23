using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using IntegrationSys.Equipment;
using Newtonsoft.Json;
using IntegrationSys.LogUtil;

namespace IntegrationSys.Net
{
    class LiteDataClient
    {
        private static LiteDataClient instance_;
        private LiteDataClient()
        { 
        }

        public static LiteDataClient Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new LiteDataClient();
                }

                return instance_;
            }
        }

        public bool SendInplaceFlag(int index)
        {
            bool result = false;
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(NetUtil.GetStationIp(0)), NetUtil.PORT_LITE_DATA_SERVER);
            using (NetworkStream stream = client.GetStream())
            {
                string request = "Inplace " + index;
                byte[] requestBuffer = System.Text.Encoding.UTF8.GetBytes(request);
                stream.Write(requestBuffer, 0, requestBuffer.Length);

                byte[] respBuffer = new byte[1024];
                int len = stream.Read(respBuffer, 0, respBuffer.Length);
                if (len > 0)
                {
                    string response = System.Text.Encoding.UTF8.GetString(respBuffer, 0, len);
                    result = true;
                }
            }

            client.Close();
            return result;
        }

        public bool SendCompleteFlag(int index)
        {
            bool result = false;
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(NetUtil.GetStationIp(0)), NetUtil.PORT_LITE_DATA_SERVER);
            using (NetworkStream stream = client.GetStream())
            {
                string request = "Complete " + index;
                byte[] requestBuffer = System.Text.Encoding.UTF8.GetBytes(request);
                stream.Write(requestBuffer, 0, requestBuffer.Length);

                byte[] respBuffer = new byte[1024];
                int len = stream.Read(respBuffer, 0, respBuffer.Length);
                if (len > 0)
                {
                    string response = System.Text.Encoding.UTF8.GetString(respBuffer, 0, len);
                    result = true;
                }
            }

            client.Close();
            return result;
        }

        public bool SendPickPlace(int index)
        {
            bool result = false;
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(NetUtil.GetStationIp(index)), NetUtil.PORT_LITE_DATA_SERVER);
            using (NetworkStream stream = client.GetStream())
            {
                string request = "PickPlace " + index;
                byte[] requestBuffer = System.Text.Encoding.UTF8.GetBytes(request);
                stream.Write(requestBuffer, 0, requestBuffer.Length);

                byte[] respBuffer = new byte[1024];
                int len = stream.Read(respBuffer, 0, respBuffer.Length);
                if (len > 0)
                {
                    string response = System.Text.Encoding.UTF8.GetString(respBuffer, 0, len);
                    result = true;
                }
            }

            client.Close();
            return result;
        }

        public string GetPhoneIP(int index)
        {
            string ip = string.Empty;
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(NetUtil.GetStationIp(0)), NetUtil.PORT_LITE_DATA_SERVER);
            using (NetworkStream stream = client.GetStream())
            {
                string request = "TargetIp " + index;
                byte[] requestBuffer = System.Text.Encoding.UTF8.GetBytes(request);
                stream.Write(requestBuffer, 0, requestBuffer.Length);

                byte[] respBuffer = new byte[1024];
                int len = stream.Read(respBuffer, 0, respBuffer.Length);
                if (len > 0)
                {
                    ip = System.Text.Encoding.UTF8.GetString(respBuffer, 0, len);
                }
            }

            client.Close();
            return ip;
        }

        public string SendEquipmentCmd(int index, string action, string paramter)
        {
            string result = string.Empty;
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(NetUtil.GetStationIp(index)), NetUtil.PORT_LITE_DATA_SERVER);
            using (NetworkStream stream = client.GetStream())
            {
                LiteData liteData = new LiteData();
                liteData.Name = "RemoteEquipmentCmd";
                liteData.Paramters = new string[] {action, paramter};
                string request = JsonConvert.SerializeObject(liteData);
                byte[] requestBuffer = System.Text.Encoding.UTF8.GetBytes(request);
                stream.Write(requestBuffer, 0, requestBuffer.Length);

                byte[] respBuffer = new byte[1024];
                int len = stream.Read(respBuffer, 0, respBuffer.Length);
                if (len > 0)
                {
                    result = System.Text.Encoding.UTF8.GetString(respBuffer, 0, len);
                }
            }
            client.Close();
            return result;
        }

        public bool BroadcastPickPlace()
        {
            Log.Debug("SendPickPlace " + 0);
            SendPickPlace(0);

            for (int i = 0; i < EquipmentInfo.STATION_NUM - 1; i++)
            {
                StationInfo stationInfo = AppInfo.EquipmentInfo.GetStationInfo(i);
                if (stationInfo.Work)
                {
                    Log.Debug("SendPickPlace " + (i + 1));
                    SendPickPlace(i + 1);
                }
                else
                {
                    break;
                }
            }

            return true;
        }
    }
}
