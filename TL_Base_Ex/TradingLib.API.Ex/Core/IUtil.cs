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
        /// <summary>
        /// 获得某个合约当前可用价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetAvabilePrice(string symbol);

        /// <summary>
        /// 获得某个合约的市场快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick  GetTickSnapshot(string symbol);

        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="o"></param>
        void SendOrder(Order o);

        /// <summary>
        /// 发送内部委托 比如风控中心，管理端
        /// 内部发送委托与普通发送委托的区别在于风控项目的不同
        /// </summary>
        /// <param name="o"></param>
        void SendOrderInternal(Order o);
       

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="oid"></param>
        void CancelOrder(long oid);
    }
}
