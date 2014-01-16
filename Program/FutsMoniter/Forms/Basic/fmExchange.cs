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


namespace FutsMoniter
{
    public partial class fmExchange : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public fmExchange()
        {
            InitializeComponent();
            this.Load += new EventHandler(fmExchange_Load);
           
        }

        void fmExchange_Load(object sender, EventArgs e)
        {
            SetPreferences();
            InitTable();
            BindToTable();
            exchangegrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(exchangegrid_RowPrePaint);

            foreach (Exchange ex in Globals.BasicInfoTracker.Exchanges)
            {
                this.InvokeGotExchange(ex);
            }

        }

        void exchangegrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }


        Dictionary<int, int> exchangeidmap = new Dictionary<int, int>();
        Dictionary<int, Exchange> exchangemap = new Dictionary<int, Exchange>();

        int ExchangeIdx(int id)
        {
            int rowid = -1;
            if (exchangeidmap.TryGetValue(id, out rowid))
            {
                return rowid;
            }
            else
            {
                return -1;
            }
        }


        delegate void ExchangeDel(Exchange ex);
        void InvokeGotExchange(Exchange ex)
        {
            if (InvokeRequired)
            {
                Invoke(new ExchangeDel(InvokeGotExchange), new object[] { ex });
            }
            else
            {
                int r = ExchangeIdx(ex.ID);
                if (r == -1)
                {
                    gt.Rows.Add(ex.ID);
                    int i = gt.Rows.Count - 1;
                    gt.Rows[i][EXNAME] = ex.Name;
                    gt.Rows[i][EXCODE] = ex.EXCode;
                    gt.Rows[i][EXCOUNTRY] = ex.Country;

                    exchangemap.Add(ex.ID, ex);
                    exchangeidmap.Add(ex.ID, i);

                }
                else
                {

                }
            }
        }


        #region 表格
        const string EXID = "全局ID";
        const string EXNAME = "名称";
        const string EXCODE = "编号";
        const string EXCOUNTRY = "国家";

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = exchangegrid;

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

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(EXID);//
            gt.Columns.Add(EXNAME);//
            gt.Columns.Add(EXCODE);//
            gt.Columns.Add(EXCOUNTRY);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = exchangegrid;

            datasource.DataSource = gt;
            grid.DataSource = datasource;

            grid.Columns[EXID].Width = 80;
            grid.Columns[EXNAME].Width = 200;
            grid.Columns[EXCODE].Width = 80;


        }





        #endregion



    }
}
