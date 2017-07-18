using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonPortCmd.Net
{
    public interface ISendCmd
    {
        string SendCmd(int index, string action, string paramter);
        

    }
}
