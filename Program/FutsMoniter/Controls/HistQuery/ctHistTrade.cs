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
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class ctHistTrade : UserControl
    {
        public ctHistTrade()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
            this.Load += new EventHandler(ctHistTrade_Load);
        }

        void ctHistTrade_Load(object sender, EventArgs e)
        {
            tradeGrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(tradeGrid_RowPrePaint);
            tradeGrid.CellFormatting += new DataGridViewCellFormattingEventHandler(tradeGrid_CellFormatting);
        }

        void tradeGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                e.CellStyle.Font = UIGlobals.BoldFont;
                string op = tradeGrid[3, e.RowIndex].Value.ToString();
                if (op.Equals("买入"))
                {
                    e.CellStyle.ForeColor = UIGlobals.LongSideColor;
                }
                else
                {
                    e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                }
            }
        }

        void tradeGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        public ComponentFactory.Krypton.Toolkit.KryptonDataGridView GridView { get { return tradeGrid; } }

        string _defaultformat = "{0:F2}";
        public void GotHistFill(Trade t)
        {
            if (InvokeRequired)
                Invoke(new FillDelegate(GotHistFill), new object[] { t });
            else
            {
                DataRow r = tb.Rows.Add(t.id);
                int i = tb.Rows.Count - 1;//得到新建的Row号

                //tradeGrid.Rows.Add(t.id, Util.ToDateTime(t.xdate, t.xtime).ToString("HH:mm:ss"), t.symbol, (t.side ? "买" : "卖"), t.xsize, string.Format(getDisplayFormat(t.symbol), t.xprice), string.Format(_defaultformat, t.Commission), Util.GetEnumDescription(t.PositionOperation), t.Account); // if we accept trade, add it to list
                //tradeGrid.Refresh();
                tb.Rows[i][ID] = t.id;
                tb.Rows[i][DATETIME] = Util.ToDateTime(t.xDate, t.xTime).ToString("HH:mm:ss");
                tb.Rows[i][SYMBOL] = t.Symbol;
                tb.Rows[i][SIDE] = (t.Side ? "买入" : "   卖出");
                tb.Rows[i][SIZE] = Math.Abs(t.xSize);
                tb.Rows[i][PRICE] = string.Format(_defaultformat, t.xPrice);
                tb.Rows[i][COMMISSION] = string.Format(_defaultformat, t.Commission);
                tb.Rows[i][OPERATION] = Util.GetEnumDescription(t.PositionOperation);
                tb.Rows[i][ACCOUNT] = t.Account;
                tb.Rows[i][PROFIT] = string.Format(_defaultformat, t.Profit);
                tb.Rows[i][FILLID] = t.TradeID;
                tb.Rows[i][ORDERREF] = t.OrderSeq;
                //toUpdateRow();
                //tb.Rows.Add(new object[] { t.id, Util.ToDateTime(t.xdate, t.xtime).ToString("HH:mm:ss"), t.symbol, (t.side ? "买" : "卖"), t.xsize, string.Format(getDisplayFormat(t.symbol), t.xprice), string.Format(_defaultformat, t.Commission), Util.GetEnumDescription(t.PositionOperation), t.Account });
            }
        }

        public void Clear()
        {
            tradeGrid.DataSource = null;
            tb.Rows.Clear();
            BindToTable();
        }

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
        const string ORDERREF = "委托流水号";

        DataTable tb = new DataTable();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = tradeGrid;

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
            tb.Columns.Add(ACCOUNT);
            tb.Columns.Add(PROFIT);
            tb.Columns.Add(FILLID);
            tb.Columns.Add(ORDERREF);
        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = tradeGrid;


            grid.DataSource = tb;
            grid.Columns[ACCOUNT].Visible = false;
        }

        //private void tradeGrid_CellFormatting(object sender, CellFormattingEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CellElement.RowInfo is GridViewDataRowInfo)
        //        {

        //            if (e.CellElement.ColumnInfo.Name == SIDE)
        //            {
        //                object side = e.CellElement.RowInfo.Cells[SIDE].Value;
        //                if (side.ToString().Equals("买入"))
        //                {
        //                    e.CellElement.ForeColor = UIGlobals.LongSideColor;
        //                    e.CellElement.Font = UIGlobals.BoldFont;
        //                }
        //                else
        //                {
        //                    e.CellElement.ForeColor = UIGlobals.ShortSideColor;
        //                    e.CellElement.Font = UIGlobals.BoldFont;
        //                }
        //            }
        //            else if (e.CellElement.ColumnInfo.Name == PROFIT)
        //            {
        //                decimal p = 0;

        //                decimal.TryParse(e.CellElement.Value.ToString(), out p);
        //                if (p < 0)
        //                {
        //                    e.CellElement.ForeColor = UIGlobals.ShortSideColor;
        //                }
        //                else if (p > 0)
        //                {
        //                    e.CellElement.ForeColor = UIGlobals.LongSideColor;
        //                }
        //                else
        //                {
        //                    e.CellElement.ForeColor = Color.Black;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
    }
}
