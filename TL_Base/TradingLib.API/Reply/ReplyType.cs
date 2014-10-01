using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    /// <summary>
    /// 返回类别,保留 0 , 1
    /// 0代表正确
    /// 1代表错误
    /// 其余类型标记了具体的返回类型
    /// </summary>
    public enum ReplyType
    {

        [Description("成功")]
        Success = 100,


        [Description("错误")]
        Error = 200,

        [Description("服务端执行操作异常")]
        ServerSideError = 201,
        [Description("Web消息处理器未绑定")]
        WebMsgHandlerNotBind = 201,
        [Description("请求模块不存在")]
        ModuleNotFund = 205,
        [Description("请求函数不存在")]
        MethodNotFund = 201,
        [Description("函数调用异常")]
        CommandExecuteError = 202,
        [Description("交易帐号不存在")]
        AccountNotFound = 203,
        [Description("交易帐号创建失败")]
        AccountCreatedError = 204,
        /// <summary>
        /// web端进行connection相关的操作时 需要指定uuid,用于向特定的连接推送对应的信息
        /// </summary>
        [Description("WebSock UUID不存在")]
        WebSockUUIDNotFound = 204,


        //------------用户登入类 错误信息------------------------//
        [Description("认证服务不可用")]
        UCenterNotAvabile = 220,
        [Description("登入用户不存在")]
        UserDoNotExist=225,
        [Description("认证失败")]
        AuthError=226,
        [Description("用户没有比赛帐号")]
        NoRaceService=230,
        [Description("用户没有配资帐号")]
        NoLoaneeService=231,
        [Description("用户乐透服务")]
        NoLottoService = 232,


        [Description("Broker通道不存在")]
        BrokerNotFound = 250,
        [Description("DataFeed通道不存在")]
        DataFeedNotFound = 251,


        [Description("风控规则类型不存在")]
        RuleTypeNotFound = 300,
        [Description("风控规则参数错误")]
        RuleConfigError = 301,



    }
}
