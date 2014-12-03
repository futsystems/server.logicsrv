using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.BrokerXAPI
{
    public abstract class TLDataFeedBase
    {

        protected ConnectorConfig _cfg;

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
