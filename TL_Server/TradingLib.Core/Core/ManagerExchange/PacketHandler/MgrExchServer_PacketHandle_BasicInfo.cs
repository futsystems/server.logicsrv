﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {

        #region Security Symbol Exchange MarketTime
        void SrvOnMGRQryExchange(MGRQryExchangeRequuest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询交易所列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IExchange[] exchs = BasicTracker.ExchagneTracker.Exchanges;

            int totalnum = exchs.Length;

            for (int i = 0; i < totalnum; i++)
            {
                RspMGRQryExchangeResponse response = ResponseTemplate<RspMGRQryExchangeResponse>.SrvSendRspResponse(request);
                response.Exchange = exchs[i] as Exchange;

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        void SrvOnMGRQryMarketTime(MGRQryMarketTimeRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询交易时间段:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            MarketTime[] mts = BasicTracker.MarketTimeTracker.MarketTimes;
            int totalnum = mts.Length;
            for (int i = 0; i < totalnum; i++)
            {
                RspMGRQryMarketTimeResponse response = ResponseTemplate<RspMGRQryMarketTimeResponse>.SrvSendRspResponse(request);
                response.MarketTime = mts[i];

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        void SrvOnMGRQrySecurity(MGRQrySecurityRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询品种:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            SecurityFamily[] seclist = BasicTracker.SecurityTracker.Securities;
            int totalnum = seclist.Length;
            //debug("security totalnum:" + totalnum, QSEnumDebugLevel.INFO);
            for (int i = 0; i < totalnum; i++)
            {

                RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                response.SecurityFaimly = seclist[i] as SecurityFamilyImpl;
                //debug("sec:" + response.ToString(), QSEnumDebugLevel.INFO);
                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        
        void SrvOnMGRQrySymbol(MGRQrySymbolRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询合约:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            Symbol[] symlis = manager.Domain.GetSymbols().ToArray();
            int totalnum = symlis.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(request);
                    response.Symbol = symlis[i] as SymbolImpl;

                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        void SrvOnMGRUpdateSecurity(MGRUpdateSecurityRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求更新品种:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            SecurityFamilyImpl sec = request.SecurityFaimly;
            BasicTracker.SecurityTracker.UpdateSecurity(sec);
            RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
            response.SecurityFaimly = BasicTracker.SecurityTracker[sec.ID] as SecurityFamilyImpl;
            CacheRspResponse(response);

            if (sec.Tradeable)
            {
                //exchsrv.RegisterSymbol(BasicTracker.SymbolTracker[manager.domain_id,sec.Code].Symbol);
            }
        }



        void SrvOnMGRUpdateSymbol(MGRUpdateSymbolRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求更新合约:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            try
            {
                SymbolImpl sym = request.Symbol;

                manager.Domain.UpdateSymbol(sym);
                RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(request);
                response.Symbol = manager.Domain.GetSymbol(sym.Symbol);
                CacheRspResponse(response);

                if (sym.Tradeable)
                {
                    exchsrv.RegisterSymbol(sym.Symbol);
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }


        void SrvOnMGRReqAddSecurity(MGRReqAddSecurityRequest request, ISession session, Manager manager)
        {
            try
            {
                debug(string.Format("管理员:{0} 请求添加品种:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

                SecurityFamilyImpl sec = request.SecurityFaimly;
                if (BasicTracker.SecurityTracker[sec.Code] == null)
                {
                    BasicTracker.SecurityTracker.UpdateSecurity(sec);

                    RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                    response.SecurityFaimly = BasicTracker.SecurityTracker[sec.Code] as SecurityFamilyImpl;
                    CacheRspResponse(response);
                }
                else
                {

                    RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                    response.RspInfo.Fill("SECURITY_EXIST");
                    CacheRspResponse(response);
                }
                if (sec.Tradeable)
                {
                    //exchsrv.RegisterSymbol(BasicTracker.SecurityTracker[sec.Code]);
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        void SrvOnMGRReqAddSymbol(MGRReqAddSymbolRequest request, ISession session, Manager manager)
        {
            try
            {
                debug(string.Format("管理员:{0} 请求添加合约:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
                
                
                
                SymbolImpl symbol = request.Symbol;
                if (manager.Domain.GetSymbol(symbol.Symbol) == null)
                {
                    symbol.Domain_ID = manager.domain_id;
                    manager.Domain.UpdateSymbol(symbol);

                    RspMGRReqAddSymbolResponse response = ResponseTemplate<RspMGRReqAddSymbolResponse>.SrvSendRspResponse(request);
                    response.Symbol = manager.Domain.GetSymbol(symbol.Symbol);
                    CacheRspResponse(response);
                }
                else
                {
                    RspMGRReqAddSymbolResponse response = ResponseTemplate<RspMGRReqAddSymbolResponse>.SrvSendRspResponse(request);
                    response.RspInfo.Fill("SYMBOL_EXIST");
                    CacheRspResponse(response);
                }
                if (symbol.Tradeable)
                {
                    exchsrv.RegisterSymbol(request.Symbol.Symbol);
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        #endregion


    }
}
