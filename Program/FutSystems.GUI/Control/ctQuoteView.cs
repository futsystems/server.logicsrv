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
using System.Collections.Concurrent;


namespace FutSystems.GUI
{
    public partial class ctQuoteView : UserControl
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        string _defaultformat = "{0:F1}";
        /// <summary>
        /// 查询某个合约用于
        /// </summary>
        public event FindSymbolDel FindSecurityEvent;
        Symbol findSecurity(string symbol)
        {
            if (FindSecurityEvent != null)
                return FindSecurityEvent(symbol);
            else
                return null;
        }

        string getDisplayFormat(string symbol)
        {
            Symbol sec = findSecurity(symbol);
            if (sec == null)
                return _defaultformat;
            else
                return UIUtil.getDisplayFormat(sec.SecurityFamily.PriceTick);
        }

        public ctQuoteView()
        {
            InitializeComponent();
            InitTable();
        }




        public void GotTick(Tick k)
        {
            string sym = k.symbol;
            int rnum = sym2tablerow(sym);
            if (rnum < 0) return;
            tb.Rows[rnum][1] = k.trade;
            tb.Rows[rnum][2] = k.size;
            tb.Rows[rnum][3] = k.AskSize;
            tb.Rows[rnum][4] = k.ask;
            tb.Rows[rnum][5] = k.bid;
            tb.Rows[rnum][6] = k.BidSize;
            tb.Rows[rnum][7] = k.Vol;
            tb.Rows[rnum][9] = k.OpenInterest;
            tb.Rows[rnum][10] = k.OpenInterest - k.PreOpenInterest;
            tb.Rows[rnum][11] = k.Open;
            tb.Rows[rnum][12] = k.High;
            tb.Rows[rnum][13] = k.Low;
            tb.Rows[rnum][14] = k.PreSettlement;

            
        }
        SymbolBasket mb = null;//new BasketImpl();
        public void SetBasket(SymbolBasket b)
        {
            foreach (Symbol s in b)
            {
                AddSecurity(s);
            }
        
        }
        /// <summary>
        /// 增加一个合约
        /// </summary>
        /// <param name="sec"></param>
        public void AddSecurity(Symbol sec)
        {
            string sym = sec.Symbol;
            //1.检查是否存在该symbol,如果存在则直接返回
            if (mb.HaveSymbol(sym)) return;
            //如果baskect中没有该symbol,我们将其加入
            mb.Add(sec);//basket.add默认有完备性检查


            tb.Rows.Add(new object[] { sec.Symbol, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0});
            symboltablemap.TryAdd(sym, tb.Rows.Count - 1);
        }

        int sym2tablerow(string symbol)
        {
            if (!symboltablemap.Keys.Contains(symbol)) return -1;
            return symboltablemap[symbol];
        }
        ConcurrentDictionary<string, int> symboltablemap = new ConcurrentDictionary<string, int>();


        const string SYMBOL = "合约";
        const string TRADE = "最新";
        const string SIZE = "现手";
        const string ASKSIZE = "卖量";
        const string ASKPRICE = "卖价";
        const string BIDPRICE = "买价";
        const string BIDSIZE = "买量";
        const string VOLUME = "成交量";
        const string CHANGE = "涨跌";
        const string OPENINTEREST = "持仓";
        const string INTERESTCHG = "仓差";
        const string OPEN = "开盘价";
        const string HIGH = "最高价";
        const string LOW = "最低价";
        const string SETTLEPRICE = "昨结算";

        DataTable tb = new DataTable();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = quoteGrid;
            grid.ShowRowHeaderColumn = false;//显示每行的头部
            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            grid.EnableHotTracking = true;
            grid.ReadOnly = true;//只读
            grid.AllowAddNewRow = false;//不允许增加新行
            grid.AllowDeleteRow = false;//
            grid.AllowRowResize = false;
            //grid.allowch
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 
        }
        /// <summary>
        /// 初始化数据表格
        /// </summary>
        private void InitTable()
        {
            tb.Columns.Add(SYMBOL);
            tb.Columns.Add(TRADE);
            tb.Columns.Add(SIZE);
            tb.Columns.Add(ASKSIZE);
            tb.Columns.Add(ASKPRICE);
            tb.Columns.Add(BIDPRICE);
            tb.Columns.Add(BIDSIZE);
            tb.Columns.Add(VOLUME);
            tb.Columns.Add(CHANGE);
            tb.Columns.Add(OPENINTEREST);
            tb.Columns.Add(INTERESTCHG);
            tb.Columns.Add(OPEN);
            tb.Columns.Add(HIGH);
            tb.Columns.Add(LOW);
            tb.Columns.Add(SETTLEPRICE);
            
        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = quoteGrid;
            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            grid.DataSource = tb;
        }
        private void ctQuoteView_Load(object sender, EventArgs e)
        {
            SetPreferences();
            
            BindToTable();

        }
    }
}
