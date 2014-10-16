using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;
using TradingLib.Mixins.LitJson;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace FutsMoniter
{
    public partial class ctCashTrans : UserControl
    {
        public ctCashTrans()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();


            start.Value = Convert.ToDateTime(DateTime.Today.AddMonths(-1).ToString("yyyy-MM-01") + " 0:00:00");
            end.Value = DateTime.Now;
            ctGridExport1.Grid = cashgrid;
            this.Disposed += new EventHandler(ctCashTrans_Disposed);
            this.Load += new EventHandler(ctCashTrans_Load);

        }

        void ctCashTrans_Load(object sender, EventArgs e)
        {
            if (ViewType == CashOpViewType.Account)
            {
                cashgrid.Columns[MGRFK].IsVisible = false;
                ctAgentList1.Visible = false;
            }
            else
            {
                cashgrid.Columns[ACCOUNT].IsVisible = false;
                lbaccount.Visible = false;
                boxaccount.Visible = false;
            }

            if (Globals.CallbackCentreReady)
            {
                if (ViewType == CashOpViewType.Account)
                {
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QueryAccountCashTrans", this.OnQryCashTrans);
                }
                else
                {
                    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QueryAgentCashTrans", this.OnQryCashTrans);
                }
            }

        }

        void ctCashTrans_Disposed(object sender, EventArgs e)
        {
            if (Globals.EnvReady)
            {
                if (ViewType == CashOpViewType.Account)
                {
                    Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QueryAccountCashTrans", this.OnQryCashTrans);
                }
                else
                {
                    Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QueryAgentCashTrans", this.OnQryCashTrans);
                }
            }
        }

        void OnQryCashTrans(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCasnTrans[] obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCasnTrans[]>(jd["Playload"].ToJson());
                foreach(JsonWrapperCasnTrans c in obj)
                {
                    GotCashTrans(c);
                }
            }


        }

        delegate void del1(JsonWrapperCasnTrans trans);
        void GotCashTrans(JsonWrapperCasnTrans trans)
        {
            if (InvokeRequired)
            {
                Invoke(new del1(GotCashTrans), new object[] { trans });
            }
            else
            {
                DataRow r = gt.Rows.Add("");
                int i = gt.Rows.Count - 1;//得到新建的Row号
                gt.Rows[i][ID] = trans.ID;
                gt.Rows[i][SETTLEDAY] = trans.Settleday;
                gt.Rows[i][DATETIME] = trans.DateTime;
                gt.Rows[i][ACCOUNT] = trans.Account;
                gt.Rows[i][MGRFK] = trans.mgr_fk;
                gt.Rows[i][OPERATION] = trans.Amount > 0 ? "入金" : "出金";
                gt.Rows[i][AMOUNT] = trans.Amount;
                gt.Rows[i][REF] = trans.TransRef;

            }
        }

        //属性获得和设置
        [DefaultValue(CashOpViewType.Account)]
        CashOpViewType _viewType = CashOpViewType.Account;
        public CashOpViewType ViewType
        {
            get
            {
                return _viewType;
            }
            set
            {
                _viewType = value;
            }
        }


        public void Clear()
        {
            cashgrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }


        #region 表格
        #region 显示字段

        const string ID = "ID";
        const string SETTLEDAY = "结算日";
        const string DATETIME = "时间";
        const string ACCOUNT = "帐户";
        const string MGRFK = "代理域ID";
        const string OPERATION = "操作";
        const string AMOUNT = "金额";
        const string REF = "出入金流水号";
        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = cashgrid;
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
            gt.Columns.Add(SETTLEDAY);//
            gt.Columns.Add(DATETIME);//
            gt.Columns.Add(ACCOUNT);//
            gt.Columns.Add(MGRFK);//
            gt.Columns.Add(OPERATION);//
            gt.Columns.Add(AMOUNT);//
            gt.Columns.Add(REF);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = cashgrid;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[KEY].IsVisible = false;
            ////grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[SOURCE].IsVisible = false;
            //grid.Columns[STATUS].IsVisible = false;
            //grid.Columns[OPERATION].IsVisible = false;
            if (ViewType == CashOpViewType.Account)
            {
                cashgrid.Columns[MGRFK].IsVisible = false;
            }
            else
            {
                cashgrid.Columns[ACCOUNT].IsVisible = false;
            }
        }
        #endregion

        private void btnQryReport_Click(object sender, EventArgs e)
        {
            this.Clear();
            if (Globals.EnvReady)
            {
                if (ViewType == CashOpViewType.Account)
                {
                    Globals.TLClient.ReqQryAccountCashTrans("", Util.ToTLDateTime(start.Value), Util.ToTLDateTimeEnd(end.Value));
                }
                else
                {
                    Globals.TLClient.ReqQryAgentCashTrans(ctAgentList1.CurrentAgentFK, Util.ToTLDateTime(start.Value), Util.ToTLDateTimeEnd(end.Value));
                }
            }
        }

        private void cashgrid_CellFormatting(object sender, Telerik.WinControls.UI.CellFormattingEventArgs e)
        {
            try
            {
                if (e.CellElement.RowInfo is GridViewDataRowInfo)
                {
                    if (e.CellElement.ColumnInfo.Name == OPERATION)
                    {
                        decimal v = decimal.Parse((e.CellElement.RowInfo.Cells[AMOUNT].Value.ToString()));

                        if (v > 0)
                        {
                            e.CellElement.ForeColor = Color.Red;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else
                        {
                            e.CellElement.ForeColor = Color.Green;
                            e.CellElement.Font = UIGlobals.BoldFont;
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
