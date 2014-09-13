using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using TradingLib.Common;
using TradingLib.API;
using CTP;
using System.Windows.Forms;
using TradingLib;

namespace DataFeed.CTP
{
    public class CTPMD:IDataFeed
    {
      
        ConfigHelper cfg = new ConfigHelper(CfgConstDataFeedCTP.XMLFN);
        public string Title { get { return "CTP数据通道"; } }

        public bool IsLive { get { return CTPDataFeedLive; } }

        bool _verb = true;
        public bool VerboseDebugging { get { return _verb; } set { _verb = value; } }
        private void v(string msg)
        {
            if (_verb)
                debug(msg);
        }
        /// <summary>
        /// 当数据服务器登入成功后调用
        /// </summary>
        public event IConnecterParamDel Connected;
        /// <summary>
        /// 当数据服务器断开后触发事件
        /// </summary>
        public event IConnecterParamDel Disconnected;
        /// <summary>
        /// 当有日志信息输出时调用
        /// </summary>
        public event DebugDelegate SendDebugEvent;
        /// <summary>
        /// 当数据服务得到一个新的tick时调用
        /// </summary>
        public event TickDelegate GotTickEvent;
        //储存CTP lastTick数据 用于得到上一个tick,用来计算size
        Dictionary<string, ThostFtdcDepthMarketDataField> _symTickSnapMap = new Dictionary<string, ThostFtdcDepthMarketDataField>();
        //CTP dll的封装,CTPAdapter用于DataFeed
        private CTPMDAdapter mdAdapter = null;
        

        #region CPTAdapter回调函数
        private bool CTPDataFeedLive = false;//数据组件可用
        private bool CTPMDConnected = false;//启动标记,注册到前置机标识
        private bool ThreadStarted = false;//监听线程运行标记
        private Thread CTPMDThread = null;


        string _FRONT_ADDR = "tcp://180.168.212.56:41205";  // 前置地址
        string _BrokerID = "8888";                       // 经纪公司代码
        string _UserID = "00295";                       // 投资者代码
        string _Password = "123456";                     // 用户密码
        

        public string IPAddress { get { return _FRONT_ADDR; } set { _FRONT_ADDR = value; } }
        public string Broker { get { return _BrokerID; } set { _BrokerID = value; } }
        public string User { get { return _UserID; } set { _UserID = value; } }
        public string Pass { get { return _Password; } set { _Password = value; } }


        // 大连,上海代码为小写
        // 郑州,中金所代码为大写
        // 郑州品种年份为一位数
        //string[] ppInstrumentID = { "IF1209" };	//{"ag1212", "cu1207", "ru1209","TA209", "SR301", "y1301", "IF1206"};	// 行情订阅列表
        
        int iRequestID = 0;


        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.DEBUG;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if ((int)level <= (int)_debuglevel && _debugEnable)
                msgdebug("[" + level.ToString() + "] " + msg);
        }

