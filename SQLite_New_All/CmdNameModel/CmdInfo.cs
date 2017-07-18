using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdNameModel
{
    /// <summary>
    /// 串口命令信息类
    /// </summary>
    public class CmdInfo
    {

        private string cmdName;

        public string CmdName
        {
            get { return cmdName; }
            set { cmdName = value; }
        }

        private string start;

        public string Start
        {
            get { return start; }
            set { start = value; }
        }


        private string length;

        public string Length
        {
            get { return length; }
            set { length = value; }
        }

        private string adress;

        public string Adress
        {
            get { return adress; }
            set { adress = value; }
        }

        private string model;

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        private string port;

        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        private string strPram;

        public string StrPram
        {
            get { return strPram; }
            set { strPram = value; }
        }


        private string end;

        public string End
        {
            get { return end; }
            set { end = value; }
        }

        private string flagDelay;

        public string FlagDelay
        {
            get { return flagDelay; }
            set { flagDelay = value; }
        }

    }
}
