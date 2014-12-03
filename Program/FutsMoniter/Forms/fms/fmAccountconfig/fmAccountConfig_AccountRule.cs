using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class fmAccountConfig
    {
        Dictionary<int, RuleItem> accountrulemap = new Dictionary<int, RuleItem>();

        void InvokeGotAccountRuleItemDel(RuleItem item, bool islast)
        {
            if (InvokeRequired)
            {
                Invoke(new RuleItemDel(InvokeGotAccountRuleItemDel), new object[] { item, islast });
            }
            else
            {
                debug("remote ruleitem,id:" + item.ID + " rule desp:" + item.RuleDescription);
                if (accountrulemap.Keys.Contains(item.ID))
                {
                    accountrulemap.Remove(item.ID);
                }
                if (islast)
                {
                    Factory.IDataSourceFactory(accountRuleItemList).BindDataSource(this.GetAccountRuleItemList());
                }
            }
        }
        void InvokeGotAccountRuleItem(RuleItem item, bool islast)
        {
            if (InvokeRequired)
            {
                Invoke(new RuleItemDel(InvokeGotAccountRuleItem), new object[] { item, islast });
            }
            else
            {
                if (item.ID == 0 || string.IsNullOrEmpty(item.Account))
                    return;
                if (accountrulemap.Keys.Contains(item.ID))
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
                    accountrulemap.Add(item.ID, item);
                }
                if (islast)//当最后一个回报时刷新数据
                {
                    Factory.IDataSourceFactory(accountRuleItemList).BindDataSource(this.GetAccountRuleItemList());
                }
            }
        }
        /// <summary>
        /// 交易帐户风控项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArrayList GetAccountRuleItemList()
        {
            ArrayList list = new ArrayList();
            foreach (RuleItem item in accountrulemap.Values)
            {
                ValueObject<RuleItem> vo = new ValueObject<RuleItem>();
                vo.Name = item.RuleDescription;
                vo.Value = item;
                list.Add(vo);
            }
            return list;
        }


        /// <summary>
        /// 添加帐户风控项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAccountRule_Click(object sender, EventArgs e)
        {
            if (accountRuleClassList.SelectedItems.Count > 0)
            {
                fmRuleSetConfig fm = new fmRuleSetConfig();
                fm.Account = _account;
                fm.RuleClass = (RuleClassItem)accountRuleClassList.SelectedValue;
                fm.Show();
            }
        }

        /// <summary>
        /// 修改帐户风控项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountRuleItemList_DoubleClick(object sender, EventArgs e)
        {

            if (accountRuleItemList.SelectedItems.Count > 0)
            {
                fmRuleSetConfig fm = new fmRuleSetConfig();
                fm.Account = _account;
                fm.Rule = (RuleItem)accountRuleItemList.SelectedValue;
                fm.Show();

            }
        }

        /// <summary>
        /// 删除帐户风控项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelAccountRule_Click(object sender, EventArgs e)
        {
            if (accountRuleItemList.SelectedItems.Count > 0)
            {
                RuleItem item = (RuleItem)accountRuleItemList.SelectedValue;
                if (fmConfirm.Show("确认删除风控项:" + item.RuleDescription) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqDelRuleItem(item);
                }
                accountRuleItemList.SelectedItem = null;

            }
        }
    }
}
