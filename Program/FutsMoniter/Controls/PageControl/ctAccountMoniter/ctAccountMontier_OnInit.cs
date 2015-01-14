using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using TradingLib.Mixins.Json;

namespace FutsMoniter
{
    public partial class ctAccountMontier
    {

        public void OnInit()
        {
            //加载帐户
            foreach (AccountLite account in Globals.LogicHandler.Accounts)
            {
                InvokeGotAccount(account);
            }
            //更新帐户数目
            UpdateAccountNum();

            //帐户事件
            Globals.LogicEvent.GotAccountEvent += new Action<AccountLite>(GotAccount);
            Globals.LogicEvent.GotFinanceInfoLiteEvent += new Action<AccountInfoLite>(GotAccountInfoLite);
            Globals.LogicEvent.GotAccountChangedEvent += new Action<AccountLite>(GotAccountChanged);
            Globals.LogicEvent.GotSessionUpdateEvent += new Action<NotifyMGRSessionUpdateNotify>(GotSessionUpdate);
            

            //if (!Globals.Domain.Super)
            {
                //只有管理员可以查看路由类别
                accountgrid.Columns[ROUTEIMG].Visible = Globals.Manager.IsRoot();
                //管理员可以查看帐户类别
                accountgrid.Columns[CATEGORYSTR].Visible = Globals.Manager.IsRoot();

                //如果有实盘交易权限则可以查看路由组
                accountgrid.Columns[ROUTERGROUPSTR].Visible = Globals.Domain.Router_Live && Globals.Manager.IsRoot();

                //只有管理员可以修改路由组和删除交易帐户
                if (!Globals.Manager.IsRoot())
                {
                    accountgrid.ContextMenuStrip.Items[4].Visible = false;

                    accountgrid.ContextMenuStrip.Items[8].Visible = false;
                    accountgrid.ContextMenuStrip.Items[9].Visible = false;
                    accountgrid.ContextMenuStrip.Items[10].Visible = false;
                    accountgrid.ContextMenuStrip.Items[11].Visible = false;
                }

            }

            //启动更新线程
            StartUpdate();
        }

        public void OnDisposed()
        {
            //帐户事件
            Globals.LogicEvent.GotAccountEvent -= new Action<AccountLite>(GotAccount);
            Globals.LogicEvent.GotFinanceInfoLiteEvent -= new Action<AccountInfoLite>(GotAccountInfoLite);
            Globals.LogicEvent.GotAccountChangedEvent -= new Action<AccountLite>(GotAccountChanged);
            Globals.LogicEvent.GotSessionUpdateEvent -= new Action<NotifyMGRSessionUpdateNotify>(GotSessionUpdate);
        }
    }
}
