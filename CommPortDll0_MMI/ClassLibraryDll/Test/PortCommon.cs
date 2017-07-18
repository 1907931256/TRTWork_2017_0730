using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommonPortCmd
{
    public interface PortCommon
    {
       

        bool ConnectPort();
         void ReadThreadProc();

         void CloseProt();

       void WriteHexData(string strHex);


        bool SendCmd(string strCmd, out string str);
        bool SendCmd(string strCmd, string strPram,out string str);

    }

   
}
