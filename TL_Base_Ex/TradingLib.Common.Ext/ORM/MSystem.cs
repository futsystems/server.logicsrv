using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    
    public class MSystem:MBase
    {

        public static TLVersion GetVersion()
        { 
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM system";
                SystemInformation info = db.Connection.Query<SystemInformation>(query).SingleOrDefault<SystemInformation>();
                TLVersion v = new TLVersion();
                v.Major = info.Maj;
                v.Minor = info.Min;
                v.Fix = info.Fix;
                v.Date = info.Date;
                v.ProductType = info.ProductType;
                return v;
            }
        }
    }
}
