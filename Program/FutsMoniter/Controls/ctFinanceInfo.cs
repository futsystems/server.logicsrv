using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter
{
    public partial class ctFinanceInfo : UserControl
    {
        public ctFinanceInfo()
        {
            InitializeComponent();
        }


        //string _format="{0:F2}"
        delegate void IAccountInfoDel(IAccountInfo info);
        public void GotAccountInfo(IAccountInfo info)
        {
            if (InvokeRequired)
            {
                Invoke(new IAccountInfoDel(GotAccountInfo), new object[] { info });
            }
            else
            {
                lastequtiy.Text = Util.FormatDecimal(info.LastEquity);
                realizedpl.Text = Util.FormatDecimal(info.RealizedPL);
                unrealizedpl.Text = Util.FormatDecimal(info.UnRealizedPL);
                commission.Text = Util.FormatDecimal(info.Commission);
                netprofit.Text = Util.FormatDecimal(info.Profit);
                cashin.Text = Util.FormatDecimal(info.CashIn);
                cashout.Text = Util.FormatDecimal(info.CashOut);
                nowequity.Text = Util.FormatDecimal(info.NowEquity);
                margin.Text = Util.FormatDecimal(info.Margin);
                marginfrozen.Text = Util.FormatDecimal(info.MarginFrozen);
            }
        }
    }
}
