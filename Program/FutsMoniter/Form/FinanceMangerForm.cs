using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;


namespace FutsMoniter
{
    
    public partial class FinanceMangerForm : Telerik.WinControls.UI.RadForm
    {
        bool _gotdata = false;
        decimal _avabile = 0;
        public FinanceMangerForm()
        {
            InitializeComponent();
            if (Globals.CallbackCentreReady)
            {
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryFinanceInfo", this.OnQryAgentFinanceInfo);
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "UpdateAgentBankAccount", this.OnUpdateAgentBankInfo);
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "RequestCashOperation", this.OnCashOperation);
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "ConfirmCashOperation", this.OnCashOperation);
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "CancelCashOperation", this.OnCashOperation);
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "RejectCashOperation", this.OnCashOperation);
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryFinanceInfoLite", this.OnQryAgentFinanceInfoLite);

                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);
                
            }
            this.FormClosing += new FormClosingEventHandler(FinanceMangerForm_FormClosing);
            this.Load += new EventHandler(FinanceMangerForm_Load);
        }

        void FinanceMangerForm_Load(object sender, EventArgs e)
        {
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryAgentFinanceInfo();
            }
        }

        void FinanceMangerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Globals.CallbackCentreReady)
            {
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryFinanceInfo", this.OnQryAgentFinanceInfo);
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "UpdateAgentBankAccount", this.OnUpdateAgentBankInfo);
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "RequestCashOperation", this.OnCashOperation);
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "ConfirmCashOperation", this.OnCashOperation);
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "CancelCashOperation", this.OnCashOperation);
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "RejectCashOperation", this.OnCashOperation);
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryFinanceInfoLite", this.OnQryAgentFinanceInfoLite);

                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "NotifyCashOperation", this.OnNotifyCashOperation);
                
            }
        }



        void OnUpdateAgentBankInfo(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperBankAccount obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperBankAccount>(jd["Playload"].ToJson());
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

        void OnQryAgentFinanceInfoLite(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperAgentFinanceInfoLite obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperAgentFinanceInfoLite>(jd["Playload"].ToJson());
                GotFinanceInfoLite(obj);

                //_gotdata = true;
            }
            else//如果没有配资服
            {

            }
        }


        void OnQryAgentFinanceInfo(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperAgentFinanceInfo obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperAgentFinanceInfo>(jd["Playload"].ToJson());
                GotFinanceInfo(obj);

                //_gotdata = true;
            }
            else//如果没有配资服
            {

            }
        }

        void OnCashOperation(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());
                ctCashOperation1.GotJsonWrapperCashOperation(obj);
            }
            else//如果没有配资服
            {

            }
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryAgentFinanceInfoLite();
            }
        }

        void OnNotifyCashOperation(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());
                ctCashOperation1.GotJsonWrapperCashOperation(obj);
            }
            else//如果没有配资服
            {

            }
            //if (Globals.EnvReady)
            //{
            //    Globals.TLClient.ReqQryAgentFinanceInfoLite();
            //}
        }

        delegate void del3(JsonWrapperAgentFinanceInfoLite info);
        void GotFinanceInfoLite(JsonWrapperAgentFinanceInfoLite info)
        {
            if (InvokeRequired)
            {
                Invoke(new del3(GotFinanceInfoLite), new object[] { info });
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
        delegate void del1(JsonWrapperAgentFinanceInfo info);
        void GotFinanceInfo(JsonWrapperAgentFinanceInfo info)
        { 
            if(InvokeRequired)
            {
                Invoke(new del1(GotFinanceInfo), new object[] { info });
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
                    btnChangeBankAccount.Text = "修改银行卡信息";
                }
                else
                {
                    lbname.Text = "--";
                    lbbankbranch.Text = "--";
                    lbbankac.Text = "--";
                    lbbankname.Text = "--";
                    btnChangeBankAccount.Text = "添加银行卡信息";
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
            BankAccountForm fm = new BankAccountForm();
            fm.SetMGRFK(_financeinfo.BaseMGRFK);
            fm.SetBankInfo(_financeinfo.BankAccount);

            fm.ShowDialog();
        }

        private void btnCashOperation_Click(object sender, EventArgs e)
        {
            CashOperationForm fm = new CashOperationForm();
            fm.SetAvabileBalance(_avabile);
            fm.ShowDialog();
        }
    }
}
