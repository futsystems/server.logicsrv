using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using CTP;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public partial class SymbolSyncCTPForm : Telerik.WinControls.UI.RadForm
    {
        public SymbolSyncCTPForm()
        {
            InitializeComponent();
        }

        public event SymbolImplDel GotSymbolImplEvent;

        CTPTraderAdapter tradeApi = null;

        
        private void btnSyncSymbol_Click(object sender, EventArgs e)
        {
            _FRONT_ADDR = ctpaddress.Text;
            _BrokerID = brokerid.Text;
            _UserID = username.Text;
            _Password = password.Text;

            synctrhead = new Thread(process);
            synctrhead.IsBackground = true;
            synctrhead.Start();
            btnSyncSymbol.Enabled = false;
            isinsync = true;
        }


        string _FRONT_ADDR = "tcp://183.129.188.37:41205";  // 前置地址
        string _BrokerID = "1019";                       // 经纪公司代码
        string _UserID = "00000025";                       // 投资者代码
        string _Password = "123456";                     // 用户密码
        int iRequestID = 0;

        delegate void StringDel(string msg);
        void updatestatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new StringDel(updatestatus), new object[] { message });
            }
            else
            {
                status.Text = message;
            }
        }

        bool isinsync = false;
        Thread synctrhead = null;
        void process()
        {
            updatestatus("开始初始化CTP");
            Globals.Debug("开始初始化CTP");
            string path = System.IO.Directory.GetCurrentDirectory();
            path = System.IO.Path.Combine(path, "Cache4Trade\\");
            System.IO.Directory.CreateDirectory(path);

            //初始化交易接口
            tradeApi = new CTPTraderAdapter(path, false);

            tradeApi.OnRspError += new RspError(tradeApi_OnRspError);
            tradeApi.OnRspUserLogin += new RspUserLogin(tradeApi_OnRspUserLogin);

            tradeApi.OnFrontConnected += new FrontConnected(tradeApi_OnFrontConnected);
            tradeApi.OnFrontDisconnected += new FrontDisconnected(tradeApi_OnFrontDisconnected);
            tradeApi.OnRspQryInstrument += new RspQryInstrument(tradeApi_OnRspQryInstrument);

            Globals.Debug("注册前置地址");
            //注册到前置机并进行接口初始化
            tradeApi.RegisterFront(_FRONT_ADDR);
            tradeApi.Init();
            Globals.Debug("Join后台线程");
            tradeApi.Join();
        }


        void updatesycpect(int donenum, int totalnum)
        {
            string s = string.Format("共计:{0}条 已处理:{1}", totalnum, donenum);
            updatestatus(s);
        }
        
        
        List<ThostFtdcInstrumentField> instrumentlist = new List<ThostFtdcInstrumentField>();
        void tradeApi_OnRspQryInstrument(ThostFtdcInstrumentField pInstrument, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            instrumentlist.Add(pInstrument);
            if (bIsLast)
            {
                int totalnum = instrumentlist.Count;
                updatestatus("合约下载完毕,共计:" + totalnum.ToString() + "条,同步数据中");
                int i=0;
                foreach(ThostFtdcInstrumentField inst in instrumentlist)
                {
                    
                    if (inst == null || string.IsNullOrEmpty(inst.InstrumentID))
                        continue;
                    SymbolImpl sym = new SymbolImpl();
                    sym.Symbol = inst.InstrumentID;
                    sym.EntryCommission = -1;
                    sym.ExitCommission = -1;
                    sym.Margin = -1;
                    sym.ExtraMargin = -1;
                    sym.MaintanceMargin = -1;
                    sym.Strike = 0;
                    sym.OptionSide = QSEnumOptionSide.NULL;

                    int year = inst.DeliveryYear;
                    int month = inst.DeliveryMonth;
                    string expiremonth = string.Format("{0}{1:00}", year, month);

                    sym.ExpireMonth = int.Parse(expiremonth);
                    sym.ExpireDate = int.Parse(inst.ExpireDate);
                    SecurityFamilyImpl sec = Globals.BasicInfoTracker.GetSecurity(inst.ProductID);
                    sym.security_fk = sec!=null?sec.ID:0;
                    sym.Tradeable = defaulttradeable.Checked;//false;//inst.IsTrading == 1 ? true : false;//是否可交易

                    if (GotSymbolImplEvent != null)
                    {
                        GotSymbolImplEvent(sym,i== instrumentlist.Count-1);
                    }
                    i++;
                    updatesycpect(i, totalnum);
                    Thread.Sleep(50);
                }
                updatestatus("处理完毕");
                btnSyncSymbol.Enabled = true;
                isinsync = false;
            }
        }

        void tradeApi_OnFrontDisconnected(int nReason)
        {
            Globals.Debug("front disconnected");
            updatestatus("前置链接断开");
        }

        void tradeApi_OnFrontConnected()
        {
            Globals.Debug("front connected");
            updatestatus("前置链接建立");
            ReqLogin();
        }

        void tradeApi_OnRspUserLogin(ThostFtdcRspUserLoginField pRspUserLogin, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //Globals.Debug("user login errorid:"+pRspInfo.ErrorID.ToString());

            if (pRspInfo.ErrorID == 0)
            {
                ThostFtdcQryInstrumentField req = new ThostFtdcQryInstrumentField();
                req.InstrumentID = null;
                int iResult = tradeApi.ReqQryInstrument(req, ++iRequestID);
                //Globals.Debug(":--->>> 发送查询合约请求: " + ((iResult == 0) ? "成功" : "失败"));
                updatestatus("下载合约信息");
            }
            else
            {
                updatestatus("登入出错");
                isinsync = false;
                btnSyncSymbol.Enabled = true;
                if (tradeApi != null)
                {
                    tradeApi.Dispose();
                    tradeApi.Release();

                }
                if (synctrhead != null)
                {
                    synctrhead.Abort();
                }
            }
        }

        void tradeApi_OnRspError(ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            Globals.Debug("some error:" + pRspInfo.ErrorMsg);
        }



        void ReqLogin()
        {

            ThostFtdcReqUserLoginField req = new ThostFtdcReqUserLoginField();
            req.BrokerID = _BrokerID;
            req.UserID = _UserID;
            req.Password = _Password;
            int iResult = tradeApi.ReqUserLogin(req, ++iRequestID);
            updatestatus("请求登入");
            //Globals.Debug(":--->>> 发送用户登录请求: " + ((iResult == 0) ? "成功" : "失败"));
        }

        private void SymbolSyncCTPForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Globals.Debug("释放tradeapi");
            if (isinsync)
            {
                fmConfirm.Show("数据同步中,请稍等");
                e.Cancel = true;
                return;
            }
            updatestatus("释放API资源");
            if (tradeApi != null)
            {
                tradeApi.Dispose();
                tradeApi.Release();
            
            }
            if (synctrhead != null)
            {
                synctrhead.Abort();
            }

        }
    }


}
