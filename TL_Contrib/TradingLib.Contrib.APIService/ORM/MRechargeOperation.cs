using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using TradingLib.Contrib.APIService;


namespace TradingLib.ORM
{
    public class MCashOperation : MBase
    {
        /// <summary>
        /// 插入出如今操作
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static bool InsertCashOperation(CashOperation op)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_cash_payment_operation (`account`,`amount`,`datetime`,`operationtype`,`gatewaytype`,`status`,`ref`,`comment`,`domain_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", op.Account, op.Amount, op.DateTime, op.OperationType, op.GateWayType, op.Status, op.Ref, op.Comment,op.Domain_ID);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 通过出入金编号查找出入金操作
        /// </summary>
        /// <param name="transid"></param>
        /// <returns></returns>
        public static CashOperation SelectCashOperation(string transid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM contrib_cash_payment_operation  WHERE ref ='{0}'", transid);
                return db.Connection.Query<CashOperation>(query).FirstOrDefault();

            }
        }

        /// <summary>
        /// 查询一个时间段内某个分区内所有出入金请求记录
        /// </summary>
        /// <param name="domain_id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<CashOperation> SelectCashOperations(int domain_id,int start,int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM contrib_cash_payment_operation  WHERE domain_id ='{0}' AND datetime >='{1}'  AND datetime <='{2}'", domain_id, (((long)start) * 1000000), (((long)end) * 1000000 + 235959));
                return db.Connection.Query<CashOperation>(query);

            }
        }

        /// <summary>
        /// 查询某个交易账户待处理出入金请求
        /// </summary>
        /// <param name="account"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static IEnumerable<CashOperation> SelectPendingCashOperation(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM contrib_cash_payment_operation  WHERE account ='{0}' AND status = '{1}'", account,QSEnumCashInOutStatus.PENDING);
                return db.Connection.Query<CashOperation>(query);

            }
        }

        /// <summary>
        /// 更新出入操作状态
        /// </summary>
        /// <param name="item"></param>
        public static void UpdateCashOperationStatus(CashOperation item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE contrib_cash_payment_operation SET status='{0}',comment='{1}' WHERE ref='{2}'", item.Status,item.Comment, item.Ref);
                db.Connection.Execute(query);
            }
        }
    }
}
