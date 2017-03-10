using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using ZeroMQ;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
using Common.Logging;


namespace FrontServer
{
    /// <summary>
    /// XLProtocol 交易协议处理
    /// XLProtocol可以通过二进制或Json与服务端交互
    /// 客户端通过Connection提交上来的数据包解析成XLPacketData 然后由HandleXLPacketData进行处理 将数据转换成内部TLMessage的格式通过传递到后端逻辑服务器
    /// 服务端返回过来的消息 转换成XLPacketData 并对外发送
    /// </summary>
    public partial class MQServer
    {


        void HandleLogicMessage(IConnection conn, IPacket lpkt)
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

                        ErrorField rsp = ConvertRspInfo(response.RspInfo);

                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_LOGIN);
                        pkt.AddField(rsp);
                        pkt.AddField(field);


                        conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
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

                        ErrorField rsp = ConvertRspInfo(response.RspInfo);
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_UPDATEPASS);
                        pkt.AddField(rsp);
                        pkt.AddField(field);

                        conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
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
                            field.SecurityType = ConvSecurityType(response.InstrumentToSend.SecurityType);
                            field.Multiple = response.InstrumentToSend.Multiple;
                            field.PriceTick = (double)response.InstrumentToSend.PriceTick;
                            field.ExpireDate = response.InstrumentToSend.ExpireDate.ToString();
                            field.Currency = ConvCurrencyType(response.InstrumentToSend.Currency);


                            XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_SYMBOL);
                            pkt.AddField(field);

                            conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQrySymbolResponse", conn.SessionID));

                        }
                        else
                        {
                            XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_SYMBOL);
                            conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQrySymbolResponse", conn.SessionID));
                        }

                        break;

                    }
                //查询委托回报
                case MessageTypes.ORDERRESPONSE:
                    {
                        RspQryOrderResponse response = lpkt as RspQryOrderResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_ORDER);

                        if (response.OrderToSend == null || !response.OrderToSend.isValid)
                        {
                            conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
                        }
                        else
                        {
                            XLOrderField field = ConvOrder(response.OrderToSend);
                            pkt.AddField(field);

                            conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);

                            
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryOrderResponse", conn.SessionID));
                        break;
                    }
                //委托实时通知
                case MessageTypes.ORDERNOTIFY:
                    {
                        OrderNotify notify = lpkt as OrderNotify;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RTN_ORDER);
                        if (notify.Order != null)
                        {
                            XLOrderField field = ConvOrder(notify.Order);
                            pkt.AddField(field);

                            conn.NotifyXLPacket(pkt);
                        }
                        else
                        {
                            logger.Warn("Order notify, order is null");
                        }
                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> OrderNotify", conn.SessionID));
                        break;
                    }
                //查询成交回报
                case MessageTypes.TRADERESPONSE:
                    {
                        RspQryTradeResponse response = lpkt as RspQryTradeResponse;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_TRADE);

                        if (response.TradeToSend == null || !response.TradeToSend.isValid)
                        {
                            conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);

                            
                        }
                        else
                        {
                            XLTradeField field = ConvTrade(response.TradeToSend);
                            pkt.AddField(field);

                            conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
                        }
                        if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryTradeResponse", conn.SessionID));
                        break;
                    }
                //成交实时通知
                case MessageTypes.EXECUTENOTIFY:
                    {
                        TradeNotify notify = lpkt as TradeNotify;
                        XLPacketData pkt = new XLPacketData(XLMessageType.T_RTN_TRADE);

                        if (notify.Trade != null)
                        {
                            XLTradeField field = ConvTrade(notify.Trade);
                            pkt.AddField(field);

                            conn.NotifyXLPacket(pkt);
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
                            conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
                            
                        }
                        else
                        {
                            XLPositionField field = ConvPosition(response.PositionToSend);
                            pkt.AddField(field);

                            conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);
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
                            XLPositionField field = ConvPosition(notify.Position);
                            pkt.AddField(field);

                            conn.NotifyXLPacket(pkt);
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
                        conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);

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
                        field.OffsetFlag = ConvOffSet(response.OffsetFlag);

                        pkt.AddField(field);
                        conn.ResponseXLPacket(pkt, (uint)response.RequestID, response.IsLast);

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
                        field.OffsetFlag = ConvOffSet(notify.Order.OffsetFlag);
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

                        conn.ResponseXLPacket(pkt, (uint)notify.Order.RequestID, true);

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

                        conn.ResponseXLPacket(pkt, (uint)notify.OrderAction.RequestID, true);

                        logger.Info(string.Format("LogicSrv Reply Session:{0} -> ErrorOrderActionNotify", conn.SessionID));
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

                conn.ResponseXLPacket(response, 0, true);
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
                            request.IPAddress = field.ClientIPAddress;
                            request.LoginType = 1;
                            request.ProductInfo = field.UserProductInfo;
                            this.TLSend(conn.SessionID, request);
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

                            this.TLSend(conn.SessionID, request);
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

                            this.TLSend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> QrySymbolRequest", conn.SessionID));

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

                            QryOrderRequest request = RequestTemplate<QryOrderRequest>.CliSendRequest(requestId);
                            this.TLSend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> QryOrderRequest", conn.SessionID));

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

                            QryTradeRequest request = RequestTemplate<QryTradeRequest>.CliSendRequest(requestId);
                            this.TLSend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> QryTradeRequest", conn.SessionID));

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
                            this.TLSend(conn.SessionID, request);
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
                            this.TLSend(conn.SessionID, request);
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
                            request.OffsetFlag = ConvOffSet(field.OffsetFlag);

                            this.TLSend(conn.SessionID, request);
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
                            order.OffsetFlag = ConvOffSet(field.OffsetFlag);


                            request.Order = order;
                            this.TLSend(conn.SessionID, request);
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

                            this.TLSend(conn.SessionID, request);
                            logger.Info(string.Format("Session:{0} >> OrderActionRequest", conn.SessionID));

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

        /// <summary>
        /// 转换合约类别
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        XLSecurityType ConvSecurityType(SecurityType type)
        {
            switch (type)
            {
                case SecurityType.FUT: return XLSecurityType.Future;
                case SecurityType.STK: return XLSecurityType.STK;
                default:
                    return XLSecurityType.Future;
            }
        }

        XLPositionField ConvPosition(PositionEx pos)
        {
            XLPositionField p = new XLPositionField();

            p.TradingDay = pos.Tradingday.ToString();
            p.UserID = pos.Account;
            p.SymbolID = pos.Symbol;
            p.ExchangeID = pos.Exchange;
            p.PosiDirection = pos.Side ? XLPosiDirectionType.Long : XLPosiDirectionType.Short;
            p.HedgeFlag = XLHedgeFlagType.Speculation;
            p.YdPosition = pos.YdPosition;
            p.Position = pos.Position;
            p.TodayPosition = pos.TodayPosition;
            p.OpenVolume = pos.OpenVolume;
            p.CloseVolume = pos.CloseVolume;
            p.OpenCost = (double)pos.OpenCost;
            p.PositionCost = (double)pos.PositionCost;
            p.Margin = (double)pos.Margin;
            p.CloseProfit = (double)pos.CloseProfit;
            p.PositionProfit = (double)pos.UnRealizedProfit;
            p.Commission = (double)pos.Commission;
            return p;

        }
        XLTradeField ConvTrade(Trade trade)
        {
            XLTradeField f = new XLTradeField();
            DateTime dt = Util.ToDateTime(trade.xDate, trade.xTime);
            f.TradingDay = trade.SettleDay.ToString();
            f.Date = trade.xDate.ToString();
            f.Time = dt.ToString("HH:mm:ss");
            f.UserID = trade.Account;
            f.SymbolID = trade.Symbol;
            f.ExchangeID = trade.Exchange;
            f.OrderRef = trade.OrderRef;
            f.OrderSysID = trade.OrderSysID;
            f.TradeID = trade.TradeID;
            f.Direction = trade.Side ? XLDirectionType.Buy : XLDirectionType.Sell;
            f.OffsetFlag = ConvOffSet(trade.OffsetFlag);
            f.HedgeFlag = XLHedgeFlagType.Speculation;
            f.Price = (double)trade.xPrice;
            f.Volume = trade.xSize;

            return f;

        }
        XLOrderField ConvOrder(Order order)
        {
            XLOrderField o = new XLOrderField();
            DateTime dt = Util.ToDateTime(order.Date, order.Time);
            o.TradingDay = order.SettleDay.ToString();
            o.Date = order.Date.ToString();
            o.Time = dt.ToString("HH:mm:ss");
            o.UserID = order.Account;
            o.SymbolID = order.Symbol;
            o.ExchangeID = order.Exchange;
            o.LimitPrice = (double)order.LimitPrice;
            o.StopPrice = (double)order.StopPrice;
            if (o.LimitPrice > 0)
            {
                o.OrderType = XLOrderType.Limit;
            }
            else
            {
                o.OrderType = XLOrderType.Market;
            }

            o.VolumeTotal = Math.Abs(order.TotalSize);
            o.VolumeFilled = Math.Abs(order.FilledSize);
            o.VolumeUnfilled = Math.Abs(order.Size);

            o.Direction = order.Side ? XLDirectionType.Buy : XLDirectionType.Sell;
            o.OffsetFlag = ConvOffSet(order.OffsetFlag);
            o.HedgeFlag = XLHedgeFlagType.Speculation;

            o.OrderRef = order.OrderRef;
            o.OrderSysID = order.OrderSysID;
            o.RequestID = order.RequestID;
            o.OrderID = order.id;
            o.OrderStatus = ConvOrderStatus(order.Status);
            o.StatusMsg = order.Comment;
            o.ForceClose = order.ForceClose ? 1 : 0;
            o.ForceCloseReason = order.ForceCloseReason;

            return o;


        }

        /// <summary>
        /// 转换开平标识
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        XLOffsetFlagType ConvOffSet(QSEnumOffsetFlag offset)
        {
            switch (offset)
            {
                case QSEnumOffsetFlag.OPEN: return XLOffsetFlagType.Open;
                case QSEnumOffsetFlag.CLOSE: return XLOffsetFlagType.Close;
                case QSEnumOffsetFlag.CLOSETODAY: return XLOffsetFlagType.CloseToday;
                case QSEnumOffsetFlag.CLOSEYESTERDAY: return XLOffsetFlagType.CloseYesterday;
                case QSEnumOffsetFlag.FORCECLOSE: return XLOffsetFlagType.ForceClose;
                case QSEnumOffsetFlag.FORCEOFF: return XLOffsetFlagType.ForceOff;
                case QSEnumOffsetFlag.UNKNOWN: return XLOffsetFlagType.Unknown;
                default:
                    return XLOffsetFlagType.Unknown;
            }
        }
        QSEnumOffsetFlag ConvOffSet(XLOffsetFlagType offset)
        {
            switch (offset)
            {
                case XLOffsetFlagType.Open: return QSEnumOffsetFlag.OPEN;
                case XLOffsetFlagType.Close: return QSEnumOffsetFlag.CLOSE;
                case XLOffsetFlagType.CloseToday: return QSEnumOffsetFlag.CLOSETODAY;
                case XLOffsetFlagType.CloseYesterday: return QSEnumOffsetFlag.CLOSEYESTERDAY;
                case XLOffsetFlagType.ForceClose: return QSEnumOffsetFlag.FORCECLOSE;
                case XLOffsetFlagType.ForceOff: return QSEnumOffsetFlag.FORCEOFF;
                case XLOffsetFlagType.Unknown: return QSEnumOffsetFlag.UNKNOWN;
                default:
                    return QSEnumOffsetFlag.UNKNOWN;
            }
        }

        /// <summary>
        /// 转换委托状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        XLOrderStatus ConvOrderStatus(QSEnumOrderStatus status)
        {
            switch (status)
            {
                case QSEnumOrderStatus.Canceled: return XLOrderStatus.Canceled;
                case QSEnumOrderStatus.Filled: return XLOrderStatus.Filled;
                case QSEnumOrderStatus.Opened: return XLOrderStatus.Opened;
                case QSEnumOrderStatus.PartFilled: return XLOrderStatus.PartFilled;
                case QSEnumOrderStatus.Placed: return XLOrderStatus.Placed;
                case QSEnumOrderStatus.PreSubmited: return XLOrderStatus.PreSubmited;
                case QSEnumOrderStatus.Reject: return XLOrderStatus.Reject;
                case QSEnumOrderStatus.Submited: return XLOrderStatus.Submited;
                case QSEnumOrderStatus.Unknown: return XLOrderStatus.Unknown;
                default:
                    return XLOrderStatus.Unknown;
            }
        }


        /// <summary>
        /// 转换货币
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        XLCurrencyType ConvCurrencyType(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.RMB: return XLCurrencyType.RMB;
                case CurrencyType.USD: return XLCurrencyType.USD;
                case CurrencyType.HKD: return XLCurrencyType.HKD;
                case CurrencyType.EUR: return XLCurrencyType.EUR;
                default:
                    return XLCurrencyType.RMB;
            }
        }

        ErrorField ConvertRspInfo(RspInfo info)
        {
            ErrorField field = new ErrorField();
            field.ErrorID = info.ErrorID;
            field.ErrorMsg = info.ErrorMessage;
            return field;
        }
    }
}
