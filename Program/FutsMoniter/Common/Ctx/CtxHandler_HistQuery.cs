using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using FutsMoniter;

namespace TradingLib.Common
{
    /// <summary>
    /// 消息处理中继,实现ILogicHandler,用于处理底层回报上来的消息
    /// 界面层订阅这里的事件 实现数据展示
    /// </summary>
    public partial class Ctx
    {
        public event Action<Order, bool> GotHistOrderEvent;
        public event Action<Trade, bool> GotHistTradeEvent;
        public event Action<RspMGRQrySettleResponse> GotSettlementEvent;

        #region 历史记录查询

        public void OnMGROrderResponse(Order o, bool islast)
        {
            if (GotHistOrderEvent != null)
                GotHistOrderEvent(o, islast);
        }

        public void OnMGRTradeResponse(Trade f, bool islast)
        {
            if (GotHistTradeEvent != null)
                GotHistTradeEvent(f, islast);
        }

        public void OnMGRSettlementResponse(RspMGRQrySettleResponse response)
        {
            if (GotSettlementEvent != null)
                GotSettlementEvent(response);
        }

        #endregion


        

    }
}
