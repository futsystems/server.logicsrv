using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI.Interop;


namespace TradingLib.BrokerXAPI
{
    public abstract class TLBrokerBase
    {
        public TLBrokerBase()
        {
            
        }

        public IBrokerClearCentre ClearCentre { get; set; }



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

        /// <summary>
        /// 当接口有成交数据时 对外触发
        /// </summary>
        public event FillDelegate GotFillEvent;
        protected void NotifyTrade(Trade f)
        {
            if (GotFillEvent != null)
                GotFillEvent(f);
        }

        /// <summary>
        /// 当接口有委托更新时 对外触发
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        protected void NotifyOrder(Order o)
        {
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }

        /// <summary>
        /// cancel acknowledgement, order is canceled
        /// </summary>
        public event LongDelegate GotCancelEvent;
        protected void NotifyCancel(long oid)
        {
            if (GotCancelEvent != null)
                GotCancelEvent(oid);
        }
        /// <summary>
        /// ordermessage acknowledgement
        /// </summary>
        public event OrderErrorDelegate GotOrderErrorEvent;
        protected void NotifyOrderError(Order o, RspInfo info)
        {
            if (GotOrderErrorEvent != null)
                GotOrderErrorEvent(o, info);
        }

        /// <summary>
        /// 向外通知委托操作错误
        /// </summary>
        public event OrderActionErrorDelegate GotOrderActionErrorEvent;
        protected void NotifyOrderOrderActionError(OrderAction acton, RspInfo info)
        {
            if (GotOrderActionErrorEvent != null)
                GotOrderActionErrorEvent(acton, info);
        }

        /// <summary>
        /// 获得当前Tick的市场快照,模拟成交时需要获得当前市场快照用于进行取价操作
        /// </summary>
        public event GetSymbolTickDel GetSymbolTickEvent;
        protected Tick FindTickSnapshot(string symbol)
        {
            if (GetSymbolTickEvent != null)
                return GetSymbolTickEvent(symbol);
            return null;
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


        #region 日志输出部分
        /// <summary>
        /// 对外发送日志事件
        /// </summary>
        public event ILogItemDel SendLogItemEvent;

        bool _debugEnable = true;
        /// <summary>
        /// 是否输出日志
        /// 如果禁用日志 则所有日志将不对外发送
        /// </summary>
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }

        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.INFO;
        /// <summary>
        /// 日志输出级别
        /// </summary>
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// 同时对外输出日志事件,用于被日志模块采集日志或分发
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        //[Conditional("DEBUG")]
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if (_debugEnable && (int)level <= (int)_debuglevel && SendLogItemEvent != null)
            {
                ILogItem item = new LogItem(msg, level, this.Token);
                SendLogItemEvent(item);
            }
        }
        #endregion




    }


}
