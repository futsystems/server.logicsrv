using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;


namespace TradingLib.ORM
{

    /// <summary>
    /// 用于操作交易帐户与实盘通道之间的绑定关系
    /// </summary>
    public class MAccountConnectorMap:MBase
    {
        /// <summary>
        /// 获取所有交易帐户与成交通道的绑定关系
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AccountConnectorPair> SelectAccountConnectorPairs()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM accounts_connector_map";
                return db.Connection.Query<AccountConnectorPair>(query);
            }
        }

        /// <summary>
        /// 删除某个交易账户的通道绑定关系
        /// </summary>
        /// <param name="account"></param>
        public static void DelAccountConnectorPair(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM accounts_connector_map WHERE account = '{0}'",account);
                db.Connection.Execute(query);
            }
        }
        /// <summary>
        /// 插入一条交易帐户与成交通道的绑定关系
        /// </summary>
        /// <param name="pair"></param>
        public static void InsertAccountConnectorPair(AccountConnectorPair pair)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into accounts_connector_map (`account`,`connector_id`) VALUES ('{0}','{1}')", pair.Account,pair.Connector_ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新交易账户所绑定的通道
        /// </summary>
        /// <param name="pair"></param>
        public static void UpdateAccountConnectorPair(AccountConnectorPair pair)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE accounts_connector_map SET connector_id='{0}' WHERE account='{1}'",pair.Connector_ID,pair.Account);
                db.Connection.Execute(query);
            }
        }
    }

    
}
