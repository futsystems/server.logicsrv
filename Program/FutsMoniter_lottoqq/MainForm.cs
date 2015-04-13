using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class MainForm : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {

        Log logfile = null;
        
        //LogServer logsrv = null;

        TLClientNet tlclient;
        bool _connected = false;
        bool _logined = false;
        string _loginfailreason = string.Empty;
        bool _gotloginrep = false;

        event DebugDelegate ShowInfoHandler;

        string _servers = "127.0.0.1";

        DebugForm debugform = new DebugForm();

        void ShowInfo(string msg)
        {
            if (ShowInfoHandler != null)
                ShowInfoHandler(msg);
            logfile.GotDebug(msg);
            //logsrv.NeLog(msg);
        }


        void debug(string msg)
        {
            debugform.GotDebug(msg);
            Globals.Debug(msg);
        }

        Ctx _ctx;
        private ComponentFactory.Krypton.Docking.KryptonDockingManager kryptonDockingManager;
        //ComponentFactory.Krypton.Docking.KryptonDockableWorkspace kryptonDockableWorkspace
        public MainForm(DebugDelegate showinfo)
        {
            //初始化界面控件
            InitializeComponent();

            // Setup docking functionality
            kryptonDockingManager = new KryptonDockingManager();
            KryptonDockingWorkspace w = kryptonDockingManager.ManageWorkspace(kryptonDockableWorkspace);
            kryptonDockingManager.ManageControl(mainpanel, w);
            kryptonDockingManager.ManageFloating(this);

            //Globals.SendDebugEvent += new DebugDelegate(debug);

            _ctx = new Ctx();
            _ctx.InitStatusEvent += new Action<string>(ShowInfo);
            _ctx.GotBasicInfoDoneEvent += new VoidDelegate(_ctx_GotBasicInfoDoneEvent);

            logfile = new Log(Globals.Config["LogFileName"].AsString(), true, true, "log", true);//日志组件
            
            //logsrv = new LogServer();
            //logsrv.Start();
            //设定对外消息显示输出
            ShowInfoHandler = showinfo;

            if (Globals.Config["HeaderImg"].AsString().Equals("OEM"))
            {
                this.Icon = Properties.Resources.moniter_terminal;
            }

            this.Load += new EventHandler(MainForm_Load);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }



        #region page

        

        void p_Disposed(object sender, EventArgs e)
        {
            Globals.Debug("page disposed");
        }

        void content_Disposed(object sender, EventArgs e)
        {
            Globals.Debug("Content control disposed....");
        }

        
        void WireEvent()
        {
            kryptonDockingManager.DockableWorkspaceCellAdding += new EventHandler<DockableWorkspaceCellEventArgs>(kryptonDockingManager_DockableWorkspaceCellAdding);
            kryptonDockingManager.DockableWorkspaceCellRemoved += new EventHandler<DockableWorkspaceCellEventArgs>(kryptonDockingManager_DockableWorkspaceCellRemoved);
            //停靠
            kryptonDockingManager.DockspaceCellRemoved += new EventHandler<DockspaceCellEventArgs>(kryptonDockingManager_DockspaceCellRemoved);
            kryptonDockingManager.DockspaceCellAdding += new EventHandler<DockspaceCellEventArgs>(kryptonDockingManager_DockspaceCellAdding);
            //浮动
            kryptonDockingManager.FloatspaceCellAdding += new EventHandler<FloatspaceCellEventArgs>(kryptonDockingManager_FloatspaceCellAdding);
            kryptonDockingManager.FloatspaceCellRemoved += new EventHandler<FloatspaceCellEventArgs>(kryptonDockingManager_FloatspaceCellRemoved);

            
            kryptonDockingManager.FloatingWindowAdding += new EventHandler<FloatingWindowEventArgs>(kryptonDockingManager_FloatingWindowAdding);
            //关闭请求
            kryptonDockingManager.PageCloseRequest += new EventHandler<CloseRequestEventArgs>(kryptonDockingManager_PageCloseRequest);
            //停靠
            kryptonDockingManager.PageDockedRequest += new EventHandler<CancelUniqueNameEventArgs>(kryptonDockingManager_PageDockedRequest);
            //浮动请求
            kryptonDockingManager.PageFloatingRequest += new EventHandler<CancelUniqueNameEventArgs>(kryptonDockingManager_PageFloatingRequest);
            //自动隐藏
            kryptonDockingManager.PageAutoHiddenRequest += new EventHandler<CancelUniqueNameEventArgs>(kryptonDockingManager_PageAutoHiddenRequest);
            //进入工作区域
            kryptonDockingManager.PageWorkspaceRequest += new EventHandler<CancelUniqueNameEventArgs>(kryptonDockingManager_PageWorkspaceRequest);
            kryptonDockingManager.ShowPageContextMenu += new EventHandler<ContextPageEventArgs>(kryptonDockingManager_ShowPageContextMenu);
        }

        void kryptonDockingManager_ShowPageContextMenu(object sender, ContextPageEventArgs e)
        {
            Globals.Debug("show page context menu");
            e.Cancel = true;
        }

        void kryptonDockingManager_FloatingWindowAdding(object sender, FloatingWindowEventArgs e)
        {
            Globals.Debug("float window add");
            
        }

        void kryptonDockingManager_FloatspaceCellRemoved(object sender, FloatspaceCellEventArgs e)
        {
            Globals.Debug("float cell remove");
            

            //foreach (KryptonWorkspaceCell cell in kryptonDockingManager.CellsFloating)
            //    cell.NavigatorMode = NavigatorMode.BarRibbonTabGroup;
        }

        void kryptonDockingManager_FloatspaceCellAdding(object sender, FloatspaceCellEventArgs e)
        {
            Globals.Debug("float cell add");
            foreach (KryptonWorkspaceCell cell in kryptonDockingManager.CellsFloating)
                cell.NavigatorMode = NavigatorMode.BarRibbonTabGroup;
        }







        void kryptonDockingManager_PageWorkspaceRequest(object sender, CancelUniqueNameEventArgs e)
        {
            Util.Debug("page into workspace.....");
        }

        //自动隐藏
        void kryptonDockingManager_PageAutoHiddenRequest(object sender, CancelUniqueNameEventArgs e)
        {
            //帐户列表不允许自动隐藏
            if (e.UniqueName == PAGE_ACCMONITER)
            {
                e.Cancel = true;
                MoniterUtils.WindowConfirm("帐户监控不能隐藏");
            }
        }
        //浮动
        void kryptonDockingManager_PageFloatingRequest(object sender, CancelUniqueNameEventArgs e)
        {
            Globals.Debug("page floatin.........:"+e.Cancel.ToString());
            if (e.UniqueName == PAGE_ACCMONITER)
            {
                    //e.Cancel = 
            }
            //e.Cancel = true;
        }
        //停靠
        void kryptonDockingManager_PageDockedRequest(object sender, CancelUniqueNameEventArgs e)
        {
            Globals.Debug("page docking...");
            //帐户列表不允许停靠
            if (e.UniqueName == PAGE_ACCMONITER)
            {
                e.Cancel = true;
                MoniterUtils.WindowConfirm("帐户监控不能停靠");
            }
            
        }
        //关闭
        void kryptonDockingManager_PageCloseRequest(object sender, CloseRequestEventArgs e)
        {
            Globals.Debug("page closeing........");
            
        }

        void kryptonDockingManager_DockableWorkspaceCellAdding(object sender, DockableWorkspaceCellEventArgs e)
        {
            Globals.Debug("DockableWorkspace cell added......");
            e.CellControl.NavigatorMode = NavigatorMode.BarRibbonTabGroup;
        }

        void kryptonDockingManager_DockableWorkspaceCellRemoved(object sender, DockableWorkspaceCellEventArgs e)
        {
            Globals.Debug("DockableWorkspace cell removed......");
        }

        void kryptonDockingManager_DockspaceCellAdding(object sender, DockspaceCellEventArgs e)
        {
            Globals.Debug("dockspace cell added");
            e.CellControl.NavigatorMode = NavigatorMode.BarRibbonTabGroup;
        }

        void kryptonDockingManager_DockspaceCellRemoved(object sender, DockspaceCellEventArgs e)
        {
            Globals.Debug("dockspace cell removed");
        }


        
        #endregion
        

        
        void MainForm_Load(object sender, EventArgs e)
        {
            //绑定事件
            WireEvent();
            //绑定菜单事件
            WireRibbon();

            Globals.RegIEventHandler(this);
        }



        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MoniterUtils.WindowConfirm("确认退出后台管理端?") != System.Windows.Forms.DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }


        public void Reset()
        {
            //停止tlclient
            debug("MainForm Reset");
            tlclient.Stop();
            //清空基础数据
            //basicinfotracker.Clear();
            //清空实时交易记录
            //infotracker.Clear();
        }

        //private void kryptonRibbonGroupButton7_Click(object sender, EventArgs e)
        //{

        //    foreach (KryptonWorkspaceCell cell in kryptonDockingManager.CellsDocked)
        //        UpdateCell2(cell);

        //    foreach (KryptonWorkspaceCell cell in kryptonDockingManager.CellsFloating)
        //        UpdateCell2(cell);
        //}

        //private void UpdateCell2(KryptonWorkspaceCell cell)
        //{
        //    cell.NavigatorMode = NavigatorMode.BarRibbonTabGroup;
        //}

        void SetExpireStatus()
        {
            int days = (int)(Util.ToDateTime(Globals.Domain.DateExpired, 0) - DateTime.Now).TotalDays;
            if (days <= 7)
            {
                expireStatus.Text = "柜台授权即将过期,请及时续费或延期";
                expireStatus.Visible = true;
            }
        }






    }
}
