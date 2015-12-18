using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.MDClient
{

    public partial class MDClient
    {

        /// <summary>
        /// Bar数据查询回调
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="rsp"></param>
        /// <param name="islast"></param>
        public virtual void OnQryBar(Bar bar,RspInfo rsp, bool islast)
        { 
            
        }

        /// <summary>
        /// 实时行情回调
        /// </summary>
        /// <param name="k"></param>
        public virtual void OnTick(Tick k)
        {
            logger.Info("got tick:" + k.Symbol);
        
        }
    }
}
