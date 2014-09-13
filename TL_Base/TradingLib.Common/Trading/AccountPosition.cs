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
    public  class AccountPosition
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
        /// 持仓成本
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

        public AccountPosition()
        {
            Account = string.Empty;
            Symbol = string.Empty;
            Multiple = 1;
            ClosedPL = 0;
            UnsignedSize = 0;
            AvgPrice = 0;
            Side = true;
            Size = 0;

        }
        public static AccountPosition Deserialize(string msg)
        {
            AccountPosition p = new AccountPosition();
            string [] rec =  msg.Split(',');
            p.Account = rec[0];
            p.Symbol = rec[1];
            p.Multiple = int.Parse(rec[2]);
            p.ClosedPL = decimal.Parse(rec[3]);
            p.UnsignedSize = int.Parse(rec[4]);
            p.AvgPrice = decimal.Parse(rec[5]);
            p.Side = bool.Parse(rec[6]);
            p.Size = int.Parse(rec[7]);
            return p;
        }

        public static string Serialize(AccountPosition p)
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
            return sb.ToString();

        }

        public static AccountPosition GenFromPosition(Position pos)
        {
            AccountPosition p = new AccountPosition();
            p.Account = pos.Account;
            p.Symbol = pos.Symbol;
            p.Multiple = pos.oSymbol.Multiple;
            p.UnsignedSize = pos.UnsignedSize;
            p.AvgPrice = pos.AvgPrice;
            p.Side = pos.isLong;
            p.ClosedPL = pos.ClosedPL;
            p.Size = pos.Size;
            return p;
        }
    }
}
