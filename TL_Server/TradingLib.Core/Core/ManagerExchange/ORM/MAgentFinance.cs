using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.ORM
{
    public class MAgentFinance : MBase
    {
        /// <summary>
        /// 获得某个代理的最新Balance信息
        /// </summary>
        /// <param name="agentfk"></param>
        /// <returns></returns>
        public static JsonWrapperAgentBalance GetAgentBalance(int agentfk)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM manager_balance  WHERE mgr_fk = '{0}'", agentfk);
                JsonWrapperAgentBalance balance = db.Connection.Query<JsonWrapperAgentBalance>(query).SingleOrDefault<JsonWrapperAgentBalance>();
                
                //该代理没有对应的Balance信息
                if (balance == null)
                {
                    //形成初始化权益信息
                    JsonWrapperAgentBalance b = new JsonWrapperAgentBalance 
                    {
                        mgr_fk = agentfk,
                        Balance=0,
                        Settleday = TLCtxHelper.Ctx.SettleCentre.LastSettleday,
                    };
                    InsertAgentBalance(b);
                    return b;
                }
                else
                {
                    return balance;
                }

            }
        }

        /// <summary>
        /// 插入Balance数据
        /// </summary>
        /// <param name="balance"></param>
        /// <returns></returns>
        public static bool InsertAgentBalance(JsonWrapperAgentBalance balance)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into manager_balance (`mgr_fk`,`balance`,`settleday`) values('{0}','{1}','{2}')",balance.mgr_fk,balance.Balance,balance.Settleday);//orderpostflag`,`forceclose`,`hedgeflag`,`orderref`,`orderexchid`,`orderseq`
                return db.Connection.Execute(query) >0;
            }
        }


        /// <summary>
        /// 获得某个代理 某个天的结算信息
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static JsonWrapperAgentSettle GetAgentSettle(int agentfk, int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM manager_settlement  WHERE mgr_fk = '{0}' AND settleday='{1}'", agentfk, settleday);
                JsonWrapperAgentSettle settle = db.Connection.Query<JsonWrapperAgentSettle>(query).SingleOrDefault<JsonWrapperAgentSettle>();
                return settle;
            }
        }


        /// <summary>
        /// 获得某个代理的银行帐户信息
        /// </summary>
        /// <param name="agentfk"></param>
        /// <returns></returns>
        public static JsonWrapperBankAccount GetAgentBankAccount(int agentfk)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM manager_bankac  WHERE mgr_fk = '{0}'", agentfk);
                JsonWrapperBankAccount bank = db.Connection.Query<JsonWrapperBankAccount>(query).SingleOrDefault<JsonWrapperBankAccount>();
                return bank;
            }
        }

        public static bool HaveAgentBankAccount(int agentfk)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT count(*) as  num FROM manager_bankac WHERE mgr_fk='{0}'", agentfk);
                TotalNum totalnum = db.Connection.Query<TotalNum>(query).SingleOrDefault();
                if (totalnum != null && totalnum.num > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public static bool UpdateAgentBankAccount(JsonWrapperBankAccount ac)
        {
            using (DBMySql db = new DBMySql())
            {
                if (HaveAgentBankAccount(ac.mgr_fk))//更新
                {
                    string query = String.Format("UPDATE manager_bankac SET bank_id = '{0}' ,name = '{1}',bank_ac = '{2}',branch = '{3}' WHERE mgr_fk = '{4}' ", ac.bank_id,ac.Name,ac.Bank_AC,ac.Branch,ac.mgr_fk);
                    return db.Connection.Execute(query) >= 0;
                }
                else//插入
                {
                    string query = string.Format("INSERT INTO manager_bankac (`mgr_fk`,`bank_id`,`name`,`bank_ac`,`branch`) VALUES ( '{0}','{1}','{2}','{3}','{4}')",ac.mgr_fk,ac.bank_id,ac.Name,ac.Bank_AC,ac.Branch);
                    return db.Connection.Execute(query) >= 0;
                }
            }
        }

        /// <summary>
        /// 查询某个代理一个月以内或者待处理的所有出入金操作
        /// </summary>
        /// <param name="agentfk"></param>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperCashOperation> GetAgentLatestCashOperation(int agentfk)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM manager_cashinout  WHERE mgr_fk = '{0}' AND (status='{1}' || datetime>= '{2}')", agentfk, QSEnumCashInOutStatus.PENDING,Util.ToTLDateTime(DateTime.Now.AddMonths(-1)));
                Util.Debug(query);
                return   db.Connection.Query<JsonWrapperCashOperation>(query);
            }
        }

        /// <summary>
        /// 插入出入金操作
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool InsertAgentCashOperation(JsonWrapperCashOperation op)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO manager_cashinout (`mgr_fk`,`datetime`,`operation`,`amount`,`ref`,`status`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}')", op.mgr_fk, op.DateTime, op.Operation, op.Amount, op.Ref, op.Status);
                return db.Connection.Execute(query) > 0;
            }
        }
    }
}
