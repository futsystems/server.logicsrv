using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Windows.Forms;
using FutSystems.GUI;



namespace FutsMoniter
{
    public partial class ctAccountMontier
    {
        void InitMenu()
        {
            accountgrid.ContextMenuStrip = new ContextMenuStrip();
            accountgrid.ContextMenuStrip.Items.Add("编辑账户", Properties.Resources.editAccount, new EventHandler(EditAccount_Click));
            accountgrid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            accountgrid.ContextMenuStrip.Items.Add("修改密码",null, new EventHandler(ChangePass_Click));
            accountgrid.ContextMenuStrip.Items.Add("修改信息", null, new EventHandler(ChangeInvestor_Click));
            accountgrid.ContextMenuStrip.Items.Add("查询密码", null, new EventHandler(QryLoginInfo_Click));
            accountgrid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            accountgrid.ContextMenuStrip.Items.Add("交易记录查询", null, new EventHandler(QryHist_Click));
            accountgrid.ContextMenuStrip.Items.Add("结算单查询", null, new EventHandler(QrySettlement_Click));
            accountgrid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            accountgrid.ContextMenuStrip.Items.Add("修改路由组", Properties.Resources.changerouter, new EventHandler(UpdateRouterGroup_Click));
            accountgrid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            accountgrid.ContextMenuStrip.Items.Add("删除帐户", Properties.Resources.deleteaccount, new EventHandler(DelAccount_Click));
        }

        void QryLoginInfo_Click(object sender, EventArgs e)
        {
            AccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                fmLoginInfo fm = new fmLoginInfo();
                fm.SetAccount(account);
                fm.ShowDialog();
            }
            else
            {
                MoniterUtils.WindowMessage("请选择需要查询的交易帐户");
            }
        }
        void UpdateRouterGroup_Click(object sender, EventArgs e)
        {
            AccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                fmChangeRouter fm = new fmChangeRouter();
                fm.SetAccount(account);
                fm.Show();
            }
            else
            {
                fmConfirm.Show("请选择需要编辑的交易帐户！");
            }
        }
        /// <summary>
        /// 删除某个交易帐号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DelAccount_Click(object sender, EventArgs e)
        {
            AccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                if (fmConfirm.Show("确认删除交易帐户?") == DialogResult.Yes)
                {
                    Globals.TLClient.ReqDelAccount(account.Account);
                    account.Deleted = true;//修改删除标识
                    GotAccount(account);//更新界面数据
                    RefreshAccountQuery();//刷新表格

                }
                else
                {
                    return;
                }
            }
            else
            {
                fmConfirm.Show("请选择需要编辑的交易帐户！");
            }
        }


        /// <summary>
        /// 编辑某个交易帐号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EditAccount_Click(object sender, EventArgs e)
        {
            AccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                fmAccountConfig fm = new fmAccountConfig();
                fm.SetAccount(account);
                fm.Show();//.ShowDialog();
            }
            else
            {
                fmConfirm.Show("请选择需要编辑的交易帐户！");
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangePass_Click(object sender, EventArgs e)
        {
            AccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                fmChangePassword fm = new fmChangePassword();
                fm.SetAccount(account.Account);
                fm.ShowDialog();

            }
            else
            {
                fmConfirm.Show("请选择需要编辑的交易帐户！");
            }
        }

        /// <summary>
        /// 修改投资者信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangeInvestor_Click(object sender, EventArgs e)
        {
            AccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                fmChangeInvestor fm = new fmChangeInvestor();
                fm.SetAccount(account);
                fm.ShowDialog();

            }
            else
            {
                fmConfirm.Show("请选择需要编辑的交易帐户！");
            }
        }

        /// <summary>
        /// 查询历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void QryHist_Click(object sender, EventArgs e)
        {
            AccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                //if (QryAccountHistEvent != null)
                //    QryAccountHistEvent(account);
                fmHistQuery fm = new fmHistQuery();
                fm.SetAccount(account.Account);
                fm.Show();

            }
            else
            {
                fmConfirm.Show("请选择需要查询的交易帐户！");
            }
        }

        /// <summary>
        /// 添加交易帐户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            fmAddAccount fm = new fmAddAccount();
            fm.TopMost = true;
            fm.ShowDialog();
        }


        void QrySettlement_Click(object sender, EventArgs e)
        {
            AccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                fmSettlement fm = new fmSettlement();
                fm.SetAccount(account.Account);
                fm.Show();

            }
            else
            {
                fmConfirm.Show("请选择需要查询的交易帐户！");
            }
        }

        DateTime _lastresumetime = DateTime.Now;
        private void accountgrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Globals.TradingInfoTracker.IsInResume)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("交易记录恢复中,请稍候!");
                return;
            }

            if (DateTime.Now.Subtract(_lastresumetime).TotalSeconds <= 1)
            {
                ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请不要频繁请求帐户日内数据");
                return;
            }
            _lastresumetime = DateTime.Now;
            string account = CurrentAccount;
            AccountLite accountlite = null;

            if (accountmap.TryGetValue(account, out accountlite))
            {
                //设定当前选中帐号
                accountselected = accountlite;
                //更新选中lable
                lbCurrentAccount.Text = accountlite.Account;

                //触发事件中继的帐户选择事件
                Globals.LogicHandler.OnAccountSelected(accountlite);
            }
        }

    }
}
