///////////////////////////////////////////////////////////////////////////////////////
// PacketTemplate逻辑包模板类
// 1.将逻辑包的生成过程封装到一个函数中，实现代码简化和统一
// 2.同时逻辑包的解析与生成归纳成四大类，服务端收到请求，服务端生成回报，客户端生成请求，服务端收到回报
// 3.针对集中解析分布处理的原则，在服务端收到请求，客户端收到回报时按照统一的格式进行了逻辑包的解析同时
// 在逻辑包统一解析部分做了路由非路由内的操作码均无法解析到正常的数据包，并且会抛出异常
// 基本处理思路如下
//  传输层-->消息层Packet集中解析生成IPacket-->逻辑层handle(IPacket)通过判断IPacket的类型进行相关取值与操作
// 
//
///////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    public class PacketError : QSError
    {
        public PacketError()
            :base(new Exception(),"packet error")
        { 
            
        }
    }

    public class PacketTypeNotAvabile : PacketError
    {
        public MessageTypes Type { get; set; }
        public string Content { get; set; }
        public string FrontID { get; set; }
        public string ClientID { get; set; }
        public PacketTypeNotAvabile(MessageTypes type,string content,string frontid,string client)
        {
            Type = type;
            Content = content;
            FrontID = frontid;
            ClientID = client;
        }
    }
    public class PacketParseError : PacketError
    {

        public Exception RawException { get; set; }
        public MessageTypes Type { get; set; }
        public string Content { get; set; }
        public string FrontID { get; set; }
        public string ClientID { get; set; }
        public PacketParseError(Exception raw,MessageTypes type,string content,string frontid,string client)
        {
            RawException = raw;
            Type = type;
            Content = content;
            FrontID = frontid;
            ClientID = client;

        }
    }
    public class PacketHelper
    {
        public static IPacket SrvRecvRequest(MessageTypes type,string content,string frontid, string clientid)
        {
            try
            {
                switch (type)
                {
                    //逻辑活动请求
                    case MessageTypes.LOGICLIVEREQUEST:
                        return RequestTemplate<LogicLiveRequest>.SrvRecvRequest(frontid, clientid, content);
                    //客户端注册
                    case MessageTypes.REGISTERCLIENT:
                        return RequestTemplate<RegisterClientRequest>.SrvRecvRequest(frontid, clientid, content);
                    //请求注销
                    case MessageTypes.CLEARCLIENT:
                        return RequestTemplate<UnregisterClientRequest>.SrvRecvRequest(frontid, clientid, content);
                    //发送心跳
                    case MessageTypes.HEARTBEAT:
                        return RequestTemplate<HeartBeat>.SrvRecvRequest(frontid, clientid, content);

                    //功能码请求
                    case MessageTypes.FEATUREREQUEST:
                        return RequestTemplate<FeatureRequest>.SrvRecvRequest(frontid, clientid, content);
                    //版本查询与连接初始化
                    case MessageTypes.VERSIONREQUEST:
                        return RequestTemplate<VersionRequest>.SrvRecvRequest(frontid, clientid, content);
                    //发送心跳请求
                    case MessageTypes.HEARTBEATREQUEST:
                        return RequestTemplate<HeartBeatRequest>.SrvRecvRequest(frontid, clientid, content);
                    //请求登入
                    case MessageTypes.LOGINREQUEST:
                        return RequestTemplate<LoginRequest>.SrvRecvRequest(frontid, clientid, content);


                    //服务查询
                    case MessageTypes.BROKERNAMEREQUEST:
                        return RequestTemplate<BrokerNameRequest>.SrvRecvRequest(frontid, clientid, content);
                    //注册合约
                    case MessageTypes.REGISTERSTOCK:
                        return RequestTemplate<RegisterSymbolsRequest>.SrvRecvRequest(frontid, clientid, content);
                    //注销合约
                    case MessageTypes.CLEARSTOCKS:
                        return RequestTemplate<UnregisterSymbolsRequest>.SrvRecvRequest(frontid, clientid, content);
                    //发送委托
                    case MessageTypes.SENDORDER:
                        return RequestTemplate<OrderInsertRequest>.SrvRecvRequest(frontid, clientid, content);
                    //发送委托操作
                    case MessageTypes.SENDORDERACTION:
                        return RequestTemplate<OrderActionRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询交易员
                    case MessageTypes.QRYINVESTOR:
                        return RequestTemplate<QryInvestorRequest>.SrvRecvRequest(frontid, clientid, content);
                    //账户信息查询
                    case MessageTypes.QRYACCOUNTINFO:
                        return RequestTemplate<QryAccountInfoRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询合约
                    case MessageTypes.QRYSYMBOL:
                        return RequestTemplate<QrySymbolRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询结算确认
                    case MessageTypes.QRYSETTLEINFOCONFIRM:
                        return RequestTemplate<QrySettleInfoConfirmRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询结算信息
                    case MessageTypes.QRYSETTLEINFO:
                        return RequestTemplate<QrySettleInfoRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询委托
                    case MessageTypes.QRYORDER:
                        return RequestTemplate<QryOrderRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询成交
                    case MessageTypes.QRYTRADE:
                        return RequestTemplate<QryTradeRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询持仓
                    case MessageTypes.QRYPOSITION:
                        return RequestTemplate<QryPositionRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询持仓明细
                    case MessageTypes.QRYPOSITIONDETAIL:
                        return RequestTemplate<QryPositionDetailRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询最大保单
                    case MessageTypes.QRYMAXORDERVOL:
                        return RequestTemplate<QryMaxOrderVolRequest>.SrvRecvRequest(frontid, clientid, content);
                    //确认结算单
                    case MessageTypes.CONFIRMSETTLEMENT:
                        return RequestTemplate<ConfirmSettlementRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询历史行情
                    case MessageTypes.QRYBAR:
                        return null;
                    //扩展命令请求
                    case MessageTypes.CONTRIBREQUEST:
                        return RequestTemplate<ContribRequest>.SrvRecvRequest(frontid, clientid, content);
                    //请求修改密码
                    case MessageTypes.REQCHANGEPASS:
                        return RequestTemplate<ReqChangePasswordRequest>.SrvRecvRequest(frontid, clientid, content);
                    //请求查询系统通知
                    case MessageTypes.QRYNOTICE:
                        return RequestTemplate<QryNoticeRequest>.SrvRecvRequest(frontid, clientid, content);
                    //请求查询签约银行列表
                    case MessageTypes.QRYCONTRACTBANK:
                        return RequestTemplate<QryContractBankRequest>.SrvRecvRequest(frontid, clientid, content);
                    //请求查询银行帐户
                    case MessageTypes.QRYREGISTERBANKACCOUNT:
                        return RequestTemplate<QryRegisterBankAccountRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询出入金流水记录
                    case MessageTypes.QRYTRANSFERSERIAL:
                        return RequestTemplate<QryTransferSerialRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询合约手续费率
                    case MessageTypes.QRYINSTRUMENTCOMMISSIONRATE:
                        return RequestTemplate<QryInstrumentCommissionRateRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询合约保证金率
                    case MessageTypes.QRYINSTRUMENTMARGINRATE:
                        return RequestTemplate<QryInstrumentMarginRateRequest>.SrvRecvRequest(frontid, clientid, content);
                    //查询市场行情
                    case MessageTypes.QRYMARKETDATA:
                        return RequestTemplate<QryMarketDataRequest>.SrvRecvRequest(frontid, clientid, content);


                    #region manager
                    case MessageTypes.MGRLOGINREQUEST://请求登入
                        return RequestTemplate<MGRLoginRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYACCOUNTS://请求帐户列表
                        return RequestTemplate<MGRQryAccountRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRWATCHACCOUNTS://请求设定观察帐户列表
                        return RequestTemplate<MGRWatchAccountRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRRESUMEACCOUNT://请求恢复交易帐号日内交易信息
                        return RequestTemplate<MGRResumeAccountRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYACCOUNTINFO://请求查询交易帐号信息
                        return RequestTemplate<MGRQryAccountInfoRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRCASHOPERATION://请求出入金操作
                        return RequestTemplate<MGRCashOperationRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATEACCOUNTCATEGORY://请求修改帐户类别
                        return RequestTemplate<MGRUpdateCategoryRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATEACCOUNTINTRADAY://请求修改日内参数
                        return RequestTemplate<MGRUpdateIntradayRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATEACCOUNTROUTETRANSFERTYPE://请求修改路由类别
                        return RequestTemplate<MGRUpdateRouteTypeRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATEACCOUNTEXECUTE://请求修改帐户交易权限
                        return RequestTemplate<MGRUpdateExecuteRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGROPENCLEARCENTRE://请求开启清算中心
                        return RequestTemplate<MGRReqOpenClearCentreRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRCLOSECLEARCENTRE://请求关闭清算中心
                        return RequestTemplate<MGRReqCloseClearCentreRequest>.SrvRecvRequest(frontid, clientid, content);
                    //case MessageTypes.MGRQRYCONNECTOR://请求查询通道列表
                    //    return RequestTemplate<MGRQryConnectorRequest>.SrvRecvRequest(frontid, clientid, content);
                    //case MessageTypes.MGRSTARTBROKER://请求启动成交通道
                    //    return RequestTemplate<MGRReqStartBrokerRequest>.SrvRecvRequest(frontid, clientid, content);
                    //case MessageTypes.MGRSTOPBROKER://请求停止成交通道
                    //    return RequestTemplate<MGRReqStopBrokerRequest>.SrvRecvRequest(frontid, clientid, content);
                    //case MessageTypes.MGRSTARTDATAFEED://请求启动行情通道
                    //    return RequestTemplate<MGRReqStartDataFeedRequest>.SrvRecvRequest(frontid,clientid,content);
                    //case MessageTypes.MGRSTOPDATAFEED://请求停止行情通道
                    //    return RequestTemplate<MGRReqStopDataFeedRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRADDACCOUNT://请求添加交易帐号
                        return RequestTemplate<MGRAddAccountRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYEXCHANGE://请求查询交易所
                        return RequestTemplate<MGRQryExchangeRequuest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYMARKETTIME://请求查询交易时间段
                        return RequestTemplate<MGRQryMarketTimeRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYSECURITY://请求查询品种列表
                        return RequestTemplate<MGRQrySecurityRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATESECURITY://更新品种信息
                        return RequestTemplate<MGRUpdateSecurityRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYSYMBOL://请求查询合约列表
                        return RequestTemplate<MGRQrySymbolRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATESYMBOL://请求更新合约
                        return RequestTemplate<MGRUpdateSymbolRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYRULECLASS://请求风控规则列表
                        return RequestTemplate<MGRQryRuleSetRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATERULEITEM://请求更新风控规则
                        return RequestTemplate<MGRUpdateRuleRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYRULEITEM://请求查询帐户风控项
                        return RequestTemplate<MGRQryRuleItemRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRDELRULEITEM://请求删除帐户风控项
                        return RequestTemplate<MGRDelRuleItemRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYSYSTEMSTATUS://请求查询系统状态
                        return RequestTemplate<MGRQrySystemStatusRequest>.SrvRecvRequest(frontid,clientid,content);
                    case MessageTypes.MGRQRYORDER://请求查询历史委托
                        return RequestTemplate<MGRQryOrderRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYTRADE://请求查询历史成交
                        return RequestTemplate<MGRQryTradeRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYPOSITION://请求查询结算持仓
                        return RequestTemplate<MGRQryPositionRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYCASH://请求查询出入金记录
                        return RequestTemplate<MGRQryCashRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYSETTLEMENT://请求查询结算单
                        return RequestTemplate<MGRQrySettleRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRCHANGEACCOUNTPASS://请求修改帐户密码
                        return RequestTemplate<MGRChangeAccountPassRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRADDSECURITY://请求添加品种
                        return RequestTemplate<MGRReqAddSecurityRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRADDSYMBOL://请求添加合约
                        return RequestTemplate<MGRReqAddSymbolRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRCHANGEINVESTOR://请求修改投资者信息
                        return RequestTemplate<MGRReqChangeInvestorRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATEPOSLOCK://请求修改帐户锁仓权限
                        return RequestTemplate<MGRReqUpdatePosLockRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYMANAGER://查询管理员列表
                        return RequestTemplate<MGRQryManagerRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRADDMANAGER://请求添加管理员
                        return RequestTemplate<MGRReqAddManagerRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATEMANAGER://请求更新管理员
                        return RequestTemplate<MGRReqUpdateManagerRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRQRYACCTSERVICE://请求查询帐户服务
                        return RequestTemplate<MGRQryAcctServiceRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRCONTRIBREQUEST://扩展请求
                        return RequestTemplate<MGRContribRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRUPDATEPASS://请求修改密码
                        return RequestTemplate<MGRUpdatePassRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRINSERTTRADE://请求插入成交
                        return RequestTemplate<MGRReqInsertTradeRequest>.SrvRecvRequest(frontid, clientid, content);
                    case MessageTypes.MGRDELACCOUNT://请求删除帐户
                        return RequestTemplate<MGRReqDelAccountRequest>.SrvRecvRequest(frontid, clientid, content);
                    #endregion

                    default:
                        throw new PacketTypeNotAvabile(type, content, frontid, clientid);
                }
            }
            catch (Exception ex)
            {
                throw new PacketParseError(ex, type, content,frontid,clientid);
            }
        }

        public static IPacket CliRecvResponse(MessageTypes type, string content)
        {
            switch (type)
            { 
                case MessageTypes.LOGICLIVERESPONSE:
                    return ResponseTemplate<LogicLiveResponse>.CliRecvResponse(content);
                case MessageTypes.FEATURERESPONSE:
                    return ResponseTemplate<FeatureResponse>.CliRecvResponse(content);
                case MessageTypes.VERSIONRESPONSE:
                    return ResponseTemplate<VersionResponse>.CliRecvResponse(content);
                case MessageTypes.HEARTBEATRESPONSE:
                    return ResponseTemplate<HeartBeatResponse>.CliRecvResponse(content);
                case MessageTypes.LOGINRESPONSE:
                    return ResponseTemplate<LoginResponse>.CliRecvResponse(content);
                case MessageTypes.BROKERNAMERESPONSE:
                    return ResponseTemplate<BrokerNameResponse>.CliRecvResponse(content);

                case MessageTypes.ORDERNOTIFY://委托通知
                    return ResponseTemplate<OrderNotify>.CliRecvResponse(content);
                case MessageTypes.ERRORORDERNOTIFY://委托错误通知
                    return ResponseTemplate<ErrorOrderNotify>.CliRecvResponse(content);
                case MessageTypes.EXECUTENOTIFY://成交通知
                    return ResponseTemplate<TradeNotify>.CliRecvResponse(content);
                case MessageTypes.POSITIONUPDATENOTIFY://持仓更新通知
                    return ResponseTemplate<PositionNotify>.CliRecvResponse(content);
                case MessageTypes.OLDPOSITIONNOTIFY://隔夜持仓通知
                    return ResponseTemplate<HoldPositionNotify>.CliRecvResponse(content);
                case MessageTypes.ORDERACTIONNOTIFY://委托操作通知
                    return ResponseTemplate<OrderActionNotify>.CliRecvResponse(content);
                case MessageTypes.ERRORORDERACTIONNOTIFY://委托操作错误通知
                    return ResponseTemplate<ErrorOrderActionNotify>.CliRecvResponse(content);
                case MessageTypes.CASHOPERATIONNOTIFY://出入金操作通知
                    return ResponseTemplate<CashOperationNotify>.CliRecvResponse(content);
                case MessageTypes.TRADINGNOTICENOTIFY://交易通知
                    return ResponseTemplate<TradingNoticeNotify>.CliRecvResponse(content);

                case MessageTypes.ORDERRESPONSE://查询委托回报
                    return ResponseTemplate<RspQryOrderResponse>.CliRecvResponse(content);
                case MessageTypes.TRADERESPONSE://成交查询回报
                    return ResponseTemplate<RspQryTradeResponse>.CliRecvResponse(content);
                case MessageTypes.POSITIONRESPONSE://持仓查询回报
                    return ResponseTemplate<RspQryPositionResponse>.CliRecvResponse(content);
                case MessageTypes.POSITIONDETAILRESPONSE://查询持仓明细回报
                    return ResponseTemplate<RspQryPositionDetailResponse>.CliRecvResponse(content);
                case MessageTypes.SYMBOLRESPONSE://合约查询回报
                    return ResponseTemplate<RspQrySymbolResponse>.CliRecvResponse(content);
                case MessageTypes.SETTLEINFORESPONSE://结算信息回报
                    return ResponseTemplate<RspQrySettleInfoResponse>.CliRecvResponse(content);
                case MessageTypes.SETTLEINFOCONFIRMRESPONSE://结算确认回报
                    return ResponseTemplate<RspQrySettleInfoConfirmResponse>.CliRecvResponse(content);
                case MessageTypes.CONFIRMSETTLEMENTRESPONSE://确认结算回报
                    return ResponseTemplate<RspConfirmSettlementResponse>.CliRecvResponse(content);
                case MessageTypes.MAXORDERVOLRESPONSE://可下单手数回报
                    return ResponseTemplate<RspQryMaxOrderVolResponse>.CliRecvResponse(content);
                case MessageTypes.ACCOUNTINFORESPONSE://帐户信息查询
                    return ResponseTemplate<RspQryAccountInfoResponse>.CliRecvResponse(content);
                case MessageTypes.INVESTORRESPONSE://交易者信息查询
                    return ResponseTemplate<RspQryInvestorResponse>.CliRecvResponse(content);
                case MessageTypes.CONTRIBRESPONSE:
                    return ResponseTemplate<RspContribResponse>.CliRecvResponse(content);

                case MessageTypes.CHANGEPASSRESPONSE://修改密码回报
                    return ResponseTemplate<RspReqChangePasswordResponse>.CliRecvResponse(content);
                case MessageTypes.NOTICERESPONSE://查询系统通知回报
                    return ResponseTemplate<RspQryNoticeResponse>.CliRecvResponse(content);
                case MessageTypes.CONTRACTBANKRESPONSE://查询签约银行通知回报
                    return ResponseTemplate<RspQryContractBankResponse>.CliRecvResponse(content);
                case MessageTypes.REGISTERBANKACCOUNTRESPONSE://查询银行帐户回报
                    return ResponseTemplate<RspQryRegisterBankAccountResponse>.CliRecvResponse(content);
                case MessageTypes.TRANSFERSERIALRESPONSE://查询出入金流水回报
                    return ResponseTemplate<RspQryTransferSerialResponse>.CliRecvResponse(content);
                case MessageTypes.INSTRUMENTCOMMISSIONRATERESPONSE://查询合约手续费率
                    return ResponseTemplate<RspQryInstrumentCommissionRateResponse>.CliRecvResponse(content);
                case MessageTypes.INSTRUMENTMARGINRATERESPONSE://查询保证金率
                    return ResponseTemplate<RspQryInstrumentMarginRateResponse>.CliRecvResponse(content);
                case MessageTypes.MARKETDATARESPONSE://查询市场行情回报
                    return ResponseTemplate<RspQryMarketDataResponse>.CliRecvResponse(content);
                
                case MessageTypes.TICKNOTIFY:
                    TickNotify ticknotify = new TickNotify();
                    ticknotify.Tick = TickImpl.Deserialize(content);
                    return ticknotify;
                case MessageTypes.TICKHEARTBEAT:
                    TickHeartBeatResponse tickhb = new TickHeartBeatResponse();
                    return tickhb;

                #region manager
                case MessageTypes.MGROPERATIONRESPONSE:
                    return ResponseTemplate<RspMGROperationResponse>.CliRecvResponse(content);
                case MessageTypes.MGRLOGINRESPONSE://登入回报
                    return ResponseTemplate<RspMGRLoginResponse>.CliRecvResponse(content);
                case MessageTypes.MGRQRYACCOUNTSRESPONSE://查询帐户列表回报
                    return ResponseTemplate<RspMGRQryAccountResponse>.CliRecvResponse(content);
                case MessageTypes.MGRACCOUNTINFOLITENOTIFY://帐户InfoLite通知回报
                    return ResponseTemplate<NotifyMGRAccountInfoLiteResponse>.CliRecvResponse(content);
                case MessageTypes.MGRRESUMEACCOUNTRESPONE://恢复交易帐户日内交易信息回报
                    return ResponseTemplate<RspMGRResumeAccountResponse>.CliRecvResponse(content);
                case MessageTypes.MGRSESSIONSTATUSUPDATE://交易帐号登入 退出 事件回报
                    return ResponseTemplate<NotifyMGRSessionUpdateNotify>.CliRecvResponse(content);
                case MessageTypes.MGRACCOUNTINFORESPONSE://查询交易帐户信息回报
                    return ResponseTemplate<RspMGRQryAccountInfoResponse>.CliRecvResponse(content);
                case MessageTypes.MGRACCOUNTCHANGEUPDATE://帐户变动回报
                    return ResponseTemplate<NotifyMGRAccountChangeUpdateResponse>.CliRecvResponse(content);
                //case MessageTypes.MGRCONNECTORRESPONSE://查询通道回报
                //    return ResponseTemplate<RspMGRQryConnectorResponse>.CliRecvResponse(content);
                case MessageTypes.MGREXCHANGERESPONSE://查询交易所回报
                    return ResponseTemplate<RspMGRQryExchangeResponse>.CliRecvResponse(content);
                case MessageTypes.MGRMARKETTIMERESPONSE://查询交易时间段回报
                    return ResponseTemplate<RspMGRQryMarketTimeResponse>.CliRecvResponse(content);
                case MessageTypes.MGRSECURITYRESPONSE://查询品种回报
                    return ResponseTemplate<RspMGRQrySecurityResponse>.CliRecvResponse(content);
                case MessageTypes.MGRSYMBOLRESPONSE://查询合约回报
                    return ResponseTemplate<RspMGRQrySymbolResponse>.CliRecvResponse(content);
                case MessageTypes.MGRRULECLASSRESPONSE://风控规则回报
                    return ResponseTemplate<RspMGRQryRuleSetResponse>.CliRecvResponse(content);
                case MessageTypes.MGRRULEITEMRESPONSE://查询风控项目回报
                    return ResponseTemplate<RspMGRQryRuleItemResponse>.CliRecvResponse(content);
                case MessageTypes.MGRUPDATERULEITEMRESPONSE://更新风控项目回报
                    return ResponseTemplate<RspMGRUpdateRuleResponse>.CliRecvResponse(content);
                case MessageTypes.MGRDELRULEITEMRESPONSE://删除风控规则项目回报
                    return ResponseTemplate<RspMGRDelRuleItemResponse>.CliRecvResponse(content);
                case MessageTypes.MGRSYSTEMSTATUSRESPONSE://请求系统状态回报
                    return ResponseTemplate<RspMGRQrySystemStatusResponse>.CliRecvResponse(content);
                case MessageTypes.MGRORDERRESPONSE://请求查询历史委托回报
                    return ResponseTemplate<RspMGRQryOrderResponse>.CliRecvResponse(content);
                case MessageTypes.MGRTRADERESPONSE://请求查询历史成交回报
                    return ResponseTemplate<RspMGRQryTradeResponse>.CliRecvResponse(content);
                case MessageTypes.MGRPOSITIONRESPONSE://请求查询历史持仓回报
                    return ResponseTemplate<RspMGRQryPositionResponse>.CliRecvResponse(content);
                case MessageTypes.MGRCASHRESPONSE://请求出入金查询回报
                    return ResponseTemplate<RspMGRQryCashResponse>.CliRecvResponse(content);
                case MessageTypes.MGRSETTLEMENTRESPONSE://请求查询结算单回报
                    return ResponseTemplate<RspMGRQrySettleResponse>.CliRecvResponse(content);
                //case MessageTypes.MGRCHANGEACCOUNTPASSRESPONSE://请求修改帐户密码回报
                //    return ResponseTemplate<RspMGRChangeAccountPassResponse>.CliRecvResponse(content);
                case MessageTypes.MGRADDSECURITYRESPONSE://请求添加品种回报
                    return ResponseTemplate<RspMGRReqAddSecurityResponse>.CliRecvResponse(content);
                case MessageTypes.MGRADDSYMBOLRESPONSE://请求添加合约回报
                    return ResponseTemplate<RspMGRReqAddSymbolResponse>.CliRecvResponse(content);
                case MessageTypes.MGRCHANGEINVESTOR://请求修改投资者信息
                    return ResponseTemplate<RspMGRReqChangeInvestorResponse>.CliRecvResponse(content);
                case MessageTypes.MGRUPDATEPOSLOCKRESPONSE://请求修改帐户锁仓权限回报
                    return ResponseTemplate<RspMGRReqUpdatePosLockResponse>.CliRecvResponse(content);
                case MessageTypes.MGRMANAGERRESPONSE://查询管理员列表回报
                    return ResponseTemplate<RspMGRQryManagerResponse>.CliRecvResponse(content);
                case MessageTypes.MGRQRYACCTSERVICERESPONSE://查询帐户服务回报
                    return ResponseTemplate<RspMGRQryAcctServiceResponse>.CliRecvResponse(content);
                case MessageTypes.MGRCONTRIBRESPONSE://扩展回报
                    return ResponseTemplate<RspMGRContribResponse>.CliRecvResponse(content);
                case MessageTypes.MGRCONTRIBRNOTIFY://扩展回报
                    return ResponseTemplate<NotifyMGRContribNotify>.CliRecvResponse(content);
                #endregion
                default:
                    throw new PacketError();
            }
            
        }
    }
    public class RequestTemplate<T>
        where T : RequestPacket, new()
    {
        /// <summary>
        /// 生成请求Packet
        /// 将前置地址,客户端ID,以及数据内容生成对应的packet
        /// </summary>
        /// <param name="frontid"></param>
        /// <param name="clientid"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T SrvRecvRequest(string frontid, string clientid, string content)
        {
            T packet = new T();
            packet.SetSource(frontid, clientid);
            packet.Deserialize(content);
            return packet;
        }
        public static T CliSendRequest(int requestid)
        {
            T package = new T();
            package.SetRequestID(requestid);
            return package;
        }
    }

    /// <summary>
    /// 逻辑数据包模板
    /// PacketBase 是所有数据包的父类
    /// 这里需要确定子类和父类的构造函数的相关调用顺序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseTemplate<T>
        where T : ResponsePacket, new()
    {
        /// <summary>
        /// 从Requestpacket生成Responsepacket
        /// 然后再有处理逻辑填充对应的参数
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public static T SrvSendRspResponse(RequestPacket request)
        {
            T packet = new T();
            packet.BindRequest(request);
            return packet;
        }

        public static T SrvSendRspResponse(ISession session)
        {
            T packet = new T();
            packet.BindSession(session);
            return packet;
        }

        public static T SrvSendNotifyResponse(string account)
        {
            T packet = new T();
            packet.BindAccount(account);
            return packet;
        }

        public static T SrvSendNotifyResponse(ILocation location)
        {
            return SrvSendNotifyResponse(new ILocation[] { location });
        }

        public static T SrvSendNotifyResponse(IEnumerable<ILocation> locations)
        {
            T packet = new T();
            packet.BindLocation(locations);
            return packet;
        }

        public static T CliRecvResponse(string content)
        {
            T packet = new T();
            packet.Deserialize(content);
            return packet;
        }
    }


}
