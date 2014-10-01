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


        /// <summary>
        /// 通过panel来讲对应的监测窗口显示到对应的dockpanel
        /// </summary>
        /// <param name="panel"></param>
        void Show(object panel);
        //清算中心 用于交易通道查询当前的委托 仓位 以及其他相关数据
        IBrokerClearCentre ClearCentre { get; set; }
        
    }
}
