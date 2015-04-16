using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;


namespace TradingLib.Contrib.MainAcctFinService.ORM
{
    internal class Count
    {
        public int Num { get; set; }
    }
    public class MFee : MBase
    {
        public static IEnumerable<Fee> SelectFees()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_mainacct_finservice_fee";
                return db.Connection.Query<Fee>(query, null);
            }
        }

        public static IEnumerable<Fee> SelectFees(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT *  FROM contrib_mainacct_finservice_fee WHERE settleday='{0}'",settleday);
                return db.Connection.Query<Fee>(query, null);
            }
        }

        /// <summary>
        /// 获得某个ID的Fee对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Fee GetFee(int id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT *  FROM contrib_mainacct_finservice_fee WHERE id='{0}'", id);
                return db.Connection.Query<Fee>(query, null).FirstOrDefault();
            }
        }

        public static FeeSetting GetFeeSetting(int id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT *  FROM contrib_mainacct_finservice_fee WHERE id='{0}'", id);
                return db.Connection.Query<FeeSetting>(query, null).FirstOrDefault();
            }
        }

        /// <summary>
        /// 插入收费项目
        /// </summary>
        /// <param name="fee"></param>
        public static void InsertFee(Fee fee)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_mainacct_finservice_fee (`settleday`,`account`,`amount`,`feetype`,`datetime`,`collected`,`comment`,`chargemethod`,`chargetime`,`feestatus` ,`error` ) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", fee.Settleday, fee.Account, fee.Amount, fee.FeeType, fee.DateTime, fee.Collected ? 1 : 0, fee.Comment, fee.ChargeMethod, fee.ChargeTime, fee.FeeStatus,fee.Error);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => fee.ID = id, "id", "contrib_mainacct_finservice_fee");
            }
        }

        /// <summary>
        /// 删除某个交易帐户某个结算日的收费项目
        /// </summary>
        /// <param name="account"></param>
        /// <param name="settleday"></param>
        public static void DeleteFee(string account, int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM contrib_mainacct_finservice_fee WHERE account = '{0}' AND settleday='{1}' ", account,settleday);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新收费项目
        /// </summary>
        /// <param name="fee"></param>
        public static void UpdateFee(FeeSetting fee)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE contrib_mainacct_finservice_fee SET amount = '{0}',comment = '{1}' WHERE id='{2}'", fee.Amount, fee.Comment, fee.ID); 
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新计费的收取状态
        /// </summary>
        /// <param name="fee"></param>
        public static void UpdateFeeCollectStatus(Fee fee)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE contrib_mainacct_finservice_fee SET collected = '{0}' WHERE id = '{1}'",fee.Collected?1:0, fee.ID);
                db.Connection.Execute(query);
            } 
        }

        /// <summary>
        /// 更新计费状态
        /// </summary>
        /// <param name="fee"></param>
        public static void UpdateFeeStatus(Fee fee,string error="")
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE contrib_mainacct_finservice_fee SET feestatus = '{0}',error = '{1}' WHERE id = '{2}'", fee.FeeStatus,fee.Error,fee.ID);
                db.Connection.Execute(query);
            } 
        }

        /// <summary>
        /// 判断某个帐户某个类型的计费手已经收取过
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static bool HaveCharged(string account, QSEnumFeeType type,int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT sum(id) as num FROM contrib_mainacct_finservice_fee where account = '{0}' AND settleday='{1}' AND feetype='{2}'", account, settleday, type);
                return db.Connection.Query<Count>(query, null).First().Num > 0;
            }
        }

    }
}
