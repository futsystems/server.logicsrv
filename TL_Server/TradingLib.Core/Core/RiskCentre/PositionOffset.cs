using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public class PositionOffset
    {
       

        public PositionOffset(string account, string symbol,bool positionside)
        {
            Account = account;
            Symbol = symbol;
            Side = positionside;
            Position = null;

            ProfitArgs = new PositionOffsetArgs(account, symbol,positionside, QSEnumPositionOffsetDirection.PROFIT);
            NeedTakeProfit = false;
            ProfitTakeOrder = -1;
            ProfitFireCount = 0;
            ProfitTakeTime = DateTime.Now;

            LossArgs = new PositionOffsetArgs(account, symbol, positionside,QSEnumPositionOffsetDirection.LOSS);
            NeedTakeLoss = false;
            LossTakeOrder = -1;
            LossFireCount = 0;
            LossTakeTime = DateTime.Now;
        }

        /// <summary>
        /// 重置positionoffset 参数
        /// </summary>
        public void Reset()
        {
            //Position = null;

            ProfitArgs.Enable = false;
            NeedTakeProfit = false;
            ProfitTakeOrder = -1;
            ProfitFireCount = 0;
            ProfitTakeTime = DateTime.Now;

            LossArgs.Enable = false;
            NeedTakeLoss = false;
            LossTakeOrder = -1;
            LossFireCount = 0;
            LossTakeTime = DateTime.Now;
        }

        public PositionOffsetArgs[] Args
        { 
            get
            {
                List<PositionOffsetArgs> l = new List<PositionOffsetArgs>();
                if (ProfitArgs != null)
                    l.Add(ProfitArgs);
                if (LossArgs != null)
                    l.Add(LossArgs);
                return l.ToArray();
            }
        }
        /// <summary>
        /// 获得客户端新提交上来的positionoffsetargs 数据 进行更新参数操作
        /// </summary>
        /// <param name="args"></param>
        public void GotPositionOffsetArgs(PositionOffsetArgs args)
        {
            if (args.Direction == QSEnumPositionOffsetDirection.PROFIT)
            {
                ProfitArgs.UpdateArgs(args);
            }
            if (args.Direction == QSEnumPositionOffsetDirection.LOSS)
            {
                LossArgs.UpdateArgs(args);
            }

        }

        public const int SendOrderDelay = 6;
        public const int SendRetry = 3;
        /// <summary>
        /// 账户-合约 序号
        /// </summary>
        public string Key { get { return Account + "-" + Symbol; } }

        /// <summary>
        /// 该监控所对应的账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 该监控的合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 限定持仓方向
        /// </summary>
        public bool Side { get; set; }
        /// <summary>
        /// 该positionoffset所监控的持仓
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// 止盈参数
        /// </summary>
        public PositionOffsetArgs ProfitArgs { get; set; }
        public long ProfitTakeOrder { get;set; }//是否已经发送止盈委托
        public int ProfitFireCount { get; set; }//止盈委托触发次数
        public bool NeedTakeProfit { get; set; }//是否需要出发止盈委托
        public DateTime ProfitTakeTime { get;set; }//止盈委托发出时间
        public decimal ProfitTakePrice
        {
            get
            {
                return CaculateProfitTakePrice(this.ProfitArgs, this.Position);
            }
        }
       

        /// <summary>
        /// 止损参数
        /// </summary>
        public PositionOffsetArgs LossArgs { get; set; }
        public long LossTakeOrder { get; set; }
        public int LossFireCount { get; set; }
        public bool NeedTakeLoss { get; set; }
        public DateTime LossTakeTime { get;set; }
        public decimal LossTakePrice {
            get {
                return CaculateLossTakePrice(this.LossArgs, this.Position);
            }
        }

        /// <summary>
        /// 是否设置了有效的止盈止损参数
        /// </summary>
        public bool NeedCheck {
            get {
                if ((ProfitArgs != null && ProfitArgs.Enable) || (LossArgs != null && LossArgs.Enable))
                    return true;
                else
                    return false;
            }
        }
        public override string ToString()
        {
            string pstr = ProfitArgs != null ? (ProfitArgs.ToString() + " Price:" + ProfitTakePrice.ToString()) : "无止盈";
            string lstr = LossArgs !=null ?(LossArgs.ToString() + " Price:" + LossTakePrice.ToString()):"无止损";

            return Key + " " + pstr + " " + lstr;
        }
        /// <summary>
        /// 检查止损
        /// </summary>
        public bool CheckTakeLoss(Tick k)
        {
            if (LossArgs != null && LossArgs.Enable)
            {
                decimal hitprice = LossTakePrice;
                if (k.trade <= hitprice)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查止盈
        /// </summary>
        /// <returns></returns>
        public bool CheckTakeProfit(Tick k)
        {
            if (ProfitArgs != null && ProfitArgs.Enable)
            {
                if (ProfitArgs.OffsetType == QSEnumPositionOffsetType.TRAILING)
                {
                    decimal hitprice = ProfitTakePrice;
                    if (hitprice > 0)
                    {
                        if (k.trade <= hitprice)
                        {
                            return true;//执行止盈
                        }
                    }
                }
                else
                {
                    decimal hitprice = ProfitTakePrice;
                    if (k.trade >= hitprice)
                    {
                        return true;//执行止盈
                    }
                }
            }
            return false;//不止盈
        }

        /// <summary>
        /// 计算止损价格
        /// </summary>
        /// <param name="LossArgs"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal CaculateLossTakePrice(PositionOffsetArgs lossArgs, Position pos)
        {
            if (lossArgs == null || pos == null) return -1;
            decimal hitprice = -1;
            switch (lossArgs.OffsetType)
            {
                case QSEnumPositionOffsetType.POINTS:
                    hitprice = pos.isLong ? (pos.AvgPrice - lossArgs.Value) : (pos.AvgPrice + lossArgs.Value);
                    break;
                case QSEnumPositionOffsetType.PRICE:
                    hitprice = lossArgs.Value;
                    break;
                case QSEnumPositionOffsetType.PERCENT:
                    hitprice = pos.isLong ? (pos.AvgPrice * (1 - lossArgs.Value / 100)) : (pos.AvgPrice * (1 + lossArgs.Value / 100));
                    break;
                case QSEnumPositionOffsetType.TRAILING:
                    {
                        if (pos.isLong && pos.AvgPrice > 0)
                        {
                            if (pos.Highest > pos.AvgPrice + lossArgs.Start)
                                hitprice = pos.Highest - lossArgs.Value;
                        }
                        if (pos.isShort && pos.AvgPrice > 0)
                        {
                            if (pos.Lowest < pos.AvgPrice - lossArgs.Start)
                                hitprice = pos.Lowest + lossArgs.Value;
                        }
                    }
                    break;
                default:
                    break;
            }
            return hitprice;
        }
        /// <summary>
        /// 计算止盈价格
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal CaculateProfitTakePrice(PositionOffsetArgs profitArgs, Position pos)
        {
            if (profitArgs == null || pos == null) return -1;
            decimal hitprice = -1;
            switch (profitArgs.OffsetType)
            {
                case QSEnumPositionOffsetType.POINTS:
                    hitprice = pos.isLong ? (pos.AvgPrice + profitArgs.Value) : (pos.AvgPrice - profitArgs.Value);
                    break;
                case QSEnumPositionOffsetType.PRICE:
                    hitprice = profitArgs.Value;
                    break;
                case QSEnumPositionOffsetType.PERCENT:
                    hitprice = pos.isLong ? (pos.AvgPrice * (1 + profitArgs.Value / 100)) : (pos.AvgPrice * (1 - profitArgs.Value / 100));
                    break;
                case QSEnumPositionOffsetType.TRAILING:
                    {
                        if (pos.isLong && pos.AvgPrice > 0)
                        {
                            if (pos.Highest > pos.AvgPrice + profitArgs.Start)
                                hitprice = pos.Highest - profitArgs.Value;
                        }
                        if (pos.isShort && pos.AvgPrice > 0)
                        {
                            if (pos.Lowest < pos.AvgPrice - profitArgs.Start)
                                hitprice = pos.Lowest + profitArgs.Value;
                        }
                    }
                    break;
                default:
                    break;
            }
            return hitprice;
        }

    }

}
