using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 自定义客户端进行的基础信息查询
    /// 这里的信息查询有别与CTP接口所提供的接口类型
    /// </summary>
    public partial class MsgExchServer
    {
        /// <summary>
        /// 响应交易时间段查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryMarketTime(XQryMarketTimeRequest request, IAccount account)
        {
            logger.Info("XMarketTime:" + request.ToString());
            MarketTime[] mts = BasicTracker.MarketTimeTracker.MarketTimes;
            int totalnum = mts.Length;
            for (int i = 0; i < totalnum; i++)
            {
                RspXQryMarketTimeResponse response = ResponseTemplate<RspXQryMarketTimeResponse>.SrvSendRspResponse(request);
                response.MarketTime = mts[i];

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        /// <summary>
        /// 响应交易所查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryExchange(XQryExchangeRequuest request,IAccount account)
        {
            logger.Info("XQryExchange:"+request.ToString());
            IExchange[] exchs = BasicTracker.ExchagneTracker.Exchanges;

            int totalnum = exchs.Length;

            for (int i = 0; i < totalnum; i++)
            {
                RspXQryExchangeResponse response = ResponseTemplate<RspXQryExchangeResponse>.SrvSendRspResponse(request);
                response.Exchange = exchs[i] as Exchange;

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        

        /// <summary>
        /// 响应品种查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQrySecurity(XQrySecurityRequest request,IAccount account)
        {
            logger.Info("XQrySecurity:" + request.ToString());

            SecurityFamilyImpl[] seclist = account.Domain.GetSecurityFamilies().ToArray();
            int totalnum = seclist.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQrySecurityResponse response = ResponseTemplate<RspXQrySecurityResponse>.SrvSendRspResponse(request);
                    response.SecurityFaimly = seclist[i];
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspXQrySecurityResponse response = ResponseTemplate<RspXQrySecurityResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 响应合约查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        void SrvOnXQrySymbol(XQrySymbolRequest request, IAccount account)
        {
            logger.Info("XQrySymbol:" + request.ToString());

            IEnumerable<Symbol> tmplsit = null;
            if (string.IsNullOrEmpty(request.Symbol))
            {
                TrdClientInfo client = tl.GetClient(request.ClientID);
                //股票交易终端只返回必要的合约数据 其余合约数据延迟加载
                if (client != null && client.ProductInfo.Equals(ProductInfo.T_XTRADER_STOCK))
                {
                    List<Symbol> list = new List<Symbol>();
                    foreach (var pos in account.Positions)
                    { 
                        if(pos.oSymbol == null) continue;
                        if (!list.Contains(pos.oSymbol))
                        {
                            list.Add(pos.oSymbol);
                        }
                    }
                    foreach (var order in account.Orders)
                    {
                        if (order.oSymbol == null) continue;
                        if (!list.Contains(order.oSymbol))
                        {
                            list.Add(order.oSymbol);
                        }
                    }
                    tmplsit = list;
                }
                else
                {
                    tmplsit = account.GetSymbols().Where(sym => sym.IsTradeable);
                }
            }
            else
            {
                Symbol sym = account.Domain.GetSymbol(request.Symbol);
                List<Symbol> list= new List<Symbol>();
                if(sym!= null)
                {
                    list.Add(sym);
                }
                tmplsit = list;
            }

            int totalnum = tmplsit.Count();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQrySymbolResponse response = ResponseTemplate<RspXQrySymbolResponse>.SrvSendRspResponse(request);
                    response.Symbol = tmplsit.ElementAt(i) as SymbolImpl;

                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspXQrySymbolResponse response = ResponseTemplate<RspXQrySymbolResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 从行情数据中心获得行情快照,
        /// 如果没有对应的快照需要获得最近的结算价数据信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryTickSnapShot(XQryTickSnapShotRequest request, IAccount account)
        {
            logger.Info("XQryTickSnapShot:" + request.ToString());
            IEnumerable<Symbol> symlist= null;
            if (string.IsNullOrEmpty(request.Symbol))
            {
                symlist = account.GetSymbols().Where(sym => sym.IsTradeable);//未指定合约 则所有可交易合约
            }
            else
            {
                symlist = account.GetSymbols().Where(sym => sym.IsTradeable).Where(sym => sym.Symbol == request.Symbol);//获得某个合约
            }

            int totalnum = symlist.Count();
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    Symbol sym = symlist.ElementAt(i);
                    Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(sym.Symbol);
                    if (k == null || !k.IsValid())
                    {
                        k = TLCtxHelper.ModuleSettleCentre.GetLastTickSnapshot(sym.Symbol);
                    }
                    if (sym.SecurityType == SecurityType.STK)
                    {
                        k.Type = EnumTickType.STKSNAPSHOT;
                    }
                    RspXQryTickSnapShotResponse response = ResponseTemplate<RspXQryTickSnapShotResponse>.SrvSendRspResponse(request);
                    response.Tick = k;
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspXQryTickSnapShotResponse response = ResponseTemplate<RspXQryTickSnapShotResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 响应查询账户请求
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryAccount(XQryAccountRequest request, IAccount account)
        {
            logger.Info("XQryAccountRequest:" + request.ToString());
            RspXQryAccountResponse respone = ResponseTemplate<RspXQryAccountResponse>.SrvSendRspResponse(request);
            respone.Account = account.GenAccountLite();
            CacheRspResponse(respone);
        }

    }
}
