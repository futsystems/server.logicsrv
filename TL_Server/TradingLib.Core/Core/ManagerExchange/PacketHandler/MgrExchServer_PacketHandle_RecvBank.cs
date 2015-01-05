using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        /// <summary>
        /// 查询收款银行列表
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryReceiveableBank", "QryReceiveableBank - query QryReceiveableBank", "查询收款银行银行列表")]
        public void CTE_QryReceiveableBank(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager != null)
            {
                JsonWrapperReceivableAccount[] splist = manager.Domain.GetRecvBanks().ToArray();
                session.ReplyMgr(splist);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateReceiveableBank", "UpdateReceiveableBank - update  ReceiveableBank", "更新收款银行银行列表",true)]
        public void CTE_UpdateReceiveableBank(ISession session,string json)
        {
            try
            {
                Manager manager = session.GetManager();
                if (!manager.IsInRoot())
                {
                    throw new FutsRspError("无权添加收款银行信息");
                }

                JsonWrapperReceivableAccount bank = TradingLib.Mixins.Json.JsonMapper.ToObject<JsonWrapperReceivableAccount>(json);
                manager.Domain.UpdateRecvBanks(bank);

                session.OperationSuccess("更新收款银行信息成功");
                //通知银行信息变更
                session.NotifyMgr("NotifyRecvBank", manager.Domain.GetRecvBank(bank.ID), manager.Domain.GetRootLocations());
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }
    }
}
