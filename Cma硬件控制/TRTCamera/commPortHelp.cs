using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonPortCmd;

namespace TRTCamera
{
     class commPortHelp
    {
        private static commPortHelp comPort;
        private  Common common_;
        private static string cmd;


        public delegate void ChuanZhi(object send, MyEventArgs e);
        public event ChuanZhi ChuanZhiEvent;

        //public delegate void ActiveData(object send, EventArgs e);
        //public event ActiveData ActiveEvent;

        private commPortHelp()
        {
            common_ = Common.CreatCommon();
            common_.RecDataSendEventHander += common__RecDataSendEventHander;
            
        }

        void common__RecDataSendEventHander(object send, ActiveReporting e)
        {
            //activeData = e.ActiveData;
        }

        public static commPortHelp CreatCommPort()
        {
            if (comPort == null)
            {
                return comPort = new commPortHelp();
              
            }
            else
            {
                return comPort;
            }
        }


        public  bool ConnectPort(out string protName)
        {
            bool b = common_.ConnectPort(out protName);
            return b;
        }
       
        public  void SendCmd(string strCmd, out string rec)
        {
            cmd = strCmd;
            rec = "";
            common_.SendCommand(strCmd, out rec);

            if (ChuanZhiEvent != null)
            {
                ChuanZhiEvent.Invoke(this, new MyEventArgs(cmd, rec));
            }
        }

        public  void SendCmd(string strCmd,string pram, out string rec)
        {
            cmd = strCmd;
            rec = "";
            common_.SendCommand(strCmd,pram, out rec);
            if (ChuanZhiEvent != null)
            {
                ChuanZhiEvent.Invoke(this, new MyEventArgs(cmd, rec));
            }

        }

    }
    public class MyEventArgs : EventArgs
    {
        private string cmd;

        public string Cmd
        {
            get { return cmd; }
            set { cmd = value; }
        }
        private string res;

        /// <summary>
        /// 发送命令后返回的数据
        /// </summary>
        public string Res
        {
            get { return res; }
            set { res = value; }
        }
        public MyEventArgs(string str_cmd, string str_res)
        {
            cmd = str_cmd;
            res = str_res;
        }
    }
}