﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 系统向管理服务推送相关消息
    /// </summary>
    public partial class MgrExchServer
    {

        /// <summary>
        /// 向管理客户端转发客户端的登入退出事件
        /// </summary>
        /// <param name="c"></param>
        /// <param name="login"></param>
        void newSessionUpdate(TrdClientInfo c, bool login)
        {
            debug("sessionupdate,will send to magr moniter", QSEnumDebugLevel.INFO);
            if (string.IsNullOrEmpty(c.Account.ID))
            {
                debug("Client:" + c.Location.ClientID + " do not have account,can not have session update event", QSEnumDebugLevel.WARNING);
            }
            else
            {
                NotifyMGRSessionUpdateNotify notify = ResponseTemplate<NotifyMGRSessionUpdateNotify>.SrvSendNotifyResponse(c.Account.ID);
                notify.TradingAccount = c.Account.ID;
                notify.IsLogin = login;
                notify.IPAddress = c.IPAddress;
                notify.HardwarCode = c.HardWareCode;
                notify.ProductInfo = c.ProductInfo;
                notify.FrontID = c.Location.FrontID;
                notify.ClientID = c.Location.ClientID;

                CachePacket(notify);
            }
        }

        /// <summary>
        /// 向管理客户端转发帐户变动
        /// </summary>
        /// <param name="account"></param>
        void newAccountChanged(string account)
        {
            debug("account changed,will send to manager montier", QSEnumDebugLevel.INFO);
            IAccount acc = TLCtxHelper.CmdAccount[account];
            NotifyMGRAccountChangeUpdateResponse notify = ResponseTemplate<NotifyMGRAccountChangeUpdateResponse>.SrvSendNotifyResponse(account);
            notify.oAccount = acc.GenAccountLite();
            CachePacket(notify);
        }

        /// <summary>
        /// 有新帐号增加时 向服务端通知
        /// </summary>
        /// <param name="account"></param>
        void newAccountAdded(string account)
        {
            debug("account added,will send to manager montier", QSEnumDebugLevel.INFO);
            IAccount acc = TLCtxHelper.CmdAccount[account];
            if (acc != null)
            {
                NotifyMGRAccountChangeUpdateResponse notify = ResponseTemplate<NotifyMGRAccountChangeUpdateResponse>.SrvSendNotifyResponse(account);
                notify.oAccount = acc.GenAccountLite();
                CachePacket(notify);
            }
        }

        void newOrder(Order o)
        {
            _ocache.Write(o);
        }

        void newOrderError(Order  order,RspInfo error)
        {
            _errorordercache.Write(new OrderErrorPack(order,error));
        }

        void newTrade(Trade f)
        {
            _fcache.Write(f);
        }

        void newCancel(long id)
        {
            Order o = TLCtxHelper.CmdUtils.SentOrder(id);
            
            if (o != null && o.isValid)
            {
                OrderAction action = new OrderActionImpl();
                action.Account = o.Account;
                action.ActionFlag = QSEnumOrderActionFlag.Delete;
                action.OrderID = o.id;
                newOrderAction(action);
            }
        }

        void newOrderAction(OrderAction action)
        {
            _occache.Write(action);
        }

        void newTick(Tick k)
        {
            tl.newTick(k);
        }
    }
}
