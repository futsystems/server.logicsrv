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
        /// 返回委托对于第一阶段拒绝的委托 查询不返回
        /// 因为飞迅
        /// strcpy(toOrder->OrderSysID,fromOrder->OrderExchID.c_str());//报单编号
        /// 如果OrderSysID缺失 会造成排序错乱导致数据汇总错误。
        /// 盘中交易时 第一风控阶段觉得委托可以正常显示 因此用
        /// .Where(o=>!string.IsNullOrEmpty(o.OrderSysID))进行过滤
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryOrder(QryOrderRequest request,IAccount account)
        {
            debug("QryOrder :" + request.ToString(), QSEnumDebugLevel.INFO);
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
                response.OrderToSend = new OrderImpl();
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
            debug("QryTrade :" + request.ToString(), QSEnumDebugLevel.INFO);
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

                    //debug("转发当日成交:" + trades[i].ToString() + " side:" + trades[i].Side.ToString(), QSEnumDebugLevel.INFO);
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspQryTradeResponse response = ResponseTemplate<RspQryTradeResponse>.SrvSendRspResponse(request);
                response.TradeToSend = new TradeImpl();
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 查询持仓
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryPosition(QryPositionRequest request, IAccount account)
        {
            debug("QryPosition :" + request.ToString(), QSEnumDebugLevel.INFO);
            Position[] positions = new Position[] { };
            //Position[] netpos = new Position[] { };

            if (string.IsNullOrEmpty(request.Symbol))
            {
                positions = account.Positions.ToArray();
                //netpos = account.PositionsNet.ToArray();
            }
            if (!string.IsNullOrEmpty(request.Symbol))
            {
                positions = account.Positions.Where(pos => pos.Symbol.Equals(request.Symbol)).ToArray();
            }

            //debug("total num:" + positions.Length.ToString(), QSEnumDebugLevel.INFO);
            int totalnum = positions.Length;

            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    
                    RspQryPositionResponse response = ResponseTemplate<RspQryPositionResponse>.SrvSendRspResponse(request);
                    response.PositionToSend = positions[i].GenPositionEx();
                    debug("转发持仓:" + response.ToString(),QSEnumDebugLevel.INFO);
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspQryPositionResponse response = ResponseTemplate<RspQryPositionResponse>.SrvSendRspResponse(request);
                response.PositionToSend = new PositionEx();
                CacheRspResponse(response);
            }
        }


        /// <summary>
        /// 查询持仓明细
        /// 注意查询持仓明细 是指查询昨日留仓持仓明细
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnQryPositionDetail(QryPositionDetailRequest request, IAccount account)
        {
            debug("QryPositionDetail" + request.ToString(), QSEnumDebugLevel.INFO);
            List<PositionDetail> list = new List<PositionDetail>();
            foreach (Position p in account.Positions)
            {
                foreach (PositionDetail pd in p.PositionDetailTotal)
                {
                    list.Add(pd);
                }
            }
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    RspQryPositionDetailResponse response = ResponseTemplate<RspQryPositionDetailResponse>.SrvSendRspResponse(request);
                    response.PositionDetailToSend = list[i];
                    CacheRspResponse(response, i == list.Count - 1);
                }
            }
            else
            {   //发送空的持仓回报
                RspQryPositionDetailResponse response = ResponseTemplate<RspQryPositionDetailResponse>.SrvSendRspResponse(request);
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
            debug("QryMaxOrderVol :" + request.ToString(), QSEnumDebugLevel.INFO);
            Symbol symbol = account.GetSymbol(request.Symbol);
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
                
                int size = account.CanOpenSize(symbol,request.Side,request.OffsetFlag);

                debug("got max opensize:" + size.ToString(), QSEnumDebugLevel.INFO);
                response.Symbol = request.Symbol;
                response.MaxVol = size;
                //如果请求查询下单数量时 没有正常提供offsetflag,则我们以开仓为默认，否则以请求的offset进行回报
                response.OffsetFlag = request.OffsetFlag == QSEnumOffsetFlag.UNKNOWN ? QSEnumOffsetFlag.OPEN : request.OffsetFlag;
                response.Side = request.Side;

                CacheRspResponse(response, true);
            }
        }


        void SrvOnQryAccountInfo(QryAccountInfoRequest request, IAccount account)
        {
            debug("QryAccountInfo :" + request.ToString(), QSEnumDebugLevel.INFO);
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

        void SrvOnQrySettleInfo(QrySettleInfoRequest request)
        {
            debug("QrySettleInfo :" + request.ToString(), QSEnumDebugLevel.INFO);
            Settlement settlement = null;
            //如果查询日期为0 则查询上个结算日
            IAccount account = _clearcentre[request.Account];
            //判断account是否为空
            int settleday = request.Tradingday;
            if (settleday == 0)
            {
                debug("Request Tradingday:0 ,try to get the settlement of lastsettleday:" + TLCtxHelper.Ctx.SettleCentre.LastSettleday, QSEnumDebugLevel.INFO);
                settleday = TLCtxHelper.Ctx.SettleCentre.LastSettleday;
            }

            settlement = ORM.MSettlement.SelectSettlement(request.Account, settleday);
            if (settlement != null)
            {
                debug("got settlement....", QSEnumDebugLevel.INFO);
                List<string> settlelist = SettlementFactory.GenSettlementFile(settlement,account);
                for (int i = 0; i < settlelist.Count; i++)
                {
                    RspQrySettleInfoResponse response = ResponseTemplate<RspQrySettleInfoResponse>.SrvSendRspResponse(request);
                    response.Tradingday = settlement.SettleDay;
                    response.TradingAccount = settlement.Account;
                    response.SettlementContent = settlelist[i] + "\n";
                    CacheRspResponse(response, i == settlelist.Count - 1);
                }
            }
            else
            {
                RspQrySettleInfoResponse response = ResponseTemplate<RspQrySettleInfoResponse>.SrvSendRspResponse(request);
                debug("can not find settlement for account:" + request.Account + " for settleday:" + request.Tradingday.ToString(), QSEnumDebugLevel.WARNING);
                response.Tradingday = settleday;
                response.TradingAccount = request.Account;
                response.SettlementContent ="该帐户没有任何结算记录";
                CachePacket(response);
            }
        }

        /// <summary>
        /// 如果tradingday 为0 则在接口侧我们判定非交易日不需要确认结算单
        /// 
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQrySettleInfoConfirm(QrySettleInfoConfirmRequest request)
        {
            debug("QrySettleInfoConfirm :" + request.ToString(), QSEnumDebugLevel.INFO);

            RspQrySettleInfoConfirmResponse response = ResponseTemplate<RspQrySettleInfoConfirmResponse>.SrvSendRspResponse(request);
            IAccount account = _clearcentre[request.Account];
            debug("confirm stamp:" + account.SettlementConfirmTimeStamp.ToString(), QSEnumDebugLevel.INFO);

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

        void SrvOnConfirmSettlement(ConfirmSettlementRequest request)
        {
            debug("ConfirmSettlement :" + request.ToString(), QSEnumDebugLevel.INFO);
            
            //获得结算时间
            long timestamp = Util.ToTLDateTime(TLCtxHelper.Ctx.SettleCentre.CurrentTradingday, Util.ToTLTime());
            ORM.MSettlement.ConfirmeSettle(request.Account,TLCtxHelper.Ctx.SettleCentre.CurrentTradingday, timestamp);
            IAccount account = _clearcentre[request.Account];
            account.SettlementConfirmTimeStamp = timestamp;

            //发送结算确认
            RspConfirmSettlementResponse response = ResponseTemplate<RspConfirmSettlementResponse>.SrvSendRspResponse(request);
            debug("confirm stamp:" + account.SettlementConfirmTimeStamp.ToString(), QSEnumDebugLevel.INFO);
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
            debug("QryInvestor :" + request.ToString(), QSEnumDebugLevel.INFO);
            RspQryInvestorResponse response = ResponseTemplate<RspQryInvestorResponse>.SrvSendRspResponse(request);

            TrdClientInfo info = tl.GetClient(request.ClientID);

            response.TradingAccount = account.ID;
            response.Email = "xxxx@xxx.com";
            //查询交易帐户时,如果token为空则生成帐户名否则传递Token
            response.NickName = account.GetCustName();
                
            CacheRspResponse(response);
        }

        void SrvOnContribRequest(ContribRequest request,ISession session)
        {
            debug("Contrib Request:" + request.ToString(), QSEnumDebugLevel.INFO);
            TLCtxHelper.Ctx.MessageExchangeHandler(session, request);
        }

        /// <summary>
        /// 请求修改密码
        /// </summary>
        /// <param name="request"></param>
        void SrvOnReqChangePassword(ReqChangePasswordRequest request)
        {
            debug("ReqChangePassword:" + request.ToString(), QSEnumDebugLevel.INFO);

            RspReqChangePasswordResponse response = ResponseTemplate<RspReqChangePasswordResponse>.SrvSendRspResponse(request);
            bool valid = _clearcentre.VaildAccount(request.Account,request.OldPassword);
            if(!valid)
            {
                response.RspInfo.Fill("OLD_PASS_ERROR");
                CachePacket(response);
                return;
            }

            //修改密码返回
            _clearcentre.ChangeAccountPass(request.Account, request.NewPassword);
            CachePacket(response);
        }

        /// <summary>
        /// 查询通知
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryNotice(QryNoticeRequest request)
        {
            debug("QryNoticeRequest:" + request.ToString(), QSEnumDebugLevel.INFO);
            RspQryNoticeResponse response = ResponseTemplate<RspQryNoticeResponse>.SrvSendRspResponse(request);

            IAccount account = _clearcentre[request.Account];
            if (account != null)
            {
                if (account.Category == QSEnumAccountCategory.SIMULATION)
                {
                    response.NoticeContent = string.IsNullOrEmpty(GlobalConfig.SimPrompt) ? ("欢迎使用" + GlobalConfig.VendorName + "交易系统") : GlobalConfig.SimPrompt;
                }
                else if (account.Category == QSEnumAccountCategory.REAL)
                {
                    response.NoticeContent = string.IsNullOrEmpty(GlobalConfig.RealPrompt) ? ("欢迎使用" + GlobalConfig.VendorName + "交易系统,市场有风险,投资需谨慎!祝您交易愉快!") : GlobalConfig.RealPrompt;
                }
            }
            //如果通知内容为空 则提供默认提示
            if(string.IsNullOrEmpty(response.NoticeContent))
            {
                response.NoticeContent = string.IsNullOrEmpty(GlobalConfig.DealerPrompt)?("欢迎使用" + GlobalConfig.VendorName + "交易系统"):GlobalConfig.DealerPrompt;
            }

            RspQryNoticeResponse response0 = ResponseTemplate<RspQryNoticeResponse>.SrvSendRspResponse(request);
            //这里需要输出帐户特定的告知内容
            response0.NoticeContent = "测试内容"+System.Environment.NewLine;
            CacheRspResponse(response0, false);
            CacheRspResponse(response,true);

        }

        /// <summary>
        /// 查询合约
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQrySymbol(QrySymbolRequest request,IAccount account)
        {
            debug("QrySymbol:" + request.ToString(), QSEnumDebugLevel.INFO);
            Util.sleep(1000);
            Instrument[] instruments = new Instrument[]{};
            if (request.SecurityType != SecurityType.NIL && string.IsNullOrEmpty(request.ExchID) && string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.Security))
            {
                debug("qry instruments via security type", QSEnumDebugLevel.INFO);
                instruments =account.GetInstruments(request.SecurityType).ToArray();  
            }
            //如果所有字段为空 则为查询所有合约列表
            if (request.SecurityType == SecurityType.NIL && string.IsNullOrEmpty(request.ExchID) && string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.Security))
            {
                //debug("it is here b", QSEnumDebugLevel.INFO);
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
            debug("QryContractBank:" + request.ToString(), QSEnumDebugLevel.INFO);
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
        void SrvOnRegisterBankAccount(QryRegisterBankAccountRequest request)
        {
            debug("QryRegisterBankAccount:" + request.ToString(), QSEnumDebugLevel.INFO);
            IAccount account = _clearcentre[request.TradingAccount];
            if (account != null)
            {
                RspQryRegisterBankAccountResponse response = ResponseTemplate<RspQryRegisterBankAccountResponse>.SrvSendRspResponse(request);
                response.TradingAccount = account.ID;
                //如果对应的BankFK不为0 则传递设置的银行帐户信息
                if (account.BankID != 0)
                {
                    response.BankAC = account.BankAC;//获得银行卡号 如果没有设置银行卡号码 会导致博易客户端频繁请求交易帐号信息
                    ContractBank bank = BasicTracker.ContractBankTracker[account.BankID];
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
            debug("QryTransferSerialRequest:" + request.ToString(), QSEnumDebugLevel.INFO);
            IList<CashTransaction> cts = ORM.MAccount.SelectHistCashTransaction(request.TradingAccount, 0, 0);
            IAccount account = _clearcentre[request.TradingAccount];
            int totalnum = cts.Count;
            debug("total transfer num:" + totalnum.ToString(), QSEnumDebugLevel.INFO);
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspQryTransferSerialResponse response = ResponseTemplate<RspQryTransferSerialResponse>.SrvSendRspResponse(request);
                    CashTransaction t = cts[i];
                    response.Date = Util.ToTLDate(t.DateTime);
                    response.Time = Util.ToTLTime(t.DateTime);
                    response.TradingAccount = request.TradingAccount;
                    response.BankAccount = account.BankAC;
                    response.Amount = t.Amount;
                    response.TransRef = t.TransRef;
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
            debug("QryInstrumentCommissionRate:" + request.ToString(), QSEnumDebugLevel.INFO);
            
            //返回所有
            if (string.IsNullOrEmpty(request.Symbol))
            {
                //
            }
            else
            {
                RspQryInstrumentCommissionRateResponse response = ResponseTemplate<RspQryInstrumentCommissionRateResponse>.SrvSendRspResponse(request);
                Symbol sym = account.GetSymbol(request.Symbol);
                if (sym == null)
                {
                    response.RspInfo.Fill("SYMBOL_NOT_EXISTED");
                }
                CommissionConfig cfg = account.GetCommissionConfig(sym);
                response.FillCommissionCfg(cfg);
                CacheRspResponse(response);
            }
        }

        void SrvOnQryInstrumentMarginRate(QryInstrumentMarginRateRequest request, IAccount account)
        {
            debug("QryInstrumentMarginRate:" + request.ToString(), QSEnumDebugLevel.INFO);
            
            if (string.IsNullOrEmpty(request.Symbol))
            {

            }
            else 
            {
                RspQryInstrumentMarginRateResponse response = ResponseTemplate<RspQryInstrumentMarginRateResponse>.SrvSendRspResponse(request);
                Symbol sym = account.GetSymbol(request.Symbol);
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
        /// </summary>
        /// <param name="request"></param>
        /// <param name="account"></param>
        void SrvOnQryMarketData(QryMarketDataRequest request, IAccount account)
        {
            debug("QryMarketData:" + request.ToString(), QSEnumDebugLevel.INFO);

            if (string.IsNullOrEmpty(request.Symbol))
            {
                Tick[] ticks = mdtickmap.Values.ToArray();
                if (ticks.Length >= 1)
                {
                    for (int i = 0; i < ticks.Length; i++)
                    {
                        RspQryMarketDataResponse response = ResponseTemplate<RspQryMarketDataResponse>.SrvSendRspResponse(request);
                        response.TickToSend = ticks[i];
                        CacheRspResponse(response, i != ticks.Length - 1);
                    }
                }
                else
                {
                    //如果数据库没有报错上个交易日的市场数据 则生成空数据回报 否则飞迅客户端会无法登入
                    Symbol sym = account.Domain.GetSymbols().Where(s=>s.IsTradeable).FirstOrDefault();
                    if (sym != null)
                    {
                        Tick k = new TickImpl(sym.Symbol);
                        RspQryMarketDataResponse response = ResponseTemplate<RspQryMarketDataResponse>.SrvSendRspResponse(request);
                        response.TickToSend = k;
                        CacheRspResponse(response);
                    }
                    else
                    {
                        Util.Debug("帐户:" + account.ID + "所在域没有可交易合约,无法生成默认市场数据");
                    }
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
            debug("QryTradingParams:" + request.ToString(), QSEnumDebugLevel.INFO);
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
