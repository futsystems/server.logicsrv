using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 净持仓操作结果 
    /// 只针对开仓做处理,平仓的话按照在那里开仓就在哪里平仓的原则,不管保证金变化,均要在该通道进行平仓
    /// </summary>
    public class NetPositionOperationResult
    {
        public NetPositionOperationResult()
        {
            this.LongEntry = 0;
            this.LongExit = 0;
            this.ShortEntry = 0;
            this.ShortExit = 0;
        }
        /// <summary>
        /// 多头开仓数量
        /// </summary>
        public int LongEntry { get; set; }
        /// <summary>
        ///多头平仓数量
        /// </summary>
        public int LongExit { get; set; }
        /// <summary>
        /// 空头开仓数量
        /// </summary>
        public int ShortEntry { get; set; }
        /// <summary>
        /// 空头平仓数量
        /// </summary>
        public int ShortExit { get; set; }
    }


    /// <summary>
    /// 经持仓状态度量 用于判断是否可以进行净持仓操作
    /// </summary>
    public class NetPositionMetric
    {
        public NetPositionMetric()
        {
            this.Symbol = string.Empty;
            this.LongHoldSize = 0;
            this.LongPendingEntrySize = 0;
            this.LongPendingExitSize = 0;

            this.ShortHoldSize = 0;
            this.ShortPendingEntrySize = 0;
            this.ShortPendingExitSize = 0;
        }
        public NetPositionMetric(NetPositionMetric copy)
        {
            this.Symbol = copy.Symbol;
            this.LongHoldSize = copy.LongHoldSize;
            this.LongPendingExitSize = copy.LongPendingExitSize;
            this.LongPendingEntrySize = copy.LongPendingEntrySize;

            this.ShortHoldSize = copy.ShortHoldSize;
            this.ShortPendingEntrySize = copy.ShortPendingEntrySize;
            this.ShortPendingExitSize = copy.ShortPendingExitSize;
        }
        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 多头持有仓位
        /// </summary>
        public int LongHoldSize { get; set; }

        /// <summary>
        /// 空头持仓仓位
        /// </summary>
        public int ShortHoldSize { get; set; }


        /// <summary>
        /// 多头待开数量
        /// </summary>
        public int LongPendingEntrySize { get; set; }

        /// <summary>
        /// 多头待平数量
        /// </summary>
        public int LongPendingExitSize { get; set; }

        /// <summary>
        /// 多方可以平掉的数量
        /// </summary>
        public int LongCanExitSize { get { return LongHoldSize - LongPendingExitSize; } }

        /// <summary>
        /// 空头待开数量
        /// </summary>
        public int ShortPendingEntrySize { get; set; }

        /// <summary>
        /// 空头待平数量
        /// </summary>
        public int ShortPendingExitSize { get; set; }

        /// <summary>
        /// 空头可以平掉的数量
        /// </summary>
        public int ShortCanExitSaize { get { return ShortHoldSize - ShortPendingExitSize; } }


        public static NetPositionMetric(Position longpos, Position shortpos)
        {
            NetPositionMetric mertic = new NetPositionMetric();
            mertic.Symbol = longpos.Symbol;
            //mertic.LongHoldSize = 
        }

        /// <summary>
        /// 按算法计算的净持仓操作 所增加的有效持仓数量
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static int GenPositionIncrement(NetPositionMetric mertic,NetPositionOperationResult operation,bool justoneside=false)
        {
            if (justoneside)
            {
                int size = Math.Max(mertic.LongHoldSize, mertic.ShortHoldSize);//取多空持仓中大的一个
                int aftersize = Math.Max(mertic.LongHoldSize + operation.LongEntry - operation.LongExit, mertic.ShortHoldSize + operation.ShortEntry - operation.ShortExit);
                return aftersize - size;
            }
            else
            { 
                //开仓数量- 平仓数量 = 累计的开仓数量
                return operation.LongEntry + operation.ShortEntry - operation.LongExit - operation.ShortExit;
            }
        }


        /// <summary>
        /// 返回净持仓开仓操作结果
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static NetPositionOperationResult GenNetPositionEntryResult(NetPositionMetric mertic, Order o)
        {
            bool posside = o.PositionSide;//持仓方向
            int opensize = o.UnsignedSize;//委托数量

            NetPositionOperationResult result = new NetPositionOperationResult();
            int canexitsize = posside ? mertic.ShortCanExitSaize : mertic.LongCanExitSize;//获得反向持仓的可平数量
            //方式一 开仓数量 可以全部转换成平仓数量 该操作不会增加任何保证金
            //如果开仓的数量 小于等于 反向持仓的可平数量,则该委托可以以净持仓的方式处理
            if (opensize <= canexitsize)
            {
                if (posside)//多头开仓
                {
                    result.ShortExit = opensize;
                }
                else//空头开仓
                {
                    result.LongExit = opensize;
                }
            }
            else
            {
                if (posside)
                {
                    result.ShortExit = canexitsize;
                    result.LongEntry = opensize - canexitsize;
                }
                else
                {
                    result.LongExit = canexitsize;
                    result.ShortEntry = opensize - canexitsize;
                }
            }
            return result;
        }
    }
}
