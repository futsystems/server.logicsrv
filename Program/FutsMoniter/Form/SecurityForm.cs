using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
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
    public partial class SecurityForm : Telerik.WinControls.UI.RadForm
    {
        public SecurityForm()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            Factory.IDataSourceFactory(cbsecurity).BindDataSource(UIUtil.GetEnumValueObjects<SecurityType>(true));
            Factory.IDataSourceFactory(cbtradeable).BindDataSource(Utils.GetTradeableCBList(true));

            _load = true;
        }

        public bool AnySecurity {
            get
            {
                return securitymap.Count > 0;
            }
        }


        Dictionary<int, SecurityFamilyImpl> securitymap = new Dictionary<int, SecurityFamilyImpl>();
        Dictionary<int, int> securityidxmap = new Dictionary<int, int>();
        int SecurityFamilyIdx(int id)
        {
            
            int rowid = -1;
            if (securityidxmap.TryGetValue(id, out rowid))
            {
                return rowid;
            }
            else
            {
                return -1;
            }
        }

        public void GotSecurity(SecurityFamilyImpl sec)
        {
            InvokGotSecurity(sec);
        }

        string GetTradeableTitle(bool tradeable)
        {
            return tradeable ? "是" : "否";
        }


        delegate void SecurityFamilyDel(SecurityFamilyImpl sec);
        void InvokGotSecurity(SecurityFamilyImpl sec)
        {
            if (InvokeRequired)
            {
                Invoke(new SecurityFamilyDel(InvokGotSecurity), new object[] { sec });
            }
            else
            {
                int r = SecurityFamilyIdx(sec.ID);
                if(r ==-1)
                {
                    gt.Rows.Add(sec.ID);
                    int i = gt.Rows.Count-1;

                    securitymap.Add(sec.ID,sec);
                    securityidxmap.Add(sec.ID,i);

                    gt.Rows[i][CCODE] =sec.Code;
                    gt.Rows[i][NAME] = sec.Name;
                    gt.Rows[i][CURRENCY] = sec.Currency;
                    gt.Rows[i][TYPE] = sec.Type;
                    gt.Rows[i][MULTIPLE] = sec.Multiple;
                    gt.Rows[i][PRICETICK] = sec.PriceTick;
                    //gt.Rows[i][TRADABLE] = sec.TradeAble;;
                    gt.Rows[i][ENTRYCOMMISSION] = sec.EntryCommission;
                    gt.Rows[i][EXITCOMMISSION] = sec.ExitCommission;
                    gt.Rows[i][MARGIN] = sec.Margin;
                    gt.Rows[i][EXTRAMARGIN] = sec.ExtraMargin;
                    gt.Rows[i][MAINTANCEMARGIN] = sec.MaintanceMargin;
                    gt.Rows[i][EXCHANGEID] = sec.exchange_fk;
                    sec.Exchange = Globals.BasicInfoTracker.GetExchange(sec.exchange_fk);
                    gt.Rows[i][EXCHANGE] =sec.Exchange!=null?sec.Exchange.EXCode:"未设置";
                    gt.Rows[i][UNDERLAYINGID] = sec.underlaying_fk;
                    gt.Rows[i][UNDERLAYING] = "";
                    gt.Rows[i][MARKETTIMEID] = sec.mkttime_fk;
                    sec.MarketTime = Globals.BasicInfoTracker.GetMarketTime(sec.mkttime_fk);
                    gt.Rows[i][MARKETTIME] = sec.MarketTime != null?sec.MarketTime.Name:"未设置";

                    gt.Rows[i][TRADEABLE] = sec.Tradeable ? 1 : -1;
                    gt.Rows[i][TRADEABLETITLE] = GetTradeableTitle(sec.Tradeable);
                }
                else
                {
                    int i = r;
                    //获得当前实例
                    SecurityFamilyImpl target = Globals.BasicInfoTracker.GetSecurity(sec.ID);
                    if (target != null)
                    {
                        //1.更新当前实例
                        target.Code = sec.Code;
                        target.Name = sec.Name;
                        target.Currency = sec.Currency;
                        target.Type = sec.Type;

                        target.exchange_fk = sec.exchange_fk;
                        target.Exchange = Globals.BasicInfoTracker.GetExchange(sec.exchange_fk);

                        target.mkttime_fk = sec.mkttime_fk;
                        target.MarketTime = Globals.BasicInfoTracker.GetMarketTime(sec.mkttime_fk);

                        target.underlaying_fk = sec.underlaying_fk;
                        //target.UnderLaying = BasicTracker.SecurityTracker[target.underlaying_fk];

                        target.Multiple = sec.Multiple;
                        target.PriceTick = sec.PriceTick;
                        target.EntryCommission = sec.EntryCommission;
                        target.ExitCommission = sec.ExitCommission;
                        target.Margin = sec.Margin;
                        target.ExtraMargin = sec.ExtraMargin;
                        target.MaintanceMargin = sec.MaintanceMargin;
                        target.Tradeable = sec.Tradeable;
                    
                        //2.更新表格
                        gt.Rows[i][CCODE] = target.Code;
                        gt.Rows[i][NAME] = target.Name;
                        gt.Rows[i][CURRENCY] = target.Currency;
                        gt.Rows[i][TYPE] = target.Type;
                        gt.Rows[i][MULTIPLE] = target.Multiple;
                        gt.Rows[i][PRICETICK] = target.PriceTick;
                        //gt.Rows[i][TRADABLE] = target.TradeAble; ;
                        gt.Rows[i][ENTRYCOMMISSION] = target.EntryCommission;
                        gt.Rows[i][EXITCOMMISSION] = target.ExitCommission;
                        gt.Rows[i][MARGIN] = target.Margin;
                        gt.Rows[i][EXTRAMARGIN] = target.ExtraMargin;
                        gt.Rows[i][MAINTANCEMARGIN] = target.MaintanceMargin;
                        gt.Rows[i][EXCHANGEID] = target.exchange_fk;
                        gt.Rows[i][EXCHANGE] = target.Exchange != null ? target.Exchange.EXCode : "未设置";
                        gt.Rows[i][UNDERLAYINGID] = target.underlaying_fk;
                        gt.Rows[i][UNDERLAYING] = "";
                        gt.Rows[i][MARKETTIMEID] = target.mkttime_fk;
                        gt.Rows[i][MARKETTIME] = target.MarketTime != null ? target.MarketTime.Name : "未设置";

                        gt.Rows[i][TRADEABLE] = sec.Tradeable?1:-1;
                        gt.Rows[i][TRADEABLETITLE] = GetTradeableTitle(sec.Tradeable);
                    }

                
                }
            }
        }

        #region 表格
        #region 显示字段

        const string ID = "全局ID";
        const string CCODE = "品种编码";
        const string NAME = "名称";
        const string CURRENCY = "货币";
        const string TYPE = "证券品种";
        const string MULTIPLE = "乘数";
        const string PRICETICK = "最小价格变动";
        //const string TRADABLE = "是否交易";
        const string ENTRYCOMMISSION = "开仓手续费";
        const string EXITCOMMISSION = "平仓手续费";
        const string MARGIN = "保证金";
        const string EXTRAMARGIN = "额外保证金";
        const string MAINTANCEMARGIN = "过夜保证金";
        const string EXCHANGEID = "ExchangeID";
        const string EXCHANGE = "交易所";
        const string UNDERLAYINGID = "UnderLayingID";
        const string UNDERLAYING = "底层证券";
        const string MARKETTIMEID = "MarketTimeID";
        const string MARKETTIME = "市场交易时间";
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
            Telerik.WinControls.UI.RadGridView grid = secgrid;
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
            gt.Columns.Add(CCODE);//
            gt.Columns.Add(NAME);//
            gt.Columns.Add(CURRENCY);//
            gt.Columns.Add(TYPE);//
            gt.Columns.Add(MULTIPLE);//
            gt.Columns.Add(PRICETICK);//
            //gt.Columns.Add(TRADABLE);//
            gt.Columns.Add(ENTRYCOMMISSION);//
            gt.Columns.Add(EXITCOMMISSION);//
            gt.Columns.Add(MARGIN);//
            gt.Columns.Add(EXTRAMARGIN);//
            gt.Columns.Add(MAINTANCEMARGIN);//
            gt.Columns.Add(EXCHANGEID);//
            gt.Columns.Add(EXCHANGE);//
            gt.Columns.Add(UNDERLAYINGID);//
            gt.Columns.Add(UNDERLAYING);//
            gt.Columns.Add(MARKETTIMEID);//
            gt.Columns.Add(MARKETTIME);//
            gt.Columns.Add(TRADEABLE);
            gt.Columns.Add(TRADEABLETITLE);
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = secgrid;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;

            
            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            grid.Columns[EXCHANGEID].IsVisible = false;
            grid.Columns[UNDERLAYINGID].IsVisible = false;
            grid.Columns[MARKETTIMEID].IsVisible = false;
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
                strFilter = string.Format(TYPE + " > '{0}'", sectype);
            }
            else
            {
                strFilter = string.Format(TYPE + " = '{0}'", sectype);
            }

            if (cbexchange.SelectedIndex != 0)
            {
                strFilter = string.Format(strFilter + " and " + EXCHANGEID + " = '{0}'",cbexchange.SelectedValue);
            }

            if (cbtradeable.SelectedIndex != 0)
            {
                int sv = int.Parse(cbtradeable.SelectedValue.ToString());
                strFilter = string.Format(strFilter + " and " + TRADEABLE + " = '{0}'", sv);
               
            }
            Globals.Debug(strFilter);
            datasource.Filter = strFilter;
        }
        private void SecurityForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        //得到当前选择的行号
        private int CurrentSecurityID
        {

            get
            {
                if (secgrid.SelectedRows.Count > 0)
                {
                    return int.Parse(secgrid.SelectedRows[0].ViewInfo.CurrentRow.Cells[ID].Value.ToString());
                }
                else
                {
                    return 0;
                }
            }
        }



        //通过行号得该行的Security
        SecurityFamilyImpl GetVisibleSecurity(int id)
        {
            SecurityFamilyImpl sec = null;
            if (securitymap.TryGetValue(id, out sec))
            {
                return sec;
            }
            else
            {
                return null;
            }
            
        }






        public void ReBindExchangeCombList()
        {
            Factory.IDataSourceFactory(cbexchange).BindDataSource(Globals.BasicInfoTracker.GetExchangeCombList(true));
        }

        private void cbsecurity_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshSecurityQuery();
        }

        private void cbexchange_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshSecurityQuery();
        }

        private void secgrid_DoubleClick(object sender, EventArgs e)
        {
            SecurityFamilyImpl sec = GetVisibleSecurity(CurrentSecurityID);
            if (sec != null)
            {
                SecEditForm fm = new SecEditForm();
                fm.Security = sec;
                fm.ShowDialog();
            }
        }

        private void cbtradeable_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshSecurityQuery();
        }

        private void btnAddSecurity_Click(object sender, EventArgs e)
        {
            SecEditForm fm = new SecEditForm();
            fm.ShowDialog();
        }

    }
}
