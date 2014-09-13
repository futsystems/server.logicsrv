using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TradingLib.Mixins.DataBase
{
    public class NewId
    {
        public int Id { get; set; }
    }

    public class MBase
    {
        /// <summary>
        /// 获得最新插入的数据ID
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="setId"></param>
        /// <param name="primarykey"></param>
        /// <param name="tableName"></param>
        public static void SetIdentity(IDbConnection conn, Action<int> setId, string primarykey, string tableName)
        {
            if (string.IsNullOrEmpty(primarykey)) primarykey = "id";
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("tableName参数不能为空，为查询的表名");
            }
            string query = string.Format("SELECT max({0}) as Id FROM {1}", primarykey, tableName);
            NewId identity = conn.Query<NewId>(query, null).Single<NewId>();
            setId(identity.Id);
        }
    }
}
