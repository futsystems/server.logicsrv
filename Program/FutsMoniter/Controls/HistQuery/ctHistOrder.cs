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
    public partial class ctHistOrder : UserControl
    {
        public ctHistOrder()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

        }

        //public Telerik.WinControls.UI.RadGridView Grid { get { return orderGrid; } }

        public void Clear()
        {
            orderGrid.DataSource = null;
            tb.Rows.Clear();
            BindToTable();
        }

        const string ID = "报单编号";
        const string DATETIME = "报单时间";
        const string SYMBOL = "合约";
        const string DIRECTION = "方向";
        const string OPERATION = "买卖";
        const string SIZE = "报单手数";
        const string PRICE = "报单价格";
        const string FILLED = "成交手数";
        const string STATUS = "挂单状态";
        const string STATUSSTR = "状态";
        const string ORDERREF = "本地编号";
        const string EXCHORDERID = "交易所编号";
        const string EXCHANGE = "交易所";
        const string ACCOUNT = "账户";

        DataTable tb = new DataTable();

        string GetOrderPrice(Order o)
        {
            if (o.isMarket)
            {
                return "市价";
            }
            if (o.isLimit)
            {
                return "限价:" + o.price.ToString();
            }
            if (o.isStop)
            {
                return "Stop:" + o.stopp.ToString();
            }
            return "未知";
        }



        public void GotHistOrder(Order o)
        {
            if (InvokeRequired)
                Invoke(new OrderDelegate(GotHistOrder), new object[] { o });
            else
            {
                try
                {
                    DataRow r = tb.Rows.Add(o.id);
                    int i = tb.Rows.Count - 1;//得到新建的Row号

                    tb.Rows[i][ID] = o.id;
                    tb.Rows[i][DATETIME] = Util.ToDateTime(o.date, o.time).ToString("HH:mm:ss");
                    tb.Rows[i][SYMBOL] = o.symbol;
                    tb.Rows[i][DIRECTION] = o.side ? "1" : "-1";
                    tb.Rows[i][OPERATION] = o.side ? "买入" : "   卖出";
                    tb.Rows[i][SIZE] = Math.Abs(o.TotalSize);
                    tb.Rows[i][PRICE] = GetOrderPrice(o);
                    tb.Rows[i][FILLED] = Math.Abs(o.Filled);
                    tb.Rows[i][STATUS] = o.Status;
                    tb.Rows[i][STATUSSTR] = Util.GetEnumDescription(o.Status);
                    tb.Rows[i][ORDERREF] = o.OrderRef;
                    tb.Rows[i][EXCHANGE] = o.Exchange;
                    tb.Rows[i][EXCHORDERID] = o.OrderExchID;
                }
                catch (Exception ex)
                {
                }
            }
        }


        BindingSource datasource = new BindingSource();
        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = orderGrid;

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
            tb.Columns.Add(DIRECTION);
            tb.Columns.Add(OPERATION);
            tb.Columns.Add(SIZE);
            tb.Columns.Add(PRICE);
            tb.Columns.Add(FILLED);
            tb.Columns.Add(STATUS);
            tb.Columns.Add(STATUSSTR);
            tb.Columns.Add(ORDERREF);
            tb.Columns.Add(EXCHANGE);
            tb.Columns.Add(EXCHORDERID);
            //tb.Columns.Add(ACCOUNT);
        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = orderGrid;

            datasource.DataSource = tb;
            datasource.Sort = DATETIME + " ASC";
            grid.DataSource = datasource;

            grid.Columns[DIRECTION].Visible = false;
            grid.Columns[STATUS].Visible = false;

            //set width
            //grid.Columns[SYMBOL].Width = 80;


        }

        //private void orderGrid_CellFormatting(object sender, Telerik.WinControls.UI.CellFormattingEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CellElement.RowInfo is GridViewDataRowInfo)
        //        {
        //            if (e.CellElement.ColumnInfo.Name == OPERATION)
        //            {
        //                object direction = e.CellElement.RowInfo.Cells[DIRECTION].Value;
        //                if (direction.ToString().Equals("1"))
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

        //            if (e.CellElement.ColumnInfo.Name == SYMBOL)
        //            {
        //                //e.CellElement.Font = UIGlobals.BoldFont;
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
    }
}
