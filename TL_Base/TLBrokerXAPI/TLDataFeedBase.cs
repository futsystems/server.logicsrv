using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.BrokerXAPI
{
    public abstract class TLDataFeedBase
    {

        protected ConnectorConfig _cfg;
        protected ILog logger;
        protected string _name;
        public TLDataFeedBase(string name)
        {
            _name = name;
            logger = LogManager.GetLogger(_name);
        }
        /// <summary>
        /// 设定接口参数
        /// </summary>
        /// <param name="cfg"></param>
        public void SetDataFeedConfig(ConnectorConfig cfg)
        {
            _cfg = cfg;
        }

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
        /// 当数据服务得到一个新的tick时调用
        /// </summary>
        public event TickDelegate GotTickEvent;
        protected void NotifyTick(Tick k)
        {
            if (GotTickEvent != null)
                GotTickEvent(k);
        }
    }
}
