using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant;
//using TradingLib.Quant.Core;

namespace TradingLib.Quant.Base
{
    public interface IStrategy
    {
        // Strategy input 策略输入 外部数据对策略的激发
        /// <summary>
        /// 获得Tick数据
        /// </summary>
        /// <param name="tick"></param>
        void GotTick(Security symbol,Tick tick,Bar bar);
        /// <summary>
        /// 获得新生成的Bar数据
        /// </summary>
        /// <param name="bar"></param>
        void GotBar(Bar bar);
        /// <summary>
        /// 获得委托数据
        /// </summary>
        /// <param name="order"></param>
        void GotOrder(Order order);
        /// <summary>
        /// 获得成交数据
        /// </summary>
        /// <param name="fill"></param>
        void GotFill(Trade fill);

        /// <summary>
        /// 开仓事件
        /// </summary>
        /// <param name="data"></param>
        void GotEntryPosition(Trade fill,PositionDataPair data);

        /// <summary>
        /// 平仓事件
        /// </summary>
        /// <param name="data"></param>
        void GotExitPosiiton(Trade fill,PositionDataPair data);
        /// <summary>
        /// 获得委托取消数据
        /// </summary>
        /// <param name="orderid"></param>
        void GotOrderCancel(long orderid);
        /// <summary>
        /// 获得其他信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="msgid"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        void GotMessage(MessageTypes type, long source, long dest, long msgid, string request, ref string response);
        
        /// <summary>
        /// 停止策略
        /// </summary>
        void Stop();
        /// <summary>
        /// 启动策略
        /// </summary>
        /// <param name="basedata"></param>
        void Start(IStrategyData basedata);



    }
}
