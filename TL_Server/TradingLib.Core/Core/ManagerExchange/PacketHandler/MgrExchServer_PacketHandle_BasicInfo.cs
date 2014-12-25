using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

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
            SecurityFamilyImpl[] seclist = manager.Domain.GetSecurityFamilies().ToArray();
            int totalnum = seclist.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                    response.SecurityFaimly = seclist[i];
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
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
            try
            {
                debug(string.Format("管理员:{0} 请求更新品种:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

                if (!manager.RightRootDomain())
                {
                    throw new FutsRspError("无权更新品种数据");
                }

                SecurityFamilyImpl sec = request.SecurityFaimly;

                manager.Domain.UpdateSecurity(sec);

                RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                response.SecurityFaimly = manager.Domain.GetSecurityFamily(sec.ID);

                CacheRspResponse(response);

                if (sec.Tradeable)
                {
                    //exchsrv.RegisterSymbol(BasicTracker.SymbolTracker[manager.domain_id,sec.Code]);
                }
                session.OperationSuccess("品种数据更新成功");
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }



        void SrvOnMGRUpdateSymbol(MGRUpdateSymbolRequest request, ISession session, Manager manager)
        {
            try
            {
                debug(string.Format("管理员:{0} 请求更新合约:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
           
                if (!manager.RightRootDomain())
                {
                    throw new FutsRspError("无权更新合约数据");
                }

                SymbolImpl sym = request.Symbol;

                manager.Domain.UpdateSymbol(sym);

                RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(request);
                response.Symbol = manager.Domain.GetSymbol(sym.ID);
                CacheRspResponse(response);

                if (sym.Tradeable)
                {
                    exchsrv.RegisterSymbol(sym);
                }
                session.OperationSuccess("合约数据更新成功");
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

                if (!manager.RightRootDomain())
                {
                    throw new FutsRspError("无权添加品种数据");
                }

                SecurityFamilyImpl sec = request.SecurityFaimly;
                if (manager.Domain.GetSecurityFamily(sec.Code) == null)
                {
                    manager.Domain.UpdateSecurity(sec);

                    RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
                    response.SecurityFaimly = manager.Domain.GetSecurityFamily(sec.Code);
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

                if (!manager.RightRootDomain())
                {
                    throw new FutsRspError("无权添加合约数据");
                }

                SymbolImpl symbol = request.Symbol;
                if (manager.Domain.GetSymbol(symbol.Symbol) == null)
                {
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
                    exchsrv.RegisterSymbol(manager.Domain.GetSymbol(symbol.Symbol));
                }
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

        #endregion

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "SyncSecInfo", "SyncSecInfo - sync  sec", "同步品种数据")]
        public void CTE_SyncSecInfo(ISession session)
        {
            try
            {
                Manager manager = session.GetManager();
                debug(string.Format("管理员{0} 同步品种数据",manager.Login), QSEnumDebugLevel.INFO);

                if (!manager.RightRootDomain())
                {
                    throw new FutsRspError("无权同步品种数据");
                }
                //
                if (manager.Domain.Super)
                {
                    throw new FutsRspError("超级域不支持同步,请手工维护数据");
                }

                Domain domain = BasicTracker.DomainTracker.SuperDomain;
                if (domain != null)
                {
                    foreach (SecurityFamilyImpl sec in domain.GetSecurityFamilies())
                    {
                        manager.Domain.SyncSecurity(sec);
                    }
                }
                else
                {
                    throw new FutsRspError("没有可以同步的主域");
                }

                session.OperationSuccess("同步品种数据完成");

                SecurityFamilyImpl[] seclist = manager.Domain.GetSecurityFamilies().ToArray();
                int totalnum = seclist.Length;
                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(session);
                        response.SecurityFaimly = seclist[i];
                        CacheRspResponse(response, i == totalnum - 1);
                    }
                }
                else
                {
                    RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(session);
                    CacheRspResponse(response);
                }

            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "SyncSymbol", "SyncSymbol - sync  symbol", "同步合约数据")]
        public void CTE_SyncSymbol(ISession session)
        {
            try
            {
                Manager manager = session.GetManager();
                debug(string.Format("管理员{0} 同步合约数据", manager.Login), QSEnumDebugLevel.INFO);

                if (!manager.RightRootDomain())
                {
                    throw new FutsRspError("无权同步合约数据");
                }
                Vendor vendor = BasicTracker.VendorTracker[manager.Domain.CFG_SyncVendor_ID];
                if (vendor != null && vendor.Domain.ID == manager.domain_id && vendor.Broker != null && vendor.Broker.IsLive)
                {
                    //如果设置了同步帐号 则从帐号中进行同步数据
                    if(vendor.Broker is TLBroker)
                    {
                        TLBroker broker = vendor.Broker as TLBroker;

                        broker.GotSymbolEvent += (sym, islast) =>
                        {
                            SymbolImpl tsym = manager.Domain.GetSymbol(sym.Symbol);
                            //更新
                            if (tsym != null)
                            {
                                tsym.EntryCommission = (decimal)sym.EntryCommission;
                                tsym.ExitCommission = (decimal)sym.ExitCommission;
                                tsym.ExpireDate = sym.ExpireDate;//到期日
                                BasicTracker.SymbolTracker.UpdateSymbol(manager.domain_id, tsym);
                            }
                            else//增加
                            {
                                SecurityFamilyImpl sec = BasicTracker.SecurityTracker[manager.domain_id, sym.SecurityCode];
                                //包含对应品种，则增加合约
                                if (sec != null)
                                {
                                    tsym = new SymbolImpl();
                                    tsym.Symbol = sym.Symbol;
                                    tsym.Domain_ID = manager.domain_id;
                                    tsym.Margin = (decimal)sym.Margin;
                                    tsym.EntryCommission = (decimal)sym.EntryCommission;
                                    tsym.ExitCommission = (decimal)sym.ExitCommission;
                                    tsym.ExpireDate = sym.ExpireDate;//到期日

                                    tsym.Strike = (decimal)sym.StrikePrice;
                                    tsym.OptionSide = sym.OptionSide;

                                    tsym.security_fk = sec.ID;
                                    tsym.Tradeable = true;
                                    BasicTracker.SymbolTracker.UpdateSymbol(manager.domain_id, tsym);
                                }
                                else
                                {
                                    debug("symbol:" + sym.Symbol + " have no sec setted. code:" + sym.SecurityCode, QSEnumDebugLevel.INFO);
                                }
                                
                            }

                        };
                        if (!broker.QryInstrument())
                        {
                            throw new FutsRspError("通道合约数据查询失败");
                        }
                        return;
                    
                    }
                    else
                    {
                        throw new FutsRspError("设置的同步实盘帐户无效,请手工维护数据");
                    }

                }
                else
                {
                    if (manager.Domain.Super)
                    {
                        throw new FutsRspError("超级域不支持同步,请手工维护数据");
                    }
                }

                Domain domain = BasicTracker.DomainTracker.SuperDomain;
                if (domain != null)
                {
                    foreach (SymbolImpl sym in domain.GetSymbols())
                    {
                        manager.Domain.SyncSymbol(sym);
                    }
                }
                else
                {
                    throw new FutsRspError("没有可以同步的主域");
                }

                session.OperationSuccess("同步合约数据完成");
                Symbol[] symlis = manager.Domain.GetSymbols().ToArray();
                int totalnum = symlis.Length;
                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(session);
                        response.Symbol = symlis[i] as SymbolImpl;

                        CacheRspResponse(response, i == totalnum - 1);
                    }
                }
                else
                {
                    RspMGRQrySymbolResponse response = ResponseTemplate<RspMGRQrySymbolResponse>.SrvSendRspResponse(session);
                    CacheRspResponse(response);
                }

            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }

    }
}
