using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MDomain:MBase
    {
        /// <summary>
        /// 获得所有域信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DomainImpl> SelectDomains()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM domain");
                return db.Connection.Query<DomainImpl>(query);
            }
        }
    }
}
