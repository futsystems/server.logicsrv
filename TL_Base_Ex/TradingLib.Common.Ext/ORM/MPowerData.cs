using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MPowerData : MBase
    {
        /// <summary>
        /// 插入除权数据
        /// </summary>
        /// <param name="t"></param>
        public static void InsertPowerData(PowerData p)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO log_stk_power (`settleday`,`symbol`,`dividend`,`donateshares`,`rationeshares`,`rationeprice`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}')",p.Settleday,p.Symbol,p.Dividend,p.DonateShares,p.RationeShares,p.RationePrice);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新除权数据
        /// </summary>
        /// <param name="t"></param>
        public static void UpdatePowerData(PowerData p)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE log_stk_power SET dividend='{0}',donateshares='{1}',rationeshares='{2}',rationeprice='{3}'  WHERE settleday='{4}' AND symbol='{5}'", p.Dividend, p.DonateShares, p.RationeShares, p.RationePrice, p.Settleday, p.Symbol);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除某个除权数据
        /// </summary>
        /// <param name="template_id"></param>
        public static void DeletePowerData(PowerData p)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM log_stk_power WHERE settleday='{0}' AND symbol='{1}'", p.Settleday, p.Symbol);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除某个交易日的除权数据
        /// </summary>
        /// <param name="template_id"></param>
        public static void DeletePowerData(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM log_stk_power WHERE settleday='{0}'",settleday);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 获得某个交易日的所有除权数据
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static IEnumerable<PowerData> SelectPowerData(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM log_stk_power  WHERE settleday='{0}'", settleday);
                return db.Connection.Query<PowerData>(query);
            }
        }


    }
}
