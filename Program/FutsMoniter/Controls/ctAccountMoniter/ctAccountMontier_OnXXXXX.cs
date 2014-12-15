﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using FutsMoniter.Common;

namespace FutsMoniter.Controls
{
    /// <summary>
    /// 服务端回报事件响应 
    /// </summary>
    public partial class ctAccountMontier
    {
        bool IsCurrentAccount(string account)
        {
            if (AccountSetlected == null) return false;
            if (account == AccountSetlected.Account) return true;
            return false;
        }

       

        

        void GotTick(Tick k)
        {
            ctPositionView1.GotTick(k);
            viewQuoteList1.GotTick(k);
        }

        /// <summary>
        /// 获得服务端转发的委托
        /// </summary>
        /// <param name="o"></param>
        void GotOrder(Order o)
        {
            debug("accountmoniter got order, accountselected:" + AccountSetlected.Account, QSEnumDebugLevel.INFO);
            if (IsCurrentAccount(o.Account) && Globals.TradingInfoTracker.IsReady(o.Account))
            {
                Globals.Debug("view control got order");
                ctOrderView1.GotOrder(o);
                ctPositionView1.GotOrder(o);
            }
        }

        /// <summary>
        /// 获得服务端转发的成交
        /// </summary>
        /// <param name="f"></param>
        void GotTrade(Trade f)
        {
            if (IsCurrentAccount(f.Account) && Globals.TradingInfoTracker.IsReady(f.Account))
            {
                ctTradeView1.GotFill(f);
                ctPositionView1.GotFill(f);
            }
        }

        /// <summary>
        /// 获得服务端的帐户信息
        /// </summary>
        /// <param name="account"></param>
        void GotAccount(IAccountLite account)
        {
            if (string.IsNullOrEmpty(account.Account))
                return;
            accountcache.Write(account);
            Globals.Debug(">>>>>>>>>>>>>>>>> ctaccountmontier got account");
        }

        /// <summary>
        /// 获得交易帐户的财务状态信息
        /// </summary>
        /// <param name="info"></param>
        void GotAccountInfoLite(IAccountInfoLite info)
        {
            accountinfocache.Write(info);
        }

        /// <summary>
        /// 响应帐户变动事件
        /// </summary>
        /// <param name="account"></param>
        public void GotAccountChanged(IAccountLite account)
        {
            accountcache.Write(account);
        }


        /// <summary>
        /// 获得恢复交易帐户交易数据的开始和结束回报
        /// </summary>
        /// <param name="response"></param>
        public void GotResumeResponse(RspMGRResumeAccountResponse response)
        {
            if (response.IsBegin)
            {
                debug("resume account:" + response.ResumeAccount + " start...", QSEnumDebugLevel.INFO);

                string account = response.ResumeAccount;
                IAccountLite acclit = null;
                if (accountmap.TryGetValue(account, out acclit))
                {

                    Globals.TradingInfoTracker.StartResume(acclit);
                }
                else
                {
                    debug("无法找到对应的帐户:" + account, QSEnumDebugLevel.WARNING);
                }
            }
            else
            {
                //数据恢复结束
                debug("resume account:" + response.ResumeAccount + " end...", QSEnumDebugLevel.INFO);
                Globals.TradingInfoTracker.EndResume();
                LoadAccountInfo(response.ResumeAccount);
            }

        }
        /// <summary>
        /// 获得客户端登入 退出状态更新
        /// </summary>
        /// <param name="notify"></param>
        public void GotSessionUpdate(NotifyMGRSessionUpdateNotify notify)
        {
            sessionupdatecache.Write(notify);
        }

    }
}
