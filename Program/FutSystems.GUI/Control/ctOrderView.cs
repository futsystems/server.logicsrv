using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI; 
using TradingLib.API;
using TradingLib.Common;
using System.Threading;


namespace FutSystems.GUI
{
    public partial class ctOrderView : UserControl,IOrderView
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }


        public event LongDelegate SendOrderCancel;
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

        public ctOrderView()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();

        }
        OrderTracker ord;
        /// <summary>
        /// 将OrderTracker传递给orderview,orderview本身不维护order信息,ordertracker由clearcentre或者tradingtracker来提供
        /// </summary>
        public OrderTracker OrderTracker { get { return ord; } set { ord = value; } }



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
        const string COMMENT = "备注";
        const string ORDERREF = "本地编号";
        const string EXCHORDERID = "交易所编号";
        const string EXCHANGE = "交易所";
        const string ACCOUNT = "账户";

        DataTable tb = new DataTable();
        ConcurrentDictionary<long, int> orderidxmap = new ConcurrentDictionary<long, int>();
        int OrderID2Idx(long id)
        {
            int idx = -1;
            if (orderidxmap.TryGetValue(id, out idx))
            {
                return idx;
            }
            return -1;
        }
        string GetOrderPrice(Order o)
        {
            if (o.isMarket)
            {
                return "市价";
            }
            if(o.isLimit)
            {
                return "限价:" + o.price.ToString();
            }
            if (o.isStop)
            {
                return "Stop:" + o.stopp.ToString();
            }
            return "未知";
        }
        public void GotOrder(Order o)
        {
            debug("order view got order:" + o.ToString());
            if (InvokeRequired)
                Invoke(new OrderDelegate(GotOrder), new object[] { o });
            else
            {
                try
                {
                    int i = OrderID2Idx(o.id);
                    if (i == -1)
                    {
                        DataRow r = tb.Rows.Add(o.id);
                        i = tb.Rows.Count - 1;//得到新建的Row号
                        orderidxmap.TryAdd(o.id, i);

                        tb.Rows[i][ID] = o.id;
                        tb.Rows[i][DATETIME] = Util.ToDateTime(o.date, o.time).ToString("HH:mm:ss");
                        tb.Rows[i][SYMBOL] = o.symbol;
                        tb.Rows[i][DIRECTION] = o.side ? "1" : "-1";
                        tb.Rows[i][OPERATION] = o.side ? "买入" : "   卖出";
                        tb.Rows[i][SIZE] = Math.Abs(o.TotalSize);
                        tb.Rows[i][PRICE] = GetOrderPrice(o);
                        tb.Rows[i][FILLED] = Math.Abs(o.Filled);
                        tb.Rows[i][STATUS] = o.Status;
                        tb.Rows[i][STATUSSTR] = LibUtil.GetEnumDescription(o.Status);
                        tb.Rows[i][ORDERREF] = o.OrderRef;
                        tb.Rows[i][EXCHANGE] = o.Exchange;
                        tb.Rows[i][EXCHORDERID] = o.OrderExchID;
                        tb.Rows[i][ACCOUNT] = o.Account;
                        tb.Rows[i][COMMENT] = o.comment;
                        num.Text = orderGrid.RowCount.ToString();
                    }
                    else
                    {
                        tb.Rows[i][FILLED] = o.Filled;
                        tb.Rows[i][STATUS] = o.Status;
                        tb.Rows[i][STATUSSTR] = LibUtil.GetEnumDescription(o.Status);
                        tb.Rows[i][COMMENT] = o.comment;
                    }
                }
                catch (Exception ex)
                {
                    debug("OrderView got order error:" + ex.ToString());
                }
            }
            debug("xxxxxxxxxxxxx got order");
        
        }


        BindingSource datasource = new BindingSource();
        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = orderGrid;
            grid.ShowRowHeaderColumn = false;//显示每行的头部
            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            grid.EnableHotTracking = true;

            grid.AllowAddNewRow = false;//不允许增加新行
            grid.AllowDeleteRow = false;//不允许删除行
            grid.AllowEditRow = false;//不允许编辑行
            grid.AllowRowResize = false;
            grid.EnableSorting = false;
            grid.TableElement.TableHeaderHeight = UIGlobals.HeaderHeight;
            grid.TableElement.RowHeight = UIGlobals.RowHeight;
            
            grid.EnableAlternatingRowColor = true;//隔行不同颜色
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 
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
            tb.Columns.Add(COMMENT);
            tb.Columns.Add(ORDERREF);
            tb.Columns.Add(EXCHANGE);
            tb.Columns.Add(EXCHORDERID);
            tb.Columns.Add(ACCOUNT);
        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = orderGrid;
            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            datasource.DataSource = tb;
            datasource.Sort = DATETIME+ " ASC";
            grid.DataSource = datasource;

            grid.Columns[DIRECTION].IsVisible = false;
            grid.Columns[STATUS].IsVisible = false;

            grid.Columns[DATETIME].Width = 40;
            grid.Columns[DATETIME].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[SYMBOL].Width = 40;
            grid.Columns[SYMBOL].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[OPERATION].Width = 30;
            grid.Columns[OPERATION].TextAlignment = ContentAlignment.MiddleCenter;
            grid.Columns[SIZE].Width = 40;
            grid.Columns[COMMENT].Width = 100;

            
            //set width
            //grid.Columns[SYMBOL].Width = 80;

            
        }

        private void ctOrderView_Load(object sender, EventArgs e)
        {
            //SetPreferences();
            //InitTable();
            //BindToTable();
        }

        #region 界面事件
        private void btnFilterAll_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            string strFilter ="";
            datasource.Filter = strFilter;
        }

        private void btnFilterPlaced_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' or " + STATUS + " = '{1}'", "Placed", "Opened");
            datasource.Filter = strFilter;
        }

        private void btnFilterFilled_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' ", "Filled");
            datasource.Filter = strFilter;
        }

        private void btnFilterCancelError_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' or " + STATUS + " = '{1}' or " + STATUS + " = '{2}'", "Canceled", "Reject", "Unknown");
            datasource.Filter = strFilter;
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            long oid = SelectedOrderID;
            if (oid == -1)
            {
                MessageForm.Show("请选择要撤销的委托");
            }
            else
            {
                if (ord.isPending(oid))
                {
                    if (SendOrderCancel != null)
                        SendOrderCancel(oid);
                }
                else
                {
                    MessageForm.Show("该委托无法撤销");
                }
            }
        }

        private void btnCancelAll_Click(object sender, EventArgs e)
        {
            foreach (Order o in ord)
            {
                if (ord.isPending(o.id))
                {
                    Thread.Sleep(5);
                    if (SendOrderCancel != null)
                        SendOrderCancel(o.id);
                }
            }
        }
        private void orderGrid_DoubleClick(object sender, EventArgs e)
        {
            long oid = SelectedOrderID;
            if (oid == -1)
            {
                MessageForm.Show("请选择要撤销的委托");
            }
            else
            {
                if (ord.isPending(oid))
                {
                    if (SendOrderCancel != null)
                        SendOrderCancel(oid);
                }
                else
                {
                    MessageForm.Show("该委托无法撤销");
                }
            }
        }
        #endregion


        long SelectedOrderID
        {
            get
            {
                if (orderGrid.SelectedRows.Count > 0)
                {
                    return long.Parse(orderGrid.SelectedRows[0].ViewInfo.CurrentRow.Cells[ID].Value.ToString());
                }
                else
                {
                    return -1;
                }
            
            }
        }


        //格式化输出
        private void orderGrid_CellFormatting(object sender, Telerik.WinControls.UI.CellFormattingEventArgs e)
        {
            try
            {
                if (e.CellElement.RowInfo is GridViewDataRowInfo)
                {
                    if (e.CellElement.ColumnInfo.Name == OPERATION)
                    {
                        object direction = e.CellElement.RowInfo.Cells[DIRECTION].Value;
                        if (direction.ToString().Equals("1"))
                        {
                            e.CellElement.ForeColor = UIGlobals.LongSideColor;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else
                        {
                            e.CellElement.ForeColor = UIGlobals.ShortSideColor;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                    }

                    if (e.CellElement.ColumnInfo.Name == SYMBOL)
                    {
                        //e.CellElement.Font = UIGlobals.BoldFont;
                    }

                }


            }
            catch (Exception ex)
            {
                debug("!!!!!!!!!!!!cell format error");
            }

        }


        public void Clear()
        {
            
            orderGrid.DataSource = null;
            orderidxmap.Clear();
            tb.Rows.Clear();
            BindToTable();
        }
    }
}
