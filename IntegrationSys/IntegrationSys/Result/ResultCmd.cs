using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntegrationSys.Flow;

namespace IntegrationSys.Result
{
    class ResultCmd : IExecutable
    {
        const string ACTION_SHOW = "显示";

        private static ResultCmd instance_;

        private ResultCmd() { }
        public static ResultCmd Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new ResultCmd();
                }

                return instance_;
            }
        }

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if (ACTION_SHOW == action)
            {
                ExecuteShow(param, out retValue);
            }
            else
            {
                retValue = "Res=CmdNotSupport";
            }
        }

        private void ExecuteShow(string param, out string retValue)
        {
            retValue = ResultRecord.GetResult(param);
        }
    }
}
