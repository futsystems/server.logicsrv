﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

using TradingLib.API;
using TradingLib.Common;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using FutSystems.GUI;

namespace FutsMoniter.Controls
{
    /// <summary>
    /// 更新交易帐户的信息
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
        const string CATEGORY = "帐户类型";
        const string RACEENTRYTIME = "参赛日期";
        const string RACEID = "比赛编号";
        const string RACESTATUS = "比赛状态";

        const string INTRADAY = "日内";
        const string AGENTCODE = "代理编号";
        const string AGENTMGRFK = "AGENTMGRFK";
        const string NAME = "姓名";
        const string POSLOK = "锁仓权限";


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
            gt.Columns.Add(AGENTMGRFK);//22
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
                //accountgrid.Columns[AGENTCODE].IsVisible = false;
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
                    //更新账户盘中动态财务信息
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
                        gt.Rows[i][AGENTMGRFK] = account.MGRID;
                        gt.Rows[i][NAME] = account.Name;
                        gt.Rows[i][POSLOK] = account.PosLock ? "有" : "无";

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
                        gt.Rows[r][AGENTCODE] = mgr.Login + " - " + mgr.Name;
                        gt.Rows[r][NAME] = account.Name;
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
                debug("got sessionupdate info account:" + notify.TradingAccount + " status:" + notify.IsLogin.ToString() + " rindex:" + i.ToString(), QSEnumDebugLevel.INFO);
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
    }
}
