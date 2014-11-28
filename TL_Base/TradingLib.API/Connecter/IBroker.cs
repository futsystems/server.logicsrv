using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.API
{
    public interface IBroker :IConnecter,ICTrdReq,ICTrdRep
    {

        /// <summary>
		/// 请求获得某个symbol的Tick快照数据
        /// </summary>
        event GetSymbolTickDel GetSymbolTickEvent;

        /// <summary>
        /// 用于交易通道中需要有Tick进行驱动的逻辑,比如委托触发等
        /// </summary>
        /// <param name="k"></param>
        void GotTick(Tick k);

        //清算中心 用于交易通道查询当前的委托 仓位 以及其他相关数据
        IBrokerClearCentre ClearCentre { get; set; }


        /// <summary>
        /// 获得成交接口所有委托
        /// </summary>
        IEnumerable<Order> Orders { get;}

        /// <summary>
        /// 获得成交接口所有成交
        /// </summary>
        IEnumerable<Trade> Trades { get; }

        /// <summary>
        /// 获得成交接口所有持仓
        /// </summary>
        IEnumerable<Position> Positions { get; }
    }
}
