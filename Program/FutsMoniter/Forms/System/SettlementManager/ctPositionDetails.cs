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


namespace  FutsMoniter
{
    public partial class ctPositionDetails : UserControl
    {
        public ctPositionDetails()
        {
            InitializeComponent();
        }


        #region 表格
        #region 显示字段

        const string SETTLEDAY = "结算日";
        const string ACCOUNT = "交易帐户";
        const string OPENDATE = "开仓日期";
        const string OPENPRICE = "开仓价";
        const string SIDE = "方向";
        const string TRADEID = "成交编号";
        const string SYMBOL = "合约";
        const string BROKER = "成交通道";
        const string BREED = "持仓源";
        const string PRICE = "结算价";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = gridPositionDetail;

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
            //gt.Columns.Add(SETTLEDAY);//
            //gt.Columns.Add(SYMBOL);//
            //gt.Columns.Add(PRICE);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = gridPositionDetail;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

        }





        #endregion

    }
}
