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
                string query = string.Format("INSERT INTO log_cashopreq (`account`,`datetime`,`operation`,`amount`,`ref`,`status`,`source`,`recvinfo`,`submitter`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", op.Account, op.DateTime, op.Operation, op.Amount, op.Ref, op.Status, op.Source, op.RecvInfo,op.Submitter);
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

        /// <summary>
        /// 确认出入金操作
        /// 这里需要在数据库事务中进行
        /// 在确认的时候 需要成功在logcashtrans_cashtrans中进行记录
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
                    string query2 = String.Format("Insert into log_cashtrans (`datetime`,`amount`,`comment`,`account`,`transref`,`settleday`) values('{0}','{1}','{2}','{3}','{4}','{5}')", Util.ToTLDateTime(), amount.ToString(), op.Source.ToString(), op.Account, op.Ref, TLCtxHelper.ModuleSettleCentre.Tradingday);

                    istransok = istransok && db.Connection.Execute(query2) > 0;
                    if (istransok)
                        transaction.Commit();
                    return istransok;
                }
            }
        }

        /// <summary>
        /// 查询一个月以内所有出入金操作或待处理操作
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperCashOperation> GetAccountLatestCashOperationTotal()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM log_cashopreq  WHERE status='{0}' || datetime>= '{1}'", QSEnumCashInOutStatus.PENDING, Util.ToTLDateTime(DateTime.Now.AddMonths(-1)));
                Util.Debug(query);
                return db.Connection.Query<JsonWrapperCashOperation>(query);
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

        /// <summary>
        /// 获取在一个时间段内所有出入金记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperCasnTrans> SelectAccountCashTrans(string account, long start, long end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Empty;
                if (string.IsNullOrEmpty(account))
                {
                    query = String.Format("SELECT * FROM log_cashtrans  WHERE  datetime>= {0} AND datetime<= {1} ", start, start);
                }
                else
                {
                    query = String.Format("SELECT * FROM log_cashtrans  WHERE account='{0}' AND datetime>= {1} AND datetime<= {2} ", account, start,end);
                }
                return db.Connection.Query<JsonWrapperCasnTrans>(query);
            }
        }

    }
}
