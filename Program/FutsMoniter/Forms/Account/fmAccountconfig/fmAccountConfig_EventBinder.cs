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
           // Globals.Debug("fmAccountConfig init called @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            
            //绑定事件
            Globals.LogicEvent.GotRuleItemEvent += new Action<RuleItem, bool>(OnRuleItem);//查询风控规则回报
            Globals.LogicEvent.GotRuleItemUpdateEvemt += new Action<RuleItem>(OnRuleItemUpdate);//风控规则更新
            Globals.LogicEvent.GotRuleItemDeleteEvent += new Action<RuleItem>(OnRuleItemDel);//风控规则删除

            Globals.LogicEvent.GotAccountChangedEvent += new Action<AccountLite>(OnAccountChanged);//帐户更新

            if (!Globals.Domain.Super)
            {
                ctRouterType1.Visible = Globals.Manager.IsRoot();//管理员可以设置帐户路由类别

                //pageConfig.Visible = Globals.UIAccess.moniter_tab_config;
                btnExecute.Visible = Globals.Manager.IsRoot() || Globals.UIAccess.r_block;

                //管理员可以进行财务操作
                pageFinance.Visible = Globals.Manager.IsRoot();
                //pageOrderCheck.Visible = Globals.UIAccess.moniter_tab_orderrule;
                //pageAccountCheck.Visible = Globals.UIAccess.moniter_tab_accountrule;

                pageMarginCommission.Visible = false;
                
            }

            //执行延迟加载 只有当延迟加载的空间加载完毕后才可以将数据显示到界面否则相关字段显示错误
            UpdateAccountSetting();
        }

        void UpdateAccountSetting()
        {
            if (_account != null)
            {
                this.Text = "交易帐户编辑[" + _account.Account + "]";
                intraday.Checked = _account.IntraDay;
                poslock.Checked = _account.PosLock;

                ctRouterType1.RouterType = _account.OrderRouteType;

                btnExecute.Text = _account.Execute ? "冻 结" : "激 活";
                btnExecute.StateCommon.Content.ShortText.Color1 = !_account.Execute ? UIGlobals.ShortSideColor : UIGlobals.LongSideColor;
                
            }
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.GotRuleItemEvent -= new Action<RuleItem, bool>(OnRuleItem);//查询风控规则回报
            Globals.LogicEvent.GotRuleItemUpdateEvemt -= new Action<RuleItem>(OnRuleItemUpdate);//风控规则更新
            Globals.LogicEvent.GotRuleItemDeleteEvent -= new Action<RuleItem>(OnRuleItemDel);//风控规则删除

            Globals.LogicEvent.GotAccountChangedEvent -= new Action<AccountLite>(OnAccountChanged);//帐户更新

        }
        
    }
}
