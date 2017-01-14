﻿using System;
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
        //void SrvOnMGRQryAccount(MGRQryAccountRequest request, ISession session, Manager manager)
        //{
        //    logger.Info(string.Format("Manager[{0}] QryAccountList", session.AuthorizedID));
        //    IAccount[] list = manager.GetVisibleAccount().ToArray();
        //    if (list.Length > 0)
        //    {
        //        for (int i = 0; i < list.Length; i++)
        //        {
        //            RspMGRQryAccountResponse response = ResponseTemplate<RspMGRQryAccountResponse>.SrvSendRspResponse(request);
        //            response.AccountItem = list[i].ToAccountItem();
        //            CacheRspResponse(response, i == list.Length - 1);
        //        }
        //    }
        //    else
        //    {
        //        RspMGRQryAccountResponse response = ResponseTemplate<RspMGRQryAccountResponse>.SrvSendRspResponse(request);
        //        CacheRspResponse(response);

        //    }
        //}


        /// <summary>
        /// 统一使用AccountCreation对象创建交易帐户
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountList", "QryAccountList - qry  account list", "查询交易账户")]
        public void CTE_QryAccountList(ISession session)
        {
            var manager = session.GetManager();

            IAccount[] list = manager.GetVisibleAccount().ToArray();
            if (list.Length > 0)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    session.ReplyMgr(list[i].ToAccountItem(), i == list.Length - 1);
                }
            }
            else
            {
                session.ReplyMgr("");
            }
        }


        ///// <summary>
        ///// 设定观察交易帐号列表
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="session"></param>
        ///// <param name="manager"></param>
        //void SrvOnMGRWatchAccount(MGRWatchAccountRequest request, ISession session, Manager manager)
        //{
        //    logger.Info(string.Format("Manager[{0}] Set WatchList:{1}", session.AuthorizedID, string.Join(",",request.AccountList.ToArray())));
        //    var c = customerExInfoMap[request.ClientID];
        //    c.Watch(request.AccountList);
        //}

        /// <summary>
        /// 设定观察交易账户列表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "WatchAccountList", "WatchAccountList - watch  account list", "设定观察账户列表",QSEnumArgParseType.Json)]
        public void CTE_WatchAccountList(ISession session,string json)
        {
            string[] accounts = json.DeserializeObject<string[]>();
            var c = customerExInfoMap[session.Location.ClientID];
            c.Watch(accounts);
        }

        /// <summary>
        /// 设定当前选中交易账户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnMGRResumeAccount(MGRResumeAccountRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("Manager[{0}] Resume Account:{1}", session.AuthorizedID, request.ResumeAccount));
            var c = customerExInfoMap[request.ClientID];
            c.Selected(request.ResumeAccount);
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
                    TLCtxHelper.ModuleAccountManager.InactiveAccount(account.ID);
                    TLCtxHelper.ModuleRiskCentre.FlatAllPositions(account.ID, QSEnumOrderSource.QSMONITER, "一键强平");
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
