//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;


//namespace TradingLib.MySql
//{
//    internal class SecForigenKey
//    {
//        public int exchange_fk { get; set; }
//        public int mkttime_fk { get; set; }
//    }
//    public class MSecurityFamily:MBase
//    {
//        /// <summary>
//        /// 返回帐户类别列表
//        /// </summary>
//        /// <returns></returns>
//        public static IEnumerable<SecurityFamilyImpl> SelectSecurity()
//        {
//            mysqlDBBase db = InfoDBHelper.BorrowDB();
//            const string query = "SELECT a.id,a.code,a.name,a.currency,a.type,a.multiple,a.pricetick,a.tradeable,a.underlaying_fk,a.entrycommission,a.exitcommission,a.margin,a.extramargin,a.maintancemargin,a.exchange_fk,a.mkttime_fk FROM info_security a";
//            IEnumerable<SecurityFamilyImpl> result = db.Connection.Query<SecurityFamilyImpl, SecForigenKey, SecurityFamilyImpl>(query, (sec, fk) => { sec.Exchange = BasicTracker.ExchagneTracker[fk.exchange_fk]; sec.MarketTime = BasicTracker.MarketTimeTracker[fk.mkttime_fk]; return sec; }, null, null, false, "exchange_fk", null, null).ToArray(); ;

//            InfoDBHelper.ReturDB(db);
//            return result;

//        }
//    }
//}
