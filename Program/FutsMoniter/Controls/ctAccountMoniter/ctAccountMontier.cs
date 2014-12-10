using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;


namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier : UserControl,IEventBinder
    {
        #region 事件
        /// <summary>
        /// 触发查询历史记录用于调用外部查询窗口
        /// </summary>
        public event IAccountLiteDel QryAccountHistEvent;

        #endregion

        


        const string PROGRAME = "AccountMontier";
        fmAccountConfig fmaccountconfig = new fmAccountConfig();
        bool _loaded = false;

        Symbol _symbolselected = null;
        Symbol SymbolSelected { get { return _symbolselected; } }
        public ctAccountMontier()
        {
            try
            {
                InitializeComponent();

                InitQueryAccountControl();

                InitAccountMoniterGrid();

                //注册回调
                Globals.RegIEventHandler(this);

                //StartUpdate();
                _loaded = true;
                this.Load += new EventHandler(ctAccountMontier_Load);


            }
            catch (Exception ex)
            {
                MessageBox.Show("error ex:" + ex.ToString());
            }
            
        }

        void ctAccountMontier_Load(object sender, EventArgs e)
        {
            WireEvents();
            
        }

        public void Start()
        {
            StartUpdate();
        }

        #region  辅助函数

        private string _format = "{0:F2}";
        private string decDisp(decimal d)
        {
            return Util.FormatDecimal(d, _format);
        }

        public event DebugDelegate SendDebugEvent;
        bool _debugEnable = true;
        /// <summary>
        /// 是否输出日志
        /// </summary>
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }

        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
        /// <summary>
        /// 日志输出级别
        /// </summary>
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// 同时对外输出日志事件,用于被日志模块采集日志或分发
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            //1.判断日志级别,然后调用日志输出 比如向控件或者屏幕输出显示
            if (_debugEnable && (int)level <= (int)_debuglevel)
                msgdebug("[" + level.ToString() + "] " + PROGRAME + ":" + msg);
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="msg"></param>
        protected void msgdebug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }



        #endregion





        

        

        private void accountgrid_Click(object sender, EventArgs e)
        {
            debug("grid mouse clicked...", QSEnumDebugLevel.INFO);

        }


        
        


        


        #region 行情部分


        void InitViewQuoteList()
        {
            
        }

        void viewQuoteList1_SymbolSelectedEvent(Symbol symbol)
        {
            _symbolselected = symbol;
            ctOrderSenderM1.SetSymbol(symbol);
        }


        public void AddSymbol(Symbol symbol)
        {
            if (symbol == null) return;
            Globals.Debug("viewquotelist1 null:" + (viewQuoteList1 == null).ToString());
            viewQuoteList1.addSecurity(symbol);
        }

        #endregion

        private void radButton1_Click(object sender, EventArgs e)
        {
            Globals.TLClient.ReqWatchAccount(GetVisualAccounts());
        }

        private void accountgrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            { 
                
            }
        }


        #region 界面事件触发

        void WireEvents()
        {
            //Globals.RegInitCallback(this.OnInitFinished);

            //交易帐户过滤控件
            ctAccountType1.AccountTypeSelectedChangedEvent += new VoidDelegate(ctAccountType1_AccountTypeSelectedChangedEvent);
            ctRouterType1.RouterTypeSelectedChangedEvent += new VoidDelegate(ctRouterType1_RouterTypeSelectedChangedEvent);
            
            accexecute.SelectedIndexChanged +=new EventHandler(accexecute_SelectedIndexChanged);
            accLogin.CheckedChanged+=new EventHandler(accLogin_CheckedChanged);
            acct.TextChanged+=new EventHandler(acct_TextChanged);
            ctAgentList1.AgentSelectedChangedEvent+=new VoidDelegate(ctAgentList1_AgentSelectedChangedEvent);
            acchodpos.CheckedChanged +=new EventHandler(acchodpos_CheckedChanged);
            btnAddAccount.Click +=new EventHandler(btnAddAccount_Click);

            //帐户表格事件
            accountgrid.CellDoubleClick +=new DataGridViewCellEventHandler(accountgrid_CellDoubleClick);//双击单元格
            accountgrid.CellFormatting +=new DataGridViewCellFormattingEventHandler(accountgrid_CellFormatting);//格式化单元格
            accountgrid.SizeChanged +=new EventHandler(accountgrid_SizeChanged);//大小改变
            accountgrid.Scroll +=new ScrollEventHandler(accountgrid_Scroll);//滚轮滚动
            accountgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(accountgrid_RowPrePaint);

            //交易信息显示控件事件
            ctOrderView1.SendDebugEvent += new DebugDelegate(msgdebug);
            ctOrderView1.SendOrderCancel += new LongDelegate(CancelOrder);

            ctPositionView1.SendDebugEvent += new DebugDelegate(msgdebug);
            ctPositionView1.SendCancelEvent += new LongDelegate(CancelOrder);
            ctPositionView1.SendOrderEvent += new OrderDelegate(SendOrder);

            ctTradeView1.SendDebugEvent += new DebugDelegate(msgdebug);

            viewQuoteList1.SymbolSelectedEvent += new SymbolDelegate(viewQuoteList1_SymbolSelectedEvent);
            viewQuoteList1.SendDebugEvent += new DebugDelegate(Globals.Debug);
            ctOrderSenderM1.SendOrderEvent += new OrderDelegate(SendOrder);



            //绑定帐户选中事件
            AccountSelectedEvent += new Action<IAccountLite>(ctFinService1.OnAccountSelected);
            //财务信息绑定帐户设置时间
            AccountSelectedEvent += ctFinanceInfo1.SetAccount;

        }








       
        





        

       

       

        private void ServiceTabHolder_SelectedPageChanged(object sender, EventArgs e)
        {
            //if (ServiceTabHolder.SelectedPage.Name.Equals("FinServicePage"))
            //{
            //    if (AccountSetlected != null)
            //    {
            //        Globals.TLClient.ReqQryFinService(AccountSetlected.Account);
            //    }

            //    //如果没有获得服务计划列表则请求服务计划列表
            //    //ctFinService1.PrepareServicePlan();
            //}
        }

        #endregion



    }
}
