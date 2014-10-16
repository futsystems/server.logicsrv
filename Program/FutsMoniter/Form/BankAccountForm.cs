using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using FutSystems.GUI;
using System.Windows.Forms;
using TradingLib.Mixins.JsonObject;


namespace FutsMoniter
{
    public partial class BankAccountForm : Telerik.WinControls.UI.RadForm
    {

        JsonWrapperBankAccount _bankacc = null;
        public void SetMGRFK(int agentfk)
        {
            lbmgrid.Text = agentfk.ToString();
        }
        public void SetBankInfo(JsonWrapperBankAccount acc)
        {
            if (acc == null)
                return;
            _bankacc = acc;
            
            name.Text = acc.Name;
            bankac.Text = acc.Bank_AC;
            branch.Text = acc.Branch;
            ctBankList1.BankSelected = acc.Bank.ID;
        }
        public BankAccountForm()
        {
            InitializeComponent();
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            if (_bankacc != null)
            {
                _bankacc.Bank_AC = bankac.Text;
                _bankacc.Branch = branch.Text;
                _bankacc.Name = name.Text;
                _bankacc.bank_id = ctBankList1.BankSelected;
                if (fmConfirm.Show("确认修改银行卡信息,信息错误会导致无法收到汇款") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateAgentBankInfo(TradingLib.Mixins.LitJson.JsonMapper.ToJson(_bankacc));
                    this.Close();
                }
            }
            else
            {
                _bankacc = new JsonWrapperBankAccount();
                _bankacc.Bank_AC = bankac.Text;
                _bankacc.Branch = branch.Text;
                _bankacc.Name = name.Text;
                _bankacc.bank_id = ctBankList1.BankSelected;
                if (fmConfirm.Show("确认添加银行卡信息,信息错误会导致无法收到汇款") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateAgentBankInfo(TradingLib.Mixins.LitJson.JsonMapper.ToJson(_bankacc));
                    this.Close();
                }
            }
            
        }
    }
}
