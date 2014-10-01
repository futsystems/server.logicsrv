using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutsMoniter;

namespace TradingLib.Common
{
    public delegate void RspMGRLoginResponseDel(RspMGRLoginResponse response);
    public partial class TLClientNet
    {

        void CliOnTickNotify(TickNotify response)
        {
            if (OnTickEvent != null)
                OnTickEvent(response.Tick);
        }


        void CliOnOldPositionNotify(HoldPositionNotify response)
        {
            debug("got holdposition notify " + response.Position.ToString(), QSEnumDebugLevel.INFO);
            PositionEx ap = response.Position;

            Position pos = new PositionImpl(ap.Symbol, ap.AvgPrice, ap.Size, 0, ap.Account,ap.DirectionType);
            //debug("symbol:" + pos.Symbol);
            Symbol osym = Globals.BasicInfoTracker.GetSymbol(pos.Symbol);
            //debug("got osymbol:" + (osym != null).ToString(), QSEnumDebugLevel.INFO);
            //debug("got oposiont" + osym != null ? osym.Symbol : "not find", QSEnumDebugLevel.INFO);
            pos.oSymbol = Globals.BasicInfoTracker.GetSymbol(pos.Symbol);
            //debug("got postion symbol:" + pos.oSymbol.Symbol,QSEnumDebugLevel.INFO);
            this.handler.OnHoldPosition(pos);
        }
        void CliOnOrderNotify(OrderNotify response)
        {
            debug("got order notify:" + response.Order.ToString(), QSEnumDebugLevel.INFO);
            Order o = response.Order;
            o.oSymbol = Globals.BasicInfoTracker.GetSymbol(o.symbol);
            this.handler.OnOrder(o);
        }
        void CliOnTradeNotify(TradeNotify response)
        {
            debug("got trade notify:" + response.Trade.ToString(), QSEnumDebugLevel.INFO);
            Trade f = response.Trade;
            f.oSymbol = Globals.BasicInfoTracker.GetSymbol(f.symbol);
            this.handler.OnTrade(response.Trade);
        }

        void CliOnPositionUpdateNotify(PositionNotify response)
        {
            //debug("got postion notify:" + response.Position.ToString(), QSEnumDebugLevel.INFO);
            //this.handler.OnPositionUpdate(response.Position);
        }

        void CliOnErrorOrderNotify(ErrorOrderNotify response)
        {
            debug(string.Format("got order error:{0} message:{1} order:{2}", response.RspInfo.ErrorID, response.RspInfo.ErrorMessage, OrderImpl.Serialize(response.Order)));

            this.handler.PopRspInfo(response.RspInfo);
        }




        void CliOnSettleInfoConfirm(RspQrySettleInfoConfirmResponse response)
        {
            debug("got confirmsettleconfirm data:" + response.ConfirmDay.ToString() + " time:" + response.ConfirmTime.ToString(), QSEnumDebugLevel.INFO);

        }

        void CliOnSettleInfo(RspQrySettleInfoResponse response)
        {
            debug("got settleinfo:", QSEnumDebugLevel.INFO);
            string[] rec = response.Content.Split('\n');
            foreach (string s in rec)
            {
                debug(s, QSEnumDebugLevel.INFO);
            }
        }


        void CliOnOrderAction(OrderActionNotify response)
        {
            debug("got order action:" + response.ToString());
        }

        void CliOnErrorOrderActionNotify(ErrorOrderActionNotify response)
        {
            debug(string.Format("got orderaction error:{0} message:{1} orderaction:{2}", response.RspInfo.ErrorID, response.RspInfo.ErrorMessage, OrderActionImpl.Serialize(response.OrderAction)));
        }


        void CliOnOperationResponse(RspMGROperationResponse response)
        {

            this.handler.PopRspInfo(response.RspInfo);
        }

