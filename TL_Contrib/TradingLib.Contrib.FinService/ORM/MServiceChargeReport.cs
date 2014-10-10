using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Contrib.FinService.ORM
{

    public class MServiceChargeReport:MBase
    {
        /// <summary>
        /// 查询某天的代理统计
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperToalReport> GenTotalReport(int settleday)
        { 
            
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("select agent_fk,settleday,sum(totalfee) as totalfee,sum(agentfee) as agentfee ,sum(agentprofit) as agentprofit FROM log_service_feecharge  WHERE settleday='{0}' GROUP BY agent_fk",settleday);
                return db.Connection.Query<JsonWrapperToalReport>(query, null).ToArray();
            }
        }

        /// <summary>
        /// 查询某个代理一段时间内的利润流水
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperToalReport> GenTotalReportByDayRange(int agentfk, int start, int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("select agent_fk,settleday,sum(totalfee) as totalfee,sum(agentfee) as agentfee ,sum(agentprofit) as agentprofit FROM log_service_feecharge  WHERE agent_fk='{0}' AND settleday>='{1}' AND settleday<='{2}' GROUP BY settleday", agentfk,start,end);
                return db.Connection.Query<JsonWrapperToalReport>(query, null).ToArray();
            }
        }

        //select agent_fk,settleday,account,sum(totalfee) as totalfee,sum(agentfee) as agentfee ,sum(agentprofit) as agentprofit FROM log_service_feecharge  WHERE settleday =20141010 GROUP BY account

        /// <summary>
        /// 查询某个代理某个交易日
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperToalReport> GenDetailReportByAccount(int agentfk, int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("select agent_fk,settleday,account,sum(totalfee) as totalfee,sum(agentfee) as agentfee ,sum(agentprofit) as agentprofit FROM log_service_feecharge  WHERE settleday ='{0}' AND agent_fk='{1}' GROUP BY account",settleday,agentfk);
                return db.Connection.Query<JsonWrapperToalReport>(query, null).ToArray();
            }
        }
    }
}
