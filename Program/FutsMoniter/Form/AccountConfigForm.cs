using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class AccountConfigForm : Telerik.WinControls.UI.RadForm
    {
        IAccountLite _account;
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);

            
        }
        public IAccountLite Account { get { return _account; } 
            set 
            { 
                _account = value;
                if (_account != null)
                {
                    this.Text = "交易帐户编辑[" + _account.Account + "]";
                    intraday.Checked = _account.IntraDay;
                    intraday.Text = _account.IntraDay ? "日内" : "隔夜";
                    accountType.SelectedValue = _account.Category;
                    routeType.SelectedValue = _account.OrderRouteType;


                    btnExecute.Text = _account.Execute ? "冻 结" : "激 活";
                    btnExecute.ForeColor = Color.Red;


                    poslock.Checked = _account.PosLock;
                    poslock.Text = _account.PosLock ? "允许" : "禁止";

                    
                }
            } 
        }
        public AccountConfigForm()
        {
            InitializeComponent();


            Factory.IDataSourceFactory(accountType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumAccountCategory>());
            Factory.IDataSourceFactory(routeType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumOrderTransferType>());
            cashop_type.Items.Add("入金");
            cashop_type.Items.Add("出金");

            
        }

        

        private void AccountConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void editpageview_PageIndexChanged(object sender, Telerik.WinControls.UI.RadPageViewIndexChangedEventArgs e)
        {

            debug("page index changed:" + e.Page.Name);
        }

        private void btnUpdateAccountInfo_Click(object sender, EventArgs e)
        {
            Globals.TLClient.ReqQryAccountInfo(_account.Account);
        }




        /// <summary>
        /// 当帐户有变化时 更新修改窗体
        /// </summary>
        /// <param name="account"></param>
        public void GotAccountChanged(IAccountLite account)
        {
            if (Account.Account.Equals(account.Account))
            {
                this.Account = account;
            }
        }
        private void btnCashOperation_Click(object sender, EventArgs e)
        {
            decimal amount = cashop_amount.Value;
            string cashopref = cashop_ref.Text;
            string comment = cashop_ref.Text;
            int cashopvalue = cashop_type.SelectedIndex;
            string cashoptitle = string.Empty;
            decimal amount2=0;
            if (cashopvalue == -1)
            {
                fmConfirm.Show("请选择出入金类型");
                return;
            }
            if (cashopvalue == 0)
            {
                cashoptitle = "入金";
                amount2 = Math.Abs(amount);
            }
            else if (cashopvalue == 1)
            {
                cashoptitle = "出金";
                amount2 = -1* Math.Abs(amount);
            }
            if (amount == 0)
            {
                fmConfirm.Show("请输入出入金金额");
                return;
            }
            if (fmConfirm.Show("确认向帐户[" + _account.Account + "] " + cashoptitle + " " + amount.ToString() + " 流水号:" + cashopref) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqCashOperation(_account.Account, amount2, cashopref, comment);
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认更新帐户属性?") == System.Windows.Forms.DialogResult.Yes)
            {
                if (intraday.Checked != _account.IntraDay)
                {
                    Globals.TLClient.ReqUpdateAccountIntraday(_account.Account, intraday.Checked);
                }
                if ((QSEnumAccountCategory)accountType.SelectedValue != _account.Category)
                {
                    Globals.TLClient.ReqUpdateAccountCategory(_account.Account, (QSEnumAccountCategory)accountType.SelectedValue);
                }
                if ((QSEnumOrderTransferType)routeType.SelectedValue != _account.OrderRouteType)
                {
                    Globals.TLClient.ReqUpdateRouteType(_account.Account, (QSEnumOrderTransferType)routeType.SelectedValue);
                }
                if (poslock.Checked != _account.PosLock)
                {
                    Globals.TLClient.ReqUpdaetAccountPosLock(_account.Account, poslock.Checked);
                }
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认[" + (_account.Execute ? "冻结" : "激活") + "]交易帐户 " + _account.Account) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqUpdateAccountExecute(_account.Account, !_account.Execute);

            }
        }


        


        



        /// <summary>
        /// tab页切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editpageview_SelectedPageChanged(object sender, EventArgs e)
        {
            
            if (editpageview.SelectedPage.Name.Equals("pageOrderCheck"))
            {
                Factory.IDataSourceFactory(orderRuleClassList).BindDataSource(Globals.BasicInfoTracker.GetOrderRuleClassListItems());
                Globals.TLClient.ReqQryRuleItem(_account.Account, QSEnumRuleType.OrderRule);
            }
            else if (editpageview.SelectedPage.Name.Equals("pageAccountCheck"))
            {
                Factory.IDataSourceFactory(accountRuleClassList).BindDataSource(Globals.BasicInfoTracker.GetAccountRuleClassListItems());
                Globals.TLClient.ReqQryRuleItem(_account.Account, QSEnumRuleType.AccountRule);
            }
            else if (editpageview.SelectedPage.Name.Equals("pageFinance"))
            {
                Globals.TLClient.ReqQryAccountInfo(_account.Account);
            }

            

        }

        private void poslock_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {

        }

       

        

       
    }
}
