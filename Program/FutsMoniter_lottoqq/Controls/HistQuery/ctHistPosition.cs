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
    public partial class ctHistPosition : UserControl
    {
        public ctHistPosition()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
            this.Load += new EventHandler(ctHistPosition_Load);
        }

        void ctHistPosition_Load(object sender, EventArgs e)
        {
            positiongrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(positiongrid_RowPrePaint);
            positiongrid.CellFormatting += new DataGridViewCellFormattingEventHandler(positiongrid_CellFormatting);
        }

        void positiongrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                e.CellStyle.Font = UIGlobals.BoldFont;
                string op = positiongrid[1, e.RowIndex].Value.ToString();
                if (op.Equals("多"))
                {
                    e.CellStyle.ForeColor = UIGlobals.LongSideColor;
                }
                else
                {
                    e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                }
            }
        }

        void positiongrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        public void Clear()
        {
            positiongrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }

        public ComponentFactory.Krypton.Toolkit.KryptonDataGridView GridView { get { return positiongrid; } }

        delegate void SettlePositionDel(PositionDetail pos);
        public void GotHistPosition(PositionDetail pos)
        {

            if (InvokeRequired)
            {
                try
                {
                    Invoke(new SettlePositionDel(GotHistPosition), new object[] { pos });
                }
                catch (Exception ex)
                { }
            }
            else
            {
                DataRow r = gt.Rows.Add(pos.Symbol);
                int i = gt.Rows.Count - 1;//得到新建的Row号
                //如果不存在,则我们将该account-symbol对插入映射列表我们的键用的是account_symbol配对
                int size = pos.Volume;
                gt.Rows[i][DIRECTION] = pos.Side ? "多" : "空";
                gt.Rows[i][SIZE] = Math.Abs(size);
                //gt.Rows[i][CANFLATSIZE] = getCanFlatSize(pos);
                gt.Rows[i][OPENPRICE] = pos.OpenPrice;
                gt.Rows[i][LASTSETTLEPRICE] = pos.LastSettlementPrice;
                gt.Rows[i][SETTLEPRICE] = pos.SettlementPrice ;

                gt.Rows[i][PROFITBYDATE] = pos.PositionProfitByDate;
            }

        }



        const string SYMBOL = "合约";
        const string DIRECTION = "买卖";
        const string SIZE = "持仓";
        const string OPENPRICE = "开仓价";
        const string LASTSETTLEPRICE = "昨结算";
        const string SETTLEPRICE = "今结算";
        const string PROFITBYDATE = "盯市盈亏";

        DataTable gt = new DataTable();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = positiongrid;

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
            gt.Columns.Add(SYMBOL);
            gt.Columns.Add(DIRECTION);
            gt.Columns.Add(SIZE, typeof(int));
            gt.Columns.Add(OPENPRICE);
            gt.Columns.Add(LASTSETTLEPRICE);
            gt.Columns.Add(SETTLEPRICE);
            gt.Columns.Add(PROFITBYDATE);
        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = positiongrid;

            grid.DataSource = gt;

            //set width
            grid.Columns[SYMBOL].Width = 100;
            grid.Columns[DIRECTION].Width = 50;
            grid.Columns[SIZE].Width = 50;

            for (int i = 0; i < gt.Columns.Count; i++)
            {
                positiongrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
    }
}
