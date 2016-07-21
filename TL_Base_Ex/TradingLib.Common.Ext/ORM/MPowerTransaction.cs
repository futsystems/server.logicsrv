using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MPowerTransaction:MBase
    {
        /// <summary>
        /// 插入除权操作
        /// </summary>
        /// <param name="t"></param>
        public static void InsertPowerTransaction(PowerTransaction p)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO log_stk_power_trans (`settleday`,`account`,`symbol`,`size`,`dividend`,`shares`,`amount`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')", p.Settleday, p.Account, p.Symbol,p.Size,p.Dividend, p.Shares,p.Amount);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除某个交易日的除权数据
        /// </summary>
        /// <param name="settleday"></param>
        public static void DeletePowerTransaction(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM log_stk_power_trans WHERE settleday='{0}'",settleday);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 获得某个交易日的所有除权操作
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static IEnumerable<PowerTransaction> SelectPowerTransaction(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM log_stk_power_trans  WHERE settleday='{0}'", settleday);
                return db.Connection.Query<PowerTransaction>(query);
            }
        }
    }
}
