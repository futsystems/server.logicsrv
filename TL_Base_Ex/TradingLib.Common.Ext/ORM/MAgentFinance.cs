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

    internal class PendingAmount
    {
        public int mgr_fk { get; set; }

        public int Amount { get; set; }
    }

    internal class CashAmount
    {
        public int mgr_fk { get; set; }

        public int TotalAmount { get; set; }
    }


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
                string query = String.Format("SELECT * FROM manager_cashopreq  WHERE mgr_fk = '{0}' AND (status='{1}' || datetime>= '{2}')", agentfk, QSEnumCashInOutStatus.PENDING, Util.ToTLDateTime(DateTime.Now.AddMonths(-1)));
                Util.Debug(query);
                return   db.Connection.Query<JsonWrapperCashOperation>(query);
            }
        }

        /// <summary>
        /// 查询一个月以内所有出入金操作或待处理操作
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperCashOperation> GetAgentLatestCashOperationTotal()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM manager_cashopreq  WHERE status='{0}' || datetime>= {1}",QSEnumCashInOutStatus.PENDING, Util.ToTLDateTime(DateTime.Now.AddMonths(-1)));
                Util.Debug(query);
                return db.Connection.Query<JsonWrapperCashOperation>(query);
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
                string query = string.Format("INSERT INTO manager_cashopreq (`mgr_fk`,`datetime`,`operation`,`amount`,`ref`,`status`,`source`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')", op.mgr_fk, op.DateTime, op.Operation, op.Amount, op.Ref, op.Status,op.Source);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 确认出入金操作
        /// 这里需要在数据库事务中进行
        /// 在确认的时候 需要成功在manager_cashtrans中进行记录
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool ConfirmAgentCashOperation(JsonWrapperCashOperation op)
        {
            op.Status = QSEnumCashInOutStatus.CONFIRMED;
            using (DBMySql db = new DBMySql())
            {
                using (var transaction = db.Connection.BeginTransaction())
                {
                    bool istransok = true;
                    string query = String.Format("UPDATE manager_cashopreq SET status = '{0}'  WHERE mgr_fk = '{1}' AND ref ='{2}'", QSEnumCashInOutStatus.CONFIRMED, op.mgr_fk, op.Ref);
                    istransok =  db.Connection.Execute(query) > 0;

                    JsonWrapperCasnTrans trans = new JsonWrapperCasnTrans();
                    trans.mgr_fk = op.mgr_fk;
                    trans.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                    trans.TransRef = op.Ref;
                    trans.DateTime = Util.ToTLDateTime();
                    trans.Comment = "";
                    trans.Amount = (op.Operation == QSEnumCashOperation.Deposit ? 1 : -1) * op.Amount;
                    string query2 = string.Format("INSERT INTO manager_cashtrans (`mgr_fk`,`settleday`,`datetime`,`amount`,`transref`,`comment`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}')",trans.mgr_fk,trans.Settleday,trans.DateTime,trans.Amount,trans.TransRef,trans.Comment);

                    istransok = istransok && db.Connection.Execute(query2) > 0;
                    if (istransok)
                        transaction.Commit();
                    return istransok;
                }
            }
        }

        /// <summary>
        /// 取消出入金操作
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool CancelAgentCashOperation(JsonWrapperCashOperation op)
        {
            op.Status = QSEnumCashInOutStatus.CANCELED;
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE manager_cashopreq SET status = '{0}'  WHERE mgr_fk = '{1}' AND ref ='{2}'", QSEnumCashInOutStatus.CANCELED, op.mgr_fk, op.Ref);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 拒绝出入金操作请求
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool RejectAgentCashOperation(JsonWrapperCashOperation op)
        {
            op.Status = QSEnumCashInOutStatus.REFUSED;
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE manager_cashopreq SET status = '{0}'  WHERE mgr_fk = '{1}' AND ref ='{2}'", QSEnumCashInOutStatus.REFUSED, op.mgr_fk, op.Ref);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 获得某个代理的待充值额度
        /// </summary>
        /// <param name="agentfk"></param>
        /// <returns></returns>
        public static decimal GetPendingDeposit(int agentfk)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT mgr_fk ,sum(amount) as amount FROM manager_cashopreq  WHERE mgr_fk = '{0}' AND status='{1}' AND operation='{2}'", agentfk, QSEnumCashInOutStatus.PENDING, QSEnumCashOperation.Deposit);
                PendingAmount pm = db.Connection.Query<PendingAmount>(query).SingleOrDefault<PendingAmount>();
                if (pm != null)
                {
                    return pm.Amount;
                }
                return 0;
            }
        }

        /// <summary>
        /// 获得某个代理的待提现额度
        /// </summary>
        /// <param name="agentfk"></param>
        /// <returns></returns>
        public static decimal GetPendingWithdraw(int agentfk)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT mgr_fk ,sum(amount) as amount FROM manager_cashopreq  WHERE mgr_fk = '{0}' AND status='{1}' AND operation='{2}'", agentfk, QSEnumCashInOutStatus.PENDING, QSEnumCashOperation.WithDraw);
                PendingAmount pm = db.Connection.Query<PendingAmount>(query).SingleOrDefault<PendingAmount>();
                if (pm != null)
                {
                    return pm.Amount;
                }
                return 0;
            }
        }

        /// <summary>
        /// 查询某个代理 某个交易日的充值金额
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static decimal GetDepositOfTradingDay(int agentfk, int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT Sum(amount) as totalamount,mgr_fk FROM manager_cashtrans where settleday ='{0}' and mgr_fk='{1}' and amount>0", tradingday, agentfk);
                CashAmount total = db.Connection.Query<CashAmount>(query, null).Single<CashAmount>();
                return Math.Abs(total.TotalAmount);
            }
        }

        /// <summary>
        /// 查询某个代理 某个交易日的提现金额
        /// </summary>
        /// <param name="agentfk"></param>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static decimal GetWithdrawOfTradingDay(int agentfk, int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT Sum(amount) as totalamount,mgr_fk FROM manager_cashtrans where settleday ='{0}' and mgr_fk='{1}' and amount<0", tradingday, agentfk);
                CashAmount total = db.Connection.Query<CashAmount>(query, null).Single<CashAmount>();
                return Math.Abs(total.TotalAmount);
            }
        }



        /// <summary>
        /// 获取在一个时间段内所有出入金记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperCasnTrans> SelectAgentCashTrans(int agentfk, long start, long end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Empty;
                if (agentfk ==0)
                {
                    query = String.Format("SELECT * FROM manager_cashtrans  WHERE  datetime>= {0} AND datetime<= {1} ",start,end);
                }
                else
                {
                    query = String.Format("SELECT * FROM manager_cashtrans  WHERE mgr_fk='{0}' AND datetime>= {1} AND datetime<= {2} ", agentfk,start, end);
                }
                return db.Connection.Query<JsonWrapperCasnTrans>(query);
            }
        }
    }
}
