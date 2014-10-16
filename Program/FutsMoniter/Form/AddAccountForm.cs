using System;
using System.Collections.Generic;
using System.Collections;
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
    public partial class AddAccountForm : Telerik.WinControls.UI.RadForm
    {
        public AddAccountForm()
        {
            InitializeComponent();
            Factory.IDataSourceFactory(accountType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumAccountCategory>());
            //Factory.IDataSourceFactory(agent).BindDataSource(Globals.BasicInfoTracker.GetBaseManagerCombList());

            if (!Globals.RightAgent)
            {
                //agent.Enabled = false;
            }
            this.Load += new EventHandler(AddAccountForm_Load);
        }

        void AddAccountForm_Load(object sender, EventArgs e)
        {
            //if(Globals)
            ctAgentList1.EnableSelected = true;
        }


        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            QSEnumAccountCategory acccat = (QSEnumAccountCategory)accountType.SelectedValue;
            string accid = account.Text;
            string pass = password.Text;
            int mgrid = ctAgentList1.CurrentAgentFK;
            if (fmConfirm.Show("确认添加交易帐号?") == System.Windows.Forms.DialogResult.Yes)
            { 
                Globals.TLClient.ReqAddAccount(acccat,accid,pass,mgrid,0);
                this.Close();
            }
        }


    }
}
