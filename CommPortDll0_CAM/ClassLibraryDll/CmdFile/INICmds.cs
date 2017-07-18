using System.Collections.Generic;

namespace CmdFile
{
    /*
     * 读取文本问价放入内存中
     * */

    public class INICmds:IGetCommandLine
    {
        private string fielPath;
        private List<EqumentCmds> equmentcms_;


        public INICmds()
        {
            equmentcms_ = new List<EqumentCmds>();
        }

        public string FielPath { get => fielPath; set => fielPath = value; }

        public void GetEqumentCommand(string cmd, out string cmdHex)
        {
            cmdHex = string.Empty;
            List<EqumentCmds> cmds = ReadINI(fielPath);

            foreach (var item in cmds)
            {
                if (item.Cmd== cmd)
                {
                    cmdHex = item.CmdHex;
                }
            }

        }

        private List<EqumentCmds> ReadINI(string file)
        {
            string[] cmds=INIHelp.INIOperationClass.INIGetAllItems(file, "cmd");


            for (int i = 0; i < cmds.Length; i++)
            {
                EqumentCmds equmentcmd_ = new EqumentCmds();
                equmentcmd_ .Cmd= cmds[i].Substring(0,cmds[i].IndexOf("="));
                equmentcmd_.CmdHex= cmds[i].Substring(cmds[i].IndexOf("=") + 1);
                equmentcms_.Add(equmentcmd_);
            }
            return equmentcms_;
        }
    }
}
