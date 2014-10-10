using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class SecEditForm : Telerik.WinControls.UI.RadForm
    {
        public SecEditForm()
        {
            InitializeComponent();

            Factory.IDataSourceFactory(currency).BindDataSource(UIUtil.GetEnumValueObjects<CurrencyType>());
            Factory.IDataSourceFactory(securitytype).BindDataSource(UIUtil.GetEnumValueObjects<SecurityType>());
            Factory.IDataSourceFactory(exchange).BindDataSource(Globals.BasicInfoTracker.GetExchangeCombList());
            Factory.IDataSourceFactory(underlay).BindDataSource(Globals.BasicInfoTracker.GetSecurityCombList(true));
            Factory.IDataSourceFactory(markettime).BindDataSource(Globals.BasicInfoTracker.GetMarketTimeCombList());

            this.Text = "添加品种";
        }

        SecurityFamilyImpl _sec = null;
        //当前编辑的合约
        public SecurityFamilyImpl Security 
        {
            get
            {
                return _sec;
            } 
            set
            {
                this.Text = "编辑品种";
                _sec = value;
                code.Text = _sec.Code;
                name.Text = _sec.Name;
                currency.SelectedValue = _sec.Currency;
                securitytype.SelectedValue = _sec.Type;
                
                multiple.Value = _sec.Multiple;
                pricetick.Value = _sec.PriceTick;
                entrycommission.Value = _sec.EntryCommission;
                exitcommission.Value = _sec.ExitCommission;
                margin.Value = _sec.Margin;
                extramargin.Value = _sec.Margin;
                maintancemargin.Value = _sec.MaintanceMargin;
                exchange.SelectedValue = _sec.exchange_fk;
                underlay.SelectedValue = _sec.underlaying_fk;
                markettime.SelectedValue = _sec.mkttime_fk;
                tradeable.Checked = _sec.Tradeable;
                tradeable.Text = this.tradeable.Checked ? "可交易" : "不可交易";

            }
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {

            
            if (_sec != null)
            {
                //
                _sec.Code = code.Text;
                _sec.Name = name.Text;
                _sec.Currency = (CurrencyType)currency.SelectedValue;
                _sec.Type = (SecurityType)securitytype.SelectedValue;

                _sec.Multiple = (int)multiple.Value;
                _sec.PriceTick = pricetick.Value;
                _sec.EntryCommission = entrycommission.Value;
                _sec.ExitCommission = exitcommission.Value;
                _sec.Margin = margin.Value;
                _sec.ExtraMargin = extramargin.Value;
                _sec.MaintanceMargin = maintancemargin.Value;
                _sec.exchange_fk = (int)exchange.SelectedValue;
                _sec.underlaying_fk = (int)underlay.SelectedValue;
                _sec.mkttime_fk = (int)markettime.SelectedValue;
                _sec.Tradeable = tradeable.Checked;

                Globals.TLClient.ReqUpdateSecurity(_sec);
            }
            else
            {
                SecurityFamilyImpl target = new SecurityFamilyImpl();

                target.ID = 0;//0标识新增 数据库ID非0
                target.Code = code.Text;
                target.Name = name.Text;
                target.Currency = (CurrencyType)currency.SelectedValue;
                target.Type = (SecurityType)securitytype.SelectedValue;

                target.Multiple = (int)multiple.Value;
                target.PriceTick = pricetick.Value;
                target.EntryCommission = entrycommission.Value;
                target.ExitCommission = exitcommission.Value;
                target.Margin = margin.Value;
                target.ExtraMargin = extramargin.Value;
                target.MaintanceMargin = maintancemargin.Value;
                target.exchange_fk = (int)exchange.SelectedValue;
                target.underlaying_fk = (int)underlay.SelectedValue;
                target.mkttime_fk = (int)markettime.SelectedValue;
                target.Tradeable = tradeable.Checked;

                Globals.TLClient.ReqAddSecurity(target);
            }
            this.Close();
        }

        private void tradeable_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            if (tradeable.Checked)
            {
                tradeable.Text = "可交易";
            }
            else
            {
                tradeable.Text = "不可交易";
            }
        }
    }
}
