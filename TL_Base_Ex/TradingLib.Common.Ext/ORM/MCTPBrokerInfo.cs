using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MCTPBrokerInfo:MBase
    {
        /// <summary>
        /// 获得所有期货公司CTP交易通道信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CTPBrokerInfo> SelectCTPBrokerInfos()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM info_ctpbrokerinfo");
                return db.Connection.Query<CTPBrokerInfo>(query);
            }
        }

    }
}
