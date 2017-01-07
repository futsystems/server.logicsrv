﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
        /// <summary>
        /// 结算价
        /// 记录了交易所合约的结算价格
        /// </summary>
    public class MarketData
    {

        /// <summary>
        /// 结算日
        /// </summary>
        public int SettleDay { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 卖价
        /// </summary>
        public decimal AskPrice { get; set; }

        /// <summary>
        /// 卖量
        /// </summary>
        public int AskSize { get; set; }

        /// <summary>
        /// 买价
        /// </summary>
        public decimal BidPrice { get; set; }

        /// <summary>
        /// 买量
        /// </summary>
        public int BidSize { get; set; }

        /// <summary>
        /// 涨停价
        /// </summary>
        public decimal UpperLimit { get; set; }

        /// <summary>
        /// 跌停价
        /// </summary>
        public decimal LowerLimit { get; set; }

        /// <summary>
        /// 上个交易日 未平持仓
        /// </summary>
        public int PreOI { get; set; }

        /// <summary>
        /// 当日未平持仓
        /// </summary>
        public int OI { get; set; }

        /// <summary>
        /// 昨日结算价
        /// </summary>
        public decimal PreSettlement { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal Settlement { get; set; }
        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal Open { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        public decimal Low { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        public decimal Close { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public int Vol { get; set; }

        public Tick ToTick()
        {
            TickImpl tick = new TickImpl(this.Symbol);
            tick.Trade = this.Close;
            tick.Size = 1;
            tick.AskSize = this.AskSize;
            tick.AskPrice = this.AskPrice;
            tick.BidSize = this.BidSize;
            tick.BidPrice = this.BidPrice;
            tick.UpperLimit = this.UpperLimit;
            tick.LowerLimit = this.LowerLimit;
            tick.PreOpenInterest = this.PreOI;
            tick.OpenInterest = this.OI;
            tick.PreSettlement = this.PreSettlement;
            this.Settlement = this.Settlement;

            tick.Open = this.Open;
            tick.High = this.High;
            tick.Low = this.Low;
            tick.PreClose = this.PreSettlement;
            tick.Date = this.SettleDay;

            return tick;
        }
    }

}
