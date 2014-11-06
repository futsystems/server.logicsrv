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
        public string Title { get { return this.BrokerToken; } }



        /// <summary>
        /// 获得成交接口唯一标识Token通过BrokerConfig中的Token进行标注
        /// </summary>
        /// <returns></returns>
        public string BrokerToken {get;set;}




        protected XServerInfoField _srvinfo;
        /// <summary>
        /// 设置服务器连接信息
        /// </summary>
        /// <param name="info"></param>
        public void SetServerInfo(XServerInfoField info)
        {
            _srvinfo = info;
        }

        protected XUserInfoField _usrinfo;
        /// <summary>
        /// 设置用户登入信息
        /// </summary>
        /// <param name="info"></param>
        public void SetUserInfo(XUserInfoField info)
        {
            _usrinfo = info;
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
                ILogItem item = new LogItem(msg, level, this.Title);
                SendLogItemEvent(item);
            }
        }
        #endregion
    }
}
