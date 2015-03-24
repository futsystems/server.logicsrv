using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    /// <summary>
    /// 仓位开平来回,从建仓开始 经过 加仓/减仓 最后 平掉 为一个仓位操作回合.系统对选手的考核通过仓位操作回合来进行
    /// 计算选手操作次数,胜率,平均持仓周期等数据
    /// </summary>
    public class PositionRoundImpl :PositionRound
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        /// <summary>
        /// 储存了成交序列,按照该序列的成交 完成了一个positionround
        /// </summary>
        ThreadSafeList<PositionTransaction> _postransactionlist = new ThreadSafeList<PositionTransaction>();

        /// <summary>
        /// 某个交易帐户 某个合约 某个方向上的持仓回合
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        public PositionRoundImpl(string account, Symbol symbol, bool side)
        {
            Account = account;//记录账户
            oSymbol = symbol;
            Side = side;
        }


        /// <summary>
        /// 对应合约对象
        /// </summary>
        public Symbol oSymbol { get; set; }

        /// <summary>
        /// 品种类别
        /// </summary>
        public SecurityType Type { get { return oSymbol.SecurityType; } }

        /// <summary>
        /// 乘数
        /// </summary>
        public int Multiple { get { return oSymbol.Multiple; } }


        public void SetOpen()
        {
            _opened = true;
        }


        bool _opened = false;
        /// <summary>
        /// 开仓回合开启标志
        /// </summary>
        public bool IsOpened { get { return _opened; } }

        bool _closed = false;
        /// <summary>
        /// 开仓回合关闭标志
        /// </summary>
        public bool IsClosed { get { return _closed; } }

        
        /// <summary>
        /// rpt获得一个positiontrans
        /// 持仓回合获得一个持仓成交记录
        /// </summary>
        /// <param name="postrans"></param>
        /// <returns></returns>
        public bool GotPositionTransaction(PositionTransaction postrans)
        {
            try
            {
                //回合记录已经关闭 则不再记录其他position
                if (IsClosed)
                {
                    //debug("回合关闭 拒绝记录");
                    return false;
                }
                //没有开启则忽略除开仓以外的其他仓位操作,没有初始开仓记录 则平仓 加仓 减仓 均不做记录
                if (!IsOpened && postrans.PosOperation != QSEnumPosOperation.EntryPosition)
                {
                    //debug("仓位没有开启,拒绝其他操作");
                    return false;//没有开仓 
                }
                //已经开仓,则忽略开仓记录
                if (IsOpened && postrans.PosOperation == QSEnumPosOperation.EntryPosition)
                {
                    //debug("仓位已经开启,拒绝开启操作");
                    return false;//已经开仓 又收到entrypostion操作
                }
                //以上为positionround的对成交的过滤

                //开仓
                if (postrans.PosOperation == QSEnumPosOperation.EntryPosition)
                {
                    //debug("仓位开启");
                    _opened = true;//标记已开仓
                    Side = postrans.Size > 0 ? true : false;//标记多空方向
                    _entrytime = postrans.Time;//记录开仓时间
                }

                //建仓或者加仓
                if ((postrans.PosOperation == QSEnumPosOperation.EntryPosition) || (postrans.PosOperation == QSEnumPosOperation.AddPosition))
                {
                    //debug("仓位增加");
                    int oldsize = _entrysize;
                    _entrysize += postrans.Size;//数量增加
                    _entryprice = (oldsize * _entryprice + postrans.Size * postrans.Price) / Convert.ToDecimal(_entrysize);//计算均价
                }

                //平仓或者减仓
                if ((postrans.PosOperation == QSEnumPosOperation.ExitPosition) || (postrans.PosOperation == QSEnumPosOperation.DelPosition))
                {
                    //debug("仓位减少");
                    int oldsize = _exitsize;
                    _exitsize += postrans.Size;//数量累加
                    _exitprice = (oldsize * _exitprice + postrans.Size * postrans.Price) / Convert.ToDecimal(_exitsize);//计算均价
                }

                //平仓
                if (postrans.PosOperation == QSEnumPosOperation.ExitPosition)
                {
                    //debug("仓位关闭");
                    _closed = true;
                    _opened = false;
                    _exittime = postrans.Time;
                }

                //记录成交记录所记录的最高价与最低价
                _highest = Math.Max(_highest, postrans.Highest);
                _lowest = Math.Min(_lowest, postrans.Lowest);

                //保存仓位变动数据
                _postransactionlist.Add(postrans);
                return true;
            }
            catch (Exception ex)
            {
                debug("some error:" + ex.ToString());
                return false;
            }           
        }

        /// <summary>
        /// 获得positiontransaction key
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string GetPRKey(PositionTransaction p)
        {
            return p.Account + "-" + p.Symbol+"-"+(p.Trade.PositionSide?QSEnumPositionDirectionType.Long.ToString():QSEnumPositionDirectionType.Short.ToString());
        }

        /// <summary>
        /// 获得position key
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string GetPRKey(Position position)
        {
			return position.Account + "-" + position.Symbol +"-"+ position.DirectionType.ToString();
        }

        public string PRKey
        {
            get {
                return Account+ "-" + Symbol +"-"+(Side?QSEnumPositionDirectionType.Long.ToString():QSEnumPositionDirectionType.Short.ToString());
            }
        }
        
        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get { return oSymbol.Symbol; }}

        /// <summary>
        /// 品种
        /// </summary>
        public string Security { get { return oSymbol.SecurityFamily.Code; } }

        /// <summary>
        /// 多空
        /// </summary>
        public bool Side { get; private set; }

        /// <summary>
        /// 当前持仓数量
        /// </summary>
        public int HoldSize { get { return (_entrysize + _exitsize); } }

        int _entrysize = 0;
        /// <summary>
        /// 总建仓数量
        /// </summary>
        public int EntrySize { get { return _entrysize; } set { _entrysize = value; } }

        DateTime _entrytime;
        /// <summary>
        /// 开仓时间
        /// </summary>
        public DateTime EntryTime { get { return _entrytime; } set { _entrytime = value; } }

        decimal _entryprice;
        /// <summary>
        /// 开仓价
        /// </summary>
        public decimal EntryPrice { get { return _entryprice; } set { _entryprice = value; } }

        /// <summary>
        /// 开仓手续费 通过累加所有开仓操作的手续费来得到 累计的开仓手续费
        /// </summary>
        public decimal EntryCommission { get {

            decimal total = 0;
            foreach (PositionTransaction pt in _postransactionlist)
            {
                if (pt.PosOperation == QSEnumPosOperation.EntryPosition || pt.PosOperation == QSEnumPosOperation.AddPosition)
                {
                    total += pt.Commission;
                }
            }
            return total;


            }}

        int _exitsize = 0;
        /// <summary>
        /// 总平仓数量
        /// </summary>
        public int ExitSize { get { return _exitsize; } set { _exitsize = value; } }
        DateTime? _exittime;
        /// <summary>
        /// 平仓时间
        /// </summary>
        public DateTime? ExitTime { get { return _exittime; } set { _exittime = value; } }

        decimal _exitprice;
        /// <summary>
        /// 平仓价格
        /// </summary>
        public decimal ExitPrice { get { return _exitprice; } set { _exitprice = value; } }


        /// <summary>
        /// 总平仓手续费
        /// </summary>
        public decimal ExitCommission { get {

            decimal total = 0;
            foreach (PositionTransaction pt in _postransactionlist)
            {
                if (pt.PosOperation == QSEnumPosOperation.ExitPosition || pt.PosOperation == QSEnumPosOperation.DelPosition)
                {
                    total += pt.Commission;
                }
            }
            return total;

            } } 



        decimal _highest = decimal.MinValue;
        public decimal Highest { get { return _highest==decimal.MinValue ?EntryPrice:_highest; }  }

        decimal _lowest = decimal.MaxValue;
        public decimal Lowest { get { return _lowest==decimal.MaxValue?EntryPrice:_lowest; } }


        //当计算仓位操作回合的时候 我们进行仓位操作是否closed的判断,如果没有closed则直接返回0
        /// <summary>
        /// 平均每手盈亏点数
        /// </summary>
        public decimal Points { get { return IsClosed ?((_entryprice - _exitprice) * (this.Side == true ? -1 : 1) ): 0; } }
        
        /// <summary>
        /// 单个回合总共盈亏点数
        /// </summary>
        public decimal TotalPoints { get { return IsClosed ?(Points * Math.Abs(this.EntrySize)):0; } }

        /// <summary>
        /// 盈亏(不含手续费)
        /// </summary>
        public decimal Profit { get { return IsClosed ?(TotalPoints * this.Multiple):0; } }

        /// <summary>
        /// 累计手续费
        /// </summary>
        public decimal Commissoin { get { return Math.Abs(EntryCommission) + Math.Abs(ExitCommission); } }

        /// <summary>
        /// 净盈亏
        /// </summary>
        public decimal NetProfit { get { return IsClosed ?(Profit - Commissoin):0; } }


        /// <summary>
        /// 盈亏标识
        /// </summary>
        public bool WL { get { return IsClosed ?((NetProfit >= 0)):true; } }

        /// <summary>
        /// 净交易数量
        /// </summary>
        public int Size { get { return Math.Abs(this.EntrySize); } }


        /// <summary>
        /// 判断一个持仓数据和PR数据是否吻合,account/symbol/holdsize
        /// PR的成本为所有开仓数据的加权成本，Position反应的是当前持仓的一个持有成本。当持有过程中发生过加减仓操作 价格数据就不吻合了
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool EqualPosition(Position p)
        {
            if (p.Account == Account && p.Symbol == Symbol && p.Size == HoldSize) return true;
            return false;
            
        }

        /// <summary>
        /// 判断当前持仓回合是否有效
        /// 用于从数据库恢复持仓回合数据 填充到持仓回合管理器中时的检查，避免相关字段为空造成的异常
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(this.Account)) return false;
                if (this.oSymbol == null) return false;

                return true;
            }
        }

        public override string ToString()
        {
                //return Account + "," + Symbol + "," +Security+","+ EntryTime.ToString() + "," + EntrySize.ToString() + "," + EntryPrice.ToString() + "," + ExitTime.ToString() + "," + ExitSize.ToString() + "," + ExitPrice.ToString() + "," + Highest.ToString() + "," + Lowest.ToString() + "," + HoldSize.ToString()+","+EntryCommission.ToString()+","+ExitCommission.ToString()+","+Side.ToString()+","+WL.ToString()+","+Points.ToString()+","+TotalPoints.ToString()+","+Profit.ToString()+","+Commissoin.ToString()+","+NetProfit.ToString();
            string nm = Account + "_" + Symbol + " 方向:" + Side.ToString() + " 开仓:" + EntryTime.ToString() + "," + EntrySize.ToString() + "," + Util.FormatDecimal(EntryPrice) + " 平仓:" + ExitTime.ToString() + "," + ExitSize.ToString() + "," + Util.FormatDecimal(ExitPrice) + " 最高:" + Util.FormatDecimal(Highest) + " 最底:" + Util.FormatDecimal(Lowest) + " 持有数量:" + HoldSize.ToString() + " 开仓手续费:" + Util.FormatDecimal(EntryCommission) + " 平仓手续费:" + Util.FormatDecimal(ExitCommission);

                if (IsClosed)
                    nm = nm + "盈亏:" + WL.ToString() + " 总点数:" + Util.FormatDecimal(TotalPoints) + " 总盈利:" + Util.FormatDecimal(Profit) + " 总手续费:" + Util.FormatDecimal(Commissoin) + " 净利润:" + Util.FormatDecimal(NetProfit);
                return nm;
        }


    }

    
}
