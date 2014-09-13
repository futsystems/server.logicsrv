using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public delegate StopLossArgs LookUpLossArgs(string sym);
    public delegate ProfitArgs LookUpProfitArgs(string sym);
    /// <summary>
    /// 传统止损 参数,用于快速设定 以及传递个对应的策略
    /// </summary>
    public class StopLossArgs
    {
        public StopLossArgs(Position pos, Symbol sec)
        {
            Enable = false;
            Type = StopOffsetType.POINTS;
            Value = 50;
            Size = 0;
            Position = pos;
            Security = sec;
        }

        public void CopyArgs(StopLossArgs args)
        {
            Enable = args.Enable;
            Type = args.Type;
            Value = args.Value;
            Size = args.Size;
        }
        public Position Position { get; set; }
        public Symbol Security { get; set; }

        public bool Enable { get; set; }
        /// <summary>
        /// 止损方式 固定点数,价格,百分比
        /// </summary>
        public StopOffsetType Type{get;set;}

        /// <summary>
        /// 设定值
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 触发时平仓的手数
        /// </summary>
        public int Size { get; set; }


        /// <summary>
        /// 计算触发价格
        /// </summary>
        /// <returns></returns>
        public decimal Caculate(Position pos)
        {
            switch (Type)
            {
                case StopOffsetType.POINTS://固定点数
                    return pos.isLong ? (pos.AvgPrice - Value) : (pos.AvgPrice + Value);
                case StopOffsetType.PRICE://价格
                    return Value;
                case StopOffsetType.PERCENT://百分比
                    return pos.isLong ? (pos.AvgPrice * (1 - Value / 100)) : (pos.AvgPrice * (1 + Value / 100));
                default:
                    return -1;
            }
        }

        public override string ToString()
        {
            if (!Enable)
                return "未设置";
            else
            {
                if (!Position.isFlat)
                    return string.Format(LibUtil.getDisplayFormat(Security.SecurityFamily.PriceTick), Caculate(this.Position));//type + "|" + Value + "|" + Size;
                else
                    return "已设置";
            }
        }
    }

    /// <summary>
    /// 传统止盈参数,用于快速设定 并传递给策略运行
    /// </summary>
    public class ProfitArgs
    {
        public ProfitArgs(Position pos, Symbol sec)
        {
            Enable = false;
            Type = ProfitOffsetType.POINTS;
            Value = 50;
            Size = 0;

            Position = pos;
            Security = sec;
        }

        public void CopyArgs(ProfitArgs args)
        {
            Enable = args.Enable;
            Type = args.Type;
            Value = args.Value;
            Size = args.Size;

            Start = args.Start;
        }


        public Position Position { get; set; }
        public Symbol Security { get; set; }

        public bool Enable { get; set; }

        /// <summary>
        /// 止损方式 固定点数,价格,百分比
        /// </summary>
        public ProfitOffsetType Type { get; set; }

        /// <summary>
        /// 设定值
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 回吐点数
        /// </summary>
        //public decimal Trailing { get; set; }
        /// <summary>
        /// 启动盈利点数
        /// </summary>
        public decimal Start { get; set; }
        /// <summary>
        /// 触发时平仓的手数
        /// </summary>
        public int Size { get; set; }

        decimal oldhit = 0;
        /// <summary>
        /// 计算触发价格
        /// </summary>
        /// <returns></returns>
        public decimal Caculate(Position pos)
        {
            decimal hit = -1;
            switch (Type)
            { 
                case ProfitOffsetType.POINTS://固定点数
                    hit= pos.isLong?(pos.AvgPrice + Value):(pos.AvgPrice-Value);
                    break;
                case ProfitOffsetType.PRICE://价格
                    hit= Value;
                    break;
                case ProfitOffsetType.PERCENT://百分比
                    hit= pos.isLong?(pos.AvgPrice * (1 + Value/100)):(pos.AvgPrice * (1 - Value/100));
                    break;
                case ProfitOffsetType.TRAILING:
                    {
                        if (pos.isLong && pos.AvgPrice>0)
                        {
                            if (pos.Highest > pos.AvgPrice + Start)
                                hit= pos.Highest - Value;
                        }
                        if(pos.isShort && pos.AvgPrice>0)
                        {
                            if (pos.Lowest < pos.AvgPrice - Start)
                                hit = pos.Lowest + Value;
                        }
                    }
                    break;
                default:
                    break;
            }
            if (oldhit != hit)
            {
                oldhit = hit;
            }
            return hit;
        }

        public override string ToString()
        {
            if (!Enable)
                return "未设置";
            else
            {

                if (Type == ProfitOffsetType.TRAILING)
                {
                    if (!Position.isFlat)
                    {
                        decimal hit = Caculate(this.Position);
                        if (hit == -1)
                            return "未触发";
                        else
                            return string.Format(LibUtil.getDisplayFormat(Security.SecurityFamily.PriceTick), Caculate(this.Position));//type + "|" + Value + "|" + Size;

                    }
                    else
                        return "已设置";
                }
                else
                {
                    if (!Position.isFlat)
                        return string.Format(LibUtil.getDisplayFormat(Security.SecurityFamily.PriceTick), Caculate(this.Position));//type + "|" + Value + "|" + Size;
                    else
                        return "已设置";

                }
            }

        }
    }


}
