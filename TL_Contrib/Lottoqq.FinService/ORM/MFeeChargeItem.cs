using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Contrib.FinService;
using TradingLib.Mixins.DataBase;


namespace TradingLib.Contrib.FinService.ORM
{
    public class MFeeChargeItem:MBase
    {
        public static bool InsertFeeChargeItem(FeeChargeItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO service_feecharge (`totalfee`,`agentfee`,`agentprofit`,`chargetype`,`collecttype`,`account`,`serviceplan_fk`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')", item.TotalFee, item.AgentFee, item.AgetProfit, item.ChargeType,item.CollectType,item.Account,item.serviceplan_fk);
                int row = db.Connection.Execute(query);

                return row > 0;
            }
        }
    }
}
