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
    public partial class ctSettlementPrice : UserControl,IEventBinder
    {
        public ctSettlementPrice()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();
            this.Load += new EventHandler(ctSettlementPrice_Load);
        }

        void ctSettlementPrice_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            gridSettlementPrice.DoubleClick += new EventHandler(gridSettlementPrice_DoubleClick);
        }

        void gridSettlementPrice_DoubleClick(object sender, EventArgs e)
        {
            SettlementPrice price = CurrentSettlementPrice;
            if (price == null)
            {
                MoniterUtils.WindowMessage("请选择要编辑的结算价信息");
                return;
            }

            fmUpdateSettlementPrice fm = new fmUpdateSettlementPrice();
            fm.SetSettlementPrice(price);
            fm.ShowDialog();
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("SettleCentre", "QrySettlementPrice", OnQrySettlementPrice);
            Globals.LogicEvent.RegisterCallback("SettleCentre", "UpdateSettlementPrice", OnUpdateSettlementPrice);
           
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("SettleCentre", "QrySettlementPrice", OnQrySettlementPrice);
            Globals.LogicEvent.UnRegisterCallback("SettleCentre", "UpdateSettlementPrice", OnUpdateSettlementPrice);
        }

        //得到当前选择的行号
        private SettlementPrice CurrentSettlementPrice
        {
            get
            {
                //gridSettlementPrice.CurrentRow.Index
                int row = (gridSettlementPrice.SelectedRows.Count > 0 ? gridSettlementPrice.SelectedRows[0].Index : -1);
                string symbol =  row == -1 ? string.Empty : (gridSettlementPrice[1, row].Value.ToString());
                SettlementPrice price = null;
                if (settlementPriceMap.TryGetValue(symbol, out price))
                {
                    return settlementPriceMap[symbol];
                }
                return null;
            }
        }


        private Dictionary<string, SettlementPrice> settlementPriceMap = new Dictionary<string, SettlementPrice>();
        private Dictionary<string, int> settlementPriceRowMap = new Dictionary<string, int>();

        /// <summary>
        /// 获得某个账户在datatable中的序号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private int SettlementPriceIdx(string symbol)
        {
            int rowid = -1;
            //map没有account键 还是会给out赋值,因此这里需要用if进行判断 来的到正确的逻辑 否则一致会返回0 出错
            if (!settlementPriceRowMap.TryGetValue(symbol, out rowid))
                return -1;
            else
                return rowid;
        }

        void OnUpdateSettlementPrice(string json)
        {
            SettlementPrice obj = MoniterUtils.ParseJsonResponse<SettlementPrice>(json);
            if (obj != null)
            {
                InvokeGotSettlementPrice(obj);
            }
        }

        void OnQrySettlementPrice(string json)
        {
            SettlementPrice[] list = MoniterUtils.ParseJsonResponse<SettlementPrice[]>(json);
            if (list != null)
            {
                foreach (var price in list)
                {
                    InvokeGotSettlementPrice(price);
                }
            }
        }

        void InvokeGotSettlementPrice(SettlementPrice price)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SettlementPrice>(InvokeGotSettlementPrice), new object[] { price });
            }
            else
            {
                int r = SettlementPriceIdx(price.Symbol);//管理端是以account为唯一键值,应该不会出现重复？？
                if (r == -1)//datatable不存在该行，我们则增加该行
                {
                    gt.Rows.Add(0);
                    int i = gt.Rows.Count - 1;//得到新建的Row号

                    gt.Rows[i][SETTLEDAY] = price.SettleDay;
                    gt.Rows[i][SYMBOL] = price.Symbol;
                    gt.Rows[i][PRICE] = price.Price;

                    settlementPriceMap.Add(price.Symbol, price);
                    settlementPriceRowMap.Add(price.Symbol, i);
                }
                else
                {
                    //更新价格信息
                    gt.Rows[r][PRICE] = price.Price;
                }

            }
        }



        void OnSettleMentPriceNotify(string json)
        { 
        
        }
        #region 表格
        #region 显示字段

        const string SETTLEDAY = "结算日";
        const string SYMBOL = "合约";
        const string PRICE = "结算价";
      
        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = gridSettlementPrice;

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
            gt.Columns.Add(SETTLEDAY);//
            gt.Columns.Add(SYMBOL);//
            gt.Columns.Add(PRICE);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = gridSettlementPrice;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[EXCHANGEID].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            //grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion

    }
}
