using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;//记得加入此引用


using TradingLib.API;
using TradingLib.Common;


/*委托核心处理流程
 * 1.客户端发送直接委托
 * 2.客户发送快捷指令,服务端生成对应的委托
 * 3.管理端发送委托
 * 4.系统内部风控生成委托
 * 
 * 关于委托的状态
 * 1.委托初始状态为Unknown
 * 2.经过风控检查 满足接受委托条件后 委托状态->Placed 此时表面系统接受了该委托 但是没有提交到 成交接口
 * 3.在lock(account)内 系统将委托提交到BrokerRouter 然后通过Broker检查开平状态 并选择对应的成交接口对外发送(成交接口必须实现线程安全)
 * 4.成交接口返回委托需要有一定的时间延迟,因此在计算冻结资金时需要将提交到接口的Order也计算在内,否则当连续发送委托会导致在成交接口返回的这个时间差内,其他委托检查到的资金占用偏小(漏掉了已经发送到成交接口的委托)
 * 发送到成交接口 则资金就必须被冻结。因此在原有的Open/(委托在成交接口处于等待成交状态) partfilled Submited必须计算在冻结资金的范围内
 * 
 **/
namespace TradingLib.Core
{

    /// <summary>
    /// TradingServer是整体的中转站,他负责底层的tlserver处理将客户端请求进行逻辑处理后分发到对应其他的组件
    /// 并且接受其他组件回报过来的信息并转给客户端
    /// </summary>
    public partial class MsgExchServer : BaseSrvObject, IMessageExchange, ICore
    {
        const string CoreName = "MsgExch";

        #region 对外触发的事件
        /// <summary>
        /// 行情事件
        /// </summary>
        public event TickDelegate GotTickEvent;
        
        /// <summary>
        /// 委托事件
        /// </summary>
        public event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 委托错误事件
        /// </summary>
        public event OrderErrorDelegate GotOrderErrorEvent;

        /// <summary>
        /// 委托操作事件
        /// </summary>
        public event OrderActionDelegate GotOrderActionEvent;

        /// <summary>
        /// 委托操作错误事件
        /// </summary>
        public event OrderActionErrorDelegate GotOrderActionErrorEvent;

        /// <summary>
        /// 成交事件
        /// </summary>
        public event FillDelegate GotFillEvent;


        /// <summary>
        /// 客户端注册事件
        /// </summary>
        public event ClientInfoDelegate<TrdClientInfo> ClientRegistedEvent;
        /// <summary>
        /// 客户端注销事件
        /// </summary>
        public event ClientInfoDelegate<TrdClientInfo> ClientUnregistedEvent;


        
        /// <summary>
        /// 客户端登入成功事件
        /// </summary>
        public event AccoundIDDel AccountLoginSuccessEvent;

        /// <summary>
        /// 客户端登入失败事件
        /// </summary>
        public event AccoundIDDel AccountLoginFailedEvent;

        /// <summary>
        /// 客户端登入 退出事件
        /// </summary>
        public event ClientLoginInfoDelegate<TrdClientInfo> ClientLoginInfoEvent;
        /// <summary>
        /// 客户端登入成功后回报消息事件
        /// </summary>
       // public event AccountIdDel NotifyLoginSuccessEvent;

        /// <summary>
        /// 客户端会话状态变化事件
        /// 1.客户端登入
        /// 2.客户端注销
        /// 3.客户端硬件地址变更
        /// </summary>
        //public event ISessionDel AccountSessionChangedEvent;


        /// <summary>
        /// 通过第三方认证中心进行认证,系统中可以加载认证插件
        /// 用于绑定到交易路由服务然后进行用户认证
        /// 如果没有绑定改事件则进行默认的本地数据库表交易帐号与密码的认证
        /// </summary>
        public event LoginRequestDel<TrdClientInfo> AuthUserEvent;
        #endregion



        public int ClientNum { get { return tl.NumClients; } }//连接的客户端数目
        public int ClientLoggedInNum { get { return tl.NumClientsLoggedIn; } }//登入的客户端数目
        


        ClearCentre _clearcentre;
        public void BindClearCentre(ClearCentre c)
        {
            _clearcentre = c;
        }

        SettleCentre _settlecentre;
        /// <summary>
        /// 结算中心
        /// </summary>
        public SettleCentre SettleCentre { get { return _settlecentre; } set { _settlecentre = value; } }


