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

            pagemap.Add(name, p);
            _count++;
            return p;
        }

        Dictionary<string, KryptonPage> pagemap = new Dictionary<string, KryptonPage>();

        /// <summary>
        /// 
        /// </summary>
        void InitPage()
        {
            kryptonDockingManager.AddDockspace("Control", DockingEdge.Bottom, new KryptonPage[] { NewTradingInfoReal() });
            kryptonDockingManager.AddDockspace("Control", DockingEdge.Bottom, new KryptonPage[] { NewFinService(), NewAccFinInfo(), NewQuote() });
            kryptonDockingManager.AddToWorkspace("Workspace", new KryptonPage[] { NewAccMoniter() });


            if (System.IO.File.Exists("config.xml"))
            {
                //kryptonDockingManager.LoadConfigFromFile("config.xml");
            }
        }
        private void UpdateCell(KryptonWorkspaceCell cell)
        {
            cell.NavigatorMode = NavigatorMode.BarTabGroup;
        }


        bool existpage(string name)
        {
            return pagemap.Keys.Contains(name);
        }
        private KryptonPage NewQuote()
        {
            
            return NewPage("quote","报价与下单",2, new ctQuoteMoniter());
        }

        private KryptonPage NewTradingInfoReal()
        {
            return NewPage("tradinginfo","交易记录", 2, new ctTradingInfoReal());
        }

        private KryptonPage NewFinService()
        {
            return NewPage("finservice","配资服务", 2, new ctFinService());
        }

        private KryptonPage NewAccFinInfo()
        {
            return NewPage("accfininfo","财务信息", 2, new ctFinanceInfo());
        }

        private KryptonPage NewAccMoniter()
        {
            return NewPage("accmoniter","帐户监控", 2, new ctAccountMontier());
        }
        #endregion
        

        
        void MainForm_Load(object sender, EventArgs e)
        {
            InitBW();

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








    }
}
