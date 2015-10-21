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
        void CacheRspResponse(RspResponsePacket packet, bool islat = true)
        {
            packet.IsLast = islat;
            if (!prioritybuffer.Write(packet))//如果该回报不是由优先缓存发送，则写入常规缓存
            {
                CachePacket(packet);
            }
            else
            {
                //logger.Info("packet:" + packet.ToString() + "写入优先缓存");
            }
        }

        void tl_newPacketRequest(ISession session,IPacket packet)
        {
            IAccount account = session.GetAccount();
            switch (packet.Type)
            {

                case MessageTypes.QRYORDER://查询委托
                    {
                        QryOrderRequest request = packet as QryOrderRequest;
                        SrvOnQryOrder(request, account);
                    }
                    break;
                case MessageTypes.QRYTRADE://查询成交
                    {
                        QryTradeRequest request = packet as QryTradeRequest;
                        SrvOnQryTrade(request, account);
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
                        SrvOnQryMaxOrderVol(request, account);
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
                        SrvOnQryInstrumentMarginRate(request, account);
                    }
                    break;
                case MessageTypes.QRYMARKETDATA://查询市场行情
                    {
                        QryMarketDataRequest request = packet as QryMarketDataRequest;
                        SrvOnQryMarketData(request, account);
                    }
                    break;
                case MessageTypes.QRYTRADINGPARAMS://查询交易参数
                    {
                        QryTradingParamsRequest request = packet as QryTradingParamsRequest;
                        SrvOnQryTradingParams(request, account);
                    }
                    break;
                case MessageTypes.XQRYMARKETTIME://查询交易时间段
                    {
                        XQryMarketTimeRequest request = packet as XQryMarketTimeRequest;
                        SrvOnXQryMarketTime(request, account);
                    }
                    break;
                case MessageTypes.XQRYEXCHANGE://查询交易所
                    {
                        XQryExchangeRequuest request = packet as XQryExchangeRequuest;
                        SrvOnXQryExchange(request, account);
                    }
                    break;
                case MessageTypes.XQRYSECURITY://查询品种
                    {
                        XQrySecurityRequest request = packet as XQrySecurityRequest;
                        SrvOnXQrySecurity(request, account);
                    }
                    break;
                case MessageTypes.XQRYSYMBOL://查询合约
                    {
                        XQrySymbolRequest request = packet as XQrySymbolRequest;
                        SrvOnXQrySymbol(request, account);
                    }
                    break;
                case MessageTypes.XQRYYDPOSITION://查询隔夜持仓
                    {
                        XQryYDPositionRequest request = packet as XQryYDPositionRequest;
                        SrvOnXQryYDPosition(request, account);
                    }
                    break;
                case MessageTypes.XQRYORDER://查询委托
                    {
                        XQryOrderRequest request = packet as XQryOrderRequest;
                        SrvOnXQryOrder(request, account);
                    }
                    break;
                case MessageTypes.XQRYTRADE://查询成交
                    {
                        XQryTradeRequest request = packet as XQryTradeRequest;
                        SrvOnXQryTrade(request, account);
                    }
                    break;
                case MessageTypes.XQRYTICKSNAPSHOT://查询行情快照
                    {
                        XQryTickSnapShotRequest request = packet as XQryTickSnapShotRequest;
                        SrvOnXQryTickSnapShot(request, account);
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
                    logger.Warn("packet:" + packet.ToString() + " can not be handled");
                    break;
            }
        }

    }
}