        RiskCentre _riskcentre;
        public void BindRiskCentre(RiskCentre r)
        {
            _riskcentre = r;
        }


        //QSEnumServerMode _srvmode = QSEnumServerMode.StandAlone;
        /// <summary>
        /// 设定服务模式,单机需要转发Tick,分布式不转发Tick
        /// </summary>
        //public QSEnumServerMode ServerMode { get { return _srvmode; } set { _srvmode = value; tl.Mode = value; } }
        public int DefaultBarsBack { get; set; }
        public int WaitBetweenEvents = 50;
        public bool ReleaseDeadSymbols = false;
        public bool AllowSendInvalidBars = false;
        public bool ReleaseBarHistoryAfteRequest = true;
        bool _valid = false;
        public bool isValid { get { return _valid; } }
        bool _barrequestsgetalldata = true;
        public bool BarRequestsGetAllData { get { return _barrequestsgetalldata; } set { _barrequestsgetalldata = value; } }



        TLServer_Exch tl;


        #region 委托编号 成交编号生成
        //委托编号生成器
        IdTracker _orderIDTracker = new IdTracker();

        //委托流水号
        int _maxOrderSeq = 0;//当前最大委托流水
        int _startOrderSeq = 0;//起始流水号
        bool _enbaleRandomOrderSeq = false;//是否随机委托流水号
        int _stepOrderSeqLow = 1;//步长最小值
        int _stepOrderSeqHigh = 10;//步长最大值
        Random _orderRandom = new Random(100);

        object _orderseqobj = new object();
        /// <summary>
        /// 获得委托流水号
        /// </summary>
        public int NextOrderSeq
        {

            get
            {
                lock (_orderseqobj)
                {

                    if (_enbaleRandomOrderSeq)
                    {
                        _maxOrderSeq += _orderRandom.Next(_stepOrderSeqLow, _stepOrderSeqHigh);
                        return _maxOrderSeq;
                    }
                    else
                    {
                        _maxOrderSeq += 1;
                        return _maxOrderSeq;
                    }
                }
            }
        }

        //成交流水号
        int _maxTradeID = 0;//当前最大委托流水
        int _startTradeID = 0;//起始流水号
        bool _enbaleRandomTradeID = false;//是否随机委托流水号
        int _stepTradeIDLow = 1;//步长最小值
        int _stepTradeIDHigh = 10;//步长最大值
        Random _tradeRandom = new Random(200);

        object _tradeseqobj = new object();
        /// <summary>
        /// 获得委托流水号
        /// </summary>
        public int NextTradeID
        {

            get
            {
                lock (_tradeseqobj)
                {

                    if (_enbaleRandomTradeID)
                    {
                        _maxTradeID += _tradeRandom.Next(_stepTradeIDLow, _stepTradeIDHigh);
                        return _maxTradeID;
                    }
                    else
                    {
                        _maxTradeID += 1;
                        return _maxTradeID;
                    }
                }
            }
        }

        /// <summary>
        /// 绑定唯一的委托编号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void AssignOrderID(ref Order o)
        {
            if (o.id <= 0)
                o.id = _orderIDTracker.AssignId;
            //获得本地递增流水号
            o.OrderSeq = this.NextOrderSeq;
        }

        public void AssignTradeID(ref Trade f)
        {
            //系统本地给成交赋日内唯一流水号 成交端的TradeID由接口负责
            f.TradeID = this.NextTradeID.ToString();

        }

        #endregion




        //交易路由管理器以及数据路由管理器
        BrokerRouter _brokerRouter = null;
        DataFeedRouter _datafeedRouter = null;

        public string CoreId { get { return this.PROGRAME; } }

        ConfigDB _cfgdb;

        string commentFilled = string.Empty;
        string commentPartFilled = string.Empty;
        string commentCanceled = string.Empty;
        string commentPlaced = string.Empty;
        string commentSubmited = string.Empty;
        string commentOpened = string.Empty;

