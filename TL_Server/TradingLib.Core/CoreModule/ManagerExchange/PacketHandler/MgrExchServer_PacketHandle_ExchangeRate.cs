//Copyright 2013 by FutSystems,Inc.
//20170112 整理无用操作

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 汇率
    /// </summary>
    public partial class MgrExchServer
    {




        ///// <summary>
        ///// 查询汇率信息
        ///// </summary>
        ///// <param name="request"></param>
        ///// <param name="session"></param>
        ///// <param name="manager"></param>
        //void SrvOnQryExchagneRate(MGRQryExchangeRateRequuest request, ISession session, Manager manager)
        //{
        //    logger.Info(string.Format("Manager[{0}] QryExchangeRate", session.AuthorizedID));
        //    IEnumerable<ExchangeRate> ratelist = manager.Domain.GetExchangeRates(TLCtxHelper.ModuleSettleCentre.Tradingday);

        //    for (int i = 0; i < ratelist.Count(); i++)
        //    {
        //        RspMGRQryExchangeRateResponse response = ResponseTemplate<RspMGRQryExchangeRateResponse>.SrvSendRspResponse(request);
        //        response.ExchangeRate = ratelist.ElementAt(i);

        //        CacheRspResponse(response, i == ratelist.Count() - 1);
        //    }
        //}

    }
}
