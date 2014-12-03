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
using TradingLib.Mixins;
using TradingLib.Mixins.LitJson;

namespace FutsMoniter
{
    public partial class fmRouterGroupEdit : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmRouterGroupEdit()
        {
            InitializeComponent();
            Factory.IDataSourceFactory(cbrgstrategytype).BindDataSource(UIUtil.genEnumList<QSEnumRouterStrategy>());
            
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            RouterGroupSetting rg = new RouterGroupSetting();
            rg.Description = rgdescrption.Text;
            rg.Name = rgname.Text;
            rg.Strategy = (QSEnumRouterStrategy)cbrgstrategytype.SelectedValue;

            if (fmConfirm.Show("确认添加路由组?") == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqUpdateRouterGroup(rg);
                this.Close();
            }

        }



    }
}
