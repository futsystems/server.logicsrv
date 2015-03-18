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


namespace TradingLib.HistReport
{
    public partial class HistReport : MonitorControl
    {
        public HistReport()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();

            this.Load += new EventHandler(HistReport_Load);
        }

        void HistReport_Load(object sender, EventArgs e)
        {
            btnQryAgentReport.Click += new EventHandler(btnQryAgentReport_Click);
        }

        void btnQryAgentReport_Click(object sender, EventArgs e)
        {
            int mgrid = ctAgentList1.CurrentAgentFK;
            int start = Util.ToTLDate(start_agent.Value);
            int end = Util.ToTLDate(end_agent.Value);
            QryAentReport(start, end, mgrid);

        }

        void QryAentReport(int start, int end, int mgrid)
        {
            Clear();
            var data = new { start_date = start, end_date = end, manager_id = mgrid };
            this.JsonRequest("HistReportCentre", "QrySummaryViaSecCode", data);
        }



        /// <summary>
        /// CallbackAttr标注 注册一个回调函数
        /// </summary>
        /// <param name="result"></param>
        [CallbackAttr("HistReportCentre", "QrySummaryViaSecCode")]
        public void OnHelloworld(string result)
        {
            Util.Debug("got result:" + result, QSEnumDebugLevel.INFO);
            OnSummaryViaSecCode(result);
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
