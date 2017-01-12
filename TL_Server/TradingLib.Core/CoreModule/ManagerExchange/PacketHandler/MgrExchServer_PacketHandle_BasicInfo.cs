//Copyright 2013 by FutSystems,Inc.
//20170112 整理无用操作

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

        //查询日历列表
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryCalendarList", "QryCalendarList - qry calendar list", "查询日历对象列表")]
        public void CTE_QryCalendarList(ISession session)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                CalendarItem[] items = BasicTracker.CalendarTracker.Calendars.Select(c => new CalendarItem() { Code = c.Code, Name = c.Name }).ToArray();
                int totalnum = items.Length;

                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        session.ReplyMgr(items[i], i == totalnum - 1);
                    }
                }
                else
                {
                    session.ReplyMgr(null);
                }
            }
        }

        void SrvOnMGRQryExchange(MGRQryExchangeRequuest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("Manager[{0}] QryExchange", session.AuthorizedID));
            Exchange[] exchs = BasicTracker.ExchagneTracker.Exchanges;
            int totalnum = exchs.Length;

            for (int i = 0; i < totalnum; i++)
            {
                RspMGRQryExchangeResponse response = ResponseTemplate<RspMGRQryExchangeResponse>.SrvSendRspResponse(request);
                response.Exchange = exchs[i] as ExchangeImpl;

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        void SrvOnMGRUpdateExchange(MGRUpdateExchangeRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("Manager[{0}] UpdateExchange:{1}", session.AuthorizedID, request.ToString()));
            if (manager.IsRoot())
            {
                if (request.Exchange != null)
                {
                    BasicTracker.ExchagneTracker.UpdateExchange(request.Exchange);

                    RspMGRUpdateExchangeResponse response = ResponseTemplate<RspMGRUpdateExchangeResponse>.SrvSendRspResponse(request);
                    response.Exchange = BasicTracker.ExchagneTracker[request.Exchange.ID];
                    CacheRspResponse(response);
                }
            }
        }

        void SrvOnMGRQryMarketTime(MGRQryMarketTimeRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("Manager[{0}] QryMarketTime", session.AuthorizedID));
            MarketTimeImpl[] mts = BasicTracker.MarketTimeTracker.MarketTimes;
            int totalnum = mts.Length;
            for (int i = 0; i < totalnum; i++)
            {
                RspMGRQryMarketTimeResponse response = ResponseTemplate<RspMGRQryMarketTimeResponse>.SrvSendRspResponse(request);
                response.MarketTime = mts[i];

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        void SrvOnMGRUpdateMarketTime(MGRUpdateMarketTimeRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("Manager[{0}] UpdateMarketTime:{1}", session.AuthorizedID, request.ToString()));
            Manager manger = session.GetManager();
            if (manager.IsRoot())
            { 
                if(request.MarketTime != null)
                {
                    BasicTracker.MarketTimeTracker.UpdateMarketTime(request.MarketTime);
                    RspMGRUpdateMarketTimeResponse response = ResponseTemplate<RspMGRUpdateMarketTimeResponse>.SrvSendRspResponse(request);
                    response.MarketTime = BasicTracker.MarketTimeTracker[request.MarketTime.ID];

                    CacheRspResponse(response);
                }

                
            }
        
        }
        void SrvOnMGRQrySecurity(MGRQrySecurityRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("Manager[{0}] QrySecurity", session.AuthorizedID));
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
            logger.Info(string.Format("Manager[{0}] QrySymbol", session.AuthorizedID));
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
            logger.Info(string.Format("Manager[{0}] UpdateSecurity:{1}", session.AuthorizedID, request.ToString()));

            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权更新品种数据");
            }

            SecurityFamilyImpl sec = request.SecurityFaimly;
            //如果已经存在该品种则不执行添加操作
            if (sec.ID==0 && manager.Domain.GetSecurityFamily(sec.Code) != null)
            {
                throw new FutsRspError("已经存在品种:" + sec.Code);
            }
            //通过对应的域更新品种对象
            manager.Domain.UpdateSecurity(sec);
            int secidupdate = sec.ID;

            //如果是超级域 则同步更新所有分区
            if (manager.Domain.Super)
            {
                //更新所有域的品种
                foreach (var d in BasicTracker.DomainTracker.Domains)
                {
                    if (d.ID == manager.Domain.ID) //超级域 跳过 已经更新过品种数据
                        continue;
                    //通过初始品种数据的Code找到对应的对象，然后将ID传入 调用更新，否则更新时 ID不对应导致重复增加
                    SecurityFamilyImpl t = d.GetSecurityFamily(sec.Code);//通过品种找到某个域的品种 
                    if (t == null) //品种不存在则为添加
                    {
                        sec.ID = 0;
                    }
                    else //品种存在则更新
                    {
                        sec.ID = t.ID;
                    }
                    d.UpdateSecurity(sec, false);
                }
            }

            //需要通过第一次更新获得sec_id来获得对象进行回报 否则在更新其他域的品种对象时id会发生同步变化
            RspMGRUpdateSecurityResponse response = ResponseTemplate<RspMGRUpdateSecurityResponse>.SrvSendRspResponse(request);
            response.SecurityFaimly = manager.Domain.GetSecurityFamily(secidupdate);
            CacheRspResponse(response);


            if (sec.Tradeable)
            {
                //exchsrv.RegisterSymbol(BasicTracker.SymbolTracker[manager.domain_id,sec.Code]);
            }
            session.OperationSuccess("品种数据更新成功");
        }



        void SrvOnMGRUpdateSymbol(MGRUpdateSymbolRequest request, ISession session, Manager manager)
        {
          
            logger.Info(string.Format("Manager[{0}] UpdaetSymbol:{1}", session.AuthorizedID, request.ToString()));

            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权更新合约数据");
            }

            SymbolImpl symbol = request.Symbol;
            //设定合约symbol为当前管理员域ID 避免管理端没有正常传输分区ID
            symbol.Domain_ID = manager.Domain.ID;

            //检查品种是否完毕
            SecurityFamilyImpl rawsec = BasicTracker.SecurityTracker[symbol.Domain_ID, symbol.security_fk];
            if (rawsec == null)
            {
                throw new FutsRspError("品种数据异常");
            }
            symbol.SecurityFamily = rawsec;

            //如果是添加合约 检查合约是否存在
            if (symbol.ID==0 && manager.Domain.GetSymbol(symbol.Exchange,symbol.Symbol) != null)
            {
                throw new FutsRspError("已经存在合约:" + symbol.Symbol);
            }

            //调用该域更新该合约
            manager.Domain.UpdateSymbol(symbol);

            //如果是超级域 则同步更新所有分区
            if (manager.Domain.Super)
            {
                //更新所有域的合约
                foreach (var d in BasicTracker.DomainTracker.Domains)
                {
                    if (d.ID == manager.Domain.ID) //超级域 跳过 已经更新过品种数据
                        continue;
                    d.UpdateSymbolViaSuper(symbol);
                }
            }


            RspMGRUpdateSymbolResponse response = ResponseTemplate<RspMGRUpdateSymbolResponse>.SrvSendRspResponse(request);
            SymbolImpl localsymbol = manager.Domain.GetSymbol(symbol.ID);
            response.Symbol = localsymbol;
            CacheRspResponse(response);

            if (localsymbol.Tradeable)
            {
                //SymbolBasket b = new SymbolBasketImpl(localsymbol);
                TLCtxHelper.ModuleDataRouter.RegisterSymbols(new List<Symbol>() { localsymbol});
            }
            session.OperationSuccess("合约数据更新成功");
        }



        #endregion

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "SyncSecInfo", "SyncSecInfo - sync  sec", "同步品种数据")]
        public void CTE_SyncSecInfo(ISession session)
        {
            Manager manager = session.GetManager();
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

        /// <summary>
        /// 禁止所有合约
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DisableAllSymbols", "DisableAllSymbols - sync  symbol", "禁止所有合约")]
        public void CTE_DisableAllSymbolsSymbol(ISession session)
        {
            Manager manager = session.GetManager();
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
            if (!manager.IsInRoot())
            {
                throw new FutsRspError("无权同步合约数据");
            }

            //通过实盘帐户同步合约数据
            if (manager.Domain.CFG_SyncVendor_ID !=0)
            {
                //Vendor vendor = BasicTracker.VendorTracker[manager.Domain.CFG_SyncVendor_ID];
                //if (vendor == null || vendor.Domain.ID != manager.domain_id || vendor.Broker == null || !vendor.Broker.IsLive || !(vendor.Broker is TLBroker))
                //{ 
                //    throw new FutsRspError("设置的同步实盘帐户无效,请修改同步实盘帐户");
                //}

                //TLBroker broker = vendor.Broker as TLBroker;
                //Action<XSymbol,bool> Handler = (sym, islast) =>
                //{
                //    if (islast)
                //    {
                //        logger.Info("got symbol synced....");
                //    }
                //    SymbolImpl tsym = manager.Domain.GetSymbol(sym.Symbol);
                //    //更新
                //    if (tsym != null)
                //    {
                //        tsym.EntryCommission = (decimal)sym.EntryCommission;
                //        tsym.ExitCommission = (decimal)sym.ExitCommission;
                //        tsym.Margin = (decimal)sym.Margin;
                //        tsym.ExpireDate = sym.ExpireDate;//到期日
                //        BasicTracker.SymbolTracker.UpdateSymbol(manager.domain_id, tsym);
                //    }
                //    else//增加
                //    {
                //        SecurityFamilyImpl sec = BasicTracker.SecurityTracker[manager.domain_id, sym.SecurityCode];
                //        //包含对应品种，则增加合约
                //        if (sec != null)
                //        {
                //            tsym = new SymbolImpl();
                //            tsym.Symbol = sym.Symbol;
                //            tsym.Domain_ID = manager.domain_id;
                //            tsym.Margin = (decimal)sym.Margin;
                //            tsym.EntryCommission = (decimal)sym.EntryCommission;
                //            tsym.ExitCommission = (decimal)sym.ExitCommission;
                //            tsym.ExpireDate = sym.ExpireDate;//到期日

                //            tsym.Strike = (decimal)sym.StrikePrice;
                //            tsym.OptionSide = sym.OptionSide;

                //            tsym.security_fk = sec.ID;
                //            tsym.Tradeable = false;
                //            BasicTracker.SymbolTracker.UpdateSymbol(manager.domain_id, tsym);
                //        }
                //        else
                //        {
                //            logger.Info("symbol:" + sym.Symbol + " have no sec setted. code:" + sym.SecurityCode);
                //        }
                //    }
                //};

                //broker.GotSymbolEvent += new Action<XSymbol, bool>(Handler);

                //if (!broker.QryInstrument())
                //{
                //    broker.GotSymbolEvent -= new Action<XSymbol, bool>(Handler);
                //    throw new FutsRspError("通道合约数据查询失败");
                //}
                //broker.GotSymbolEvent -= new Action<XSymbol, bool>(Handler);
                //session.OperationSuccess("同步合约数据完成");

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

        /// <summary>
        /// 从行情数据中心获得行情快照,
        /// 如果没有对应的快照需要获得最近的结算价数据信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnMGRQryTickSnapShot(MGRQryTickSnapShotRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("Manager[{0}] QryTickSnapshot", session.AuthorizedID));
            RspMGRQryTickSnapShotResponse response = null;
            if (string.IsNullOrEmpty(request.Exchange) && string.IsNullOrEmpty(request.Symbol))
            {
                IEnumerable<Tick> ticks = TLCtxHelper.ModuleDataRouter.GetTickSnapshot();
                for (int i = 0; i < ticks.Count(); i++)
                { 
                    response  =ResponseTemplate<RspMGRQryTickSnapShotResponse>.SrvSendRspResponse(request);
                    response.Tick = TickImpl.NewTick(ticks.ElementAt(i), "S");
                    CacheRspResponse(response, i == ticks.Count() - 1);
                }
                return;
            }
            else
            {
                Symbol sym = manager.Domain.GetSymbol(request.Exchange, request.Symbol);
                if (sym != null)
                {
                    Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(sym.Exchange, sym.Symbol);
                    if (k != null)
                    {
                        response = ResponseTemplate<RspMGRQryTickSnapShotResponse>.SrvSendRspResponse(request);
                        response.Tick = TickImpl.NewTick(k, "S");
                        CacheRspResponse(response);
                        return;
                    }
                }
            }

            response = ResponseTemplate<RspMGRQryTickSnapShotResponse>.SrvSendRspResponse(request);
            CacheRspResponse(response);
        }


    }
}
