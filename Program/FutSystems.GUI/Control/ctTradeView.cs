using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI; 
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;

namespace FutSystems.GUI.Control
{
    public partial class ctTradeView : UserControl,ITradeView
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }


        string _defaultformat = "{0:F2}";
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




        public ctTradeView()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();

            tradeGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(tradeGrid_CellFormatting);
            tradeGrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(tradeGrid_RowPrePaint);
        }

        void tradeGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        

       
        public void GotFill(Trade t)
        {
            if (InvokeRequired)
                Invoke(new FillDelegate(GotFill), new object[] { t });
            else
            {
                DataRow r = tb.Rows.Add(t.id);
                int i = tb.Rows.Count - 1;//得到新建的Row号

                //tradeGrid.Rows.Add(t.id, Util.ToDateTime(t.xdate, t.xtime).ToString("HH:mm:ss"), t.symbol, (t.side ? "买" : "卖"), t.xsize, string.Format(getDisplayFormat(t.symbol), t.xprice), string.Format(_defaultformat, t.Commission), Util.GetEnumDescription(t.PositionOperation), t.Account); // if we accept trade, add it to list
                //tradeGrid.Refresh();
                tb.Rows[i][ID] = t.id;
                tb.Rows[i][DATETIME] = Util.ToDateTime(t.xdate, t.xtime).ToString("HH:mm:ss");
                tb.Rows[i][SYMBOL] = t.symbol;
                tb.Rows[i][SIDE] = (t.side ? "买入" : "   卖出");
                tb.Rows[i][SIZE] = Math.Abs(t.xsize);
                tb.Rows[i][PRICE] = string.Format(getDisplayFormat(t.symbol), t.xprice);
                tb.Rows[i][COMMISSION] = string.Format(_defaultformat, t.Commission);
                tb.Rows[i][OPERATION] = Util.GetEnumDescription(t.PositionOperation);
                tb.Rows[i][ACCOUNT] = t.Account;
                tb.Rows[i][PROFIT] = string.Format(_defaultformat,t.Profit);
                tb.Rows[i][FILLID] = t.BrokerKey;
                num.Text = tradeGrid.RowCount.ToString();

                //toUpdateRow();
                //tb.Rows.Add(new object[] { t.id, Util.ToDateTime(t.xdate, t.xtime).ToString("HH:mm:ss"), t.symbol, (t.side ? "买" : "卖"), t.xsize, string.Format(getDisplayFormat(t.symbol), t.xprice), string.Format(_defaultformat, t.Commission), Util.GetEnumDescription(t.PositionOperation), t.Account });
            }
        }

        public void Clear()
        {
            //kryptonDataGridView1.Enabled = false;
            //tradeGrid.Rows.Clear();
            //kryptonDataGridView1.Enabled = true;
            tradeGrid.DataSource = null;

            tb.Rows.Clear();
            BindToTable();
        }
        //void toUpdateRow()
        //{
        //    for (int i = 0; i < tradeGrid.Rows.Count; i++)
        //    {
        //        if (i == tradeGrid.Rows.Count - 1)
        //        {
        //            tradeGrid.Rows[i].Selected = true;
        //        }
        //        else
        //        {
        //            tradeGrid.Rows[i].Selected = false;
        //        }
        //    }
        //    num.Text = tradeGrid.RowCount.ToString();
        //    //tradeGrid..FirstDisplayedScrollingRowIndex = tradeGrid.RowCount - 1;
        //}


        const string ID = "委托编号";
        const string DATETIME = "成交时间";
        const string SYMBOL = "合约";
        const string SIDE = "买卖";
        const string SIZE = "成交手数";
        const string PRICE = "成交价格";
        const string COMMISSION = "手续费";
        const string OPERATION = "开平";
        const string ACCOUNT = "账户";
        const string PROFIT = "盈亏";
        const string FILLID = "成交编号";

        DataTable tb = new DataTable();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences() 
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = tradeGrid;
;

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeight = 25;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            grid.StateCommon.Background.Color1 = Color.WhiteSmoke;
            grid.StateCommon.Background.Color2 = Color.WhiteSmoke;
        }
        /// <summary>
        /// 初始化数据表格
        /// </summary>
        private void InitTable()
        {
            tb.Columns.Add(ID);
            tb.Columns.Add(DATETIME);
            tb.Columns.Add(SYMBOL);
            tb.Columns.Add(SIDE);
            tb.Columns.Add(SIZE);
            tb.Columns.Add(PRICE);
            tb.Columns.Add(COMMISSION);
            tb.Columns.Add(OPERATION);
            
            tb.Columns.Add(PROFIT);
            tb.Columns.Add(FILLID);
            tb.Columns.Add(ACCOUNT);
        }

        BindingSource datasource = new BindingSource();
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = tradeGrid;

            datasource.DataSource = tb;
            datasource.Sort = DATETIME + " DESC";
            grid.DataSource = datasource;

            for (int i = 0; i < tb.Columns.Count; i++)
            {
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            //grid.Columns[ACCOUNT].IsVisible = false;
        }
        private void ctTradeView_Load(object sender, EventArgs e)
        {
           
        }

        void tradeGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2)
                {
                    e.CellStyle.Font = UIGlobals.BoldFont;
                }

                
                if (e.ColumnIndex == 3)
                {
                    e.CellStyle.Font = UIGlobals.BoldFont;
                    if (e.Value.ToString() == "买入")
                    {
                        e.CellStyle.ForeColor = UIGlobals.LongSideColor;

                    }
                    else
                    {
                        e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                    }
                }
            }
            catch (Exception ex)
            {
                debug("Cellformating error:" + ex.ToString());
            }
        }




    }
}
