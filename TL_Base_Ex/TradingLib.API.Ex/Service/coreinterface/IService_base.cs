using System;
using System.Collections.Generic;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 交易对外请求操作
    /// </summary>
    public interface ISTrdReq
    {
        /// <summary>
        /// 接收到客户端的委托请求
        /// </summary>
        event OrderDelegate newSendOrderRequest;//service接收到委托请求
        /// <summary>
        /// 接收到客户端的取消委托请求
        /// </summary>
        event LongDelegate newOrderCancelRequest;//service接收到取消委托请求
    }
    /// <summary>
    /// 数据对外请求操作
    /// </summary>
    public interface ISDataReq
    {
        event SymbolRegisterDel newRegisterSymbols;//服务器接收到订阅数据请求

    }

    /// <summary>
    /// 交易中IBroker进行的回报事件,盘中交易的实时回报
    /// 客户端请求恢复单日交易数据,必须指明指定的客户端地址,否则会造成其他同名客户端的数据重复
    /// 某个帐号可以同时登入不同的交易终端
    /// </summary>
    public interface ISTrdRep
    {
        /// <summary>
        /// 向客户端发送系统信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clientid"></param>
        void newSysMessage(string msg, string clientid);
        /// <summary>
        /// 向客户端发送委托信息
        /// </summary>
        /// <param name="o"></param>
        void newOrderMessage(Order o, string msg);

        /// <summary>
        /// 向客户端发送委托回报
        /// </summary>
        /// <param name="o"></param>
        void newOrder(Order o);
        /// <summary>
        /// 向客户端发送取消委托回报
        /// </summary>
        /// <param name="id"></param>
        void newCancel(Order o);
        /// <summary>
        /// 向客户端发送成交回报
        /// </summary>
        /// <param name="o"></param>
        void newFill(Trade f);

        /// <summary>
        /// 向客户端发送持仓变动信息
        /// </summary>
        /// <param name="p"></param>
        void newPosition(Position p);


    }
    public interface ISDataRep
    {
        /// <summary>
        /// 向客户端发送一条Tick
        /// </summary>
        /// <param name="k"></param>
        void newTick(Tick k);
    }
}
