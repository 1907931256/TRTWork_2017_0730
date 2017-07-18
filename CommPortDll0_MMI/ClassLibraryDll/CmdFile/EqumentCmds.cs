namespace CmdFile
{
    public class EqumentCmds
    {
        /// <summary>
        /// 中文命令
        /// </summary>
        private string cmd;

        public string Cmd
        {
            get { return cmd; }
            set { cmd = value; }
        }
        
        /// <summary>
        /// 对应的下位机命令
        /// </summary>
        private string cmdHex;

        public string CmdHex
        {
            get { return cmdHex; }
            set { cmdHex = value; }
        }


      
    }
}
