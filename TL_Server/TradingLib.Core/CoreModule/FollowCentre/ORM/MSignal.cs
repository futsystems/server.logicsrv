using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;


namespace TradingLib.ORM
{
    public class MSignal : MBase
    {
        /// <summary>
        /// 从数据库加载所有信号设置
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SignalConfig> SelectSignalConfigs()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM follow_signals";
                IEnumerable<SignalConfig> result = db.Connection.Query<SignalConfig>(query, null);
                return result;
            }
        }

        /// <summary>
        /// 插入信号列表
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        public static void InsertSignalConfig(string account, QSEnumSignalType type = QSEnumSignalType.Account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_signals (`signaltype`,`signaltoken`) values('{0}','{1}')", type, account);
                db.Connection.Execute(query);
            }
        }
        /// <summary>
        /// 从数据库加载所有策略信号项
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<StrategySignalItem> SelectStrategySignalItems()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM follow_strategy_signals";
                IEnumerable<StrategySignalItem> result = db.Connection.Query<StrategySignalItem>(query, null);
                return result;
            }
        }

        /// <summary>
        /// 将信号从跟单策略中删除
        /// </summary>
        /// <param name="signalID"></param>
        /// <param name="strategyID"></param>
        public static void RemoveSignalFromStrategy(int signalID, int strategyID)
        {
            using (DBMySql db = new DBMySql())
            {
                string delquery = string.Format("DELETE FROM follow_strategy_signals WHERE signalid = '{0}' AND strategyid = '{1}'", signalID,strategyID);//删除帐户列表
                db.Connection.Execute(delquery);
            }
        }

        /// <summary>
        /// 将信号添加到跟单策略中
        /// </summary>
        /// <param name="signalID"></param>
        /// <param name="strategyID"></param>
        public static void AppendSignalFromStrategy(int signalID, int strategyID)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_strategy_signals (`signalid`,`strategyid`) values('{0}','{1}')", signalID, strategyID);
                db.Connection.Execute(query);
            }
        }
    }
}
