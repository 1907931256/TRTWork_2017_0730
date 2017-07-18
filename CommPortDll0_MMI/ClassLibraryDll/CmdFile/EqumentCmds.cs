namespace CmdFile
{
    public class EqumentCmds
    {
        /// <summary>
        /// 中文命令
        /// </summary>
        private string cmd;
        
        /// <summary>
        /// 对应的下位机命令
        /// </summary>
        private string cmdHex;

        public string Cmd { get => cmd; set => cmd = value; }
        public string CmdHex { get => cmdHex; set => cmdHex = value; }

      
    }
}