        //Server将融合多个Broker和DataFeed通道
        //int _orderlimitsize = 0;
        bool needConfirmSettlement = true;
        //单一客户端登入
        int loginTerminalNum = 6;
        int workernum = 5;
        public MsgExchServer()
            : base(MsgExchServer.CoreName)
        {
            try
            {

                debug("初始化TradingServer");

                //1.加载配置文件
                _cfgdb = new ConfigDB(MsgExchServer.CoreName);
                if (!_cfgdb.HaveConfig("TLServerIP"))
                {
                    _cfgdb.UpdateConfig("TLServerIP", QSEnumCfgType.String,"*", "TL_MQ监听IP地址");
                }
                if (!_cfgdb.HaveConfig("TLPort"))
                {
                    _cfgdb.UpdateConfig("TLPort", QSEnumCfgType.Int, 5570, "TL_MQ监听Base端口");
                }
                if (!_cfgdb.HaveConfig("VerbDebug"))
                {
                    _cfgdb.UpdateConfig("VerbDebug", QSEnumCfgType.Bool,false.ToString(), "是否输出verb日志");
                }

                #region 委托流水号
                if (!_cfgdb.HaveConfig("StartOrderSeq"))
                {
                    _cfgdb.UpdateConfig("StartOrderSeq", QSEnumCfgType.Int, 1000, "默认起始委托流水号");
                }
                _startOrderSeq = _cfgdb["StartOrderSeq"].AsInt();

                if (!_cfgdb.HaveConfig("RandomOrderSeqEnable"))
                {
                    _cfgdb.UpdateConfig("RandomOrderSeqEnable", QSEnumCfgType.Bool, true, "启用委托流水号随机");
                }
                _enbaleRandomOrderSeq = _cfgdb["RandomOrderSeqEnable"].AsBool();

                if (!_cfgdb.HaveConfig("OrderSeqStepLow"))
                {
                    _cfgdb.UpdateConfig("OrderSeqStepLow", QSEnumCfgType.Int, 50, "委托流水号随机步长低值");
                }
                _stepOrderSeqLow = _cfgdb["OrderSeqStepLow"].AsInt();

                if (!_cfgdb.HaveConfig("OrderSeqStepHigh"))
                {
                    _cfgdb.UpdateConfig("OrderSeqStepHigh", QSEnumCfgType.Int, 100, "委托流水号随机步长高值");
                }
                _stepOrderSeqHigh = _cfgdb["OrderSeqStepHigh"].AsInt();
                #endregion

                #region 成交编号
                if (!_cfgdb.HaveConfig("StartTradeID"))
                {
                    _cfgdb.UpdateConfig("StartTradeID", QSEnumCfgType.Int, 2000, "默认起始成交编号");
                }
                _startTradeID = _cfgdb["StartTradeID"].AsInt();

                if (!_cfgdb.HaveConfig("RandomTradeIDEnable"))
                {
                    _cfgdb.UpdateConfig("RandomTradeIDEnable", QSEnumCfgType.Bool, true, "启用成交编号随机");
                }
                _enbaleRandomTradeID = _cfgdb["RandomTradeIDEnable"].AsBool();

                if (!_cfgdb.HaveConfig("TradeIDStepLow"))
                {
                    _cfgdb.UpdateConfig("TradeIDStepLow", QSEnumCfgType.Int, 50, "成交编号号随机步长低值");
                }
                _stepTradeIDLow = _cfgdb["TradeIDStepLow"].AsInt();

                if (!_cfgdb.HaveConfig("TradeIDStepHigh"))
                {
                    _cfgdb.UpdateConfig("TradeIDStepHigh", QSEnumCfgType.Int, 150, "成交编号随机步长高值");
                }
                _stepTradeIDHigh = _cfgdb["TradeIDStepHigh"].AsInt();
                #endregion


                if (!_cfgdb.HaveConfig("CommentFilled"))
                {
                    _cfgdb.UpdateConfig("CommentFilled", QSEnumCfgType.String,"全部成交", "全部成交备注");
                }
                commentFilled = _cfgdb["CommentFilled"].AsString();

                if (!_cfgdb.HaveConfig("CommentPartFilled"))
                {
                    _cfgdb.UpdateConfig("CommentPartFilled", QSEnumCfgType.String,"部分成交", "部分成交备注");
                }
                commentPartFilled = _cfgdb["CommentPartFilled"].AsString();

                if (!_cfgdb.HaveConfig("CommentCanceled"))
                {
                    _cfgdb.UpdateConfig("CommentCanceled", QSEnumCfgType.String, "委托已取消", "取消委托备注");
                }
                commentCanceled = _cfgdb["CommentCanceled"].AsString();

                if (!_cfgdb.HaveConfig("CommentPlaced"))
                {
                    _cfgdb.UpdateConfig("CommentPlaced", QSEnumCfgType.String, "已接受", "提交委托备注");
                }
                commentPlaced = _cfgdb["CommentPlaced"].AsString();

                if (!_cfgdb.HaveConfig("CommentSubmited"))
                {
                    _cfgdb.UpdateConfig("CommentSubmited", QSEnumCfgType.String, "已提交", "发送委托备注");
                }
                commentSubmited = _cfgdb["CommentSubmited"].AsString();

                if (!_cfgdb.HaveConfig("CommentOpened"))
                {
                    _cfgdb.UpdateConfig("CommentOpened", QSEnumCfgType.String, "已经报入", "取消委托备注");
                }
                commentOpened = _cfgdb["CommentOpened"].AsString();

                if (!_cfgdb.HaveConfig("NeedConfirmSettlement"))
                {
                    _cfgdb.UpdateConfig("NeedConfirmSettlement", QSEnumCfgType.Bool,true, "是否需要确认结算单");
                }
                needConfirmSettlement = _cfgdb["NeedConfirmSettlement"].AsBool();

                if (!_cfgdb.HaveConfig("LoginTerminalNum"))
                {
                    _cfgdb.UpdateConfig("LoginTerminalNum", QSEnumCfgType.Int, 6, "客户端允许登入终端个数");
                }

                loginTerminalNum = _cfgdb["LoginTerminalNum"].AsInt();

                if (!_cfgdb.HaveConfig("MessageWorkerNum"))
                {
                    _cfgdb.UpdateConfig("MessageWorkerNum", QSEnumCfgType.Int,5, "消息处理Worker数量");
                }

                workernum = _cfgdb["MessageWorkerNum"].AsInt();
                workernum = (workernum <= 0 ? 5 : workernum);


                tl = new TLServer_Exch(CoreName,_cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt(), true);

                //tl = new TLServer_Exch("TradingServer", _cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt());
                //VerboseDebugging = _cfgdb["VerbDebug"].AsBool();
                tl.ProviderName = Providers.QSPlatform;
                tl.NumWorkers = workernum;


                //设定日志输出
                //tl.VerboseDebugging = false;
                //tlserver内部直接发送的消息通过回调将消息缓存到外部缓存中进行队列发送
                tl.CachePacketEvent +=new IPacketDelegate(CachePacket);
                //查找对应交易账户
                tl.newLoginRequest += new LoginRequestDel<TrdClientInfo>(tl_newLoginRequest);
                //处理Symbol数据请求
                tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
                //处理Feature请求
                tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
                //处理Order提交
                tl.newSendOrderRequest += new OrderDelegate(tl_newSendOrderRequest);
                //处理OrderAction操作
                tl.newOrderActionRequest += new OrderActionRequestDel(tl_newOrderActionRequest);

                //处理其他请求消息
                tl.newPacketRequest += new PacketRequestDel(tl_newPacketRequest);

                tl.ClientRegistedEvent += (TrdClientInfo c) =>
                    {
                        if (ClientRegistedEvent != null)
                        {
                            ClientRegistedEvent(c);
                        }
                        //debug("客户端:" + c.Location.ClientID + " 注册到系统", QSEnumDebugLevel.INFO);
                    };
                tl.ClientUnregistedEvent += (TrdClientInfo c) =>
                    {
                        if (ClientUnregistedEvent != null)
                        {
                            ClientUnregistedEvent(c);
                        }
                        //debug("客户端:" + c.Location.ClientID + " 从系统注销", QSEnumDebugLevel.INFO);
                    };
                tl.ClientLoginInfoEvent += (TrdClientInfo c, bool login) =>
                    {
                        if (ClientLoginInfoEvent != null)
                        {
                            //检查对应的帐户是否还有交易客户端
                            if(c.Account != null)
                            {
                                //注销操作
                                if (!login)
                                {
                                    //查询该交易帐户是否还有登入的回话 如果存在则不更新注销消息
                                    TrdClientInfo info = tl.ClientsForAccount(c.Account.ID).FirstOrDefault();
                                    if (info == null)
                                    {
                                        ClientLoginInfoEvent(c, login);
                                    }
                                    else
                                    {
                                        ClientLoginInfoEvent(info,true);//还有其他客户端登入，则显示该客户端回话信息
                                    }
                                }
                                else//登入操作
                                {
                                    ClientLoginInfoEvent(c, login);
                                }
                            }
                        }
                        debug("客户端:" + c.Location.ClientID + " 登入状态:"+login.ToString(), QSEnumDebugLevel.INFO);
                    };

                int maxorderseq = ORM.MTradingInfo.MaxOrderSeq();
                _maxOrderSeq = maxorderseq > _startOrderSeq ? maxorderseq : _startOrderSeq;
                debug("Max OrderSeq:" + _maxOrderSeq, QSEnumDebugLevel.INFO);

                int maxtradeid = ORM.MTradingInfo.MaxTradeID();
                _maxTradeID = maxtradeid > _startTradeID ? maxtradeid : _startTradeID;
                debug("Max TradeID:" + _maxTradeID, QSEnumDebugLevel.INFO);


                //初始化优先发送缓存对象
                InitPriorityBuffer();
                //启动消息服务
                StartMessageRouter();

                TLCtxHelper.EventAccount.AccountCashOperationEvent += new Action<string, QSEnumCashOperation, decimal>(EventAccount_AccountCashOperationEvent);
                TLCtxHelper.EventAccount.AccountTradingNoticeEvent += new Action<string, string>(EventAccount_AccountTradingNoticeEvent);
            }
            catch (Exception ex)
            {
                debug("初始化服务异常:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                throw (new QSTradingServerInitError(ex));
            }
        }

        void EventAccount_AccountTradingNoticeEvent(string arg1, string arg2)
        {
            NotifyTradingNotice(arg1, arg2);
        }

        void EventAccount_AccountCashOperationEvent(string arg1, QSEnumCashOperation arg2, decimal arg3)
        {
            NotifyCashOperation(arg1, arg2, arg3);
        }

        


        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            tl.Dispose();
            tl = null;

            //是否将messagerouter放入stop start的地方
            StopMessageRouter();
        }






