using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 用于将内部的Account,Race,FinService等对象生成通讯Info对象
    /// 1.Account 账户对象包含了逻辑操作函数
    /// 2.AccountInfo 账户信息对象用于向客户端或管理传送信息 用于显示
    /// </summary>
    public class ObjectInfoHelper
    {

        /// <summary>
        /// 将Account对象生成对应的信息传递对象
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static IAccountInfo GenAccountInfo(IAccount acc)
        {
            AccountInfo a = new AccountInfo();
            a.LastEquity = acc.LastEquity;
            a.NowEquity = acc.NowEquity;
            //a.Cash = 0;// acc.Cash;
            a.RealizedPL = acc.RealizedPL;
            a.UnRealizedPL = acc.UnRealizedPL;

            //a.Margin = 0;// acc.Margin;
            //a.ForzenMargin = 0;// acc.ForzenMargin;
            a.Commission = acc.Commission;
            a.CashIn = acc.CashIn;
            a.CashOut = acc.CashOut;
            a.Profit = acc.Profit;
            //a.BuyMultiplier = 0;// acc.BuyMultiplier;
            //a.BuyPower = 0;// acc.BuyPower;
            a.MoneyUsed = acc.MoneyUsed;

            a.Category = acc.Category;
            a.OrderRouteType = acc.OrderRouteType;
            a.Execute = acc.Execute;
            a.IntraDay = acc.IntraDay;
            //a.ObverseProfit = acc.ObverseProfit;
            a.Account = acc.ID;
            //a.AgentCode = acc.AgentCode;
            //a.AgentSubToken = acc.AgentSubToken;

            a.FutMarginUsed = acc.FutMarginUsed;
            a.FutMarginFrozen = acc.FutMarginFrozen;
            a.FutRealizedPL = acc.FutRealizedPL;
            a.FutUnRealizedPL = acc.FutUnRealizedPL;
            a.FutCommission = acc.FutCommission;

            a.OptPositionCost = acc.OptPositionCost;
            a.OptPositionValue = acc.OptPositionValue;
            a.OptRealizedPL = acc.OptRealizedPL;
            a.OptCommission = acc.OptCommission;

            a.FutCash = acc.FutCash;
            a.FutLiquidation = acc.FutLiquidation;
            a.FutMoneyUsed = acc.FutMoneyUsed;


            a.OptCash = acc.OptCash;
            a.OptMarketValue = acc.OptMarketValue;
            a.OptLiquidation = acc.OptLiquidation;
            a.OptMoneyUsed = acc.OptMoneyUsed;


            a.TotalLiquidation = acc.TotalLiquidation;
            a.AvabileFunds = acc.AvabileFunds;
            a.FutAvabileFunds = acc.FutAvabileFunds;
            a.OptAvabileFunds = acc.OptAvabileFunds;

            a.InnovPositionCost = acc.InnovPositionCost;
            a.InnovPositionValue = acc.InnovPositionValue;
            a.InnovCommission = acc.InnovCommission;
            a.InnovRealizedPL = acc.InnovRealizedPL;
            a.InnovMargin = acc.InnovMargin;
            a.InnovCash = acc.InnovCash;
            a.InnovMarketValue = acc.InnovMarketValue;
            a.InnovLiquidation = acc.InnovLiquidation;
            a.InnovMoneyUsed = acc.InnovMoneyUsed;
            a.InnovAvabileFunds = acc.InnovAvabileFunds;

            a.Margin = acc.Margin;
            a.MarginFrozen = acc.MarginFrozen;

            return a;
        }

        /// <summary>
        /// 生成账户的实时盈亏数据
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static IAccountInfoLite GenAccountInfoLite(IAccount acc)
        {
            AccountInfoLite info = new AccountInfoLite();
            info.Account = acc.ID;
            info.NowEquity = acc.NowEquity;
            info.Margin = acc.Margin;
            info.ForzenMargin = acc.MarginFrozen;// acc.ForzenMargin;
            info.BuyPower = acc.AvabileFunds;
            info.RealizedPL = acc.RealizedPL;
            info.UnRealizedPL = acc.UnRealizedPL;
            info.Commission = acc.Commission;
            info.Profit = acc.Profit;

            return info;
        }

        public static IAccountLite GenAccountLite(IAccount acc)
        {
            AccountLite info = new AccountLite();
            info.Account = acc.ID;
            info.CashIn = acc.CashIn;
            info.CashOut = acc.CashOut;
            info.Category = acc.Category;
            info.Commission = acc.Commission;
            info.Execute = acc.Execute;
            info.IntraDay = acc.IntraDay;
            info.LastEquity = acc.LastEquity;
            info.MoneyUsed = acc.MoneyUsed;
            info.NowEquity = acc.NowEquity;
            info.OrderRouteType = acc.OrderRouteType;
            info.Profit = acc.Profit;
            info.RealizedPL = acc.RealizedPL;
            info.UnRealizedPL = acc.UnRealizedPL;
            info.Token = acc.Token;
            return info;
        }
    }
}
