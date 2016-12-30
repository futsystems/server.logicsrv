using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    internal class ExchangeSettleInfo : IComparable
    {
        /// <summary>
        /// 默认 按时间升序排列，第一个元素是最近的一个时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            ExchangeSettleInfo info = obj as ExchangeSettleInfo;
            if (info.LocalSysSettleTime > this.LocalSysSettleTime)
            {
                return -1;
            }
            else if (info.LocalSysSettleTime == this.LocalSysSettleTime)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        /// <summary>
        /// 交易所
        /// </summary>
        public IExchange Exchange { get; set; }

        /// <summary>
        /// 该交易所下一个结算时间
        /// </summary>
        public DateTime NextExchangeSettleTime { get; set; }

        /// <summary>
        /// 结算时间对应的本地时间
        /// </summary>
        public DateTime LocalSysSettleTime { get; set; }

        /// <summary>
        /// 对应的交易所结算日
        /// </summary>
        public int Settleday { get; set; }


        public override string ToString()
        {
            return string.Format("{0} Now:{1} NextSettleTime:{2} SysTime:{3} Settleday:{4} ", Exchange.EXCode, Exchange.GetExchangeTime().ToString("yyyyMMdd HH:mm:ss"), NextExchangeSettleTime.ToString("yyyyMMdd HH:mm:ss"), LocalSysSettleTime.ToString("yyyyMMdd HH:mm:ss"), Settleday);
        }

    }
}
