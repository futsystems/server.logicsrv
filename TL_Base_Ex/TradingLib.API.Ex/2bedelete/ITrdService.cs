using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    ///// <summary>
    ///// 交易服务器服务内核ITrdService实现服务端数据Rep,Req 交易Rep Req
    ///// </summary>
    //public interface ITrdService : ITLService, ISDataRep, ISDataReq, ISTrdRep, ISTrdReq, IDisposable
    //{
    //    /// <summary>
    //    /// 返回所有客户端连接列表
    //    /// </summary>
    //    IClientInfo[] Clients { get; }

    //    /// <summary>
    //    /// 返回所有客户端所订阅的数据
    //    /// </summary>
    //    SymbolBasket AllClientBasket { get; }

    //    /// <summary>
    //    /// server接收到登入请求事件(对外调用验证函数)
    //    /// </summary>
    //    //event LoginRequestDel newLoginRequest;
    //    /// <summary>
    //    /// 触发某个客户端登入或者注销信息(通知程序某个客户端登入或注销)
    //    /// </summary>
    //    event LoginInfoDel SendLoginInfoEvent;
    //    /// <summary>
    //    /// 接收到客户端功能列表请求
    //    /// </summary>
    //    event MessageArrayDelegate newFeatureRequest;//请求功能特性列表
    //    /// <summary>
    //    /// 接收到客户端的其他未定义请求,需要在具体的TradingServer/中定义
    //    /// </summary>
    //    event UnknownMessageDelegateSession newUnknownRequestSource;//带地址信息处理

    //    /// <summary>
    //    /// 向特定客户端发送委托回报,用于恢复日内交易数据
    //    /// </summary>
    //    /// <param name="o"></param>
    //    void RestoreOrder(Order o, string source);

    //    /// <summary>
    //    /// 向特定客户端发送取消委托回报,用于恢复日内交易数据
    //    /// </summary>
    //    /// <param name="id"></param>
    //    void RestoreCancel(Order o, string source);
    //    /// <summary>
    //    /// 向特定客户端发送成交回报,用于恢复日内交易数据
    //    /// </summary>
    //    /// <param name="o"></param>
    //    void RestoreFill(Trade f, string source);

    //    /// <summary>
    //    /// 向客户端发送持仓状态更新,当有成交发生时,我们就有发送该持仓状态信息
    //    /// </summary>
    //    /// <param name="pos"></param>
    //    //void newPositionUpdate(Position pos);

    //    /// <summary>
    //    /// 向特定客户端转发持仓数据,用于恢复隔夜仓数据
    //    /// </summary>
    //    /// <param name="pos"></param>
    //    void RestorePosition(Position pos, string source);
    
    //}
}
