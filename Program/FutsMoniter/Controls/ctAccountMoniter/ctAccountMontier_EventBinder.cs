using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using TradingLib.Mixins.LitJson;

namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier
    {

        public void OnInit()
        {
            //加载帐户
            foreach (IAccountLite account in Globals.LogicHandler.Accounts)
            {
                InvokeGotAccount(account);
            }

            //加载合约列表
            Globals.Debug("加载合约列表~~~~~~~~~~~~~~~~");
            foreach (Symbol sym in Globals.BasicInfoTracker.GetSymbolTradable())
            {
                Globals.Debug("symbol:" + sym.Symbol);
                viewQuoteList1.addSecurity(sym);
            }


            //帐户事件
            Globals.LogicEvent.GotAccountEvent += new Action<IAccountLite>(GotAccount);
            Globals.LogicEvent.GotFinanceInfoLiteEvent += new Action<IAccountInfoLite>(GotAccountInfoLite);
            Globals.LogicEvent.GotAccountChangedEvent += new Action<IAccountLite>(GotAccountChanged);
            Globals.LogicEvent.GotSessionUpdateEvent += new Action<NotifyMGRSessionUpdateNotify>(GotSessionUpdate);
            Globals.LogicEvent.GotResumeResponseEvent += new Action<RspMGRResumeAccountResponse>(GotResumeResponse);
            
            //交易事件
            Globals.LogicEvent.GotTickEvent += new TickDelegate(GotTick);
            Globals.LogicEvent.GotOrderEvent +=new OrderDelegate(GotOrder);
            Globals.LogicEvent.GotFillEvent += new FillDelegate(GotTrade);



            Globals.Debug("ctAccountMontier init called @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            if (!Globals.LoginResponse.Domain.Super)
            {
                if (!Globals.UIAccess.moniter_acctype)
                {
                    //lbcategory.Visible = false;
                    ctAccountType1.Visible = false;
                    //ctAccountType1.Visible = false;
                    accountgrid.Columns[CATEGORYSTR].Visible = false;
                }

                if (!Globals.UIAccess.moniter_router)
                {
                    ctRouterType1.Visible = false;
                    accountgrid.Columns[ROUTEIMG].Visible = false;
                }

                accountgrid.ContextMenuStrip.Items[0].Visible = Globals.UIAccess.moniter_menu_editaccount;
                accountgrid.ContextMenuStrip.Items[1].Visible = Globals.UIAccess.moniter_menu_changepass;
                accountgrid.ContextMenuStrip.Items[2].Visible = Globals.UIAccess.moniter_menu_changeinvestor;
                accountgrid.ContextMenuStrip.Items[3].Visible = Globals.UIAccess.moniter_menu_queryhist;
                accountgrid.ContextMenuStrip.Items[4].Visible = Globals.UIAccess.moniter_menu_delaccount;

                funpagePlaceOrder.Visible = Globals.UIAccess.fun_tab_placeorder;
                funpageFinservice.Visible = Globals.UIAccess.fun_tab_finservice;
                funpageFinanceInfo.Visible = Globals.UIAccess.fun_tab_financeinfo;

                ctOrderView1.EnableOperation = Globals.UIAccess.fun_info_operation;
                ctPositionView1.EnableOperation = Globals.UIAccess.fun_info_operation;

                //模块限制
                funpageFinservice.Visible = Globals.LoginResponse.Domain.Module_FinService;
            }

            //Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryAccountInfo", this.OnQryAccountInfo);
        }

        public void OnDisposed()
        {
            //Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryAccountInfo", this.OnQryAccountInfo);
        }


        
    }
}
