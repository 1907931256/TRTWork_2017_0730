using System.Collections.Generic;
namespace CmdFile
{
    /*
     * 读取文本问价放入内存中
     * */

    public class INICmds:IGetCommandLine
    {
        private List<EqumentCmds> equmentcms_;
        List<EqumentCmds> cmds;

        public INICmds(string fielPath)
        {
            equmentcms_ = new List<EqumentCmds>();
            cmds = ReadINI(fielPath);
        }


        public void GetEqumentCommand(string cmd, out string cmdHex)
        {
            cmdHex = string.Empty;

            foreach (var item in cmds)
            {
                if (item.Cmd== cmd)
                {
                    cmdHex = item.CmdHex;
                    break;
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
