using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using PortCmdDAL;

namespace PortCmdDAL
{
    public class CmdDAl
    {
        /// <summary>
        /// 根据发送命令，在数据库中查找对应的命令
        /// 
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public CmdInfo CmdByCmdName_DAL(string cmdName)
        {
            string sql = "select * from MMI where CmdName=@cmdName";

            DataTable dt = SqliteHelper.ExecuteTable(sql, new SQLiteParameter("@cmdName", cmdName));
            CmdInfo cmd = null;
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cmd = RowToCmdInfo(dr);
                }
            }
            return cmd;
        }

        /// <summary>
        /// 把表中的一行数据转换成对象
        /// </summary>
        /// <param name="dr">表中的一行数据</param>
        /// <returns></returns>
        private CmdInfo RowToCmdInfo(DataRow dr)//DataRow表示DataTable中的一行数据
        {
            CmdInfo cmd = new CmdInfo();

            cmd.CmdName = dr["CmdName"].ToString();
            cmd.Start = dr["Start"].ToString();
            cmd.Length = dr["Length"].ToString();
            cmd.Adress = dr["Adress"].ToString();
            cmd.Model = dr["Model"].ToString();
            cmd.Port = dr["Port"].ToString();
            cmd.StrPram = dr["StrPram"].ToString();
            cmd.End = dr["End"].ToString();
            return cmd;
        }
    }
}
