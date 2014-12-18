using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using System.Windows.Forms;

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

            //执行延迟加载 只有当延迟加载的空间加载完毕后才可以将数据显示到界面否则相关字段显示错误
            if (_account != null)
            {
                this.Text = "交易帐户编辑[" + _account.Account + "]";
                intraday.Checked = _account.IntraDay;
                intraday.Text = _account.IntraDay ? "日内" : "隔夜";

                ctRouterType1.RouterType = _account.OrderRouteType;
                btnExecute.Text = _account.Execute ? "冻 结" : "激 活";

                poslock.Checked = _account.PosLock;
                poslock.Text = _account.PosLock ? "允许" : "禁止";
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
