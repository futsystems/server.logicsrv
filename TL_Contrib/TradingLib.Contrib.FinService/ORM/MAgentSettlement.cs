using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;
using TradingLib.Mixins.JsonObject;
using TradingLib.Core;

namespace TradingLib.Contrib.FinService.ORM
{
    public class MAgentSettlement:MBase
    {

        /// <summary>
        /// 判断某个主域是否已经结算过
        /// </summary>
        /// <param name="agentfk"></param>
        /// <returns></returns>
        public static bool IsAgentSettled(int agentfk)
        {
            return IsAgentSettled(agentfk, TLCtxHelper.CmdSettleCentre.NextTradingday);
        }


        /// <summary>
        /// 检查账户是否结算过,搜索结算信息表,如果该日有结算信息,则结算过,没有则没有结算过
        /// </summary>
        /// <param name="account"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static bool IsAgentSettled(int agentfk, int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("select * from manager_settlement  where `mgr_fk` = '{0}' and `settleday` = '{1}'", agentfk, settleday);
                return db.Connection.Query(query).Count() > 0;
            }
        }

        /// <summary>
        /// 对管理域进行结算
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static bool SettleAgent(Manager mgr)
        {
            if (IsAgentSettled(mgr.mgr_fk)) return true;//如果该账户已经结算过，则直接返回
            JsonWrapperAgentSettle settle = new JsonWrapperAgentSettle();

            JsonWrapperToalReport report = ORM.MServiceChargeReport.GenTotalReport(mgr.mgr_fk,TLCtxHelper.CmdSettleCentre.NextTradingday);

            settle.LastEquity = mgr.GetAgentBalance().Balance;
            settle.mgr_fk = mgr.mgr_fk;
            settle.Settleday = TLCtxHelper.CmdSettleCentre.NextTradingday;
            settle.Profit_Commission = report.CommissionProfit;
            settle.Profit_Fee = report.AgentProfit;
            settle.CashIn = mgr.GetDepositNotSettled();//待结算入金
            settle.CashOut = mgr.GetWithdrawNotSettled();//待结算出金
            settle.NowEquity = settle.LastEquity + settle.Profit_Fee + settle.Profit_Commission + settle.CashIn - settle.CashOut;
            
            using (DBMySql db = new DBMySql())
            {
                using (var transaction = db.Connection.BeginTransaction())
                {
                    bool istransok = true;
                    //1.插入结算记录
                    string query = String.Format("Insert into manager_settlement (`mgr_fk`,`settleday`,`profit_fee`,`profit_commission`,`lastequity`,`cashin`,`cashout`,`nowequity`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", settle.mgr_fk,settle.Settleday,settle.Profit_Fee,settle.Profit_Commission,settle.LastEquity,settle.CashIn,settle.CashOut,settle.NowEquity);
                    istransok =  istransok &&  (db.Connection.Execute(query) > 0);

                    //2.更新管理域的Balance信息
                    query = String.Format("UPDATE manager_balance SET balance = '{0}', settleday='{1}' WHERE mgr_fk = '{2}'", settle.NowEquity, settle.Settleday,settle.mgr_fk);
                    istransok = istransok && (db.Connection.Execute(query) >= 0);

                    //如果所有操作均正确,则提交数据库transactoin
                    if (istransok)
                        transaction.Commit();

                    return istransok;
                }
            }

        }
        
    }
}
