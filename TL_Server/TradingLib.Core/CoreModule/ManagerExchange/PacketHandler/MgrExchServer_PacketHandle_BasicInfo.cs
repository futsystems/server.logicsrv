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
            logger.Info(string.Format("管理员:{0} 请求查询交易所列表:{1}", session.AuthorizedID, request.ToString()));
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
            logger.Info(string.Format("管理员:{0} 请求查询交易时间段:{1}", session.AuthorizedID, request.ToString()));
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
            logger.Info(string.Format("管理员:{0} 请求查询品种:{1}", session.AuthorizedID, request.ToString()));
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
            logger.Info(string.Format("管理员:{0} 请求查询合约:{1}", session.AuthorizedID, request.ToString()));
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
                logger.Info(string.Format("管理员:{0} 请求更新品种:{1}", session.AuthorizedID, request.ToString()));

                if (!manager.IsInRoot())
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
                logger.Info(string.Format("管理员:{0} 请求更新合约:{1}", session.AuthorizedID, request.ToString()));

                if (!manager.IsInRoot())
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
                    TLCtxHelper.ModuleExCore.RegisterSymbol(sym);
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
                logger.Info(string.Format("管理员:{0} 请求添加品种:{1}", session.AuthorizedID, request.ToString()));

                if (!manager.IsInRoot())
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
                logger.Info(string.Format("管理员:{0} 请求添加合约:{1}", session.AuthorizedID, request.ToString()));

                if (!manager.IsInRoot())
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
                    TLCtxHelper.ModuleExCore.RegisterSymbol(manager.Domain.GetSymbol(symbol.Symbol));
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
                logger.Info(string.Format("管理员{0} 同步品种数据", manager.Login));

                if (!manager.IsInRoot())
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

        /// <summary>
        /// 禁止所有合约
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DisableAllSymbols", "DisableAllSymbols - sync  symbol", "禁止所有合约")]
        public void CTE_DisableAllSymbolsSymbol(ISession session)
        {
            Manager manager = session.GetManager();
            logger.Info(string.Format("管理员{0} 禁止所有合约交易", manager.Login));

            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权禁止合约");
            }

            foreach (SymbolImpl sym in manager.Domain.GetSymbols())
            {
                sym.Tradeable = false;
                //更新合约
                BasicTracker.SymbolTracker.UpdateSymbol(manager.domain_id, sym);
            }
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


        /// <summary>
        /// 同步合约数据
        /// 1.从设置的实盘帐户同步合约，通过TLBroker 查询合约 将接口内预先查询好的合约数据同步出来
        /// 2.从主域同步合约，分区柜台从主域进行同步
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "SyncSymbol", "SyncSymbol - sync  symbol", "同步合约数据")]
        public void CTE_SyncSymbol(ISession session)
        {
            Manager manager = session.GetManager();
            logger.Info(string.Format("管理员{0} 同步合约数据", manager.Login));

            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权同步合约数据");
            }

            //通过实盘帐户同步合约数据
            if (manager.Domain.CFG_SyncVendor_ID !=0)
            {
                Vendor vendor = BasicTracker.VendorTracker[manager.Domain.CFG_SyncVendor_ID];
                if (vendor == null || vendor.Domain.ID != manager.domain_id || vendor.Broker == null || !vendor.Broker.IsLive || !(vendor.Broker is TLBroker))
                { 
                    throw new FutsRspError("设置的同步实盘帐户无效,请修改同步实盘帐户");
                }

                TLBroker broker = vendor.Broker as TLBroker;
                Action<XSymbol,bool> Handler = (sym, islast) =>
                {
                    if (islast)
                    {
                        logger.Info("got symbol synced....");
                    }
                    SymbolImpl tsym = manager.Domain.GetSymbol(sym.Symbol);
                    //更新
                    if (tsym != null)
                    {
                        tsym.EntryCommission = (decimal)sym.EntryCommission;
                        tsym.ExitCommission = (decimal)sym.ExitCommission;
                        tsym.Margin = (decimal)sym.Margin;
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
                            tsym.Tradeable = false;
                            BasicTracker.SymbolTracker.UpdateSymbol(manager.domain_id, tsym);
                        }
                        else
                        {
                            logger.Info("symbol:" + sym.Symbol + " have no sec setted. code:" + sym.SecurityCode);
                        }
                    }
                };

                broker.GotSymbolEvent += new Action<XSymbol, bool>(Handler);

                if (!broker.QryInstrument())
                {
                    broker.GotSymbolEvent -= new Action<XSymbol, bool>(Handler);
                    throw new FutsRspError("通道合约数据查询失败");
                }
                broker.GotSymbolEvent -= new Action<XSymbol, bool>(Handler);
                session.OperationSuccess("同步合约数据完成");

            }//通过主域同步合约数据
            else
            {
                if (manager.Domain.Super)
                {
                    throw new FutsRspError("超级域不支持主域同步,请手工维护数据");
                }

                //通过主域进行同步合约
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
                session.OperationSuccess("主域同步合约数据完成");
            }

            //将所有合约回报给客户端
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
    }
}
