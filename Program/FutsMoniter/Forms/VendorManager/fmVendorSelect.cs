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
    public partial class fmVendorSelect : ComponentFactory.Krypton.Toolkit.KryptonForm
    {

        public fmVendorSelect()
        {
            InitializeComponent();

        }

        ConnectorConfig _cfg = null;
        public void SetConnectorConfig(ConnectorConfig cfg)
        {
            _cfg = cfg;
        }
        public void SetVendorCBList(ArrayList list)
        {
            Factory.IDataSourceFactory(cbvendorlist).BindDataSource(list);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            VendorSetting setting = (VendorSetting)cbvendorlist.SelectedValue;

            if(fmConfirm.Show(string.Format("确认将通道[{0}]绑定到实盘帐户:{1}",_cfg.Token,setting.Name)) == System.Windows.Forms.DialogResult.Yes)
            {
                Globals.TLClient.ReqBindVendor(_cfg.ID,setting.ID);
                this.Close();
            }
        }


    }
}
