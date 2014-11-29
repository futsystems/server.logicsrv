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
            Factory.IDataSourceFactory(accountType).BindDataSource(GetAccountTypeCombList());
            this.Load += new EventHandler(fmAddAccount_Load);
        }

        void fmAddAccount_Load(object sender, EventArgs e)
        {
            accountType.SelectedIndexChanged +=new EventHandler(accountType_SelectedIndexChanged);
            accountType_SelectedIndexChanged(null, null);
        }

        void accountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            QSEnumAccountCategory cat = (QSEnumAccountCategory)accountType.SelectedValue;
            if (cat == QSEnumAccountCategory.REAL)
            {
                ctRouterGroupList1.Visible = true;
            }
            else
            {
                ctRouterGroupList1.Visible = false;
            }
        }

        /// <summary>
        /// 返回manger选择项
        /// 用于创建用户
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAccountTypeCombList(bool all = false, bool includeself = true)
        {
            ArrayList list = new ArrayList();
            if (Globals.UIAccess.acctype_sim)
            {
                ValueObject<QSEnumAccountCategory> vo = new ValueObject<QSEnumAccountCategory>();
                vo.Name = Util.GetEnumDescription(QSEnumAccountCategory.SIMULATION);
                vo.Value = QSEnumAccountCategory.SIMULATION;
                list.Add(vo);
            }
            if (Globals.UIAccess.acctype_live)
            {
                ValueObject<QSEnumAccountCategory> vo = new ValueObject<QSEnumAccountCategory>();
                vo.Name = Util.GetEnumDescription(QSEnumAccountCategory.REAL);
                vo.Value = QSEnumAccountCategory.REAL;
                list.Add(vo);
            }
            if (Globals.UIAccess.acctype_dealer)
            {
                ValueObject<QSEnumAccountCategory> vo = new ValueObject<QSEnumAccountCategory>();
                vo.Name = Util.GetEnumDescription(QSEnumAccountCategory.DEALER);
                vo.Value = QSEnumAccountCategory.DEALER;
                list.Add(vo);
            }

            return list;
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            QSEnumAccountCategory acccat = (QSEnumAccountCategory)accountType.SelectedValue;
            int gid = (acccat == QSEnumAccountCategory.REAL ? ctRouterGroupList1.RouterGroudSelected.ID : 0);
            string accid = account.Text;
            string pass = password.Text;
            int mgrid = ctAgentList1.CurrentAgentFK;
            if (fmConfirm.Show("确认添加交易帐号?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqAddAccount(acccat, accid, pass, mgrid, 0,gid);
                this.Close();
            }
        }
    }
}
