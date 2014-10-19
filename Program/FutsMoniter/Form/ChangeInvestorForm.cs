using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using FutSystems.GUI;
using TradingLib.API;

namespace FutsMoniter
{
    public partial class ChangeInvestorForm : Telerik.WinControls.UI.RadForm
    {
        public ChangeInvestorForm()
        {
            InitializeComponent();
        }


        IAccountLite _account = null;
        public void SetAccount(IAccountLite acc)
        {
            _account = acc;
            account.Text = _account.Account;
            name.Text = string.IsNullOrEmpty(_account.Name) ? "未设置" : _account.Name;
            broker.Text = string.IsNullOrEmpty(_account.Broker) ? "未设置" : _account.Broker;
            bankac.Text = string.IsNullOrEmpty(_account.BankAC) ? "未设置" : _account.BankAC;
            //bank.Text = string.IsNullOrEmpty(_account.BankID) ? "未设置" : _account.BankID;
            if (_account.BankID != 0)
            {
                ctBankList1.BankSelected = _account.BankID;
            }
        }
        System.Text.RegularExpressions.Regex regexbankid = new System.Text.RegularExpressions.Regex(@"^[-]?[1-9]{1}\d*$|^[0]{1}$");
        System.Text.RegularExpressions.Regex regexname = new System.Text.RegularExpressions.Regex(@"^[\u4e00-\u9fa5]+$");
        
        
        

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string _name = name.Text;
            if (string.IsNullOrEmpty(_name) || _name.Length > 5)
            {
                fmConfirm.Show("请输入正确的投资者姓名!");
                return;
            }
            
            
            string _broker = broker.Text;

            int _bankfk=0;
            string _bankac=string.Empty;

            //如果要设置银行卡信息 则将界面上的银行卡信息提交到服务端
            if (!cbnosetbank.Checked)
            {
                if (!regexbankid.IsMatch(bankac.Text))
                {
                    fmConfirm.Show("请输入有效的银行帐户");
                    return;
                }
                _bankac = bankac.Text;

                _bankfk = ctBankList1.BankSelected;
            }
            Globals.TLClient.ReqChangeInverstorInfo(_account.Account, _name, _broker, _bankfk, _bankac);
            this.Close();
        }
    }
}
