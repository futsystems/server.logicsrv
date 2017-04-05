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
        public static void InsertSignalConfig(IAccount account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_signals (`signaltype`,`signaltoken`,`domain_id`) values('{0}','{1}','{2}')", QSEnumSignalType.Account, account.ID, account.Domain.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除信号
        /// </summary>
        /// <param name="cfg"></param>
        public static void DelSignalConfigWithOutAccount(SignalConfig cfg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("DELETE FROM follow_signals WHERE id = '{0}'", cfg.ID);//删除信号配置
                db.Connection.Execute(query);

                query = String.Format("DELETE FROM follow_strategy_signals WHERE signalid = '{0}'", cfg.ID);//删除添加了该信号的策略信号映射
                db.Connection.Execute(query);
            }
        }



        public static void InsertSignalConfig(SignalConfig cfg)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_signals (`signaltype`,`signaltoken`,`domain_id`) values('{0}','{1}','{2}')", cfg.SignalType, cfg.SignalToken, cfg.Domain_ID);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => cfg.ID = id, "id", "follow_signals");
            }
        }

        /// <summary>
        /// 删除某个信号配置
        /// </summary>
        /// <param name="cfg"></param>
        public static void DelSignalConfig(SignalConfig cfg)
        {
            using (DBMySql db = new DBMySql())
            {
                string delquery = string.Format("DELETE FROM follow_signals WHERE id = '{0}'", cfg.ID);
                db.Connection.Execute(delquery);
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
        public static void AppendSignalToStrategy(int signalID, int strategyID)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_strategy_signals (`signalid`,`strategyid`) values('{0}','{1}')", signalID, strategyID);
                db.Connection.Execute(query);
            }
        }
    }
}
