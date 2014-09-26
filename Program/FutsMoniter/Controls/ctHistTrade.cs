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
        }

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
                tb.Rows[i][DATETIME] = Util.ToDateTime(t.xdate, t.xtime).ToString("HH:mm:ss");
                tb.Rows[i][SYMBOL] = t.symbol;
                tb.Rows[i][SIDE] = (t.side ? "买入" : "   卖出");
                tb.Rows[i][SIZE] = Math.Abs(t.xsize);
                tb.Rows[i][PRICE] = string.Format(_defaultformat, t.xprice);
                tb.Rows[i][COMMISSION] = string.Format(_defaultformat, t.Commission);
                tb.Rows[i][OPERATION] = Util.GetEnumDescription(t.PositionOperation);
                tb.Rows[i][ACCOUNT] = t.Account;
                tb.Rows[i][PROFIT] = string.Format(_defaultformat, 0);
                tb.Rows[i][FILLID] = "fillid";
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

        DataTable tb = new DataTable();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = tradeGrid;
            grid.ShowRowHeaderColumn = false;//显示每行的头部
            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            grid.EnableHotTracking = true;
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 

            grid.AllowAddNewRow = false;//不允许增加新行
            grid.AllowDeleteRow = false;//不允许删除行
            grid.AllowEditRow = false;//不允许编辑行
            grid.AllowRowResize = false;
            grid.EnableSorting = false;
            grid.TableElement.TableHeaderHeight = UIGlobals.HeaderHeight;
            grid.TableElement.RowHeight = UIGlobals.RowHeight;

            grid.EnableAlternatingRowColor = true;//隔行不同颜色
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
        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = tradeGrid;
            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            grid.DataSource = tb;
            grid.Columns[ACCOUNT].IsVisible = false;
        }

        private void tradeGrid_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            try
            {
                if (e.CellElement.RowInfo is GridViewDataRowInfo)
                {

                    if (e.CellElement.ColumnInfo.Name == SIDE)
                    {
                        object side = e.CellElement.RowInfo.Cells[SIDE].Value;
                        if (side.ToString().Equals("买入"))
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
                    else if (e.CellElement.ColumnInfo.Name == PROFIT)
                    {
                        decimal p = 0;

                        decimal.TryParse(e.CellElement.Value.ToString(), out p);
                        if (p < 0)
                        {
                            e.CellElement.ForeColor = UIGlobals.ShortSideColor;
                        }
                        else if (p > 0)
                        {
                            e.CellElement.ForeColor = UIGlobals.LongSideColor;
                        }
                        else
                        {
                            e.CellElement.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
