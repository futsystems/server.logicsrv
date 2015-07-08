﻿using System;
using System.Collections.Generic;
using System.Collections;
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
            //绑定事件
            Globals.LogicEvent.GotRuleItemEvent += new Action<RuleItem, bool>(OnRuleItem);//查询风控规则回报
            Globals.LogicEvent.GotRuleItemUpdateEvemt += new Action<RuleItem>(OnRuleItemUpdate);//风控规则更新
            Globals.LogicEvent.GotRuleItemDeleteEvent += new Action<RuleItem>(OnRuleItemDel);//风控规则删除

            Globals.LogicEvent.GotAccountChangedEvent += new Action<AccountLite>(OnAccountChanged);//帐户更新

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryCommissionTemplate", this.OnQryCommissionTemplate);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryMarginTemplate", this.OnQryMarginTemplate);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryExStrategyTemplate", this.OnQryExStrategyTemplate);




            if (!Globals.Domain.Super)
            {
                //ctRouterType1.Visible = Globals.Manager.IsRoot();//管理员可以设置帐户路由类别
                if (Globals.Manager.IsRoot())
                {
                    ctRouterType1.Visible = Globals.Domain.Router_Live && Globals.Domain.Router_Sim && Globals.Domain.Switch_Router;
                }
                else
                {
                    ctRouterType1.Visible = false;
                    intraday.Visible = false;
                }

                
                btnExecute.Visible = Globals.Manager.IsRoot() || Globals.UIAccess.r_block;

                //管理员可以进行财务操作
                pageFinance.Visible = Globals.Manager.IsRoot();
                pageMarginCommission.Visible = Globals.Manager.IsRoot();
            }

            //执行延迟加载 只有当延迟加载的空间加载完毕后才可以将数据显示到界面否则相关字段显示错误
            UpdateAccountSetting();
        }

        void OnQryCommissionTemplate(string json)
        {
            CommissionTemplateSetting[] list = MoniterUtils.ParseJsonResponse<CommissionTemplateSetting[]>(json);
            if (list != null)
            {
                Factory.IDataSourceFactory(cbCommissionTemplate).BindDataSource(GetCommissionTemplateCBList(list));
                cbCommissionTemplate.SelectedValue = _account.Commissin_ID;
            }
        }

        void OnQryMarginTemplate(string json)
        {
            MarginTemplateSetting[] list = MoniterUtils.ParseJsonResponse<MarginTemplateSetting[]>(json);
            if (list != null)
            {
                Factory.IDataSourceFactory(cbMarginTemplate).BindDataSource(GetMarginTemplateCBList(list));
                cbMarginTemplate.SelectedValue = _account.Margin_ID;
            }
        }

        void OnQryExStrategyTemplate(string json)
        {
            ExStrategyTemplateSetting[] list = MoniterUtils.ParseJsonResponse<ExStrategyTemplateSetting[]>(json);
            if (list != null)
            {
                Factory.IDataSourceFactory(cbExStrategyTemplate).BindDataSource(GetExStrategyTemplateCBList(list));
                cbExStrategyTemplate.SelectedValue = _account.ExStrategy_ID;
            }
        }

        public static ArrayList GetExStrategyTemplateCBList(ExStrategyTemplateSetting[] items)
        {
            ArrayList list = new ArrayList();
            ValueObject<int> vo1 = new ValueObject<int>();
            vo1.Name = "系统默认";
            vo1.Value = 0;
            list.Add(vo1);

            foreach (ExStrategyTemplateSetting item in items)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = item.Name;
                vo.Value = item.ID;
                list.Add(vo);
            }
            return list;
        }
        public static ArrayList GetMarginTemplateCBList(MarginTemplateSetting[] items)
        {
            ArrayList list = new ArrayList();
            ValueObject<int> vo1 = new ValueObject<int>();
            vo1.Name = "系统默认";
            vo1.Value = 0;
            list.Add(vo1);

            foreach (MarginTemplateSetting item in items)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = item.Name;
                vo.Value = item.ID;
                list.Add(vo);
            }
            return list;
        }
        public static ArrayList GetCommissionTemplateCBList(CommissionTemplateSetting[] items)
        {
            ArrayList list = new ArrayList();
            ValueObject<int> vo1 = new ValueObject<int>();
            vo1.Name = "系统默认";
            vo1.Value =0;
            list.Add(vo1);

            foreach (CommissionTemplateSetting item in items)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = item.Name;
                vo.Value = item.ID;
                list.Add(vo);
            }
            return list;
        }


        void UpdateAccountSetting()
        {
            if (_account != null)
            {
                this.Text = "交易帐户编辑[" + _account.Account + "]";
                intraday.Checked = _account.IntraDay;
                //poslock.Checked = _account.PosLock;
                //sidemargin.Checked = _account.SideMargin;
                ctRouterType1.RouterType = _account.OrderRouteType;
                //cbCreditSeparate.Checked = _account.CreditSeparate;

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