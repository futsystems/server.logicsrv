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

namespace FutsMoniter
{
    public partial class SystemStatusForm : Telerik.WinControls.UI.RadForm
    {
        public SystemStatusForm()
        {
            InitializeComponent();
        }

        delegate void SystemStatusDel(SystemStatus status);
        public void GotSystemStatus(SystemStatus s)
        {
            if (InvokeRequired)
            {
                Invoke(new SystemStatusDel(GotSystemStatus), new object[] { s });
            }
            else
            {
                lastsettleday.Text = s.LastSettleday.ToString();
                currenttradingday.Text = s.CurrentTradingday.ToString();
                nexttradingday.Text = s.NextTradingday.ToString();
                settlecentrestatus.Text = s.IsSettleNormal ? "正常" : "异常";
                istradingday.Text = s.IsTradingday ? "开市" : "休市";

                clearcentrestatus.Text = s.IsClearCentreOpen ? "开启" : "关闭";
                totalaccountnum.Text = s.TotalAccountNum.ToString();
                marketopencheck.Text = s.MarketOpenCheck ? "检查" : "不检查";
                runmode.Text = s.IsDevMode ? "开发模式" : "工作模式";
            }
        }

        private void SystemStatusForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
