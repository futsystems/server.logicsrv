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
    public partial class fmSettlement : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmSettlement()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmSettlement_Load);
            
        }

        void fmSettlement_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
        }

        public void OnInit()
        {
            Globals.LogicEvent.GotSettlementEvent += new Action<RspMGRQrySettleResponse>(GotSettlement);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.GotSettlementEvent -= new Action<RspMGRQrySettleResponse>(GotSettlement);
        }

        public void SetAccount(string acc)
        {
            account.Text = acc;
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

        void UpdateSettlebox(string content)
        {
            if (this.settlebox.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateSettlebox), new object[] { content });
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
