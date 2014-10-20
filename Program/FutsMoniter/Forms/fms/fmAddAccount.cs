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
    public partial class fmAddAccount : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmAddAccount()
        {
            InitializeComponent();
            InitializeComponent();
            Factory.IDataSourceFactory(accountType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumAccountCategory>());

        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            QSEnumAccountCategory acccat = (QSEnumAccountCategory)accountType.SelectedValue;
            string accid = account.Text;
            string pass = password.Text;
            int mgrid = ctAgentList1.CurrentAgentFK;
            if (fmConfirm.Show("确认添加交易帐号?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqAddAccount(acccat, accid, pass, mgrid, 0);
                this.Close();
            }
        }
    }
}
