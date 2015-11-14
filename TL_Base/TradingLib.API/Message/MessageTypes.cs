

namespace TradingLib.API
{
    /// <summary>
    /// 交易协议,消息信息类别
    /// </summary>
    public enum MessageTypes
    {
        //状态类消息
        // START  STATUS MESSAGES - DO NOT REMOVE OR RENAME MESSAGES (only add/insert)
        // IF CHANGED, MUST COPY THIS ENUM'S CONTENTS TO BROKERSERVERS\TRADELIBFAST\TRADELINK.H
        ORDER_NOT_FOUND = -112,//没有该委托
        TLCLIENT_NOT_FOUND = -111,//没有该客户端
        ACCOUNT_NOT_LOGGEDIN = -110,//无效账户
        UNKNOWN_ERROR = -109,//未知错误
        FEATURE_NOT_IMPLEMENTED = -108,//功能没有实现
        CLIENTNOTREGISTERED = -107,//客户端没有注册
        EMPTY_ORDER = -106,//空委托
        UNKNOWN_MESSAGE = -105,//未知消息
        UNKNOWN_SYMBOL = -104,//未知symbol
        BROKERSERVER_NOT_FOUND = -103,//Broker未找到
        INVALID_ORDERSIZE = -102,//无效委托数量
        DUPLICATE_ORDERID = -101,//重复委托ID
        SYMBOL_NOT_LOADED = -100,//合约没有加载
        INVALID_ORDER=-99,//无效委托
        OK = 0,//ok
        // END STATUS MESSAGES

        
        // START CUSTOM MESSAGES  - DO NOT REMOVE OR RENAME MESSAGES
        QRYENDPOINTCONNECTED = 1,//用于接入服务器查询 通过该接入服务器所连接的客户数,用于接入服务器 恢复unknow这样可以避免服务过载
        LOGICLIVEREQUEST=2,//前置与逻辑服务器之间的心跳包
        LOGICLIVERESPONSE = 3,//当服务端收到逻辑心跳包后,服务端会返回一个Response告知服务端Router端处可工作状态
        UPDATECLIENTFRONTID=4,//重启前置后，由于前置编号发生变化，需要更新原来交易客户端回话的前置地址，否则后期的通讯将会被丢弃
        FRONTSTATUSREQUEST=5,//前置机工作状态请求
        FRONTSTATUSRESPONSE=6,//前置机工作状态回报
        SERVICEREQUEST=7,//服务查询请求
        SERVICERESPONSE=8,//服务查询回报
        CUSTOM9,
        CUSTOM10,
       
        // END CUSTOM MESSAGES
        


        //////////////////////////////////////////////////////交易消息码///////////////////////////////////////////////////////////////////////////////////////////
        // START STANDARD MESSAGES
        // basic request
        REQUEST = 5000,
        VERSIONREQUEST,//版本
        BROKERNAMEREQUEST,//Broker名称
        FEATUREREQUEST,//请求功能特征
        HEARTBEATREQUEST,//请求服务端给客户端发送一个消息 已确认客户端与服务端连接有效
        HEARTBEAT,//客户端向服务端定时发送HEARTBEAT,以证明客户端存活,超过一定时间后服务端没有收到客户端心跳就会认为客户端已经死掉，注意这里是一个双向心跳机制
        REGISTERCLIENT,//注册客户端
        CLEARCLIENT,//注销客户端

        REGISTERSTOCK,//注册市场数据
        CLEARSTOCKS,//取消市场数据注册
        
        //
        SENDORDER=5100,//发送委托
        SENDORDERACTION,//请求委托取消


        //extra request
        LOGINREQUEST=5200,//登入请求
        QRYINVESTOR,//交易者信息查询
        QRYSYMBOL,//查询合约
        QRYSETTLEINFOCONFIRM,//查询结算
        QRYSETTLEINFO,
        QRYORDER,//查询委托
        QRYTRADE,//查询成交
        QRYPOSITION,//查询持仓

