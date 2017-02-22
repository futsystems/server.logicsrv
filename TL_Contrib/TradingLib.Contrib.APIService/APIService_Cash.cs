using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.APIService
{
    public partial class APIServiceBundle
    {
         /// <summary>
        /// 更新跟单策略设置
        /// </summary>
        /// <param name="session"></param>
        /// <param name="payload"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateGateWayConfig", "UpdateGateWayConfig - update config of payment gateway", "更新支付网关信息", QSEnumArgParseType.Json)]
        public void CTE_UpdateFollowStrategyCfg(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }
            GateWayConfig config = json.DeserializeObject<GateWayConfig>();

            if (config.ID == 0)
            {
                config.Domain_ID = manager.domain_id;
            }
            else
            {
                if (config.Domain_ID != manager.domain_id)
                {
                    throw new FutsRspError("无权进行此操作");
                }
            }

            APITracker.GateWayTracker.UpdateGateWayConfig(config);

            session.NotifyMgr("NotifyGateWayConfig", APITracker.GateWayTracker.GetGateWayConfig(config.ID));
            session.RspMessage("更新支付网关成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryGateWayConfig", "QryGateWayConfig - qry config of payment gateway", "查询支付网关")]
        public void CTE_UpdateFollowStrategyCfg(ISession session)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }
            GateWayConfig config = ORM.MGateWayConfig.SelectGateWayConfig(manager.domain_id);

            session.ReplyMgr(config, true);
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCashOperation", "QryCashOperation - qry cash operation", "查询出入金操作记录", QSEnumArgParseType.Json)]
        public void CTE_QryCashOperation(ISession session, string json)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }
            if (!manager.Domain.Module_PayOnline)
            {
                throw new FutsRspError("无权进行此操作");
            }

            var data = json.DeserializeObject();
            int start = int.Parse(data["start"].ToString());
            int end = int.Parse(data["end"].ToString());



            CashOperation[] items = ORM.MCashOperation.SelectCashOperations(manager.domain_id, start, end).ToArray();
            session.ReplyMgrArray(items);

        }
    }
}
