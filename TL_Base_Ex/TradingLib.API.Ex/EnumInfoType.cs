using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TradingLib.API
{
    /// <summary>
    /// 交易平台web管理端部分的消息分类
    /// 交易服务端有一个pub端口一直对外发送实时消息,
    /// 交易服务端管理后台有一个websock进程,用于监听消息并将消息发送到对应的web浏览器
    /// </summary>
    public enum InfoType
    {

        [Description("原生字符串")]
        RawStr=100,
        [Description("实时行情")]
        Tick=101,
        [Description("服务端状态")]
        Health = 102,
        [Description("模拟交易帐户类的统计")]
        StatisticDealer = 103,
        /// <summary>
        /// WebSock端请求数据后,系统将数据准备好后以WebSockTopic的InfoType对外发送,sockjsrouter收到消息解析后按照对应的uuid编号向某个具体的connection进行转发
        /// </summary>
        [Description("WebSock端的数据请求")]
        WebSockTopic = 200,
        [Description("某个交易帐号的盘中动态信息动态权益，浮动盈亏等")]
        AccInfoLite = 201,
        [Description("某个交易帐号设置发生改变")]
        AccSettingUpdate=202,
        [Description("某个交易帐号连接回话状态改变,登入 注销或者ip地址变更")]
        SessionUpdate=203,
    }

}
