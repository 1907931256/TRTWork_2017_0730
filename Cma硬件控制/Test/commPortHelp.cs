using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonPortCmd;
namespace TRTCamera
{
    public class commPortHelp
    {
        private static commPortHelp comPort;
        private Common common_=new Common();
        private static string cmd;
        public delegate void ChuanZhi(object send, MyEventArgs e);
        public event ChuanZhi ChuanZhiEvent;
      

     
        //private commPortHelp()
        //{

        //}
        public static commPortHelp CreatCommPort()
        {
            if (comPort != null)
            {
                return comPort;
            }
            else
            {
                return comPort = new commPortHelp();
            }
        }


        public  bool ConnectPort(out string protName)
        {
           
            bool b = common_.ConnectPort(out protName);
            return b;
        }
        public  void SendCmd(string strHex,int flag, out string rec)
        {
            rec = "";
            common_.SendCommand(strHex,out rec);
            
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
            common_.SendCommand(strCmd, out rec);

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