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

    }
}
