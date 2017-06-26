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
        void SrvOnXQryMarketTime(ISession session, XQryMarketTimeRequest request, IAccount account)
        {
            logger.Info("XMarketTime:" + request.ToString());
            MarketTimeImpl[] mts = BasicTracker.MarketTimeTracker.MarketTimes;
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
        void SrvOnXQryExchange(ISession session, XQryExchangeRequuest request, IAccount account)
        {
            logger.Info("XQryExchange:"+request.ToString());
            ExchangeImpl[] exchs = BasicTracker.ExchagneTracker.Exchanges;

            int totalnum = exchs.Length;

            for (int i = 0; i < totalnum; i++)
            {
                RspXQryExchangeResponse response = ResponseTemplate<RspXQryExchangeResponse>.SrvSendRspResponse(request);
                response.Exchange = exchs[i];

                CacheRspResponse(response, i == totalnum - 1);
            }
        }

        

        /// <summary>
        /// 响应品种查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQrySecurity(ISession session, XQrySecurityRequest request, IAccount account)
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
        void SrvOnXQrySymbol(ISession session, XQrySymbolRequest request, IAccount account)
        {
            logger.Info("XQrySymbol:" + request.ToString());

            IEnumerable<Symbol> tmplsit = null;
            if (string.IsNullOrEmpty(request.Symbol))
            {
                TrdClientInfo client = tl.GetClient(request.ClientID);
                //股票交易终端只返回必要的合约数据 其余合约数据延迟加载
                if (client != null && client.ProductInfo.Equals("XTrader.Stock"))
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
                //TODO SmbolKey
                Symbol sym = account.Domain.GetSymbol(request.Exchange,request.Symbol);
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
        void SrvOnXQryTickSnapShot(ISession session, XQryTickSnapShotRequest request, IAccount account)
        {
            logger.Info("XQryTickSnapShot:" + request.ToString());
            Symbol sym = account.Domain.GetSymbol(request.Exchange, request.Symbol);
            RspXQryTickSnapShotResponse response = null;
            if (sym != null)
            {
                Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(sym.Exchange, sym.Symbol);
                if (k != null)
                {
                    response = ResponseTemplate<RspXQryTickSnapShotResponse>.SrvSendRspResponse(request);
                    response.Tick = TickImpl.NewTick(k, "S");
                    CacheRspResponse(response);
                    return;
                }
            }

            response = ResponseTemplate<RspXQryTickSnapShotResponse>.SrvSendRspResponse(request);
            CacheRspResponse(response);
            


            //IEnumerable<Symbol> symlist= null;
            //if (string.IsNullOrEmpty(request.Symbol))
            //{
            //    symlist = account.GetSymbols().Where(sym => sym.IsTradeable);//未指定合约 则所有可交易合约
            //}
            //else
            //{
            //    Symbol sym = account.Domain.GetSymbol(request.Exchange, request.Symbol);

            //    symlist = account.GetSymbols().Where(sym => sym.IsTradeable).Where(sym =>
            //    {
            //        if (string.IsNullOrEmpty(request.Exchange))//没有设定交易所 则单独比较Symbol
            //        {
            //            return sym.Symbol == request.Symbol;
            //        }
            //        else
            //        {
            //            return sym.UniqueKey == string.Format("{0}-{1}", request.Exchange, request.Symbol);
            //        }
            //    });//获得某个合约
            //}

            //int totalnum = symlist.Count();
            //if (totalnum > 0)
            //{
            //    for (int i = 0; i < totalnum; i++)
            //    {
            //        Symbol sym = symlist.ElementAt(i);
            //        Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(sym.Exchange,sym.Symbol);
            //        if (k == null || !k.IsValid())
            //        {
            //            k = TLCtxHelper.ModuleSettleCentre.GetLastTickSnapshot(sym.Symbol);
            //        }
            //        if (sym.SecurityType == SecurityType.STK)
            //        {
            //            k.Type = EnumTickType.STKSNAPSHOT;
            //        }
            //        RspXQryTickSnapShotResponse response = ResponseTemplate<RspXQryTickSnapShotResponse>.SrvSendRspResponse(request);
            //        response.Tick = k;
            //        CacheRspResponse(response, i == totalnum - 1);
            //    }
            //}
            //else
            //{
            //    RspXQryTickSnapShotResponse response = ResponseTemplate<RspXQryTickSnapShotResponse>.SrvSendRspResponse(request);
            //    CacheRspResponse(response);
            //}
        }



        /// <summary>
        /// 查询汇率信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnXQryExchangeRate(ISession session, XQryExchangeRateRequest request, IAccount account)
        {
            logger.Info("XQryExchangeRate:" + request.ToString());
            //结算时没有重置 导致 交易日进入下一个交易日后无法获得汇率数据 自制客户端无法登入
            IEnumerable<ExchangeRate> ratelist = account.Domain.GetExchangeRates(TLCtxHelper.ModuleSettleCentre.Tradingday);
            for (int i = 0; i < ratelist.Count(); i++)
            {
                RspXQryExchangeRateResponse response = ResponseTemplate<RspXQryExchangeRateResponse>.SrvSendRspResponse(request);
                response.ExchangeRate = ratelist.ElementAt(i);
                CacheRspResponse(response, i == ratelist.Count() - 1);
            }
        
        }

        void SrvOnXQryBankCard(ISession session, XQryBankCardRequest request, IAccount account)
        {
            logger.Info("XQryBankCard:" + request.ToString());

            AccountProfile profile = BasicTracker.AccountProfileTracker[account.ID];
            BankCardInfo info = null;
            ContractBank bank= BasicTracker.ContractBankTracker[profile.Bank_ID];
            if (bank != null)
            {
                info = new BankCardInfo();
                info.Name = profile.Name;
                info.CertCode = profile.IDCard;
                info.MobilePhone = profile.Mobile;
                info.BankAccount = profile.BankAC;
                info.BankBrch = profile.Branch;
                info.BankID = bank.BrankID;
            }
            
            RspXQryBankCardResponse response = ResponseTemplate<RspXQryBankCardResponse>.SrvSendRspResponse(request);
            response.BankCardInfo = info;
            CacheRspResponse(response);
        }

        void SrvOnXReqUpdateBankCard(ISession session, XReqUpdateBankCardRequest request, IAccount account)
        {
            logger.Info("XUpdateBankCard:" + request.ToString());
            AccountProfile profile = BasicTracker.AccountProfileTracker[account.ID];
            ContractBank bank= BasicTracker.ContractBankTracker[request.BankCardInfo.BankID];
            if (bank != null)
            {
                profile.Name = request.BankCardInfo.Name;
                profile.IDCard = request.BankCardInfo.CertCode;
                profile.Mobile = request.BankCardInfo.MobilePhone;
                profile.BankAC = request.BankCardInfo.BankAccount;
                profile.Branch = request.BankCardInfo.BankBrch;
                profile.Bank_ID = bank.ID;
                BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            }

            BankCardInfo info = null;
            bank = BasicTracker.ContractBankTracker[profile.Bank_ID];
            if (bank != null)
            {
                info = new BankCardInfo();
                info.Name = profile.Name;
                info.CertCode = profile.IDCard;
                info.MobilePhone = profile.Mobile;
                info.BankAccount = profile.BankAC;
                info.BankBrch = profile.Branch;
                info.BankID = bank.BrankID;
            }
            
            RspXReqUpdateBankCardResponse response = ResponseTemplate<RspXReqUpdateBankCardResponse>.SrvSendRspResponse(request);
            response.BankCardInfo = info;
            CacheRspResponse(response);


        }

        void SrvOnXReqCashOperation(ISession session, XReqCashOperationRequest request, IAccount account)
        {
            logger.Info("XReqCashOperationRequest:" + request.ToString());

            RspXReqCashOperationResponse response = ResponseTemplate<RspXReqCashOperationResponse>.SrvSendRspResponse(request);

            if (!account.Domain.Module_PayOnline)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "柜台支付模块未激活";

                CacheRspResponse(response);
                return;
            }
            
            if (TLCtxHelper.ModuleSettleCentre.SettleMode != QSEnumSettleMode.StandbyMode)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "柜台结算中,出入金业务暂停";

                CacheRspResponse(response);
                return;
            }

            /*
            if (request.Amount > 0 && request.Amount > _depositLimit)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "单笔入金超过限额:" + _depositLimit.ToFormatStr();

                CacheRspResponse(response);
                return;
            }
            

            if (request.Amount < 0 && Math.Abs(request.Amount) > account.NowEquity-(account.Margin + account.MarginFrozen))
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "出金超过可提资金:" + (account.NowEquity - (account.Margin + account.MarginFrozen)).ToFormatStr();

                CacheRspResponse(response);
                return;
            }
            **/

            CashOperationRequest tmp = new CashOperationRequest();
            tmp.Account = account.ID;
            tmp.Amount = request.Amount;
            tmp.Args = request.Args;
            tmp.GateWay = request.Gateway;

            bool ret = TLCtxHelper.EventSystem.FireCashOperationProcess(tmp);

            if (!ret)
            {
                response.RspInfo.ErrorID = 1;
                response.RspInfo.ErrorMessage = "操作失败:" + tmp.ProcessComment;

                CacheRspResponse(response);
                return;
            }

            response.CashOperationRequest = tmp;
            CacheRspResponse(response);





        }
    }
}
