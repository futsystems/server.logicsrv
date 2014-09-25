using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter
{
    public partial class MainForm : Telerik.WinControls.UI.RadForm, ILogicHandler
    {

        
        Log logfile = null;

        TLClientNet tlclient;
        bool _connected = false;
        bool _logined = false;
        bool _gotloginrep = false;
        bool _basicinfodone = false;//基本数据是否已经查询完毕
        event DebugDelegate ShowInfoHandler;
        
        string _servers = "127.0.0.1";


        RouterMoniterForm routerform;
        ExchangeForm exchangeform;
        MarketTimeForm markettimeform;
        SecurityForm securityform;
        SymbolForm symbolform;
        SystemStatusForm systemstatusfrom;
        HistQryForm histqryform;
        BasicInfoTracker basicinfotracker;
        ManagerForm mgrform;
        void ShowInfo(string msg)
        {
            if (ShowInfoHandler != null)
                ShowInfoHandler(msg);
            logfile.GotDebug(msg);
        }


        void debug(string msg)
        {
            ctDebug1.GotDebug(msg);
            logfile.GotDebug(msg);
        }

        TradingInfoTracker infotracker;
        System.Threading.Timer _timer;
        public MainForm(DebugDelegate showinfo)
        {
            InitializeComponent();
            logfile = new Log(Globals.Config["LogFileName"].AsString(), true, true, "log", true);//日志组件

            //设定对外消息显示输出
            ShowInfoHandler = showinfo;
            

            ThemeResolutionService.ApplicationThemeName = Globals.Config["ThemeName"].AsString();

            if (Globals.Config["HeaderImg"].AsString().Equals("OEM"))
            {
                this.Icon = Properties.Resources.moniter_oem;
            }
            Init();
            _timer = new System.Threading.Timer(FakeOutStatus, null, 800, 150);
        }

        public void Init()
        {
            
            ctAccountMontier1.SendDebugEvent +=new DebugDelegate(debug);

            infotracker = new TradingInfoTracker();
            Globals.RegisterInfoTracker(infotracker);
            basicinfotracker = new BasicInfoTracker();
            Globals.RegisterBasicInfoTracker(basicinfotracker);
            routerform = new RouterMoniterForm();
            exchangeform = new ExchangeForm();
            markettimeform = new MarketTimeForm();
            securityform = new SecurityForm();
            symbolform = new SymbolForm();
            systemstatusfrom = new SystemStatusForm();
            histqryform = new HistQryForm();

            mgrform = new ManagerForm();

            basicinfotracker.GotMarketTimeEvent += new MarketTimeDel(markettimeform.GotMarketTime);
            basicinfotracker.GotExchangeEvent += new ExchangeDel(exchangeform.GotExchange);
            basicinfotracker.GotSecurityEvent += new SecurityDel(securityform.GotSecurity);
            basicinfotracker.GotSymbolEvent += new SymbolDel(symbolform.GotSymbol);

            basicinfotracker.GotManagerEvent += new ManagerDel(mgrform.GotManager);

            Globals.SendDebugEvent +=new DebugDelegate(debug);

            if (!Globals.Config["Agent"].AsBool())
            {
                btnManagerGP.Enabled = false;
            }
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

        public void Login(string server, string username, string pass)
        {
            _servers = server;
            //在后台线程执行客户端tlclient初始化
            new Thread(InitClient).Start();

            string s = ".";
            DateTime now = DateTime.Now;
            while (!_connected && (DateTime.Now - now).TotalSeconds < 5)
            {
                ShowInfo("连接中" + s);
                Thread.Sleep(500);
                s += ".";
            }
            if (!_connected)
            {
                ShowInfo("连接超时,无法连接到服务器");
                Globals.LoginStatus.SetInitMessage("连接超时,无法连接到服务器");
                return;
            }
            else //如果连接服务器成功,则我们请求登入系统
            {
                debug("请求服务端登入");
                tlclient.ReqLogin(username, pass);
            }
            s = ".";
            now = DateTime.Now;
            while (!_gotloginrep && (DateTime.Now - now).TotalSeconds < 5)
            {
                ShowInfo("登入中" + s);
                Thread.Sleep(500);
                s += ".";
            }
            if (!_gotloginrep)
            {
                ShowInfo("登入超时,无法登入到服务器");
                Globals.LoginStatus.SetInitMessage("登入超时,无法登入到服务器");
                return;
            }
            else
            {
                if (_logined)
                {
                    ShowInfo("登入成功,请求下载帐户列表");
                    
                    //查询基础数据
                    Globals.TLClient.ReqQryMarketTime();

                    s = ".";
                    now = DateTime.Now;
                    while (!_basicinfodone && (DateTime.Now - now).TotalSeconds < 10)
                    {
                        ShowInfo("查询基本数据中" + s);
                        Thread.Sleep(500);
                        s += ".";
                    }
                    if (_basicinfodone)
                    {
                        if (Globals.Manager == null)
                        {
                            ShowInfo("柜员数据获取异常,请重新登入!");
                            Globals.LoginStatus.SetInitMessage("加载基础数据失败");

                        }
                        else
                        {
                            this.Text = Globals.Config["CopName"].AsString() + " " + Globals.Config["Version"].AsString() +"           柜员用户名:"+Globals.Manager.Login +" 名称:"+Globals.Manager.Name +" 类型:"+LibUtil.GetEnumDescription(Globals.Manager.Type);

                            //如果不是总平台柜员 隐藏

                            ctAccountMontier1.ValidView();
                            ShowInfo("初始化行情报表");
                            InitSymbol2View();
                            Globals.LoginStatus.IsInitSuccess = true;
                        }
                    }
                    else
                    {
                        ShowInfo("加载基础数据失败!");
                        Globals.LoginStatus.SetInitMessage("加载基础数据失败");
                    }
                }
                else
                {
                    ShowInfo("登入失败!");
                    Globals.LoginStatus.SetInitMessage("登入失败");
                    return;
                }
            }

        }

        void InitClient()
        {
            tlclient = new TLClientNet(new string[] { _servers }, 6670, true);
            tlclient.OnDebugEvent += new DebugDelegate(debug);
            tlclient.DebugLevel = QSEnumDebugLevel.DEBUG;
            tlclient.OnConnectEvent += new VoidDelegate(tlclient_OnConnectEvent);
            tlclient.OnDisconnectEvent += new VoidDelegate(tlclient_OnDisconnectEvent);
            tlclient.OnLoginEvent += new RspMGRLoginResponseDel(tlclient_OnLoginEvent);
            tlclient.BindLogicHandler(this);
            Globals.RegisterClient(tlclient);
            _gotloginrep = false;
            _logined = false;
            _connected = false;
            tlclient.Start();
        }

        void InitSymbol2View()
        { 
            foreach(Symbol sym in Globals.BasicInfoTracker.SymbolsTradable)
            {
                ctAccountMontier1.AddSymbol(sym);
                //Globals.Debug("symbol:" + sym.Symbol);
            }
        }

        void tlclient_OnLoginEvent(RspMGRLoginResponse response)
        {
            debug("获得登入回报:" + response.ToString());
            _gotloginrep = true;
            if (response.Authorized)
            {
                _logined = true;
                Globals.MgrFK = response.mgr_fk;//保存管理端登入获得的全局ID用于获取Manager列表时 绑定对应的Manager
            }
            else
            {
                _logined = false;
            }
        }

        void tlclient_OnDisconnectEvent()
        {
            _connected = false;
        }


        void tlclient_OnConnectEvent()
        {
            _connected = true;
        }

        private void ctAccountMontier1_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void radMenuItem6_Click(object sender, EventArgs e)
        {
            InitSymbol2View();
        }






        
        void StatusMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new StringParamDelegate(StatusMessage), new object[] { message });
            }
            else
            {
                statusmessage.Opacity = 1;
                statusmessage.Text = message;
            }
        }

        void FakeOutStatus(object obj)
        { 
            double o = statusmessage.Opacity - 0.05;
            statusmessage.Opacity = o >= 0 ? o : 0;
        }

        




       



    }
}
