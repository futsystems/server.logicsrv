using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.LitJson;


namespace TradingLib.Common
{
    /// <summary>
    /// 平仓明细
    /// </summary>
    public class PositionCloseDetailImpl: PositionCloseDetail
    {

        public PositionCloseDetailImpl(PositionDetail pd,Trade f,int closesize)
        {
            //生成平仓明细数据
            //设定持仓主体信息
            this.Account = pd.Account;
            this.Symbol = pd.Symbol;
            this.oSymbol = pd.oSymbol;
            this.Side = pd.Side;
            this.IsCloseYdPosition = pd.IsHisPosition;//是否是平昨仓
            this.LastSettlementPrice = pd.LastSettlementPrice;//
            //?tradingday settleday

            //设定开仓时间
            this.OpenDate = pd.OpenDate;
            this.OpenTime = pd.OpenTime;
            this.OpenTradeID = pd.TradeID;
            this.OpenPrice = pd.OpenPrice;

            //设定平仓时间
            this.CloseDate = f.xdate;
            this.CloseTime = f.xtime;
            this.CloseTradeID = f.BrokerKey;
            this.ClosePrice = f.xprice;
            this.CloseVolume = closesize; 
        }
        public PositionCloseDetailImpl()
        {
            this.Account = string.Empty;
            this.Settleday = 0;
            this.Side = true;

            this.OpenDate = 0;
            this.OpenTime = 0;
            this.OpenTradeID = string.Empty;
            this.OpenPrice = 0;

            this.CloseDate = 0;
            this.CloseTime = 0;
            this.CloseTradeID = string.Empty;
            this.ClosePrice = 0;
            this.CloseVolume = 0;

            this.IsCloseYdPosition = false;
            this.LastSettlementPrice = 0;
            this.CloseAmount = 0;
            this.CloseProfitByDate = 0;
            this.CloseProfitByTrade = 0;
            this.ClosePointByDate = 0;

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
        /// 平仓所在结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public bool Side { get; set; }

        /// <summary>
        /// 平昨仓还是平今仓
        /// </summary>
        public bool IsCloseYdPosition { get; set; }

        /// <summary>
        /// 昨结算价
        /// </summary>
        public decimal LastSettlementPrice { get; set; }



        #region 开平属性
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
        /// 开仓价格
        /// </summary>
        public decimal OpenPrice { get; set; }

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
        /// 平仓价格
        /// </summary>
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// 平仓量
        /// </summary>
        public int CloseVolume { get; set; }

        #endregion

        

        decimal? _closeProfitByDate;
        /// <summary>
        /// 盯市平仓盈亏金额
        /// 平当日仓 (开仓-平仓)*手数*乘数
        /// </summary>
        public decimal CloseProfitByDate 
        {
            get
            {
                if (_closeProfitByDate != null)
                    return (decimal)_closeProfitByDate;
                else
                {
                    //平今仓
                    decimal profit = 0;
                    if (!IsCloseYdPosition)
                    {
                        //今仓 平仓盈亏为平仓价-平仓价
                        profit = (ClosePrice - OpenPrice) * CloseVolume * oSymbol.Multiple * (Side ? 1 : -1);
                    }
                    else
                    {
                        //昨仓 平仓盈亏为昨结算-平仓价
                        profit = (ClosePrice - LastSettlementPrice) * CloseVolume * oSymbol.Multiple * (Side ? 1 : -1);
                    }
                    return profit;
                }
            }

            set { _closeProfitByDate = value; }
        }

        decimal? _closeProfitByTrade;
        /// <summary>
        /// 逐笔平仓盈亏金额
        /// </summary>
        public decimal CloseProfitByTrade 
        {
            get
            {
                if (_closeProfitByTrade != null)
                    return (decimal)_closeProfitByTrade;
                else
                {
                    //逐笔平仓盈亏 平仓价格-开仓价格
                    return (ClosePrice - OpenPrice) * CloseVolume * oSymbol.Multiple * (Side ? 1 : -1);
                }
            }
            set { _closeProfitByTrade = value; }
        }

        decimal? _closePointByDate;
        /// <summary>
        /// 盯市平仓盈亏点数
        /// </summary>
        public decimal ClosePointByDate 
        {
            get
            {
                //平今仓
                decimal profit = 0;
                if (!IsCloseYdPosition)
                {
                    //今仓 平仓盈亏为平仓价-平仓价
                    profit = (ClosePrice - OpenPrice) * CloseVolume * (Side ? 1 : -1);
                }
                else
                {
                    //昨仓 平仓盈亏为昨结算-平仓价
                    profit = (ClosePrice - LastSettlementPrice) * CloseVolume * (Side ? 1 : -1);
                }
                return profit;
            }
            set { _closePointByDate = value; }
        }

        decimal? _closeamount;
        /// <summary>
        /// 平仓金额
        /// </summary>
        public decimal CloseAmount
        {
            get
            {
                if (_closeamount != null)
                    return (decimal)_closeamount;
                else
                { 
                    return this.CloseVolume * this.ClosePrice * oSymbol.Multiple;
                }
            }

            set
            {
                _closeamount = value;
            }
        }

        /// <summary>
        /// 合约信息
        /// </summary>
        [NoJsonExportAttr()]
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
