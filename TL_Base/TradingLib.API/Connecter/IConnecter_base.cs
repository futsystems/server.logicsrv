using System;
using System.Collections.Generic;
using System.Text;


namespace TradingLib.API
{
    /// <summary>
    /// connecter对外发送委托与取消委托 接口
    /// 实现该接口用于实现发送委托与取消委托功能
    /// </summary>
    public interface ICTrdReq
    {
        /// <summary>
        /// 向Broker发送Order
        /// </summary>
        /// <param name="o"></param>
        void SendOrder(Order o);
        /// <summary>
        /// 向broker取消一个order
        /// </summary>
        /// <param name="oid"></param>
        void CancelOrder(long oid);
        /// <summary>
        /// 用于交易通道中需要有Tick进行驱动的逻辑,比如委托触发等
        /// </summary>
        /// <param name="k"></param>
    }
    /// <summary>
    /// connecter 交易信息回报，成交回报 委托回报 取消回报 委托信息回报等
    /// </summary>
    public interface ICTrdRep
    {
        //事件
        /// <summary>
        /// 当有成交时候回报客户端
        /// </summary>
        event FillDelegate GotFillEvent;
        /// <summary>
        /// 委托正确回报时回报客户端
        /// </summary>
        event OrderDelegate GotOrderEvent;
        /// <summary>
        /// 撤单正确回报时回报客户端
        /// </summary>
        event LongDelegate GotCancelEvent;
        /// <summary>
        /// 某个委托回报相应的委托信息
        /// </summary>
        event OrderMessageDel GotOrderMessageEvent;

    }

    /// <summary>
    /// connecter对外请求行情数据
    /// </summary>
    public interface ICDataReq
    {
        /// <summary>
        /// 订阅一组合约的行情,指定该组合约通过哪个数据通道进行发送
        /// 用于兼容FastTickSrv 在FastTick模式下
        /// 行情服务器对接所有行情通道接口,在接受客户端的行情订阅时需要明确指定某组合约通过哪个通道进行订阅
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="type"></param>
        void RegisterSymbols(string[] symbols,QSEnumDataFeedTypes type = QSEnumDataFeedTypes.DEFAULT);


    }
    /// <summary>
    /// connecter 市场行情回报
    /// </summary>
    public interface ICDataRep
    {
        event TickDelegate GotTickEvent;
    }
}
