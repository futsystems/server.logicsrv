using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    public class SettlePosition
    {
        public SettlePosition()
        {
            this.Account = string.Empty;
            this.Symbol = string.Empty;
            this.Size = 0;
            this.AVGPrice = 0;
            this.SettlePrice = 0;
            this.Settleday = 0;
            this.SecurityCode = string.Empty;
            this.Margin = 0;
        }
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 持仓数量
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 持仓均价
        /// </summary>
        public decimal AVGPrice { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlePrice { get; set; }

        /// <summary>
        /// 结算日
        /// </summary>
        public int Settleday { get; set; }

        /// <summary>
        /// 乘数
        /// </summary>
        public int Multiple { get; set; }

        /// <summary>
        /// 占用保证金
        /// </summary>
        public decimal Margin { get; set; }

        /// <summary>
        /// 品种字头
        /// </summary>
        public string SecurityCode { get; set; }

        public static string Serialize(SettlePosition p)
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(p.Account);
            sb.Append(d);
            sb.Append(p.Symbol);
            sb.Append(d);
            sb.Append(p.Size.ToString());
            sb.Append(d);
            sb.Append(p.AVGPrice.ToString());
            sb.Append(d);
            sb.Append(p.SettlePrice.ToString());
            sb.Append(d);
            sb.Append(p.Settleday.ToString());
            sb.Append(d);
            sb.Append(p.Multiple.ToString());
            sb.Append(d);
            sb.Append(p.SecurityCode);
            return sb.ToString();
        }

        public static SettlePosition Deserialize(string msg)
        {
            string[] rec = msg.Split(',');
            SettlePosition pos = new SettlePosition();
            pos.Account = rec[0];
            pos.Symbol = rec[1];
            pos.Size = int.Parse(rec[2]);
            pos.AVGPrice = decimal.Parse(rec[3]);
            pos.SettlePrice = decimal.Parse(rec[4]);
            pos.Settleday = int.Parse(rec[5]);
            pos.Multiple = int.Parse(rec[6]);
            pos.SecurityCode = rec[7];
            return pos;
        }
    }

    /// <summary>
    /// PostionEx用于封装持仓汇总信息
    /// 将Position工作对象的持仓状态 封装成PositionEx用于接收客户端的查询
    /// </summary>
    public  class PositionEx
    {
        /// <summary>
        /// 帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 乘数
        /// </summary>
        public int Multiple { get; set; }

        /// <summary>
        /// 平仓盈亏点数
        /// </summary>
        public decimal ClosedPL{ get; set; }

        /// <summary>
        /// 持仓数量
        /// </summary>
        public int UnsignedSize { get; set; }

        /// <summary>
        /// 持仓成本 点数
        /// </summary>
        public decimal AvgPrice { get; set; }

        /// <summary>
        /// 持仓方向
        /// </summary>
        public bool Side { get; set; }

        /// <summary>
        /// 带方向的数量
        /// </summary>
        public int Size { get; set; }

        #region 开平汇总
        /// <summary>
        /// 开仓量
        /// </summary>
        public int OpenVolume { get; set; }

        /// <summary>
        /// 开仓金额
        /// </summary>
        public decimal OpenAmount { get; set; }

        /// <summary>
        /// 开仓均价
        /// </summary>
        public decimal OpenAVGPrice { get; set; }

        /// <summary>
        /// 平仓量
        /// </summary>
        public int CloseVolume { get; set; }

        /// <summary>
        /// 平仓金额
        /// </summary>
        public decimal CloseAmount { get; set; }

        /// <summary>
        /// 平仓均价
        /// </summary>
        public decimal CloseAVGPrice { get; set; }

        #endregion


        /// <summary>
        /// 持仓描述类型
        /// </summary>
        public QSEnumPositionDirectionType DirectionType { get; set; }

        /// <summary>
        /// 当日平仓盈亏金额
        /// </summary>
        public decimal CloseProfit { get; set; }

        /// <summary>
        /// 持仓成本金额
        /// </summary>
        public decimal PositionCost { get; set; }

        /// <summary>
        /// 当日浮动盈亏点数
        /// </summary>
        public decimal UnRealizedPL { get; set; }

        /// <summary>
        /// 浮动盈亏金额/持仓盈亏
        /// </summary>
        public decimal UnRealizedProfit { get; set; }

        /********后期添加*******/
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }


        /// <summary>
        /// 今仓数量
        /// </summary>
        public int TodayPosition { get; set; }

        /// <summary>
        /// 昨仓数量
        /// </summary>
        public int YdPosition { get; set; }

        //昨仓数量 + 今仓数量 = 总持仓数量 = UnsignedSize 

        /// <summary>
        /// 昨日结算价
        /// </summary>
        public decimal LastSettlementPrice { get; set; }


        /// <summary>
        /// 结算价
        /// </summary>
        public decimal SettlementPrice { get; set; }


        /// <summary>
        /// 盯市平仓盈亏
        /// 每个持仓明细中有一个盯市平仓盈亏 用于结算时累计盯市盈亏
        /// </summary>
        public decimal CloseProfitByDate { get; set; }

        /// <summary>
        /// 交易日
        /// </summary>
        public int Tradingday { get; set; }

        public PositionEx()
        {
            Account = string.Empty;
            Symbol = string.Empty;
            Multiple = 1;
            ClosedPL = 0;
            UnsignedSize = 0;
            AvgPrice = 0;
            Side = true;
            Size = 0;

            OpenAmount = 0;
            OpenAVGPrice = 0;
            OpenVolume = 0;
            CloseAmount = 0;
            CloseAVGPrice = 0;
            CloseVolume = 0;
            DirectionType = QSEnumPositionDirectionType.BothSide;
            CloseProfit = 0;
            PositionCost = 0;
            UnRealizedPL = 0;
            UnRealizedProfit = 0;
        }

        public static string Serialize(PositionEx p)
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(p.Account);
            sb.Append(d);
            sb.Append(p.Symbol);
            sb.Append(d);
            sb.Append(p.Multiple.ToString());
            sb.Append(d);
            sb.Append(p.ClosedPL.ToString());
            sb.Append(d);
            sb.Append(p.UnsignedSize.ToString());
            sb.Append(d);
            sb.Append(p.AvgPrice.ToString());
            sb.Append(d);
            sb.Append(p.Side.ToString());
            sb.Append(d);
            sb.Append(p.Size.ToString());
            sb.Append(d);
            sb.Append(p.OpenAmount.ToString());
            sb.Append(d);
            sb.Append(p.OpenAVGPrice.ToString());
            sb.Append(d);
            sb.Append(p.OpenVolume.ToString());
            sb.Append(d);
            sb.Append(p.CloseAmount.ToString());
            sb.Append(d);
            sb.Append(p.CloseAVGPrice.ToString());
            sb.Append(d);
            sb.Append(p.CloseVolume.ToString());
            sb.Append(d);
            sb.Append(p.DirectionType.ToString());
            sb.Append(d);
            sb.Append(p.CloseProfit.ToString());
            sb.Append(d);
            sb.Append(p.PositionCost.ToString());
            sb.Append(d);
            sb.Append(p.UnRealizedPL.ToString());
            sb.Append(d);
            sb.Append(p.UnRealizedProfit.ToString());
            sb.Append(d);
            sb.Append(p.Commission);
            sb.Append(d);
            sb.Append(p.TodayPosition);
            sb.Append(d);
            sb.Append(p.YdPosition);
            sb.Append(d);
            sb.Append(p.LastSettlementPrice);
            sb.Append(d);
            sb.Append(p.SettlementPrice);
            sb.Append(d);
            sb.Append(p.CloseProfitByDate);
            sb.Append(d);
            sb.Append(p.Tradingday);
            return sb.ToString();

        }


        public static PositionEx Deserialize(string msg)
        {
            PositionEx p = new PositionEx();
            string [] rec =  msg.Split(',');
            p.Account = rec[0];
            p.Symbol = rec[1];
            p.Multiple = int.Parse(rec[2]);
            p.ClosedPL = decimal.Parse(rec[3]);
            p.UnsignedSize = int.Parse(rec[4]);
            p.AvgPrice = decimal.Parse(rec[5]);
            p.Side = bool.Parse(rec[6]);
            p.Size = int.Parse(rec[7]);
            p.OpenAmount = decimal.Parse(rec[8]);
            p.OpenAVGPrice = decimal.Parse(rec[9]);
            p.OpenVolume = int.Parse(rec[10]);
            p.CloseAmount = decimal.Parse(rec[11]);
            p.CloseAVGPrice = decimal.Parse(rec[12]);
            p.CloseVolume = int.Parse(rec[13]);
            p.DirectionType = (QSEnumPositionDirectionType)Enum.Parse(typeof(QSEnumPositionDirectionType),rec[14]);
            p.CloseProfit = decimal.Parse(rec[15]);
            p.PositionCost = decimal.Parse(rec[16]);
            p.UnRealizedPL = decimal.Parse(rec[17]);
            p.UnRealizedProfit = decimal.Parse(rec[18]);
            p.Commission = decimal.Parse(rec[19]);
            p.TodayPosition = int.Parse(rec[20]);
            p.YdPosition = int.Parse(rec[21]);
            p.LastSettlementPrice = decimal.Parse(rec[22]);
            p.SettlementPrice = decimal.Parse(rec[23]);
            p.CloseProfitByDate = decimal.Parse(rec[24]);
            p.Tradingday = int.Parse(rec[25]);
            return p;
        }
    }
}
