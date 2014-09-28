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
            bank.Text = string.IsNullOrEmpty(_account.BankID) ? "未设置" : _account.BankID;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string _name = name.Text;
            if (string.IsNullOrEmpty(_name) || _name.Length > 5)
            {
                fmConfirm.Show("请输入正确的投资者姓名!");
                return;
            }

            string _broker = broker.Text;

            string _bankac = bankac.Text;

            string _bank = bank.Text;

            Globals.TLClient.ReqChangeInverstorInfo(_account.Account, _name, _broker,_bank, _bankac);
            this.Close();
        }
    }
}
