using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase 
    {
        /// <summary>
        /// 是否有持仓
        /// </summary>
        public bool AnyPosition { get { return this.GetAnyPosition(); } }




        /// <summary>
        /// 持仓维护器
        /// </summary>
        public LSPositionTracker TKPosition { get; set; }

        /// <summary>
        /// 当日所有持仓数据 包含已经平仓的持仓对象和有持仓的持仓对象
        /// </summary>
        public IEnumerable<Position> Positions { get { return this.TKPosition; } }

        /// <summary>
        /// 多头持仓维护器
        /// </summary>
        public IEnumerable<Position> PositionsLong { get { return this.TKPosition.LongPositionTracker; } }

        /// <summary>
        /// 空头持仓维护器
        /// </summary>
        public IEnumerable<Position> PositionsShort { get { return this.TKPosition.ShortPositionTracker; } }


        /// <summary>
        /// 委托维护器
        /// </summary>
        public OrderTracker TKOrder { get; set; }


        /// <summary>
        /// BO委托维护器
        /// </summary>
        public BOOrderTracker TKBOOrder { get; set; }
        /// <summary>
        /// 返回维护器所发送的委托数量
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public int SentSize(long oid)
        {
            return TKOrder.Sent(oid);
        }

        /// <summary>
        /// 返回维护其所维护的成交数量
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public int FilledSize(long oid)
        {
            return TKOrder.Filled(oid);
        }

        /// <summary>
        /// 是否已经取消
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool IsCanceled(long oid)
        {
            return TKOrder.isCanceled(oid);
        }

        /// <summary>
        /// 是否已经完成
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public bool IsComplate(long oid)
        {
            return TKOrder.isCompleted(oid);
        }

        /// <summary>
        /// 当日所有委托数据 
        /// </summary>
        public IEnumerable<Order> Orders { get { return this.TKOrder; } }


        /// <summary>
        /// 成交维护器
        /// </summary>
        public TradeTracker TKTrade { get; set; }

        /// <summary>
        /// 当日所有成交数据
        /// </summary>
        public IEnumerable<Trade> Trades { get { return this.TKTrade; } }

        /// <summary>
        /// 昨日持仓数据
        /// 用于管理端获得昨日持仓 当日成交 然后生成当前的交易持仓状态
        /// </summary>
        public IEnumerable<PositionDetail> YdPositions 
        {
            //这里不用单独维护LSPositionTracker从当前持仓中获得昨日持仓明细然后生成Position
            get 
            {
                List<PositionDetail> list = new List<PositionDetail>();
                foreach(Position p in this.Positions)
                {
                    foreach (PositionDetail pd in p.PositionDetailYdRef)
                    {
                        list.Add(pd);
                    }
                }
                return list;
            } 
        }

        /// <summary>
        /// 获得某个合约的持仓对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position GetPosition(string symbol, bool side)
        {
            return this.TKPosition[symbol, side];
        }
    }
}
