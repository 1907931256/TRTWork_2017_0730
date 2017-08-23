using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;
using System.IO;
using IntegrationSys.Net;

namespace IntegrationSys.FileUtil
{
    class FileCmd : IExecutable
    {
        const string ACTION_FILE_COPY = "文件拷贝";
        const string ACTION_FILE_REMOVE = "文件删除";
        const string ACTION_FILE_MOVE = "文件剪切";
        const string ACTION_FILE_TRANSFER = "文件传输";

        private delegate void ExecuteMatchCmd(string param, out string retValue);
        private Dictionary<string, ExecuteMatchCmd> cmdDict_;

        private static FileCmd instance_;
        public static FileCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new FileCmd();
                }
                return instance_;
            }
        }

        private FileCmd()
        {
            cmdDict_ = new Dictionary<string, ExecuteMatchCmd>();
            cmdDict_.Add(ACTION_FILE_COPY, new ExecuteMatchCmd(ExecuteCopy));
            cmdDict_.Add(ACTION_FILE_REMOVE, new ExecuteMatchCmd(ExecuteRemove));
            cmdDict_.Add(ACTION_FILE_MOVE, new ExecuteMatchCmd(ExecuteMove));
            cmdDict_.Add(ACTION_FILE_TRANSFER, new ExecuteMatchCmd(ExecuteTransfer));
        }


        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (cmdDict_.ContainsKey(action))
            {
                cmdDict_[action](param, out retValue);
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }
        }

        private void ExecuteCopy(string param, out string retValue)
        {
            if (!string.IsNullOrEmpty(param))
            {
                string[] paths = param.Split(' ');
                if (paths.Length == 2)
                {
                    File.Copy(paths[0], paths[1], true);
                    retValue = "Res=Pass";
                }
                else
                {
                    retValue = "Res=ArgumentException";
                }
            }
            else
            {
                retValue = "Res=ArgumentException";
            }
        }

        private void ExecuteRemove(string param, out string retValue)
        {
            if (!string.IsNullOrEmpty(param))
            {
                File.Delete(param);
                retValue = "Res=Pass";
            }
            else
            {
                retValue = "Res=ArgumentException";
            }
        }

        private void ExecuteMove(string param, out string retValue)
        {
            if (!string.IsNullOrEmpty(param))
            {
                string[] paths = param.Split(' ');
                if (paths.Length == 2)
                {
                    if (File.Exists(paths[1]))
                    {
                        File.Delete(paths[1]);
                    }
                    File.Move(paths[0], paths[1]);
                    retValue = "Res=Pass";
                }
                else
                {
                    retValue = "Res=ArgumentException";
                }
            }
            else
            {
                retValue = "Res=ArgumentException";
            }
        }

        private void ExecuteTransfer(string param, out string retValue)
        {
            retValue = "Res=ArgumentException";

            if (!string.IsNullOrEmpty(param))
            {
                string[] paths = param.Split(' ');
                if (paths.Length == 2)
                {
                    string[] array = paths[1].Split('@');
                    int index = 0;

                    if (array[0].StartsWith("machine"))
                    {
                        try
                        {
                            index = Int32.Parse(array[0].Substring("machine".Length));

                            FileTransferClient client = new FileTransferClient(NetUtil.GetStationIp(index - 1));
                            if (FileTransferClient.TRANSFER_ERROR_NONE == client.Upload(paths[0], array[1]))
                            {
                                retValue = "Res=Pass";
                            }
                            else
                            {
                                retValue = "Res=Fail";
                            }
                        }
                        catch (Exception)
                        {
                            
                        }
                    }
                }
            }
        }
    }
}
