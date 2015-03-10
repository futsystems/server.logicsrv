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
            debug(string.Format("管理员:{0} 请求下载交易帐户列表:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);
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
            debug(string.Format("管理员:{0} 请求设定观察列表:{1}", session.AuthorizedID, request.ToString()), QSEnumDebugLevel.INFO);
            CustInfoEx c = customerExInfoMap[request.ClientID];
            c.Watch(request.AccountList);
        }

        void SrvOnMGRResumeAccount(MGRResumeAccountRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求恢复交易数据,帐号:{1}", session.AuthorizedID, request.ResumeAccount), QSEnumDebugLevel.INFO);
            //判断权限

            //将请求放入队列等待处理
            CustInfoEx c = customerExInfoMap[request.ClientID];
            c.Selected(request.ResumeAccount);//保存管理客户端选中的交易帐号
            _resumecache.Write(request);
        }

    }
}
