using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradeLink.API;

namespace demoBroker
{
    public class demobroker:IBroker
    {
        public string Title
        {
            get { return "空白Broker模板"; }
        }
        //该Broker支持的交易所
        IExchange[] _exlist = null;
        public IExchange[] ExchangeSupported { get { return _exlist; } }
        //该broker支持的证券类型
        SecurityType[] _sectypelist = null;
        public SecurityType[] SecuritySupported { get { return _sectypelist; } }

        public event DebugDelegate SendDebugEvent;
        /// <summary>
        /// fill acknowledged, order filled.
        /// </summary>
        public event FillDelegate GotFillEvent;
        /// <summary>
        /// order acknowledgement, order placed.
        /// </summary>
        public event OrderDelegate GotOrderEvent;
        /// <summary>
        /// cancel acknowledgement, order is canceled
        /// </summary>
        public event LongDelegate GotCancelEvent;
        public event OrderMessageDel GotOrderMessageEvent;

        public void Start()
        { }

        public void Stop()
        { }

        public void SendOrder(Order o)
        { }
        public void CancelOrder(long oid)
        { }
        public bool IsLive
        {
            get

            {
                return true;
            }
        }

    }
}
