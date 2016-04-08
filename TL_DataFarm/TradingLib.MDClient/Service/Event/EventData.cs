using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.DataCore
{
    public class EventData
    {

        /// <summary>
        /// 响应实时行情数据
        /// </summary>
        public event Action<Tick> OnRtnTickEvent;


        /// <summary>
        /// 响应Bar数据查询
        /// </summary>
        public event Action<RspQryBarResponseBin> OnRspBarEvent;


        internal void FireRtnTickEvent(Tick k)
        {
            if (OnRtnTickEvent != null)
                OnRtnTickEvent(k);
        }

        internal void FireOnRspBarEvent(RspQryBarResponseBin barResponse)
        {
            if (OnRspBarEvent != null)
                OnRspBarEvent(barResponse);
        }
    }
}
