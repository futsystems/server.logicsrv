using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 开仓或者加仓数据
    /// </summary>
    [Serializable]
    public class PositionTransaction
    {
        /// <summary>
        /// 通过持仓前后数量 获得对应的仓位操作标识
        /// </summary>
        /// <param name="beforesize"></param>
        /// <param name="aftersize"></param>
        /// <returns></returns>
        public static QSEnumPosOperation GenPositionOperation(int beforesize,int aftersize)
        {
            if (aftersize > beforesize)
            {
                if (beforesize == 0)
                    return QSEnumPosOperation.EntryPosition;
                else
                    return QSEnumPosOperation.AddPosition;
            }
            else
            {
                if (aftersize == 0)
                    return QSEnumPosOperation.ExitPosition;
                else
                    return QSEnumPosOperation.DelPosition;
            }
        }

        /// <summary>
        /// 生成PositionTransaction
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="security"></param>
        /// <param name="beforesize"></param>
        /// <param name="aftersize"></param>
        /// <param name="highest"></param>
        /// <param name="lowest"></param>
        public PositionTransaction(Trade fill, Symbol symbol, int beforesize, int aftersize, decimal highest, decimal lowest)
        {
            Trade = fill;
            oSymbol = symbol;

            BeforeSize = beforesize;
            AfterSize = aftersize;
            Highest = highest;
            Lowest = lowest;

            
        }

        /// <summary>
        /// 复制一个PositionTransaction
        /// </summary>
        /// <param name="copythis"></param>
        public PositionTransaction(PositionTransaction copythis)
        {
            this.Trade = copythis.Trade;
            this.oSymbol = copythis.oSymbol;
            BeforeSize = copythis.BeforeSize;
            AfterSize = copythis.AfterSize;
            Highest = copythis.Highest;
            Lowest = copythis.Lowest;            
        }

        /// <summary>
        /// 该成交前仓位数量(绝对值)
        /// </summary>
        public int BeforeSize { get; private set; }

        /// <summary>
        /// 该成交后仓位数量(绝对值)
        /// </summary>
        public int AfterSize { get; private set; }

        /// <summary>
        /// 该positiontransaction底层的成交记录
        /// </summary>
        public Trade Trade { get;private set; }
        /// <summary>
        /// 仓位操作对应的品种数据
        /// </summary>
        //public Symbol SecurityInstance { get; private set; }
        /// <summary>
        /// 仓位操作标识 开仓 加仓 减仓 平仓
        /// </summary>
        public QSEnumPosOperation PosOperation { get {return  PositionTransaction.GenPositionOperation(BeforeSize,AfterSize); } }

        public Symbol oSymbol { get; private set; }
        /// <summary>
        /// 交易帐户ID
        /// </summary>
        public string Account { get { return Trade.Account; } }
        /// <summary>
        /// 交易合约
        /// </summary>
        public string Symbol { get { return oSymbol.Symbol; } }
        /// <summary>
        /// 品种
        /// </summary>
        public string Security { get { return oSymbol.FullName; } }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get { return Util.ToDateTime(Trade.xdate, Trade.xtime); } }
        /// <summary>
        /// 数量
        /// </summary>
        public int Size { get { return Trade.xsize; } }
        /// <summary>
        /// 合约乘数
        /// </summary>
        public int Multiple { get { return oSymbol.Multiple; } }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get { return Trade.xprice; } }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get { return Trade.Commission; } }
        /// <summary>
        /// 持仓期间最高价
        /// </summary>
        public decimal Highest { get; private set; }
        /// <summary>
        /// 持仓期间最低价
        /// </summary>
        public decimal Lowest { get; private set; }

        public override string ToString()
        {
            string s = Account + LibUtil.GetEnumDescription(PosOperation) + "  " + Size.ToString() + "手" + Symbol.ToString() + " @ " + Price.ToString() + "#" + Time.ToString();
            return s;
        }

        
    }
}
