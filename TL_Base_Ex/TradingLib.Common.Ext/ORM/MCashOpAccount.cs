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
    public class MCashOpAccount:MBase
    {
        /// <summary>
        /// 插入出入金操作
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool InsertAccountCashOperation(JsonWrapperCashOperation op)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO log_cashopreq (`account`,`datetime`,`operation`,`amount`,`ref`,`status`,`source`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')", op.Account, op.DateTime, op.Operation, op.Amount, op.Ref, op.Status,op.Source);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 更新出入金操作的MD5Sign
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool UpdateAccountCashOperationMD5Sign(JsonWrapperCashOperation op)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE log_cashopreq SET md5sign = '{0}'  WHERE ref = '{1}'",op.MD5Sign,op.Ref);
                return db.Connection.Execute(query) > 0;
            }
        }
        /// <summary>
        /// 通过CashOperation Reference获得某个交易帐户某个CashOperation
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ordref"></param>
        /// <returns></returns>
        public static JsonWrapperCashOperation GetAccountCashOperation(string opref)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM log_cashopreq  WHERE ref= '{0}'",opref);
                return db.Connection.Query<JsonWrapperCashOperation>(query).SingleOrDefault();
            }
        }
        ///// <summary>
        ///// 通过opref 和 MD5Sign来获得对应的operation 用于验证支付成功页面的访问是否是正常访问
        ///// </summary>
        ///// <param name="opref"></param>
        ///// <param name="md5sign"></param>
        ///// <returns></returns>
        //public static JsonWrapperCashOperation GetAccountCashOperation(string opref, string md5sign)
        //{
        //    using (DBMySql db = new DBMySql())
        //    {
        //        string query = String.Format("SELECT * FROM log_cashopreq  WHERE ref= '{0}' AND md5sign='{1}'", opref,md5sign);
        //        return db.Connection.Query<JsonWrapperCashOperation>(query).SingleOrDefault();
        //    }
        //}

        /// <summary>
        /// 确认出入金操作
        /// 这里需要在数据库事务中进行
        /// 在确认的时候 需要成功在manager_cashtrans中进行记录
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool ConfirmAccountCashOperation(JsonWrapperCashOperation op)
        {
            op.Status = QSEnumCashInOutStatus.CONFIRMED;
            using (DBMySql db = new DBMySql())
            {
                using (var transaction = db.Connection.BeginTransaction())
                {
                    bool istransok = true;
                    string query = String.Format("UPDATE log_cashopreq SET status = '{0}'  WHERE account = '{1}' AND ref ='{2}'", QSEnumCashInOutStatus.CONFIRMED, op.Account, op.Ref);
                    istransok = db.Connection.Execute(query) > 0;

                    decimal amount = op.Operation == QSEnumCashOperation.Deposit ? op.Amount : op.Amount * -1;
                    string query2 = String.Format("Insert into log_cashtrans (`datetime`,`amount`,`comment`,`account`,`transref`,`settleday`) values('{0}','{1}','{2}','{3}','{4}','{5}')", DateTime.Now.ToString(), amount.ToString(),"Online Deposit",op.Account, op.Ref, TLCtxHelper.Ctx.SettleCentre.NextTradingday);
                    //return db.Connection.Execute(query) > 0;

                    //JsonWrapperCasnTrans trans = new JsonWrapperCasnTrans();
                    //trans.mgr_fk = op.mgr_fk;
                    //trans.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                    //trans.TransRef = op.Ref;
                    //trans.DateTime = DateTime.Now;
                    //trans.Comment = "";
                    //trans.Amount = (op.Operation == QSEnumCashOperation.Deposit ? 1 : -1) * op.Amount;
                    //string query2 = string.Format("INSERT INTO manager_cashtrans (`mgr_fk`,`settleday`,`datetime`,`amount`,`transref`,`comment`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}')", trans.mgr_fk, trans.Settleday, trans.DateTime, trans.Amount, trans.TransRef, trans.Comment);

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
        public static bool CancelAccountCashOperation(JsonWrapperCashOperation op)
        {
            op.Status = QSEnumCashInOutStatus.CANCELED;
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE log_cashopreq SET status = '{0}'  WHERE account = '{1}' AND ref ='{2}'", QSEnumCashInOutStatus.CANCELED, op.Account, op.Ref);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 拒绝出入金操作请求
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool RejectAccountCashOperation(JsonWrapperCashOperation op)
        {
            op.Status = QSEnumCashInOutStatus.REFUSED;
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE log_cashopreq SET status = '{0}'  WHERE account = '{1}' AND ref ='{2}'", QSEnumCashInOutStatus.REFUSED, op.Account, op.Ref);
                return db.Connection.Execute(query) > 0;
            }
        }

    }
}
