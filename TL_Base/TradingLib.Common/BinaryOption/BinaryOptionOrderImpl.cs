using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;

namespace TradingLib.Common
{
    public class BinaryOptionOrderImpl:BinaryOptionOrder
    {

        public BinaryOptionOrderImpl()
        {
            this.ID = 0;
            this.Account = string.Empty;
            this.Date = Util.ToTLDate();
            this.Time = Util.ToTLTime();
            this.Symbol = string.Empty;
            this.oSymbol = null;
            this.LocalSymbol = string.Empty;


            this.Side = true;
            this.Amount = 0;
            this.OptionType = EnumBinaryOptionType.UpDown;
            this.SuccessRatio = 0.7M;
            this.FailRatio = 1;
            this.Result = false;
            this.Status = EnumBOOrderStatus.Unknown;
            this.TimeSpanType = EnumBOTimeSpan.MIN1;
            this.NextRoundedTime = 0;


            this.EntryTime = 0;
            this.EntryPrice = 0;
            this.ExitTime = 0;
            this.ExitPrice = 0;

            this.LastPrice = 0;
            this.Highest = decimal.MinValue;
            this.Lowest = decimal.MaxValue;
        }


        public BinaryOptionOrderImpl(BinaryOptionOrder o)
        {
            this.ID = o.ID;
            this.Account = o.Account;
            this.Date = o.Date;
            this.Time = o.Time;
            this.Symbol = o.Symbol;
            this.LocalSymbol = o.LocalSymbol;
            this.oSymbol = o.oSymbol;


            this.Amount = o.Amount;
            this.SuccessRatio = o.SuccessRatio;
            this.FailRatio = o.FailRatio;

            this.OptionType = o.OptionType;
            this.TimeSpanType = o.TimeSpanType;
            this.Status = o.Status;

            this.Side = o.Side;
            
            this.ExitTime = o.ExitTime;
            this.EntryTime = o.EntryTime;
            this.NextRoundedTime = o.NextRoundedTime;

            this.ExitPrice = o.ExitPrice;
            this.EntryPrice = o.EntryPrice;
            

            this.LastPrice = o.LastPrice;
            this.Highest = o.Highest;
            this.Lowest = o.Lowest;

            this.Result = o.Result;



        }

        /// <summary>
        /// 委托编号
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 交易账号
        /// </summary>
        public string Account { get; set; }


        /// <summary>
        /// 日期
        /// </summary>
        public int Date { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public int Time { get; set; }


        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 本地合约编号
        /// </summary>
        public string LocalSymbol { get; set; }

        /// <summary>
        /// 合约对象
        /// </summary>
        public Symbol oSymbol { get; set; }



        #region 金额以及回报率数据
        /// <summary>
        /// 下单金额
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        /// 成功返回系数
        /// </summary>
        public decimal SuccessRatio { get; set; }

        /// <summary>
        /// 失败扣费系数
        /// </summary>
        public decimal FailRatio { get; set; }


        #endregion

        /// <summary>
        /// 二元期权类别
        /// </summary>
        public EnumBinaryOptionType OptionType { get; set; }

        /// <summary>
        /// 时间间隔
        /// </summary>
        public EnumBOTimeSpan TimeSpanType { get; set; }     

        /// <summary>
        /// 委托状态
        /// </summary>
        public EnumBOOrderStatus Status { get; set; }


        string _comment = string.Empty;
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Comment { get { return _comment; } set { _comment = value; } }

        /// <summary>
        /// 成交方向
        /// </summary>
        public bool Side { get; set; }


        #region 时间信息
        /// <summary>
        /// 开权时间
        /// </summary>
        public long EntryTime { get; set; }


        /// <summary>
        /// 下一轮时间
        /// </summary>
        public long NextRoundedTime { get; set; }

        

        /// <summary>
        /// 平权时间
        /// </summary>
        public long ExitTime { get; set; }

        #endregion



        #region 价格信息
        /// <summary>
        /// 开权价格
        /// </summary>
        public decimal EntryPrice { get; set; }

        /// <summary>
        /// 平权价格
        /// </summary>
        public decimal ExitPrice { get; set; }

        /// <summary>
        /// 最新价格
        /// </summary>
        public decimal LastPrice { get; set; }

        /// <summary>
        /// 持权期内最高价
        /// </summary>
        public decimal Highest { get; set; }

        /// <summary>
        /// 持权期内最低价
        /// </summary>
        public decimal Lowest { get; set; }

        #endregion

        /// <summary>
        /// 平权胜负
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 响应行情数据
        /// 注意 行情事件统一调整成系统本地时间
        /// 返回true表面委托关闭
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            //持权状态才处理行情更新
            if (this.Status == EnumBOOrderStatus.Entry)
            {
                if (k.Symbol != (this.oSymbol != null ? this.oSymbol.TickSymbol : this.Symbol))//非本委托合约的行情直接返回
                    return;

                long ktime = k.Date * 1000000 + k.Time;//130101

                //行情事件大于我们设定的下次平权时间 则执行平权操作
                if (ktime >= this.NextRoundedTime)
                { 
                    //执行平权
                    BinaryOptionOrderImpl.ExitOrder(this as BinaryOptionOrder);
                    return;
                }

                decimal _last = 0;
                bool _needupdate = false;
                //只处理成交价格
                if (k.IsTrade())
                {
                    _last = k.Trade;
                    _needupdate = true;
                }

                if (_needupdate)
                {
                    //如果处于持权状态,则需要更新行情数据到先关参数
                    this.LastPrice = _last;
                    this.Highest = this.Highest >= _last ? this.Highest : _last;
                    this.Lowest = this.Lowest <= _last ? this.Lowest : _last;

                }
            }

        }


