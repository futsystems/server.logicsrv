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
using TradingLib.Mixins.JsonObject;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class ctCashTrans : UserControl,IEventBinder
    {
        public ctCashTrans()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();

            start.Value = Convert.ToDateTime(DateTime.Today.AddMonths(-1).ToString("yyyy-MM-01") + " 0:00:00");
            end.Value = DateTime.Now;

            

            //控件设置的属性在构造函数中是不起作用的，需要构造函数完毕在load事件内才发生作用，没有构造完毕则无法获得对应的参数 因此初始化工程中 如果有用到控件参数的需要延迟到load事件过程中执行
            //这里需要研究下控件的相关加载机制和事件先后顺序
            this.Load += new EventHandler(ctCashTrans_Load);
        }

        void ctCashTrans_Load(object sender, EventArgs e)
        {
            //全局事件回调
            Globals.RegIEventHandler(this);

            //绑定查询事件
            this.btnQryReport.Click += new EventHandler(btnQryReport_Click);
            cashgrid.CellFormatting += new DataGridViewCellFormattingEventHandler(cashgrid_CellFormatting);
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
                OnViewTypeChange();
            }
        }




        public void OnInit()
        {
            //调整空间可视
            OnViewTypeChange();

            //绑定事件
            if (ViewType == CashOpViewType.Account)
            {
                Globals.LogicEvent.RegisterCallback("MgrExchServer", "QueryAccountCashTrans", this.OnQryCashTrans);
            }
            else
            {
                Globals.LogicEvent.RegisterCallback("MgrExchServer", "QueryAgentCashTrans", this.OnQryCashTrans);
            }
        }

        public void OnDisposed()
        {
            if (ViewType == CashOpViewType.Account)
            {
                Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QueryAccountCashTrans", this.OnQryCashTrans);
            }
            else
            {
                Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QueryAgentCashTrans", this.OnQryCashTrans);
            }
        }

        /// <summary>
        /// 当viewtype调整时 调整控件可视
        /// </summary>
        void OnViewTypeChange()
        {
            //调整界面
            if (ViewType == CashOpViewType.Account)
            {
                cashgrid.Columns[MGRFK].Visible = false;
                ctAgentList1.Visible = false;
            }
            else
            {
                cashgrid.Columns[ACCOUNT].Visible = false;
                lbaccount.Visible = false;
                boxaccount.Visible = false;
            }
        }


        void OnQryCashTrans(string jsonstr)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            JsonWrapperCasnTrans[] obj = MoniterUtils.ParseJsonResponse<JsonWrapperCasnTrans[]>(jsonstr);
            if (obj != null)
            {
                //JsonWrapperCasnTrans[] obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCasnTrans[]>(jd["Playload"].ToJson());
                foreach (JsonWrapperCasnTrans c in obj)
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
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = cashgrid;

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
            gt.Columns.Add(ID);//1
            gt.Columns.Add(SETTLEDAY);//2
            gt.Columns.Add(DATETIME);//3
            gt.Columns.Add(ACCOUNT);//4
            gt.Columns.Add(MGRFK);//5
            gt.Columns.Add(OPERATION);//6
            gt.Columns.Add(AMOUNT);//
            gt.Columns.Add(REF);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = cashgrid;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            grid.Columns[ID].Width = 40;
            grid.Columns[SETTLEDAY].Width = 80;
            grid.Columns[DATETIME].Width = 120;
            grid.Columns[ACCOUNT].Width = 80;
            grid.Columns[OPERATION].Width = 40;

            if (ViewType == CashOpViewType.Account)
            {
                cashgrid.Columns[MGRFK].Visible = false;
            }
            else
            {
                cashgrid.Columns[ACCOUNT].Visible = false;
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
                    if (string.IsNullOrEmpty(boxaccount.Text))
                    {
                        fmConfirm.Show("请输入正确的交易帐户");
                        return;
                    }

                    Globals.TLClient.ReqQryAccountCashTrans(boxaccount.Text, Util.ToTLDateTime(start.Value), Util.ToTLDateTimeEnd(end.Value));
                }
                else
                {
                    Globals.TLClient.ReqQryAgentCashTrans(ctAgentList1.CurrentAgentFK, Util.ToTLDateTime(start.Value), Util.ToTLDateTimeEnd(end.Value));
                }
            }
        }


        void cashgrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                string op = cashgrid[6, e.RowIndex].Value.ToString();
                if (op.Equals("入金"))
                {
                    e.CellStyle.ForeColor = UIGlobals.LongSideColor;
                }
                else
                {
                    e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                }
            }
            if (e.ColumnIndex == 5 || e.ColumnIndex == 6)
            {
                e.CellStyle.Font = UIGlobals.BoldFont;
            }
        }
    }
}
