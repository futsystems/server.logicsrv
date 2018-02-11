using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
using Common.Logging;

namespace FrontServer
{
    public class XLServiceHostBase
    {
        protected ILog logger = null;

        string _name = string.Empty;
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        public string Name { get { return _name; } }

        public XLServiceHostBase(string name)
        {
            _name = name;
            logger = LogManager.GetLogger(name);
        }

        public virtual void ForwardToBackend(string address, IPacket packet)
        { 
            
        }

        public virtual void ResponseXLPacket(IConnection conn,XLPacketData data, uint requestID, bool isLast)
        {
            //byte[] ret = XLPacketData.PackToBytes(data, XLEnumSeqType.SeqReq, this.NextSeqReqId, requestID, isLast);
            byte[] ret = XLPacketData.PackToBytes(data, XLEnumSeqType.SeqReq,0, requestID, isLast);
            conn.Send(ret);
        }

        public virtual byte[] GetResponseXLPacketData(XLPacketData data, uint requestID, bool isLast)
        {
            byte[] ret = XLPacketData.PackToBytes(data, XLEnumSeqType.SeqReq, 0, requestID, isLast);
            return ret;
        }

        public virtual void NotifyXLPacket(IConnection conn, XLPacketData data)
        {
            //byte[] ret = XLPacketData.PackToBytes(data, XLEnumSeqType.SeqRtn, this.NextSeqRtnId, 0, true);
            byte[] ret = XLPacketData.PackToBytes(data, XLEnumSeqType.SeqRtn,0, 0, true);
            conn.Send(ret);
        }

        public void HandleLogicMessage(IConnection conn, IPacket lpkt)
        {
            switch (lpkt.Type)
            {
                case MessageTypes.LOGINRESPONSE:
                    {
                        LoginResponse response = lpkt as LoginResponse;
                        //将数据转换成CTP业务结构体
                        XLRspLoginField field = new XLRspLoginField();
                        field.TradingDay = response.TradingDay;
                        field.UserID = response.LoginID;
                        field.Name = response.NickName;
                        field.Currency = XLConvert.ConvCurrencyType(response.Currency);



                        ErrorField rsp = XLConvert.ConvertRspInfo(response.RspInfo);

                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_LOGIN);
                        pkt.AddField(rsp);
                        pkt.AddField(field);


                        ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        if (response.RspInfo.ErrorID == 0)
                        {
                            conn.IState.Authorized = true;
                            conn.IState.LoginID = response.LoginID;
                        }
                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> LoginResponse", conn.SessionID));
                        break;
                    }
                case MessageTypes.CHANGEPASSRESPONSE:
                    {
                        RspReqChangePasswordResponse response = lpkt as RspReqChangePasswordResponse;

                        XLRspUserPasswordUpdateField field = new XLRspUserPasswordUpdateField();

                        //field.UserID = "";

                        ErrorField rsp = XLConvert.ConvertRspInfo(response.RspInfo);
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_UPDATEPASS);
                        pkt.AddField(rsp);
                        pkt.AddField(field);

                        ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspReqChangePasswordResponse", conn.SessionID));

                        break;
                    }
                //合约回报
                case MessageTypes.SYMBOLRESPONSE:
                    {
                        RspQrySymbolResponse response = lpkt as RspQrySymbolResponse;

                        if (response.InstrumentToSend != null)
                        {
                            XLSymbolField field = new XLSymbolField();
                            field.SymbolID = response.InstrumentToSend.Symbol;
                            field.ExchangeID = response.InstrumentToSend.ExchangeID;
                            field.SymbolName = response.InstrumentToSend.Name;
                            field.SecurityID = response.InstrumentToSend.Security;
                            field.SecurityType = XLConvert.ConvSecurityType(response.InstrumentToSend.SecurityType);
                            field.Multiple = response.InstrumentToSend.Multiple;
                            field.PriceTick = (double)response.InstrumentToSend.PriceTick;
                            field.ExpireDate = response.InstrumentToSend.ExpireDate.ToString();
                            field.Currency = XLConvert.ConvCurrencyType(response.InstrumentToSend.Currency);


                            XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_SYMBOL);
                            pkt.AddField(field);

                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQrySymbolResponse", conn.SessionID));

                        }
                        else
                        {
                            XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_SYMBOL);
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQrySymbolResponse", conn.SessionID));
                        }

