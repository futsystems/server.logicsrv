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



    public static class PositionDetailUtil
    {
        /// <summary>
        /// 判断是否是历史持仓
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="currtradingday"></param>
        /// <returns></returns>
        public static bool IsHisPosition(this PositionDetail pos)
        {
            //如果交易日没有标注 表明该持仓明细是当日新开持仓明细 不为历史持仓，如果是历史持仓在写入历史持仓明细表的时候会加入结算日信息
            if (pos.Tradingday == 0)
            {
                return false;
            }

            //如果交易日等于对应的结算日则为当日持仓，否则为历史持仓
            if (pos.Tradingday == pos.Settleday)
            {
                //如果持仓明细的交易日与当前交易日不同，则为历史持仓，否则为当日持仓
                return true;
            }
            return false;
        }


        public static long GetDateTime(this PositionDetail pos)
        {
            return Util.ToTLDateTime(pos.OpenDate,pos.OpenTime);
        }
        /// <summary>
        /// 该持仓是否已经被关闭
        /// 如果开仓量等于平仓量则该持仓关闭
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool IsClosed(this PositionDetail pos)
        {
            if (pos.Volume == pos.CloseVolume)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 当前剩余持仓数量 开仓数量-平仓数量 即为该持仓当前持有的数量
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int HoldSize(this PositionDetail pos)
        {
            return pos.Volume - pos.CloseVolume;
        }

        public static string GetPositionDetailStr(this PositionDetail pos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(pos.Account);
            sb.Append(" ");
            sb.Append(pos.Symbol);
            sb.Append(" ");
            sb.Append(" T:" + pos.GetDateTime().ToString());
            sb.Append(" S:" + (pos.Side ? "Long" : "Short"));
            sb.Append(string.Format(" {0}@{1}", pos.Volume, pos.OpenPrice));
            sb.Append(" HoldSize:" + pos.HoldSize());
            sb.Append(" TradeID:" + pos.TradeID);
            return sb.ToString();
        }


        public static string GetPositionCloseStr(this PositionCloseDetail d)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(d.Account+" ");
            sb.Append(d.Symbol + " ");
            sb.Append("Open:" + d.OpenDate.ToString() + " " + d.OpenPrice.ToString() +" ID:"+d.OpenTradeID +" ");
            sb.Append("Close:" + d.CloseDate.ToString() + " " + d.ClosePrice.ToString() +" ID:"+d.CloseTradeID+" ");
            sb.Append(string.Format("{0} {1}手@{2} PreS:{3}",d.Side?"买入":"卖出",d.CloseVolume,d.ClosePrice,d.LastSettlementPrice));
            return sb.ToString();
        }


        
    }



    /// <summary>
    /// 平仓明细
    /// </summary>
    public class PositionCloseDetail
    {


        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 交易日
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
        /// <summary>
        /// 盯市平仓盈亏
        /// 平当日仓 (开仓-平仓)*手数*乘数
        /// </summary>
        public decimal CloseProfitByDate{ get; set; }


        /// <summary>
        /// 合约信息
        /// </summary>
        public Symbol oSymbol { get; set; }

        string _exchange = string.Empty;
        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange {
            get
            {
                
                return  oSymbol!=null?oSymbol.SecurityFamily.Exchange.EXCode:_exchange;
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


    }
}
