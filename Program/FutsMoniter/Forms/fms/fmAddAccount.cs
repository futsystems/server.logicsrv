using System;
using System.Collections.Generic;
using System.Collections;
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
            //Factory.IDataSourceFactory(accountType).BindDataSource(MoniterUtil.GetAccountTypeCombList());
            this.Load += new EventHandler(fmAddAccount_Load);
        }

        void fmAddAccount_Load(object sender, EventArgs e)
        {
            //accountType.SelectedIndexChanged +=new EventHandler(accountType_SelectedIndexChanged);
            ctAccountType1.AccountTypeSelectedChangedEvent += new VoidDelegate(ctAccountType1_AccountTypeSelectedChangedEvent);
            ctAccountType1_AccountTypeSelectedChangedEvent();
        }

        void ctAccountType1_AccountTypeSelectedChangedEvent()
        {
            QSEnumAccountCategory cat = ctAccountType1.AccountType;
            if (cat == QSEnumAccountCategory.REAL)
            {
                ctRouterGroupList1.Visible = true;
            }
            else
            {
                ctRouterGroupList1.Visible = false;
            }
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            int grid = 0;
            QSEnumAccountCategory acccat = ctAccountType1.AccountType;
            try
            {
                grid = (acccat == QSEnumAccountCategory.REAL ? ctRouterGroupList1.RouterGroudSelected.ID : 0);
            }
            catch (Exception ex)
            { 
                
            }
            string accid = account.Text;
            string pass = password.Text;
            int mgrid = ctAgentList1.CurrentAgentFK;
            if (fmConfirm.Show("确认添加交易帐号?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqAddAccount(acccat, accid, pass, mgrid, 0,grid);
                this.Close();
            }
        }
    }
}
