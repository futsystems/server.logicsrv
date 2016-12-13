using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.XLProtocol
{
    /// <summary>
    ///操作标志类型
    /// </summary>
    public enum XLActionFlagType : byte
    {
        /// <summary>
        /// 删除
        /// </summary>
        Delete = (byte)'0',

        /// <summary>
        /// 修改
        /// </summary>
        Modify = (byte)'3'
    }


    /// <summary>
    ///持仓多空方向类型
    /// </summary>
    public enum XLPosiDirectionType : byte
    {
        /// <summary>
        /// 多头
        /// </summary>
        Long = (byte)'0',

        /// <summary>
        /// 空头
        /// </summary>
        Short = (byte)'1'
    }

    /// <summary>
    /// 买卖方向类型
    /// </summary>
    public enum XLDirectionType : byte
    {
        /// <summary>
        /// 买
        /// </summary>
        Buy = (byte)'0',

        /// <summary>
        /// 卖
        /// </summary>
        Sell = (byte)'1'
    }

    /// <summary>
    /// 委托类别
    /// </summary>
    public enum XLOrderType : byte
    { 
        /// <summary>
        /// 市价单
        /// </summary>
        Market = (byte)'0',

        /// <summary>
        /// 限价单
        /// </summary>
        Limit = (byte)'1',
    }

    /// <summary>
    /// 委托状态
    /// </summary>
    public enum XLOrderStatus : byte
    {
        /// <summary>
        /// 委托提交到柜台
        /// </summary>
        Placed = (byte)0,

        /// <summary>
        /// 委托已经通过接口正常提交,但是没有获得成交端的任何返回,成交侧可能拒绝也可能接受
        /// 在其他系统中，如果Submited是表明该委托已经处于待成交状态,那么该状态对应于PendingSubmited
        /// </summary>
        Submited = (byte)1,

        /// <summary>
        /// 表明委托已经被成交侧接受,处于待成交状态
        /// </summary>
        Opened = (byte)2,

        /// <summary>
        /// 委托被全部成交
        /// </summary>
        Filled = (byte)3,

        /// <summary>
        /// 委托被部分成交
        /// </summary>
        PartFilled = (byte)4,

        /// <summary>
        /// 委托被取消
        /// </summary>
        Canceled = (byte)5,

        /// <summary>
        /// 委托被拒绝
        /// </summary>
        Reject = (byte)6,

        /// <summary>
        /// 如果是系统模拟的委托,则返回预提交状态，当模拟的委托条件触发,提交到成交侧时,状态更新为Submited
        /// </summary>
        PreSubmited = (byte)7,

        /// <summary>
        /// 状态未知
        /// </summary>
        Unknown = (byte)8,
    }
    /// <summary>
    /// 投机套保标志类型
    /// </summary>
    public enum XLHedgeFlagType : byte
    {
        /// <summary>
        /// 投机
        /// </summary>
        Speculation = (byte)'1',

        /// <summary>
        /// 套利
        /// </summary>
        Arbitrage = (byte)'2',

        /// <summary>
        /// 套保
        /// </summary>
        Hedge = (byte)'3'
    }

    /// <summary>
    /// 开平标志类型
    /// </summary>
    public enum XLOffsetFlagType : byte
    {
        /// <summary>
        /// 开仓
        /// </summary>
        Open = (byte)'0',

        /// <summary>
        /// 平仓
        /// </summary>
        Close = (byte)'1',

        /// <summary>
        /// 强平
        /// </summary>
        ForceClose = (byte)'2',

        /// <summary>
        /// 平今
        /// </summary>
        CloseToday = (byte)'3',

        /// <summary>
        /// 平昨
        /// </summary>
        CloseYesterday = (byte)'4',

        /// <summary>
        /// 强减
        /// </summary>
        ForceOff = (byte)'5',

        /// <summary>
        /// 本地强平
        /// </summary>
        LocalForceClose = (byte)'6',

        /// <summary>
        /// 未知
        /// </summary>
        Unknown = (byte)'7',
    }

    public enum XLCurrencyType : byte
    {
        /// <summary>
        /// 人民币
        /// </summary>
        RMB = (byte)'1',
        /// <summary>
        /// 美元
        /// </summary>
        USD = (byte)'2',
        /// <summary>
        /// 港币
        /// </summary>
        HKD = (byte)'3',
        /// <summary>
        /// 欧元
        /// </summary>
        EUR = (byte)'4',
    }

    public enum XLSecurityType : byte
    {
        /// <summary>
        /// 期货
        /// </summary>
        Future = (byte)'1',

        /// <summary>
        /// 股票
        /// </summary>
        STK = (byte)'2',
    }
}