        #region 其他函数部分
        /// <summary>
        /// 返回某个交易帐户所有终端
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<ClientInfoBase> GetNotifyTargets(string account)
        {
            return tl.ClientsForAccount(account);
        }

        /// <summary>
        /// 恢复交易连接数据
        /// </summary>
        public void RestoreSession()
        {
            tl.RestoreSession();
        }

        void CachePacket(IPacket packet)
        {
            _packetcache.Write(packet);
        }



        #endregion


        Dictionary<string, MarketData> mdmap = new Dictionary<string, MarketData>();
        Dictionary<string, Tick> mdtickmap = new Dictionary<string, Tick>();
        /// <summary>
        /// 从数据库加载上个交易日的市场数据
        /// 飞迅客户端需要查询市场数据来获得隔日持仓的结算价信息
        /// </summary>
        void ReloadMarketData()
        {
            mdtickmap.Clear();
            mdmap.Clear();
            foreach(var d in ORM.MSettlement.SelectMarketData(TLCtxHelper.CmdSettleCentre.LastSettleday))
            {
                try
                {
                    mdmap[d.Symbol] = d;
                    Tick k = new TickImpl();
                    k.Symbol = d.Symbol;
                    k.AskPrice = d.AskPrice;
                    k.AskSize = d.AskSize;
                    k.BidPrice = d.BidPrice;
                    k.BidSize = d.BidSize;
                    k.Date = TLCtxHelper.CmdSettleCentre.LastSettleday;
                    k.Time = 0;
                    k.Trade = d.Close;
                    k.UpperLimit = d.UpperLimit;
                    k.Vol = d.Vol;
                    k.High = d.High;
                    k.Low = d.Low;
                    k.LowerLimit = d.LowerLimit;
                    k.Open = d.Open;
                    k.OpenInterest = d.OI;
                    k.PreOpenInterest = d.PreOI;
                    k.PreSettlement = d.PreSettlement;
                    k.Settlement = d.Settlement;
                    k.Size = 0;

                    mdtickmap[d.Symbol] = k;
                }
                catch (Exception ex)
                {
                    Util.Debug("load symbol:" + d.Symbol + " marketdata error:" + ex.ToString());
                }
            }
        }

        #region 开始 停止部分
        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            //debug("##########启动交易服务###################",QSEnumDebugLevel.INFO);
            try
            {
                tl.Start();
                
            }
            catch (Exception ex)
            {
                _valid = false;
                return;
            }
            _valid = true;
            if (_valid)
            {
                debug("Trading Server Starting success");
                Notify("启动", "启动时间:" + DateTime.Now.ToString());
            }
            else
                debug("Trading Server Starting failed.");

            //加载昨日市场数据
            ReloadMarketData();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
            if (tl != null && tl.IsLive)
            {
                tl.Stop();
            }
        }
        #endregion
    }
}
