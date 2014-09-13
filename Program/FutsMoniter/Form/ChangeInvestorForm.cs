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
            token.Text = string.IsNullOrEmpty(_account.Token) ? "未设置" : _account.Token;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string t = token.Text;
            if (string.IsNullOrEmpty(t) || t.Length > 5)
            {
                fmConfirm.Show("请输入正确的投资者姓名!");
                return;
            }

            Globals.TLClient.ReqChangeAccountToken(_account.Account, t);
            this.Close();
        }
    }
}
