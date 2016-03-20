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
    public partial class MsgExchServer : ExCore, IMessageExchange, IModuleExCore
    {
        const string CoreName = "MsgExch";
        

        public int ClientNum { get { return tl.NumClients; } }//连接的客户端数目
        public int ClientLoggedInNum { get { return tl.NumClientsLoggedIn; } }//登入的客户端数目
        
        bool _valid = false;
        public bool isValid { get { return _valid; } }

        TLServer_Exch tl;

        bool needConfirmSettlement = true;
        int loginTerminalNum = 6;

        bool simpromptenable = false;
        string simprompt = string.Empty;

        BinaryOptionQuoteEngine boEngine = null;
        public MsgExchServer()
            : base(MsgExchServer.CoreName)
        {
            try
            {

                logger.Info("初始化TradingServer");

                
                if (!_cfgdb.HaveConfig("TLServerIP"))
                {
                    _cfgdb.UpdateConfig("TLServerIP", QSEnumCfgType.String,"*", "TL_MQ监听IP地址");
                }
                if (!_cfgdb.HaveConfig("TLPort"))
                {
                    _cfgdb.UpdateConfig("TLPort", QSEnumCfgType.Int, 5570, "TL_MQ监听Base端口");
                }
                //if (!_cfgdb.HaveConfig("VerbDebug"))
                //{
                //    _cfgdb.UpdateConfig("VerbDebug", QSEnumCfgType.Bool,false.ToString(), "是否输出verb日志");
                //}

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


                if (!_cfgdb.HaveConfig("SIMPromtEnable"))
                {
                    _cfgdb.UpdateConfig("SIMPromtEnable", QSEnumCfgType.Bool, false, "模拟委托注明模拟二字");
                }
                simpromptenable = _cfgdb["SIMPromtEnable"].AsBool();


                if (!_cfgdb.HaveConfig("SIMPromt"))
                {
                    _cfgdb.UpdateConfig("SIMPromt", QSEnumCfgType.String, "模拟", "模拟委托标注内容");
                }
                simprompt = _cfgdb["SIMPromt"].AsString();



                tl = new TLServer_Exch(CoreName,_cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt(), true);

                //tl = new TLServer_Exch("TradingServer", _cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt());
                //VerboseDebugging = _cfgdb["VerbDebug"].AsBool();
                tl.ProviderName = Providers.QSPlatform;
                tl.NumWorkers = 5;

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
                        TLCtxHelper.EventSession.FireClientConnectedEvent(c);
                        logger.Info("客户端:" + c.Location.ClientID + " 注册到系统");
                    };
                tl.ClientUnregistedEvent += (TrdClientInfo c) =>
                    {
                        TLCtxHelper.EventSession.FireClientDisconnectedEvent(c);
                        logger.Info("客户端:" + c.Location.ClientID + " 从系统注销");
                    };
                tl.ClientLoginInfoEvent += (TrdClientInfo c, bool login) =>
                    {
                        //检查对应的帐户是否还有交易客户端
                        if (c.Account != null)
                        {
                            //注销操作
                            if (!login)
                            {
                                //查询该交易帐户是否还有登入的回话 如果存在则不更新注销消息
                                TrdClientInfo info = tl.ClientsForAccount(c.Account.ID).FirstOrDefault();
                                if (info == null)
                                {
                                    //如果该交易帐户没有任何终端注册 则清空回话信息
                                    c.Account.UnBindClient();

                                    TLCtxHelper.EventSession.FireClientLoginInfoEvent(c, login);

                                }
                                else
                                {
                                    //还有其他客户端登入，则显示该客户端回话信息 同时回话信息绑定到该终端
                                    c.Account.BindClient(info);
                                    
                                    TLCtxHelper.EventSession.FireClientLoginInfoEvent(info, true);
                                    
                                }
                            }
                            else//登入操作
                            {
                                c.Account.BindClient(c);

                                TLCtxHelper.EventSession.FireClientLoginInfoEvent(c, login);
                                logger.Info("客户端:" + c.Location.ClientID + " 登入状态:" + login.ToString());
                            }
                        }

                        
                    };

                //初始化优先发送缓存对象
                InitPriorityBuffer();

                //启动消息服务
                StartMessageRouter();

                //订阅系统事件
                TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);

                TLCtxHelper.EventAccount.AccountCashOperationEvent += new Action<string, QSEnumCashOperation, decimal>(EventAccount_AccountCashOperationEvent);
                TLCtxHelper.EventAccount.AccountTradingNoticeEvent += new Action<string, string>(EventAccount_AccountTradingNoticeEvent);

                boEngine = new BinaryOptionQuoteEngine(); 
            
            }
            catch (Exception ex)
            {
                logger.Error("初始化服务异常:" + ex.ToString());
                throw (new QSTradingServerInitError(ex));
            }
        }

        /// <summary>
        /// 向交易端发送通知
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void EventAccount_AccountTradingNoticeEvent(string arg1, string arg2)
        {
            
        }

        /// <summary>
        /// 出入金操作用于msgexch通知交易端
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        void EventAccount_AccountCashOperationEvent(string arg1, QSEnumCashOperation arg2, decimal arg3)
        {
            
        }

        void EventSystem_SettleResetEvent(object sender, SystemEventArgs e)
        {
            this.Reset();

            
        }



        protected override void AmendOrderComment(ref Order o)
        {
            base.AmendOrderComment(ref o);

            if (simpromptenable && o.Broker == "SIMBROKER")
            {
                o.Comment = simprompt + ":" + o.Comment;
            }
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
            foreach (var d in ORM.MSettlement.SelectMarketData(TLCtxHelper.ModuleSettleCentre.LastSettleday))
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
                    k.Date = TLCtxHelper.ModuleSettleCentre.LastSettleday;
                    k.Time = 153000;
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
                    logger.Error("Load MarketData Error:" + ex.ToString());
                }
            }
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
                logger.Info("Trading Server Starting success");
            }
            else
                logger.Info("Trading Server Starting failed.");

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
