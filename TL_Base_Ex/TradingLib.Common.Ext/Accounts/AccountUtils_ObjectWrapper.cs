using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AccountUtils_ObjectWrapper
    {

        /// <summary>
        /// 生成财务信息
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static AccountInfo ToAccountInfo(this IAccount acc)
        {
            AccountInfo a = new AccountInfo();
            a.LastEquity = acc.LastEquity;
            a.NowEquity = acc.NowEquity;
            a.RealizedPL = acc.RealizedPL;
            a.UnRealizedPL = acc.UnRealizedPL;
            a.Commission = acc.Commission;
            a.CashIn = acc.CashIn;
            a.CashOut = acc.CashOut;
            a.Profit = acc.Profit;
            a.MoneyUsed = acc.MoneyUsed;
            a.Category = acc.Category;
            a.OrderRouteType = acc.OrderRouteType;
            a.Execute = acc.Execute;
            a.IntraDay = acc.IntraDay;
            a.Account = acc.ID;

            a.FutMarginUsed = acc.CalFutMargin();
            a.FutMarginFrozen = acc.CalFutMarginFrozen();
            a.FutRealizedPL = acc.CalFutRealizedPL();
            a.FutUnRealizedPL = acc.CalFutUnRealizedPL();
            a.FutCommission = acc.CalFutCommission();

            a.OptPositionCost = acc.CalOptPositionCost();
            a.OptPositionValue = acc.CalOptPositionValue();
            a.OptRealizedPL = acc.CalOptRealizedPL();
            a.OptCommission = acc.CalOptCommission();

            a.FutCash = acc.CalFutCash();
            a.FutLiquidation = acc.CalFutLiquidation();
            a.FutMoneyUsed = acc.CalFutMoneyUsed();


            a.OptCash = acc.CalOptCash();
            a.OptMarketValue = acc.CalOptPositionValue();
            a.OptLiquidation = acc.CalOptLiquidation();
            a.OptMoneyUsed = acc.CalOptMoneyUsed();


            a.TotalLiquidation = acc.TotalLiquidation;
            a.AvabileFunds = acc.AvabileFunds;
            a.FutAvabileFunds = acc.AvabileFunds; ;
            a.OptAvabileFunds = acc.AvabileFunds;

            a.InnovPositionCost = acc.CalInnovPositionCost();
            a.InnovPositionValue = acc.CalInnovPositionValue();
            a.InnovCommission = acc.CalInnovCommission() ;
            a.InnovRealizedPL = acc.CalInnovRealizedPL();
            a.InnovMargin = acc.CalInnovMargin();
            a.InnovCash = acc.CalInnovCash();
            a.InnovMarketValue = acc.CalInnovPositionValue();
            a.InnovLiquidation = acc.CalInnovLiquidation();
            a.InnovMoneyUsed = acc.CalInnovMoneyUsed();
            a.InnovAvabileFunds = acc.AvabileFunds;

            a.Margin = acc.Margin;
            a.MarginFrozen = acc.MarginFrozen;

            return a;
        }

        /// <summary>
        /// 生成当前实时财务统计信息
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static AccountInfoLite ToAccountInfoLite(this IAccount acc)
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

        /// <summary>
        /// 生成客户端使用的交易帐户信息
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static AccountLite ToAccountLite(this IAccount acc)
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
            info.Name = acc.Name;
            info.Broker = acc.Broker;
            info.BankID = acc.BankID;
            info.BankAC = acc.BankAC;
            info.PosLock = acc.PosLock;
            info.MGRID = acc.Mgr_fk;
            return info;
        }

        /// <summary>
        /// 生成结算单
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Settlement ToSettlement(this IAccount account)
        {
            Settlement settle = new SettlementImpl();
            settle.Account = account.ID;
            settle.CashIn = account.CashIn;
            settle.CashOut = account.CashOut;
            settle.Commission = account.Commission;
            settle.Confirmed = false;
            settle.LastEqutiy = account.LastEquity;
            settle.RealizedPL = account.RealizedPL;
            settle.UnRealizedPL = account.SettleUnRealizedPL;
            settle.NowEquity = settle.LastEqutiy + settle.RealizedPL + settle.UnRealizedPL - settle.Commission + settle.CashIn - settle.CashOut;

            //指定交易日期
            settle.SettleDay = Util.ToTLDate();
            settle.SettleTime = Util.ToTLTime();
            return settle;
        }
    }
}
