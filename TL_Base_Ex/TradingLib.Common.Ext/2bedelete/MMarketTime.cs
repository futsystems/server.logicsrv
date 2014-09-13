//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MySql.Data.MySqlClient;
//using System.Data;
//using TradingLib.API;
//using TradingLib.Common;

//namespace TradingLib.MySql
//{
//    internal class MarketTimeString
//    {
//        public string MarketTime { get; set; }
//    }
//    public class MMarketTime:MBase
//    {
//        /// <summary>
//        /// 返回帐户类别列表
//        /// </summary>
//        /// <returns></returns>
//        public static IEnumerable<MarketTime> SelectSession()
//        {
//            mysqlDBBase db = InfoDBHelper.BorrowDB();
//            const string query = "SELECT a.id,a.name,a.description,a.markettime FROM info_markettime a";
//            IEnumerable<MarketTime> result = db.Connection.Query<MarketTime, MarketTimeString, MarketTime>(query, (mkttime, mkttimestr) => { mkttime.DeserializeMktTimeString(mkttimestr.MarketTime); return mkttime; }, null, null, false, "markettime", null, null).ToList<MarketTime>();

//            InfoDBHelper.ReturDB(db);
//            return result;

//        }
//    }
//}
