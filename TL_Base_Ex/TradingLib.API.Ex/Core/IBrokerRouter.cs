using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 成交路由控制器接口
    /// 实现从成交通道获得交易回报，以及向交易通道下单
    /// </summary>
    public interface IBrokerRouter
    {
        event OrderErrorDelegate GotOrderErrorEvent;
        event OrderActionErrorDelegate GotOrderActionErrorEvent;
        event FillDelegate GotFillEvent;
        event OrderDelegate GotOrderEvent;
        event LongDelegate GotCancelEvent;

        /// <summary>
        /// 提交委托
        /// </summary>
        /// <param name="o"></param>
        void SendOrder(Order o);

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="val"></param>
        void CancelOrder(long val);

        /// <summary>
        /// 查找路右侧分解的委托
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        Order SentRouterOrder(long val);

        /// <summary>
        /// 重置
        /// </summary>
        void Reset();

        /// <summary>
        /// 启动
        /// </summary>
        void Start();


        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        void LoadBroker(IBroker broker);
    }
}
