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
    public class MFinService : MBase
    {
        /// <summary>
        /// 获得所有配资服务
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FinService> SelectFinServices()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_mainacct_finservice_service";
                return db.Connection.Query<FinService>(query, null);
            }
        }

        /// <summary>
        /// 插入配资服务
        /// </summary>
        /// <param name="fs"></param>
        public static void InsertFinService(FinServiceSetting fs)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_mainacct_finservice_service (`account`,`servicetype`,`chargefreq`,`interesttype`,`chargevalue`,`chargetime`,`chargemethod`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')",fs.Account,fs.ServiceType,fs.ChargeFreq,fs.InterestType,fs.ChargeValue,fs.ChargeTime,fs.ChargeMethod);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新配资本服务
        /// </summary>
        /// <param name="fs"></param>
        public static void UpdateFinService(FinServiceSetting fs)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE contrib_mainacct_finservice_service SET servicetype = '{0}',chargefreq = '{1}',interesttype = '{2}',chargevalue = '{3}',chargetime = '{4}',chargemethod = '{5}' WHERE account = '{6}'", fs.ServiceType, fs.ChargeFreq, fs.InterestType, fs.ChargeValue, fs.ChargeTime, fs.ChargeMethod,fs.Account);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除某个交易帐户的配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool DeleteFinService(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM contrib_mainacct_finservice_service WHERE account = '{0}'", account);
                return db.Connection.Execute(query) >= 0;
            }
        }

    }
}
