using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;//记得加入此引用


using TradingLib.API;
using TradingLib.Common;
using TradingLib;
using TradingLib.LitJson;

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
    public partial class MsgExchServer : BaseSrvObject, IMessageExchange, ICTrdReq, ICore
    {
        const string CoreName = "MsgExch";

        #region 对外触发的事件
        /// <summary>
        /// 将客户端的登入 退出信息提交给风控中心进行处理
        /// </summary>
        //public event LoginInfoDel SendLoginInfoEvent;
        /// <summary>
        /// 客户端报名事件
        /// </summary>
        //public event IAccountSignForPreraceDel SignupPreraceEvent;
        /// <summary>
        /// TradingServer接收到委托后进行委托发送前风控检查,事件回调了风控中心的风控规则检查函数
        /// </summary>
        //public event RiskCheckOrderDel SendOrderRiskCheckEvent;

        /// <summary>
        /// 获得某个账户的比赛信息
        /// </summary>
        //public event GetRaceInfoDel GetRaceInfoEvent;
        //IRaceInfo GetRaceInfo(string acc)
        //{
        //    if (GetRaceInfoEvent != null)
        //        return GetRaceInfoEvent(acc);
        //    return null;
        //}
       

        public event TickDelegate GotTickEvent;
        
        public event OrderDelegate GotOrderEvent;
        public event ErrorOrderDel GotErrorOrderEvent;

        public event LongDelegate GotCancelEvent;
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
        public event AccountIdDel AccountLoginSuccessEvent;

        /// <summary>
        /// 客户端登入失败事件
        /// </summary>
        public event AccountIdDel AccountLoginFailedEvent;

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
        public event ISessionDel AccountSessionChangedEvent;


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
        /// <summary>
        /// 绑定清算中心用于清算中心得到相关数据
        /// </summary>
        public ClearCentre ClearCentre{get { return _clearcentre; } set{ _clearcentre  =  value;}}


        SettleCentre _settlecentre;
        /// <summary>
        /// 结算中心
        /// </summary>
        public SettleCentre SettleCentre { get { return _settlecentre; } set { _settlecentre = value; } }


        RiskCentre _riskcentre;
        /// <summary>
        /// 绑定风控中心
        /// </summary>
        public RiskCentre RiskCentre { get { return _riskcentre; } set { _riskcentre = value; } }

        

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
        /// <summary>
        /// 主要将底层交易消息暴露给外层，用于实时修改日志级别
        /// </summary>
        //public IDebug TrdService { get { return tl; } }
        //委托编号生成器
        IdTracker _idt = new IdTracker();

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

                tl = new TLServer_Exch(CoreName,_cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt(), true);

                //tl = new TLServer_Exch("TradingServer", _cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt());
                //VerboseDebugging = _cfgdb["VerbDebug"].AsBool();
                tl.ProviderName = Providers.QSPlatform;
                tl.NumWorks = 5;

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
                tl.newPacketRequest += new TrdPacketRequestDel(tl_newPacketRequest);

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
                            ClientLoginInfoEvent(c, login);
                        }
                        debug("客户端:" + c.Location.ClientID + " 登入状态:"+login.ToString(), QSEnumDebugLevel.INFO);
                    };
                //初始化优先发送缓存对象
                InitPriorityBuffer();
                //启动消息服务
                StartMessageRouter();
                
            }
            catch (Exception ex)
            {
                debug("初始化服务异常:" + ex.ToString(),QSEnumDebugLevel.ERROR);
                throw (new QSTradingServerInitError(ex));
            }
        }

        


        public override void Dispose()
        {
            base.Dispose();
            tl.Dispose();
            tl = null;

            //是否将messagerouter放入stop start的地方
            StopMessageRouter();
        }






        #region 其他函数部分
        /// <summary>
        /// 通过Account查找对应的clientID如果登入了则返回地址 如果没有登入则返回null
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public string[] LoginID2ClientID(string account)
        {
            //return tl.AddListForAccount(account);
            return null;
        }

        /// <summary>
        ///通过地址反向查找其登入帐号,如果存在则返回account,若不存在则返回null
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public string AccountFromAddress(string address)
        {
            //return tl.AccountForAddress(address);
            return null;
        }

        public TrdClientInfo FirstClientInfoForAccount(string account)
        {
            TrdClientInfo[] list = tl.ClientsForAccount(account);
            if (list.Length > 0)
                return list[0];
            else
                return null;
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



        /// <summary>
        /// 获得某个合约的有效价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetAvabilePrice(string symbol)
        {
            if (_datafeedRouter == null)
            {
                return -1;
            }
            return _datafeedRouter.GetAvabilePrice(symbol);
        }
        #endregion

        #region 开始 停止部分
        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            debug("##########启动交易服务###################",QSEnumDebugLevel.INFO);
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
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            // request thread be stopped
            debug("##########停止交易服务###################", QSEnumDebugLevel.INFO);
            if (tl != null && tl.IsLive)
            {
                tl.Stop();
            }
        }
        #endregion
    }
}
