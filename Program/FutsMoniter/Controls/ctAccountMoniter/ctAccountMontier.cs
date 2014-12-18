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


namespace FutsMoniter
{
    public partial class ctAccountMontier : UserControl,IEventBinder
    {

        const string PROGRAME = "AccountMontier";
        //fmAccountConfig fmaccountconfig = new fmAccountConfig();
        bool _loaded = false;

        Symbol _symbolselected = null;
        Symbol SymbolSelected { get { return _symbolselected; } }
        public ctAccountMontier()
        {
            try
            {
                InitializeComponent();

                this.Load += new EventHandler(ctAccountMontier_Load);


            }
            catch (Exception ex)
            {
                MessageBox.Show("error ex:" + ex.ToString());
            }
            
        }

        void ctAccountMontier_Load(object sender, EventArgs e)
        {
            accexecute.Items.Add("<Any>");
            accexecute.Items.Add("允许");
            accexecute.Items.Add("冻结");
            accexecute.SelectedIndex = 0;

            SetPreferences();
            InitTable();
            BindToTable();

            //初始表格右键化右键菜单
            InitMenu();

            WireEvents();
            
        }

        public void Start()
        {
            StartUpdate();
        }


        private void accountgrid_Click(object sender, EventArgs e)
        {
            debug("grid mouse clicked...", QSEnumDebugLevel.INFO);

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

        void WireEvents()
        {
            

            //交易帐户过滤控件
            ctAccountType1.AccountTypeSelectedChangedEvent += new VoidDelegate(ctAccountType1_AccountTypeSelectedChangedEvent);
            ctRouterType1.RouterTypeSelectedChangedEvent += new VoidDelegate(ctRouterType1_RouterTypeSelectedChangedEvent);
            accexecute.SelectedIndexChanged +=new EventHandler(accexecute_SelectedIndexChanged);
            accLogin.CheckedChanged+=new EventHandler(accLogin_CheckedChanged);
            acct.TextChanged+=new EventHandler(acct_TextChanged);
            ctAgentList1.AgentSelectedChangedEvent+=new VoidDelegate(ctAgentList1_AgentSelectedChangedEvent);
            acchodpos.CheckedChanged +=new EventHandler(acchodpos_CheckedChanged);
            ctRouterGroupList1.RouterGroupSelectedChangedEvent += new VoidDelegate(ctRouterGroupList1_RouterGroupSelectedChangedEvent);
            

            //帐户表格事件
            accountgrid.CellDoubleClick +=new DataGridViewCellEventHandler(accountgrid_CellDoubleClick);//双击单元格
            accountgrid.CellFormatting +=new DataGridViewCellFormattingEventHandler(accountgrid_CellFormatting);//格式化单元格
            accountgrid.SizeChanged +=new EventHandler(accountgrid_SizeChanged);//大小改变
            accountgrid.Scroll +=new ScrollEventHandler(accountgrid_Scroll);//滚轮滚动
            accountgrid.RowPrePaint += new DataGridViewRowPrePaintEventHandler(accountgrid_RowPrePaint);

            //路由组初始化完毕
            ctRouterGroupList1.RouterGroupInitEvent += new VoidDelegate(ctRouterGroupList1_RouterGroupInitEvent);

            //绑定事件
            btnAddAccount.Click += new EventHandler(btnAddAccount_Click);

            Globals.RegIEventHandler(this);
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


    }
}
