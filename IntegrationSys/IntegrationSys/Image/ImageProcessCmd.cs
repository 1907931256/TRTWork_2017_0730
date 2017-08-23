using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;
using System.Threading;
using System.IO;
using IntegrationSys.LogUtil;
using IntegrationSys.Net;
using IntegrationSys.Equipment;

namespace IntegrationSys.Image
{
    class ImageProcessCmd : IExecutable, IDisposable
    {
        const string ACTION_RESULT = "图像处理结果";

        private Timer timer_;
        private FileSystemWatcher watcher_;


        private static ImageProcessCmd instance_;

        public static ImageProcessCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new ImageProcessCmd();
                }

                return instance_;
            }
        }

        private ImageProcessCmd() 
        {
            try
            {
                watcher_ = new FileSystemWatcher(@"C:\TRT_Camera_Tester_Picture\test result", "result.txt");
                watcher_.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                watcher_.Changed += new FileSystemEventHandler(OnChanged);
                watcher_.EnableRaisingEvents = true;
            }
            catch (Exception)
            {
                Log.Debug(@"C:\TRT_Camera_Tester_Picture\test result\" + " not exist");
            }

            timer_ = new Timer(new TimerCallback(OnWatchedFileChange), null, Timeout.Infinite, Timeout.Infinite);
        }

        ~ImageProcessCmd()
        {
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer_.Dispose();
                watcher_.Dispose();
            }
        }

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (ACTION_RESULT == action)
            {
                ProcessResult(param, out retValue);
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }
            
        }

        private void ProcessResult(string param, out string retValue)
        {
            retValue = string.Empty;
            for (int i = 0; i < EquipmentInfo.STATION_NUM; i++)
            {
                string filename = @"d:\DataFragment\result_" + i + ".txt";

                try
                {
                    using (StreamReader reader = new StreamReader(filename))
                    {
                        string line1 = null;
                        string line2 = null;

                        while ((line1 = reader.ReadLine()) != null)
                        {
                            line2 = reader.ReadLine();

                            if (line1.IndexOf(param) != -1)
                            {
                                string[] pairArray = line2.Split(';');

                                foreach (string pair in pairArray)
                                {
                                    if (!string.IsNullOrEmpty(pair))
                                    {
                                        string[] keyvalue = pair.Split(',');
                                        retValue += keyvalue[0] + "=" + keyvalue[1] + ";";
                                    }
                                }
                            }
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    Log.Debug(e.Message, e);
                }
                catch (Exception e)
                {
                    Log.Debug(e.Message, e);
                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Log.Debug("File: " + e.FullPath + " " + e.ChangeType);

            timer_.Change(500, Timeout.Infinite);
        }

        private void OnWatchedFileChange(object state)
        {
            Console.WriteLine("OnWatchedFileChange");

            string srcfilename = @"C:\TRT_Camera_Tester_Picture\test result\result.txt";
            string destfilename = @"d:\DataFragment\result_" + NetUtil.GetStationIndex() + ".txt";
            FileTransferClient client = new FileTransferClient(NetUtil.GetStationIp(AppInfo.STATION_SERVER));
            client.Upload(srcfilename, destfilename);
        }
    }
}
