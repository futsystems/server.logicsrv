using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MStrategy: MBase
    {
        public static IEnumerable<FollowStrategyConfig> SelectFollowStrategyConfigs()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM follow_strategys";
                return db.Connection.Query<FollowStrategyConfig>(query, null);
            }
        }
    }
}
