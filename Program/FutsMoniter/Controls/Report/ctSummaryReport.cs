using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Mixins.JsonObject;
using TradingLib.Mixins.LitJson;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class ctSummaryReport : UserControl
    {
        public ctSummaryReport()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
            ctGridExport1.Grid = totalgrid;
            Globals.RegInitCallback(OnInitFinished);
            start.Value = Convert.ToDateTime(DateTime.Today.AddMonths(-1).ToString("yyyy-MM-01") + " 0:00:00");
            end.Value = DateTime.Now;
        }

        public void Clear()
        {
            totalgrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }

        public void OnTotalReport(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperToalReport[] objlist = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperToalReport[]>(jd["Playload"].ToJson());
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
                //gt.Rows[i][SETTLEDAY] = report.SettleDay;
                gt.Rows[i][TOTALFEE] = report.TotalFee;
                gt.Rows[i][AGENTFEE] = report.AgentFee;
                gt.Rows[i][AGENTPROFIT] = report.AgentProfit;
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
        const string AGENTPROFIT = "代理利润";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = totalgrid;
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
            gt.Columns.Add(AGENTNAME);//
            //gt.Columns.Add(MOBILE);//
            //gt.Columns.Add(QQ);//
            //gt.Columns.Add(SETTLEDAY);//
            gt.Columns.Add(TOTALFEE);//
            gt.Columns.Add(AGENTFEE);//
            gt.Columns.Add(AGENTPROFIT);//

        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = totalgrid;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[EXCHANGEID].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            //grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion

        /// <summary>
        /// 用于响应初始化完成事件
        /// 初始化完成后 会针对初始化得到的数据去填充或者修改界面数据
        /// </summary>
        void OnInitFinished()
        {
            if (Globals.RootRight)
            {
                Factory.IDataSourceFactory(agent).BindDataSource(Globals.BasicInfoTracker.GetBaseManagerCombList(true));
            }
            else
            {
                Factory.IDataSourceFactory(agent).BindDataSource(Globals.BasicInfoTracker.GetBaseManagerCombList());
            }
            Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "QrySummaryReport", this.OnTotalReport);
        }


        private void btnQryReport_Click(object sender, EventArgs e)
        {
            this.Clear();
            Globals.TLClient.ReqQrySummaryReport(int.Parse(agent.SelectedValue.ToString()), Util.ToTLDate(start.Value), Util.ToTLDate(end.Value));
        }

 
    }
}
