using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    public partial class MsgExchServer
    {

        /// <summary>
        /// 查询委托
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryOrder(QryOrderRequest request,IAccount account)
        {
            logger.Info("QryOrder :" + request.ToString());
            Order[] orders = new Order[]{};
            //合约为空 查询所有
            if (string.IsNullOrEmpty(request.Symbol))
            {
                orders = account.Orders.Where(o=>!string.IsNullOrEmpty(o.OrderSysID)).ToArray();
            }

            int totalnum = orders.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspQryOrderResponse response = ResponseTemplate<RspQryOrderResponse>.SrvSendRspResponse(request);
                    response.OrderToSend = orders[i];
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                //返回空
                RspQryOrderResponse response = ResponseTemplate<RspQryOrderResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }

        }

        /// <summary>
        /// 查询成交
        /// 
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryTrade(QryTradeRequest request, IAccount account)
        {
            logger.Info("QryTrade :" + request.ToString());
            Trade[] trades = new Trade[] { };
            if (string.IsNullOrEmpty(request.Symbol))
            {
                trades = account.Trades.ToArray();
            }

            int totalnum = trades.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspQryTradeResponse response = ResponseTemplate<RspQryTradeResponse>.SrvSendRspResponse(request);
                    response.TradeToSend = trades[i];

                    logger.Info("转发当日成交:" + trades[i].ToString() + " side:" + trades[i].Side.ToString());
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspQryTradeResponse response = ResponseTemplate<RspQryTradeResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 查询持仓
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryPosition(QryPositionRequest request, IAccount account)
        {
            logger.Info("QryPosition :" + request.ToString());
            Position[] positions = new Position[] { };
            if (string.IsNullOrEmpty(request.Symbol))
            {
                positions = account.Positions.ToArray();
            }
            if (!string.IsNullOrEmpty(request.Symbol))
            {
                positions = account.Positions.Where(pos => pos.Symbol.Equals(request.Symbol)).ToArray();
            }

            logger.Info("total num:" + positions.Length.ToString());
            int totalnum = positions.Length;

            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspQryPositionResponse response = ResponseTemplate<RspQryPositionResponse>.SrvSendRspResponse(request);
                    response.PositionToSend = positions[i].GenPositionEx();
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspQryPositionResponse response = ResponseTemplate<RspQryPositionResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }


        

        /// <summary>
        /// 查询可开手数
        /// 开平/方向/合约
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryMaxOrderVol(QryMaxOrderVolRequest request, IAccount account)
        {
            logger.Info("QryMaxOrderVol :" + request.ToString());
            Symbol symbol = account.Domain.GetSymbol(request.Symbol);
            RspQryMaxOrderVolResponse response = ResponseTemplate<RspQryMaxOrderVolResponse>.SrvSendRspResponse(request);
            if(symbol == null)
            {
                response.RspInfo.Fill("SYMBOL_NOT_EXISTED");
                CachePacket(response);
            }
            if (account == null)
            {
                response.RspInfo.Fill("TRADING_ACCOUNT_NOT_FOUND");
                CachePacket(response);
            }
            else
            {
                
                //
                int size = account.CanOpenSize(symbol,request.Side,request.OffsetFlag);

                logger.Info("got max opensize:" + size.ToString());
                response.Symbol = request.Symbol;
                response.MaxVol = size >= 0 ? size : 0;
                response.OffsetFlag = request.OffsetFlag;
                response.Side = request.Side;

                CacheRspResponse(response, true);
            }
        }


        void SrvOnQryAccountInfo(QryAccountInfoRequest request, IAccount account)
        {
            logger.Info("QryAccountInfo :" + request.ToString());
            AccountInfo info = account.GenAccountInfo();
            //需要合并信用额度
            if (!account.GetParamCreditSeparate())
            {
                info.NowEquity += info.Credit;
                info.Credit = 0;
            }
            RspQryAccountInfoResponse response  = ResponseTemplate<RspQryAccountInfoResponse>.SrvSendRspResponse(request);
            response.AccInfo = info;
            CachePacket(response);
        }

        

        void SrvOnQrySettleInfoConfirm(QrySettleInfoConfirmRequest request,IAccount account)
        {
            logger.Info("QrySettleInfoConfirm :" + request.ToString());

            RspQrySettleInfoConfirmResponse response = ResponseTemplate<RspQrySettleInfoConfirmResponse>.SrvSendRspResponse(request);
            
            //IAccount account = TLCtxHelper.ModuleAccountManager[request.Account];
            logger.Info("confirm stamp:" + account.SettlementConfirmTimeStamp.ToString());

            response.TradingAccount = request.Account;
            //如果需要确认结算单 则将帐户的最新结算单确认时间回报
            if (needConfirmSettlement)
            {
                //如果结算时间戳为0 则表明为新注册用户,新注册用户不用注册
                DateTime confirmtime = Util.ToDateTime(account.SettlementConfirmTimeStamp);
                response.ConfirmDay = Util.ToTLDate(confirmtime);
                response.ConfirmTime = Util.ToTLTime(confirmtime);
            }
            else //如果不需要确认结算单 则返回上个交易日时间
            {
                DateTime confirmtime = DateTime.Now;
                response.ConfirmDay = Util.ToTLDate(confirmtime);
                response.ConfirmTime = Util.ToTLTime(confirmtime);

            }

            CachePacket(response);
        }

        void SrvOnConfirmSettlement(ConfirmSettlementRequest request,IAccount account)
        {
            logger.Info("ConfirmSettlement :" + request.ToString());
            
            //获得结算时间
            long timestamp = Util.ToTLDateTime(TLCtxHelper.ModuleSettleCentre.Tradingday, Util.ToTLTime());
            ORM.MSettlement.ConfirmeSettle(account.ID, TLCtxHelper.ModuleSettleCentre.Tradingday, timestamp);
            account.SettlementConfirmTimeStamp = timestamp;

            //发送结算确认
            RspConfirmSettlementResponse response = ResponseTemplate<RspConfirmSettlementResponse>.SrvSendRspResponse(request);
            logger.Info("confirm stamp:" + account.SettlementConfirmTimeStamp.ToString());
            DateTime confirmtime = Util.ToDateTime(account.SettlementConfirmTimeStamp);
            response.TradingAccount = request.Account;
            response.ConfirmDay = Util.ToTLDate(confirmtime);
            response.ConfirmTime = Util.ToTLTime(confirmtime);

            CachePacket(response);
            
        }

        /// <summary>
        /// 查询投资者信息
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryInvestor(QryInvestorRequest request, IAccount account)
        {
            logger.Info("QryInvestor :" + request.ToString());
            RspQryInvestorResponse response = ResponseTemplate<RspQryInvestorResponse>.SrvSendRspResponse(request);

            TrdClientInfo info = tl.GetClient(request.ClientID);

            AccountProfile profile = BasicTracker.AccountProfileTracker[account.ID];
            response.TradingAccount = account.ID;
            response.Email = profile.Email;
            //查询交易帐户时,如果token为空则生成帐户名否则传递Token
            response.NickName = string.IsNullOrEmpty(profile.Name) ? account.ID : profile.Name;
                
            CacheRspResponse(response);
        }

        void SrvOnContribRequest(ContribRequest request,ISession session)
        {
            logger.Info("Contrib Request:" + request.ToString());
            TLCtxHelper.Ctx.MessageExchangeHandler(session, request);
        }

        /// <summary>
        /// 请求修改密码
        /// </summary>
        /// <param name="request"></param>
        void SrvOnReqChangePassword(ReqChangePasswordRequest request,IAccount account)
        {
            logger.Info("ReqChangePassword:" + request.ToString());

            RspReqChangePasswordResponse response = ResponseTemplate<RspReqChangePasswordResponse>.SrvSendRspResponse(request);
            bool valid = TLCtxHelper.ModuleAccountManager.VaildAccount(account.ID, request.OldPassword);
            if(!valid)
            {
                response.RspInfo.Fill("OLD_PASS_ERROR");
                CachePacket(response);
                return;
            }

            //修改密码返回
            TLCtxHelper.ModuleAccountManager.UpdateAccountPass(account.ID, request.NewPassword);
            CachePacket(response);
        }

        /// <summary>
        /// 查询通知
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryNotice(QryNoticeRequest request,IAccount account)
        {
            logger.Info("QryNoticeRequest:" + request.ToString());
            RspQryNoticeResponse response = ResponseTemplate<RspQryNoticeResponse>.SrvSendRspResponse(request);
            if (account != null)
            {
                if (account.Category == QSEnumAccountCategory.SUBACCOUNT)
                {
                    response.NoticeContent = ("欢迎使用" + GlobalConfig.VendorName + "交易系统");
                }
                //else if (account.Category == QSEnumAccountCategory.REAL)
                //{
                //    response.NoticeContent = string.IsNullOrEmpty(GlobalConfig.RealPrompt) ? ("欢迎使用" + GlobalConfig.VendorName + "交易系统,市场有风险,投资需谨慎!祝您交易愉快!") : GlobalConfig.RealPrompt;
                //}
            }
            //如果通知内容为空 则提供默认提示
            if(string.IsNullOrEmpty(response.NoticeContent))
            {
                response.NoticeContent =("欢迎使用" + GlobalConfig.VendorName + "交易系统");
            }

            //输出交易帐户 交易通知
            foreach (var notice in account.GetNotice())
            {
                RspQryNoticeResponse tmp = ResponseTemplate<RspQryNoticeResponse>.SrvSendRspResponse(request);
                tmp.NoticeContent = notice;
                CacheRspResponse(tmp, false);
            }
            CacheRspResponse(response, true);

        }

        /// <summary>
        /// 查询合约
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQrySymbol(QrySymbolRequest request,IAccount account)
        {
            logger.Info("QrySymbol:" + request.ToString());
            Util.sleep(1000);
            Instrument[] instruments = new Instrument[]{};
            if (request.SecurityType != SecurityType.NIL && string.IsNullOrEmpty(request.ExchID) && string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.Security))
            {
                instruments =account.GetInstruments(request.SecurityType).ToArray();  
                
            }
            //如果所有字段为空 则为查询所有合约列表
            if (request.SecurityType == SecurityType.NIL && string.IsNullOrEmpty(request.ExchID) && string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.Security))
            {
                instruments = account.GetInstruments().ToArray();
            }

            if (instruments.Length > 0)
            {
                for (int i = 0; i < instruments.Length; i++)
                {
                    RspQrySymbolResponse response = ResponseTemplate<RspQrySymbolResponse>.SrvSendRspResponse(request);
                    response.InstrumentToSend = instruments[i];
                    CacheRspResponse(response, i == instruments.Length - 1);
                }
            }
            else
            {
                RspQrySymbolResponse response = ResponseTemplate<RspQrySymbolResponse>.SrvSendRspResponse(request);
                response.InstrumentToSend = new Instrument();
                CacheRspResponse(response);
            }
            
        }

        /// <summary>
        /// 查询签约银行
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryContractBank(QryContractBankRequest request)
        {
            logger.Info("QryContractBank:" + request.ToString());
            ContractBank[] banks = BasicTracker.ContractBankTracker.Banks;
            if (banks.Length > 0)
            {

                for(int i=0; i<banks.Length;i++)
                {
                    RspQryContractBankResponse response = ResponseTemplate<RspQryContractBankResponse>.SrvSendRspResponse(request);
                    response.BankName = banks[i].Name;
                    response.BankID = banks[i].BrankID;
                    response.BankBrchID = banks[i].BrankBrchID;
                    CacheRspResponse(response, i == banks.Length - 1);


                }
            }
            else
            {
                RspQryContractBankResponse response = ResponseTemplate<RspQryContractBankResponse>.SrvSendRspResponse(request);

                CachePacket(response);
            }
        }

        /// <summary>
        /// 系统返回签约银行列表后
        /// 客户端会查询每个签约银行所对应的银行帐户
        /// 这里所有查询均返回同样的帐户
        /// </summary>
        /// <param name="request"></param>
        void SrvOnRegisterBankAccount(QryRegisterBankAccountRequest request,IAccount account)
        {
            logger.Info("QryRegisterBankAccount:" + request.ToString());
            if (account != null)
            {
                RspQryRegisterBankAccountResponse response = ResponseTemplate<RspQryRegisterBankAccountResponse>.SrvSendRspResponse(request);
                response.TradingAccount = account.ID;
                AccountProfile profile = BasicTracker.AccountProfileTracker[account.ID];
                //如果对应的BankFK不为0 则传递设置的银行帐户信息
                if (profile.Bank_ID!=0)
                {
                    response.BankAC = profile.BankAC;//获得银行卡号 如果没有设置银行卡号码 会导致博易客户端频繁请求交易帐号信息
                    ContractBank bank = BasicTracker.ContractBankTracker[profile.Bank_ID];
                    response.BankName =  bank.Name;
                    response.BankID = bank.BrankID;
                }
                CacheRspResponse(response);
            }
            else
            {
                RspQryRegisterBankAccountResponse response = ResponseTemplate<RspQryRegisterBankAccountResponse>.SrvSendRspResponse(request);
                response.RspInfo.Fill("TRADING_ACCOUNT_NOT_FOUND");
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 查询出入金记录
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryTransferSerial(QryTransferSerialRequest request)
        {
            //TODO:交易CTP接口查询出入金记录
            logger.Info("QryTransferSerialRequest:" + request.ToString());
            CashTransaction[] cts = ORM.MCashTransaction.SelectHistCashTransactions(request.TradingAccount, 0, 0).ToArray();
            IAccount account = TLCtxHelper.ModuleAccountManager[request.TradingAccount];
            int totalnum = cts.Length;
            logger.Info("total transfer num:" + totalnum.ToString());
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspQryTransferSerialResponse response = ResponseTemplate<RspQryTransferSerialResponse>.SrvSendRspResponse(request);
                    CashTransaction t = cts[i];
                    AccountProfile profile = BasicTracker.AccountProfileTracker[account.ID];

                    response.Date = Util.ToTLDate(t.DateTime);
                    response.Time = 0;// Util.ToTLTime(t.DateTime);
                    response.TradingAccount = request.TradingAccount;
                    response.BankAccount = profile.BankAC;
                    response.Amount = t.Amount;
                    response.TransRef = "";//t.TransRef;
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                //返回空项目
                RspQryTransferSerialResponse response = ResponseTemplate<RspQryTransferSerialResponse>.SrvSendRspResponse(request);
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 查询合约手续费率
        /// 这里可以通过传递特殊的参数来影响每个交易帐户的手续费
        /// 同时在扩展模块中 可以调整对应的手续费
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnQryInstrumentCommissionRate(QryInstrumentCommissionRateRequest request, IAccount account)
        {
            logger.Info("QryInstrumentCommissionRate:" + request.ToString());
            
            //返回所有
            if (string.IsNullOrEmpty(request.Symbol))
            {
                //
            }
            else
            {
                RspQryInstrumentCommissionRateResponse response = ResponseTemplate<RspQryInstrumentCommissionRateResponse>.SrvSendRspResponse(request);
                Symbol sym = account.Domain.GetSymbol(request.Exchange,request.Symbol);
                if (sym == null)
                {
                    response.RspInfo.Fill("SYMBOL_NOT_EXISTED");
                }
                else
                {
                    CommissionConfig cfg = account.GetCommissionConfig(sym);
                    response.FillCommissionCfg(cfg);
                }
                CacheRspResponse(response);
            }
        }

        void SrvOnQryInstrumentMarginRate(QryInstrumentMarginRateRequest request, IAccount account)
        {
            logger.Info("QryInstrumentMarginRate:" + request.ToString());
            
            if (string.IsNullOrEmpty(request.Symbol))
            {

            }
            else 
            {
                RspQryInstrumentMarginRateResponse response = ResponseTemplate<RspQryInstrumentMarginRateResponse>.SrvSendRspResponse(request);
                Symbol sym = account.Domain.GetSymbol(request.Exchange,request.Symbol);
                if (sym == null)
                {
                    response.RspInfo.Fill("SYMBOL_NOT_EXISTED");
                }
                else
                {
                    MarginConfig cfg = account.GetMarginConfig(sym);
                    response.FillMarginCfg(cfg);

                }
                CacheRspResponse(response);

            }
        }


        /// <summary>
        /// 查询市场行情处理
        /// 市场行情回报处理错误 会导致FX无法正常登入 在查询市场行情 这步操作会失败
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnQryMarketData(QryMarketDataRequest request, IAccount account)
        {
            logger.Info("QryMarketData:" + request.ToString());

            if (string.IsNullOrEmpty(request.Symbol))
            {
                //这里通过查询持仓来获得对应的结算价 保持快期交易端与服务端数据一致
                Symbol[] symlist = account.GetSymbols().ToArray();//获得交易帐户可交易列表
                for (int i = 0; i < symlist.Length; i++)
                {
                    Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(symlist[i].Exchange,symlist[i].Symbol);// CmdUtils.GetTickSnapshot(symlist[i].Symbol);
                    if (k == null || !k.IsValid())
                    {
                        k = TLCtxHelper.ModuleSettleCentre.GetLastTickSnapshot(symlist[i].Symbol);
                    }

                    //如果没有找到对应的合约则进入下一个合约进行行情数据处理
                    if (k == null)
                    {
                        k = new TickImpl(symlist[i].Symbol);
                        k.Exchange = symlist[i].SecurityFamily.Exchange.EXCode;
                    }
                    //TODO:行情和结算优化后 这里只需要查询当前DataRouter的行情快照即可
                    RspQryMarketDataResponse response = ResponseTemplate<RspQryMarketDataResponse>.SrvSendRspResponse(request);

                    //按帐户对应隔夜持仓的结算价来设定查询行情昨日结算价，快期交易客户端通过查询行情来获得隔夜持仓的持仓成本（持仓结算价依赖于结算模式，国内期货是按结算价进行结算，国外持仓按初始开仓价进行结算-不用盯市结算）
                    Position longpos = account.GetPosition(symlist[i].Symbol, true);
                    Position shortpos = account.GetPosition(symlist[i].Symbol, true);

                    decimal presettlement = 0;
                    if (longpos!= null && longpos.PositionDetailYdRef.Count() > 0)
                    {
                        presettlement = longpos.PositionDetailYdRef.FirstOrDefault().SettlementPrice;
                    }
                    if (shortpos != null && shortpos.PositionDetailYdRef.Count() > 0)
                    {
                        presettlement = shortpos.PositionDetailYdRef.FirstOrDefault().SettlementPrice;
                    }
                    if (presettlement != 0)
                    {
                        k.PreSettlement = presettlement;
                    }

                    response.TickToSend = k;

                    CacheRspResponse(response, i == symlist.Length - 1);
                }
            }
        }

        /// <summary>
        /// 查询交易参数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnQryTradingParams(QryTradingParamsRequest request, IAccount account)
        {
            logger.Info("QryTradingParams:" + request.ToString());
            ExStrategy setting = account.GetExStrategy();

            if (setting == null)
            {
                RspQryTradingParamsResponse response = ResponseTemplate<RspQryTradingParamsResponse>.SrvSendRspResponse(request);
                response.Account = account.ID;
                response.Algorithm = QSEnumAlgorithm.AG_All;
                response.MarginPriceType = QSEnumMarginPrice.OpenPrice;
                response.IncludeCloseProfit = true;
                CacheRspResponse(response);
            }
            else
            {
                RspQryTradingParamsResponse response = ResponseTemplate<RspQryTradingParamsResponse>.SrvSendRspResponse(request);
                response.Account = account.ID;
                response.Algorithm = account.GetParamAlgorithm();
                response.MarginPriceType = account.GetParamMarginPriceType();
                response.IncludeCloseProfit = account.GetParamIncludeCloseProfit();

                CacheRspResponse(response);
            }
        }

    }
}
