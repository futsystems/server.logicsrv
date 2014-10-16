using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using FutSystems.GUI;
using Telerik.WinControls;
using Telerik.WinControls.UI;


namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier
    {
        RadContextMenu menu = new RadContextMenu();
        void InitMenu()
        {
            Telerik.WinControls.UI.RadMenuItem MenuItem_edit = new Telerik.WinControls.UI.RadMenuItem("编辑");
            MenuItem_edit.Image = Properties.Resources.editAccount_16;
            MenuItem_edit.Click += new EventHandler(EditAccount_Click);

            //Telerik.WinControls.UI.RadMenuItem MenuItem_add = new Telerik.WinControls.UI.RadMenuItem("添加");
            //MenuItem_add.Image = Properties.Resources.addAccount_16;
            //MenuItem_add.Click += new EventHandler(AddAccount_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_changepass = new Telerik.WinControls.UI.RadMenuItem("修改密码");
            //MenuItem_changepass.Image = Properties.Resources.addAccount_16;
            MenuItem_changepass.Click += new EventHandler(ChangePass_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_changeinvestor = new Telerik.WinControls.UI.RadMenuItem("修改信息");
            //MenuItem_changepass.Image = Properties.Resources.addAccount_16;
            MenuItem_changeinvestor.Click += new EventHandler(ChangeInvestor_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_qryhist = new Telerik.WinControls.UI.RadMenuItem("历史记录");
            //MenuItem_changepass.Image = Properties.Resources.addAccount_16;
            MenuItem_qryhist.Click += new EventHandler(QryHist_Click);

            //Telerik.WinControls.UI.RadMenuItem MenuItem_inserttrade = new Telerik.WinControls.UI.RadMenuItem("插入成交");
            ////MenuItem_changepass.Image = Properties.Resources.addAccount_16;
            //MenuItem_inserttrade.Click += new EventHandler(InsertTrade_Click);

            menu.Items.Add(MenuItem_edit);
            //menu.Items.Add(MenuItem_add);
            menu.Items.Add(MenuItem_changepass);
            menu.Items.Add(MenuItem_changeinvestor);
            menu.Items.Add(MenuItem_qryhist);

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
                ChangePassForm fm = new ChangePassForm();
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
                ChangeInvestorForm fm = new ChangeInvestorForm();

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
