//Copyright 2013 by FutSystems,Inc.
//20161223 清理与消息路由无关函数

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
        /// 路由客户端提交的请求 执行逻辑业务
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <param name="account"></param>
        void OnPacketRequest(ISession session,IPacket packet,IAccount account)
        {
            switch (packet.Type)
            {
                case MessageTypes.SENDORDER://提交委托
                    {
                        OrderInsertRequest request = packet as OrderInsertRequest;
                        SrvOnOrderRequest(session,request, account);
                    }
                    break;
                case MessageTypes.SENDORDERACTION://提交委托操作
                    {
                        OrderActionRequest request = packet as OrderActionRequest;
                        SrvOnOrderActionRequest(session, request, account);
                    }
                    break;
                case MessageTypes.QRYORDER://查询委托
                    {
                        QryOrderRequest request = packet as QryOrderRequest;
                        SrvOnQryOrder(session, request, account);
                    }
                    break;
                case MessageTypes.QRYTRADE://查询成交
                    {
                        QryTradeRequest request = packet as QryTradeRequest;
                        SrvOnQryTrade(session, request, account);
                    }
                    break;
                case MessageTypes.QRYPOSITION://查询持仓
                    {
                        QryPositionRequest request = packet as QryPositionRequest;
                        SrvOnQryPosition(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYPOSITIONDETAIL://查询持仓明细
                    {
                        XQryPositionDetailRequest request = packet as XQryPositionDetailRequest;
                        SrvOnXQryPositionDetail(session, request, account);
                    }
                    break;
                case MessageTypes.QRYACCOUNTINFO://查询帐户信息
                    {
                        QryAccountInfoRequest request = packet as QryAccountInfoRequest;
                        SrvOnQryAccountInfo(session, request, account);
                    }
                    break;
                case MessageTypes.QRYMAXORDERVOL://查询委托可开手数
                    {
                        QryMaxOrderVolRequest request = packet as QryMaxOrderVolRequest;
                        SrvOnQryMaxOrderVol(session, request, account);
                    }
                    break;

                case MessageTypes.QRYSETTLEINFOCONFIRM://查询结算确认
                    {
                        QrySettleInfoConfirmRequest request = packet as QrySettleInfoConfirmRequest;
                        SrvOnQrySettleInfoConfirm(session, request, account);
                    }
                    break;
                case MessageTypes.CONFIRMSETTLEMENT://确认结算信息
                    {
                        ConfirmSettlementRequest request = packet as ConfirmSettlementRequest;
                        SrvOnConfirmSettlement(session, request, account);
                    }
                    break;
                case MessageTypes.QRYINVESTOR://查询投资者信息
                    {
                        QryInvestorRequest request = packet as QryInvestorRequest;
                        SrvOnQryInvestor(session, request, account);
                    }
                    break;
                case MessageTypes.REQCHANGEPASS://请求修改交易帐户密码
                    {
                        ReqChangePasswordRequest request = packet as ReqChangePasswordRequest;
                        SrvOnReqChangePassword(session, request, account);
                    }
                    break;
                case MessageTypes.QRYNOTICE://请求查询交易系统通知
                    {
                        QryNoticeRequest request = packet as QryNoticeRequest;
                        SrvOnQryNotice(session, request, account);
                    }
                    break;
                case MessageTypes.QRYSYMBOL://查询合约列表
                    {
                        QrySymbolRequest request = packet as QrySymbolRequest;
                        SrvOnQrySymbol(session, request, account);
                    }
                    break;
                case MessageTypes.QRYCONTRACTBANK://查询签约银行
                    {
                        QryContractBankRequest request = packet as QryContractBankRequest;
                        SrvOnQryContractBank(session, request);
                    }
                    break;
                case MessageTypes.QRYREGISTERBANKACCOUNT://查询银行帐户
                    {
                        //debug("查询银行帐户.............", QSEnumDebugLevel.INFO);
                        QryRegisterBankAccountRequest request = packet as QryRegisterBankAccountRequest;
                        SrvOnRegisterBankAccount(session, request, account);
                    }
                    break;
                case MessageTypes.QRYTRANSFERSERIAL://查询出入金记录
                    {
                        QryTransferSerialRequest request = packet as QryTransferSerialRequest;
                        SrvOnQryTransferSerial(session, request, account);
                    }
                    break;
                case MessageTypes.QRYINSTRUMENTCOMMISSIONRATE://查询合约手续费率
                    {
                        QryInstrumentCommissionRateRequest request = packet as QryInstrumentCommissionRateRequest;
                        SrvOnQryInstrumentCommissionRate(session, request, account);
                    }
                    break;
                case MessageTypes.QRYINSTRUMENTMARGINRATE://查询合约保证金率
                    {
                        QryInstrumentMarginRateRequest request = packet as QryInstrumentMarginRateRequest;
                        SrvOnQryInstrumentMarginRate(session, request, account);
                    }
                    break;
                case MessageTypes.QRYMARKETDATA://查询市场行情
                    {
                        QryMarketDataRequest request = packet as QryMarketDataRequest;
                        SrvOnQryMarketData(session, request, account);
                    }
                    break;
                case MessageTypes.QRYTRADINGPARAMS://查询交易参数
                    {
                        QryTradingParamsRequest request = packet as QryTradingParamsRequest;
                        SrvOnQryTradingParams(session, request, account);
                    }
                    break;


                //XAPI 调用接口
                case MessageTypes.XQRYMARKETTIME://查询交易时间段
                    {
                        XQryMarketTimeRequest request = packet as XQryMarketTimeRequest;
                        SrvOnXQryMarketTime(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYEXCHANGE://查询交易所
                    {
                        XQryExchangeRequuest request = packet as XQryExchangeRequuest;
                        SrvOnXQryExchange(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYSECURITY://查询品种
                    {
                        XQrySecurityRequest request = packet as XQrySecurityRequest;
                        SrvOnXQrySecurity(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYSYMBOL://查询合约
                    {
                        XQrySymbolRequest request = packet as XQrySymbolRequest;
                        SrvOnXQrySymbol(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYYDPOSITION://查询隔夜持仓
                    {
                        XQryYDPositionRequest request = packet as XQryYDPositionRequest;
                        SrvOnXQryYDPosition(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYORDER://查询委托
                    {
                        XQryOrderRequest request = packet as XQryOrderRequest;
                        SrvOnXQryOrder(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYTRADE://查询成交
                    {
                        XQryTradeRequest request = packet as XQryTradeRequest;
                        SrvOnXQryTrade(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYTICKSNAPSHOT://查询行情快照
                    {
                        XQryTickSnapShotRequest request = packet as XQryTickSnapShotRequest;
                        SrvOnXQryTickSnapShot(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYACCOUNT://查询交易账户
                    {
                        XQryAccountRequest request = packet as XQryAccountRequest;
                        SrvOnXQryAccount(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYMAXORDERVOL://查询最大下单量
                    {
                        XQryMaxOrderVolRequest request = packet as XQryMaxOrderVolRequest;
                        SrvOnXQryMaxVol(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYACCOUNTFINANCE://查询财务数据
                    {
                        XQryAccountFinanceRequest request = packet as XQryAccountFinanceRequest;
                        SrvOnQryAccountFinance(session, request, account);
                    }
                    break;

                case MessageTypes.XQRYSETTLEINFO://查询结算信息
                    {
                        XQrySettleInfoRequest request = packet as XQrySettleInfoRequest;
                        SrvOnXQrySettleInfo(session, request, account);
                    }
                    break;
                case MessageTypes.XQRYEXCHANGERATE://查询汇率信息
                    {
                        XQryExchangeRateRequest request = packet as XQryExchangeRateRequest;
                        SrvOnXQryExchangeRate(session, request, account);
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


                case MessageTypes.BOSENDORDER://请求二元委托
                    {
                        BOOrderInsertRequest request = packet as BOOrderInsertRequest;
                        SrvOnBOOrderInsertOrder(request, account);
                    }
                    break;
                default:
                    logger.Warn("packet:" + packet.ToString() + " can not be handled");
                    break;
            }
        }

    }
}