                        break;

                    }
                //查询汇率回报
                case MessageTypes.XQRYEXCHANGERATERESPONSE:
                    {
                        RspXQryExchangeRateResponse response = lpkt as RspXQryExchangeRateResponse;

                        XLExchangeRateField field = new XLExchangeRateField();
                        field.TradingDay = response.ExchangeRate.Settleday;
                        field.IntermediateRate = (double)response.ExchangeRate.IntermediateRate;
                        field.Currency = XLConvert.ConvCurrencyType(response.ExchangeRate.Currency);

                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_EXCHANGE_RATE);
                        pkt.AddField(field);

                        ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXQryExchangeRateResponse", conn.SessionID));


                        break;
                    }
                //查询结算单回报
                case MessageTypes.XSETTLEINFORESPONSE:
                    {

                        RspXQrySettleInfoResponse response = lpkt as RspXQrySettleInfoResponse;
                        XLSettlementInfoField field = new XLSettlementInfoField();
                        field.TradingDay = response.Tradingday;
                        field.UserID = response.TradingAccount;
                        field.Content = response.SettlementContent;

                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_SETTLEINFO);
                        pkt.AddField(field);

                        ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        if (response.IsLast)
                        {
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXQryExchangeRateResponse", conn.SessionID));
                        }
                        break;
                    }
                //查询结算汇总回报
                case MessageTypes.XQRYSETTLESUMMAYRESPONSE:
                    {
                        RspXqrySettleSummaryResponse response = lpkt as RspXqrySettleSummaryResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_SETTLE_SUMMARY);

                        if (response.Settlement == null)
                        {
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        else
                        {
                            XLSettleSummaryField field = XLConvert.ConvSettleSummary(response.Settlement);
                            pkt.AddField(field);
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXqrySettleSummaryResponse", conn.SessionID));
                        break;
                    }
                //查询银行回报
                case MessageTypes.XQRYBANKRESPONSE:
                    {
                        RspXQryBankCardResponse response = lpkt as RspXQryBankCardResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_BANK);

                        if (response.BankCardInfo == null)
                        {
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        else
                        {
                            XLBankCardField field = XLConvert.ConvBankCard(response.BankCardInfo);
                            pkt.AddField(field);
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXQryBankCardResponse", conn.SessionID));
                        break;
                    }
                //更新银行回报
                case MessageTypes.XUPDATEBANKRESPONSE:
                    {
                        RspXReqUpdateBankCardResponse response = lpkt as RspXReqUpdateBankCardResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_UPDATE_BANK);

                        if (response.BankCardInfo == null)
                        {
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        else
                        {
                            XLBankCardField field = XLConvert.ConvBankCard(response.BankCardInfo);
                            pkt.AddField(field);
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXReqUpdateBankCardResponse", conn.SessionID));
                        break;
                    }
                //出入金操作回报
                case MessageTypes.XREQCASHOPRESPONSE:
                    {
                        RspXReqCashOperationResponse response = lpkt as RspXReqCashOperationResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_CASHOP);

                        ErrorField rsp = XLConvert.ConvertRspInfo(response.RspInfo);
                        pkt.AddField(rsp);
                        if (rsp.ErrorID == 0)
                        {
                            XLCashOperationField field = new XLCashOperationField();
                            field.Amount = (double)response.CashOperationRequest.Amount;
                            field.Args = response.CashOperationRequest.Args;
                            field.Gateway = response.CashOperationRequest.GateWay;
                            field.RefID = response.CashOperationRequest.RefID;
                            pkt.AddField(field);
                        }

                        ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXReqCashOperationResponse", conn.SessionID));
                        break;
                    }

                //查询委托回报
                case MessageTypes.XORDERRESPONSE:
                    {
                        RspXQryOrderResponse response = lpkt as RspXQryOrderResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_ORDER);

                        if (response.Order == null || !response.Order.isValid)
                        {
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        else
                        {
                            XLOrderField field = XLConvert.ConvOrder(response.Order);
                            pkt.AddField(field);
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXQryOrderResponse", conn.SessionID));
                        break;
                    }
                //委托实时通知
                case MessageTypes.ORDERNOTIFY:
                    {
                        OrderNotify notify = lpkt as OrderNotify;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RTN_ORDER);
                        if (notify.Order != null)
                        {
                            XLOrderField field = XLConvert.ConvOrder(notify.Order);
                            pkt.AddField(field);

                            NotifyXLPacket(conn,pkt);
                        }
                        else
                        {
                            logger.Warn("Order notify, order is null");
                        }
                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> OrderNotify", conn.SessionID));
                        break;
                    }
                //查询成交回报
                case MessageTypes.XTRADERESPONSE:
                    {
                        RspXQryTradeResponse response = lpkt as RspXQryTradeResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_TRADE);

                        if (response.Trade == null || !response.Trade.isValid)
                        {
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);


                        }
                        else
                        {
                            XLTradeField field = XLConvert.ConvTrade(response.Trade);
                            pkt.AddField(field);

                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXQryTradeResponse", conn.SessionID));
                        break;
                    }
                //成交实时通知
                case MessageTypes.EXECUTENOTIFY:
                    {
                        TradeNotify notify = lpkt as TradeNotify;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RTN_TRADE);

                        if (notify.Trade != null)
                        {
                            XLTradeField field = XLConvert.ConvTrade(notify.Trade);
                            pkt.AddField(field);

                            NotifyXLPacket(conn, pkt);
                        }
                        else
                        {
                            logger.Warn("Trade notify, trade is null");
                        }
                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> TradeNotify", conn.SessionID));
                        break;
                    }
                //查询持仓回报
                case MessageTypes.POSITIONRESPONSE:
                    {
                        RspQryPositionResponse response = lpkt as RspQryPositionResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_POSITION);

                        if (response.PositionToSend == null || !response.PositionToSend.IsValid)
                        {
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);

                        }
                        else
                        {
                            XLPositionField field = XLConvert.ConvPosition(response.PositionToSend);
                            pkt.AddField(field);

                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryPositionResponse", conn.SessionID));
                        break;
                    }
                //持仓实时更新
                case MessageTypes.POSITIONUPDATENOTIFY:
                    {
                        PositionNotify notify = lpkt as PositionNotify;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RTN_POSITIONUPDATE);

                        if (notify.Position != null)
                        {
                            XLPositionField field = XLConvert.ConvPosition(notify.Position);
                            pkt.AddField(field);

                            NotifyXLPacket(conn, pkt);
                        }
                        else
                        {
                            logger.Warn("Position notify, trade is null");
                        }
                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> PositionNotify", conn.SessionID));
                        break;
                    }
                //账户查询回报
                case MessageTypes.ACCOUNTINFORESPONSE:
                    {
                        RspQryAccountInfoResponse response = lpkt as RspQryAccountInfoResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_ACCOUNT);

                        XLTradingAccountField field = new XLTradingAccountField();
                        AccountInfo info = response.AccInfo;

                        field.PreCredit = (double)info.LastCredit;
                        field.Credit = (double)info.Credit;

                        field.PreEquity = (double)info.LastEquity;
                        field.Deposit = (double)info.CashIn;
                        field.Withdraw = (double)info.CashOut;
                        field.FrozenMargin = (double)info.FutMarginFrozen;
                        field.Margin = (double)info.FutMarginUsed;

                        field.Commission = (double)info.Commission;
                        field.CloseProfit = (double)info.RealizedPL;
                        field.PositionProfit = (double)info.UnRealizedPL;

                        field.NowEquity = (double)info.NowEquity;
                        field.Available = (double)info.AvabileFunds;//当前可用


                        pkt.AddField(field);
                        ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);

                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryAccountInfoResponse", conn.SessionID));
                        break;
                    }
                case MessageTypes.MAXORDERVOLRESPONSE:
                    {
                        RspQryMaxOrderVolResponse response = lpkt as RspQryMaxOrderVolResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_MAXORDVOL);
                        XLQryMaxOrderVolumeField field = new XLQryMaxOrderVolumeField();

                        field.Direction = response.Side ? XLDirectionType.Buy : XLDirectionType.Sell;
                        field.HedgeFlag = XLHedgeFlagType.Speculation;
                        field.SymbolID = response.Symbol;
                        field.MaxVolume = response.MaxVol;
                        field.OffsetFlag = XLConvert.ConvOffSet(response.OffsetFlag);

                        pkt.AddField(field);
                        ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);

                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryMaxOrderVolResponse", conn.SessionID));
                        break;
                    }
                //错误委托回报
                case MessageTypes.ERRORORDERNOTIFY:
                    {
                        ErrorOrderNotify notify = lpkt as ErrorOrderNotify;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_INSERTORDER);

                        XLInputOrderField field = new XLInputOrderField();
                        field.HedgeFlag = XLHedgeFlagType.Speculation;
                        field.OffsetFlag = XLConvert.ConvOffSet(notify.Order.OffsetFlag);
                        field.Direction = notify.Order.Side ? XLDirectionType.Buy : XLDirectionType.Sell;

                        field.SymbolID = notify.Order.Symbol;
                        field.UserID = notify.Order.Account;
                        field.OrderRef = notify.Order.OrderRef;

                        field.LimitPrice = (double)notify.Order.LimitPrice;
                        field.StopPrice = (double)notify.Order.StopPrice;
                        field.VolumeTotalOriginal = Math.Abs(notify.Order.TotalSize);
                        field.RequestID = notify.Order.RequestID;

                        if (field.LimitPrice == 0)
                        {
                            field.OrderType = XLOrderType.Market;
                        }
                        else
                        {
                            field.OrderType = XLOrderType.Limit;
                        }
                        ErrorField rsp = new ErrorField();
                        rsp.ErrorID = notify.RspInfo.ErrorID;
                        rsp.ErrorMsg = notify.RspInfo.ErrorMessage;

                        pkt.AddField(rsp);
                        pkt.AddField(field);

                        ResponseXLPacket(conn, pkt, (uint)notify.Order.RequestID, true);

                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> ErrorOrderNotify", conn.SessionID));
                        break;
                    }
                case MessageTypes.ERRORORDERACTIONNOTIFY:
                    {
                        ErrorOrderActionNotify notify = lpkt as ErrorOrderActionNotify;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_ORDERACTION);

                        XLInputOrderActionField field = new XLInputOrderActionField();
                        field.UserID = notify.OrderAction.Account;
                        field.ExchangeID = notify.OrderAction.Exchagne;
                        field.OrderSysID = notify.OrderAction.OrderExchID;

                        field.OrderID = notify.OrderAction.OrderID;
                        field.ActionFlag = notify.OrderAction.ActionFlag == QSEnumOrderActionFlag.Delete ? XLActionFlagType.Delete : XLActionFlagType.Modify;

                        field.RequestID = notify.OrderAction.RequestID;

                        ErrorField rsp = new ErrorField();
                        rsp.ErrorID = notify.RspInfo.ErrorID;
                        rsp.ErrorMsg = notify.RspInfo.ErrorMessage;

                        pkt.AddField(rsp);
                        pkt.AddField(field);

                        ResponseXLPacket(conn, pkt, (uint)notify.OrderAction.RequestID, true);

                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> ErrorOrderActionNotify", conn.SessionID));
                        break;
                    }
                //查询出入金回报
                case MessageTypes.XQRYCASHTXNRESPONSE:
                    {
                        RspXQryCashTransResponse response = lpkt as RspXQryCashTransResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_CASH_TXN);

                        if (response.CashTransaction == null)
                        {
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        else
                        {
                            XLCashTxnField field = XLConvert.ConvCashTxn(response.CashTransaction);
                            pkt.AddField(field);
                            ResponseXLPacket(conn, pkt, (uint)response.RequestID, response.IsLast);
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXQryCashTransResponse", conn.SessionID));
                        break;
                    }
                default:
                    logger.Warn(string.Format("Logic Packet:{0} not handled", lpkt.Type));
                    break;

            }
        }

        public void HandleXLPacketData(IConnection conn, XLPacketData pkt, int requestId)
        {
            if (!conn.IState.Authorized && pkt.MessageType != XLMessageType.T_REQ_LOGIN)
            {
                logger.Warn(string.Format("Session:{0} not authroized,only support T_REQ_LOGIN", conn.SessionID));
                XLPacketData response = new XLPacketData(XLMessageType.T_RSP_ERROR);
                ErrorField rsp = new ErrorField();
                rsp.ErrorID = 400;
                rsp.ErrorMsg = string.Format("Session未登入,无法执行操作:{0}", pkt.MessageType);
                response.AddField(rsp);

                ResponseXLPacket(conn, response, 0, true);
                return;
            }
            switch (pkt.MessageType)
            {
                //用户登入
                case XLMessageType.T_REQ_LOGIN:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLReqLoginField)
                        {
                            XLReqLoginField field = (XLReqLoginField)data;
                            LoginRequest request = RequestTemplate<LoginRequest>.CliSendRequest(requestId);
                            request.LoginID = field.UserID;
                            request.Passwd = field.Password;
                            request.MAC = field.MacAddress;
                            request.IPAddress = conn.IState.IPAddress;//field.ClientIPAddress;
                            request.LoginType = 1;
                            request.ProductInfo = field.UserProductInfo;
                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> ReqUserLogin", conn.SessionID));
                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //更新密码
                case XLMessageType.T_REQ_UPDATEPASS:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLReqUserPasswordUpdateField)
                        {
                            XLReqUserPasswordUpdateField field = (XLReqUserPasswordUpdateField)data;

                            ReqChangePasswordRequest request = RequestTemplate<ReqChangePasswordRequest>.CliSendRequest(requestId);

                            request.OldPassword = field.OldPassword;
                            request.NewPassword = field.NewPassword;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> ReqUserPasswordUpdate", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询合约
                case XLMessageType.T_QRY_SYMBOL:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQrySymbolField)
                        {
                            XLQrySymbolField field = (XLQrySymbolField)data;

                            QrySymbolRequest request = RequestTemplate<QrySymbolRequest>.CliSendRequest(requestId);

                            request.ExchID = field.ExchangeID;
                            request.Symbol = field.SymbolID;
                            request.Security = field.SecurityID;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> QrySymbolRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询汇率
                case XLMessageType.T_QRY_EXCHANGE_RATE:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQryExchangeRateField)
                        {
                            XLQryExchangeRateField field = (XLQryExchangeRateField)data;

                            XQryExchangeRateRequest request = RequestTemplate<XQryExchangeRateRequest>.CliSendRequest(requestId);

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XQryExchangeRateRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询结算单
                case XLMessageType.T_QRY_SETTLEINFO:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQrySettlementInfoField)
                        {
                            XLQrySettlementInfoField field = (XLQrySettlementInfoField)data;

                            XQrySettleInfoRequest request = RequestTemplate<XQrySettleInfoRequest>.CliSendRequest(requestId);
                            request.Tradingday = field.TradingDay;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XQrySettleInfoRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询结算汇总
                case XLMessageType.T_QRY_SETTLE_SUMMARY:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQrySettleSummaryField)
                        {
                            XLQrySettleSummaryField field = (XLQrySettleSummaryField)data;

                            XQrySettleSummaryRequest request = RequestTemplate<XQrySettleSummaryRequest>.CliSendRequest(requestId);
                            request.StartSettleday = field.StartSettleday;
                            request.EndSettleday = field.EndSettleday;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XLQrySettleSummaryField", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //请求出入金操作
                case XLMessageType.T_REQ_CASHOP:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLReqCashOperationField)
                        {
                            XLReqCashOperationField field = (XLReqCashOperationField)data;

                            XReqCashOperationRequest request = RequestTemplate<XReqCashOperationRequest>.CliSendRequest(requestId);
                            request.Amount = (decimal)field.Amount;
                            request.Args = field.Args;
                            request.Gateway = field.Gateway;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XLReqCashOperationField", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询委托
                case XLMessageType.T_QRY_ORDER:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQryOrderField)
                        {
                            XLQryOrderField field = (XLQryOrderField)data;

                            XQryOrderRequest request = RequestTemplate<XQryOrderRequest>.CliSendRequest(requestId);
                            request.Start = field.StartSettleday;
                            request.End = field.EndSettleday;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XQryOrderRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询成交
                case XLMessageType.T_QRY_TRADE:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQryTradeField)
                        {
                            XLQryTradeField field = (XLQryTradeField)data;

                            XQryTradeRequest request = RequestTemplate<XQryTradeRequest>.CliSendRequest(requestId);
                            request.Start = field.StartSettleday;
                            request.End = field.EndSettleday;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XQryTradeRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询持仓
                case XLMessageType.T_QRY_POSITION:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQryPositionField)
                        {
                            XLQryPositionField field = (XLQryPositionField)data;

                            QryPositionRequest request = RequestTemplate<QryPositionRequest>.CliSendRequest(requestId);
                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> QryPositionRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询交易账户
                case XLMessageType.T_QRY_ACCOUNT:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQryTradingAccountField)
                        {
                            XLQryTradingAccountField field = (XLQryTradingAccountField)data;

                            QryAccountInfoRequest request = RequestTemplate<QryAccountInfoRequest>.CliSendRequest(requestId);
                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> QryAccountInfoRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询最大报单数量
                case XLMessageType.T_QRY_MAXORDVOL:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQryMaxOrderVolumeField)
                        {
                            XLQryMaxOrderVolumeField field = (XLQryMaxOrderVolumeField)data;

                            QryMaxOrderVolRequest request = RequestTemplate<QryMaxOrderVolRequest>.CliSendRequest(requestId);
                            request.Side = field.Direction == XLDirectionType.Buy ? true : false;
                            request.Symbol = field.SymbolID;
                            //request.ex = field.ExchangeID;
                            request.OffsetFlag = XLConvert.ConvOffSet(field.OffsetFlag);

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> ReqQueryMaxOrderVolume", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //提交委托
                case XLMessageType.T_REQ_INSERTORDER:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLInputOrderField)
                        {
                            XLInputOrderField field = (XLInputOrderField)data;

                            OrderInsertRequest request = RequestTemplate<OrderInsertRequest>.CliSendRequest(requestId);

                            Order order = new OrderImpl();
                            order.Account = string.IsNullOrEmpty(field.UserID) ? conn.IState.LoginID : field.UserID;
                            order.Symbol = field.SymbolID;
                            order.Exchange = field.ExchangeID;

                            order.Side = field.Direction == XLDirectionType.Buy ? true : false;
                            order.TotalSize = field.VolumeTotalOriginal;
                            order.Size = order.TotalSize;

                            if (field.OrderType == XLOrderType.Market)
                            {
                                order.LimitPrice = 0;
                                order.StopPrice = 0;
                            }
                            else if (field.OrderType == XLOrderType.Limit)
                            {
                                order.LimitPrice = (decimal)field.LimitPrice;
                                order.StopPrice = 0;
                            }

                            order.TimeInForce = QSEnumTimeInForce.DAY;
                            order.Currency = CurrencyType.RMB;
                            order.OrderRef = field.OrderRef;
                            order.RequestID = field.RequestID;

                            order.HedgeFlag = QSEnumHedgeFlag.Speculation;
                            order.OffsetFlag = XLConvert.ConvOffSet(field.OffsetFlag);


                            request.Order = order;
                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> ReqQueryMaxOrderVolume", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //提交委托操作
                case XLMessageType.T_REQ_ORDERACTION:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLInputOrderActionField)
                        {
                            XLInputOrderActionField field = (XLInputOrderActionField)data;

                            OrderActionRequest request = RequestTemplate<OrderActionRequest>.CliSendRequest(requestId);

                            OrderAction action = new OrderActionImpl();
                            action.Account = string.IsNullOrEmpty(field.UserID) ? conn.IState.LoginID : field.UserID;
                            action.ActionFlag = QSEnumOrderActionFlag.Delete;

                            action.Exchagne = field.ExchangeID;
                            action.OrderExchID = field.OrderSysID;

                            action.OrderID = field.OrderID;

                            request.OrderAction = action;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> OrderActionRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                //查询出入金记录
                case XLMessageType.T_QRY_CASH_TXN:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQryCashTxnField)
                        {
                            XLQryCashTxnField field = (XLQryCashTxnField)data;

                            XQryCashTransRequest request = RequestTemplate<XQryCashTransRequest>.CliSendRequest(requestId);
                            request.Start = field.StartSettleday;
                            request.End = field.EndSettleday;

                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XQryCashTransRequest", conn.SessionID));

                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                case XLMessageType.T_QRY_BANK:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLQryBankCardField)
                        {
                            XLQryBankCardField field = (XLQryBankCardField)data;

                            XQryBankCardRequest request = RequestTemplate<XQryBankCardRequest>.CliSendRequest(requestId);
                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XQryBankCardRequest", conn.SessionID));
                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                case XLMessageType.T_REQ_UPDATE_BANK:
                    {
                        var data = pkt.FieldList[0];
                        if (data is XLReqUpdateBankCardField)
                        {
                            XLReqUpdateBankCardField field = (XLReqUpdateBankCardField)data;

                            XReqUpdateBankCardRequest request = RequestTemplate<XReqUpdateBankCardRequest>.CliSendRequest(requestId);
                            request.BankCardInfo = XLConvert.ConvBankCard(field);
                            this.ForwardToBackend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> XReqUpdateBankCardRequest", conn.SessionID));
                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", pkt.MessageType));
                        }
                        break;
                    }
                default:
                    logger.Warn(string.Format("Packet:{0} logic not handled", pkt.MessageType));
                    break;
            }
        }
    }
}
