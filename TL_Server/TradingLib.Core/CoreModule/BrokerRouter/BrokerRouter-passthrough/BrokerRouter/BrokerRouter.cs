using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 简单路由
    /// 分帐户侧提交委托，通过绑定的通道进行下单
    /// 通道侧返回的委托，统一以对应的分帐户对系统内进行回报
    /// 
    /// </summary>
    public partial class BrokerRouterPassThrough:BaseSrvObject,IModuleBrokerRouter
    {

        const string CoreName = "BrokerRouterPassThrough";
        public string CoreId { get { return this.PROGRAME; } }

        /// <summary>
        /// 交易帐户与通道的映射关系维护器
        /// </summary>
        //AccountConnectorTracker _acctokentracker = null;
        public BrokerRouterPassThrough()
            : base(BrokerRouterPassThrough.CoreName)
        {

            //_acctokentracker = new AccountConnectorTracker();

            StartProcessMsgOut();
        }


        event TickDelegate GotTickEvent;
        public void GotTick(Tick k)
        {
            if (GotTickEvent != null)
                GotTickEvent(k);

        }


        public void Stop()
        { 
        
        }

        public void Start()
        { 
        
        }
        
    }
}
