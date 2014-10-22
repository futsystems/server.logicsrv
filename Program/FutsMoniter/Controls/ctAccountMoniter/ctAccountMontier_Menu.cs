using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Windows.Forms;
using FutSystems.GUI;



namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier
    {
        void InitMenu()
        {
            accountgrid.ContextMenuStrip = new ContextMenuStrip();
            accountgrid.ContextMenuStrip.Items.Add("编辑账户", Properties.Resources.editAccount, new EventHandler(EditAccount_Click));
            accountgrid.ContextMenuStrip.Items.Add("修改密码",null, new EventHandler(ChangePass_Click));
            accountgrid.ContextMenuStrip.Items.Add("修改信息", null, new EventHandler(ChangeInvestor_Click));
            accountgrid.ContextMenuStrip.Items.Add("历史查询", null, new EventHandler(QryHist_Click));


        }
        /// <summary>
        /// 编辑某个交易帐号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EditAccount_Click(object sender, EventArgs e)
        {
            IAccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                fmaccountconfig.Account = account;
                fmaccountconfig.Show();//.ShowDialog();
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
            IAccountLite account = GetVisibleAccount(CurrentAccount);
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
            IAccountLite account = GetVisibleAccount(CurrentAccount);
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
            IAccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                if (QryAccountHistEvent != null)
                    QryAccountHistEvent(account);

            }
            else
            {
                fmConfirm.Show("请选择需要查询的交易帐户！");
            }
        }
    }
}
