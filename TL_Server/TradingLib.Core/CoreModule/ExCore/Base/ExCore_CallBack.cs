using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 路由侧的交易事件回报 经过记录和处理后通过Notify的函数进行通知
    /// 1.接收交易客户端的服务则需要override这些函数以实现对应的消息回报处理
    /// 2.不接收外部客户端登入的服务，则不需要override这些函数
    /// </summary>
    public partial class ExCore
    {

        protected virtual void NotifyTick(Tick k)
        { 
        
        }

        protected virtual void NotifyOrderActionError(OrderAction a, RspInfo e)
        { 
            
        }

        protected virtual void NotifyOrderError(Order o, RspInfo e)
        { 
        
        }


        protected virtual void NotifyOrder(Order o)
        { 
        
        
        }

        protected virtual void NotifyBOOrder(BinaryOptionOrder o)
        { 
        
        }

        protected virtual void NotifyBOOrderError(BinaryOptionOrder o, RspInfo e)
        { 
        
        }



        protected virtual void NotifyFill(Trade f)
        { 
        
        }

        protected virtual void NotifyPositionUpdate(Position pos)
        { 
        
        }
        protected virtual void NotifyCancel(long oid)
        { 
            
        }



    }
}
