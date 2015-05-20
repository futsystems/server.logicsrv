using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using TradingLib.Mixins.JsonObject;
using FutsMoniter.Common;


namespace FutsMoniter
{
    public enum CashOpViewType
    {
        /// <summary>
        /// 显示代理
        /// </summary>
        Agent,

        /// <summary>
        /// 显示交易帐户
        /// </summary>
        Account,
    }

    public partial class ctCashOperation : UserControl, IEventBinder
    {

        ContextMenuStrip menu = new ContextMenuStrip();
        public ctCashOperation()
        {
            InitializeComponent();
            try
            {
                SetPreferences();
                InitTable();
                BindToTable();

                menu.Items.Add("确认", Properties.Resources.editAccount, new EventHandler(Confirm_Click));
                menu.Items.Add("拒绝", Properties.Resources.editAccount, new EventHandler(Reject_Click));
                menu.Items.Add("取消", Properties.Resources.editAccount, new EventHandler(Cancel_Click));

                WireEvent();
                
            }
            catch (Exception ex)
            {

            }
        }

        void ctCashOperation_Load(object sender, EventArgs e)
        {
            if (ViewType == CashOpViewType.Agent)
            {
                opgrid.Columns[ACCOUNT].Visible = false;
            }
            else
            {
                opgrid.Columns[MGRFK].Visible = false;
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

        /// <summary>
        /// 检查出入金有效新
        /// 不为null 且处于pending状态
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        bool validCashOperation(JsonWrapperCashOperation op)
        {
            if (op == null)
            {
                fmConfirm.Show("请选择对应的出入金操作请求记录");
                return false;
            }
            //如果不是待处理状态 则提示返回
            if (op.Status != QSEnumCashInOutStatus.PENDING)
            {
                fmConfirm.Show("已处理请求无法重复处理");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 如果是入金操作 则须为手工发起的请求才可以进行手工操作
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        bool validManualDeposit(JsonWrapperCashOperation op)
        {
            if (op.Operation == QSEnumCashOperation.Deposit)
            {
                //如果是在线提交的入金请求 则无法手工确认
                if (op.Source == QSEnumCashOPSource.Online)
                {
                    fmConfirm.Show("在线入金请求无法手工执行操作,需等待支付网关返回");
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 添加交易帐户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Confirm_Click(object sender, EventArgs e)
        {
            
            JsonWrapperCashOperation op = CurrentCashOperation;

            if (!validCashOperation(op)) return;

            if (!validManualDeposit(op)) return;
            if (MoniterUtils.WindowConfirm(string.Format("确认出入金请求:",op.Ref)) == DialogResult.Yes)
            {
                //如果出金 就打印支付申请单
                if (op.Operation == QSEnumCashOperation.WithDraw)
                {
                    if (ViewType == CashOpViewType.Agent)//代理支付凭证
                    {
                        //生成支付单
                        fmPaySlipAgent fm = new fmPaySlipAgent();
                        fm.SetCashOperation(op);
                        if (fm.ShowDialog() != DialogResult.Yes)
                        {
                            return;
                        }
                    }
                    else
                    {
                        fmPaySlip fm = new fmPaySlip();
                        fm.SetCashOperation(op);
                        if (fm.ShowDialog() != DialogResult.Yes)
                        {
                            return;
                        }
                    }
                }

                if (ViewType == CashOpViewType.Agent)
                {
                    Globals.TLClient.ReqConfirmCashOperation(op.ToJson());
                }
                else
                {
                    Globals.TLClient.ReqConfirmAccountCashOperation(op.ToJson());
                }
            }
        }


        /// <summary>
        /// 拒绝出入金操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Reject_Click(object sender, EventArgs e)
        {
            JsonWrapperCashOperation op = CurrentCashOperation;

            if (!validCashOperation(op)) return;

            if (!validManualDeposit(op)) return;

            if (fmConfirm.Show("确认该操作?") == DialogResult.Yes)
            {
                if (ViewType == CashOpViewType.Agent)
                {
                    Globals.TLClient.ReqRejectCashOperation(op.ToJson());
                }
                else
                {
                    Globals.TLClient.ReqRejectAccountCashOperation(op.ToJson());
                }
            }
        }

        /// <summary>
        /// 取消出入金操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Cancel_Click(object sender, EventArgs e)
        {
            JsonWrapperCashOperation op = CurrentCashOperation;

            if (!validCashOperation(op)) return;

            if (!validManualDeposit(op)) return;

            if (fmConfirm.Show("确认该操作?") == DialogResult.Yes)
            {
                if (ViewType == CashOpViewType.Agent)
                {
                    Globals.TLClient.ReqCancelCashOperation(op.ToJson());
                }
                else
                {
                    Globals.TLClient.ReqCancelAccountCashOperation(op.ToJson());
                }
            }
        }


        string getkey(JsonWrapperCashOperation op)
        {
            if (ViewType == CashOpViewType.Account)
            {
                return op.Account + "-" + op.Ref;
            }
            else
            {
                return op.mgr_fk + "-" + op.Ref;
            }
        }

        ConcurrentDictionary<string, JsonWrapperCashOperation> operationkeymap = new ConcurrentDictionary<string, JsonWrapperCashOperation>();
        ConcurrentDictionary<string, int> idxmap = new ConcurrentDictionary<string, int>();

        int CashOperationIdx(JsonWrapperCashOperation op)
        {
            string key = getkey(op);
            if (idxmap.Keys.Contains(key))
            {
                return idxmap[key];
            }
            return -1;
        }


        int CurrentRow
        {
            get
            {
                return opgrid.SelectedRows.Count > 0 ? opgrid.SelectedRows[0].Index : -1;
            }
        }
        //得到当前选择的行号
        private string CurrentKey
        {
            get
            {
                if (opgrid.SelectedRows.Count > 0)
                    return opgrid[KEY,CurrentRow].Value.ToString();
                else
                    return string.Empty;
            }
        }

        public JsonWrapperCashOperation CurrentCashOperation
        {
            get
            {
                string key = CurrentKey;
                if (operationkeymap.Keys.Contains(key))
                {
                    return operationkeymap[key];
                }
                return null;
            }
        }


        delegate void del2(JsonWrapperCashOperation op);
        public void GotJsonWrapperCashOperation(JsonWrapperCashOperation op)
        {
            if (InvokeRequired)
            {
                Invoke(new del2(GotJsonWrapperCashOperation), new object[] { op });
            }
            else
            {
                int id = CashOperationIdx(op);
                if (id == -1)
                {
                    DataRow r = gt.Rows.Add("");
                    int i = gt.Rows.Count - 1;//得到新建的Row号
                    string key = getkey(op);
                    gt.Rows[i][KEY] = key;
                    gt.Rows[i][MGRFK] = op.mgr_fk;
                    gt.Rows[i][ACCOUNT] = op.Account;
                    gt.Rows[i][DATETIME] = Util.ToDateTime(op.DateTime).ToString("yy-MM-dd HH:mm:ss");
                    gt.Rows[i][OPERATIONSTR] = Util.GetEnumDescription(op.Operation);
                    gt.Rows[i][OPERATION] = op.Operation;
                    gt.Rows[i][AMOUNT] = op.Amount;
                    gt.Rows[i][REF] = op.Ref;
                    gt.Rows[i][STATUS] = op.Status;
                    gt.Rows[i][STATUSSTR] = Util.GetEnumDescription(op.Status);
                    gt.Rows[i][SOURCE] = op.Source;
                    gt.Rows[i][SOURCESTR] = Util.GetEnumDescription(op.Source);
                    gt.Rows[i][RECVINFO] = op.RecvInfo;
                    gt.Rows[i][SUBMITTER] = op.Submiter;
                    operationkeymap.TryAdd(key, op);
                    idxmap.TryAdd(key, i);
                }
                else //更新
                {
                    gt.Rows[id][STATUS] = op.Status;
                    gt.Rows[id][STATUSSTR] = Util.GetEnumDescription(op.Status);

                }

            }
        }

        #region 表格
        #region 显示字段

        //const string ID = "编号D";
        const string KEY = "主键";
        const string MGRFK = "管理主域ID";
        const string ACCOUNT = "交易帐号";
        const string DATETIME = "时间";
        const string OPERATIONSTR = "操作";
        const string OPERATION = "OPERATION";
        const string AMOUNT = "金额";
        const string REF = "流水号";
        const string STATUS = "STATUS";
        const string STATUSSTR = "状态";
        const string SOURCE = "SOURCE";
        const string SOURCESTR = "来源";
        const string RECVINFO = "收款信息";
        const string SUBMITTER = "提交者信息";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = opgrid;

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
            gt.Columns.Add(KEY);//0
            gt.Columns.Add(MGRFK);//1
            gt.Columns.Add(ACCOUNT);//2
            gt.Columns.Add(DATETIME);//3
            gt.Columns.Add(OPERATION);//4
            gt.Columns.Add(OPERATIONSTR);//5
            gt.Columns.Add(AMOUNT);//6
            gt.Columns.Add(REF);//7
            gt.Columns.Add(STATUS);//8
            gt.Columns.Add(STATUSSTR);//9
            gt.Columns.Add(SOURCE);//10
            gt.Columns.Add(SOURCESTR);//RECVINFO
            gt.Columns.Add(RECVINFO);//
            gt.Columns.Add(SUBMITTER);
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = opgrid;

            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            grid.Columns[KEY].Visible = false;

            grid.Columns[SOURCE].Visible = false;
            grid.Columns[STATUS].Visible = false;
            grid.Columns[OPERATION].Visible = false;

            grid.Columns[MGRFK].Width = 80;
            grid.Columns[ACCOUNT].Width = 80;

            grid.Columns[DATETIME].Width = 120;
            grid.Columns[OPERATIONSTR].Width = 60;
            grid.Columns[AMOUNT].Width = 60;
            grid.Columns[REF].Width = 150;
            grid.Columns[STATUS].Width = 40;
            grid.Columns[SOURCESTR].Width = 80;
        }





        #endregion

        void WireEvent()
        {
            //全局事件回调
            Globals.RegIEventHandler(this);


            btnFilterPending.Click +=new EventHandler(btnFilterPending_Click);//过滤
            btnFilterConfirmed.Click +=new EventHandler(btnFilterConfirmed_Click);//
            btnFilterCancelOrReject.Click+=new EventHandler(btnFilterCancelOrReject_Click);//

            opgrid.CellMouseClick += new DataGridViewCellMouseEventHandler(opgrid_CellMouseClick);
            opgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(opgrid_RowPrePaint);
            opgrid.CellFormatting += new DataGridViewCellFormattingEventHandler(opgrid_CellFormatting);
            this.Load += new EventHandler(ctCashOperation_Load);
        }

        public void OnInit()
        {
            if (!Globals.Manager.RightRootDomain())
            {
                this.menu.Items[0].Visible = false;
                this.menu.Items[1].Visible = false;
            }
        }

        public void OnDisposed()
        {
           
        }


        void opgrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 5)
            {
                e.CellStyle.Font = UIGlobals.BoldFont;
                QSEnumCashOperation op = (QSEnumCashOperation)Enum.Parse(typeof(QSEnumCashOperation),opgrid[4,e.RowIndex].Value.ToString());
                if (op == QSEnumCashOperation.Deposit)
                {
                    e.CellStyle.ForeColor = UIGlobals.LongSideColor;
                }
                else
                {
                    e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                }
            }
            else if (e.ColumnIndex == 9)
            {
                e.CellStyle.Font = UIGlobals.BoldFont;
                QSEnumCashInOutStatus status = (QSEnumCashInOutStatus)Enum.Parse(typeof(QSEnumCashInOutStatus), opgrid[8, e.RowIndex].Value.ToString());
                switch (status)
                {
                    case QSEnumCashInOutStatus.CONFIRMED:
                        e.CellStyle.ForeColor = Color.Green;
                        break;
                    case QSEnumCashInOutStatus.REFUSED:
                        e.CellStyle.ForeColor = Color.Red;
                        break;
                    case QSEnumCashInOutStatus.CANCELED:
                        e.CellStyle.ForeColor = Color.Sienna;
                        break;
                    default:
                        break;
                }
            }


        }

        void opgrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }

        void opgrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                GetRightMenu().Show(Control.MousePosition);
            }
        }

        /// <summary>
        /// 动态设置菜单的可见性
        /// </summary>
        /// <returns></returns>
        ContextMenuStrip GetRightMenu()
        {
            return menu;
        }

        private void btnFilterPending_Click(object sender, EventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' ", QSEnumCashInOutStatus.PENDING);
            datasource.Filter = strFilter;
        }

        private void btnFilterConfirmed_Click(object sender, EventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' ", QSEnumCashInOutStatus.CONFIRMED);
            datasource.Filter = strFilter;
        }

        private void btnFilterCancelOrReject_Click(object sender, EventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' or " + STATUS + " = '{1}'", QSEnumCashInOutStatus.CANCELED, QSEnumCashInOutStatus.REFUSED);
            datasource.Filter = strFilter;
        }







    }
}
