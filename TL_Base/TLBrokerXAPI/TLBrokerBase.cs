using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI.Interop;
using Common.Logging;



namespace TradingLib.BrokerXAPI
{
    public abstract class TLBrokerBase
    {
        protected ILog logger = null;
        public TLBrokerBase()
        {
            logger = LogManager.GetLogger("TLBrokerBase");
        }

        public IBrokerClearCentre ClearCentre { get; set; }

        /// <summary>
        /// 交易接口交易数据维护器
        /// </summary>
        protected BrokerTracker BrokerTracker { get { return _brokerTracker; } }
        BrokerTracker _brokerTracker = null;
        /// <summary>
        /// 获得成交接口所有委托
        /// </summary>
        public virtual IEnumerable<Order> Orders { get { return _brokerTracker.Orders; } }

        /// <summary>
        /// 获得成交接口所有成交
        /// </summary>
        public virtual IEnumerable<Trade> Trades { get { return _brokerTracker.Trades; } }

        /// <summary>
        /// 获得成交接口所有持仓
        /// </summary>
        public virtual IEnumerable<Position> Positions { get { return _brokerTracker.Positions; } }


        /// <summary>
        /// 执行交易所结算
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="settleday"></param>
        public virtual void SettleExchange(IExchange exchange, int settleday)
        {
            logger.Warn(string.Format("SettleExchange Exch:{0} Settleday:{1}", exchange.EXCode, settleday));
        }

        #region 交易回报事件事件
        /// <summary>
        /// 当数据服务器登入成功后调用
        /// </summary>
        public event IConnecterParamDel Connected;
        protected void NotifyConnected()
        {
            if (Connected != null)
                Connected(this.Token);
        }

        /// <summary>
        /// 当数据服务器断开后触发事件
        /// </summary>
        public event IConnecterParamDel Disconnected;
        protected void NotifyDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this.Token);
        }

        public event Action<RspInfo> GotRspInfoEvent;
        protected void NotifyMessage(XErrorField message)
        {
            RspInfo info = new RspInfoImpl();
            info.ErrorID = message.ErrorID;
            info.ErrorMessage = message.ErrorMsg;
            if (GotRspInfoEvent != null)
                GotRspInfoEvent(info);
        }


        /// <summary>
        /// 当接口有成交数据时 对外触发
        /// </summary>
        public event FillDelegate GotFillEvent;
        protected void NotifyTrade(Trade f)
        {
            logger.Info("Notify Trade:" + f.GetTradeDetail());
            if (GotFillEvent != null)
                GotFillEvent(f);
        }

        /// <summary>
        /// 当接口有委托更新时 对外触发
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        protected void NotifyOrder(Order o)
        {
            logger.Info("Notify Order:" + o.GetOrderInfo());
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }

        public event BOOrderDelegate GotBOOrderEvent;
        protected void NotifyBOOrder(BinaryOptionOrder o)
        {
            logger.Info("Notify BOOrder:" + o.ToString());
            if (GotBOOrderEvent != null)
                GotBOOrderEvent(o);          
        }

        /// <summary>
        /// cancel acknowledgement, order is canceled
        /// </summary>
        public event LongDelegate GotCancelEvent;
        protected void NotifyCancel(long oid)
        {
            logger.Info("Notify Cancel:" + oid.ToString());
            if (GotCancelEvent != null)
                GotCancelEvent(oid);
        }

        /// <summary>
        /// ordermessage acknowledgement
        /// </summary>
        public event OrderErrorDelegate GotOrderErrorEvent;
        protected void NotifyOrderError(Order o, RspInfo info)
        {
            logger.Info("Notify OrderError:" + o.GetOrderInfo());
            if (GotOrderErrorEvent != null)
                GotOrderErrorEvent(o, info);
        }

        public event BOOrderErrorDelegate GotBOOrderErrorEvent;
        protected void NotifyBOOrderError(BinaryOptionOrder o, RspInfo info)
        {
            logger.Info("Notify BOOrderError:" + o.ToString());
            if (GotBOOrderErrorEvent != null)
                GotBOOrderErrorEvent(o, info);
        }

        /// <summary>
        /// 向外通知委托操作错误
        /// </summary>
        public event OrderActionErrorDelegate GotOrderActionErrorEvent;
        protected void NotifyOrderOrderActionError(OrderAction acton, RspInfo info)
        {
            logger.Info(string.Format("Notify OrderActionError: OrderID:{0} ErrorID:{1} ErrorMessage:{2}", acton.OrderID, info.ErrorID, info.ErrorMessage));
            if (GotOrderActionErrorEvent != null)
                GotOrderActionErrorEvent(acton, info);
        }

        #endregion

        #region 配置信息

        /// <summary>
        /// 获得成交接口唯一标识Token通过BrokerConfig中的Token进行标注
        /// </summary>
        /// <returns></returns>
        public string Token { get { return _cfg.Token; } }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string Name { get { return _cfg.Name; } }

        /// <summary>
        /// 获得该通道连接的数据库全局ID,用于实现与Broker进行绑定
        /// </summary>
        public int VendorID { get { return _cfg.vendor_id; } }


        protected ConnectorConfig _cfg;


        /// <summary>
        /// 设定接口参数
        /// </summary>
        /// <param name="cfg"></param>
        public void SetBrokerConfig(ConnectorConfig cfg)
        {
            _cfg = cfg;
            //设定cfg后 重新生成logger用于更新 logger的 name
            logger = LogManager.GetLogger(string.Format("Broker[{0}]", this.Token));
            //初始化BrokerTracker 用于记录接口交易数据
            _brokerTracker = new BrokerTracker(this.Token);
        }

        /// <summary>
        /// 启动时每次从_cfg生成启动所需参数，这样修改参数后重启就可以生效，避免多次调用SetBrokerconfig
        /// cfg是通过引用来传递的，因此可以实时修改
        /// </summary>
        protected void ParseConfigInfo()
        {
            //从配置文件生成对应的服务器连接信息和用户登入信息 用于连接服务器并登入
            _srvinfo = XAPIHelper.GenServerInfo(_cfg);
            _usrinfo = XAPIHelper.GenUserInfo(_cfg);
        }


        protected XServerInfoField _srvinfo;
        protected XUserInfoField _usrinfo;

        #endregion


        #region 成交侧 交易记录通过事件向外暴露 然后异步更新到数据库
        public event OrderDelegate NewBrokerOrderEvent;
        protected void LogBrokerOrder(Order o)
        {
            if (NewBrokerOrderEvent != null)
                NewBrokerOrderEvent(o);
        }
        public event OrderDelegate NewBrokerOrderUpdateEvent;
        protected void LogBrokerOrderUpdate(Order o)
        {
            if (NewBrokerOrderUpdateEvent != null)
                NewBrokerOrderUpdateEvent(o);
            
        }
        public event FillDelegate NewBrokerFillEvent;
        protected void LogBrokerTrade(Trade fill)
        {
            if (NewBrokerFillEvent != null)
                NewBrokerFillEvent(fill);
        }

        public event Action<PositionCloseDetail> NewBrokerPositionCloseDetailEvent;
        protected void LogBrokerPositionClose(PositionCloseDetail detail)
        {
            if (NewBrokerPositionCloseDetailEvent != null)
                NewBrokerPositionCloseDetailEvent(detail);
        }


        #endregion





    }


}
