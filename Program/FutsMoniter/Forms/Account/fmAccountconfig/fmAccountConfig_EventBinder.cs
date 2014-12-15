using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class fmAccountConfig
    {
        public void OnInit()
        {
            Globals.Debug("fmAccountConfig init called @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            
            //绑定事件
            Globals.LogicEvent.GotRuleItemEvent += new Action<RuleItem, bool>(OnRuleItem);//查询风控规则回报
            Globals.LogicEvent.GotRuleItemUpdateEvemt += new Action<RuleItem>(OnRuleItemUpdate);//风控规则更新
            Globals.LogicEvent.GotRuleItemDeleteEvent += new Action<RuleItem>(OnRuleItemDel);//风控规则删除

            Globals.LogicEvent.GotAccountChangedEvent += new Action<IAccountLite>(OnAccountChanged);//帐户更新


            if (!Globals.LoginResponse.Domain.Super)
            {
                pageConfig.Visible = Globals.UIAccess.moniter_tab_config;
                btnExecute.Visible = Globals.UIAccess.moniter_tab_config_inactive;

                pageFinance.Visible = Globals.UIAccess.moniter_tab_finance;
                pageOrderCheck.Visible = Globals.UIAccess.moniter_tab_orderrule;
                pageAccountCheck.Visible = Globals.UIAccess.moniter_tab_accountrule;
                pageMarginCommission.Visible = Globals.UIAccess.moniter_tab_margincommissoin;

                //ctAccountType1.Visible= Globals.UIAccess.moniter_acctype;
                ctRouterType1.Visible = Globals.UIAccess.moniter_router;
            }
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.GotRuleItemEvent -= new Action<RuleItem, bool>(OnRuleItem);//查询风控规则回报
            Globals.LogicEvent.GotRuleItemUpdateEvemt -= new Action<RuleItem>(OnRuleItemUpdate);//风控规则更新
            Globals.LogicEvent.GotRuleItemDeleteEvent -= new Action<RuleItem>(OnRuleItemDel);//风控规则删除

            Globals.LogicEvent.GotAccountChangedEvent -= new Action<IAccountLite>(OnAccountChanged);//帐户更新

        }
        
    }
}
