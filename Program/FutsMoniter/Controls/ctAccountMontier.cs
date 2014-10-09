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
using Telerik.WinControls;
using Telerik.WinControls.UI;
using FutSystems.GUI;


namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier : UserControl
    {
        #region 事件
        public event IAccountLiteDel QryAccountHistEvent;

        #endregion

        const string PROGRAME = "AccountMontier";
        AccountConfigForm fmaccountconfig = null;
        bool _loaded = false;

        Symbol _symbolselected = null;
        Symbol SymbolSelected { get { return _symbolselected; } }
        public ctAccountMontier()
        {
            InitializeComponent();

            Factory.IDataSourceFactory(accountType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumAccountCategory>(true));
            Factory.IDataSourceFactory(routeType).BindDataSource(UIUtil.GetEnumValueObjects<QSEnumOrderTransferType>(true));

            accexecute.Items.Add("<Any>");
            accexecute.Items.Add("允许");
            accexecute.Items.Add("冻结");
            accexecute.SelectedIndex = 0;

            Init();

            SetPreferences();
            InitTable();
            BindToTable();

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
        }
        RadContextMenu menu = new RadContextMenu();
        #region  初始化与事件绑定
        void Init()
        {
            this.accountgrid.TableElement.VScrollBar.ValueChanged += new EventHandler(VScrollBar_ValueChanged);

            ctOrderView1.SendDebugEvent += new DebugDelegate(msgdebug);
            ctOrderView1.SendOrderCancel += new LongDelegate(CancelOrder);

            ctPositionView1.SendDebugEvent += new DebugDelegate(msgdebug);
            ctPositionView1.SendCancelEvent +=new LongDelegate(CancelOrder);
            ctPositionView1.SendOrderEvent += new OrderDelegate(SendOrder);

            ctTradeView1.SendDebugEvent += new DebugDelegate(msgdebug);


            //accountgrid.ContextMenu = new System.Windows.Forms.ContextMenu();
            //accountgrid.ContextMenu.MenuItems.Add("xyz");
            //accountgrid.ContextMenu.MenuItems.Add("abc");

            Telerik.WinControls.UI.RadMenuItem  MenuItem_edit = new Telerik.WinControls.UI.RadMenuItem("编辑");
            MenuItem_edit.Image = Properties.Resources.editAccount_16;
            MenuItem_edit.Click += new EventHandler(EditAccount_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_add = new Telerik.WinControls.UI.RadMenuItem("添加");
            MenuItem_add.Image = Properties.Resources.addAccount_16;
            MenuItem_add.Click += new EventHandler(AddAccount_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_changepass = new Telerik.WinControls.UI.RadMenuItem("修改密码");
            //MenuItem_changepass.Image = Properties.Resources.addAccount_16;
            MenuItem_changepass.Click += new EventHandler(ChangePass_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_changeinvestor = new Telerik.WinControls.UI.RadMenuItem("修改信息");
            //MenuItem_changepass.Image = Properties.Resources.addAccount_16;
            MenuItem_changeinvestor.Click += new EventHandler(ChangeInvestor_Click);

            Telerik.WinControls.UI.RadMenuItem MenuItem_qryhist = new Telerik.WinControls.UI.RadMenuItem("历史记录");
            //MenuItem_changepass.Image = Properties.Resources.addAccount_16;
            MenuItem_qryhist.Click += new EventHandler(QryHist_Click);

            //Telerik.WinControls.UI.RadMenuItem MenuItem_inserttrade = new Telerik.WinControls.UI.RadMenuItem("插入成交");
            ////MenuItem_changepass.Image = Properties.Resources.addAccount_16;
            //MenuItem_inserttrade.Click += new EventHandler(InsertTrade_Click);

            menu.Items.Add(MenuItem_edit);
            menu.Items.Add(MenuItem_add);
            menu.Items.Add(MenuItem_changepass);
            menu.Items.Add(MenuItem_changeinvestor);
            menu.Items.Add(MenuItem_qryhist);

            //menu.Items.Add(MenuItem_inserttrade);


            Globals.CallBackCentre.RegisterCallback("FinServiceCentre", "QryFinService", ctFinService1.OnQryFinService);
        }

        //void InsertTrade_Click(object sender, EventArgs e)
        //{
        //    IAccountLite account = GetVisibleAccount(CurrentAccount);
        //    if (account != null)
        //    {
        //        InsertTradeForm fm = new InsertTradeForm();
        //        fm.SetAccount(account.Account);
        //        fm.SetSymbol(SymbolSelected);
        //        fm.ShowDialog();
        //    }
        //    else
        //    {
        //        fmConfirm.Show("请选择交易帐户！");
        //    }
        //}
        /// <summary>
        /// 添加交易帐户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddAccount_Click(object sender, EventArgs e)
        {
            AddAccountForm fm = new AddAccountForm();
            fm.TopMost = true;
            fm.ShowDialog();
        }

        /// <summary>
        /// 编辑某个交易帐号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EditAccount_Click(object sender, EventArgs e)
        {
            IAccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                fmaccountconfig.Account = account;
                fmaccountconfig.Show();//.ShowDialog();
            }
            else
            {
                fmConfirm.Show("请选择需要编辑的交易帐户！");
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangePass_Click(object sender, EventArgs e)
        {
            IAccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                ChangePassForm fm = new ChangePassForm();
                fm.SetAccount(account.Account);
                fm.ShowDialog();

            }
            else
            {
                fmConfirm.Show("请选择需要编辑的交易帐户！");
            }
        }

        /// <summary>
        /// 修改投资者信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangeInvestor_Click(object sender, EventArgs e)
        {
            IAccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                ChangeInvestorForm fm = new ChangeInvestorForm();

                fm.SetAccount(account);
                fm.ShowDialog();

            }
            else
            {
                fmConfirm.Show("请选择需要编辑的交易帐户！");
            }
        }

        /// <summary>
        /// 查询历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void QryHist_Click(object sender, EventArgs e)
        {
            IAccountLite account = GetVisibleAccount(CurrentAccount);
            if (account != null)
            {
                if (QryAccountHistEvent != null)
                    QryAccountHistEvent(account);

            }
            else
            {
                fmConfirm.Show("请选择需要查询的交易帐户！");
            }
        }



        #endregion

        #region 表格
        #region 显示字段
        const string ACCOUNT = "账户";
        const string ROUTE = "RouteType";
        const string ROUTEIMG = "路由";

        const string EXECUTE = "ExecuteStatus";
        const string EXECUTEIMG = "状态";
        const string PROFITLOSSIMG = "盈/亏";

        const string LOGINSTATUS = "LoginStatus";
        const string LOGINSTATUSIMG = "登入";
        const string ADDRESS = "地址";

        const string LASTEQUITY = "昨日权益";
        const string NOWEQUITY = "当前权益";
        const string MARGIN = "保证金";
        const string FROZENMARGIN = "冻结保证金";
        const string CASH = "可用资金";
        const string BUYPOWER = "购买力";
        const string REALIZEDPL = "平仓盈亏";
        const string UNREALIZEDPL = "浮动盈亏";
        const string COMMISSION = "手续费";
        const string PROFIT = "净利";
        const string CATEGORY = "帐户类型";
        const string RACEENTRYTIME = "参赛日期";
        const string RACEID = "比赛编号";
        const string RACESTATUS = "比赛状态";

        const string INTRADAY = "日内";
        const string AGENTCODE = "代理编号";
        const string NAME = "姓名";
        const string POSLOK = "锁仓权限";
        #endregion

        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();

        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            //Telerik.WinControls.UI.RadGridView grid = accountgrid;
            //grid.ShowRowHeaderColumn = false;//显示每行的头部
            //grid.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;//列的填充方式
            //grid.ShowGroupPanel = false;//是否显示顶部的panel用于组合排序
            //grid.MasterTemplate.EnableGrouping = false;//是否允许分组
            //grid.EnableHotTracking = true;
            //this.radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On; 

            Telerik.WinControls.UI.RadGridView grid = accountgrid;
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
            gt.Columns.Add(ACCOUNT);//0
            gt.Columns.Add(ROUTE);//1
            gt.Columns.Add(ROUTEIMG, typeof(Image));//2

            gt.Columns.Add(EXECUTE);//3
            gt.Columns.Add(EXECUTEIMG, typeof(Image));//4
            gt.Columns.Add(PROFITLOSSIMG, typeof(Image));//5

            gt.Columns.Add(LOGINSTATUS);//6
            gt.Columns.Add(LOGINSTATUSIMG, typeof(Image));//7
            gt.Columns.Add(ADDRESS);//8

            gt.Columns.Add(LASTEQUITY);//9
            gt.Columns.Add(NOWEQUITY);//10
            gt.Columns.Add(MARGIN);//11
            gt.Columns.Add(FROZENMARGIN);//12
            gt.Columns.Add(REALIZEDPL);//14
            gt.Columns.Add(UNREALIZEDPL);//15

            gt.Columns.Add(COMMISSION, typeof(Decimal));//16
            gt.Columns.Add(PROFIT);//17
            gt.Columns.Add(CATEGORY);//18
            gt.Columns.Add(INTRADAY);//19
            gt.Columns.Add(AGENTCODE);//22
            gt.Columns.Add(POSLOK);//
            gt.Columns.Add(NAME);//22


            //accountlist.ContextMenuStrip = new ContextMenuStrip();
            //accountlist.ContextMenuStrip.Items.Add("添加账户", Properties.Resources.addAccount, new EventHandler(addAccount));
            //accountlist.ContextMenuStrip.Items.Add("编辑账户", Properties.Resources.editAccount, new EventHandler(editAccount));
            //accountlist.ContextMenuStrip.Items.Add("删除账户", Properties.Resources.delAccount, new EventHandler(delAccount));

            

            //隐藏搜索关键字
            /*
            accountgrid.Columns[MODE].Visible = false;
            accountgrid.Columns[EXECUTE].Visible = false;
            accountgrid.Columns[REGSTATUS].Visible = false;


            accountgrid.Columns[ACCOUNT].Width = 60;
            accountgrid.Columns[MODEIMG].Width = 20;
            accountgrid.Columns[EXECUTEIMG].Width = 20;
            accountgrid.Columns[PROFITLOSSIMG].Width = 20;
            accountgrid.Columns[REGSTATUSIMG].Width = 20;
            accountgrid.Columns[ADDRESS].Width = 120;
            **/

            //检查模块 然后隐藏对应的界面


            for (int i = 0; i < gt.Columns.Count; i++)
            {
                //accountgrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// validview 调整视图状态
        /// 根据具体的权限状态进行调整
        /// </summary>
        public void ValidView()
        {
            if (!Globals.RightRouter)
            {
                accountgrid.Columns[ROUTEIMG].IsVisible = false;
            }
            if (!Globals.RightAgent)
            {
                accountgrid.Columns[AGENTCODE].IsVisible = false;
            }

            fmaccountconfig.ValidView();
        }
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            Telerik.WinControls.UI.RadGridView grid = accountgrid;
            
            //grid.TableElement.BeginUpdate();             
            //grid.MasterTemplate.Columns.Clear(); 
            //accountlist.DataSource = gt;
            datasource.DataSource = gt;
            datasource.Sort = ACCOUNT + " ASC";
            accountgrid.DataSource = datasource;

            accountgrid.Columns[EXECUTE].IsVisible = false;
            accountgrid.Columns[ROUTE].IsVisible = false;
            accountgrid.Columns[LOGINSTATUS].IsVisible = false;

            //if (! (Globals.Manager.Type == QSEnumManagerType.ROOT))
            //{
            //    accountgrid.Columns[ROUTEIMG].IsVisible = false;
            //    accountgrid.Columns[AGENTCODE].IsVisible = false;
            //}
            accountgrid.Columns[ACCOUNT].Width = 60;
            accountgrid.Columns[ROUTEIMG].Width = 20;
            accountgrid.Columns[EXECUTEIMG].Width = 20;
            accountgrid.Columns[PROFITLOSSIMG].Width = 20;
            accountgrid.Columns[LOGINSTATUSIMG].Width = 20;
            accountgrid.Columns[ADDRESS].Width = 120;
        }

        /// <summary>
        /// 刷新帐户筛选结果
        /// </summary>
        void RefreshAccountQuery()
        {
            if (!_loaded) return;
            string acctype = string.Empty;
            if (accountType.SelectedIndex == 0)
            {
                acctype = "*";
            }
            else
            {
                acctype = accountType.SelectedItem.Text;
            }
            
            string strFilter = string.Empty;
            //帐户类别
            if (accountType.SelectedIndex == 0)
            {
                strFilter = string.Format(CATEGORY + " > '{0}'", acctype);
            }
            else
            {
                strFilter = string.Format(CATEGORY + " = '{0}'", acctype);
            }


            //路由
            if (routeType.SelectedIndex != 0)
            {
                strFilter = string.Format(strFilter + " and " + ROUTE + " = '{0}'", routeType.SelectedValue.ToString());
            }

            if (accexecute.SelectedIndex != 0)
            {
                if (accexecute.SelectedIndex == 1)
                {
                    strFilter = string.Format(strFilter + " and " + EXECUTE + " = '{0}'",getExecuteStatus(true));
                }
                if (accexecute.SelectedIndex == 2)
                {
                    strFilter = string.Format(strFilter + " and " + EXECUTE + " = '{0}'",getExecuteStatus(false));
                }
            }

            if (accLogin.Checked)
            {
                strFilter = string.Format(strFilter + " and " + LOGINSTATUS + " = '{0}'", getLoginStatus(true));
            }

            string acctstr = acct.Text;
            if (!string.IsNullOrEmpty(acctstr))
            {
                strFilter = string.Format(strFilter + " and " + ACCOUNT + " like '{0}*'", acctstr);
            }
            debug("strfilter:" + strFilter, QSEnumDebugLevel.INFO);
            datasource.Filter = strFilter;
            UpdateAccountNum();
        }
        
        void UpdateAccountNum()
        {
            num.Text = accountgrid.Rows.Count.ToString();
        }


        #endregion

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


        #region 根据账户属性获得对应的string 或者 image
        /// <summary>
        /// 检查是否是当前选中的账户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        //bool isCurrentAccount(string account)
        //{
        //    if (_accselected == null) return false;
        //    if (_accselected.ID == account) return true;
        //    return false;
        //}
        Image getRouteStatusImage(QSEnumOrderTransferType type)
        {
            return type == QSEnumOrderTransferType.SIM ? (Image)Properties.Resources.Router_simuate : (Image)Properties.Resources.Router_real;

        }
        Image getExecuteStatusImage(bool execute)
        {
            if (execute)
                return (Image)Properties.Resources.account_go;
            else
                return (Image)Properties.Resources.account_stop;

        }
        string getExecuteStatus(bool execute)
        {
            return execute ? "execute" : "noexecute";
        }
        Image getLoginStatusImage(bool online)
        {
            if (online)
                return (Image)Properties.Resources.online;
            else
                return (Image)Properties.Resources.offline;
        }
        string getLoginStatus(bool online)
        {
            return online ? "online" : "offline";
        }

        Image getProfitLossImage(decimal profit)
        {
            if (profit > 0)
                return (Image)Properties.Resources.profit;
            if (profit < 0)
                return (Image)Properties.Resources.loss;
            return (Image)Properties.Resources.no_profit_loss;
        }
        #endregion


        #region 获得某个账户的table id与当前viewid用于更新数据源与现实情况
        private ConcurrentDictionary<string, IAccountLite> accountmap = new ConcurrentDictionary<string, IAccountLite>();
        private ConcurrentDictionary<string, int> accountrowmap = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// 获得某个账户在datatable中的序号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private int accountIdx(string account)
        {
            int rowid = -1;
            //map没有account键 还是会给out赋值,因此这里需要用if进行判断 来的到正确的逻辑 否则一致会返回0 出错
            if (!accountrowmap.TryGetValue(account, out rowid))
                return -1;
            else
                return rowid;
        }

        /// <summary>
        /// 查询是否存在某个交易帐号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private bool HaveAccount(string account)
        {
            return (accountmap.ContainsKey(account));
        }


        //得到当前选择的行号
        private string  CurrentAccount {

            get
            { 
                if (accountgrid.SelectedRows.Count > 0)
                {
                    return accountgrid.SelectedRows[0].ViewInfo.CurrentRow.Cells[ACCOUNT].Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }



        //通过行号得该行的Security
        IAccountLite  GetVisibleAccount(string account)
        {
            if (HaveAccount(account))
                return accountmap[account];
            return null;
        }

        #endregion





        #region 响应事件

        bool IsCurrentAccount(string account)
        {
            if (AccountSetlected == null) return false;
            if (account == AccountSetlected.Account) return true;
            return false;
        }

        public void GotTick(Tick k)
        {
            //debug("account montier got tick:" + k.ToString(), QSEnumDebugLevel.INFO);
            ctPositionView1.GotTick(k);
            viewQuoteList1.GotTick(k);
        }
        /// <summary>
        /// 获得服务端的帐户信息
        /// </summary>
        /// <param name="account"></param>
        public void GotAccount(IAccountLite account)
        {
            if (string.IsNullOrEmpty(account.Account))
                return;
            accountcache.Write(account);
        }

        /// <summary>
        /// 获得交易帐户的财务状态信息
        /// </summary>
        /// <param name="info"></param>
        public void GotAccountInfoLite(IAccountInfoLite info)
        {
            accountinfocache.Write(info);
        }

        /// <summary>
        /// 获得服务端转发的委托
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            debug("accountmoniter got order, accountselected:" + AccountSetlected.Account, QSEnumDebugLevel.INFO);
            if (IsCurrentAccount(o.Account) && Globals.TradingInfoTracker.IsReady(o.Account))
            {
                ctOrderView1.GotOrder(o);
                ctPositionView1.GotOrder(o);
            }
        }

        /// <summary>
        /// 获得服务端转发的成交
        /// </summary>
        /// <param name="f"></param>
        public void GotTrade(Trade f)
        {
            if (IsCurrentAccount(f.Account) && Globals.TradingInfoTracker.IsReady(f.Account))
            {
                ctTradeView1.GotFill(f);
                ctPositionView1.GotFill(f);
            }
        }

        /// <summary>
        /// 获得恢复交易帐户交易数据的开始和结束回报
        /// </summary>
        /// <param name="response"></param>
        public void GotMGRResumeResponse(RspMGRResumeAccountResponse response)
        {
            if (response.IsBegin)
            {
                debug("resume account:" + response.ResumeAccount + " start...", QSEnumDebugLevel.INFO);

                string account = response.ResumeAccount;
                IAccountLite acclit = null;
                if (accountmap.TryGetValue(account, out acclit))
                {
                   
                    Globals.TradingInfoTracker.StartResume(acclit);
                }
                else
                {
                    debug("无法找到对应的帐户:" + account, QSEnumDebugLevel.WARNING);
                }
            }
            else
            { 
                //数据恢复结束
                debug("resume account:" + response.ResumeAccount + " end...", QSEnumDebugLevel.INFO);
                Globals.TradingInfoTracker.EndResume();
                LoadAccountInfo(response.ResumeAccount);
            }
        
        }
        /// <summary>
        /// 获得客户端登入 退出状态更新
        /// </summary>
        /// <param name="notify"></param>
        public void GotMGRSessionUpdate(NotifyMGRSessionUpdateNotify notify)
        {
            sessionupdatecache.Write(notify);
        }

        /// <summary>
        /// 获得交易帐户财务信息
        /// </summary>
        /// <param name="accountinfo"></param>
        public void GotAccountInfo(IAccountInfo accountinfo)
        {
            fmaccountconfig.GotAccountInfo(accountinfo);
        }

        /// <summary>
        /// 响应帐户变动事件
        /// </summary>
        /// <param name="account"></param>
        public void GotAccountChanged(IAccountLite account)
        {
            accountcache.Write(account);
            fmaccountconfig.GotAccountChanged(account);
           
        }


        /// <summary>
        /// 帐户风控规则项目回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        public void GotRuleItem(RuleItem item, bool islast)
        {
            fmaccountconfig.GotRuleItem(item, islast);
        }

        public void GotRuleItemDel(RuleItem item, bool islast)
        {
            fmaccountconfig.GotRuleItemDel(item, islast);
        }

        #endregion


        #region 更新交易账户

        /// <summary>
        /// 检查帐户缓存中是否有帐号未填充
        /// </summary>
        public bool AnyAccountInCache
        {
            get
            {
                return accountcache.hasItems;
            }
        }
        bool _accountgo = false;
        Thread _accountthread = null;
        void acccountproc()
        {
            while (_accountgo)
            {
                try
                {
                    //更新账户主体信息
                    while (accountcache.hasItems)
                    {
                        IAccountLite account = accountcache.Read();
                        InvokeGotAccount(account);
                        UpdateAccountNum();
                        Thread.Sleep(1);
                    }

                    //更新账户登入 或者 注销 状态
                    while (!accountcache.hasItems && sessionupdatecache.hasItems)
                    {
                        NotifyMGRSessionUpdateNotify notify = sessionupdatecache.Read();
                        InvokeGotMGRSessionUpdate(notify);
                        Thread.Sleep(1);
                    }

                    //更新账户信息
                    while (!accountcache.hasItems && accountinfocache.hasItems)
                    {
                        InvokeGotAccountInfoLite(accountinfocache.Read());
                        Thread.Sleep(1);
                    }
                    

                    //设置观察帐户列表
                    SwtWathAccounts();
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    debug("Account信息更新错误:" + ex.ToString());
                }
            }
        }

        void StartUpdate()
        {
            if (_accountgo) return;
            _accountgo = true;
            _accountthread = new Thread(acccountproc);
            _accountthread.IsBackground = true;
            _accountthread.Start();
        }

        void StopUpdate()
        {
            if (!_accountgo) return;
            _accountgo = false;
            _accountthread.Abort();
        }





        const int bufferisze = 1000;
        RingBuffer<IAccountLite> accountcache = new RingBuffer<IAccountLite>(bufferisze);//交易帐户缓存
        RingBuffer<IAccountInfoLite> accountinfocache = new RingBuffer<IAccountInfoLite>(bufferisze);//交易帐户财务数据更新缓存
        RingBuffer<NotifyMGRSessionUpdateNotify> sessionupdatecache = new RingBuffer<NotifyMGRSessionUpdateNotify>(bufferisze);//交易帐户session更新缓存
        //RingBuffer<IAccountLite> accountchagnecache = new RingBuffer<IAccountLite>(bufferisze);//交易帐户变动缓存
        

       
        void InvokeGotAccount(IAccountLite account)
        {
            if (InvokeRequired)
            {
                Invoke(new IAccountLiteDel(InvokeGotAccount), new object[] { account });
            }
            else
            {
                try
                {
                    int r = accountIdx(account.Account);
                    if (r == -1)//datatable不存在该行，我们则增加该行
                    {
                        
                        gt.Rows.Add(account.Account);
                        int i = gt.Rows.Count - 1;
                        gt.Rows[i][ROUTE] = account.OrderRouteType.ToString();
                        gt.Rows[i][ROUTEIMG] = getRouteStatusImage(account.OrderRouteType);
                        gt.Rows[i][EXECUTE] = getExecuteStatus(account.Execute);
                        gt.Rows[i][EXECUTEIMG] = getExecuteStatusImage(account.Execute);
                        gt.Rows[i][PROFITLOSSIMG] = getProfitLossImage(0);
                        gt.Rows[i][LOGINSTATUS] = getLoginStatus(false);
                        gt.Rows[i][LOGINSTATUSIMG] = getLoginStatusImage(false);
                        gt.Rows[i][ADDRESS] = "";
                        gt.Rows[i][LASTEQUITY] = decDisp(account.LastEquity);

                        gt.Rows[i][NOWEQUITY] = decDisp(account.NowEquity);
                        gt.Rows[i][MARGIN] = decDisp(0);
                        gt.Rows[i][FROZENMARGIN] = decDisp(0);
                        gt.Rows[i][REALIZEDPL] = decDisp(0);
                        gt.Rows[i][UNREALIZEDPL] = decDisp(0);
                        gt.Rows[i][COMMISSION] = decDisp(0);
                        gt.Rows[i][PROFIT] = decDisp(0);
                        gt.Rows[i][CATEGORY] = Util.GetEnumDescription(account.Category);
                        gt.Rows[i][INTRADAY] = account.IntraDay ? "日内" : "隔夜";
                        Manager mgr = Globals.BasicInfoTracker.GetManager(account.MGRID);
                        gt.Rows[i][AGENTCODE] = mgr.Login + " - " + mgr.Name;
                        gt.Rows[i][NAME] = account.Name;
                        gt.Rows[i][POSLOK] = account.PosLock?"有":"无";

                        accountmap.TryAdd(account.Account, account);
                        accountrowmap.TryAdd(account.Account, i);
                        //debug("got account:" + account.Account, QSEnumDebugLevel.INFO);
                    }
                    else //如果存在表面是进行修改
                    {
                        accountmap[account.Account] = account;

                        gt.Rows[r][ROUTE] = account.OrderRouteType.ToString();
                        gt.Rows[r][ROUTEIMG] = getRouteStatusImage(account.OrderRouteType);
                        gt.Rows[r][EXECUTE] = getExecuteStatus(account.Execute);
                        gt.Rows[r][EXECUTEIMG] = getExecuteStatusImage(account.Execute);
                        gt.Rows[r][CATEGORY] = Util.GetEnumDescription(account.Category);
                        gt.Rows[r][INTRADAY] = account.IntraDay ? "日内" : "隔夜";
                        gt.Rows[r][POSLOK] = account.PosLock ? "有" : "无";

                        Manager mgr = Globals.BasicInfoTracker.GetManager(account.MGRID);
                        gt.Rows[r][AGENTCODE] = mgr.Login +" - "+ mgr.Name;
                        gt.Rows[r][NAME] = account.Name;

                    }

                }
                catch (Exception ex)
                {
                    debug("got account error:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                }
            }
        }




        delegate void IAccountInfoLiteDel(IAccountInfoLite account);
        void InvokeGotAccountInfoLite(IAccountInfoLite account)
        {
            if (InvokeRequired)
            {
                Invoke(new IAccountInfoLiteDel(InvokeGotAccountInfoLite), new object[] { account });
            }
            else
            {
                string acc = account.Account;
                int r = accountIdx(acc);
                if (r == -1)
                    return;
                else
                {

                    gt.Rows[r][NOWEQUITY] = decDisp(account.NowEquity);
                    gt.Rows[r][MARGIN] = decDisp(account.Margin);
                    gt.Rows[r][FROZENMARGIN] = decDisp(account.ForzenMargin);
                    gt.Rows[r][REALIZEDPL] = decDisp(account.RealizedPL);
                    gt.Rows[r][UNREALIZEDPL] = decDisp(account.UnRealizedPL);
                    gt.Rows[r][COMMISSION] = decDisp(account.Commission);
                    gt.Rows[r][PROFIT] = decDisp(account.Profit);
                    gt.Rows[r][PROFITLOSSIMG] = getProfitLossImage(account.Profit);
                }
            }
        }


        delegate void MGRSessionUpdateDel(NotifyMGRSessionUpdateNotify notify);
        void InvokeGotMGRSessionUpdate(NotifyMGRSessionUpdateNotify notify)
        {
            if (InvokeRequired)
            {
                Invoke(new MGRSessionUpdateDel(InvokeGotMGRSessionUpdate), new object[] { notify });
            }
            else
            {
                
                string acc = notify.TradingAccount;
                int  i= accountIdx(acc);
                debug("got sessionupdate info account:" + notify.TradingAccount + " status:" + notify.IsLogin.ToString() +" rindex:"+i.ToString(), QSEnumDebugLevel.INFO);
                if (i == -1)
                    return;
                else
                {
                    gt.Rows[i][LOGINSTATUS] = getLoginStatus(notify.IsLogin);
                    gt.Rows[i][LOGINSTATUSIMG] = getLoginStatusImage(notify.IsLogin);
                    if (notify.IsLogin)
                    {
                        gt.Rows[i][ADDRESS] = notify.IPAddress;
                    }
                    else
                    {
                        gt.Rows[i][ADDRESS] = "";
                    }
                }
            }
        }
        #endregion


        #region 动态更新帐户观察列表
        DateTime _gridChangeTime;//滚动时间
        int _freshdeay = 2;//滚动停止多少秒后开始刷新数据
        bool _watchchanged = false;

        void GridChanged()
        {
            _watchchanged = true;
            _gridChangeTime = DateTime.Now;
        }


        List<string> GetVisualAccounts()
        {
            List<string> accountlist = new List<string>();
            int num = this.accountgrid.TableElement.VisualRows.Count;
            for (int i = 1; i < num; i++)
            {
                accountlist.Add(this.accountgrid.TableElement.VisualRows[i].VisualCells[0].Value.ToString());
            }
            return accountlist;
        }

        /// <summary>
        /// 设定观察账户列表
        /// </summary>
        void SwtWathAccounts()
        {
            if ((!_watchchanged) || (DateTime.Now - _gridChangeTime).TotalSeconds < _freshdeay) return;
            Globals.TLClient.ReqWatchAccount(GetVisualAccounts());
            _watchchanged = false;
        }

        #endregion

        





        

        

        private void accountgrid_Click(object sender, EventArgs e)
        {
            debug("grid mouse clicked...", QSEnumDebugLevel.INFO);

        }


        #region 事件操作

        /// <summary>
        /// accountgrid滚动条滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            GridChanged();
        }

        /// <summary>
        /// accountgrid改变大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountgrid_Resize(object sender, EventArgs e)
        {
            GridChanged();
        }


        IAccountLite accountselected = null;

        public IAccountLite AccountSetlected { get { return accountselected; } }
        /// <summary>
        /// accountgrid双击某行
        /// 获取该交易帐户日内交易数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accountgrid_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show("curent account:" + CurrentAccount);
            string account = CurrentAccount;
            IAccountLite accountlite = null;
            if(accountmap.TryGetValue(account,out accountlite))
            {
                //设定选中帐号
                accountselected = accountlite;
                lbCurrentAccount.Text = account;

                //A 更新交易记录区域
                //清空交易记录
                ClearTradingInfo();
                //请求恢复交易帐户交易记录
                Globals.TLClient.ReqResumeAccount(account);

                if (ctOrderSenderM1 != null)
                {
                    ctOrderSenderM1.SetAccount(accountlite);
                }

                //B 更新ServiceTab区域
                ServiceTabRefresh();
            }
        }
        #endregion

        
        /// <summary>
        /// 清空交易记录
        /// </summary>
        void ClearTradingInfo()
        {
            Globals.TradingInfoTracker.Clear();
            ctOrderView1.Clear();
            ctPositionView1.Clear();
            ctTradeView1.Clear();
        }

        void LoadAccountInfo(string account)
        {
            debug("try to load trading info tracker account:" + Globals.TradingInfoTracker.Account.Account + " request account:" + account, QSEnumDebugLevel.INFO);
            if (Globals.TradingInfoTracker.Account.Account.Equals(account))
            {
                ctPositionView1.PositionTracker = Globals.TradingInfoTracker.PositionTracker;
                ctPositionView1.OrderTracker = Globals.TradingInfoTracker.OrderTracker;
                ctOrderView1.OrderTracker = Globals.TradingInfoTracker.OrderTracker;

                foreach (Position pos in Globals.TradingInfoTracker.HoldPositionTracker)
                {
                    ctPositionView1.GotPosition(pos);
                }

                foreach (Order o in Globals.TradingInfoTracker.OrderTracker)
                {
                    ctOrderView1.GotOrder(o);
                }

                foreach (Trade f in Globals.TradingInfoTracker.TradeTracker)
                {
                    ctTradeView1.GotFill(f);
                    ctPositionView1.GotFill(f);
                    //ctOrderView1
                }

            }
            else
            {
                debug("TradingInfoTracker 维护帐户与请求加载帐户不一致..", QSEnumDebugLevel.ERROR);
            }

        }




        #region 对外交易操作

        void CancelOrder(long oid)
        {
            OrderAction actoin = new OrderActionImpl();
            actoin.Account = AccountSetlected.Account;
            actoin.ActionFlag = QSEnumOrderActionFlag.Delete;
            actoin.OrderID = oid;
            SendOrderAction(actoin);
        }
        void SendOrderAction(OrderAction action)
        {
            Globals.TLClient.ReqOrderAction(action);
        }

        void SendOrder(Order o)
        {
            o.Account = AccountSetlected.Account;
            Globals.TLClient.ReqOrderInsert(o);
        }

        #endregion

        private void accountgrid_ContextMenuOpening(object sender, Telerik.WinControls.UI.ContextMenuOpeningEventArgs e)
        {
            //debug("context menu opening",QSEnumDebugLevel.INFO);
            e.ContextMenu = menu.DropDown;
        }

        private void accountType_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshAccountQuery();
        }

        private void routeType_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshAccountQuery();
        }

        private void accexecute_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            RefreshAccountQuery();
        }

        private void accLogin_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            RefreshAccountQuery();
        }

        private void acct_TextChanged(object sender, EventArgs e)
        {
            RefreshAccountQuery();
        }

        private void accountgrid_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            try
            {
                if (e.CellElement.RowInfo is GridViewDataRowInfo)
                {
                    if (e.CellElement.ColumnInfo.Name == REALIZEDPL)
                    {
                        decimal value =decimal.Parse( e.CellElement.RowInfo.Cells[REALIZEDPL].Value.ToString());

                        if (value < 0)
                        {
                            e.CellElement.ForeColor = UIGlobals.ShortSideColor;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else if (value > 0)
                        {
                            e.CellElement.ForeColor = UIGlobals.LongSideColor;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else
                        {
                            e.CellElement.ForeColor = UIGlobals.DefaultColor;
                            e.CellElement.Font = UIGlobals.DefaultFont;
                        }
                    }

                    if (e.CellElement.ColumnInfo.Name == UNREALIZEDPL)
                    {
                        decimal value = decimal.Parse(e.CellElement.RowInfo.Cells[UNREALIZEDPL].Value.ToString());

                        if (value < 0)
                        {
                            e.CellElement.ForeColor = UIGlobals.ShortSideColor;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else if (value > 0)
                        {
                            e.CellElement.ForeColor = UIGlobals.LongSideColor;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else
                        {
                            e.CellElement.ForeColor = UIGlobals.DefaultColor;
                            e.CellElement.Font = UIGlobals.DefaultFont;
                        }
                    }

                    if (e.CellElement.ColumnInfo.Name == PROFIT)
                    {
                        decimal value = decimal.Parse(e.CellElement.RowInfo.Cells[PROFIT].Value.ToString());

                        if (value < 0)
                        {
                            e.CellElement.ForeColor = UIGlobals.ShortSideColor;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else if (value > 0)
                        {
                            e.CellElement.ForeColor = UIGlobals.LongSideColor;
                            e.CellElement.Font = UIGlobals.BoldFont;
                        }
                        else
                        {
                            e.CellElement.ForeColor = UIGlobals.DefaultColor;
                            e.CellElement.Font = UIGlobals.DefaultFont;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                debug("!!!!!!!!!!!!cell format error");
            }
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

       


    }
}
