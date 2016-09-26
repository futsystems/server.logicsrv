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

        #region 检查品种与合约数据

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "CheckSecurityAndSymbol", "CheckSecurityAndSymbol - check security and symbol data", "检查品种和合约数据")]
        public void CTE_CheckSecurityAndSymbol(ISession session)
        {
            logger.Info("检查品种和合约数据");
            Manager manager = session.GetManager();

            bool right = manager.Domain.Super && manager.IsRoot();//超级分区的管理员才有权限执行数据检查
            if (!right)
            {
                throw new FutsRspError("无权进行基础数据检查");
            }

            #region 更新品种
            //遍历所有品种
            foreach (var sec in manager.Domain.GetSecurityFamilies())
            {
                //更新所有域对应品种
                foreach (var d in BasicTracker.DomainTracker.Domains)
                {
                    if (d.ID == manager.Domain.ID) //不更新超级域
                        continue;

                    //获得该域的品种对象 品种更新时按数据库ID进行查询
                    SecurityFamilyImpl target = d.GetSecurityFamily(sec.Code);
                    if (target == null)
                    {
                        //创建品种对象
                        target = new SecurityFamilyImpl();
                        target.Domain_ID = d.ID;
                    }

                    //复制品种数据
                    target.Code = sec.Code;
                    target.Name = sec.Name;
                    target.Currency = sec.Currency;
                    target.Type = sec.Type;

                    target.exchange_fk = sec.exchange_fk;
                    target.Exchange = BasicTracker.ExchagneTracker[target.exchange_fk];

                    target.mkttime_fk = sec.mkttime_fk;
                    target.MarketTime = BasicTracker.MarketTimeTracker[target.mkttime_fk];

                    target.underlaying_fk = sec.underlaying_fk;
                    target.UnderLaying = BasicTracker.SecurityTracker[target.Domain_ID, target.underlaying_fk];

                    target.Multiple = sec.Multiple;
                    target.PriceTick = sec.PriceTick;
                    target.EntryCommission = sec.EntryCommission;
                    target.ExitCommission = sec.ExitCommission;
                    target.Margin = sec.Margin;
                    target.ExtraMargin = sec.ExtraMargin;
                    target.MaintanceMargin = sec.MaintanceMargin;
                    target.Tradeable = sec.Tradeable;

                    //对该分区更新或添加品种信息
                    d.UpdateSecurity(target, false);
                }
            }

            #endregion


            #region 更新合约
            foreach (var sym in manager.Domain.GetSymbols())
            {
                //更新所有域对应品种
                foreach (var d in BasicTracker.DomainTracker.Domains)
                {
                    if (d.ID == manager.Domain.ID) //不更新超级域
                        continue;
                    d.UpdateSymbolViaSuper(sym);
                }
            }
            session.OperationSuccess("基础数据检查完毕");
            #endregion
        }

        #endregion

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

        void SrvOnMGRUpdateExchange(MGRUpdateExchangeRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求更新交易所信息:{1}", session.AuthorizedID, request.ToString()));
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

        void SrvOnMGRUpdateMarketTime(MGRUpdateMarketTimeRequest request, ISession session, Manager manager)
        {
            logger.Info(string.Format("管理员:{0} 请求更新交易时间段:{1}", session.AuthorizedID, request.ToString()));
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

                SymbolImpl symbol = request.Symbol;
                //设定合约symbol为当前管理员域ID 避免管理端没有正常传输分区ID
                symbol.Domain_ID = manager.Domain.ID;

                //检查品种是否完毕
                SecurityFamilyImpl rawsec = BasicTracker.SecurityTracker[symbol.Domain_ID, symbol.security_fk];
                if (rawsec == null)
                {
                    throw new FutsRspError("品种数据异常");
                }

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
                    TLCtxHelper.ModuleExCore.RegisterSymbol(localsymbol);
                }
                session.OperationSuccess("合约数据更新成功");
            }
            catch (FutsRspError ex)
            {
                session.OperationError(ex);
            }
        }


        //void SrvOnMGRReqAddSecurity(MGRReqAddSecurityRequest request, ISession session, Manager manager)
        //{
        //    try
        //    {
        //        logger.Info(string.Format("管理员:{0} 请求添加品种:{1}", session.AuthorizedID, request.ToString()));

        //        if (!manager.IsInRoot())
        //        {
        //            throw new FutsRspError("无权添加品种数据");
        //        }

        //        SecurityFamilyImpl sec = request.SecurityFaimly;
        //        if (manager.Domain.GetSecurityFamily(sec.Code) == null)
        //        {
        //            manager.Domain.UpdateSecurity(sec);

        //            //如果是超级域 则同步更新所有分区
        //            if (manager.Domain.Super)
        //            {
        //                //更新所有域的品种
        //                foreach (var d in BasicTracker.DomainTracker.Domains)
        //                {
        //                    if (d.ID == manager.Domain.ID) //超级域 跳过 已经更新过品种数据
        //                        continue;
        //                    d.UpdateSecurity(sec);
        //                }
        //            }



        //            RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
        //            response.SecurityFaimly = manager.Domain.GetSecurityFamily(sec.Code);
        //            CacheRspResponse(response);
        //        }
        //        else
        //        {

        //            RspMGRQrySecurityResponse response = ResponseTemplate<RspMGRQrySecurityResponse>.SrvSendRspResponse(request);
        //            response.RspInfo.Fill("SECURITY_EXIST");
        //            CacheRspResponse(response);
        //        }
        //        if (sec.Tradeable)
        //        {
        //            //exchsrv.RegisterSymbol(BasicTracker.SecurityTracker[sec.Code]);
        //        }
        //    }
        //    catch (FutsRspError ex)
        //    {
        //        session.OperationError(ex);
        //    }
        //}

        //void SrvOnMGRReqAddSymbol(MGRReqAddSymbolRequest request, ISession session, Manager manager)
        //{
        //    try
        //    {
        //        logger.Info(string.Format("管理员:{0} 请求添加合约:{1}", session.AuthorizedID, request.ToString()));

        //        if (!manager.IsInRoot())
        //        {
        //            throw new FutsRspError("无权添加合约数据");
        //        }

        //        SymbolImpl symbol = request.Symbol;
        //        //设定合约symbol为当前管理员域ID 避免管理端没有正常传输分区ID
        //        symbol.Domain_ID = manager.Domain.ID;

        //        SecurityFamilyImpl rawsec = BasicTracker.SecurityTracker[symbol.Domain_ID, symbol.security_fk];
        //        if (rawsec == null)
        //        {
        //            Util.Debug("品种数据异常，直接返回");
        //            return;
        //        }

        //        if (manager.Domain.GetSymbol(symbol.Symbol) == null)
        //        {
        //            manager.Domain.UpdateSymbol(symbol);

        //            //如果是超级域 则同步更新所有分区
        //            if (manager.Domain.Super)
        //            {
        //                //更新所有域的合约
        //                foreach (var d in BasicTracker.DomainTracker.Domains)
        //                {
        //                    if (d.ID == manager.Domain.ID) //超级域 跳过 已经更新过品种数据
        //                        continue;
        //                    d.UpdateSymbolViaSuper(symbol);
        //                }
        //            }


        //            RspMGRReqAddSymbolResponse response = ResponseTemplate<RspMGRReqAddSymbolResponse>.SrvSendRspResponse(request);
        //            response.Symbol = manager.Domain.GetSymbol(symbol.Symbol);
        //            CacheRspResponse(response);
        //        }
        //        else
        //        {
        //            RspMGRReqAddSymbolResponse response = ResponseTemplate<RspMGRReqAddSymbolResponse>.SrvSendRspResponse(request);
        //            response.RspInfo.Fill("SYMBOL_EXIST");
        //            CacheRspResponse(response);
        //        }
        //        if (symbol.Tradeable)
        //        {
        //            TLCtxHelper.ModuleExCore.RegisterSymbol(manager.Domain.GetSymbol(symbol.Symbol));
        //        }
        //    }
        //    catch (FutsRspError ex)
        //    {
        //        session.OperationError(ex);
        //    }
        //}

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
    }
}
