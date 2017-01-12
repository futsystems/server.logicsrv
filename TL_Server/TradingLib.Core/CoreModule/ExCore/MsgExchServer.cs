//Copyright 2013 by FutSystems,Inc.
//20161223 删除会话储存与恢复功能 整理日志输出

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
 * 
 * 关于委托Copy的过程
 * 1.客户端Session提交下单操作 此处为第一次Copy1
 * 2.Copy后的委托直接被ClearCentre记录并通过BrokerRouter发送
 * 3.BrokerRouter发送委托过程中不执行委托Copy 委托直接通过Broker发送 Copy1
 * 4.Broerk发送委托后直接修改了Copy1的状态,同时经过BrokerRouter复制后放入队列准备处理Copy2
 * 5.Broker接口获得下单错误 此事获得Copy1进行更新状态并放入队列 这个过程原来没有复制委托
 * 6.在队列中委托状态更新时 先更新下单返回状态Submit此事Copy1被修改了,在队列中的错误reject状态已经被破话 导致无法正常形成逻辑闭环
 * 
 * 总结
 * 委托通知操作 进入队列前对委托进行复制 避免其他过程在发送过程中修改状态
 * 
 * 
 **/
namespace TradingLib.Core
{

    /// <summary>
    /// TradingServer是整体的中转站,他负责底层的tlserver处理将客户端请求进行逻辑处理后分发到对应其他的组件
    /// 并且接受其他组件回报过来的信息并转给客户端
    /// </summary>
    public partial class MsgExchServer :BaseSrvObject, IModuleExCore
    {
        const string CoreName = "MsgExch";
        public string CoreId { get { return this.PROGRAME; } }

        /// <summary>
        /// 连接客户端数目
        /// </summary>
        public int ClientNum { get { return tl.NumClients; } }

        /// <summary>
        /// 登入客户端数目
        /// </summary>
        public int ClientLoggedInNum { get { return tl.NumClientsLoggedIn; } }
        
        TLServer_Exch tl;
        protected ConfigDB _cfgdb;

        bool needConfirmSettlement = true;
        int loginTerminalNum = 6;

        bool simpromptenable = false;
        string simprompt = string.Empty;

        string commentFilled = string.Empty;
        string commentPartFilled = string.Empty;
        string commentCanceled = string.Empty;
        string commentPlaced = string.Empty;
        string commentSubmited = string.Empty;
        string commentOpened = string.Empty;

        IdTracker _orderIDTracker = new IdTracker();
        SeqGenerator _orderSeqGen = null;
        SeqGenerator _tradeSeqGen = null;

