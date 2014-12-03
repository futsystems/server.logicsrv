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
    public partial class fmRouterItem : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmRouterItem()
        {
            InitializeComponent();
            Factory.IDataSourceFactory(cbpriority).BindDataSource(GetPriorityCBList());
        }

        ArrayList GetPriorityCBList()
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i <= 9; i++)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = string.Format("{0}级", i);
                vo.Value = i;
                list.Add(vo);
            }
            return list;
        }
        RouterItemSetting _routeritem = null;
        public void SetRouterItemSetting(RouterItemSetting setting)
        {
            _routeritem = setting;
            itemid.Text = setting.ID.ToString();
            cbvendor.SelectedValue = setting.vendor_id;
            rule.Text = setting.rule;
            active.Checked = setting.Active;
            cbpriority.SelectedValue = setting.priority;
            cbvendor.Enabled = false;
        }

        public void SetVendorCBList(ArrayList list)
        {
            Factory.IDataSourceFactory(cbvendor).BindDataSource(list);
        }

        RouterGroupSetting _group = null;
        public void SetRouterGroup(RouterGroupSetting rg)
        {
            _group = rg;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (_routeritem == null)
            {
                int vendorid = int.Parse(cbvendor.SelectedValue.ToString());
                RouterItemSetting item = new RouterItemSetting();
                item.Active = active.Checked;
                item.priority = (int)cbpriority.SelectedValue;
                item.vendor_id = vendorid;
                item.routegroup_id = _group.ID;
                item.rule = rule.Text;
                Globals.TLClient.ReqUpdateRouterItem(item);
                this.Close();
            }
            else
            {
                int vendorid = int.Parse(cbvendor.SelectedValue.ToString());
                _routeritem.vendor_id = vendorid;
                _routeritem.routegroup_id = _group.ID;
                _routeritem.rule = rule.Text;
                _routeritem.Active = active.Checked;
                _routeritem.priority = (int)cbpriority.SelectedValue;

                Globals.TLClient.ReqUpdateRouterItem(_routeritem);
                this.Close();
            }
        }


    }
}
