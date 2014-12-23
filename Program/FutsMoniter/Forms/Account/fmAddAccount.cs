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
            this.Load += new EventHandler(fmAddAccount_Load);
        }

        void fmAddAccount_Load(object sender, EventArgs e)
        {
            ctAccountType1.AccountTypeSelectedChangedEvent += new VoidDelegate(ctAccountType1_AccountTypeSelectedChangedEvent);
            ctAccountType1_AccountTypeSelectedChangedEvent();
            if (!Globals.Domain.Super)
            {
                //如果开通实盘交易 则默认添加实盘帐户
                if (Globals.Domain.Router_Live)
                {
                    ctAccountType1.AccountType = QSEnumAccountCategory.REAL;
                }
            }
            //代理无法选择添加哪种帐号，只能按照系统设定进行默认添加
            if (!Globals.Manager.IsRoot())
            {
                ctRouterGroupList1.Enabled = false;

                //如果代理不允许添加模拟帐号
                if (!Globals.UIAccess.r_simacc)
                {
                    ctAccountType1.Enabled = false;
                    //如果没有实盘交易 则默认添加模拟帐户 且界面隐藏
                    if (ctAccountType1.AccountType == QSEnumAccountCategory.SIMULATION)
                    {
                        ctAccountType1.Visible = false;
                    }
                }
            }
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
                grid = (acccat == QSEnumAccountCategory.REAL ? ctRouterGroupList1.RouterGroup.ID : 0);
            }
            catch (Exception ex)
            { 
                
            }

            if (acccat == QSEnumAccountCategory.REAL && grid == 0)
            {
                MoniterUtils.WindowMessage("请选择路由组");
                return;
            }
            string accid = account.Text;
            string pass = password.Text;
            int mgrid = ctAgentList1.CurrentAgentFK;

            if (MoniterUtils.WindowConfirm("确认添加交易帐号?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqAddAccount(acccat, accid, pass, mgrid, 0,grid);
                this.Close();
            }
        }
    }
}
