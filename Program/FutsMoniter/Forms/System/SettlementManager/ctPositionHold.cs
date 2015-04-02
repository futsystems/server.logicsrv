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
    public partial class ctPositionHold : UserControl,IEventBinder
    {
        public ctPositionHold()
        {
            InitializeComponent();
            try
            {
                SetPreferences();
                InitTable();
                BindToTable();
                this.Load += new EventHandler(ctPositionHold_Load);
            }
            catch (Exception ex)
            { 
                
            }
        }

        void ctPositionHold_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            gridPositionHold.RowPrePaint += new DataGridViewRowPrePaintEventHandler(gridPositionHold_RowPrePaint);
            gridPositionHold.DoubleClick += new EventHandler(gridPositionHold_DoubleClick);
            
            Globals.RegIEventHandler(this);
        }

        void gridPositionHold_DoubleClick(object sender, EventArgs e)
        {
            PositionEx pos = CurrentPosition;
            if (pos == null)
            {
                MoniterUtils.WindowMessage("请选择要处理的持仓");
                return;
            }

            fmFlatPosition fm = new fmFlatPosition();
            fm.SetPosition(pos);
            fm.ShowDialog();
        }

        void gridPositionHold_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        public void Clear()
        {
            positionExMap.Clear();
            positionExRowMap.Clear();

            gridPositionHold.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("SettleCentre", "QryPositionHold", OnQryPositionHold);
            //Globals.LogicEvent.RegisterCallback("SettleCentre", "UpdateSettlementPrice", OnUpdateSettlementPrice);

        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("SettleCentre", "QryPositionHold", OnQryPositionHold);
            //Globals.LogicEvent.UnRegisterCallback("SettleCentre", "UpdateSettlementPrice", OnUpdateSettlementPrice);
        }

        void OnQryPositionHold(string json)
        {
            PositionEx[] list = MoniterUtils.ParseJsonResponse<PositionEx[]>(json);
            if (list != null)
            {
                foreach (var p in list)
                {
                    InvokeGotPositionEx(p);
                }
            }
        }

        //得到当前选择的行号
        private PositionEx CurrentPosition
        {
            get
            {
                int row = (gridPositionHold.SelectedRows.Count > 0 ? gridPositionHold.SelectedRows[0].Index : -1);
                string key = row == -1 ? string.Empty : (gridPositionHold[0, row].Value.ToString());
                PositionEx pos = null;
                if (positionExMap.TryGetValue(key, out pos))
                {
                    return pos;
                }
                return null;
            }
        }


        private Dictionary<string, PositionEx> positionExMap = new Dictionary<string, PositionEx>();
        private Dictionary<string, int> positionExRowMap = new Dictionary<string, int>();

        string GenPositionExKey(PositionEx pos)
        {
            return string.Format("{0}-{1}-{2}",pos.Account,pos.Symbol,pos.Side?"Long":"Short");
        }
        /// <summary>
        /// 获得某个账户在datatable中的序号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private int PositionExIdx(string key)
        {
            int rowid = -1;
            //map没有account键 还是会给out赋值,因此这里需要用if进行判断 来的到正确的逻辑 否则一致会返回0 出错
            if (!positionExRowMap.TryGetValue(key, out rowid))
                return -1;
            else
                return rowid;
        }

        void InvokeGotPositionEx(PositionEx pos)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<PositionEx>(InvokeGotPositionEx), new object[] { pos });
            }
            else
            {
                string key = GenPositionExKey(pos);

                int r = PositionExIdx(key);//管理端是以account为唯一键值,应该不会出现重复？？
                if (r == -1)//datatable不存在该行，我们则增加该行
                {
                    gt.Rows.Add(0);
                    int i = gt.Rows.Count - 1;//得到新建的Row号

                    gt.Rows[i][KEY] = key;
                    gt.Rows[i][ACCOUNT] = pos.Account;
                    gt.Rows[i][SYMBOL] = pos.Symbol;
                    gt.Rows[i][SIDE] = pos.Side?"多":"空";
                    gt.Rows[i][SIZE] = pos.Position;
                    gt.Rows[i][YDSIZE] = pos.YdPosition;
                    gt.Rows[i][TDSIZE] = pos.TodayPosition;
                    gt.Rows[i][AVGPRICE] = pos.PositionCost;
                    gt.Rows[i][OPENVOL] = pos.OpenVolume;
                    gt.Rows[i][CLOSEVOL] = pos.CloseVolume;



                    positionExMap.Add(key, pos);
                    positionExRowMap.Add(key, i);
                }
                else
                {
                    //更新价格信息
                    //gt.Rows[r][PRICE] = price.Price;
                }

            }
        }



        #region 表格
        #region 显示字段

        const string KEY = "键值";
        const string ACCOUNT = "帐户";
        const string SYMBOL = "合约";
        const string SIDE = "多空";
        const string SIZE = "持仓量";
        const string YDSIZE = "昨日持仓";
        const string TDSIZE = "今日持仓";
        const string AVGPRICE = "持仓均价";
        const string OPENVOL = "开仓数量";
        const string CLOSEVOL = "平仓数量";
        

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = gridPositionHold;

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
            gt.Columns.Add(KEY);
            gt.Columns.Add(ACCOUNT);//
            gt.Columns.Add(SYMBOL);//
            gt.Columns.Add(SIDE);//
            gt.Columns.Add(SIZE);//
            gt.Columns.Add(YDSIZE);//
            gt.Columns.Add(TDSIZE);//
            gt.Columns.Add(AVGPRICE);//
            gt.Columns.Add(OPENVOL);//
            gt.Columns.Add(CLOSEVOL);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = gridPositionHold;


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