        /// <summary>
        /// 响应定时时间
        /// </summary>
        /// <param name="time"></param>
        public void GotTime(long time)
        {
            if (this.Status == EnumBOOrderStatus.Entry)
            {
                if (time >= this.NextRoundedTime)
                {
                    //执行平权
                    BinaryOptionOrderImpl.ExitOrder(this as BinaryOptionOrder);
                }
            }
        }
        /// <summary>
        /// 浮动盈亏 根据不同的权类状态判定当前浮动盈亏,需要实时更新当前最新价格
        /// </summary>
        public decimal UnRealizedPL
        {
            get
            {
                switch (this.Status)
                {
                    case EnumBOOrderStatus.Entry:
                        {
                            switch (this.OptionType)
                            {
                                //涨跌 价格高于开仓价格为赢 否则为数
                                case EnumBinaryOptionType.UpDown:
                                    if (this.Side)
                                    {
                                        if (this.LastPrice > this.EntryPrice) return this.Amount * this.SuccessRatio;
                                        return -1 * this.Amount * this.FailRatio;
                                    }
                                    else
                                    {
                                        if (this.LastPrice < this.EntryPrice) return this.Amount * this.SuccessRatio;
                                        return -1 * this.Amount * this.FailRatio;
                                    }
                                case EnumBinaryOptionType.RangeIn:
                                case EnumBinaryOptionType.RangeOut:
                                    return 0;
                                default:
                                    return 0;
                            }
                        }
                    default:
                        return 0;
                }
            }
        }

        decimal ?_realizedpl = null;
        /// <summary>
        /// 平权盈亏 平权后 按最终状态计算盈亏权益
        /// </summary>
        public decimal RealizedPL
        {
            get
            {
                if (_realizedpl != null) return (decimal)_realizedpl;
                switch (this.Status)
                {
                        //平权后 根据输赢计算 平权盈亏
                    case EnumBOOrderStatus.Exit:
                        {
                            if (this.Result)
                            {
                                return this.Amount * this.SuccessRatio;
                            }
                            else
                            {
                                return this.Amount * this.FailRatio;
                            }
                        }
                        //其余状态 平权盈亏为0
                    default:
                        return 0;
                }
                
            }
            set
            {
                if (this.Status == EnumBOOrderStatus.Exit)
                {
                    _realizedpl = value;
                }
            }
        }


        /// <summary>
        /// 按开权时间计算某个时间间隔后的确权时间
        /// 计算下一个Round时间
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static long CalcNextRoundedTime(long entrytime, EnumBOTimeSpan type)
        { 
            TimeSpan ts = TimeSpan.FromMinutes(1);
            switch(type)
            {
                case EnumBOTimeSpan.MIN1:
                    ts = TimeSpan.FromMinutes(1);
                    break;
                case EnumBOTimeSpan.MIN2:
                    ts = TimeSpan.FromMinutes(2);
                    break;
                case EnumBOTimeSpan.MIN5:
                    ts = TimeSpan.FromMinutes(5);
                    break;
                case EnumBOTimeSpan.MIN10:
                    ts = TimeSpan.FromMinutes(10);
                    break;
                default:
                    break;
            }

            DateTime dt =   TimeFrequency.NextRoundedTime(Util.ToDateTime(entrytime), ts);
            return dt.ToTLDateTime();
        }

        /// <summary>
        /// 开权操作
        /// </summary>
        /// <param name="order"></param>
        /// <param name="k"></param>
        public static void EntryOrder(BinaryOptionOrder order, Tick k)
        {
            //记录开权时间
            order.EntryTime = DateTime.Now.ToTLTime();
            //计算平权时间
            order.NextRoundedTime = BinaryOptionOrderImpl.CalcNextRoundedTime(order.EntryTime, order.TimeSpanType);
            //记录开权价格
            order.EntryPrice = k.Trade;
            order.Status = EnumBOOrderStatus.Entry;
        }


        /// <summary>
        /// 平权操作
        /// </summary>
        /// <param name="order"></param>
        public static void ExitOrder(BinaryOptionOrder order)
        {
            order.ExitTime = DateTime.Now.ToTLTime();
            order.ExitPrice = order.LastPrice;
            order.Status = EnumBOOrderStatus.Exit; 

            //判定最终输赢状态
            switch (order.OptionType)
            {
                case EnumBinaryOptionType.UpDown:
                    {
                        if (order.Side)
                        {
                            if (order.ExitPrice > order.EntryPrice)
                            {
                                order.Result = true;
                            }
                            else
                            {
                                order.Result = false;
                            }
                        }
                        else
                        {
                            if (order.ExitPrice < order.EntryPrice)
                            {
                                order.Result = true;
                            }
                            else
                            {
                                order.Result = false;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
