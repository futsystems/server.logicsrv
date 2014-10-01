using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class AccountInfoLite : IAccountInfoLite
    {
        public string Account { get; set; }
        public decimal NowEquity { get; set; }//当前动态权益
        public decimal Margin { get; set; }//占用保证金
        public decimal ForzenMargin { get; set; }//冻结保证金
        public decimal BuyPower { get; set; }//购买能力
        public decimal RealizedPL { get; set; }//平仓盈亏
        public decimal UnRealizedPL { get; set; }//浮动盈亏
        public decimal Commission { get; set; }//手续费
        public decimal Profit { get; set; }//净利

        /*
        public static IAccountInfoLite genAccountInfo(IAccount acc)
        {
            AccountInfoLite info = new AccountInfoLite();
            info.Account = acc.ID;
            info.NowEquity = acc.NowEquity;
            info.Margin = acc.Margin;
            info.ForzenMargin = acc.ForzenMargin;
            info.BuyPower = acc.BuyPower;
            info.RealizedPL = acc.RealizedPL;
            info.UnRealizedPL = acc.UnRealizedPL;
            info.Commission = acc.Commission;
            info.Profit = acc.Profit;

            return info;
        }
        public static string Serialize(IAccount account)
        {
            return Serialize(genAccountInfo(account));
        }**/

        public static string Serialize(IAccountInfoLite info)
        {
            const char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(info.NowEquity);
            sb.Append(d);
            sb.Append(info.Margin);
            sb.Append(d);
            sb.Append(info.ForzenMargin);
            sb.Append(d);
            sb.Append(info.BuyPower);
            sb.Append(d);
            sb.Append(info.RealizedPL);
            sb.Append(d);
            sb.Append(info.UnRealizedPL);
            sb.Append(d);
            sb.Append(info.Commission);
            sb.Append(d);
            sb.Append(info.Profit);
            sb.Append(d);
            sb.Append(info.Account);

            return sb.ToString();

        }

        public static IAccountInfoLite Deserialize(string msg)
        {
            string[] r = msg.Split(',');
            AccountInfoLite a = new AccountInfoLite();
            if (r.Length >= 9)
            {
                a.NowEquity = Decimal.Parse(r[0]);
                a.Margin = Decimal.Parse(r[1]);
                a.ForzenMargin = Decimal.Parse(r[2]);
                a.BuyPower = Decimal.Parse(r[3]);
                a.RealizedPL = Decimal.Parse(r[4]);
                a.UnRealizedPL = Decimal.Parse(r[5]);
                a.Commission = Decimal.Parse(r[6]);
                a.Profit = Decimal.Parse(r[7]);
                a.Account = r[8];
            }
            return a;
        }
    }
}
