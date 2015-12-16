using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 信号源接口
    /// 用于获得信号源的交易记录和状态以及实时交易事件
    /// 这里考虑是否将所有的信号都整理成IAccount
    /// 通过创建不同类别的IAccount由IAccount去适配所有信号类别
    /// 1.系统内子账户 该账户可以进行交易，同时生成对应的交易记录
    /// 2.子账户 绑定Connector 通过Account来采集通道的交易记录并形成对应的数据集
    /// 
    /// 
    /// 在这2种情况的基础上 再次封装ISignal用于跟单策略。这样可以避免ISignal直接去适配通道 造成交易数据处理的麻烦。
    /// 原来IAccount已经完成了对所通道数据的适配
    /// 帐户类别
    /// 1.SubAccount 子账户 资管子账户 用于客户端登入 交易
    /// 2.SigAccount 信号账户 该帐户用于绑定对应的通道 并获取通道历史交易数据和实时交易数据 这样可以适配所有CTP类型的帐户
    /// 3.           它付资管账户 该账户用于映射到资管其他部署系统中的帐户
    /// 
    /// 关于帐户类 这里需要进行更加深度的抽象，IAccount需要同时具备交易与信号采集的功能
    /// 这样可以简化ISignal的封装 直接暴露接口然后投入使用
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public interface ISignal
    {
        /// <summary>
        /// 信号委托事件
        /// </summary>
        event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 信号成交事件
        /// </summary>
        event FillDelegate GotFillEvent;

        /// <summary>
        /// 持仓变动事件
        /// </summary>
        event Action<ISignal, Trade, IPositionEvent> GotPositionEvent;

        /// <summary>
        /// 数据库统一编号
        /// </summary>
        int ID { get; }

        /// <summary>
        /// 信号源标识 该标识与底层通道标识相同
        /// 信号源必须绑定一个底层TLBroker跟单通道接口
        /// </summary>
        string Token { get; }

        /// <summary>
        /// 持仓数据
        /// </summary>
        IEnumerable<Position> Positions { get;}

        /// <summary>
        /// 多头持仓
        /// </summary>
        IEnumerable<Position> PositionsLong { get; }

        /// <summary>
        /// 空头持仓
        /// </summary>
        IEnumerable<Position> PositionsShort { get; }

        /// <summary>
        /// 委托数据
        /// </summary>
        IEnumerable<Order> Orders { get; }

        /// <summary>
        /// 成交数据
        /// </summary>
        IEnumerable<Trade> Trades { get; }


        /// <summary>
        /// 信号对应的交易帐户
        /// </summary>
        IAccount Account { get; }
        
    }
}
