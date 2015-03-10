using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 工具类操作 系统内核暴露出来的相关操作
    /// </summary>
    public interface IUtil
    {
        void ManualInsertOrder(Order o);

        void ManualInsertTrade(Trade t);

        
    }
}
