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
                string query = string.Format("INSERT INTO log_service_feecharge (`totalfee`,`agentfee`,`agentprofit`,`chargetype`,`collecttype`,`account`,`serviceplan_fk`,`agent_fk`,`comment`,`settleday`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", item.TotalFee, item.AgentFee, item.AgetProfit, item.ChargeType, item.CollectType, item.Account, item.serviceplan_fk, item.Agent_fk, item.Comment, item.Settleday);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => item.ID = id, "id", "log_service_feecharge");
                return row > 0;
            }
        }

        /// <summary>
        /// 查找所有盘后采集的收费记录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FeeChargeItem> SelectFeeChargeItemAfterSettle()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM log_service_feecharge WHERE settleday = '{0}' AND collecttype='{1}'", TLCtxHelper.CmdSettleCentre.NextTradingday, EnumFeeCollectType.CollectAfterSettle);
                return db.Connection.Query<FeeChargeItem>(query);
            }
        }

        public static bool InsertCommissionItem(CommissionItem item)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO log_service_commission (`settleday`,`agent_fk`,`commission`,`subagent_fk`,`feecharge_fk`) VALUES ( '{0}','{1}','{2}','{3}','{4}')", item.Settleday, item.Agent_FK, item.Commission, item.SubAgent_FK, item.FeeCharge_FK);
                int row = db.Connection.Execute(query);

                return row > 0;
            }
        }


    }
}
