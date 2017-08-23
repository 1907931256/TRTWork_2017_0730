using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;
using IntegrationSys.LibWrap;

namespace IntegrationSys.CommandUtils
{
    class RStechCmd : IExecutable
    {
        private static RStechCmd instance_;

        private string path_ = string.Empty;

        public static RStechCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RStechCmd();
                }
                return instance_;
            }
        }

        private RStechCmd() { }

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (action == "SetAppPath")
            {
                SetPath(param);
                retValue = "Res=Pass";
            }
            else if (action == "TSEnd")
            {
                int result = TrustSystem.TSEnd(path_);
                retValue = "Res=" + result;
            }
            else if (action == "Demo")
            {
                int result = TrustSystem.Demo(path_);
                retValue = "Res=" + result;                
            }
            else if (action == "FLogin")
            {
                int result = TrustSystem.FLogin(path_, param);
                retValue = "Res=" + result; 
            }
            else if (action == "GetDataLen")
            {
                int result = TrustSystem.GetDataLen(path_, param);
                retValue = "Res=" + result; 
            }
            else if (action == "Goto")
            {
                int result = TrustSystem.Goto(path_, param);
                retValue = "Res=" + result;
            }
            else if (action == "LoadPro")
            {
                TrustSystem.LoadPro(path_, param);
                retValue = "Res=Pass";
            }
            else if (action == "LoadProOver")
            {
                int result = TrustSystem.LoadProOver(path_);
                retValue = "Res=" + result;
            }
            else if (action == "Result")
            {
                int result = TrustSystem.Result(path_);
                retValue = "Res=" + result;
            }
            else if (action == "SN_Number")
            {
                TrustSystem.SN_Number(path_, param);
                retValue = "Res=Pass";
            }
            else if (action == "TSCommand")
            {
                TrustSystem.TSCommand(path_, param);
                retValue = "Res=Pass";
            }
            else if (action == "ShowTheWindow")
            {
                TrustSystem.ShowTheWindow(path_, param);
                retValue = "Res=Pass";
            }
            else if (action == "Test")
            {
                retValue = "Res=-1.09";
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }
        }

        private void SetPath(string path)
        {
            path_ = path;
        }
    }
}
