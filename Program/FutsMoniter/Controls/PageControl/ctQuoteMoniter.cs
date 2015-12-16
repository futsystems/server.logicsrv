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
    public partial class ctQuoteMoniter : UserControl,IEventBinder
    {
        public ctQuoteMoniter()
        {

            InitializeComponent();
            viewquotemap.Add("SHFE", quote_shfe);
            viewquotemap.Add("DCE", quote_dce);
            viewquotemap.Add("CZCE", quote_czce);
            viewquotemap.Add("CFFEX", quote_cffex);
            viewquotemap.Add("INNOVEX", quote_innov);
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
            return GetViewQuote(symbol.SecurityFamily);
                
        }
        FutSystems.GUI.ViewQuoteList GetViewQuote(SecurityFamily sec)
        {
            if (sec == null) return null;
            if (sec.Exchange == null) return null;

            FutSystems.GUI.ViewQuoteList vq = null;
            
            if (viewquotemap.TryGetValue(sec.Exchange.EXCode, out vq))
            {
                return vq;
            }
            return null;
        }

        public void OnInit()
        {
            if (!Globals.Domain.Super)
            {
                ctOrderSenderM1.Visible = Globals.Manager.IsRoot()||Globals.UIAccess.r_execution;
                if (!ctOrderSenderM1.Visible)
                {
                    quotenav.Height = quotenav.Height + ctOrderSenderM1.Height;

                }
            }

            quote_cffex.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);
            quote_czce.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);
            quote_dce.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);
            quote_shfe.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);
            quote_innov.SymbolSelectedEvent += new SymbolDelegate(SelectSymbol);
            //初始化合约列表
            foreach (Symbol s in Globals.BasicInfoTracker.GetSymbolTradable())
            {
                //quote_czce.addSecurity(s);

                FutSystems.GUI.ViewQuoteList vq = GetViewQuote(s);
                if (vq != null)
                {
                    vq.addSecurity(s);
                }
            }
            //响应行情回报行情
            Globals.LogicEvent.GotTickEvent += new TickDelegate(GotTick);
            Globals.BasicInfoTracker.GotSymbolEvent += new Action<SymbolImpl>(GotSymbol);
            Globals.BasicInfoTracker.GotSecurityEvent += new Action<SecurityFamilyImpl>(GotSecurity);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.GotTickEvent -= new TickDelegate(GotTick);
            Globals.BasicInfoTracker.GotSymbolEvent -= new Action<SymbolImpl>(GotSymbol);
            Globals.BasicInfoTracker.GotSecurityEvent -= new Action<SecurityFamilyImpl>(GotSecurity);
        }

        void SelectSymbol(Symbol symbol)
        {
            ctOrderSenderM1.SetSymbol(symbol);
        }

        void GotSecurity(SecurityFamily sec)
        {
            //Globals.Debug("got security reply");
            FutSystems.GUI.ViewQuoteList vq = GetViewQuote(sec);
            if (vq == null) return;
            //品种可交易 则加载该品种的所有可交易合约
            if (sec.Tradeable)
            {
                //Globals.Debug("品种:" + sec.Code + " 可交易");
                foreach (Symbol s in Globals.BasicInfoTracker.GetSymbolTradable().Where(s => s.SecurityFamily.Code.Equals(sec.Code)))
                {
                    //Globals.Debug("添加合约:" + s.Symbol);
                    vq.addSecurity(s);
                }
            }
            else//如果不可交易 则删除所有报价列表的合约
            {
                //Globals.Debug("品种:" + sec.Code + " 不可交易");
                Symbol[] syms = vq.Symbols.Where(s => s.SecurityFamily.Code.Equals(sec.Code)).ToArray();
                foreach (Symbol sym in syms)
                {
                    //Globals.Debug("删除合约:" + sym.Symbol);
                    vq.delSecurity(sym);
                }
            }
        }
        void GotSymbol(SymbolImpl symbol)
        {
            if (symbol.IsTradeable)
            {
                FutSystems.GUI.ViewQuoteList vq = GetViewQuote(symbol);
                if (vq != null)
                {
                    vq.addSecurity(symbol);
                }
            }
            else
            {
                FutSystems.GUI.ViewQuoteList vq = GetViewQuote(symbol);
                if (vq != null)
                {
                    vq.delSecurity(symbol);
                }
            }
        }

        void GotTick(Tick k)
        {
            Symbol sym = Globals.BasicInfoTracker.GetSymbol(k.Symbol);
            FutSystems.GUI.ViewQuoteList vq = GetViewQuote(sym);
            if (vq != null)
            {
                vq.GotTick(k);
            }
            if (k.Symbol.StartsWith("IF"))
            {
                Tick nk = TickImpl.Copy(k);
                nk.Symbol = string.Format("{0}-mini", nk.Symbol);
                quote_innov.GotTick(nk);
            }
        }


    }
}
