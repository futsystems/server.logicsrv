using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 委托编号
    /// 从委托进入系统的第一个入口 
    /// 设定委托唯一编号id和日内流水OrderSeq
    /// 
    /// </summary>
    public partial class MsgExchServer
    {

        /// <summary>
        /// 客户端提交委托
        /// </summary>
        /// <param name="o"></param>
        void tl_newSendOrderRequest(Order o)
        {
            o.id = 0;//从外部传入的委托 统一置委托编号为0,由内部统一进行委托编号
            OrderRequestHandler(o, false, true);//外部委托
        }



    }
}
