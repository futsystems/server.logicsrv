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
        internal class DecimalSum
        {
            public decimal Total { get; set; }
        }
        /// <summary>
        /// 查询某天的代理统计
        /// select * from (select sum(commission) as total,agent_fk  from log_service_commission GROUP BY agent_fk)a INNER JOIN (SELECT sum(agentfee) as fee ,agent_fk from log_service_feecharge GROUP BY agent_fk)b on a.agent_fk=b.agent_fk
        /// 查询某个代理 在某个结算日的原始记录统计
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static JsonWrapperToalReport GenTotalReport(int agentfk,int settleday)
        { 
            if(agentfk ==0)
            {
                throw new Exception("必须指定代理主域");
            }
            using (DBMySql db = new DBMySql())
            {
                string query = string.Empty;
                query = string.Format("select agent_fk,settleday,sum(totalfee) as totalfee,sum(agentfee) as agentfee ,sum(agentprofit) as agentprofit FROM log_service_feecharge  WHERE settleday='{0}' AND agent_fk ='{1}'", settleday,agentfk);

                //1.生成直客记录
                JsonWrapperToalReport rep = db.Connection.Query<JsonWrapperToalReport>(query).SingleOrDefault();
                //如果rep为空 返回空的统计记录
                if (rep == null)
                {
                    rep = new JsonWrapperToalReport();
                    rep.SettleDay = settleday;
                    rep.Agent_FK = agentfk;
                }
                else
                {
                    if (rep.Agent_FK == 0)//如果agentfk对应没有收费记录 搜索语句会返回agentfk为0的空记录 这要进行agentfk赋值
                    {
                        rep.Agent_FK = agentfk;
                    }
                }

                //2.查询代理的代理提成收入
                rep.CommissionProfit = GetAgentCommissionProfit(agentfk, settleday);

                //3.将Manager基本信息填充进去
                FillReportMangerInfo(rep);

                return rep;
            }
        }

        static JsonWrapperToalReport FillReportMangerInfo(JsonWrapperToalReport rep)
        {
            Manager m = BasicTracker.ManagerTracker[rep.Agent_FK];
            if (m != null)
            {
                rep.AgentName = m.Name;
                rep.Mobile = m.Mobile;
                rep.QQ = m.QQ;
            }
            return rep;
        }

        /// <summary>
        /// 查询某个代理某个结算日的代理佣金收入
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        static decimal GetAgentCommissionProfit(int agentfk, int settleday)
        {
            if (agentfk == 0)
            {
                throw new Exception("必须指定代理主域");
            }

            using (DBMySql db = new DBMySql())
            {
                string query = string.Empty;
                query = string.Format("select sum(commission) as total FROM log_service_commission  WHERE settleday='{0}' AND agent_fk='{1}'", settleday, agentfk);
                DecimalSum sum = db.Connection.Query<DecimalSum>(query, null).SingleOrDefault();
                if (sum == null)
                {
                    return 0;
                }
                else
                {
                    return sum.Total;
                }
            }
        }

        

        /// <summary>
        /// 获得某个代理 在某个时间段内的流水汇总
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static JsonWrapperToalReport GenSummaryReportByDayRange(int agentfk, int start, int end)
        {
            if (agentfk == 0)
            {
                throw new Exception("必须指定代理主域");
            }

            using (DBMySql db = new DBMySql())
            {   
                string query = string.Empty;
                query = string.Format("select agent_fk,sum(totalfee) as totalfee,sum(agentfee) as agentfee ,sum(agentprofit) as agentprofit FROM log_service_feecharge  WHERE settleday>='{0}' AND settleday<='{1}' AND agent_fk='{2}'", start, end,agentfk);
                //1.生成直客记录
                JsonWrapperToalReport rep = db.Connection.Query<JsonWrapperToalReport>(query).SingleOrDefault();
                //如果rep为空 返回空的统计记录
                if (rep == null)
                {
                    rep = new JsonWrapperToalReport();
                    rep.SettleDay = 0;
                    rep.Agent_FK = agentfk;
                }
                else
                {
                    if (rep.Agent_FK == 0)//如果agentfk对应没有收费记录 搜索语句会返回agentfk为0的空记录 这要进行agentfk赋值
                    {
                        rep.Agent_FK = agentfk;
                    }
                }

                //2.查询代理的代理提成收入
                rep.CommissionProfit = GetSummaryAgentCommissionProfit(agentfk, start,end);

                //3.将Manager基本信息填充进去
                FillReportMangerInfo(rep);

                return rep;
            }
        }


        /// <summary>
        /// 查询某个代理 在某个时间区间内所有佣金收入
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="settleday"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        static decimal GetSummaryAgentCommissionProfit(int agentfk, int start, int end)
        {
            if (agentfk == 0)
            {
                throw new Exception("必须指定代理主域");
            }

            using (DBMySql db = new DBMySql())
            {
                string query = string.Empty;
                query = string.Format("select sum(commission) as total FROM log_service_commission  WHERE settleday>='{0}' AND settleday<='{1}' AND agent_fk='{2}'", start, end, agentfk);
                DecimalSum sum = db.Connection.Query<DecimalSum>(query, null).SingleOrDefault();
                if (sum == null)
                {
                    return 0;
                }
                else
                {
                    return sum.Total;
                }
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
                JsonWrapperToalReport[] reports =  db.Connection.Query<JsonWrapperToalReport>(query, null).ToArray();
                foreach (JsonWrapperToalReport rep in reports)
                {
                    if (rep.Agent_FK == 0)//如果agentfk对应没有收费记录 搜索语句会返回agentfk为0的空记录 这要进行agentfk赋值
                    {
                        rep.Agent_FK = agentfk;
                    }

                    //2.查询代理的代理提成收入
                    rep.CommissionProfit = GetAgentCommissionProfit(agentfk,rep.SettleDay);

                    //3.将Manager基本信息填充进去
                    FillReportMangerInfo(rep);
                }
                return reports;
            }
        }

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