        private void msgdebug(string s)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(s);
        }
        static string PROGRAME = "CTPMD";

        FormLoginTrade ul;

        bool exit = false;
        /// <summary>
        /// 启动过程
        /// 1.登入窗口显示,选择服务地址与用户
        /// 2.确认后，start_api 初始化接口
        /// 3.api启动后 前置连接建立 用户登入等一连串的事件触发
        /// </summary>
        public void Start()
        {
            exit = false;
            if (CTPMDConnected == false)
            {
                ul = new FormLoginTrade(cfg);
                ul.btnLogin.Click += new EventHandler(btnLogin_Click); //登录画面:按钮事件
                ul.btnExit.Click += new EventHandler(btnExit_Click);
                ul.TimeOutEvent += new VoidDelegate(ul_TimeOutEvent);
                if (ul.ShowDialog() == DialogResult.OK)
                {
                    /*if (strInfo != string.Empty)	//在userlogin中调用qrysettleinfo确保此条件成立
                    {
                        //显示确认结算窗口
                        using (SettleInfo info = new SettleInfo())
                        {
                            info.richTextInfo.Text = strInfo;
                            if (info.ShowDialog() != DialogResult.OK)//注有结算信息就会弹出结算对话框，如果确认则往下运行，如果不确认则直接返回。
                            {
                                //this.Close();
                                debug("初始化连接失败");
                                //tradeApi.DisConnect();
                                CTPTraderConnected = false;
                                return;//直接返回
                            }
                        }
                    }
                     * */
                }
                else
                {
                    exit = true;
                    CTPMDConnected = false;
                    return;
                }

            }

            debug(PROGRAME + ":启动数据通道完毕...", QSEnumDebugLevel.INFO);
            CTPDataFeedLive = true;
            //连接成功对外触发连接成功事件
            //if (Connected != null)
            //    Connected(this);
        }

        void ul_TimeOutEvent()
        {
            form_start_login();
        }

        //登录:生成tradeApi/注册事件
        void btnLogin_Click(object sender, EventArgs e)
        {
            (sender as Button).Enabled = false;
            form_start_login();
        }

        void form_start_login()
        {
            if (exit) return;
            if (ul.cbServer.SelectedIndex >= 0)
            {
                string[] servers = cfg.GetConfig(CfgConstDataFeedCTP.Servers).Split(',');
                string[] server = servers[ul.cbServer.SelectedIndex].Split('|');
                int save = Convert.ToInt16(cfg.GetConfig(CfgConstDataFeedCTP.SavePwd));

                if (server.Length >= 5)
                {
                    //通过读取设置来更新连接数据
                    _UserID = ul.tbUserID.Text;
                    _Password = ul.tbPassword.Text;

                    _BrokerID = server[1];
                    _FRONT_ADDR = server[2] + ":" + server[3];

                    //保存设置
                    servers[ul.cbServer.SelectedIndex] = server[0] + "|" + server[1] + "|" + server[2] + "|" + server[3] + "|" + ul.tbUserID.Text + "|" + (save == 1 ? ul.tbPassword.Text : "") + "|";
                    cfg.SetConfig(CfgConstDataFeedCTP.Servers, string.Join(",", servers));
                    cfg.SetConfig(CfgConstDataFeedCTP.SavePwd, (ul.savePWDCheckBox.Checked == true ? "1" : "0"));
                    start_api();

                }
            }
            
        }

        void start_api()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            path = System.IO.Path.Combine(path, "Cache4Md\\");
            System.IO.Directory.CreateDirectory(path);

            mdAdapter = new CTPMDAdapter(path, false);

            //注册事件
            mdAdapter.OnFrontConnected += new FrontConnected(OnFrontConnected);
            mdAdapter.OnFrontDisconnected += new FrontDisconnected(OnFrontDisconnected);

            mdAdapter.OnRspError += new RspError(OnRspError);

            mdAdapter.OnRspSubMarketData += new RspSubMarketData(OnRspSubMarketData);
            //mdAdapter.OnRspUnSubMarketData += new RspUnSubMarketData(OnRspUnSubMarketData);

            mdAdapter.OnRspUserLogin += new RspUserLogin(OnRspUserLogin);
            mdAdapter.OnRtnDepthMarketData += new RtnDepthMarketData(OnRtnDepthMarketData);

            //初始化接口
            mdAdapter.RegisterFront(_FRONT_ADDR);//->前置服务器连接成功后回调用户登入请求
            mdAdapter.Init();
            //mdAdapter.Join();
            CTPMDConnected = true;
            debug(PROGRAME+":初始化 Server Address:" + _FRONT_ADDR.ToString(),QSEnumDebugLevel.INFO);
        
        }
        //退出
        void btnExit_Click(object sender, EventArgs e)
        {
            //this.Close();
        }


        public bool CTPMDDispose()
        {
            if (mdAdapter != null)
            {
                debug(PROGRAME+":CTPAdapter release",QSEnumDebugLevel.INFO);
                mdAdapter.Release();
                CTPMDConnected = false;
                mdAdapter = null;
            }
            else
            {
                CTPMDConnected = false;
            }

            return true;
        }

        // 开始启动线程进行市场数据监听
        public bool RunCTPMD()
        {
            debug(PROGRAME+":开始监听数据...",QSEnumDebugLevel.INFO);
            if (CTPMDConnected == false)
                Start();

            if (!ThreadStarted)
            {
                ThreadStarted = true;
                //启动线程,等待api结束
                CTPMDThread = new Thread(new ThreadStart(ThreadFunc));
                CTPMDThread.IsBackground = true;
                CTPMDThread.Name = "CTPMarketData";
                CTPMDThread.Start();
                ThreadTracker.Register(CTPMDThread);
            }
            return true;
        }

        //获得tick数据的函数，用于线程中调用
        void ThreadFunc()
        {
            try
            {
                mdAdapter.Join();
            }
            catch (Exception e)
            {
                mdAdapter.Release();
            }
        }


        // 停止监听线程
        //注意当有数据请求的时候,我们无法正常关闭监听线程,在停止线程前我们需要正常关闭各个业务请求
        public void Stop()
        {
            //DebugPrintFunction(new StackTrace(false));
            debug(PROGRAME+":准备断开连接....", QSEnumDebugLevel.INFO);
            if (!CTPDataFeedLive) return;
            CTPDataFeedLive = false;

            if (ThreadStarted)
            {
                ThreadStarted = false;
                CTPMDDispose();
                if (CTPMDThread != null && !CTPMDThread.Join(200))
                {   
                    CTPMDThread.Abort();

                }
                CTPMDThread = null;
            
            
            }
            symlist.Clear();//清空本地订阅列表
            if (Disconnected != null)
                Disconnected(this);
        }



        bool IsErrorRspInfo(ThostFtdcRspInfoField pRspInfo)
        {
            // 如果ErrorID != 0, 说明收到了错误的响应
            bool bResult = ((pRspInfo != null) && (pRspInfo.ErrorID != 0));
            if (bResult)
                debug("--->>> ErrorID=" + pRspInfo.ErrorID.ToString() + "ErrorMsg=" + pRspInfo.ErrorMsg.ToString(),QSEnumDebugLevel.ERROR);
            return bResult;
        }

        //用户登入服务器
        public void ReqUserLogin()
        {
            //请求用户登入
            debug(PROGRAME+":请求用户登入",QSEnumDebugLevel.INFO);

            ThostFtdcReqUserLoginField req = new ThostFtdcReqUserLoginField();
            req.BrokerID = _BrokerID;
            req.UserID = _UserID;
            req.Password = _Password;
            int iResult = mdAdapter.ReqUserLogin(req, ++iRequestID);

            debug(PROGRAME+":--->>> 发送用户登录请求: " + ((iResult == 0) ? "成功" : "失败"));
        }
        /*
        //用户登出
        public void ReqUserLogout()
        {
            //请求用户登入
            debug("CTPMD:请求用户注销");

            ThostFtdcUserLogoutField req = new ThostFtdcUserLogoutField();
            req.BrokerID = _BrokerID;
            req.UserID = _UserID;
            //req.Password = _Password;
            int iResult = mdAdapter.ReqUserLogout(req, ++iRequestID);

            debug("CTPMD:--->>> 发送用户登出请求: " + ((iResult == 0) ? "成功" : "失败"));
        }
        **/

        List<string> symlist = new List<string>();
        //请求市场数据datafeed接口
        public void RegisterSymbols(string[] symbols)
        {
            if (!IsLive)
            {
                debug(PROGRAME+":没有正常启动,无法订阅市场数据");
                return;
            }

            bool havenew = false;
            foreach (string sym in symbols)
            {
                //如果本地订阅列表不包含该合约,则我们需要向服务端订阅
                if (!symlist.Contains(sym))
                {
                    symlist.Add(sym);
                    havenew = true;
                }
            
            }
            //如果有新合约,则我们重发订阅
            if(havenew)
                SubscribeMarketData(symlist.ToArray());
        }

        //订阅市场数据
        public void SubscribeMarketData(string[] symbols)
        {
            //请求市场数据
            debug(PROGRAME+":订阅市场数据:"+string.Join(",",symbols),QSEnumDebugLevel.INFO);
            int iResult = mdAdapter.SubscribeMarketData(symbols);
            debug(PROGRAME+":--->>> 发送行情订阅请求: " + ((iResult == 0) ? "成功" : "失败"));
        }
        /*
        //请求市场数据
        public void unSubscribeMarketData(string[] symbols)
        {
            //请求市场数据
            debug("CTPMD:退订市场数据");
            int iResult = mdAdapter.UnSubscribeMarketData(symbols);
            debug("CTPMD:--->>> 发送行情退订请求: " + ((iResult == 0) ? "成功" : "失败"));
        }**/

        /*
        void OnRspUserLogout(ThostFtdcUserLogoutField pUserLogout, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            debug("CTPMD:用户注销回报");
            
        }**/

        //当用户注册成功后的回报
        void OnRspUserLogin(ThostFtdcRspUserLoginField pRspUserLogin, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            debug(PROGRAME+":用户登入回报",QSEnumDebugLevel.INFO);
            if (bIsLast && !IsErrorRspInfo(pRspInfo))
            {
                ///获取当前交易日
                debug(PROGRAME+": 获取当前交易日 = " + mdAdapter.GetTradingDay(),QSEnumDebugLevel.INFO);
                debug(PROGRAME+":数据服务组件启动成功");
                //连接成功对外触发连接成功事件
                if (Connected != null)
                    Connected(this);
                ul.DialogResult = System.Windows.Forms.DialogResult.OK;	//退出登入窗口
            }
        }
        /*
        void OnRspUnSubMarketData(ThostFtdcSpecificInstrumentField pSpecificInstrument, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            debug("CTPMD:退订市场数据回报");
            //DebugPrintFunc(new StackTrace());
            //ReqUserLogout();
        }**/

        void OnRspSubMarketData(ThostFtdcSpecificInstrumentField pSpecificInstrument, ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            //debug("CTPMD:订阅市场数据回报",QSEnumDebugLevel.INFO);
            //DebugPrintFunc(new StackTrace());
        }

        void OnRspError(ThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast)
        {
            debug(PROGRAME+":错误回报");
            IsErrorRspInfo(pRspInfo);
        }
        /*
        void OnHeartBeatWarning(int nTimeLapse)
        {
            debug("CTPMD:心跳回报");
            //DebugPrintFunc(new StackTrace());
            debug("CTPMD:--->>> nTimerLapse = " + nTimeLapse);
        }**/

        void OnFrontDisconnected(int nReason)
        {
            debug(PROGRAME+":前置断开回报");
            debug(PROGRAME+":前置连接断开....", QSEnumDebugLevel.ERROR);
        }

        void OnFrontConnected()
        {
            debug(PROGRAME+":前置连接回报-准备启动监听线程",QSEnumDebugLevel.INFO);
            RunCTPMD();//前段连接注册成功后启动线程
            ReqUserLogin();//启动线程后登入用户
        }

        public ThostFtdcDepthMarketDataField getLastTick(string symbol)
        {
            ThostFtdcDepthMarketDataField mktdata = null;
            _symTickSnapMap.TryGetValue(symbol, out mktdata);
            return mktdata;
        }

        decimal getvalue(double value)
        {
            return Convert.ToDecimal(value);
        }
        //行情数据解析
        void OnRtnDepthMarketData(ThostFtdcDepthMarketDataField pDepthMarketData)
        {
            try
            {
                string sym = pDepthMarketData.InstrumentID;
                //形成本地的tick数据
                Tick k = new TickImpl(sym);

                k.ask = getvalue(pDepthMarketData.AskPrice1); 
                k.bid = getvalue(pDepthMarketData.BidPrice1);
                k.bs = pDepthMarketData.BidVolume1;
                k.os = pDepthMarketData.AskVolume1;
                //k.AskSize = pDepthMarketData.AskVolume1;
                //k.BidSize = pDepthMarketData.BidVolume1;


                k.trade = getvalue(pDepthMarketData.LastPrice);
                
                DateTime t = Convert.ToDateTime(pDepthMarketData.UpdateTime);
                k.time = Util.ToTLTime(t);
                k.date = Util.ToTLDate(t.Date);

                //tick extend field
                k.Vol = pDepthMarketData.Volume;
                k.Open = (decimal)pDepthMarketData.OpenPrice;
                k.High = (decimal)pDepthMarketData.HighestPrice;
                k.Low = (decimal)pDepthMarketData.LowestPrice;

                k.PreOpenInterest = Convert.ToInt32(pDepthMarketData.PreOpenInterest);
                k.OpenInterest = Convert.ToInt32(pDepthMarketData.OpenInterest);
                k.PreSettlement = (decimal)pDepthMarketData.PreSettlementPrice;
                //k.Settlement = (decimal)pDepthMarketData.SettlementPrice;


                ThostFtdcDepthMarketDataField mktData = null;
                //如果存在该symbol的上次ticksnapshot
                if (_symTickSnapMap.TryGetValue(sym, out mktData))
                {
                    k.size = pDepthMarketData.Volume - mktData.Volume;
                }
                else
                {
                    _symTickSnapMap.Add(sym, pDepthMarketData);//插入新的键值
                    k.size = pDepthMarketData.Volume;
                }
                _symTickSnapMap[sym] = pDepthMarketData;

                //debug("Tick:" + k.ToString(), QSEnumDebugLevel.INFO);
                if (GotTickEvent != null)
                    GotTickEvent(k);
            }
            catch (Exception ex)
            {
                debug(PROGRAME+":解析tick数据出错"+ ex.ToString(),QSEnumDebugLevel.ERROR);
            }
        }



        #endregion
    }
}
