using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.MDClient
{
    /// <summary>
    /// 行情回调对象
    /// 集成该类覆写对应的回调函数
    /// 1.主动推送 OnRtnXXX 
    /// 2.操作回报/响应 OnRspXXXX OnRspQryBar OnRspQryExchange等
    /// </summary>
    public class MDHandlerBase
    {
        ILog logger = LogManager.GetLogger("MDSpi");
        /// <summary>
        /// 响应实时行情数据
        /// </summary>
        /// <param name="k"></param>
        public virtual void OnRtnTick(Tick k)
        {
            logger.Info("Tick:" + k != null ? k.ToString() : "Null");
        }
        
        /// <summary>
        /// 响应历史Bar数据查询
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="rsp"></param>
        /// <param name="requestID"></param>
        /// <param name="isLast"></param>
        public virtual void OnRspQryBar(Bar bar, RspInfo rsp, int requestID, bool isLast)
        {
            logger.Info("Bar:" + bar != null ? bar.ToString() : "Null");
        }


    }
}
