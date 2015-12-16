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
    public partial class HistReport
    {


        #region 表格
        #region 显示字段

        //const string ID = "代理全局ID";
        const string MANAGER_ID = "管理域编号";
        const string SEC_CODE = "品种编码";
        const string TOTAL_SIZE = "累计交易量";
        const string TOTAL_COMMISSION = "累计手续费";
        const string TOTAL_REALIZEDPROFIT = "累计平仓盈亏";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = gridAgentSummary;

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
            gt.Columns.Add(MANAGER_ID);//
            gt.Columns.Add(SEC_CODE);//
            gt.Columns.Add(TOTAL_SIZE);//
            gt.Columns.Add(TOTAL_COMMISSION);//
            gt.Columns.Add(TOTAL_REALIZEDPROFIT);
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = gridAgentSummary;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[EXCHANGEID].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            //grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion


        public void OnSummaryViaSecCode(string jsonstr)
        {
            SummaryViaSec[] objlist = MoniterControl.MoniterHelper.ParseJsonResponse<SummaryViaSec[]>(jsonstr);
            if (objlist != null)
            {
                foreach (SummaryViaSec obj in objlist)
                {
                    InvokeGotSummaryViaSecCode(obj);
                }
            }
        }



        void InvokeGotSummaryViaSecCode(SummaryViaSec report)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SummaryViaSec>(InvokeGotSummaryViaSecCode), new object[] { report });
            }
            else
            {
                DataRow r = gt.Rows.Add(0);
                int i = gt.Rows.Count - 1;//得到新建的Row号

                gt.Rows[i][MANAGER_ID] = report.Manager_ID;
                gt.Rows[i][SEC_CODE] = report.Sec_Code;
                gt.Rows[i][TOTAL_SIZE] = report.Total_Size;
                gt.Rows[i][TOTAL_COMMISSION] = report.Total_Commission;
                gt.Rows[i][TOTAL_REALIZEDPROFIT] = report.Total_Profit;
            }
        }

        public void Clear()
        {
            gridAgentSummary.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }



    }
}
