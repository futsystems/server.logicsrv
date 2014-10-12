using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;
using Telerik.WinControls;
using Telerik.WinControls.UI;


namespace FutsMoniter
{
    public partial class ctCashOperation : UserControl
    {

        RadContextMenu menu = new RadContextMenu();
        public ctCashOperation()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();
            //if (Globals.CallbackCentreReady)
            //{
            //    Globals.CallBackCentre.RegisterCallback("MgrExchServer", "RequestCashOperation", this.OnRequestCashOperation);
            //}
            Telerik.WinControls.UI.RadMenuItem MenuItem_confirm = new Telerik.WinControls.UI.RadMenuItem("确认");
            //MenuItem_confirm.Image = Properties.Resources.editAccount_16;
            MenuItem_confirm.Click += new EventHandler(Confirm_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_reject = new Telerik.WinControls.UI.RadMenuItem("拒绝");
            //MenuItem_confirm.Image = Properties.Resources.editAccount_16;
            MenuItem_reject.Click += new EventHandler(Reject_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_cancel = new Telerik.WinControls.UI.RadMenuItem("取消");
            //MenuItem_confirm.Image = Properties.Resources.editAccount_16;
            MenuItem_cancel.Click += new EventHandler(Cancel_Click);


            menu.Items.Add(MenuItem_confirm);
            menu.Items.Add(MenuItem_reject);
            menu.Items.Add(MenuItem_cancel);
            ctGridExport1.Grid = opgrid;
        }

        /// <summary>
        /// 添加交易帐户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Confirm_Click(object sender, EventArgs e)
        {
            JsonWrapperCashOperation op = CurrentCashOperation;
            if(op == null)
            {
                fmConfirm.Show("请选择对应的出入金操作请求记录");
                return;
            }
            if (op.Operation == QSEnumCashOperation.WithDraw)
            { 
                //生成支付单
                PaySlipForm fm = new PaySlipForm();
                fm.SetCashOperation(op);
                if (fm.ShowDialog() != DialogResult.Yes)
                {
                    return;
                }
            }

            //if (fmConfirm.Show("确认该操作?") == DialogResult.Yes)
            {

                Globals.TLClient.ReqConfirmCashOperation(TradingLib.Mixins.LitJson.JsonMapper.ToJson(op));
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
            if (op == null)
            {
                fmConfirm.Show("请选择对应的出入金操作请求记录");
                return;
            }
            if (fmConfirm.Show("确认该操作?") == DialogResult.Yes)
            {

                Globals.TLClient.ReqRejectCashOperation(TradingLib.Mixins.LitJson.JsonMapper.ToJson(op));
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
            if (op == null)
            {
                fmConfirm.Show("请选择对应的出入金操作请求记录");
                return;
            }
            if (fmConfirm.Show("确认该操作?") == DialogResult.Yes)
            {

                Globals.TLClient.ReqCancelCashOperation(TradingLib.Mixins.LitJson.JsonMapper.ToJson(op));
            }
        }


        string getkey(JsonWrapperCashOperation op)
        {
            return op.mgr_fk + "-" + op.Ref;
        }
        ConcurrentDictionary<string, JsonWrapperCashOperation> operationkeymap = new ConcurrentDictionary<string, JsonWrapperCashOperation>();
        ConcurrentDictionary<string, int> idxmap = new ConcurrentDictionary<string, int>();

        int CashOperationIdx(JsonWrapperCashOperation op)
        {
            int id = -1;
            string key = getkey(op);
            if (idxmap.Keys.Contains(key))
            {
                return idxmap[key];
            }
            return -1;
        }
        //得到当前选择的行号
        private string CurrentKey
        {

            get
            {

                if (opgrid.SelectedRows.Count > 0)
                {
                    return opgrid.SelectedRows[0].ViewInfo.CurrentRow.Cells[KEY].Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
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
                if(id==-1)
                {
                    DataRow r = gt.Rows.Add("");
                    int i = gt.Rows.Count - 1;//得到新建的Row号
                    string key = getkey(op);
                    gt.Rows[i][KEY] = key;
                    gt.Rows[i][MGRFK] = op.mgr_fk;
                    gt.Rows[i][DATETIME] = Util.ToDateTime(op.DateTime).ToString("yy-MM-dd HH:mm:ss");
                    gt.Rows[i][OPERATIONSTR] = Util.GetEnumDescription(op.Operation);
                    gt.Rows[i][OPERATION] = op.Operation;
                    gt.Rows[i][AMOUNT] = op.Amount;
                    gt.Rows[i][REF] = op.Ref;
                    gt.Rows[i][STATUS] = op.Status;
                    gt.Rows[i][STATUSSTR] = Util.GetEnumDescription(op.Status);
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
        const string DATETIME = "时间";
        const string OPERATIONSTR = "操作";
        const string OPERATION = "OPERATION";
        const string AMOUNT = "金额";
        const string REF = "流水号";
        const string STATUS = "STATUS";
        const string STATUSSTR = "状态";

        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            Telerik.WinControls.UI.RadGridView grid = opgrid;
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
            gt.Columns.Add(KEY);//
            gt.Columns.Add(MGRFK);//
            gt.Columns.Add(DATETIME);//
            gt.Columns.Add(OPERATION);//
            gt.Columns.Add(OPERATIONSTR);//
            gt.Columns.Add(AMOUNT);//
            gt.Columns.Add(REF);//
            gt.Columns.Add(STATUS);//
            gt.Columns.Add(STATUSSTR);//
        }

        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = opgrid;

            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;


            datasource.DataSource = gt;
            grid.DataSource = datasource;

            //需要在绑定数据源后设定具体的可见性
            grid.Columns[KEY].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            grid.Columns[STATUS].IsVisible = false;
            grid.Columns[OPERATION].IsVisible = false;
        }





        #endregion

        private void opgrid_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            e.ContextMenu = menu.DropDown;
        }

        private void opgrid_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            try
            {
                if (e.CellElement.RowInfo is GridViewDataRowInfo)
                {
                    if (e.CellElement.ColumnInfo.Name == STATUSSTR)
                    {
                        QSEnumCashInOutStatus status = (QSEnumCashInOutStatus)Enum.Parse(typeof(QSEnumCashInOutStatus), (e.CellElement.RowInfo.Cells[STATUS].Value.ToString()));

                        switch (status)
                        {
                            case QSEnumCashInOutStatus.CONFIRMED:
                                e.CellElement.ForeColor = Color.Green;
                                e.CellElement.Font = UIGlobals.BoldFont;
                                break;
                            case QSEnumCashInOutStatus.REFUSED:
                                e.CellElement.ForeColor = Color.Red;
                                e.CellElement.Font = UIGlobals.BoldFont;
                                break;
                            case QSEnumCashInOutStatus.CANCELED:
                                e.CellElement.ForeColor = Color.Sienna;
                                e.CellElement.Font = UIGlobals.BoldFont;
                                break;
                            default:
                                break;
                        }
                    }
                    if (e.CellElement.ColumnInfo.Name == OPERATIONSTR)
                    {

                        QSEnumCashOperation op = (QSEnumCashOperation)Enum.Parse(typeof(QSEnumCashOperation), (e.CellElement.RowInfo.Cells[OPERATION].Value.ToString()));
                        if (op == QSEnumCashOperation.Deposit)
                        {
                            e.CellElement.ForeColor = Color.Red;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else
                        {
                            e.CellElement.ForeColor = Color.Green;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                    }

                       

                }


            }
            catch (Exception ex)
            {
                
            }
        }

        private void btnFilterPending_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' ",QSEnumCashInOutStatus.PENDING);
            datasource.Filter = strFilter;
        }

        private void btnFilterConfirmed_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' ", QSEnumCashInOutStatus.CONFIRMED);
            datasource.Filter = strFilter;
        }

        private void btnFilterCancelOrReject_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            string strFilter = DATETIME + " ASC";
            strFilter = String.Format(STATUS + " = '{0}' or " + STATUS + " = '{1}'", QSEnumCashInOutStatus.CANCELED, QSEnumCashInOutStatus.REFUSED);
            datasource.Filter = strFilter;
        }









    }
}
