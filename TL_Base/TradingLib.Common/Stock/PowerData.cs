using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 除权数据
    /// 除权产生以下操作
    /// 1.分配红利到股东账户
    /// 2.配送股份到股东账户
    /// 
    /// </summary>
    public class PowerData
    {
        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 股票合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 每股送
        /// </summary>
        public int BonusShare { get; set; }

        /// <summary>
        /// 每股配
        /// </summary>
        public int RationedShare {get;set;}

        /// <summary>
        /// 每股配价 当配股数不为0时有效
        /// </summary>
        public decimal RationedPrice { get; set; }

        /// <summary>
        /// 每股分配红利
        /// </summary>
        public decimal Dividend { get; set; }

    }
}
