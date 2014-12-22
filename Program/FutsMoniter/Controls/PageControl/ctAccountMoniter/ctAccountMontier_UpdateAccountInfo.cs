using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    /// <summary>
    /// 更新交易帐户的信息
    /// 替换Grid性能大幅提高，后期会对控件做一个整理
    /// </summary>
    public partial class ctAccountMontier
    {
        #region 表格

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
        const string HOLDSIZE = "持";
        const string CATEGORYSTR = "帐户类型";
        const string CATEGORY = "CATEGORY";
        const string RACEENTRYTIME = "参赛日期";
        const string RACEID = "比赛编号";
        const string RACESTATUS = "比赛状态";

        const string INTRADAY = "日内";
        const string AGENTCODE = "代理编号";
        const string AGENTMGRFK = "AGENTMGRFK";
        const string NAME = "姓名";
        const string POSLOK = "锁仓权限";
        const string DELETE = "DELETE";
        const string ROUTERGROUP = "Group";
        const string ROUTERGROUPSTR = "路由组";


        DataTable gt = new DataTable();
        BindingSource datasource = new BindingSource();




        /// <summary>
        /// 设定表格控件的属性
        /// </summary>
        private void SetPreferences()
        {
            ComponentFactory.Krypton.Toolkit.KryptonDataGridView grid = accountgrid;

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeight = 25;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.StateCommon.Background.Color1 = Color.WhiteSmoke;
            grid.StateCommon.Background.Color2 = Color.WhiteSmoke;

        }

        //初始化Account显示空格
        private void InitTable()
        {
            gt.Columns.Add(ACCOUNT);//0
            gt.Columns.Add(NAME);//23
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
            gt.Columns.Add(REALIZEDPL);//13
            gt.Columns.Add(UNREALIZEDPL);//14

            gt.Columns.Add(COMMISSION, typeof(Decimal));//15
            gt.Columns.Add(PROFIT);//16
            gt.Columns.Add(HOLDSIZE);//17
            gt.Columns.Add(CATEGORY);//18
            gt.Columns.Add(CATEGORYSTR);
            gt.Columns.Add(INTRADAY);//19
            gt.Columns.Add(AGENTCODE);//20
            gt.Columns.Add(AGENTMGRFK);//21
            gt.Columns.Add(POSLOK);//22
            gt.Columns.Add(DELETE);
            gt.Columns.Add(ROUTERGROUP);
            gt.Columns.Add(ROUTERGROUPSTR);
            
        }
        
        /// <summary>
        /// 绑定数据表格到grid
        /// </summary>
        private void BindToTable()
        {
            datasource.DataSource = gt;
            datasource.Sort = ACCOUNT + " ASC";
            accountgrid.DataSource = datasource;

            accountgrid.Columns[EXECUTE].Visible = false;
            accountgrid.Columns[ROUTE].Visible = false;
            accountgrid.Columns[LOGINSTATUS].Visible = false;
            accountgrid.Columns[AGENTMGRFK].Visible = false;
            accountgrid.Columns[CATEGORY].Visible = false;
            accountgrid.Columns[DELETE].Visible = false;
            accountgrid.Columns[ROUTERGROUP].Visible = false;

            accountgrid.Columns[ACCOUNT].Width = 100;
            accountgrid.Columns[ROUTEIMG].Width = 30;
            accountgrid.Columns[EXECUTEIMG].Width = 20;
            accountgrid.Columns[PROFITLOSSIMG].Width = 20;
            accountgrid.Columns[LOGINSTATUSIMG].Width = 20;
            accountgrid.Columns[ADDRESS].Width = 120;
            accountgrid.Columns[HOLDSIZE].Width = 20;
             for (int i = 0; i < gt.Columns.Count; i++)
            {
                accountgrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        #endregion

        #region 帐户内存数据结构
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

        IAccountLite accountselected = null;
        public IAccountLite AccountSetlected { get { return accountselected; } }

        //得到当前选择的行号
        private string CurrentAccount
        {
            get
            {
                int row = (accountgrid.SelectedRows.Count > 0 ? accountgrid.SelectedRows[0].Index : -1);
                return accountgrid[0, row].Value.ToString();
            }
        }



        //通过行号得该行的Security
        IAccountLite GetVisibleAccount(string account)
        {
            //MessageBox.Show("account:" + account + " haveaccount:" + HaveAccount(account).ToString());
            if (HaveAccount(account))
                return accountmap[account];
            return null;
        }
        #endregion







        #region 更新交易账户

        /// <summary>
        /// 检查帐户缓存中是否有帐号未填充
        /// 用于初始化登入时 等待所有帐户加载完毕
        /// </summary>
        public bool AnyAccountInCache
        {
            get
            {
                return accountcache.hasItems;
            }
        }

        #region 根据账户属性获得对应的string 或者 image
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



        bool _accountgo = false;
        Thread _accountthread = null;
        
        void acccountproc()
        {
            while (_accountgo)
            {
               // Globals.Debug("got account in cache*************************");
                try
                {
                    //更新账户主体信息
                    while (accountcache.hasItems)
                    {
                       // Globals.Debug("got account in cache*************************");
                        IAccountLite account = accountcache.Read();
                        InvokeGotAccount(account);
                        UpdateAccountNum();
                        //如果在初始化之后获得AccountLite信息 则表明该帐户是新增造成的 需要重新watchaccount
                        if (Globals.EnvReady)
                        {
                            GridChanged();
                        }
                        Thread.Sleep(1);
                    }
                    //更新账户登入 或者 注销 状态
                    while (!accountcache.hasItems && sessionupdatecache.hasItems)
                    {
                        NotifyMGRSessionUpdateNotify notify = sessionupdatecache.Read();
                        InvokeGotMGRSessionUpdate(notify);
                        Thread.Sleep(20);
                    }
                    //更新账户盘中动态财务信息
                    while (!accountcache.hasItems && accountinfocache.hasItems)
                    {
                        //Globals.Debug("got account in cache*************************");
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


        /// <summary>
        /// 当有帐户新增或者初始化时调用
        /// </summary>
        /// <param name="account"></param>
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
                    int r = accountIdx(account.Account);//管理端是以account为唯一键值,应该不会出现重复？？
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
                        gt.Rows[i][CATEGORYSTR] = Util.GetEnumDescription(account.Category);
                        gt.Rows[i][CATEGORY] = account.Category.ToString();
                        gt.Rows[i][INTRADAY] = account.IntraDay ? "日内" : "隔夜";
                        ManagerSetting mgr = Globals.BasicInfoTracker.GetManager(account.MGRID);
                        gt.Rows[i][AGENTCODE] = mgr.Login + " - " + mgr.Name;
                        gt.Rows[i][AGENTMGRFK] = account.MGRID;
                        gt.Rows[i][NAME] = account.Name;
                        gt.Rows[i][POSLOK] = account.PosLock ? "有" : "无";
                        gt.Rows[i][DELETE] = account.Deleted;
                        //gt.Rows[i][ROUTERGROUP] = account.RG_ID;
                        //gt.Rows[i][ROUTERGROUPSTR] = ctRouterGroupList1.GetRrouterGroupName(account.RG_ID);

                        accountmap.TryAdd(account.Account, account);
                        accountrowmap.TryAdd(account.Account, i);
                        Globals.Debug("got account:" + account.Account);
                    }
                    else //如果存在表面是进行修改
                    {
                        accountmap[account.Account] = account;

                        gt.Rows[r][ROUTE] = account.OrderRouteType.ToString();
                        gt.Rows[r][ROUTEIMG] = getRouteStatusImage(account.OrderRouteType);
                        gt.Rows[r][EXECUTE] = getExecuteStatus(account.Execute);
                        gt.Rows[r][EXECUTEIMG] = getExecuteStatusImage(account.Execute);
                        gt.Rows[r][CATEGORYSTR] = Util.GetEnumDescription(account.Category);
                        gt.Rows[r][CATEGORY] = account.Category.ToString();
                        gt.Rows[r][INTRADAY] = account.IntraDay ? "日内" : "隔夜";
                        gt.Rows[r][POSLOK] = account.PosLock ? "有" : "无";

                        ManagerSetting mgr = Globals.BasicInfoTracker.GetManager(account.MGRID);
                        gt.Rows[r][AGENTCODE] = mgr.Login + " - " + mgr.Name;
                        gt.Rows[r][NAME] = account.Name;
                        gt.Rows[r][DELETE] = account.Deleted;

                        //gt.Rows[r][ROUTERGROUP] = account.RG_ID;
                        //gt.Rows[r][ROUTERGROUPSTR] = ctRouterGroupList1.GetRrouterGroupName(account.RG_ID);

                    }

                }
                catch (Exception ex)
                {
                    debug("got account error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }
        }



        /// <summary>
        /// 服务端推送的帐户实时财务数据
        /// </summary>
        /// <param name="account"></param>
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
                //Globals.Debug("account idx:" + r);
                if (r == -1)
                    return;
                else
                {
                    //Globals.Debug("account:"+account.Account + "now:" + decDisp(account.NowEquity) + " margin:" + decDisp(account.Margin));
                    gt.Rows[r][NOWEQUITY] = decDisp(account.NowEquity);
                    gt.Rows[r][MARGIN] = decDisp(account.Margin);
                    gt.Rows[r][FROZENMARGIN] = decDisp(account.ForzenMargin);
                    gt.Rows[r][REALIZEDPL] = decDisp(account.RealizedPL);
                    gt.Rows[r][UNREALIZEDPL] = decDisp(account.UnRealizedPL);
                    gt.Rows[r][COMMISSION] = decDisp(account.Commission);
                    gt.Rows[r][PROFIT] = decDisp(account.Profit);
                    gt.Rows[r][PROFITLOSSIMG] = getProfitLossImage(account.Profit);
                    gt.Rows[r][HOLDSIZE] = account.TotalPositionSize;
                }
            }
        }

        /// <summary>
        /// 登入登出状态更新
        /// </summary>
        /// <param name="notify"></param>
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
                int i = accountIdx(acc);
                //debug("got sessionupdate info account:" + notify.TradingAccount + " status:" + notify.IsLogin.ToString() + " rindex:" + i.ToString(), QSEnumDebugLevel.INFO);
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


        private void accountgrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 14 || e.ColumnIndex == 15 || e.ColumnIndex == 17)
            {
                e.CellStyle.Font = UIGlobals.BoldFont;
                decimal v = 0;
                decimal.TryParse(e.Value.ToString(), out v);
                if (v > 0)
                {
                    e.CellStyle.ForeColor = UIGlobals.LongSideColor;
                }
                else if (v < 0)
                {
                    e.CellStyle.ForeColor = UIGlobals.ShortSideColor;
                }
                else if (v == 0)
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Black;
                }

            }
        }

        void accountgrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts = e.PaintParts ^ DataGridViewPaintParts.Focus;
        }


        void ctRouterGroupList1_RouterGroupInitEvent()
        {
            int gid=0;
            for (int i = 0; i < gt.Rows.Count; i++)
            {
                int.TryParse(gt.Rows[i][ROUTERGROUP].ToString(),out gid);
                gt.Rows[i][ROUTERGROUPSTR] = ctRouterGroupList1.GetRrouterGroupName(gid);

            }
        }


    }
}
