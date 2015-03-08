using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 清算中心/ 接受交易记录，形成交易状态的接口
    /// </summary>
    public interface IGotTradingRecord
    {
        void GotOrder(Order o);
        void GotOrderError(Order o, RspInfo e);
        void GotFill(Trade f);
        void GotTick(Tick k);
        void GotCancel(long oid);
    }
}
