using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.XLProtocol
{
    /// <summary>
    /// 协议 消息类别
    /// </summary>
    public enum XLMessageType : short
    {
        /// <summary>
        /// 心跳
        /// </summary>
        T_HEARTBEEAT = 0,
        
        /// <summary>
        /// 错误回报
        /// </summary>
        T_RSP_ERROR = 1,

        /// <summary>
        /// 请求登入
        /// </summary>
        T_REQ_LOGIN = 1000,

        /// <summary>
        /// 登入响应
        /// </summary>
        T_RSP_LOGIN = 1001,

        /// <summary>
        /// 请求更新密码
        /// </summary>
        T_REQ_UPDATEPASS = 1002,

        /// <summary>
        /// 更新密码回报
        /// </summary>
        T_RSP_UPDATEPASS = 1003,

        /// <summary>
        /// 请求查询合约
        /// </summary>
        T_QRY_SYMBOL = 2000,

        /// <summary>
        /// 查询合约响应
        /// </summary>
        T_RSP_SYMBOL = 2001,

        /// <summary>
        /// 请求查询委托
        /// </summary>
        T_QRY_ORDER = 3000,

        /// <summary>
        /// 查询委托响应
        /// </summary>
        T_RSP_ORDER = 30001,

        /// <summary>
        /// 请求查询成交
        /// </summary>
        T_QRY_TRADE = 3002,

        /// <summary>
        /// 查询成交响应
        /// </summary>
        T_RSP_TRADE = 3003,

        /// <summary>
        /// 请求查询持仓
        /// </summary>
        T_QRY_POSITION = 3004,

        /// <summary>
        /// 查询持仓响应
        /// </summary>
        T_RSP_POSITION = 3005,

        /// <summary>
        /// 请求查询交易账户
        /// </summary>
        T_QRY_ACCOUNT = 3006,

        /// <summary>
        /// 查询交易账户响应
        /// </summary>
        T_RSP_ACCOUNT = 3007,

        /// <summary>
        /// 请求查询最大报单数量
        /// </summary>
        T_QRY_MAXORDVOL = 3008,

        /// <summary>
        /// 查询最大报单数量响应
        /// </summary>
        T_RSP_MAXORDVOL = 3009,


        /// <summary>
        /// 请求提交委托
        /// </summary>
        T_REQ_INSERTORDER = 3010,

        /// <summary>
        /// 提交委托响应
        /// 1.委托参数异常服务端会给出该响应
        /// 2.委托正常 则直接返回 OrderNotify
        /// </summary>
        T_RSP_INSERTORDER = 3011,

        /// <summary>
        /// 请求提交委托操作
        /// </summary>
        T_REQ_ORDERACTION = 3012,

        /// <summary>
        /// 提交委托操作响应
        /// </summary>
        T_RSP_ORDERACTION = 3013,

        /// <summary>
        /// 委托回报
        /// </summary>
        T_RTN_ORDER = 3020,

        /// <summary>
        /// 成交回报
        /// </summary>
        T_RTN_TRADE = 3021,

        /// <summary>
        /// 持仓更新回报
        /// </summary>
        T_RTN_POSITIONUPDATE = 3022,


        /// <summary>
        /// 请求注册市场行情
        /// </summary>
        T_REQ_MARJETDATA = 4000,

        /// <summary>
        /// 请求注册市场行情回报
        /// </summary>
        T_RSP_MARJETDATA = 4001,

        /// <summary>
        /// 市场行情数据
        /// </summary>
        T_RTN_MARKETDATA = 4002
    }
}
