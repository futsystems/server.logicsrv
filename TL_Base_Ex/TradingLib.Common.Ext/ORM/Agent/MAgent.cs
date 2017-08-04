using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MAgent : MBase
    {
        /// <summary>
        /// 加载所有代理财务账户
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AgentImpl> SelectAgent()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM agents";
                return db.Connection.Query<AgentImpl>(query);
            }
        }


        /// <summary>
        /// 添加代理商财务账户
        /// </summary>
        /// <param name="agent"></param>
        public static void AddAgent(AgentSetting agent)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO agents (`account`,`lastequity`,`lastcredit`,`currency`,`margin_id`,`commission_id`,`exstrategy_id`,`createdtime`,`settledtime`,`agenttype`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", agent.Account, agent.LastEquity, agent.LastCredit, agent.Currency, agent.Margin_ID, agent.Commission_ID, agent.ExStrategy_ID, Util.ToTLDateTime(), 0,agent.AgentType);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => agent.ID = id, "id", "agents");
            }
        }

        /// <summary>
        /// 更新代理模板参数
        /// </summary>
        /// <param name="agent"></param>
        public static void UpdateAgentFlatEquity(AgentImpl agent)
        {
            using (DBMySql db = new DBMySql())
            {

                string query = String.Format("UPDATE agents SET  flatequity = '{0}'  WHERE id = '{1}'", agent.FlatEquity,agent.ID);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 更新代理模板参数
        /// </summary>
        /// <param name="agent"></param>
        public static void UpdateAgentTemplate(AgentImpl agent)
        {
            using (DBMySql db = new DBMySql())
            {

                string query = String.Format("UPDATE agents SET  margin_id = '{0}' , commission_id = '{1}' , exstrategy_id = '{2}' WHERE id = '{3}'", agent.Margin_ID,agent.Commission_ID,agent.ExStrategy_ID,agent.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新代理默认配置模板
        /// </summary>
        /// <param name="agent"></param>
        public static void UpdateAgentDefaultConfigTemplate(AgentImpl agent)
        {
            using (DBMySql db = new DBMySql())
            {

                string query = String.Format("UPDATE agents SET  default_config_id = '{0}'  WHERE id = '{1}'", agent.Default_Config_ID, agent.ID);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public static void UpdateAgentCommissionTemplate(string account, int templateid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE agents SET commission_id = {0} WHERE account = '{1}'", templateid, account);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新保证金模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public static void UpdateAgentMarginTemplate(string account, int templateid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE agents SET margin_id = {0} WHERE account = '{1}'", templateid, account);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新交易参数模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public static void UpdateAccountExStrategyTemplate(string account, int templateid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE agents SET exstrategy_id = {0} WHERE account = '{1}'", templateid, account);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 获得某个交易日结束 权益统计数据
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<EquityReport> SelectEquityReport(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT account,equitysettled as equity,creditsettled as credit FROM log_agent_settlement WHERE settleday = '{0}'", settleday);
                return db.Connection.Query<EquityReport>(query);//包含多个元素则异常
            }
        }

    }
}