        QRYACCOUNTINFO,//查询交易账户信息
        QRYMAXORDERVOL,//查询最大开仓量
        QRYBAR,//请求Bar数据
        CONTRIBREQUEST,//扩展请求
        REQCHANGEPASS,//请求修改密码
        QRYNOTICE,//查询交易服务器通知
        CONFIRMSETTLEMENT,//确认结算数据
        QRYCONTRACTBANK,//查询签约银行
        QRYREGISTERBANKACCOUNT,//查询银期转账帐户
        QRYTRANSFERSERIAL,//查询转账流水
        QRYPOSITIONDETAIL,//查询持仓明细
        QRYINSTRUMENTCOMMISSIONRATE,//查询合约手续费率
        QRYINSTRUMENTMARGINRATE,//查询合约保证金率
        QRYMARKETDATA,//查询市场行情
        QRYTRADINGPARAMS,//查询交易参数

        XQRYMARKETTIME,//查询交易时间段
        XQRYEXCHANGE,//查询交易所
        XQRYSECURITY,//查询品种
        XQRYSYMBOL,//查询合约
        XQRYYDPOSITION,//查询隔夜持仓 (通过隔夜持仓数据与当日成交数据可以完全恢复一个交易帐户的交易状态)
        XQRYORDER,//查询委托
        XQRYTRADE,//查询成交
        UPDATELOCATION,//更新地址信息
        XQRYTICKSNAPSHOT,//查询行情快照

        DOMREQUEST,//请求DOM市场Level2数据
        IMBALANCEREQUEST,//imbalance..查询这个是什么意思

        // responses or acks
        RESPONSE = 6000,
        VERSIONRESPONSE,//版本回报
        BROKERNAMERESPONSE,//服务名查询回报
        FEATURERESPONSE,//功能特征回报
        HEARTBEATRESPONSE,//服务端应答客户端,如果客户端在一定时间内没有收到数据 就会触发发送heartbeatrequest,然后服务端就会发送一个response以证明客户端与服务端之间连接有效
        REGISTERCLIENTRESPONSE,//客户端注册连接回报


        TICKNOTIFY=6100,//Tick数据
        TICKHEARTBEAT,//行情心跳
        INDICATORNOTIFY,//指标通知
        OLDPOSITIONNOTIFY,//隔夜持仓回报 用于恢复日内数据时,先发送结算后的持仓,然后再发送日内交易数据 用于形成当前持仓状态[昨天+当日变动] = 当前状态
        ORDERNOTIFY,//委托回报
        ERRORORDERNOTIFY,//委托错误回报
        EXECUTENOTIFY,//成交回报
        POSITIONUPDATENOTIFY,//服务端向客户端发仓位状态信息,PC交易客户端自己计算持仓数据,网页交易客户端则需要服务端进行响应
        ORDERACTIONNOTIFY,//委托操作回报
        ERRORORDERACTIONNOTIFY,//委托操作回报
        CASHOPERATIONNOTIFY,//出入金操作回报
        TRADINGNOTICENOTIFY,//交易通知回报

        //request replay
        LOGINRESPONSE=6200,//登入回报
        INVESTORRESPONSE,//交易者信息回报
        SYMBOLRESPONSE,//合约查询回报
        SETTLEINFOCONFIRMRESPONSE,//结算确认回报
        SETTLEINFORESPONSE,
        ORDERRESPONSE,//查询委托回报
        TRADERESPONSE,//查询成交回报
        POSITIONRESPONSE,//查询持仓回报
        ACCOUNTINFORESPONSE,//交易账户信息回报
        
