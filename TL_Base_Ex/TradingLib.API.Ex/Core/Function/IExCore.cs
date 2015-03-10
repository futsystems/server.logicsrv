using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IExCore
    {
        /// <summary>
        /// 注册合约行情
        /// </summary>
        /// <param name="sym"></param>
        void RegisterSymbol(Symbol sym);


        /// <summary>
        /// 分配委托编号
        /// 在某些情况下 系统内部需要提前知道委托编号，用于扯单或记录 比如风控中心的委托 需要记录该委托编号用于维护该事务状态
        /// </summary>
        /// <param name="o"></param>
        void AssignOrderID(ref Order o);

        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="o"></param>
        void SendOrder(Order o);

        /// <summary>
        /// 发送内部委托
        /// 内部委托 风控中心会忽略某些规则检查
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
