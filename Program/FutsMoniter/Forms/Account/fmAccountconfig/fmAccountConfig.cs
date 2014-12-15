using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class fmAccountConfig : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
       IAccountLite _account;
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);

            
        }

        public void SetAccount(IAccountLite account)
        {
            _account = account;
            if (_account != null)
            {
                this.Text = "交易帐户编辑[" + _account.Account + "]";
                intraday.Checked = _account.IntraDay;
                intraday.Text = _account.IntraDay ? "日内" : "隔夜";
                //ctAccountType1.AccountType = _account.Category;
                ctRouterType1.RouterType = _account.OrderRouteType;

                btnExecute.Text = _account.Execute ? "冻 结" : "激 活";
                btnExecute.ForeColor = Color.Red;


                poslock.Checked = _account.PosLock;
                poslock.Text = _account.PosLock ? "允许" : "禁止";

                ctFinanceInfo1.SetAccount(_account);
            }
        }


        public fmAccountConfig()
        {
            InitializeComponent();

            cashop_type.Items.Add("入金");
            cashop_type.Items.Add("出金");

            this.Load += new EventHandler(fmAccountConfig_Load);
            
        }

        void fmAccountConfig_Load(object sender, EventArgs e)
        {
            //绑定事件
            WireEvent();

            Factory.IDataSourceFactory(orderRuleClassList).BindDataSource(Globals.BasicInfoTracker.GetOrderRuleClassListItems());
            Factory.IDataSourceFactory(accountRuleClassList).BindDataSource(Globals.BasicInfoTracker.GetAccountRuleClassListItems());
                
        
        }

        void WireEvent()
        {
            Globals.RegIEventHandler(this);

            this.btnCashOperation.Click +=new EventHandler(btnCashOperation_Click);//出入金按钮
            this.btnExecute.Click +=new EventHandler(btnExecute_Click);//冻结 激活
            this.btnUpdate.Click +=new EventHandler(btnUpdate_Click);//更新属性设置

            this.btnAddAccountRule.Click +=new EventHandler(btnAddAccountRule_Click);
            this.btnAddOrderRule.Click +=new EventHandler(btnAddOrderRule_Click);
            this.btnDelAccountRule.Click +=new EventHandler(btnDelAccountRule_Click);
            this.btnDelOrderRule.Click +=new EventHandler(btnDelOrderRule_Click);
            this.pagenav.SelectedPageChanged += new EventHandler(pagenav_SelectedPageChanged);


        }









        /// <summary>
        /// 出入金按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 更新帐户属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (fmConfirm.Show("确认更新帐户属性?") == System.Windows.Forms.DialogResult.Yes)
            {
                if (intraday.Checked != _account.IntraDay)
                {
                    Globals.TLClient.ReqUpdateAccountIntraday(_account.Account, intraday.Checked);
                }
                if (ctRouterType1.RouterType != _account.OrderRouteType)
                {
                    Globals.TLClient.ReqUpdateRouteType(_account.Account, ctRouterType1.RouterType);
                }
                if (poslock.Checked != _account.PosLock)
                {
                    Globals.TLClient.ReqUpdaetAccountPosLock(_account.Account, poslock.Checked);
                }
            }
        }

        /// <summary>
        /// 冻结激活
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void pagenav_SelectedPageChanged(object sender, EventArgs e)
        {

            if (pagenav.SelectedPage.Name.Equals("pageOrderCheck"))
            {
                Globals.TLClient.ReqQryRuleItem(_account.Account, QSEnumRuleType.OrderRule);
            }
            else if (pagenav.SelectedPage.Name.Equals("pageAccountCheck"))
            {
                Globals.TLClient.ReqQryRuleItem(_account.Account, QSEnumRuleType.AccountRule);
            }
            else if (pagenav.SelectedPage.Name.Equals("pageFinance"))
            {
                Globals.TLClient.ReqQryAccountFinInfo(_account.Account);
            }

            

        }



        public void ValidView()
        {
            //if (!Globals.RightRouter)
            //{
            //    routerpanel.Visible = false;
            //}
            //if (!Globals.RightCashOperation)
            //{
            //    cashoppanel.Visible = false;
            //}
        }

    }
}
