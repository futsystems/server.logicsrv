using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.Common.DataFarm
{
    public partial class DataServerBase
    {
        [DataCommandAttr("QryHistTable", "QryHistTable - Qry HistBar Table Info", "查询历史数据表信息")]
        public void Command_QryTableInfo(IServiceHost host, IConnection conn)
        {
            logger.Info("Conn:{0} QryHistTable Infomation".Put(conn.SessionID));
            //foreach (var t in GetHistDataSotre().HistTableInfo)
            //{
            //    if(t.Name.StartsWith("CFFEX"))
            //    logger.Info(t.ToString());
            //}
            
        }
    }
}
