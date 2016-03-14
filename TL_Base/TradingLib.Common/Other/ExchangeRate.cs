using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 货币汇率
    /// </summary>
    public class ExchangeRate
    {

        public ExchangeRate()
        {
            this.ID = 0;
            this.Settleday = 0;
            this.Currency = CurrencyType.RMB;
            this.AskRate = 1;
            this.IntermediateRate = 1;
            this.BidRate = 1;
            this.UpdateTime = Util.ToTLDateTime();
        }

        /// <summary>
        /// 数据库全局ID
        /// </summary>
        public int ID { get; set; }


        /// <summary>
        /// 结算日 标注该汇率数据属于哪个结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 货币类别
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// 卖价
        /// </summary>
        public decimal AskRate { get; set; }

        /// <summary>
        /// 中间价
        /// </summary>
        public decimal IntermediateRate { get; set; }

        /// <summary>
        /// 买价
        /// </summary>
        public decimal BidRate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public long UpdateTime { get; set; }
    }
}
