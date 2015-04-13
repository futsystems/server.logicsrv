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
using TradingLib.Protocol;

namespace TradingLib.HistReport
{
    public partial class HistReport_SummaryAccount : UserControl
    {
        public event Action<string, int, int> QryAccountEvent;

        public HistReport_SummaryAccount()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
            this.Load += new EventHandler(HistReport_SummaryAccount_Load);
        }

        void HistReport_SummaryAccount_Load(object sender, EventArgs e)
        {
            btnQry.Click += new EventHandler(btnQry_Click);
        }

        void btnQry_Click(object sender, EventArgs e)
        {
            int start = Util.ToTLDate(start_agent.Value);
            int end = Util.ToTLDate(end_agent.Value);
            string acct = account.Text;

            if (QryAccountEvent != null)
            {
                QryAccountEvent(acct, start, end);
            }
        }


        


        #region 表格
        #region 显示字段

        const string ACCOUNT = "交易帐户";
        const string SEC_CODE = "品种编码";
        const string TOTAL_REALIZEDPL = "累计平仓盈亏";
        const string TOTAL_COMMISSION = "累计手续费";
        const string TOTAL_VOLUME = "累计交易量";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = summaryAccountGrid;

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
            gt.Columns.Add(ACCOUNT);//
            gt.Columns.Add(SEC_CODE);//
            gt.Columns.Add(TOTAL_REALIZEDPL);//
            gt.Columns.Add(TOTAL_COMMISSION);//
            gt.Columns.Add(TOTAL_VOLUME);
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = summaryAccountGrid;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[EXCHANGEID].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            //grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion


        public void OnSummaryAccount(string jsonstr)
        {
            SummaryAccount obj = MoniterControl.MoniterHelper.ParseJsonResponse<SummaryAccount>(jsonstr);
            if (obj != null)
            {
                lbAccount.Text = obj.Account;
                lbCashIn.Text = obj.CashIn.ToString();
                lbCashOut.Text = obj.CashOut.ToString();

                foreach (var  item in obj.Items)
                {
                    InvokeGotSummaryAccount(item);
                }
            }
        }



        void InvokeGotSummaryAccount(SummaryAccountItem report)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SummaryAccountItem>(InvokeGotSummaryAccount), new object[] { report });
            }
            else
            {
                DataRow r = gt.Rows.Add(0);
                int i = gt.Rows.Count - 1;//得到新建的Row号

                gt.Rows[i][ACCOUNT] = report.Account;
                gt.Rows[i][SEC_CODE] = report.SecCode;
                gt.Rows[i][TOTAL_REALIZEDPL] = report.RealizedPL;
                gt.Rows[i][TOTAL_COMMISSION] = report.Commission;
                gt.Rows[i][TOTAL_VOLUME] = report.Volume;
            }
        }

        public void Clear()
        {
            summaryAccountGrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }

    }
}