        //BinaryOptionQuoteEngine boEngine = null;
        public MsgExchServer()
            : base(MsgExchServer.CoreName)
        {
            try
            {
                //1.加载配置文件
                _cfgdb = new ConfigDB(this.PROGRAME);

                #region 服务端设置
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
                    _cfgdb.UpdateConfig("VerbDebug", QSEnumCfgType.Bool, false.ToString(), "是否输出verb日志");
                }

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
                #endregion

                #region 委托流水号
                if (!_cfgdb.HaveConfig("StartOrderSeq"))
                {
                    _cfgdb.UpdateConfig("StartOrderSeq", QSEnumCfgType.Int, 20000, "默认起始委托流水号");
                }
                int startOrderSeq = _cfgdb["StartOrderSeq"].AsInt();

                if (!_cfgdb.HaveConfig("RandomOrderSeqEnable"))
                {
                    _cfgdb.UpdateConfig("RandomOrderSeqEnable", QSEnumCfgType.Bool, true, "启用委托流水号随机");
                }
                bool enbaleRandomOrderSeq = _cfgdb["RandomOrderSeqEnable"].AsBool();

                if (!_cfgdb.HaveConfig("OrderSeqStepLow"))
                {
                    _cfgdb.UpdateConfig("OrderSeqStepLow", QSEnumCfgType.Int, 50, "委托流水号随机步长低值");
                }
                int stepOrderSeqLow = _cfgdb["OrderSeqStepLow"].AsInt();

                if (!_cfgdb.HaveConfig("OrderSeqStepHigh"))
                {
                    _cfgdb.UpdateConfig("OrderSeqStepHigh", QSEnumCfgType.Int, 100, "委托流水号随机步长高值");
                }
                int stepOrderSeqHigh = _cfgdb["OrderSeqStepHigh"].AsInt();
                int maxorderseq = ORM.MTradingInfo.MaxOrderSeq();

                _orderSeqGen = new SeqGenerator(startOrderSeq, stepOrderSeqLow, stepOrderSeqHigh, enbaleRandomOrderSeq, maxorderseq);
                #endregion

                #region 成交流水号
                if (!_cfgdb.HaveConfig("StartTradeID"))
                {
                    _cfgdb.UpdateConfig("StartTradeID", QSEnumCfgType.Int, 20000, "默认起始成交编号");
                }
                int startTradeID = _cfgdb["StartTradeID"].AsInt();

                if (!_cfgdb.HaveConfig("RandomTradeIDEnable"))
                {
                    _cfgdb.UpdateConfig("RandomTradeIDEnable", QSEnumCfgType.Bool, true, "启用成交编号随机");
                }
                bool enbaleRandomTradeID = _cfgdb["RandomTradeIDEnable"].AsBool();

                if (!_cfgdb.HaveConfig("TradeIDStepLow"))
                {
                    _cfgdb.UpdateConfig("TradeIDStepLow", QSEnumCfgType.Int, 50, "成交编号号随机步长低值");
                }
                int stepTradeIDLow = _cfgdb["TradeIDStepLow"].AsInt();

                if (!_cfgdb.HaveConfig("TradeIDStepHigh"))
                {
                    _cfgdb.UpdateConfig("TradeIDStepHigh", QSEnumCfgType.Int, 150, "成交编号随机步长高值");
                }
                int stepTradeIDHigh = _cfgdb["TradeIDStepHigh"].AsInt();
                int maxtradeid = ORM.MTradingInfo.MaxTradeID();

                _tradeSeqGen = new SeqGenerator(startTradeID, stepTradeIDLow, stepTradeIDHigh, enbaleRandomTradeID, maxtradeid);
                #endregion

                logger.Info(string.Format("Max OrderSeq:{0} TradeID:{1}", _orderSeqGen.CurrentSeq, _tradeSeqGen.CurrentSeq));

                #region 委托状态注释
                if (!_cfgdb.HaveConfig("CommentFilled"))
                {
                    _cfgdb.UpdateConfig("CommentFilled", QSEnumCfgType.String, "全部成交", "全部成交备注");
                }
                commentFilled = _cfgdb["CommentFilled"].AsString();

                if (!_cfgdb.HaveConfig("CommentPartFilled"))
                {
                    _cfgdb.UpdateConfig("CommentPartFilled", QSEnumCfgType.String, "部分成交", "部分成交备注");
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
                #endregion

                //初始化TLServer
                InitTLServer();

                //订阅路由侧事件
                TLCtxHelper.EventRouter.GotTickEvent += new TickDelegate(this.OnTickEvent);
                TLCtxHelper.EventRouter.GotOrderEvent += new OrderDelegate(this.OnOrderEvent);
                TLCtxHelper.EventRouter.GotFillEvent += new FillDelegate(this.OnFillEvent);
                TLCtxHelper.EventRouter.GotCancelEvent += new LongDelegate(this.OnCancelEvent);
                TLCtxHelper.EventRouter.GotOrderErrorEvent += new OrderErrorDelegate(this.OnOrderErrorEvent);
                TLCtxHelper.EventRouter.GotOrderActionErrorEvent += new OrderActionErrorDelegate(this.OnOrderActionErrorEvent);


                //订阅系统事件
                TLCtxHelper.EventSystem.SettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_SettleResetEvent);
                TLCtxHelper.EventAccount.AccountCashOperationEvent += new Action<string, QSEnumCashOperation, decimal>(EventAccount_AccountCashOperationEvent);
                TLCtxHelper.EventAccount.AccountTradingNoticeEvent += new Action<string, string>(EventAccount_AccountTradingNoticeEvent);

                //boEngine = new BinaryOptionQuoteEngine(); 

                //启动消息发送线程
                StartSendWorker();
            
            }
            catch (Exception ex)
            {
                logger.Error("初始化服务异常:" + ex.ToString());
                throw ex;
            }
        }

