using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
    public partial class fmVendorEdit :ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmVendorEdit()
        {
            InitializeComponent();
            this.Text = "添加帐户";
        }

        VendorSetting _setting = null;
        public void SetVendorSetting(VendorSetting setting)
        {
            _setting = setting;
            this.Text = "修改帐户:" + setting.Name;
            id.Text = setting.ID.ToString();
            name.Text = setting.Name;
            futcompany.Text = setting.FutCompany;
            lastequity.Value = setting.LastEquity;
            marginlimit.Value = setting.MarginLimit;
            description.Text = setting.Description;

            name.Enabled = false;

        }
        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (_setting == null)
            {
                VendorSetting setting = new VendorSetting();
                setting.Description = description.Text;
                setting.FutCompany = futcompany.Text;
                setting.LastEquity = lastequity.Value;
                setting.MarginLimit = marginlimit.Value;
                setting.Name = name.Text;
                if (string.IsNullOrEmpty(name.Text))
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请设置帐户名称");
                    return;
                }
                if (setting.MarginLimit ==0)
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请设置资金使用限额规则,小于1按百分比计算,大于1按额度计算");
                    return;
                }
                //if (setting.LastEquity == 0)
                //{
                //    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("帐户资金为0将不能进行任何交易");
                //}
                if (fmConfirm.Show("确认添加帐户:" + setting.Name) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateVendor(setting);
                    this.Close();
                }

            }
            else
            {
                _setting.Name = name.Text;
                _setting.FutCompany = futcompany.Text;
                _setting.LastEquity = lastequity.Value;
                _setting.MarginLimit = marginlimit.Value;
                _setting.Description = description.Text;
                if (_setting.MarginLimit == 0)
                {
                    ComponentFactory.Krypton.Toolkit.KryptonMessageBox.Show("请设置资金使用限额规则,小于1按百分比计算,大于1按额度计算");
                    return;
                }
                if (fmConfirm.Show("确认修改帐户") == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.TLClient.ReqUpdateVendor(_setting);
                    this.Close();
                }
            }
        }
    }
}
