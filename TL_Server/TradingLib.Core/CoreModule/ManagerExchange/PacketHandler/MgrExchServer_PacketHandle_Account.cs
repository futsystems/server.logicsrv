using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        



        /// <summary>
        /// 查询交易帐号列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRQryAccount(MGRQryAccountRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求下载交易帐户列表:{1}", session.AuthorizedID, request.ToString()));
            IAccount[] list = manager.GetVisibleAccount().ToArray();
            if (list.Length > 0)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    RspMGRQryAccountResponse response = ResponseTemplate<RspMGRQryAccountResponse>.SrvSendRspResponse(request);
                    response.oAccount = list[i].GenAccountLite();
                    CacheRspResponse(response, i == list.Length - 1);
                }
            }
            else
            {
                RspMGRQryAccountResponse response = ResponseTemplate<RspMGRQryAccountResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);

            }
        }


        /// <summary>
        /// 设定观察交易帐号列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRWatchAccount(MGRWatchAccountRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求设定观察列表:{1}", session.AuthorizedID, request.ToString()));
            CustInfoEx c = customerExInfoMap[request.ClientID];
            c.Watch(request.AccountList);
        }

        void SrvOnMGRResumeAccount(MGRResumeAccountRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求恢复交易数据,帐号:{1}", session.AuthorizedID, request.ResumeAccount));
            //判断权限

            //将请求放入队列等待处理
            CustInfoEx c = customerExInfoMap[request.ClientID];
            c.Selected(request.ResumeAccount);//保存管理客户端选中的交易帐号
            _resumecache.Write(request);
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "FlatAllPosition", "FlatAllPosition - falt all position", "平调所有子账户持仓")]
        public void CTE_UpdateAccountExStrategyTemplate(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                foreach (var account in manager.Domain.GetAccounts())
                {
                    account.InactiveAccount();
                    account.FlatPosition(QSEnumOrderSource.QSMONITER, "一键强平");

                    Util.sleep(500);
                }
                session.OperationSuccess("强平成功");
            }
            else
            {
                throw new FutsRspError("无权执行强平操作");
            }
        }

    }
}
