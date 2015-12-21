using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.MDClient
{
    /// <summary>
    /// 行情接口 回调接口
    /// 用于注入回调操作 响应行情数据
    /// </summary>
    public interface IMDNotify
    {
        /// <summary>
        /// 响应实时行情数据
        /// </summary>
        /// <param name="k"></param>
        void OnTickNotify(Tick k);

        /// <summary>
        /// 响应Bar数据查询
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="rsp"></param>
        /// <param name="islast"></param>
        void OnRspQryBar(Bar bar, RspInfo rsp,int nRequestID, bool isLast);
    }
}
