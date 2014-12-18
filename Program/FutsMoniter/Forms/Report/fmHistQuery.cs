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
using FutSystems.GUI;


namespace FutsMoniter
{

    public partial class fmHistQuery : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmHistQuery()
        {
            InitializeComponent();

            this.Load += new EventHandler(fmHistQuery_Load);
            settleday.Value = DateTime.Now;

            //this.FormClosing +=new FormClosingEventHandler(fmHistQuery_FormClosing);
        }

        void fmHistQuery_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
        }

        public void OnInit()
        {
            Globals.LogicEvent.GotHistOrderEvent += new Action<Order, bool>(GotHistOrder);
            Globals.LogicEvent.GotHistTradeEvent += new Action<Trade, bool>(GotHistTrade);
            //Globals.LogicEvent.GotSettlementEvent +=new Action<RspMGRQrySettleResponse>(LogicEvent_GotSettlementEvent);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.GotHistOrderEvent -= new Action<Order, bool>(GotHistOrder);
            Globals.LogicEvent.GotHistTradeEvent -= new Action<Trade, bool>(GotHistTrade);
        }

        void GotHistOrder(Order o, bool islast)
        {

            if (islast)
            {
                if (o!=null)
                {
                    ctHistOrder1.GotHistOrder(o);
                }
                Globals.TLClient.ReqQryHistTrades(account.Text, Settleday);
            }
            else
            {
                ctHistOrder1.GotHistOrder(o);
            }
        }
        public void GotHistTrade(Trade f, bool islast)
        {
            if (islast)
            {
                if (f!=null)
                {
                    ctHistTrade1.GotHistFill(f);
                }
                Globals.TLClient.ReqQryHistPosition(account.Text, Settleday);
            }
            else
            {
                ctHistTrade1.GotHistFill(f);
            }
        }

        public void GotHistPosition(PositionDetail pos, bool islast)
        {
            if (islast)
            {
                if (!string.IsNullOrEmpty(pos.Account))
                {
                    ctHistPosition1.GotHistPosition(pos);
                }
                //Globals.TLClient.ReqQryHistCashTransaction(account.Text, Settleday);
            }
            else
            {
                ctHistPosition1.GotHistPosition(pos);
            }
        }



       

        public void SetAccount(string acc)
        {
            account.Text = acc;
        }

        private void fmHistQuery_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        DateTime lastqrytime = DateTime.Now;
        bool _first = true;
        private void btnQryHist_Click(object sender, EventArgs e)
        {
            if (_first)
            {
                _first = false;
            }
            else
            {
                if (!(DateTime.Now.Subtract(lastqrytime).TotalSeconds > 5))
                {
                    fmConfirm.Show("请不要频繁查询,每隔5秒查询一次!");
                    return;
                }
            }

            lastqrytime = DateTime.Now;
            //清空当前数据
            ctHistOrder1.Clear();
            ctHistTrade1.Clear();
            ctHistPosition1.Clear();
            //ctHistCashTransaction1.Clear();
            //sb.Clear();
            Globals.TLClient.ReqQryHistOrders(account.Text, Settleday);
            
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
