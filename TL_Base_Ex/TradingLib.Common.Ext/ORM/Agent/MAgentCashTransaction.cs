using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MAgentCashTransaction : MBase
    {
        /// <summary>
        /// 插入一条出入金记录
        /// </summary>
        /// <param name="txn"></param>
        public static void InsertCashTransaction(CashTransaction txn)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO log_agent_cashtrans (`account`,`amount`,`txntype`,`equitytype`,`txnref`,`comment`,`settleday`,`settled`,`datetime`,`operator`,`txnid`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", txn.Account, txn.Amount, txn.TxnType, txn.EquityType, txn.TxnRef, txn.Comment, txn.Settleday, txn.Settled ? 1 : 0, txn.DateTime, txn.Operator, txn.TxnID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 标注出入金记录已结算
        /// </summary>
        /// <param name="txn"></param>
        public static void MarkeCashTransactionSettled(CashTransaction txn)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE log_agent_cashtrans SET settled=1 WHERE txnid='{0}'", txn.TxnID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 获得所有未结算出入金记录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CashTransactionImpl> SelectAgentCashTransactionsUnSettled(int tradingday)
        {

            using (DBMySql db = new DBMySql())
            {
                if (tradingday == 0)
                {
                    string query = string.Format("SELECT * FROM log_agent_cashtrans WHERE settled=0");
                    return db.Connection.Query<CashTransactionImpl>(query);
                }
                else
                {
                    string query = string.Format("SELECT * FROM log_agent_cashtrans WHERE settled=0 AND settleday='{0}'", tradingday);
                    return db.Connection.Query<CashTransactionImpl>(query);
                }
            }
        }
    }
}
