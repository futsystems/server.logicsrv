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
        ///// <summary>
        ///// 获得所有服务计划
        ///// </summary>
        ///// <returns></returns>
        //public static IEnumerable<AccountContact> SelectAccountContacts()
        //{
        //    using (DBMySql db = new DBMySql())
        //    {
        //        const string query = "SELECT *  FROM contrib_ng_contact";
        //        return db.Connection.Query<AccountContact>(query, null);
        //    }
        //}

        /// <summary>
        /// 插入出如今操作
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static bool InsertCashOperation(CashOperation op)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_cash_payment_operation (`account`,`amount`,`datetime`,`operationtype`,`gatewaytype`,`status`,`ref`,`comment`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", op.Account, op.Amount, op.DateTime, op.OperationType, op.GateWayType, op.Status, op.Ref,op.Comment);
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
