using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class SymbolForm : Telerik.WinControls.UI.RadForm
    {
        public SymbolForm()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();

            Factory.IDataSourceFactory(cbsecurity).BindDataSource(UIUtil.GetEnumValueObjects<SecurityType>(true));
            Factory.IDataSourceFactory(cbtradeable).BindDataSource(Utils.GetTradeableCBList(true));
            
            _load = true;
        }

        public bool AnySymbol
        {
            get
            {
                return symbolmap.Count > 0;
            }
        }


        Dictionary<int, SymbolImpl> symbolmap = new Dictionary<int, SymbolImpl>();
        Dictionary<int, int> symbolidxmap = new Dictionary<int, int>();

        int SymbolIdx(int id)
        {
            int rowid = -1;
            if (symbolidxmap.TryGetValue(id, out rowid))
            {
                return rowid;
            }
            else
            {
                return -1;
            }
        }

        public void GotSymbol(SymbolImpl sym)
        {
            InvokeGotSymbol(sym);
        }

        string GetTradeableTitle(bool tradeable)
        {
            return tradeable ? "是" : "否";
        }

        delegate void SymbolImplDel(SymbolImpl sym);
        void InvokeGotSymbol(SymbolImpl sym)
        {
            if (InvokeRequired)
            {
                Invoke(new SymbolImplDel(InvokeGotSymbol), new object[] { sym });
            }
            else
            {
                if (string.IsNullOrEmpty(sym.Symbol))
                    return;
                int r = SymbolIdx(sym.ID);
                if (r == -1)
                {
                    gt.Rows.Add(sym.ID);
                    int i = gt.Rows.Count - 1;

                    symbolmap.Add(sym.ID, sym);
                    symbolidxmap.Add(sym.ID, i);

                    gt.Rows[i][SYMBOL] = sym.Symbol;
                    gt.Rows[i][ENTRYCOMMISSION] = sym.EntryCommission;
                    gt.Rows[i][EXITCOMMISSION] = sym.ExitCommission;
                    gt.Rows[i][MARGIN] = sym.Margin;
                    gt.Rows[i][EXTRAMARGIN] = sym.ExtraMargin;
                    gt.Rows[i][MAINTANCEMARGIN] = sym.MaintanceMargin;

                    gt.Rows[i][SECCODE] = sym.SecurityFamily != null ? sym.SecurityFamily.Code : "未设置";
                    gt.Rows[i][SECTYPE] = sym.SecurityType;
                    gt.Rows[i][EXCHANGEID] = sym.SecurityFamily != null ? (sym.SecurityFamily.Exchange as Exchange).ID : 0;
                    gt.Rows[i][EXCHANGE] = sym.SecurityFamily != null ? (sym.SecurityFamily.Exchange as Exchange).EXCode : "未设置";

                    gt.Rows[i][UNDERLAYINGID] = sym.underlaying_fk;
                    gt.Rows[i][UNDERLAYING] = sym.ULSymbol != null ? sym.ULSymbol.Symbol : "无";

                    gt.Rows[i][UNDERLAYINGSYMBOLID] =sym.underlayingsymbol_fk;
                    gt.Rows[i][UNDERLAYINGSYMBOL] = sym.UnderlayingSymbol != null ? sym.UnderlayingSymbol.Symbol : "无";
                    gt.Rows[i][EXPIREMONTH] = sym.ExpireMonth;
                    gt.Rows[i][EXPIREDATE] = sym.ExpireDate;
                    gt.Rows[i][TRADEABLE] = sym.Tradeable;
                    gt.Rows[i][TRADEABLETITLE] = GetTradeableTitle(sym.Tradeable);

                }
                else
                {
                    int i = r;
                    gt.Rows[i][SYMBOL] = sym.Symbol;
                    gt.Rows[i][ENTRYCOMMISSION] = sym.EntryCommission;
                    gt.Rows[i][EXITCOMMISSION] = sym.ExitCommission;
                    gt.Rows[i][MARGIN] = sym.Margin;
                    gt.Rows[i][EXTRAMARGIN] = sym.ExtraMargin;
                    gt.Rows[i][MAINTANCEMARGIN] = sym.MaintanceMargin;
                    gt.Rows[i][SECCODE] = sym.SecurityFamily != null ? sym.SecurityFamily.Code : "未设置";
                    gt.Rows[i][SECTYPE] = sym.SecurityType;
                    gt.Rows[i][EXCHANGEID] = sym.SecurityFamily != null ? (sym.SecurityFamily.Exchange as Exchange).ID : 0;
                    gt.Rows[i][EXCHANGE] = sym.SecurityFamily != null ? (sym.SecurityFamily.Exchange as Exchange).EXCode : "未设置";

                    gt.Rows[i][UNDERLAYINGID] = sym.underlaying_fk;
                    gt.Rows[i][UNDERLAYING] = sym.ULSymbol != null ? sym.ULSymbol.Symbol : "无";

                    gt.Rows[i][UNDERLAYINGSYMBOLID] = sym.underlayingsymbol_fk;
                    gt.Rows[i][UNDERLAYINGSYMBOL] = sym.UnderlayingSymbol != null ? sym.UnderlayingSymbol.Symbol : "无";
                    gt.Rows[i][EXPIREMONTH] = sym.ExpireMonth;
                    gt.Rows[i][EXPIREDATE] = sym.ExpireDate;
                    gt.Rows[i][TRADEABLE] = sym.Tradeable;
                    gt.Rows[i][TRADEABLETITLE] = GetTradeableTitle(sym.Tradeable);
                }
            }
        }



        #region 表格
        #region 显示字段

        const string ID = "全局ID";
        const string SYMBOL = "合约";
        
        const string ENTRYCOMMISSION = "开仓手续费";
        const string EXITCOMMISSION = "平仓手续费";
        const string MARGIN = "保证金";
        const string EXTRAMARGIN = "额外保证金";
        const string MAINTANCEMARGIN = "过夜保证金";
        const string SECCODE = "品种字头";
        const string SECTYPE = "SecType";
        const string EXCHANGEID = "ExchangeID";
        const string EXCHANGE = "交易所";
        const string UNDERLAYINGID = "UnderLayingID";
        const string UNDERLAYING = "异化底层证券";
        const string UNDERLAYINGSYMBOLID = "UnderLayingSymbolID";
        const string UNDERLAYINGSYMBOL = "底层证券";
        const string EXPIREMONTH = "到期月份";
        const string EXPIREDATE = "到期日";
        const string TRADEABLE = "TRADEABLE";
        const string TRADEABLETITLE = "是否可交易";


        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = symgrid;
            grid.ShowRowHeaderColumn = false;//显示每行的头部
            grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            grid.EnableHotTracking = true;

            grid.AllowAddNewRow = false;//不允许增加新行
            grid.AllowDeleteRow = false;//不允许删除行
            grid.AllowEditRow = false;//不允许编辑行
            grid.AllowRowResize = false;
            //grid.EnableSorting = false;
            grid.TableElement.TableHeaderHeight = Globals.HeaderHeight;
            grid.TableElement.RowHeight = Globals.RowHeight;

            grid.EnableAlternatingRowColor = true;//隔行不同颜色
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 

        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(ID);//
            gt.Columns.Add(SYMBOL);//
            gt.Columns.Add(ENTRYCOMMISSION);//
            gt.Columns.Add(EXITCOMMISSION);//
            gt.Columns.Add(MARGIN);//
            gt.Columns.Add(EXTRAMARGIN);//
            gt.Columns.Add(MAINTANCEMARGIN);//
            gt.Columns.Add(SECCODE);
            gt.Columns.Add(SECTYPE);
            gt.Columns.Add(EXCHANGEID);//
            gt.Columns.Add(EXCHANGE);//
            gt.Columns.Add(UNDERLAYINGID);//
            gt.Columns.Add(UNDERLAYING);//
            gt.Columns.Add(UNDERLAYINGSYMBOLID);//
            gt.Columns.Add(UNDERLAYINGSYMBOL);//
            gt.Columns.Add(EXPIREMONTH);
            gt.Columns.Add(EXPIREDATE);
            gt.Columns.Add(TRADEABLE);
            gt.Columns.Add(TRADEABLETITLE);
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = symgrid;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[EXCHANGEID].IsVisible = false;
            grid.Columns[UNDERLAYINGID].IsVisible = false;
            grid.Columns[UNDERLAYINGSYMBOLID].IsVisible = false;
            grid.Columns[EXCHANGEID].IsVisible = false;
            grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion


        bool _load = false;
        void RefreshSecurityQuery()
        {
            if (!_load) return;
            string sectype = string.Empty;
            
            if (cbsecurity.SelectedIndex == 0)
            {
                sectype = "*";
            }
            else
            {
                sectype = cbsecurity.SelectedValue.ToString();
            }
            
            string strFilter = string.Empty;


            if (cbsecurity.SelectedIndex == 0)
            {
                strFilter = string.Format(SECTYPE + " > '{0}'", sectype);
            }
            else
            {
                strFilter = string.Format(SECTYPE + " = '{0}'", sectype);
            }


            if (cbexchange.SelectedIndex != 0)
            {
                strFilter = string.Format(strFilter + " and " + EXCHANGEID + " = '{0}'", cbexchange.SelectedValue);
            }

            if (cbtradeable.SelectedIndex != 0)
            {
                int sv = int.Parse(cbtradeable.SelectedValue.ToString());
                if (sv == 1)
                {
                    strFilter = string.Format(strFilter + " and " + TRADEABLE + " = '{0}'", true);
                }
                else if(sv==-1)
                {
                    strFilter = string.Format(strFilter + " and " + TRADEABLE + " = '{0}'", false);
                }
            }
            Globals.Debug("strFilter:" + strFilter);
            datasource.Filter = strFilter;
        }


        //得到当前选择的行号
        private int CurrentSymbolID
        {

            get
            {
                if (symgrid.SelectedRows.Count > 0)
                {
                    return int.Parse(symgrid.SelectedRows[0].ViewInfo.CurrentRow.Cells[ID].Value.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }



        //通过行号得该行的Security
        SymbolImpl GetVisibleSymbol(int id)
        {
            SymbolImpl sym = null;
            if (symbolmap.TryGetValue(id, out sym))
            {
                return sym;
            }
            else
            {
                return null;
            }

        }


        private void SymbolForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void ReBindExchangeCombList()
        {
            Factory.IDataSourceFactory(cbexchange).BindDataSource(Globals.BasicInfoTracker.GetExchangeCombList(true));
        }

        private void cbsecurity_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            this.RefreshSecurityQuery();
        }

        private void cbexchange_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            this.RefreshSecurityQuery();
        }

        private void symgrid_DoubleClick(object sender, EventArgs e)
        {
            SymbolImpl symbol = GetVisibleSymbol(CurrentSymbolID);
            if (symbol != null)
            {
                SymbolEditForm fm = new SymbolEditForm();
                fm.Symbol = symbol;
                fm.ShowDialog();
            }
        }

        private void cbtradeable_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            this.RefreshSecurityQuery();
        }

        private void btnAddSymbol_Click(object sender, EventArgs e)
        {
            SymbolEditForm fm = new SymbolEditForm();
            fm.ShowDialog();
        }


        private void btnSyncSymbols_Click(object sender, EventArgs e)
        {
            SymbolSyncCTPForm fm = new SymbolSyncCTPForm();
            fm.GotSymbolImplEvent += new FutsMoniter.SymbolImplDel(fm_GotSymbolImplEvent);
            fm.ShowDialog();
        }

        void fm_GotSymbolImplEvent(SymbolImpl sym,bool islast)
        {
            //如果不存在合约则新增
            if (sym.security_fk == 0)
            {
                return;
            }
            if (Globals.BasicInfoTracker.GetSymbol(sym.Symbol) == null)
            {

                Globals.TLClient.ReqAddSymbol(sym);
            }
            else
            {
                Globals.TLClient.ReqUpdateSymbol(sym);
            }
        }

    }
}
