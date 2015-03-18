using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;
using TradingLib.Protocol;


namespace TradingLib.ORM
{
    public class  MReport:MBase
    {

        /// <summary>
        /// select securitycode as sec_code,sum(ABS(log_trades.xsize)) as total_size,sum(log_trades.commission) as total_commission,sum(log_trades.profit) as total_profit  from log_trades ,accounts where accounts.mgr_fk =2 and log_trades.account = accounts.account GROUP BY securitycode
        /// 统计某个代理的客户 在一段时间内按品种分组的 总成交量，手续费，总盈亏
        /// </summary>
        public static IEnumerable<SummaryViaSec> GenSummaryViaSecCode(int startDate,int endDate,int manager_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("select  t1.securitycode as sec_code,sum(ABS(t1.xsize)) as total_size,sum(t1.commission) as total_commission,sum(t1.profit) as total_profit,t2.mgr_fk as manager_id from (select * from log_trades where xdate>='{0}' and xdate<='{1}') t1 LEFT JOIN accounts t2 on t1.account = t2.account where mgr_fk='{2}' GROUP BY sec_code", startDate, endDate, manager_id);
                return db.Connection.Query<SummaryViaSec>(query);
            }
        }
    }
}
