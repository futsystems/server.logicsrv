using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MsgExchServer
    {
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "ClearAccountTerminals", "ClearAccountTerminals - clear account logined terminal", "注销某个帐号的所登入的所有终端")]
        public void CTE_UpdateAccountMarginTemplate(ISession session, string account)
        {
            Manager manager = session.GetManager();
            if (!manager.IsRoot())
            {
                throw new FutsRspError("无权进行此操作");
            }

            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("无权修改该交易帐户");
            }

            tl.ClearTerminalsForAccount(account);
            session.RspMessage("注销交易终端成功");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QrySessionInfo", "QrySessionInfo - 查询回话信息", "查询某个交易帐户的登入信息")]
        public void CTE_QrySessionInfo(ISession session, string account)
        {
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }


            Manager manager = session.GetManager();
            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("无权查询该交易帐户");
            }
            SessionInfo[] infos = tl.ClientsForAccount(account).Select(c => new SessionInfo()
            {
                Account = account,
                ClientID = c.Location.ClientID,
                FrontID = c.Location.FrontID,
                IPAddress = c.IPAddress,
                ProductInfo = c.ProductInfo,
                CreatedTime = c.CreatedTime,
                Login = true,

            }).ToArray();
            for (int i = 0; i < infos.Length; i++)
            {
                session.ReplyMgr(infos[i], i == infos.Length - 1);
            }

        }
    }
}
