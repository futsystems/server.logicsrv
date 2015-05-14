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
using FutSystems.GUI;

namespace TradingLib.RaceMoniter
{
    public partial class ctRaceService : UserControl
    {

        public event Action<string, string> QryRaceEvent;
        public event Action<string> EliminateAccountEvent;
        public event Action<string> PrompotAccountEvent;
        public event Action<string> SignRaceAccountEvent;

        public ctRaceService()
        {
            InitializeComponent();

            SetPreferences();
            InitTable();
            BindToTable();

            MoniterControl.MoniterHelper.AdapterToIDataSource(status).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumAccountRaceStatus>(true));

            rsGrid.ContextMenuStrip = new ContextMenuStrip();
            rsGrid.ContextMenuStrip.Items.Add("报名参赛", null, new EventHandler(SignRaceAccount_Click));
            rsGrid.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            rsGrid.ContextMenuStrip.Items.Add("淘汰帐户",null,new EventHandler(EliminateAccount_Click));
            rsGrid.ContextMenuStrip.Items.Add("晋级帐户", null, new EventHandler(PromptAccount_Click));
            this.Load += new EventHandler(ctRaceService_Load);

        }


        //得到当前选择的行号
        private string CurrentAccount
        {
            get
            {
                int row = (rsGrid.SelectedRows.Count > 0 ? rsGrid.SelectedRows[0].Index : -1);
                return row == -1 ? string.Empty : (rsGrid[0, row].Value.ToString());
            }
        }


        /// <summary>
        /// 编辑某个交易帐号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EliminateAccount_Click(object sender, EventArgs e)
        {
            string account = CurrentAccount;
            if (string.IsNullOrEmpty(account))
            {
                MessageBox.Show("请选择交易帐户");
                return;
            }
            if (EliminateAccountEvent != null)
                EliminateAccountEvent(account);

            
        }

        /// <summary>
        /// 编辑某个交易帐号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SignRaceAccount_Click(object sender, EventArgs e)
        {
            string account = CurrentAccount;
            if (string.IsNullOrEmpty(account))
            {
                MessageBox.Show("请选择交易帐户");
                return;
            }
            if (SignRaceAccountEvent != null)
                SignRaceAccountEvent(account);


        }

        /// <summary>
        /// 编辑某个交易帐号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PromptAccount_Click(object sender, EventArgs e)
        {
            string account = CurrentAccount;
            if (string.IsNullOrEmpty(account))
            {
                MessageBox.Show("请选择交易帐户");
                return;
            }
            if (PrompotAccountEvent != null)
                PrompotAccountEvent(account);


        }




        void ctRaceService_Load(object sender, EventArgs e)
        {
            btnQry.Click += new EventHandler(btnQry_Click);
        }

        void btnQry_Click(object sender, EventArgs e)
        {
            Clear();
            if (QryRaceEvent != null)
            {
                string acct = account.Text;
                string rt = null;
                if (status.SelectedIndex != 0)
                {
                    rt = ((QSEnumAccountRaceStatus)(status.SelectedValue)).ToString();
                }

                QryRaceEvent(acct, rt);
            }
        }

        public void Clear()
        {
            rsGrid.DataSource = null;
            gt.Rows.Clear();
            BindToTable();
        }
        #region 表格
        #region 显示字段

        const string ACCOUNT = "交易帐号";
        const string RACEID = "比赛编号";
        const string ENTRYTIME = "参赛日期";
        const string RACESTATUS = "比赛状态";
        const string EXAMINETIME = "考核时间";
        const string EXAMINEEQUITY = "折算权益";
        const string STATUS = "状态";


        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();



        public void OnRaceService(string jsonstr, bool islast)
        {
            try
            {
                RaceServiceSetting rs = MoniterControl.MoniterHelper.ParseJsonResponse<RaceServiceSetting>(jsonstr);

                if (rs != null)
                {
                    InvokeGotRaceService(rs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void InvokeGotRaceService(RaceServiceSetting rs)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<RaceServiceSetting>(InvokeGotRaceService), new object[] { rs });
            }
            else
            {
                DataRow r = gt.Rows.Add(0);
                int i = gt.Rows.Count - 1;//得到新建的Row号

                gt.Rows[i][ACCOUNT] = rs.Acct;
                gt.Rows[i][RACEID] = rs.RaceID;
                gt.Rows[i][ENTRYTIME] = Util.ToDateTime(rs.EntryTime).ToString("yy-MM-dd");
                gt.Rows[i][RACESTATUS] = Util.GetEnumDescription(rs.RaceStatus);
                gt.Rows[i][EXAMINETIME] = Util.ToDateTime(rs.ExamineTime).ToString();
                gt.Rows[i][EXAMINEEQUITY] = Util.FormatDecimal(rs.ExamineEquity);
                gt.Rows[i][STATUS] = rs.IsAvabile ? "激活" : "冻结";
                
            }
        }
        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = rsGrid;

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
            gt.Columns.Add(RACEID);//
            gt.Columns.Add(ENTRYTIME);//
            gt.Columns.Add(RACESTATUS);//
            gt.Columns.Add(EXAMINETIME);//
            gt.Columns.Add(EXAMINEEQUITY);//
            gt.Columns.Add(STATUS);

        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = rsGrid;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            //grid.Columns[EXCHANGEID].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            //grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion


    }
}
