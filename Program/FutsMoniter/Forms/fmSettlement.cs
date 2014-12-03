using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class fmSettlement : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmSettlement()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(fmSettlement_FormClosing);
        }

        void fmSettlement_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        StringBuilder sb = new StringBuilder();
        public void GotSettlement(RspMGRQrySettleResponse response)
        {
            sb.Append(response.SettlementContent);
            if (response.IsLast)
            {
                UpdateSettlebox(sb.ToString());
            }
            
        }
        delegate void del1(string text);
        void UpdateSettlebox(string content)
        {
            if (this.settlebox.InvokeRequired)
            {
                this.Invoke(new del1(UpdateSettlebox), new object[] { content });
            }
            else
            {
                settlebox.Text = content;
            }
        }


        DateTime lastqrytime = DateTime.Now;

        private void btnQryHist_Click(object sender, EventArgs e)
        {
            sb.Clear();
            settlebox.Clear();
            if (string.IsNullOrEmpty(account.Text))
            {
                fmConfirm.Show("请输入要查询的交易帐号");
                return;
            }
            string ac = account.Text;
            //if (!(DateTime.Now.Subtract(lastqrytime).TotalSeconds >5))
            //{
            //    fmConfirm.Show("请不要频繁查询,每隔5秒查询一次!");
            //    return;
            //}
            
            lastqrytime = DateTime.Now;

            Globals.TLClient.ReqQryHistSettlement(ac, Settleday);
        }
        int Settleday
        {
            get
            {
                return TradingLib.Common.Util.ToTLDate(settleday.Value);
            }
        }
    }
}
