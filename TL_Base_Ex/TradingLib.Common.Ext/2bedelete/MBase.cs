//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MySql.Data.MySqlClient;
//using System.Data;

//namespace TradingLib.MySql
//{
//    public class NewId
//    {
//        public int Id { get; set; }
//    }

//    public class MBase
//    {
//        public static void SetIdentity(IDbConnection conn, Action<int> setId, string primarykey, string tableName)
//        {
//            if (string.IsNullOrEmpty(primarykey)) primarykey = "id";
//            if (string.IsNullOrEmpty(tableName))
//            {
//                throw new ArgumentException("tableName参数不能为空，为查询的表名");
//            }
//            string query = string.Format("SELECT max({0}) as Id FROM {1}", primarykey, tableName);
//            NewId identity = conn.Query<NewId>(query, null).Single<NewId>();
//            setId(identity.Id);
//        }
//    }
//}
