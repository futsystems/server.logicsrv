using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.API;

namespace Strategy.GUI
{
    public partial class fmStopLoss:KryptonForm
    {
        public decimal StopLoss { get { return _stopLoss.Value; } }
       

        public fmStopLoss(string symbol, decimal p)
        {
            InitializeComponent();
            //_size.Value = s;
            _stopLoss.Value = p;
            Text = symbol;
            this.FormClosing += new FormClosingEventHandler(fmTargetProfit_FormClosing);
        }

        void fmTargetProfit_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
        public void updateForm(decimal price,decimal cost,bool side)
        {
            _last.Text = formatdisp(price);
            _targetprice.Text = formatdisp(side ? cost - StopLoss : cost + StopLoss);

        }
        public void SetSecurity(Security sec)
        { 
            decimal a = sec.PriceTick;
            int p = TradingLib.GUI.UIUtil.getDecimalPlace(sec.PriceTick);
            _format = TradingLib.GUI.UIUtil.getDisplayFormat(sec.PriceTick);
            
            _stopLoss.Increment = a;
            _stopLoss.DecimalPlaces = p;
        }
        private string _format = "{0:F1}";
        private string formatdisp(decimal d)
        {
            return string.Format(_format, d);
        }

        private void kryptonLinkLabel1_LinkClicked(object sender, EventArgs e)
        {
            MemoStopLoss fm = new MemoStopLoss();
            fm.Show();
        }
       
    }
}
