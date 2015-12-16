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
            

            if(Globals.TLClient.ServerVersion.ProductType == QSEnumProductType.CounterSystem)
            {
                //只有管理员可以查看路由类别
                accountgrid.Columns[ROUTEIMG].Visible = Globals.Manager.IsRoot();
                //管理员可以查看帐户类别
                accountgrid.Columns[CATEGORYSTR].Visible = Globals.Manager.IsRoot();

                //如果有实盘交易权限则可以查看路由组
                accountgrid.Columns[ROUTERGROUPSTR].Visible = Globals.Domain.Router_Live && Globals.Manager.IsRoot();


                accountgrid.Columns[MAINACCOUNT].Visible = false;


                //只有管理员可以修改路由组和删除交易帐户
                if (!Globals.Manager.IsRoot())
                {
                    accountgrid.ContextMenuStrip.Items[4].Visible = false;

                    accountgrid.ContextMenuStrip.Items[8].Visible = false;
                    accountgrid.ContextMenuStrip.Items[9].Visible = false;
                    accountgrid.ContextMenuStrip.Items[11].Visible = false;
                    accountgrid.ContextMenuStrip.Items[12].Visible = false;
                }
                accountgrid.ContextMenuStrip.Items[10].Visible = false;


            }
            if (Globals.TLClient.ServerVersion.ProductType == QSEnumProductType.VendorMoniter)
            {
                //隐藏帐户过滤窗口以及其他过滤控件
                btnAcctFilter.Visible = false;
                accLogin.Visible = false;
                acchodpos.Visible = false;
                btnAddAccount.Text = "添加配资客户";
                btnAddAccount.Width = 100;
                btnAddAccount.Location = new System.Drawing.Point(btnAddAccount.Location.X - 40, btnAddAccount.Location.Y);

                accountgrid.Columns[ROUTEIMG].Visible = false;
                accountgrid.Columns[LOGINSTATUSIMG].Visible = false;
                accountgrid.Columns[ADDRESS].Visible = false;
                accountgrid.Columns[CATEGORYSTR].Visible = false;
                accountgrid.Columns[ROUTERGROUPSTR].Visible = false;
                accountgrid.Columns[HOLDSIZE].Visible = false;
                accountgrid.Columns[AGENTCODE].Visible = false;

                //隐藏右键菜单
                accountgrid.ContextMenuStrip.Items[1].Visible = false;

                accountgrid.ContextMenuStrip.Items[3].Visible = false;
                accountgrid.ContextMenuStrip.Items[4].Visible = false;
                accountgrid.ContextMenuStrip.Items[5].Visible = false;
                accountgrid.ContextMenuStrip.Items[6].Visible = false;

                accountgrid.ContextMenuStrip.Items[8].Visible = false;


                //accountgrid.ContextMenuStrip.Items[11].Visible = false;
                //调整宽度
                VendorMoniterWidth();
            }

            //根据产品类别来调整界面

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