        #region 查询
        void CliOnRspQryAccountInfoResponse(RspQryAccountInfoResponse response)
        {
            debug("------------帐户信息-------------", QSEnumDebugLevel.INFO);
            debug("         Account:" + response.AccInfo.Account, QSEnumDebugLevel.INFO);
            debug("LastEqutiy:" + response.AccInfo.LastEquity.ToString(), QSEnumDebugLevel.INFO);
            debug("NowEquity:" + response.AccInfo.NowEquity.ToString(), QSEnumDebugLevel.INFO);
            debug("RealizedPL:" + response.AccInfo.RealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("UnRealizedPL:" + response.AccInfo.UnRealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("Commission:" + response.AccInfo.Commission.ToString(), QSEnumDebugLevel.INFO);
            debug("Profit:" + response.AccInfo.Profit.ToString(), QSEnumDebugLevel.INFO);
            debug("CashIn:" + response.AccInfo.CashIn.ToString(), QSEnumDebugLevel.INFO);
            debug("CashOut:" + response.AccInfo.CashOut.ToString(), QSEnumDebugLevel.INFO);
            debug("MoneyUsed:" + response.AccInfo.MoneyUsed.ToString(), QSEnumDebugLevel.INFO);
            debug("TotalLiquidation:" + response.AccInfo.TotalLiquidation.ToString(), QSEnumDebugLevel.INFO);
            debug("AvabileFunds:" + response.AccInfo.AvabileFunds.ToString(), QSEnumDebugLevel.INFO);
            debug("Category:" + response.AccInfo.Category.ToString(), QSEnumDebugLevel.INFO);
            debug("OrderRouterType:" + response.AccInfo.OrderRouteType.ToString(), QSEnumDebugLevel.INFO);
            debug("Excute:" + response.AccInfo.Execute.ToString(), QSEnumDebugLevel.INFO);
            debug("Intraday:" + response.AccInfo.IntraDay.ToString(), QSEnumDebugLevel.INFO);

            debug("FutMarginUsed:" + response.AccInfo.FutMarginUsed.ToString(), QSEnumDebugLevel.INFO);
            debug("FutMarginFrozen:" + response.AccInfo.FutMarginFrozen.ToString(), QSEnumDebugLevel.INFO);
            debug("FutRealizedPL:" + response.AccInfo.FutRealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("FutUnRealizedPL:" + response.AccInfo.FutUnRealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("FutCommission:" + response.AccInfo.FutCommission.ToString(), QSEnumDebugLevel.INFO);
            debug("FutCash:" + response.AccInfo.FutCash.ToString(), QSEnumDebugLevel.INFO);
            debug("FutLiquidation:" + response.AccInfo.FutLiquidation.ToString(), QSEnumDebugLevel.INFO);
            debug("FutMoneyUsed:" + response.AccInfo.FutMoneyUsed.ToString(), QSEnumDebugLevel.INFO);
            debug("FutAvabileFunds:" + response.AccInfo.FutAvabileFunds.ToString(), QSEnumDebugLevel.INFO);

            debug("OptPositionCost:" + response.AccInfo.OptPositionCost.ToString(), QSEnumDebugLevel.INFO);
            debug("OptPositionValue:" + response.AccInfo.OptPositionValue.ToString(), QSEnumDebugLevel.INFO);
            debug("OptRealizedPL:" + response.AccInfo.OptRealizedPL.ToString(), QSEnumDebugLevel.INFO);
            debug("OptCommission:" + response.AccInfo.OptCommission.ToString(), QSEnumDebugLevel.INFO);
            debug("OptMoneyFrozen:" + response.AccInfo.OptMoneyFrozen.ToString(), QSEnumDebugLevel.INFO);
            debug("OptCash:" + response.AccInfo.OptCash.ToString(), QSEnumDebugLevel.INFO);
            debug("OptMarketValue:" + response.AccInfo.OptMarketValue.ToString(), QSEnumDebugLevel.INFO);
            debug("OptLiquidation:" + response.AccInfo.OptLiquidation.ToString(), QSEnumDebugLevel.INFO);
            debug("OptMoneyUsed:" + response.AccInfo.OptMoneyUsed.ToString(), QSEnumDebugLevel.INFO);
            debug("OptAvabileFunds:" + response.AccInfo.OptAvabileFunds.ToString(), QSEnumDebugLevel.INFO);

            debug("InnovPositionCost:" + response.AccInfo.InnovPositionCost.ToString(), QSEnumDebugLevel.INFO);
            debug("InnovPositionValue:" + response.AccInfo.InnovPositionValue.ToString(), QSEnumDebugLevel.INFO);
            debug("InnovCommission:" + response.AccInfo.InnovCommission.ToString(), QSEnumDebugLevel.INFO);
            debug("InnovRealizedPL:" + response.AccInfo.InnovRealizedPL.ToString(), QSEnumDebugLevel.INFO);


        }

        void CliOnMaxOrderVol(RspQryMaxOrderVolResponse response)
        {

        }

        /// <summary>
        /// 查询委托回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnRspQryOrderResponse(RspQryOrderResponse response)
        {

        }
        /// <summary>
        /// 查询成交回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnRspQryTradeResponse(RspQryTradeResponse response)
        {

        }

        /// <summary>
        /// 查询持仓回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnRspQryPositionResponse(RspQryPositionResponse response)
        {

        }

        void CliOnRspQryInvestorResponse(RspQryInvestorResponse response)
        {

        }

        
        void CliOnRspMGRLoginResponse(RspMGRLoginResponse response)
        {
            debug("got login responsexxxxxxxxxxxxxxxxxx:" + response.ToString(), QSEnumDebugLevel.INFO);
            if (OnLoginEvent != null)
                OnLoginEvent(response);
        }
        #endregion








        #region 交易帐号类操作
        /// <summary>
        /// 查询可查看交易帐号列表回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnMGRQryAccount(RspMGRQryAccountResponse response)
        {
            debug("got mgrqryaccount response:" + response.ToString(), QSEnumDebugLevel.INFO);

            this.handler.OnAccountLite(response.oAccount,response.IsLast);
        }

        /// <summary>
        /// 交易帐号信息更新
        /// </summary>
        /// <param name="response"></param>
        void CliOnNotifyMGRAccountInfo(NotifyMGRAccountInfoLiteResponse response)
        {
            //debug("got notify accountinfo:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnAccountInfoLite(response.InfoLite);
        }

        /// <summary>
        /// 恢复交易记录回报
        /// </summary>
        /// <param name="response"></param>
        void CliOnMGRResumeAccountResponse(RspMGRResumeAccountResponse response)
        {
            debug("got resume account response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRResumeResponse(response);
        }

        /// <summary>
        /// 交易帐户登入 退出更新
        /// </summary>
        /// <param name="notify"></param>
        void CliOnMGRSesssionUpdate(NotifyMGRSessionUpdateNotify notify)
        {
            debug("got session update notify:" + notify.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRSessionUpdate(notify);
        }

        void CliOnMGRQryAccountInfo(RspMGRQryAccountInfoResponse response)
        {
            debug("got mgr account info response:"+response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnAccountInfo(response.AccountInfoToSend);
        }

        void CliOnMGRAccountChangeUpdaet(NotifyMGRAccountChangeUpdateResponse notify)
        {
            debug("got account change update:" + notify.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnAccountChagne(notify.oAccount);

        }
        #endregion



        #region 系统操作回报
        void CliOnMGRQryConnector(RspMGRQryConnectorResponse response)
        {
            debug("got connector response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRConnectorResponse(response.Connector, response.IsLast);
        }

        void CliOnMGRExchange(RspMGRQryExchangeResponse response)
        {
            debug("got exchange response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRExchangeResponse(response.Exchange, response.IsLast);
        }
        void CliOnMGRMarketTime(RspMGRQryMarketTimeResponse response)
        {
            debug("got markettime response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRMarketTimeResponse(response.MarketTime, response.IsLast);
        }

        void CliOnMGRSecurity(RspMGRQrySecurityResponse response)
        {
            debug("got security response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRSecurityResponse(response.SecurityFaimly, response.IsLast);
        }

        void CliOnMGRSymbol(RspMGRQrySymbolResponse response)
        {
            debug("got symbol response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRSymbolResponse(response.Symbol, response.IsLast);
        }
        #endregion


        #region 风控规则类回报
        void CliOnMGRRuleClass(RspMGRQryRuleSetResponse response)
        {
            debug("got ruleset response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRRuleClassResponse(response.RuleClassItem, response.IsLast);
        }

        void CliOnMGRRuleItem(RspMGRQryRuleItemResponse response)
        {
            debug("got ruleitem response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRRuleItemResponse(response.RuleItem, response.IsLast);
        }
        void CliOnMGRUpdateRuleItem(RspMGRUpdateRuleResponse response)
        {
            debug("got ruleitem update response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRRuleItemUpdate(response.RuleItem, response.IsLast);
        }
        void CliOnMGRDelRule(RspMGRDelRuleItemResponse response)
        {
            debug("got ruleitem delete response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRRulteItemDelete(response.RuleItem, response.IsLast);
        }

        void CliOnMGRystemStatus(RspMGRQrySystemStatusResponse response)
        {
            debug("got systemstatus response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRSytstemStatus(response.Status, response.IsLast);
        }


        void CliOnMGROrderResponse(RspMGRQryOrderResponse response)
        {
            debug("got historder response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGROrderResponse(response.OrderToSend, response.IsLast);
        }

        void CliOnMGRTradeResponse(RspMGRQryTradeResponse response)
        {
            debug("got histtrade response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRTradeResponse(response.TradeToSend, response.IsLast);
        }


        void CliOnMGRPositionResponse(RspMGRQryPositionResponse response)
        {
            debug("got histpostion response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRPositionResponse(response.PostionToSend, response.IsLast);
        }

        void CliOnMGRCashTransactionResponse(RspMGRQryCashResponse response)
        {
            debug("got cashtransaction response:" + response.ToString(), QSEnumDebugLevel.INFO);
            this.handler.OnMGRCashTransactionResponse(response.CashTransToSend, response.IsLast);
        }

        void CliOnMGRSettlementResponse(RspMGRQrySettleResponse response)
        { 
            debug("got settlement response:"+response.ToString(),QSEnumDebugLevel.INFO);
            this.handler.OnMGRSettlementResponse(response);
        }

        void CliOnMGRChangePassResponse(RspMGRChangeAccountPassResponse response)
        {
            debug("got changepass response:" + response.ToString(), QSEnumDebugLevel.INFO);

        }
        void CliOnMGRAddSecurityResponse(RspMGRReqAddSecurityResponse response)
        {
            debug("got add security response:" + response.ToString(), QSEnumDebugLevel.INFO);
            if (response.RspInfo.ErrorID != 0)
            {
                
            }
            else
            {
                this.handler.OnMGRSecurityAddResponse(response.SecurityFaimly, response.IsLast);
            }
        }

        void CliOnMGRAddSymbolResponse(RspMGRReqAddSymbolResponse response)
        {
            debug("got add symbol response:" + response.ToString(), QSEnumDebugLevel.INFO);
            if (response.RspInfo.ErrorID != 0)
            {

            }
            else
            {
                this.handler.OnMGRSymbolAddResponse(response.Symbol, response.IsLast);

            }
        }
        #endregion



        #region 管理员管理

        void CliOnMGRManagerResponse(RspMGRQryManagerResponse response)
        {
            debug("got manager response:" + response.ToString());
            this.handler.OnMGRMangerResponse(response.ManagerToSend, response.IsLast);
        }
        #endregion




        void connecton_OnPacketEvent(IPacket packet)
        {
            switch (packet.Type)
            {
                //Tick数据
                case MessageTypes.TICKNOTIFY:
                    CliOnTickNotify(packet as TickNotify);
                    break;
                //昨日持仓数据
                case MessageTypes.OLDPOSITIONNOTIFY:
                    CliOnOldPositionNotify(packet as HoldPositionNotify);
                    break;
                //委托回报
                case MessageTypes.ORDERNOTIFY:
                    CliOnOrderNotify(packet as OrderNotify);
                    break;
                case MessageTypes.ERRORORDERNOTIFY:
                    CliOnErrorOrderNotify(packet as ErrorOrderNotify);
                    break;
                //成交回报
                case MessageTypes.EXECUTENOTIFY:
                    CliOnTradeNotify(packet as TradeNotify);
                    break;
                //持仓更新回报
                case MessageTypes.POSITIONUPDATENOTIFY:
                    CliOnPositionUpdateNotify(packet as PositionNotify);
                    break;
                //委托操作回报
                case MessageTypes.ORDERACTIONNOTIFY:
                    CliOnOrderAction(packet as OrderActionNotify);
                    break;

                case MessageTypes.ERRORORDERACTIONNOTIFY:
                    break;

                case MessageTypes.MGRLOGINRESPONSE://管理登入回报
                    CliOnRspMGRLoginResponse(packet as RspMGRLoginResponse);
                    break;
                case MessageTypes.MGRQRYACCOUNTSRESPONSE://管理查询帐户列表回报
                    CliOnMGRQryAccount(packet as RspMGRQryAccountResponse);
                    break;
                case MessageTypes.MGRACCOUNTINFOLITENOTIFY://管理帐户财务数据更新
                    CliOnNotifyMGRAccountInfo(packet as NotifyMGRAccountInfoLiteResponse);
                    break;
                case MessageTypes.MGRRESUMEACCOUNTRESPONE://管理恢复帐户日内交易数据回报
                    CliOnMGRResumeAccountResponse(packet as RspMGRResumeAccountResponse);
                    break;
                case MessageTypes.MGRSESSIONSTATUSUPDATE://管理 交易帐户登入 退出信息更新
                    CliOnMGRSesssionUpdate(packet as NotifyMGRSessionUpdateNotify);
                    break;
                case MessageTypes.MGRACCOUNTINFORESPONSE://管理 查询交易帐户信息
                    CliOnMGRQryAccountInfo(packet as RspMGRQryAccountInfoResponse);
                    break;
                case MessageTypes.MGRACCOUNTCHANGEUPDATE://交易客户端更改通知
                    CliOnMGRAccountChangeUpdaet(packet as NotifyMGRAccountChangeUpdateResponse);
                    break;
                case MessageTypes.MGRCONNECTORRESPONSE://通道列表回报
                    CliOnMGRQryConnector(packet as RspMGRQryConnectorResponse);
                    break;
                case MessageTypes.MGREXCHANGERESPONSE://交易所列表回报
                    CliOnMGRExchange(packet as RspMGRQryExchangeResponse);
                    break;
                case MessageTypes.MGRMARKETTIMERESPONSE://交易时间段回报
                    CliOnMGRMarketTime(packet as RspMGRQryMarketTimeResponse);
                    break;
                case MessageTypes.MGRSECURITYRESPONSE://品种回报
                    CliOnMGRSecurity(packet as RspMGRQrySecurityResponse);
                    break;
                case MessageTypes.MGRSYMBOLRESPONSE://合约回报
                    CliOnMGRSymbol(packet as RspMGRQrySymbolResponse);
                    break;
                case MessageTypes.MGRRULECLASSRESPONSE://风控规则回报
                    CliOnMGRRuleClass(packet as RspMGRQryRuleSetResponse);
                    break;
                case MessageTypes.MGRRULEITEMRESPONSE://帐户风控项目回报
                    CliOnMGRRuleItem(packet as RspMGRQryRuleItemResponse);
                    break;
                case MessageTypes.MGRUPDATERULEITEMRESPONSE://帐户风控更新回报
                    CliOnMGRUpdateRuleItem(packet as RspMGRUpdateRuleResponse);
                    break;
                case MessageTypes.MGRDELRULEITEMRESPONSE://删除风控项回报
                    CliOnMGRDelRule(packet as RspMGRDelRuleItemResponse);
                    break;
                case MessageTypes.MGRSYSTEMSTATUSRESPONSE://查询系统状态回报
                    CliOnMGRystemStatus(packet as RspMGRQrySystemStatusResponse);
                    break;
                case MessageTypes.MGRORDERRESPONSE://查询委托回报
                    CliOnMGROrderResponse(packet as RspMGRQryOrderResponse);
                    break;
                case MessageTypes.MGRTRADERESPONSE://查询成交回报
                    CliOnMGRTradeResponse(packet as RspMGRQryTradeResponse);
                    break;
                case MessageTypes.MGRPOSITIONRESPONSE://查询结算持仓回报
                    CliOnMGRPositionResponse(packet as RspMGRQryPositionResponse);
                    break;
                case MessageTypes.MGRCASHRESPONSE://查询出入金记录
                    CliOnMGRCashTransactionResponse(packet as RspMGRQryCashResponse);
                    break;
                case MessageTypes.MGRSETTLEMENTRESPONSE://查询结算单回报
                    CliOnMGRSettlementResponse(packet as RspMGRQrySettleResponse);
                    break;
                case MessageTypes.MGRCHANGEACCOUNTPASSRESPONSE://修改密码回报
                    CliOnMGRChangePassResponse(packet as RspMGRChangeAccountPassResponse);
                    break;
                case MessageTypes.MGRADDSECURITYRESPONSE://添加品种回报
                    CliOnMGRAddSecurityResponse(packet as RspMGRReqAddSecurityResponse);
                    break;
                case MessageTypes.MGRADDSYMBOLRESPONSE://添加合约回报
                    CliOnMGRAddSymbolResponse(packet as RspMGRReqAddSymbolResponse);
                    break;
                case MessageTypes.MGROPERATIONRESPONSE://常规操作回报
                    CliOnOperationResponse(packet as RspMGROperationResponse);
                    break;
                case MessageTypes.MGRMANAGERRESPONSE://管理员查询回报
                    CliOnMGRManagerResponse(packet as RspMGRQryManagerResponse);
                    break;
                #region 查询
                case MessageTypes.ORDERRESPONSE://查询委托回报
                    CliOnRspQryOrderResponse(packet as RspQryOrderResponse);
                    break;
                case MessageTypes.TRADERESPONSE://查询成交回报
                    CliOnRspQryTradeResponse(packet as RspQryTradeResponse);
                    break;
                case MessageTypes.POSITIONRESPONSE://查询持仓回报
                    CliOnRspQryPositionResponse(packet as RspQryPositionResponse);
                    break;

                case MessageTypes.ACCOUNTINFORESPONSE://帐户信息回报
                    CliOnRspQryAccountInfoResponse(packet as RspQryAccountInfoResponse);
                    break;
                case MessageTypes.INVESTORRESPONSE:
                    CliOnRspQryInvestorResponse(packet as RspQryInvestorResponse);
                    break;

                case MessageTypes.MAXORDERVOLRESPONSE: //最大可开数量回报
                    CliOnMaxOrderVol(packet as RspQryMaxOrderVolResponse);
                    break;
                case MessageTypes.SETTLEINFOCONFIRMRESPONSE://结算确认回报
                    CliOnSettleInfoConfirm(packet as RspQrySettleInfoConfirmResponse);
                    break;

                case MessageTypes.SETTLEINFORESPONSE://结算信息会回报
                    CliOnSettleInfo(packet as RspQrySettleInfoResponse);
                    break;
                #endregion

                default:
                    debug("Packet Handler Not Set, Packet:" + packet.ToString(), QSEnumDebugLevel.ERROR);
                    break;
            }
        }
    }



}
