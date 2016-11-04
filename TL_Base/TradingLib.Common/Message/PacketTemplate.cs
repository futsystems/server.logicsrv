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
        public static IPacket SrvRecvRequest(Message message,string frontid, string clientid)
        {
            try
            {
                switch (message.Type)
                {

                    case MessageTypes.SERVICEREQUEST:
                        return RequestTemplate<QryServiceRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //逻辑活动请求
                    case MessageTypes.LOGICLIVEREQUEST:
                        return RequestTemplate<LogicLiveRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //客户端注册
                    case MessageTypes.REGISTERCLIENT:
                        return RequestTemplate<RegisterClientRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //请求注销
                    case MessageTypes.CLEARCLIENT:
                        return RequestTemplate<UnregisterClientRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //发送心跳
                    case MessageTypes.HEARTBEAT:
                        return RequestTemplate<HeartBeat>.SrvRecvRequest(frontid, clientid, message.Content);

                    //功能码请求
                    case MessageTypes.FEATUREREQUEST:
                        return RequestTemplate<FeatureRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //版本查询与连接初始化
                    case MessageTypes.VERSIONREQUEST:
                        return RequestTemplate<VersionRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //发送心跳请求
                    case MessageTypes.HEARTBEATREQUEST:
                        return RequestTemplate<HeartBeatRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //请求登入
                    case MessageTypes.LOGINREQUEST:
                        return RequestTemplate<LoginRequest>.SrvRecvRequest(frontid, clientid, message.Content);


                    //服务查询
                    case MessageTypes.BROKERNAMEREQUEST:
                        return RequestTemplate<BrokerNameRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //注册合约
                    case MessageTypes.REGISTERSYMTICK:
                        return RequestTemplate<RegisterSymbolTickRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //注销合约
                    case MessageTypes.UNREGISTERSYMTICK:
                        return RequestTemplate<UnregisterSymbolTickRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //发送委托
                    case MessageTypes.SENDORDER:
                        return RequestTemplate<OrderInsertRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //发送委托操作
                    case MessageTypes.SENDORDERACTION:
                        return RequestTemplate<OrderActionRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询交易员
                    case MessageTypes.QRYINVESTOR:
                        return RequestTemplate<QryInvestorRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //账户信息查询
                    case MessageTypes.QRYACCOUNTINFO:
                        return RequestTemplate<QryAccountInfoRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询合约
                    case MessageTypes.QRYSYMBOL:
                        return RequestTemplate<QrySymbolRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询结算确认
                    case MessageTypes.QRYSETTLEINFOCONFIRM:
                        return RequestTemplate<QrySettleInfoConfirmRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询结算信息
                    case MessageTypes.XQRYSETTLEINFO:
                        return RequestTemplate<XQrySettleInfoRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询委托
                    case MessageTypes.QRYORDER:
                        return RequestTemplate<QryOrderRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询成交
                    case MessageTypes.QRYTRADE:
                        return RequestTemplate<QryTradeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询持仓
                    case MessageTypes.QRYPOSITION:
                        return RequestTemplate<QryPositionRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询持仓明细
                    case MessageTypes.QRYPOSITIONDETAIL:
                        return RequestTemplate<QryPositionDetailRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询最大保单
                    case MessageTypes.QRYMAXORDERVOL:
                        return RequestTemplate<QryMaxOrderVolRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //确认结算单
                    case MessageTypes.CONFIRMSETTLEMENT:
                        return RequestTemplate<ConfirmSettlementRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询历史行情
                    case MessageTypes.BARREQUEST:
                        return RequestTemplate<QryBarRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //扩展命令请求
                    case MessageTypes.CONTRIBREQUEST:
                        return RequestTemplate<ContribRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //请求修改密码
                    case MessageTypes.REQCHANGEPASS:
                        return RequestTemplate<ReqChangePasswordRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //请求查询系统通知
                    case MessageTypes.QRYNOTICE:
                        return RequestTemplate<QryNoticeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //请求查询签约银行列表
                    case MessageTypes.QRYCONTRACTBANK:
                        return RequestTemplate<QryContractBankRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //请求查询银行帐户
                    case MessageTypes.QRYREGISTERBANKACCOUNT:
                        return RequestTemplate<QryRegisterBankAccountRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询出入金流水记录
                    case MessageTypes.QRYTRANSFERSERIAL:
                        return RequestTemplate<QryTransferSerialRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询合约手续费率
                    case MessageTypes.QRYINSTRUMENTCOMMISSIONRATE:
                        return RequestTemplate<QryInstrumentCommissionRateRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询合约保证金率
                    case MessageTypes.QRYINSTRUMENTMARGINRATE:
                        return RequestTemplate<QryInstrumentMarginRateRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询市场行情
                    case MessageTypes.QRYMARKETDATA:
                        return RequestTemplate<QryMarketDataRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询交易参数
                    case MessageTypes.QRYTRADINGPARAMS:
                        return RequestTemplate<QryTradingParamsRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询交易时间段
                    case MessageTypes.XQRYMARKETTIME:
                        return RequestTemplate<XQryMarketTimeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询交易所
                    case MessageTypes.XQRYEXCHANGE:
                        return RequestTemplate<XQryExchangeRequuest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询品种
                    case MessageTypes.XQRYSECURITY:
                        return RequestTemplate<XQrySecurityRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询合约
                    case MessageTypes.XQRYSYMBOL:
                        return RequestTemplate<XQrySymbolRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询隔夜持仓
                    case MessageTypes.XQRYYDPOSITION:
                        return RequestTemplate<XQryYDPositionRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询委托
                    case MessageTypes.XQRYORDER:
                        return RequestTemplate<XQryOrderRequest>.SrvRecvRequest(frontid,clientid,message.Content);
                    //查询成交
                    case MessageTypes.XQRYTRADE:
                        return RequestTemplate<XQryTradeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //更新地址信息
                    case MessageTypes.UPDATELOCATION:
                        return RequestTemplate<UpdateLocationInfoRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询行情快照
                    case MessageTypes.XQRYTICKSNAPSHOT:
                        return RequestTemplate<XQryTickSnapShotRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询交易账户
                    case MessageTypes.XQRYACCOUNT:
                        return RequestTemplate<XQryAccountRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询最大下单量
                    case MessageTypes.XQRYMAXORDERVOL:
                        return RequestTemplate<XQryMaxOrderVolRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询账户财务信息
                    case MessageTypes.XQRYACCOUNTFINANCE:
                        return RequestTemplate<XQryAccountFinanceRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //查询汇率信息
                    case MessageTypes.XQRYEXCHANGERATE:
                        return RequestTemplate<XQryExchangeRateRequest>.SrvRecvRequest(frontid, clientid, message.Content);

                    #region manager
                    case MessageTypes.MGRLOGINREQUEST://请求登入
                        return RequestTemplate<MGRLoginRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYACCOUNTS://请求帐户列表
                        return RequestTemplate<MGRQryAccountRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRWATCHACCOUNTS://请求设定观察帐户列表
                        return RequestTemplate<MGRWatchAccountRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRRESUMEACCOUNT://请求恢复交易帐号日内交易信息
                        return RequestTemplate<MGRResumeAccountRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYACCOUNTINFO://请求查询交易帐号信息
                        return RequestTemplate<MGRQryAccountInfoRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRCASHOPERATION://请求出入金操作
                        return RequestTemplate<MGRCashOperationRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATEACCOUNTCATEGORY://请求修改帐户类别
                        return RequestTemplate<MGRUpdateCategoryRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATEACCOUNTINTRADAY://请求修改日内参数
                        return RequestTemplate<MGRUpdateIntradayRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATEACCOUNTROUTETRANSFERTYPE://请求修改路由类别
                        return RequestTemplate<MGRUpdateRouteTypeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATEACCOUNTEXECUTE://请求修改帐户交易权限
                        return RequestTemplate<MGRUpdateExecuteRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGROPENCLEARCENTRE://请求开启清算中心
                        return RequestTemplate<MGRReqOpenClearCentreRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRCLOSECLEARCENTRE://请求关闭清算中心
                        return RequestTemplate<MGRReqCloseClearCentreRequest>.SrvRecvRequest(frontid, clientid, message.Content);

                    //case MessageTypes.MGRQRYCONNECTOR://请求查询通道列表
                    //    return RequestTemplate<MGRQryConnectorRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //case MessageTypes.MGRSTARTBROKER://请求启动成交通道
                    //    return RequestTemplate<MGRReqStartBrokerRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //case MessageTypes.MGRSTOPBROKER://请求停止成交通道
                    //    return RequestTemplate<MGRReqStopBrokerRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //case MessageTypes.MGRSTARTDATAFEED://请求启动行情通道
                    //    return RequestTemplate<MGRReqStartDataFeedRequest>.SrvRecvRequest(frontid,clientid,message.Content);
                    //case MessageTypes.MGRSTOPDATAFEED://请求停止行情通道
                    //    return RequestTemplate<MGRReqStopDataFeedRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRADDACCOUNT://请求添加交易帐号
                        return RequestTemplate<MGRAddAccountRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYEXCHANGE://请求查询交易所
                        return RequestTemplate<MGRQryExchangeRequuest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATEEXCHANGE://请求更新交易所
                        return RequestTemplate<MGRUpdateExchangeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYMARKETTIME://请求查询交易时间段
                        return RequestTemplate<MGRQryMarketTimeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATEMARKETTIME://请求更新交易时间段
                        return RequestTemplate<MGRUpdateMarketTimeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYSECURITY://请求查询品种列表
                        return RequestTemplate<MGRQrySecurityRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATESECURITY://更新品种信息
                        return RequestTemplate<MGRUpdateSecurityRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYSYMBOL://请求查询合约列表
                        return RequestTemplate<MGRQrySymbolRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATESYMBOL://请求更新合约
                        return RequestTemplate<MGRUpdateSymbolRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYEXCHANGERATE://请求查询汇率
                        return RequestTemplate<MGRQryExchangeRateRequuest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYRULECLASS://请求风控规则列表
                        return RequestTemplate<MGRQryRuleSetRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATERULEITEM://请求更新风控规则
                        return RequestTemplate<MGRUpdateRuleRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYRULEITEM://请求查询帐户风控项
                        return RequestTemplate<MGRQryRuleItemRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRDELRULEITEM://请求删除帐户风控项
                        return RequestTemplate<MGRDelRuleItemRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYSYSTEMSTATUS://请求查询系统状态
                        return RequestTemplate<MGRQrySystemStatusRequest>.SrvRecvRequest(frontid,clientid,message.Content);
                    case MessageTypes.MGRQRYORDER://请求查询历史委托
                        return RequestTemplate<MGRQryOrderRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYTRADE://请求查询历史成交
                        return RequestTemplate<MGRQryTradeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYPOSITION://请求查询结算持仓
                        return RequestTemplate<MGRQryPositionRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYCASH://请求查询出入金记录
                        return RequestTemplate<MGRQryCashRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYSETTLEMENT://请求查询结算单
                        return RequestTemplate<MGRQrySettleRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRCHANGEACCOUNTPASS://请求修改帐户密码
                        return RequestTemplate<MGRChangeAccountPassRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //case MessageTypes.MGRADDSECURITY://请求添加品种
                    //    return RequestTemplate<MGRReqAddSecurityRequest>.SrvRecvRequest(frontid, clientid, message.Content);

                    //case MessageTypes.MGRADDSYMBOL://请求添加合约
                    //    return RequestTemplate<MGRReqAddSymbolRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRCHANGEINVESTOR://请求修改投资者信息
                        return RequestTemplate<MGRReqChangeInvestorRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATEPOSLOCK://请求修改帐户锁仓权限
                        return RequestTemplate<MGRReqUpdatePosLockRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //case MessageTypes.MGRQRYMANAGER://查询管理员列表
                    //    return RequestTemplate<MGRQryManagerRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //case MessageTypes.MGRADDMANAGER://请求添加管理员
                    //    return RequestTemplate<MGRReqAddManagerRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    //case MessageTypes.MGRUPDATEMANAGER://请求更新管理员
                    //    return RequestTemplate<MGRReqUpdateManagerRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRQRYACCTSERVICE://请求查询帐户服务
                        return RequestTemplate<MGRQryAcctServiceRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRCONTRIBREQUEST://扩展请求
                        return RequestTemplate<MGRContribRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRUPDATEPASS://请求修改密码
                        return RequestTemplate<MGRUpdatePassRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRINSERTTRADE://请求插入成交
                        return RequestTemplate<MGRReqInsertTradeRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRDELACCOUNT://请求删除帐户
                        return RequestTemplate<MGRReqDelAccountRequest>.SrvRecvRequest(frontid, clientid, message.Content);

                    case MessageTypes.MGRQRYTICKSNAPSHOT://请求查询行情快照
                        return RequestTemplate<MGRQryTickSnapShotRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    #endregion

                    #region 行情部分
                    case MessageTypes.MGRSTARTDATAFEED://启动行情通道
                        return RequestTemplate<MDReqStartDataFeedRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRSTOPDATAFEED://停止行情通道
                        return RequestTemplate<MDReqStopDataFeedRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.MGRREGISTERSYMBOLS://注册行情
                        return RequestTemplate<MDRegisterSymbolsRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    #endregion

                    case MessageTypes.MD_DEMOTICK:
                        return RequestTemplate<MDDemoTickRequest>.SrvRecvRequest(frontid, clientid, message.Content);

                    case MessageTypes.BOSENDORDER:
                        return RequestTemplate<BOOrderInsertRequest>.SrvRecvRequest(frontid, clientid, message.Content);

                    case MessageTypes.MGRUPLOADBARDATA:
                        UploadBarDataRequest request = new UploadBarDataRequest();
                        request.DeserializeBin(message.Data);
                        return request;
                    case MessageTypes.XQRYTRADSPLIT://查询成交明细
                        return RequestTemplate<XQryTradeSplitRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.XQRYPRICEVOL://查询价格成交量分布
                        return RequestTemplate<XQryPriceVolRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    case MessageTypes.XQRYMINUTEDATA://查询分时数据
                        return RequestTemplate<XQryMinuteDataRequest>.SrvRecvRequest(frontid, clientid, message.Content);
                    default:
                        throw new PacketTypeNotAvabile(message.Type, message.Content, frontid, clientid);
                }
            }
            catch (Exception ex)
            {
                throw new PacketParseError(ex, message.Type, message.Content,frontid,clientid);
            }
        }


        public static IPacket CliRecvResponse(Message message)
        {
            switch (message.Type)
            {
                case MessageTypes.BIN_BARRESPONSE:
                    {
                        RspQryBarResponseBin response = new RspQryBarResponseBin();
                        response.DeserializeBin(message.Data);
                        return response;
                    }
                case MessageTypes.XQRYTRADSPLITRESPONSE:
                    {
                        RspXQryTradeSplitResponse response = new RspXQryTradeSplitResponse();
                        response.DeserializeBin(message.Data);
                        return response;
                    }
                case MessageTypes.XQRYPRICEVOLRESPONSE:
                    {
                        RspXQryPriceVolResponse response = new RspXQryPriceVolResponse();
                        response.DeserializeBin(message.Data);
                        return response;
                    }
                case MessageTypes.XQRYMINUTEDATARESPONSE:
                    {
                        RspXQryMinuteDataResponse response = new RspXQryMinuteDataResponse();
                        response.DeserializeBin(message.Data);
                        return response;
                    }

                case MessageTypes.SERVICERESPONSE:
                    return ResponseTemplate<RspQryServiceResponse>.CliRecvResponse(message);
                case MessageTypes.REGISTERCLIENTRESPONSE:
                    return ResponseTemplate<RspRegisterClientResponse>.CliRecvResponse(message);
                case MessageTypes.LOGICLIVERESPONSE:
                    return ResponseTemplate<LogicLiveResponse>.CliRecvResponse(message);
                case MessageTypes.FEATURERESPONSE:
                    return ResponseTemplate<FeatureResponse>.CliRecvResponse(message);
                case MessageTypes.VERSIONRESPONSE:
                    return ResponseTemplate<VersionResponse>.CliRecvResponse(message);
                case MessageTypes.HEARTBEATRESPONSE:
                    return ResponseTemplate<HeartBeatResponse>.CliRecvResponse(message);
                case MessageTypes.LOGINRESPONSE:
                    return ResponseTemplate<LoginResponse>.CliRecvResponse(message);
                case MessageTypes.BROKERNAMERESPONSE:
                    return ResponseTemplate<BrokerNameResponse>.CliRecvResponse(message);

                case MessageTypes.ORDERNOTIFY://委托通知
                    return ResponseTemplate<OrderNotify>.CliRecvResponse(message);
                case MessageTypes.ERRORORDERNOTIFY://委托错误通知
                    return ResponseTemplate<ErrorOrderNotify>.CliRecvResponse(message);
                case MessageTypes.EXECUTENOTIFY://成交通知
                    return ResponseTemplate<TradeNotify>.CliRecvResponse(message);
                case MessageTypes.POSITIONUPDATENOTIFY://持仓更新通知
                    return ResponseTemplate<PositionNotify>.CliRecvResponse(message);
                case MessageTypes.OLDPOSITIONNOTIFY://隔夜持仓通知
                    return ResponseTemplate<HoldPositionNotify>.CliRecvResponse(message);
                case MessageTypes.ORDERACTIONNOTIFY://委托操作通知
                    return ResponseTemplate<OrderActionNotify>.CliRecvResponse(message);
                case MessageTypes.ERRORORDERACTIONNOTIFY://委托操作错误通知
                    return ResponseTemplate<ErrorOrderActionNotify>.CliRecvResponse(message);
                case MessageTypes.CASHOPERATIONNOTIFY://出入金操作通知
                    return ResponseTemplate<CashOperationNotify>.CliRecvResponse(message);
                case MessageTypes.TRADINGNOTICENOTIFY://交易通知
                    return ResponseTemplate<TradingNoticeNotify>.CliRecvResponse(message);

                case MessageTypes.ORDERRESPONSE://查询委托回报
                    return ResponseTemplate<RspQryOrderResponse>.CliRecvResponse(message);
                case MessageTypes.TRADERESPONSE://成交查询回报
                    return ResponseTemplate<RspQryTradeResponse>.CliRecvResponse(message);
                case MessageTypes.POSITIONRESPONSE://持仓查询回报
                    return ResponseTemplate<RspQryPositionResponse>.CliRecvResponse(message);
                case MessageTypes.POSITIONDETAILRESPONSE://查询持仓明细回报
                    return ResponseTemplate<RspQryPositionDetailResponse>.CliRecvResponse(message);
                case MessageTypes.SYMBOLRESPONSE://合约查询回报
                    return ResponseTemplate<RspQrySymbolResponse>.CliRecvResponse(message);
                case MessageTypes.XSETTLEINFORESPONSE://结算信息回报
                    return ResponseTemplate<RspXQrySettleInfoResponse>.CliRecvResponse(message);
                case MessageTypes.BARRESPONSE://历史数据回报
                    return ResponseTemplate<RspQryBarResponse>.CliRecvResponse(message);
                case MessageTypes.SETTLEINFOCONFIRMRESPONSE://结算确认回报
                    return ResponseTemplate<RspQrySettleInfoConfirmResponse>.CliRecvResponse(message);
                case MessageTypes.CONFIRMSETTLEMENTRESPONSE://确认结算回报
                    return ResponseTemplate<RspConfirmSettlementResponse>.CliRecvResponse(message);
                case MessageTypes.MAXORDERVOLRESPONSE://可下单手数回报
                    return ResponseTemplate<RspQryMaxOrderVolResponse>.CliRecvResponse(message);
                case MessageTypes.ACCOUNTINFORESPONSE://帐户信息查询
                    return ResponseTemplate<RspQryAccountInfoResponse>.CliRecvResponse(message);
                case MessageTypes.INVESTORRESPONSE://交易者信息查询
                    return ResponseTemplate<RspQryInvestorResponse>.CliRecvResponse(message);
                case MessageTypes.CONTRIBRESPONSE:
                    return ResponseTemplate<RspContribResponse>.CliRecvResponse(message);

                case MessageTypes.CHANGEPASSRESPONSE://修改密码回报
                    return ResponseTemplate<RspReqChangePasswordResponse>.CliRecvResponse(message);
                case MessageTypes.NOTICERESPONSE://查询系统通知回报
                    return ResponseTemplate<RspQryNoticeResponse>.CliRecvResponse(message);
                case MessageTypes.CONTRACTBANKRESPONSE://查询签约银行通知回报
                    return ResponseTemplate<RspQryContractBankResponse>.CliRecvResponse(message);
                case MessageTypes.REGISTERBANKACCOUNTRESPONSE://查询银行帐户回报
                    return ResponseTemplate<RspQryRegisterBankAccountResponse>.CliRecvResponse(message);
                case MessageTypes.TRANSFERSERIALRESPONSE://查询出入金流水回报
                    return ResponseTemplate<RspQryTransferSerialResponse>.CliRecvResponse(message);
                case MessageTypes.INSTRUMENTCOMMISSIONRATERESPONSE://查询合约手续费率
                    return ResponseTemplate<RspQryInstrumentCommissionRateResponse>.CliRecvResponse(message);
                case MessageTypes.INSTRUMENTMARGINRATERESPONSE://查询保证金率
                    return ResponseTemplate<RspQryInstrumentMarginRateResponse>.CliRecvResponse(message);
                case MessageTypes.MARKETDATARESPONSE://查询市场行情回报
                    return ResponseTemplate<RspQryMarketDataResponse>.CliRecvResponse(message);
                case MessageTypes.TRADINGPARAMSRESPONSE://交易参数回报
                    return ResponseTemplate<RspQryTradingParamsResponse>.CliRecvResponse(message);

                case MessageTypes.XMARKETTIMERESPONSE://交易时间回报
                    return ResponseTemplate<RspXQryMarketTimeResponse>.CliRecvResponse(message);
                case MessageTypes.XEXCHANGERESPNSE://交易所回报
                    return ResponseTemplate<RspXQryExchangeResponse>.CliRecvResponse(message);
                case MessageTypes.XSECURITYRESPONSE://品种回报
                    return ResponseTemplate<RspXQrySecurityResponse>.CliRecvResponse(message);
                case MessageTypes.XSYMBOLRESPONSE://合约回报
                    return ResponseTemplate<RspXQrySymbolResponse>.CliRecvResponse(message);
                case MessageTypes.XYDPOSITIONRESPONSE://持仓回报
                    return ResponseTemplate<RspXQryYDPositionResponse>.CliRecvResponse(message);
                case MessageTypes.XORDERRESPONSE://委托回报
                    return ResponseTemplate<RspXQryOrderResponse>.CliRecvResponse(message);
                case MessageTypes.XTRADERESPONSE://成交回报
                    return ResponseTemplate<RspXQryTradeResponse>.CliRecvResponse(message);
                case MessageTypes.XTICKSNAPSHOTRESPONSE://行情快照回报
                    return ResponseTemplate<RspXQryTickSnapShotResponse>.CliRecvResponse(message);
                case MessageTypes.XACCOUNTRESPONSE://交易账户回报
                    return ResponseTemplate<RspXQryAccountResponse>.CliRecvResponse(message);
                case MessageTypes.XQRYMAXORDERVOLRESPONSE://最大下单数量回报
                    return ResponseTemplate<RspXQryMaxOrderVolResponse>.CliRecvResponse(message);
                case  MessageTypes.XQRYACCOUNTFINANCERESPONSE://账户财务数据回报
                    return ResponseTemplate<RspXQryAccountFinanceResponse>.CliRecvResponse(message);
                case MessageTypes.XQRYEXCHANGERATERESPONSE://汇率数据回报
                    return ResponseTemplate<RspXQryExchangeRateResponse>.CliRecvResponse(message);

                case MessageTypes.TICKNOTIFY:
                    return ResponseTemplate<TickNotify>.CliRecvResponse(message);

                case MessageTypes.TICKHEARTBEAT:
                    TickHeartBeatResponse tickhb = new TickHeartBeatResponse();
                    return tickhb;

                #region manager
                case MessageTypes.MGROPERATIONRESPONSE:
                    return ResponseTemplate<RspMGROperationResponse>.CliRecvResponse(message);
                case MessageTypes.MGRLOGINRESPONSE://登入回报
                    return ResponseTemplate<RspMGRLoginResponse>.CliRecvResponse(message);
                case MessageTypes.MGRQRYACCOUNTSRESPONSE://查询帐户列表回报
                    return ResponseTemplate<RspMGRQryAccountResponse>.CliRecvResponse(message);
                case MessageTypes.MGRACCOUNTINFOLITENOTIFY://帐户InfoLite通知回报
                    return ResponseTemplate<NotifyMGRAccountInfoLiteResponse>.CliRecvResponse(message);
                case MessageTypes.MGRRESUMEACCOUNTRESPONE://恢复交易帐户日内交易信息回报
                    return ResponseTemplate<RspMGRResumeAccountResponse>.CliRecvResponse(message);
                case MessageTypes.MGRSESSIONSTATUSUPDATE://交易帐号登入 退出 事件回报
                    return ResponseTemplate<NotifyMGRSessionUpdateNotify>.CliRecvResponse(message);
                case MessageTypes.MGRACCOUNTINFORESPONSE://查询交易帐户信息回报
                    return ResponseTemplate<RspMGRQryAccountInfoResponse>.CliRecvResponse(message);
                case MessageTypes.MGRACCOUNTCHANGEUPDATE://帐户变动回报
                    return ResponseTemplate<NotifyMGRAccountChangeUpdateResponse>.CliRecvResponse(message);
                //case MessageTypes.MGRCONNECTORRESPONSE://查询通道回报
                //    return ResponseTemplate<RspMGRQryConnectorResponse>.CliRecvResponse(message);
                case MessageTypes.MGREXCHANGERESPONSE://查询交易所回报
                    return ResponseTemplate<RspMGRQryExchangeResponse>.CliRecvResponse(message);
                case MessageTypes.MGRUPDATEEXCHANGERESPONSE://更新交易所回报
                    return ResponseTemplate<RspMGRUpdateExchangeResponse>.CliRecvResponse(message);
                case MessageTypes.MGRMARKETTIMERESPONSE://查询交易时间段回报
                    return ResponseTemplate<RspMGRQryMarketTimeResponse>.CliRecvResponse(message);
                case MessageTypes.MGRUPDATEMARKETTIMERESPONSE://更新交易时间段回报
                    return ResponseTemplate<RspMGRUpdateMarketTimeResponse>.CliRecvResponse(message);
                case MessageTypes.MGRSECURITYRESPONSE://查询品种回报
                    return ResponseTemplate<RspMGRQrySecurityResponse>.CliRecvResponse(message);
                case MessageTypes.MGRSYMBOLRESPONSE://查询合约回报
                    return ResponseTemplate<RspMGRQrySymbolResponse>.CliRecvResponse(message);
                case MessageTypes.MGRQRYEXCHANGERATERESPONSE://查询汇率回报
                    return ResponseTemplate<RspMGRQryExchangeRateResponse>.CliRecvResponse(message);
                case MessageTypes.MGRRULECLASSRESPONSE://风控规则回报
                    return ResponseTemplate<RspMGRQryRuleSetResponse>.CliRecvResponse(message);
                case MessageTypes.MGRRULEITEMRESPONSE://查询风控项目回报
                    return ResponseTemplate<RspMGRQryRuleItemResponse>.CliRecvResponse(message);
                case MessageTypes.MGRUPDATERULEITEMRESPONSE://更新风控项目回报
                    return ResponseTemplate<RspMGRUpdateRuleResponse>.CliRecvResponse(message);
                case MessageTypes.MGRDELRULEITEMRESPONSE://删除风控规则项目回报
                    return ResponseTemplate<RspMGRDelRuleItemResponse>.CliRecvResponse(message);
                case MessageTypes.MGRSYSTEMSTATUSRESPONSE://请求系统状态回报
                    return ResponseTemplate<RspMGRQrySystemStatusResponse>.CliRecvResponse(message);
                case MessageTypes.MGRORDERRESPONSE://请求查询历史委托回报
                    return ResponseTemplate<RspMGRQryOrderResponse>.CliRecvResponse(message);
                case MessageTypes.MGRTRADERESPONSE://请求查询历史成交回报
                    return ResponseTemplate<RspMGRQryTradeResponse>.CliRecvResponse(message);
                case MessageTypes.MGRPOSITIONRESPONSE://请求查询历史持仓回报
                    return ResponseTemplate<RspMGRQryPositionResponse>.CliRecvResponse(message);
                case MessageTypes.MGRCASHRESPONSE://请求出入金查询回报
                    return ResponseTemplate<RspMGRQryCashResponse>.CliRecvResponse(message);
                case MessageTypes.MGRSETTLEMENTRESPONSE://请求查询结算单回报
                    return ResponseTemplate<RspMGRQrySettleResponse>.CliRecvResponse(message);
                //case MessageTypes.MGRCHANGEACCOUNTPASSRESPONSE://请求修改帐户密码回报
                //    return ResponseTemplate<RspMGRChangeAccountPassResponse>.CliRecvResponse(message);
                //case MessageTypes.MGRADDSECURITYRESPONSE://请求添加品种回报
                //    return ResponseTemplate<RspMGRReqAddSecurityResponse>.CliRecvResponse(message);
                case MessageTypes.MGRUPDATESECURITYRESPONSE://请求更新品种
                    return ResponseTemplate<RspMGRUpdateSecurityResponse>.CliRecvResponse(message);

                //case MessageTypes.MGRADDSYMBOLRESPONSE://请求添加合约回报
                //    return ResponseTemplate<RspMGRReqAddSymbolResponse>.CliRecvResponse(message);
                case MessageTypes.MGRUPDATESYMBOLRESPONSE://请求更新合约回报
                    return ResponseTemplate<RspMGRUpdateSymbolResponse>.CliRecvResponse(message);
                case MessageTypes.MGRCHANGEINVESTOR://请求修改投资者信息
                    return ResponseTemplate<RspMGRReqChangeInvestorResponse>.CliRecvResponse(message);
                case MessageTypes.MGRUPDATEPOSLOCKRESPONSE://请求修改帐户锁仓权限回报
                    return ResponseTemplate<RspMGRReqUpdatePosLockResponse>.CliRecvResponse(message);
                case MessageTypes.MGRQRYTICKSNAPSHOTRESPONSE://请求查询行情快照
                    return ResponseTemplate<RspMGRQryTickSnapShotResponse>.CliRecvResponse(message);
                //case MessageTypes.MGRMANAGERRESPONSE://查询管理员列表回报
                //    return ResponseTemplate<RspMGRQryManagerResponse>.CliRecvResponse(message);
                case MessageTypes.MGRQRYACCTSERVICERESPONSE://查询帐户服务回报
                    return ResponseTemplate<RspMGRQryAcctServiceResponse>.CliRecvResponse(message);
                case MessageTypes.MGRCONTRIBRESPONSE://扩展回报
                    return ResponseTemplate<RspMGRContribResponse>.CliRecvResponse(message);
                case MessageTypes.MGRCONTRIBRNOTIFY://扩展回报
                    return ResponseTemplate<NotifyMGRContribNotify>.CliRecvResponse(message);
                #endregion


                #region 行情部分
                case MessageTypes.MGRQRYSYMBOLSREGISTEDRESPONSE://FeedHandler请求查询已注册合约回报
                    return ResponseTemplate<RspMDQrySymbolsRegistedResponse>.CliRecvResponse(message);
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
        /// <param name="message"></param>
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

        public static T SrvSendRspResponse(string front, string clientid, int reqId)
        {
            T packet = new T();
            packet.BindSession(front, clientid, reqId);
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

        public static T CliRecvResponse(Message message)
        {
            T packet = new T();
            packet.Deserialize(message.Content);
            return packet;
        }
    }


}
