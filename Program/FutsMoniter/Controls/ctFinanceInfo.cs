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

        delegate void IAccountInfoDel(IAccountInfo info);
        public void GotAccountInfo(IAccountInfo info)
        {
            if (InvokeRequired)
            {
                Invoke(new IAccountInfoDel(GotAccountInfo), new object[] { info });
            }
            else
            {
                lastequtiy.Text = LibUtil.FormatDisp(info.LastEquity);
                realizedpl.Text = LibUtil.FormatDisp(info.RealizedPL);
                unrealizedpl.Text = LibUtil.FormatDisp(info.UnRealizedPL);
                commission.Text = LibUtil.FormatDisp(info.Commission);
                netprofit.Text = LibUtil.FormatDisp(info.Profit);
                cashin.Text = LibUtil.FormatDisp(info.CashIn);
                cashout.Text = LibUtil.FormatDisp(info.CashOut);
                nowequity.Text = LibUtil.FormatDisp(info.NowEquity);
                margin.Text = LibUtil.FormatDisp(info.Margin);
                marginfrozen.Text = LibUtil.FormatDisp(info.MarginFrozen);
            }
        }
    }
}
