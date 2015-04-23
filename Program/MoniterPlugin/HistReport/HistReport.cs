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
using TradingLib.MoniterControl;
using TradingLib.Protocol;

namespace TradingLib.HistReport
{

    [MoniterControlAttr("HistReport","统计报表",QSEnumControlPlace.WorkSpace)]
    public partial class HistReport : MonitorControl
    {
        public HistReport()
        {
            InitializeComponent();


            this.Load += new EventHandler(HistReport_Load);
        }

        void HistReport_Load(object sender, EventArgs e)
        {
            histReport_SummaryAccount1.QryAccountEvent += new Action<string, int, int>(histReport_SummaryAccount1_QryAccountEvent);
            histReport_SummaryAgent1.QryAgentEvent += new Action<int, int, int>(histReport_SummaryAgent21_QryAgentEvent);
        }

        void histReport_SummaryAgent21_QryAgentEvent(int arg1, int arg2, int arg3)
        {
            histReport_SummaryAgent1.Clear();
            QryAentReport(arg2, arg3, arg1);
        }

        void histReport_SummaryAccount1_QryAccountEvent(string arg1, int arg2, int arg3)
        {
            histReport_SummaryAccount1.Clear();
            QryAccountReport(arg2, arg3, arg1);
        }


        /// <summary>
        /// 查询代理统计
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="mgrid"></param>
        void QryAentReport(int start, int end, int mgrid)
        {
            var data = new { start_date = start, end_date = end, manager_id = mgrid };
            this.JsonRequest("HistReportCentre", "QrySummaryViaSecCode", data);
        }

        /// <summary>
        /// 查询交易帐户统计
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="account"></param>
        void QryAccountReport(int start, int end, string account)
        {
            var data = new { start_date = start, end_date = end, account = account };
            this.JsonRequest("HistReportCentre", "QryAccountSummary", data);
        }

        /// <summary>
        /// CallbackAttr标注 注册一个回调函数
        /// </summary>
        /// <param name="result"></param>
        [CallbackAttr("HistReportCentre", "Statistic")]
        public void OnStatisticNotify(string result)
        {
            Util.Debug("got result:" + result, QSEnumDebugLevel.INFO);
            DomainStatistic ds = MoniterHelper.ParseJsonResponse<DomainStatistic>(result);
            if (ds != null)
            {
                InvokeGotDomainStatistic(ds);
            }
        }

        void InvokeGotDomainStatistic(DomainStatistic ds)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<DomainStatistic>(InvokeGotDomainStatistic), new object[] { ds });
            }
            else
            {
                lbaccnumtotal.Text = ds.AccNumTotal.ToString();
                lbaccnumtraded.Text = ds.AccNumTraded.ToString();
                lbaccnumregisted.Text = ds.MaxAccNumRegisted.ToString();
                lbequity.Text = Util.FormatDecimal(ds.Equity);
                lbcredit.Text = Util.FormatDecimal(ds.Credit);
                lbcashin.Text = Util.FormatDecimal(ds.CashIn);
                lbcashout.Text = Util.FormatDecimal(ds.CashOut);
                lbcommission.Text = Util.FormatDecimal(ds.Commission);
                lbrealizedpl.Text = Util.FormatDecimal(ds.RealizedPL);
                lbunrealizedpl.Text = Util.FormatDecimal(ds.UnRealizedPL);

                lbmargin.Text = Util.FormatDecimal(ds.MarginTotal);
                lbmaxmargin.Text = Util.FormatDecimal(ds.MaxMarginTotal);
                lblonghod.Text = ds.LongPositionHold.ToString();
                lbshorthold.Text = ds.ShortPositionHold.ToString();
                lbmaxlonghold.Text = ds.MaxLongPositionHold.ToString();
                lbmaxshorthold.Text = ds.MaxShortPositionHold.ToString();
            }
        }

        /// <summary>
        /// CallbackAttr标注 注册一个回调函数
        /// </summary>
        /// <param name="result"></param>
        [CallbackAttr("HistReportCentre", "QrySummaryViaSecCode")]
        public void OnQrySummaryViaSecCode(string result)
        {
            Util.Debug("got result:" + result, QSEnumDebugLevel.INFO);
            histReport_SummaryAgent1.OnSummaryViaSecCode(result);
        }

        [CallbackAttr("HistReportCentre", "QryAccountSummary")]
        public void OnQrySummaryAccount(string result)
        {
            Util.Debug("got result:" + result, QSEnumDebugLevel.INFO);
            histReport_SummaryAccount1.OnSummaryAccount(result);
        }

        public override string Title
        {
            get
            {
                return "统计报表";
            }
        }


        
    }
}
