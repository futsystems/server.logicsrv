using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    //public class PositionDetailUtils
    //{
    //    /// <summary>
    //    /// 历史持仓明细
    //    /// </summary>
    //    public IEnumerable<PositionDetail> Position_Hist { get; set; }

    //    /// <summary>
    //    /// 今日成交
    //    /// </summary>
    //    public IEnumerable<Trade> Trades_Today { get; set; }

    //    /* 关于平仓明细
    //     * 每一个平仓操作就会产生平仓明细
    //     * 如果平仓的数量在某个持仓明细的职场数量之内，则生成一条平仓明细
    //     * 如果平仓的数量跨越了多个持仓明细的持仓数量，则产生对应的多条平仓明细
    //     * 
    //     * 
    //     * 
    //     * 
    //     * 
    //     * 
    //     * 
    //     * 
    //     * 
    //     * */
    //    /// <summary>
    //    /// 将历史持仓明细 + 当日成交记录 =>当日持仓明细 和当日平仓明细
    //    /// </summary>
    ////    public IEnumerable<PositionDetail> CalPositonDetail(IEnumerable<PositionDetail> pos_hist,IEnumerable<Trade> trade_today,out PositionFlatDetail[] posflat)
    ////    { 
    ////        //对开仓成交进行时间排序，对平仓委托进行按合约分组累加
    ////        TradeInfo[] trade_open = null;//trade_today.Where(f=>f.IsEntryPosition);
    ////        TradeInfo[] trade_close = null;//trade_today.Where(f => !f.IsEntryPosition);

    ////        List<PositionDetail> today_new_open = new List<PositionDetail>();//当日新开持仓
    ////        List<PositionDetail> new_hist_pos = new List<PositionDetail>();//当日持仓明细
            
            
    ////        //如果当日平仓成交分组为空 则trade_open即为新开仓 放入today_new_open
    ////        if (trade_close.Length == 0)
    ////        {

    ////        }
    ////        else
    ////        {
    ////            //如果不为空则便利所有平仓分组进行处理
    ////            foreach (TradeInfo info in trade_close)
    ////            {
    ////                //判断成交信息是平今还是平昨
    ////                if (info.OffsetFlag == QSEnumOffsetFlag.CLOSETODAY)
    ////                {
    ////                    //平今和trade_open进行处理
    ////                    //1.合约月第一条开仓成交合约不一致，则开仓成交=>当日新开持仓

    ////                    //2.如果相同，则有开仓也有平仓，比较数量
    ////                        //如果平仓量大于开仓量，计算剩余平仓量，同时开仓成交下移一条

    ////                        //如果平仓量小于开仓量，计算剩余开仓量，=>当日新开持仓
    ////                }
    ////                else
    ////                {
    ////                    //平昨和pos_hist进行处理
                    
    ////                }

    ////                //将未处理的pos_hist和trade_open 放入new_hist_pos和today_new_open
    ////                //两者合并就是当日综合持仓明细
    ////            }
    ////        }


            
    ////    }
    //}



    


    /// <summary>
    /// 持仓明细
    /// 结算时通过历史持仓,交易帐户的成交数据分组获得开仓记录，平仓记录(汇总)
    /// 
    /// 持仓明细是以成交为单位的交易历史记录，每条开仓成交会形成一条持仓明细
    /// 有平仓成交时按照先开先平或者上期所的 平今 平昨指令，从对应的持仓明细列表中进行平仓
    /// 
    /// 历史持仓明细(隔夜持仓)
    /// 当日持仓明细 由当日开仓成交形成的持仓明细
    /// 
    /// 当有平仓成交时按照规则从对应的持仓明细中获得对应的持仓进行平仓
    /// 
    /// 
    /// 
    /// </summary>
    public class PositionDetailImpl:PositionDetail
    {

        public PositionDetailImpl()
        {
            this.Account = string.Empty;
            this.Symbol = string.Empty;
            this.OpenDate = 0;
            this.OpenTime = 0;
            this.Tradingday = 0;
            this.Side = true;
            this.Volume = 0;
            this.OpenPrice = 0;
            this.TradeID = string.Empty;
            
            this.LastSettlementPrice = 0M;
            this.SettlementPrice = 0M;
            this.CloseVolume = 0;

            this.HedgeFlag = string.Empty;
            this.oSymbol = null;
            this.Exchange = string.Empty;
            this.SecCode = string.Empty;
            this.Margin = 0M;
            this.CloseProfitByDate = 0;
            this.UnRealizedProfitByDate = 0;
        }

        public PositionDetailImpl(PositionDetail p)
        {
            this.Account = p.Account;
            this.Symbol = p.Symbol;
            this.OpenDate = p.OpenDate;
            this.OpenTime = p.OpenTime;
            this.Tradingday = p.Tradingday;
            this.Side = p.Side;
            this.Volume = p.Volume;
            this.OpenPrice = p.OpenPrice;
            this.TradeID = p.TradeID;

            this.LastSettlementPrice = p.LastSettlementPrice;
            this.SettlementPrice = p.SettlementPrice;
            this.CloseVolume = p.CloseVolume;
            this.HedgeFlag = p.HedgeFlag;
            this.oSymbol = p.oSymbol;
            this.Exchange = p.Exchange;
            this.SecCode = p.SecCode;
            this.Margin = p.Margin;
            this.CloseProfitByDate = p.CloseProfitByDate;
            this.UnRealizedProfitByDate = p.UnRealizedProfitByDate;
        }
        
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

         /// <summary>
        /// 开仓日期
        /// </summary>
        public int OpenDate { get; set; }


        /// <summary>
        /// 开仓时间
        /// </summary>
        public int OpenTime { get; set; }

        /// <summary>
        /// 交易日
        /// </summary>
        public int Tradingday { get; set; }

        /// <summary>
        /// 结算日 表明该持仓明细记录属于哪个结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 方向 多或空
        /// </summary>
        public bool Side {get;set;}

        /// <summary>
        /// 数量
        /// </summary>
        public int Volume { get; set; }


        /// <summary>
        /// 开仓价格
        /// </summary>
        public decimal OpenPrice { get; set; }
       

        /// <summary>
        /// 成交编号
        /// </summary>
        public string TradeID { get; set; }

        /// <summary>
        /// 昨结算价
        /// </summary>
        public decimal LastSettlementPrice { get; set; }

        /// <summary>
        /// 今结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }

        /// <summary>
        /// 平仓量
        /// </summary>
        public int CloseVolume { get; set; }

        /// <summary>
        /// 投机套保标识
        /// </summary>
        public string HedgeFlag { get; set; }

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


        /// <summary>
        /// 投资者保证金
        /// </summary>
        public decimal Margin { get; set; }


        /// <summary>
        /// 盯市平仓盈亏
        /// </summary>
        public decimal CloseProfitByDate { get; set; }

        /// <summary>
        /// 盯市浮动盈亏
        /// 今仓 (开仓价格-结算价)*手数*乘数
        /// 
        /// 在结算过程中 盯市浮动盈亏会计入结算单并反映在帐户权益上
        /// </summary>
        public decimal UnRealizedProfitByDate { get; set; }


        public string Serialize(PositionDetail p)
        {
            char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(p.Account);
            sb.Append(d);
            sb.Append(p.CloseProfitByDate);
            sb.Append(d);
            sb.Append(p.CloseVolume);
            sb.Append(d);
            sb.Append(p.Exchange);
            sb.Append(d);
            sb.Append(p.HedgeFlag);
            sb.Append(d);
            sb.Append(p.LastSettlementPrice);
            sb.Append(d);
            sb.Append(p.Margin);
            sb.Append(d);
            sb.Append(p.OpenDate);
            sb.Append(d);
            sb.Append(p.OpenPrice);
            sb.Append(d);
            sb.Append(p.OpenTime);
            sb.Append(d);
            sb.Append(p.SecCode);
            sb.Append(d);
            sb.Append(p.Settleday);
            sb.Append(d);
            sb.Append(p.SettlementPrice);
            sb.Append(d);
            sb.Append(p.Side);
            sb.Append(d);
            sb.Append(p.Symbol);
            sb.Append(d);
            sb.Append(p.TradeID);
            sb.Append(d);
            sb.Append(p.Tradingday);
            sb.Append(d);
            sb.Append(p.UnRealizedProfitByDate);
            sb.Append(d);
            sb.Append(p.Volume);
            return sb.ToString();
        }
    }
}
