﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MCashTransaction:MBase
    {

        /// <summary>
        /// 插入一条出入金记录
        /// </summary>
        /// <param name="txn"></param>
        public static void InsertCashTransaction(CashTransaction txn)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO log_cashtrans (`account`,`amount`,`txntype`,`equitytype`,`txnref`,`comment`,`settleday`,`settled`,`datetime`,`operator`,`txnid`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", txn.Account, txn.Amount, txn.TxnType, txn.EquityType, txn.TxnRef, txn.Comment, txn.Settleday, txn.Settled ? 1 : 0, txn.DateTime, txn.Operator,txn.TxnID);
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
                string query = string.Format("UPDATE log_cashtrans SET settled=1 WHERE txnid='{0}'", txn.TxnID);
                db.Connection.Execute(query);
            }
        }
        /// <summary>
        /// 获得所有未结算出入金记录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CashTransactionImpl> SelectCashTransactionsUnSettled(int tradingday)
        {

            using (DBMySql db = new DBMySql())
            {
                if (tradingday == 0)
                {
                    string query = string.Format("SELECT * FROM log_cashtrans WHERE settled=0");
                    return db.Connection.Query<CashTransactionImpl>(query);
                }
                else
                {
                    string query = string.Format("SELECT * FROM log_cashtrans WHERE settled=0 AND settleday='{0}'",tradingday);
                    return db.Connection.Query<CashTransactionImpl>(query);
                }
            }
        }

        /// <summary>
        /// 查询某个交易帐户某个时间段内的出入金记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<CashTransactionImpl> SelectHistCashTransactions(string account, long begin, long end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM log_cashtrans WHERE account='{0}' AND datetime>='{1}' AND datetime<='{2}'", account, begin, end);
                return db.Connection.Query<CashTransactionImpl>(query);
            }
        }

        public static bool IsTransRefExist(string account, string transref)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM log_cashtrans WHERE account={0} AND transref ={1}", account, transref);
                return db.Connection.Query<TransRefFields>(query, null).ToArray().Length > 0;
            }
        }
    }
}