using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdFile
{
    public interface IGetCommandLine
    {
        /// <summary>
        /// 输入中文命令，返回对应的字符串命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="cmdHex"></param>
       void GetEqumentCommand(string cmd, out string cmdHex);
    }
}
