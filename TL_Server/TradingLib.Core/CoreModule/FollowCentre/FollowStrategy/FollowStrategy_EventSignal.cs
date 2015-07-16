using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Core
{
    
    public partial class FollowStrategy
    {
        /// <summary>
        /// 响应信号账户的持仓事件
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        void OnSignalPositionEvent(ISignal arg1, Trade arg2, IPositionEvent arg3)
        {
            //1.过滤器过滤


            //2.生成跟单项目
            TradeFollowItem followitem = new TradeFollowItem(this,arg1, arg2, arg3);

            //signalTracker.GetFollowItemTracker(arg1.Token).GotTradeFollowItem(followitem);

            //3.跟单项目+配置文件触发委托



        }


        void OnSignalOrderEvent(Order order)
        {
            //throw new NotImplementedException();
        }

        void OnSignalFillEvent(Trade t)
        {
            //throw new NotImplementedException();
        }
       
    }
}
