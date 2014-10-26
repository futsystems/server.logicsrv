using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    public class PositionDetailUtils
    {
        /// <summary>
        /// 历史持仓明细
        /// </summary>
        public IEnumerable<PositionDetail> Position_Hist { get; set; }

        /// <summary>
        /// 今日成交
        /// </summary>
        public IEnumerable<Trade> Trades_Today { get; set; }

        /* 关于平仓明细
         * 每一个平仓操作就会产生平仓明细
         * 如果平仓的数量在某个持仓明细的职场数量之内，则生成一条平仓明细
         * 如果平仓的数量跨越了多个持仓明细的持仓数量，则产生对应的多条平仓明细
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * */
        /// <summary>
        /// 将历史持仓明细 + 当日成交记录 =>当日持仓明细 和当日平仓明细
        /// </summary>
    //    public IEnumerable<PositionDetail> CalPositonDetail(IEnumerable<PositionDetail> pos_hist,IEnumerable<Trade> trade_today,out PositionFlatDetail[] posflat)
    //    { 
    //        //对开仓成交进行时间排序，对平仓委托进行按合约分组累加
    //        TradeInfo[] trade_open = null;//trade_today.Where(f=>f.IsEntryPosition);
    //        TradeInfo[] trade_close = null;//trade_today.Where(f => !f.IsEntryPosition);

    //        List<PositionDetail> today_new_open = new List<PositionDetail>();//当日新开持仓
    //        List<PositionDetail> new_hist_pos = new List<PositionDetail>();//当日持仓明细
            
            
    //        //如果当日平仓成交分组为空 则trade_open即为新开仓 放入today_new_open
    //        if (trade_close.Length == 0)
    //        {

    //        }
    //        else
    //        {
    //            //如果不为空则便利所有平仓分组进行处理
    //            foreach (TradeInfo info in trade_close)
    //            {
    //                //判断成交信息是平今还是平昨
    //                if (info.OffsetFlag == QSEnumOffsetFlag.CLOSETODAY)
    //                {
    //                    //平今和trade_open进行处理
    //                    //1.合约月第一条开仓成交合约不一致，则开仓成交=>当日新开持仓

    //                    //2.如果相同，则有开仓也有平仓，比较数量
    //                        //如果平仓量大于开仓量，计算剩余平仓量，同时开仓成交下移一条

    //                        //如果平仓量小于开仓量，计算剩余开仓量，=>当日新开持仓
    //                }
    //                else
    //                {
    //                    //平昨和pos_hist进行处理
                    
    //                }

    //                //将未处理的pos_hist和trade_open 放入new_hist_pos和today_new_open
    //                //两者合并就是当日综合持仓明细
    //            }
    //        }


            
    //    }
    }

    /// <summary>
    /// 平仓明细
    /// </summary>
    public class PositionFlatDetail
    {
        
    }

    internal class TradeInfo
    {
        public string Account { get; set; }

        public string Symbol { get; set; }

        public decimal Price { get; set; }

        public int Size { get; set; }

        public int Settleday { get; set; }

        public QSEnumOffsetFlag OffsetFlag { get; set; }
    }

    public static class PositionDetailUtil
    { 
        public static long GetDateTime(this PositionDetail pos)
        {
            return Util.ToTLDateTime(pos.OpenDate,pos.OpenTime);
        }
        public static string GetPosDetailStr(this PositionDetail pos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(pos.Account);
            sb.Append(" ");
            sb.Append(pos.Symbol);
            sb.Append(" ");
            sb.Append(" T:"+pos.GetDateTime().ToString());
            sb.Append(" S:"+(pos.Side?"Long":"Short"));
            sb.Append(string.Format(" {0}@{1}",pos.Volume,pos.OpenPrice));
            sb.Append(" HoldSize:" + pos.HoldSize());
            return sb.ToString();
        }

        /// <summary>
        /// 该持仓是否已经被关闭
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
        /// 当前剩余持仓数量
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int HoldSize(this PositionDetail pos)
        {
            return pos.Volume - pos.CloseVolume;
        }
    }
    /// <summary>
    /// 持仓明细
    /// 结算时通过历史持仓,交易帐户的成交数据分组获得开仓记录，平仓记录(汇总),
    /// </summary>
    public class PositionDetail
    {

        public PositionDetail()
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
            this.HedgeFlag = string.Empty;
            this.Exchange = string.Empty;
            this.Margin = 0M;
            this.LastSettlementPrice = 0M;
            this.SettlementPrice = 0M;
            this.CloseVolume = 0;

        }
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol{get;set;}

         /// <summary>
        /// 开仓日期
        /// </summary>
        public int OpenDate { get; set; }


        /// <summary>
        /// 开仓时间
        /// </summary>
        public int OpenTime { get; set; }

        /// <summary>
        /// 交易日 持仓日期
        /// </summary>
        public int Tradingday { get; set; }


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
        /// 投机套保标识
        /// </summary>
        public string HedgeFlag {get;set;}



        /// <summary>
        /// 交易所编号
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 投资者保证金
        /// </summary>
        public decimal Margin { get; set; }


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
    }
}
