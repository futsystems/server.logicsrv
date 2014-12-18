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
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class fmFinanceCentre : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
         bool _gotdata = false;
        decimal _avabile = 0;
        public fmFinanceCentre()
        {
            InitializeComponent();
            this.Load += new EventHandler(FinanceMangerForm_Load);
            
        }

        void FinanceMangerForm_Load(object sender, EventArgs e)
        {
            this.btnCashOperation.Click += new EventHandler(btnCashOperation_Click);
            this.btnChangeBankAccount.Click += new EventHandler(btnChangeBankAccount_Click);
            Globals.RegIEventHandler(this);
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryFinanceInfo", this.OnQryAgentFinanceInfo);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryFinanceInfoLite", this.OnQryAgentFinanceInfoLite);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "UpdateAgentBankAccount", this.OnUpdateAgentBankInfo);

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "RequestCashOperation", this.OnCashOperation);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "ConfirmCashOperation", this.OnCashOperation);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "CancelCashOperation", this.OnCashOperation);
            Globals.LogicEvent.RegisterCallback("MgrExchServer", "RejectCashOperation", this.OnCashOperation);


            Globals.LogicEvent.RegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);
            //请求代理财务数据
            Globals.TLClient.ReqQryAgentFinanceInfo();
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryFinanceInfo", this.OnQryAgentFinanceInfo);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "UpdateAgentBankAccount", this.OnUpdateAgentBankInfo);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "RequestCashOperation", this.OnCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "ConfirmCashOperation", this.OnCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "CancelCashOperation", this.OnCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "RejectCashOperation", this.OnCashOperation);
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryFinanceInfoLite", this.OnQryAgentFinanceInfoLite);

            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);
                
        }



        /// <summary>
        /// 响应代理银行信息
        /// </summary>
        /// <param name="jsonstr"></param>
        void OnUpdateAgentBankInfo(string jsonstr)
        {
            JsonWrapperBankAccount obj = MoniterUtils.ParseJsonResponse<JsonWrapperBankAccount>(jsonstr);
            if (obj !=null)
            {
                if (_financeinfo != null)
                {
                    _financeinfo.BankAccount = obj;
                    GotFinanceInfo(_financeinfo);
                }
            }
            else//如果没有配资服
            {

            }
        }

        /// <summary>
        /// 响应代理财务信息lite
        /// </summary>
        /// <param name="jsonstr"></param>
        void OnQryAgentFinanceInfoLite(string jsonstr)
        {
            JsonWrapperAgentFinanceInfoLite obj = MoniterUtils.ParseJsonResponse<JsonWrapperAgentFinanceInfoLite>(jsonstr);
            if (obj != null)
            {
                GotFinanceInfoLite(obj);
            }
            else//如果没有配资服
            {

            }
        }

        /// <summary>
        /// 响应代理财务信息
        /// </summary>
        /// <param name="jsonstr"></param>
        void OnQryAgentFinanceInfo(string jsonstr)
        {
            JsonWrapperAgentFinanceInfo obj = MoniterUtils.ParseJsonResponse<JsonWrapperAgentFinanceInfo>(jsonstr);
            if (obj !=null)
            {
                GotFinanceInfo(obj);
            }
            else//如果没有配资服
            {

            }
        }

        /// <summary>
        /// 响应出入金操作
        /// </summary>
        /// <param name="jsonstr"></param>
        void OnCashOperation(string jsonstr)
        {
            JsonWrapperCashOperation obj = MoniterUtils.ParseJsonResponse<JsonWrapperCashOperation>(jsonstr);
            if (obj!= null)
            {
                ctCashOperation1.GotJsonWrapperCashOperation(obj);
                //当有出入金操作变更通知时 重新查询财务信息
                Globals.TLClient.ReqQryAgentFinanceInfoLite();
            }
            else//如果没有配资服
            {

            }
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryAgentFinanceInfoLite();
            }
        }


        /// <summary>
        /// 响应出入金操作通知
        /// </summary>
        /// <param name="jsonstr"></param>
        void OnNotifyCashOperation(string jsonstr)
        {
            JsonWrapperCashOperation obj = MoniterUtils.ParseJsonResponse<JsonWrapperCashOperation>(jsonstr);
            if (obj != null)
            {
                ctCashOperation1.GotJsonWrapperCashOperation(obj);

                //当有出入金操作变更通知时 重新查询财务信息
                Globals.TLClient.ReqQryAgentFinanceInfoLite();
            }
            else//如果没有配资服
            {

            }
        }

        void GotFinanceInfoLite(JsonWrapperAgentFinanceInfoLite info)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<JsonWrapperAgentFinanceInfoLite>(GotFinanceInfoLite), new object[] { info });
            }
            else
            {
                _avabile = info.Balance.Balance - info.PendingWithDraw + info.CashIn - info.CashOut;
                lbbalance.Text = Util.FormatDecimal(_avabile);
                lbpendingdeposit.Text = Util.FormatDecimal(info.PendingWithDraw);
                lbpendingwithdraw.Text = Util.FormatDecimal(info.PendingDeposit);
                if (_financeinfo != null)
                {
                    _financeinfo.Balance.Balance = info.Balance.Balance;
                    _financeinfo.PendingDeposit = info.PendingDeposit;
                    _financeinfo.PendingWithDraw = info.PendingWithDraw;
                }
            }
        }

        JsonWrapperAgentFinanceInfo _financeinfo = null;
        void GotFinanceInfo(JsonWrapperAgentFinanceInfo info)
        { 
            if(InvokeRequired)
            {
                Invoke(new Action<JsonWrapperAgentFinanceInfo>(GotFinanceInfo), new object[] { info });
            }
            else
            {
                _avabile = info.Balance.Balance - info.PendingWithDraw + info.CashIn - info.CashOut;
                lbbalance.Text = Util.FormatDecimal(_avabile);
                lbpendingdeposit.Text = Util.FormatDecimal(info.PendingWithDraw);
                lbpendingwithdraw.Text = Util.FormatDecimal(info.PendingDeposit);
                if (info.BankAccount != null)
                {
                    lbname.Text = info.BankAccount.Name;
                    lbbankbranch.Text = info.BankAccount.Branch;
                    lbbankac.Text = info.BankAccount.Bank_AC;
                    lbbankname.Text = info.BankAccount.Bank.Name;
                    btnChangeBankAccount.Text = "修改银行卡";
                }
                else
                {
                    lbname.Text = "--";
                    lbbankbranch.Text = "--";
                    lbbankac.Text = "--";
                    lbbankname.Text = "--";
                    btnChangeBankAccount.Text = "添加银行卡";
                }

                if (info.LastSettle != null)
                {
                    lblastprofitfee.Text = Util.FormatDecimal(info.LastSettle.Profit_Fee);
                    lblastprofitcommission.Text = Util.FormatDecimal(info.LastSettle.Profit_Commission);
                }
                else
                { 
                    
                }
                if (info.LatestCashOperations != null && !_gotdata )
                { 
                    foreach(JsonWrapperCashOperation op in info.LatestCashOperations )
                    {
                        //GotJsonWrapperCashOperation(op);
                        ctCashOperation1.GotJsonWrapperCashOperation(op);
                    }
                }
                
                _financeinfo = info;
                _gotdata = true;
            }
        }






        










        private void btnChangeBankAccount_Click(object sender, EventArgs e)
        {
            if (_financeinfo == null)
            {
                fmConfirm.Show("无财务信息");
                return;
            }
            fmAgentBankAccount fm = new fmAgentBankAccount();
            fm.SetMGRFK(_financeinfo.BaseMGRFK);
            fm.SetBankInfo(_financeinfo.BankAccount);

            fm.ShowDialog();
        }

        private void btnCashOperation_Click(object sender, EventArgs e)
        {
            fmAgentCashOperation fm = new fmAgentCashOperation();
            fm.SetAvabileBalance(_avabile);
            fm.ShowDialog();
        }
    
    }
}
