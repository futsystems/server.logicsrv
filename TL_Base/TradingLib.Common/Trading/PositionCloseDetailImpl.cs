using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 平仓明细
    /// </summary>
    public class PositionCloseDetailImpl: PositionCloseDetail
    {

        public PositionCloseDetailImpl()
        {
            this.Account = string.Empty;
            this.Settleday = 0;
            this.Tradingday = 0;
            this.Side = true;
            this.OpenDate = 0;
            this.OpenTime = 0;
            this.OpenTradeID = string.Empty;
            this.CloseDate = 0;
            this.CloseTime = 0;
            this.CloseTradeID = string.Empty;
            this.OpenPrice = 0;
            this.LastSettlementPrice = 0;
            this.ClosePrice = 0;
            this.CloseVolume = 0;
            this.CloseProfitByDate = 0;
            this.oSymbol = null;
            this.Exchange = string.Empty;
            this.Symbol = string.Empty;
            this.SecCode = string.Empty;
        }
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 开仓所在交易日
        /// </summary>
        public int Tradingday { get; set; }


        /// <summary>
        /// 平仓所在结算日
        /// </summary>
        public int Settleday { get; set; }


        /// <summary>
        /// 方向
        /// </summary>
        public bool Side { get; set; }
        /// <summary>
        /// 开仓日期
        /// </summary>
        public int OpenDate { get; set; }

        /// <summary>
        /// 开仓时间
        /// </summary>
        public int OpenTime { get; set; }

        /// <summary>
        /// 开仓成交编号
        /// </summary>
        public string OpenTradeID { get; set; }

        /// <summary>
        /// 平仓日期
        /// </summary>
        public int CloseDate { get; set; }

        /// <summary>
        /// 平仓时间
        /// </summary>
        public int CloseTime { get; set; }

        /// <summary>
        /// 平仓成交编号
        /// </summary>
        public string CloseTradeID { get; set; }
        /// <summary>
        /// 开仓价格
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// 昨结算价
        /// </summary>
        public decimal LastSettlementPrice { get; set; }

        /// <summary>
        /// 平仓价格
        /// </summary>
        public decimal ClosePrice { get; set; }


        /// <summary>
        /// 平仓量
        /// </summary>
        public int CloseVolume { get; set; }

        //decimal _closeprofitbydate = 0;
        /// <summary>
        /// 盯市平仓盈亏金额
        /// 平当日仓 (开仓-平仓)*手数*乘数
        /// </summary>
        public decimal CloseProfitByDate {get;set;}


        /// <summary>
        /// 盯市平仓盈亏点数
        /// </summary>
        public decimal ClosePointByDate { get; set; }

        /// <summary>
        /// 合约信息
        /// </summary>
        public Symbol oSymbol { get; set; }

        string _exchange = string.Empty;
        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange
        {
            get
            {

                return oSymbol != null ? oSymbol.SecurityFamily.Exchange.EXCode : _exchange;
            }
            set
            {
                _exchange = value;
            }
        }

        string _symbol = string.Empty;
        public string Symbol
        {
            get
            {
                return oSymbol != null ? oSymbol.Symbol : _symbol;
            }
            set
            {
                _symbol = value;
            }
        }

        string _seccode = string.Empty;
        public string SecCode
        {
            get
            {
                return oSymbol != null ? oSymbol.SecurityFamily.Code : _seccode;
            }
            set
            {
                _seccode = value;
            }
        }
    }
}
