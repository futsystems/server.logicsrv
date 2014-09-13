using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 持仓止盈与止损,用于向服务端提交止损与止盈参数,从而实现服务端的止损与止盈
    /// </summary>
    public class PositionOffsetArgs
    {
        public PositionOffsetArgs(string account, string symbol,QSEnumPositionOffsetDirection direction)
        {
            _account = account;
            _symbol = symbol;
            _direction = direction;
            Enable = false;
            OffsetType = QSEnumPositionOffsetType.POINTS;
            Value = 0;
            Start = 0;
            Size = 0;
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

        string _account;
        /// <summary>
        /// 该监控所对应的账户
        /// </summary>
        public string Account { get { return _account; } }

        string _symbol;
        /// <summary>
        /// 该监控的合约
        /// </summary>
        public string Symbol { get { return _symbol; } }

        QSEnumPositionOffsetDirection _direction;
        /// <summary>
        /// 止盈还是止损标识
        /// </summary>
        public QSEnumPositionOffsetDirection Direction { get { return _direction; } }

        /// <summary>
        /// 止盈是否有效
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 止盈方式
        /// </summary>
        public QSEnumPositionOffsetType OffsetType { get; set; }

        /// <summary>
        /// 止盈值
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 跟踪止盈的启动值
        /// </summary>
        public decimal Start { get; set; }

        /// <summary>
        /// 止盈手数
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 用一个止盈止损参数来更新当前止盈止损参数
        /// </summary>
        /// <param name="args"></param>
        public void UpdateArgs(PositionOffsetArgs args)
        {
            //account symbol direction相同的情况下才可以传递参数
            if((Account == args.Account) && (Symbol == args.Symbol) &&(Direction == args.Direction))
            {
                Enable = args.Enable;
                OffsetType = args.OffsetType;
                Value = args.Value;
                Start = args.Start;
                Size = args.Size;
            }
        }
        /// <summary>
        /// 计算针对某个position其触发的止盈止损价格
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public decimal TargetPrice(Position pos) {
                if (!this.Enable) return -1;
                decimal price = 0;
                if (Direction == QSEnumPositionOffsetDirection.LOSS)
                {
                    price = CaculateLossTakePrice(this, pos);
                    return price;
                }
                else
                {
                    price = CaculateProfitTakePrice(this, pos);
                    return price;
                }
        }
        public override string ToString()
        {
            return LibUtil.GetEnumDescription(Direction) + " " + (Enable ? "有效" : "无效") + " 类型:" + OffsetType.ToString() +" V:"+Value.ToString() +" S:"+Size.ToString() +" St:"+Start.ToString();
        }
        public static string Serialize(PositionOffsetArgs po)
        {
            const char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(po.Account);
            sb.Append(d);
            sb.Append(po.Symbol);
            sb.Append(d);
            sb.Append(po.Enable.ToString());
            sb.Append(d);
            sb.Append(po.Direction);
            sb.Append(d);
            sb.Append(po.OffsetType.ToString());
            sb.Append(d);
            sb.Append(po.Value.ToString());
            sb.Append(d);
            sb.Append(po.Size.ToString());
            sb.Append(d);
            sb.Append(po.Start.ToString());

            return sb.ToString();
        }

        public static PositionOffsetArgs Deserialize(string message)
        {
            string[] rec = message.Split(',');
            if (rec.Length < 8) return null;

            string account = rec[0];
            string symbol = rec[1];
            bool enable = Convert.ToBoolean(rec[2]);
            QSEnumPositionOffsetDirection direction = (QSEnumPositionOffsetDirection)Enum.Parse(typeof(QSEnumPositionOffsetDirection), rec[3]);
            QSEnumPositionOffsetType type = (QSEnumPositionOffsetType)Enum.Parse(typeof(QSEnumPositionOffsetType), rec[4]);
            decimal value = Convert.ToDecimal(rec[5]);
            int size = Convert.ToInt32(rec[6]);
            decimal start = Convert.ToDecimal(rec[7]);

            PositionOffsetArgs po = new PositionOffsetArgs(account, symbol, direction);
            po.Enable = enable;
            po.OffsetType = type;

            po.Value = value;
            po.Size = size;
            po.Start = start;

            return po;
        }
    }


}