        ACCOUNTRESPONSE,//账户通知
        MAXORDERVOLRESPONSE,//最大可开仓回报
        BARRESPONSE,//Bar数据回报
        CONTRIBRESPONSE,//扩展回报
        CHANGEPASSRESPONSE,//修改密码回报
        NOTICERESPONSE,//交易通知回报
        CONFIRMSETTLEMENTRESPONSE,//确认结算回报
        CONTRACTBANKRESPONSE,//查询签约银行回报
        REGISTERBANKACCOUNTRESPONSE,//查询银期签约帐户回报
        TRANSFERSERIALRESPONSE,//查询转账流水回报
        POSITIONDETAILRESPONSE,//查询持仓明细回报
        INSTRUMENTCOMMISSIONRATERESPONSE,//查询合约手续费率回报
        INSTRUMENTMARGINRATERESPONSE,//查询合约保证金率回报
        MARKETDATARESPONSE,//查询市场行情回报
        TRADINGPARAMSRESPONSE,//查询交易参数回报

        XMARKETTIMERESPONSE,//查询交易时间段
        XEXCHANGERESPNSE,//查询交易所
        XSECURITYRESPONSE,//查询品种
        XSYMBOLRESPONSE,//查询合约
        XYDPOSITIONRESPONSE,//隔夜持仓回报
        XORDERRESPONSE,//委托回报
        XTRADERESPONSE,//成交回报
        //UPDATELOCATIONRESPONSE,//更新地址回报
        XTICKSNAPSHOTRESPONSE,//行情快照回报
        // END STANDARD MESSAGES




        //////////////////////////////////////////////////////管理消息码///////////////////////////////////////////////////////////////////////////////////////////
        //MGR 服务端控制 行情服务,行情通道等
        MGRSTARTTICKPUB=7000,//启动tickpub服务
        MGRSTOPTICKPUB,//停止tickpub服务
        MGRSTARTDATAFEED,//启动数据通道
        MGRSTOPDATAFEED,//停止数据通道
        MGRREGISTERSYMBOLS,//订阅行情

        // START MANAGER MESSAGES
        MGRQRYACCOUNTS=8000,//查询帐户列表
        MGRWATCHACCOUNTS,//管理端发送观察账户列表,管理服务器根据观察列表推送实时的权益以及盈亏数据
        MGRLOGINREQUEST,//管理登入请求
        MGRRESUMEACCOUNT,//管理客户端收到某个账户时 我们请求该账户的交易信息
        MGRADDACCOUNT,//增加交易账户
        MGRQRYACCOUNTINFO,//查询交易帐号信息
        MGRCASHOPERATION,//请求资金操作
        MGRUPDATEACCOUNTINTRADAY,//请求修改账户日内交易还是隔夜交易
        MGRUPDATEACCOUNTCATEGORY,//更新账户类别
        MGRUPDATEACCOUNTROUTETRANSFERTYPE,//更新账户路由转发列别
        MGRUPDATEACCOUNTEXECUTE,//更新帐户交易权限
        
        MGROPENCLEARCENTRE,//开启清算中心
        MGRCLOSECLEARCENTRE,//关闭清算中心

        MGRQRYCONNECTOR,//查询通道
        MGRSTARTBROKER,//关闭交易通道
        MGRSTOPBROKER,//停止交易通道
        
        
        MGRQRYEXCHANGE,//查询交易所信息
        MGRUPDATEEXCHANGE,//更新交易所信息
        MGRQRYMARKETTIME,//查询市场时间段
        MGRUPDATEMARKETTIME,//更新市场时间段
        MGRQRYSECURITY,//查询品种
        MGRUPDATESECURITY,//更新品种信息
        MGRQRYSYMBOL,//查询合约信息
        MGRUPDATESYMBOL,//更新合约信息

        

