using IntegrationSys.Flow;
using System;

namespace IntegrationSys.Result
{
	internal class ResultCmd : IExecutable
	{
		private const string ACTION_SHOW = "显示";

		private static ResultCmd instance_;

		public static ResultCmd Instance
		{
			get
			{
				if (ResultCmd.instance_ == null)
				{
					ResultCmd.instance_ = new ResultCmd();
				}
				return ResultCmd.instance_;
			}
		}

		private ResultCmd()
		{
		}

		public void ExecuteCmd(string action, string param, out string retValue)
		{
			if ("显示" == action)
			{
				this.ExecuteShow(param, out retValue);
				return;
			}
			retValue = "Res=CmdNotSupport";
		}

		private void ExecuteShow(string param, out string retValue)
		{
			retValue = ResultRecord.GetResult(param);
		}
	}
}
