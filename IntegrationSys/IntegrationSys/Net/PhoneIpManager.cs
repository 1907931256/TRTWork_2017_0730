using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IntegrationSys.Net
{
    class IpContainer
    {
        const int MAX_SIZE = 8;
        private List<string> ips_;

        public void Add(string ip)
        {
            if (ips_ == null)
            {
                ips_ = new List<string>();
            }

            if (ips_.Count >= MAX_SIZE)
            {
                ips_.RemoveAt(0);
            }

            ips_.Add(ip);
        }

        public string Get(int index)
        {
            string ip = string.Empty;

            if (ips_ != null && ips_.Count > index)
            {
                ip = ips_[index];
            }

            return ip;
        }

        public int Size()
        {
            int size = 0;

            if (ips_ != null)
            {
                size = ips_.Count;
            }

            return size;
        }
    }

    class PhoneIpManager
    {
        private static PhoneIpManager instance_;

        private PhoneIpManager()
        {
            Init();
        }

        public static PhoneIpManager Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new PhoneIpManager();
                }

                return instance_;
            }
        }

        private IpContainer ips_;

        public void Add(string ip)
        {
            if (ips_.Size() > 0 && ips_.Get(ips_.Size() - 1) == ip) return;
            ips_.Add(ip);
            Save();
        }

        private void Init()
        {
            ips_ = new IpContainer();

            string path = @"IpList.txt";

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        ips_.Add(line);
                    }
                }
            }
            catch (FileNotFoundException)
            { 
            }
        }

        private void Save()
        {
            string path = @"IpList.txt";

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                for (int i = 0; i < ips_.Size(); i++)
                {
                    writer.WriteLine(ips_.Get(i));
                }
            }
        }
    }
}
