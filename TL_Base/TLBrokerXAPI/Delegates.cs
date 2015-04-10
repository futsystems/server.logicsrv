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
    /// 查询持仓明细回调函数
    /// </summary>
    /// <param name="pPosition"></param>
    /// <param name="islast"></param>
    public delegate void CBOnQryPositionDetail(ref XPositionDetail pPosition,bool islast);

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

    /// <summary>
    /// 接口侧日志输出
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="len"></param>
    public delegate void CBOnLog(IntPtr handler,int len);


    /// <summary>
    /// 接口侧相关操作回报
    /// 1.操作错误通知
    /// 2.操作成功通知
    /// </summary>
    /// <param name="pError"></param>
    /// <param name="islast"></param>
    public delegate void CBOnMessage(ref XErrorField pMessage,bool islast);

}
