using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Common;

namespace TradingLib.API
{
    public interface IDataClient
    {
        #region 实时行情回报 Bar查询回报
        /// <summary>
        /// 行情服务器历史数据返回回调
        /// </summary>
        event Action<RspQryBarResponseBin> OnRspBarEvent;

        /// <summary>
        /// 行情回报
        /// </summary>
        event Action<Tick> OnRtnTickEvent;

        #endregion

        /// <summary>
        /// 注册合约行情
        /// </summary>
        /// <param name="symbols"></param>
        void RegisterSymbol(string[] symbols);


        /// <summary>
        /// 查询历史数据
        /// </summary>
        /// <param name="symbol">合约</param>
        /// <param name="interval">时间间隔 单位秒</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="maxcount">最大返回数量</param>
        /// <returns></returns>
        int QryBar(string symbol, int interval, DateTime start, DateTime end, int maxcount);


        /// <summary>
        /// 查询某个合约
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Symbol GetSymbol(string symbol);

    }
}
