using System;
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
            logger.Debug("sessionupdate,will send to magr moniter");
            if (string.IsNullOrEmpty(c.Account.ID))
            {
                logger.Warn("Client:" + c.Location.ClientID + " do not have account,can not have session update event");
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
        void OnAccountChanged(IAccount account)
        {

            logger.Debug(string.Format("Account[{0}] Changed",account.ID));
            NotifyMGRAccountChangeUpdateResponse notify = ResponseTemplate<NotifyMGRAccountChangeUpdateResponse>.SrvSendNotifyResponse(account.ID);
            notify.AccountItem = account.ToAccountItem();
            CachePacket(notify);
        }

        /// <summary>
        /// 有新帐号增加时 向服务端通知
        /// </summary>
        /// <param name="account"></param>
        void OnAccountAdded(IAccount account)
        {
            NotifyMGRAccountChangeUpdateResponse notify = ResponseTemplate<NotifyMGRAccountChangeUpdateResponse>.SrvSendNotifyResponse(account.ID);
            notify.AccountItem = account.ToAccountItem();
            CachePacket(notify);
            
        }

        void OnAccountDeleted(IAccount account)
        {
            IEnumerable<ILocation> locations = GetNotifyTargets(account.GetNotifyPredicate());
            NotifyMGRAccountChangeUpdateResponse notify = ResponseTemplate<NotifyMGRAccountChangeUpdateResponse>.SrvSendNotifyResponse(locations);
            notify.AccountItem = account.ToAccountItem();
            CachePacket(notify);
        }


        void OnTick(Tick k)
        {
            tl.newTick(k);
        }


        void OnOrder(Order o)
        {
            _ocache.Write(o);
        }

        void OnOrderError(Order  order,RspInfo error)
        {
            _errorordercache.Write(new OrderErrorPack(order,error));
        }

        void OnTrade(Trade f)
        {
            _fcache.Write(f);
        }
    }
}
