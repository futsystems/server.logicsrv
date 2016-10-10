using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// EodBar数据Partial更新
    /// 用于更新到BarList 这样访问日线数据时即可获取
    /// </summary>
    public class EodBarEventArgs:EventArgs
    {
        public EodBarEventArgs(Symbol symbol, BarImpl partial)
        {
            this.Symbol = symbol;
            this.EodPartialBar = partial;
        }
        /// <summary>
        /// 合约
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <summary>
        /// EodPartialBar
        /// </summary>
        public BarImpl EodPartialBar { get; set; }
    }


}