        MGRQRYRULECLASS,//查询风控规则列表
        MGRQRYRULEITEM,//查询某个交易帐号的风控规则
        MGRUPDATERULEITEM,//更新风控规则
        MGRDELRULEITEM,//删除风控规则
        MGRQRYSYSTEMSTATUS,//查询系统状态
        MGRQRYORDER,//查询历史委托
        MGRQRYTRADE,//查询历史成交
        MGRQRYPOSITION,//查询结算持仓
        MGRQRYCASH,//查询出入金
        MGRQRYSETTLEMENT,//查询结算单
        MGRCHANGEACCOUNTPASS,//修改交易密码
        MGRCHANGEINVESTOR,//
        MGRUPDATEPOSLOCK,//修改帐户锁仓权限
        MGRQRYMANAGER,//查询管理员列表
        MGRADDMANAGER,//添加管理员
        MGRUPDATEMANAGER,//更新管理员
        MGRQRYACCTSERVICE,//查询交易帐户服务
        MGRUPDATEPASS,//更改管理员密码



        MGRCONTRIBREQUEST,//管理扩展请求
        MGRINSERTTRADE,//插入成交
        MGRDELACCOUNT,//删除交易帐户





        MGRRESPONSE = 9000,//管理服务端通用回报
        MGRQRYACCOUNTSRESPONSE,
        MGRLOGINRESPONSE,//管理登入回报
        MGRRESUMEACCOUNTRESPONE,//恢复交易帐号数据回报 开始恢复前会给出开始标识,恢复结束后会给出结束标识
        MGRACCOUNTINFOLITENOTIFY,//某个账户简短账户信息,用于反映账户当日的交易状态
        MGRSESSIONSTATUSUPDATE,//客户端回话状态更新,比如登入 退出 以及IP地址 硬件码改变等
        MGRACCOUNTINFORESPONSE,//查询交易帐号信息回报
        MGRACCOUNTCHANGEUPDATE,//交易帐户变动回报
        MGRCONNECTORRESPONSE,//
        MGREXCHANGERESPONSE,//查询交易所回报
        MGRUPDATEEXCHANGERESPONSE,//更新交易所回报
        MGRMARKETTIMERESPONSE,//查询市场时间段回报
        MGRUPDATEMARKETTIMERESPONSE,//更新市场时间段回报
        MGRSECURITYRESPONSE,//查询品种回报
        //MGRADDSECURITYRESPONSE,//添加品种回报
        MGRUPDATESECURITYRESPONSE,//更新品种回报
        MGRSYMBOLRESPONSE,//合约信息回报
        //MGRADDSYMBOLRESPONSE,//添加合约回报
        MGRUPDATESYMBOLRESPONSE,//更新合约回报

        MGRRULECLASSRESPONSE,//风控规则回报
        MGRRULEITEMRESPONSE,//帐户风控规则回报
        MGRUPDATERULEITEMRESPONSE,//更新风控项目回报
        MGRDELRULEITEMRESPONSE,//删除风控规则回报
        MGRSYSTEMSTATUSRESPONSE,//系统状态回报

        MGRORDERRESPONSE,//查询委托回报
        MGRTRADERESPONSE,//查询成交回报
        MGRPOSITIONRESPONSE,//查询结算持仓回报
        MGRCASHRESPONSE,//查询出入金回报
        MGRSETTLEMENTRESPONSE,//查询结算单回报
        MGROPERATIONRESPONSE,//服务端操作回报 修改密码 添加帐户 出入金等 统一使用同一个Operation回报 用于通知管理端是否成功或失败
        MGRCHANGEACCOUNTPASSRESPONSE,//修改密码回报
        MGRCHANGEINVESTORRESPONSE,//修改token回报
        MGRUPDATEPOSLOCKRESPONSE,//修改帐户锁仓权限回报
        MGRMANAGERRESPONSE,//查询管理员列表回报
        MGRQRYACCTSERVICERESPONSE,//查询交易帐户服务回报




        MGRCONTRIBRESPONSE,//管理扩展回报
        MGRCONTRIBRNOTIFY,//管理扩展通知

        //FLATALL,//清仓
        UPDATEPOSOFFSET,
        UPDATEAGENTTOKEN,
        //REQFLATFROZEN,//账户请求全平并锁定账户

    }

}