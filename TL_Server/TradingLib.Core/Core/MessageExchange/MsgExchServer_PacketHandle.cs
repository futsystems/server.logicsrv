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
            CachePacket(packet);
        }
        /// <summary>
        /// 查询委托
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryOrder(QryOrderRequest request)
        {
            debug("QryOrder :" + request.ToString(), QSEnumDebugLevel.INFO);
            Order[] orders = new Order[]{};
            //合约为空 查询所有
            if (string.IsNullOrEmpty(request.Symbol))
            {
                string account = request.Account;
                orders = _clearcentre.getOrders(account);
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
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryTrade(QryTradeRequest request)
        {
            debug("QryTrade :" + request.ToString(), QSEnumDebugLevel.INFO);
            Trade[] trades = new Trade[] { };
            if (string.IsNullOrEmpty(request.Symbol))
            {
                string account = request.Account;
                trades = _clearcentre.getTrades(account);
                
            }

            int totalnum = trades.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspQryTradeResponse response = ResponseTemplate<RspQryTradeResponse>.SrvSendRspResponse(request);
                    response.TradeToSend = trades[i];

                    debug("转发当日成交:" + trades[i].ToString() + " side:" + trades[i].side.ToString(), QSEnumDebugLevel.INFO);
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
        void SrvOnQryPosition(QryPositionRequest request)
        {
            debug("QryPosition :" + request.ToString(), QSEnumDebugLevel.INFO);
            Position[] positions = new Position[] { };
            if (string.IsNullOrEmpty(request.Symbol))
            {
                string account = request.Account;
                positions = _clearcentre.getPositions(account);
            }
            if (!string.IsNullOrEmpty(request.Symbol))
            {
                positions = positions.Where(pos => pos.Symbol.Equals(request.Symbol)).ToArray();
            }

            debug("total num:" + positions.Length.ToString(), QSEnumDebugLevel.INFO);
            int totalnum = positions.Length;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspQryPositionResponse response = ResponseTemplate<RspQryPositionResponse>.SrvSendRspResponse(request);
                    response.PositionToSend = AccountPosition.GenFromPosition(positions[i]);
                    CacheRspResponse(response, i == totalnum - 1);
                }
            }
            else
            {
                RspQryPositionResponse response = ResponseTemplate<RspQryPositionResponse>.SrvSendRspResponse(request);
                response.PositionToSend = new AccountPosition();
                CacheRspResponse(response);
            }
        }

        /// <summary>
        /// 查询可开手数
        /// </summary>
        /// <param name="request"></param>
        void SrvOnQryMaxOrderVol(QryMaxOrderVolRequest request)
        {
            debug("QryMaxOrderVol :" + request.ToString(), QSEnumDebugLevel.INFO);
            
            IAccount account = _clearcentre[request.Account];
            Symbol symbol = BasicTracker.SymbolTracker[request.Symbol];
            RspQryMaxOrderVolResponse response = ResponseTemplate<RspQryMaxOrderVolResponse>.SrvSendRspResponse(request);
            if(symbol == null)
            {
                response.RspInfo.FillError("SYMBOL_NOT_EXISTED");
                CachePacket(response);
            }
            if (account == null)
            {
                response.RspInfo.FillError("TRADING_ACCOUNT_NOT_FOUND");
                CachePacket(response);
            }
            else
            {
                int size = account.CanOpenSize(symbol);
                response.Symbol = request.Symbol;
                response.MaxVol = size;
                response.PostFlag = request.PostFlag;
                request.Side = request.Side;
                CachePacket(response);
            }
        }


        void SrvOnQryAccountInfo(QryAccountInfoRequest request)
        {
            debug("QryAccountInfo :" + request.ToString(), QSEnumDebugLevel.INFO);
            IAccount account = _clearcentre[request.Account];
            IAccountInfo info = ObjectInfoHelper.GenAccountInfo(account);
            RspQryAccountInfoResponse response  = ResponseTemplate<RspQryAccountInfoResponse>.SrvSendRspResponse(request);
            response.AccInfo = info;

            CachePacket(response);
        }

        void SrvOnQrySettleInfo(QrySettleInfoRequest request)
        {
            debug("QrySettleInfo :" + request.ToString(), QSEnumDebugLevel.INFO);

            Settlement settlement = null;
            
            if (request.Tradingday == 0)
            {
                settlement = ORM.MSettlement.SelectSettlementInfoUnconfirmed(request.Account);
            }
            else
            {
                settlement = ORM.MSettlement.SelectSettlementInfoUnconfirmed(request.Account, request.Tradingday);
            }
            if (settlement != null)
            {
                string s = TemplateHelper.RenderSettlementInfo(new SettlementInfo(settlement));
                string[] lines = s.Split('\n');
                debug("got lines:" + lines.Length.ToString(), QSEnumDebugLevel.INFO);
                for(int i=0;i<lines.Length;i++)
                {
                    RspQrySettleInfoResponse response = ResponseTemplate<RspQrySettleInfoResponse>.SrvSendRspResponse(request);
                    response.Tradingday = settlement.SettleDay;
                    response.TradingAccount = settlement.Account;
                    response.SettlementID = 1000;
                    response.SequenceNo = i+1;
                    response.SettlementContent = lines[i];
                    CacheRspResponse(response, i == lines.Length - 1);
                }
                
            }
            else
            {
                RspQrySettleInfoResponse response = ResponseTemplate<RspQrySettleInfoResponse>.SrvSendRspResponse(request);
                debug("can not find settlement for account:" + request.Account + " for settleday:" + request.Tradingday.ToString(), QSEnumDebugLevel.WARNING);
                response.RspInfo.FillError("SELLTEINFO_NOT_FOUND");
                CachePacket(response);
            }
        }

        void SrvOnQrySettleInfoConfirm(QrySettleInfoConfirmRequest request)
        {
            debug("QrySettleInfoConfirm :" + request.ToString(), QSEnumDebugLevel.INFO);
            RspQrySettleInfoConfirmResponse response = ResponseTemplate<RspQrySettleInfoConfirmResponse>.SrvSendRspResponse(request);
            IAccount account = _clearcentre[request.Account];
            debug("confirm stamp:" + account.SettlementConfirmTimeStamp.ToString(), QSEnumDebugLevel.INFO);

            //如果结算时间戳为0 则表明为新注册用户,新注册用户不用注册
            DateTime confirmtime = Util.ToDateTime(account.SettlementConfirmTimeStamp);
            response.TradingAccount = request.Account;
            response.ConfirmDay = Util.ToTLDate(confirmtime);
            response.ConfirmTime = Util.ToTLTime(confirmtime);

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
        void SrvOnQryInvestor(QryInvestorRequest request)
        {
            debug("QryInvestor :" + request.ToString(), QSEnumDebugLevel.INFO);
            RspQryInvestorResponse response = ResponseTemplate<RspQryInvestorResponse>.SrvSendRspResponse(request);
            IAccount account = _clearcentre[request.Account];
            if (account != null)
            {
                response.TradingAccount = request.Account;
                response.Email = "xxxx@xxx.com";
                //查询交易帐户时,如果token为空则生成帐户名否则传递Token
                response.NickName = string.IsNullOrEmpty(account.Token)?LibUtil.GetEnumDescription(account.Category) + "[" + request.Account + "]" : account.Token;
                
            }
            else
            {
                response.RspInfo.FillError("TRADING_ACCOUNT_NOT_FOUND");
               
            }
            CachePacket(response);
        }

        void SrvOnContribRequest(ContribRequest request,ISession session)
        {
            debug("Contrib Request:" + request.ToString(), QSEnumDebugLevel.INFO);
            TLCtxHelper.Ctx.MessageExchangeHandler(session, request);
        }

        void SrvOnReqChangePassword(ReqChangePasswordRequest request)
        {
            debug("ReqChangePassword:" + request.ToString(), QSEnumDebugLevel.INFO);

            RspReqChangePasswordResponse response = ResponseTemplate<RspReqChangePasswordResponse>.SrvSendRspResponse(request);
            bool valid = _clearcentre.VaildAccount(request.Account,request.OldPassword);
            if(!valid)
            {
                response.RspInfo.FillError("OLD_PASS_ERROR");
                CachePacket(response);
                return;
            }

            //修改密码返回
            _clearcentre.ChangeAccountPass(request.Account, request.NewPassword);
            CachePacket(response);
        }

        void SrvOnQryNotice(QryNoticeRequest request)
        {
            debug("QryNoticeRequest:" + request.ToString(), QSEnumDebugLevel.INFO);
            RspQryNoticeResponse response = ResponseTemplate<RspQryNoticeResponse>.SrvSendRspResponse(request);

            IAccount account = _clearcentre[request.Account];
            if (account != null)
            {
                if (account.Category == QSEnumAccountCategory.SIMULATION)
                {
                    response.NoticeContent = string.IsNullOrEmpty(GlobalConfig.SimPrompt) ? ("欢迎使用" + GlobalConfig.VendorName + "模拟交易系统,部署环境:模拟") : GlobalConfig.SimPrompt;
                }
                else if (account.Category == QSEnumAccountCategory.REAL)
                {
                    response.NoticeContent = string.IsNullOrEmpty(GlobalConfig.RealPrompt) ? ("欢迎使用" + GlobalConfig.VendorName + "实盘交易系统,市场有风险,投资需谨慎!祝您交易愉快!") : GlobalConfig.RealPrompt;
                }
            }
            else
            {
                response.NoticeContent = string.IsNullOrEmpty(GlobalConfig.DealerPrompt)?("欢迎使用" + GlobalConfig.VendorName + "交易系统"):GlobalConfig.DealerPrompt;
            }
            CachePacket(response);

        }

        void SrvOnQrySymbol(QrySymbolRequest request)
        {
            debug("QrySymbol:" + request.ToString(), QSEnumDebugLevel.INFO);
            Instrument[] instruments = new Instrument[]{};
            if (request.SecurityType != SecurityType.NIL && string.IsNullOrEmpty(request.ExchID) && string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.Security))
            {
                instruments = BasicTracker.SymbolTracker.GetInstrumentByType(request.SecurityType).Where(s=>s.Tradeable).ToArray();  
            }
            //如果所有字段为空 则为查询所有合约列表
            if (request.SecurityType == SecurityType.NIL && string.IsNullOrEmpty(request.ExchID) && string.IsNullOrEmpty(request.Symbol) && string.IsNullOrEmpty(request.Security))
            {
                instruments  = BasicTracker.SymbolTracker.GetAllInstrument();
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
        void tl_newPacketRequest(IPacket packet,ISession session)
        {

            switch (packet.Type)
            {

                case MessageTypes.QRYORDER://查询委托
                    { 
                        QryOrderRequest request = packet as QryOrderRequest;
                        SrvOnQryOrder(request);
                    }
                    break;
                case MessageTypes.QRYTRADE://查询成交
                    {
                        QryTradeRequest request = packet as QryTradeRequest;
                        SrvOnQryTrade(request);
                    }
                    break;
                case MessageTypes.QRYPOSITION://查询持仓
                    {
                        QryPositionRequest request = packet as QryPositionRequest;
                        SrvOnQryPosition(request);
                    }
                    break;
                case MessageTypes.QRYACCOUNTINFO://查询帐户信息
                    {
                        QryAccountInfoRequest request = packet as QryAccountInfoRequest;
                        SrvOnQryAccountInfo(request);
                    }
                    break;
                case MessageTypes.QRYMAXORDERVOL://查询委托可开手数
                    {
                        QryMaxOrderVolRequest request = packet as QryMaxOrderVolRequest;
                        SrvOnQryMaxOrderVol(request);
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
                        SrvOnQryInvestor(request);
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
                        SrvOnQrySymbol(request);
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
