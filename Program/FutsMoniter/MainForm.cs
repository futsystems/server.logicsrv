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


        System.Threading.Timer _timer;
        Ctx _ctx;
        public MainForm(DebugDelegate showinfo)
        {
            //绑定回调函数
            //Globals.RegisterCallBackCentre(this);

            _ctx = new Ctx();
            _ctx.InitStatusEvent += new Action<string>(ShowInfo);

            //初始化界面控件
            InitializeComponent();

            logfile = new Log(Globals.Config["LogFileName"].AsString(), true, true, "log", true);//日志组件

            //设定对外消息显示输出
            ShowInfoHandler = showinfo;

            if (Globals.Config["HeaderImg"].AsString().Equals("OEM"))
            {
                this.Icon = Properties.Resources.moniter_terminal;
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
            if (MoniterUtils.WindowConfirm("确认退出后台管理端?") != System.Windows.Forms.DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }



       


        public void Init()
        {

            ctAccountMontier1.SendDebugEvent += new DebugDelegate(debug);
            //ctAccountMontier1.QryAccountHistEvent += new IAccountLiteDel(ctAccountMontier1_QryAccountHistEvent);

            

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
            //basicinfotracker.Clear();
            //清空实时交易记录
            //infotracker.Clear();
        }





        void InitSymbol2View()
        {
            
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
