﻿using System;
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
using Telerik.WinControls;
using Telerik.WinControls.UI;
using FutSystems.GUI;


namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier : UserControl
    {
        #region 事件
        /// <summary>
        /// 触发查询历史记录用于调用外部查询窗口
        /// </summary>
        public event IAccountLiteDel QryAccountHistEvent;

        #endregion

        


        const string PROGRAME = "AccountMontier";
        AccountConfigForm fmaccountconfig = null;
        bool _loaded = false;

        Symbol _symbolselected = null;
        Symbol SymbolSelected { get { return _symbolselected; } }
        public ctAccountMontier()
        {
            try
            {
            InitializeComponent();

            Factory.IDataSourceFactory(accountType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumAccountCategory>(true));
            Factory.IDataSourceFactory(routeType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumOrderTransferType>(true));

            accexecute.Items.Add("<Any>");
            accexecute.Items.Add("允许");
            accexecute.Items.Add("冻结");
            accexecute.SelectedIndex = 0;

            Init();
            InitAccountMoniterGrid();
            Globals.RegInitCallback(this.OnInitFinished);

            StartUpdate();
            fmaccountconfig = new AccountConfigForm();
            fmaccountconfig.SendDebugEvent += new DebugDelegate(msgdebug);

            InitViewQuoteList();
            _loaded = true;

            
                if (!Globals.Config["FinService"].AsBool())
                {
                    FinServicePage.Text = "开发中";
                    FinServicePage.Enabled = false;
                }
                if (!Globals.Config["RaceService"].AsBool())
                {
                    RaceServicePage.Text = "开发中";
                    FinServicePage.Enabled = false;
                }
                if (!Globals.Config["LottoService"].AsBool())
                {
                    LottoServicePage.Text = "开发中";
                    LottoServicePage.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("error ex:" + ex.ToString());
            }
            
        }
        

        void Init()
        {
            //this.accountgrid.TableElement.VScrollBar.ValueChanged += new EventHandler(VScrollBar_ValueChanged);

            try
            {
                ctOrderView1.SendDebugEvent += new DebugDelegate(msgdebug);
                ctOrderView1.SendOrderCancel += new LongDelegate(CancelOrder);

                ctPositionView1.SendDebugEvent += new DebugDelegate(msgdebug);
                ctPositionView1.SendCancelEvent += new LongDelegate(CancelOrder);
                ctPositionView1.SendOrderEvent += new OrderDelegate(SendOrder);

                ctTradeView1.SendDebugEvent += new DebugDelegate(msgdebug);

                ctAgentList1.AgentSelectedChangedEvent += new VoidDelegate(ctAgentList1_AgentSelectedChangedEvent);


                Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "QryFinService", ctFinService1.OnQryFinService);
                Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "QryFinServicePlan", ctFinService1.OnQryServicePlan);
                Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "UpdateArguments", ctFinService1.OnQryFinService);
                Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "ChangeServicePlane", ctFinService1.OnQryFinService);
                Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "DeleteServicePlane", ctFinService1.OnQryFinService);

                //初始化菜单
                InitMenu();
            }
            catch (Exception ex)
            { 
                
            }
        }

        #region  辅助函数

        private string _format = "{0:F2}";
        private string decDisp(decimal d)
        {
            return string.Format(_format, d);
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
            viewQuoteList1.SymbolSelectedEvent += new SymbolDelegate(viewQuoteList1_SymbolSelectedEvent);
            viewQuoteList1.SendDebugEvent += new DebugDelegate(Globals.Debug);
            ctOrderSenderM1.SendOrderEvent += new OrderDelegate(SendOrder);
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


















    }
}
