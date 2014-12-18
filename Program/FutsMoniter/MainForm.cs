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

        TLClientNet tlclient;
        bool _connected = false;
        bool _logined = false;
        bool _gotloginrep = false;

        event DebugDelegate ShowInfoHandler;

        string _servers = "127.0.0.1";

        DebugForm debugform = new DebugForm();

        void ShowInfo(string msg)
        {
            if (ShowInfoHandler != null)
                ShowInfoHandler(msg);
            logfile.GotDebug(msg);
        }


        void debug(string msg)
        {

            debugform.GotDebug(msg);
            logfile.GotDebug(msg);
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

            Globals.SendDebugEvent += new DebugDelegate(debug);

            _ctx = new Ctx();
            _ctx.InitStatusEvent += new Action<string>(ShowInfo);

            logfile = new Log(Globals.Config["LogFileName"].AsString(), true, true, "log", true);//日志组件

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

        private int _count = 1;
        private KryptonPage NewPage(string name,string title ,int image, Control content)
        {
            if (existpage(name))
            {
                return null;
            }
            // Create new page with title and image
            KryptonPage p = new KryptonPage();
            p.Text = title;
            p.TextTitle = title;
            p.TextDescription = title;
            p.UniqueName = name;
            //p.ImageSmall = imageListSmall.Images[image];
            //ContentFlags contentFlags = new ContentFlags(p)
            // Add the control for display inside the page
            content.Dock = DockStyle.Fill;
            p.Controls.Add(content);
            
            //content.Disposed += new EventHandler(content_Disposed);
            pagemap.Add(name, p);
            
            //销毁一个页面
            //kryptonDockingManager.RemovePage(p.UniqueName, true);
            
            //p.Disposed += new EventHandler(p_Disposed);
            _count++;
            return p;
        }

        void p_Disposed(object sender, EventArgs e)
        {
            Globals.Debug("page disposed");
        }

        void content_Disposed(object sender, EventArgs e)
        {
            Globals.Debug("Content control disposed....");
        }

        Dictionary<string, KryptonPage> pagemap = new Dictionary<string, KryptonPage>();

        void WireEvent()
        {
            kryptonDockingManager.DockableWorkspaceCellAdding += new EventHandler<DockableWorkspaceCellEventArgs>(kryptonDockingManager_DockableWorkspaceCellAdding);
            kryptonDockingManager.PageCloseRequest += new EventHandler<CloseRequestEventArgs>(kryptonDockingManager_PageCloseRequest);
            kryptonDockingManager.PageDockedRequest += new EventHandler<CancelUniqueNameEventArgs>(kryptonDockingManager_PageDockedRequest);
            kryptonDockingManager.PageFloatingRequest += new EventHandler<CancelUniqueNameEventArgs>(kryptonDockingManager_PageFloatingRequest);
            
        }

        void kryptonDockingManager_PageFloatingRequest(object sender, CancelUniqueNameEventArgs e)
        {
            Globals.Debug("page floatin.........");
        }

        void kryptonDockingManager_PageDockedRequest(object sender, CancelUniqueNameEventArgs e)
        {
            Globals.Debug("page docking...");
        }

        void kryptonDockingManager_PageCloseRequest(object sender, CloseRequestEventArgs e)
        {
            Globals.Debug("page closeing........");
        }
        /// <summary>
        /// 
        /// </summary>
        void InitPage()
        {
            kryptonDockingManager.AddToWorkspace("Workspace", new KryptonPage[] { NewAccMoniter() });
            kryptonDockingManager.AddDockspace("Control", DockingEdge.Bottom, new KryptonPage[] { NewTradingInfoReal() });
            kryptonDockingManager.AddDockspace("Control", DockingEdge.Right, new KryptonPage[] { NewQuote(), NewAccFinInfo(), NewFinService() });
            
            
            if (System.IO.File.Exists("config.xml"))
            {
                kryptonDockingManager.LoadConfigFromFile("config.xml");
            }
            }

        void kryptonDockingManager_DockableWorkspaceCellAdding(object sender, DockableWorkspaceCellEventArgs e)
        {
            Globals.Debug("cell added......");
            e.CellControl.NavigatorMode = NavigatorMode.BarRibbonTabGroup;
        }

        private void UpdateCell(KryptonWorkspaceCell cell)
        {
            cell.NavigatorMode = NavigatorMode.BarTabGroup;
        }

        const string PAGE_FINSERVICE = "FINSERVICE";
        const string PAGE_QUOTE = "QUOTE";
        const string PAGE_TRADINGINFO = "TRADINGINFO";
        const string PAGE_ACCINFO = "ACCINFO";
        const string PAGE_ACCMONITER = "ACCMONITER";
            
        bool existpage(string name)
        {
            return pagemap.Keys.Contains(name);
        }
        private KryptonPage NewQuote()
        {

            return NewPage(PAGE_QUOTE, "报价与下单", 2, new ctQuoteMoniter());
        }

        private KryptonPage NewTradingInfoReal()
        {
            return NewPage(PAGE_TRADINGINFO, "交易记录", 2, new ctTradingInfoReal());
        }

        private KryptonPage NewFinService()
        {
            return NewPage(PAGE_FINSERVICE, "配资服务", 2, new ctFinService());
        }

        private KryptonPage NewAccFinInfo()
        {
            return NewPage(PAGE_ACCINFO, "财务信息", 2, new ctFinanceInfo());
        }

        private KryptonPage NewAccMoniter()
        {
            return NewPage(PAGE_ACCMONITER, "帐户监控", 2, new ctAccountMontier());
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
            tlclient.Stop();
            //清空基础数据
            //basicinfotracker.Clear();
            //清空实时交易记录
            //infotracker.Clear();
        }

        private void kryptonRibbonGroupButton7_Click(object sender, EventArgs e)
        {

            foreach (KryptonWorkspaceCell cell in kryptonDockingManager.CellsDocked)
                UpdateCell2(cell);

            foreach (KryptonWorkspaceCell cell in kryptonDockingManager.CellsFloating)
                UpdateCell2(cell);
        }

        private void UpdateCell2(KryptonWorkspaceCell cell)
        {
            cell.NavigatorMode = NavigatorMode.BarRibbonTabGroup;
        }








    }
}
