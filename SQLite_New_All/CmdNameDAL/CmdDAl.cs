
using CmdNameModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;


namespace CmdNamDAL
{
    /// <summary>
    /// 数据库查询层
    /// </summary>
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
       /// 根据表名 返回一张具体的表
       /// </summary>
       /// <param name="tableName"></param>
       /// <returns></returns>
        public CmdInfo[] CmdByDataTable(string tableName)
        {
            string sql = "select * from " + tableName;
            DataTable dt = SqliteHelper.ExecuteTable(sql);
            CmdInfo[] cmds = new CmdInfo[1024];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmds[i] = RowToCmdInfo(dt.Rows[i]);
                }
                cmds = (from str in cmds where str != null select str).ToArray();
            }

            return cmds;
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
            cmd.FlagDelay = dr["FlagDelay"].ToString();
            return cmd;
        }
    }
}
