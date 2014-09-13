//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;


//namespace TradingLib.MySql
//{
//public class MSymbol:MBase
//{
//    public static IEnumerable<SymbolImpl> SelectSymbol()
//    {
//        mysqlDBBase db = InfoDBHelper.BorrowDB();
//        const string query = "SELECT a.id,a.symbol,a.entrycommission,a.exitcommission,a.margin,a.expiremonth,a.date,a.strike,a.optionside,a.extramargin,a.maintancemargin,a.underlaying_fk,a.underlayingsymbol_fk,s.id FROM info_symbols a LEFT JOIN info_security s on a.security_fk = s.id";
//        IEnumerable<SymbolImpl> result = db.Connection.Query<SymbolImpl, NewId, SymbolImpl>(query, (symbol, newid) => { symbol.SecurityFamily = BasicTracker.SecurityTracker[newid.Id]; return symbol; }, null, null, false, "id", null, null).ToArray(); ;

//        InfoDBHelper.ReturDB(db);
//        return result;
            
//    }
//}
//}
