using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;
using TradingLib.ORM;
using Autofac;
using Autofac.Configuration;

namespace TradingSrv.Win
{
    public partial class MainForm : Form
    {
        ILog logger = LogManager.GetLogger("MainForm");
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            WireEvent();
        }

        
        void WireEvent()
        {
            ControlLogFactoryAdapter.SendDebugEvent += new Action<string>(PrintMsg);
            btnStart.Click += new EventHandler(btnStart_Click);
            btnStop.Click += new EventHandler(btnStop_Click);
            btnExitSrv.Click += new EventHandler(btnExitSrv_Click);

            btnOpenMainWindow.Click += new EventHandler(btnOpenMainWindow_Click);
            notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_DoubleClick);

            this.Resize += new EventHandler(MainForm_Resize);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        void btnExitSrv_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; //取消关闭窗体事件
            notifyIcon1.Visible = true;
            this.Hide();
            this.ShowInTaskbar = false;
        }

        void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.notifyIcon1.Visible = true;
            }
        }

        void btnOpenMainWindow_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }

        void btnStop_Click(object sender, EventArgs e)
        {
            logger.Info("Start TradingSrv...");
            
        }

        void btnStart_Click(object sender, EventArgs e)
        {
            logger.Info("Stop TradingSrv...");
            System.Threading.ThreadPool.QueueUserWorkItem(StartTradingSrv);
        }


        void PrintMsg(string msg)
        {
            ctDebug1.GotDebug(msg);
        }

        bool running = false;
        void StartTradingSrv(object arg)
        {
            try
            {
                running = true;

                logger.Info("********* start core daemon *********");
                System.OperatingSystem osInfo = System.Environment.OSVersion;
                System.PlatformID platformID = osInfo.Platform;
                Console.WriteLine(platformID.ToString());

                Util.StatusSection("Database", "INIT", QSEnumInfoColor.INFOGREEN, true);
                //读取配置文件 初始化数据库参数 系统其余设置均从数据库中加载
                ConfigFile _configFile = ConfigFile.GetConfigFile();
                DBHelper.InitDBConfig(_configFile["DBAddress"].AsString(), _configFile["DBPort"].AsInt(), _configFile["DBName"].AsString(), _configFile["DBUser"].AsString(), _configFile["DBPass"].AsString());

                //加载配置文件并生成容器
                var builder = new ContainerBuilder();
                builder.RegisterModule(new ConfigurationSettingsReader(TLCtxHelper.Version.ProductType.ToString(), Util.GetConfigFile("autofac.xml")));

                using (var container = builder.Build())
                {
                    using (var scope = container.BeginLifetimeScope())
                    {
                        //注册全局scope 用于动态生成组件
                        TLCtxHelper.RegisterScope(scope);

                        using (var coreMgr = scope.Resolve<ICoreManager>())//1.核心模块管理器,加载核心服务组件
                        {
                            coreMgr.Init();
                            using (var connectorMgr = scope.Resolve<IConnectorManager>())//2.路由管理器,绑定核心部分的数据与成交路由,并加载Connector
                            {
                                connectorMgr.Init();
                                using (var contribMgr = scope.Resolve<IContribManager>())//3.扩展模块管理器 加载扩展模块,启动扩展模块
                                {
                                    contribMgr.Init();
                                    contribMgr.Load();

                                    ////////////////////////////////// Stat Section
                                    //0.启动扩展服务
                                    contribMgr.Start();

                                    //1.待所有服务器启动完毕后 启动核心服务
                                    coreMgr.Start();

                                    //3.绑定扩展模块调用事件
                                    TLCtxHelper.BindContribEvent();

                                    //启动连接管理器 启动通道
                                    connectorMgr.Start();

                                    //解析版本信息
                                    TLCtxHelper.ParseVersion();
                                    //最后确认主备机服务状态，并启用全局状态标识，所有的消息接收需要该标识打开,否则不接受任何操作类的消息
                                    TLCtxHelper.IsReady = true;

                                    TLCtxHelper.PrintVersion();

                                    while (running)
                                    {
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error:" + ex.ToString());
            }
        }
    }
}
