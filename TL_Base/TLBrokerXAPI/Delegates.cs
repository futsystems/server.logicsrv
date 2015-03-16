using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.BrokerXAPI
{

    /// <summary>
    /// 连接建立回调委托
    /// </summary>
    public delegate void CBOnConnected();

    /// <summary>
    /// 连接断开回调委托
    /// </summary>
    public delegate void CBOnDisconnected();

    /// <summary>
    /// 登入回调委托
    /// </summary>
    /// <param name="pRspUserLogin"></param>
    public delegate void CBOnLogin(ref XRspUserLoginField pRspUserLogin);

    /// <summary>
    /// 成交回调委托
    /// </summary>
    /// <param name="pTrade"></param>
    public delegate void CBRtnTrade(ref XTradeField pTrade);

    /// <summary>
    /// 查询成交回调委托
    /// </summary>
    /// <param name="pTrade"></param>
    public delegate void CBOnQryTrade(ref XTradeField pTrade,bool islast);

    /// <summary>
    /// 委托回调委托
    /// </summary>
    /// <param name="pOrder"></param>
    public delegate void CBRtnOrder(ref XOrderField pOrder);

    /// <summary>
    /// 查询委托回调委托
    /// </summary>
    /// <param name="pOrder"></param>
    public delegate void CBOnQryOrder(ref XOrderField pOrder,bool islast);

    /// <summary>
    /// 委托错误回调委托
    /// </summary>
    /// <param name="pOrder"></param>
    /// <param name="pError"></param>
    public delegate void CBRtnOrderError(ref XOrderField pOrder, ref XErrorField pError);


    /// <summary>
    /// 委托操作错误回调委托
    /// </summary>
    /// <param name="pOrderAction"></param>
    /// <param name="pError"></param>
    public delegate void CBRtnOrderActionError(ref XOrderActionField pOrderAction,ref XErrorField pError);

    /// <summary>
    /// 合约查询回调委托
    /// </summary>
    /// <param name="pSymbolField"></param>
    /// <param name="islast"></param>
    public delegate void CBOnSymbol(ref XSymbol pSymbolField,bool islast);

    /// <summary>
    /// 交易帐户财务信息查询回调委托
    /// </summary>
    /// <param name="?"></param>
    /// <param name="islast"></param>
    public delegate void CBOnAccountInfo(ref XAccountInfo pAccountInfo,bool islast);
}
