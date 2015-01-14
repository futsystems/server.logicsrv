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
        }

        void fmHistQuery_Load(object sender, EventArgs e)
        {
            this.btnQryHist.Click +=new EventHandler(btnQryHist_Click);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            Globals.RegIEventHandler(this);
        }

        void btnExport_Click(object sender, EventArgs e)
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView gv = GetCurrentGridView();
            if (gv != null)
            {
                MoniterUtils.ExportToCSV(GetCsvFilePrefix(), gv);
            }
        }

        ComponentFactory.Krypton.Toolkit.KryptonDataGridView GetCurrentGridView()
        {
            if (kryptonNavigator1.SelectedPage.Name == "pageOrder")
            {
                return ctHistOrder1.GridView;
            }
            else if (kryptonNavigator1.SelectedPage.Name == "pageTrade")
            {
                return ctHistTrade1.GridView;
            }
            else if (kryptonNavigator1.SelectedPage.Name == "pagePosition")
            {
                return ctHistPosition1.GridView;
            }
            return null;
        }
        string GetCsvFilePrefix()
        {
            string b= string.Format("{0}_{1}", account.Text, Settleday);
            if (kryptonNavigator1.SelectedPage.Name == "pageOrder")
            {
                return b + "_Order";
            }
            else if (kryptonNavigator1.SelectedPage.Name == "pageTrade")
            {
                return b + "_Trade";
            }
            else if (kryptonNavigator1.SelectedPage.Name == "pagePosition")
            {
                return b + "_Position";
            }
            return b;
        }
        public void OnInit()
        {
            Globals.LogicEvent.GotHistOrderEvent += new Action<Order, bool>(GotHistOrder);
            Globals.LogicEvent.GotHistTradeEvent += new Action<Trade, bool>(GotHistTrade);
            Globals.LogicEvent.GotHistPositionEvent += new Action<PositionDetail, bool>(GotHistPosition);
            
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.GotHistOrderEvent -= new Action<Order, bool>(GotHistOrder);
            Globals.LogicEvent.GotHistTradeEvent -= new Action<Trade, bool>(GotHistTrade);
        }

        void GotHistPosition(PositionDetail d, bool islast)
        {
            if (islast)
            {
                if (d != null)
                {
                    ctHistPosition1.GotHistPosition(d);
                }
            }
            else
            {
                ctHistPosition1.GotHistPosition(d);
            }
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
