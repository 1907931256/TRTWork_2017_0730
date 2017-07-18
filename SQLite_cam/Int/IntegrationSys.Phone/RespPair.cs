using System;
using System.Threading;

namespace IntegrationSys.Phone
{
	internal class RespPair
	{
		public ManualResetEvent RespEvent
		{
			get;
			set;
		}

		public string Resp
		{
			get;
			set;
		}
	}
}
