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


namespace FutsMoniter
{
    public partial class fmRouterItem : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmRouterItem()
        {
            InitializeComponent();

        }

        RouterItemSetting routeritem = null;
        public void SetRouterItemSetting(RouterItemSetting setting)
        {
            routeritem = setting;
            itemid.Text = setting.ID.ToString();
            cbvendor.SelectedValue = setting.vendor_id;
            rule.Text = setting.rule;
            active.Checked = setting.Active;


        }
    }
}
