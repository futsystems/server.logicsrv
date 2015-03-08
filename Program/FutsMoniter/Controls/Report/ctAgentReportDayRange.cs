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


namespace FutsMoniter
{
    public partial class ctAgentReportDayRange : UserControl,IEventBinder
    {
        public ctAgentReportDayRange()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            this.Load += new EventHandler(ctAgentReportDayRange_Load);
            start.Value = Convert.ToDateTime(DateTime.Today.AddMonths(-1).ToString("yyyy-MM-01") + " 0:00:00");
            end.Value = DateTime.Now;
            

        }

        void ctAgentReportDayRange_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
            btnQryReport.Click += new EventHandler(btnQryReport_Click);
        }

        public void OnInit()
        {
            Globals.LogicEvent.RegisterCallback("FinServiceCentre", "QryTotalReportDayRange", this.OnTotalReport);

        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("FinServiceCentre", "QryTotalReportDayRange", this.OnTotalReport);
        }


        public void Clear()
        {
            totalgrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }

        public void OnTotalReport(string jsonstr, bool islast)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            JsonWrapperToalReport[] objlist = MoniterUtils.ParseJsonResponse<JsonWrapperToalReport[]>(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            if (objlist != null)
            {
                //JsonWrapperToalReport[] objlist = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperToalReport[]>(jd["Playload"].ToJson());
                foreach (JsonWrapperToalReport obj in objlist)
                {
                    InvokeGotJsonWrapperTotalReport(obj);
                }
            }
        }

        delegate void JsonWrapperToalReportDel(JsonWrapperToalReport report);
        void InvokeGotJsonWrapperTotalReport(JsonWrapperToalReport report)
        {
            if (InvokeRequired)
            {
                Invoke(new JsonWrapperToalReportDel(InvokeGotJsonWrapperTotalReport), new object[] { report });
            }
            else
            {
                DataRow r = gt.Rows.Add(report.Agent_FK);
                int i = gt.Rows.Count - 1;//得到新建的Row号

                gt.Rows[i][ID] = report.Agent_FK;
                gt.Rows[i][AGENTNAME] = report.AgentName;
                //gt.Rows[i][MOBILE] = report.Mobile;
                //gt.Rows[i][QQ] = report.QQ;
                gt.Rows[i][SETTLEDAY] = report.SettleDay;
                gt.Rows[i][TOTALFEE] = report.TotalFee;
                gt.Rows[i][AGENTFEE] = report.AgentFee;
                gt.Rows[i][CUSTOMERPROFIT] = report.AgentProfit;
                gt.Rows[i][COMMISSIONPROFIT] = report.CommissionProfit;
                gt.Rows[i][TOTALPROFIT] = report.CommissionProfit + report.AgentProfit;
            }
        }

        #region 表格
        #region 显示字段

        const string ID = "代理全局ID";
        const string AGENTNAME = "代理名称";
        const string MOBILE = "代理手机";
        const string QQ = "代理QQ";
        const string SETTLEDAY = "结算日";
        const string TOTALFEE = "客户收费";
        const string AGENTFEE = "代理成本";
        const string CUSTOMERPROFIT = "直客利润";
        const string COMMISSIONPROFIT = "代理提成";
        const string TOTALPROFIT = "利润总和";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = totalgrid;

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
            gt.Columns.Add(ID);//
            gt.Columns.Add(AGENTNAME);//
            //gt.Columns.Add(MOBILE);//
            //gt.Columns.Add(QQ);//
            gt.Columns.Add(SETTLEDAY);//
            gt.Columns.Add(TOTALFEE);//
            gt.Columns.Add(AGENTFEE);//
            gt.Columns.Add(CUSTOMERPROFIT);
            gt.Columns.Add(COMMISSIONPROFIT);
            gt.Columns.Add(TOTALPROFIT);

        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = totalgrid;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[EXCHANGEID].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            //grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion



        private void btnQryReport_Click(object sender, EventArgs e)
        {
            this.Clear();
            Globals.TLClient.ReqQryTotalReportByDayRange(ctAgentList1.CurrentAgentFK, Util.ToTLDate(start.Value), Util.ToTLDate(end.Value));

        }
    }
}
