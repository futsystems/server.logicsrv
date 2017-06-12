using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MAgentCommissionSplit : MBase
    {

        /// <summary>
        /// 添加手续费分拆记录
        /// </summary>
        /// <param name="agent"></param>
        public static void AddAgentCommissionSplit(AgentCommissionSplit split)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO log_agent_commission_split (`account`,`settleday`,`traderid`,`commissionincome`,`commissioncost`,`settled`,`currency`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", split.Account, split.Settleday, split.TradeID, split.CommissionIncome, split.CommissionCost, split.Settled ? 1 : 0,split.Currency);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 获得所有未结算手续费记录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AgentCommissionSplit> SelectAgentCommissionSplitUnSettled(int tradingday)
        {

            using (DBMySql db = new DBMySql())
            {
                if (tradingday == 0)
                {
                    string query = string.Format("SELECT * FROM log_agent_commission_split WHERE settled=0");
                    return db.Connection.Query<AgentCommissionSplitImpl>(query);
                }
                else
                {
                    string query = string.Format("SELECT * FROM log_agent_commission_split WHERE settled=0 AND settleday='{0}'", tradingday);
                    return db.Connection.Query<AgentCommissionSplitImpl>(query);
                }
            }
        }

        /// <summary>
        /// 标注手续费拆分
        /// </summary>
        /// <param name="txn"></param>
        public static void MarkeAgentCommissionSplitSettled(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE log_agent_commission_split SET settled=1 WHERE settleday = '{0}'", settleday);
                db.Connection.Execute(query);
            }
        }

    }
}
