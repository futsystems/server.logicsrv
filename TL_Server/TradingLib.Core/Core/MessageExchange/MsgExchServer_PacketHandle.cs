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
        void CacheRspResponse(RspResponsePacket packet,bool islat=true)
        {
            packet.IsLast = islat;
            if (!prioritybuffer.Write(packet))//如果该回报不是由优先缓存发送，则写入常规缓存
            {
                CachePacket(packet);
            }
            else
            {
                debug("packet:"+packet.ToString()+"写入优先缓存",QSEnumDebugLevel.INFO);
            }
            //if (packet is RspQryInvestorResponse)
            //{
            //    investorbuf.Write(packet as RspQryInvestorResponse);
            //}
            //else if (packet is RspQrySymbolResponse)
            //{
            //    symbolbuf.Write(packet as RspQrySymbolResponse);
            //}
            //else
            //{
            //    CachePacket(packet);
            //}
        }
        /// <summary>
        /// 查询委托
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryOrder(QryOrderRequest request,IAccount account)
        {
            debug("QryOrder :" + request.ToString(), QSEnumDebugLevel.INFO);
            Order[] orders = new Order[]{};
            //IAccount account = _clearcentre[request.Account];


            //合约为空 查询所有
            if (string.IsNullOrEmpty(request.Symbol))
            {
                orders = account.Orders.ToArray();
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

                    debug("转发当日成交:" + trades[i].ToString() + " side:" + trades[i].Side.ToString(), QSEnumDebugLevel.INFO);
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

            debug("total num:" + positions.Length.ToString(), QSEnumDebugLevel.INFO);
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
                int size = account.CanOpenSize(symbol);
                debug("got max opensize:" + size.ToString(), QSEnumDebugLevel.INFO);
                response.Symbol = request.Symbol;
                response.MaxVol = size;
                response.OffsetFlag = request.OffsetFlag;
                request.Side = request.Side;
                CachePacket(response);
            }
        }


        void SrvOnQryAccountInfo(QryAccountInfoRequest request, IAccount account)
        {
            debug("QryAccountInfo :" + request.ToString(), QSEnumDebugLevel.INFO);
            IAccountInfo info = account.ToAccountInfo();
            RspQryAccountInfoResponse response  = ResponseTemplate<RspQryAccountInfoResponse>.SrvSendRspResponse(request);
            response.AccInfo = info;
            Util.sleep(2000);
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
                response.RspInfo.Fill("SELLTEINFO_NOT_FOUND");
                CachePacket(response);
            }
        }

        void SrvOnQrySettleInfoConfirm(QrySettleInfoConfirmRequest request)
        {
            debug("QrySettleInfoConfirm :" + request.ToString(), QSEnumDebugLevel.INFO);
            RspQrySettleInfoConfirmResponse response = ResponseTemplate<RspQrySettleInfoConfirmResponse>.SrvSendRspResponse(request);
            IAccount account = _clearcentre[request.Account];
            debug("confirm stamp:" + account.SettlementConfirmTimeStamp.ToString(), QSEnumDebugLevel.INFO);

           
            response.TradingAccount = request.Account;
            if (needConfirmSettlement)
            {
                //如果结算时间戳为0 则表明为新注册用户,新注册用户不用注册
                DateTime confirmtime = Util.ToDateTime(account.SettlementConfirmTimeStamp);
                response.ConfirmDay = Util.ToTLDate(confirmtime);
                response.ConfirmTime = Util.ToTLTime(confirmtime);
            }
            else
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
            CachePacket(response);

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
                debug("it is here a", QSEnumDebugLevel.INFO);
                instruments =account.GetInstruments(request.SecurityType).ToArray();  
            }
            //如果所有字段为空 则为查询所有合约列表
            if (request.SecurityType == SecurityType.NIL && string.IsNullOrEmpty(request.ExchID) && string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.Security))
            {
                debug("it is here b", QSEnumDebugLevel.INFO);
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
            debug("QryInstrumentCommissionRate:" + request.ToString(), QSEnumDebugLevel.DEBUG);
            
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
            debug("QryInstrumentMarginRate:" + request.ToString(), QSEnumDebugLevel.DEBUG);
            
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
                    sym.FillSymbolMarginResponse(ref response);
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
            debug("QryMarketData:" + request.ToString(), QSEnumDebugLevel.DEBUG);

            if (string.IsNullOrEmpty(request.Symbol))
            {
                Tick[] ticks = _datafeedRouter.GetTickSnapshot();
                for (int i = 0; i < ticks.Length; i++)
                {
                    RspQryMarketDataResponse response = ResponseTemplate<RspQryMarketDataResponse>.SrvSendRspResponse(request);
                    response.TickToSend = ticks[i];
                    CacheRspResponse(response, i != ticks.Length - 1);
                }
            }
            
        }
        void tl_newPacketRequest(IPacket packet,ISession session)
        {
            IAccount account = _clearcentre[session.AccountID]; //获得对应的IAccount对象
            switch (packet.Type)
            {

                case MessageTypes.QRYORDER://查询委托
                    { 
                        QryOrderRequest request = packet as QryOrderRequest;
                        SrvOnQryOrder(request,account);
                    }
                    break;
                case MessageTypes.QRYTRADE://查询成交
                    {
                        QryTradeRequest request = packet as QryTradeRequest;
                        SrvOnQryTrade(request,account);
                    }
                    break;
                case MessageTypes.QRYPOSITION://查询持仓
                    {
                        QryPositionRequest request = packet as QryPositionRequest;
                        SrvOnQryPosition(request, account);
                    }
                    break;
                case MessageTypes.QRYPOSITIONDETAIL://查询持仓明细
                    {
                        QryPositionDetailRequest request = packet as QryPositionDetailRequest;
                        SrvOnQryPositionDetail(request, account);
                    }
                    break;
                case MessageTypes.QRYACCOUNTINFO://查询帐户信息
                    {
                        QryAccountInfoRequest request = packet as QryAccountInfoRequest;
                        SrvOnQryAccountInfo(request, account);
                    }
                    break;
                case MessageTypes.QRYMAXORDERVOL://查询委托可开手数
                    {
                        QryMaxOrderVolRequest request = packet as QryMaxOrderVolRequest;
                        SrvOnQryMaxOrderVol(request,account);
                    }
                    break;
                case MessageTypes.QRYSETTLEINFO://查询结算信息
                    {
                        QrySettleInfoRequest request = packet as QrySettleInfoRequest;
                        SrvOnQrySettleInfo(request);
                    }
                    break;
                case MessageTypes.QRYSETTLEINFOCONFIRM://查询结算确认
                    {
                        QrySettleInfoConfirmRequest request = packet as QrySettleInfoConfirmRequest;
                        SrvOnQrySettleInfoConfirm(request);
                    }
                    break;
                case MessageTypes.CONFIRMSETTLEMENT://确认结算信息
                    {
                        ConfirmSettlementRequest request = packet as ConfirmSettlementRequest;
                        SrvOnConfirmSettlement(request);
                    }
                    break;
                case MessageTypes.QRYINVESTOR://查询投资者信息
                    {
                        QryInvestorRequest request = packet as QryInvestorRequest;
                        SrvOnQryInvestor(request, account);
                    }
                    break;
                case MessageTypes.REQCHANGEPASS://请求修改交易帐户密码
                    {
                        ReqChangePasswordRequest request = packet as ReqChangePasswordRequest;
                        SrvOnReqChangePassword(request);
                    }
                    break;
                case MessageTypes.QRYNOTICE://请求查询交易系统通知
                    {
                        QryNoticeRequest request = packet as QryNoticeRequest;
                        SrvOnQryNotice(request);
                    }
                    break;
                case MessageTypes.QRYSYMBOL://查询合约列表
                    {
                        QrySymbolRequest request = packet as QrySymbolRequest;
                        SrvOnQrySymbol(request, account);
                    }
                    break;
                case MessageTypes.QRYCONTRACTBANK://查询签约银行
                    {
                        QryContractBankRequest request = packet as QryContractBankRequest;
                        SrvOnQryContractBank(request);
                    }
                    break;
                case MessageTypes.QRYREGISTERBANKACCOUNT://查询银行帐户
                    {
                        //debug("查询银行帐户.............", QSEnumDebugLevel.INFO);
                        QryRegisterBankAccountRequest request = packet as QryRegisterBankAccountRequest;
                        SrvOnRegisterBankAccount(request);
                    }
                    break;
                case MessageTypes.QRYTRANSFERSERIAL://查询出入金记录
                    {
                        QryTransferSerialRequest request = packet as QryTransferSerialRequest;
                        SrvOnQryTransferSerial(request);
                    }
                    break;
                case MessageTypes.QRYINSTRUMENTCOMMISSIONRATE://查询合约手续费率
                    {
                        QryInstrumentCommissionRateRequest request = packet as QryInstrumentCommissionRateRequest;
                        SrvOnQryInstrumentCommissionRate(request, account);
                    }
                    break;
                case MessageTypes.QRYINSTRUMENTMARGINRATE://查询合约保证金率
                    {
                        QryInstrumentMarginRateRequest request = packet as QryInstrumentMarginRateRequest;
                        SrvOnQryInstrumentMarginRate(request,account);
                    }
                    break;
                case MessageTypes.QRYMARKETDATA://查询市场行情
                    {
                        QryMarketDataRequest request = packet as QryMarketDataRequest;
                        SrvOnQryMarketData(request, account);
                    }
                    break;
                case MessageTypes.CONTRIBREQUEST://扩展请求
                    {
                        ContribRequest request = packet as ContribRequest;
                        session.ContirbID = request.ModuleID;
                        session.CMDStr = request.CMDStr;
                        session.RequestID = request.RequestID;

                        SrvOnContribRequest(request, session);
                    }
                    break;
                
                default:
                    debug("packet:"+packet.ToString() +" can not be handled",QSEnumDebugLevel.WARNING);
                    break;
            }
        }

    }
}
