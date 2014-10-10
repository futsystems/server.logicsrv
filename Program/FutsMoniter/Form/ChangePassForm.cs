using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class ChangePassForm : Telerik.WinControls.UI.RadForm
    {
        public ChangePassForm()
        {
            InitializeComponent();
        }

        public void SetAccount(string account)
        {
            this.account.Text = account;
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (newpass.Text.Contains(",") || newpass.Text.Contains("|") || newpass.Text.Contains("^"))
            {

                fmConfirm.Show("密码不能含有系统保留字符 | , ^");
                return;
            }
            if (newpass.Text.Length < 4)
            {
                fmConfirm.Show("密码长度不能小于4位");
                return;
            }
            if (newpass.Text.Length > 10)
            {
                fmConfirm.Show("密码不能大于10位");
                return;
            }

            Globals.TLClient.ReqChangeAccountPass(account.Text, newpass.Text);
        }
    }
}