        void InitTLServer()
        {
            tl = new TLServer_Exch(CoreName, _cfgdb["TLServerIP"].AsString(), _cfgdb["TLPort"].AsInt(), _cfgdb["VerbDebug"].AsBool());
            tl.ProviderName = Providers.QSPlatform;
            tl.NumWorkers = 1;
            tl.ClientLoginTerminalLimit = loginTerminalNum;

            //绑定事件
            //tlserver内部直接发送的消息通过回调将消息缓存到外部缓存中进行队列发送
            tl.CachePacketEvent += new IPacketDelegate(CachePacket);

            //处理其他请求消息
            tl.newPacketRequest += new Action<ISession, IPacket, IAccount>(OnPacketRequest);

            //处理Client会话事件
            tl.ClientRegistedEvent += new Action<TrdClientInfo>(tl_ClientRegistedEvent);
            tl.ClientUnregistedEvent += new Action<TrdClientInfo>(tl_ClientUnregistedEvent);
            tl.ClientLoginEvent += new Action<TrdClientInfo>(tl_ClientLoginEvent);
            tl.ClientLogoutEvent += new Action<TrdClientInfo>(tl_ClientLogoutEvent);
        }

        #region TLServer Client会话事件
        void tl_ClientUnregistedEvent(TrdClientInfo obj)
        {
            TLCtxHelper.EventSession.FireClientDisconnectedEvent(obj);
            logger.Info("客户端:" + obj.Location.ClientID + " 从系统注销");
        }

        void tl_ClientRegistedEvent(TrdClientInfo obj)
        {
            TLCtxHelper.EventSession.FireClientConnectedEvent(obj);
            logger.Info("客户端:" + obj.Location.ClientID + " 注册到系统");
        }

        /* 交易账户登入 登出 过程通过交易账户当前连接终端数量进行该账户是否处于登入状态判断 同时通过账户变化这个事件来触发 向管理端发送通知
         * 
         * 每次登入与登出 均触发FireClientSessionEvent事件
         * 
         * 
         * */
        void tl_ClientLogoutEvent(TrdClientInfo obj)
        {
            if (obj.Account != null)
            {
                //查询交易账户对应会话数量 会话数量大于0则表明该账户处于登入状态
                int num = tl.ClientsForAccount(obj.Account.ID).Count();
                if (num ==0)
                {
                    obj.Account.IsLogin = false;
                    TLCtxHelper.EventAccount.FireAccountChangeEent(obj.Account);
                }
                else
                {
                    obj.Account.IsLogin = true;
                    TLCtxHelper.EventAccount.FireAccountChangeEent(obj.Account);
                }

                TLCtxHelper.EventSession.FireClientSessionEvent(obj, false);
                logger.Info("客户端:" + obj.Location.ClientID + " 登入状态:" + false.ToString());
            }
        }

        void tl_ClientLoginEvent(TrdClientInfo obj)
        {
            if (obj.Account != null)
            {
                obj.Account.IsLogin = true;
                TLCtxHelper.EventAccount.FireAccountChangeEent(obj.Account);

                TLCtxHelper.EventSession.FireClientSessionEvent(obj, true);
                logger.Info("客户端:" + obj.Location.ClientID + " 登入状态:" + true.ToString());
            }
        }
        #endregion


        #region 系统事件处理
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

        #endregion



        public int NextOrderSeq
        {
            get { return _orderSeqGen.NextSeq; }
        }

        public int NextTradeID
        {
            get { return _tradeSeqGen.NextSeq; }
        }


        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
            tl.Dispose();
            tl = null;
            //是否将messagerouter放入stop start的地方
            StopSendWorker();
        }




        Dictionary<string, SettlementPrice> mdmap = new Dictionary<string, SettlementPrice>();
        Dictionary<string, Tick> mdtickmap = new Dictionary<string, Tick>();
        /// <summary>
        /// 从数据库加载上个交易日的市场数据
        /// 飞迅客户端需要查询市场数据来获得隔日持仓的结算价信息
        /// </summary>
        void ReloadMarketData()
        {
            mdtickmap.Clear();
            mdmap.Clear();
            foreach (var d in ORM.MSettlement.SelectSettlementPrice(TLCtxHelper.ModuleSettleCentre.LastSettleday))
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


        #region 开始 停止部分
        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
            bool ret = tl.Start();
            if (ret)
            {
                logger.Info("Trading Server Starting success");
            }
            else
            {
                logger.Error("Trading Server Starting failed.");
            }

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
