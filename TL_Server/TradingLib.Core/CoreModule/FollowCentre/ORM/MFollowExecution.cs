using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{

    public class MFollowExecution : MBase
    {
        /// <summary>
        /// 插入跟单项目
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        public static void InsertFollowExecution(FollowExecution ex)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO follow_executions (`settleday`,`strategyid`,`followkey`,`sourcesignal`,`signalinfo`,`exchange`,`symbol`,`side`,`size`,`opentime`,`openavgprice`,`openslip`,`closetime`,`closeavgprice`,`closeslip`,`realizedpl`,`commisson`,`profit`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')", ex.Settleday, ex.StrategyID, ex.FollowKey, ex.SourceSignal, ex.SignalInfo, ex.Exchange, ex.Symbol, ex.Side ? 1 : 0, ex.Size, ex.OpenTime, ex.OpenAvgPrice, ex.OpenSlip, ex.CloseTime, ex.CloseAvgPrice, ex.CloseSlip, ex.RealizedPL, ex.Commission, ex.Profit);
                db.Connection.Execute(query);
            }
        }
    }
}
