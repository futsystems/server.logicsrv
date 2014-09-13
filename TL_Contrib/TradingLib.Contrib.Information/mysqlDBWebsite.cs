using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Data;
using TradingLib.API;

namespace TradingLib.MySql
{
    /// <summary>
    /// web站点数据库连接用于获得账户所对应的网站注册用户的相关信息
    /// </summary>
    public class mysqlDBWebsite : mysqlDBBase
    {

        public DataSet getProfile(int userid)
        {
            this.SqlReady();
            string sql = String.Format("select * from accounts_myprofile,auth_user where accounts_myprofile.user_id=auth_user.id and auth_user.id='{0}'", userid.ToString());
            // 创建一个适配器
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            // 创建DataSet，用于存储数据.
            DataSet retSet = new DataSet();
            // 执行查询，并将数据导入DataSet.
            adapter.Fill(retSet, "profile");
            return retSet;

        }


    }
}
