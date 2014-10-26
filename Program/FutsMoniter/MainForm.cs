using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;

namespace FutsMoniter
{
    public partial class MainForm : ComponentFactory.Krypton.Toolkit.KryptonForm, ILogicHandler, ICallbackCentre,IEventBinder
    {

        Log logfile = null;

        TLClientNet tlclient;
        bool _connected = false;
        bool _logined = false;
        bool _gotloginrep = false;
        bool _basicinfodone = false;//基本数据是否已经查询完毕
        event DebugDelegate ShowInfoHandler;

        string _servers = "127.0.0.1";

        DebugForm debugform = new DebugForm();
        fmRouterMoniter routerform;
        fmExchange exchangeform;
        fmMarketTime markettimeform;
        fmSecurity securityform;
        fmSymbol symbolform;
        fmCoreStatus systemstatusfrom;
        fmHistQuery histqryform;
        BasicInfoTracker basicinfotracker;
        fmManagerCentre mgrform;
        fmAgentProfitReport agentprofitreportform;
        void ShowInfo(string msg)
        {
            if (ShowInfoHandler != null)
                ShowInfoHandler(msg);
            logfile.GotDebug(msg);
        }


        void debug(string msg)
        {
            //ctDebug1.GotDebug(msg);
            debugform.GotDebug(msg);
            logfile.GotDebug(msg);
        }

        TradingInfoTracker infotracker;
        System.Threading.Timer _timer;
        public MainForm(DebugDelegate showinfo)
        {
            //绑定回调函数
            Globals.RegisterCallBackCentre(this);

            

            //初始化界面控件
            InitializeComponent();


            logfile = new Log(Globals.Config["LogFileName"].AsString(), true, true, "log", true);//日志组件

            //设定对外消息显示输出
            ShowInfoHandler = showinfo;


            ///ThemeResolutionService.ApplicationThemeName = Globals.Config["ThemeName"].AsString();

            if (Globals.Config["HeaderImg"].AsString().Equals("OEM"))
            {
                this.Icon = Properties.Resources.moniter_oem;
            }


            Init();
            _timer = new System.Threading.Timer(FakeOutStatus, null, 800, 150);

            InitBW();
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
            this.Load += new EventHandler(MainForm_Load);
            ctAccountMontier1.Start();

            Globals.RegIEventHandler(this);
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
        }



       


        public void Init()
        {

            ctAccountMontier1.SendDebugEvent += new DebugDelegate(debug);
            ctAccountMontier1.QryAccountHistEvent += new IAccountLiteDel(ctAccountMontier1_QryAccountHistEvent);

            infotracker = new TradingInfoTracker();
            Globals.RegisterInfoTracker(infotracker);

            basicinfotracker = new BasicInfoTracker();
            Globals.RegisterBasicInfoTracker(basicinfotracker);



            routerform = new fmRouterMoniter();
            exchangeform = new fmExchange();
            markettimeform = new fmMarketTime();
            securityform = new fmSecurity();
            symbolform = new fmSymbol();
            systemstatusfrom = new fmCoreStatus();
            histqryform = new fmHistQuery();

            mgrform = new fmManagerCentre();

            agentprofitreportform = new fmAgentProfitReport();

            //基础数据窗口维护了基础数据 当有基础数据到达时候需要通知窗体 窗体进行加载和现实
            basicinfotracker.GotMarketTimeEvent += new MarketTimeDel(markettimeform.GotMarketTime);
            basicinfotracker.GotExchangeEvent += new ExchangeDel(exchangeform.GotExchange);
            basicinfotracker.GotSecurityEvent += new SecurityDel(securityform.GotSecurity);
            basicinfotracker.GotSymbolEvent += new SymbolDel(symbolform.GotSymbol);

            basicinfotracker.GotManagerEvent += new ManagerDel(mgrform.GotManager);

            Globals.SendDebugEvent += new DebugDelegate(debug);

            if (!Globals.Config["Agent"].AsBool())
            {
                //btnGPAgent.Enabled = false;
            }

            WireRibbon();

        }




        public void Reset()
        {
            //停止tlclient
            tlclient.Stop();
            //清空基础数据
            basicinfotracker.Clear();
            //清空实时交易记录
            infotracker.Clear();
        }





        void InitSymbol2View()
        {
            foreach (Symbol sym in Globals.BasicInfoTracker.SymbolsTradable)
            {
                ctAccountMontier1.AddSymbol(sym);
                //Globals.Debug("symbol:" + sym.Symbol);
            }
        }











        void StatusMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new StringParamDelegate(StatusMessage), new object[] { message });
            }
            else
            {
                //toolStripStatusLabel1.Opacity = 1;
                //toolStripStatusLabel1.Text = message;
            }
        }

        void FakeOutStatus(object obj)
        {
            //double o = toolStripStatusLabel1.Opacity - 0.05;
            //toolStripStatusLabel1.Opacity = o >= 0 ? o : 0;
        }

        private void radMenuItem1_Click(object sender, EventArgs e)
        {
            //MainForm2 mf = new MainForm2();
            //mf.Show();
        }

    }
}
