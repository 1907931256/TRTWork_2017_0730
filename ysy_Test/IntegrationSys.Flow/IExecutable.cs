using System;

namespace IntegrationSys.Flow
{
	internal interface IExecutable
	{
		void ExecuteCmd(string action, string param, out string retValue);
	}
}
