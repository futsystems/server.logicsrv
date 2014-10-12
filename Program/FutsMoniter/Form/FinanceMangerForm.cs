using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FutSystems.GUI;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;


namespace FutsMoniter
{
    public partial class FinanceMangerForm : Telerik.WinControls.UI.RadForm
    {
        bool _gotdata = false;
        public FinanceMangerForm()
        {
            InitializeComponent();
            SetPreferences();
            InitTable();
            BindToTable();

            if (Globals.CallbackCentreReady)
            {
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "QryFinanceInfo", this.OnQryAgentFinanceInfo);
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "UpdateAgentBankAccount", this.OnUpdateAgentBankInfo);
                Globals.CallBackCentre.RegisterCallback("MgrExchServer", "RequestCashOperation", this.OnRequestCashOperation);
            }
            this.FormClosing += new FormClosingEventHandler(FinanceMangerForm_FormClosing);
            this.Load += new EventHandler(FinanceMangerForm_Load);
        }

        void FinanceMangerForm_Load(object sender, EventArgs e)
        {
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryAgentFinanceInfo();
            }
        }

        void FinanceMangerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Globals.CallbackCentreReady)
            {
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "QryFinanceInfo", this.OnQryAgentFinanceInfo);
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "UpdateAgentBankAccount", this.OnUpdateAgentBankInfo);
                Globals.CallBackCentre.UnRegisterCallback("MgrExchServer", "RequestCashOperation", this.OnRequestCashOperation);
            }
        }

        private void btnChangeBankAccount_Click(object sender, EventArgs e)
        {
            if (_financeinfo == null)
            {
                fmConfirm.Show("无财务信息");
                return;
            }
            BankAccountForm fm = new BankAccountForm();
            fm.SetMGRFK(_financeinfo.BaseMGRFK);
            fm.SetBankInfo(_financeinfo.BankAccount);

            fm.ShowDialog();
        }

        void OnUpdateAgentBankInfo(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperBankAccount obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperBankAccount>(jd["Playload"].ToJson());
                if (_financeinfo != null)
                {
                    _financeinfo.BankAccount = obj;
                    GotFinanceInfo(_financeinfo);
                }
            }
            else//如果没有配资服
            {

            }
        }


        void OnQryAgentFinanceInfo(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperAgentFinanceInfo obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperAgentFinanceInfo>(jd["Playload"].ToJson());
                GotFinanceInfo(obj);

                //_gotdata = true;
            }
            else//如果没有配资服
            {

            }
        }
        JsonWrapperAgentFinanceInfo _financeinfo = null;
        delegate void del1(JsonWrapperAgentFinanceInfo info);
        void GotFinanceInfo(JsonWrapperAgentFinanceInfo info)
        { 
            if(InvokeRequired)
            {
                Invoke(new del1(GotFinanceInfo), new object[] { info });
            }
            else
            {
                lbbalance.Text = Util.FormatDecimal(info.Balance.Balance);
                if (info.BankAccount != null)
                {
                    lbname.Text = info.BankAccount.Name;
                    lbbankbranch.Text = info.BankAccount.Branch;
                    lbbankac.Text = info.BankAccount.Bank_AC;
                    lbbankname.Text = info.BankAccount.Bank.Name;
                    btnChangeBankAccount.Text = "修改银行卡信息";
                }
                else
                {
                    lbname.Text = "--";
                    lbbankbranch.Text = "--";
                    lbbankac.Text = "--";
                    lbbankname.Text = "--";
                    btnChangeBankAccount.Text = "添加银行卡信息";
                }

                if (info.LastSettle != null)
                {
                    lblastprofitfee.Text = Util.FormatDecimal(info.LastSettle.Profit_Fee);
                    lblastprofitcommission.Text = Util.FormatDecimal(info.LastSettle.Profit_Commission);
                }
                else
                { 
                    
                }
                if (info.LatestCashOperations != null && !_gotdata )
                { 
                    foreach(JsonWrapperCashOperation op in info.LatestCashOperations )
                    {
                        GotJsonWrapperCashOperation(op);
                    }
                }
                _financeinfo = info;
                _gotdata = true;
            }
        }






        void OnRequestCashOperation(string jsonstr)
        {
            JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            int code = int.Parse(jd["Code"].ToString());
            if (code == 0)
            {
                JsonWrapperCashOperation obj = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperCashOperation>(jd["Playload"].ToJson());
                GotJsonWrapperCashOperation(obj);
            }
            else//如果没有配资服
            {

            }
        }


        delegate void del2(JsonWrapperCashOperation op);
        void GotJsonWrapperCashOperation(JsonWrapperCashOperation op)
        {
            if (InvokeRequired)
            {
                Invoke(new del2(GotJsonWrapperCashOperation), new object[] { op });
            }
            else
            {
                DataRow r = gt.Rows.Add("");
                int i = gt.Rows.Count - 1;//得到新建的Row号

                //gt.Rows[i][ID] = o
                gt.Rows[i][MGRFK] = op.mgr_fk;
                gt.Rows[i][DATETIME] = Util.ToDateTime(op.DateTime).ToString("yy-MM-dd HH:mm:ss");
                gt.Rows[i][OPERATION] = Util.GetEnumDescription(op.Operation);
                gt.Rows[i][AMOUNT] = op.Amount;
                gt.Rows[i][REF] = op.Ref;
                gt.Rows[i][STATUS] = op.Status;
                gt.Rows[i][STATUSSTR] = Util.GetEnumDescription(op.Status);
            }
        }

        #region 表格
        #region 显示字段

        //const string ID = "编号D";
        const string MGRFK = "管理主域ID";
        const string DATETIME = "时间";
        const string OPERATION = "操作";
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
            
            gt.Columns.Add(MGRFK);//
            gt.Columns.Add(DATETIME);//
            gt.Columns.Add(OPERATION);//
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
            //grid.Columns[EXCHANGEID].IsVisible = false;
            //grid.Columns[UNDERLAYINGID].IsVisible = false;
            //grid.Columns[MARKETTIMEID].IsVisible = false;
            //grid.Columns[TRADEABLE].IsVisible = false;
        }





        #endregion

















        private void btnDeposit_Click(object sender, EventArgs e)
        {
           
        }

        private void btnWithDraw_Click(object sender, EventArgs e)
        {
            CashOperationForm fm = new CashOperationForm();
            fm.ShowDialog();
        }
    }
}
