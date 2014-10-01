﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class AccountConfigForm
    {
        


        /// <summary>
        /// 委托风控项
        /// </summary>
        Dictionary<int, RuleItem> ruleitemmap = new Dictionary<int, RuleItem>();

        delegate void RuleItemDel(RuleItem item, bool islast);

        void InvokeGotOrderRuleItemDel(RuleItem item, bool islast)
        {
            if (InvokeRequired)
            {
                Invoke(new RuleItemDel(InvokeGotOrderRuleItemDel), new object[] { item, islast });
            }
            else
            {
                debug("remote ruleitem,id:" + item.ID + " rule desp:" + item.RuleDescription);
                if (ruleitemmap.Keys.Contains(item.ID))
                {
                    ruleitemmap.Remove(item.ID);
                }
                if (islast)
                {
                    Factory.IDataSourceFactory(orderRuleItemList).BindDataSource(this.GetOrderRuleItemList());
                }
            }
        }
        void InvokeGotOrderRuleItem(RuleItem item, bool islast)
        {
            if (InvokeRequired)
            {
                Invoke(new RuleItemDel(InvokeGotOrderRuleItem), new object[] { item, islast });
            }
            else
            {
                if (item.ID == 0 || string.IsNullOrEmpty(item.Account))
                    return;
                if (ruleitemmap.Keys.Contains(item.ID))
                {
                    RuleItem target = ruleitemmap[item.ID];
                    target.Account = item.Account;
                    target.Compare = item.Compare;
                    target.Enable = item.Enable;
                    target.RuleDescription = item.RuleDescription;
                    target.RuleName = item.RuleName;
                    target.RuleType = item.RuleType;
                    target.SymbolSet = item.SymbolSet;
                    target.Value = item.SymbolSet;

                }
                else
                {
                    ruleitemmap.Add(item.ID, item);
                }
                if (islast)//当最后一个回报时刷新数据
                {
                    Factory.IDataSourceFactory(orderRuleItemList).BindDataSource(this.GetOrderRuleItemList());
                }
            }
        }

        /// <summary>
        /// 交易帐户风控项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArrayList GetOrderRuleItemList()
        {
            ArrayList list = new ArrayList();
            foreach (RuleItem item in ruleitemmap.Values)
            {
                ValueObject<RuleItem> vo = new ValueObject<RuleItem>();
                vo.Name = item.RuleDescription;
                vo.Value = item;
                list.Add(vo);
            }
            return list;
        }


        /// <summary>
        /// 添加委托风控项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddOrderRule_Click(object sender, EventArgs e)
        {
            //orderRuleClassList.selected
            if (orderRuleClassList.SelectedItems.Count > 0)
            {
                RuleSetConfig fm = new RuleSetConfig();
                fm.Account = _account;
                fm.RuleClass = (RuleClassItem)orderRuleClassList.SelectedValue;
                fm.Show();
            }
        }
        /// <summary>
        /// 修改委托风控项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orderRuleItemList_DoubleClick(object sender, EventArgs e)
        {
            if (orderRuleItemList.SelectedItems.Count > 0)
            {
                RuleSetConfig fm = new RuleSetConfig();
                fm.Account = _account;
                fm.Rule = (RuleItem)orderRuleItemList.SelectedValue;
                fm.Show();

            }
        }
        /// <summary>
        /// 删除委托风控项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelOrderRule_Click(object sender, EventArgs e)
        {
            if (orderRuleItemList.SelectedItems.Count > 0)
            {
                RuleItem item = (RuleItem)orderRuleItemList.SelectedValue;
                if (fmConfirm.Show("确认删除风控项:" + item.RuleDescription) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqDelRuleItem(item);
                }
                orderRuleItemList.SelectedItem = null;

            }
        }
    }
}
