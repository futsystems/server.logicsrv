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
using TradingLib.Mixins.LitJson;


namespace FutsMoniter
{
    public partial class ctQuoteMoniter : UserControl,IEventBinder
    {
        public ctQuoteMoniter()
        {

            InitializeComponent();
            viewquotemap.Add("SHFE", quote_cffex);
            viewquotemap.Add("DCE", quote_dce);
            viewquotemap.Add("CZCE", quote_czce);
            viewquotemap.Add("CFFEX", quote_cffex);

            this.Load += new EventHandler(ctQuoteMoniter_Load);
        }

        Dictionary<string, FutSystems.GUI.ViewQuoteList> viewquotemap = new Dictionary<string, FutSystems.GUI.ViewQuoteList>();
        void ctQuoteMoniter_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);

        }


        FutSystems.GUI.ViewQuoteList GetViewQuote(Symbol symbol)
        {
            if (symbol == null) return null;
            FutSystems.GUI.ViewQuoteList vq = null;
            if (viewquotemap.TryGetValue(symbol.SecurityFamily.Exchange.EXCode, out vq))
            {
                return vq;
            }
            return null;
                
        }
        public void OnInit()
        {
            quote_cffex.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);
            quote_czce.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);
            quote_dce.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);
            quote_shfe.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);

            //初始化合约列表
            foreach (Symbol s in Globals.BasicInfoTracker.GetSymbolTradable())
            {
                FutSystems.GUI.ViewQuoteList vq = GetViewQuote(s);
                if (vq != null)
                {
                    vq.addSecurity(s);
                }
            }
            //订阅行情
            Globals.LogicEvent.GotTickEvent += new TickDelegate(GotTick);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.GotTickEvent -= new TickDelegate(GotTick);
        }

        void SelectSymbol(Symbol symbol)
        {
            ctOrderSenderM1.SetSymbol(symbol);
        }


        public void GotTick(Tick k)
        {
            Symbol sym = Globals.BasicInfoTracker.GetSymbol(k.Symbol);
            FutSystems.GUI.ViewQuoteList vq = GetViewQuote(sym);
            if (vq != null)
            {
                vq.GotTick(k);
            }
        }


    }
}
