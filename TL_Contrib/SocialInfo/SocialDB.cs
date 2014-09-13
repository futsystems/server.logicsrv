using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MySql;
using MySql.Data.MySqlClient;

namespace SocialLink
{

    public class SocialDB:mysqlDBBase
    {

        /// <summary>
        /// 通过email来获得user对应的followers数目
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public int GetFollowers(string email)
        {
            this.SqlReady();
            string sql = String.Format("select count(*) as num_followers from ts_user_follow where fid in (SELECT uid from ts_user where email='{0}')", email);
            cmd.CommandText = sql;
            MySqlDataReader myReader = cmd.ExecuteReader();
            try
            {
                myReader.Read();
                int num = myReader.GetInt32("num_followers");
                return num;

            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                myReader.Close();

            }
        }
    }
}
