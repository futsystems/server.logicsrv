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
                string query = String.Format("INSERT INTO log_agent_commission_split (`account`,`settleday`,`traderid`,`commissionincome`,`commissioncost`,`settled`) values('{0}','{1}','{2}','{3}','{4}','{5}')", split.Account, split.Settleday, split.TradeID, split.CommissionIncome, split.CommissionCost, split.Settled ? 1 : 0);
                db.Connection.Execute(query);
            }
        }

    }
}
